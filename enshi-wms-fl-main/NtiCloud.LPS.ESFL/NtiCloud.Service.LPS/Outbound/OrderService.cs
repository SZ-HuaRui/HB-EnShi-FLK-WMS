using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NtiCloud.Interface.Core;
using NtiCloud.Interface.WMS;
using NtiCloud.Kernel;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace NtiCloud.Service.WMS.Outbound
{
    public class OrderService : IOrderService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Func<string, IDBContextProvider> _dbFactory;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IServiceProvider provider,
            ILogger<OrderService> logger,
            Func<string, IDBContextProvider> dbFactory)
        {
            serviceProvider = provider;
            _logger = logger;
            _dbFactory = dbFactory;
            LogHelper.Init(_logger);
            Context.Init(serviceProvider, _dbFactory);
        }

        public WebJsonInfo AddDetailByBom(OutboundModel.AddDetailByBomParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"出库单{p.OrderNoOrID}根据配方单添加明细",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "根据配方单添加出库明细",
                CommonName = "api/wms/Order/AddDetailByBom",
            },
            () => WmsFactory.Make<OutboundOrder>().AddDetailByBom(p.OrderNoOrID, p.BomID, p.Times));
        }

        public WebJsonInfo PickOut(OutboundModel.PickOutParam p)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数
            if (string.IsNullOrWhiteSpace(p.OrderID)
                || string.IsNullOrWhiteSpace(p.ContainerBarcode)
                || string.IsNullOrWhiteSpace(p.ItemID)
                || string.IsNullOrWhiteSpace(p.LocationNo)
                || p.Qty <= 0
                )
            {
                // 出错
                info.errcode = 500;
                info.isSucceed = false;
                info.errmsg = "参数错误，停止处理";
                Console.WriteLine(info.errmsg);
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                new OutboundOrder().PickOut(p.OrderID,
                    p.ItemID, p.LocationNo, p.ContainerBarcode, p.Qty);

                DBHelper.EndTransaction();
            }
            catch (Exception ex)
            {
                if (info.errcode == 0)
                {
                    info.errcode = 500;
                }
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                DBHelper.EndTransaction(false);
            }
            return info;
        }

        public WebJsonInfo AllocateInventory(OutboundModel.PreallocateInventoryParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"出库单{p.OrderNoOrID}分配库存",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "分配库存",
                CommonName = "api/wms/Order/AllocateInventory",
            },
            () => WmsFactory.Make<OutboundOrder>().AllocateInventory(p.OrderNoOrID));
        }

        public WebJsonInfo PreallocateInventory(OutboundModel.PreallocateInventoryParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"出库单{p.OrderNoOrID}预分配库存",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "预分配库存",
                CommonName = "api/wms/Order/PreallocateInventory",
            },
            () => WmsFactory.Make<OutboundOrder>().PreallocateInventoryWithWave(p.OrderNoOrID));
        }

        public WebJsonInfo CancelInventoryPreallocation(OutboundModel.CancelInventoryPreallocationParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"出库单{p.OrderNoOrID}取消预分配库存",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "取消预分配库存",
                CommonName = "api/wms/Order/CancelInventoryPreallocation",
            },
            () => WmsFactory.Make<OutboundOrder>().CancelInventoryPreallocation(p.OrderNoOrID));
        }

        public WebJsonInfo ConfirmPreallocation(OutboundModel.ConfirmPreallocationParam p)
        {
            string errmsg = "OK";
            WebJsonInfo retInfo = WmsHelper.Do(new UserLogModel()
            {
                LogText = $"出库单{p.OrderNoOrID}确认预分配库存",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "确认预分配库存",
                CommonName = "api/wms/Order/ConfirmPreallocation",
            },
            () =>
            {
                WmsFactory.Make<OutboundOrder>().ConfirmPreallocation(p.OrderNoOrID, out WebJsonInfo info);
                if (info.errmsg != errmsg)
                {
                    errmsg = "确认已成功，但发送报文到WCS有错误发生：" + info.errmsg;
                }
            });
            if (errmsg != "OK")
            {
                retInfo.errmsg = errmsg;
            }
            return retInfo;
        }

        public WebJsonInfo CancelConfirmPreallocation(OutboundModel.CancelConfirmPreallocationParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"出库单{p.OrderNoOrID}取消预分配库存",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "取消确认预分配库存",
                CommonName = "api/wms/Order/CancelConfirmPreallocation",
            },
            () => WmsFactory.Make<OutboundOrder>().CancelConfirmPreallocation(p.OrderNoOrID));
        }
        public WebJsonInfo CloseOrder(OutboundModel.CloseOrderParam p)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数
            if (string.IsNullOrWhiteSpace(p.OrderNoOrID))
            {
                // 出错
                info.errcode = 500;
                info.isSucceed = false;
                info.errmsg = "参数不能为空，停止处理";
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                WmsFactory.Make<OutboundOrder>().CloseOrder(p.OrderNoOrID);

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
                info.isSucceed = false;
                info.errmsg = ex.Message;
                LogHelper.LogException(ex);
                DBHelper.EndTransaction(false);
            }
            return info;

        }

        public WebJsonInfo CreateWave(OutboundModel.CreateWaveParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"创建波次{p.WaveNo}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "创建波次",
                CommonName = "api/wms/Order/CreateWave",
            },
            () => WmsFactory.Make<Wave>().CreateWave(p.WaveNo, p.OrderIds));
        }

        public WebJsonInfo AppendWave(OutboundModel.AppendWaveParam p)
        {
            throw new NotImplementedException();
        }

        public WebJsonInfo SplitWaveDetail(OutboundModel.SplitWaveDetailParam p)
        {
            throw new NotImplementedException();
        }

        public WebJsonInfo MergeWaveDetail(OutboundModel.MergeWaveDetailParam p)
        {
            throw new NotImplementedException();
        }


        public WebJsonInfo AddOutboundTask(OutboundModel.AddOutboundTaskParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"手工增加预分配信息{p.LocationNoOrID}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "手工预分配库存",
                CommonName = "api/wms/Order/AddOutboundTask",
            },
            () => WmsFactory.Make<OutboundOrder>().AddOutboundTask(p.OrderDetailID, p.LocationNoOrID, p.Qty));
        }

        public WebJsonInfo ModifyOutboundTask(OutboundModel.ModifyOutboundTaskParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"手工修改预分配信息{p.TaskNoOrID}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "手工修改预分配库存",
                CommonName = "api/wms/Order/ModifyOutboundTask",
            },
            () => WmsFactory.Make<OutboundOrder>().ModifyOutboundTask(p.TaskNoOrID, p.Qty, p.LocationID));
        }

        public WebJsonInfo ModifyOutboundTaskItem(OutboundModel.ModifyOutboundTaskItemParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"手工修改预分配明细信息{p.TaskItemID}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "手工修改预分配库存",
                CommonName = "api/wms/Order/ModifyOutboundTaskItem",
            },
            () => WmsFactory.Make<OutboundOrder>().ModifyOutboundTaskItem(p.TaskItemID, p.Qty, p.LocationID));
        }

        public WebJsonInfo ModifyWaveTask(OutboundModel.ModifyWaveTaskParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"手工修改波次任务{p.TaskNoOrID}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "手工修改波次任务",
                CommonName = "api/wms/Order/ModifyWaveTask",
            },
            () => WmsFactory.Make<Wave>().ModifyWaveTask(p.TaskNoOrID, p.Qty, p.LocationID));
        }

        public WebJsonInfo PrecomputeWave(OutboundModel.PrecomputeWaveParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"预计算波次{p.WaveHeadNoOrID}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "预计算波次",
                CommonName = "api/wms/Order/PrecomputeWave",
            },
            () => WmsFactory.Make<Wave>().PrecomputeWave(p.WaveHeadNoOrID, p.Target, p.LocationGroup));
        }

        public WebJsonInfo ComputeWave(OutboundModel.ComputeWaveParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = "计算波次" + p.WaveHeadNoOrID,
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "波次运算",
                CommonName = "api/wms/Order/ComputeWave",
            }, () => WmsFactory.Make<Wave>().ComputeWave(p.WaveHeadNoOrID, p.Type));
        }

        public WebJsonInfo ComputeWaveDetail(OutboundModel.ComputeWaveDetailParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"计算波次明细{p.DetailID}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "对波次明细计算",
                CommonName = "api/wms/Order/ComputeWaveDetail",
            },
            () => WmsFactory.Make<Wave>().ComputeWaveDetail(p.DetailID, p.Room));
        }

        public WebJsonInfo ReleaseWave(OutboundModel.ReleaseWaveParam p)
        {
            string errmsg = string.Empty;
            WebJsonInfo retInfo = WmsHelper.Do(new UserLogModel()
            {
                LogText = $"释放波次{p.WaveHeadNoOrID}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "释放波次",
                CommonName = "api/wms/Order/ReleaseWave",
            },
            () =>
            {
                WmsFactory.Make<Wave>().ReleaseWave(p.WaveHeadNoOrID, p.StockUpLocID, out WebJsonInfo info);
                errmsg = info.errmsg;
            });
            if (errmsg != "OK")
            {
                retInfo.errmsg = errmsg;
            }
            return retInfo;
        }

        public WebJsonInfo DeleteWave(OutboundModel.DeleteWaveParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"删除波次{p.WaveHeadNoOrID}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "删除波次",
                CommonName = "api/wms/Order/DeleteWave",
            },
            () => WmsFactory.Make<Wave>().DeleteWave(p.WaveHeadNoOrID));
        }

        public WebJsonInfo FinishWave(OutboundModel.FinishWaveParam p)

        {
            throw new NotImplementedException();
        }

        public WebJsonInfo CancelPrellocation(OutboundModel.CancelPrellocationParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"取消预分配{p.WaveHeadNoOrID}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "取消预分配",
                CommonName = "api/wms/Order/CancelPrellocation",
            },
            () => WmsFactory.Make<Wave>().CancelPreallocation(p.WaveHeadNoOrID));
        }

        public WebJsonInfo SaveBarcodeNumber(OutboundModel.SaveBarcodeNumberParam p)
        {
            WebJsonInfo info = new WebJsonInfo();

            // 检查参数
            if (string.IsNullOrWhiteSpace(p.WaveNo) || string.IsNullOrWhiteSpace(p.JsonItemList))
            {
                // 出错
                info.errcode = 500;
                info.isSucceed = false;
                info.errmsg = "参数不能为空，停止处理";
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                WmsFactory.Make<Wave>().SaveBarcodeNumber(p.WaveNo, p.JsonItemList);

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

        public WebJsonInfo RemoveWave(OutboundModel.RemoveWaveParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"移除波次{p.WaveHeadNoOrID}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "移除波次",
                CommonName = "api/wms/Order/RemoveWave",
            },
            () => WmsFactory.Make<Wave>().RemoveWave(p.WaveHeadNoOrID));
        }

        public WebJsonInfo RemoveWaveDetails(OutboundModel.RemoveWaveDetailsParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"移除波次明细{p.DetailIds}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "移除波次明细",
                CommonName = "api/wms/Order/RemoveWaveDetails",
            },
            () => WmsFactory.Make<Wave>().RemoveWaveDetails(p.DetailIds));
        }

        public WebJsonInfo RemoveOutboundTasks(OutboundModel.RemoveOutboundTasksParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"移除预分配信息{p.TaskIds}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "移除预分配信息",
                CommonName = "api/wms/Order/RemoveOutboundTasks",
            },
            () => WmsFactory.Make<OutboundOrder>().RemoveOutboundTasks(p.TaskIds));
        }

        public WebJsonInfo RemoveOutboundTaskItems(OutboundModel.RemoveOutboundTaskItemsParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"移除预分配明细信息{p.TaskItemIds}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "移除预分配明细信息",
                CommonName = "api/wms/Order/RemoveOutboundTaskItems",
            },
            () => WmsFactory.Make<OutboundOrder>().RemoveOutboundTaskItems(p.TaskItemIds));
        }

        public WebJsonInfo RemoveWaveTasks(OutboundModel.RemoveWaveTasksParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"移除波次任务{p.WaveTaskIds}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "移除波次任务",
                CommonName = "api/wms/Order/RemoveWaveTasks",
            },
            () => WmsFactory.Make<Wave>().RemoveWaveTasks(p.WaveTaskIds));
        }

        public WebJsonInfo AddWaveTask(OutboundModel.AddWaveTaskParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"手工分配波次任务{p.TaskNo}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "手工分配波次任务",
                CommonName = "api/wms/Order/AddWaveTask",
            },
            () => WmsFactory.Make<Wave>().AddWaveTask(p.TaskNo,
                p.GroupNo, p.SeqNo, p.Priority, p.WaveDetailID, p.Qty, p.LocationNoOrID));
        }

        public WebJsonInfo CancelRelease(OutboundModel.CancelReleaseParam p)
        {
            return WmsHelper.Do(new UserLogModel()
            {
                LogText = $"取消释放波次{p.WaveHeadNoOrID}",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(p),
                TypeCode = "取消释放波次",
                CommonName = "api/wms/Order/CancelRelease",
            },
            () => WmsFactory.Make<Wave>().CancelRelease(p.WaveHeadNoOrID));
        }

        public WebJsonInfo TransferOwnership(OutboundModel.TransferOwnershipParam p)
        {
            throw new NotImplementedException();
        }

        public WebJsonInfo CreateShippingOrder(OutboundModel.CreateShippingOrderParam p)
        {
            throw new NotImplementedException();
        }

        public WebJsonInfo OutboundTest(OutboundModel.CreateShippingOrderParam p)
        {
            WebJsonInfo info = new WebJsonInfo();

            /*
            var userMode = serviceProvider.GetService<IUserService>().GetUserMode();
            var userLoginInfo = userMode.userLoginInfo;
            var dbContext = _dbFactory(userMode.userLoginInfo.UserDbConnR.dbType);

            string appCode = string.Empty;

            var tokens = serviceProvider.GetService<IUserService>().GetToken();
            if (tokens.token.ContainsKey("appCode"))
            {
                appCode = tokens.token["appCode"];
            }

            string sql = $"select t_id from &DataSource&swms_wi_warehouse where Code = @appCode ";

            var list = new List<Dictionary<string, object>>() {
                    new Dictionary<string, object>
                    {
                        { "appCode", appCode }
                    }
            };

            GloballyDBChange globallyDB = serviceProvider.GetService<IKernelGloballyVariable>().
                Globally_SQL(tokens, sql);
            sql = globallyDB.sql;
            userMode = globallyDB.userMode;

            DataTable dt = dbContext.DBSelectAll(userMode.userLoginInfo, sql, list).Tables[0];
            if (dt.Columns.Count == 1 && dt.Rows.Count == 1 && dt.Columns[0].ColumnName == "error")
            {
                throw new WmsException(500, $"{dt.Rows[0][0].ToString()}!");
            }

            if (dt.Rows.Count == 0)
            {
                throw new WmsException(500, $"记录未找到!");
            }
            */
            // 检查参数
            if (string.IsNullOrWhiteSpace(p.Json))
            {
                // 出错
                info.errcode = 500;
                info.isSucceed = false;
                info.errmsg = "参数不能为空，停止处理";
                return info;
            }

            try
            {
                DBHelper.BeginTransaction();

                //info = new OutboundTest().ProcessTest(p.Json);

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

    }
}
