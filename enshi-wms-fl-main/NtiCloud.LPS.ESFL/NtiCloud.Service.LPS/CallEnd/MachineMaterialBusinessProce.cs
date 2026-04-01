using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.LPS.Inbound;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Inventory;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Service.WMS.Tasking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using static NtiCloud.Model.WMS.MachineMaterialInput;



namespace NtiCloud.Service.WMS.CallEnd
{
    public class MachineMaterialBusinessProce : WmsObject
    {
        NtiCloud.Service.WMS.Base.CallEnd ce = new Base.CallEnd();

        #region 机台要料
        /// <summary>
        /// 机台要料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void CreateOutBoundByMachineDock(MachineMaterialModel model)
        {
            if (ce.MachineHaveDockInfo(model.Machine, model.Dock) == false)
            {
                throw new WmsException(1, "机台上没有该站台，请重新登录账号！");
            }
            CreateOutBound(model, null);
        }

        #region 物料出库
        /// <summary>
        /// 物料出库
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dependentTask">依赖任务信息</param>
        private void CreateOutBound(MachineMaterialModel model, Model.WorkTaskModel dependentTask)
        {
            #region 1.检查站台当前任务 
            //查询站台未执行完成的任务数量
            DataRow dtTaskNumInfo = ce.SelectTaskNumByDock(model.Dock);
            int cnt = Convert.ToInt32(dtTaskNumInfo["cnt"].ToString());//站台未执行完成的任务数量
            if (cnt > 0 && dependentTask == null)
            {
                throw new WmsException(500, "该站台上有未执行完成的任务，无法要料");
            }
            #endregion

            #region 2.获取机台信息
            DataRow _drMachineInfo = ce.GetMachineInfoByMachineId(model.Machine);
            //机台所在区域
            string machineZoneId = _drMachineInfo["zone_id"].ToString();
            //设备用途
            string equipmentUsage = _drMachineInfo["equipment_usage"].ToString();
            #endregion

            #region 3.获取站台信息
            //获取站台信息
            DataRow dtDockInfo = ce.GetDockStatus(model.Dock);
            string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            string container_id = dtDockInfo["container_id"].ToString();//挂在站台上的容器主档id
            string dock_name = dtDockInfo["dock_name"].ToString();//站台名称
            string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            string equipment_code = dtDockInfo["equipment_code"].ToString();//站台编码
            ////下面的方法是为了包装机上两个区段对应一个实际存放托盘的地点的检查。
            //if (equipment_code.ToLower().Contains("bzr") && dependentTask == null && ce.IsHaveContainerInfo(model.Machine))
            //{
            //    throw new WmsException(500, "机台上存在托盘，请退空盘完成后，再进行要料！");
            //}
            ////SelectTaskNumByMachine
            ////下面的方法是为了包装机上两个区段对应一个实际存放托盘的地点的检查。
            //if (equipment_code.ToLower().Contains("bzr") && dependentTask == null && ce.SelectTaskNumByMachine(model.Machine))
            //{
            //    throw new WmsException(500, "机台上存在未完成的任务，请等待完成后，在进行要料！");
            //}

            //手包机不需要判断站台上是否存在物料，也就是允许手包机在站台上有物料的时候允许要料
            if (dependentTask == null && !string.IsNullOrEmpty(container_id) && !string.Equals(equipmentUsage, EquipmentUsage.EquipmentUsage_Hand.ToString()) && !string.Equals(equipmentUsage, EquipmentUsage.EquipmentUsage_SuppleMaterial.ToString()))
            {
                throw new WmsException(500, "当前站台位上存在物料，无法再次要料！");
            }
            if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
            {
                throw new WmsException(500, "当前站台位处于故障状态，无法使用！");
            }
            if (dependentTask == null && dock_status == EquipmentStatus.EquipmentStatus_Using.ToString())
            {
                throw new WmsException(500, "当前站台位已被使用！");
            }
            if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
            {
                throw new WmsException(500, "当前站台对应的工艺段已停用，无法进行要料！");
            }
            #endregion

            #region 4.获取生产工单计划信息
            DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(model.ProductPlanOutId);
            if (dtProductPlanInfo == null || dtProductPlanInfo.Length < 1)
            {
                throw new WmsException(500, "未查询到该机台对应的生产工单！");
            }
            if (dtProductPlanInfo.Length != 1)
            {
                throw new WmsException(500, "该机台对应的正在生产中的工单不唯一！");
            }
            string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态
            if (plan_status != PlanOutStatus.PlanOutStatus_Executing.ToString())
            {
                throw new WmsException(500, "该机台对应的生产工单不可执行！");
            }
            #endregion

            //二楼区域id
            string twoFloorZoneId = WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336");

            if (string.Equals(twoFloorZoneId, machineZoneId))
            {
                TwoFloorOutBound(model, dependentTask);
            }
            else
            {
                OneFloorOutBound(model, dependentTask);
            }
        }
        #endregion

        #region 一楼托盘出库
        /// <summary>
        /// 一楼托盘出库
        /// </summary>
        /// <param name="model"></param>
        private void OneFloorOutBound(MachineMaterialModel model, Model.WorkTaskModel dependentTask)
        {
            #region 2.检查站台状态
            //查询站台状态
            DataRow dtDockInfo = ce.GetDockStatus(model.Dock);
            string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            string equipment_code = dtDockInfo["equipment_code"].ToString();//站台编码
            #endregion

            #region 3.获取生产工单计划信息
            DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(model.ProductPlanOutId);
            if (dtProductPlanInfo == null || dtProductPlanInfo.Length < 1)
            {
                throw new WmsException(500, "未查询到该机台对应的生产工单！");
            }
            if (dtProductPlanInfo.Length != 1)
            {
                throw new WmsException(500, "该机台对应的正在生产中的工单不唯一！");
            }
            string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态
            string job_no = dtProductPlanInfo[0]["job_no"].ToString();//生产工单单号
            string product_item_id = dtProductPlanInfo[0]["item_id"].ToString();//成品物料id
            string product_item_code = dtProductPlanInfo[0]["item_code"].ToString();//成品编码
            string product_item_name = dtProductPlanInfo[0]["item_name"].ToString();//成品名称
            if (plan_status != PlanOutStatus.PlanOutStatus_Executing.ToString())
            {
                throw new WmsException(500, "该机台对应的生产工单不可执行！");
            }
            #endregion

            #region 4.获取机台信息
            DataRow _drMachineInfo = ce.GetMachineInfoByMachineId(model.Machine);
            //机台所在区域
            string machineZoneId = _drMachineInfo["zone_id"].ToString();
            #endregion
            List<string> _BonIdList = new List<string>();
            #region 5.获取机台BOM信息
            DataTable dtBOMInfo = ce.GetBOMInfo(model.Dock);
            if (dtBOMInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "无法获取机台BOM信息！");
            }
            string bomId = string.Empty;
            string firstFloorZone = WmsConfig.GetOrDefault("Outbound:FirstFloorZone", "1291999756888969216");  //1楼区域
            for (int i = 0; i < dtBOMInfo.Rows.Count; i++)
            {
                _BonIdList.Add(dtBOMInfo.Rows[i]["t_id"].ToString());
                if (ce.isHaveStockByBomId(dtBOMInfo.Rows[i]["t_id"].ToString(), firstFloorZone))//指定bomId是否有库存
                {
                    bomId = dtBOMInfo.Rows[i]["t_id"].ToString();
                    break;
                }
            }

            if (string.IsNullOrEmpty(bomId))
            {
                throw new WmsException(500, "未查询到库存信息！");
            }

            #endregion
            Model.UserLogModel _logModel = new Model.UserLogModel();
            _logModel.ModifyIP = Context.UserInfo.LoginIP;
            _logModel.WarehouseID = Context.WarehouseID;
            _logModel.TypeCode = "一楼机台要料，获取BOM信息";
            _logModel.LogTextLong = "BOM信息：" + string.Join(",", _BonIdList.ToArray()) + "    要料入参" + JsonConvert.SerializeObject(model) + "   最终选择的BOM为" + bomId;
            _logModel.CommonName = "一楼要料时获取的BOM信息";
            _logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            _logModel.CreatedBy = Context.UserInfo.UserId;
            _logModel.LogText = "";
            UserLog.Add(_logModel);

            #region 6.获取货位信息
            //通过货位分配策略，获取可用货位
            string loc_id = ce.GetLocationInfoByBomId(bomId, machineZoneId);// ;//"1285384185019932672"
            //查询货位对应区域信息
            DataTable dtLocationInfo = ce.GetLocationInfo(loc_id);
            //货位状态
            string location_status = dtLocationInfo.Rows[0]["location_status"].ToString();
            //区域
            string zone_id = dtLocationInfo.Rows[0]["zone_id"].ToString();
            //货位对应的编码
            string loc_no = dtLocationInfo.Rows[0]["location_code"].ToString();
            string loc_row = dtLocationInfo.Rows[0]["location_row"].ToString();
            if (location_status != LocationStatus.LocationStatus_Stored.ToString())
            {
                throw new WmsException(500, "货位分配策略分配的货位，状态不为已存储状态，无法进行出库");
            }
            #endregion

            DataTable _BomInLoc = ce.GetBOMByLocationId(loc_id);
            if (_BomInLoc == null || _BomInLoc.Rows.Count == 0)
            {
                throw new WmsException(500, $"货位分配策略分配的货位为{loc_no}，实际上这个货位没有站台需要的盘型！");
            }
            if (_BomInLoc.Rows.Count != 1)
            {
                throw new WmsException(500, $"货位分配策略分配的货位为{loc_no}，实际上这个货位上有多个托盘，请联系库管人员处理！");
            }
            string _bomInLoc = _BomInLoc.Rows[0]["bom_id"].ToString();
            if (!string.Equals(bomId, _bomInLoc))
            {
                throw new WmsException(500, $"货位分配策略分配的货位为{loc_no}，该货位上的盘型与站台上需要的盘型不一致，请联系库管人员处理！");
            }

            #region 7.查询执行设备(堆垛机)
            //string ddjEquipId = ce.GetDDJ(loc_id); 
            string loc_New = loc_id;
            string ddjEquipId = ce.GetEquipmentByLoc(loc_row, ref loc_New);

            if (!loc_id.Equals(loc_New))
            {
                #region 重新获取货位信息
                loc_id = loc_New;
                dtLocationInfo = ce.GetLocationInfo(loc_id);
                //货位状态
                location_status = dtLocationInfo.Rows[0]["location_status"].ToString();
                //区域
                zone_id = dtLocationInfo.Rows[0]["zone_id"].ToString();
                //货位对应的编码
                loc_no = dtLocationInfo.Rows[0]["location_code"].ToString();

                if (location_status != LocationStatus.LocationStatus_Stored.ToString())
                {
                    throw new WmsException(500, $"货位{loc_no}状态不为已存储状态，无法进行出库");
                }
                #endregion
            }
            #endregion

            #region 8.获取货位对应容器信息
            //查询库位对应的容器头表信息
            DataRow[] _dtHeadsInfo = Container.GetHeadsByLocation(loc_id);
            if (_dtHeadsInfo == null || _dtHeadsInfo.Length <= 0)
            {
                throw new WmsException(500, "货位分配返回空货位");
            }
            DataRow dtcontainerhInfo = _dtHeadsInfo[0];
            //容器头表主键
            string container_htid = dtcontainerhInfo["t_id"].ToString();
            //容器头表编码
            string container_barcode = dtcontainerhInfo["container_barcode"].ToString();
            //查询容器明细信息
            DataRow[] dtrContainerDInfo = Container.GetDetailByHeadID(container_htid);
            #endregion

            #region 9.产生要料任务，包括修改站台状态
            //根据站台修改机台站台表中的容器id，并将生产工单id挂在挂站台上，用于退余料、退坏料和退空托盘
            //UpdateMachineDockDeatilId(machineDockTid, container_htid, model.ProductPlanOutId, bomId, string.Empty);
            //修改站台状态
            ce.UpdateDockStatus(machineDockTid, model.Dock, EquipmentStatus.EquipmentStatus_Using.ToString());

            #region 插入任务主档
            Model.WorkTaskModel taskModel = new Model.WorkTaskModel()
            {
                ID = SnowflakeID.Guid().ToString(),//主键ID
                ZoneID = zone_id,//区域ID
                TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                DependentTaskNo = dependentTask == null ? "" : dependentTask.TaskNo,//依赖任务号
                TaskType = TaskType.TaskType_OutBound.ToString(),//任务类型编码 "机台要料"
                ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                //ExecutedBy = "",//任务执行人
                EquipmentID = ddjEquipId,//执行设备ID
                //EquipmentType = "",//执行设备类型
                //ActualEquipmentID = "",//实际执行设备ID
                SeqNo = 1,//顺序号
                Priority = 1,//优先级
                ContainerID = container_htid,//容器ID
                ContainerBarcode = container_barcode,//容器条码
                RefDocType = DocType.DocType_CallTerminal.ToString(),//来源单据类型
                RefDocID = model.ProductPlanOutId,//来源单ID
                RefDocDetailID = model.Planno,//来源单明细ID,存放mes叫料号
                RefDocNo = job_no,//来源单号
                SrcAddrID = loc_id,//来源地址ID
                SrcAddrType = AddressType.AddressType_Location.ToString(),//来源地址类型 "location"
                SrcAddrText = loc_no,//来源地址
                DestAddrID = model.Dock,//目标地址ID
                DestAddrType = AddressType.AddressType_MachineStation.ToString(),//目标地址类型"dock"
                DestAddrText = equipment_code,//目标地址
                //ActualDestAddrID = "",//实际目标地址ID
                //ActualDestAddrText = "",//实际目标地址
                TaskStatus = dependentTask != null ? TaskStatus.TaskStatus_NotExecute.ToString() : TaskStatus.TaskStatus_Executing.ToString(),//任务状态
                //StartDate = dateTime,//开始时间
                //EndDate = dateTime,//结束时间
                IsExecutable = dependentTask == null ? "1" : "0",//任务是否可执行
                //PriorityChangeTime = dateTime,//优先级修改时间        
                LedText = "",//LED显示内容，此字段有其他用途
                ExistLgv = model.HasAGV,//是否存在LGV
                //CreateDate = dateTime,//创建时间
                //CreatedBy = "",//创建人帐号
                //ModifyIP = "",//操作人IP 
                //ModifyDate = dateTime,//编辑时间
                //LastModifiedBy = "",//最后修改人帐号
                ActStatus = "1",//状态
            };
            WorkTask.Insert(taskModel);
            #endregion

            for (int i = 0; i < dtrContainerDInfo.Length; i++)
            {
                decimal qty = Convert.ToDecimal(dtrContainerDInfo[i]["qty"].ToString() == "" ? "0" : dtrContainerDInfo[i]["qty"].ToString());
                //修改容器明细中物料待出数量
                Container.UpdateDetailOutQty(dtrContainerDInfo[i]["t_id"].ToString(), qty);
                #region 插入任务物料信息
                WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                    //ClientID = "",//集团ID
                    //OrgID = "",//组织ID
                    //WarehouseID = "",//仓库ID
                    TaskID = taskModel.ID,//主任务ID
                    ItemID = dtrContainerDInfo[i]["item_id"].ToString(),//物料ID
                    ItemName = dtrContainerDInfo[i]["item_name"].ToString(),//物料名称
                    PlanQty = Convert.ToDecimal(dtrContainerDInfo[i]["qty"].ToString()),//计划数量
                    //ProcessedQty = "",//已处理数量
                    //FinishedQty = "",//完成数量
                    UomQtyID = dtrContainerDInfo[i]["uom_qty_id"].ToString(),//数量单位
                    //FinishedStatQty = "",//完成统计数量
                    UomStatID = dtrContainerDInfo[i]["uom_stat_id"].ToString(),//统计数量单位
                    ContainerDetailID = dtrContainerDInfo[i]["t_id"].ToString(),//容器明细ID
                    RefDocType = DocType.DocType_CallTerminal.ToString(),//来源单据类型  "机台要料"
                    RefDocID = model.ProductPlanOutId,//来源单ID
                    //RefDocDetailID = "",//来源单明细ID
                    RefDocNo = job_no,//来源单号
                    //StartDate = "",//开始时间
                    //EndDate = "",//结束时间
                    ProcessStatus = ProcessStatus.ProcessStatus_New.ToString()//工作状态
                    //CreateDate = "",//创建时间
                    //CreatedBy = "",//创建人帐号 
                    //ModifyIP = "",//操作人IP 
                    //ModifyDate = "",//编辑时间
                    //LastModifiedBy = "",//最后修改人帐号
                    //ActStatus = ""//状态
                };
                WorkTaskItem.Insert(taskItemModel);
                #endregion
            }
            #endregion

            //DataTable tempDt = ce.GetLocationInfo(loc_id);
            //string locStatus = tempDt.Rows[0]["location_status"].ToString();
            //if (!string.Equals(locStatus.Trim(), LocationStatus.LocationStatus_Stored.ToString()))
            //{
            //    throw new WmsException(500, $"货位{loc_no}状态不为已存储状态，无法进行出库！没有找到可以出库的库存,请稍后重试！");
            //}
            string e1 = WmsConfig.GetOrDefault("Task:SC01", "1288091969666551808");  //1号堆垛机id
            string locLine = dtLocationInfo.Rows[0]["location_row"].ToString();
            bool isHaveContainerBeforeStoredOut;

            if (string.Equals(ddjEquipId, e1))
            {
                isHaveContainerBeforeStoredOut = ce.JustHaveContainerBeforeStoredOut(loc_no, locLine, false);
            }
            else
            {
                isHaveContainerBeforeStoredOut = ce.JustHaveContainerBeforeStoredOut(loc_no, locLine, true);
            }
            if (isHaveContainerBeforeStoredOut == true)
            {
                throw new WmsException(500, $"出库行存在未完成的任务,请稍后重试！");
            }

            #region 12.修改货位状态为待出库
            //修改货位状态为待出库
            ce.UpdateLocStatusByLocationId(loc_id, LocationStatus.LocationStatus_Out.ToString(), location_status, "");
            #endregion

            //不存在依赖任务，则需要下发
            if (dependentTask == null)
            {
                WebJsonInfo res= SendToWCS(taskModel.ID);
                if (res.isSucceed == false)
                {
                    throw new WmsException(500, res.errmsg);
                }
            }

        }
        #endregion

        #region 二楼托盘出库
        /// <summary>
        /// 二楼托盘出库
        /// </summary>
        /// <param name="model"></param>
        private void TwoFloorOutBound(MachineMaterialModel model, Model.WorkTaskModel dependentTask)
        {
            #region 2.检查站台状态
            //查询站台状态
            DataRow dtDockInfo = ce.GetDockStatus(model.Dock);
            string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            string equipment_code = dtDockInfo["equipment_code"].ToString();//站台编码
            #endregion

            #region 3.获取生产工单计划信息
            DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(model.ProductPlanOutId);
            string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态
            string job_no = dtProductPlanInfo[0]["job_no"].ToString();//生产工单单号
            if (plan_status != PlanOutStatus.PlanOutStatus_Executing.ToString())
            {
                throw new WmsException(500, "该机台对应的生产工单不可执行！");
            }
            #endregion

            #region 4.获取机台信息
            DataRow _drMachineInfo = ce.GetMachineInfoByMachineId(model.Machine);
            //机台所在区域
            string machineZoneId = _drMachineInfo["zone_id"].ToString();
            #endregion

            List<string> _BonIdList = new List<string>();
            #region 5.获取机台BOM信息
            DataTable dtBOMInfo = ce.GetBOMInfo(model.Dock);
            if (dtBOMInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "无法获取机台BOM信息！");
            }
            string bomId = string.Empty;
            string secondFloorZone = WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336");  //2楼区域
            for (int i = 0; i < dtBOMInfo.Rows.Count; i++)
            {
                string _sss = dtBOMInfo.Rows[i]["t_id"].ToString();
                if (!_BonIdList.Contains(_sss))
                {
                    _BonIdList.Add(dtBOMInfo.Rows[i]["t_id"].ToString());
                }
                if (ce.isHaveStockByBomId(dtBOMInfo.Rows[i]["t_id"].ToString(), secondFloorZone))
                {
                    bomId = dtBOMInfo.Rows[i]["t_id"].ToString();
                    break;
                }
            }
            if (string.IsNullOrEmpty(bomId))
            {
                for (int i = 0; i < dtBOMInfo.Rows.Count; i++)
                {
                    string _sss = dtBOMInfo.Rows[i]["t_id"].ToString();
                    if (!_BonIdList.Contains(_sss))
                    {
                        _BonIdList.Add(dtBOMInfo.Rows[i]["t_id"].ToString());
                    }
                    if (ce.isHaveStockByBomId2(dtBOMInfo.Rows[i]["t_id"].ToString(), secondFloorZone))
                    {
                        bomId = dtBOMInfo.Rows[i]["t_id"].ToString();
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(bomId))
            {
                throw new WmsException(500, "未查询到可用配盘信息！");
            }
            #endregion

            Model.UserLogModel _logModel = new Model.UserLogModel();
            _logModel.ModifyIP = Context.UserInfo.LoginIP;
            _logModel.WarehouseID = Context.WarehouseID;
            _logModel.TypeCode = "二楼机台要料，获取BOM信息";
            _logModel.LogTextLong = "BOM信息：" + string.Join(",", _BonIdList.ToArray()) + "    要料入参" + JsonConvert.SerializeObject(model) + "   最终选择的BOM为" + bomId;
            _logModel.CommonName = "二楼要料时获取的BOM信息";
            _logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
            _logModel.CreatedBy = Context.UserInfo.UserId;
            _logModel.LogText = "";
            UserLog.Add(_logModel);

            #region 6.获取货位信息
            //通过货位分配策略，获取可用货位
            string loc_id = ce.GetLocationInfoByBomId(bomId, machineZoneId);//此处为行号 ;//"1285384185019932672"

            ////查询货位对应区域信息
            //DataTable dtLocationInfo = ce.GetLocationInfo(loc_id);
            ////货位状态
            //string location_status = dtLocationInfo.Rows[0]["location_status"].ToString();
            //区域
            string zone_id = WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336");//dtLocationInfo.Rows[0]["zone_id"].ToString();
            ////货位对应的编码
            //string loc_no = dtLocationInfo.Rows[0]["location_code"].ToString();

            //if (location_status != LocationStatus.LocationStatus_Stored.ToString())
            //{
            //    throw new WmsException(500, "货位分配策略分配的货位，状态不为已存储状态，无法进行出库");
            //}
            #endregion

            //获取最大的已存储货位
            DataTable dtStoredLoc = ce.GetStoredLocByLine(loc_id, true);
            string storedLocLocId = dtStoredLoc.Rows[0]["t_id"].ToString();//货位Id
            string storedLocNo = dtStoredLoc.Rows[0]["location_no"].ToString();//货位编码
            string storedLocFlag = dtStoredLoc.Rows[0]["location_flag"].ToString();//货位标识
            if (!string.Equals(storedLocFlag,LocationFlag.LocationFlag_OutOnly.ToString())&& !string.Equals(storedLocFlag, LocationFlag.LocationFlag_Usable.ToString()) && !string.Equals(storedLocFlag, LocationFlag.LocationFlag_InOnly.ToString()))
            {
                throw new WmsException(500, $"货位分配策略分配的行{loc_id}，最外面的货位为{storedLocNo}，货位标识非可用或可出不可入或可入不可出，无法出库，请联系库管人员处理！");
            }

            DataTable _BomInLoc = ce.GetBOMByLocationId(storedLocLocId);
            if (_BomInLoc == null || _BomInLoc.Rows.Count == 0)
            {
                throw new WmsException(500, $"货位分配策略分配的行{loc_id}，最外面的货位为{storedLocNo}，实际上这个货位没有站台需要的盘型！");
            }
            if (_BomInLoc.Rows.Count != 1)
            {
                throw new WmsException(500, $"货位分配策略分配的行{loc_id}，最外面的货位为{storedLocNo}，实际上这个货位上有多个托盘，请联系库管人员处理！");
            }
            string _bomInLoc = _BomInLoc.Rows[0]["bom_id"].ToString();
            if (!string.Equals(bomId, _bomInLoc))
            {
                throw new WmsException(500, $"货位分配策略分配的行{loc_id}，最外面的货位为{storedLocNo}，该货位上的盘型与站台上需要的盘型不一致，请联系库管人员处理！");
            }

            ce.UpdateLocStatusByLocationId(storedLocLocId, LocationStatus.LocationStatus_Plan.ToString(), LocationStatus.LocationStatus_Stored.ToString(), "");//LocationStatus_Plan
            //修改整行的货位标识
            ce.UpdatelocationflagByRow(loc_id, LocationFlag.LocationFlag_OutOnly.ToString());

            #region 7.产生要料任务，包括修改站台状态
            //修改站台状态
            ce.UpdateDockStatus(machineDockTid, model.Dock, EquipmentStatus.EquipmentStatus_Using.ToString());

            #region 插入任务主档
            Model.WorkTaskModel taskModel = new Model.WorkTaskModel()
            {
                ID = SnowflakeID.Guid().ToString(),//主键ID
                ZoneID = zone_id,//区域ID
                TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                DependentTaskNo = dependentTask == null ? "" : dependentTask.TaskNo,//依赖任务号
                TaskType = TaskType.TaskType_OutBound.ToString(),//任务类型编码 "机台要料"
                ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                //ExecutedBy = "",//任务执行人
                EquipmentID = "",//执行设备ID
                //EquipmentType = "",//执行设备类型
                //ActualEquipmentID = "",//实际执行设备ID
                SeqNo = 1,//顺序号
                Priority = 1,//优先级
                ContainerID = "",//container_htid,//容器ID
                ContainerBarcode = "",//container_barcode,//容器条码
                RefDocType = DocType.DocType_CallTerminal.ToString(),//来源单据类型
                RefDocID = model.ProductPlanOutId,//来源单ID
                RefDocDetailID = model.Planno,//来源单明细ID,存放mes叫料号
                RefDocNo = job_no,//来源单号
                SrcAddrID = loc_id,//来源地址ID,货位id,此处存放行号
                SrcAddrType = AddressType.AddressType_Location.ToString(),//来源地址类型 "location"
                SrcAddrText = loc_id,//loc_no,//来源地址,货位
                DestAddrID = model.Dock,//目标地址ID
                DestAddrType = AddressType.AddressType_MachineStation.ToString(),//目标地址类型"dock"
                DestAddrText = equipment_code,//目标地址
                //ActualDestAddrID = "",//实际目标地址ID
                //ActualDestAddrText = "",//实际目标地址
                TaskStatus = dependentTask != null ? TaskStatus.TaskStatus_NotExecute.ToString() : TaskStatus.TaskStatus_Executing.ToString(),//任务状态
                //StartDate = dateTime,//开始时间
                //EndDate = dateTime,//结束时间
                IsExecutable = dependentTask == null ? "1" : "0",//任务是否可执行
                //PriorityChangeTime = dateTime,//优先级修改时间        
                LedText = "",//LED显示内容
                ExistLgv = model.HasAGV,//是否存在LGV
                //CreateDate = dateTime,//创建时间
                //CreatedBy = "",//创建人帐号
                //ModifyIP = "",//操作人IP 
                //ModifyDate = dateTime,//编辑时间
                //LastModifiedBy = "",//最后修改人帐号
                ActStatus = "1",//状态
            };
            WorkTask.Insert(taskModel);
            #endregion

            #endregion

            //不存在依赖任务，则需要下发
            if (dependentTask == null)
            {
                SendToWCS(taskModel.ID);
            }
        }
        #endregion

        #endregion

        #region 机台要料任务强制完成
        /// <summary>
        /// 机台要料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        public void FinishMachineMaterialModel(FinishMachineMaterialModel model)
        {
            #region 1.获取要料任务信息
            //获取要料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台要料任务！");
            }
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string loc_id = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//任务中，源地址id，此处指的是要料的货位id
            string destDockid = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//任务中，目的站台id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//任务中，生产工单id
            string dependent_task_no = dtTaskInfo.Rows[0]["dependent_task_no"].ToString();//依赖任务号
            if (refDocType != DocType.DocType_CallTerminal.ToString())
            {
                throw new WmsException(500, "该任务不是机台要料任务，无法强制完成！");
            }
            //if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString() && taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            //{
            //    throw new WmsException(500, "任务仅在处于执行中或者已部分完成状态下，才能强制完成！");
            //}
            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "任务已完成，无需再次完成！");
            }
            #endregion

            //存在依赖任务
            if (!string.IsNullOrEmpty(dependent_task_no))
            {
                DataTable _dtDependTask = ce.GetDependentTaskByTaskNo(dependent_task_no);
                if (_dtDependTask != null && _dtDependTask.Rows.Count > 0)
                {
                    throw new WmsException(500, "该任务存在未完成的依赖任务，请完成依赖任务后，再进行完成该任务！");
                }
            }

            //任务不是部分完成，则执行部分完成
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                PartionFinishMachineMaterialModel pft = new PartionFinishMachineMaterialModel();
                pft.TaskTid = model.TaskTid;
                PartionFinishMachineMaterial(pft);
            }
            //执行完成任务
            FinishMaterialModel ft = new FinishMaterialModel();
            ft.TaskTid = model.TaskTid;
            FinishMaterial(ft);

            #region 旧版强完代码
            //#region 1.获取要料任务信息
            ////获取要料任务信息
            //DataTable dtTaskInfo = GeTaskInfoByTaskId(model.TaskTid);
            //if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            //{
            //    throw new WmsException(500, "未查询到站台要料任务！");
            //}
            //string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            //string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            //string loc_id = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//任务中，源地址id，此处指的是要料的货位id
            //string destDockid = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//任务中，目的站台id
            //string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            //string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//任务中，生产工单id
            //if (refDocType != DocType.DocType_CallTerminal.ToString())
            //{
            //    throw new WmsException(500, "该任务不是机台要料任务，无法强制完成！");
            //}
            //if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString() && taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            //{
            //    throw new WmsException(500, "任务仅在处于执行中或者已部分完成状态下，才能强制完成！");
            //}
            //#endregion

            //#region 2.获取站台信息
            //DataRow dtDockInfo = GetDockStatus(destDockid);
            //string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id 
            //#endregion

            //#region 3.获取机台BOM信息
            //DataTable dtBOMInfo = GetBOMInfo(destDockid);
            //if (dtBOMInfo.Rows.Count <= 0)
            //{
            //    throw new WmsException(500, "无法获取机台BOM信息！");
            //}
            //string bomId = dtBOMInfo.Rows[0]["t_id"].ToString();//bomId
            //#endregion

            //#region 4.获取任务物料明细
            ////获取任务物料明细
            //DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
            //#endregion

            //#region 5.修改货位状态、解除容器与货位绑定关系
            ////根据容器id，取消与货位的绑定关系
            //SetLocIsNullInContainerHead(container_id);
            ////修改货位状态为空
            //UpdateLocStatusByLocationId(loc_id, LocationStatus.LocationStatus_Empty.ToString(), LocationStatus.LocationStatus_Out.ToString(), "机台要料出库,修改货位状态为空");
            //#endregion

            //#region 6.更新任务和任务明细，并清空容器明细中物料待出数量
            ////将任务更新为完成
            //WorkTask.UpdateTaskComplete(model.TaskTid);
            ////更新任务物料明细中的已处理数量和完成数量
            //for (int i = 0; i < taskItemInfos.Length; i++)
            //{
            //    string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
            //    string containerDTid = taskItemInfos[i]["container_d_id"].ToString();//容器明细id

            //    //根据任务物料id，修改任务物料状态为已完成，并且修改完成和处理数量
            //    WorkTask.UpdateTaskItemCompleteAndQty(cdTid);
            //    //清空容器明细中物料待出数量
            //    Container.UpdateDetailOutQty(containerDTid, (decimal)0.0);

            //}
            //WorkTask.MoveToHistory(model.TaskTid);
            //#endregion

            //#region 7.将容器挂在站台上，包括生产工单、bomId
            ////根据站台修改机台站台表中的容器id，并将生产工单id挂在挂站台上，用于退余料、退坏料和退空托盘
            //UpdateMachineDockDeatilId(machineDockTid, container_id, productPlanOutId, bomId, string.Empty); 
            //#endregion 
            #endregion

        }
        #endregion

        #region 机台要料任务回收
        /// <summary>
        /// 机台要料任务回收
        /// </summary>
        /// <param name="model"></param>
        public void RecycleMachineMaterial(RecycleMachineMaterialModel model)
        {
            #region 1.获取要料任务信息
            //获取要料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台要料任务！");
            }
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string taskZoneId = dtTaskInfo.Rows[0]["zone_id"].ToString();//任务中的区域
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string loc_id = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//任务中，源地址id，此处指的是要料的货位id
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//任务中，生产工单id
            string destDockid = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//任务中，目的站台id
            string srcAddrText = dtTaskInfo.Rows[0]["src_addr_text"].ToString();//任务中，源货位编号
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_CallTerminal.ToString())
            {
                throw new WmsException(500, "该任务不是机台要料任务，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "该任务已完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "该任务已部分完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "该任务正在执行中，无法回收任务！");
            }
            #endregion
            //二楼，第一次货位分配时需要回收任务
            if (string.Equals(taskZoneId, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))
            {
                if (srcAddrText.Length <= 3)//一次货位分配
                {
                    RecycleOutBoundFirstDistributionLocationTask(dtTaskInfo.Rows[0]);
                    //修改任务软删除标记为“0”
                    WorkTask.UpdateActStatus(model.TaskTid, "0");
                    //修改站台状态为OK状态
                    ce.UpdateDockStatus("", destDockid, EquipmentStatus.EquipmentStatus_OK.ToString());
                }
                else//二次货位分配
                {
                    RecycleOutBoundSecoundDistributionLocationTask(dtTaskInfo.Rows[0]);

                    #region 删除任务，清空站台信息，删除容器明细中的待出数量
                    RecycleMachineMaterialTaskAndContainer(model, container_id, productPlanOutId, destDockid);
                    #endregion
                }

                return;
            }


            RecycleMachineMaterialTaskAndContainer(model, container_id, productPlanOutId, destDockid);

            #region 7.任务回收时，修改原货位状态为已存储
            //修改货位状态为已存储
            ce.UpdateLocStatusByLocationId(loc_id, LocationStatus.LocationStatus_Stored.ToString(), LocationStatus.LocationStatus_Out.ToString(), "机台要料任务回收,物料无需出库,修改货位状态为已存储");
            #endregion

        }

        #region 回收要料任务时删除任务，并清空挂在站台上的信息
        /// <summary>
        /// 回收要料任务时删除任务，并清空挂在站台上的信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="container_id"></param>
        /// <param name="productPlanOutId"></param>
        /// <param name="destDockid"></param>
        private void RecycleMachineMaterialTaskAndContainer(RecycleMachineMaterialModel model, string container_id, string productPlanOutId, string destDockid)
        {
            #region 2.获取生产工单信息
            //获取生产工单信息
            DataRow[] productPlanOutInfo = ce.GetProductPlanInfo(productPlanOutId);
            string machineId = productPlanOutInfo[0]["equipment_id"].ToString();//机台id 
            #endregion

            #region 3.根据机台和站台id获取机台站台表中的信息
            //根据机台和站台id获取机台站台表中的信息
            DataTable dtMachineDockInfo = ce.GetMachineDockInfoByMachineAndDock(machineId, destDockid);
            if (dtMachineDockInfo == null || dtMachineDockInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到该任务对应的站台信息，无法回收任务！");
            }
            if (dtMachineDockInfo == null || dtMachineDockInfo.Rows.Count != 1)
            {
                throw new WmsException(500, "查询到该任务对应多个站台信息，无法回收任务！");
            }
            //机台站台表id
            string machineDockTid = dtMachineDockInfo.Rows[0]["t_id"].ToString();
            #endregion

            #region 4.获取任务物料明细
            //获取任务物料明细
            DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
            #endregion

            #region 5.清空挂在站台上的容器信息，修改站台状态为ok状态
            //根据站台，清空机台站台表中的容器id,并清空来源单号
            ce.UpdateMachineDockDeatilId(machineDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            //修改站台状态为OK状态
            ce.UpdateDockStatus("", destDockid, EquipmentStatus.EquipmentStatus_OK.ToString());
            #endregion

            #region 6.更新任务和任务明细软删除标记为“0”，并清空容器明细中物料待出数量
            //修改任务软删除标记为“0”
            WorkTask.UpdateActStatus(model.TaskTid, "0");
            //更新任务物料明细中的已处理数量和完成数量
            for (int i = 0; i < taskItemInfos.Length; i++)
            {
                string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
                string containerDTid = taskItemInfos[i]["container_d_id"].ToString();//容器明细id
                                                                                     //修改任务物料表中的软删除标记为“0”
                WorkTask.UpdateTaskItemActStatus(cdTid, "0");
                //清空容器明细中物料待出数量
                Container.UpdateDetailOutQty(containerDTid, (decimal)0.0);
            }
            #endregion

            //恢复删除容器主档
            Container.UpdateHeadActStatus(container_id, "1");
        }
        #endregion

        #endregion

        #region 机台要料任务部分完成
        /// <summary>
        /// 机台要料任务部分完成
        /// </summary>
        /// <param name="model"></param>
        public void PartionFinishMachineMaterial(PartionFinishMachineMaterialModel model)
        {
            #region 1.获取要料任务信息
            //获取要料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台要料任务！");
            }
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string loc_id = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//任务中，源地址id，此处指的是要料的货位id
            string destDockid = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//任务中，目的站台id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//任务中，生产工单id
            if (refDocType != DocType.DocType_CallTerminal.ToString())
            {
                throw new WmsException(500, "该任务不是机台要料任务，无法强制完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "任务仅在处于执行中状态下，才能部分完成！");
            }
            #endregion

            #region 2.将任务修改为部分完成
            WorkTask.UpdateTaskStatus(model.TaskTid, TaskStatus.TaskStatus_PartialFinished.ToString(), TaskStatus.TaskStatus_Executing.ToString());
            #endregion

            #region 3.修改货位状态、解除容器与货位绑定关系
            //根据容器id，取消与货位的绑定关系
            ce.SetLocIsNullInContainerHead(container_id);
            //删除容器主档
            Container.UpdateHeadActStatus(container_id, "0");
            //修改货位状态为空
            ce.UpdateLocStatusByLocationId(loc_id, LocationStatus.LocationStatus_Empty.ToString(), LocationStatus.LocationStatus_Out.ToString(), "机台要料出库,修改货位状态为空");
            #endregion
        }
        #endregion

        #region 机台要料任务完成
        /// <summary>
        /// 机台要料任务完成
        /// </summary>
        /// <param name="model"></param>
        public void FinishMaterial(FinishMaterialModel model)
        {
            #region 1.获取要料任务信息
            //获取要料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台要料任务！");
            }
            string zone_id = dtTaskInfo.Rows[0]["zone_id"].ToString();//区域id
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string loc_id = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//任务中，源地址id，此处指的是要料的货位id
            string destDockid = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//任务中，目的站台id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//任务中，生产工单id
            if (refDocType != DocType.DocType_CallTerminal.ToString())
            {
                throw new WmsException(500, "该任务不是机台要料任务，无法完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "任务仅在处于已部分完成状态下，才能完成！");
            }
            #endregion

            #region 2.获取站台信息
            DataRow dtDockInfo = ce.GetDockStatus(destDockid);
            string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id 
            string machineTid = dtDockInfo["machine_id"].ToString();//机台的t_id 
            #endregion

            #region 4.获取机台信息
            DataRow _drMachineInfo = ce.GetMachineInfoByMachineId(machineTid);
            //机台所在区域
            string machineZoneId = _drMachineInfo["zone_id"].ToString();
            //设备用途
            string equipmentUsage = _drMachineInfo["equipment_usage"].ToString();
            #endregion

            #region 3.获取机台BOM信息
            //DataTable dtBOMInfo = ce.GetBOMInfo(destDockid);
            //if (dtBOMInfo.Rows.Count <= 0)
            //{
            //    throw new WmsException(500, "无法获取机台BOM信息！");
            //}
            //string bomId = dtBOMInfo.Rows[0]["t_id"].ToString();//bomId
            #endregion

            #region 4.获取任务物料明细
            //获取任务物料明细
            DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
            #endregion

            #region 5.更新任务和任务明细，并清空容器明细中物料待出数量
            //将任务更新为完成
            WorkTask.UpdateTaskComplete(model.TaskTid);
            //更新任务物料明细中的已处理数量和完成数量
            for (int i = 0; i < taskItemInfos.Length; i++)
            {
                string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
                string containerDTid = taskItemInfos[i]["container_d_id"].ToString();//容器明细id

                //根据任务物料id，修改任务物料状态为已完成，并且修改完成和处理数量
                WorkTask.UpdateTaskItemCompleteAndQty(cdTid);
                //清空容器明细中物料待出数量
                Container.UpdateDetailOutQty(containerDTid, (decimal)0.0);

            }
            WorkTask.MoveToHistory(model.TaskTid);
            #endregion

            #region 6.将容器挂在站台上，包括生产工单、bomId
            //根据站台修改机台站台表中的容器id，并将生产工单id挂在挂站台上，用于退余料、退坏料和退空托盘
            ce.UpdateMachineDockDeatilId(machineDockTid, container_id, productPlanOutId, "", string.Empty);
            #endregion

            //手包机任务完成的时候允许要料，需要将站台状态置为ok
            if (string.Equals(equipmentUsage, EquipmentUsage.EquipmentUsage_Hand.ToString()) || string.Equals(equipmentUsage, EquipmentUsage.EquipmentUsage_SuppleMaterial.ToString()))
            {
                //修改站台状态
                ce.UpdateDockStatus(machineDockTid, destDockid, EquipmentStatus.EquipmentStatus_OK.ToString());
            }

            if (string.Equals(zone_id, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))
            {
                Location lo = new Location();
                LocationModel _locInfo = lo.GetLocationById(loc_id);



                bool isLineEmpty = ce.IsLineEmpty(_locInfo.location_row.ToString());
                Model.UserLogModel _logModel = new Model.UserLogModel();
                _logModel.ModifyIP = Context.UserInfo.LoginIP;
                _logModel.WarehouseID = Context.WarehouseID;
                _logModel.TypeCode = "二楼机台要料任务完成";
                _logModel.LogTextLong = $"货位编码{_locInfo.location_no},行号：" + _locInfo.location_row.ToString() + "  结果为" + isLineEmpty;
                _logModel.CommonName = "查询指定行是否已空";
                _logModel.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
                _logModel.CreatedBy = Context.UserInfo.UserId;
                _logModel.LogText = "";
                UserLog.Add(_logModel);

                bool IsLineContainsOut = ce.IsLineContainsOut(_locInfo.location_row.ToString());

                Model.UserLogModel _logModel1 = new Model.UserLogModel();
                _logModel1.ModifyIP = Context.UserInfo.LoginIP;
                _logModel1.WarehouseID = Context.WarehouseID;
                _logModel1.TypeCode = "二楼机台要料任务完成";
                _logModel1.LogTextLong = $"货位编码{_locInfo.location_no},行号：" + _locInfo.location_row.ToString() + "  结果为" + IsLineContainsOut;
                _logModel1.CommonName = "查询指定行是否全部为可出不可入";
                _logModel1.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
                _logModel1.CreatedBy = Context.UserInfo.UserId;
                _logModel1.LogText = "";
                UserLog.Add(_logModel1);
                if (isLineEmpty == true && IsLineContainsOut == true)
                {
                    Model.UserLogModel _logModel2 = new Model.UserLogModel();
                    _logModel2.ModifyIP = Context.UserInfo.LoginIP;
                    _logModel2.WarehouseID = Context.WarehouseID;
                    _logModel2.TypeCode = "二楼机台要料任务完成";
                    _logModel2.LogTextLong = $"货位编码{_locInfo.location_no},行号：" + _locInfo.location_row.ToString();
                    _logModel2.CommonName = "查询指定行已全部为空，且全部为可出不可入";
                    _logModel2.CriticalLevel = (decimal)CriticalLevel.CriticalLevel_Info;
                    _logModel2.CreatedBy = Context.UserInfo.UserId;
                    _logModel2.LogText = "";
                    UserLog.Add(_logModel2);
                    //lo.UpdateLocationStatus(_locInfo.t_id, LocationStatus.LocationStatus_Stored);
                    lo.UpdateLocationFlag(_locInfo.location_row.ToString(), LocationFlag.LocationFlag_Usable.ToString(), LocationFlag.LocationFlag_OutOnly.ToString());
                }
            }


        }
        #endregion

        #region 退余料,也允许有条件的继续要料
        /// <summary>
        /// 退余料,也允许有条件的继续要料
        /// </summary>
        /// <param name="model"></param>
        public WebJsonInfo ClientReturnPallet(ClientReturnPalletModel model)
        {
            //退余料+要料：errcode为0，表示成功退余料+要料；errcode为1，表示退余料失败；errcode为500表示退余料成功，但要料失败
            string machineDockTid = string.Empty;
            string equipment_code = string.Empty;
            string job_no = string.Empty;
            Model.WorkTaskModel taskModel = null;
            string logText = $"退余料" + (model.IsNeedCallPallet == "1" ? "+要料" : "");
            WebJsonInfo info = WmsHelper.Do(new UserLogModel()
            {
                LogText = $"工单{model.ProductPlanOutId},机台{model.Machine},站台{model.Dock},退余料",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = logText,
                CommonName = "api/wms/MachineMaterialService/ClientReturnPallet",
            }, () => FirstCreateReturnPalletTask(model, out machineDockTid, out equipment_code, out job_no, out taskModel));//FirstCreateReturnPalletTask   ReturnPallet

            if (info.errcode != 0)//退余料失败了，就不再进行要料
            {
                info.errcode = 1;
                return info;
            }
            if (model.IsNeedCallPallet == "1")
            {
                info = WmsHelper.Do(new UserLogModel()
                {
                    LogText = $"工单{model.ProductPlanOutId},机台{model.Machine},站台{model.Dock},要料",
                    LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                    TypeCode = logText,
                    CommonName = "api/wms/MachineMaterialService/CreateOutBoundByMachineDock",
                }, () => CallPalletWhenReturnPallet(model, machineDockTid, equipment_code, job_no, taskModel));
            }
            info.errcode = 0;
            return info;
        }

        #region 退余料
        /// <summary>
        /// 退余料
        /// </summary>
        /// <param name="model">退余料参数model</param>
        /// <param name="machineDockTid">站台在机台站台表中的t_id</param>
        /// <param name="equipment_code">站台编码</param>
        /// <param name="job_no">生产工单单号</param>
        /// <param name="taskModel">退余料任务model</param>
        private void ReturnPallet(ClientReturnPalletModel model, out string machineDockTid, out string equipment_code, out string job_no, out Model.WorkTaskModel taskModel)
        {
            Putaway putaway = new Putaway();

            #region 1.检查站台当前任务 
            //查询站台未执行完成的任务数量
            DataRow dtTaskNumInfo = ce.SelectTaskNumByDock(model.Dock);
            int cnt = Convert.ToInt32(dtTaskNumInfo["cnt"].ToString());//站台未执行完成的任务数量
            if (cnt > 0)
            {
                throw new WmsException(1, "该站台上有未执行完成的任务，无法退余料");
            }
            #endregion

            if (ce.MachineHaveDockInfo(model.Machine, model.Dock) == false)
            {
                throw new WmsException(1, "机台上没有该站台，请重新登录账号！");
            }

            #region 2.检查站台状态
            //查询站台状态
            DataRow dtDockInfo = ce.GetDockAndBomInfo(model.Dock);
            machineDockTid = dtDockInfo["t_id"].ToString();
            string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            string doc_id = dtDockInfo["doc_id"].ToString();//来源单号，此处存放的是要料时的生产工单
            equipment_code = dtDockInfo["equipment_code"].ToString();
            string machine_id = dtDockInfo["machine_id"].ToString();//机台id
            string container_id = dtDockInfo["container_id"].ToString();//容器主档id
            string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            string bomId = dtDockInfo["bomId"].ToString();//bomId
            if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
            {
                throw new WmsException(1, "当前站台位处于故障状态，无法使用！");
            }
            if (dock_status != EquipmentStatus.EquipmentStatus_Using.ToString())
            {
                throw new WmsException(1, "当前站台位未使用！");
            }
            if (string.IsNullOrEmpty(container_id))
            {
                throw new WmsException(1, "当前站台位无托盘信息！");
            }
            if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
            {
                throw new WmsException(1, "当前站台对应的工艺段已停用，无法进行退余料！");
            }
            #endregion

            DataRow _drMachineInfo = ce.GetMachineInfoByMachineId(machine_id);

            #region 3.获取货位信息
            ////通过货位分配策略，获取可用货位
            //string loc_id = ce.GetEmptyLocation();//"1285384218838605824";
            //参数：仓库id，容器明细中物料id，bomId，区域id
            string loc_id = putaway.AllocateOneLocationForPallet(_drMachineInfo["warehouse_id"].ToString(), "", bomId, _drMachineInfo["zone_id"].ToString());
            #endregion

            #region 7.获取要料时的生产工单计划信息
            DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(doc_id);
            if (dtProductPlanInfo == null || dtProductPlanInfo.Length < 1)
            {
                throw new WmsException(1, "未查询到该机台对应的生产工单！");
            }
            string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态
            job_no = dtProductPlanInfo[0]["job_no"].ToString();
            string product_item_id = dtProductPlanInfo[0]["item_id"].ToString();//成品物料id
            string product_item_code = dtProductPlanInfo[0]["item_code"].ToString();//成品编码
            string product_item_name = dtProductPlanInfo[0]["item_name"].ToString();//成品名称
            #endregion

            if (loc_id.Length <= 3)//小于等于3，表示获取的是行，不是货位id.只要二楼才会出现小于等于3的情况
            {
                if (string.IsNullOrEmpty(loc_id))
                {
                    throw new WmsException(1, "分配货位失败");
                }
                TwoFloorReturnPallet(model, equipment_code, loc_id, doc_id, out taskModel);
                return;
            }

            #region 4.查询库位信息
            //查询货位对应区域信息和容器信息
            DataTable dtLocationInfo = ce.GetLocationInfo(loc_id);

            if (dtLocationInfo.Rows.Count > 1)
            {
                throw new WmsException(1, "退余料时分配的货位属于多个区域");
            }

            //区域
            string zone_id = dtLocationInfo.Rows[0]["zone_id"].ToString();
            //货位对应的编码
            string loc_no = dtLocationInfo.Rows[0]["location_code"].ToString();
            //容器主档id
            string locContainer_htid = dtLocationInfo.Rows[0]["t_id"].ToString();
            //货位状态
            string location_status = dtLocationInfo.Rows[0]["location_status"].ToString();
            //判断货位分配策略返回的货位上是否已经与容器主档关联，如果有关联，则说明货位上有物料
            if (!string.IsNullOrEmpty(locContainer_htid))
            {
                throw new WmsException(1, "退余料时分配的货位上已有物料，退料失败！");
            }
            if (location_status != LocationStatus.LocationStatus_Empty.ToString())
            {
                throw new WmsException(1, "货位分配策略分配的货位，状态不为空，无法进行退余料");
            }
            #endregion

            #region 5.将退余料json信息转化成json对象
            //退余料数据信息
            JObject entity = JObject.Parse(model.Details);
            if (entity["ReturnItems"].Count() == 0)
            {
                throw new WmsException(1, "无法获取退余料明细数据！");
            }
            #endregion

            #region 6.获取容器主档信息，容器明细信息
            //获取挂在当前站台上容器id
            DataTable containerHInfo = ce.GetContainerInfoByDockIdAndMachineId(model.Dock, model.Machine);
            if (containerHInfo.Rows.Count > 1)
            {
                throw new WmsException(1, "根据机台和站台，获取挂在站台上面的容器主档不唯一");
            }
            string container_htid = containerHInfo.Rows[0]["container_id"].ToString();
            //根据容器主档id，获取容器主档信息
            DataRow dtrContainerInfo = Container.GetContainerH(container_htid);
            //查询容器明细
            DataTable dtContainerDInfo = ce.ToDataTable(Container.GetDetailByHeadID(container_htid));
            #endregion

            //修改容器主档是否允许被呼叫
            ce.UpdateContainerAllowCalled(container_htid, model.IsCanBeCallOut);

            //查询执行设备(堆垛机)
            string ddjEquipId = ce.GetDDJ(loc_id);

            #region 8.创建入库任务

            #region 插入任务主档
            taskModel = new Model.WorkTaskModel()
            {
                ID = SnowflakeID.Guid().ToString(),//主键ID
                ZoneID = zone_id,//区域ID
                TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                //DependentTaskNo = "",//依赖任务号
                TaskType = TaskType.TaskType_InBound.ToString(),//任务类型编码"机台退余料"
                ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                //ExecutedBy = "",//任务执行人
                EquipmentID = ddjEquipId,//equipRow == null ? "" : equipRow["t_id"].ToString(),//执行设备ID
                //EquipmentType = "",//执行设备类型
                //ActualEquipmentID = "",//实际执行设备ID
                SeqNo = 1,//顺序号
                Priority = 1,//优先级
                ContainerID = container_htid,//容器ID
                ContainerBarcode = dtrContainerInfo["container_barcode"].ToString(),//容器条码
                RefDocType = DocType.DocType_ClientReturnPallet.ToString(),//来源单据类型  "机台退余料"
                RefDocID = doc_id,//来源单ID
                RefDocDetailID = model.Planno,//来源单明细ID,存放mes叫料号
                RefDocNo = job_no,//来源单号
                SrcAddrID = model.Dock,//来源地址ID
                SrcAddrType = AddressType.AddressType_MachineStation.ToString(),//来源地址类型"dock"
                SrcAddrText = equipment_code,//来源地址
                DestAddrID = loc_id,//目标地址ID
                DestAddrType = AddressType.AddressType_Location.ToString(),//目标地址类型"location"
                DestAddrText = loc_no,//目标地址
                //ActualDestAddrID = "",//实际目标地址ID
                //ActualDestAddrText = "",//实际目标地址
                TaskStatus = TaskStatus.TaskStatus_Executing.ToString(),//任务状态
                //StartDate = dateTime,//开始时间
                //EndDate = dateTime,//结束时间
                IsExecutable = "1",//任务是否可执行
                //PriorityChangeTime = dateTime,//优先级修改时间        
                LedText = "",//LED显示内容
                ExistLgv = model.HasAGV,//是否存在LGV
                //CreateDate = dateTime,//创建时间
                //CreatedBy = "",//创建人帐号
                //ModifyIP = "",//操作人IP 
                //ModifyDate = dateTime,//编辑时间
                //LastModifiedBy = "",//最后修改人帐号
                ActStatus = "1"//状态
            };
            WorkTask.Insert(taskModel);
            #endregion

            for (int i = 0; i < entity["ReturnItems"].Count(); i++)
            {
                var job = entity["ReturnItems"][i];
                decimal qty = Convert.ToDecimal(job["qty"].ToString() == "" ? "0" : job["qty"].ToString());
                string containerDetailId = job["containerDetailId"].ToString();//容器明细id
                DataRow[] drs = dtContainerDInfo.Select($"t_id='{containerDetailId}'");
                if (drs.Length == 1)
                {
                    //修改容器明细中物料数量
                    Container.UpdateDetailQty(containerDetailId, qty);
                }
                else
                {
                    throw new WmsException(500, "无法获取退余料明细数据！");
                }

                #region 插入任务物料信息
                WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                    //ClientID = "",//集团ID
                    //OrgID = "",//组织ID
                    //WarehouseID = "",//仓库ID
                    TaskID = taskModel.ID,//主任务ID
                    ItemID = drs[0]["item_id"].ToString(),//物料ID
                    ItemName = drs[0]["item_name"].ToString(),//物料名称
                    PlanQty = qty,//计划数量
                    //ProcessedQty = "",//已处理数量
                    //FinishedQty = "",//完成数量
                    UomQtyID = drs[0]["uom_qty_id"].ToString(),//数量单位
                    //FinishedStatQty = "",//完成统计数量
                    UomStatID = drs[0]["uom_stat_id"].ToString(),//统计数量单位
                    ContainerDetailID = drs[0]["t_id"].ToString(),//容器明细ID
                    RefDocType = DocType.DocType_ClientReturnPallet.ToString(),//来源单据类型
                    RefDocID = doc_id,//来源单ID
                    //RefDocDetailID = "",//来源单明细ID
                    RefDocNo = job_no,//来源单号
                    //StartDate = "",//开始时间
                    //EndDate = "",//结束时间
                    ProcessStatus = ProcessStatus.ProcessStatus_New.ToString(),//工作状态
                    //CreateDate = "",//创建时间
                    //CreatedBy = "",//创建人帐号 
                    //ModifyIP = "",//操作人IP 
                    //ModifyDate = "",//编辑时间
                    //LastModifiedBy = "",//最后修改人帐号
                    //ActStatus = ""//状态
                };
                WorkTaskItem.Insert(taskItemModel);
                #endregion
            }
            #endregion

            #region 9.获取交接班和人工领料站台
            ////获取交接班站台
            //DataRow drShiftDockInfo = ce.GetShiftDockByMachine(model.Machine);
            //string jiaojiebanDockTid = drShiftDockInfo["t_id"].ToString();//交接班站台在机台站台表中的t_id 
            #endregion

            #region 10.清空挂在交接班和人工领料站台上的容器
            ////清空挂在交接班站台上的容器
            //ce.UpdateMachineDockDeatilId(jiaojiebanDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            ////清空挂在人工领料站台上的容器
            //UpdateMachineDockDeatilId(jiebanDockTid, string.Empty, string.Empty);
            #endregion

            #region 11.修改货位状态为待入库
            //修改货位状态为待入库
            ce.UpdateLocStatusByLocationId(loc_id, LocationStatus.LocationStatus_In.ToString(), location_status, "机台退余料,物料入库,修改货位状态为待入库");
            #endregion

            //发送给wcs
            List<string> _idList = new List<string>();
            _idList.Add(taskModel.ID);
            WorkTaskInterface.SendInBonud(_idList);
        }

        #endregion

        #region 20201222退余料两步走，第一步生产到输送线站台的任务；第二步，生成到目标货位的任务
        private void FirstCreateReturnPalletTask(ClientReturnPalletModel model, out string machineDockTid, out string equipment_code, out string job_no, out Model.WorkTaskModel taskModel)
        {
            Putaway putaway = new Putaway();

            #region 1.检查站台当前任务 
            //查询站台未执行完成的任务数量
            DataRow dtTaskNumInfo = ce.SelectTaskNumByDock(model.Dock);
            int cnt = Convert.ToInt32(dtTaskNumInfo["cnt"].ToString());//站台未执行完成的任务数量
            if (cnt > 0)
            {
                throw new WmsException(1, "该站台上有未执行完成的任务，无法退余料");
            }
            #endregion

            #region 2.机台上是否有任务
            if (ce.MachineHaveDockInfo(model.Machine, model.Dock) == false)
            {
                throw new WmsException(1, "机台上没有该站台，请重新登录账号！");
            }
            #endregion

            #region 3.检查站台状态
            //查询站台状态
            DataRow dtDockInfo = ce.GetDockAndBomInfo(model.Dock);
            machineDockTid = dtDockInfo["t_id"].ToString();
            string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            string doc_id = dtDockInfo["doc_id"].ToString();//来源单号，此处存放的是要料时的生产工单
            equipment_code = dtDockInfo["equipment_code"].ToString();
            string machine_id = dtDockInfo["machine_id"].ToString();//机台id
            string container_id = dtDockInfo["container_id"].ToString();//容器主档id
            string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            string bomId = dtDockInfo["bomId"].ToString();//bomId
            if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
            {
                throw new WmsException(1, "当前站台位处于故障状态，无法使用！");
            }
            if (dock_status != EquipmentStatus.EquipmentStatus_Using.ToString())
            {
                throw new WmsException(1, "当前站台位未使用！");
            }
            if (string.IsNullOrEmpty(container_id))
            {
                throw new WmsException(1, "当前站台位无托盘信息！");
            }
            if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
            {
                throw new WmsException(1, "当前站台对应的工艺段已停用，无法进行退余料！");
            }
            #endregion

            #region 4.获取机台信息
            DataRow _drMachineInfo = ce.GetMachineInfoByMachineId(machine_id);
            string zoneId = _drMachineInfo["zone_id"].ToString();
            #endregion

            #region 5.获取要料时的生产工单计划信息
            DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(doc_id);
            if (dtProductPlanInfo == null || dtProductPlanInfo.Length < 1)
            {
                throw new WmsException(1, "未查询到该机台对应的生产工单！");
            }
            string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态
            job_no = dtProductPlanInfo[0]["job_no"].ToString();
            string product_item_id = dtProductPlanInfo[0]["item_id"].ToString();//成品物料id
            string product_item_code = dtProductPlanInfo[0]["item_code"].ToString();//成品编码
            string product_item_name = dtProductPlanInfo[0]["item_name"].ToString();//成品名称
            #endregion

            #region 一楼和二楼分别处理
            string loc_id = "";
            if (string.Equals(zoneId, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))//二楼
            {
                loc_id = putaway.AllocateOneLocationForPallet(_drMachineInfo["warehouse_id"].ToString(), "", bomId, _drMachineInfo["zone_id"].ToString());
                if (loc_id.Length <= 3)//小于等于3，表示获取的是行，不是货位id.只要二楼才会出现小于等于3的情况
                {
                    if (string.IsNullOrEmpty(loc_id))
                    {
                        throw new WmsException(1, "分配货位失败");
                    }
                    TwoFloorReturnPallet(model, equipment_code, loc_id, doc_id, out taskModel);
                    return;
                }
                else
                {
                    throw new WmsException(1, "二楼退余料无法分配到行！");
                }
            }
            else//一楼
            {
                #region 6.获取退余料目标站台，该站台与退空盘的站台一致
                DataTable NullPalletDestAddrInfo = ce.GetReturnNullPalletDestAddrInfo(zoneId, EquipmentUsage.EquipmentUsage_ReturnNullPallet.ToString());
                if (NullPalletDestAddrInfo == null || NullPalletDestAddrInfo.Rows.Count <= 0)
                {
                    throw new WmsException(1, "退余料时未查询到目的站台信息，请创建目的站台信息后在进行退余料！");
                }
                if (NullPalletDestAddrInfo.Rows.Count > 1)
                {
                    throw new WmsException(1, "退余料时查询到多个目的站台信息，目的站台只能有一个！");
                }
                string destAddrID = NullPalletDestAddrInfo.Rows[0]["t_id"].ToString();//退余料时的目的输送线站台id
                string destAddrText = NullPalletDestAddrInfo.Rows[0]["equipment_code"].ToString();//退余料时的目的输送线站台编码
                #endregion

                #region 7.将退余料json信息转化成json对象
                //退余料数据信息
                JObject entity = JObject.Parse(model.Details);
                if (entity["ReturnItems"].Count() == 0)
                {
                    throw new WmsException(1, "无法获取退余料明细数据！");
                }
                #endregion

                #region 8.获取容器主档信息，容器明细信息
                //获取挂在当前站台上容器id
                DataTable containerHInfo = ce.GetContainerInfoByDockIdAndMachineId(model.Dock, model.Machine);
                if (containerHInfo.Rows.Count > 1)
                {
                    throw new WmsException(1, "根据机台和站台，获取挂在站台上面的容器主档不唯一");
                }
                string container_htid = containerHInfo.Rows[0]["container_id"].ToString();
                //根据容器主档id，获取容器主档信息
                DataRow dtrContainerInfo = Container.GetContainerH(container_htid);
                //查询容器明细
                DataTable dtContainerDInfo = ce.ToDataTable(Container.GetDetailByHeadID(container_htid));
                #endregion

                #region 修改容器主档是否允许被呼叫
                //修改容器主档是否允许被呼叫
                ce.UpdateContainerAllowCalled(container_htid, model.IsCanBeCallOut);
                #endregion

                //获取AGV
                DataRow drAGV = ce.GetAgv();

                string taskHtId = SnowflakeID.Guid().ToString();//主键ID

                #region 9.创建入库任务

                #region 插入任务主档
                taskModel = new Model.WorkTaskModel()
                {
                    ID = taskHtId,//主键ID
                    ZoneID = zoneId,//区域ID
                    TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                    //DependentTaskNo = "",//依赖任务号
                    TaskType = TaskType.TaskType_InBound.ToString(),//任务类型编码"机台退余料"
                    ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                    //ExecutedBy = "",//任务执行人
                    EquipmentID = drAGV["t_id"].ToString(),//执行设备ID
                    //EquipmentType = "",//执行设备类型
                    //ActualEquipmentID = "",//实际执行设备ID
                    SeqNo = 1,//顺序号
                    Priority = 1,//优先级
                    ContainerID = container_htid,//容器ID
                    ContainerBarcode = dtrContainerInfo["container_barcode"].ToString(),//容器条码
                    RefDocType = DocType.DocType_ClientReturnPallet.ToString(),//来源单据类型  "退余料"
                    RefDocID = doc_id,//来源单ID
                    RefDocDetailID = model.Planno,//来源单明细ID,存放mes叫料号
                    RefDocNo = job_no,//来源单号
                    SrcAddrID = model.Dock,//来源地址ID
                    SrcAddrType = AddressType.AddressType_MachineStation.ToString(),//来源地址类型"dock"
                    SrcAddrText = equipment_code,//来源地址
                    DestAddrID = destAddrID,//目标地址ID
                    DestAddrType = AddressType.AddressType_Station.ToString(),//目标地址类型:输送线站台
                    DestAddrText = destAddrText,//目标地址
                    //ActualDestAddrID = "",//实际目标地址ID
                    //ActualDestAddrText = "",//实际目标地址
                    TaskStatus = TaskStatus.TaskStatus_Executing.ToString(),//任务状态
                    //StartDate = dateTime,//开始时间
                    //EndDate = dateTime,//结束时间
                    IsExecutable = "1",//任务是否可执行
                    //PriorityChangeTime = dateTime,//优先级修改时间        
                    LedText = "",//LED显示内容
                    ExistLgv = model.HasAGV,//是否存在LGV
                    //CreateDate = dateTime,//创建时间
                    //CreatedBy = "",//创建人帐号
                    //ModifyIP = "",//操作人IP 
                    //ModifyDate = dateTime,//编辑时间
                    //LastModifiedBy = "",//最后修改人帐号
                    ActStatus = "1"//状态
                };
                WorkTask.Insert(taskModel);
                #endregion

                for (int i = 0; i < entity["ReturnItems"].Count(); i++)
                {
                    var job = entity["ReturnItems"][i];
                    decimal qty = Convert.ToDecimal(job["qty"].ToString() == "" ? "0" : job["qty"].ToString());
                    string containerDetailId = job["containerDetailId"].ToString();//容器明细id
                    DataRow[] drs = dtContainerDInfo.Select($"t_id='{containerDetailId}'");
                    if (drs.Length == 1)
                    {
                        //修改容器明细中物料数量
                        Container.UpdateDetailQty(containerDetailId, qty);
                    }
                    else
                    {
                        throw new WmsException(500, "无法获取退余料明细数据！");
                    }

                    #region 插入任务物料信息
                    WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
                    {
                        ID = SnowflakeID.Guid().ToString(),//主键ID
                        //ClientID = "",//集团ID
                        //OrgID = "",//组织ID
                        //WarehouseID = "",//仓库ID
                        TaskID = taskHtId,//主任务ID
                        ItemID = drs[0]["item_id"].ToString(),//物料ID
                        ItemName = drs[0]["item_name"].ToString(),//物料名称
                        PlanQty = qty,//计划数量
                        //ProcessedQty = "",//已处理数量
                        //FinishedQty = "",//完成数量
                        UomQtyID = drs[0]["uom_qty_id"].ToString(),//数量单位
                        //FinishedStatQty = "",//完成统计数量
                        UomStatID = drs[0]["uom_stat_id"].ToString(),//统计数量单位
                        ContainerDetailID = drs[0]["t_id"].ToString(),//容器明细ID
                        RefDocType = DocType.DocType_ClientReturnPallet.ToString(),//来源单据类型
                        RefDocID = doc_id,//来源单ID
                        //RefDocDetailID = "",//来源单明细ID
                        RefDocNo = job_no,//来源单号
                        //StartDate = "",//开始时间
                        //EndDate = "",//结束时间
                        ProcessStatus = ProcessStatus.ProcessStatus_New.ToString(),//工作状态
                        //CreateDate = "",//创建时间
                        //CreatedBy = "",//创建人帐号 
                        //ModifyIP = "",//操作人IP 
                        //ModifyDate = "",//编辑时间
                        //LastModifiedBy = "",//最后修改人帐号
                        //ActStatus = ""//状态
                    };
                    WorkTaskItem.Insert(taskItemModel);
                    #endregion
                }

                //发送给wcs
                List<string> _idList = new List<string>();
                _idList.Add(taskModel.ID);
                WorkTaskInterface.SendInBonud(_idList);
                #endregion
            }
            #endregion
        }

        #endregion

        #region 退余料时进行要料
        /// <summary>
        /// 退余料时进行要料
        /// </summary>
        /// <param name="model">退余料参数model</param>
        /// <param name="machineDockTid">站台在机台站台表中的t_id</param>
        /// <param name="equipment_code">站台编码</param>
        /// <param name="job_no">生产工单单号</param>
        /// <param name="taskModel">退余料任务model</param>
        private void CallPalletWhenReturnPallet(ClientReturnPalletModel model, string machineDockTid, string equipment_code, string job_no, Model.WorkTaskModel taskModel)
        {

            MachineMaterialModel mm = new MachineMaterialModel();
            mm.Machine = model.Machine;
            mm.Dock = model.Dock;
            mm.ProductPlanOutId = model.ProductPlanOutId;
            mm.ShiftId = model.ShiftId;
            mm.TeamId = model.TeamId;
            mm.HasAGV = model.HasAGV;
            mm.Planno = model.Planno;
            CreateOutBound(mm, taskModel);

            #region 旧版代码，暂停使用
            //#region 12.获取机台BOM信息
            //DataTable dtBOMInfo = ce.GetBOMInfo(model.Dock);
            //if (dtBOMInfo.Rows.Count <= 0)
            //{
            //    throw new WmsException(500, "无法获取机台BOM信息！");
            //}
            //string bomId = dtBOMInfo.Rows[0]["t_id"].ToString();//bomId
            //#endregion

            ////4.获取机台信息
            //DataRow _drMachineInfo = ce.GetMachineInfoByMachineId(model.Machine);
            ////机台所在区域
            //string machineZoneId = _drMachineInfo["zone_id"].ToString();

            //#region 13.获取货位信息
            ////通过货位分配策略，获取可用货位
            //string _locId = ce.GetLocationInfoByBomId(bomId, machineZoneId);//"1288307405184176201";

            ////查询货位对应区域信息
            //DataTable _dtLocationInfo = ce.GetLocationInfo(_locId);
            ////货位状态
            //string _locationStatus = _dtLocationInfo.Rows[0]["location_status"].ToString();
            ////区域
            //string _zoneId = _dtLocationInfo.Rows[0]["zone_id"].ToString();
            ////货位对应的编码
            //string _locNo = _dtLocationInfo.Rows[0]["location_code"].ToString();

            //if (_locationStatus != LocationStatus.LocationStatus_Stored.ToString())
            //{
            //    throw new WmsException(500, "货位分配策略分配的货位，状态不为已存储状态，无法在退余料时进行要料");
            //}
            //#endregion

            ////查询执行设备(堆垛机)
            //string ddjEquipId = ce.GetDDJ(_locId);

            //#region 14.获取货位对应容器信息
            ////查询库位对应的容器头表信息
            //DataRow[] _dtHeadsInfo = Container.GetHeadsByLocation(_locId);
            //if (_dtHeadsInfo == null || _dtHeadsInfo.Length <= 0)
            //{
            //    throw new WmsException(500, "货位分配返回空货位，无法在退余料时进行要料");
            //}
            //DataRow _dtContainerhInfo = _dtHeadsInfo[0];
            ////容器头表主键
            //string _containerHTid = _dtContainerhInfo["t_id"].ToString();
            ////容器头表编码
            //string _containerBarcode = _dtContainerhInfo["container_barcode"].ToString();
            ////查询容器明细信息
            //DataRow[] _dtrContainerDInfo = Container.GetDetailByHeadID(_containerHTid);
            //#endregion

            //#region 15.产生要料任务，包括修改站台状态
            ////根据站台修改机台站台表中的容器id，并将生产工单id挂在挂站台上，用于退余料、退坏料和退空托盘
            ////UpdateMachineDockDeatilId(machineDockTid, _containerHTid, model.ProductPlanOutId, bomId, string.Empty);
            ////修改站台状态
            //ce.UpdateDockStatus(machineDockTid, model.Dock, EquipmentStatus.EquipmentStatus_Using.ToString());

            //#region 插入任务主档
            //Model.WorkTaskModel _taskModel = new Model.WorkTaskModel()
            //{
            //    ID = SnowflakeID.Guid().ToString(),//主键ID
            //    ZoneID = _zoneId,//区域ID
            //    TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
            //    DependentTaskNo = taskModel.TaskNo,//依赖任务号
            //    TaskType = TaskType.TaskType_OutBound.ToString(),//任务类型编码 "机台要料"
            //    ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
            //    //ExecutedBy = "",//任务执行人
            //    EquipmentID = ddjEquipId,//执行设备ID
            //    //EquipmentType = "",//执行设备类型
            //    //ActualEquipmentID = "",//实际执行设备ID
            //    SeqNo = 1,//顺序号
            //    Priority = 1,//优先级
            //    ContainerID = _containerHTid,//容器ID
            //    ContainerBarcode = _containerBarcode,//容器条码
            //    RefDocType = DocType.DocType_CallTerminal.ToString(),//来源单据类型
            //    RefDocID = model.ProductPlanOutId,//来源单ID
            //    //RefDocDetailID = "",//来源单明细ID
            //    RefDocNo = job_no,//来源单号
            //    SrcAddrID = _locId,//来源地址ID
            //    SrcAddrType = AddressType.AddressType_Location.ToString(),//来源地址类型 "location"
            //    SrcAddrText = _locNo,//来源地址
            //    DestAddrID = model.Dock,//目标地址ID
            //    DestAddrType = AddressType.AddressType_MachineStation.ToString(),//目标地址类型"dock"
            //    DestAddrText = equipment_code,//目标地址
            //    //ActualDestAddrID = "",//实际目标地址ID
            //    //ActualDestAddrText = "",//实际目标地址
            //    TaskStatus = TaskStatus.TaskStatus_NotExecute.ToString(),//任务状态
            //    //StartDate = dateTime,//开始时间
            //    //EndDate = dateTime,//结束时间
            //    IsExecutable = "0",//任务是否可执行
            //    //PriorityChangeTime = dateTime,//优先级修改时间        
            //    LedText = "",//LED显示内容
            //    ExistLgv = model.HasAGV,//是否存在LGV
            //    //CreateDate = dateTime,//创建时间
            //    //CreatedBy = "",//创建人帐号
            //    //ModifyIP = "",//操作人IP 
            //    //ModifyDate = dateTime,//编辑时间
            //    //LastModifiedBy = "",//最后修改人帐号
            //    ActStatus = "1",//状态
            //};
            //WorkTask.Insert(taskModel);
            //#endregion

            //for (int i = 0; i < _dtrContainerDInfo.Length; i++)
            //{
            //    decimal qty = Convert.ToDecimal(_dtrContainerDInfo[i]["qty"].ToString() == "" ? "0" : _dtrContainerDInfo[i]["qty"].ToString());
            //    //修改容器明细中物料待出数量
            //    Container.UpdateDetailOutQty(_dtrContainerDInfo[i]["t_id"].ToString(), qty);
            //    #region 插入任务物料信息
            //    WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
            //    {
            //        ID = SnowflakeID.Guid().ToString(),//主键ID
            //        //ClientID = "",//集团ID
            //        //OrgID = "",//组织ID
            //        //WarehouseID = "",//仓库ID
            //        TaskID = _taskModel.ID,//主任务ID
            //        ItemID = _dtrContainerDInfo[i]["item_id"].ToString(),//物料ID
            //        ItemName = _dtrContainerDInfo[i]["item_name"].ToString(),//物料名称
            //        PlanQty = qty,//计划数量
            //        //ProcessedQty = "",//已处理数量
            //        //FinishedQty = "",//完成数量
            //        UomQtyID = _dtrContainerDInfo[i]["uom_qty_id"].ToString(),//数量单位
            //        //FinishedStatQty = "",//完成统计数量
            //        UomStatID = _dtrContainerDInfo[i]["uom_stat_id"].ToString(),//统计数量单位
            //        ContainerDetailID = _dtrContainerDInfo[i]["t_id"].ToString(),//容器明细ID
            //        RefDocType = DocType.DocType_CallTerminal.ToString(),//来源单据类型  "机台要料"
            //        RefDocID = model.ProductPlanOutId,//来源单ID
            //        //RefDocDetailID = "",//来源单明细ID
            //        RefDocNo = job_no,//来源单号
            //        //StartDate = "",//开始时间
            //        //EndDate = "",//结束时间
            //        ProcessStatus = TaskStatus.TaskStatus_NotExecute.ToString()//工作状态
            //        //CreateDate = "",//创建时间
            //        //CreatedBy = "",//创建人帐号 
            //        //ModifyIP = "",//操作人IP 
            //        //ModifyDate = "",//编辑时间
            //        //LastModifiedBy = "",//最后修改人帐号
            //        //ActStatus = ""//状态
            //    };
            //    WorkTaskItem.Insert(taskItemModel);
            //    #endregion
            //}
            //#endregion 
            #endregion
        }
        #endregion

        #region 二楼退余料，实际是第一次货位分配
        /// <summary>
        /// 二楼退余料，实际上是实托盘入库，此时需要进行两次货位分配
        /// 第一次分配到行
        /// 第二次分配到实际的货位
        /// 本方法为第一次货位分配后生成的任务
        /// </summary>
        /// <param name="model"></param>
        /// <param name="equipment_code"></param>
        /// <param name="line"></param>
        /// <param name="productPlanOutId"></param>
        /// <param name="taskModel"></param>
        private void TwoFloorReturnPallet(ClientReturnPalletModel model, string equipment_code, string line, string productPlanOutId, out Model.WorkTaskModel taskModel)
        {
            #region 5.将退余料json信息转化成json对象
            //退余料数据信息
            JObject entity = JObject.Parse(model.Details);
            if (entity["ReturnItems"].Count() == 0)
            {
                throw new WmsException(1, "无法获取退余料明细数据！");
            }
            #endregion

            #region 7.获取要料时的生产工单计划信息
            DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(productPlanOutId);
            if (dtProductPlanInfo == null || dtProductPlanInfo.Length < 1)
            {
                throw new WmsException(1, "未查询到该机台对应的生产工单！");
            }
            string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态
            string job_no = dtProductPlanInfo[0]["job_no"].ToString();
            string product_item_id = dtProductPlanInfo[0]["item_id"].ToString();//成品物料id
            string product_item_code = dtProductPlanInfo[0]["item_code"].ToString();//成品编码
            string product_item_name = dtProductPlanInfo[0]["item_name"].ToString();//成品名称
            #endregion

            #region 6.获取容器主档信息，容器明细信息
            //获取挂在当前站台上容器id
            DataTable containerHInfo = ce.GetContainerInfoByDockIdAndMachineId(model.Dock, model.Machine);
            if (containerHInfo.Rows.Count > 1)
            {
                throw new WmsException(1, "根据机台和站台，获取挂在站台上面的容器主档不唯一");
            }
            string container_htid = containerHInfo.Rows[0]["container_id"].ToString();
            //根据容器主档id，获取容器主档信息
            DataRow dtrContainerInfo = Container.GetContainerH(container_htid);
            //查询容器明细
            DataTable dtContainerDInfo = ce.ToDataTable(Container.GetDetailByHeadID(container_htid));
            #endregion

            //修改容器主档是否允许被呼叫
            ce.UpdateContainerAllowCalled(container_htid, model.IsCanBeCallOut);

            DataRow drAGV = ce.GetAgv();

            #region 8.创建入库任务

            #region 插入任务主档
            taskModel = new Model.WorkTaskModel()
            {
                ID = SnowflakeID.Guid().ToString(),//主键ID
                ZoneID = WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336"),//区域ID
                TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                //DependentTaskNo = "",//依赖任务号
                TaskType = TaskType.TaskType_InBound.ToString(),//任务类型编码"机台退余料"
                ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                //ExecutedBy = "",//任务执行人
                EquipmentID = drAGV["t_id"].ToString(),//执行设备ID
                //EquipmentType = "",//执行设备类型
                //ActualEquipmentID = "",//实际执行设备ID
                SeqNo = 1,//顺序号
                Priority = 1,//优先级
                ContainerID = container_htid,//容器ID
                ContainerBarcode = dtrContainerInfo["container_barcode"].ToString(),//容器条码
                RefDocType = DocType.DocType_ClientReturnPallet.ToString(),//来源单据类型  "机台退余料"
                RefDocID = productPlanOutId,//来源单ID
                RefDocDetailID = model.Planno,//来源单明细ID,存放mes叫料号
                RefDocNo = job_no,//来源单号
                SrcAddrID = model.Dock,//来源地址ID
                SrcAddrType = AddressType.AddressType_MachineStation.ToString(),//来源地址类型"dock"
                SrcAddrText = equipment_code,//来源地址
                DestAddrID = line,//目标地址ID
                DestAddrType = AddressType.AddressType_Location.ToString(),//目标地址类型"location"
                DestAddrText = line,//目标地址
                //ActualDestAddrID = "",//实际目标地址ID
                //ActualDestAddrText = "",//实际目标地址
                TaskStatus = TaskStatus.TaskStatus_Executing.ToString(),//任务状态
                //StartDate = dateTime,//开始时间
                //EndDate = dateTime,//结束时间
                IsExecutable = "1",//任务是否可执行
                //PriorityChangeTime = dateTime,//优先级修改时间        
                LedText = "",//LED显示内容
                ExistLgv = model.HasAGV,//是否存在LGV
                //CreateDate = dateTime,//创建时间
                //CreatedBy = "",//创建人帐号
                //ModifyIP = "",//操作人IP 
                //ModifyDate = dateTime,//编辑时间
                //LastModifiedBy = "",//最后修改人帐号
                ActStatus = "1"//状态
            };
            WorkTask.Insert(taskModel);
            #endregion

            for (int i = 0; i < entity["ReturnItems"].Count(); i++)
            {
                var job = entity["ReturnItems"][i];
                decimal qty = Convert.ToDecimal(job["qty"].ToString() == "" ? "0" : job["qty"].ToString());
                string containerDetailId = job["containerDetailId"].ToString();//容器明细id
                DataRow[] drs = dtContainerDInfo.Select($"t_id='{containerDetailId}'");
                if (drs.Length == 1)
                {
                    //修改容器明细中物料数量
                    Container.UpdateDetailQty(containerDetailId, qty);
                }
                else
                {
                    throw new WmsException(500, "无法获取退余料明细数据！");
                }

                #region 插入任务物料信息
                WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                    //ClientID = "",//集团ID
                    //OrgID = "",//组织ID
                    //WarehouseID = "",//仓库ID
                    TaskID = taskModel.ID,//主任务ID
                    ItemID = drs[0]["item_id"].ToString(),//物料ID
                    ItemName = drs[0]["item_name"].ToString(),//物料名称
                    PlanQty = qty,//计划数量
                    //ProcessedQty = "",//已处理数量
                    //FinishedQty = "",//完成数量
                    UomQtyID = drs[0]["uom_qty_id"].ToString(),//数量单位
                    //FinishedStatQty = "",//完成统计数量
                    UomStatID = drs[0]["uom_stat_id"].ToString(),//统计数量单位
                    ContainerDetailID = drs[0]["t_id"].ToString(),//容器明细ID
                    RefDocType = DocType.DocType_ClientReturnPallet.ToString(),//来源单据类型
                    RefDocID = productPlanOutId,//来源单ID
                    //RefDocDetailID = "",//来源单明细ID
                    RefDocNo = job_no,//来源单号
                    //StartDate = "",//开始时间
                    //EndDate = "",//结束时间
                    ProcessStatus = ProcessStatus.ProcessStatus_New.ToString(),//工作状态
                    //CreateDate = "",//创建时间
                    //CreatedBy = "",//创建人帐号 
                    //ModifyIP = "",//操作人IP 
                    //ModifyDate = "",//编辑时间
                    //LastModifiedBy = "",//最后修改人帐号
                    //ActStatus = ""//状态
                };
                WorkTaskItem.Insert(taskItemModel);
                #endregion
            }
            #endregion

            //发送给wcs
            List<string> _idList = new List<string>();
            _idList.Add(taskModel.ID);
            WorkTaskInterface.SendInBonud(_idList);
        }
        #endregion

        #endregion

        #region 退余料任务强制完成
        /// <summary>
        /// 退余料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        public void FinishClientReturnPallet(FinishClientReturnPalletModel model)
        {

            #region 1.获取退余料任务信息
            //获取退余料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退余料任务！");
            }
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ClientReturnPallet.ToString())
            {
                throw new WmsException(500, "该任务不是退余料任务，无法强制完成！");
            }
            //if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString() && taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            //{
            //    throw new WmsException(500, "任务仅在处于执行中或者已部分完成状态下，才能强制完成！");
            //}
            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "任务已完成，无需再次完成！");
            }
            #endregion
            //任务不是部分完成，则执行部分完成
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                PartionFinishClientReturnPalletModel pft = new PartionFinishClientReturnPalletModel();
                pft.TaskTid = model.TaskTid;
                PartionFinishClientReturnPallet(pft);
            }
            //执行完成任务
            FinishReturnPalletTaskModel ft = new FinishReturnPalletTaskModel();
            ft.TaskTid = model.TaskTid;
            FinishReturnPalletTask(ft);

            #region 旧版强完任务代码
            //#region 1.获取退余料任务信息
            ////获取退余料任务信息
            //DataTable dtTaskInfo = GeTaskInfoByTaskId(model.TaskTid);
            //if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            //{
            //    throw new WmsException(500, "未查询到站台退余料任务！");
            //}
            //string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            //string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            //string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            //string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            //string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退余料时托盘目的库位id
            //string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            //string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            //if (refDocType != DocType.DocType_ClientReturnPallet.ToString())
            //{
            //    throw new WmsException(500, "该任务不是退余料任务，无法强制完成！");
            //}
            //if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString() && taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            //{
            //    throw new WmsException(500, "任务仅在处于执行中或者已部分完成状态下，才能强制完成！");
            //}
            //#endregion

            //#region 2.获取任务物料明细
            ////获取任务物料明细
            //DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
            //#endregion

            //#region 3.检查站台状态
            ////检查站台状态
            //DataRow dtDockInfo = GetDockStatus(dockId);
            //string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            //string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            //string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            ////if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
            ////{
            ////    throw new WmsException(500, "当前站台位处于故障状态，无法使用！");
            ////}
            ////if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
            ////{
            ////    throw new WmsException(500, "当前站台对应的工艺段已停用，无法进行退余料！");
            ////}
            //#endregion

            //#region 4.获取挂在站台上的容器明细
            ////获取挂在站台上的容器明细
            //DataRow[] dtrContainerDInfo = Container.GetDetailByHeadID(container_id);
            //#endregion

            //#region 5.将容器明细物料数量为0，则软删除容器明细
            ////容器明细中物料数量qty全为0的情况，已在前台界面限制，走退空托盘业务
            ////该目的是在统计消耗时直接使用出库数量减去容器明细中的数量，作为消耗量
            //for (int i = 0; i < dtrContainerDInfo.Length; i++)
            //{
            //    string containerDetailId = dtrContainerDInfo[i]["t_id"].ToString();
            //    decimal qty = Convert.ToDecimal(dtrContainerDInfo[i]["qty"].ToString() == "" ? "0" : dtrContainerDInfo[i]["qty"].ToString());
            //    if (qty <= 0)//物料数量为0，则删除容器明细，设置成软删除
            //    {
            //        Container.UpdateDetailActStatus(containerDetailId, "0");
            //    }
            //    //else//待入量不为0，则将容器明细待入量写入数量字段，并清空待入数量
            //    //{
            //    //    Container.UpdateDetailQty(containerDetailId, qty).UpdateDetailInQty(containerDetailId, (decimal)0.0);
            //    //}
            //}
            //#endregion

            //#region 6.将容器主档与库位关联
            ////将容器主档与库位关联
            //Container.UpdateContainerLocation(container_id, locId);
            //#endregion

            //#region 7.更新退余料任务和任务明细
            ////将任务更新为完成
            //WorkTask.UpdateTaskComplete(model.TaskTid);
            ////更新任务物料明细中的已处理数量和完成数量
            //for (int i = 0; i < taskItemInfos.Length; i++)
            //{
            //    string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
            //    //根据任务物料id，修改任务物料状态为已完成，并且修改完成和处理数量
            //    WorkTask.UpdateTaskItemCompleteAndQty(cdTid);
            //}
            //WorkTask.MoveToHistory(model.TaskTid);
            //#endregion

            //#region 8.清空挂在站台上的容器
            ////清空挂在站台上的容器
            //UpdateMachineDockDeatilId(machineDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            //#endregion

            //#region 9.修改站台状态
            ////修改站台状态
            //UpdateDockStatus(machineDockTid, dockId, EquipmentStatus.EquipmentStatus_OK.ToString());
            //#endregion

            //#region 10.修改货位状态为已存储
            ////修改货位状态为已存储
            //UpdateLocStatusByLocationId(locId, LocationStatus.LocationStatus_Stored.ToString(), LocationStatus.LocationStatus_In.ToString(), "机台退余料,物料入库完成,修改货位状态为已存储");
            //#endregion 
            #endregion
        }
        #endregion

        #region 退余料任务回收
        /// <summary>
        /// 退余料任务回收
        /// </summary>
        /// <param name="model"></param>
        public void FirstRecycleClientReturnPallet(RecycleClientReturnPalletModel model)
        {
            #region 1.获取退余料任务信息
            //获取退余料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退余料任务！");
            }
            string taskNo = dtTaskInfo.Rows[0]["task_no"].ToString();//任务号
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string taskZoneId = dtTaskInfo.Rows[0]["zone_id"].ToString();//任务中的区域
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退余料时托盘目的库位id
            string destAddrText = dtTaskInfo.Rows[0]["dest_addr_text"].ToString();//任务中，目的货位编号
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ClientReturnPallet.ToString())
            {
                throw new WmsException(500, "该任务不是退余料任务，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "该任务已完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "该任务已部分完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "该任务正在执行中，无法回收任务！");
            }
            #endregion

            //二楼，第一次货位分配时需要回收任务
            if (string.Equals(taskZoneId, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))
            {
                if (destAddrText.Length <= 3)//一次货位分配
                {
                    RecycleInBoundFirstDistributionLocationTask(dtTaskInfo.Rows[0]);
                    //修改任务软删除标记为“0”
                    WorkTask.UpdateActStatus(model.TaskTid, "0");
                }
                else//二次货位分配
                {
                    RecycleInBoundSecoundDistributionLocationTask(dtTaskInfo.Rows[0]);
                    #region 删除任务，恢复容器明细中的物料数量
                    RecycleClientReturnPallentTaskAndContainer(model, container_id, productPlanOutId);
                    #endregion
                }
                return;
            }

            RecycleClientReturnPallentTaskAndContainer(model, container_id, productPlanOutId);

            #region 5.任务回收时，修改货位状态为空
            //修改货位状态为空
            ce.UpdateLocStatusByLocationId(locId, LocationStatus.LocationStatus_Empty.ToString(), LocationStatus.LocationStatus_In.ToString(), "机台退余料任务回收,物料无需入库,修改货位状态为空");
            #endregion

            #region 6.存在依赖任务，则回收。否则忽略此步
            DataTable _dtTask = ce.GetDependentTask(taskNo);
            //存在依赖任务
            if (_dtTask.Rows.Count > 0)
            {
                string _taskId = _dtTask.Rows[0]["t_id"].ToString();
                RecycleMachineMaterialModel rmt = new RecycleMachineMaterialModel();
                rmt.TaskTid = _taskId;
                RecycleMachineMaterial(rmt);
            }
            #endregion

        }

        #region 回收退余料任务时删除任务，并恢复容器明细的物料数量
        /// <summary>
        /// 回收退余料任务时删除任务，并恢复容器明细的物料数量
        /// </summary>
        /// <param name="model"></param>
        /// <param name="container_id"></param>
        /// <param name="productPlanOutId"></param>
        private void RecycleClientReturnPallentTaskAndContainer(RecycleClientReturnPalletModel model, string container_id, string productPlanOutId)
        {
            #region 2.获取任务物料明细
            //获取任务物料明细
            DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
            #endregion

            #region 3.获取要料或者接班任务，并恢复退余料任务产生前容器中的物料数量
            //获取要料或者接班任务
            DataTable theNewTaskItemInfo = ce.GetTheNewTaskInfoByPlanOutId(productPlanOutId);

            //根据要料或者接班任务，恢复退余料任务产生前容器中的物料数量
            for (int i = 0; i < theNewTaskItemInfo.Rows.Count; i++)
            {
                string cdTid = theNewTaskItemInfo.Rows[i]["container_d_id"].ToString();
                decimal qty = Convert.ToDecimal(theNewTaskItemInfo.Rows[i]["plan_qty"].ToString().ToString() == "" ? "0" : theNewTaskItemInfo.Rows[i]["plan_qty"].ToString());
                Container.UpdateDetailQty(cdTid, qty);
            }
            //修改容器主档允许被呼叫
            ce.UpdateContainerAllowCalled(container_id, "1");
            #endregion

            #region 4.更新任务和任务明细软删除标记为“0”
            //修改任务软删除标记为“0”
            WorkTask.UpdateActStatus(model.TaskTid, "0");
            //更新任务物料软删除标记为“0”
            for (int i = 0; i < taskItemInfos.Length; i++)
            {
                string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
                string containerDTid = taskItemInfos[i]["container_d_id"].ToString();//容器明细id
                                                                                     //修改任务物料表中的软删除标记为“0”
                WorkTask.UpdateTaskItemActStatus(cdTid, "0");
            }
            #endregion
        }
        #endregion

        //FirstRecycleClientReturnPallet   RecycleClientReturnPallet
        #region 20201222回收第一个退余料任务
        /// <summary>
        /// 20201222回收第一个退余料任务
        /// </summary>
        /// <param name="model"></param>
        public void RecycleClientReturnPallet(RecycleClientReturnPalletModel model)
        {
            #region 1.获取退余料任务信息
            //获取退余料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退余料任务！");
            }
            string taskNo = dtTaskInfo.Rows[0]["task_no"].ToString();//任务号
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string taskZoneId = dtTaskInfo.Rows[0]["zone_id"].ToString();//任务中的区域
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退余料时托盘目的库位id
            string destAddrText = dtTaskInfo.Rows[0]["dest_addr_text"].ToString();//任务中，目的货位编号
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ClientReturnPallet.ToString())
            {
                throw new WmsException(500, "该任务不是退余料任务，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "该任务已完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "该任务已部分完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "该任务正在执行中，无法回收任务！");
            }
            #endregion

            //二楼，第一次货位分配时需要回收任务
            if (string.Equals(taskZoneId, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))
            {
                if (destAddrText.Length <= 3)//一次货位分配
                {
                    RecycleInBoundFirstDistributionLocationTask(dtTaskInfo.Rows[0]);
                    //修改任务软删除标记为“0”
                    WorkTask.UpdateActStatus(model.TaskTid, "0");
                }
                else//二次货位分配
                {
                    RecycleInBoundSecoundDistributionLocationTask(dtTaskInfo.Rows[0]);
                    #region 删除任务，恢复容器明细中的物料数量
                    RecycleClientReturnPallentTaskAndContainer(model, container_id, productPlanOutId);
                    #endregion
                }
                return;
            }

            RecycleClientReturnPallentTaskAndContainer(model, container_id, productPlanOutId);

            #region 6.存在依赖任务，则回收。否则忽略此步
            DataTable _dtTask = ce.GetDependentTask(taskNo);
            //存在依赖任务
            if (_dtTask.Rows.Count > 0)
            {
                string _taskId = _dtTask.Rows[0]["t_id"].ToString();
                RecycleMachineMaterialModel rmt = new RecycleMachineMaterialModel();
                rmt.TaskTid = _taskId;
                RecycleMachineMaterial(rmt);
            }
            #endregion
        }
        #endregion

        #endregion

        #region 退余料任务部分完成
        /// <summary>
        /// 退余料任务部分完成
        /// </summary>
        /// <param name="model"></param>
        public void FirstPartionFinishClientReturnPallet(PartionFinishClientReturnPalletModel model)
        {
            #region 1.获取退余料任务信息
            //获取退余料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退余料任务！");
            }
            string taskNo = dtTaskInfo.Rows[0]["task_no"].ToString();//任务号
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退余料时托盘目的库位id
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ClientReturnPallet.ToString())
            {
                throw new WmsException(500, "该任务不是退余料任务，无法部分完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "任务仅在处于执行中状态下，才能部分完成！");
            }
            #endregion

            #region 7.将任务修改为部分完成
            WorkTask.UpdateTaskStatus(model.TaskTid, TaskStatus.TaskStatus_PartialFinished.ToString(), TaskStatus.TaskStatus_Executing.ToString());
            #endregion

            #region 2.存在依赖任务，则修改是否可执行状态，并下发任务。否则忽略此步
            DataTable _dtTask = ce.GetDependentTask(taskNo);
            //存在依赖任务
            if (_dtTask.Rows.Count > 0)
            {
                string _taskId = _dtTask.Rows[0]["t_id"].ToString();
                ce.UpdateTaskIsExecutableStatus(_taskId);
                //发送给wcs
                SendToWCS(_taskId);
                return;
            }
            #endregion

            #region 3.检查站台状态
            //检查站台状态
            DataRow dtDockInfo = ce.GetDockStatus(dockId);
            string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            //if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
            //{
            //    throw new WmsException(500, "当前站台位处于故障状态，无法使用！");
            //}
            //if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
            //{
            //    throw new WmsException(500, "当前站台对应的工艺段已停用，无法进行退余料！");
            //}
            #endregion

            #region 4.获取挂在站台上的容器明细
            //获取挂在站台上的容器明细
            DataRow[] dtrContainerDInfo = Container.GetDetailByHeadID(container_id);
            #endregion

            #region 5.修改容器主档是否允许呼叫
            //修改容器主档是否允许呼叫
            //Container.UpdateHeadIsAllowCalled(container_id, WmsConfig.GetOrDefault("CallEnd:IsAllowedCall", "0"));
            #endregion

            //容器关联货位
            Container.UpdateContainerLocation(container_id, locId);

            //恢复已删除的容器主档为1
            Container.UpdateHeadActStatus(container_id, "1");

            #region 5.将容器明细物料数量为0，则软删除容器明细
            //容器明细中物料数量qty全为0的情况，已在前台界面限制，走退空托盘业务
            //该目的是在统计消耗时直接使用出库数量减去容器明细中的数量，作为消耗量
            for (int i = 0; i < dtrContainerDInfo.Length; i++)
            {
                string containerDetailId = dtrContainerDInfo[i]["t_id"].ToString();
                decimal qty = Convert.ToDecimal(dtrContainerDInfo[i]["qty"].ToString() == "" ? "0" : dtrContainerDInfo[i]["qty"].ToString());
                if (qty <= 0)//物料数量为0，则删除容器明细，设置成软删除
                {
                    Container.UpdateDetailActStatus(containerDetailId, "0");
                }
            }
            #endregion

            #region 6.修改站台状态
            //修改站台状态
            ce.UpdateDockStatus(machineDockTid, dockId, EquipmentStatus.EquipmentStatus_OK.ToString());
            #endregion

            #region 8.清空挂在站台上的容器
            //清空挂在站台上的容器
            ce.UpdateMachineDockDeatilId(machineDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            #endregion
        }

        //FirstPartionFinishClientReturnPallet   PartionFinishClientReturnPallet

        #region 20201222退余料任务部分完成
        public void PartionFinishClientReturnPallet(PartionFinishClientReturnPalletModel model)
        {
            #region 1.获取退余料任务信息
            //获取退余料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退余料任务！");
            }
            string taskNo = dtTaskInfo.Rows[0]["task_no"].ToString();//任务号
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退余料时托盘目的库位id
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ClientReturnPallet.ToString())
            {
                throw new WmsException(500, "该任务不是退余料任务，无法部分完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "任务仅在处于执行中状态下，才能部分完成！");
            }
            #endregion

            #region 7.将任务修改为部分完成
            WorkTask.UpdateTaskStatus(model.TaskTid, TaskStatus.TaskStatus_PartialFinished.ToString(), TaskStatus.TaskStatus_Executing.ToString());
            #endregion

            #region 2.存在依赖任务，则修改是否可执行状态，并下发任务。否则忽略此步
            DataTable _dtTask = ce.GetDependentTask(taskNo);
            //存在依赖任务
            if (_dtTask.Rows.Count > 0)
            {
                string _taskId = _dtTask.Rows[0]["t_id"].ToString();
                ce.UpdateTaskIsExecutableStatus(_taskId);
                //发送给wcs
                SendToWCS(_taskId);
                return;
            }
            #endregion

            #region 3.检查站台状态
            //检查站台状态
            DataRow dtDockInfo = ce.GetDockStatus(dockId);
            string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            //if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
            //{
            //    throw new WmsException(500, "当前站台位处于故障状态，无法使用！");
            //}
            //if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
            //{
            //    throw new WmsException(500, "当前站台对应的工艺段已停用，无法进行退余料！");
            //}
            #endregion

            #region 4.获取挂在站台上的容器明细
            //获取挂在站台上的容器明细
            DataRow[] dtrContainerDInfo = Container.GetDetailByHeadID(container_id);
            #endregion

            //获取暂存区货位
            DataRow drInboundBuffer = new Location().GetInboundBufferLocation();

            //容器关联货位
            Container.UpdateContainerLocation(container_id, drInboundBuffer["t_id"].ToString());

            //恢复已删除的容器主档为1
            Container.UpdateHeadActStatus(container_id, "1");

            #region 5.将容器明细物料数量为0，则软删除容器明细
            //容器明细中物料数量qty全为0的情况，已在前台界面限制，走退空托盘业务
            //该目的是在统计消耗时直接使用出库数量减去容器明细中的数量，作为消耗量
            for (int i = 0; i < dtrContainerDInfo.Length; i++)
            {
                string containerDetailId = dtrContainerDInfo[i]["t_id"].ToString();
                decimal qty = Convert.ToDecimal(dtrContainerDInfo[i]["qty"].ToString() == "" ? "0" : dtrContainerDInfo[i]["qty"].ToString());
                if (qty <= 0)//物料数量为0，则删除容器明细，设置成软删除
                {
                    Container.UpdateDetailActStatus(containerDetailId, "0");
                }
            }
            #endregion

            #region 6.修改站台状态
            //修改站台状态
            ce.UpdateDockStatus(machineDockTid, dockId, EquipmentStatus.EquipmentStatus_OK.ToString());
            #endregion

            #region 8.清空挂在站台上的容器
            //清空挂在站台上的容器
            ce.UpdateMachineDockDeatilId(machineDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            #endregion
        }
        #endregion


        #endregion

        #region 退余料任务完成
        /// <summary>
        /// 退余料任务完成
        /// </summary>
        /// <param name="model"></param>
        public void FirstFinishReturnPalletTask(FinishReturnPalletTaskModel model)
        {
            #region 1.获取退余料任务信息
            //获取退余料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退余料任务！");
            }
            string taskNo = dtTaskInfo.Rows[0]["task_no"].ToString();//任务号
            string zone_id = dtTaskInfo.Rows[0]["zone_id"].ToString();//区域id
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退余料时托盘目的库位id
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ClientReturnPallet.ToString())
            {
                throw new WmsException(500, "该任务不是退余料任务，无法完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "任务仅在处于已部分完成状态下，才能完成！");
            }
            #endregion

            #region 2.获取任务物料明细
            //获取任务物料明细
            DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
            #endregion

            #region 3.将容器主档与库位关联
            //将容器主档与库位关联
            Container.UpdateContainerLocation(container_id, locId);
            Container.UpdateContainerIsFull(container_id, "0");
            #endregion

            #region 4.更新退余料任务和任务明细
            //将任务更新为完成
            WorkTask.UpdateTaskComplete(model.TaskTid);
            //更新任务物料明细中的已处理数量和完成数量
            for (int i = 0; i < taskItemInfos.Length; i++)
            {
                string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
                //根据任务物料id，修改任务物料状态为已完成，并且修改完成和处理数量
                WorkTask.UpdateTaskItemCompleteAndQty(cdTid);
            }
            WorkTask.MoveToHistory(model.TaskTid);
            #endregion

            #region 5.修改货位状态为已存储
            //修改货位状态为已存储
            ce.UpdateLocStatusByLocationId(locId, LocationStatus.LocationStatus_Stored.ToString(), LocationStatus.LocationStatus_In.ToString(), "机台退余料,物料入库完成,修改货位状态为已存储");
            #endregion

            if (string.Equals(zone_id, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))
            {
                Location lo = new Location();
                LocationModel _locInfo = lo.GetLocationById(locId);
                bool isLineFull = ce.IsLineFull(_locInfo.location_row.ToString());
                if (isLineFull == true)
                {
                    //lo.UpdateLocationStatus(_locInfo.t_id, LocationStatus.LocationStatus_Stored);
                    lo.UpdateLocationFlag(_locInfo.location_row.ToString(), LocationFlag.LocationFlag_Usable.ToString(), LocationFlag.LocationFlag_InOnly.ToString());
                }
            }

            #region 6.判断退余料任务完成时，依赖该任务的要料任务，是否允许完成如果允许，则完成要料任务。否则忽略此步
            DataTable _dtTask = ce.GetDependentTask(taskNo);
            //存在依赖任务
            if (_dtTask.Rows.Count > 0 && string.Equals(_dtTask.Rows[0]["LED_TEXT"].ToString(), "55555555"))
            {

                string _taskId = _dtTask.Rows[0]["t_id"].ToString();
                FinishMaterialModel rmt = new FinishMaterialModel();
                rmt.TaskTid = _taskId;
                FinishMaterial(rmt);
            }
            #endregion
        }

        //FirstFinishReturnPalletTask   FinishReturnPalletTask

        #region 20201222退余料任务完成
        public void FinishReturnPalletTask(FinishReturnPalletTaskModel model)
        {
            #region 1.获取退余料任务信息
            //获取退余料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退余料任务！");
            }
            string taskNo = dtTaskInfo.Rows[0]["task_no"].ToString();//任务号
            string zone_id = dtTaskInfo.Rows[0]["zone_id"].ToString();//区域id
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退余料时托盘目的库位id
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ClientReturnPallet.ToString())
            {
                throw new WmsException(500, "该任务不是退余料任务，无法完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "任务仅在处于已部分完成状态下，才能完成！");
            }
            #endregion

            #region 2.获取任务物料明细
            //获取任务物料明细
            DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
            #endregion

            #region 3.将容器主档与库位关联
            ////将容器主档与库位关联
            //Container.UpdateContainerLocation(container_id, locId);
            Container.UpdateContainerIsFull(container_id, "0");
            #endregion

            #region 4.更新退余料任务和任务明细
            //将任务更新为完成
            WorkTask.UpdateTaskComplete(model.TaskTid);
            //更新任务物料明细中的已处理数量和完成数量
            for (int i = 0; i < taskItemInfos.Length; i++)
            {
                string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
                //根据任务物料id，修改任务物料状态为已完成，并且修改完成和处理数量
                WorkTask.UpdateTaskItemCompleteAndQty(cdTid);
            }
            WorkTask.MoveToHistory(model.TaskTid);
            #endregion

            if (string.Equals(zone_id, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))
            {
                Location lo = new Location();
                LocationModel _locInfo = lo.GetLocationById(locId);
                bool isLineFull = ce.IsLineFull(_locInfo.location_row.ToString());
                if (isLineFull == true)
                {
                    //lo.UpdateLocationStatus(_locInfo.t_id, LocationStatus.LocationStatus_Stored);
                    lo.UpdateLocationFlag(_locInfo.location_row.ToString(), LocationFlag.LocationFlag_Usable.ToString(), LocationFlag.LocationFlag_InOnly.ToString());
                }
            }

            #region 6.判断退余料任务完成时，依赖该任务的要料任务，是否允许完成如果允许，则完成要料任务。否则忽略此步
            DataTable _dtTask = ce.GetDependentTask(taskNo);
            //存在依赖任务
            if (_dtTask.Rows.Count > 0 && string.Equals(_dtTask.Rows[0]["LED_TEXT"].ToString(), "55555555"))
            {

                string _taskId = _dtTask.Rows[0]["t_id"].ToString();
                FinishMaterialModel rmt = new FinishMaterialModel();
                rmt.TaskTid = _taskId;
                FinishMaterial(rmt);
            }
            #endregion
        }
        #endregion

        #endregion

        #region 人工领料确认
        /// <summary>
        /// 人工领料确认
        /// </summary>
        /// <param name="model"></param>
        public void ConfirmPersonReceiveMateriel(ConfirmPersonReceiveMaterielModel model)
        {
            #region 1.查询人工领料主档信息
            //查询人工领料主档信息
            DataRow drPickReturnHInfo = ce.SelectPickReturnHById(model.PickReturn_htid);
            string pick_return_type = drPickReturnHInfo["pick_return_type"].ToString();//单据类型
            string pick_return_status = drPickReturnHInfo["pick_return_status"].ToString();//单据状态
            string pick_return_no = drPickReturnHInfo["pick_return_no"].ToString();//单据号
            string job_no = drPickReturnHInfo["job_no"].ToString();//生产工单号
            string shift_id = drPickReturnHInfo["shift_id"].ToString();//班次
            string team_id = drPickReturnHInfo["team_id"].ToString();//班组
            string equipment_id = drPickReturnHInfo["equipment_id"].ToString();//机台 
            string dockId = drPickReturnHInfo["dock_id"].ToString();//人工领料站台id
            string dockCode = drPickReturnHInfo["dock_code"].ToString();//人工领料站台编码
            if (pick_return_type != PickReturnType.PickReturnType_RGLL.ToString())
            {
                throw new WmsException(500, "该单据不是人工领料单据");
            }
            if (pick_return_status != PickReturnStatus.PickReturnStatus_New.ToString())
            {
                throw new WmsException(500, "该人工领料单据不是新建状态，无法确认");
            }
            #endregion

            #region 2.获取人工领料站台在机台站台表中的信息
            DataRow dtDockInfo = ce.GetJieBDockByMachine(equipment_id);
            string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            string container_ids = dtDockInfo["container_id"].ToString();//挂在站台上的容器主档id(使用“,”分割)
            #endregion

            #region 3.获取人工领料单据中的生产工单计划信息
            //获取人工领料单据中的生产工单计划信息
            DataRow drpp = ce.GetProductPlanInfoByNo(job_no);//人工领料单中的生产工单信息
            if (drpp["plan_status"].ToString() == PlanOutStatus.PlanOutStatus_Finished.ToString())
            {
                throw new WmsException(500, "该人工领料单中的生产工单已完成，无法再次领料");
            }
            #endregion

            #region 4.获取机台当前生产工单信息，并比对人工领料单据中的生产工单号是否与当前生产工单号是否一致
            ////获取机台中人工领料站台id
            DataRow[] drProductPlanInfo = ce.GetProductPlanInfoByMachineId(equipment_id);
            if (drProductPlanInfo.Length > 1)
            {
                throw new WmsException(500, "该人工领料单中机台对应正在生产中的生产工单数量大于1，无法确认");
            }
            if (drProductPlanInfo.Length == 0)
            {
                throw new WmsException(500, "该人工领料单中机台对应正在生产中的生产工单数量为0，无法确认");
            }
            //string nowjobNo = drProductPlanInfo[0]["job_no"].ToString();//机台，对应的正在生产中的工单号
            //if (!nowjobNo.Equals(job_no))
            //{
            //    throw new WmsException(500, "该人工领料单中生产工单单号，与正在执行中的单号不一致，无法确认");
            //}
            string nowProductPlanId = drProductPlanInfo[0]["t_id"].ToString();//当前生产中的生产工单id
            #endregion

            #region 5.当人工领料站台上挂的生产工单与当前生产工单不一致情况下，清空人工领料站台上的信息
            //当人工领料站台上挂的生产工单与当前生产工单不一致情况下，清空人工领料站台上的信息
            if (!string.Equals(drpp["t_id"].ToString(), nowProductPlanId))
            {
                ce.UpdateMachineDockDeatilId(machineDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
                container_ids = string.Empty;
            }
            #endregion

            #region 6.获取暂存区货位对应的容器主档信息
            //获取暂存区货位对应的容器主档信息
            DataTable dtInboundBufferContainerHInfo = ce.GetContainerHTidOfInboundBufferLoc();
            if (dtInboundBufferContainerHInfo == null || dtInboundBufferContainerHInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "无法暂存区容器主档");
            }
            if (dtInboundBufferContainerHInfo.Rows.Count > 1)
            {
                throw new WmsException(500, "暂存区容器主档不唯一");
            }
            string containerHTid = dtInboundBufferContainerHInfo.Rows[0]["t_id"].ToString();//暂存区货位对应的容器主档id
            string zone_id = dtInboundBufferContainerHInfo.Rows[0]["zone_id"].ToString();//暂存区货位对应的区域id
            string container_barcode = dtInboundBufferContainerHInfo.Rows[0]["container_barcode"].ToString();//容器编码
            string location_code = dtInboundBufferContainerHInfo.Rows[0]["location_code"].ToString();//库位编码
            string loc_id = dtInboundBufferContainerHInfo.Rows[0]["loc_id"].ToString();//库位id 
            #endregion

            #region 7.更新人工领料单据状态
            ce.UpdatePickReturnhStatusById(model.PickReturn_htid, PickReturnStatus.PickReturnStatus_Confirmed.ToString());
            #endregion

            #region 8.获取人工领料单明细信息
            DataTable dtPickReturnDInfo = ce.SelectPickReturnDById(model.PickReturn_htid);
            #endregion

            #region 9.创建新的容器（主档和明细），创建出库任务
            string _containerHTid = SnowflakeID.Guid().ToString();
            dtInboundBufferContainerHInfo.Rows[0]["t_id"] = _containerHTid;
            dtInboundBufferContainerHInfo.Rows[0]["location_id"] = string.Empty;
            dtInboundBufferContainerHInfo.Rows[0]["container_barcode"] = "人工领料";
            Container.InsertHead(dtInboundBufferContainerHInfo.Rows[0]);
            DateTime dateTime = DateTime.Now;
            #region 插入任务主档
            Model.WorkTaskModel taskModel = new Model.WorkTaskModel()
            {
                ID = SnowflakeID.Guid().ToString(),//主键ID
                ZoneID = zone_id,//区域ID
                TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                //DependentTaskNo = "",//依赖任务号
                TaskType = TaskType.TaskType_OutBound.ToString(),//任务类型编码"人工领料"
                ExecutorType = TaskExecutor.TaskExecutor_WMS.ToString(),//执行者类型
                //ExecutedBy = "",//任务执行人
                //EquipmentID = "",//执行设备ID
                //EquipmentType = "",//执行设备类型
                //ActualEquipmentID = "",//实际执行设备ID
                SeqNo = 1,//顺序号
                Priority = 1,//优先级
                ContainerID = _containerHTid,//容器ID
                ContainerBarcode = container_barcode,//容器条码
                RefDocType = DocType.DocType_PersonReceiveMateriel.ToString(),//来源单据类型
                RefDocID = model.PickReturn_htid,//来源单ID
                //RefDocDetailID = "",//来源单明细ID
                RefDocNo = pick_return_no,//来源单号
                SrcAddrID = loc_id,//来源地址ID
                SrcAddrType = AddressType.AddressType_Location.ToString(),//来源地址类型"location"
                SrcAddrText = location_code,//来源地址
                DestAddrID = dockId,//目标地址ID
                DestAddrType = AddressType.AddressType_MachineStation.ToString(),//目标地址类型"dock"
                DestAddrText = dockCode,//目标地址
                //ActualDestAddrID = "",//实际目标地址ID
                //ActualDestAddrText = "",//实际目标地址
                TaskStatus = TaskStatus.TaskStatus_NotExecute.ToString(),//任务状态
                StartDate = dateTime,//开始时间
                //EndDate = dateTime,//结束时间
                IsExecutable = "1",//任务是否可执行
                //PriorityChangeTime = dateTime,//优先级修改时间        
                LedText = "",//LED显示内容
                ExistLgv = "0",//是否存在LGV
                //CreateDate = dateTime,//创建时间
                //CreatedBy = "",//创建人帐号
                //ModifyIP = "",//操作人IP 
                //ModifyDate = dateTime,//编辑时间
                //LastModifiedBy = "",//最后修改人帐号
                ActStatus = "1"//状态
            };
            WorkTask.Insert(taskModel);
            #endregion

            for (int i = 0; i < dtPickReturnDInfo.Rows.Count; i++)
            {
                decimal qty = (decimal)0.0;
                if (!decimal.TryParse(dtPickReturnDInfo.Rows[i]["qty"].ToString(), out qty))
                {
                    throw new WmsException(500, "该单据明细中存在输入的数量不为数字");
                }
                if (qty <= 0)
                {
                    throw new WmsException(500, "该单据明细存在数量为0的物料");
                }
                string itemId = dtPickReturnDInfo.Rows[i]["item_id"].ToString();//物料id
                string lotNo = dtPickReturnDInfo.Rows[i]["lot_no"].ToString();//批次
                string supplierId = dtPickReturnDInfo.Rows[i]["SUPPLIY"].ToString();//供应商id
                //根据物料、供应商和批次获取暂存区中未组盘的物料
                DataTable dtItemInfo = ce.GetItemInboundBuffer(itemId, lotNo, supplierId, qty.ToString());
                if (dtItemInfo == null || dtItemInfo.Rows.Count <= 0)
                {
                    throw new WmsException(500, "在暂存区中不能完全找不到该单据明细中的物料");
                }
                string containerDTid = dtItemInfo.Rows[0]["t_id"].ToString();//容器明细id
                //获取暂存区容器明细信息
                DataRow drContainerDInfo = Container.GetDetail(containerDTid);
                decimal containerDQty = Convert.ToDecimal(drContainerDInfo["qty"].ToString() == "" ? "0" : drContainerDInfo["qty"].ToString());//库存量
                decimal containerDLockQty = Convert.ToDecimal(drContainerDInfo["lock_qty"].ToString() != "" ? drContainerDInfo["lock_qty"].ToString() : "0");//锁定量
                decimal containerDOutQty = Convert.ToDecimal(drContainerDInfo["out_qty"].ToString() != "" ? drContainerDInfo["out_qty"].ToString() : "0");//待出量

                string _cdTid = SnowflakeID.Guid().ToString();
                drContainerDInfo["t_id"] = _cdTid;
                drContainerDInfo["container_id"] = _containerHTid;
                drContainerDInfo["qty"] = qty;
                drContainerDInfo["stat_qty"] = qty * Convert.ToDecimal(drContainerDInfo["ratio"].ToString());
                //新建容器明细
                Container.InsertDetail(drContainerDInfo);
                //更新容器明细库存量
                Container.UpdateDetailQty(containerDTid, containerDQty - qty);
                //如果物料被人工领取完，则删除容器明细
                if (containerDQty - qty <= 0)
                {
                    Container.UpdateDetailActStatus(containerDTid, "0");
                }
                //将容器明细id，反写入单据明细中
                ce.UpdateContainerDTidInPickReturnD(dtPickReturnDInfo.Rows[i]["t_id"].ToString(), _cdTid);

                #region 插入任务物料信息
                WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                    //ClientID ="",//集团ID
                    //OrgID ="",//组织ID
                    //WarehouseID ="",//仓库ID
                    TaskID = taskModel.ID,//主任务ID
                    ItemID = dtPickReturnDInfo.Rows[i]["item_id"].ToString(),//物料ID
                    ItemName = dtPickReturnDInfo.Rows[i]["item_name"].ToString(),//物料名称
                    PlanQty = qty,//计划数量
                    //ProcessedQty="",//已处理数量
                    //FinishedQty="",//完成数量
                    UomQtyID = dtPickReturnDInfo.Rows[i]["uom_qty_id"].ToString(),//数量单位
                    //FinishedStatQty="",//完成统计数量
                    UomStatID = dtItemInfo.Rows[0]["uom_stat_id"].ToString(),//统计数量单位
                    ContainerDetailID = _cdTid,//容器明细ID
                    RefDocType = DocType.DocType_PersonReceiveMateriel.ToString(),//来源单据类型
                    RefDocID = model.PickReturn_htid,//来源单ID
                    RefDocDetailID = dtPickReturnDInfo.Rows[i]["t_id"].ToString(),//来源单明细ID
                    RefDocNo = pick_return_no,//来源单号
                    StartDate = dateTime,//开始时间
                    //EndDate="",//结束时间
                    ProcessStatus = ProcessStatus.ProcessStatus_Doing.ToString(),//工作状态
                    //CreateDate="",//创建时间
                    //CreatedBy ="",//创建人帐号 
                    //ModifyIP ="",//操作人IP 
                    //ModifyDate="",//编辑时间
                    //LastModifiedBy ="",//最后修改人帐号
                    //ActStatus =""//状态
                };
                WorkTaskItem.Insert(taskItemModel);
                #endregion
            }
            #endregion

            #region 10.将容器主档挂在人工领料站台上
            //获取当前站台所挂的所有容器主档集合
            List<string> container_idList = container_ids.Split(",").ToList();
            container_idList.Remove("");
            //将需要挂在该站台的新的主档增加进容器主档集合
            container_idList.Add(_containerHTid);
            //将容器集合以“,”分割挂在人工领料站台上
            ce.UpdateMachineDockDeatilId(machineDockTid, string.Join(",", container_idList.ToArray()), nowProductPlanId, string.Empty, string.Empty);
            //将容器主档反写入人工领料主档上
            ce.UpdatePickReturnContainerIdById(model.PickReturn_htid, _containerHTid);
            #endregion
        }
        #endregion

        #region 人工领料完成
        /// <summary>
        /// 人工领料完成
        /// </summary>
        /// <param name="model"></param>
        public void FinishPersonReceiveMateriel(FinishPersonReceiveMaterielModel model)
        {
            #region 1.查询人工领料主档信息
            //查询人工领料主档信息
            DataRow drPickReturnHInfo = ce.SelectPickReturnHById(model.PickReturn_htid);
            string pick_return_type = drPickReturnHInfo["pick_return_type"].ToString();//单据类型
            string pick_return_status = drPickReturnHInfo["pick_return_status"].ToString();//单据状态
            string pick_return_no = drPickReturnHInfo["pick_return_no"].ToString();//单据号
            string job_no = drPickReturnHInfo["job_no"].ToString();//生产工单号
            string shift_id = drPickReturnHInfo["shift_id"].ToString();//班次
            string team_id = drPickReturnHInfo["team_id"].ToString();//班组
            string equipment_id = drPickReturnHInfo["equipment_id"].ToString();//机台 
            if (pick_return_type != PickReturnType.PickReturnType_RGLL.ToString())
            {
                throw new WmsException(500, "该单据不是人工领料单据");
            }
            if (pick_return_status != PickReturnStatus.PickReturnStatus_Confirmed.ToString())
            {
                throw new WmsException(500, "单据仅在已确认状态下，才能完成");
            }
            #endregion

            #region 2.更新人工领料单据状态
            ce.UpdatePickReturnhStatusById(model.PickReturn_htid, PickReturnStatus.PickReturnStatus_Finish.ToString());
            #endregion

            #region 3.获取任务主表
            //获取任务主表
            DataRow[] drTaskInfo = WmsFactory.Make<WorkTask>().GetByRefDocID(model.PickReturn_htid);
            //此处增加这个判断，是因为人工领料任务存在强制完成，在强制完成时也会修改任务和容器信息,并将任务写入历史表，所以在此处增加判断，避免重复操作
            //人工领料界面上的完成单据，会完成任务和修改容器明细信息，也会将任务写入历史表
            //不为0，则表示在任务表中查到了任务，即不在任务历史表中，就表示任务未完成；为0表示，任务已完成
            if (drTaskInfo.Length == 0)
            {
                return;
            }
            #endregion

            #region 4.获取人工领料单明细信息
            DataTable dtPickReturnDInfo = ce.SelectPickReturnDById(model.PickReturn_htid);
            #endregion

            #region 5.更新容器中的物料数量和任务信息            
            DateTime dateTime = DateTime.Now;
            string taskId = drTaskInfo[0]["t_id"].ToString();
            string taskStatus = drTaskInfo[0]["task_status"].ToString();//任务状态
            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                return;
            }
            //更新任务是否还可执行和状态
            WorkTask.UpdateIsExecutable(taskId, "0").UpdateTaskStatus(taskId, TaskStatus.TaskStatus_Finished.ToString());

            List<TaskItemModel> timList = WmsFactory.Make<WorkTask>().getTaskItems(taskId);
            for (int i = 0; i < timList.Count; i++)
            {
                decimal qty = Convert.ToDecimal(dtPickReturnDInfo.Rows[i]["qty"].ToString() == "" ? "0" : dtPickReturnDInfo.Rows[i]["qty"].ToString());
                TaskItemModel tm = new TaskItemModel();
                tm.t_id = timList[i].t_id;
                tm.task_id = taskId;//主任务ID
                tm.processed_qty = qty;//已处理数量
                tm.finished_qty = qty;//完成数量,完成单据时该数量就是任务中的计划数量
                tm.start_date = dateTime;//开始时间
                tm.end_date = dateTime;//结束时间
                tm.process_status = ProcessStatus.ProcessStatus_Done.ToString();//工作状态
                //更新任务明细状态
                WmsFactory.Make<WorkTask>().UpdateTaskitemStatus(tm);

            }
            //将完成的任务插入历史表
            WorkTask.MoveToHistory(taskId);
            #endregion
        }
        #endregion

        #region 人工领料任务强制完成
        /// <summary>
        /// 人工领料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        public void FinishPersonReceiveMaterielTask(FinishPersonReceiveMaterielTaskModel model)
        {
            #region 1.获取人工领料任务信息
            //获取人工领料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到人工领料任务！");
            }
            string pickReturnhTid = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//人工领料主档id
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_PersonReceiveMateriel.ToString())
            {
                throw new WmsException(500, "该任务不是人工领料任务，无法强制完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString() && taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "任务仅在处于执行中或者已部分完成状态下，才能强制完成！");
            }
            #endregion

            #region 2.获取任务物料明细
            //获取任务物料明细
            DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
            #endregion

            #region 3.更新人工领料单据状态
            ce.UpdatePickReturnhStatusById(pickReturnhTid, PickReturnStatus.PickReturnStatus_Finish.ToString());
            #endregion

            #region 4.更新任务、任务明细
            //将任务更新为完成
            WorkTask.UpdateTaskComplete(model.TaskTid);
            //更新任务物料明细中的已处理数量和完成数量
            for (int i = 0; i < taskItemInfos.Length; i++)
            {
                string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
                //string containerDTid = taskItemInfos[i]["container_d_id"].ToString();//容器明细id
                //DataRow drContainerInfo = Container.GetDetail(containerDTid);
                //decimal qty = Convert.ToDecimal(drContainerInfo["qty"].ToString());
                //decimal outQty = Convert.ToDecimal(drContainerInfo["out_qty"].ToString());
                //decimal processed_qty = Convert.ToDecimal(taskItemInfos[i]["plan_qty"].ToString());//计划数量
                //根据任务物料id，修改任务物料状态为已完成，并且修改完成和处理数量
                WorkTask.UpdateTaskItemCompleteAndQty(cdTid);
            }
            WorkTask.MoveToHistory(model.TaskTid);
            #endregion
        }
        #endregion

        #region 人工领料任务回收
        /// <summary>
        /// 人工领料任务回收
        /// </summary>
        /// <param name="model"></param>
        public void RecyclePersonReceiveMaterielTask(RecyclePersonReceiveMaterielTaskModel model)
        {
            #region 1.获取人工领料任务信息
            //获取人工领料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到人工领料任务！");
            }
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string pickReturnhTid = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//人工领料主档id
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态

            if (refDocType != DocType.DocType_PersonReceiveMateriel.ToString())
            {
                throw new WmsException(500, "该任务不是人工领料任务，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "该任务已完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "该任务已部分完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "该任务正在执行中，无法回收任务！");
            }
            #endregion

            #region 2.获取任务物料明细
            //获取任务物料明细
            DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
            #endregion

            #region 3.查询人工领料主档信息
            //查询人工领料主档信息
            DataRow drPickReturnHInfo = ce.SelectPickReturnHById(pickReturnhTid);
            string equipment_id = drPickReturnHInfo["equipment_id"].ToString();//机台  
            #endregion

            #region 4.获取人工领料明细
            DataTable dtPickReturnDInfo = ce.SelectPickReturnDById(pickReturnhTid);
            #endregion

            #region 5.获取人工领料站台在机台站台表中的信息
            DataRow dtDockInfo = ce.GetJieBDockByMachine(equipment_id);
            string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            string container_ids = dtDockInfo["container_id"].ToString();//挂在站台上的容器主档id(使用“,”分割)
            #endregion

            #region 6.获取暂存区货位对应的容器主档信息
            //获取暂存区货位对应的容器主档信息
            DataTable dtInboundBufferContainerHInfo = ce.GetContainerHTidOfInboundBufferLoc();
            if (dtInboundBufferContainerHInfo == null || dtInboundBufferContainerHInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "无法暂存区容器主档");
            }
            if (dtInboundBufferContainerHInfo.Rows.Count > 1)
            {
                throw new WmsException(500, "暂存区容器主档不唯一");
            }
            string containerHTid = dtInboundBufferContainerHInfo.Rows[0]["t_id"].ToString();//暂存区货位对应的容器主档id
            string zone_id = dtInboundBufferContainerHInfo.Rows[0]["zone_id"].ToString();//暂存区货位对应的区域id
            string container_barcode = dtInboundBufferContainerHInfo.Rows[0]["container_barcode"].ToString();//容器编码
            string location_code = dtInboundBufferContainerHInfo.Rows[0]["location_code"].ToString();//库位编码
            string loc_id = dtInboundBufferContainerHInfo.Rows[0]["loc_id"].ToString();//库位id 
            #endregion

            #region 7.修改人工领料明细中关联的容器明细id
            //修改人工领料明细中关联的容器明细id
            for (int i = 0; i < dtPickReturnDInfo.Rows.Count; i++)
            {
                ce.UpdateContainerDTidInPickReturnD(dtPickReturnDInfo.Rows[i]["t_id"].ToString(), string.Empty);
            }
            #endregion

            #region 8.更新人工领料单据状态,为新建状态
            ce.UpdatePickReturnhStatusById(pickReturnhTid, PickReturnStatus.PickReturnStatus_New.ToString());
            #endregion

            #region 9.更新任务和任务明细软删除标记为“0”，并软删除人工领料时创建的容器主档和容器明细（并恢复库存），移出挂在人工领料站台上的容器主档
            //删除人工领料时创建的容器主档
            Container.UpdateHeadActStatus(container_id, "0");
            //将容器主档反写入人工领料主档上
            ce.UpdatePickReturnContainerIdById(pickReturnhTid, string.Empty);
            //获取当前站台所挂的所有容器主档集合
            List<string> container_idList = container_ids.Split(",").ToList();
            //移出挂在人工领料站台上的容器主档
            container_idList.Remove(container_id);
            if (container_idList.Count <= 0)//表明没有挂在人工领料站台上的容器了
            {
                ce.UpdateMachineDockDeatilId(machineDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            }
            else
            {
                //将容器集合以“,”分割挂在人工领料站台上
                ce.UpdateMachineDockDeatilId(machineDockTid, string.Join(",", container_idList.ToArray()), string.Empty, string.Empty, string.Empty);
            }
            //修改任务软删除标记为“0”
            WorkTask.UpdateActStatus(model.TaskTid, "0");
            //更新任务物料软删除标记为“0”
            for (int i = 0; i < taskItemInfos.Length; i++)
            {
                string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
                string containerDTid = taskItemInfos[i]["container_d_id"].ToString();//容器明细id
                //获取容器明细信息
                DataRow drContainerDInfo = Container.GetDetail(containerDTid);
                string itemId = drContainerDInfo["item_id"].ToString();//物料id
                string lotNo = drContainerDInfo["lot_no"].ToString();//批次
                string supplierId = drContainerDInfo["SUPPLIER_ID"].ToString();//供应商id
                decimal qty = Convert.ToDecimal(drContainerDInfo["qty"].ToString() == "" ? "0" : drContainerDInfo["qty"].ToString());//人工领料数量
                //根据物料、供应商和批次获取暂存区中未组盘的物料
                DataTable dtItemInfo = ce.GetItemInboundBuffer(itemId, lotNo, supplierId);
                if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                {
                    //原库存数量
                    decimal _qty = Convert.ToDecimal(dtItemInfo.Rows[0]["qty"].ToString() == "" ? "0" : dtItemInfo.Rows[0]["qty"].ToString());
                    Container.UpdateDetailQty(dtItemInfo.Rows[0]["t_id"].ToString(), _qty + qty);
                }
                else
                {
                    drContainerDInfo["container_id"] = containerHTid;
                    drContainerDInfo["t_id"] = SnowflakeID.Guid().ToString();
                    //新建容器明细,挂在暂存区容器主档下面
                    Container.InsertDetail(drContainerDInfo);
                }
                //软删除人工领料时创建容器明细
                Container.UpdateDetailActStatus(containerDTid, "0");
                //修改任务物料表中的软删除标记为“0”
                WorkTask.UpdateTaskItemActStatus(cdTid, "0");
            }
            #endregion
        }
        #endregion

        #region 人工退料确认
        /// <summary>
        /// 人工退料确认
        /// </summary>
        /// <param name="model"></param>
        public void ConfirmPersonReturnMateriel(ConfirmPersonReturnMaterielModel model)
        {
            #region 1.查询人工退料主档信息
            //查询人工退料主档信息
            DataRow drPickReturnHInfo = ce.SelectPickReturnHById(model.PickReturn_htid);
            string pick_return_type = drPickReturnHInfo["pick_return_type"].ToString();//单据类型
            string pick_return_status = drPickReturnHInfo["pick_return_status"].ToString();//单据状态
            string pick_return_no = drPickReturnHInfo["pick_return_no"].ToString();//单据号
            string job_no = drPickReturnHInfo["job_no"].ToString();//生产工单号
            string ref_doc_no = drPickReturnHInfo["ref_doc_no"].ToString();//人工领料单据号
            string shift_id = drPickReturnHInfo["shift_id"].ToString();//班次
            string team_id = drPickReturnHInfo["team_id"].ToString();//班组
            string equipment_id = drPickReturnHInfo["equipment_id"].ToString();//机台 
            string dock_id = drPickReturnHInfo["dock_id"].ToString();//人工领料站台id
            string dock_code = drPickReturnHInfo["dock_code"].ToString();//人工领料站台编码
            if (pick_return_type != PickReturnType.PickReturnType_RGTL.ToString())
            {
                throw new WmsException(500, "该单据不是人工退料单据");
            }
            if (pick_return_status != PickReturnStatus.PickReturnStatus_New.ToString())
            {
                throw new WmsException(500, "该人工退料单据不是新建状态，无法确认");
            }
            #endregion

            #region 2.获取人工退料单明细信息
            DataTable dtPickReturnDInfo = ce.SelectPickReturnDById(model.PickReturn_htid);
            #endregion

            #region 3.判断单据中的数量是否大于领取物料的数量
            //判断单据中的数量是否大于领取物料的数量
            for (int i = 0; i < dtPickReturnDInfo.Rows.Count; i++)
            {
                string container_dtid = dtPickReturnDInfo.Rows[i]["container_dtid"].ToString();//容器明细id
                string itemName = dtPickReturnDInfo.Rows[i]["item_name"].ToString();//物料名称
                decimal qty = Convert.ToDecimal(dtPickReturnDInfo.Rows[i]["qty"].ToString() == "" ? "0" : dtPickReturnDInfo.Rows[i]["qty"].ToString());//人工退料单据中的物料数量
                DataTable _dt = ce.GetFinishTaskItemInfo(ref_doc_no, container_dtid);//人工领料任务明细信息
                decimal _q = Convert.ToDecimal(_dt.Rows[0]["plan_qty"].ToString() == "" ? "0" : _dt.Rows[0]["plan_qty"].ToString());//任务明细信息的计划数量
                if (qty < 0)
                {
                    throw new WmsException(500, $"物料{itemName}的数量，不能小于0");
                }
                if (qty > _q)
                {
                    throw new WmsException(500, $"物料{itemName}的数量，超过了领取的数量");
                }
            }
            #endregion

            #region 4.判断人工领料是否已经被退料，已退料则不允许退料
            DataTable finishPersonReturnNum = ce.GetFinishPersonReturnNum(ref_doc_no);
            int finishNum = Convert.ToInt32(finishPersonReturnNum.Rows[0]["cnt"].ToString());
            if (finishNum >= 1)
            {
                throw new WmsException(500, $"来源单据号为{ref_doc_no}的人工领料已退料，无须再次退料！");
            }
            #endregion
            //获取人工退料单据中关联的人工领料单据信息
            DataRow _drPr = ce.SelectPickReturnHByNo(ref_doc_no);
            string containerHTidInPr = _drPr["container_id"].ToString();//人工领料单据中的容器主档

            #region 5.获取暂存区货位对应的容器主档信息
            //获取暂存区货位对应的容器主档信息
            DataTable dtInboundBufferContainerHInfo = ce.GetContainerHTidOfInboundBufferLoc();
            if (dtInboundBufferContainerHInfo == null || dtInboundBufferContainerHInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "无法暂存区容器id");
            }
            if (dtInboundBufferContainerHInfo.Rows.Count > 1)
            {
                throw new WmsException(500, "暂存区容器id不唯一");
            }
            //string containerHTid = dtInboundBufferContainerHInfo.Rows[0]["t_id"].ToString();//暂存区货位对应的容器主档id
            string zone_id = dtInboundBufferContainerHInfo.Rows[0]["zone_id"].ToString();//暂存区货位对应的区域id
            //string container_barcode = dtInboundBufferContainerHInfo.Rows[0]["container_barcode"].ToString();//容器编码
            string location_code = dtInboundBufferContainerHInfo.Rows[0]["location_code"].ToString();//暂存区库位编码
            string loc_id = dtInboundBufferContainerHInfo.Rows[0]["loc_id"].ToString();//暂存区库位id 
            #endregion
            //获取人工领料单所挂容器主档信息
            DataRow _drContainerHInfo = Container.GetContainerH(containerHTidInPr);
            string container_barcode = string.Empty;//;_drContainerHInfo["container_barcode"].ToString();//容器编码

            #region 6.更新人工退料单据状态，并将容器主档信息反写入单据主档上
            ce.UpdatePickReturnhStatusById(model.PickReturn_htid, PickReturnStatus.PickReturnStatus_Confirmed.ToString());
            //将容器主档反写入人工退料主档上
            ce.UpdatePickReturnContainerIdById(model.PickReturn_htid, containerHTidInPr);
            #endregion

            #region 7.更新容器待入量，创建入库任务
            DateTime dateTime = DateTime.Now;
            #region 插入任务主档
            Model.WorkTaskModel taskModel = new Model.WorkTaskModel()
            {
                ID = SnowflakeID.Guid().ToString(),//主键ID
                ZoneID = zone_id,//区域ID
                TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                //DependentTaskNo = "",//依赖任务号
                TaskType = TaskType.TaskType_InBound.ToString(),//任务类型编码"人工退料"
                ExecutorType = TaskExecutor.TaskExecutor_WMS.ToString(),//执行者类型
                //ExecutedBy = "",//任务执行人
                //EquipmentID = "",//执行设备ID
                //EquipmentType = "",//执行设备类型
                //ActualEquipmentID = "",//实际执行设备ID
                SeqNo = 1,//顺序号
                Priority = 1,//优先级
                ContainerID = containerHTidInPr,//容器ID
                ContainerBarcode = container_barcode,//容器条码
                RefDocType = DocType.DocType_PersonReturnMateriel.ToString(),//来源单据类型
                RefDocID = model.PickReturn_htid,//来源单ID
                //RefDocDetailID = "",//来源单明细ID
                RefDocNo = pick_return_no,//来源单号
                SrcAddrID = dock_id,//来源地址ID
                SrcAddrType = AddressType.AddressType_MachineStation.ToString(),//来源地址类型
                SrcAddrText = dock_code,//来源地址
                DestAddrID = loc_id,//目标地址ID
                DestAddrType = AddressType.AddressType_Location.ToString(),//目标地址类型
                DestAddrText = location_code,//目标地址
                //ActualDestAddrID = "",//实际目标地址ID
                //ActualDestAddrText = "",//实际目标地址
                TaskStatus = TaskStatus.TaskStatus_NotExecute.ToString(),//任务状态
                StartDate = dateTime,//开始时间
                //EndDate = dateTime,//结束时间
                IsExecutable = "1",//任务是否可执行
                //PriorityChangeTime = dateTime,//优先级修改时间        
                LedText = "",//LED显示内容
                ExistLgv = "0",//是否存在LGV
                //CreateDate = dateTime,//创建时间
                //CreatedBy = "",//创建人帐号
                //ModifyIP = "",//操作人IP 
                //ModifyDate = dateTime,//编辑时间
                //LastModifiedBy = "",//最后修改人帐号
                ActStatus = "1"//状态
            };
            WorkTask.Insert(taskModel);
            #endregion

            for (int i = 0; i < dtPickReturnDInfo.Rows.Count; i++)
            {
                DataRow temp = Container.GetDetail(dtPickReturnDInfo.Rows[i]["container_dtid"].ToString());
                string container_dtid = dtPickReturnDInfo.Rows[i]["container_dtid"].ToString();
                decimal qty = Convert.ToDecimal(dtPickReturnDInfo.Rows[i]["qty"].ToString() == "" ? "0" : dtPickReturnDInfo.Rows[i]["qty"].ToString());
                ////qty为0表示不需要退料,有可能存在物料已经被人工领取完了的情况。
                //if (qty <= 0)
                //{
                //    Container.UpdateDetailActStatus(container_dtid, "0");
                //    continue;
                //}
                #region 更新容器明细物料数量
                Container.UpdateDetailQty(container_dtid, qty);
                #endregion

                #region 插入任务物料信息
                WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                    //ClientID ="",//集团ID
                    //OrgID ="",//组织ID
                    //WarehouseID ="",//仓库ID
                    TaskID = taskModel.ID,//主任务ID
                    ItemID = dtPickReturnDInfo.Rows[i]["item_id"].ToString(),//物料ID
                    ItemName = dtPickReturnDInfo.Rows[i]["item_name"].ToString(),//物料名称
                    PlanQty = qty,//计划数量
                    //ProcessedQty="",//已处理数量
                    //FinishedQty="",//完成数量
                    UomQtyID = dtPickReturnDInfo.Rows[i]["uom_qty_id"].ToString(),//数量单位
                    //FinishedStatQty="",//完成统计数量
                    UomStatID = temp["uom_stat_id"].ToString(),//统计数量单位
                    ContainerDetailID = container_dtid,//容器明细ID
                    RefDocType = DocType.DocType_PersonReturnMateriel.ToString(),//来源单据类型
                    RefDocID = model.PickReturn_htid,//来源单ID
                    //RefDocDetailID ="",//来源单明细ID
                    RefDocNo = pick_return_no,//来源单号
                    StartDate = dateTime,//开始时间
                    //EndDate="",//结束时间
                    ProcessStatus = ProcessStatus.ProcessStatus_Doing.ToString(),//工作状态
                    //CreateDate="",//创建时间
                    //CreatedBy ="",//创建人帐号 
                    //ModifyIP ="",//操作人IP 
                    //ModifyDate="",//编辑时间
                    //LastModifiedBy ="",//最后修改人帐号
                    //ActStatus =""//状态
                };
                WorkTaskItem.Insert(taskItemModel);
                #endregion
            }
            #endregion

        }
        #endregion

        #region 人工退料完成
        /// <summary>
        /// 人工退料完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void FinishPersonReturnMateriel(FinishPersonReturnMaterielModel model)
        {
            #region 1.查询人工退料主档信息
            //查询人工退料主档信息
            DataRow drPickReturnHInfo = ce.SelectPickReturnHById(model.PickReturn_htid);
            string pick_return_type = drPickReturnHInfo["pick_return_type"].ToString();//单据类型
            string pick_return_status = drPickReturnHInfo["pick_return_status"].ToString();//单据状态
            string pick_return_no = drPickReturnHInfo["pick_return_no"].ToString();//单据号
            string job_no = drPickReturnHInfo["job_no"].ToString();//生产工单号
            string shift_id = drPickReturnHInfo["shift_id"].ToString();//班次
            string team_id = drPickReturnHInfo["team_id"].ToString();//班组
            string equipment_id = drPickReturnHInfo["equipment_id"].ToString();//机台 
            if (pick_return_type != PickReturnType.PickReturnType_RGTL.ToString())
            {
                throw new WmsException(500, "该单据不是人工退料单据");
            }
            if (pick_return_status != PickReturnStatus.PickReturnStatus_Confirmed.ToString())
            {
                throw new WmsException(500, "该人工退料单据不是确认状态，无法完成");
            }
            #endregion

            #region 2.更新人工退料单据状态
            ce.UpdatePickReturnhStatusById(model.PickReturn_htid, PickReturnStatus.PickReturnStatus_Finish.ToString());
            #endregion

            #region 3.获取任务主表
            //获取任务主表
            DataRow[] drTaskInfo = WmsFactory.Make<WorkTask>().GetByRefDocID(model.PickReturn_htid);
            string taskId = drTaskInfo[0]["t_id"].ToString();
            string container_id = drTaskInfo[0]["container_id"].ToString();//任务中，容器主档id
            #endregion

            #region 4.获取人工领料单明细信息
            DataTable dtPickReturnDInfo = ce.SelectPickReturnDById(model.PickReturn_htid);
            #endregion

            #region 5.获取人工领料站台在机台站台表中的信息
            DataRow dtDockInfo = ce.GetJieBDockByMachine(equipment_id);
            string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            string container_ids = dtDockInfo["container_id"].ToString();//挂在站台上的容器主档id(使用“,”分割)
            #endregion

            #region 5.获取暂存区货位对应的容器主档信息
            //获取暂存区货位对应的容器主档信息
            DataTable dtInboundBufferContainerHInfo = ce.GetContainerHTidOfInboundBufferLoc();
            if (dtInboundBufferContainerHInfo == null || dtInboundBufferContainerHInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "无法暂存区容器id");
            }
            if (dtInboundBufferContainerHInfo.Rows.Count > 1)
            {
                throw new WmsException(500, "暂存区容器id不唯一");
            }
            string containerHTid = dtInboundBufferContainerHInfo.Rows[0]["t_id"].ToString();//暂存区货位对应的容器主档id
            string zone_id = dtInboundBufferContainerHInfo.Rows[0]["zone_id"].ToString();//暂存区货位对应的区域id
            string container_barcode = dtInboundBufferContainerHInfo.Rows[0]["container_barcode"].ToString();//容器编码
            string location_code = dtInboundBufferContainerHInfo.Rows[0]["location_code"].ToString();//暂存区库位编码
            string loc_id = dtInboundBufferContainerHInfo.Rows[0]["loc_id"].ToString();//暂存区库位id 
            #endregion

            #region 6.更新容器中的物料数量和任务信息 ，并软删除人工领料时创建的容器主档和容器明细（并更新库存）   
            //删除人工领料时创建的容器主档
            Container.UpdateHeadActStatus(container_id, "0");

            //获取当前站台所挂的所有容器主档集合
            List<string> container_idList = container_ids.Split(",").ToList();
            //移出挂在人工领料站台上的容器主档
            container_idList.Remove(container_id);
            //将容器集合以“,”分割挂在人工领料站台上
            ce.UpdateMachineDockDeatilId(machineDockTid, string.Join(",", container_idList.ToArray()), string.Empty, string.Empty, string.Empty);

            DateTime dateTime = DateTime.Now;
            //更新任务是否还可执行和状态
            WorkTask.UpdateIsExecutable(taskId, "0").UpdateTaskStatus(taskId, TaskStatus.TaskStatus_Finished.ToString());
            //查询任务明细
            List<TaskItemModel> timList = WmsFactory.Make<WorkTask>().getTaskItems(taskId, false);
            for (int i = 0; i < timList.Count; i++)
            {
                TaskItemModel tm = new TaskItemModel();
                tm.t_id = timList[i].t_id;
                tm.task_id = taskId;//主任务ID
                decimal plan_qty = Convert.ToDecimal(timList[i].plan_qty);//任务中的计划数量
                tm.processed_qty = plan_qty;//已处理数量               
                tm.finished_qty = plan_qty;//完成数量
                tm.start_date = dateTime;//开始时间
                tm.end_date = dateTime;//结束时间
                tm.process_status = ProcessStatus.ProcessStatus_Done.ToString();//工作状态
                string containerDtid = dtPickReturnDInfo.Rows[i]["container_dtid"].ToString();

                DataRow temp = Container.GetDetail(containerDtid);
                string itemId = temp["item_id"].ToString();//物料id
                string lotNo = temp["lot_no"].ToString();//批次
                string supplierId = temp["supplier_id"].ToString();//供应商id
                decimal qty = Convert.ToDecimal(temp["qty"].ToString());//人工退料数量
                //根据物料、供应商和批次获取暂存区中未组盘的物料
                DataTable dtItemInfo = ce.GetItemInboundBuffer(itemId, lotNo, supplierId);
                if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                {
                    //原库存数量
                    decimal _qty = Convert.ToDecimal(dtItemInfo.Rows[0]["qty"].ToString());
                    Container.UpdateDetailQty(dtItemInfo.Rows[0]["t_id"].ToString(), _qty + qty);
                }
                else
                {
                    temp["container_id"] = containerHTid;
                    temp["t_id"] = SnowflakeID.Guid().ToString();
                    //新建容器明细,挂在暂存区容器主档下面
                    Container.InsertDetail(temp);
                    //软删除人工领料时创建容器明细
                    Container.UpdateDetailActStatus(containerDtid, "0");
                }
                //更新任务明细状态
                WmsFactory.Make<WorkTask>().UpdateTaskitemStatus(tm);

            }

            //将任务插入历史表
            WorkTask.InsertHistory(taskId);
            //删除任务
            WorkTask.DeleteWorkTask(taskId);
            //若存在任务明细，则插入历史表
            if (timList.Count > 0)
            {
                //将任务明细插入历史表
                WorkTask.TaskItemInsertHistory(taskId);
                WorkTask.DeleteWorkTaskItem(taskId);
            }
            //将完成的任务插入历史表(包含明细删除和插入历史表)，一旦不存在明细，则该操作不成功
            //WorkTask.MoveToHistory(taskId);
            #endregion
        }
        #endregion

        #region 人工退料任务强制完成
        /// <summary>
        /// 人工退料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        public void FinishPersonReturnMaterielTask(FinishPersonReturnMaterielTaskModel model)
        {
            #region 1.获取人工退料任务信息
            //获取人工退料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到人工退料任务！");
            }
            string pickReturnhTid = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//人工退料主档id
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            if (refDocType != DocType.DocType_PersonReturnMateriel.ToString())
            {
                throw new WmsException(500, "该任务不是人工退料任务，无法强制完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString() && taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "任务仅在处于执行中或者已部分完成状态下，才能强制完成！");
            }
            #endregion

            #region 2.获取任务物料明细
            string taskId = model.TaskTid;
            //获取任务物料明细
            //DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid,"1",false);
            List<TaskItemModel> timList = WmsFactory.Make<WorkTask>().getTaskItems(taskId, false);
            #endregion

            #region 3.查询人工领料主档信息
            //查询人工领料主档信息
            DataRow drPickReturnHInfo = ce.SelectPickReturnHById(pickReturnhTid);
            string equipment_id = drPickReturnHInfo["equipment_id"].ToString();//机台  
            #endregion

            #region 5.获取人工领料站台在机台站台表中的信息
            DataRow dtDockInfo = ce.GetJieBDockByMachine(equipment_id);
            string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            string container_ids = dtDockInfo["container_id"].ToString();//挂在站台上的容器主档id(使用“,”分割)
            #endregion

            #region 3.获取暂存区货位对应的容器主档信息
            //获取暂存区货位对应的容器主档信息
            DataTable dtInboundBufferContainerHInfo = ce.GetContainerHTidOfInboundBufferLoc();
            if (dtInboundBufferContainerHInfo == null || dtInboundBufferContainerHInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "无法暂存区容器id");
            }
            if (dtInboundBufferContainerHInfo.Rows.Count > 1)
            {
                throw new WmsException(500, "暂存区容器id不唯一");
            }
            string containerHTid = dtInboundBufferContainerHInfo.Rows[0]["t_id"].ToString();//暂存区货位对应的容器主档id
            string zone_id = dtInboundBufferContainerHInfo.Rows[0]["zone_id"].ToString();//暂存区货位对应的区域id
            string container_barcode = dtInboundBufferContainerHInfo.Rows[0]["container_barcode"].ToString();//容器编码
            string location_code = dtInboundBufferContainerHInfo.Rows[0]["location_code"].ToString();//暂存区库位编码
            string loc_id = dtInboundBufferContainerHInfo.Rows[0]["loc_id"].ToString();//暂存区库位id 
            #endregion

            #region 4.更新人工领料单据状态
            ce.UpdatePickReturnhStatusById(pickReturnhTid, PickReturnStatus.PickReturnStatus_Finish.ToString());
            #endregion

            #region 5.更新容器中的物料数量和任务信息，并软删除人工领料时创建的容器主档和容器明细（并更新库存） 
            //删除人工领料时创建的容器主档
            Container.UpdateHeadActStatus(container_id, "0");
            //获取当前站台所挂的所有容器主档集合
            List<string> container_idList = container_ids.Split(",").ToList();
            //移出挂在人工领料站台上的容器主档
            container_idList.Remove(container_id);
            //将容器集合以“,”分割挂在人工领料站台上
            ce.UpdateMachineDockDeatilId(machineDockTid, string.Join(",", container_idList.ToArray()), string.Empty, string.Empty, string.Empty);
            DateTime dateTime = DateTime.Now;
            //更新任务是否还可执行和状态
            WorkTask.UpdateIsExecutable(taskId, "0").UpdateTaskStatus(taskId, TaskStatus.TaskStatus_Finished.ToString());
            for (int i = 0; i < timList.Count; i++)
            {
                TaskItemModel tm = new TaskItemModel();
                tm.t_id = timList[i].t_id;
                tm.task_id = taskId;//主任务ID
                decimal plan_qty = Convert.ToDecimal(timList[i].plan_qty);//任务中的计划数量
                tm.processed_qty = plan_qty;//已处理数量               
                tm.finished_qty = plan_qty;//完成数量
                tm.start_date = dateTime;//开始时间
                tm.end_date = dateTime;//结束时间
                tm.process_status = ProcessStatus.ProcessStatus_Done.ToString();//工作状态
                string containerDtid = timList[i].container_d_id;
                DataRow temp = Container.GetDetail(containerDtid);
                string itemId = temp["item_id"].ToString();//物料id
                string lotNo = temp["lot_no"].ToString();//批次
                string supplierId = temp["SUPPLIER_ID"].ToString();//供应商id
                decimal qty = Convert.ToDecimal(temp["qty"].ToString());//人工退料数量
                //根据物料、供应商和批次获取暂存区中未组盘的物料
                DataTable dtItemInfo = ce.GetItemInboundBuffer(itemId, lotNo, supplierId);
                if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                {
                    //原库存数量
                    decimal _qty = Convert.ToDecimal(dtItemInfo.Rows[0]["qty"].ToString());
                    Container.UpdateDetailQty(dtItemInfo.Rows[0]["t_id"].ToString(), _qty + qty);
                }
                else
                {
                    temp["container_id"] = containerHTid;
                    temp["t_id"] = SnowflakeID.Guid().ToString();
                    //新建容器明细,挂在暂存区容器主档下面
                    Container.InsertDetail(temp);
                    //软删除人工领料时创建容器明细
                    Container.UpdateDetailActStatus(containerDtid, "0");
                }
                //更新任务明细状态
                WmsFactory.Make<WorkTask>().UpdateTaskitemStatus(tm);
            }
            //将任务插入历史表
            WorkTask.InsertHistory(taskId);
            //删除任务
            WorkTask.DeleteWorkTask(taskId);
            //若存在任务明细，则插入历史表
            if (timList.Count > 0)
            {
                //将任务明细插入历史表
                WorkTask.TaskItemInsertHistory(taskId);
                WorkTask.DeleteWorkTaskItem(taskId);
            }
            #endregion
        }
        #endregion

        #region 人工退料任务回收
        /// <summary>
        /// 人工退料任务回收
        /// </summary>
        /// <param name="model"></param>
        public void RecyclePersonReturnMaterielTask(RecyclePersonReturnMaterielTaskModel model)
        {
            #region 1.获取人工退料任务信息
            //获取人工退料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到人工退料任务！");
            }
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string pickReturnhTid = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//人工退料主档id
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            string ref_doc_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//人工退料单据号
            if (refDocType != DocType.DocType_PersonReturnMateriel.ToString())
            {
                throw new WmsException(500, "该任务不是人工退料任务，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "该任务已完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "该任务已部分完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "该任务正在执行中，无法回收任务！");
            }
            #endregion

            #region 2.获取任务物料明细
            //获取任务物料明细
            DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid, "1", false);
            #endregion
            //获取人工退料单据主档信息
            DataRow _drPrh = ce.SelectPickReturnHByNo(ref_doc_no);
            string _rgRefDocNo = _drPrh["ref_doc_no"].ToString();//人工领料单号

            #region 3.获取人工退料明细
            DataTable dtPickReturnDInfo = ce.SelectPickReturnDById(pickReturnhTid);
            #endregion

            #region 4.更新人工退料单据状态,为新建状态
            ce.UpdatePickReturnhStatusById(pickReturnhTid, PickReturnStatus.PickReturnStatus_New.ToString());
            //清空人工退料上关联的容器主档
            ce.UpdatePickReturnContainerIdById(pickReturnhTid, string.Empty);
            #endregion

            #region 5.更新任务和任务明细软删除标记为“0”，并清空容器明细中的待入量
            //修改任务软删除标记为“0”
            WorkTask.UpdateActStatus(model.TaskTid, "0");
            //更新任务物料软删除标记为“0”
            for (int i = 0; i < taskItemInfos.Length; i++)
            {
                string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
                decimal plan_qty = Convert.ToDecimal(taskItemInfos[i]["plan_qty"]);//任务中的计划数量
                string containerDTid = taskItemInfos[i]["container_d_id"].ToString();//容器明细id
                DataTable _dt = ce.GetFinishTaskItemInfo(_rgRefDocNo, containerDTid);//人工领料任务明细信息
                //恢复容器明细中库存量
                Container.UpdateDetailQty(containerDTid, Convert.ToDecimal(_dt.Rows[0]["plan_qty"].ToString()));
                ////重置软删除标记为1
                //Container.UpdateDetailActStatus(containerDTid, "1");
                //修改任务物料表中的软删除标记为“0”
                WorkTask.UpdateTaskItemActStatus(cdTid, "0");
            }
            #endregion
        }
        #endregion

        #region 退空托盘+要料
        /// <summary>
        /// 退空托盘+要料
        /// </summary>
        /// <param name="model"></param>
        public WebJsonInfo ReturnNullAndCallPallet(ReturnNullAndCallPalletModel model)
        {
            ClientReturnNullPalletModel crnp = new ClientReturnNullPalletModel();
            crnp.Machine = model.Machine;
            crnp.Dock = model.Dock;
            crnp.ProductPlanOutId = model.ProductPlanOutId;
            crnp.ShiftId = model.ShiftId;
            crnp.TeamId = model.TeamId;
            crnp.HasAGV = model.HasAGV;
            crnp.Planno = model.Planno;
            Model.WorkTaskModel returnNullPalletTask = null;
            WebJsonInfo info = WmsHelper.Do(new UserLogModel()
            {
                LogText = $"工单{model.ProductPlanOutId},机台{model.Machine},站台{model.Dock},退空托盘",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退空托盘+要料",
                CommonName = "api/wms/MachineMaterialService/ReturnNullAndCallPallet",
            }, () => ReturnNullPallet(crnp, out returnNullPalletTask));
            if (info.errcode != 0)//等于0，表示退空盘任务产生成功
            {
                info.errcode = 1;
                return info;
            }
            MachineMaterialModel mm = new MachineMaterialModel();
            mm.Machine = model.Machine;
            mm.Dock = model.Dock;
            mm.ProductPlanOutId = model.ProductPlanOutId;
            mm.ShiftId = model.ShiftId;
            mm.TeamId = model.TeamId;
            mm.HasAGV = model.HasAGV;
            mm.Planno = model.Planno;
            info = WmsHelper.Do(new UserLogModel()
            {
                LogText = $"工单{model.ProductPlanOutId},机台{model.Machine},站台{model.Dock},要料",
                LogTextLong = "参数：" + JsonConvert.SerializeObject(model),
                TypeCode = "退空托盘+要料",
                CommonName = "api/wms/MachineMaterialService/CreateOutBoundByMachineDock",
            }, () => CreateOutBoundByMachineDock(mm, returnNullPalletTask));
            if (info.errcode != 0)
            {
                //info.errmsg = "库存不足";
            }
            info.errcode = 0;
            return info;
        }

        /// <summary>
        /// 机台要料（依赖任务可为空）
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnNullPalletTask">退空盘任务</param>
        private void CreateOutBoundByMachineDock(MachineMaterialModel model, Model.WorkTaskModel returnNullPalletTask)
        {
            CreateOutBound(model, returnNullPalletTask);

            #region 旧版代码

            //#region 1.检查站台当前任务 
            //////查询站台未执行完成的任务数量
            ////DataRow dtTaskNumInfo = SelectTaskNumByDock(model.Dock);
            ////int cnt = Convert.ToInt32(dtTaskNumInfo["cnt"].ToString());//站台未执行完成的任务数量
            ////if (cnt > 0 && returnNullPalletTask == null)
            ////{
            ////    throw new WmsException(500, "该站台上有未执行完成的任务，无法要料");
            ////}
            //#endregion

            //#region 2.检查站台状态
            ////查询站台状态
            //DataRow dtDockInfo = ce.GetDockStatus(model.Dock);
            //string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            //string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            //string container_id = dtDockInfo["container_id"].ToString();//挂在站台上的容器主档id
            //string dock_name = dtDockInfo["dock_name"].ToString();//站台名称
            //string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            //string equipment_code = dtDockInfo["equipment_code"].ToString();//站台编码
            ////if (!string.IsNullOrEmpty(container_id))
            ////{
            ////    throw new WmsException(500, "当前站台位上存在物料，无法再次要料！");
            ////}
            ////if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
            ////{
            ////    throw new WmsException(500, "当前站台位处于故障状态，无法使用！");
            ////}
            ////if (dock_status == EquipmentStatus.EquipmentStatus_Using.ToString())
            ////{
            ////    throw new WmsException(500, "当前站台位已被使用！");
            ////}
            //if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
            //{
            //    throw new WmsException(500, "当前站台对应的工艺段已停用，无法进行要料！");
            //}
            //#endregion

            //#region 3.获取生产工单计划信息
            //DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(model.ProductPlanOutId);
            //if (dtProductPlanInfo == null || dtProductPlanInfo.Length < 1)
            //{
            //    throw new WmsException(500, "未查询到该机台对应的生产工单！");
            //}
            //if (dtProductPlanInfo.Length != 1)
            //{
            //    throw new WmsException(500, "该机台对应的正在生产中的工单不唯一！");
            //}
            //string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态
            //string job_no = dtProductPlanInfo[0]["job_no"].ToString();//生产工单单号
            //string product_item_id = dtProductPlanInfo[0]["item_id"].ToString();//成品物料id
            //string product_item_code = dtProductPlanInfo[0]["item_code"].ToString();//成品编码
            //string product_item_name = dtProductPlanInfo[0]["item_name"].ToString();//成品名称
            //if (plan_status != PlanOutStatus.PlanOutStatus_Executing.ToString())
            //{
            //    throw new WmsException(500, "该机台对应的生产工单不可执行！");
            //}
            //#endregion

            ////4.获取机台信息
            //DataRow _drMachineInfo = ce.GetMachineInfoByMachineId(model.Machine);
            ////机台所在区域
            //string machineZoneId = _drMachineInfo["zone_id"].ToString();

            //#region 4.获取机台BOM信息
            //DataTable dtBOMInfo = ce.GetBOMInfo(model.Dock);
            //if (dtBOMInfo.Rows.Count <= 0)
            //{
            //    throw new WmsException(500, "无法获取机台BOM信息！");
            //}
            //string bomId = dtBOMInfo.Rows[0]["t_id"].ToString();//bomId
            //#endregion

            //#region 5.获取货位信息
            ////通过货位分配策略，获取可用货位
            //string loc_id = ce.GetLocationInfoByBomId(bomId, machineZoneId);//"1288307405184176201";

            ////查询货位对应区域信息
            //DataTable dtLocationInfo = ce.GetLocationInfo(loc_id);
            ////货位状态
            //string location_status = dtLocationInfo.Rows[0]["location_status"].ToString();
            ////区域
            //string zone_id = dtLocationInfo.Rows[0]["zone_id"].ToString();
            ////货位对应的编码
            //string loc_no = dtLocationInfo.Rows[0]["location_code"].ToString();

            //if (location_status != LocationStatus.LocationStatus_Stored.ToString())
            //{
            //    throw new WmsException(500, "货位分配策略分配的货位，状态不为已存储状态，无法进行出库");
            //}
            //#endregion

            //#region 6.获取货位对应容器信息
            ////查询库位对应的容器头表信息
            //DataRow[] _dtHeadsInfo = Container.GetHeadsByLocation(loc_id);
            //if (_dtHeadsInfo == null || _dtHeadsInfo.Length <= 0)
            //{
            //    throw new WmsException(500, "货位分配返回空货位");
            //}
            //DataRow dtcontainerhInfo = _dtHeadsInfo[0];
            ////容器头表主键
            //string container_htid = dtcontainerhInfo["t_id"].ToString();
            ////容器头表编码
            //string container_barcode = dtcontainerhInfo["container_barcode"].ToString();
            ////查询容器明细信息
            //DataRow[] dtrContainerDInfo = Container.GetDetailByHeadID(container_htid);
            //#endregion

            ////查询执行设备(堆垛机)
            //DataRow equipRow = Equipment.GetStackByLocation(loc_id);

            //#region 7.产生要料任务，包括修改站台状态
            ////根据站台修改机台站台表中的容器id，并将生产工单id挂在挂站台上，用于退余料、退坏料和退空托盘
            ////UpdateMachineDockDeatilId(machineDockTid, container_htid, model.ProductPlanOutId, bomId, string.Empty);
            ////修改站台状态
            //ce.UpdateDockStatus(machineDockTid, model.Dock, EquipmentStatus.EquipmentStatus_Using.ToString());

            //#region 插入任务主档
            //Model.WorkTaskModel taskModel = new Model.WorkTaskModel()
            //{
            //    ID = SnowflakeID.Guid().ToString(),//主键ID
            //    ZoneID = zone_id,//区域ID
            //    TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
            //    DependentTaskNo = returnNullPalletTask.TaskNo,//依赖任务号
            //    TaskType = TaskType.TaskType_OutBound.ToString(),//任务类型编码 "机台要料"
            //    ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
            //    //ExecutedBy = "",//任务执行人
            //    EquipmentID = equipRow == null ? "" : equipRow["t_id"].ToString(),//执行设备ID
            //    //EquipmentType = "",//执行设备类型
            //    //ActualEquipmentID = "",//实际执行设备ID
            //    SeqNo = 1,//顺序号
            //    Priority = 1,//优先级
            //    ContainerID = container_htid,//容器ID
            //    ContainerBarcode = container_barcode,//容器条码
            //    RefDocType = DocType.DocType_CallTerminal.ToString(),//来源单据类型
            //    RefDocID = model.ProductPlanOutId,//来源单ID
            //    //RefDocDetailID = "",//来源单明细ID
            //    RefDocNo = job_no,//来源单号
            //    SrcAddrID = loc_id,//来源地址ID
            //    SrcAddrType = AddressType.AddressType_Location.ToString(),//来源地址类型 "location"
            //    SrcAddrText = loc_no,//来源地址
            //    DestAddrID = model.Dock,//目标地址ID
            //    DestAddrType = AddressType.AddressType_MachineStation.ToString(),//目标地址类型"dock"
            //    DestAddrText = equipment_code,//目标地址
            //    //ActualDestAddrID = "",//实际目标地址ID
            //    //ActualDestAddrText = "",//实际目标地址
            //    TaskStatus = TaskStatus.TaskStatus_NotExecute.ToString(),//任务状态
            //    //StartDate = dateTime,//开始时间
            //    //EndDate = dateTime,//结束时间
            //    IsExecutable = "0",//任务是否可执行
            //    //PriorityChangeTime = dateTime,//优先级修改时间        
            //    LedText = "",//LED显示内容
            //    ExistLgv = model.HasAGV,//是否存在LGV
            //    //CreateDate = dateTime,//创建时间
            //    //CreatedBy = "",//创建人帐号
            //    //ModifyIP = "",//操作人IP 
            //    //ModifyDate = dateTime,//编辑时间
            //    //LastModifiedBy = "",//最后修改人帐号
            //    ActStatus = "1",//状态
            //};
            //WorkTask.Insert(taskModel);
            //#endregion

            //for (int i = 0; i < dtrContainerDInfo.Length; i++)
            //{
            //    decimal qty = Convert.ToDecimal(dtrContainerDInfo[i]["qty"].ToString() == "" ? "0" : dtrContainerDInfo[i]["qty"].ToString());
            //    //修改容器明细中物料待出数量
            //    Container.UpdateDetailOutQty(dtrContainerDInfo[i]["t_id"].ToString(), qty);
            //    #region 插入任务物料信息
            //    WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
            //    {
            //        ID = SnowflakeID.Guid().ToString(),//主键ID
            //        //ClientID = "",//集团ID
            //        //OrgID = "",//组织ID
            //        //WarehouseID = "",//仓库ID
            //        TaskID = taskModel.ID,//主任务ID
            //        ItemID = dtrContainerDInfo[i]["item_id"].ToString(),//物料ID
            //        ItemName = dtrContainerDInfo[i]["item_name"].ToString(),//物料名称
            //        PlanQty = Convert.ToDecimal(dtrContainerDInfo[i]["qty"].ToString()),//计划数量
            //        //ProcessedQty = "",//已处理数量
            //        //FinishedQty = "",//完成数量
            //        UomQtyID = dtrContainerDInfo[i]["uom_qty_id"].ToString(),//数量单位
            //        //FinishedStatQty = "",//完成统计数量
            //        UomStatID = dtrContainerDInfo[i]["uom_stat_id"].ToString(),//统计数量单位
            //        ContainerDetailID = dtrContainerDInfo[i]["t_id"].ToString(),//容器明细ID
            //        RefDocType = DocType.DocType_CallTerminal.ToString(),//来源单据类型  "机台要料"
            //        RefDocID = model.ProductPlanOutId,//来源单ID
            //        //RefDocDetailID = "",//来源单明细ID
            //        RefDocNo = job_no,//来源单号
            //        //StartDate = "",//开始时间
            //        //EndDate = "",//结束时间
            //        ProcessStatus = ProcessStatus.ProcessStatus_New.ToString()//工作状态
            //        //CreateDate = "",//创建时间
            //        //CreatedBy = "",//创建人帐号 
            //        //ModifyIP = "",//操作人IP 
            //        //ModifyDate = "",//编辑时间
            //        //LastModifiedBy = "",//最后修改人帐号
            //        //ActStatus = ""//状态
            //    };
            //    WorkTaskItem.Insert(taskItemModel);
            //    #endregion
            //}
            //#endregion

            //#region 8.获取交接班和人工领料站台
            //////获取交接班站台
            ////DataRow drShiftDockInfo = ce.GetShiftDockByMachine(model.Machine);
            ////string jiaojiebanDockTid = drShiftDockInfo["t_id"].ToString();//交接班站台在机台站台表中的t_id 
            //#endregion

            //#region 9.清空挂在交接班和人工领料站台上的容器
            //////清空挂在交接班站台上的容器
            ////ce.UpdateMachineDockDeatilId(jiaojiebanDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            //////清空挂在人工领料站台上的容器
            ////UpdateMachineDockDeatilId(jiebanDockTid, string.Empty, string.Empty);
            //#endregion

            //#region 10.修改货位状态为待出库
            ////修改货位状态为待出库
            //ce.UpdateLocStatusByLocationId(loc_id, LocationStatus.LocationStatus_Out.ToString(), location_status, "机台要料出库,修改货位状态为待出库");
            //#endregion 


            #endregion
        }
        #endregion

        #region 退空托盘
        /// <summary>
        /// 退空托盘
        /// </summary>
        /// <param name="model"></param>
        public void ClientReturnNullPallet(ClientReturnNullPalletModel model)
        {
            ReturnNullPallet(model, out Model.WorkTaskModel returnNullPalletTaskModel);
        }

        #region 退空托盘,带出任务信息
        /// <summary>
        /// 退空托盘,带出任务信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnNullPalletTaskModel">带出任务信息</param>
        private void ReturnNullPallet(ClientReturnNullPalletModel model, out Model.WorkTaskModel returnNullPalletTaskModel)
        {
            #region 1.检查站台当前任务 
            //查询站台未执行完成的任务数量
            DataRow dtTaskNumInfo = ce.SelectTaskNumByDock(model.Dock);
            int cnt = Convert.ToInt32(dtTaskNumInfo["cnt"].ToString());//站台未执行完成的任务数量
            if (cnt > 0)
            {
                throw new WmsException(1, "该站台上有未执行完成的任务，无法退空托盘");
            }
            #endregion

            if (ce.MachineHaveDockInfo(model.Machine, model.Dock) == false)
            {
                throw new WmsException(1, "机台上没有该站台，请重新登录账号！");
            }

            #region 2.检查站台状态
            //查询站台状态
            DataRow dtDockInfo = ce.GetDockStatus(model.Dock);
            string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            string doc_id = dtDockInfo["doc_id"].ToString();//来源单号，此处存放的是要料时的生产工单
            string equipment_code = dtDockInfo["equipment_code"].ToString();//站台编码
            string container_id = dtDockInfo["container_id"].ToString();//容器主档id
            string bom_hid = dtDockInfo["bom_code"].ToString();//bom主档id
            string zoneId = dtDockInfo["zone_id"].ToString();//站台所在的区域
            string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
            {
                throw new WmsException(1, "当前站台位处于故障状态，无法使用！");
            }
            if (dock_status != EquipmentStatus.EquipmentStatus_Using.ToString())
            {
                throw new WmsException(1, "当前站台位未使用！");
            }
            if (string.IsNullOrEmpty(container_id))
            {
                throw new WmsException(1, "当前站台位无托盘信息！");
            }
            //if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
            //{
            //    throw new WmsException(500, "当前站台对应的工艺段已停用，无法进行退空托盘！");
            //}
            #endregion

            #region 3.获取空托盘存放的指定位置
            //获取空托盘存放的指定位置
            DataTable NullPalletDestAddrInfo = ce.GetReturnNullPalletDestAddrInfo(zoneId, EquipmentUsage.EquipmentUsage_ReturnNullPallet.ToString());
            if (NullPalletDestAddrInfo == null || NullPalletDestAddrInfo.Rows.Count <= 0)
            {
                throw new WmsException(1, "退空托盘时未查询到目的站台信息，请创建目的站台信息后在进行退空托盘！");
            }
            if (NullPalletDestAddrInfo.Rows.Count > 1)
            {
                throw new WmsException(1, "退空托盘时查询到多个目的站台信息，目的站台只能有一个！");
            }
            string destAddrID = NullPalletDestAddrInfo.Rows[0]["t_id"].ToString();//退空托盘时的目的输送线站台id
            string destAddrText = NullPalletDestAddrInfo.Rows[0]["equipment_code"].ToString();//退空托盘时的目的输送线站台编码
            #endregion

            #region 4.获取容器信息
            //根据容器主档id，获取容器主档信息
            DataRow dtrContainerInfo = Container.GetContainerH(container_id);

            //查询容器明细
            DataRow[] dtrContainerDInfo = Container.GetDetailByHeadID(container_id);

            #endregion

            #region 5.获取要料时的生产工单计划信息
            DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(doc_id);
            string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态
            string job_no = dtProductPlanInfo[0]["job_no"].ToString();//生产工单单号
            string product_item_id = dtProductPlanInfo[0]["item_id"].ToString();//成品物料id
            string product_item_code = dtProductPlanInfo[0]["item_code"].ToString();//成品编码
            string product_item_name = dtProductPlanInfo[0]["item_name"].ToString();//成品名称
            #endregion

            //获取AGV
            DataRow drAGV = ce.GetAgv();

            #region 6.产生任务
            DateTime dateTime = DateTime.Now;

            #region 产生空托盘入库任务，仅需要插入任务主档
            returnNullPalletTaskModel = new Model.WorkTaskModel()
            {
                ID = SnowflakeID.Guid().ToString(),//主键ID
                ZoneID = zoneId,//区域ID
                TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                //DependentTaskNo = "",//依赖任务号
                TaskType = TaskType.TaskType_InBound.ToString(),//任务类型编码"退空托盘"
                ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                //ExecutedBy = "",//任务执行人
                EquipmentID = drAGV["t_id"].ToString(),//执行设备ID
                //EquipmentType = "",//执行设备类型
                //ActualEquipmentID = "",//实际执行设备ID
                SeqNo = 1,//顺序号
                Priority = 1,//优先级
                ContainerID = container_id,//容器ID
                //WmsConfig.GetOrDefault("CallEnd:NullPalletBarCode", "99999999")
                ContainerBarcode = WmsConfig.GetOrDefault("CallEnd:NullPalletBarCode", "99999999"),//容器条码dtrContainerInfo["container_barcode"].ToString()
                RefDocType = DocType.DocType_ClientReturnNullPallet.ToString(),//来源单据类型
                RefDocID = doc_id,//来源单ID
                RefDocDetailID = model.Planno,//来源单明细ID,此处存放mes下发的号
                RefDocNo = job_no,//来源单号
                SrcAddrID = model.Dock,//来源地址ID
                SrcAddrType = AddressType.AddressType_MachineStation.ToString(),//来源地址类型
                SrcAddrText = equipment_code,//来源地址
                DestAddrID = destAddrID,//目标地址ID             
                DestAddrType = AddressType.AddressType_Station.ToString(),//目标地址类型            
                DestAddrText = destAddrText,//目标地址         
                //ActualDestAddrID = "",//实际目标地址ID
                //ActualDestAddrText = "",//实际目标地址
                TaskStatus = TaskStatus.TaskStatus_Executing.ToString(),//任务状态
                //StartDate = dateTime,//开始时间
                //EndDate = dateTime,//结束时间
                IsExecutable = "1",//任务是否可执行
                //PriorityChangeTime = dateTime,//优先级修改时间        
                LedText = "",//LED显示内容
                ExistLgv = model.HasAGV,//是否存在LGV
                //CreateDate = dateTime,//创建时间
                //CreatedBy = "",//创建人帐号
                //ModifyIP = "",//操作人IP 
                //ModifyDate = dateTime,//编辑时间
                //LastModifiedBy = "",//最后修改人帐号
                ActStatus = "1"//状态
            };
            WorkTask.Insert(returnNullPalletTaskModel);
            #endregion

            #region 创建入库任务物料明细

            #region 插入入库任务物料信息
            for (int i = 0; i < dtrContainerDInfo.Length; i++)
            {
                decimal qty = Convert.ToDecimal(dtrContainerDInfo[i]["qty"]);
                string itemId = dtrContainerDInfo[i]["item_id"].ToString();
                //修改容器明细中物料数量
                Container.UpdateDetailQty(dtrContainerDInfo[i]["t_id"].ToString(), (decimal)0.0);
                WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                    //ClientID ="",//集团ID
                    //OrgID ="",//组织ID
                    //WarehouseID ="",//仓库ID
                    TaskID = returnNullPalletTaskModel.ID,//主任务ID
                    ItemID = itemId,//物料ID
                    ItemName = dtrContainerDInfo[i]["item_name"].ToString(),//物料名称
                    PlanQty = (decimal)0.0,//计划数量
                    //ProcessedQty = qty,//已处理数量
                    //FinishedQty = qty,//完成数量
                    UomQtyID = dtrContainerDInfo[i]["uom_qty_id"].ToString(),//数量单位
                    //FinishedStatQty="",//完成统计数量
                    UomStatID = dtrContainerDInfo[i]["uom_stat_id"].ToString(),//统计数量单位
                    ContainerDetailID = dtrContainerDInfo[i]["t_id"].ToString(),//容器明细ID
                    RefDocType = DocType.DocType_ClientReturnNullPallet.ToString(),//来源单据类型"退空托盘产生的出库任务"
                    RefDocID = model.ProductPlanOutId,//来源单ID
                    //RefDocDetailID ="",//来源单明细ID
                    RefDocNo = job_no,//来源单号
                    //StartDate = dateTime,//开始时间
                    //EndDate = dateTime,//结束时间
                    ProcessStatus = ProcessStatus.ProcessStatus_New.ToString(),//工作状态
                    //CreateDate="",//创建时间
                    //CreatedBy ="",//创建人帐号 
                    //ModifyIP ="",//操作人IP 
                    //ModifyDate="",//编辑时间
                    //LastModifiedBy ="",//最后修改人帐号
                    //ActStatus =""//状态
                };
                WorkTaskItem.Insert(taskItemModel);
            }
            #endregion

            #endregion

            ////修改站台状态
            //UpdateDockStatus(machineDockTid, model.Dock, EquipmentStatus.EquipmentStatus_Using.ToString());
            #endregion

            #region 7.获取交接班和人工领料站台
            ////获取交接班站台
            //DataRow drShiftDockInfo = ce.GetShiftDockByMachine(model.Machine);
            //string jiaojiebanDockTid = drShiftDockInfo["t_id"].ToString();//交接班站台在机台站台表中的t_id 
            ////获取人工领料站台
            //DataRow drJieBDockInfo = GetJieBDockByMachine(model.Machine);
            //string jiebanDockTid = drJieBDockInfo["t_id"].ToString();//人工领料站台在机台站台表中的t_id 
            #endregion

            #region 8.清空挂在交接班和人工领料站台上的容器
            ////清空挂在交接班站台上的容器
            //ce.UpdateMachineDockDeatilId(jiaojiebanDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            ////清空挂在人工领料站台上的容器
            //UpdateMachineDockDeatilId(jiebanDockTid, string.Empty, string.Empty);
            #endregion

            //发送给wcs
            SendToWCS(returnNullPalletTaskModel.ID);
        }
        #endregion

        #endregion

        #region 退空托盘任务强制完成
        /// <summary>
        /// 退空托盘任务强制完成
        /// </summary>
        /// <param name="model"></param>
        public void FinishClientReturnNullPallet(FinishClientReturnNullPalletModel model)
        {
            #region 1.获取退空盘任务信息
            //获取退空盘任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退空盘任务！");
            }
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退空托盘时托盘目的库位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ClientReturnNullPallet.ToString())
            {
                throw new WmsException(500, "该任务不是退空盘任务，无法强制完成！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "任务已完成，无需再次完成！");
            }
            #endregion
            //任务不是部分完成，则执行部分完成
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                PartionFinishReturnNullPalletModel pft = new PartionFinishReturnNullPalletModel();
                pft.TaskTid = model.TaskTid;
                PartionFinishReturnNullPallet(pft);
            }
            //执行完成任务
            FinishReturnNullPalletModel ft = new FinishReturnNullPalletModel();
            ft.TaskTid = model.TaskTid;
            FinishReturnNullPallet(ft);

            #region 旧版强完代码
            //#region 1.获取退空盘任务信息
            ////获取退空盘任务信息
            //DataTable dtTaskInfo = GeTaskInfoByTaskId(model.TaskTid);
            //if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            //{
            //    throw new WmsException(500, "未查询到站台退空盘任务！");
            //}
            //string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            //string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            //string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            //string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            //string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            //string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退空托盘时托盘目的库位id
            //string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            //if (refDocType != DocType.DocType_ClientReturnNullPallet.ToString())
            //{
            //    throw new WmsException(500, "该任务不是退空盘任务，无法强制完成！");
            //}
            //if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString() && taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            //{
            //    throw new WmsException(500, "任务仅在处于执行中或者已部分完成状态下，才能强制完成！");
            //}
            //#endregion

            //#region 2.检查站台状态
            ////检查站台状态
            //DataRow dtDockInfo = GetDockStatus(dockId);
            //string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            //string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            //string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            //if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
            //{
            //    throw new WmsException(500, "当前站台位处于故障状态，无法使用！");
            //}
            ////if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
            ////{
            ////    throw new WmsException(500, "当前站台对应的工艺段已停用，无法进行退空托盘！");
            ////}
            //#endregion

            //#region 3.获取挂在站台上的容器明细
            ////获取挂在站台上的容器明细
            //DataRow[] dtrContainerDInfo = Container.GetDetailByHeadID(container_id);
            //#endregion

            //#region 4.获取任务物料明细
            ////获取任务物料明细
            //DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
            //#endregion

            //#region 5.将容器明细物料数量为0，则软删除容器明细
            ////容器明细中物料数量qty全为0的情况，已在前台界面限制，走退空托盘业务
            ////该目的是在统计消耗时直接使用出库数量减去容器明细中的数量，作为消耗量
            //for (int i = 0; i < dtrContainerDInfo.Length; i++)
            //{
            //    string containerDetailId = dtrContainerDInfo[i]["t_id"].ToString();
            //    decimal qty = Convert.ToDecimal(dtrContainerDInfo[i]["qty"].ToString());
            //    //if (in_qty <= 0)//物料数量为0，则删除容器明细，设置成软删除
            //    //{
            //    //    Container.UpdateDetailActStatus(containerDetailId, "0");
            //    //}
            //    Container.UpdateDetailQty(containerDetailId, (decimal)0.0).UpdateDetailActStatus(containerDetailId, "0");
            //}
            //#endregion

            //#region 6.将退空盘任务更新为完成
            ////将退空盘任务更新为完成
            //WorkTask.UpdateTaskComplete(model.TaskTid);
            ////更新任务物料明细中的已处理数量和完成数量
            //for (int i = 0; i < taskItemInfos.Length; i++)
            //{
            //    string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料明细id
            //    string containerDTid = taskItemInfos[i]["container_d_id"].ToString();//容器明细id
            //    //根据任务物料id，修改任务物料状态为已完成，并且修改完成和处理数量
            //    WorkTask.UpdateTaskItemCompleteAndQty(cdTid);
            //    //容器明细软删除标记置为0
            //    Container.UpdateDetailActStatus(containerDTid, "0");
            //}
            //WorkTask.MoveToHistory(model.TaskTid);
            //#endregion

            //#region 7.将容器主档与库位关联
            ////将容器主档与库位关联
            //Container.UpdateContainerLocation(container_id, locId);
            //#endregion

            //#region 8.清空挂在站台上的容器
            ////清空挂在站台上的容器
            //UpdateMachineDockDeatilId(machineDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            //#endregion

            //#region 9.修改站台状态
            ////修改站台状态
            //UpdateDockStatus(machineDockTid, dockId, EquipmentStatus.EquipmentStatus_OK.ToString());
            //#endregion 
            #endregion
        }
        #endregion

        #region 退空托盘任务回收
        /// <summary>
        /// 退空托盘任务回收
        /// </summary>
        /// <param name="model"></param>
        public void RecycleClientReturnNullPallet(RecycleClientReturnNullPalletModel model)
        {
            #region 1.获取退空盘任务信息
            //获取退空盘任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退空盘任务！");
            }
            string taskNo = dtTaskInfo.Rows[0]["task_no"].ToString();//任务号
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退空托盘时托盘目的库位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ClientReturnNullPallet.ToString())
            {
                throw new WmsException(500, "该任务不是退空盘任务，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "该任务已完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "该任务已部分完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "该任务正在执行中，无法回收任务！");
            }
            #endregion

            #region 2.获取任务物料明细
            //获取任务物料明细
            DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
            #endregion

            //回收时不修改站台状态是因为站台本身就处于使用状态，因此不需要修改

            #region 3.获取要料或者接班任务，并恢复退空托盘任务产生前容器中的物料数量
            //获取要料或者接班任务
            DataTable theNewTaskItemInfo = ce.GetTheNewTaskInfoByPlanOutId(productPlanOutId);

            //根据要料或者接班任务，恢复退空托盘任务产生前容器中的物料数量
            for (int i = 0; i < theNewTaskItemInfo.Rows.Count; i++)
            {
                string cdTid = theNewTaskItemInfo.Rows[i]["container_d_id"].ToString();
                decimal qty = Convert.ToDecimal(theNewTaskItemInfo.Rows[i]["plan_qty"].ToString());
                Container.UpdateDetailQty(cdTid, qty);
            }
            #endregion

            #region 4.更新任务和任务明细软删除标记为“0”
            //修改任务软删除标记为“0”
            WorkTask.UpdateActStatus(model.TaskTid, "0");
            //更新任务物料软删除标记为“0”
            for (int i = 0; i < taskItemInfos.Length; i++)
            {
                string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
                string containerDTid = taskItemInfos[i]["container_d_id"].ToString();//容器明细id
                //修改任务物料表中的软删除标记为“0”
                WorkTask.UpdateTaskItemActStatus(cdTid, "0");
            }
            #endregion

            #region 6.存在依赖任务，则回收。否则忽略此步
            DataTable _dtTask = ce.GetDependentTask(taskNo);
            //存在依赖任务
            if (_dtTask.Rows.Count > 0)
            {
                string _taskId = _dtTask.Rows[0]["t_id"].ToString();
                RecycleMachineMaterialModel rmt = new RecycleMachineMaterialModel();
                rmt.TaskTid = _taskId;
                RecycleMachineMaterial(rmt);
            }
            #endregion
        }
        #endregion

        #region 退空托盘任务部分完成
        /// <summary>
        /// 退空托盘任务部分完成
        /// </summary>
        /// <param name="model"></param>
        public void PartionFinishReturnNullPallet(PartionFinishReturnNullPalletModel model)
        {
            #region 1.获取退空盘任务信息
            //获取退空盘任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退空盘任务！");
            }
            string taskNo = dtTaskInfo.Rows[0]["task_no"].ToString();//任务号
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            //string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退空托盘时托盘目的库位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ClientReturnNullPallet.ToString())
            {
                throw new WmsException(500, "该任务不是退空盘任务，无法部分完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "任务仅在处于执行中状态下，才能部分完成！");
            }
            #endregion

            #region 7.将任务修改为部分完成
            WorkTask.UpdateTaskStatus(model.TaskTid, TaskStatus.TaskStatus_PartialFinished.ToString(), TaskStatus.TaskStatus_Executing.ToString());
            #endregion

            #region 2.存在依赖任务，则修改是否可执行状态，并下发任务。否则忽略此步
            DataTable _dtTask = ce.GetDependentTask(taskNo);
            //存在依赖任务
            if (_dtTask.Rows.Count > 0)
            {
                string _taskId = _dtTask.Rows[0]["t_id"].ToString();
                ce.UpdateTaskIsExecutableStatus(_taskId);
                //发送给wcs
                SendToWCS(_taskId);
                return;
            }
            #endregion

            #region 3.获取站台信息
            DataRow dtDockInfo = ce.GetDockStatus(dockId);
            string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            #endregion

            #region 4.获取挂在站台上的容器明细
            //获取挂在站台上的容器明细
            DataRow[] dtrContainerDInfo = Container.GetDetailByHeadID(container_id);
            #endregion

            #region 5.将容器明细物料数量为0，则软删除容器明细
            //容器明细中物料数量qty全为0的情况，已在前台界面限制，走退空托盘业务
            //该目的是在统计消耗时直接使用出库数量减去容器明细中的数量，作为消耗量
            for (int i = 0; i < dtrContainerDInfo.Length; i++)
            {
                string containerDetailId = dtrContainerDInfo[i]["t_id"].ToString();
                decimal qty = Convert.ToDecimal(dtrContainerDInfo[i]["qty"].ToString());
                Container.UpdateDetailQty(containerDetailId, (decimal)0.0).UpdateDetailActStatus(containerDetailId, "0");
            }
            #endregion

            #region 6.清空挂在站台上的容器
            //清空挂在站台上的容器
            ce.UpdateMachineDockDeatilId(machineDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            #endregion

            #region 7.修改站台状态
            //修改站台状态
            ce.UpdateDockStatus(machineDockTid, dockId, EquipmentStatus.EquipmentStatus_OK.ToString());
            #endregion
        }
        #endregion

        #region 退空托盘任务完成
        /// <summary>
        /// 退空托盘任务完成
        /// </summary>
        /// <param name="model"></param>
        public void FinishReturnNullPallet(FinishReturnNullPalletModel model)
        {
            #region 1.获取退空盘任务信息
            //获取退空盘任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退空盘任务！");
            }
            string taskNo = dtTaskInfo.Rows[0]["task_no"].ToString();//任务号
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退空托盘时托盘目的库位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ClientReturnNullPallet.ToString())
            {
                throw new WmsException(500, "该任务不是退空盘任务，无法强制完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString() && taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "任务仅在处于执行中或者已部分完成状态下，才能强制完成！");
            }
            #endregion

            #region 2.获取站台信息
            DataRow dtDockInfo = ce.GetDockStatus(dockId);
            string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            //if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
            //{
            //    throw new WmsException(500, "当前站台位处于故障状态，无法使用！");
            //}
            //if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
            //{
            //    throw new WmsException(500, "当前站台对应的工艺段已停用，无法进行退空托盘！");
            //}
            #endregion

            #region 3.获取任务物料明细
            //获取任务物料明细
            DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
            #endregion

            #region 4.将退空盘任务更新为完成
            //将退空盘任务更新为完成
            WorkTask.UpdateTaskComplete(model.TaskTid);
            //更新任务物料明细中的已处理数量和完成数量
            for (int i = 0; i < taskItemInfos.Length; i++)
            {
                string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料明细id
                string containerDTid = taskItemInfos[i]["container_d_id"].ToString();//容器明细id
                //根据任务物料id，修改任务物料状态为已完成，并且修改完成和处理数量
                WorkTask.UpdateTaskItemCompleteAndQty(cdTid);
                //容器明细软删除标记置为0
                Container.UpdateDetailActStatus(containerDTid, "0");
            }
            WorkTask.MoveToHistory(model.TaskTid);
            #endregion

            #region 6.判断退空盘任务完成时，依赖该任务的要料任务，是否允许完成如果允许，则完成要料任务。否则忽略此步
            DataTable _dtTask = ce.GetDependentTask(taskNo);
            //存在依赖任务
            if (_dtTask.Rows.Count > 0 && string.Equals(_dtTask.Rows[0]["LED_TEXT"].ToString(), "55555555"))
            {

                string _taskId = _dtTask.Rows[0]["t_id"].ToString();
                FinishMaterialModel rmt = new FinishMaterialModel();
                rmt.TaskTid = _taskId;
                FinishMaterial(rmt);
            }
            #endregion
        }
        #endregion

        #region 退坏料
        /// <summary>
        /// 退坏料
        /// </summary>
        /// <param name="model"></param>
        public void ClientReturnBadPallet(ClientReturnBadPalletModel model)
        {
            Putaway putaway = new Putaway();

            #region 1.检查站台当前任务 
            //查询站台未执行完成的任务数量
            DataRow dtTaskNumInfo = ce.SelectTaskNumByDock(model.Dock);
            int cnt = Convert.ToInt32(dtTaskNumInfo["cnt"].ToString());//站台未执行完成的任务数量
            if (cnt > 0)
            {
                throw new WmsException(500, "该站台上有未执行完成的任务，无法退坏料");
            }
            #endregion

            if (ce.MachineHaveDockInfo(model.Machine, model.Dock) == false)
            {
                throw new WmsException(1, "机台上没有该站台，请重新登录账号！");
            }

            #region 2.检查站台状态
            //查询站台状态
            DataRow dtDockInfo = ce.GetDockAndBomInfo(model.Dock);
            string machine_id = dtDockInfo["machine_id"].ToString();//机台id
            string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            string doc_id = dtDockInfo["doc_id"].ToString();//来源单号，此处存放的是要料时的生产工单
            string equipment_code = dtDockInfo["equipment_code"].ToString();//站台编码
            string container_id = dtDockInfo["container_id"].ToString();//容器主档id
            string bomId = dtDockInfo["bomId"].ToString();//bomId
            string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
            {
                throw new WmsException(500, "当前站台位处于故障状态，无法使用！");
            }
            if (dock_status != EquipmentStatus.EquipmentStatus_Using.ToString())
            {
                throw new WmsException(500, "当前站台位未使用！");
            }
            if (string.IsNullOrEmpty(container_id))
            {
                throw new WmsException(500, "当前站台位无托盘信息！");
            }
            if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
            {
                throw new WmsException(500, "当前站台对应的工艺段已停用，无法进行退坏料！");
            }
            #endregion

            DataRow _drMachineInfo = ce.GetMachineInfoByMachineId(machine_id);

            #region 3.获取坏料存放的货位
            //string destAddrID = ce.GetEmptyLocation();
            //参数：仓库id，容器明细中物料id，bomId，区域id
            string destAddrID = putaway.AllocateOneLocationForPallet(_drMachineInfo["warehouse_id"].ToString(), "", bomId, _drMachineInfo["zone_id"].ToString());
            #endregion

            if (destAddrID.Length <= 3)//小于等于3，表示获取的是行，不是货位id.只要二楼才会出现小于等于3的情况
            {
                if (string.IsNullOrEmpty(destAddrID))
                {
                    throw new WmsException(1, "分配货位失败");
                }
                TwoFloorReturnBadPallet(model, equipment_code, destAddrID, doc_id);
                return;
            }

            #region 4.获取货位信息
            //查询货位对应区域信息和容器信息
            DataTable dtLocationInfo = ce.GetLocationInfo(destAddrID);
            if (dtLocationInfo.Rows.Count > 1)
            {
                throw new WmsException(500, "退坏料时分配的货位属于多个区域");
            }
            string locContainer_htid = dtLocationInfo.Rows[0]["t_id"].ToString();
            //货位对应的编码
            string destAddrText = dtLocationInfo.Rows[0]["location_code"].ToString();
            string zoneId = dtLocationInfo.Rows[0]["zone_id"].ToString();
            //货位状态
            string location_status = dtLocationInfo.Rows[0]["location_status"].ToString();
            //判断货位分配策略返回的货位上是否已经与容器主档关联，如果有关联，则说明货位上有物料
            if (!string.IsNullOrEmpty(locContainer_htid))
            {
                throw new WmsException(500, "退坏料时分配的货位上已有物料，退料失败！");
            }
            if (location_status != LocationStatus.LocationStatus_Empty.ToString())
            {
                throw new WmsException(500, "货位分配策略分配的货位，状态不为空，无法进行退坏料");
            }
            #endregion

            //查询执行设备(堆垛机)
            string ddjEquipId = ce.GetDDJ(destAddrID);
            #region 5.将退余料json信息转化成json对象
            //退余料数据信息
            JObject entity = JObject.Parse(model.Details);
            if (entity["ReturnItems"].Count() == 0)
            {
                throw new WmsException(500, "无法获取退坏料明细数据！");
            }
            #endregion

            #region 6.获取容器主档信息，容器明细信息
            //获取挂在当前站台上容器id
            DataTable containerHInfo = ce.GetContainerInfoByDockIdAndMachineId(model.Dock, model.Machine);
            if (containerHInfo.Rows.Count > 1)
            {
                throw new WmsException(500, "根据机台和站台，获取挂在站台上面的容器主档不唯一");
            }
            string container_htid = containerHInfo.Rows[0]["container_id"].ToString();
            //根据容器主档id，获取容器主档信息
            DataRow dtrContainerInfo = Container.GetContainerH(container_htid);
            //查询容器明细
            DataTable dtContainerDInfo = ce.ToDataTable(Container.GetDetailByHeadID(container_htid));
            #endregion

            //修改容器主档是否允许被呼叫
            ce.UpdateContainerAllowCalled(container_htid, "0");

            #region 7.获取要料时的生产工单计划信息
            DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(doc_id);
            if (dtProductPlanInfo == null || dtProductPlanInfo.Length < 1)
            {
                throw new WmsException(500, "未查询到该机台对应的生产工单！");
            }
            string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态

            string job_no = dtProductPlanInfo[0]["job_no"].ToString();//生产工单单号
            string product_item_id = dtProductPlanInfo[0]["item_id"].ToString();//成品物料id
            string product_item_code = dtProductPlanInfo[0]["item_code"].ToString();//成品编码
            string product_item_name = dtProductPlanInfo[0]["item_name"].ToString();//成品名称
            #endregion

            #region 8.创建入库任务
            DateTime dateTime = DateTime.Now;

            #region 插入入库任务主档
            Model.WorkTaskModel inBoundTaskModel = new Model.WorkTaskModel()
            {
                ID = SnowflakeID.Guid().ToString(),//主键ID
                ZoneID = zoneId,//区域ID
                TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                //DependentTaskNo = "",//依赖任务号
                TaskType = TaskType.TaskType_InBound.ToString(),//任务类型编码"机台退坏料"
                ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                //ExecutedBy = "",//任务执行人
                EquipmentID = ddjEquipId,//执行设备ID
                //EquipmentType = "",//执行设备类型
                //ActualEquipmentID = "",//实际执行设备ID
                SeqNo = 1,//顺序号
                Priority = 1,//优先级
                ContainerID = container_htid,//容器ID
                ContainerBarcode = dtrContainerInfo["container_barcode"].ToString(),//容器条码
                RefDocType = DocType.DocType_ReturnBadPallet.ToString(),//来源单据类型"机台退坏料"
                RefDocID = doc_id,//来源单ID
                //RefDocDetailID = "",//来源单明细ID
                RefDocNo = job_no,//来源单号
                SrcAddrID = model.Dock,//来源地址ID
                SrcAddrType = AddressType.AddressType_MachineStation.ToString(),//来源地址类型"dock"
                SrcAddrText = equipment_code,//来源地址
                DestAddrID = destAddrID,//目标地址ID             
                DestAddrType = AddressType.AddressType_Location.ToString(),//目标地址类型 "location"            
                DestAddrText = destAddrText,//目标地址         
                //ActualDestAddrID = "",//实际目标地址ID
                //ActualDestAddrText = "",//实际目标地址
                TaskStatus = TaskStatus.TaskStatus_Executing.ToString(),//任务状态
                //StartDate = dateTime,//开始时间
                //EndDate = dateTime,//结束时间
                IsExecutable = "1",//任务是否可执行
                //PriorityChangeTime = dateTime,//优先级修改时间        
                LedText = "",//LED显示内容
                ExistLgv = model.HasAGV,//是否存在LGV
                //CreateDate = dateTime,//创建时间
                //CreatedBy = "",//创建人帐号
                //ModifyIP = "",//操作人IP 
                //ModifyDate = dateTime,//编辑时间
                //LastModifiedBy = "",//最后修改人帐号
                ActStatus = "1"//状态
            };
            WorkTask.Insert(inBoundTaskModel);
            #endregion

            for (int i = 0; i < entity["ReturnItems"].Count(); i++)
            {
                var job = entity["ReturnItems"][i];
                decimal qty = Convert.ToDecimal(job["qty"]);
                string containerDetailId = job["containerDetailId"].ToString();//容器明细id
                DataRow[] drs = dtContainerDInfo.Select($"t_id='{containerDetailId}'");
                decimal totalQty = Convert.ToDecimal(drs[0]["qty"].ToString());
                if (drs.Length == 1)
                {
                    //修改容器明细中物料数量
                    Container.UpdateDetailQty(containerDetailId, qty);
                }
                else
                {
                    throw new WmsException(500, "无法获取退坏料明细数据！");
                }
                #region 插入入库任务物料信息
                WorkTaskItemModel inBoundTaskItemModel = new WorkTaskItemModel()
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                                                       //ClientID = "",//集团ID
                                                       //OrgID = "",//组织ID
                                                       //WarehouseID = "",//仓库ID
                    TaskID = inBoundTaskModel.ID,//主任务ID
                    ItemID = drs[0]["item_id"].ToString(),//物料ID
                    ItemName = drs[0]["item_name"].ToString(),//物料名称
                    PlanQty = qty,//计划数量
                                  //ProcessedQty = "",//已处理数量
                                  //FinishedQty = "",//完成数量
                    UomQtyID = drs[0]["uom_qty_id"].ToString(),//数量单位
                                                               //FinishedStatQty = "",//完成统计数量
                    UomStatID = drs[0]["uom_stat_id"].ToString(),//统计数量单位
                    ContainerDetailID = drs[0]["t_id"].ToString(),//容器明细ID
                    RefDocType = DocType.DocType_ReturnBadPallet.ToString(),//来源单据类型"机台退坏料"
                    RefDocID = doc_id,//来源单ID
                                      //RefDocDetailID = "",//来源单明细ID
                    RefDocNo = job_no,//来源单号
                                      //StartDate = "",//开始时间
                                      //EndDate = "",//结束时间
                    ProcessStatus = ProcessStatus.ProcessStatus_New.ToString(),//工作状态
                                                                               //CreateDate = "",//创建时间
                                                                               //CreatedBy = "",//创建人帐号 
                                                                               //ModifyIP = "",//操作人IP 
                                                                               //ModifyDate = "",//编辑时间
                                                                               //LastModifiedBy = "",//最后修改人帐号
                    ActStatus = "1"//状态
                };
                WorkTaskItem.Insert(inBoundTaskItemModel);
                #endregion

            }
            #endregion

            #region 9.获取交接班和人工领料站台
            ////获取交接班站台
            //DataRow drShiftDockInfo = ce.GetShiftDockByMachine(model.Machine);
            //string jiaojiebanDockTid = drShiftDockInfo["t_id"].ToString();//交接班站台在机台站台表中的t_id 
            ////获取人工领料站台
            //DataRow drJieBDockInfo = GetJieBDockByMachine(model.Machine);
            //string jiebanDockTid = drJieBDockInfo["t_id"].ToString();//人工领料站台在机台站台表中的t_id 
            #endregion

            #region 10.清空挂在交接班和人工领料站台上的容器
            ////清空挂在交接班站台上的容器
            //ce.UpdateMachineDockDeatilId(jiaojiebanDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            ////清空挂在人工领料站台上的容器
            //UpdateMachineDockDeatilId(jiebanDockTid, string.Empty, string.Empty);
            #endregion

            #region 11.修改货位状态为待入库
            ce.UpdateLocStatusByLocationId(destAddrID, LocationStatus.LocationStatus_In.ToString(), location_status, "机台退坏料,物料入库,修改货位状态为待入库");
            #endregion

            //发送给wcs
            SendToWCS(inBoundTaskModel.ID);
        }

        /// <summary>
        /// 二楼退坏料，实际上是实托盘入库，此时需要进行两次货位分配
        /// 第一次分配到行
        /// 第二次分配到实际的货位
        /// 本方法为第一次货位分配后生成的任务
        /// </summary>
        /// <param name="model"></param>
        /// <param name="equipment_code"></param>
        /// <param name="line"></param>
        /// <param name="productPlanOutId"></param>
        /// <param name="taskModel"></param>
        private void TwoFloorReturnBadPallet(ClientReturnBadPalletModel model, string equipment_code, string line, string productPlanOutId)
        {
            #region 5.将退余料json信息转化成json对象
            //退余料数据信息
            JObject entity = JObject.Parse(model.Details);
            if (entity["ReturnItems"].Count() == 0)
            {
                throw new WmsException(1, "无法获取退余料明细数据！");
            }
            #endregion

            #region 7.获取要料时的生产工单计划信息
            DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(productPlanOutId);
            if (dtProductPlanInfo == null || dtProductPlanInfo.Length < 1)
            {
                throw new WmsException(1, "未查询到该机台对应的生产工单！");
            }
            string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态
            string job_no = dtProductPlanInfo[0]["job_no"].ToString();
            string product_item_id = dtProductPlanInfo[0]["item_id"].ToString();//成品物料id
            string product_item_code = dtProductPlanInfo[0]["item_code"].ToString();//成品编码
            string product_item_name = dtProductPlanInfo[0]["item_name"].ToString();//成品名称
            #endregion

            #region 6.获取容器主档信息，容器明细信息
            //获取挂在当前站台上容器id
            DataTable containerHInfo = ce.GetContainerInfoByDockIdAndMachineId(model.Dock, model.Machine);
            if (containerHInfo.Rows.Count > 1)
            {
                throw new WmsException(1, "根据机台和站台，获取挂在站台上面的容器主档不唯一");
            }
            string container_htid = containerHInfo.Rows[0]["container_id"].ToString();
            //根据容器主档id，获取容器主档信息
            DataRow dtrContainerInfo = Container.GetContainerH(container_htid);
            //查询容器明细
            DataTable dtContainerDInfo = ce.ToDataTable(Container.GetDetailByHeadID(container_htid));
            #endregion

            //修改容器主档是否允许被呼叫
            ce.UpdateContainerAllowCalled(container_htid, "0");

            DataRow drAGV = ce.GetAgv();

            #region 8.创建入库任务

            #region 插入任务主档
            Model.WorkTaskModel taskModel = new Model.WorkTaskModel()
            {
                ID = SnowflakeID.Guid().ToString(),//主键ID
                ZoneID = WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336"),//区域ID
                TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                //DependentTaskNo = "",//依赖任务号
                TaskType = TaskType.TaskType_InBound.ToString(),//任务类型编码"机台退余料"
                ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                //ExecutedBy = "",//任务执行人
                EquipmentID = drAGV["t_id"].ToString(),//执行设备ID
                //EquipmentType = "",//执行设备类型
                //ActualEquipmentID = "",//实际执行设备ID
                SeqNo = 1,//顺序号
                Priority = 1,//优先级
                ContainerID = container_htid,//容器ID
                ContainerBarcode = dtrContainerInfo["container_barcode"].ToString(),//容器条码
                RefDocType = DocType.DocType_ReturnBadPallet.ToString(),//来源单据类型  "机台退余料"
                RefDocID = productPlanOutId,//来源单ID
                //RefDocDetailID = "",//来源单明细ID
                RefDocNo = job_no,//来源单号
                SrcAddrID = model.Dock,//来源地址ID
                SrcAddrType = AddressType.AddressType_MachineStation.ToString(),//来源地址类型"dock"
                SrcAddrText = equipment_code,//来源地址
                DestAddrID = line,//目标地址ID
                DestAddrType = AddressType.AddressType_Location.ToString(),//目标地址类型"location"
                DestAddrText = line,//目标地址
                //ActualDestAddrID = "",//实际目标地址ID
                //ActualDestAddrText = "",//实际目标地址
                TaskStatus = TaskStatus.TaskStatus_Executing.ToString(),//任务状态
                //StartDate = dateTime,//开始时间
                //EndDate = dateTime,//结束时间
                IsExecutable = "1",//任务是否可执行
                //PriorityChangeTime = dateTime,//优先级修改时间        
                LedText = "",//LED显示内容
                ExistLgv = model.HasAGV,//是否存在LGV
                //CreateDate = dateTime,//创建时间
                //CreatedBy = "",//创建人帐号
                //ModifyIP = "",//操作人IP 
                //ModifyDate = dateTime,//编辑时间
                //LastModifiedBy = "",//最后修改人帐号
                ActStatus = "1"//状态
            };
            WorkTask.Insert(taskModel);
            #endregion

            for (int i = 0; i < entity["ReturnItems"].Count(); i++)
            {
                var job = entity["ReturnItems"][i];
                decimal qty = Convert.ToDecimal(job["qty"].ToString() == "" ? "0" : job["qty"].ToString());
                string containerDetailId = job["containerDetailId"].ToString();//容器明细id
                DataRow[] drs = dtContainerDInfo.Select($"t_id='{containerDetailId}'");
                if (drs.Length == 1)
                {
                    //修改容器明细中物料数量
                    Container.UpdateDetailQty(containerDetailId, qty);
                }
                else
                {
                    throw new WmsException(500, "无法获取退余料明细数据！");
                }

                #region 插入任务物料信息
                WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                    //ClientID = "",//集团ID
                    //OrgID = "",//组织ID
                    //WarehouseID = "",//仓库ID
                    TaskID = taskModel.ID,//主任务ID
                    ItemID = drs[0]["item_id"].ToString(),//物料ID
                    ItemName = drs[0]["item_name"].ToString(),//物料名称
                    PlanQty = qty,//计划数量
                    //ProcessedQty = "",//已处理数量
                    //FinishedQty = "",//完成数量
                    UomQtyID = drs[0]["uom_qty_id"].ToString(),//数量单位
                    //FinishedStatQty = "",//完成统计数量
                    UomStatID = drs[0]["uom_stat_id"].ToString(),//统计数量单位
                    ContainerDetailID = drs[0]["t_id"].ToString(),//容器明细ID
                    RefDocType = DocType.DocType_ClientReturnPallet.ToString(),//来源单据类型
                    RefDocID = productPlanOutId,//来源单ID
                    //RefDocDetailID = "",//来源单明细ID
                    RefDocNo = job_no,//来源单号
                    //StartDate = "",//开始时间
                    //EndDate = "",//结束时间
                    ProcessStatus = ProcessStatus.ProcessStatus_New.ToString(),//工作状态
                    //CreateDate = "",//创建时间
                    //CreatedBy = "",//创建人帐号 
                    //ModifyIP = "",//操作人IP 
                    //ModifyDate = "",//编辑时间
                    //LastModifiedBy = "",//最后修改人帐号
                    //ActStatus = ""//状态
                };
                WorkTaskItem.Insert(taskItemModel);
                #endregion
            }
            #endregion

            //发送给wcs
            SendToWCS(taskModel.ID);
        }
        #endregion

        #region 退坏料任务强制完成
        /// <summary>
        /// 退坏料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        public void FinishClientReturnBadPallet(FinishClientReturnBadPalletModel model)
        {
            #region 1.获取退坏料任务信息
            //获取退坏料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退坏料任务！");
            }
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ReturnBadPallet.ToString())
            {
                throw new WmsException(500, "该任务不是退坏料任务，无法完成！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "任务已完成，无需再次完成！");
            }
            #endregion
            //任务不是部分完成，则执行部分完成
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                PartionFinishReturnBadPalletModel pft = new PartionFinishReturnBadPalletModel();
                pft.TaskTid = model.TaskTid;
                PartionFinishReturnBadPallet(pft);
            }
            //执行完成任务
            FinishReturnBadPalletModel ft = new FinishReturnBadPalletModel();
            ft.TaskTid = model.TaskTid;
            FinishReturnBadPallet(ft);
            #region 旧版强完代码
            //#region 1.获取退坏料任务信息
            ////获取退坏料任务信息
            //DataTable dtTaskInfo = GeTaskInfoByTaskId(model.TaskTid);
            //if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            //{
            //    throw new WmsException(500, "未查询到站台退坏料任务！");
            //}
            //string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            //string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            //string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            //string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            //string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            //string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退坏料时托盘目的库位id
            //string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            //if (refDocType != DocType.DocType_ReturnBadPallet.ToString())
            //{
            //    throw new WmsException(500, "该任务不是退坏料任务，无法强制完成！");
            //}
            //if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString() && taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            //{
            //    throw new WmsException(500, "任务仅在处于执行中或者已部分完成状态下，才能强制完成！");
            //}
            //#endregion

            //#region 2.获取任务物料明细
            ////获取任务物料明细
            //DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
            //#endregion

            //#region 3.检查站台状态
            ////检查站台状态
            //DataRow dtDockInfo = GetDockStatus(dockId);
            //string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            //string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            //string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            //if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
            //{
            //    throw new WmsException(500, "当前站台位处于故障状态，无法使用！");
            //}
            ////if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
            ////{
            ////    throw new WmsException(500, "当前站台对应的工艺段已停用，无法进行退坏料！");
            ////}
            //#endregion

            //#region 4.获取挂在站台上的容器明细
            ////获取挂在站台上的容器明细
            //DataRow[] dtrContainerDInfo = Container.GetDetailByHeadID(container_id);
            //#endregion

            //#region 5.将容器明细物料数量为0，则软删除容器明细；修改容器明细质检状态为“不合格”
            ////容器明细中物料数量qty全为0的情况，已在前台界面限制，走退空托盘业务
            ////该目的是在统计消耗时直接使用出库数量减去容器明细中的数量，作为消耗量
            //for (int i = 0; i < dtrContainerDInfo.Length; i++)
            //{
            //    string containerDetailId = dtrContainerDInfo[i]["t_id"].ToString();
            //    decimal qty = Convert.ToDecimal(dtrContainerDInfo[i]["qty"].ToString());
            //    if (qty <= 0)//物料数量为0，则删除容器明细，设置成软删除
            //    {
            //        Container.UpdateDetailActStatus(containerDetailId, "0");
            //    }
            //    else//待入量不为0，则将容器明细待入量写入数量字段，并清空待入数量,修改容器明细质检状态为“不合格”；
            //    {
            //        Container.UpdateDetailQCStatus(containerDetailId, QcStatus.QcStatus_CheckFailed.ToString());
            //    }
            //}
            //#endregion

            //#region 6.修改容器主档是否允许呼叫
            ////修改容器主档是否允许呼叫
            //Container.UpdateHeadIsAllowCalled(container_id, "0");
            //#endregion

            //#region 7.将容器主档与库位关联
            ////将容器主档与库位关联
            //Container.UpdateContainerLocation(container_id, locId);
            //#endregion

            //#region 8.更新退坏料任务和任务明细
            ////将任务更新为完成
            //WorkTask.UpdateTaskComplete(model.TaskTid);
            ////更新任务物料明细中的已处理数量和完成数量
            //for (int i = 0; i < taskItemInfos.Length; i++)
            //{
            //    string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
            //    //根据任务物料id，修改任务物料状态为已完成，并且修改完成和处理数量
            //    WorkTask.UpdateTaskItemCompleteAndQty(cdTid);
            //}
            //WorkTask.MoveToHistory(model.TaskTid);
            //#endregion

            //#region 9.清空挂在站台上的容器
            ////清空挂在站台上的容器
            //UpdateMachineDockDeatilId(machineDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            //#endregion

            //#region 10.修改站台状态
            ////修改站台状态
            //UpdateDockStatus(machineDockTid, dockId, EquipmentStatus.EquipmentStatus_OK.ToString());
            //#endregion

            //#region 11.修改货位状态为已存储
            ////修改货位状态为已存储
            //UpdateLocStatusByLocationId(locId, LocationStatus.LocationStatus_Stored.ToString(), LocationStatus.LocationStatus_In.ToString(), "机台退坏料,物料入库完成,修改货位状态为已存储");
            //#endregion 
            #endregion
        }
        #endregion

        #region 退坏料任务回收
        /// <summary>
        /// 退坏料任务回收
        /// </summary>
        /// <param name="model"></param>
        public void RecycleClientReturnBadPallet(RecycleClientReturnBadPalletModel model)
        {
            #region 1.获取退坏料任务信息
            //获取退坏料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退坏料任务！");
            }
            string taskZoneId = dtTaskInfo.Rows[0]["zone_id"].ToString();//任务中的区域
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退坏料时托盘目的库位id
            string destAddrText = dtTaskInfo.Rows[0]["dest_addr_text"].ToString();//任务中，目的货位编号
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ReturnBadPallet.ToString())
            {
                throw new WmsException(500, "该任务不是退坏料任务，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "该任务已完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "该任务已部分完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "该任务正在执行中，无法回收任务！");
            }
            #endregion

            //修改容器主档是否允许被呼叫
            ce.UpdateContainerAllowCalled(container_id, "1");

            //二楼，第一次货位分配时需要回收任务
            if (string.Equals(taskZoneId, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))
            {
                if (destAddrText.Length <= 3)//一次货位分配
                {
                    RecycleInBoundFirstDistributionLocationTask(dtTaskInfo.Rows[0]);
                    //修改任务软删除标记为“0”
                    WorkTask.UpdateActStatus(model.TaskTid, "0");
                }
                else//二次货位分配
                {
                    RecycleInBoundSecoundDistributionLocationTask(dtTaskInfo.Rows[0]);
                    RecycleClientReturnBadPalletTaskAndContainer(model, productPlanOutId);
                }
                return;
                //throw new WmsException(500, "该任务为二楼要料货位一次分配产生的任务，目前无法回收！");
            }

            RecycleClientReturnBadPalletTaskAndContainer(model, productPlanOutId);
            #region 5.任务回收时，修改货位状态为空
            //修改货位状态为空
            ce.UpdateLocStatusByLocationId(locId, LocationStatus.LocationStatus_Empty.ToString(), LocationStatus.LocationStatus_In.ToString(), "机台退坏料任务回收,物料无需入库,修改货位状态为空");
            #endregion
        }

        #region 回收退坏料时删除任务和恢复容器明细的物料数量
        /// <summary>
        /// 回收退坏料时删除任务和恢复容器明细的物料数量
        /// </summary>
        /// <param name="model"></param>
        /// <param name="productPlanOutId"></param>
        private void RecycleClientReturnBadPalletTaskAndContainer(RecycleClientReturnBadPalletModel model, string productPlanOutId)
        {
            #region 2.获取任务物料明细
            //获取任务物料明细
            DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
            #endregion

            #region 3.获取要料或者接班任务，并恢复退坏料任务产生前容器中的物料数量
            //获取要料或者接班任务
            DataTable theNewTaskItemInfo = ce.GetTheNewTaskInfoByPlanOutId(productPlanOutId);

            //根据要料或者接班任务，恢复退坏料任务产生前容器中的物料数量
            for (int i = 0; i < theNewTaskItemInfo.Rows.Count; i++)
            {
                string cdTid = theNewTaskItemInfo.Rows[i]["container_d_id"].ToString();
                decimal qty = Convert.ToDecimal(theNewTaskItemInfo.Rows[i]["plan_qty"].ToString());
                Container.UpdateDetailQty(cdTid, qty);
            }
            #endregion

            #region 4.更新任务和任务明细软删除标记为“0”
            //修改任务软删除标记为“0”
            WorkTask.UpdateActStatus(model.TaskTid, "0");
            //更新任务物料软删除标记为“0”
            for (int i = 0; i < taskItemInfos.Length; i++)
            {
                string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
                string containerDTid = taskItemInfos[i]["container_d_id"].ToString();//容器明细id
                //修改任务物料表中的软删除标记为“0”
                WorkTask.UpdateTaskItemActStatus(cdTid, "0");
            }
            #endregion
        }
        #endregion

        #endregion

        #region 退坏料任务部分完成
        /// <summary>
        /// 退坏料任务部分完成
        /// </summary>
        /// <param name="model"></param>
        public void PartionFinishReturnBadPallet(PartionFinishReturnBadPalletModel model)
        {
            #region 1.获取退坏料任务信息
            //获取退坏料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退坏料任务！");
            }
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退坏料时托盘目的库位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ReturnBadPallet.ToString())
            {
                throw new WmsException(500, "该任务不是退坏料任务，无法强制完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "任务仅在处于执行中状态下，才能部分完成！");
            }
            #endregion

            #region 7.将任务修改为部分完成
            WorkTask.UpdateTaskStatus(model.TaskTid, TaskStatus.TaskStatus_PartialFinished.ToString(), TaskStatus.TaskStatus_Executing.ToString());
            #endregion

            #region 2.检查站台状态
            //检查站台状态
            DataRow dtDockInfo = ce.GetDockStatus(dockId);
            string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            //if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
            //{
            //    throw new WmsException(500, "当前站台位处于故障状态，无法使用！");
            //}
            //if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
            //{
            //    throw new WmsException(500, "当前站台对应的工艺段已停用，无法进行退坏料！");
            //}
            #endregion

            #region 3.获取挂在站台上的容器明细
            //获取挂在站台上的容器明细
            DataRow[] dtrContainerDInfo = Container.GetDetailByHeadID(container_id);
            #endregion

            #region 4.将容器明细物料数量为0，则软删除容器明细；修改容器明细质检状态为“不合格”
            //容器明细中物料数量qty全为0的情况，已在前台界面限制，走退空托盘业务
            //该目的是在统计消耗时直接使用出库数量减去容器明细中的数量，作为消耗量
            for (int i = 0; i < dtrContainerDInfo.Length; i++)
            {
                string containerDetailId = dtrContainerDInfo[i]["t_id"].ToString();
                decimal qty = Convert.ToDecimal(dtrContainerDInfo[i]["qty"].ToString());
                if (qty <= 0)//物料数量为0，则删除容器明细，设置成软删除
                {
                    Container.UpdateDetailActStatus(containerDetailId, "0");
                }
                else//待入量不为0，则将容器明细待入量写入数量字段，并清空待入数量,修改容器明细质检状态为“不合格”；
                {
                    Container.UpdateDetailQCStatus(containerDetailId, QcStatus.QcStatus_CheckFailed.ToString());
                }
            }
            #endregion

            #region 5.修改容器主档是否允许呼叫
            //修改容器主档是否允许呼叫
            Container.UpdateHeadIsAllowCalled(container_id, "0");
            #endregion

            //容器关联货位
            Container.UpdateContainerLocation(container_id, locId);

            //恢复删除的容器主档
            Container.UpdateHeadActStatus(container_id, "1");

            #region 6.清空挂在站台上的容器
            //清空挂在站台上的容器
            ce.UpdateMachineDockDeatilId(machineDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            #endregion

            #region 7.修改站台状态
            //修改站台状态
            ce.UpdateDockStatus(machineDockTid, dockId, EquipmentStatus.EquipmentStatus_OK.ToString());
            #endregion
        }
        #endregion

        #region 退坏料任务完成
        /// <summary>
        /// 退坏料任务完成
        /// </summary>
        /// <param name="model"></param>
        public void FinishReturnBadPallet(FinishReturnBadPalletModel model)
        {
            #region 1.获取退坏料任务信息
            //获取退坏料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退坏料任务！");
            }
            string zone_id = dtTaskInfo.Rows[0]["zone_id"].ToString();//区域id
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退坏料时托盘目的库位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ReturnBadPallet.ToString())
            {
                throw new WmsException(500, "该任务不是退坏料任务，无法强制完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "任务仅在处于已部分完成状态下，才能完成！");
            }
            #endregion

            #region 2.获取任务物料明细
            //获取任务物料明细
            DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
            #endregion

            #region 3.检查站台状态
            //检查站台状态
            DataRow dtDockInfo = ce.GetDockStatus(dockId);
            string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
            {
                throw new WmsException(500, "当前站台位处于故障状态，无法使用！");
            }
            //if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
            //{
            //    throw new WmsException(500, "当前站台对应的工艺段已停用，无法进行退坏料！");
            //}
            #endregion

            #region 4.将容器主档与库位关联
            //将容器主档与库位关联
            Container.UpdateContainerLocation(container_id, locId);
            Container.UpdateContainerIsFull(container_id, "0");
            #endregion

            #region 5.更新退坏料任务和任务明细
            //将任务更新为完成
            WorkTask.UpdateTaskComplete(model.TaskTid);
            //更新任务物料明细中的已处理数量和完成数量
            for (int i = 0; i < taskItemInfos.Length; i++)
            {
                string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
                //根据任务物料id，修改任务物料状态为已完成，并且修改完成和处理数量
                WorkTask.UpdateTaskItemCompleteAndQty(cdTid);
            }
            WorkTask.MoveToHistory(model.TaskTid);
            #endregion

            #region 6.修改货位状态为已存储
            //修改货位状态为已存储
            ce.UpdateLocStatusByLocationId(locId, LocationStatus.LocationStatus_Stored.ToString(), LocationStatus.LocationStatus_In.ToString(), "机台退坏料,物料入库完成,修改货位状态为已存储");
            #endregion
            //if (string.Equals(zone_id, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))
            //{
            //Location lo = new Location();
            //LocationModel _locInfo = lo.GetLocationById(locId);
            //bool isLineFull = ce.IsLineFull(_locInfo.location_row.ToString());
            //if (isLineFull == true)
            //{
            //    lo.UpdateLocationStatus(_locInfo.t_id, LocationStatus.LocationStatus_Stored);
            //    lo.UpdateLocationFlag(_locInfo.location_row.ToString(), LocationFlag.LocationFlag_Usable.ToString(), LocationFlag.LocationFlag_InOnly.ToString());
            //}
            //}
        }
        #endregion

        #region 退废料实际实现的是退余料至指定地点
        #region 退废料
        /// <summary>
        /// 退废料
        /// </summary>
        public void ReturnWasteMaterials(ReturnWasteMaterialsModel model, out WebJsonInfo webinfo)
        {
            #region 1.获取生产工单计划信息
            DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(model.ProductPlanOutId);
            if (dtProductPlanInfo == null || dtProductPlanInfo.Length < 1)
            {
                throw new WmsException(500, "未查询到该机台对应的生产工单！");
            }
            string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态
            string job_no = dtProductPlanInfo[0]["job_no"].ToString();//生产工单单号
            string product_item_id = dtProductPlanInfo[0]["item_id"].ToString();//成品物料id
            string product_item_code = dtProductPlanInfo[0]["item_code"].ToString();//成品编码
            string product_item_name = dtProductPlanInfo[0]["item_name"].ToString();//成品名称
            #endregion

            #region 2.检查站台状态
            //查询站台状态
            DataRow dtDockInfo = ce.GetEquipmentInfo(model.Dock);
            string equipment_code = dtDockInfo["equipment_code"].ToString();//站台编码
            string zoneId = dtDockInfo["zone_id"].ToString();//站台所在的区域
            #endregion

            #region 3.获取废料存放的指定位置
            //获取废料存放的指定位置
            DataTable WasteDestAddrInfo = ce.GetReturnNullPalletDestAddrInfo(zoneId, EquipmentUsage.EquipmentUsage_ReturnWasteMaterials.ToString());
            if (WasteDestAddrInfo == null || WasteDestAddrInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "退废料时未查询到目的站台信息，请创建目的站台信息后在进行退废料！");
            }
            //if (WasteDestAddrInfo.Rows.Count > 1)
            //{
            //    throw new WmsException(500, "退废料时查询到多个目的站台信息，目的站台只能有一个！");
            //}
            int _num = WasteDestAddrInfo.Rows.Count;
            Random random = new Random();
            int _index = random.Next(0, _num - 1);
            string destAddrID = WasteDestAddrInfo.Rows[_index]["t_id"].ToString();//退废料时的目的输送线站台id
            string destAddrText = WasteDestAddrInfo.Rows[_index]["equipment_code"].ToString();//退废料时的目的输送线站台编码
            #endregion

            //获取AGV
            DataRow drAGV = ce.GetAgv();

            #region 4.产生废料移库任务，仅需要插入任务主档
            Model.WorkTaskModel returnNullPalletTaskModel = new Model.WorkTaskModel()
            {
                ID = SnowflakeID.Guid().ToString(),//主键ID
                ZoneID = zoneId,//区域ID
                TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                //DependentTaskNo = "",//依赖任务号
                TaskType = TaskType.TaskType_Move.ToString(),//任务类型编码"退废料"
                ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                //ExecutedBy = "",//任务执行人
                EquipmentID = drAGV["t_id"].ToString(),//执行设备ID
                //EquipmentType = "",//执行设备类型
                //ActualEquipmentID = "",//实际执行设备ID
                SeqNo = 1,//顺序号
                Priority = 1,//优先级
                ContainerID = string.Empty,//容器ID
                ContainerBarcode = string.Empty,//容器条码
                RefDocType = DocType.DocType_ClientReturnWaste.ToString(),//来源单据类型
                RefDocID = model.ProductPlanOutId,//来源单ID
                //RefDocDetailID = "",//来源单明细ID
                RefDocNo = job_no,//来源单号
                SrcAddrID = model.Dock,//来源地址ID
                SrcAddrType = AddressType.AddressType_MachineStation.ToString(),//来源地址类型
                SrcAddrText = equipment_code,//来源地址
                DestAddrID = destAddrID,//目标地址ID             
                DestAddrType = AddressType.AddressType_Station.ToString(),//目标地址类型            
                DestAddrText = destAddrText,//目标地址         
                //ActualDestAddrID = "",//实际目标地址ID
                //ActualDestAddrText = "",//实际目标地址
                TaskStatus = TaskStatus.TaskStatus_Executing.ToString(),//任务状态
                //StartDate = dateTime,//开始时间
                //EndDate = dateTime,//结束时间
                IsExecutable = "1",//任务是否可执行
                //PriorityChangeTime = dateTime,//优先级修改时间        
                LedText = "",//LED显示内容
                ExistLgv = "1",//是否存在LGV
                //CreateDate = dateTime,//创建时间
                //CreatedBy = "",//创建人帐号
                //ModifyIP = "",//操作人IP 
                //ModifyDate = dateTime,//编辑时间
                //LastModifiedBy = "",//最后修改人帐号
                ActStatus = "1"//状态
            };
            WorkTask.Insert(returnNullPalletTaskModel);
            #endregion

            //发送给wcs
            webinfo = SendToWCS(returnNullPalletTaskModel.ID);
        }
        #endregion

        #region 退废料任务强制完成
        /// <summary>
        /// 退废料任务强制完成
        /// </summary>
        /// <param name="model"></param>
        public void FinishReturnWasteMaterials(FinishReturnWasteMaterialsModel model)
        {
            #region 1.获取退废料任务信息
            //获取退废料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退废料任务！");
            }
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退废料时托盘目的库位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ClientReturnWaste.ToString())
            {
                throw new WmsException(500, "该任务不是退废料任务，无法强制完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString() && taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "任务仅在处于执行中或者已部分完成状态下，才能强制完成！");
            }
            #endregion

            //if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString() && taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            //{
            //    throw new WmsException(500, "任务仅在处于执行中或者已部分完成状态下，才能强制完成！");
            //}
            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "任务已完成，无需再次完成！");
            }
            //任务不是部分完成，则执行部分完成
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                PartionFinishReturnWasteMaterialsModel pft = new PartionFinishReturnWasteMaterialsModel();
                pft.TaskTid = model.TaskTid;
                PartionFinishReturnWasteMaterials(pft);
            }
            //执行完成任务
            FinishWasteMaterialsModel ft = new FinishWasteMaterialsModel();
            ft.TaskTid = model.TaskTid;
            FinishWasteMaterialsTask(ft);
        }
        #endregion

        #region 退废料任务回收
        /// <summary>
        /// 退废料任务回收
        /// </summary>
        /// <param name="model"></param>
        public void RecycleReturnWasteMaterials(RecycleReturnWasteMaterialsModel model)
        {
            #region 1.获取退废料任务信息
            //获取退废料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退废料任务！");
            }
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退废料时托盘目的库位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ClientReturnWaste.ToString())
            {
                throw new WmsException(500, "该任务不是退废料任务，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "该任务已完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "该任务已部分完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "该任务正在执行中，无法回收任务！");
            }
            #endregion

            //修改任务软删除标记为“0”
            WorkTask.UpdateActStatus(model.TaskTid, "0");
        }
        #endregion

        #region 退废料任务完成
        /// <summary>
        /// 退废料任务完成
        /// </summary>
        /// <param name="model"></param>
        public void FinishWasteMaterialsTask(FinishWasteMaterialsModel model)
        {
            #region 1.获取退废料任务信息
            //获取退废料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退废料任务！");
            }
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退废料时托盘目的库位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ClientReturnWaste.ToString())
            {
                throw new WmsException(500, "该任务不是退废料任务，无法强制完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString() && taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "任务仅在处于执行中或者已部分完成状态下，才能强制完成！");
            }
            #endregion

            //将任务更新为完成
            WorkTask.UpdateTaskComplete(model.TaskTid);
            //将任务插入历史表
            WorkTask.InsertHistory(model.TaskTid);
            //删除任务
            WorkTask.DeleteWorkTask(model.TaskTid);
        }
        #endregion

        #region 退废料任务部分完成
        /// <summary>
        /// 退废料任务部分完成
        /// </summary>
        /// <param name="model"></param>
        public void PartionFinishReturnWasteMaterials(PartionFinishReturnWasteMaterialsModel model)
        {
            #region 1.获取退废料任务信息
            //获取退废料任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台退废料任务！");
            }
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退废料时托盘目的库位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ClientReturnWaste.ToString())
            {
                throw new WmsException(500, "该任务不是退废料任务，无法强制完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString() && taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "任务仅在处于执行中或者已部分完成状态下，才能强制完成！");
            }
            #endregion

            #region 7.将任务修改为部分完成
            WorkTask.UpdateTaskStatus(model.TaskTid, TaskStatus.TaskStatus_PartialFinished.ToString(), TaskStatus.TaskStatus_Executing.ToString());
            #endregion

            #region 3.检查站台状态
            //检查站台状态
            DataRow dtDockInfo = ce.GetDockStatus(dockId);
            string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
            string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            #endregion

            #region 6.获取站台上的托盘信息
            DataTable _DtContainerId = ce.GetContainerInfoByMachineDockId(machineDockTid);
            #endregion
            if (_DtContainerId == null || _DtContainerId.Rows.Count != 1)
            {
                throw new WmsException(500, "获取站台上的托盘信息失败！");
            }
            string _tempContainerId = _DtContainerId.Rows[0]["container_id"].ToString();
            ////软删除的容器主档为0
            //Container.UpdateHeadActStatus(_tempContainerId, "0");
            #region 6.修改站台状态
            //修改站台状态
            ce.UpdateDockStatus(machineDockTid, dockId, EquipmentStatus.EquipmentStatus_OK.ToString());
            #endregion

            #region 8.清空挂在站台上的容器
            //清空挂在站台上的容器
            ce.UpdateMachineDockDeatilId(machineDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            #endregion
        }
        #endregion 
        #endregion

        #region 成品转移
        /// <summary>
        /// 成品转移
        /// </summary>
        public void ProductTransfer(ProductTransferModel model, out WebJsonInfo webinfo)
        {
            #region 1.获取生产工单计划信息
            DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(model.ProductPlanOutId);
            if (dtProductPlanInfo == null || dtProductPlanInfo.Length < 1)
            {
                throw new WmsException(500, "未查询到该机台对应的生产工单！");
            }
            string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态
            string job_no = dtProductPlanInfo[0]["job_no"].ToString();//生产工单单号
            string product_item_id = dtProductPlanInfo[0]["item_id"].ToString();//成品物料id
            string product_item_code = dtProductPlanInfo[0]["item_code"].ToString();//成品编码
            string product_item_name = dtProductPlanInfo[0]["item_name"].ToString();//成品名称
            #endregion

            #region 2.检查站台状态
            //查询站台状态
            DataRow dtDockInfo = ce.GetEquipmentInfo(model.Dock);
            string equipment_code = dtDockInfo["equipment_code"].ToString();//站台编码
            string zoneId = dtDockInfo["zone_id"].ToString();//站台所在的区域
            string dock_status = dtDockInfo["equipment_status"].ToString();//使用设备表中的站台状态
            #endregion

            if (dock_status == EquipmentStatus.EquipmentStatus_Using.ToString())
            {
                throw new WmsException(500, "当前站台位已被使用！");
            }

            #region 3.获取成品转移的指定位置
            //获取成品转移的指定位置
            DataTable WasteDestAddrInfo = ce.GetProductTransferDestAddrInfo(zoneId, EquipmentUsage.EquipmentUsage_Prouduct.ToString());
            if (WasteDestAddrInfo == null || WasteDestAddrInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "成品转移时未查询指定位置，请创建指定位置信息后在进行成品转移！");
            }
            int _num = WasteDestAddrInfo.Rows.Count;
            Random random = new Random();
            int _index = random.Next(0, _num - 1);
            string destAddrID = WasteDestAddrInfo.Rows[_index]["t_id"].ToString();//成品转移时的目的输送线站台id
            string destAddrText = WasteDestAddrInfo.Rows[_index]["equipment_code"].ToString();//成品转移时的目的输送线站台编码
            #endregion

            ce.UpdateDockStatus("", model.Dock, EquipmentStatus.EquipmentStatus_Using.ToString());

            //获取AGV
            DataRow drAGV = ce.GetAgv();

            #region 4.产生成品转移任务，仅需要插入任务主档
            Model.WorkTaskModel productTransferTaskModel = new Model.WorkTaskModel()
            {
                ID = SnowflakeID.Guid().ToString(),//主键ID
                ZoneID = zoneId,//区域ID
                TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                //DependentTaskNo = "",//依赖任务号
                TaskType = TaskType.TaskType_Move.ToString(),//任务类型编码"成品转移"
                ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                //ExecutedBy = "",//任务执行人
                EquipmentID = drAGV["t_id"].ToString(),//执行设备ID
                //EquipmentType = "",//执行设备类型
                //ActualEquipmentID = "",//实际执行设备ID
                SeqNo = 1,//顺序号
                Priority = 1,//优先级
                ContainerID = string.Empty,//容器ID
                ContainerBarcode = string.Empty,//容器条码
                RefDocType = DocType.DocType_ProductTransfer.ToString(),//来源单据类型
                RefDocID = model.ProductPlanOutId,//来源单ID
                RefDocDetailID = model.Planno,//来源单明细ID,此处存放mes下发的号码
                RefDocNo = job_no,//来源单号
                SrcAddrID = model.Dock,//来源地址ID
                SrcAddrType = AddressType.AddressType_MachineStation.ToString(),//来源地址类型
                SrcAddrText = equipment_code,//来源地址
                DestAddrID = destAddrID,//目标地址ID             
                DestAddrType = AddressType.AddressType_Station.ToString(),//目标地址类型            
                DestAddrText = destAddrText,//目标地址         
                //ActualDestAddrID = "",//实际目标地址ID
                //ActualDestAddrText = "",//实际目标地址
                TaskStatus = TaskStatus.TaskStatus_Executing.ToString(),//任务状态
                //StartDate = dateTime,//开始时间
                //EndDate = dateTime,//结束时间
                IsExecutable = "1",//任务是否可执行
                //PriorityChangeTime = dateTime,//优先级修改时间        
                LedText = "",//LED显示内容
                ExistLgv = "1",//是否存在LGV
                //CreateDate = dateTime,//创建时间
                //CreatedBy = "",//创建人帐号
                //ModifyIP = "",//操作人IP 
                //ModifyDate = dateTime,//编辑时间
                //LastModifiedBy = "",//最后修改人帐号
                ActStatus = "1"//状态
            };
            WorkTask.Insert(productTransferTaskModel);
            #endregion

            //发送给wcs
            webinfo = SendToWCS(productTransferTaskModel.ID);
        }
        #endregion

        #region 成品转移任务完成
        /// <summary>
        /// 成品转移任务完成
        /// </summary>
        /// <param name="model"></param>
        public void FinishTransfer(FinishTransferModel model)
        {
            #region 1.获取成品转移任务信息
            //获取成品转移任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台成品转移任务！");
            }
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//成品转移时托盘目的库位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ProductTransfer.ToString())
            {
                throw new WmsException(500, "该任务不是成品转移任务，无法完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "任务仅在处于已部分完成状态下，才能完成！");
            }
            #endregion

            //将任务更新为完成
            WorkTask.UpdateTaskComplete(model.TaskTid);
            //将任务插入历史表
            WorkTask.InsertHistory(model.TaskTid);
            //删除任务
            WorkTask.DeleteWorkTask(model.TaskTid);
        }
        #endregion

        #region 成品转移任务部分完成
        /// <summary>
        /// 成品转移任务部分完成
        /// </summary>
        /// <param name="model"></param>
        public void PartionFinishProductTransferTask(PartionFinishProductTransferTaskModel model)
        {
            #region 1.获取成品转移任务信息
            //获取成品转移任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到站台成品转移任务！");
            }
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//成品转移时托盘目的库位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ProductTransfer.ToString())
            {
                throw new WmsException(500, "该任务不是退废料任务，无法强制完成！");
            }
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString() && taskStatus != TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "任务仅在处于执行中或者已部分完成状态下，才能强制完成！");
            }
            #endregion
            ce.UpdateDockStatus("", dockId, EquipmentStatus.EquipmentStatus_OK.ToString());
            WorkTask.UpdateTaskStatus(model.TaskTid, TaskStatus.TaskStatus_PartialFinished.ToString(), TaskStatus.TaskStatus_Executing.ToString());
        }
        #endregion

        #region 成品转移任务强制完成
        /// <summary>
        /// 成品转移任务强制完成
        /// </summary>
        /// <param name="model"></param>
        public void FinishProductTransferTask(FinishProductTransferTaskModel model)
        {
            #region 1.获取成品转移任务信息
            //获取成品转移任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到成品转移任务！");
            }
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            //string loc_id = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//任务中，源地址id，此处指的是要料的货位id
            string destDockid = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//任务中，目的站台id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//任务中，生产工单id
            string dependent_task_no = dtTaskInfo.Rows[0]["dependent_task_no"].ToString();//依赖任务号
            if (refDocType != DocType.DocType_ProductTransfer.ToString())
            {
                throw new WmsException(500, "该任务不是成品转移任务，无法强制完成！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "任务已完成，无需再次完成！");
            }
            #endregion

            //任务不是部分完成，则执行部分完成
            if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                PartionFinishProductTransferTaskModel pft = new PartionFinishProductTransferTaskModel();
                pft.TaskTid = model.TaskTid;
                PartionFinishProductTransferTask(pft);
            }
            //执行完成任务
            FinishTransferModel ft = new FinishTransferModel();
            ft.TaskTid = model.TaskTid;
            FinishTransfer(ft);
        }
        #endregion

        #region 成品转移任务回收
        /// <summary>
        /// 成品转移任务回收
        /// </summary>
        /// <param name="model"></param>
        public void RecycleProductTransferTask(RecycleProductTransferTaskModel model)
        {
            #region 1.获取成品转移任务信息
            //获取成品转移任务信息
            DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
            if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
            {
                throw new WmsException(500, "未查询到成品转移任务！");
            }
            string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
            string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
            string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
            string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
            string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
            string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//成品转移时托盘目的库位id
            string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
            if (refDocType != DocType.DocType_ProductTransfer.ToString())
            {
                throw new WmsException(500, "该任务不是成品转移任务，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
            {
                throw new WmsException(500, "该任务已完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_PartialFinished.ToString())
            {
                throw new WmsException(500, "该任务已部分完成，无法回收任务！");
            }
            if (taskStatus == TaskStatus.TaskStatus_Executing.ToString())
            {
                throw new WmsException(500, "该任务正在执行中，无法回收任务！");
            }
            #endregion
            ce.UpdateDockStatus("", dockId, EquipmentStatus.EquipmentStatus_OK.ToString());
            //修改任务软删除标记为“0”
            WorkTask.UpdateActStatus(model.TaskTid, "0");
        }
        #endregion

        #region 批量退料
        /// <summary>
        /// 批量退料
        /// </summary>
        /// <param name="model"></param>
        public void ChangeCard(ChangeCardModel model)
        {
            #region 1.将余料json信息转化成json对象
            //余料数据信息
            JObject entity = JObject.Parse(model.Details);
            if (entity["ReturnItems"].Count() == 0)
            {
                throw new WmsException(500, "无法获取余料明细数据！");
            }
            #endregion

            #region 2.构建退余料入参，调用退余料方法，产生任务
            //退余料入参
            ClientReturnPalletModel crm = new ClientReturnPalletModel();
            crm.Machine = model.Machine;
            crm.HasAGV = model.HasAGV;
            crm.ProductPlanOutId = model.ProductPlanOutId;
            crm.IsNeedCallPallet = "0";
            crm.ShiftId = model.ShiftId;
            crm.TeamId = model.TeamId;
            crm.IsCanBeCallOut = model.IsCanBeCallOut;
            List<string> dockIdList = new List<string>();//保存站台id
            for (int i = 0; i < entity["ReturnItems"].Count(); i++)
            {
                var job = entity["ReturnItems"][i];
                string dockId = job["dock_id"].ToString();
                if (!dockIdList.Contains(dockId))
                {
                    dockIdList.Add(dockId);
                }
            }
            //拼接余料明细
            for (int k = 0; k < dockIdList.Count; k++)
            {
                JArray jaList = new JArray();
                for (int i = 0; i < entity["ReturnItems"].Count(); i++)
                {
                    JToken job = entity["ReturnItems"][i];
                    string dockId = job["dock_id"].ToString();
                    if (dockIdList[k] != dockId)
                    {
                        continue;
                    }
                    jaList.Add(job);
                }
                crm.Details = "{ReturnItems:" + jaList.ToString() + "}";//余料明细（json格式）
                crm.Dock = dockIdList[k];
                //ReturnPalletWhenChangeCard(crm);
                ClientReturnPallet(crm);
            }
            #endregion

            #region 3.获取交接班站台
            ////获取交接班站台
            //DataRow drShiftDockInfo = ce.GetShiftDockByMachine(model.Machine);
            //string machineDockTid = drShiftDockInfo["t_id"].ToString();//交接班站台在机台站台表中的t_id 
            #endregion

            #region 4.清空挂在交接班站台上的容器
            ////清空挂在站台上的容器
            //ce.UpdateMachineDockDeatilId(machineDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            #endregion
        }

        #region 换牌退余料
        /// <summary>
        /// 换牌退余料
        /// </summary>
        /// <param name="model"></param>
        public void ReturnPalletWhenChangeCard(ClientReturnPalletModel model)
        {
            #region 1.检查站台当前任务 
            //查询站台未执行完成的任务数量
            DataRow dtTaskNumInfo = ce.SelectTaskNumByDock(model.Dock);
            int cnt = Convert.ToInt32(dtTaskNumInfo["cnt"].ToString());//站台未执行完成的任务数量
            if (cnt > 0)
            {
                throw new WmsException(500, "该站台上有未执行完成的任务，无法退余料");
            }
            #endregion

            #region 2.检查站台状态
            //查询站台状态
            DataRow dtDockInfo = ce.GetDockStatus(model.Dock);
            string machineDockTid = dtDockInfo["t_id"].ToString();
            string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
            string doc_id = dtDockInfo["doc_id"].ToString();//来源单号，此处存放的是要料时的生产工单
            string equipment_code = dtDockInfo["equipment_code"].ToString();
            string container_id = dtDockInfo["container_id"].ToString();//容器主档id
            string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
            if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
            {
                throw new WmsException(500, "当前站台位处于故障状态，无法使用！");
            }
            //if (dock_status != EquipmentStatus.EquipmentStatus_Using.ToString())
            //{
            //    throw new WmsException(500, "当前站台位未使用！");
            //}
            if (string.IsNullOrEmpty(container_id))
            {
                throw new WmsException(500, "当前站台位无托盘信息！");
            }
            //if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
            //{
            //    throw new WmsException(500, "当前站台对应的工艺段已停用，无法进行退余料！");
            //}
            #endregion

            #region 3.获取货位信息
            //通过货位分配策略，获取可用货位
            string loc_id = ce.GetEmptyLocation();//"1285384218838605824";
            #endregion

            #region 4.查询库位信息
            //查询货位对应区域信息和容器信息
            DataTable dtLocationInfo = ce.GetLocationInfo(loc_id);

            if (dtLocationInfo.Rows.Count > 1)
            {
                throw new WmsException(500, "退余料时分配的货位属于多个区域");
            }

            //区域
            string zone_id = dtLocationInfo.Rows[0]["zone_id"].ToString();
            //货位对应的编码
            string loc_no = dtLocationInfo.Rows[0]["location_code"].ToString();
            //容器主档id
            string locContainer_htid = dtLocationInfo.Rows[0]["t_id"].ToString();
            //货位状态
            string location_status = dtLocationInfo.Rows[0]["location_status"].ToString();
            //判断货位分配策略返回的货位上是否已经与容器主档关联，如果有关联，则说明货位上有物料
            if (!string.IsNullOrEmpty(locContainer_htid))
            {
                throw new WmsException(500, "退余料时分配的货位上已有物料，退料失败！");
            }
            if (location_status != LocationStatus.LocationStatus_Empty.ToString())
            {
                throw new WmsException(500, "货位分配策略分配的货位，状态不为空，无法进行退余料");
            }
            #endregion

            #region 5.将退余料json信息转化成json对象
            //退余料数据信息
            JObject entity = JObject.Parse(model.Details);
            if (entity["ReturnItems"].Count() == 0)
            {
                throw new WmsException(500, "无法获取退余料明细数据！");
            }
            #endregion

            #region 6.获取容器主档信息，容器明细信息
            //获取挂在当前站台上容器id
            DataTable containerHInfo = ce.GetContainerInfoByDockIdAndMachineId(model.Dock, model.Machine);
            if (containerHInfo.Rows.Count > 1)
            {
                throw new WmsException(500, "根据机台和站台，获取挂在站台上面的容器主档不唯一");
            }
            string container_htid = containerHInfo.Rows[0]["container_id"].ToString();
            //根据容器主档id，获取容器主档信息
            DataRow dtrContainerInfo = Container.GetContainerH(container_htid);
            //查询容器明细
            DataTable dtContainerDInfo = ce.ToDataTable(Container.GetDetailByHeadID(container_htid));
            #endregion

            #region 7.获取要料时的生产工单计划信息
            DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(doc_id);
            if (dtProductPlanInfo == null || dtProductPlanInfo.Length < 1)
            {
                throw new WmsException(500, "未查询到该机台对应的生产工单！");
            }
            string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态
            string job_no = dtProductPlanInfo[0]["job_no"].ToString();
            string product_item_id = dtProductPlanInfo[0]["item_id"].ToString();//成品物料id
            string product_item_code = dtProductPlanInfo[0]["item_code"].ToString();//成品编码
            string product_item_name = dtProductPlanInfo[0]["item_name"].ToString();//成品名称
            #endregion

            #region 8.创建入库任务

            #region 插入任务主档
            Model.WorkTaskModel taskModel = new Model.WorkTaskModel()
            {
                ID = SnowflakeID.Guid().ToString(),//主键ID
                ZoneID = zone_id,//区域ID
                TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                //DependentTaskNo = "",//依赖任务号
                TaskType = TaskType.TaskType_InBound.ToString(),//任务类型编码"机台退余料"
                ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                //ExecutedBy = "",//任务执行人
                //EquipmentID = "",//执行设备ID
                //EquipmentType = "",//执行设备类型
                //ActualEquipmentID = "",//实际执行设备ID
                SeqNo = 1,//顺序号
                Priority = 1,//优先级
                ContainerID = container_htid,//容器ID
                ContainerBarcode = dtrContainerInfo["container_barcode"].ToString(),//容器条码
                RefDocType = DocType.DocType_ClientReturnPallet.ToString(),//来源单据类型  "机台退余料"
                RefDocID = doc_id,//来源单ID
                RefDocDetailID = model.Planno,//来源单明细ID,存放mes叫料号
                RefDocNo = job_no,//来源单号
                SrcAddrID = model.Dock,//来源地址ID
                SrcAddrType = AddressType.AddressType_MachineStation.ToString(),//来源地址类型"dock"
                SrcAddrText = equipment_code,//来源地址
                DestAddrID = loc_id,//目标地址ID
                DestAddrType = AddressType.AddressType_Location.ToString(),//目标地址类型"location"
                DestAddrText = loc_no,//目标地址
                //ActualDestAddrID = "",//实际目标地址ID
                //ActualDestAddrText = "",//实际目标地址
                TaskStatus = TaskStatus.TaskStatus_WaitingExecute.ToString(),//任务状态
                //StartDate = dateTime,//开始时间
                //EndDate = dateTime,//结束时间
                IsExecutable = "1",//任务是否可执行
                //PriorityChangeTime = dateTime,//优先级修改时间        
                LedText = "",//LED显示内容
                ExistLgv = model.HasAGV,//是否存在LGV
                //CreateDate = dateTime,//创建时间
                //CreatedBy = "",//创建人帐号
                //ModifyIP = "",//操作人IP 
                //ModifyDate = dateTime,//编辑时间
                //LastModifiedBy = "",//最后修改人帐号
                ActStatus = "1"//状态
            };
            WorkTask.Insert(taskModel);
            #endregion

            for (int i = 0; i < entity["ReturnItems"].Count(); i++)
            {
                var job = entity["ReturnItems"][i];
                decimal qty = Convert.ToDecimal(job["qty"]);
                string containerDetailId = job["containerDetailId"].ToString();//容器明细id
                DataRow[] drs = dtContainerDInfo.Select($"t_id='{containerDetailId}'");
                if (drs.Length == 1)
                {
                    //修改容器明细中物料数量
                    Container.UpdateDetailQty(containerDetailId, qty);
                }
                else
                {
                    throw new WmsException(500, "无法获取退余料明细数据！");
                }

                #region 插入任务物料信息
                WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                    //ClientID = "",//集团ID
                    //OrgID = "",//组织ID
                    //WarehouseID = "",//仓库ID
                    TaskID = taskModel.ID,//主任务ID
                    ItemID = drs[0]["item_id"].ToString(),//物料ID
                    ItemName = drs[0]["item_name"].ToString(),//物料名称
                    PlanQty = qty,//计划数量
                    //ProcessedQty = "",//已处理数量
                    //FinishedQty = "",//完成数量
                    UomQtyID = drs[0]["uom_qty_id"].ToString(),//数量单位
                    //FinishedStatQty = "",//完成统计数量
                    UomStatID = drs[0]["uom_stat_id"].ToString(),//统计数量单位
                    ContainerDetailID = drs[0]["t_id"].ToString(),//容器明细ID
                    RefDocType = DocType.DocType_ClientReturnPallet.ToString(),//来源单据类型
                    RefDocID = doc_id,//来源单ID
                    //RefDocDetailID = "",//来源单明细ID
                    RefDocNo = job_no,//来源单号
                    //StartDate = "",//开始时间
                    //EndDate = "",//结束时间
                    ProcessStatus = ProcessStatus.ProcessStatus_New.ToString(),//工作状态
                    //CreateDate = "",//创建时间
                    //CreatedBy = "",//创建人帐号 
                    //ModifyIP = "",//操作人IP 
                    //ModifyDate = "",//编辑时间
                    //LastModifiedBy = "",//最后修改人帐号
                    //ActStatus = ""//状态
                };
                WorkTaskItem.Insert(taskItemModel);
                #endregion
            }
            #endregion

            #region 9.获取交接班和人工领料站台
            ////获取交接班站台
            //DataRow drShiftDockInfo = ce.GetShiftDockByMachine(model.Machine);
            //string jiaojiebanDockTid = drShiftDockInfo["t_id"].ToString();//交接班站台在机台站台表中的t_id 
            #endregion

            #region 10.清空挂在交接班和人工领料站台上的容器
            ////清空挂在交接班站台上的容器
            //ce.UpdateMachineDockDeatilId(jiaojiebanDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
            ////清空挂在人工领料站台上的容器
            //UpdateMachineDockDeatilId(jiebanDockTid, string.Empty, string.Empty);
            #endregion

            #region 11.修改货位状态为待入库
            //修改货位状态为待入库
            ce.UpdateLocStatusByLocationId(loc_id, LocationStatus.LocationStatus_In.ToString(), location_status, "机台退余料,物料入库,修改货位状态为待入库");
            #endregion

        }
        #endregion
        #endregion

        #region 交班
        /// <summary>
        /// 交班
        /// </summary>
        /// <param name="model"></param>
        public void ReturnMaterielToNext(ReturnMaterielToNextModel model)
        {
            #region 1.检查机台上除交接班站台以外的所有站台上是否存在未执行完的任务
            //获取机台上除交接班站台以外的所有站台
            DataTable dtDockInfo = ce.GetAllDockByMachineAndIsChangePersonReceive(model.Machine, "1");//获取除了交接班站台以外的站台信息，目的是判断其他站台上是否存在任务
            for (int i = 0; i < dtDockInfo.Rows.Count; i++)
            {
                string dockId = dtDockInfo.Rows[i]["dock_id"].ToString();//站台id
                //查询站台未执行完成的任务数量
                DataRow dtTaskNumInfo = ce.SelectTaskNumByDock(dockId);
                int cnt = Convert.ToInt32(dtTaskNumInfo["cnt"].ToString());//站台未执行完成的任务数量
                if (cnt > 0)
                {
                    throw new WmsException(500, "机台上有未执行完成的任务，无法进行交班");
                }
            }
            if (string.Equals(model.IsChangePersonReceive, "0"))
            {
                dtDockInfo = ce.GetAllDockByMachineAndIsChangePersonReceive(model.Machine, model.IsChangePersonReceive);//获取除了交接班和人工领料站台以外的站台信息
            }
            #endregion

            #region 2.获取交接班站台
            //获取交接班站台
            DataRow drShiftDockInfo = ce.GetShiftDockByMachine(model.Machine);
            string machineDockTid = drShiftDockInfo["t_id"].ToString();//交接班站台在机台站台表中的t_id
            string shiftDockId = drShiftDockInfo["dock_id"].ToString();//交接班站台id 
            #endregion

            #region 3.获取生产工单计划信息
            DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(model.ProductPlanOutId);
            if (dtProductPlanInfo == null || dtProductPlanInfo.Length < 1)
            {
                throw new WmsException(500, "未查询到该机台对应的生产工单！");
            }
            string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态
            string job_no = dtProductPlanInfo[0]["job_no"].ToString();//生产工单单号
            string product_item_id = dtProductPlanInfo[0]["item_id"].ToString();//成品物料id
            string product_item_code = dtProductPlanInfo[0]["item_code"].ToString();//成品编码
            string product_item_name = dtProductPlanInfo[0]["item_name"].ToString();//成品名称
            #endregion

            #region 4.获取站台上所挂的容器主档id
            //获取站台上所挂的容器主档id
            List<string> containerHList = new List<string>();
            //使用“,”连接挂在站台上的所有容器主档id
            string containerHTidInfo = string.Empty;
            for (int i = 0; i < dtDockInfo.Rows.Count; i++)
            {
                string container_id = dtDockInfo.Rows[i]["container_id"].ToString();//站台上容器id
                if (!string.IsNullOrEmpty(container_id))
                {
                    //恢复已删除的容器主档的软删除标记为1
                    Container.UpdateHeadActStatus(container_id, "1");
                    containerHList.Add(container_id);
                }
            }
            containerHTidInfo = string.Join(",", containerHList.ToArray());
            #endregion

            #region 5.将交班物料json信息转化成json对象，交班时更新容器上的物料数量
            //交班物料数据信息
            JObject entity = JObject.Parse(model.Details);
            if (entity["ReturnItems"].Count() == 0)
            {
                throw new WmsException(500, "无法获取交班物料明细数据！");
            }
            for (int i = 0; i < entity["ReturnItems"].Count(); i++)
            {
                var job = entity["ReturnItems"][i];
                decimal planQty = Convert.ToDecimal(job["planQty"]);
                decimal qty = Convert.ToDecimal(job["qty"]);
                string detailId = job["containerDetailId"].ToString();
                //修改容器明细中物料数量
                Container.UpdateDetailQty(detailId, qty);
            }
            #endregion

            #region 6.将交接班物料对应的容器挂在交接班站台上
            //将交接班物料对应的容器挂在交接班站台上
            ce.UpdateMachineDockDeatilId(machineDockTid, containerHTidInfo, model.ProductPlanOutId, string.Empty, model.IsChangePersonReceive);
            #endregion
        }
        #endregion

        #region 接班
        /// <summary>
        /// 接班
        /// </summary>
        /// <param name="model"></param>
        public void ReturnMaterielReturnBill(ReturnMaterielReturnBillModel model)
        {
            #region 1.交班物料数据信息
            //交班物料数据信息
            JObject entity = JObject.Parse(model.Details);
            if (entity["ItemList"].Count() == 0)
            {
                throw new WmsException(500, "无法获取交班物料明细数据！");
            }
            #endregion

            #region 2.获取接班生产工单计划信息
            DateTime dateTime = DateTime.Now;
            DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(model.ProductPlanOutId);
            if (dtProductPlanInfo == null || dtProductPlanInfo.Length < 1)
            {
                throw new WmsException(500, "未查询到该机台对应的生产工单！");
            }
            string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态
            if (plan_status != PlanOutStatus.PlanOutStatus_Executing.ToString())
            {
                throw new WmsException(500, "该机台对应的生产工单,不可执行！");
            }
            string nextProductPlanOutId = dtProductPlanInfo[0]["t_id"].ToString();//接班时当前生产工单id
            string next_job_no = dtProductPlanInfo[0]["job_no"].ToString();//当前生产工单单号
            string next_item_id = dtProductPlanInfo[0]["item_id"].ToString();//当前生产牌号
            string next_item_name = dtProductPlanInfo[0]["item_name"].ToString();//当前生产牌号名称
            string next_shift_id = dtProductPlanInfo[0]["shift_id"].ToString();//当前班组
            string next_team_id = dtProductPlanInfo[0]["team_id"].ToString();//当前班次
            #endregion

            #region 3.处理除了人工领料和交接班站台以外的站台交接班事宜：创建虚入和虚出任务
            //获取机台上除了人工领料和交接班站台以外的所有站台信息
            DataTable _dtDockInfo = ce.GetAllDockNotContainsRJDockByMachine(model.Machine);
            for (int i = 0; i < _dtDockInfo.Rows.Count; i++)
            {
                string tId = _dtDockInfo.Rows[i]["t_id"].ToString();//机台站台表t_id
                string docId = _dtDockInfo.Rows[i]["doc_id"].ToString();//要料时保存的工单id
                string cH_id = _dtDockInfo.Rows[i]["container_id"].ToString();//挂在站台上的容器id
                if (string.IsNullOrEmpty(cH_id) || string.IsNullOrEmpty(docId))
                {
                    continue;
                }
                DataRow[] tempProductPlanInfo = ce.GetProductPlanInfo(docId);
                DataRow drContainerHInfo = Container.GetContainerH(cH_id);//容器主档

                #region 创建虚入和虚出任务

                #region 虚入任务主档
                //虚入任务主档
                Model.WorkTaskModel taskModel = new Model.WorkTaskModel()
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                    ZoneID = "",//区域ID
                    TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                    //DependentTaskNo = "",//依赖任务号
                    TaskType = TaskType.TaskType_InBound.ToString(),//任务类型编码"接班"
                    ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                    //ExecutedBy = "",//任务执行人
                    //EquipmentID = "",//执行设备ID
                    //EquipmentType = "",//执行设备类型
                    //ActualEquipmentID = "",//实际执行设备ID
                    SeqNo = 1,//顺序号
                    Priority = 1,//优先级
                    ContainerID = cH_id,//容器ID
                    ContainerBarcode = drContainerHInfo["container_barcode"].ToString(),//容器条码
                    RefDocType = DocType.DocType_ReturnMaterielToNext.ToString(),//来源单据类型"接班"
                    RefDocID = docId,//来源单ID
                    //RefDocDetailID = "",//来源单明细ID
                    RefDocNo = tempProductPlanInfo[0]["job_no"].ToString(),//来源单号
                    SrcAddrID = _dtDockInfo.Rows[i]["t_id"].ToString(),//来源地址ID
                    SrcAddrType = AddressType.AddressType_MachineStation.ToString(),//来源地址类型"dock"
                    SrcAddrText = _dtDockInfo.Rows[i]["DOCK_NAME"].ToString(),//来源地址
                    DestAddrID = _dtDockInfo.Rows[i]["t_id"].ToString(),//目标地址ID
                    DestAddrType = AddressType.AddressType_MachineStation.ToString(),//目标地址类型"dock"
                    DestAddrText = _dtDockInfo.Rows[i]["DOCK_NAME"].ToString(),//目标地址
                    //ActualDestAddrID = "",//实际目标地址ID
                    //ActualDestAddrText = "",//实际目标地址
                    TaskStatus = TaskStatus.TaskStatus_Finished.ToString(),//任务状态
                    StartDate = dateTime,//开始时间
                    EndDate = dateTime,//结束时间
                    IsExecutable = "0",//任务是否可执行
                    //PriorityChangeTime = dateTime,//优先级修改时间        
                    LedText = "",//LED显示内容
                    ExistLgv = "0",//是否存在LGV
                    //CreateDate = dateTime,//创建时间
                    //CreatedBy = "",//创建人帐号
                    //ModifyIP = "",//操作人IP 
                    //ModifyDate = dateTime,//编辑时间
                    //LastModifiedBy = "",//最后修改人帐号
                    ActStatus = "1"//状态
                };
                WorkTask.InsertFinishedTask(taskModel);
                #endregion

                #region 虚出任务主档
                //虚出任务主档
                Model.WorkTaskModel outTaskModel = new Model.WorkTaskModel()
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                    ZoneID = "",//区域ID
                    TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                    DependentTaskNo = taskModel.TaskNo,//依赖任务号
                    TaskType = TaskType.TaskType_OutBound.ToString(),//任务类型编码"接班"
                    ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                    //ExecutedBy = "",//任务执行人
                    //EquipmentID = "",//执行设备ID
                    //EquipmentType = "",//执行设备类型
                    //ActualEquipmentID = "",//实际执行设备ID
                    SeqNo = 1,//顺序号
                    Priority = 1,//优先级
                    ContainerID = cH_id,//容器ID
                    ContainerBarcode = drContainerHInfo["container_barcode"].ToString(),//容器条码
                    RefDocType = DocType.DocType_ReturnMaterielReturnBill.ToString(),//来源单据类型"接班"
                    RefDocID = nextProductPlanOutId,//来源单ID
                    //RefDocDetailID = "",//来源单明细ID
                    RefDocNo = next_job_no,//来源单号
                    SrcAddrID = _dtDockInfo.Rows[i]["t_id"].ToString(),//来源地址ID
                    SrcAddrType = AddressType.AddressType_MachineStation.ToString(),//来源地址类型"dock"
                    SrcAddrText = _dtDockInfo.Rows[i]["dock_name"].ToString(),//来源地址
                    DestAddrID = _dtDockInfo.Rows[i]["t_id"].ToString(),//目标地址ID
                    DestAddrType = AddressType.AddressType_MachineStation.ToString(),//目标地址类型"dock"
                    DestAddrText = _dtDockInfo.Rows[i]["dock_name"].ToString(),//目标地址
                    //ActualDestAddrID = "",//实际目标地址ID
                    //ActualDestAddrText = "",//实际目标地址
                    TaskStatus = TaskStatus.TaskStatus_Finished.ToString(),//任务状态
                    StartDate = dateTime,//开始时间
                    EndDate = dateTime,//结束时间
                    IsExecutable = "0",//任务是否可执行
                    //PriorityChangeTime = dateTime,//优先级修改时间        
                    LedText = "",//LED显示内容
                    ExistLgv = "0",//是否存在LGV
                    //CreateDate = dateTime,//创建时间
                    //CreatedBy = "",//创建人帐号
                    //ModifyIP = "",//操作人IP 
                    //ModifyDate = dateTime,//编辑时间
                    //LastModifiedBy = "",//最后修改人帐号
                    ActStatus = "1"//状态
                };
                WorkTask.InsertFinishedTask(outTaskModel);
                #endregion

                for (int j = 0; j < entity["ItemList"].Count(); j++)
                {
                    var job = entity["ItemList"][j];
                    //decimal planQty = Convert.ToDecimal(job["planQty"]);
                    decimal qty = Convert.ToDecimal(job["qty"]);
                    string detailId = job["containerDetailId"].ToString();
                    string containerId = job["containerId"].ToString();
                    if (!string.Equals(cH_id, containerId))
                    {
                        continue;
                    }
                    DataRow detail = Container.GetDetail(detailId);

                    #region 虚入任务物料信息
                    WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
                    {
                        ID = SnowflakeID.Guid().ToString(),//主键ID
                        //ClientID ="",//集团ID
                        //OrgID ="",//组织ID
                        //WarehouseID ="",//仓库ID
                        TaskID = taskModel.ID,//主任务ID
                        ItemID = detail["item_id"].ToString(),//物料ID
                        ItemName = detail["item_name"].ToString(),//物料名称
                        PlanQty = qty,//计划数量
                        ProcessedQty = qty,//已处理数量
                        FinishedQty = qty,//完成数量
                        UomQtyID = detail["uom_qty_id"].ToString(),//数量单位
                        //FinishedStatQty="",//完成统计数量
                        UomStatID = detail["uom_stat_id"].ToString(),//统计数量单位
                        ContainerDetailID = detail["t_id"].ToString(),//容器明细ID
                        RefDocType = DocType.DocType_ReturnMaterielToNext.ToString(),//来源单据类型
                        RefDocID = docId,//来源单ID
                        //RefDocDetailID ="",//来源单明细ID
                        RefDocNo = tempProductPlanInfo[0]["job_no"].ToString(),//来源单号
                        StartDate = dateTime,//开始时间
                        EndDate = dateTime,//结束时间
                        ProcessStatus = ProcessStatus.ProcessStatus_Done.ToString(),//工作状态
                        //CreateDate="",//创建时间
                        //CreatedBy ="",//创建人帐号 
                        //ModifyIP ="",//操作人IP 
                        //ModifyDate="",//编辑时间
                        //LastModifiedBy ="",//最后修改人帐号
                        //ActStatus =""//状态
                    };
                    WorkTaskItem.InsertFinishedTaskItem(taskItemModel);
                    #endregion

                    #region 虚出任务物料信息
                    WorkTaskItemModel outTaskItemModel = new WorkTaskItemModel()
                    {
                        ID = SnowflakeID.Guid().ToString(),//主键ID
                        //ClientID ="",//集团ID
                        //OrgID ="",//组织ID
                        //WarehouseID ="",//仓库ID
                        TaskID = outTaskModel.ID,//主任务ID
                        ItemID = detail["item_id"].ToString(),//物料ID
                        ItemName = detail["item_name"].ToString(),//物料名称
                        PlanQty = qty,//计划数量
                        ProcessedQty = qty,//已处理数量
                        FinishedQty = qty,//完成数量
                        UomQtyID = detail["uom_qty_id"].ToString(),//数量单位
                        //FinishedStatQty="",//完成统计数量
                        UomStatID = detail["uom_stat_id"].ToString(),//统计数量单位
                        ContainerDetailID = detail["t_id"].ToString(),//容器明细ID
                        RefDocType = DocType.DocType_ReturnMaterielReturnBill.ToString(),//来源单据类型
                        RefDocID = nextProductPlanOutId,//来源单ID
                        //RefDocDetailID ="",//来源单明细ID
                        RefDocNo = next_job_no,//来源单号
                        StartDate = dateTime,//开始时间
                        EndDate = dateTime,//结束时间
                        ProcessStatus = ProcessStatus.ProcessStatus_Done.ToString(),//工作状态
                        //CreateDate="",//创建时间
                        //CreatedBy ="",//创建人帐号 
                        //ModifyIP ="",//操作人IP 
                        //ModifyDate="",//编辑时间
                        //LastModifiedBy ="",//最后修改人帐号
                        //ActStatus =""//状态
                    };
                    WorkTaskItem.InsertFinishedTaskItem(outTaskItemModel);
                    #endregion

                    //修改容器明细中物料数量
                    Container.UpdateDetailQty(detailId, qty);
                }
                #endregion

                //修稿挂在站台上容器和生产工单
                ce.UpdateMachineDockDeatilId(tId, cH_id, nextProductPlanOutId, string.Empty, string.Empty);
            }
            #endregion

            #region 4.获取交接班站台
            //获取交接班站台
            DataRow drShiftDockInfo = ce.GetShiftDockByMachine(model.Machine);
            string mdTid = drShiftDockInfo["t_id"].ToString();//交接班站台在机台站台表中的t_id
            string beforeProductPlanOutId = drShiftDockInfo["doc_id"].ToString();//交班时的生产工单
            string container_ids = drShiftDockInfo["container_id"].ToString();//交接班站台对应的容器id（该容器id可能是使用逗号连接的很多容器主档id） 
            #endregion            

            #region 5.清空挂在交接班站台上容器和生产工单
            //清空挂在交接班站台上容器和生产工单
            ce.UpdateMachineDockDeatilId(mdTid, string.Empty, string.Empty, string.Empty, string.Empty);
            #endregion            

            #region 6.如果人工领料数据不需要交接班的话，则返回
            //如果人工领料数据不需要交接班的话，则返回
            if (string.Equals(model.IsChangePersonReceive, "0"))
            {
                return;
            }
            #endregion

            #region 7.获取人工领料站台
            //获取人工领料站台
            DataRow drJieBDockInfo = ce.GetJieBDockByMachine(model.Machine);
            string jieBContainer_ids = drJieBDockInfo["container_id"].ToString();//人工领料站台对应的容器id（该容器id可能是使用逗号连接的很多容器主档id）
            string jieBDockId = drJieBDockInfo["dock_id"].ToString();//人工领料站台id
            string jieBDockName = drJieBDockInfo["dock_name"].ToString();//人工领料站台名称
            string jieBDockTId = drJieBDockInfo["t_id"].ToString();//人工领料站台在机台站台表中的t_id
            string doc_id = drJieBDockInfo["doc_id"].ToString();//人工领料时的生产工单id
            #endregion

            #region 8.清空挂在人工领料站台上容器和生产工单
            //清空挂在人工领料站台上容器和生产工单
            ce.UpdateMachineDockDeatilId(jieBDockTId, string.Empty, string.Empty, string.Empty, string.Empty);
            #endregion

            //无人工领料
            if (string.IsNullOrEmpty(doc_id))
            {
                return;
            }

            #region 9.获取挂在人工领料站台上的生产工单信息
            DataRow[] drBeforeProductPlanInfo = ce.GetProductPlanInfo(doc_id);//人工领料时的生产工单信息
            string before_job_no = drBeforeProductPlanInfo[0]["job_no"].ToString();//人工领料时的生产工单单号
            string before_item_id = drBeforeProductPlanInfo[0]["item_id"].ToString();//人工领料时的生产牌号
            string before_item_name = drBeforeProductPlanInfo[0]["item_name"].ToString();//人工领料时的牌号名称
            string before_shift_id = drBeforeProductPlanInfo[0]["shift_id"].ToString();//人工领料时的班组
            string before_team_id = drBeforeProductPlanInfo[0]["team_id"].ToString();//人工领料时的班次  
            #endregion

            #region 10.单独处理接班时处理人工领料数据，产生已完成的虚退单据和虚领单据，并产生两者的已完成任务
            string[] jieBContainer_idArr = jieBContainer_ids.Split(",");
            //接班时处理人工领料数据
            for (int i = 0; i < jieBContainer_idArr.Length; i++)
            {
                string cH_id = jieBContainer_idArr[i];
                if (string.IsNullOrEmpty(cH_id))
                {
                    continue;
                }
                DataTable _pickReturnHInfo = ce.GetPickReturnHByPlanOutNoAndContailnerH(before_job_no, cH_id);
                if (_pickReturnHInfo.Rows.Count > 1)
                {
                    throw new WmsException(500, "接班时根据生产工单单号和容器主档获取人工领料单据信息多以一个");
                }
                string _pickReturnNo = _pickReturnHInfo.Rows[0]["pick_return_no"].ToString();//人工领料单据号
                DataRow _drContainerHInfo = Container.GetContainerH(cH_id);//容器主档

                #region 创建虚退和虚领单据,以及对应的任务

                #region 虚退单据(人工退料)主档
                PickReturnHModel ph = new PickReturnHModel();
                ph.Tid = SnowflakeID.Guid().ToString();
                ph.PickReturnNo = WmsHelper.GetSequence("PersonReturn_No");
                ph.PickReturnType = PickReturnType.PickReturnType_RGTL.ToString();
                ph.JobNo = before_job_no;
                ph.EquipmentId = model.Machine;
                ph.DockId = jieBDockId;
                ph.DockName = jieBDockName;
                ph.ItemId = before_item_id;
                ph.ItemName = before_item_name;
                ph.ContainerId = cH_id;
                ph.ShiftId = before_shift_id;
                ph.TeamId = before_team_id;
                ph.PickReturnDate = dateTime;
                //ph.Route = string.Empty;
                ph.RefDocNo = _pickReturnNo;
                //ph.SrcAddrId = string.Empty;
                //ph.SrcAddrType = string.Empty;
                //ph.SrcAddrText = string.Empty;
                //ph.DestAddrId = string.Empty;
                //ph.DestAddrType = string.Empty;
                //ph.DestAddrText = string.Empty;
                //ph.ProductionLine = string.Empty;
                //ph.IsRf = string.Empty;
                ph.Reason = "呼叫终端交接班时产生退料";
                ph.PickReturnStatus = PickReturnStatus.PickReturnStatus_Finish.ToString();
                ce.InsertPickReturnH(ph);
                #endregion

                #region 虚退任务主档
                Model.WorkTaskModel taskModel = new Model.WorkTaskModel()
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                    ZoneID = "",//区域ID
                    TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                    //DependentTaskNo = "",//依赖任务号
                    TaskType = TaskType.TaskType_InBound.ToString(),//任务类型编码"接班"
                    ExecutorType = TaskExecutor.TaskExecutor_WMS.ToString(),//执行者类型
                    //ExecutedBy = "",//任务执行人
                    //EquipmentID = "",//执行设备ID
                    //EquipmentType = "",//执行设备类型
                    //ActualEquipmentID = "",//实际执行设备ID
                    SeqNo = 1,//顺序号
                    Priority = 1,//优先级
                    ContainerID = cH_id,//容器ID
                    ContainerBarcode = _drContainerHInfo["container_barcode"].ToString(),//容器条码
                    RefDocType = DocType.DocType_PersonReturnMateriel.ToString(),//来源单据类型"接班"
                    RefDocID = beforeProductPlanOutId,//来源单ID
                    //RefDocDetailID = "",//来源单明细ID
                    RefDocNo = before_job_no,//来源单号
                    SrcAddrID = jieBDockId,//来源地址ID
                    SrcAddrType = AddressType.AddressType_MachineStation.ToString(),//来源地址类型"dock"
                    SrcAddrText = jieBDockName,//来源地址
                    DestAddrID = jieBDockId,//目标地址ID
                    DestAddrType = AddressType.AddressType_MachineStation.ToString(),//目标地址类型"dock"
                    DestAddrText = jieBDockName,//目标地址
                    //ActualDestAddrID = "",//实际目标地址ID
                    //ActualDestAddrText = "",//实际目标地址
                    TaskStatus = TaskStatus.TaskStatus_Finished.ToString(),//任务状态
                    StartDate = dateTime,//开始时间
                    EndDate = dateTime,//结束时间
                    IsExecutable = "0",//任务是否可执行
                    //PriorityChangeTime = dateTime,//优先级修改时间        
                    LedText = "",//LED显示内容
                    ExistLgv = "0",//是否存在LGV
                    //CreateDate = dateTime,//创建时间
                    //CreatedBy = "",//创建人帐号
                    //ModifyIP = "",//操作人IP 
                    //ModifyDate = dateTime,//编辑时间
                    //LastModifiedBy = "",//最后修改人帐号
                    ActStatus = "1"//状态
                };
                WorkTask.InsertFinishedTask(taskModel);
                #endregion

                #region 虚领单据(人工领料)主档
                PickReturnHModel lph = new PickReturnHModel();
                lph.Tid = SnowflakeID.Guid().ToString();
                lph.PickReturnNo = WmsHelper.GetSequence("PersonReturn_No");
                lph.PickReturnType = PickReturnType.PickReturnType_RGLL.ToString();
                lph.JobNo = next_job_no;
                lph.EquipmentId = model.Machine;
                lph.DockId = jieBDockId;
                lph.DockName = jieBDockName;
                lph.ItemId = next_item_id;
                lph.ItemName = next_item_name;
                lph.ContainerId = cH_id;
                lph.ShiftId = next_shift_id;
                lph.TeamId = next_team_id;
                lph.PickReturnDate = dateTime;
                //lph.Route = string.Empty;
                lph.RefDocNo = string.Empty;
                //lph.SrcAddrId = string.Empty;
                //lph.SrcAddrType = string.Empty;
                //lph.SrcAddrText = string.Empty;
                //lph.DestAddrId = string.Empty;
                //lph.DestAddrType = string.Empty;
                //lph.DestAddrText = string.Empty;
                //lph.ProductionLine = string.Empty;
                //lph.IsRf = string.Empty;
                lph.Reason = "呼叫终端交接班时产生领料";
                lph.PickReturnStatus = PickReturnStatus.PickReturnStatus_Finish.ToString();
                ce.InsertPickReturnH(lph);
                #endregion

                #region 虚领任务主档
                Model.WorkTaskModel outTaskModel = new Model.WorkTaskModel()
                {
                    ID = SnowflakeID.Guid().ToString(),//主键ID
                    ZoneID = "",//区域ID
                    TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
                    DependentTaskNo = taskModel.TaskNo,//依赖任务号
                    TaskType = TaskType.TaskType_OutBound.ToString(),//任务类型编码"接班"
                    ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
                    //ExecutedBy = "",//任务执行人
                    //EquipmentID = "",//执行设备ID
                    //EquipmentType = "",//执行设备类型
                    //ActualEquipmentID = "",//实际执行设备ID
                    SeqNo = 1,//顺序号
                    Priority = 1,//优先级
                    ContainerID = cH_id,//容器ID
                    ContainerBarcode = _drContainerHInfo["container_barcode"].ToString(),//容器条码
                    RefDocType = DocType.DocType_PersonReceiveMateriel.ToString(),//来源单据类型"接班"
                    RefDocID = nextProductPlanOutId,//来源单ID
                    //RefDocDetailID = "",//来源单明细ID
                    RefDocNo = next_job_no,//来源单号
                    SrcAddrID = jieBDockId,//来源地址ID
                    SrcAddrType = AddressType.AddressType_MachineStation.ToString(),//来源地址类型"dock"
                    SrcAddrText = jieBDockName,//来源地址
                    DestAddrID = jieBDockId,//目标地址ID
                    DestAddrType = AddressType.AddressType_MachineStation.ToString(),//目标地址类型"dock"
                    DestAddrText = jieBDockName,//目标地址
                    //ActualDestAddrID = "",//实际目标地址ID
                    //ActualDestAddrText = "",//实际目标地址
                    TaskStatus = TaskStatus.TaskStatus_Finished.ToString(),//任务状态
                    StartDate = dateTime,//开始时间
                    EndDate = dateTime,//结束时间
                    IsExecutable = "0",//任务是否可执行
                    //PriorityChangeTime = dateTime,//优先级修改时间        
                    LedText = "",//LED显示内容
                    ExistLgv = "0",//是否存在LGV
                    //CreateDate = dateTime,//创建时间
                    //CreatedBy = "",//创建人帐号
                    //ModifyIP = "",//操作人IP 
                    //ModifyDate = dateTime,//编辑时间
                    //LastModifiedBy = "",//最后修改人帐号
                    ActStatus = "1"//状态
                };
                WorkTask.InsertFinishedTask(outTaskModel);
                #endregion

                for (int j = 0; j < entity["ItemList"].Count(); j++)
                {
                    var job = entity["ItemList"][j];
                    //decimal planQty = Convert.ToDecimal(job["planQty"]);
                    decimal qty = Convert.ToDecimal(job["qty"]);
                    string detailId = job["containerDetailId"].ToString();
                    string containerId = job["containerId"].ToString();
                    if (!string.Equals(cH_id, containerId))
                    {
                        continue;
                    }
                    DataRow detail = Container.GetDetail(detailId);

                    #region 虚退单据明细
                    PickReturnDModel _prdm = new PickReturnDModel();
                    _prdm.Tid = SnowflakeID.Guid().ToString();//主键
                    _prdm.PickReturnId = ph.Tid;//领料退料单ID
                    _prdm.ContainerDtid = detailId;//容器明细id
                    _prdm.ItemId = detail["item_id"].ToString();//物料ID
                    _prdm.ItemCode = detail["item_code"].ToString();//物料编码
                    _prdm.ItemName = detail["item_name"].ToString();//物料名称
                    _prdm.LotNo = detail["lot_no"].ToString();//批次号
                    _prdm.Suppliy = job["ownerId"].ToString();//供应商
                    _prdm.Qty = qty;//数量
                    _prdm.UomQtyId = detail["uom_qty_id"].ToString();//数量单位
                    _prdm.Memo = string.Empty;//备注
                    _prdm.CreateDate = dateTime;//创建时间
                    _prdm.ActStatus = "1";//
                    ce.InsertPickReturnD(_prdm);
                    #endregion

                    #region 虚退任务明细
                    WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
                    {
                        ID = SnowflakeID.Guid().ToString(),//主键ID
                        //ClientID ="",//集团ID
                        //OrgID ="",//组织ID
                        //WarehouseID ="",//仓库ID
                        TaskID = taskModel.ID,//主任务ID
                        ItemID = detail["item_id"].ToString(),//物料ID
                        ItemName = detail["item_name"].ToString(),//物料名称
                        PlanQty = qty,//计划数量
                        ProcessedQty = qty,//已处理数量
                        FinishedQty = qty,//完成数量
                        UomQtyID = detail["uom_qty_id"].ToString(),//数量单位
                        //FinishedStatQty="",//完成统计数量
                        UomStatID = detail["uom_stat_id"].ToString(),//统计数量单位
                        ContainerDetailID = detailId,//容器明细ID
                        RefDocType = DocType.DocType_PersonReturnMateriel.ToString(),//来源单据类型
                        RefDocID = beforeProductPlanOutId,//来源单ID
                        //RefDocDetailID ="",//来源单明细ID
                        RefDocNo = before_job_no,//来源单号
                        StartDate = dateTime,//开始时间
                        EndDate = dateTime,//结束时间
                        ProcessStatus = ProcessStatus.ProcessStatus_Done.ToString(),//工作状态
                        //CreateDate="",//创建时间
                        //CreatedBy ="",//创建人帐号 
                        //ModifyIP ="",//操作人IP 
                        //ModifyDate="",//编辑时间
                        //LastModifiedBy ="",//最后修改人帐号
                        //ActStatus =""//状态
                    };
                    WorkTaskItem.InsertFinishedTaskItem(taskItemModel);
                    #endregion

                    #region 虚领单据明细
                    PickReturnDModel _lprdm = new PickReturnDModel();
                    _lprdm.Tid = SnowflakeID.Guid().ToString();//主键
                    _lprdm.PickReturnId = lph.Tid;//领料退料单ID
                    _lprdm.ContainerDtid = detailId;//容器明细id
                    _lprdm.ItemId = detail["item_id"].ToString();//物料ID
                    _lprdm.ItemCode = detail["item_code"].ToString();//物料编码
                    _lprdm.ItemName = detail["item_name"].ToString();//物料名称
                    _lprdm.LotNo = detail["lot_no"].ToString();//批次号
                    _lprdm.Suppliy = job["ownerId"].ToString();//供应商
                    _lprdm.Qty = qty;//数量
                    _lprdm.UomQtyId = detail["uom_qty_id"].ToString();//数量单位
                    _lprdm.Memo = string.Empty;//备注
                    _lprdm.CreateDate = dateTime;//创建时间
                    _lprdm.ActStatus = "1";//
                    ce.InsertPickReturnD(_lprdm);
                    #endregion

                    #region 虚领任务明细
                    WorkTaskItemModel outTaskItemModel = new WorkTaskItemModel()
                    {
                        ID = SnowflakeID.Guid().ToString(),//主键ID
                        //ClientID ="",//集团ID
                        //OrgID ="",//组织ID
                        //WarehouseID ="",//仓库ID
                        TaskID = outTaskModel.ID,//主任务ID
                        ItemID = detail["item_id"].ToString(),//物料ID
                        ItemName = detail["item_name"].ToString(),//物料名称
                        PlanQty = qty,//计划数量
                        ProcessedQty = qty,//已处理数量
                        FinishedQty = qty,//完成数量
                        UomQtyID = detail["uom_qty_id"].ToString(),//数量单位
                        //FinishedStatQty="",//完成统计数量
                        UomStatID = detail["uom_stat_id"].ToString(),//统计数量单位
                        ContainerDetailID = detailId,//容器明细ID
                        RefDocType = DocType.DocType_PersonReceiveMateriel.ToString(),//来源单据类型
                        RefDocID = model.ProductPlanOutId,//来源单ID
                        //RefDocDetailID ="",//来源单明细ID
                        RefDocNo = next_job_no,//来源单号
                        StartDate = dateTime,//开始时间
                        EndDate = dateTime,//结束时间
                        ProcessStatus = ProcessStatus.ProcessStatus_Done.ToString(),//工作状态
                        //CreateDate="",//创建时间
                        //CreatedBy ="",//创建人帐号 
                        //ModifyIP ="",//操作人IP 
                        //ModifyDate="",//编辑时间
                        //LastModifiedBy ="",//最后修改人帐号
                        //ActStatus =""//状态
                    };
                    WorkTaskItem.InsertFinishedTaskItem(outTaskItemModel);
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion

        #region 根据货位获取执行任务的堆垛机(上位系统对接时调用该方法)
        /// <summary>
        /// 根据货位获取执行任务的堆垛机(上位系统对接时调用该方法)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebJsonInfo GetEquipmentByLoc(GetEquipmentByLocModel model)
        {
            WebJsonInfo wji = new WebJsonInfo();
            string loc = model.LocationId;
            wji.json = new { _equipment = ce.GetEquipmentByLoc(model.Row, ref loc), _loc = loc };
            wji.isSucceed = true;
            return wji;
        }
        #endregion

        #region 接班前修改物料数量
        /// <summary>
        /// 接班前修改物料数量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void EditMaterielBillModel(EditMaterielBillModel model)
        {
            //交班物料数据信息
            JObject entity = JObject.Parse(model.Details);
            if (entity["ItemList"].Count() == 0)
            {
                throw new WmsException(500, "无法获取交班物料明细数据！");
            }
            #region 获取接班生产工单计划信息
            DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(model.ProductPlanOutId);
            if (dtProductPlanInfo == null || dtProductPlanInfo.Length < 1)
            {
                throw new WmsException(500, "未查询到该机台对应的生产工单！");
            }
            string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态
            if (plan_status != PlanOutStatus.PlanOutStatus_Executing.ToString())
            {
                throw new WmsException(500, "该机台对应的生产工单,不可执行！");
            }
            #endregion

            //获取交接班站台
            DataRow drShiftDockInfo = ce.GetShiftDockByMachine(model.Machine);
            string mdTid = drShiftDockInfo["t_id"].ToString();//交接班站台在机台站台表中的t_id
            string shiftDockId = drShiftDockInfo["dock_id"].ToString();//交接班站台id
            string beforeProductPlanOutId = drShiftDockInfo["doc_id"].ToString();//交班时的生产工单
            string container_ids = drShiftDockInfo["container_id"].ToString();//交接班站台对应的容器id（该容器id可能是使用逗号连接的很多容器主档id）
            string[] container_idArr = container_ids.Split(",");
            for (int i = 0; i < container_idArr.Length; i++)
            {
                #region 修改容器明细物料数量
                for (int j = 0; j < entity["ItemList"].Count(); j++)
                {
                    var job = entity["ItemList"][j];
                    //decimal planQty = Convert.ToDecimal(job["planQty"]);
                    decimal qty = Convert.ToDecimal(job["qty"]);
                    string detailId = job["containerDetailId"].ToString();
                    //修改容器明细中物料数量
                    Container.UpdateDetailQty(detailId, qty);
                }
                #endregion
            }
        }
        #endregion

        #region 向wcs发送任务id
        /// <summary>
        /// 向wcs发送任务id
        /// </summary>
        /// <param name="taskIid"></param>
        private WebJsonInfo SendToWCS(string taskIid)
        {
            //发送给wcs
            List<string> _idList = new List<string>();
            _idList.Add(taskIid);
            return WorkTaskInterface.Send(_idList);
        }
        #endregion

        #region 回收出库货位一次分配后的任务
        /// <summary>
        /// 回收出库货位一次分配后的任务
        /// </summary>
        /// <param name="taskRow">任务主档信息</param>
        /// <returns></returns>
        public void RecycleOutBoundFirstDistributionLocationTask(DataRow taskRow)
        {
            string tid = taskRow["t_id"].ToString();
            string row = taskRow["src_addr_text"].ToString();
            string containerId = taskRow["container_id"].ToString();
            string zoneId = taskRow["zone_id"].ToString();
            //获取预分配最小的货位，并修改成预分配状态
            DataTable dt = ce.GetLocByLine(row, false);
            string locId = dt.Rows[0]["t_id"].ToString();
            ce.UpdateLocStatusByLocationId(locId, LocationStatus.LocationStatus_Stored.ToString(), LocationStatus.LocationStatus_Plan.ToString(), "");
            bool isHavePlanLoc = ce.GetPlanLocNumByLine(row);
            if (isHavePlanLoc == false)
            {
                ce.UpdatelocationflagByRow(row, LocationFlag.LocationFlag_Usable.ToString());
            }
        }
        #endregion

        #region 回收出库货位二次分配后的任务
        /// <summary>
        /// 回收出库货位二次分配后的任务
        /// </summary>
        /// <param name="taskRow">任务主档信息</param>
        /// <returns></returns>
        public void RecycleOutBoundSecoundDistributionLocationTask(DataRow taskRow)
        {
            string tid = taskRow["t_id"].ToString();
            LocationModel locRow = Location.GetLocationById(taskRow["src_addr_id"].ToString());
            string row = locRow.location_row.ToString();
            string containerId = taskRow["container_id"].ToString();
            string zoneId = taskRow["zone_id"].ToString();
            //获取预分配最小的货位，并修改成预分配状态
            DataTable dt = ce.GetLocByLine(row, false);
            string locId = dt.Rows[0]["t_id"].ToString();
            ce.UpdateLocStatusByLocationId(locId, LocationStatus.LocationStatus_Stored.ToString(), LocationStatus.LocationStatus_Out.ToString(), "");
            bool isHavePlanLoc = ce.GetPlanLocNumByLine(row);
            if (isHavePlanLoc == false)
            {
                ce.UpdatelocationflagByRow(row, LocationFlag.LocationFlag_Usable.ToString());
            }
        }
        #endregion

        #region 回收入库货位一次分配后的任务
        /// <summary>
        /// 回收入库货位一次分配后的任务
        /// </summary>
        /// <param name="taskRow">任务主档信息</param>
        /// <returns></returns>
        public void RecycleInBoundFirstDistributionLocationTask(DataRow taskRow)
        {
            string tid = taskRow["t_id"].ToString();
            string row = taskRow["dest_addr_text"].ToString();
            string containerId = taskRow["container_id"].ToString();
            string zoneId = taskRow["zone_id"].ToString();
            NtiCloud.Service.WMS.Base.CallEnd ce = new Base.CallEnd();
            DataTable dt = ce.GetLocByLine(row, true);
            string locId = dt.Rows[0]["t_id"].ToString();
            ce.UpdateLocStatusByLocationId(locId, LocationStatus.LocationStatus_Empty.ToString(), LocationStatus.LocationStatus_Plan.ToString(), "");
            bool isHavePlanLoc = ce.GetPlanLocNumByLine(row);
            if (isHavePlanLoc == false)
            {
                ce.UpdatelocationflagByRow(row, LocationFlag.LocationFlag_Usable.ToString());
            }
        }
        #endregion

        #region 回收入库货位二次分配后的任务
        /// <summary>
        /// 回收入库货位二次分配后的任务
        /// </summary>
        /// <param name="taskRow">任务主档信息</param>
        /// <returns></returns>
        public void RecycleInBoundSecoundDistributionLocationTask(DataRow taskRow)
        {
            string tid = taskRow["t_id"].ToString();

            LocationModel locRow = Location.GetLocationById(taskRow["dest_addr_id"].ToString());
            string row = locRow.location_row.ToString();
            string containerId = taskRow["container_id"].ToString();
            string zoneId = taskRow["zone_id"].ToString();
            NtiCloud.Service.WMS.Base.CallEnd ce = new Base.CallEnd();
            ce.UpdateLocStatusByLocationId(locRow.t_id, LocationStatus.LocationStatus_Plan.ToString(), LocationStatus.LocationStatus_In.ToString(), "");
            bool isHavePlanLoc = ce.GetPlanLocNumByLine(row);
            if (isHavePlanLoc == false)
            {
                ce.UpdatelocationflagByRow(row, LocationFlag.LocationFlag_Usable.ToString());
            }
            else
            {
                //获取预分配最大的货位，并修改成空状态
                DataTable _dt = ce.GetLocByLine(row, true);
                string _locId = _dt.Rows[0]["t_id"].ToString();
                ce.UpdateLocStatusByLocationId(_locId, LocationStatus.LocationStatus_Empty.ToString(), LocationStatus.LocationStatus_Plan.ToString(), "");
                //清空容器主档货位字段信息，并软删除容器主档
                Container.UpdateHeadActStatus(containerId, "0").UpdateContainerLocation(containerId, string.Empty);
            }
        }
        #endregion

        //#region 备份一次性生成退余料到货位的任务，该方法，不使用了，使用产生两步任务替代。同时任务的回收、部分完成、完成均需要备份，重新按照新的逻辑实现

        //#region 备份退余料任务产生
        ///// <summary>
        ///// 备份退余料任务产生
        ///// </summary>
        ///// <param name="model">退余料参数model</param>
        ///// <param name="machineDockTid">站台在机台站台表中的t_id</param>
        ///// <param name="equipment_code">站台编码</param>
        ///// <param name="job_no">生产工单单号</param>
        ///// <param name="taskModel">退余料任务model</param>
        //private void ReturnPallet_20201221(ClientReturnPalletModel model, out string machineDockTid, out string equipment_code, out string job_no, out Model.WorkTaskModel taskModel)
        //{
        //    Putaway putaway = new Putaway();
        //    #region 1.检查站台当前任务 
        //    //查询站台未执行完成的任务数量
        //    DataRow dtTaskNumInfo = ce.SelectTaskNumByDock(model.Dock);
        //    int cnt = Convert.ToInt32(dtTaskNumInfo["cnt"].ToString());//站台未执行完成的任务数量
        //    if (cnt > 0)
        //    {
        //        throw new WmsException(1, "该站台上有未执行完成的任务，无法退余料");
        //    }
        //    #endregion

        //    if (ce.MachineHaveDockInfo(model.Machine, model.Dock) == false)
        //    {
        //        throw new WmsException(1, "机台上没有该站台，请重新登录账号！");
        //    }

        //    #region 2.检查站台状态
        //    //查询站台状态
        //    DataRow dtDockInfo = ce.GetDockAndBomInfo(model.Dock);
        //    machineDockTid = dtDockInfo["t_id"].ToString();
        //    string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
        //    string doc_id = dtDockInfo["doc_id"].ToString();//来源单号，此处存放的是要料时的生产工单
        //    equipment_code = dtDockInfo["equipment_code"].ToString();
        //    string machine_id = dtDockInfo["machine_id"].ToString();//机台id
        //    string container_id = dtDockInfo["container_id"].ToString();//容器主档id
        //    string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
        //    string bomId = dtDockInfo["bomId"].ToString();//bomId
        //    if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
        //    {
        //        throw new WmsException(1, "当前站台位处于故障状态，无法使用！");
        //    }
        //    if (dock_status != EquipmentStatus.EquipmentStatus_Using.ToString())
        //    {
        //        throw new WmsException(1, "当前站台位未使用！");
        //    }
        //    if (string.IsNullOrEmpty(container_id))
        //    {
        //        throw new WmsException(1, "当前站台位无托盘信息！");
        //    }
        //    if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
        //    {
        //        throw new WmsException(1, "当前站台对应的工艺段已停用，无法进行退余料！");
        //    }
        //    #endregion

        //    DataRow _drMachineInfo = ce.GetMachineInfoByMachineId(machine_id);

        //    #region 3.获取货位信息
        //    ////通过货位分配策略，获取可用货位
        //    //string loc_id = ce.GetEmptyLocation();//"1285384218838605824";
        //    //参数：仓库id，容器明细中物料id，bomId，区域id
        //    string loc_id = putaway.AllocateOneLocationForPallet(_drMachineInfo["warehouse_id"].ToString(), "", bomId, _drMachineInfo["zone_id"].ToString());
        //    #endregion

        //    #region 7.获取要料时的生产工单计划信息
        //    DataRow[] dtProductPlanInfo = ce.GetProductPlanInfo(doc_id);
        //    if (dtProductPlanInfo == null || dtProductPlanInfo.Length < 1)
        //    {
        //        throw new WmsException(1, "未查询到该机台对应的生产工单！");
        //    }
        //    string plan_status = dtProductPlanInfo[0]["plan_status"].ToString();//生产工单状态
        //    job_no = dtProductPlanInfo[0]["job_no"].ToString();
        //    string product_item_id = dtProductPlanInfo[0]["item_id"].ToString();//成品物料id
        //    string product_item_code = dtProductPlanInfo[0]["item_code"].ToString();//成品编码
        //    string product_item_name = dtProductPlanInfo[0]["item_name"].ToString();//成品名称
        //    #endregion



        //    if (loc_id.Length <= 3)//小于等于3，表示获取的是行，不是货位id.只要二楼才会出现小于等于3的情况
        //    {
        //        if (string.IsNullOrEmpty(loc_id))
        //        {
        //            throw new WmsException(1, "分配货位失败");
        //        }
        //        TwoFloorReturnPallet(model, equipment_code, loc_id, doc_id, out taskModel);
        //        return;
        //    }



        //    #region 4.查询库位信息
        //    //查询货位对应区域信息和容器信息
        //    DataTable dtLocationInfo = ce.GetLocationInfo(loc_id);

        //    if (dtLocationInfo.Rows.Count > 1)
        //    {
        //        throw new WmsException(1, "退余料时分配的货位属于多个区域");
        //    }

        //    //区域
        //    string zone_id = dtLocationInfo.Rows[0]["zone_id"].ToString();
        //    //货位对应的编码
        //    string loc_no = dtLocationInfo.Rows[0]["location_code"].ToString();
        //    //容器主档id
        //    string locContainer_htid = dtLocationInfo.Rows[0]["t_id"].ToString();
        //    //货位状态
        //    string location_status = dtLocationInfo.Rows[0]["location_status"].ToString();
        //    //判断货位分配策略返回的货位上是否已经与容器主档关联，如果有关联，则说明货位上有物料
        //    if (!string.IsNullOrEmpty(locContainer_htid))
        //    {
        //        throw new WmsException(1, "退余料时分配的货位上已有物料，退料失败！");
        //    }
        //    if (location_status != LocationStatus.LocationStatus_Empty.ToString())
        //    {
        //        throw new WmsException(1, "货位分配策略分配的货位，状态不为空，无法进行退余料");
        //    }
        //    #endregion

        //    #region 5.将退余料json信息转化成json对象
        //    //退余料数据信息
        //    JObject entity = JObject.Parse(model.Details);
        //    if (entity["ReturnItems"].Count() == 0)
        //    {
        //        throw new WmsException(1, "无法获取退余料明细数据！");
        //    }
        //    #endregion

        //    #region 6.获取容器主档信息，容器明细信息
        //    //获取挂在当前站台上容器id
        //    DataTable containerHInfo = ce.GetContainerInfoByDockIdAndMachineId(model.Dock, model.Machine);
        //    if (containerHInfo.Rows.Count > 1)
        //    {
        //        throw new WmsException(1, "根据机台和站台，获取挂在站台上面的容器主档不唯一");
        //    }
        //    string container_htid = containerHInfo.Rows[0]["container_id"].ToString();
        //    //根据容器主档id，获取容器主档信息
        //    DataRow dtrContainerInfo = Container.GetContainerH(container_htid);
        //    //查询容器明细
        //    DataTable dtContainerDInfo = ce.ToDataTable(Container.GetDetailByHeadID(container_htid));
        //    #endregion

        //    //修改容器主档是否允许被呼叫
        //    ce.UpdateContainerAllowCalled(container_htid, model.IsCanBeCallOut);

        //    //查询执行设备(堆垛机)
        //    string ddjEquipId = ce.GetDDJ(loc_id);

        //    #region 8.创建入库任务

        //    #region 插入任务主档
        //    taskModel = new Model.WorkTaskModel()
        //    {
        //        ID = SnowflakeID.Guid().ToString(),//主键ID
        //        ZoneID = zone_id,//区域ID
        //        TaskNo = WmsHelper.GetSequence("TaskNo"),//任务编号
        //        //DependentTaskNo = "",//依赖任务号
        //        TaskType = TaskType.TaskType_InBound.ToString(),//任务类型编码"机台退余料"
        //        ExecutorType = TaskExecutor.TaskExecutor_WCS.ToString(),//执行者类型
        //        //ExecutedBy = "",//任务执行人
        //        EquipmentID = ddjEquipId,//equipRow == null ? "" : equipRow["t_id"].ToString(),//执行设备ID
        //        //EquipmentType = "",//执行设备类型
        //        //ActualEquipmentID = "",//实际执行设备ID
        //        SeqNo = 1,//顺序号
        //        Priority = 1,//优先级
        //        ContainerID = container_htid,//容器ID
        //        ContainerBarcode = dtrContainerInfo["container_barcode"].ToString(),//容器条码
        //        RefDocType = DocType.DocType_ClientReturnPallet.ToString(),//来源单据类型  "机台退余料"
        //        RefDocID = doc_id,//来源单ID
        //        RefDocDetailID = model.Planno,//来源单明细ID,存放mes叫料号
        //        RefDocNo = job_no,//来源单号
        //        SrcAddrID = model.Dock,//来源地址ID
        //        SrcAddrType = AddressType.AddressType_MachineStation.ToString(),//来源地址类型"dock"
        //        SrcAddrText = equipment_code,//来源地址
        //        DestAddrID = loc_id,//目标地址ID
        //        DestAddrType = AddressType.AddressType_Location.ToString(),//目标地址类型"location"
        //        DestAddrText = loc_no,//目标地址
        //        //ActualDestAddrID = "",//实际目标地址ID
        //        //ActualDestAddrText = "",//实际目标地址
        //        TaskStatus = TaskStatus.TaskStatus_Executing.ToString(),//任务状态
        //        //StartDate = dateTime,//开始时间
        //        //EndDate = dateTime,//结束时间
        //        IsExecutable = "1",//任务是否可执行
        //        //PriorityChangeTime = dateTime,//优先级修改时间        
        //        LedText = "",//LED显示内容
        //        ExistLgv = model.HasAGV,//是否存在LGV
        //        //CreateDate = dateTime,//创建时间
        //        //CreatedBy = "",//创建人帐号
        //        //ModifyIP = "",//操作人IP 
        //        //ModifyDate = dateTime,//编辑时间
        //        //LastModifiedBy = "",//最后修改人帐号
        //        ActStatus = "1"//状态
        //    };
        //    WorkTask.Insert(taskModel);
        //    #endregion

        //    for (int i = 0; i < entity["ReturnItems"].Count(); i++)
        //    {
        //        var job = entity["ReturnItems"][i];
        //        decimal qty = Convert.ToDecimal(job["qty"].ToString() == "" ? "0" : job["qty"].ToString());
        //        string containerDetailId = job["containerDetailId"].ToString();//容器明细id
        //        DataRow[] drs = dtContainerDInfo.Select($"t_id='{containerDetailId}'");
        //        if (drs.Length == 1)
        //        {
        //            //修改容器明细中物料数量
        //            Container.UpdateDetailQty(containerDetailId, qty);
        //        }
        //        else
        //        {
        //            throw new WmsException(500, "无法获取退余料明细数据！");
        //        }

        //        #region 插入任务物料信息
        //        WorkTaskItemModel taskItemModel = new WorkTaskItemModel()
        //        {
        //            ID = SnowflakeID.Guid().ToString(),//主键ID
        //            //ClientID = "",//集团ID
        //            //OrgID = "",//组织ID
        //            //WarehouseID = "",//仓库ID
        //            TaskID = taskModel.ID,//主任务ID
        //            ItemID = drs[0]["item_id"].ToString(),//物料ID
        //            ItemName = drs[0]["item_name"].ToString(),//物料名称
        //            PlanQty = qty,//计划数量
        //            //ProcessedQty = "",//已处理数量
        //            //FinishedQty = "",//完成数量
        //            UomQtyID = drs[0]["uom_qty_id"].ToString(),//数量单位
        //            //FinishedStatQty = "",//完成统计数量
        //            UomStatID = drs[0]["uom_stat_id"].ToString(),//统计数量单位
        //            ContainerDetailID = drs[0]["t_id"].ToString(),//容器明细ID
        //            RefDocType = DocType.DocType_ClientReturnPallet.ToString(),//来源单据类型
        //            RefDocID = doc_id,//来源单ID
        //            //RefDocDetailID = "",//来源单明细ID
        //            RefDocNo = job_no,//来源单号
        //            //StartDate = "",//开始时间
        //            //EndDate = "",//结束时间
        //            ProcessStatus = ProcessStatus.ProcessStatus_New.ToString(),//工作状态
        //            //CreateDate = "",//创建时间
        //            //CreatedBy = "",//创建人帐号 
        //            //ModifyIP = "",//操作人IP 
        //            //ModifyDate = "",//编辑时间
        //            //LastModifiedBy = "",//最后修改人帐号
        //            //ActStatus = ""//状态
        //        };
        //        WorkTaskItem.Insert(taskItemModel);
        //        #endregion
        //    }
        //    #endregion

        //    #region 9.获取交接班和人工领料站台
        //    ////获取交接班站台
        //    //DataRow drShiftDockInfo = ce.GetShiftDockByMachine(model.Machine);
        //    //string jiaojiebanDockTid = drShiftDockInfo["t_id"].ToString();//交接班站台在机台站台表中的t_id 
        //    #endregion

        //    #region 10.清空挂在交接班和人工领料站台上的容器
        //    ////清空挂在交接班站台上的容器
        //    //ce.UpdateMachineDockDeatilId(jiaojiebanDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
        //    ////清空挂在人工领料站台上的容器
        //    //UpdateMachineDockDeatilId(jiebanDockTid, string.Empty, string.Empty);
        //    #endregion

        //    #region 11.修改货位状态为待入库
        //    //修改货位状态为待入库
        //    ce.UpdateLocStatusByLocationId(loc_id, LocationStatus.LocationStatus_In.ToString(), location_status, "机台退余料,物料入库,修改货位状态为待入库");
        //    #endregion

        //    //发送给wcs
        //    List<string> _idList = new List<string>();
        //    _idList.Add(taskModel.ID);
        //    WorkTaskInterface.SendInBonud(_idList);
        //}
        //#endregion

        //#region 退余料任务强制完成
        ///// <summary>
        ///// 退余料任务强制完成
        ///// </summary>
        ///// <param name="model"></param>
        //public void FinishClientReturnPallet_20201221(FinishClientReturnPalletModel model)
        //{

        //    #region 1.获取退余料任务信息
        //    //获取退余料任务信息
        //    DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
        //    if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
        //    {
        //        throw new WmsException(500, "未查询到站台退余料任务！");
        //    }
        //    string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
        //    string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
        //    if (refDocType != DocType.DocType_ClientReturnPallet.ToString())
        //    {
        //        throw new WmsException(500, "该任务不是退余料任务，无法强制完成！");
        //    }
        //    //if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString() && taskStatus != TaskStatus.TaskStatus_Executing.ToString())
        //    //{
        //    //    throw new WmsException(500, "任务仅在处于执行中或者已部分完成状态下，才能强制完成！");
        //    //}
        //    if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
        //    {
        //        throw new WmsException(500, "任务已完成，无需再次完成！");
        //    }
        //    #endregion
        //    //任务不是部分完成，则执行部分完成
        //    if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
        //    {
        //        PartionFinishClientReturnPalletModel pft = new PartionFinishClientReturnPalletModel();
        //        pft.TaskTid = model.TaskTid;
        //        PartionFinishClientReturnPallet(pft);
        //    }
        //    //执行完成任务
        //    FinishReturnPalletTaskModel ft = new FinishReturnPalletTaskModel();
        //    ft.TaskTid = model.TaskTid;
        //    FinishReturnPalletTask(ft);

        //    #region 旧版强完任务代码
        //    //#region 1.获取退余料任务信息
        //    ////获取退余料任务信息
        //    //DataTable dtTaskInfo = GeTaskInfoByTaskId(model.TaskTid);
        //    //if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
        //    //{
        //    //    throw new WmsException(500, "未查询到站台退余料任务！");
        //    //}
        //    //string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
        //    //string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
        //    //string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
        //    //string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
        //    //string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退余料时托盘目的库位id
        //    //string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
        //    //string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
        //    //if (refDocType != DocType.DocType_ClientReturnPallet.ToString())
        //    //{
        //    //    throw new WmsException(500, "该任务不是退余料任务，无法强制完成！");
        //    //}
        //    //if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString() && taskStatus != TaskStatus.TaskStatus_Executing.ToString())
        //    //{
        //    //    throw new WmsException(500, "任务仅在处于执行中或者已部分完成状态下，才能强制完成！");
        //    //}
        //    //#endregion

        //    //#region 2.获取任务物料明细
        //    ////获取任务物料明细
        //    //DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
        //    //#endregion

        //    //#region 3.检查站台状态
        //    ////检查站台状态
        //    //DataRow dtDockInfo = GetDockStatus(dockId);
        //    //string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
        //    //string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
        //    //string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
        //    ////if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
        //    ////{
        //    ////    throw new WmsException(500, "当前站台位处于故障状态，无法使用！");
        //    ////}
        //    ////if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
        //    ////{
        //    ////    throw new WmsException(500, "当前站台对应的工艺段已停用，无法进行退余料！");
        //    ////}
        //    //#endregion

        //    //#region 4.获取挂在站台上的容器明细
        //    ////获取挂在站台上的容器明细
        //    //DataRow[] dtrContainerDInfo = Container.GetDetailByHeadID(container_id);
        //    //#endregion

        //    //#region 5.将容器明细物料数量为0，则软删除容器明细
        //    ////容器明细中物料数量qty全为0的情况，已在前台界面限制，走退空托盘业务
        //    ////该目的是在统计消耗时直接使用出库数量减去容器明细中的数量，作为消耗量
        //    //for (int i = 0; i < dtrContainerDInfo.Length; i++)
        //    //{
        //    //    string containerDetailId = dtrContainerDInfo[i]["t_id"].ToString();
        //    //    decimal qty = Convert.ToDecimal(dtrContainerDInfo[i]["qty"].ToString() == "" ? "0" : dtrContainerDInfo[i]["qty"].ToString());
        //    //    if (qty <= 0)//物料数量为0，则删除容器明细，设置成软删除
        //    //    {
        //    //        Container.UpdateDetailActStatus(containerDetailId, "0");
        //    //    }
        //    //    //else//待入量不为0，则将容器明细待入量写入数量字段，并清空待入数量
        //    //    //{
        //    //    //    Container.UpdateDetailQty(containerDetailId, qty).UpdateDetailInQty(containerDetailId, (decimal)0.0);
        //    //    //}
        //    //}
        //    //#endregion

        //    //#region 6.将容器主档与库位关联
        //    ////将容器主档与库位关联
        //    //Container.UpdateContainerLocation(container_id, locId);
        //    //#endregion

        //    //#region 7.更新退余料任务和任务明细
        //    ////将任务更新为完成
        //    //WorkTask.UpdateTaskComplete(model.TaskTid);
        //    ////更新任务物料明细中的已处理数量和完成数量
        //    //for (int i = 0; i < taskItemInfos.Length; i++)
        //    //{
        //    //    string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
        //    //    //根据任务物料id，修改任务物料状态为已完成，并且修改完成和处理数量
        //    //    WorkTask.UpdateTaskItemCompleteAndQty(cdTid);
        //    //}
        //    //WorkTask.MoveToHistory(model.TaskTid);
        //    //#endregion

        //    //#region 8.清空挂在站台上的容器
        //    ////清空挂在站台上的容器
        //    //UpdateMachineDockDeatilId(machineDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
        //    //#endregion

        //    //#region 9.修改站台状态
        //    ////修改站台状态
        //    //UpdateDockStatus(machineDockTid, dockId, EquipmentStatus.EquipmentStatus_OK.ToString());
        //    //#endregion

        //    //#region 10.修改货位状态为已存储
        //    ////修改货位状态为已存储
        //    //UpdateLocStatusByLocationId(locId, LocationStatus.LocationStatus_Stored.ToString(), LocationStatus.LocationStatus_In.ToString(), "机台退余料,物料入库完成,修改货位状态为已存储");
        //    //#endregion 
        //    #endregion
        //}
        //#endregion

        //#region 退余料任务回收
        ///// <summary>
        ///// 退余料任务回收
        ///// </summary>
        ///// <param name="model"></param>
        //public void RecycleClientReturnPallet_20201221(RecycleClientReturnPalletModel model)
        //{
        //    #region 1.获取退余料任务信息
        //    //获取退余料任务信息
        //    DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
        //    if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
        //    {
        //        throw new WmsException(500, "未查询到站台退余料任务！");
        //    }
        //    string taskNo = dtTaskInfo.Rows[0]["task_no"].ToString();//任务号
        //    string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
        //    string taskZoneId = dtTaskInfo.Rows[0]["zone_id"].ToString();//任务中的区域
        //    string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
        //    string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
        //    string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
        //    string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退余料时托盘目的库位id
        //    string destAddrText = dtTaskInfo.Rows[0]["dest_addr_text"].ToString();//任务中，目的货位编号
        //    string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
        //    string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
        //    if (refDocType != DocType.DocType_ClientReturnPallet.ToString())
        //    {
        //        throw new WmsException(500, "该任务不是退余料任务，无法回收任务！");
        //    }
        //    if (taskStatus == TaskStatus.TaskStatus_Finished.ToString())
        //    {
        //        throw new WmsException(500, "该任务已完成，无法回收任务！");
        //    }
        //    if (taskStatus == TaskStatus.TaskStatus_PartialFinished.ToString())
        //    {
        //        throw new WmsException(500, "该任务已部分完成，无法回收任务！");
        //    }
        //    if (taskStatus == TaskStatus.TaskStatus_Executing.ToString())
        //    {
        //        throw new WmsException(500, "该任务正在执行中，无法回收任务！");
        //    }
        //    #endregion

        //    //二楼，第一次货位分配时需要回收任务
        //    if (string.Equals(taskZoneId, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))
        //    {
        //        if (destAddrText.Length <= 3)//一次货位分配
        //        {
        //            RecycleInBoundFirstDistributionLocationTask(dtTaskInfo.Rows[0]);
        //            //修改任务软删除标记为“0”
        //            WorkTask.UpdateActStatus(model.TaskTid, "0");
        //        }
        //        else//二次货位分配
        //        {
        //            RecycleInBoundSecoundDistributionLocationTask(dtTaskInfo.Rows[0]);
        //            #region 删除任务，恢复容器明细中的物料数量
        //            RecycleClientReturnPallentTaskAndContainer(model, container_id, productPlanOutId);
        //            #endregion
        //        }
        //        return;
        //    }

        //    RecycleClientReturnPallentTaskAndContainer_20201221(model, container_id, productPlanOutId);
        //    #region 5.任务回收时，修改货位状态为空
        //    //修改货位状态为空
        //    ce.UpdateLocStatusByLocationId(locId, LocationStatus.LocationStatus_Empty.ToString(), LocationStatus.LocationStatus_In.ToString(), "机台退余料任务回收,物料无需入库,修改货位状态为空");
        //    #endregion


        //    #region 6.存在依赖任务，则回收。否则忽略此步
        //    DataTable _dtTask = ce.GetDependentTask(taskNo);
        //    //存在依赖任务
        //    if (_dtTask.Rows.Count > 0)
        //    {
        //        string _taskId = _dtTask.Rows[0]["t_id"].ToString();
        //        RecycleMachineMaterialModel rmt = new RecycleMachineMaterialModel();
        //        rmt.TaskTid = _taskId;
        //        RecycleMachineMaterial(rmt);
        //    }
        //    #endregion

        //}

        //#region 回收退余料任务时删除任务，并恢复容器明细的物料数量
        ///// <summary>
        ///// 回收退余料任务时删除任务，并恢复容器明细的物料数量
        ///// </summary>
        ///// <param name="model"></param>
        ///// <param name="container_id"></param>
        ///// <param name="productPlanOutId"></param>
        //private void RecycleClientReturnPallentTaskAndContainer_20201221(RecycleClientReturnPalletModel model, string container_id, string productPlanOutId)
        //{
        //    #region 2.获取任务物料明细
        //    //获取任务物料明细
        //    DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
        //    #endregion

        //    #region 3.获取要料或者接班任务，并恢复退余料任务产生前容器中的物料数量
        //    //获取要料或者接班任务
        //    DataTable theNewTaskItemInfo = ce.GetTheNewTaskInfoByPlanOutId(productPlanOutId);

        //    //根据要料或者接班任务，恢复退余料任务产生前容器中的物料数量
        //    for (int i = 0; i < theNewTaskItemInfo.Rows.Count; i++)
        //    {
        //        string cdTid = theNewTaskItemInfo.Rows[i]["container_d_id"].ToString();
        //        decimal qty = Convert.ToDecimal(theNewTaskItemInfo.Rows[i]["plan_qty"].ToString().ToString() == "" ? "0" : theNewTaskItemInfo.Rows[i]["plan_qty"].ToString());
        //        Container.UpdateDetailQty(cdTid, qty);
        //    }
        //    //修改容器主档允许被呼叫
        //    ce.UpdateContainerAllowCalled(container_id, "1");
        //    #endregion

        //    #region 4.更新任务和任务明细软删除标记为“0”
        //    //修改任务软删除标记为“0”
        //    WorkTask.UpdateActStatus(model.TaskTid, "0");
        //    //更新任务物料软删除标记为“0”
        //    for (int i = 0; i < taskItemInfos.Length; i++)
        //    {
        //        string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
        //        string containerDTid = taskItemInfos[i]["container_d_id"].ToString();//容器明细id
        //                                                                             //修改任务物料表中的软删除标记为“0”
        //        WorkTask.UpdateTaskItemActStatus(cdTid, "0");
        //    }
        //    #endregion
        //}
        //#endregion

        //#endregion

        //#region 退余料任务部分完成
        ///// <summary>
        ///// 退余料任务部分完成
        ///// </summary>
        ///// <param name="model"></param>
        //public void PartionFinishClientReturnPallet_20201221(PartionFinishClientReturnPalletModel model)
        //{
        //    #region 1.获取退余料任务信息
        //    //获取退余料任务信息
        //    DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
        //    if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
        //    {
        //        throw new WmsException(500, "未查询到站台退余料任务！");
        //    }
        //    string taskNo = dtTaskInfo.Rows[0]["task_no"].ToString();//任务号
        //    string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
        //    string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
        //    string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
        //    string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
        //    string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退余料时托盘目的库位id
        //    string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
        //    string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
        //    if (refDocType != DocType.DocType_ClientReturnPallet.ToString())
        //    {
        //        throw new WmsException(500, "该任务不是退余料任务，无法部分完成！");
        //    }
        //    if (taskStatus != TaskStatus.TaskStatus_Executing.ToString())
        //    {
        //        throw new WmsException(500, "任务仅在处于执行中状态下，才能部分完成！");
        //    }
        //    #endregion

        //    #region 7.将任务修改为部分完成
        //    WorkTask.UpdateTaskStatus(model.TaskTid, TaskStatus.TaskStatus_PartialFinished.ToString(), TaskStatus.TaskStatus_Executing.ToString());
        //    #endregion

        //    #region 2.存在依赖任务，则修改是否可执行状态，并下发任务。否则忽略此步
        //    DataTable _dtTask = ce.GetDependentTask(taskNo);
        //    //存在依赖任务
        //    if (_dtTask.Rows.Count > 0)
        //    {
        //        string _taskId = _dtTask.Rows[0]["t_id"].ToString();
        //        ce.UpdateTaskIsExecutableStatus(_taskId);
        //        //发送给wcs
        //        SendToWCS(_taskId);
        //        return;
        //    }
        //    #endregion

        //    #region 3.检查站台状态
        //    //检查站台状态
        //    DataRow dtDockInfo = ce.GetDockStatus(dockId);
        //    string dock_status = dtDockInfo["dock_status"].ToString();//使用设备表中的站台状态
        //    string machineDockTid = dtDockInfo["t_id"].ToString();//机台站台表中的t_id
        //    string sectionStatus = dtDockInfo["sectionStatus"].ToString();//工艺段状态
        //    //if (dock_status == EquipmentStatus.EquipmentStatus_Error.ToString())
        //    //{
        //    //    throw new WmsException(500, "当前站台位处于故障状态，无法使用！");
        //    //}
        //    //if (sectionStatus == SectionStatus.SectionStatus_Unuse.ToString())
        //    //{
        //    //    throw new WmsException(500, "当前站台对应的工艺段已停用，无法进行退余料！");
        //    //}
        //    #endregion

        //    #region 4.获取挂在站台上的容器明细
        //    //获取挂在站台上的容器明细
        //    DataRow[] dtrContainerDInfo = Container.GetDetailByHeadID(container_id);
        //    #endregion

        //    #region 5.修改容器主档是否允许呼叫
        //    //修改容器主档是否允许呼叫
        //    //Container.UpdateHeadIsAllowCalled(container_id, WmsConfig.GetOrDefault("CallEnd:IsAllowedCall", "0"));
        //    #endregion

        //    //容器关联货位
        //    Container.UpdateContainerLocation(container_id, locId);

        //    //恢复已删除的容器主档为1
        //    Container.UpdateHeadActStatus(container_id, "1");

        //    #region 5.将容器明细物料数量为0，则软删除容器明细
        //    //容器明细中物料数量qty全为0的情况，已在前台界面限制，走退空托盘业务
        //    //该目的是在统计消耗时直接使用出库数量减去容器明细中的数量，作为消耗量
        //    for (int i = 0; i < dtrContainerDInfo.Length; i++)
        //    {
        //        string containerDetailId = dtrContainerDInfo[i]["t_id"].ToString();
        //        decimal qty = Convert.ToDecimal(dtrContainerDInfo[i]["qty"].ToString() == "" ? "0" : dtrContainerDInfo[i]["qty"].ToString());
        //        if (qty <= 0)//物料数量为0，则删除容器明细，设置成软删除
        //        {
        //            Container.UpdateDetailActStatus(containerDetailId, "0");
        //        }
        //    }
        //    #endregion

        //    #region 6.修改站台状态
        //    //修改站台状态
        //    ce.UpdateDockStatus(machineDockTid, dockId, EquipmentStatus.EquipmentStatus_OK.ToString());
        //    #endregion

        //    #region 8.清空挂在站台上的容器
        //    //清空挂在站台上的容器
        //    ce.UpdateMachineDockDeatilId(machineDockTid, string.Empty, string.Empty, string.Empty, string.Empty);
        //    #endregion
        //}
        //#endregion

        //#region 退余料任务完成
        ///// <summary>
        ///// 退余料任务完成
        ///// </summary>
        ///// <param name="model"></param>
        //public void FinishReturnPalletTask_20201221(FinishReturnPalletTaskModel model)
        //{
        //    #region 1.获取退余料任务信息
        //    //获取退余料任务信息
        //    DataTable dtTaskInfo = ce.GeTaskInfoByTaskId(model.TaskTid);
        //    if (dtTaskInfo == null || dtTaskInfo.Rows.Count <= 0)
        //    {
        //        throw new WmsException(500, "未查询到站台退余料任务！");
        //    }
        //    string taskNo = dtTaskInfo.Rows[0]["task_no"].ToString();//任务号
        //    string zone_id = dtTaskInfo.Rows[0]["zone_id"].ToString();//区域id
        //    string container_id = dtTaskInfo.Rows[0]["container_id"].ToString();//任务中，容器主档id
        //    string job_no = dtTaskInfo.Rows[0]["ref_doc_no"].ToString();//生产工单单号
        //    string productPlanOutId = dtTaskInfo.Rows[0]["ref_doc_id"].ToString();//生产工单id
        //    string dockId = dtTaskInfo.Rows[0]["src_addr_id"].ToString();//站台id,即设备表的t_id 
        //    string locId = dtTaskInfo.Rows[0]["dest_addr_id"].ToString();//退余料时托盘目的库位id
        //    string refDocType = dtTaskInfo.Rows[0]["ref_doc_type"].ToString();//任务单据类型
        //    string taskStatus = dtTaskInfo.Rows[0]["task_status"].ToString();//任务状态
        //    if (refDocType != DocType.DocType_ClientReturnPallet.ToString())
        //    {
        //        throw new WmsException(500, "该任务不是退余料任务，无法完成！");
        //    }
        //    if (taskStatus != TaskStatus.TaskStatus_PartialFinished.ToString())
        //    {
        //        throw new WmsException(500, "任务仅在处于已部分完成状态下，才能完成！");
        //    }
        //    #endregion

        //    #region 2.获取任务物料明细
        //    //获取任务物料明细
        //    DataRow[] taskItemInfos = WorkTask.GetTaskItemsBy(model.TaskTid);
        //    #endregion

        //    #region 3.将容器主档与库位关联
        //    //将容器主档与库位关联
        //    Container.UpdateContainerLocation(container_id, locId);
        //    Container.UpdateContainerIsFull(container_id, "0");
        //    #endregion

        //    #region 4.更新退余料任务和任务明细
        //    //将任务更新为完成
        //    WorkTask.UpdateTaskComplete(model.TaskTid);
        //    //更新任务物料明细中的已处理数量和完成数量
        //    for (int i = 0; i < taskItemInfos.Length; i++)
        //    {
        //        string cdTid = taskItemInfos[i]["t_id"].ToString();//任务物料id
        //        //根据任务物料id，修改任务物料状态为已完成，并且修改完成和处理数量
        //        WorkTask.UpdateTaskItemCompleteAndQty(cdTid);
        //    }
        //    WorkTask.MoveToHistory(model.TaskTid);
        //    #endregion

        //    #region 5.修改货位状态为已存储
        //    //修改货位状态为已存储
        //    ce.UpdateLocStatusByLocationId(locId, LocationStatus.LocationStatus_Stored.ToString(), LocationStatus.LocationStatus_In.ToString(), "机台退余料,物料入库完成,修改货位状态为已存储");
        //    #endregion

        //    if (string.Equals(zone_id, WmsConfig.GetOrDefault("Outbound:OutboundZone", "1293844199577358336")))
        //    {
        //        Location lo = new Location();
        //        LocationModel _locInfo = lo.GetLocationById(locId);
        //        bool isLineFull = ce.IsLineFull(_locInfo.location_row.ToString());
        //        if (isLineFull == true)
        //        {
        //            //lo.UpdateLocationStatus(_locInfo.t_id, LocationStatus.LocationStatus_Stored);
        //            lo.UpdateLocationFlag(_locInfo.location_row.ToString(), LocationFlag.LocationFlag_Usable.ToString(), LocationFlag.LocationFlag_InOnly.ToString());
        //        }
        //    }
        //    #region 6.判断退余料任务完成时，依赖该任务的要料任务，是否允许完成如果允许，则完成要料任务。否则忽略此步
        //    DataTable _dtTask = ce.GetDependentTask(taskNo);
        //    //存在依赖任务
        //    if (_dtTask.Rows.Count > 0 && string.Equals(_dtTask.Rows[0]["LED_TEXT"].ToString(), "55555555"))
        //    {

        //        string _taskId = _dtTask.Rows[0]["t_id"].ToString();
        //        FinishMaterialModel rmt = new FinishMaterialModel();
        //        rmt.TaskTid = _taskId;
        //        FinishMaterial(rmt);
        //    }
        //    #endregion
        //}
        //#endregion

        //#endregion

    }
}
