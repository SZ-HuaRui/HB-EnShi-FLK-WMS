using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.BaseData;
using NtiCloud.Service.WMS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.CountContainer
{
    public class CountContainerService : ICountContainerService
    {
        public CountContainerService(IServiceProvider _serviceProvider, ILogger<BaseDataService> logger, IConfiguration configuration, Func<string, IDBContextProvider> dbFactory)
        {
            BaseDataService baseDataService = new BaseDataService(_serviceProvider, logger, configuration, dbFactory);
        }

        #region 外部-盘点-立库，执行盘点单，生成盘点任务
        /// <summary>
        /// 外部-盘点-立库，执行盘点单，生成盘点任务
        /// </summary>
        /// <param name="model">model必填值：
        /// CountHeadID 盘点单ID
        /// </param>
        /// <returns></returns>
        public WebJsonInfo GenCountTask(CountContainerInput.CountContainerModel model)
        {
            WebJsonInfo info = new WebJsonInfo();

            if (string.IsNullOrWhiteSpace(model.CountHeadID))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "盘点单不能为空";
                info.isSucceed = false;
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<CountContainer>().GenCountTask(model.CountHeadID);

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
        #endregion 

        #region 外部-盘点-立库，盘点审核通过或不通过
        /// <summary>
        /// 外部-盘点-立库，盘点审核通过或不通过
        /// </summary>
        /// <param name="model">model必填值：
        /// CountHeadID 盘点单ID
        /// </param>
        /// <returns></returns>
        public WebJsonInfo PassCountAudit(CountContainerInput.CountContainerModel model)
        {
            WebJsonInfo info = new WebJsonInfo();


            if (string.IsNullOrWhiteSpace(model.CountHeadID))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "盘点单不能为空";
                info.isSucceed = false;
                return info;
            }
            
            if (string.IsNullOrEmpty(model.Audit))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "操作结果为空";
                info.isSucceed = false;
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<CountContainer>().PassCountAudit(model.CountHeadID,model.Audit);

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
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion 

        #region 外部-盘点-立库，删除盘点明细
        /// <summary>
        /// 外部-盘点-立库，删除盘点明细
        /// </summary>
        /// <param name="model">model必填值：
        /// CountDetailID 盘点明细ID
        /// </param>
        /// <returns></returns>
        public WebJsonInfo DelCountDetail(CountContainerInput.CountContainerModel model)
        {
            WebJsonInfo info = new WebJsonInfo();


            if (string.IsNullOrWhiteSpace(model.CountDetailID))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "盘点明细不能为空";
                info.isSucceed = false;
                return info;
            }
            

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<CountContainer>().DelCountDetail(model.CountDetailID);

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
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion

        #region 外部-盘点-立库，删除盘点明细
        /// <summary>
        /// 外部-盘点-立库，删除盘点明细
        /// </summary>
        /// <param name="model">model必填值：
        /// CountDetailID 盘点明细ID
        /// </param>
        /// <returns></returns>
        public WebJsonInfo AddCountDetail(CountContainerInput.CountContainerModel model)
        {
            WebJsonInfo info = new WebJsonInfo();

            if (string.IsNullOrEmpty(model.CountHeadID))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "盘点主档ID不能为空";
                info.isSucceed = false;
                return info;
            }

            if (model.DetailIDList==null || model.DetailIDList.Count<1)
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "盘点明细不能为空";
                info.isSucceed = false;
                return info;
            }


            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<CountContainer>().AddCountDetail(model.CountHeadID,model.DetailIDList);

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
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion


        #region RF盘点确认
        public WebJsonInfo ConfirmCountQty(CountContainerInput.CountContainerModel model)
        {
            WebJsonInfo info = new WebJsonInfo();


            if (string.IsNullOrWhiteSpace(model.CountDetailID))
            {
                // 出错
                info.isSucceed = false;
                info.errcode = 500;
                info.errmsg = "盘点明细不能为空";
                info.isSucceed = false;
                return info;
            }

            if (string.IsNullOrWhiteSpace(model.IsConErr))
            {
                // 出错
                info.isSucceed = false;
                info.errcode = 500;
                info.errmsg = "条码是否错误不能为空";
                info.isSucceed = false;
                return info;
            }

            //if (string.IsNullOrWhiteSpace(model.Remark))
            //{
            //    // 出错
            //    info.isSucceed = false;
            //    info.errcode = 500;
            //    info.errmsg = "备注不能为空";
            //    return info;
            //}

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<CountContainer>().ConfirmCountQty(model.CountDetailID, model.IsConErr, model.Remark, model.QtyCount);

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
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion

        #region 提交盘点到审核
        public WebJsonInfo SubmitCountAudit(CountContainerInput.CountContainerModel model)
        {
            WebJsonInfo info = new WebJsonInfo();


            if (string.IsNullOrWhiteSpace(model.CountHeadID))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "盘点ID不能为空";
                info.isSucceed = false;
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<CountContainer>().SubmitCountAudit(model.CountHeadID);

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
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion

        #region 获取盘点容器库存信息
        public WebJsonInfo CountGetConInfo(CountContainerInput.CountContainerModel model)
        {
            WebJsonInfo info = new WebJsonInfo();


            if (string.IsNullOrWhiteSpace(model.ContainerCode))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "托盘条码不能为空";
                info.isSucceed = false;
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<CountContainer>().CountGetConInfo(model.ContainerCode);

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
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion

        #region 外部-盘点-立库，RF快速盘点，提交产生盘点回库任务
        public WebJsonInfo RFSingleGenInCountTask(CountContainerInput.CountContainerModel model)
        {
            WebJsonInfo info = new WebJsonInfo();


            if (string.IsNullOrWhiteSpace(model.ContainerCode))
            {
                // 出错
                info.errcode = 500;
                info.errmsg = "托盘条码不能为空";
                info.isSucceed = false;
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                info = WmsFactory.Make<CountContainer>().RFSingleGenInCountTask(model.CountHeadID,model.ContainerCode);

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
                info.errmsg = ex.Message;
                info.isSucceed = false;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;
        }
        #endregion

    }
}
