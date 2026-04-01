using System;
using System.Collections;
using System.Text;
using System.Data;
using System.Collections.Generic;
using WMSInterface.EntityDefination;
using System.ComponentModel;
using WMSInterface.Entity50;
namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface CallEndWS : _KeepAlive
    {
        [System.ServiceModel.OperationContract()]
        new void KeepAlive();

        #region 登录初始化界面数据相关
        //登录 QWH 2009/06/08
        [System.ServiceModel.OperationContract()]
        bool WorkShop_Loging(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID);

        [System.ServiceModel.OperationContract()]
        DataTable CheckAutoLoginUser(string warehouseId, string code);

        //获取权限 QWH 2009/06/08
        [System.ServiceModel.OperationContract()]
        List<string> GetLogingUserPermission(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID);

        //获取要料策略类型数据 QWH 2009/05/26
        [System.ServiceModel.OperationContract()]
        DataSet GetTypeDataByFlag(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string Flag);


        //根据机台号获取机台站台位信息 QWH 2009/05/13
        [System.ServiceModel.OperationContract()]
        DataSet GetMachineDockInfo(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);

        //根据机台号获取机台当前存在的任务明细 QWH 2009/05/13
        [System.ServiceModel.OperationContract()]
        DataSet GetMachineCurTaskByMachineNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);

        //根据机台号获取机台当前存在的任务 
        [System.ServiceModel.OperationContract()]
        DataSet GetMachineTaskByMachineNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);
        //根据机台号获取机台当前生产计划 QWH 2009/05/14
        [System.ServiceModel.OperationContract()]
        DataSet GetMachineCurPlanByMachineNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);

      
        [System.ServiceModel.OperationContract()]
        int UpdateMachineCurPlanBySchemeid(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber,string schemeid);

        [System.ServiceModel.OperationContract()]
        int UpdateMachineCurPlanBySchemeid_accept(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string schemeid);
        [System.ServiceModel.OperationContract()]
        DataSet GetMachineCurPlanByMachineNumber1(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber,string schemeid);


        //根据车间类型获取机台数据 QWH 2009/06/06
        [System.ServiceModel.OperationContract()]
        DataSet GetMachineDataByWorkShopType(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string WorkShopType);


        //根据机台号、站台位号获取可供呼叫出库的配盘单明细 QWH 2009/07/09
        [System.ServiceModel.OperationContract()]
        DataSet GetBomDetailByMachineNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber);

        //根据机台号获取所属生产区间 HJ 2012/08/22
        [System.ServiceModel.OperationContract()]
        DataSet GetZoneByMachineNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);
        //根据机台获取itemID
        [System.ServiceModel.OperationContract()]
        DataSet GetItemid(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);
        //根据机台获取itemID
        [System.ServiceModel.OperationContract()]
        DataSet GetEquimentid(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);
      
        //根据机台号、班次获取物料消耗 HJ 2012/11/20
        [System.ServiceModel.OperationContract()]
        DataSet GetUsedMaterialsByRuns(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string StartTime, string EndTime);
        //写入机台消耗确认数据 HJ 2012/11/21
        [System.ServiceModel.OperationContract()]
        String InsertUsedMaterials(WMSInterface.UsedMaterialsHead head, List<WMSInterface.UsedMaterialsDetail> useddetails);
        //检查当前机台当前班次是否已经确认过消耗 HJ 2012/11/26
        [System.ServiceModel.OperationContract()]
        bool CheckHavedEnsure(string WarehouseID,string MachineNumber, string RunsTypeCode, string StartTime, string EndTime);
        #endregion

        #region 呼叫终端 呼、退、交接、仓库人员接收退料等操作

        #region 退废料与退残烟
        //退废料
        [System.ServiceModel.OperationContract()]
        WorkFlowResult ClientReturnWaste(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber);

        //退残烟
        [System.ServiceModel.OperationContract()]
        WorkFlowResult ClientReturnReject(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber);

        #endregion

        //根据站台编号，判断该站台位与其从属站台位的当前状态
        [System.ServiceModel.OperationContract()]
        ChildAndPersonDock CheckDockAndPersonDockData(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string DockNumber);

        //检查站台位任务数据（仅用于要料呼叫）
        [System.ServiceModel.OperationContract()]
        bool CheckDockCurTaskByDockNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string DockNumber);

        //检查所呼叫的实托盘是否库存
        [System.ServiceModel.OperationContract()]
        bool CheckContainerInventory(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber);

        //客户端退空托盘+要料操作 QWH 2009/06/23
        [System.ServiceModel.OperationContract()]
        int ClientReturnAndCallPallet(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber, string Model, string hasLGV, string Runs);

        //客户端呼叫实托盘出库 QWH 2009/05/26
        [System.ServiceModel.OperationContract()]
        WorkFlowResult ClientCallPallet(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber, string Model, string hasLGV, string Runs);

        //客户端发出退余料呼叫操作 QWH 2009/06/16
        [System.ServiceModel.OperationContract()]
        WorkFlowResult ClientReturnPallet(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber, string BomID, ContainerEntity ContainerEntity, string ReturnBillType, string HasLGV, string Destination, string Runs);

        //客户端确认接收交接班物料操作 QWH 2009/06/16
        [System.ServiceModel.OperationContract()]
        WorkFlowResult ClientConfirmReturnBill_WorkShop(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string ReturnBillID, string Runs);

        //客户端发出交接班/假退呼叫操作 QWH 2009/06/16
        [System.ServiceModel.OperationContract()]
        WorkFlowResult ClientReturnPallet_ToNext(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber, string BomID, ContainerEntity ContainerEntity, string ReturnBillType, string Destination, string Runs);

        [System.ServiceModel.OperationContract()]
        bool UpdateChangeClassItemQty(string warehouseId, string billId, DataTable dtChangeItmes, out string msg);

        //修改机台为换牌后的首次呼叫状态 QWH 2009/07/10
        [System.ServiceModel.OperationContract()]
        int UpdateMachineToFirstCallState(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);

        //客户端发出退坏料呼叫操作 QWH 2009/06/16
        [System.ServiceModel.OperationContract()]
        WorkFlowResult ClientReturnBadPallet(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber, string BomID, ContainerEntity ContainerEntity, string ReturnBillType, string HasLGV, string Destination, string Runs);

        //仓库人员确认接收退料单操作 QWH 2009/06/17
        [System.ServiceModel.OperationContract()]
        WorkFlowResult ClientConfirmReturnBill_Warehouse(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, ContainerEntity containerEntity, string returnBillID);
        
        //查询交接班信息
        [System.ServiceModel.OperationContract()]
        DataSet GetMaterialBillDetail(string WarehouseID, string MachineNumber, string StartTime, string EndTime, string Field4);
        #endregion

        #region 退料相关

        //仓库人员退回某退料单
        [System.ServiceModel.OperationContract()]
        int Return_MaterialBillByBillID(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string BillID, string PalletNumber, string ContainerID, string MachineNumber, string DockNumber);


        //获取所有未完成的退料单信息 QWH 2009/05/14
        [System.ServiceModel.OperationContract()]
        DataSet GetAllReturnMaterialBill(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID);

        //根据ContainerDetailID修改物料的退料量
        [System.ServiceModel.OperationContract()]
        int UpdateReturnItemQty(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string ContainerDetailID, decimal Qty, decimal odd1, decimal odd2, decimal odd3);

        //检查站台位状态
        [System.ServiceModel.OperationContract()]
        bool CheckDockState(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber);

        //检查站台位任务数据（仅用于退料）
        [System.ServiceModel.OperationContract()]
        bool CheckDockCurTaskByDockNumber_Return(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string DockNumber);

        //检查站台位退料单数据（仅用于退料）
        [System.ServiceModel.OperationContract()]
        bool CheckDockReturnBill_Exists(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string DockNumber, string MachineNumber);

        //获取某机台未完成的交接单信息 QWH 2009/05/14
        [System.ServiceModel.OperationContract()]
        DataSet GetMachineReturnMaterialBill(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);

        //根据机台号获取机台所有站台位的容器ContainerInfo和ContainerDetail信息 QWH 2009/05/13
        [System.ServiceModel.OperationContract()]
        DataSet GetMachineCurContainInfoByMachineNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);

        [System.ServiceModel.OperationContract()]
        DataSet GetMachineCurItemsInfoByMachineNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);
        //获取存交接班数据的站台
        [System.ServiceModel.OperationContract()]
        DataSet GetClassDockNumberByMachineNumber(string WarehouseID, string MachineNumber);
        //根据BOMID获取配盘单明细（含ITEM的允差率）QWH 2009/06/08
        [System.ServiceModel.OperationContract()]
        DataSet GetBOMDetailByBomID(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string BomID);

        //根据机台号、站台位号获取站台位当前托盘数据 QWH 2009/06/09
        [System.ServiceModel.OperationContract()]
        DataSet GetContainerDetailByDockNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber);

        [System.ServiceModel.OperationContract()]
        DataSet GetManPowerReturnDetailByDockNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber);

        [System.ServiceModel.OperationContract()]
        DataSet GetManPowerCallDetailByDockNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber);

        [System.ServiceModel.OperationContract()]//要料成功后查找该托盘上的物料 YYl
        DataSet CallPalletItem(string WareHouseID, string MachineNumber, string DockNumber, string workshopid);

        [System.ServiceModel.OperationContract()]//检查物料是否上机完成YYL
        DataSet CheckCallPalletItem(string WareHouseID, string MachineNumber, string DockNumber, string palletcode, string containerid, string itemid);

        [System.ServiceModel.OperationContract()]//根据相应条件查询已完成退料任务及明细
        DataSet CheckReturnRemnantTask(string MachineNumber, string WareHouseID, string StartTime, string EndTime);

        [System.ServiceModel.OperationContract()]//根据条件修改退余料数量
        bool ChangeReturnQty(string WarehouseID, DataTable change_DataTable);
        #endregion

        #region 退空托盘
        //机台站台位退空托盘
        [System.ServiceModel.OperationContract()]
        WorkFlowResult ClientReturnNullPallet(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber, string hasLGV);
        //根据机台号、站台位号获取当前机台信息
        [System.ServiceModel.OperationContract()]
        DataSet SelectMachineDockInfo(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber);
        #endregion

        #region 手工领、退料相关

        //创建手工领料单  PEQ   保存成品牌号
        [System.ServiceModel.OperationContract()]
        int CreateHandWorkMaterialBill(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string machineNumber, string dockNumber, ContainerEntity containerEntity, string strItemName);

        //根据暂存区ID，获取有库存的物料数据
        [System.ServiceModel.OperationContract()]
        DataSet GetItemDataByZoneID(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string ZoneID);

        //仓库人员获取未完成的手工领料单数据 QWH 2009/06/17
        [System.ServiceModel.OperationContract()]
        DataSet GetMaterialsBill_HandWork(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID);

        //车间人员获取手工领料单数据 QWH 2009/06/17
        [System.ServiceModel.OperationContract()]
        DataSet GetMaterialsBill_HandWork_Warehouse(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);

        //车间人员接收物料（手工发料） QWH 2009/06/19    新增人工选择的牌号、人工领料单信息  添加人  PEQ  2010/8/8  
        [System.ServiceModel.OperationContract()]
        WorkFlowResult Client_AcceptMaterials_HandWork(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber, string BillID, string Runs8, string strFlag, string strBillInfo);

        //仓库人员确认发料（手工领料单） QWH 2009/06/19
        [System.ServiceModel.OperationContract()]
        WorkFlowResult Client_SentMaterials_HandWork(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string BillID, ContainerEntity containerEntity);

        //仓库人员删除单据（手工领料单） QWH 2009/06/19
        [System.ServiceModel.OperationContract()]
        int Client_delete_HandWork(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string BillID, string Detailid);

        //删除(手工领料单)然后检查表，修改状态 HJ 2012/5/29
        [System.ServiceModel.OperationContract()]
        bool UpdateMachineDockStatus(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string DockNumber, string MachineNumber);

        //仓库人员获取已完成的手工领料单数据 QWH 2009/07/2   
        [System.ServiceModel.OperationContract()]
        DataSet GetFinishedMaterialsBill_HandWork(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID);
        //仓库人员获取全部的手工领料单数据 QWH 2009/07/2   
        [System.ServiceModel.OperationContract()]
        DataSet GetAllMaterialsBill_HandWork(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID);

        //仓库人员修改暂存区库存以及实际发料数量 QWH 2009/08/08   
        [System.ServiceModel.OperationContract()]
        int UpdateTempZoneAndSentItemQty(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string ContainerDetailID, decimal Qty, decimal Odd1, decimal Odd2, decimal Odd3);

        [System.ServiceModel.OperationContract()]
        int UpdateContainerDetailShipper(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string containerDetailID, string itemid, string Itemsupplier);

        //仓库人员修改自动分配量
        [System.ServiceModel.OperationContract()]
        int UpdateContainerDetailDealQty(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string BillID,string ContainerDetailID, decimal Qty);

        //修改已分配标记
        [System.ServiceModel.OperationContract()]
        int UpdateMaterialBillHead(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string BillID,  string Flag);

         //删除已分配数据并修改已分配标记
        [System.ServiceModel.OperationContract()]
        int DeleteBatchSplit(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string BillID,  string Flag);
       
        //获取批次分配明细
        [System.ServiceModel.OperationContract()]
        DataSet GetBatchSplit(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, List<string> BillIds,string StateFlag);

        //获取未处理单据按物料小计数据
        [System.ServiceModel.OperationContract()]
        DataSet GetMaterialsBillDataByItem(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, List<string> BillIds);

        //获取当前物料的已分配量
        [System.ServiceModel.OperationContract()]
        DataSet ReturnHaveDistributeQty(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string ItemId);

        //获取手工领料站台位
        [System.ServiceModel.OperationContract()]
        DataSet GetHandWorkDockNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);

        //手工领退料  添加牌号   PEQ 2010/8/5
        [System.ServiceModel.OperationContract()]
        DataSet GetItemName(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);

        //管理员创建人工领料单
        [System.ServiceModel.OperationContract()]
        string CreateManPowerCallMaterTask(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, TaskEntity taskEntity,
            Container_View containerinfo, List<ContainerDetailN> details);

        [System.ServiceModel.OperationContract()]
        bool ModifyManPowerCallMaterTask(string warehouseId, WMSInterface.Entity50.TaskEntity taskEntity, Container_View containerinfo,
            List<ContainerDetailN> details, string type, out string msg);

        //管理员修改人工领料单
        [System.ServiceModel.OperationContract()]
        int UpdateContainerDetailQty(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string ContainerDetailId, string ItemId, string SchemeId, decimal Qty);
        //确认人工发料
        [System.ServiceModel.OperationContract()]
        int ConfirmManCallTask(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string tasknumber);
        //删除人工领料
        [System.ServiceModel.OperationContract()]
        int DeleteManCallTask(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string tasknumber);

        //删除已完成人工领料
        [System.ServiceModel.OperationContract()]
        int DeleteFinishedManCallTask(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string tasknumber);

        //人工退空托盘
        [System.ServiceModel.OperationContract()]
        String CreateManPowerReturnNullPallet(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string DockNumber);
        //根据条件查询人工领料单
        [System.ServiceModel.OperationContract()]
        DataSet GetManCallByConditions(WMSInterface.Entity50.LoginInfo login, ManCallQueryConditionBase Conditions);

        //根据条件查询人工领料单明细
        [System.ServiceModel.OperationContract()]
        DataSet GetManCallDetailByConditions(WMSInterface.Entity50.LoginInfo login, ManCallQueryConditionBase Conditions);

        [System.ServiceModel.OperationContract()]
        DataTable GetDockManReturnItems(string warehouseId, string dockNumber);

        //确认人工领料操作
        [System.ServiceModel.OperationContract()]
        bool FinishManPowerCallTask(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string TaskNumber);
        
        //确认人工退料操作
        [System.ServiceModel.OperationContract()]
        bool FinishManPowerReturnTask(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string TaskNumber);

        //删除人工领料单
        [System.ServiceModel.OperationContract()]
        int DeleteManPowerCallTask(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TaskNumber);

        //确认人工领退料操作后保存任务信息
        [System.ServiceModel.OperationContract()]
        bool InsertManPowerTask(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string TaskNumber);

        #endregion

        #region 退料目的地
        //根据仓库ID、TYPECODE获取退料目的地
        [System.ServiceModel.OperationContract()]
        DataSet SelectEquipmentWithTypeCode(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string TypeCode);
        #endregion

        #region 辅料上机
        //根据辅料编号获取辅料ID
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceItemIDWithItemNumber(string WareHouseID, string ItemNumber);
        //写入辅料上机记录       
        [System.ServiceModel.OperationContract()]
        int InsertFLOnMachine(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string FLOnMachineID, string BOMID, string MachineNumber, string DockNumber, string ContainerID, string ContainerBarCode, string ItemID, string ItemBarCode, string SubItemID, decimal Qty, string UserID, DateTime CreateTime, string Field1, string Field2, string Field3, string Field4, string Field5);
        #endregion

        #region 消耗管理//HJ
        [System.ServiceModel.OperationContract()]
        DataSet GetConUsedMaterialHeadByCondition(string WareHouseID, ConfirmUsedQueryConditionBase Conditions);

        [System.ServiceModel.OperationContract()]
        DataSet GetConUsedMaterialsByHeadID(string WareHouseID, ConfirmUsedQueryConditionBase Conditions, string ConUsedMaterHeadID);

        [System.ServiceModel.OperationContract()]
        DataSet GetConUsedMaterialHeadByID(string WareHouseID, ConfirmUsedQueryConditionBase Conditions);

        [System.ServiceModel.OperationContract()]
        bool ConfirmConUsedMaterialHead(string WareHouseID, string ConusedmaterheadId);
        //修改消耗明细数量
        [System.ServiceModel.OperationContract()]
        bool UpdateConUsedDetailQty(string ConUsedMaterDetailID, string Itemid, decimal Callqty, decimal Returnqty, decimal Acceptqty, decimal Handoutqty, decimal Statisticalqty);

        [System.ServiceModel.OperationContract()]
        Result InsertConUsedMaterial(WMSInterface.EntityDefination.CONUSEDMATERHEAD head, List<WMSInterface.EntityDefination.CONUSEDMATERDETAIL> details, WMSInterface.Entity50.LoginInfo login);

        [System.ServiceModel.OperationContract()]
        Result UpdateConUsedMaterial(string ConusedmaterheadId, WMSInterface.EntityDefination.CONUSEDMATERHEAD head, List<WMSInterface.EntityDefination.CONUSEDMATERDETAIL> details, WMSInterface.Entity50.LoginInfo login);
        #endregion

        //根据machinenumber获取machinename  zxhong 2011-01-17
        [System.ServiceModel.OperationContract()]

        DataSet GetMachineInfoByMachineNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);

        //根据machineNumber获取站台的数量  peq 2011-3-16
        [System.ServiceModel.OperationContract()]
        DataSet GetDockCountMachineNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);

        //根据站台获取当前站台的物料信息
        [System.ServiceModel.OperationContract()]
        DataSet GetContainerDetailsDockNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string DockNumber);

        //插入到货确认信息
        [System.ServiceModel.OperationContract()]
        int InsertArrivalMaterilsQustion(string WarehouseId, string RunsCode, string WorkShopId, string ClassGroup, string TeamDate, string operater, string machinenumber, string docknumber, string containerbarcode, string itemcode, string itemname, string questiondescription, string containerdetailid);

        [System.ServiceModel.OperationContract()]

        DataSet GetMachineInfoByMachineNumberFL(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber);

        //根据站台信息获取当前站台的托盘物料信息  用于辅料结存  peq 
        [System.ServiceModel.OperationContract()]
        DataSet GetDockContainerDetailInfo(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string DockNumber);

        //修改批次库存分解数量  peq 9、5
        [System.ServiceModel.OperationContract()]
        int UpdateDockContainerDetailsByMahineNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string containerdetaild,decimal qty);

        //根据机台编号修改containerdetail的数量  peq 20110426
        [System.ServiceModel.OperationContract()]
        int UpdateDockContainerDetailsByDockNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string DockNumber);

        //插入分配后的数据
        [System.ServiceModel.OperationContract()]
        int InsertChangeClassBatchSplitQty(string itemName, string containerDetailId, decimal qty, string cname, string containerType,string lotnumber,string field1,string field2,string feild3);

        [System.ServiceModel.OperationContract()]
        DataSet GetUnionChangeClassInfo(string itemid,string billid);

        [System.ServiceModel.OperationContract()]
        DataSet GetTwoProductName(string warehouseid,string machinenumber);

        [System.ServiceModel.OperationContract()]
        void TrackCallEndErrors(string EquipmentNumber, string UserName, string WareHouseID, int result, string Message, string RightMessages, string Errormessage);

        //根据schemeID重置工单归集标识
        [System.ServiceModel.OperationContract()]
        int UpdateSendHistory(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string schemeID);

        [System.ServiceModel.OperationContract()]
        bool UpdateMachineProductPlan(string warehouseId, string machineNumber, string planNo, string state, out string msg);

        [System.ServiceModel.OperationContract()]
        bool updatePlan(string warehouseId, string equitmentidwd, string equitmentnumberwd, string schemeidwd, string itemidwd, string itemcodewd, string statecodewd, string memowd, string useridwd, out string msg);
          
        [System.ServiceModel.OperationContract()]
        bool updateSendTest(string warehouseId, string equitmentidwd, string equitmentnumberwd, string schemeidwd, string itemidwd, string itemcodewd, string statecodewd, string memowd, string useridwd, out string msg);

        //根据机台号获取机台旧的生产计划 wd 2015/03/03
        [System.ServiceModel.OperationContract()]
        DataSet GetMachineOldPlanByMachineNumber(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string MachineNumber, string time);
       
    
    
    }
}
