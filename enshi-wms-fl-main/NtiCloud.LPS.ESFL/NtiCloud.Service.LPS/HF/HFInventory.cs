using Microsoft.AspNetCore.Mvc.Formatters;
using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

namespace NtiCloud.Service.WMS.HF
{
    public class HFInventory
    {

        /// <summary>
        /// 出库数据同步ERP
        /// </summary>
        public bool SyncData(  List<NtiCloud.Model.WMS.SysErpIn> lstOrderno, bool isDelete)
        { 
            if (lstOrderno.Count == 0)
            { 
                throw new Exception("没有需要同步的单据!");
            }
            foreach (NtiCloud.Model.WMS.SysErpIn key in lstOrderno)
            {
                string msg = Sync(  key.order_no, key.service_type, isDelete);
                if (msg != "")
                {
                   throw new Exception("出库数同步失败:"+msg);
                }
            }
            return true;
        }

        public string Sync(string code, string type, bool isDelete = false)
        { 
            DataSet ds = GetData(code, type);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return code + ":" + "没有找到需要同步的数据";
            }
            if (ds.Tables[0].Columns.Contains("ReturnMessage"))
            {
                return code + ":" + ds.Tables[0].Rows[0]["ReturnMessage"].ToString();
            }
            string billType = GetBillType(type, ds.Tables[0].Rows[0]);
            string shortname = code + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff");
            string filename = AppDomain.CurrentDomain.BaseDirectory + "Message\\" + shortname;
            string xml = BuildXML(ds, billType, isDelete ? "delete" : "add", shortname);
            try
            {
                DataTable result = Send(xml, filename);
                return DealResult( result, type, code, isDelete);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(code + ":" + ex.Message);
                return code + ":" + ex.Message;
            }

        }
        /// <summary>
        /// 根据业务类型和结算类型调用ERP的单据类型
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="settlementType"></param>
        /// <returns></returns>
        private string GetBillType(string serviceType, DataRow row)
        {

            #region 入库

            //新烟入库,外加工烟入库
            if (serviceType == "ReceivingBusiness_New" || serviceType == "ReceivingBusiness_Process")
            {
                if (row["ERPSERVICETYPE"].ToString() == "MT422")
                    return "MT42";

                return row["ERPSERVICETYPE"].ToString();
            }

            //厂际和厂内移库入库
            if (serviceType == "ReceivingBusiness_Transport"
                 || serviceType == "ReceivingBusiness_Inturn"
                 || serviceType == "ReceivingBusiness_MES"
                 || serviceType == "ReceivingBusiness_Return"
                 || serviceType == "ReceivingBusiness_Chasing"
                )
            {
                return "MX1C";
            }

            //委外加工入库
            if (serviceType == "ReceivingBusiness_OutsourceIn")
            {
                if (row["ERPSERVICETYPE"].ToString() == "MT26")
                    return "MT26";
                return "MO62";
            }

            //委内加工入库
            if (serviceType == "ReceivingBusiness_InsourceIn")
            {
                return "MX17";
            }

            #endregion

            #region 出库和损溢

            //厂际和厂内移库出库
            if (serviceType == "MESStock_Service"
                 || serviceType == "Inturn_Service"
                 || serviceType == "Chasing_Service"
                 || serviceType == "Return_Service"
                 || serviceType == "Transfer_Service"
                )
            {
                return "MX53";
            }

            //委外加工入库
            if (serviceType == "Outsource_Service")
            {
                return "MX2G";
            }

            //委内,生产领用,非生产领用
            if (serviceType == "Insource_Service"
                || serviceType == "Productive_Service"
                || serviceType == "Unproductive_Service"
               )
            {
                return "MX22";
            }

            //损益，盘亏，盘盈
            if (serviceType == "LossAdjustment")
                return row["ERPSERVICETYPE"].ToString();

            //销售
            if (serviceType == "Sales_Service")
            {
                return "MU22";
            }

            #endregion

            //等级调整
            if (serviceType == "GradeAdjustment")
            {
                return "MX54";
            }

            //数量调整
            if (serviceType == "QuantityAdjustment")
            {
                return "MX5C";
            }

            return "";
        }
        /// <summary>
        /// 构建XML报文
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="billType"></param>
        /// <param name="proc"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private  string BuildXML(DataSet ds, string billType, string proc, string fileName)
        {

            string Sender = "jtgj";
            string Receiver = "0001";
            string Account = "ahzy01";

            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\"  encoding=\"UTF-8\"?> ");
            sb.Append("<ufinterface account=\"" + Account + "\" filename=\"" + fileName + "\" billtype=\"" + billType + "\" proc=\"" + proc + "\" receiver=\"" + Receiver + "\" sender=\"" + Sender + "\">");

            sb.Append("<bill id=\"" + ds.Tables[0].Rows[0]["pk"].ToString() + "\">");

            sb.Append("<billhead>");

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                sb.AppendLine("<" + ds.Tables[0].Columns[i].ColumnName.ToLower() + ">" + ds.Tables[0].Rows[0][i] + "</" + ds.Tables[0].Columns[i].ColumnName.ToLower() + ">");
            }

            sb.Append("</billhead>");

            sb.Append("<billbody>");
            for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
            {
                sb.Append("<entry>");
                for (int i = 0; i < ds.Tables[1].Columns.Count; i++)
                {
                    sb.AppendLine("<" + ds.Tables[1].Columns[i].ColumnName.ToLower() + ">" + ds.Tables[1].Rows[j][i] + "</" + ds.Tables[1].Columns[i].ColumnName.ToLower() + ">");
                }
                sb.Append("</entry>");
            }
            sb.Append("</billbody>");

            sb.Append("</bill>");
            sb.Append("</ufinterface>");

            return sb.ToString();
        }

        /// <summary>
        /// 模拟发送
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private DataTable Send(string xml, string filename)
        {
            WriteXMLByFileName(xml, filename);
            //WMSReferenceService.ERPService.AHGY_IMPORT_SERVICEClient client = new WMSReferenceService.ERPService.AHGY_IMPORT_SERVICEClient();
            string xml2 = "";//; client.doImportForEas(xml);
            WriteXMLByFileName(xml2, filename + "_接收");

            return XmlToDataTableByString(xml2);
        }

        /// <summary>
        /// 记录过来的XML文件
        /// </summary>
        /// <param name="xml">XML</param>
        /// <param name="filename">文件名称</param>
        private  void WriteXMLByFileName(string xml, string filename)
        {
            try
            {
                File.WriteAllText(filename, xml);
            }
            catch { }
        }


        /// <summary>
        ///  Xml结构的字符中读到DataTable中
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public DataTable XmlToDataTableByString(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            //WriteTxt(xml);
            DataTable dt = new DataTable("result");
            //以第一个元素的子元素建立表结构
            XmlNode songNode = doc.SelectSingleNode("/result");
            string colName;
            if (songNode != null)
            {
                for (int i = 0; i < songNode.ChildNodes.Count; i++)
                {
                    colName = songNode.ChildNodes.Item(i).Name;
                    dt.Columns.Add(colName);
                }
            }
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            //获取Xml字串
            string xmlString = doc.InnerXml;
            StringReader sr = new StringReader(xmlString);
            XmlTextReader xr = new XmlTextReader(sr);
            ds.ReadXml(xr);

            return ds.Tables[0];

        }


        private string DealResult(DataTable dt, string type, string code, bool isDelete)
        {

            string errormsg = "";
            //根据返回结果更新状态
            foreach (DataRow item in dt.Rows)
            {
                string billid = item["billid"].ToString();
                string flag = item["returnvalue"].ToString();
                string msg = item["message"].ToString();
                bool isSuccess = flag == "1";
                msg = msg.Length > 200 ? msg.Substring(0, 200) : msg;
                if (!isSuccess)
                {
                    errormsg += " " + code + ":" + msg;
                }

                SyncOrderModel model = GetModel(code, type);
                bool isnew = false;
                if (model == null)
                {
                    isnew = true;
                    model = new SyncOrderModel();
                    model.ID = SnowflakeID.Guid().ToString();
                }
                model.OrderNO = code;
                model.ServiceType = type;
                model.UpdateDate = DateTime.Now;
                model.UpdateCount++;
                model.UpdateUser = Context.UserInfo.UserId;
                model.ReturnMessage = isSuccess ? "" : msg;
                if (!isDelete)
                {
                    if (model.Status == 0)
                    {
                        model.Status = isSuccess ? 1 : 0;
                    }
                }
                else
                {
                    model.Status = isSuccess ? 0 : 1;
                }

                if (isnew)
                {
                    if (!isDelete)
                    {
                        AddSyncOrder(model);
                    }
                }
                else
                {
                    if (isDelete && isSuccess)
                    {
                        DeleteSyncOrder(model.ID);
                    }
                    else
                    {
                        UpdateSyncOrder(model);
                    }
                }

            }

            return errormsg;

        }
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool AddSyncOrder(SyncOrderModel model)
        {
            string sql= @"insert into SYNCORDER@hfchk_link(
             ID,ORDERNO,SERVICETYPE,STATUS,UPDATEUSER,UPDATEDATE,UPDATECOUNT,RETURNMESSAGE)
              values (
             @ID,@ORDERNO,@SERVICETYPE,@STATUS,@UPDATEUSER,to_date(@UPDATEDATE,'yyyy-MM-dd HH24:mi:ss'),@UPDATECOUNT,@RETURNMESSAGE)";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("ID", model.ID);
            lstParams.Add("ORDERNO", model.OrderNO);
            lstParams.Add("SERVICETYPE", model.ServiceType);
            lstParams.Add("STATUS", model.Status.ToString());
            lstParams.Add("UPDATEUSER", model.UpdateUser);
            lstParams.Add("UPDATEDATE",DBHelper.GetDateStr( model.UpdateDate));
            lstParams.Add("UPDATECOUNT", model.UpdateCount.ToString());
            lstParams.Add("RETURNMESSAGE", model.ReturnMessage);

            int rows = DBHelper.RunSql(sql, lstParams);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool DeleteSyncOrder(string id)
        {

            string sql = @"delete from SYNCORDER@hfchk_link 
                            where ID= @ID";
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("ID", id);
            int rows = DBHelper.RunSql(sql,lstParams);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool UpdateSyncOrder(SyncOrderModel model)
        {
            string sql = @"update SYNCORDER set 
                                ORDERNO = @ORDERNO, 
                                SERVICETYPE = @SERVICETYPE, 
                                STATUS = @STATUS, 
                                UPDATEUSER = @UPDATEUSER, 
                                UPDATEDATE = @UPDATEDATE,  
                                UPDATECOUNT = @UPDATECOUNT, 
                                RETURNMESSAGE = @RETURNMESSAGE 
                            where ID= @ID" ;
            Dictionary<string, object> lstParams = new Dictionary<string, object>();
            lstParams.Add("ID", model.ID);
            lstParams.Add("ORDERNO", model.OrderNO);
            lstParams.Add("SERVICETYPE", model.ServiceType);
            lstParams.Add("STATUS", model.Status.ToString());
            lstParams.Add("UPDATEUSER", model.UpdateUser);
            lstParams.Add("UPDATEDATE", DBHelper.GetDateStr(model.UpdateDate));
            lstParams.Add("UPDATECOUNT", model.UpdateCount.ToString());
            lstParams.Add("RETURNMESSAGE", model.ReturnMessage);
             

            int rows = DBHelper.RunSql(sql,lstParams);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public SyncOrderModel GetModel(string orderno, string servicetype)
        {
            string sql = @"select ID from SYNCORDER@hfchk_link 
                        where servicetype= @servicetype and orderno= @orderno";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("servicetype", servicetype);
            lstParam.Add("orderno", orderno);
            string id = DBHelper.SelectOne(sql, lstParam);
            if (id == "")
                return null;

            return GetModel(id);

        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public SyncOrderModel GetModel(string id)
        {

            string sql=@"select ID,ORDERNO,SERVICETYPE,STATUS,UPDATEUSER,UPDATEDATE,UPDATECOUNT,RETURNMESSAGE 
                    from SYNCORDER@hfchk_link
                    where ID= @ID ";
            Dictionary<string, object> lstParam = new Dictionary<string, object>();
            lstParam.Add("ID", id);
            DataTable dt = DBHelper.Select(sql, lstParam, "", false);
             
            if (dt.Rows.Count > 0)
            {
                return DBHelper.ConvertTableToObject<SyncOrderModel>(dt)[0];
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 获取同步数据
        /// </summary>
        /// <param name="code">出库单号</param>
        /// <param name="type">业务类型</param>
        /// <returns></returns>
        public DataSet  GetData(string code, string type)
        {
            string spname= "root.hf_sp_syncerp";
            List<Tuple<string, string, ParameterDirection>> spParams = new List<Tuple<string, string, ParameterDirection>>();
            spParams.Add(new Tuple<string, string, ParameterDirection>("i_code", code, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("i_type", type, ParameterDirection.Input));
            spParams.Add(new Tuple<string, string, ParameterDirection>("v_cur", "", ParameterDirection.Output));
            spParams.Add(new Tuple<string, string, ParameterDirection>("v_curDetail", "", ParameterDirection.Output));
           
            DataSet ds = DBHelper.RunProcDataSet(spname, spParams);
          //  var outputs = DBHelper(spname, spParams, "", DBHelper.OperatedNumType.ZeroOrMore);
           
            return ds;

        }
    }



    //单据同步到ERP
    public class SyncOrderModel
    {

        #region Model
        private string _id;
        private string _orderno;
        private string _servicetype;
        private decimal _status;
        private string _updateuser;
        private DateTime _updatedate;
        private decimal _updatecount;
        private string _returnmessage;
        /// <summary>
        /// ID#主键
        /// </summary>
        public string ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 单据号
        /// </summary>
        public string OrderNO
        {
            set { _orderno = value; }
            get { return _orderno; }
        }
        /// <summary>
        /// 业务类型
        /// </summary>
        public string ServiceType
        {
            set { _servicetype = value; }
            get { return _servicetype; }
        }
        /// <summary>
        /// 同步状态
        /// </summary>
        public decimal Status
        {
            set { _status = value; }
            get { return _status; }
        }
        /// <summary>
        /// 同步人
        /// </summary>
        public string UpdateUser
        {
            set { _updateuser = value; }
            get { return _updateuser; }
        }
        /// <summary>
        /// 同步时间
        /// </summary>
        public DateTime UpdateDate
        {
            set { _updatedate = value; }
            get { return _updatedate; }
        }
        /// <summary>
        /// 同步次数
        /// </summary>
        public decimal UpdateCount
        {
            set { _updatecount = value; }
            get { return _updatecount; }
        }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string ReturnMessage
        {
            set { _returnmessage = value; }
            get { return _returnmessage; }
        }
        #endregion Model

    }
}
