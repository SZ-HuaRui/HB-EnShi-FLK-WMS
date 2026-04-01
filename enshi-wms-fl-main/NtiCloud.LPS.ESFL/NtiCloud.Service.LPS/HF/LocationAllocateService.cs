using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.HF
{
    public class LocationAllocateService : NtiCloud.Interface.WMS.HF.ILocationAllocateService
    {
        #region 公共方法
        private readonly IServiceProvider serviceProvider;
        private IConfiguration _configuration;
        private readonly Func<string, IDBContextProvider> _dbFactory;
        private readonly ILogger<LocationAllocateService> _logger;
        public LocationAllocateService(IServiceProvider _serviceProvider, IConfiguration configuration, Func<string,
        IDBContextProvider> dbFactory)
        {
            serviceProvider = _serviceProvider;
            _configuration = configuration;
            _dbFactory = dbFactory;
            LogHelper.Init(_logger);
            Context.Init(serviceProvider, _dbFactory);

        }
        #endregion

         /// <summary>
         /// 保存出库单信息
         /// </summary>
         /// <param name="outboundInfo"></param>
         /// <returns></returns>
        public WebJsonInfo UpdateOutOrder(EditOutboundIn outboundInfo)
        {
            WebJsonInfo info = new WebJsonInfo();
         
            try
            {
                DBHelper.BeginTransaction();
                bool re = WmsFactory.Make<LocationAllocate>().UpdateOutOrder(outboundInfo);
                info.isSucceed = true;
                info.errcode = 0;
                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }

        public WebJsonInfo SaveAndConfirmOutbound(EditOutboundIn outboundInfo) {
            WebJsonInfo info = new WebJsonInfo();

            try
            {
                DBHelper.BeginTransaction();
                bool re = WmsFactory.Make<LocationAllocate>().SaveAndConfirmOutbound(outboundInfo);
                info.isSucceed = true;
                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        /// <summary>
        /// 删除出库单
        /// </summary>
        /// <param name="outboundid"></param>
        /// <returns></returns>
        public WebJsonInfo DeleteOutboundOrder(string order_no)
        {
            WebJsonInfo info = new WebJsonInfo();

            try
            {
                DBHelper.BeginTransaction();
                bool re = WmsFactory.Make<LocationAllocate>().AdjustOrder("P_DeleteOutboundOrder@hfchk_link", order_no);
                info.isSucceed = true;
                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        /// <summary>
        /// 删除出库单明细
        /// </summary>
        /// <param name="orderdetailid"></param>
        /// <returns></returns>
        public WebJsonInfo DeleteOutboundOrderDetail(string orderdetailid)
        {
            WebJsonInfo info = new WebJsonInfo();

            try
            {
                DBHelper.BeginTransaction();
                bool re = WmsFactory.Make<LocationAllocate>().AdjustOrder("P_DELETEPRODUCTIVEORDERID@hfchk_link", orderdetailid);
                info.isSucceed = true;
                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }



        /// <summary>
        /// 取消货位分配
        /// </summary>
        /// <param name="order_no"></param>
        /// <returns></returns>
        public WebJsonInfo UnconfirmOutboundOrder(string order_no)
        {
            WebJsonInfo info = new WebJsonInfo();

            try
            {
                DBHelper.BeginTransaction();
                bool re = WmsFactory.Make<LocationAllocate>().AdjustOrder("P_UndoOutboundConfirm@hfchk_link", order_no);
                info.isSucceed = true;
                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }

        /// <summary>
        /// 确认货位分配
        /// </summary>
        ///<param name="order_no">出库单号</param>
        /// ///<param name="service_type">业务类型</param>
        /// <param name="BatchNum">总批次数</param>
        ///<param name="BatchExNum">已执行批次数</param>
        ///<param name="ExNum">本次执行批次数</param>
        ///<param name="BarCode_Flag">有码无码：0=有码，1=无码</param>
        [Description("确认货位分配", "", "")]
        public WebJsonInfo ConfirmLocationInfo2(ConfirmLocationIn orderinfo) {
            WebJsonInfo info = new WebJsonInfo();

            try
            {
                if (string.IsNullOrEmpty(orderinfo.order_no) || string.IsNullOrEmpty(orderinfo.service_type) || (orderinfo.isbarcode != "0" && orderinfo.isbarcode != "1"))
                {
                    throw new Exception("参数错误.");
                }

                if (orderinfo.batchnum < orderinfo.batchexnum + orderinfo.thisexnum)
                {
                    throw new Exception("超过可执行批次");
                }

                DBHelper.BeginTransaction();

                if (orderinfo.service_type == "Productive_Service" && orderinfo.isbarcode == "1" && orderinfo.thisexnum > 0 && orderinfo.batchnum - orderinfo.batchexnum >= orderinfo.thisexnum && orderinfo.batchnum != orderinfo.thisexnum)
                {
                       WmsFactory.Make<LocationAllocate>().ConfirmLocationInfo2(orderinfo.order_no, orderinfo.isbarcode, orderinfo.thisexnum);
                     
                }
                else
                {
                    WmsFactory.Make<LocationAllocate>().ConfirmOutourderInfo(orderinfo.order_no, orderinfo.isbarcode); 
                }
                info.isSucceed = true;
                DBHelper.EndTransaction();
            } 
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;

        }
         
        /// <summary>
        /// 出库单拆分：将有码与无码拆分成两个出库单
        /// </summary>
        /// <param name="order_no"></param>
        /// <returns></returns>
        public WebJsonInfo SplitOutbound(string order_no) {
            WebJsonInfo info = new WebJsonInfo();

            try
            {
                DBHelper.BeginTransaction();
                bool re = WmsFactory.Make<LocationAllocate>().SplitOutbound(order_no);
                info.isSucceed = true;
                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        /// <summary>
        /// 取消出库
        /// </summary>
        /// <param name="order_no">出库单号</param>
        /// <returns></returns>
        public WebJsonInfo UndoOutbound(string order_no)
        {
            WebJsonInfo info = new WebJsonInfo();

            try
            {
                DBHelper.BeginTransaction();
                bool re = WmsFactory.Make<LocationAllocate>().AdjustOrder("P_UndoOutbound@hfchk_link", order_no);
                info.isSucceed = true;
                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }

        public WebJsonInfo TestSP() {
            WebJsonInfo info = new WebJsonInfo();
            try
            { 
                  
              DataSet ds=  WmsFactory.Make<WMS.HF.HFInventory>().GetData("T20200804000002", "Inturn_Service");
                info.isSucceed = true;
                info.json = Newtonsoft.Json.JsonConvert.SerializeObject( ds );
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
                
            }
            return info;
        }
        /// <summary>
        /// 同步ERP
        /// </summary>
        /// <param name="lstOrderno"></param>
        /// <returns></returns>
        public WebJsonInfo SyncData(List<SysErpIn> lstOrderno)
        {
            WebJsonInfo info = new WebJsonInfo();
            try
            {
                WmsFactory.Make<WMS.HF.HFInventory>().SyncData(lstOrderno,false);
                info.isSucceed = true;
                info.errcode = 0;
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);

            }
            return info;
        }
        /// <summary>
        /// 撤销同步ERP
        /// </summary>
        /// <param name="lstOrderno"></param>
        /// <returns></returns>
        public WebJsonInfo DeleteSyncOrder(List<SysErpIn> lstOrderno)
        {
            WebJsonInfo info = new WebJsonInfo();
            try
            {
                WmsFactory.Make<WMS.HF.HFInventory>().SyncData(lstOrderno, true);
                info.isSucceed = true;
                info.errcode = 0;
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);

            }
            return info;
        }

        /// <summary>
        /// 按出库单生成新的烟包条码
        /// </summary>
        /// <param name="order_no"></param>
        /// <returns></returns>
        public WebJsonInfo MakeNewBoxNumberByOrder(string order_no)
        {
            WebJsonInfo info = new WebJsonInfo();
            try
            {
                string newBatchId=WmsFactory.Make<WMS.HF.LocationAllocate>().MakeNewBoxNumberByOrder(order_no);
                info.isSucceed = true;
                info.json=new { NewBatchId= newBatchId };
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);

            }
            return info;
        } 

        /// <summary>
        /// 更新出库单打印状态
        /// </summary>
        /// <param name="order_nos"></param>
        /// <returns></returns>
        public WebJsonInfo UpdatePrintStatus(string order_nos)
        {
            WebJsonInfo info = new WebJsonInfo();
            try
            {
                bool re = WmsFactory.Make<LocationAllocate>().UpdatePrintStatus(order_nos);
                info.isSucceed = true;
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);

            }
            return info;
        }
        /// <summary>
        /// 生成分组打印数据并插入到临时表
        /// </summary>
        /// <param name="searchinfo"></param>
        /// <returns></returns>
        public WebJsonInfo GenerateDetailGroupData(OutboundDetailGroupIn searchinfo)
        {
            WebJsonInfo info = new WebJsonInfo();
            try
            {
                string  batchid = WmsFactory.Make<LocationAllocate>().GenerateDetailGroupData(searchinfo);
                info.isSucceed = true;
                info.json = new { BrachId = batchid };
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);

            }
            return info;
        }
        /// <summary>
        /// 按出库单+项次生成条码
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <returns></returns>
        public WebJsonInfo MakeNewBoxNumberByOrderItem(MakeNewbarckdeIn applyInfo)
        {
            WebJsonInfo info = new WebJsonInfo();
            try
            {
                //_logger.Log(LogLevel.Information, "产生新条码:开始");
                string batchid = WmsFactory.Make<LocationAllocate>().MakeNewBoxNumberByOrder(applyInfo);
                //_logger.Log(LogLevel.Information, "产生新条码:结束");
                info.isSucceed = true;
                info.json = new { BrachId = batchid };
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);

            }
            return info;
        }
        /// <summary>
        /// 删除已扫描条码重新扫描
        /// </summary>
        /// <param name="orderdetail"></param>
        /// <returns></returns>
        public WebJsonInfo RestoreScanedBarcode(RestoreScanedBarcodeIn orderdetail)
        {
            WebJsonInfo info = new WebJsonInfo();
            Model.UserLogModel logModel = new Model.UserLogModel();
            logModel.ModifyIP = Context.UserInfo.LoginIP;
            logModel.TypeCode = "删除已扫描条码重新扫描";
            logModel.LogTextLong = "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(orderdetail);
            logModel.CommonName = "/api/wms/HF/LocationAllocate/RestoreScanedBarcode";
            logModel.CriticalLevel = (decimal)Base.CriticalLevel.CriticalLevel_Info;
            try
            {
                WmsFactory.Make<LocationAllocate>().RestoreScanedBarcode(orderdetail.OrderNo,
                    orderdetail.OrderDetailId);
                info.isSucceed = true;
                logModel.LogText = "删除已扫描条码重新扫描:成功";
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);

                logModel.CriticalLevel = (decimal)Base.CriticalLevel.CriticalLevel_Error;
                logModel.LogText = "完成任务:失败";
            }
            logModel.LogTextLong += ";返回:" + Newtonsoft.Json.JsonConvert.SerializeObject(info);
            Base.UserLog.Add(logModel); 
            return info;
        }
        /// <summary>
        /// 按出库单明细ID生成条码
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <returns></returns>
        public WebJsonInfo MakeNewBoxNumberByOrderDetailId(MakeNewbarckde2In applyInfo)
        {
            WebJsonInfo info = new WebJsonInfo();
            try
            {
                //_logger.Log(LogLevel.Information, "按出库单明细ID生成条码:开始");
                string batchid = WmsFactory.Make<LocationAllocate>().MakeNewBoxNumberByOrder(applyInfo);
                //_logger.Log(LogLevel.Information, "按出库单明细ID生成条码:结束");
                info.isSucceed = true;
                info.json = new { BrachId = batchid };
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
            }
            return info;
        }

        public WebJsonInfo MakeNewBoxNumberBydetailIds(List<MakeNewbarckde2In> lsyApplyInfo)
        {
            WebJsonInfo info = new WebJsonInfo();
            try
            {
                //_logger.Log(LogLevel.Information, "按出库单明细ID生成条码:开始");              
                string batchid=WmsFactory.Make<LocationAllocate>().MakeNewBoxNumberByOrder(lsyApplyInfo);
                //_logger.Log(LogLevel.Information, "按出库单明细ID生成条码:结束");
                info.isSucceed = true;
                info.json = new { BrachId = batchid };
            }
            catch (Exception ex)
            {
                WmsException e = ex as WmsException;
                if (e != null && e.ErrorCode != 0)
                {
                    info.errcode = e.ErrorCode;
                }
                else if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
            }
            return info;
        }
    }
}
