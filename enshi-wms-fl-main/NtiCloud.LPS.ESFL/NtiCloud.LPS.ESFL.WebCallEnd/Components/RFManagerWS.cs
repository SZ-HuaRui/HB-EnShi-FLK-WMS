using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using System.Collections;
using NTI.WMS.BR.RFManager;

namespace WMSInterface
{
    //作者：YYL
    //编写日期：2011-10-11
    //版本：V5.0

    [System.ServiceModel.ServiceContract]
    public interface RFManagerWS : _KeepAlive
    {
        [System.ServiceModel.OperationContract()] //获取仓库名称 2011-10-11 YYL
        DataSet GetWarehouse();

        [System.ServiceModel.OperationContract()] //获取设备名称 2011-10-11 YYL
        DataSet GetEquipment();

        [System.ServiceModel.OperationContract]//检查用户登录 2011-10-13 YYL
        bool CheckLonginUser(string WareHouseID, string EquipmentNumber, string UserName, string UserPassword);

        #region 备件库
        [System.ServiceModel.OperationContract()]
        DataSet GetBJReceivingHead(string WarehouseID, string ItemCode); //【备件库】根据物料编码查找收货单 2011-10-28 YYL

        [System.ServiceModel.OperationContract()]  //【备件库】入库工作流 2011-11-07 YYL
        WorkFlowResult GradeReceiptWrokFlow(string Equipment, string WarehouseID, string userID, DataTable Mydt, string PalletCode);

        [System.ServiceModel.OperationContract()] // 【备件库】地面区,平库区，货柜区 入库 2011-11-24 YYL
        WorkFlowResult DMQInWarehouse(string Equipment, string WarehouseID, string userID, DataTable Mydt, string PalletCode, string LocationNumber, string TypeZone);

        [System.ServiceModel.OperationContract()]//【备件库】根据物料编码找出存储的货位信息 2012-02-13 YYL
        DataSet GetLocationItem(string WarehouseID, string Code);

        [System.ServiceModel.OperationContract()] //【备件库】获取盘点单号 2012-01-17 YYL
        DataSet GetCountHead(string WarehouseID);

        [System.ServiceModel.OperationContract()]//【备件库】根据盘点单号找盘点物料明细 2012-01-17 YYL
        DataSet GetCountDetali(string WarehouseID, string CountNumber);

        [System.ServiceModel.OperationContract()]//【备件库】修改盘点明细及其库存数量 2012-02-01 YYL
        int UpdateInvDetail(string WarehouseID, string Containerid, string Containerdetailid, string itemid, Decimal Qty);

        [System.ServiceModel.OperationContract()]//【备件库】修改盘点单状态 2012-02-10 YYL
        int EditCountHeadState(string WarehouseID, string CountHeadID);

        [System.ServiceModel.OperationContract()] //【备件库】 查找货位号是否存在 2012-02-02 YYL         
        DataSet GetBJLocationNumber(string WarehouseID, string ZoneID, string LocationNumber);

        [System.ServiceModel.OperationContract()] //【备件库】 查找货位上的托盘条码 2012-02-01 YYL
        DataSet GetPalletLumber(string WarehouseID, string zoneid, string Locationnumber);

        [System.ServiceModel.OperationContract()]//【备件库】获取出库任务 2012-02-07 YYL
        DataSet GetTask(string WarehouseID, string OrderNumber);

        [System.ServiceModel.OperationContract()]//【备件库】获取出库订单 2012-02-07 YYL
        DataSet GetOrderNumber(string WarehouseID);

        [System.ServiceModel.OperationContract()]//【备件库】完成出库任务 2012-02-07 YYL
        int OutTaskFinished(string WarehouseID, DataTable Dt);

        #endregion

        #region 辅料库
        [System.ServiceModel.OperationContract()]//获取配盘单生产牌号 2011-10-17 YYL
        DataSet GetPeipanItem(string WarehouseID);

        [System.ServiceModel.OperationContract()] //获取配盘单号 2011-10-17 YYL
        DataSet GetDatasetBillofmaterielhead(string WarehouseID, string ItemID);

        [System.ServiceModel.OperationContract()] //根据配盘单号找配盘信息 2011-10-17 YYL
        DataSet GetDatasetDetailed(string WarehouseID, string BomID);

        [System.ServiceModel.OperationContract()] //查找配盘中每种物料是否在暂存区 2011-10-18 YYL
        DataSet GetContainer(string WarehouseID, string itemid);

        [System.ServiceModel.OperationContract()]  //配盘工作流 2011-10-19 YYL
        WorkFlowResult AssemblyWrokFlow(string Equipment, string WarehouseID, string Bomid, string Bomcode, string PalletCode, string userID, DataTable Tab, string modecode, int flagnumber);

        [System.ServiceModel.OperationContract()] //检查托盘条码是否存在  2011-10-19 YYL
        DataSet Check_Pallet(string Warehouseid, string PalletCode);

        [System.ServiceModel.OperationContract()] //空托盘组入库工作流 2011-11-07 YYL
        WorkFlowResult InputPalletWrokFlow(string Equipment, string WarehouseID, string ItemID, string ItemName, string PalletCode, string userID, Decimal qty);

        [System.ServiceModel.OperationContract()] //获取空托盘入库相关信息 2011-11-08 YYL
        DataSet GetEmptyPalletIn(string Wrehouseid);

        [System.ServiceModel.OperationContract()] //获取手工发料机台站台 2011-11-09 YYL
        DataSet GetSenditemStation(string Warehouseid);

        [System.ServiceModel.OperationContract()] //根据机组站台找手工发料数据 2011-11-09 YYL  
        DataSet GetSenItemSource(string Warehouseid, string address);

        [System.ServiceModel.OperationContract()]  //手工发料工作流 2011-11-10 YYL
        WorkFlowResult WrokFlowHandSendItem(string Equipment, string WarehouseID, string userID, string PalletCode, string address, string tasknumber, string bomcode, string bomid, string ItemId, string ItemName, Decimal ItemQty, string SType);

        [System.ServiceModel.OperationContract()]//获取空托盘出库站台 2012-01-06 YYL
        DataSet GetCallPalletEquipment(string WarehouseID);

        [System.ServiceModel.OperationContract()]//呼叫空托盘出库 工作流2012-01-06 YYL
        WorkFlowResult CallPalletCode(string Equipment, string WarehouseID, string Address, int Number, string Userid);

        [System.ServiceModel.OperationContract()] //检查空托盘组有几条任务[每次在任务表中只能有8条空托盘出库任务] 2012-01-06 YYL 
        DataSet getPalletcount(string warehouseid, string addressid);

        [System.ServiceModel.OperationContract()] //根据配盘单号查找散盘数量 2012-04-01 YYL
        DataSet GetFullPallet(string Warehouseid, string Bomid);

        [System.ServiceModel.OperationContract()] //查找丝束入库口类型 2012-04-01 YYL
        DataSet GetSenItemType(string WarehouseID);

        [System.ServiceModel.OperationContract()]
        DataSet GetItemShipper(string WarehouseID, string ItemID);

        [System.ServiceModel.OperationContract()]
        DataTable GetItemShipperInNormal(string warehouseID, string itemId);

        [System.ServiceModel.OperationContract()]
        DataTable GetItemLotNumber(string warehouseId, string itemId);

        [System.ServiceModel.OperationContract()]
        DataTable GetItemLotNumberByShipper(string warehouseId, string itemId, string shipperId);

        [System.ServiceModel.OperationContract()]
        DataSet QueryRelationByItemId(string WarehouseId, string ItemId);

        [System.ServiceModel.OperationContract()]
        DataTable GetPalletInfo(string warehouseId, string palletCode);

        [System.ServiceModel.OperationContract()]
        bool ConfirmPallet(string warehouseId, string taskNumber);
        #endregion

        [System.ServiceModel.OperationContract()]
        DataTable GetItemLotNumberQty(string warehouseId, string itemId, string lotNumber);

        [System.ServiceModel.OperationContract()]
        DataTable GetItemLotNumberShipperQty(string warehouseId, string itemId, string lotNumber, string shipperId);

        [System.ServiceModel.OperationContract()]
        DataTable GetItemShipperRate(string warehouseId, string itemId, string shipperId);

        // #region  备件库
        // [System.ServiceModel.OperationContract()] //获取仓库
        // DataSet GetWarehouse();


        // [System.ServiceModel.OperationContract()]  //获取收货单
        // DataSet GetReceiving(string WarehouseID);



        // #endregion


        // #region 辅料库

        // #region 收货

        [System.ServiceModel.OperationContract()] //获取收货单号
        DataSet GetDatasetReceiving(string WarehouseID, string ReceivingNumber);

        // [System.ServiceModel.OperationContract()]  //收货工作流
        //// WorkFlowResult ReceivingWorkFlow(string Equipment, string WarehouseID, string ReceivingID, string ReceivingNumber, string UserID,ContainerEntity container);// , int Count
        // WorkFlowResult Receiving_WorkFlow(string Equipment, string WarehouseID, string ReceivingID, string ReceivingNumber, string UserID, ContainerEntity container);
        // #endregion



        [System.ServiceModel.OperationContract()]//修改收货単状态
        int UpdateReceivinghead(string Warehouseid, string ReceivingNumber);

        [System.ServiceModel.OperationContract()] //获取所有的暂存区收获单号
        DataSet ZC_GetAllReceivingNumber(string Warehouseid);

        // #region 配盘



        [System.ServiceModel.OperationContract()]
        DataSet GetDataset_Billofmaterielhead(string WarehouseID, string Bomid);//根据牌号找牌号对应的编码

        [System.ServiceModel.OperationContract()]//根据收货单获取物料
        DataSet GetDataSetItem(string WarehouseID, string ReceivingNumber);






        [System.ServiceModel.OperationContract()]//配盘物料百分比
        DataSet GetBillofmaterieldetailitem(string warehouseID, string Bomid);

        [System.ServiceModel.OperationContract()] //组盘获取暂存区物料详细
        DataSet Get_Item_Container(string warehouseid, string Itemid);


        [System.ServiceModel.OperationContract()]  //检查物料批次号
        DataSet GetLotnumber(string warehouseID, string lotnumber);

        [System.ServiceModel.OperationContract()] //检索物料批次号
        DataSet get_searche(string warehouseid, string itemid);

        // [System.ServiceModel.OperationContract()]   //加载自分配的结果  peq 2011/7/31
        // DataSet AutoDistributeLotInventry(string itemID, decimal qty, DataSet dsContaienrDetails,string filed1, string field2);

        // [System.ServiceModel.OperationContract()]   //加载自分配的结果  peq 2011/7/31
        // DataSet LoadDistributeLotInventry(string billid, string typecode);

        // [System.ServiceModel.OperationContract()]   //加载自分配的结果  peq 2011/7/31
        // int DeleteDistributeLotInventry(string billid, string typecode);


        // #endregion

        // #region 呼叫空托盘
        // [System.ServiceModel.OperationContract()] //获取区域
        // DataSet GetZone(string WarehouseID);



        //         //检查空托盘是否存在
        // [System.ServiceModel.OperationContract()]
        // DataSet GetCheckPalletCode(string warehouseid, string Palletcode);



        [System.ServiceModel.OperationContract()] //收货单物料批次号
        DataSet GetDatasetlotnumber(string Itemid, string Warehouseid);

        // [System.ServiceModel.OperationContract()]
        // DataSet get_ADequpment(string warehouseid, string des);
        // #endregion

        // #region 空托盘入库
        // [System.ServiceModel.OperationContract()]
        // int ReturnPallet(string WarehouseID, string UserId, string PalletCode, string Equipment);

        // [System.ServiceModel.OperationContract()] //空托盘入库工作流
        // WorkFlowResult InputPalletWrokFlow(string Equipment, string WarehouseID, string ItemID, string ItemName, string PalletCode, string userID, decimal qty);



        // #endregion 

        // #region 组盘入库（高架库）

        // [System.ServiceModel.OperationContract()] //通过物料获取详细信息
        // DataSet GetGradeReceipt(string WarehouseID, string ItemID, string ReceivingID);

        [System.ServiceModel.OperationContract()]
        DataSet GetGradeReceiptNumber(string Warehouseid);//获取所有的收货单号

        [System.ServiceModel.OperationContract()]//根据物料编码找暂存区物料(辅料库)
        DataSet Get_FL_item(string Warehouseid, string Code);

        // //[System.ServiceModel.OperationContract()] //检查物料库存是否为0；
        // //DataSet GetReceivingDeatil(string warehouseid, string receivingid);


        // //[System.ServiceModel.OperationContract()] //将Task表中的收货单数据插入到finishedtask中
        // //int FinishedTask(string ReceivingNumber, string WarehouseID);

        // //[System.ServiceModel.OperationContract()] //修改finishedtask 执行状态
        // //int UpdateFinishedTask(string ReceivingNumber, string Warehouseid);

        // //[System.ServiceModel.OperationContract()] //删除task 表中的记录
        // //int DeltetdTask(string ReceivingNumber, string Warehouseid);
        // #endregion

        // #region 余料回库
        [System.ServiceModel.OperationContract()]
        DataSet GetReturnStuff(string WarehouseID, string ItemID);

        [System.ServiceModel.OperationContract()]  //获取暂存区物料
        DataSet GetDataItem(string WarehouseID);

        // //退余料接口添加生产日期、质检状态  PEQ 20110322
        [System.ServiceModel.OperationContract()]  //退余料工作流
        WorkFlowResult ReturnStuffWorkFlow(string Equipment, string WarehouseID, string Itemid, string ItemName, string Qty, string Palletcode, string Rate, string userid, string lotnumber, IList list, string ProductDate, string CheckState);

        [System.ServiceModel.OperationContract()]//获取高架库区物料（成品库）
        DataSet Get_P_DataItem(string WarehouseID);

        [System.ServiceModel.OperationContract()] //根据物料ID找物料
        DataSet Get_PItem(string WarehouseID, string ItemID);

        [System.ServiceModel.OperationContract()]//获取FinishedTask
        DataSet GetFinishedTask(string WareshouseID, string Containerbarcode);

        // //成品库RF不扫原来托盘 PEQ 20110322
        [System.ServiceModel.OperationContract()]
        DataSet SelectReturnSuffer(string WarehouseID, string ItemID);
        // //绑定成品余料回库的牌号 PEQ 20110322
        [System.ServiceModel.OperationContract()]
        DataSet GetReturnSufferItemName(string WarehouseID);


        // #endregion

        // #region 手工发料





        [System.ServiceModel.OperationContract()] //根据目的判断是否是配盘发料还是手工发料
        DataSet Send_Item(string Warehouseid, string Address);


        // #endregion

        // #region 越库

        [System.ServiceModel.OperationContract()] //获取所有的越库单号
        DataSet GetAllCrossNumber(string warehouseid);

        [System.ServiceModel.OperationContract()]        //根据越库单查找越库物料
        DataSet GetCrossItem(string Warehouseid, string CrossNumber);


        [System.ServiceModel.OperationContract()]        //越库工作流
        WorkFlowResult CrossWorkFlow(string Equipment, string WarehouseID, string userid, string CrossId, string CrossNumber, string itemid, string itemname, string address, string palletcode, decimal strcount, int I_number, bool _strflag);

        [System.ServiceModel.OperationContract()]  //根据托盘条码找物料(完成越库)
        DataSet GetFinishedCrossDetail(string warehouseid, string Containerbarcode);

        [System.ServiceModel.OperationContract()] //完成越库任务
        //int FinishedCross(string Warehouseid, string Tasknumber,decimal itemcount,string itemid);
        WorkFlowResult FinishedCross(string Equipment, string userid, string Warehouseid, string Tasknumber, decimal itemcount, string itemid);
        //#endregion

        [System.ServiceModel.OperationContract()]
        DataSet Getdetail(string warehouseid, string crossnumber, string itemid); //根据物料名称找详细


        // #region 交接班

        // [System.ServiceModel.OperationContract()]//查询交班信息默认信息
        // DataSet Get_ClassGroup(string WarehouseID, string WorkShop, string TeamGroup);

        // [System.ServiceModel.OperationContract()]//查询机台未交班的信息
        // DataSet GetHisTransferInfo(string WarehouseID, string WorkShop, string strMachinNumber);

        // [System.ServiceModel.OperationContract()]        // 插入交接班默认数据 
        // int InsertClassGroup(string Warehouseid, string RunID, string WorkShop, string TeamGroup, string ClassState,string MachineID,string strRemark);

        // [System.ServiceModel.OperationContract()]        // 插入交接班默认数据 
        // int BeforeAskItems(string Warehouseid, string RunID, string WorkShop, string TeamGroup, string MachineID, string strRemark,string strTime);

        // [System.ServiceModel.OperationContract()] //插入机台交班信息数据
        // int InsertClassMachineNumber(string WarehouseID, string RunID, string WorkShop, string MachineNumber, string TeamGroup, string ClassState);

        // [System.ServiceModel.OperationContract()]  //查询每个机台的班次编号
        // DataSet GetMachineShiftcode(string WarehouseID, string WorkShop, string MachineNumber, string ClassState,string teamGroup);

        // [System.ServiceModel.OperationContract()]
        // DataSet GetMachineRunId(string strWarehouseId, string strWorkShop, string strMachineNumber, string strClassState);

        // [System.ServiceModel.OperationContract()]  //查询机台的交接班历史信息
        // DataSet GetMachineHistoryId(string strWarehouseId,string strWorkShop,string strMachinNumber);

        // [System.ServiceModel.OperationContract()]  //查询机台的交接班历史信息
        // DataSet GetStateByClassId(string warehouseid, string WorkShop, string strClassId);

        // [System.ServiceModel.OperationContract()]  //查询车间的班次编号
        // DataSet GetWorkShopShiftcode(string WarehouseID, string WorkShop,string TeamGroup);

        // [System.ServiceModel.OperationContract()]  //查询班组编号
        // DataSet GetChangeTeamNumber(string strWarehouseId, string strMachineNumber, string strWorkShop, string strRunId);

        // [System.ServiceModel.OperationContract()] //修改机台交班状态
        // int UpdateClassMachineNumber(string WarehouseID,  string WorkShop, string MachineNumber, string TeamGroup, string ClassState,string strRemark,string strRemark2);

        // [System.ServiceModel.OperationContract()] //修改机台交班状态
        // int UpdateAllMachineState(string WarehouseID, string WorkShop, string TeamGroup, string ClassState);

        // [System.ServiceModel.OperationContract()] //查询交班中，已交班的记录
        // DataSet GetState(string Warehouseid, string MachineNumber, string Workshop, string TeamDate);

        // [System.ServiceModel.OperationContract()] //查询交班中，已交班的记录
        // DataSet GetOnlineState(string Warehouseid, string MachineNumber, string Workshop, string TeamDate);

        // [System.ServiceModel.OperationContract()] //插入日志
        // DataSet WriteLog(string EquipmentNumber, string UserName, string WareHouseID, int result, string Message, string Messages, string Errormessage);

        // #endregion


        // [System.ServiceModel.OperationContract()] //查询机台是否存在AGV
        // DataSet SelMachineDockState(string WarehouseID,string WorkShopID,string MACHINENUMBER);

        // [System.ServiceModel.OperationContract()]  // 插入记录Agv
        // int InsertMachineDockState(string WarehouseID,string WorkShopID,string MACHINENUMBER,string HvaeAGV,string Field1,string Field2,string Field3,string Field4,string Field5);

        // [System.ServiceModel.OperationContract()]   //修改记录Agv
        // int UpdateMachineDockState(string WarehouseID, string WorkShopID, string MACHINENUMBER, string HvaeAGV);



        // #endregion

        #region 成品库
        //根据收货单找物料
        [System.ServiceModel.OperationContract()]
        DataSet Get_P_ReceivingDetail(string Warehouseid, string Receivingnumber);

        [System.ServiceModel.OperationContract()]
        DataSet get_P_Allnumber(string warehouseid);

        [System.ServiceModel.OperationContract()]
        DataSet Get_P_Source(string Warehouseid, string receivingID, string Itemid);

        [System.ServiceModel.OperationContract()]
        DataSet Get_P_Item(string WarehousID, string exteriorcode);

        [System.ServiceModel.OperationContract()]//检查一号工程码是否属于这种物料
        DataSet SeleceCode(string Warehouseid, string exteriorcode, string itemid);

        //配盘工作流
        [System.ServiceModel.OperationContract()]
        WorkFlowResult FP_AssemblyWrokFlow(string Equipment, string WarehouseID, string Count, string ItemId, string ItemName, string Rate, string PalletCode, string UserID, IList list, int I_number);

        //收货工作流  添加质检状态   PEQ 20110321
        [System.ServiceModel.OperationContract()]
        WorkFlowResult FP_RFReceiptWrokFlow(string Equipment, string ReceivingID, string ReceivingNumber, string ItemId, string itemName, string WarehouseID, string Count, string PalletCode, string Rate, string UserID, IList list, int i_number, string CheckState);

        //波次拣选
        [System.ServiceModel.OperationContract()]
        //DataSet SelectPicking(string Warehouseid);
        DataSet SelectPicking(string Warehouseid, string addresstext);

        //查找执行中的拣选任务
        [System.ServiceModel.OperationContract()]
        DataSet SelectPickExecuting(string Warehouseid, string Userid);

        //根据目的查找有没有执行中的拣选任务
        [System.ServiceModel.OperationContract()]
        DataSet SelectPick_Executing(string Warehouseid, string Addresstext);

        // 查找订单号
        [System.ServiceModel.OperationContract()]
        DataSet selectOrder(string Warehouseid, string TaskNumber);

        //波次拣选工作流
        [System.ServiceModel.OperationContract()]
        WorkFlowResult PickingWrokFlow(string Equipment, string TaskNumber, string WarehouseID, string UserID, string address, IList list, string CommonId, string CommonDetailid);

        //修改波次任务为执行中
        [System.ServiceModel.OperationContract()]
        int UpdatePick(string warehouseid, string Tasknumber, string Userid);

        //获取目的地
        [System.ServiceModel.OperationContract()]
        DataSet GetAddress(string Warehouseid);

        //修改波次任务为未执行
        [System.ServiceModel.OperationContract()]
        int UpdatePickState(string warehouseid, string Tasknumber, string address);

        //修改波次任务为未执行
        [System.ServiceModel.OperationContract()]
        int Update_PickState(string warehouseid, string Tasknumber);

        [System.ServiceModel.OperationContract()]
        int ExcutPick(string warehouseid, string tasknumber, string[] ary);

        [System.ServiceModel.OperationContract()]///////////
        DataSet StrSelecPick(string WarehouseID, string TaskNumber, string Userid);



        #endregion

        [System.ServiceModel.OperationContract()]   //找供应商
        DataSet SelectShipperByCondition(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string sqlcondition);

        [System.ServiceModel.OperationContract()]
        new void KeepAlive();

        #region 成品库 RF异性烟入库
        [System.ServiceModel.OperationContract] //获取下一个托盘编号
        DataSet GetPalletNumber(string warehouseId);

        [System.ServiceModel.OperationContract] //查询物料是否已经扫描过
        DataSet SelectItemCode(string warehouseId, string itemCode);

        [System.ServiceModel.OperationContract] //添加记录
        bool InsertItem(string warehouseId, string itemCode, string number);

        [System.ServiceModel.OperationContract] //查询托盘所有扫描历史记录
        DataSet GetItemsByPallet(string warehouseId, string number);

        [System.ServiceModel.OperationContract] //批量删除记录
        bool DeleteItems(string warehouseId, string itemCodes);
        #endregion
    }
}
