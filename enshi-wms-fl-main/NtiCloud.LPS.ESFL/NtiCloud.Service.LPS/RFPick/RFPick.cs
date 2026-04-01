using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NtiCloud.Interface.Core;
using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Service.WMS.Tasking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.Json;
using NtiCloud.Service.WMS.Inventory;
using IContainer = NtiCloud.Service.WMS.Common.IContainer;
using Container = NtiCloud.Service.WMS.Inventory.Container;
using NtiCloud.Service.WMS.Interface.WCS;

namespace NtiCloud.Service.WMS.RFPick
{
    public class RFPick : WmsObject, IRFPick
    {
        private readonly IWorkTask _workTask;
        private readonly IWorkTaskItem _workTaskItem;
        private readonly IContainer _container;

        public RFPick(IWorkTask workTask, IWorkTaskItem workTaskItem, IContainer container)
        {
            _workTask = workTask;
            _workTaskItem = workTaskItem;
            _container = container;
        }

        public RFPick()
        {
            _workTask = new WorkTask();
            _workTaskItem = new WorkTaskItem();
            _container = new Container();
        }

        // RF拣选-立库

        #region 外部-拣选-立库，扫托盘码绑定单据下拉框 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，扫托盘码绑定单据下拉框 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的托盘号-ContainerCode</param>
        /// <returns></returns>
        public WebJsonInfo LK_ScanContainerCode(string ContainerCode)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                string orderID = "";
                string selectOne = "";
                List<string> RefDocNoList = new List<string>();

                string sql = "select count(*) from &DataSource&wms_b_container_h h where h.container_barcode=@ContainerCode and h.act_status='1'";
                param.Clear();
                param.Add("ContainerCode", ContainerCode);
                selectOne = DBHelper.SelectOne(sql, param, "查询托盘是否存在");
                if (Convert.ToInt32(selectOne) < 1)
                {
                    throw new WmsException(500, "该托盘不存在于容器主档表");
                }

                //查询该托盘对应的任务明细表数据
                sql = @"select i.ref_doc_id,i.ref_doc_type,i.ref_doc_d_id from &DataSource&wms_b_task t inner join &DataSource&wms_b_task_item i 
                           on t.t_id = i.task_id where t.TASK_Type='TaskType_Picking' and i.PROCESS_STATUS = 'ProcessStatus_New' and i.ref_doc_type in 
                           ('DocType_Wave','DocType_Order')  and  t.container_barcode = @ContainerCode";

                param.Clear();
                param.Add("ContainerCode", ContainerCode);
                DataTable dtRefDocNo = DBHelper.Select(sql, param, "查询该托盘对应的任务明细表数据", false);

                if (dtRefDocNo.Rows.Count < 1)
                {
                    throw new WmsException(500, "该托盘不存在需要拣选的物料");
                }

                //根据任务明细表数据查询拣选单号
                foreach (DataRow r in dtRefDocNo.Rows)
                {
                    //DocType_Order 出库订单， DocType_Wave 波次
                    string ref_doc_type = r["ref_doc_type"].ToString(); //来源类型
                    string ref_doc_id = r["ref_doc_id"].ToString();    //来源单ID
                    string ref_doc_d_id = r["ref_doc_d_id"].ToString(); //来源单明细ID
                    if (ref_doc_type == "DocType_Order") //无波次出库订单，来源单据明细ID为出库单明细ID
                    {
                        sql = @"select distinct doc_no from &DataSource&wms_d_outbound_h h where h.t_id=@ref_doc_id";

                        param.Clear();
                        param.Add("ref_doc_id", ref_doc_id);
                        DataTable dtDocNo = DBHelper.Select(sql, param, "查询该托盘对应的所有单据", false);

                        foreach (DataRow docNoRow in dtDocNo.Rows)
                        {
                            string docNo = docNoRow["doc_no"].ToString();

                            sql = @" select t_id from &DataSource&wms_d_outbound_h w where w.doc_no = @docNo";

                            param.Clear();
                            param.Add("docNo", docNo);
                            orderID = DBHelper.SelectOne(sql, param, "查询该托盘对应的所有单据", false);

                            sql = @" select count(1) from &DataSource&wms_b_wave_d w where w.outbound_h_id = @orderID";
                            param.Clear();
                            param.Add("orderID", orderID);
                            selectOne = DBHelper.SelectOne(sql, param, "查询出库单是否存在波次", false);

                            if (Convert.ToInt32(selectOne) > 0)
                            {
                                throw new WmsException(500, "该任务为无波次出库拣选明细任务，但却存在单据的波次信息");
                            }

                            if (!RefDocNoList.Contains(docNo))
                            {
                                RefDocNoList.Add(docNo);
                            }
                        }
                    }
                    else if (ref_doc_type == "DocType_Wave") //有波次出库订单，来源单据明细ID为波次号明细ID
                    {
                        sql = @"select distinct doc_no from &DataSource&wms_b_wave_d d inner join &DataSource&wms_b_task_item i 
                                on d.t_id = i.ref_doc_d_id inner join &DataSource&wms_d_outbound_h o on o.t_id = d.OUTBOUND_H_ID
                                where d.t_id = @ref_doc_d_id ";

                        param.Clear();
                        param.Add("ref_doc_d_id", ref_doc_d_id);
                        DataTable dtDocNo = DBHelper.Select(sql, param, "查询该托盘对应的波次的所有单据", false);

                        foreach (DataRow docNoRow in dtDocNo.Rows)
                        {
                            string docNo = docNoRow["doc_no"].ToString();

                            sql = @" select t_id from &DataSource&wms_d_outbound_h w where w.doc_no = @docNo";
                            param.Clear();
                            param.Add("docNo", docNo);
                            orderID = DBHelper.SelectOne(sql, param, "查询出库单号对应的ID", false);

                            sql = @" select count(1) from &DataSource&wms_b_wave_d w where w.outbound_h_id = @orderID";
                            param.Clear();
                            param.Add("orderID", orderID);
                            selectOne = DBHelper.SelectOne(sql, param, "查询出库单是否存在波次", false);

                            if (Convert.ToInt32(selectOne) < 1)
                            {
                                throw new WmsException(500, "该任务为有波次出库拣选明细任务，但却不存在单据的波次信息");
                            }


                            if (!RefDocNoList.Contains(docNo))
                            {
                                RefDocNoList.Add(docNo);
                            }
                        }
                    }
                }

                //去除重复的单据
                RefDocNoList = RefDocNoList.Distinct().OrderBy(o => o).ToList();


                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = RefDocNoList };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 外部-拣选-立库，选定单据下拉框后，获取需要拣选的单行明细数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，选定单据下拉框后，获取需要拣选的单行明细数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的托盘号-ContainerCode、用户选择的单据-Order、页码-Page（默认为1）</param>
        /// <returns></returns>
        public WebJsonInfo LK_ContainerPickInfo(string ContainerCode, string Order, int Page)
        {
            try
            {
                string sql = @"";
                string selectOne = "";
                string orderID = ""; //出库主档ID
                Dictionary<string, object> param = new Dictionary<string, object>();

                sql = @" select count(1) from &DataSource&wms_d_outbound_h w where w.doc_no = '" + Order + "'";
                param.Clear();
                param.Add("orderID", orderID);
                selectOne = DBHelper.SelectOne(sql, param, "查询出库单是否存在", false);

                if (Convert.ToInt32(selectOne) == 0)
                {
                    throw new WmsException(500, "单据编号不存在");
                }
                if (Convert.ToInt32(selectOne) > 1)
                {
                    throw new WmsException(500, "单据编号有多个");
                }

                sql = @" select t_id from &DataSource&wms_d_outbound_h w where w.doc_no = @OutOrder";
                param.Clear();
                param.Add("OutOrder", Order);
                orderID = DBHelper.SelectOne(sql, param, "查询出库单号对应的ID", false);

                sql = @" select count(1) from &DataSource&wms_b_wave_d w where w.outbound_h_id = @orderID";
                param.Clear();
                param.Add("orderID", orderID);
                selectOne = DBHelper.SelectOne(sql, param, "查询出库单是否存在波次", false);

                if (Convert.ToInt32(selectOne) < 1) //无波次出库订单
                {
                    throw new WmsException(500, "未查询到波次单");
                }
                else //有波次出库订单
                {
                    sql = @"select h.ref_doc_no 来源单号,t.item_code 物料编号,t.item_name 物料名称,d.lot_no 批次,(select sum(td.qty) from &DataSource&wms_b_container_d td where td.container_id = d.container_id and td.item_id = d.item_id) 物料数量,
                        (ifnull(i.plan_qty, 0) - ifnull(i.processed_qty, 0)) 应拣,ifnull(i.processed_qty, 0) 已拣,i.t_id  from &DataSource&wms_b_task_item i inner join &DataSource&wms_b_wave_d w on i.ref_doc_d_id = w.t_id
                        inner join &DataSource&wms_d_outbound_h h
                        on w.outbound_h_id = h.t_id
                        inner join &DataSource&wms_m_item t on t.t_id = i.item_id
                        inner join &DataSource&wms_b_container_d d on i.container_d_id = d.t_id
                        inner join &DataSource&wms_b_container_h c on c.t_id = d.container_id 
                        inner join &DataSource&wms_b_task ts on ts.t_id=i.TASK_ID
                        where ts.TASK_Type='TaskType_Picking' and i.PROCESS_STATUS = 'ProcessStatus_New' and c.container_barcode = @ContainerCode and h.doc_no = @OutOrder and rownum =@Page ";

                    param.Clear();
                    param.Add("ContainerCode", ContainerCode);
                    param.Add("OutOrder", Order);
                    param.Add("Page", Page.ToString());
                }

                DataTable dtRefDocNo = DBHelper.Select(sql, param, "查询RF所需的单据数据", false);

                if (dtRefDocNo.Rows.Count < 1)
                {
                    return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = null };
                }
                else
                {
                    DataRow dr = dtRefDocNo.Rows[0];
                    var ResObj = new
                    {
                        来源单号 = dr["来源单号"] == null ? "" : dr["来源单号"].ToString(), //来源单号
                        物料编号 = dr["物料编号"] == null ? "" : dr["物料编号"].ToString(), //物料编号
                        物料名称 = dr["物料名称"] == null ? "" : dr["物料名称"].ToString(), //物料名称
                        批次 = dr["批次"] == null ? "" : dr["批次"].ToString(),     //批次
                        物料数量 = dr["物料数量"] == null ? "" : dr["物料数量"].ToString(), //物料数量
                        已拣 = dr["已拣"] == null ? "" : dr["已拣"].ToString(),     //已拣
                        应拣 = dr["应拣"] == null ? "" : dr["应拣"].ToString(),     //应拣
                        TaskDetailID = dr["t_id"] == null ? "" : dr["t_id"].ToString(),      //任务明细ID
                        CurPage = Page //当前页码
                    };
                    return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = ResObj };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 外部-拣选-立库，提交拣选数据 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，提交拣选数据 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">model必填值：
        /// TaskDetailID 任务明细ID
        /// PickType     拣选模式  (1为默认，2为交换托盘)
        /// OrderContainerCode  空托盘条码
        /// PickNum      拣选数量
        /// NeedPick     本次应拣数量
        /// BNList       箱码(有箱码则填，无箱码不填)
        /// </param>
        /// <returns></returns>
        public WebJsonInfo LK_PickSubmit(string TaskDetailID, string PickType, string OrderContainerCode, int readyPickNum, int PickNum, int NeedPick, IList<string> BNList)
        {
            try
            {
                int o_result = 0;
                string o_msg = "";

                PickCommitForLK(TaskDetailID, PickType, OrderContainerCode, readyPickNum, PickNum, NeedPick, BNList, out o_msg, out o_result);
                if (o_result == 1)
                {
                    return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = o_msg, json = null };
                }
                else
                {
                    throw new WmsException(500, o_msg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 私有 拣选-立库，提交拣选数据 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 私有 拣选-立库，提交拣选数据 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">model必填值：
        /// TaskDetailID 任务明细ID
        /// PickType     拣选模式  (1为默认，2为交换托盘)
        /// OrderContainerCode  空托盘条码
        /// PickNum      拣选数量
        /// NeedPick     本次应拣数量
        /// BNList       箱码(有箱码则填，无箱码不填)
        /// </param>
        /// <param name="o_msg">返回描述信息</param>
        /// <param name="o_result">1 成功，0 失败</param>
        private void PickCommitForLK(string TaskDetailID, string PickType, string OrderContainerCode, int readyPickNum, int PickNum, int NeedPick, IList<string> BNList, out string o_msg, out int o_result)
        {
            try
            {
                o_msg = "";
                o_result = 0;
                string selectOne = "";
                int rescount = 0;
                DataTable dt = null;

                string v_ContainerCode;      //托盘条码
                string v_OutboundID;         //出库单ID
                string v_OutboundDetailID;   //出库单明细ID
                string v_WaveID = "";        //波次ID
                string v_WaveDetailID = "";  //波次明细ID
                string v_ConDetailID;        //库存明细ID
                string v_ConID;              //库存主表ID
                string v_TaskID;             //任务主档ID
                string v_TaskNo;             //主任务编号
                int v_ConDetailNum;          //托盘明细的剩余数量
                string v_Ref_doc_type;       //来源单据类型
                string v_ConNewID;           //新库存头表ID
                string v_ConNewDetailID;     //新库存明细表ID
                string v_DocNo;              //出库单号
                string v_Dependent_task_no;  //依赖任务号
                string v_Dependent_TagAddress;//依赖任务的站台号
                string v_LocationID;         //货位ID
                string allowLess;            //是否允许少拣 Yes、No
                string IsAutoGenTask;        //是否自动产生余料回库任务

                /****************************基本数据赋值*******************************/
                Dictionary<string, object> param = new Dictionary<string, object>();



                string sql = @"select count(1) from &DataSource&wms_b_task_item w where w.t_id =@TaskDetailID";
                param.Clear();
                param.Add("TaskDetailID", TaskDetailID);
                selectOne = DBHelper.SelectOne(sql, param, "查询任务明细是否存在", false);

                if (Convert.ToInt32(selectOne) < 1)
                {
                    o_msg = "拣选任务不存在";
                    o_result = 0;
                    return;
                }
                if (Convert.ToInt32(selectOne) > 1)
                {
                    o_msg = "拣选任务存在多条";
                    o_result = 0;
                    return;
                }

                sql = @"select w.ref_doc_type,w.ref_doc_id, w.ref_doc_d_id, w.container_d_id, w.task_id from &DataSource&wms_b_task_item w where w.t_id = @TaskDetailID";
                param.Clear();
                param.Add("TaskDetailID", TaskDetailID);
                dt = DBHelper.Select(sql, param, "查询任务明细", false);

                if (dt == null || dt.Rows.Count != 1)
                {
                    o_msg = "任务明细存在多条";
                    o_result = 0;
                    return;
                }
                v_Ref_doc_type = dt.Rows[0]["ref_doc_type"] == null ? "" : dt.Rows[0]["ref_doc_type"].ToString();

                if (v_Ref_doc_type == "DocType_Wave")  //波次出库(有波次)
                {
                    v_WaveID = dt.Rows[0]["ref_doc_id"] == null ? "" : dt.Rows[0]["ref_doc_id"].ToString();
                    v_WaveDetailID = dt.Rows[0]["ref_doc_d_id"] == null ? "" : dt.Rows[0]["ref_doc_d_id"].ToString();
                    v_ConDetailID = dt.Rows[0]["container_d_id"] == null ? "" : dt.Rows[0]["container_d_id"].ToString();
                    v_TaskID = dt.Rows[0]["task_id"] == null ? "" : dt.Rows[0]["task_id"].ToString();

                    sql = @"select count(*) from &DataSource&wms_b_task t where t.t_id=@TaskID";
                    param.Clear();
                    param.Add("TaskID", v_TaskID);
                    selectOne = DBHelper.SelectOne(sql, param, "查询主任务是否存在", false);
                    if (Convert.ToInt32(selectOne) < 1)
                    {
                        o_msg = "主任务不存在";
                        o_result = 0;
                        return;
                    }

                    sql = @"select dependent_task_no from &DataSource&wms_b_task t where t.t_id=@TaskID";
                    param.Clear();
                    param.Add("TaskID", v_TaskID);
                    v_Dependent_task_no = DBHelper.SelectOne(sql, param, "查询主任务是否存在", false);
                    if (string.IsNullOrEmpty(v_Dependent_task_no))
                    {
                        o_msg = "依赖任务值为空";
                        o_result = 0;
                        return;
                    }

                    sql = @"select count(*) from &DataSource&wms_b_task_his t where t.Task_No=@ParentTask_No";
                    param.Clear();
                    param.Add("ParentTask_No", v_Dependent_task_no);
                    selectOne = DBHelper.SelectOne(sql, param, "查询依赖任务是否存在历史表", false);
                    if (Convert.ToInt32(selectOne) != 1)
                    {
                        sql = @"select count(*) from &DataSource&wms_b_task t where t.Task_No=@ParentTask_No";
                        param.Clear();
                        param.Add("ParentTask_No", v_Dependent_task_no);
                        selectOne = DBHelper.SelectOne(sql, param, "查询依赖任务是否存在", false);
                        if (Convert.ToInt32(selectOne) == 0)
                        {
                            o_msg = "依赖任务不存在：" + v_Dependent_task_no;
                            o_result = 0;
                            return;
                        }
                        else
                        {
                            o_msg = "请先完成依赖任务：" + v_Dependent_task_no;
                            o_result = 0;
                            return;
                        }
                    }

                    sql = @"select t.src_addr_id,t.DEST_ADDR_ID from &DataSource&wms_b_task_his t where t.Task_No=@ParentTask_No";
                    param.Clear();
                    param.Add("ParentTask_No", v_Dependent_task_no);
                    DataTable oldTask = DBHelper.Select(sql, param, "查询依赖任务的来源货位", false);
                    v_LocationID = oldTask.Rows[0]["src_addr_id"] == null ? "" : oldTask.Rows[0]["src_addr_id"].ToString();
                    v_Dependent_TagAddress = oldTask.Rows[0]["DEST_ADDR_ID"] == null ? "" : oldTask.Rows[0]["DEST_ADDR_ID"].ToString();
                    if (string.IsNullOrEmpty(v_LocationID))
                    {
                        o_msg = "来源任务货位ID不存在";
                        o_result = 0;
                        return;
                    }

                    sql = @"select count(1) from &DataSource&wms_b_wave_h h where h.t_id = @v_WaveID";
                    param.Clear();
                    param.Add("v_WaveID", v_WaveID);
                    selectOne = DBHelper.SelectOne(sql, param, "查询波次是否存在", false);

                    if (Convert.ToInt32(selectOne) < 1)
                    {
                        o_msg = "波次主表数据不存在";
                        o_result = 0;
                        return;
                    }
                    if (Convert.ToInt32(selectOne) > 1)
                    {
                        o_msg = "波次主表数据存在多条";
                        o_result = 0;
                        return;
                    }

                    sql = @"select count(1) from &DataSource&wms_b_wave_d h where h.t_id = @v_WaveDetailID";
                    param.Clear();
                    param.Add("v_WaveDetailID", v_WaveDetailID);
                    selectOne = DBHelper.SelectOne(sql, param, "查询波次明细是否存在", false);

                    if (Convert.ToInt32(selectOne) < 1)
                    {
                        o_msg = "波次明细数据不存在";
                        o_result = 0;
                        return;
                    }

                    sql = @" select h.outbound_h_id, h.outbound_d_id from &DataSource&wms_b_wave_d h where h.t_id = '" + v_WaveDetailID + "'";
                    param.Clear();
                    param.Add("v_WaveDetailID", v_WaveDetailID);
                    dt = DBHelper.Select(sql, param, "查询波次明细信息", false);

                    v_OutboundID = dt.Rows[0]["outbound_h_id"] == null ? "" : dt.Rows[0]["outbound_h_id"].ToString();
                    v_OutboundDetailID = dt.Rows[0]["outbound_d_id"] == null ? "" : dt.Rows[0]["outbound_d_id"].ToString();
                }
                else
                {
                    o_msg = "未知的波次产生方式";
                    o_result = 0;
                    return;
                }

                sql = @"select count(1) from &DataSource&wms_d_outbound_h h where h.t_id = @v_OutboundID";
                param.Clear();
                param.Add("v_OutboundID", v_OutboundID);
                selectOne = DBHelper.SelectOne(sql, param, "查询出库主档是否存在", false);
                if (Convert.ToInt32(selectOne) < 1)
                {
                    o_msg = "出库主档不存在";
                    o_result = 0;
                    return;
                }

                sql = @" select h.doc_no from &DataSource&wms_d_outbound_h h where h.t_id = @v_OutboundID";
                param.Clear();
                param.Add("v_OutboundID", v_OutboundID);
                dt = DBHelper.Select(sql, param, "查询出库主档信息", false);
                v_DocNo = dt.Rows[0]["doc_no"] == null ? "" : dt.Rows[0]["doc_no"].ToString();

                sql = @"select count(1) from &DataSource&wms_b_task t where t.t_id = @v_TaskID";
                param.Clear();
                param.Add("v_TaskID", v_TaskID);
                selectOne = DBHelper.SelectOne(sql, param, "查询任务主档是否存在", false);

                if (Convert.ToInt32(selectOne) < 1)
                {
                    o_msg = "任务主档信息不存在";
                    o_result = 0;
                    return;
                }

                sql = @"select t.task_no from &DataSource&wms_b_task t where t.t_id = @v_TaskID";
                param.Clear();
                param.Add("v_TaskID", v_TaskID);
                dt = DBHelper.Select(sql, param, "查询任务编号", false);

                v_TaskNo = dt.Rows[0]["task_no"] == null ? "" : dt.Rows[0]["task_no"].ToString();

                sql = @"select count(1) from &DataSource&wms_b_container_d d where d.t_id = @v_ConDetailID";
                param.Clear();
                param.Add("v_ConDetailID", v_ConDetailID);
                selectOne = DBHelper.SelectOne(sql, param, "拣选任务的库存明细是否存在", false);

                if (Convert.ToInt32(selectOne) < 1)
                {
                    o_msg = "拣选任务的库存明细不存在";
                    o_result = 0;
                    return;
                }

                sql = @"select d.qty, d.container_id  from &DataSource&wms_b_container_d d where d.t_id = @v_ConDetailID";
                param.Clear();
                param.Add("v_ConDetailID", v_ConDetailID);
                dt = DBHelper.Select(sql, param, "查询库存信息", false);

                v_ConDetailNum = Convert.ToInt32(dt.Rows[0]["qty"] == null ? "0" : dt.Rows[0]["qty"].ToString());
                v_ConID = dt.Rows[0]["container_id"] == null ? "" : dt.Rows[0]["container_id"].ToString();

                sql = @" select count(1) from &DataSource&wms_b_container_h h where h.t_id = @v_ConID";
                param.Clear();
                param.Add("v_ConID", v_ConID);
                selectOne = DBHelper.SelectOne(sql, param, "查询拣选任务的库存主档信息是否存在", false);

                if (Convert.ToInt32(selectOne) < 1)
                {
                    o_msg = "拣选任务的库存主档不存在";
                    o_result = 0;
                    return;
                }

                sql = @"  select h.container_barcode from &DataSource&wms_b_container_h h where h.t_id = @v_ConID";
                param.Clear();
                param.Add("v_ConID", v_ConID);
                dt = DBHelper.Select(sql, param, "查询拣选任务的库存主档信息", false);

                v_ContainerCode = dt.Rows[0]["container_barcode"] == null ? "0" : dt.Rows[0]["container_barcode"].ToString();

                if (v_ContainerCode == OrderContainerCode)
                {
                    o_result = 0;
                    o_msg = "目标容器不能为拣选容器";
                    return;
                }

                /****************************判断模式和数量*******************************/

                sql = @"select Config_Value from &DataSource&wms_s_config w where w.config_key='ISRFAllowPickLess'";
                param.Clear();
                allowLess = DBHelper.SelectOne(sql, param, "查询配置是否可少拣", false);

                if (allowLess != "Yes" && allowLess != "No")
                {
                    o_result = 0;
                    o_msg = "未配置是否可以少拣(Yes/No)：Config_Key=ISRFAllowPickLess";
                    return;
                }


                if (PickType == "1") //正常模式
                {
                    if (PickNum != 0)
                    {
                        if (NeedPick < (PickNum + readyPickNum))
                        {
                            o_result = 0;
                            o_msg = "不允许多拣";
                            return;
                        }
                        if (allowLess == "No")
                        {
                            if (NeedPick > (PickNum + readyPickNum))
                            {
                                o_msg = "策略不允许少拣";
                                o_result = 0;
                                return;
                            }
                        }
                    }
                }
                else if (PickType == "2") //交换模式
                {
                    if (PickNum != v_ConDetailNum)
                    {
                        if (NeedPick < v_ConDetailNum - PickNum + readyPickNum)
                        {
                            o_msg = "不允许多拣";
                            o_result = 0;
                            return;
                        }

                        if (allowLess == "No")
                        {
                            if (NeedPick > v_ConDetailNum - PickNum + readyPickNum)
                            {
                                o_msg = "策略不允许少拣";
                                o_result = 0;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    o_msg = "无效的拣选模式";
                    o_result = 0;
                    return;
                }



                /*********************更新任务、单据、波次信息******************************/

                //更新拣选任务数量
                sql = @"update &DataSource&wms_b_task_item i set i.processed_qty    = ifnull(i.processed_qty, 0) + @PickNum1,
                    i.finished_qty     = ifnull(i.finished_qty, 0) + @PickNum2,
                    i.process_status   = 'ProcessStatus_Done',
                    i.modify_date      = to_date(@modify_date,'yy-mm-dd hh24:mi:ss'),
                    i.last_modified_by = @last_modified_by 
                    where i.t_id = @TaskDetailID";

                param.Clear();
                param.Add("PickNum1", PickNum.ToString());
                param.Add("PickNum2", PickNum.ToString());
                param.Add("last_modified_by", Context.UserInfo.UserName);
                param.Add("modify_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                param.Add("TaskDetailID", TaskDetailID);
                rescount = DBHelper.RunSql(sql, param, "更新拣选明细任务数量", DBHelper.OperatedNumType.ZeroOrMore);

                if (rescount < 1)
                {
                    o_msg = "更新拣选任务数量出错";
                    o_result = 0;
                    return;
                }

                sql = @"select count(*) from &DataSource&wms_b_task_item i where i.task_id=@v_TaskID and i.process_status = 'ProcessStatus_Done'";
                param.Clear();
                param.Add("v_TaskID", v_TaskID);
                selectOne = DBHelper.SelectOne(sql, param, "查询任务明细是否都已完成", false);
                if (Convert.ToInt32(selectOne) == 0)
                {
                    sql = @"update &DataSource&wms_b_task i set i.Task_Status='TaskStatus_Finished' where i.t_id=@v_TaskID ";
                    param.Clear();
                    param.Add("v_TaskID", v_TaskID);
                    rescount = DBHelper.RunSql(sql, param, "更新任务主档为已完成", DBHelper.OperatedNumType.ZeroOrMore);
                    if (rescount < 1)
                    {
                        o_msg = "更新任务主档为已完成出错";
                        o_result = 0;
                        return;
                    }
                }


                //更新出库单明细已处理数量
                sql = @"update &DataSource&wms_d_outbound_d d set d.processed_qty = ifnull(d.processed_qty, 0) + @PickNum where d.t_id = @v_OutboundDetailID";
                param.Clear();
                param.Add("PickNum", PickNum.ToString());
                param.Add("v_OutboundDetailID", v_OutboundDetailID);
                rescount = DBHelper.RunSql(sql, param, "更新出库单明细已处理数量", DBHelper.OperatedNumType.ZeroOrMore);
                if (rescount < 1)
                {
                    o_msg = "更新出库单明细已处理数量出错";
                    o_result = 0;
                    return;
                }

                if (v_Ref_doc_type == "DocType_Wave")
                {
                    //有波次需要更新波次信息
                    sql = @" update &DataSource&wms_b_wave_d d set d.processed_qty = ifnull(d.processed_qty, 0) + @PickNum where d.t_id = @v_WaveDetailID";
                    param.Clear();
                    param.Add("PickNum", PickNum.ToString());
                    param.Add("v_WaveDetailID", v_WaveDetailID);
                    rescount = DBHelper.RunSql(sql, param, "更新波次已处理数量", DBHelper.OperatedNumType.ZeroOrMore);
                    if (rescount < 1)
                    {
                        o_msg = "更新波次已处理数量出错";
                        o_result = 0;
                        return;
                    }


                    //sql = @"select count(1)  from &DataSource&wms_b_task_item d where d.ref_doc_id = @v_OutboundID  and d.process_status = 'ProcessStatus_New'";
                    //param.Clear();
                    //param.Add("v_OutboundID", v_OutboundID);
                    //selectOne = DBHelper.SelectOne(sql, param, "查询波次主表的波次明细任务是否都已经完成", false);

                    //if (Convert.ToInt32(selectOne) == 0)
                    //{
                    //    //波次主表的波次明细任务都已经完成
                    //    sql = @" update &DataSource&wms_b_wave_h d set d.wave_status = 'WMSBWaveH_Finished',d.modify_date =to_date(@modify_date,'yy-mm-dd hh24:mi:ss'),
                    //      d.last_modified_by = @last_modified_by  where d.t_id = '" + v_WaveID + "'";
                    //    param.Clear();
                    //    param.Add("modify_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    //    param.Add("last_modified_by", Context.UserInfo.UserName);
                    //    rescount = DBHelper.RunSql(sql, param, "更新波次主表状态", DBHelper.OperatedNumType.ZeroOrMore);
                    //    if (rescount < 1)
                    //    {
                    //        o_msg = "更新波次主表状态出错";
                    //        o_result = 0;
                    //        return;
                    //    }
                    //}
                }

                /**余料回库任务**/
                sql = @"select count(1) from &DataSource&wms_b_task_item w inner join &DataSource&wms_b_task t 
                        on t.t_id = w.task_id   where w.process_status = 'ProcessStatus_New'
                    and t.CONTAINER_ID =@container_id and t.REF_DOC_ID = @REF_DOC_ID";
                param.Clear();
                param.Add("container_id", v_ConID);
                param.Add("REF_DOC_ID", v_OutboundID);
                selectOne = DBHelper.SelectOne(sql, param, "查询任务明细任务是否已全部完成", false);

                if (Convert.ToInt32(selectOne) < 1)
                {
                    sql = @"select Config_Value from &DataSource&wms_s_config w where w.config_key='ISPickAutoGenBackTask'";
                    param.Clear();
                    IsAutoGenTask = DBHelper.SelectOne(sql, param, "查询配置是托盘拣选完既产生余料回库任务", false);

                    if (IsAutoGenTask != "Yes" && IsAutoGenTask != "No")
                    {
                        o_result = 0;
                        o_msg = "未配置是否可以少拣(Yes/No)：Config_Key=ISRFAllowPickLess";
                        return;
                    }

                    if (IsAutoGenTask == "Yes")
                    {
                        //查询该托盘是否需要产生余料回库任务
                        WebJsonInfo tempInfo = LK_PickBack(v_ContainerCode, v_Dependent_TagAddress);
                        if (!tempInfo.isSucceed)
                        {
                            o_result = 0;
                            o_msg = "产生拣选回库任务失败";
                            return;
                        }
                    }
                }


                /*更新单据信息*/

                //该拣选任务完成后，判断同出库明细单的拣选明细任务是否都已经全部完成

                sql = @"select count(1) from &DataSource&wms_b_task_item w where w.process_status = 'ProcessStatus_New'
                    and w.ref_doc_d_id = (select ref_doc_d_id
                    from &DataSource&wms_b_task_item where t_id = @TaskDetailID)";
                param.Clear();
                param.Add("TaskDetailID", TaskDetailID);
                selectOne = DBHelper.SelectOne(sql, param, "查询任务明细任务是否已全部完成", false);

                if (Convert.ToInt32(selectOne) < 1)
                {
                    //任务明细任务已全部完成
                    sql = @"update &DataSource&wms_d_outbound_d d set d.finished_qty = ifnull(d.processed_qty, 0)
                        where d.t_id = @v_OutboundDetailID";
                    param.Clear();
                    param.Add("v_OutboundDetailID", v_OutboundDetailID);
                    rescount = DBHelper.RunSql(sql, param, "更新出库单明数量", DBHelper.OperatedNumType.ZeroOrMore);

                    if (rescount < 1)
                    {
                        o_msg = "更新出库单明数量出错";
                        o_result = 0;
                        return;
                    }

                    sql = @"select count(1) from &DataSource&wms_d_outbound_d d where (d.finished_qty is null) and d.outbound_h_id = @v_OutboundID";
                    param.Clear();
                    param.Add("v_OutboundID", v_OutboundID);
                    selectOne = DBHelper.SelectOne(sql, param, "出库明细是否全部达到计划量", false);

                    if (Convert.ToInt32(selectOne) < 1)
                    {
                        //全部达到计划量
                        sql = @"update &DataSource&wms_d_outbound_h h set h.outbound_status = 'OutDocStatus_Finished' where h.t_id = @v_OutboundID";
                        param.Clear();
                        param.Add("v_OutboundID", v_OutboundID);
                        rescount = DBHelper.RunSql(sql, param, "更新库存主档为删除", DBHelper.OperatedNumType.ZeroOrMore);
                        if (rescount < 1)
                        {
                            o_msg = "更新出库单状态出错";
                            o_result = 0;
                            return;
                        }
                        else
                        {
                            //清空货位
                            sql = "update &DataSource&wms_m_location l set l.LOCATION_STATUS='LocationStatus_Empty' where l.t_id = @LocationID";
                            param.Clear();
                            param.Add("LocationID", v_LocationID);
                            rescount = DBHelper.RunSql(sql, param, "更新货位信息为空", DBHelper.OperatedNumType.ZeroOrMore);
                            if (rescount < 1)
                            {
                                o_msg = "更新货位信息为空出错";
                                o_result = 0;
                                return;
                            }
                        }
                    }


                }

                /**************************扣减出库托盘库存、新增空托盘库存********************************/

                sql = @"select count(1) from &DataSource&wms_b_container_h h where h.container_barcode = @OrderContainerCode
                    and h.act_status = 1";
                param.Clear();
                param.Add("OrderContainerCode", OrderContainerCode);
                selectOne = DBHelper.SelectOne(sql, param, "空托盘是否存在主档", false);

                if (Convert.ToInt32(selectOne) == 0) // 空托盘不存在主档
                {
                    v_ConNewID = SnowflakeID.Guid().ToString();

                    sql = @"select @v_ConNewID as t_id, 
                         client_id,
                         org_id,
                         warehouse_id,
                         '' location_id,
                         @OrderContainerCode container_barcode,
                         is_full,
                         container_type,
                         container_catalog_id,
                         bom_id,
                         equipment_id,
                         allow_called,
                         first_in_date,
                         label_code,
                         owner_id,
                         is_in,
                         take_in_by,
                         is_out,
                         take_out_by,
                         is_mixed_item,
                         is_mixed_lot,
                         '立库RF拣选产生' memo,
                         @create_date create_date,
                         @created_by created_by,
                         modify_ip,
                         modify_date,
                         @last_modified_by last_modified_by,
                         '1'  act_status
                    from &DataSource&wms_b_container_h h where h.t_id = @v_ConID";

                    param.Clear();
                    param.Add("v_ConNewID", v_ConNewID);
                    param.Add("OrderContainerCode", OrderContainerCode);
                    param.Add("create_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    param.Add("created_by", Context.UserInfo.UserName);
                    param.Add("last_modified_by", Context.UserInfo.UserName);
                    param.Add("v_ConID", v_ConID);
                    dt = DBHelper.Select(sql, param, "查询并生成库存主表信息", false);

                    sql = @"insert into &DataSource&wms_b_container_h
                          (t_id,
                           client_id,
                           org_id,
                           warehouse_id,
                           location_id,
                           container_barcode,
                           is_full,
                           container_type,
                           container_catalog_id,
                           bom_id,
                           equipment_id,
                           allow_called,
                           first_in_date,
                           label_code,
                           owner_id,
                           is_in,
                           take_in_by,
                           is_out,
                           take_out_by,
                           is_mixed_item,
                           is_mixed_lot,
                           memo,
                           create_date,
                           created_by,
                           modify_ip,
                           modify_date,
                           last_modified_by,
                           act_status) 
                           values
                          (@t_id,
                           @client_id,
                           @org_id,
                           @warehouse_id,
                           @location_id,
                           @container_barcode,
                           @is_full,
                           @container_type,
                           @container_catalog_id,
                           @bom_id,
                           @equipment_id,
                           @allow_called,
                           to_date(@first_in_date,'yy-mm-dd hh24:mi:ss'),
                           @label_code,
                           @owner_id,
                           @is_in,
                           @take_in_by,
                           @is_out,
                           @take_out_by,
                           @is_mixed_item,
                           @is_mixed_lot,
                           @memo,
                           to_date(@create_date,'yy-mm-dd hh24:mi:ss'),
                           @created_by,
                           @modify_ip,
                           to_date(@modify_date,'yy-mm-dd hh24:mi:ss'),
                           @last_modified_by,
                           @act_status)";

                    param.Clear();
                    param.Add("t_id", (dt.Rows[0]["t_id"] == null ? "" : dt.Rows[0]["t_id"].ToString()));
                    param.Add("client_id", (dt.Rows[0]["client_id"] == null ? "" : dt.Rows[0]["client_id"].ToString()));
                    param.Add("org_id", (dt.Rows[0]["org_id"] == null ? "" : dt.Rows[0]["org_id"].ToString()));
                    param.Add("warehouse_id", (dt.Rows[0]["warehouse_id"] == null ? "" : dt.Rows[0]["warehouse_id"].ToString()));
                    param.Add("location_id", (dt.Rows[0]["location_id"] == null ? "" : dt.Rows[0]["location_id"].ToString()));
                    param.Add("container_barcode", (dt.Rows[0]["container_barcode"] == null ? "" : dt.Rows[0]["container_barcode"].ToString()));
                    param.Add("is_full", (dt.Rows[0]["is_full"] == null ? "" : dt.Rows[0]["is_full"].ToString()));
                    param.Add("container_type", (dt.Rows[0]["container_type"] == null ? "" : dt.Rows[0]["container_type"].ToString()));
                    param.Add("container_catalog_id", (dt.Rows[0]["container_catalog_id"] == null ? "" : dt.Rows[0]["container_catalog_id"].ToString()));
                    param.Add("bom_id", (dt.Rows[0]["bom_id"] == null ? "" : dt.Rows[0]["bom_id"].ToString()));
                    param.Add("equipment_id", (dt.Rows[0]["equipment_id"] == null ? "" : dt.Rows[0]["equipment_id"].ToString()));
                    param.Add("allow_called", (dt.Rows[0]["allow_called"] == null ? "" : dt.Rows[0]["allow_called"].ToString()));
                    param.Add("first_in_date", (dt.Rows[0]["first_in_date"] == null ? "" : dt.Rows[0]["first_in_date"].ToString()));
                    param.Add("label_code", (dt.Rows[0]["label_code"] == null ? "" : dt.Rows[0]["label_code"].ToString()));
                    param.Add("owner_id", (dt.Rows[0]["owner_id"] == null ? "" : dt.Rows[0]["owner_id"].ToString()));
                    param.Add("is_in", (dt.Rows[0]["is_in"] == null ? "" : dt.Rows[0]["is_in"].ToString()));
                    param.Add("take_in_by", (dt.Rows[0]["take_in_by"] == null ? "" : dt.Rows[0]["take_in_by"].ToString()));
                    param.Add("is_out", (dt.Rows[0]["is_out"] == null ? "" : dt.Rows[0]["is_out"].ToString()));
                    param.Add("take_out_by", (dt.Rows[0]["take_out_by"] == null ? "" : dt.Rows[0]["take_out_by"].ToString()));
                    param.Add("is_mixed_item", (dt.Rows[0]["is_mixed_item"] == null ? "" : dt.Rows[0]["is_mixed_item"].ToString()));
                    param.Add("is_mixed_lot", (dt.Rows[0]["is_mixed_lot"] == null ? "" : dt.Rows[0]["is_mixed_lot"].ToString()));
                    param.Add("memo", (dt.Rows[0]["memo"] == null ? "" : dt.Rows[0]["memo"].ToString()));
                    param.Add("create_date", (dt.Rows[0]["create_date"] == null ? "" : dt.Rows[0]["create_date"].ToString()));
                    param.Add("created_by", (dt.Rows[0]["created_by"] == null ? "" : dt.Rows[0]["created_by"].ToString()));
                    param.Add("modify_ip", (dt.Rows[0]["modify_ip"] == null ? "" : dt.Rows[0]["modify_ip"].ToString()));
                    param.Add("modify_date", (dt.Rows[0]["modify_date"] == null ? "" : dt.Rows[0]["modify_date"].ToString()));
                    param.Add("last_modified_by", (dt.Rows[0]["last_modified_by"] == null ? "" : dt.Rows[0]["last_modified_by"].ToString()));
                    param.Add("act_status", (dt.Rows[0]["act_status"] == null ? "" : dt.Rows[0]["act_status"].ToString()));

                    rescount = DBHelper.RunSql(sql, param, "插入空托盘库存", DBHelper.OperatedNumType.ZeroOrMore);
                    if (rescount < 1)
                    {
                        o_msg = "插入空托盘库存出错";
                        o_result = 0;
                        return;
                    }
                }
                else if (Convert.ToInt32(selectOne) == 1)
                {
                    sql = @"select h.t_id  from &DataSource&wms_b_container_h h where h.container_barcode = @OrderContainerCode 
                        and h.act_status = 1";

                    param.Clear();
                    param.Add("OrderContainerCode", OrderContainerCode);
                    dt = DBHelper.Select(sql, param, "查询空托盘主档信息", false);
                    v_ConNewID = dt.Rows[0]["t_id"] == null ? "0" : dt.Rows[0]["t_id"].ToString();
                }
                else
                {
                    o_msg = "存在多个空托盘主档信息";
                    o_result = 0;
                    return;
                }

                v_ConNewDetailID = SnowflakeID.Guid().ToString();
                sql = @"select '" + v_ConNewDetailID + @"' t_id,
                        client_id,
                        org_id,
                        warehouse_id,
                        '" + v_ConNewID + @"' container_id,
                        inbound_h_id,
                        outbound_h_id,
                        sub_container_no,
                        supplier_id,
                        bom_id,
                        owner_id,
                        item_id,
                        item_name,
                        lot_no,
                        owner_lot_no,
                        wms_lot_no,
                        exist_barcode,
                        is_full,
                        given_year,
                        sub_factory,
                        sub_inventory,
                        sub_location,
                        qc_status,
                        '" + PickNum + @"' qty,
                        stat_qty,
                        0 in_qty,
                        0 out_qty,
                        0 lock_qty,
                        spec,
                        uom_qty_id,
                        uom_stat_id,
                        ratio,
                        first_in_date,
                        start_out_date,
                        production_date,
                        exp_date,
                        warning_date,
                        first_out_date,
                        out_end_date,
                        use_by_date,
                        @create_date create_date,
                        @created_by created_by,
                        modify_ip,
                        modify_date,
                        @last_modified_by last_modified_by,
                        1 act_status
                        from &DataSource&wms_b_container_d
                        where t_id = '" + v_ConDetailID + "'";

                param.Clear();
                param.Add("create_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                param.Add("created_by", Context.UserInfo.UserName);
                param.Add("last_modified_by", Context.UserInfo.UserName);
                dt = DBHelper.Select(sql, param, "查询并生成空托盘明细信息", false);

                sql = @" insert into &DataSource&wms_b_container_d
                    (   t_id,
                        client_id,
                        org_id,
                        warehouse_id,
                        container_id,
                        inbound_h_id,
                        outbound_h_id,
                        sub_container_no,
                        supplier_id,
                        bom_id,
                        owner_id,
                        item_id,
                        item_name,
                        lot_no,
                        owner_lot_no,
                        wms_lot_no,
                        exist_barcode,
                        is_full,
                        given_year,
                        sub_factory,
                        sub_inventory,
                        sub_location,
                        qc_status,
                        qty,
                        stat_qty,
                        in_qty,
                        out_qty,
                        lock_qty,
                        spec,
                        uom_qty_id,
                        uom_stat_id,
                        ratio,
                        first_in_date,
                        start_out_date,
                        production_date,
                        exp_date,
                        warning_date,
                        first_out_date,
                        out_end_date,
                        use_by_date,
                        create_date,
                        created_by,
                        modify_ip,
                        modify_date,
                        last_modified_by,
                        act_status)
                    values(@t_id,
                        @client_id,
                        @org_id,
                        @warehouse_id,
                        @container_id,
                        @inbound_h_id,
                        @outbound_h_id,
                        @sub_container_no,
                        @supplier_id,
                        @bom_id,
                        @owner_id,
                        @item_id,
                        @item_name,
                        @lot_no,
                        @owner_lot_no,
                        @wms_lot_no,
                        @exist_barcode,
                        @is_full,
                        @given_year,
                        @sub_factory,
                        @sub_inventory,
                        @sub_location,
                        @qc_status,
                        @qty,
                        @stat_qty,
                        @in_qty,
                        @out_qty,
                        @lock_qty,
                        @spec,
                        @uom_qty_id,
                        @uom_stat_id,
                        @ratio,
                        to_date(@first_in_date,'yy-mm-dd hh24:mi:ss'),
                        to_date(@start_out_date,'yy-mm-dd hh24:mi:ss'),
                        to_date(@production_date,'yy-mm-dd hh24:mi:ss'),
                        to_date(@exp_date,'yy-mm-dd hh24:mi:ss'),
                        to_date(@warning_date,'yy-mm-dd hh24:mi:ss'),
                        to_date(@first_out_date,'yy-mm-dd hh24:mi:ss'),
                        to_date(@out_end_date,'yy-mm-dd hh24:mi:ss'),
                        to_date(@use_by_date,'yy-mm-dd hh24:mi:ss'),
                        to_date(@create_date,'yy-mm-dd hh24:mi:ss'),
                        @created_by,
                        @modify_ip,
                        to_date(@modify_date,'yy-mm-dd hh24:mi:ss'),
                        @last_modified_by,
                        @act_status)";

                param.Clear();
                param.Add("t_id", dt.Rows[0]["t_id"] == null ? "" : dt.Rows[0]["t_id"].ToString());
                param.Add("client_id", dt.Rows[0]["client_id"] == null ? "" : dt.Rows[0]["client_id"].ToString());
                param.Add("org_id", dt.Rows[0]["org_id"] == null ? "" : dt.Rows[0]["org_id"].ToString());
                param.Add("warehouse_id", dt.Rows[0]["warehouse_id"] == null ? "" : dt.Rows[0]["warehouse_id"].ToString());
                param.Add("container_id", dt.Rows[0]["container_id"] == null ? "" : dt.Rows[0]["container_id"].ToString());
                param.Add("inbound_h_id", dt.Rows[0]["inbound_h_id"] == null ? "" : dt.Rows[0]["inbound_h_id"].ToString());
                param.Add("outbound_h_id", dt.Rows[0]["outbound_h_id"] == null ? "" : dt.Rows[0]["outbound_h_id"].ToString());
                param.Add("sub_container_no", dt.Rows[0]["sub_container_no"] == null ? "" : dt.Rows[0]["sub_container_no"].ToString());
                param.Add("supplier_id", dt.Rows[0]["supplier_id"] == null ? "" : dt.Rows[0]["supplier_id"].ToString());
                param.Add("bom_id", dt.Rows[0]["bom_id"] == null ? "" : dt.Rows[0]["bom_id"].ToString());
                param.Add("owner_id", dt.Rows[0]["owner_id"] == null ? "" : dt.Rows[0]["owner_id"].ToString());
                param.Add("item_id", dt.Rows[0]["item_id"] == null ? "" : dt.Rows[0]["item_id"].ToString());
                param.Add("item_name", dt.Rows[0]["item_name"] == null ? "" : dt.Rows[0]["item_name"].ToString());
                param.Add("lot_no", dt.Rows[0]["lot_no"] == null ? "" : dt.Rows[0]["lot_no"].ToString());
                param.Add("owner_lot_no", dt.Rows[0]["owner_lot_no"] == null ? "" : dt.Rows[0]["owner_lot_no"].ToString());
                param.Add("wms_lot_no", dt.Rows[0]["wms_lot_no"] == null ? "" : dt.Rows[0]["wms_lot_no"].ToString());
                param.Add("exist_barcode", dt.Rows[0]["exist_barcode"] == null ? "" : dt.Rows[0]["exist_barcode"].ToString());
                param.Add("is_full", dt.Rows[0]["is_full"] == null ? "" : dt.Rows[0]["is_full"].ToString());
                param.Add("given_year", dt.Rows[0]["given_year"] == null ? "" : dt.Rows[0]["given_year"].ToString());
                param.Add("sub_factory", dt.Rows[0]["sub_factory"] == null ? "" : dt.Rows[0]["sub_factory"].ToString());
                param.Add("sub_inventory", dt.Rows[0]["sub_inventory"] == null ? "" : dt.Rows[0]["sub_inventory"].ToString());
                param.Add("sub_location", dt.Rows[0]["sub_location"] == null ? "" : dt.Rows[0]["sub_location"].ToString());
                param.Add("qc_status", dt.Rows[0]["qc_status"] == null ? "" : dt.Rows[0]["qc_status"].ToString());
                param.Add("qty", dt.Rows[0]["qty"] == null ? "" : dt.Rows[0]["qty"].ToString());
                param.Add("stat_qty", dt.Rows[0]["stat_qty"] == null ? "" : dt.Rows[0]["stat_qty"].ToString());
                param.Add("in_qty", dt.Rows[0]["in_qty"] == null ? "" : dt.Rows[0]["in_qty"].ToString());
                param.Add("out_qty", dt.Rows[0]["out_qty"] == null ? "" : dt.Rows[0]["out_qty"].ToString());
                param.Add("lock_qty", dt.Rows[0]["lock_qty"] == null ? "" : dt.Rows[0]["lock_qty"].ToString());
                param.Add("spec", dt.Rows[0]["spec"] == null ? "" : dt.Rows[0]["spec"].ToString());
                param.Add("uom_qty_id", dt.Rows[0]["uom_qty_id"] == null ? "" : dt.Rows[0]["uom_qty_id"].ToString());
                param.Add("uom_stat_id", dt.Rows[0]["uom_stat_id"] == null ? "" : dt.Rows[0]["uom_stat_id"].ToString());
                param.Add("ratio", dt.Rows[0]["ratio"] == null ? "" : dt.Rows[0]["ratio"].ToString());
                param.Add("first_in_date", dt.Rows[0]["first_in_date"] == null ? "" : dt.Rows[0]["first_in_date"].ToString());
                param.Add("start_out_date", dt.Rows[0]["start_out_date"] == null ? "" : dt.Rows[0]["start_out_date"].ToString());
                param.Add("production_date", dt.Rows[0]["production_date"] == null ? "" : dt.Rows[0]["production_date"].ToString());
                param.Add("exp_date", dt.Rows[0]["exp_date"] == null ? "" : dt.Rows[0]["exp_date"].ToString());
                param.Add("warning_date", dt.Rows[0]["warning_date"] == null ? "" : dt.Rows[0]["warning_date"].ToString());
                param.Add("first_out_date", dt.Rows[0]["first_out_date"] == null ? "" : dt.Rows[0]["first_out_date"].ToString());
                param.Add("out_end_date", dt.Rows[0]["out_end_date"] == null ? "" : dt.Rows[0]["out_end_date"].ToString());
                param.Add("use_by_date", dt.Rows[0]["use_by_date"] == null ? "" : dt.Rows[0]["use_by_date"].ToString());
                param.Add("create_date", dt.Rows[0]["create_date"] == null ? "" : dt.Rows[0]["create_date"].ToString());
                param.Add("created_by", dt.Rows[0]["created_by"] == null ? "" : dt.Rows[0]["created_by"].ToString());
                param.Add("modify_ip", dt.Rows[0]["modify_ip"] == null ? "" : dt.Rows[0]["modify_ip"].ToString());
                param.Add("modify_date", dt.Rows[0]["modify_date"] == null ? "" : dt.Rows[0]["modify_date"].ToString());
                param.Add("last_modified_by", dt.Rows[0]["last_modified_by"] == null ? "" : dt.Rows[0]["last_modified_by"].ToString());
                param.Add("act_status", dt.Rows[0]["act_status"] == null ? "" : dt.Rows[0]["act_status"].ToString());

                rescount = DBHelper.RunSql(sql, param, "插入空托盘库存明细", DBHelper.OperatedNumType.ZeroOrMore);
                if (rescount < 1)
                {
                    o_msg = "插入空托盘库存明细出错";
                    o_result = 0;
                    return;
                }

                sql = @"update &DataSource&wms_b_container_d d set d.qty    = (d.qty - @PickNum1),
                    d.out_qty = (d.out_qty - @PickNum2) where d.t_id = @v_ConDetailID";
                param.Clear();
                param.Add("PickNum1", PickNum.ToString());
                param.Add("PickNum2", PickNum.ToString());
                param.Add("v_ConDetailID", v_ConDetailID);
                rescount = DBHelper.RunSql(sql, param, "扣减库存", DBHelper.OperatedNumType.ZeroOrMore);
                if (rescount < 1)
                {
                    o_msg = "扣减库存出错";
                    o_result = 0;
                    return;
                }

                //删除库存为0的数据
                sql = @"update &DataSource&wms_b_container_d d set d.act_status = '0' where d.t_id = @v_ConDetailID and d.qty=0 and d.act_status=1 ";

                param.Clear();
                param.Add("v_ConDetailID", v_ConDetailID);
                rescount = DBHelper.RunSql(sql, param, "更新出库托盘库存为0的明细", DBHelper.OperatedNumType.ZeroOrMore);

                if (rescount > 0) // 影响行数大于1表示托盘明细为0的库存有删除
                {
                    sql = @"select count(*) from &DataSource&wms_b_container_d d where d.CONTAINER_ID = @v_ConID and d.qty<>0 and d.act_status=1 ";
                    param.Clear();
                    param.Add("v_ConID", v_ConID);
                    selectOne = DBHelper.SelectOne(sql, param, "更新库存为空的托盘主档");
                    if (Convert.ToInt32(selectOne) < 1)
                    {
                        sql = @"update &DataSource&wms_b_container_h d set d.act_status = '0' where d.t_id = @v_ConID ";

                        param.Clear();
                        param.Add("v_ConID", v_ConID);
                        rescount = DBHelper.RunSql(sql, param, "更新库存为空的托盘主档", DBHelper.OperatedNumType.ZeroOrMore);

                        if (rescount < 1) // 空托盘不存在主档
                        {
                            o_msg = "更新库存为空的托盘主档出错";
                            o_result = 0;
                            return;
                        }
                    }
                }




                /********************************交换托盘模式*********************************/

                if (PickType == "2")
                {
                    //交换托盘模式
                    sql = @"update &DataSource&wms_b_container_d d set d.container_id = @v_ConNewID1 where d.container_id = @v_ConNewID2";
                    param.Clear();
                    param.Add("v_ConNewID1", "Tep" + v_ConNewID);
                    param.Add("v_ConNewID2", v_ConNewID);
                    rescount = DBHelper.RunSql(sql, param, "交换托盘模式更换空托盘库存信息为临时库存", DBHelper.OperatedNumType.ZeroOrMore);
                    if (rescount < 1)
                    {
                        o_msg = "交换托盘模式时更换空托盘库存信息为临时库存时出错";
                        o_result = 0;
                        return;
                    }

                    sql = @"update &DataSource&wms_b_container_d d set d.container_id = @v_ConNewID where d.container_id = @v_ConID";
                    param.Clear();
                    param.Add("v_ConNewID", v_ConNewID);
                    param.Add("v_ConID", v_ConID);
                    rescount = DBHelper.RunSql(sql, param, "交换托盘模式更换原库存信息", DBHelper.OperatedNumType.ZeroOrMore);

                    if (rescount < 1)
                    {
                        o_msg = "交换托盘模式时更换原库存信息出错";
                        o_result = 0;
                        return;
                    }

                    sql = @"update &DataSource&wms_b_container_d d set d.container_id = @v_ConID where d.container_id = @v_ConNewID ";
                    param.Clear();
                    param.Add("v_ConID", v_ConID);
                    param.Add("v_ConNewID", "Tep" + v_ConNewID);
                    rescount = DBHelper.RunSql(sql, param, "交换托盘模式更换新库存信息", DBHelper.OperatedNumType.ZeroOrMore);
                    if (rescount < 1)
                    {
                        o_msg = "交换托盘模式时更换新库存信息出错";
                        o_result = 0;
                        return;
                    }
                }
                else if (PickType == "1") //正常模式不做处理
                {

                }
                else
                {
                    o_msg = "未知的拣选模式";
                    o_result = 0;
                    return;
                }

                /********************************箱码处理*********************************/

                if (BNList != null && BNList.Count > 0)
                {
                    string bnstr = string.Join(',', BNList);
                    sql = @"select count(1) from &DataSource&wms_b_sn_h where item_sn is not null
                       and ',' || @bnstr || ',' like '%,' || item_sn || ',%'
                       and store_status = '1'";
                    param.Clear();
                    param.Add("bnstr", bnstr);
                    selectOne = DBHelper.SelectOne(sql, param, "查询是否存在箱码数据", false);
                    if (Convert.ToInt32(selectOne) == 0) // 不存在当前箱码数据
                    {
                        o_msg = "不存在当前箱码数据";
                        o_result = 0;
                        return;
                    }

                    if (Convert.ToInt32(selectOne) != BNList.Count)
                    {
                        o_msg = "箱码计数和需处理计数不一致";
                        o_result = 0;
                        return;
                    }

                    string newbnlogid = SnowflakeID.Guid().ToString();
                    sql = @"select  @newbnlogid t_id, 
                             client_id,
                             org_id,
                             warehouse_id,
                             h.t_id sn_id, 
                             h.location_id from_location_id, 
                             '' from_location_no,
                             h.zone_name from_location_name,
                             '' to_location_id, 
                             '' to_location_no, 
                             '' to_location_name, 
                             '出库' action,
                             '立库RF拣选出库' reason, 
                             @v_DocNo doc_no,
                             @v_TaskNo task_no, 
                             @create_date create_date, 
                             @created_by created_by, 
                             modify_ip,
                             modify_date,
                             @last_modified_by last_modified_by,
                             1 act_status 
                             from &DataSource&wms_b_sn_h h
                             where h.item_sn is not null
                             and ',' || @bnstr || ',' like '%,' || h.item_sn || ',%'
                             and h.store_status = '1'";

                    param.Clear();
                    param.Add("newbnlogid", newbnlogid);
                    param.Add("v_DocNo", v_DocNo);
                    param.Add("v_TaskNo", v_TaskNo);
                    param.Add("create_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    param.Add("created_by", Context.UserInfo.UserName);
                    param.Add("last_modified_by", Context.UserInfo.UserName);
                    param.Add("bnstr", bnstr);
                    dt = DBHelper.Select(sql, param, "查询BN主档并创建插入数据", false);


                    sql = @"  insert into &DataSource&wms_b_sn_d
                          (t_id,
                           client_id,
                           org_id,
                           warehouse_id,
                           sn_id,
                           from_location_id,
                           from_location_no,
                           from_location_name,
                           to_location_id,
                           to_location_no,
                           to_location_name,
                           action,
                           reason,
                           doc_no,
                           task_no,
                           create_date,
                           created_by,
                           modify_ip,
                           modify_date,
                           last_modified_by,
                           act_status)
                           values(@t_id,
                           @client_id,
                           @org_id,
                           @warehouse_id,
                           @sn_id,
                           @from_location_id,
                           @from_location_no,
                           @from_location_name,
                           @to_location_id,
                           @to_location_no,
                           @to_location_name,
                           @action,
                           @reason,
                           @doc_no,
                           @task_no,
                           to_date(@create_date,'yy-mm-dd hh24:mi:ss'),
                           @created_by,
                           @modify_ip,
                           to_date(@modify_date,'yy-mm-dd hh24:mi:ss'),
                           @last_modified_by,
                           @act_status)";
                    param.Clear();
                    param.Add("t_id", dt.Rows[0]["t_id"] == null ? "" : dt.Rows[0]["t_id"].ToString());
                    param.Add("client_id", dt.Rows[0]["client_id"] == null ? "" : dt.Rows[0]["client_id"].ToString());
                    param.Add("org_id", dt.Rows[0]["org_id"] == null ? "" : dt.Rows[0]["org_id"].ToString());
                    param.Add("warehouse_id", dt.Rows[0]["warehouse_id"] == null ? "" : dt.Rows[0]["warehouse_id"].ToString());
                    param.Add("sn_id", dt.Rows[0]["sn_id"] == null ? "" : dt.Rows[0]["sn_id"].ToString());
                    param.Add("from_location_id", dt.Rows[0]["from_location_id"] == null ? "from_location_id" : dt.Rows[0][""].ToString());
                    param.Add("from_location_no", dt.Rows[0]["from_location_no"] == null ? "" : dt.Rows[0]["from_location_no"].ToString());
                    param.Add("from_location_name", dt.Rows[0]["from_location_name"] == null ? "" : dt.Rows[0]["from_location_name"].ToString());
                    param.Add("to_location_id", dt.Rows[0]["to_location_id"] == null ? "" : dt.Rows[0]["to_location_id"].ToString());
                    param.Add("to_location_no", dt.Rows[0]["to_location_no"] == null ? "" : dt.Rows[0]["to_location_no"].ToString());
                    param.Add("to_location_name", dt.Rows[0]["to_location_name"] == null ? "" : dt.Rows[0]["to_location_name"].ToString());
                    param.Add("action", dt.Rows[0]["action"] == null ? "" : dt.Rows[0]["action"].ToString());
                    param.Add("reason", dt.Rows[0]["reason"] == null ? "" : dt.Rows[0]["reason"].ToString());
                    param.Add("doc_no", dt.Rows[0]["doc_no"] == null ? "" : dt.Rows[0]["doc_no"].ToString());
                    param.Add("task_no", dt.Rows[0]["task_no"] == null ? "" : dt.Rows[0]["task_no"].ToString());
                    param.Add("create_date", dt.Rows[0]["create_date"] == null ? "" : dt.Rows[0]["create_date"].ToString());
                    param.Add("created_by", dt.Rows[0]["created_by"] == null ? "" : dt.Rows[0]["created_by"].ToString());
                    param.Add("modify_ip", dt.Rows[0]["modify_ip"] == null ? "" : dt.Rows[0]["modify_ip"].ToString());
                    param.Add("modify_date", dt.Rows[0]["modify_date"] == null ? "" : dt.Rows[0]["modify_date"].ToString());
                    param.Add("last_modified_by", dt.Rows[0]["last_modified_by"] == null ? "" : dt.Rows[0]["last_modified_by"].ToString());
                    param.Add("act_status", dt.Rows[0]["act_status"] == null ? "" : dt.Rows[0]["act_status"].ToString());
                    rescount = DBHelper.RunSql(sql, param, "插入箱码获取插入的行数", DBHelper.OperatedNumType.ZeroOrMore);
                    if (rescount != BNList.Count)
                    {
                        o_msg = "插入箱码出库明细流水行数和需处理行数不一致";
                        o_result = 0;
                        return;
                    }
                    sql = @"update &DataSource&wms_b_sn_h h set h.store_status = '0'  where h.item_sn is not null
                        and ',' || @bnstr || ',' like '%,' || h.item_sn || ',%' and h.store_status = '1'";
                    param.Clear();
                    param.Add("bnstr", bnstr);
                    rescount = DBHelper.RunSql(sql, param, "更新箱码出库明细,获取影响行数", DBHelper.OperatedNumType.ZeroOrMore);
                    if (rescount != BNList.Count)
                    {
                        o_msg = "更新箱码出库明细行数和需处理行数不一致";
                        o_result = 0;
                        return;
                    }

                    LogHelper.LogInformation("立库-RF拣选成功:TaskDetailID=" + TaskDetailID + ", PickType=" + PickType + ", OrderContainerCode=" + OrderContainerCode + ", PickNum=" + PickNum + ", NeedPick=" + NeedPick + ",BNList" + string.Join(',', BNList));
                }

                o_result = 1;

            }
            catch (Exception ex)
            {
                LogHelper.LogInformation("立库-RF拣选失败:TaskDetailID=" + TaskDetailID + ", PickType=" + PickType + ", OrderContainerCode=" + OrderContainerCode + ", PickNum=" + PickNum + ", NeedPick=" + NeedPick + ",BNList" + string.Join(',', BNList) + ",Err=" + ex.Message);
                o_msg = "发生错误:" + ex.Message;
                o_result = 0;
                throw ex;
            }
        }
        #endregion

        #region 外部-拣选-立库，拣选回库(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，拣选回库(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="ContainerCode">当前托盘</param>
        /// <param name="SourceAddress">当前站台地址</param>
        /// <param name="GenType">1实托盘回库，2订单托盘回库</param>
        /// <returns></returns>
        public WebJsonInfo LK_PickBack(string ContainerCode, string SourceAddress)
        {
            try
            {
                string sql = @"";
                string selectOne = "";
                string orderID = ""; //出库主档ID
                Dictionary<string, object> param = new Dictionary<string, object>();
                bool taskRes = InsertTask(ContainerCode, SourceAddress);
                if (taskRes == true)
                {
                    return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = null };
                }
                else
                {
                    throw new WmsException(500, "产生拣选回库任务失败");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 外部-拣选-立库，订单托盘回库(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，订单托盘回库(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="OrderContainerCode">当前订单托盘</param>
        /// <param name="SourceAddress">当前站台地址</param>
        /// <param name="GenType">1实托盘回库，2订单托盘回库</param>
        /// <returns></returns>
        public WebJsonInfo LK_OrderContainerBack(string OrderContainerCode, string SourceAddress)
        {
            try
            {
                string sql = @"";
                string selectOne = "";
                string orderID = ""; //出库主档ID
                Dictionary<string, object> param = new Dictionary<string, object>();
                bool taskRes = InsertTask(OrderContainerCode, SourceAddress);
                if (taskRes == true)
                {
                    return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = null };
                }
                else
                {
                    throw new WmsException(500, "产生订单托盘回库任务失败");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region  产生拣选回库任务
        private bool InsertTask(string containerBarCode, string sourceAddress)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                string sql = "";
                string selectOne = "";

                sql = "select d.ITEM_NAME ITEMNAME,h.t_id CONTAINER_ID,d.t_id CONTAINER_D_ID,d.ITEM_ID,d.qty  from &DataSource&WMS_B_CONTAINER_D d inner join &DataSource&wms_b_container_h h" +
                    "  on d.container_id = h.t_id where h.CONTAINER_BARCODE=@containerBarCode";
                param.Clear();
                param.Add("containerBarCode", containerBarCode);
                DataTable dt = DBHelper.Select(sql, param, "查询库存明细", false);
                if (dt.Rows.Count < 1)
                {
                    return false;
                }

                Route route = new Route();
                string anyItemCode = dt.Rows[0]["ITEM_ID"].ToString();
                route.Init(Item.Get(anyItemCode)["catalog_id"].ToString(), RoutePurpose.RoutePurpose_Inbound.ToString());

                Address src = new Address()
                {
                    ID = sourceAddress,
                    Type = "AddressType_Station", //输送机站台
                    Text = "输送机站台",
                };


                Address dest = new Address()
                {
                    ID = "",
                    Type = "AddressType_Zone", //存储区
                    Text = "",
                };

                if (!route.MatchRoute(src, dest, out DataRow[] sequences))
                {
                    throw new WmsException(500, "无法找到合适的配置来生成任务");
                }

                decimal num = dt.AsEnumerable().Sum(o=>o.Field<decimal>("QTY"));
                string taskID = SnowflakeID.Guid().ToString();
                Model.WorkTaskModel taskModel = new Model.WorkTaskModel()
                {
                    ID = taskID,
                    SeqNo = 100,
                    Priority = 100,
                    ContainerID = dt.Rows[0]["CONTAINER_ID"].ToString(),
                    ContainerBarcode = containerBarCode,
                    SrcAddrID = src.ID,
                    SrcAddrType = src.Type,
                    SrcAddrText = src.Text,
                    DestAddrID = dest.ID,
                    DestAddrType = dest.Type,
                    DestAddrText = dest.Text,
                    RefDocType = DocType.DocType_PickSurpluBack.ToString(),
                    ExecutedBy = TaskExecutor.TaskExecutor_WCS.ToString(),
                    ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),
                    TaskStatus = TaskStatus.TaskStatus_NotExecute.ToString(),
                    IsExecutable = "1"
                };

                List<WorkTaskItemModel> taskItemList = GetWorkItemList(dt.Rows, num);

                if (taskItemList == null)
                {
                    return false;
                }

                decimal taskNum = WorkTask.SaveTask(sequences, taskModel, taskItemList);
                List<string> taskIDList = new List<string>();
                taskIDList.Add(taskID);
                WorkTaskInterface.Send(taskIDList);

                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private List<WorkTaskItemModel> GetWorkItemList(DataRowCollection rowsInSameTask,decimal num)
        {
            List<WorkTaskItemModel> list = new List<WorkTaskItemModel>();

            foreach (DataRow row in rowsInSameTask)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                string sql = "select * from &DataSource&wms_m_item i where i.t_id=@ITEM_ID";
                param.Add("ITEM_ID", row["ITEM_ID"].ToString());
                DataTable dtItem = DBHelper.Select(sql, param, "拣选查询物料信息", false);
                if (dtItem == null || dtItem.Rows.Count != 1)
                {
                    return null;
                }
                list.Add(
                    new WorkTaskItemModel()
                    {
                        ItemID = dtItem.Rows[0]["T_ID"].ToString(),
                        ItemName = row["ITEMNAME"].ToString(),
                        PlanQty = num,
                        ProcessedQty = 0,
                        FinishedQty = 0,
                        FinishedStatQty = 0,
                        ContainerDetailID = row["CONTAINER_D_ID"].ToString(),
                        ProcessStatus = "TaskStatus_Create"
                    });
            }
            return list;
        }
        #endregion

        #region 完成余料回库任务
        /// <summary>
        /// 完成余料回库任务
        /// </summary>
        /// <param name="barcode"></param>
        public void RFPickSurpluBackOver(string taskId)
        {

            _workTask.UpdateTaskStatus(taskId, TaskStatus.TaskStatus_Finished.ToString());

            DataRow drTask = _workTask.GetWorkTask(taskId);



            DataRow[] drTaskItem = _workTaskItem.GetByTaskID(taskId);

            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "ref_doc_id",  drTask["ref_doc_id"].ToString() },
                    { "t_id",  drTask["t_id"].ToString() },
                    { "warehouse_id",  Context.WarehouseID }
                  };

            UpdateContainerLoc(drTask["container_id"].ToString(), drTask["dest_addr_id"].ToString());//更新库存对应的货位id


            foreach (var item in drTaskItem)
            {
                //更新任务明细
                UpdateTaskItem(item["t_id"].ToString());
            }

            UpdateLocStatus(drTask["dest_addr_id"].ToString(), LocationStatus.LocationStatus_Stored.ToString());//更新货位状态


            _workTask.MoveToHistory(taskId);
        }

        /// <summary>
        /// 更新任务物料
        /// </summary>
        /// <param name="tItemId">id</param>
        /// <param name="qty">操作量</param>
        /// <param name="diffQty">任务剩余量</param>
        private void UpdateTaskItem(string tItemId)
        {
            string strSql = string.Empty;
            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "t_id",  tItemId },
                    { "warehouse_id",  Context.WarehouseID}
                  };
            strSql = @"update &DataSource&wms_b_task_item t
                    set t.processed_qty=t.plan_qty,
                    t.finished_qty=t.plan_qty,t.start_date=nvl(t.start_date ,sysdate),t.end_date=sysdate ,
                    t.PROCESS_STATUS='ProcessStatus_Done'
                    where t.t_id=@t_id and t.client_id='&CLIENT_ID&' and t.org_id='&ORG_ID&' and t.warehouse_id = @warehouse_id";

            DBHelper.RunSql(strSql, dicParam, "更新任务明细", DBHelper.OperatedNumType.OnlyOne);
        }

        /// <summary>
        ///  更新容器货位id
        /// </summary>
        /// <param name="cId"></param>
        /// <param name="locationId"></param>
        private void UpdateContainerLoc(string cId, string locationId)
        {
            string strSql = @"update &DataSource&wms_b_container_h h set h.location_id=@location_id where h.t_id=@t_id
                             and h.client_id='&CLIENT_ID&' and h.org_id='&ORG_ID&' and h.warehouse_id = @warehouse_id ";
            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "location_id", locationId},
                    { "t_id",  cId},
                    { "warehouse_id",  Context.WarehouseID}
                  };

            DBHelper.RunSql(strSql, dicParam, "更新库存主档货位", DBHelper.OperatedNumType.OnlyOne);
        }

        /// <summary>
        /// 更新货位状态
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="status"></param>
        private void UpdateLocStatus(string locationId, string status)
        {
            string strSql = @"update &DataSource&wms_m_location l set l.location_status=@status where l.t_id=@t_id
                     and l.client_id='&CLIENT_ID&' and l.org_id='&ORG_ID&' and l.warehouse_id = @warehouse_id
                    ";
            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "status", status},
                    { "t_id",  locationId },
                    { "warehouse_id",  Context.WarehouseID}
                  };

            DBHelper.RunSql(strSql, dicParam, "更新货位状态", DBHelper.OperatedNumType.ZeroOrMore);
        }
        #endregion

        #region 完成订单箱回库        
        /// <summary>
        /// 完成订单箱回库
        /// </summary>
        /// <param name="taskId"></param>
        public void RFPickOrderBackOver(string taskId)
        {
            _workTask.UpdateTaskStatus(taskId, TaskStatus.TaskStatus_Finished.ToString());

            DataRow drTask = _workTask.GetWorkTask(taskId);
            DataRow[] drTaskItem = _workTaskItem.GetByTaskID(taskId);

            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "ref_doc_id",  drTask["ref_doc_id"].ToString() },
                    { "t_id",  drTask["t_id"].ToString() },
                    { "warehouse_id",  Context.WarehouseID }
                  };

            UpdateContainerLoc(drTask["container_id"].ToString(), drTask["dest_addr_id"].ToString());//更新库存对应的货位id


            foreach (var item in drTaskItem)
            {
                //更新任务明细
                UpdateTaskItem(item["t_id"].ToString());
            }

            UpdateLocStatus(drTask["dest_addr_id"].ToString(), LocationStatus.LocationStatus_Stored.ToString());//更新货位状态
            _workTask.MoveToHistory(taskId);
        }

        #endregion

        #region 回收余料回库任务
        /// <summary>
        /// 回收余料回库任务
        /// </summary>
        /// <param name="taskRow">任务表</param>
        public void RecyclePickSurpluBack(DataRow taskRow)
        {
            if (taskRow == null)
            {
                throw new WmsException(500, "查询不到任务");
            }

            if (taskRow["task_type"].ToString() == TaskType.TaskType_InBound.ToString())
            {
                UpdateLocStatus(taskRow["dest_addr_id"].ToString(), LocationStatus.LocationStatus_Empty.ToString());//更新货位状态
            }
            else if (taskRow["task_type"].ToString() == TaskType.TaskType_OutBound.ToString())
            {
                UpdateLocStatus(taskRow["src_addr_id"].ToString(), LocationStatus.LocationStatus_Stored.ToString());//更新货位状态   
            }

            _workTask.MoveToHistory(taskRow["t_id"].ToString());
        }

        #endregion

        #region 回收备料回库任务
        /// <summary>
        /// 回收备料回库任务
        /// </summary>
        /// <param name="taskRow">任务表</param>
        public void RecyclePickOrderBack(DataRow taskRow)
        {
            if (taskRow == null)
            {
                throw new WmsException(500, "查询不到任务");
            }

            if (taskRow["task_type"].ToString() == TaskType.TaskType_InBound.ToString())
            {
                UpdateLocStatus(taskRow["dest_addr_id"].ToString(), LocationStatus.LocationStatus_Empty.ToString());//更新货位状态
            }
            else if (taskRow["task_type"].ToString() == TaskType.TaskType_OutBound.ToString())
            {
                UpdateLocStatus(taskRow["src_addr_id"].ToString(), LocationStatus.LocationStatus_Stored.ToString());//更新货位状态   
            }

            _workTask.MoveToHistory(taskRow["t_id"].ToString());
        }

        #endregion


        // RF拣选-平库

        #region 外部-拣选-平库，获取需要拣选的出库单(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-平库，获取需要拣选的出库单(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <returns></returns>
        public WebJsonInfo PK_GetOutBoundInfo()
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                string sql = @"";
                string selectOne = "";
                List<string> OrderList = new List<string>();
                DataTable dt = null;

                sql = @" select   d.doc_no from  &DataSource&wms_b_task t 
                            inner join &DataSource&wms_b_task_item i on t.t_id = i.task_id
                            inner join  &DataSource&wms_b_wave_d w on w.t_id = i.ref_doc_d_id
                            inner join  &DataSource&wms_b_container_h ch on t.container_id=ch.t_id
                            inner join &DataSource&WMS_M_LOCATION l on l.t_id = ch.location_id
                            inner join &DataSource&wms_d_outbound_h d on d.t_id=w.outbound_h_id
                            where 
                            l.location_type='LocationType_FlatStock' 
                            and t.act_status=1 and i.act_status=1 and ch.act_status=1
                            and l.act_status=1 and d.act_status=1 
                            and t.task_status='"+TaskStatus.TaskStatus_WaitingExecute.ToString()+@"' 
                            group by d.doc_no
                            order by doc_no desc";
                param.Clear();
                dt = DBHelper.Select(sql, param, "查询所有拣选中的出库单", false);

                foreach (DataRow r in dt.Rows)
                {
                    string order = r["doc_no"] == null ? "" : r["doc_no"].ToString();
                    OrderList.Add(order);
                }
                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = OrderList };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 外部-拣选-平库，选定单据下拉框后，获取需要拣选的货位数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-平库，选定单据下拉框后，获取需要拣选的货位数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的用户选择的单据-Order</param>
        /// <returns></returns>
        public WebJsonInfo PK_PickLocationInfo(string Order)
        {
            try
            {
                string sql = @"";
                string selectOne = "";
                string orderID = "";
                List<object> list = new List<object>();
                DataTable dt = null;
                Dictionary<string, object> param = new Dictionary<string, object>();
                sql = @" select count(*) num from &DataSource&wms_d_outbound_h w where w.doc_no = @OutOrder";
                param.Clear();
                param.Add("OutOrder", Order);
                selectOne = DBHelper.SelectOne(sql, param, "查询单据编号是否存在", false);
                if (Convert.ToInt32(selectOne) == 0)
                {
                    throw new WmsException(500, "单据编号不存在");
                }
                if (Convert.ToInt32(selectOne) > 1)
                {
                    throw new WmsException(500, "单据编号有多个");
                }

                sql = @" select t_id from &DataSource&wms_d_outbound_h w where w.doc_no = @OutOrder";
                param.Clear();
                param.Add("OutOrder", Order);
                orderID = DBHelper.SelectOne(sql, param, "查询单据编号的ID", false);


                sql = @" select count(1) from &DataSource&wms_b_wave_d w where w.outbound_h_id = @orderID";
                param.Clear();
                param.Add("orderID", orderID);
                selectOne = DBHelper.SelectOne(sql, param, "查询是否是波次订单", false);

                if (Convert.ToInt32(selectOne) < 1) //无波次出库订单
                {
                    throw new WmsException(500, "出库单波次不存在");
                }
                else //有波次出库订单
                {
                    sql = @"select  t.task_no,l.LOCATION_CODE,sum(i.plan_qty) qtysum from &DataSource&wms_b_task t 
                            inner join &DataSource&wms_b_task_item i on t.t_id = i.task_id
                            inner join &DataSource&wms_b_wave_d w on w.t_id = i.ref_doc_d_id
                            inner join &DataSource&wms_b_container_h ch on t.container_id=ch.t_id
                            inner join &DataSource&WMS_M_LOCATION l on l.t_id = ch.location_id
                            where l.usage='LocationUse_OutboundBuffer' and t.task_status='TaskStatus_NotExecute' 
                            and  w.outbound_h_id = @orderID group by t.task_no,l.LOCATION_CODE";

                    param.Clear();
                    param.Add("orderID", orderID);
                }

                dt = DBHelper.Select(sql, param, "查询订单信息", false);
                foreach (DataRow r in dt.Rows)
                {
                    list.Add(new
                    {
                        task_no = r["task_no"] == null ? "" : r["task_no"].ToString(), //来源单号
                        locationcode = r["LOCATION_CODE"] == null ? "" : r["LOCATION_CODE"].ToString(), //物料编号
                        qtysum = r["qtysum"] == null ? "" : r["qtysum"].ToString(), //物料名称
                    });
                }

                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = list };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 外部-拣选-平库，选定单据下拉框后，获取需要拣选的容器数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-平库，选定单据下拉框后，获取需要拣选的容器数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的用户选择的单据-Order</param>
        /// <returns></returns>
        public WebJsonInfo PK_PickContainerInfo(string Order)
        {
            try
            {
                string sql = @"";
                string selectOne = "";
                string orderID = "";
                List<object> list = new List<object>();
                DataTable dt = null;
                Dictionary<string, object> param = new Dictionary<string, object>();
                sql = @" select count(*) num from &DataSource&wms_d_outbound_h w where w.doc_no = @OutOrder";
                param.Clear();
                param.Add("OutOrder", Order);
                selectOne = DBHelper.SelectOne(sql, param, "查询单据编号是否存在", false);
                if (Convert.ToInt32(selectOne) == 0)
                {
                    throw new WmsException(500, "单据编号不存在");
                }
                if (Convert.ToInt32(selectOne) > 1)
                {
                    throw new WmsException(500, "单据编号有多个");
                }

                sql = @" select t_id from &DataSource&wms_d_outbound_h w where w.doc_no = @OutOrder";
                param.Clear();
                param.Add("OutOrder", Order);
                orderID = DBHelper.SelectOne(sql, param, "查询单据编号的ID", false);


                sql = @" select count(1) from &DataSource&wms_b_wave_d w where w.outbound_h_id = @orderID";
                param.Clear();
                param.Add("orderID", orderID);
                selectOne = DBHelper.SelectOne(sql, param, "查询是否是波次订单", false);

                if (Convert.ToInt32(selectOne) < 1) //无波次出库订单
                {
                    throw new WmsException(500, "出库单波次不存在");
                }
                else //有波次出库订单
                {
                    sql = @"select  t.task_no,ch.container_barcode,sum(i.plan_qty) qtysum from &DataSource&wms_b_task t 
                            inner join &DataSource&wms_b_task_item i on t.t_id = i.task_id
                            inner join &DataSource&wms_b_wave_d w on w.t_id = i.ref_doc_d_id
                            inner join &DataSource&wms_b_container_h ch on t.container_id=ch.t_id
                            inner join &DataSource&WMS_M_LOCATION l on l.t_id = ch.location_id
                            where l.location_type='LocationType_FlatStock' 
                                and  w.outbound_h_id = @orderID 
                                and t.act_status=1 and i.act_status=1
                            group by t.task_no,ch.container_barcode";

                    param.Clear();
                    param.Add("orderID", orderID);
                }

                dt = DBHelper.Select(sql, param, "查询订单信息", false);
                foreach (DataRow r in dt.Rows)
                {
                    list.Add(new
                    {
                        task_no = r["task_no"] == null ? "" : r["task_no"].ToString(), //来源单号
                        containercode = r["container_barcode"] == null ? "" : r["container_barcode"].ToString(), //物料编号
                        qtysum = r["qtysum"] == null ? "" : r["qtysum"].ToString(), //物料名称
                    });
                }

                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = list };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 外部-拣选-平库，确定出库单，扫描容器或货位编码后，获取需要拣选的单行明细数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-平库，确定出库单，扫描容器或货位编码后，获取需要拣选的单行明细数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的货位编码-LocationCode或容器编码-ContainerCode、用户选择的单据-Order、页码-Page（默认为1）</param>
        /// <returns></returns>
        public WebJsonInfo PK_ContainerPickInfo(string LocationCode, string ContainerCode, string Order, int Page)
        {
            try
            {
                string sql = @"";
                string selectOne = "";
                string orderID = ""; //出库主档ID
                Dictionary<string, object> param = new Dictionary<string, object>();

                sql = @" select count(1) from &DataSource&wms_d_outbound_h w where w.doc_no = @OutOrder";
                param.Clear();
                param.Add("OutOrder", Order);
                selectOne = DBHelper.SelectOne(sql, param, "查询单据编号是否存在", false);

                if (Convert.ToInt32(selectOne) == 0)
                {
                    throw new WmsException(500, "单据编号不存在");
                }
                if (Convert.ToInt32(selectOne) > 1)
                {
                    throw new WmsException(500, "单据编号有多个");
                }

                sql = @" select t_id from &DataSource&wms_d_outbound_h w where w.doc_no = @OutOrder ";
                param.Clear();
                param.Add("OutOrder", Order);
                orderID = DBHelper.SelectOne(sql, param, "根据单号查询单据", false);

                sql = @" select count(1) from &DataSource&wms_b_wave_d w where w.outbound_h_id = @orderID";
                param.Clear();
                param.Add("orderID", orderID);
                selectOne = DBHelper.SelectOne(sql, param, "根据单号查询单据", false);

                if (Convert.ToInt32(selectOne) < 1) //无波次出库订单
                {
                    throw new WmsException(500, "无对应波次");
                }
                else //有波次出库订单
                {
                    if (!string.IsNullOrEmpty(LocationCode))  //按货位
                    {
                        sql = @"select th.task_no 任务编号,h.ref_doc_no 来源单号,t.item_code 物料编号,t.item_name 物料名称,d.lot_no 批次,(select sum(td.qty) from &DataSource&wms_b_container_d td where td.container_id = d.container_id and td.item_id = d.item_id) 物料数量,
                        ifnull(i.plan_qty, 0) 应拣,ifnull(i.processed_qty, 0) 已拣,i.t_id  from &DataSource&wms_b_task_item i inner join &DataSource&wms_b_wave_d w on i.ref_doc_d_id = w.t_id
                        inner join &DataSource&wms_d_outbound_h h
                        on w.OUTBOUND_H_ID = h.t_id
                        inner join &DataSource&wms_m_item t on t.t_id = i.item_id
                        inner join &DataSource&wms_b_container_d d on i.container_d_id = d.t_id
                        inner join &DataSource&wms_b_container_h c on c.t_id = d.container_id 
                        inner join &DataSource&wms_b_task th on th.t_id = i.task_id 
                        inner join &DataSource&wms_m_location l on l.t_id=c.location_id
                        where l.location_code = @LocationCode and h.doc_no = @OutOrder and rownum = @Page";

                        param.Clear();
                        param.Add("LocationCode", LocationCode);
                        param.Add("OutOrder", Order);
                        param.Add("Page", Page.ToString());
                    }
                    else //按托盘
                    {
                        sql = @"select th.task_no 任务编号,h.ref_doc_no 来源单号,t.item_code 物料编号,t.item_name 物料名称,d.lot_no 批次,(select sum(td.qty) from &DataSource&wms_b_container_d td where td.container_id = d.container_id and td.item_id = d.item_id) 物料数量,
                        ifnull(i.plan_qty, 0) 应拣,ifnull(i.processed_qty, 0) 已拣,i.t_id  from &DataSource&wms_b_task_item i inner join &DataSource&wms_b_wave_d w on i.ref_doc_d_id = w.t_id
                        inner join &DataSource&wms_d_outbound_h h
                        on w.OUTBOUND_H_ID = h.t_id
                        inner join &DataSource&wms_m_item t on t.t_id = i.item_id
                        inner join &DataSource&wms_b_container_d d on i.container_d_id = d.t_id
                        inner join &DataSource&wms_b_container_h c on c.t_id = d.container_id 
                        inner join &DataSource&wms_b_task th on th.t_id = i.task_id 
                        inner join &DataSource&wms_m_location l on l.t_id=c.location_id
                        where c.container_barcode = @ContainerCode and h.doc_no = @OutOrder and rownum = @Page";

                        param.Clear();
                        param.Add("ContainerCode", ContainerCode);
                        param.Add("OutOrder", Order);
                        param.Add("Page", Page.ToString());
                    }
                }

                DataTable dtRefDocNo = DBHelper.Select(sql, param, "根据货位查询页面需要的信息", false);

                if (dtRefDocNo.Rows.Count < 1)
                {
                    return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "没有更多数据", json = null };
                }
                else
                {
                    DataRow dr = dtRefDocNo.Rows[0];
                    var ResObj = new
                    {
                        任务编号 = dr["任务编号"] == null ? "" : dr["任务编号"].ToString(), //任务编号
                        来源单号 = dr["来源单号"] == null ? "" : dr["来源单号"].ToString(), //来源单号
                        物料编号 = dr["物料编号"] == null ? "" : dr["物料编号"].ToString(), //物料编号
                        物料名称 = dr["物料名称"] == null ? "" : dr["物料名称"].ToString(), //物料名称
                        批次 = dr["批次"] == null ? "" : dr["批次"].ToString(),     //批次
                        物料数量 = dr["物料数量"] == null ? "" : dr["物料数量"].ToString(), //物料数量
                        已拣 = dr["已拣"] == null ? "" : dr["已拣"].ToString(),     //已拣
                        应拣 = dr["应拣"] == null ? "" : dr["应拣"].ToString(),     //应拣
                        taskDetailID = dr["t_id"] == null ? "" : dr["t_id"].ToString(),      //任务明细ID
                        curPage = Page //当前页码
                    };
                    return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = ResObj };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 外部-拣选-平库，提交拣选数据 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-平库，提交拣选数据 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">model必填值：
        /// TaskDetailID 任务明细ID
        /// PickNum      拣选数量
        /// NeedPick     本次应拣数量
        /// BNList       箱码(有箱码则填，无箱码不填)
        /// </param>
        /// <returns></returns>
        public WebJsonInfo PK_PickSubmit(string TaskDetailID, int readyPickNum, int PickNum, int NeedPick, IList<string> BNList)
        {
            try
            {
                int o_result = 0;
                string o_msg = "";

                PickCommitForPK(TaskDetailID, readyPickNum, PickNum, NeedPick, BNList, out o_msg, out o_result);
                if (o_result == 1)
                {
                    return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = null };
                }
                else
                {
                    throw new WmsException(500, o_msg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 私有 拣选-平库，提交拣选数据 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 私有 拣选-平库，提交拣选数据 (波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">model必填值：
        /// TaskDetailID 任务明细ID
        /// PickNum      拣选数量
        /// NeedPick     本次应拣数量
        /// BNList       箱码(有箱码则填，无箱码不填)
        /// </param>
        /// <param name="o_msg">返回描述信息</param>
        /// <param name="o_result">1 成功，0 失败</param>
        private void PickCommitForPK(string TaskDetailID, int readyPickNum, int PickNum, int NeedPick, IList<string> BNList, out string o_msg, out int o_result)
        {
            try
            {
                o_msg = "";
                o_result = 0;
                string selectOne = "";
                string resmsg = "";
                int rescount = 0;
                DataTable dt = null;
                bool IsContainerPickOver = false;
                Dictionary<string, object> param = new Dictionary<string, object>();

                string v_ContainerCode;      //托盘条码
                string v_OutboundID;         //出库单ID
                string v_OutboundDetailID;   //出库单明细ID
                string v_WaveID = "";        //波次ID
                string v_WaveDetailID = "";  //波次明细ID
                string v_ConDetailID;        //库存明细ID
                string v_ConID;              //库存主表ID
                string v_TaskID;             //任务主档ID
                string v_TaskNo;             //主任务编号
                int v_ConDetailNum;          //托盘明细的剩余数量
                string v_Ref_doc_type;       //来源单据类型
                string v_DocNo;              //出库单号
                string allowLess;            //是否允许少拣 Yes、No

                /****************************基本数据赋值*******************************/

                string sql = @"select count(1) from &DataSource&wms_b_task_item w where w.t_id =@TaskDetailID ";
                param.Clear();
                param.Add("TaskDetailID", TaskDetailID);
                selectOne = DBHelper.SelectOne(sql, param, "查询拣选任务是否存在", false);
                if (Convert.ToInt32(selectOne) < 1)
                {
                    o_msg = "拣选任务不存在";
                    o_result = 0;
                    return;
                }
                if (Convert.ToInt32(selectOne) > 1)
                {
                    o_msg = "拣选任务存在多条";
                    o_result = 0;
                    return;
                }

                sql = @" select w.ref_doc_type from &DataSource&wms_b_task_item w where w.t_id = @TaskDetailID ";
                param.Clear();
                param.Add("TaskDetailID", TaskDetailID);
                v_Ref_doc_type = DBHelper.SelectOne(sql, param, "查询单据是否为波次出库单", false);

                sql = @"select w.ref_doc_id, w.ref_doc_d_id, w.container_d_id, w.task_id from &DataSource&wms_b_task_item w where w.t_id = @TaskDetailID ";
                param.Clear();
                param.Add("TaskDetailID", TaskDetailID);
                dt = DBHelper.Select(sql, param, "查询出库任务明细基本信息", false);

                if (v_Ref_doc_type == "DocType_Wave")  //波次出库(有波次)
                {
                    v_WaveID = dt.Rows[0]["ref_doc_id"] == null ? "" : dt.Rows[0]["ref_doc_id"].ToString();
                    v_WaveDetailID = dt.Rows[0]["ref_doc_d_id"] == null ? "" : dt.Rows[0]["ref_doc_d_id"].ToString();
                    v_ConDetailID = dt.Rows[0]["container_d_id"] == null ? "" : dt.Rows[0]["container_d_id"].ToString();
                    v_TaskID = dt.Rows[0]["task_id"] == null ? "" : dt.Rows[0]["task_id"].ToString();

                    sql = @"select count(1) from &DataSource&wms_b_wave_h h where h.t_id = @v_WaveID";
                    param.Clear();
                    param.Add("v_WaveID", v_WaveID);
                    selectOne = DBHelper.SelectOne(sql, param, "查询波次主表数据信息", false);
                    if (Convert.ToInt32(selectOne) < 1)
                    {
                        o_msg = "波次主表数据不存在";
                        o_result = 0;
                        return;
                    }
                    if (Convert.ToInt32(selectOne) > 1)
                    {
                        o_msg = "波次主表数据存在多条";
                        o_result = 0;
                        return;
                    }

                    sql = @"select count(1) from &DataSource&wms_b_wave_d h where h.t_id = @v_WaveDetailID";
                    param.Clear();
                    param.Add("v_WaveDetailID", v_WaveDetailID);
                    selectOne = DBHelper.SelectOne(sql, param, "查询波次明细数据信息", false);

                    if (Convert.ToInt32(selectOne) < 1)
                    {
                        o_msg = "波次明细数据不存在";
                        o_result = 0;
                        return;
                    }

                    sql = @" select h.outbound_h_id, h.outbound_d_id from &DataSource&wms_b_wave_d h where h.t_id = @v_WaveDetailID ";
                    param.Clear();
                    param.Add("v_WaveDetailID", v_WaveDetailID);
                    dt = DBHelper.Select(sql, param, "查询波次明细数据信息", false);

                    v_OutboundID = dt.Rows[0]["outbound_h_id"] == null ? "" : dt.Rows[0]["outbound_h_id"].ToString();
                    v_OutboundDetailID = dt.Rows[0]["outbound_d_id"] == null ? "" : dt.Rows[0]["outbound_d_id"].ToString();


                }
                else
                {
                    o_msg = "未知的单据类型";
                    o_result = 0;
                    return;
                }

                sql = @"select count(1) from &DataSource&wms_d_outbound_h h where h.t_id = @v_OutboundID ";
                param.Clear();
                param.Add("v_OutboundID", v_OutboundID);
                selectOne = DBHelper.SelectOne(sql, param, "查询出库主档信息", false);

                if (Convert.ToInt32(selectOne) < 1)
                {
                    o_msg = "出库主档不存在";
                    o_result = 0;
                    return;
                }

                sql = @" select h.doc_no from &DataSource&wms_d_outbound_h h where h.t_id = @v_OutboundID ";

                param.Clear();
                param.Add("v_OutboundID", v_OutboundID);
                dt = DBHelper.Select(sql, param, "查询出库主档信息", false);

                v_DocNo = dt.Rows[0]["doc_no"] == null ? "" : dt.Rows[0]["doc_no"].ToString();

                sql = @"select count(1) from &DataSource&wms_b_task t where t.t_id = @v_TaskID ";

                param.Clear();
                param.Add("v_TaskID", v_TaskID);
                selectOne = DBHelper.SelectOne(sql, param, "查询任务主档信息", false);

                if (Convert.ToInt32(selectOne) < 1)
                {
                    o_msg = "任务主档信息不存在";
                    o_result = 0;
                    return;
                }

                sql = @"select t.task_no from &DataSource&wms_b_task t where t.t_id = @v_TaskID";
                param.Clear();
                param.Add("v_TaskID", v_TaskID);
                dt = DBHelper.Select(sql, param, "根据ID查询任务号", false);

                v_TaskNo = dt.Rows[0]["task_no"] == null ? "" : dt.Rows[0]["task_no"].ToString();

                sql = @"select count(1) from &DataSource&wms_b_container_d d where d.t_id = @v_ConDetailID";
                param.Clear();
                param.Add("v_ConDetailID", v_ConDetailID);
                selectOne = DBHelper.SelectOne(sql, param, "根据ID查询拣选任务的库存明细是否存在", false);

                if (Convert.ToInt32(selectOne) < 1)
                {
                    o_msg = "拣选任务的库存明细不存在";
                    o_result = 0;
                    return;
                }

                sql = @"select d.qty, d.container_id  from &DataSource&wms_b_container_d d where d.t_id = @v_ConDetailID";
                param.Clear();
                param.Add("v_ConDetailID", v_ConDetailID);
                dt = DBHelper.Select(sql, param, "根据ID查询拣选任务的库存明细", false);

                v_ConDetailNum = Convert.ToInt32(dt.Rows[0]["qty"] == null ? "0" : dt.Rows[0]["qty"].ToString());
                v_ConID = dt.Rows[0]["container_id"] == null ? "" : dt.Rows[0]["container_id"].ToString();

                sql = @" select count(1) from &DataSource&wms_b_container_h h where h.t_id = @v_ConID";
                param.Clear();
                param.Add("v_ConID", v_ConID);
                selectOne = DBHelper.SelectOne(sql, param, "拣选任务的库存主档是否存在", false);

                if (Convert.ToInt32(selectOne) < 1)
                {
                    o_msg = "拣选任务的库存主档不存在";
                    o_result = 0;
                    return;
                }

                sql = @"  select h.container_barcode from &DataSource&wms_b_container_h h where h.t_id = @v_ConID";
                param.Clear();
                param.Add("v_ConID", v_ConID);
                dt = DBHelper.Select(sql, param, "拣选任务的库存主档", false);

                v_ContainerCode = dt.Rows[0]["container_barcode"] == null ? "0" : dt.Rows[0]["container_barcode"].ToString();

                /****************************判断模式和数量*******************************/

                sql = @"select Config_Value from &DataSource&wms_s_config w where w.config_key='ISRFAllowPickLess'";
                param.Clear();
                param.Add("v_ConID", v_ConID);
                allowLess = DBHelper.SelectOne(sql, param, "查询是否存在可少拣的配置信息", false);

                if (allowLess != "Yes" && allowLess != "No")
                {
                    o_result = 0;
                    o_msg = "未配置是否可以少拣(Yes/No)：Config_Key=ISRFAllowPickLess";
                    return;
                }

                if (PickNum != 0)
                {
                    if (NeedPick < (PickNum + readyPickNum))
                    {
                        o_result = 0;
                        o_msg = "不允许多拣";
                        return;
                    }
                    if (allowLess == "No")
                    {
                        if (NeedPick > (PickNum + readyPickNum))
                        {
                            o_msg = "策略不允许少拣";
                            o_result = 0;
                            return;
                        }
                    }
                }

                /*********************更新任务、单据、波次信息******************************/

                //更新拣选任务数量
                sql = @"update &DataSource&wms_b_task_item i set i.processed_qty    = ifnull(i.processed_qty, 0) + @PickNum1,
                    i.finished_qty     = ifnull(i.finished_qty, 0) + @PickNum2,
                    i.process_status   = 'ProcessStatus_Done',
                    i.modify_date      = to_date(@modify_date,'yy-mm-dd hh24:mi:ss'),
                    i.last_modified_by = @last_modified_by
                    where i.t_id = @t_id";
                param.Clear();
                param.Add("PickNum1", PickNum.ToString());
                param.Add("PickNum2", PickNum.ToString());
                param.Add("modify_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                param.Add("last_modified_by", Context.UserInfo.UserName);
                param.Add("t_id", TaskDetailID);
                rescount = DBHelper.RunSql(sql, param, "更新拣选任务数量", DBHelper.OperatedNumType.ZeroOrMore);


                if (rescount < 1)
                {
                    o_msg = "更新拣选任务数量出错" + resmsg;
                    o_result = 0;
                    return;
                }

                //更新出库单明细已处理数量
                sql = @"update &DataSource&wms_d_outbound_d d set d.processed_qty = ifnull(d.processed_qty, 0) + @PickNum where d.t_id = @v_OutboundDetailID";
                param.Clear();
                param.Add("PickNum", PickNum.ToString());
                param.Add("v_OutboundDetailID", v_OutboundDetailID);
                rescount = DBHelper.RunSql(sql, param, "更新拣选任务数量", DBHelper.OperatedNumType.ZeroOrMore);
                if (rescount < 1)
                {
                    o_msg = "更新出库单明细已处理数量出错" + resmsg;
                    o_result = 0;
                    return;
                }

                if (v_Ref_doc_type == "DocType_Wave")
                {
                    //有波次需要更新波次信息
                    sql = @" update &DataSource&wms_b_wave_d d set d.FINISHED_QTY= (ifnull(d.FINISHED_QTY, 0) + @PickNum) where d.t_id = @v_WaveDetailID";
                    param.Clear();
                    param.Add("PickNum", PickNum.ToString());
                    param.Add("v_WaveDetailID", v_WaveDetailID);
                    rescount = DBHelper.RunSql(sql, param, "更新拣选任务数量", DBHelper.OperatedNumType.ZeroOrMore);

                    if (rescount < 1)
                    {
                        o_msg = "更新波次已处理数量出错" + resmsg;
                        o_result = 0;
                        return;
                    }


                    sql = @"select count(1)  from &DataSource&wms_b_task_item d where d.task_id = @task_id  and d.process_status = 'ProcessStatus_New'";
                    param.Clear();
                    param.Add("task_id", v_TaskID);
                    selectOne = DBHelper.SelectOne(sql, param, "统计任务明细表的任务是否都已经完成", false);

                    if (Convert.ToInt32(selectOne) == 0)
                    {
                        //波次主表的波次明细任务都已经完成
                        sql = @" update &DataSource&wms_b_wave_h d set d.LEFT_TASK_QTY = 0,d.wave_status = 'WaveStatus_Finished',d.modify_date =to_date(@modify_date,'yy-mm-dd hh24:mi:ss'),
                          d.last_modified_by = @last_modified_by ,FINISH_DATE=to_date(@modify_date,'yy-mm-dd hh24:mi:ss') where d.t_id = @v_WaveID";
                        param.Clear();
                        param.Add("modify_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        param.Add("last_modified_by", Context.UserInfo.UserName);
                        param.Add("v_WaveID", v_WaveID);
                        rescount = DBHelper.RunSql(sql, param, "更新波次主表状态", DBHelper.OperatedNumType.ZeroOrMore);
                        if (rescount < 1)
                        {
                            o_msg = "更新波次主表状态出错:" + resmsg;
                            o_result = 0;
                            return;
                        }

                        //更新任务主表
                        sql = @" update &DataSource&wms_b_task t set t.TASK_STATUS = 'TaskStatus_Finished',t.END_DATE =to_date(@modify_date,'yy-mm-dd hh24:mi:ss'),
                          t.last_modified_by = @last_modified_by  where t.t_id = @task_id";
                        param.Clear();
                        param.Add("modify_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        param.Add("last_modified_by", Context.UserInfo.UserName);
                        param.Add("task_id", v_TaskID);
                        rescount = DBHelper.RunSql(sql, param, "更新任务主表状态", DBHelper.OperatedNumType.ZeroOrMore);
                        if (rescount < 1)
                        {
                            o_msg = "更新任务主表状态出错:" + resmsg;
                            o_result = 0;
                            return;
                        }

                        IsContainerPickOver = true;
                    }
                    else
                    {
                        //波次主表的波次明细任务尚未全部完成
                        sql = @" update &DataSource&wms_b_wave_h d set d.LEFT_TASK_QTY = @lefttaskqty,d.modify_date =to_date(@modify_date,'yy-mm-dd hh24:mi:ss'),
                          d.last_modified_by = @last_modified_by  where d.t_id = @v_WaveID";
                        param.Clear();
                        param.Add("lefttaskqty", selectOne);
                        param.Add("modify_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        param.Add("last_modified_by", Context.UserInfo.UserName);
                        param.Add("v_WaveID", v_WaveID);
                        rescount = DBHelper.RunSql(sql, param, "更新波次主表状态", DBHelper.OperatedNumType.ZeroOrMore);
                        if (rescount < 1)
                        {
                            o_msg = "更新波次主表状态出错:" + resmsg;
                            o_result = 0;
                            return;
                        }
                    }
                }

                //该拣选任务完成后，判断同出库明细单的拣选明细任务是否都已经全部完成

                sql = @"select count(1) from &DataSource&wms_b_task_item w where w.process_status = 'ProcessStatus_New'
                    and w.ref_doc_d_id = (select ref_doc_d_id
                    from &DataSource&wms_b_task_item where t_id = @TaskDetailID)";
                param.Clear();
                param.Add("TaskDetailID", TaskDetailID);
                selectOne = DBHelper.SelectOne(sql, param, "统计任务明细任务是否都已经完成", false);

                if (Convert.ToInt32(selectOne) < 1)
                {
                    //任务明细任务已全部完成
                    sql = @"update &DataSource&wms_d_outbound_d d set d.finished_qty = ifnull(d.processed_qty, 0)
                        where d.t_id = @v_OutboundDetailID";
                    param.Clear();
                    param.Add("v_OutboundDetailID", v_OutboundDetailID);
                    rescount = DBHelper.RunSql(sql, param, "更新出库单明细数量", DBHelper.OperatedNumType.ZeroOrMore);
                    if (rescount < 1)
                    {
                        o_msg = "更新出库单明细数量出错:" + resmsg;
                        o_result = 0;
                        return;
                    }

                    sql = @"select count(1) from &DataSource&wms_d_outbound_d d where (d.finished_qty is null  or d.finished_qty = 0 ) and d.outbound_h_id = @v_OutboundID";
                    param.Clear();
                    param.Add("v_OutboundID", v_OutboundID);
                    selectOne = DBHelper.SelectOne(sql, param, "更新出库单明细数量", false);

                    if (Convert.ToInt32(selectOne) < 1)
                    {
                        //全部达到计划量
                        sql = @"update &DataSource&wms_d_outbound_h h set h.outbound_status = 'OutDocStatus_Finished' where h.t_id = @v_OutboundID";
                        param.Clear();
                        param.Add("v_OutboundID", v_OutboundID);
                        rescount = DBHelper.RunSql(sql, param, "更新出库单明细数量", DBHelper.OperatedNumType.ZeroOrMore);

                        if (rescount < 1)
                        {
                            o_msg = "更新出库单状态出错:" + resmsg;
                            o_result = 0;
                            return;
                        }
                    }
                }

                /**************************扣减出库托盘库存、新增空托盘库存********************************/

                sql = @"update &DataSource&wms_b_container_d d set d.qty    = (ifnull(d.qty,0) - to_number(@PickNum1))
                        where d.t_id = @v_ConDetailID";
                param.Clear();
                param.Add("PickNum1", PickNum.ToString());
                param.Add("v_ConDetailID", v_ConDetailID);
                rescount = DBHelper.RunSql(sql, param, "扣减库存", DBHelper.OperatedNumType.ZeroOrMore);

                if (rescount < 1)
                {
                    o_msg = "扣减库存出错:" + resmsg;
                    o_result = 0;
                    return;
                }

                //删除库存为0的数据
                sql = @" select count(1) from &DataSource&wms_b_container_d d where d.t_id = @v_ConDetailID and d.qty = 0";
                param.Clear();
                param.Add("v_ConDetailID", v_ConDetailID);
                selectOne = DBHelper.SelectOne(sql, param, "判断空托盘主档是否存在", false);

                if (Convert.ToInt32(selectOne) > 0) // 存在为0的库存
                {
                    sql = @"update &DataSource&wms_b_container_d d set d.act_status = '0' where d.t_id = @v_ConDetailID";
                    param.Clear();
                    param.Add("v_ConDetailID", v_ConDetailID);
                    rescount = DBHelper.RunSql(sql, param, "删除为0库存", DBHelper.OperatedNumType.ZeroOrMore);

                    if (rescount < 1)
                    {
                        o_msg = "删除为0库存出错:" + resmsg;
                        o_result = 0;
                        return;
                    }
                }


                /********************************交换托盘模式*********************************/

                //平库无交换托盘模式

                /********************************箱码处理*********************************/

                if (BNList != null && BNList.Count > 0)
                {
                    string bnstr = string.Join(',', BNList);
                    sql = @"select count(1) from &DataSource&wms_b_sn_h where item_sn is not null
                       and ',' || @bnstr || ',' like '%,' || item_sn || ',%'
                       and store_status = '1'";
                    param.Clear();
                    param.Add("bnstr", bnstr);
                    selectOne = DBHelper.SelectOne(sql, param, "判断当前箱码数据是否存在", false);

                    if (Convert.ToInt32(selectOne) == 0) // 不存在当前箱码数据
                    {
                        o_msg = "不存在当前箱码数据";
                        o_result = 0;
                        return;
                    }

                    if (Convert.ToInt32(selectOne) != BNList.Count)
                    {
                        o_msg = "箱码计数和需处理计数不一致";
                        o_result = 0;
                        return;
                    }

                    string newbnlogid = SnowflakeID.Guid().ToString();
                    sql = @"select  '" + newbnlogid + @"' t_id, 
                             client_id,
                             org_id,
                             warehouse_id,
                             h.t_id sn_id, 
                             h.location_id from_location_id, 
                             '' from_location_no,
                             h.zone_name from_location_name,
                             '' to_location_id, 
                             '' to_location_no, 
                             '' to_location_name, 
                             '出库' action,
                             '立库RF拣选出库' reason, 
                             @v_DocNo doc_no,
                             @task_no task_no, 
                             @create_date create_date, 
                             @created_by created_by, 
                             modify_ip,
                             modify_date,
                             @last_modified_by last_modified_by,
                             1 act_status 
                             from &DataSource&wms_b_sn_h h
                             where h.item_sn is not null
                             and ',' || @bnstr || ',' like '%,' || h.item_sn || ',%'
                             and h.store_status = '1'";

                    param.Clear();
                    param.Add("newbnlogid", newbnlogid);
                    param.Add("v_DocNo", v_DocNo);
                    param.Add("v_TaskNo", v_TaskNo);
                    param.Add("create_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    param.Add("created_by", Context.UserInfo.UserName);
                    param.Add("last_modified_by", Context.UserInfo.UserName);
                    param.Add("bnstr", bnstr);

                    dt = DBHelper.Select(sql, param, "查询SN主表并创建插入数据", false);

                    sql = @"  insert into &DataSource&wms_b_sn_d
                          (t_id,
                           client_id,
                           org_id,
                           warehouse_id,
                           sn_id,
                           from_location_id,
                           from_location_no,
                           from_location_name,
                           to_location_id,
                           to_location_no,
                           to_location_name,
                           action,
                           reason,
                           doc_no,
                           task_no,
                           create_date,
                           created_by,
                           modify_ip,
                           modify_date,
                           last_modified_by,
                           act_status)
                           values(@t_id,
                           @client_id,
                           @org_id,
                           @warehouse_id,
                           @sn_id,
                           @from_location_id,
                           @from_location_no,
                           @from_location_name,
                           @to_location_id,
                           @to_location_no,
                           @to_location_name,
                           @action,
                           @reason,
                           @doc_no,
                           @task_no,
                           @create_date,
                           @created_by,
                           @modify_ip,
                           to_date(@modify_date,'yy-mm-dd hh24:mi:ss'),
                           @last_modified_by,
                           @act_status)";

                    param.Clear();
                    param.Add("t_id", dt.Rows[0]["t_id"] == null ? "" : dt.Rows[0]["t_id"].ToString());
                    param.Add("client_id", dt.Rows[0]["client_id"] == null ? "" : dt.Rows[0]["client_id"].ToString());
                    param.Add("org_id", dt.Rows[0]["org_id"] == null ? "" : dt.Rows[0]["org_id"].ToString());
                    param.Add("warehouse_id", dt.Rows[0]["warehouse_id"] == null ? "" : dt.Rows[0]["warehouse_id"].ToString());
                    param.Add("sn_id", dt.Rows[0]["sn_id"] == null ? "" : dt.Rows[0]["sn_id"].ToString());
                    param.Add("from_location_id", dt.Rows[0]["from_location_id"] == null ? "" : dt.Rows[0]["from_location_id"].ToString());
                    param.Add("from_location_no", dt.Rows[0]["from_location_no"] == null ? "" : dt.Rows[0]["from_location_no"].ToString());
                    param.Add("from_location_name", dt.Rows[0]["from_location_name"] == null ? "" : dt.Rows[0]["from_location_name"].ToString());
                    param.Add("to_location_id", dt.Rows[0]["to_location_id"] == null ? "" : dt.Rows[0]["to_location_id"].ToString());
                    param.Add("to_location_no", dt.Rows[0]["to_location_no"] == null ? "" : dt.Rows[0]["to_location_no"].ToString());
                    param.Add("to_location_name", dt.Rows[0]["to_location_name"] == null ? "" : dt.Rows[0]["to_location_name"].ToString());
                    param.Add("action", dt.Rows[0]["action"] == null ? "" : dt.Rows[0]["action"].ToString());
                    param.Add("reason", dt.Rows[0]["reason"] == null ? "" : dt.Rows[0]["reason"].ToString());
                    param.Add("doc_no", dt.Rows[0]["doc_no"] == null ? "" : dt.Rows[0]["doc_no"].ToString());
                    param.Add("task_no", dt.Rows[0]["task_no"] == null ? "" : dt.Rows[0]["task_no"].ToString());
                    param.Add("create_date", dt.Rows[0]["create_date"] == null ? "" : dt.Rows[0]["create_date"].ToString());
                    param.Add("created_by", dt.Rows[0]["created_by"] == null ? "" : dt.Rows[0]["created_by"].ToString());
                    param.Add("modify_ip", dt.Rows[0]["modify_ip"] == null ? "" : dt.Rows[0]["modify_ip"].ToString());
                    param.Add("modify_date", dt.Rows[0]["modify_date"] == null ? "" : dt.Rows[0]["modify_date"].ToString());
                    param.Add("last_modified_by", dt.Rows[0]["last_modified_by"] == null ? "" : dt.Rows[0]["last_modified_by"].ToString());
                    param.Add("act_status", dt.Rows[0]["act_status"] == null ? "" : dt.Rows[0]["act_status"].ToString());

                    rescount = DBHelper.RunSql(sql, param, "插入箱码出库明细流水行数并获取影响行数", DBHelper.OperatedNumType.ZeroOrMore);
                    if (rescount != BNList.Count)
                    {
                        o_msg = "插入箱码出库明细流水行数和需处理行数不一致:" + resmsg;
                        o_result = 0;
                        return;
                    }
                    sql = @"update &DataSource&wms_b_sn_h h set h.store_status = '0'  where h.item_sn is not null
                        and ',' || @bnstr || ',' like '%,' || h.item_sn || ',%' and h.store_status = '1'";

                    param.Clear();
                    param.Add("bnstr", bnstr);
                    rescount = DBHelper.RunSql(sql, param, "更新箱码出库明细并返回影响行数", DBHelper.OperatedNumType.ZeroOrMore);
                    if (rescount != BNList.Count)
                    {
                        o_msg = "更新箱码出库明细行数和需处理行数不一致:" + resmsg;
                        o_result = 0;
                        return;
                    }

                    LogHelper.LogInformation("平库-RF拣选成功:TaskDetailID=" + TaskDetailID + ", PickNum=" + PickNum + ", NeedPick=" + NeedPick + ",BNList" + string.Join(',', BNList));

                }

                if(IsContainerPickOver)
                {
                    _workTask.MoveToHistory(v_TaskID);
                }

                o_result = 1;
                o_msg = "拣选成功";
            }
            catch (Exception ex)
            {
                LogHelper.LogInformation("平库-RF拣选失败:TaskDetailID=" + TaskDetailID + ", PickNum=" + PickNum + ", NeedPick=" + NeedPick + ",BNList" + string.Join(',', BNList) + ",Err=" + ex.Message);
                o_msg = "发生错误:" + ex.Message;
                o_result = 0;
                throw ex;
            }
        }
        #endregion

        // RF拣选-[立库/平库]

        #region 外部-拣选-[立库/平库]，扫箱码获取箱码数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，扫箱码获取箱码数据(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="model">传入model的托盘号-BNStr</param>
        /// <returns></returns>
        public WebJsonInfo BNInfo(string BNStr)
        {
            try
            {
                string sql = @"";
                string selectOne = "";
                DataTable dt = null;
                Dictionary<string, object> param = new Dictionary<string, object>();

                sql = @" select count(1) from &DataSource&wms_b_sn_h w where w.item_sn = @BNStr";
                param.Clear();
                param.Add("BNStr", BNStr);
                selectOne = DBHelper.SelectOne(sql, param, "判断箱码是否存在", false);
                if (Convert.ToInt32(selectOne) == 0)
                {
                    throw new WmsException(500, "箱码数据不存在");
                }
                if (Convert.ToInt32(selectOne) > 1)
                {
                    throw new WmsException(500, "箱码数据有多个");
                }

                sql = @" select w.t_id,w.item_sn,w.item_id,w.item_name,w.qty from &DataSource&wms_b_sn_h w where w.item_sn = @BNStr";
                param.Clear();
                param.Add("BNStr", BNStr);
                dt = DBHelper.Select(sql, param, "查询箱码数据", false);

                if (dt.Rows.Count < 1)
                {
                    return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "箱码不存在", json = "" };
                }

                sql = @" select w.t_id,w.item_sn,w.item_id,w.item_name,w.qty,i.item_code from &DataSource&wms_b_sn_h w inner join &DataSource&WMS_M_ITEM i 
                         on w.ITEM_ID = i.T_ID where w.item_sn = @BNStr";
                param.Clear();
                param.Add("BNStr", BNStr);
                dt = DBHelper.Select(sql, param, "查询箱码数据", false);
                if (dt.Rows.Count < 1)
                {
                    return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "箱码对应的物料不存在", json = "" };
                }
                if (dt.Rows.Count > 1)
                {
                    return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "箱码对应的物料有多个", json = "" };
                }

                DataRow dr = dt.Rows[0];
                var ResObj = new
                {
                    t_id = dr["t_id"] == null ? "" : dr["t_id"].ToString(), //BN主档ID
                    item_sn = dr["item_sn"] == null ? "" : dr["item_sn"].ToString(), //BN编号
                    item_id = dr["item_id"] == null ? "" : dr["item_id"].ToString(), //物料ID
                    item_name = dr["item_name"] == null ? "" : dr["item_name"].ToString(), //物料名称
                    item_code = dr["item_code"] == null ? "" : dr["item_code"].ToString(), //物料编码
                    qty = dr["qty"] == null ? "" : dr["qty"].ToString(), //BN数量
                };
                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = ResObj };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 外部-拣选-[立库/平库]，拆分箱码(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// <summary>
        /// 外部-拣选-立库，拆分箱码(波次[有箱码、无箱码]、无波次[有箱码、无箱码] 通用)
        /// </summary>
        /// <param name="NeedPick">传入model箱码应拣选数量-NeedPick</param>
        /// <param name="MoreNum">传入model箱码拣选多余时多余的数量-MoreNum</param>
        /// <param name="BNStr">传入model的箱码编号-BNStr</param>
        /// <returns></returns>
        public WebJsonInfo BNSpilt(int NeedPick, int MoreNum, string BNStr)
        {
            try
            {
                string sql = @"";
                string selectOne = "";
                DataTable dt = null;
                Dictionary<string, object> param = new Dictionary<string, object>();

                sql = @" select count(1) from &DataSource&wms_b_sn_h w where w.item_sn = @BNStr";
                param.Clear();
                param.Add("BNStr", BNStr);
                selectOne = DBHelper.SelectOne(sql, param, "判断箱码是否存在", false);
                if (Convert.ToInt32(selectOne) == 0)
                {
                    throw new WmsException(500, "箱码数据不存在");
                }
                if (Convert.ToInt32(selectOne) > 1)
                {
                    throw new WmsException(500, "箱码数据有多个");
                }

                sql = @" select w.t_id,w.item_sn,w.item_id,w.item_name,w.qty from &DataSource&wms_b_sn_h w where w.item_sn = @BNStr";
                param.Clear();
                param.Add("BNStr", BNStr);
                dt = DBHelper.Select(sql, param, "查询箱码数据", false);

                if (dt.Rows.Count < 1)
                {
                    throw new WmsException(500, "箱码不存在");
                }

                sql = @" select w.t_id,w.item_sn,w.item_id,w.item_name,w.qty,i.item_code from &DataSource&wms_b_sn_h w inner join &DataSource&WMS_M_ITEM i 
                         on w.ITEM_ID = i.T_ID where w.item_sn = @BNStr";
                param.Clear();
                param.Add("BNStr", BNStr);
                dt = DBHelper.Select(sql, param, "查询箱码数据", false);
                if (dt.Rows.Count < 1)
                {
                    throw new WmsException(500, "箱码对应的物料不存在");
                }
                if (dt.Rows.Count > 1)
                {
                    throw new WmsException(500, "箱码对应的物料有多个");
                }

                DataRow dr = dt.Rows[0];
                int qty = dr["qty"] == null ? 0 : Convert.ToInt32(dr["qty"].ToString()); //BN数量
                if (NeedPick + MoreNum != qty)
                {
                    throw new WmsException(500, "传入的拆分数量与箱码数量不一致");
                }

                // ToDo: 拆分BNSN

                return new WebJsonInfo { errcode = 200, isSucceed = true, errmsg = "OK", json = null };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
