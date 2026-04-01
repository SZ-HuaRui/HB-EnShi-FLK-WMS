using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public class NTIMessage
    {
        #region License System

        public const string License_FileNotFound = "操作执行失败，无法找到授权文件！";
        public const string License_FileCannotRead = "操作执行失败，无法读取授权文件！";
        public const string License_FileError = "操作执行失败，授权文件损坏！";
        public const string License_Expired = "操作执行失败，授权文件过期！";
        public const string License_HardwareError = "操作执行失败，授权文件与当前计算机不符！";

        #endregion

        #region General
        public const string System_GeneralOKMessage = "操作执行成功！";
        public const string System_GeneralErrorMessage = "操作执行失败！";
        public const string System_GeneralErrorToGetComboboxData = "操作执行失败，系统相关数据没有被初始化！";
        public const string System_GeneralNoSelectMessage = "您没有选择需要修改的纪录！";
        public const string System_GeneralInsertSameItemMessage = "操作执行失败，纪录已存在！";
        public const string System_GeneralUpdateSameItemMessage = "操作执行失败，纪录已存在！";
        public const string System_GeneralDeleteWarning = "您确认要删除选择的纪录吗？";
        public const string System_GeneralDeleteWarning_Title = "删除";
        public const string ListFilter_NoFilter = "未启用筛选模式，您可以看到所有数据。";
        public const string ListFilter_Filtered = "筛选模式：您可以输入包含的文本（对于字符串类型），或输入数学表达式（例如>3，对于数字类型）。";
        public const string ListFilter_AllShow = "显示全部";
        public const string System_ServerError = "操作执行失败，无法访问服务器，或服务器未能返回有效信息！";
        public const string System_QuitSystem = "您确认要退出系统吗？";
        public const string System_QuitSystem_Title = "退出系统";
        public const string System_Common_ConvertError = "换算错误";
        public const string System_Common_NeedRefresh = "表单需要刷新";
        public const string System_Common_CheckedMark = " √";
        public const string System_Common_UncheckedMark = " ×";
        public const string System_Common_DefaultMark = "(默认)";
        public const string System_Common_DefaultItem = "(所有未指定)";
        public const string System_Common_NA = "不适用";
        public const string System_Common_Confirm = "您确认要执行确认操作吗？\n本操作一旦执行不可取消或修改。";

        public const string System_Common_Destination = "您没有选择目的地！\n请选择您要收货的目的地。";
        public const string System_Common_Short = "缺货";
        public const string Confirm_InsertUpdate = "您确认要添加或修改本条信息吗？";
        public const string Confirm_Insert = "您确认要添加本条信息吗？";
        public const string Confirm_Update = "您确认要修改本条信息吗？";
        public const string Confirm_Finish = "您确认要完成该计划吗？";
        public const string System_Common_NotFound = "不存在";
        public const string System_Common_NotUsed = "未使用";
        public const string System_NoBOMState = "配方单状态为停用或者未选择，不允许配盘！";
        public const string System_SelectStartBOMState = "配盘单状态只能选择为：即将启用、可呼叫出库或者不可呼叫出库！";
        public const string System_SelectUsingBOMState = "配盘单状态只能选择为：在用、可呼叫出库或者不可呼叫出库！";


        public static string System_Common_DateToString(DateTime Date)
        {
            return Date.ToString("yyyy年MM月dd日");
        }

        public static string System_Common_DateToString(string Date)
        {
            return Date.Substring(0, 4) + "年" + Date.Substring(4, 2) + "月" + Date.Substring(6, 2) + "日";
        }

        public static string System_Common_StringToDate(string Date)
        {
            return Date.Substring(0, 4) + Date.Substring(5, 2) + Date.Substring(8, 2);
        }

        public static string System_Common_PageCode(int Now, int Total)
        {
            return ("第" + Now.ToString() + "页，共" + Total.ToString() + "页");
        }

        public static string System_Common_SelectItems(int Count)
        {
            return "共选择" + Count.ToString() + "条";
        }
        #endregion

        #region Login
        public const string Login_WrongPassword = "请输入正确用户和密码！";
        public const string Login_NeedID = "请输入帐号！";
        public const string Login_AdminWrongPassword = "请输入正确管理员帐号和密码！";
        public const string Login_NeedChoiseWarehouse = "请选择操作的仓库！";
        public const string Login_WrongWarehouse = "对不起，您没有权限访问该仓库。请与管理员联系。";
        public const string Login_DelUser_Fail = "操作执行失败，当前用户正在使用， 无法删除！";
        #endregion

        #region Strategy
        public static string Strategy_Initialize_Warehouse_Delete(string warehouseName)
        {
            return "您确认要删除选定的" + warehouseName + "吗？";
        }
        public const string Strategy_Initialize_Warehouse_Delete_Title = "删除仓库";
        public const string Strategy_Initialize_Warehouse_DeleteCurrent = "删除当前登录仓库，系统将在删除动作后自动关闭。您确认要继续吗？";
        public const string Strategy_Initialize_Warehouse_CreateWithNoName = "请您输入仓库代号和名称！";
        public const string Strategy_Initialize_Warehouse_DeleteFailed = "操作执行失败，无法删除指定仓库，请检查该仓库是否仍在使用中。";
        public const string Strategy_Initialize_Warehouse_NoWarehouse = "操作执行失败，系统中没有添加仓库，请在添加仓库后再试。";

        public const string Strategy_Initialize_Dock_CreateWithNoName = "请输入月台编号！";
        public const string Strategy_Initialize_Dock_DeleteFailed = "操作执行失败，无法删除指定月台，请检查该月台是否仍在使用中！";
        public const string Strategy_Initialize_Dock_WrongSize = "您输入的数据不符合系统要求，尺寸值应该介于0与10000之间。";

        public const string Strategy_Initialize_Type_NeedDescription = "您需要写明描述信息！";
        public const string Strategy_Initialize_Type_DeleteFailed = "操作执行失败，无法删除指定类型，请检查该类型是否仍在使用中！";
        public const string Strategy_Initialize_Type_InsertUpdateFailed = "请输入完整的数据！";

        public const string Strategy_Initialize_State_NeedDescription = "您需要写明描述信息！";
        public const string Strategy_Initialize_State_DeleteFailed = "操作执行失败，无法删除指定状态，请检查该状态是否仍中！";
        public const string Strategy_Initialize_State_InsertUpdateFailed = "请输入完整的数据！";

        public const string Strategy_Initialize_Equipment_CreateWithNoFullInfo = "请输入设备编号以及说明信息！";
        public const string Strategy_Initialize_Equipment_DeleteFailed = "操作执行失败，无法删除指定设备，请检查该设备是否仍在使用中！";
        public const string Strategy_Initialize_Equipment_WrongSize = "您输入的数据不符合系统要求，尺寸值应该介于0与10000之间。";

        public const string Strategy_Initialize_EquipmentLine_DeleteFailed = "操作执行失败，无法删除指定记录。";
        public const string Strategy_Initialize_EquipmentLine_InsertFailed = "操作执行失败，无法添加该设置，请检查该是否存在相同的设置信息！";

        public const string Strategy_Initialize_Zone_CreateWithNoFullInfo = "请输入完整信息！";
        public const string Strategy_Initialize_Zone_DeleteFailed = "操作执行失败，无法删除指定区域，请检查该区域是否仍在使用中！";
        public const string Strategy_Initialize_Zone_NoSelect = "未选择";

        public const string Strategy_Initialize_ItemCatalog_CreateWithNoFullInfo = "请输入完整信息！";
        public const string Strategy_Initialize_ItemCatalog_DeleteFailed = "操作执行失败，无法删除指定货位种类，请检查该货位种类是否仍在使用中！";

        public const string Strategy_Initialize_Location_CreateWithErrorInfo = "您输入的数据不符合系统要求，尺寸值应该介于1与999之间。";
        public const string Strategy_Initialize_Location_BatchCreateWithNoItem = "操作执行失败，在您指定的区域中，无法添加货位！";
        public const string Strategy_Initialize_Location_WrongSize = "您输入的数据不符合系统要求，尺寸值应该介于0与10000之间。";
        public const string Strategy_Initialize_Location_UpdateZone = "您确认要调整选择纪录的区域吗？";

        public const string Strategy_Initialize_Section_NeedDescription = "您需要写明描述信息！";
        public const string Strategy_Initialize_Section_DeleteFailed = "操作执行失败，无法删除指定区段，请检查该区段是否仍在使用中！";
        public const string Strategy_Initialize_Section_InsertUpdateFailed = "请输入完整的数据！";




        public static string Strategy_Initialize_Location_Create(int total, int rc)
        {
            if (rc == total)
                return "所有指定的货位（" + total.ToString() + "个）添加完成。";
            else
                return "你所添加" + total.ToString() + "个货位，其中有" + rc.ToString() + "个添加成功。";
        }
        public static string Strategy_Initialize_Location_Update(int total, int rc)
        {
            if (rc == total)
                return "所有指定的货位（" + total.ToString() + "个）修改完成。";
            else
                return "您所修改" + total.ToString() + "个货位，其中有" + rc.ToString() + "个修改成功。";
        }
        public static string Strategy_Initialize_Location_Delete(int total, int rc)
        {
            if (rc == total)
                return "所有指定的货位（" + total.ToString() + "个）删除完成。";
            else
                return "您所删除" + total.ToString() + "个货位，其中有" + rc.ToString() + "个删除成功，请检查其它货位是否仍在使用中。";
        }

        public const string Strategy_Initialize_ItemPrice_WrongPrice = "您输入的数据不符合系统要求，价格应该介于0与100000之间。";
        public const string Strategy_Initialize_ItemPrice_DeleteFailed = "操作执行失败，无法删除指定价格，请检查该价格是否仍在使用中！";

        public const string Strategy_Initialize_Shipper_CreateWithNoName = "请输入名称！";
        public const string Strategy_Initialize_Shipper_DeleteFailed = "操作执行失败，无法删除指定货主信息，请检查该货主信息是否仍在使用中！";

        public const string Strategy_Initialize_UnitGroup_CreateWithNoName = "请输入名称！";
        public const string Strategy_Initialize_UnitGroup_DeleteFailed = "操作执行失败，无法删除指定组，请确保该组中没有单位信息，并检查该单位组是否仍在使用中！";
        public const string Strategy_Initialize_ItemUnit_NoFullInfo = "请输入完整信息！";
        public const string Strategy_Initialize_ItemUnit_WrongRate = "您输入的数据不符合系统要求，转换率应介于正负20亿之内，可保留最多三位小数。";
        public const string Strategy_Initialize_ItemUnit_NotZero = "转换率不能为0，如果您需要转换请设为1。";

        public const string Strategy_Initialize_Item_ChangeUnitGroup = "您选择的单位与原先不属于同一个组，如果继续，则需要人工修改相关数据值。您确认要继续吗？";
        public const string Strategy_Initialize_Item_ChangeUnitGroup_Title = "变换单位组";
        public const string Strategy_Initialize_Item_CreateWithNoName = "请输入名称及代号！";
        public const string Strategy_Initialize_Item_DeleteFailed = "操作执行失败，无法删除指定物料信息，请检查该物料是否仍在使用中！";
        public const string Strategy_Initialize_Item_SameCodeExisted = "您输入的惟一编号在本库内已经存在，请修改！";
        public const string Strategy_Initialize_Item_PalletFillError = "您输入的数据不符合系统要求,每托盘载量必须输入大于0的整数，且小于系统最大值限制（此值与该物料的单位有关）。";

        public const string Strategy_Initialize_Inventory_DataOverflow = "您输入的数据不符合系统要求,数据过大或者小于0。";
        public const string Strategy_Initialize_Inventory_LowHighError = "您输入的数据不符合系统要求,安全库存最小值不能大于最大值。";
        public const string Strategy_Initialize_Inventory_Overflow = "该修改失败。\n可能原因：输入的值过大，无法接受。";

        public const string Strategy_Initialize_BOM_WarehouseNotSpecified_Display = "未定义";
        public const string Strategy_Initialize_BOM_WarehouseNotSpecified_Text = "当前仓库未配置启动BOM表功能。";
        public const string Strategy_Initialize_BOM_DeleteFailed = "操作执行失败，无法删除指定BOM表，请检查该BOM表是否仍在使用中。";
        public const string Strategy_Initialize_BOM_RefreshDetail = "重新刷新当前BOM详单，将取消该对此详单的编辑操作，您确认要继续吗？";
        public const string Strategy_Initialize_BOM_RefreshDetail_Title = "刷新";
        public const string Strategy_Initialize_BOM_DataOverflow = "您输入的数据不符合系统要求,数据过大或者小于0。系统自动以0代替。";
        public const string Strategy_Initialize_BOM_InsertSameItemMessage = "操作执行失败,无法执行插入操作，请检查该物料是否已存在！";
        public const string Strategy_Initialize_BOM_NoSelectMessage = "请先在左侧选择需要修改的BOM表！";
        public const string Strategy_Initialize_BOM_NoSelectBOM = "请先选择左侧的BOM表！";
        public const string Strategy_Initialize_BOM_FirstLevel = "首选";
        public static string Strategy_Initialize_BOM_BackupLevel(int Level)
        {
            return "第" + Level.ToString() + "备选";
        }

        public const string Strategy_Initialize_Bill_CreateWithNoName = "请输入单据名称！";
        public const string Strategy_Initialize_Bill_DeleteFailed = "操作执行失败，无法删除指定单据，请检查该单据是否仍在使用中！";
        public static string Strategy_Initialize_Bill_Example(string Code)
        {
            return "按照您的设定，下一次产生的单据编号为：\n" + Code;
        }
        public const string Strategy_Initialize_Bill_Example_Title = "单据编号样例";

        public const string Strategy_BillFlow_DeleteFailed = "操作执行失败，无法删除指定单据流，请检查该单据流是否仍在使用中！";
        public const string Strategy_BillFlow_InsertFailed = "操作执行失败，无法插入此单据流，请检查该单据流是否已存在！";

        public const string Strategy_WorkType_InsertFailed = "操作执行失败，该操作的类型已存在。\n此设置为全局设置，会涉及至其它仓库与其它已经存在的类型。\n获得所有类型列表，请至策略初始化中的系统设置中，查看类型设置。";

        public const string Strategy_WorkFlowClass_DeleteFailed = "操作执行失败，无法删除指定作业，请检查该作业是否仍在使用中！";
        public const string Strategy_WorkFlowClass_InsertFailed = "操作执行失败，请检查该作业是否已存在。";
        public const string Strategy_WorkFlowClass_NoEnoughData = "请输入描述和类名！";
        public const string Strategy_WorkFlowHead_DeleteFailed = "操作执行失败，无法删除指定业务流，请检查该业务流是否仍在使用中！";
        public const string Strategy_WorkFlowHead_NoSelectedType = "请先选择一项工作类型！";
        public const string Strategy_WorkFlowHead_InsertFailed = "操作执行失败，请检查该操作的业务流是否已经存在重复项！";


        public const string Strategy_WorkFlowDetail_NoSelectedClass = "请先选择一项作业。";
        public const string Strategy_WorkFlowDetail_DeleteFailed = "操作执行失败，无法删除指定业务流细节，请检查该业务流细节是否仍在使用中！";
        public const string Strategy_WorkTools_Saved = "操作执行成功！";
        public const string Strategy_WorkPlace_Saved = "操作执行成功！";



        #endregion

        #region ProductionScheme
        public const string ProductionScheme_NeedSelect = "请先选择要处理的计划！";
        public const string ProductionSchemeDetail_InsertFailed = "操作执行失败，无法插入该批次。请检查该计划是否是非手工录入！";
        public const string ProductionSchemeDetail_UpdateFailed = "操作执行失败，无法修改该批次。请检查该批次是否已经被锁定、被分解，或计划非手工录！。";
        public const string ProductionSchemeDetail_DeleteFailed = "操作执行失败，无法删除该批次。请检查该计划是否是非手工录入！";
        public const string ProductionSchemeDetail_NeedSelectItem = "请选择物料！";
        public const string ProductionSchemeDetail_Overflow = "您输入的数据不符合系统要求，请重新输入！";
        public const string ProductionSchemeDetail_Qty0 = "您输入的数据不符合系统要求，数量设置应该大于0。";
        public const string ProductionSchemeHead_InsertFailed = "操作执行失败，无法添加新计划！";
        public const string ProductionSchemeHead_DeleteFailed = "操作执行失败，无法删除此计划！";
        public const string ProductionSchemeDetail_NoLotNumber = "根据系统策略设置，您必须指定一个生产批号。";
        public const string ProductionSchemeDetail_SameLotNumber = "您输入的生产批号已存在！";
        public const string ProductionSchemeDetail_Dismantled = "操作执行失败，无法修改，该计划已经完成分解！";
        public const string ProductionSchemeDetail_UnitNonSelected = "未选择";

        public const string DismantleProductionScheme_NonSelect_Step1 = "您没有选择任何任务进行分解，或者您选择的任务无法分解！";
        public const string DismantleProductionScheme_LevelName_Product = "成品";
        public const string DismantleProductionScheme_NonSelect_Step2 = "您没有选择任何任务进行分解！";
        public const string DismantleProductionScheme_Finish = "操作执行成功！。";
        public const string DismantleProductionScheme_StatusError = "操作执行失败，无法继续，任务状态错误！";
        public const string DismantleProductionScheme_OrderQty = "计划量：";

        public const string DismantleProductionScheme_Strategy_WorkName = "DismantleProductionScheme";
        public const string DismantleProductionScheme_Strategy_WorkDescription = "计划分解作业";

        public static string ProductionSchemeStateCode(int InBoundLeft, int OutBoundLeft, ref bool Finished)
        {
            if (InBoundLeft > 0)
                return "入库剩余:" + InBoundLeft.ToString();
            else if (OutBoundLeft > -InBoundLeft)
                return "出库剩余:" + (OutBoundLeft - InBoundLeft).ToString();
            else
            {
                Finished = true;
                return "已完成";
            }
        }

        public static string ProductionSchemeStateCode(int InBoundLeft, int OutBoundLeft)
        {
            if (InBoundLeft > 0)
                return "入库剩余:" + InBoundLeft.ToString();
            else if (OutBoundLeft > -InBoundLeft)
                return "出库剩余:" + (OutBoundLeft - InBoundLeft).ToString();
            else
                return "已完成";
        }

        public static string DismantleProductionSchemeStateCode(int InBoundLeft, int OutBoundLeft)
        {
            if (InBoundLeft > 0)
                return "等待入库";
            else if (OutBoundLeft > -InBoundLeft)
                return "等待出库";
            else
                return "已完成";
        }
        public const string DismantleProductionScheme_ShortInventory = "当前可用库存不足。";


        #endregion

        #region WorkFlowReturn

        public static string WorkFlow_ManyReturn_Text(int Total, int Succeeded, int Failed)
        {
            if (Total == Succeeded)
                return "成功处理" + Total.ToString() + "条。";
            else
                return "共处理" + Total.ToString() + "条（成功：" + Succeeded.ToString() + "，失败：" + Failed.ToString() + "）。";
        }

        public static string WorkFlow_ReturnCode(int Code)
        {
            switch (Code)
            {
                case 32767:
                    return "成功。";
                case -32768:
                    return "系统错误。";
                case -32767:
                    return "设置错误：业务流超过指定处理次数。";
                case -32766:
                    return "当前单据不能确认，单据未处于可确认的状态。";
                case -32765:
                    return "单据流执行错误。";
                case -1:
                    return "按照系统策略设置，收货数量必须等于计划量。";
                case -2:
                    return "按照系统策略设置，收货日期必须与计划相同。";
                case -3:
                    return "货物没有通过检验标准。";
                case -4:
                    return "货物超过最高安全库存要求。";
                case -5:
                    return "操作执行失败，价格备份失败！";
                case -6:
                    return "操作执行失败，无法修改计划数量！";
                case -7:
                    return "操作执行失败，无法修改短货单数量！";
                case -8:
                    return "操作执行失败，短货单无法结单！";
                case -9:
                    return "操作执行失败，无法修改计划状态！";
                case -10:
                    return "操作执行失败，无法修改库存量！";
                case -11:
                    return "操作执行失败，无法添加表单数据！";
                case -12:
                    return "操作执行失败，收货单无法结单！";
                case -13:
                    return "操作执行失败，无法货位分配，请检查可用货位数量以及货位设置！";
                case -14:
                    return "操作执行失败，无法获得对应行关联设备，可能是设备损坏！";
                case -15:
                    return "操作执行失败，无法修改货位状态到已分配！";
                case -16:
                    return "操作执行失败，无法获得有效的货位！";
                case -17:
                    return "操作执行失败，无法获得可用的设备！";
                case -18:
                    return "操作执行失败，无法确认有效的目的地！";
                case -19:
                    return "操作执行失败，无法锁定货位！";
                case -20:
                    return "操作执行失败，无法下架！";
                case -21:
                    return "操作执行失败，无法将数据转移至上架！";
                case -22:
                    return "操作执行失败，无法上架！";
                case -23:
                    return "操作执行失败，无法将上架数据追加到批次库存纪录表！";
                case -24:
                    return "操作执行失败，无法修改订单状态！";
                case -25:
                    return "操作执行失败，可用库存量不足！";
                case -26:
                    return "操作执行失败，无法增加库存分配量！";
                case -27:
                    return "操作执行失败，无法锁定货位！";
                case -28:
                    return "批次对应的货物未完全入库！";
                case -29:
                    return "操作执行失败，无法生成单据！";
                case -30:
                    return "操作执行失败，无法修改订单目标站台位！";
                case -31:
                    return "操作执行失败，无法开始新的班次，请检查该收货单是否仍在使用中！";
                case -32:
                    return "操作执行失败，无法开始新的班次，请检查用户是否正处于班次之中！";
                case -33:
                    return "操作执行失败，无法修改收货单至执行中状态，请检查您选择的收货单是否未执行！";
                case -34:
                    return "操作执行失败，无法修改执行中收货单！";
                case -35:
                    return "操作执行失败，无法设置用户班次信息，请检查当前时间与班次设置是否不符，请刷新后重试！";
                case -36:
                    return "操作执行失败，无法追加班次操作历史记录！";
                case -37:
                    return "操作执行失败，无法结束班次，请检查是否不存在正在执行的收货单！";
                case -38:
                    return "操作执行失败，无法结束班次，请检查是否用户未处于班次之中！";
                case -39:
                    return "操作执行失败，无法修改收货单至完成状态，请检查收货单状态！";
                case -40:
                    return "操作执行失败，无法清除用户班次信息！";
                case -41:
                    return "操作执行失败，无法插入在库质检记录！";
                case -42:
                    return "操作执行失败，无法修改质检单执行状态！";
                case -43:
                    return "操作执行失败，无法锁定货位为质检中！";
                case -44:
                    return "操作执行失败，无法增加库存分配量！";
                case -45:
                    return "操作执行失败，无法添加质检出库任务！";
                case -46:
                    return "操作执行失败，无法修改质检单合格状态！";
                case -47:
                    return "操作执行失败，无法增加库存变化量！";
                case -48:
                    return "操作执行失败，无法添加质检入库任务！";
                case -49:
                    return "操作执行失败，质检出库任务未完成！";
                case -50:
                    return "操作执行失败，质检不合格！";
                case -51:
                    return "操作执行失败，波次订单不处于可确认状态！";
                case -52:
                    return "操作执行失败，库存不满足该波次运算！";
                case -53:
                    return "操作执行失败，波次无法锁定货位或修改库存不成功！";
                case -54:
                    return "操作执行失败，无法保存波次数据！";
                case -55:
                    return "操作执行失败，无法结波次订单！";
                case -56:
                    return "操作执行失败，无法生成波次任务！";
                case -57:
                    return "操作执行失败，无法记录订单确认人！";
                default:
                    return "操作执行失败，系统执行失败！";
            }
        }
        #endregion

        #region Receiving

        public const string Receiving_NeedSelect = "请先选择需要处理的收货单！";
        public const string Receiving_NoSelectItem = "未选择";
        public const string ReceivingDetail_NeedSelectItem = "请选择物料！";
        public const string ReceivingDetail_NeedSelectDock = "请选择月台！";
        public const string ReceivingDetail_Overflow = "您输入的数据不符合系统要求，请重新输入！";
        public const string ReceivingDetail_EditFailed_Proceeded = "本收货单不能修改！";
        public const string ReceivingDetail_InsertFailed = "操作执行失败，无法插入该项，请检查本收货单是否已经存在相同的物料！";
        public const string ReceivingDetail_UpdateFailed = "操作执行失败，无法修改该项，请检查本收货单已经存在相同的物料！";
        public const string ReceivingDetail_DeleteFailed = "操作执行失败，无法删除该项！";
        public const string ReceivingHead_InsertFailed = "操作执行失败，无法添加新收货单！";
        public const string ReceivingHead_DeleteFailed = "操作执行失败，无法删除此收货单！";
        public const string ReceivingHead_UpdateFailed = "操作执行失败，无法修改此收货单！";

        public const string Receiving_NoWorkFlowHead = "请首先配置业务流！";
        public const string Receiving_NoWorkPlace = "本业务流未配置月台。";

        public const string Receiving_TypeNeedLot = "您选择的收货类型，要求您必须选择一个批次！";
        public const string Receiving_TypeNeedShortInventory = "您选择的收货类型，要求您必须选择一个缺货单！";

        public const string Receiving_ChooseLot_SchemeNotFound = "未找到符合条件的计划。";
        public const string Receiving_ChooseLot_LotNotFound = "未找到符合条件的批次。";
        public const string Receiving_ChooseLot_NonSelect = "您没有选择批号！";
        public const string Receiving_ChooseLot_NoItemToAdd = "您选择的批号中，不存在需要收货的物料！";
        public const string Receiving_ChooseLot_ErrorState = "当前收货单不允许修改物料列表！";
        public const string Receiving_ChooseLot_NotThisShipper = "(本供货商不再对此供货。)";

        public const string Receiving_ChooseShortInventory_NonSelect = "您没有选择缺货单！";
        public const string Receiving_ChooseShortInventory_ShortInventoryNotFound = "指定批次没有缺货单。";

        public const string Receiving_ChooseShipper_NoneText = "无";
        public const string Receiving_ChooseShipper_NoneDescription = "不限制供货商";

        public const string Receiving_Confirm_Checked = "已检验";
        public const string Receiving_Confirm_NotChecked = "未检验";

        public static string Receiving_Confirm_Return(string ReceivingNumber, int Code)
        {
            return "收货单：" + ReceivingNumber + " 状态：" + WorkFlow_ReturnCode(Code);
        }

        public const string Receiving_Confirm_NotSelect = "请先选择需要处理的收货单！";

        public const string Receiving_Finished_DeleteFailed = "操作执行失败，无法删除指定的收货单！";
        public const string Receiving_NoSelect = "未选择";
        public const string Receiving_DockNotExisted = "该单据对应的月台，已经不存在于所属的系统设置中。";

        #endregion

        #region PermissionManager
        public const string Strategy_Initialize_Permision_InsertSameRoot = "请选择父目录！";
        public const string Strategy_Initialize_Role_NoSelectRole = "请先选择角色！";
        public const string Strategy_Initialize_User_CreateWithNoName = "请输入用户名！";
        public const string Strategy_Initialize_User_CreateWithNoPassword = "请输入密码！";
        public const string Strategy_Initialize_Permission_DeleteFailed = "操作执行失败，无法删除此记录，请检查是否仍在使用中！";
        public const string Strategy_Initialize_Role_DeleteFailed = "操作执行失败，无法删除此角色，请检查是否仍在使用中！";
        public const string Strategy_Initialize_UserRole_DeleteFailed = "操作执行失败，无法删除此记录，请检查是否仍在使用！。";
        public const string Strategy_Initialize_User_DeleteFailed = "操作执行失败，无法删除此用户，请检查是否仍在使用中！";
        public const string Strategy_Initialize_Equipment_IsNULL = "未使用";
        public const string Strategy_Initialize_Equipment_Unspecified = "未指定";
        public const string Permission_NoPermissionMessage = "对不起，您没有此操作的权限。";
        public const string Permission_User_UpdateWithNoPassword = "请输入新密码！";
        public const string Permission_User_OldWrongPassword = "输入的旧密码错误，请重新输入！";
        public const string Permission_User_UpdatePassword = "对不起，两次密码输入不相同，请重新输入。";
        public const string Permission_User_UpdatePasswordSuccess = "密码修改成功！";
        public const string Strategy_Initialize_Role_CreateWithNoFullInfo = "请输入完整的信息！";
        public const string Permission_SavePermissionSetting = "设置操作执行成功！";
        public const string Permission_SavePermission = "权限修改操作执行成功！";
        public const string Permission_InvalidPassword = "对不起,密码中不能包括非法字符(如:空格)";


        #endregion

        #region LabelPrint

        public const string LabelPrint_BarCodeLimit_Title = "条码限制说明";
        public const string LabelPrint_BarCodeLimit_Data = "由于物料系统对于每条记录仅保存最多两项条码，所以针对物料的条码系统中自第三条码起不允许存在单据序号的调用。系统在生成条码时忽略违规调用。\n此窗口的测试打印，以及批量打印功能不受此限制。";
        public const string LabelPrint_TextSetting_NeedText = "您需要输入文本！";
        public const string LabelPrint_BarSetting_NeedText = "您需要输入条码信息！";
        public const string LabelPrint_Description = "描述";
        public const string LabelPrint_Code = "编码";
        public const string LabelPrint_DeleteFailed = "该标签删除失败。";
        public const string LabelPrint_InsertUpdateFailed = "该标签修改失败。";
        public const string LabelPrint_NonSelect = "未选择";
        public const string LabelPrint_NeedNumber = "您需要输入物料编码！";

        public const string LableTemplate_NeedText = "您需要输入名称！";
        public const string LabelTemplate_PictureError = "您选择的图片无法识别或读取失败！";
        public const string LabelTemplate_PictureSize = "您选择的文件过大。系统最大支持16K（16384字节）大小的图片。";

        public const string LabelPrint_NoDataToPrint = "无打印数据。";
        public const string LabelPrint_Step1 = "正在获得数据";
        public const string LabelPrint_Step2 = "正在转换打印指令";
        public const string LabelPrint_Step3 = "正在打印";

        public static string LabelPrint_Remote(int ReturnCode)
        {
            switch (ReturnCode)
            {
                case 0:
                    return "打印指令已经发送至远程打印机操作执行成功！";
                case -1:
                    return "无法添加到远程打印机的连接，请确保远程客户端开启。";
                case -2:
                    return "无法添加到远程打印机的连接，可能由于版本冲突或网络问题。";
                case -3:
                    return "远程打印机打印失败。";
                case -4:
                    return "远程打印中发生错误。";
                default:
                    return "发生错误。";
            }
        }

        public const string LabelPrint_ItemCode = "物料编号";
        public const string LabelPrint_ItemLabelCode = "标签编号";
        public const string LabelPrint_BillNumber = "单据号：";
        public const string LabelPrint_UseBillNumber = "依单据号设置";

        #endregion

        #region BillFlow

        public const string BillFlow_CurrentWarehouse = "<当前库>";
        public const string BillFlow_ERPWarehouse = "<ERP数据交换库>";
        public const string BillFlow_ERPWarehouseID = "ERP";

        #endregion

        #region Location

        public const string Location_Rule_NeedData = "您需要输入类名和描述！";
        public const string Location_Rule_DeleteFailed = "操作执行失败，无法删除此规则细节，请检查是否仍在使用中。";
        public const string Strategy_Rule_InsertFailed = "操作执行失败。\n可能原因：该操作的规则已经存在重复项。";

        public const string Location_LocationRule_WorkFlowHead = "业务流：";
        public const string Location_LocationRule_Item = "物料：";
        public const string Strategy_LocationRule_InsertFailed = "操作执行失败。\n可能原因：该操作的货位选择规则已经存在重复项。";

        public static string Location_LocationRule_ZoneMode(bool Mode)
        {
            if (Mode) return "平均分配"; else return "顺序优先";
        }
        public const string Location_LocationRules_DeleteFailed = "操作执行失败，无法删除此规则，请检查是否仍在使用中。";
        public const string Location_LocationZone_DeleteFailed = "操作执行失败，无法删除此区域。";
        public const string Strategy_LocationZone_InsertFailed = "操作执行失败。\n可能原因：该操作的区域在本货位选择规则中已经存在重复项。";
        public const string Location_LocationZoneSQLRule_DeleteFailed = "操作执行失败，无法删除此规则。";
        public const string Strategy_LocationZoneSQLRule_InsertFailed = "操作执行失败。\n可能原因：该操作的规则细节在本货位选择规则和区域中已经存在重复项。";
        public const string Location_LocationZoneSQL_Failed = "操作执行失败，无法修改此设置。";
        public const string Location_LocationZoneSQL_Success = "设置操作执行成功！";


        #endregion

        #region OutBounding
        public const string OrderDetail_Update_Wrong = "操作执行失败，无法修改订单明细数据！";
        public const string OrderDetail_Delete_Wrong = "操作执行失败，无法删除订单明细数据。";
        public const string Order_NeedSelect = "请先选择要处理的订单！";
        public const string OrderDetail_NeedSelectItem = "没有物料选择！";
        public const string OrderDetail_Qty0 = "您需要将数量设置为大于0。";
        public const string OrderDetail_Overflow = "您输入的数据不符合系统要求，请重新输入！";
        public const string OrderHead_InsertFailed = "操作执行失败，无法添加新出库订单！";
        public const string OrderHead_UpdateFailed = "操作执行失败，无法修改出库订单！";
        public const string OrderHead_DeteleFailed = "操作执行失败，无法删除出库订单！";
        public const string Order_Shipper_NonSelect = "未指定";
        public const string Order_ChooseShipper_NotThisShipper = "(本供货商不再对此供货。)";
        public const string Order_NotExit = "请选择出库站台！";
        public const string Order_SelectedExit = "您选择的出库站台为";

        #endregion

        #region PreOutBound
        public const string PreOutBound_Edit_CreateFailed = "操作执行失败，无法添加下架。";
        public const string PreOutBound_Edit_Created = "下架添加操作执行成功！";
        public const string PreOutBound_UpdateFailed = "操作执行失败，无法修改下架。";
        public const string PreOutBound_UpdateOK = "下架修改操作执行成功！";
        public const string PreOutBound_UpdateFailedPart = "部分下架修改操作执行成功！";
        public const string PreOutBound_Edit_UpdateFailed = "操作执行失败，无法修改下架。";
        public const string PreOutBound_NotSelect = "请先选择需要处理的下架！";
        public const string PreOutBound_Not_SelectItem = "你没有选择要确认的纪录！";
        public const string PreOutBound_DeteleFailed = "操作执行失败，无法删除此下架。提示：您仅可以删除手工添加的且处于非执行状态的下架。";
        public const string PreOutBound_DeleteFailed = "操作执行失败，无法删除下架！";
        public const string PreOutBound_DeleteFailedPart = "部分下架删除操作执行成功！";
        public const string PreOutBound_DeleteOK = "下架删除操作执行成功！";
        public const string PreOutBound_MoreThanOneWindow = "您不能打开超过一个下架编辑窗口。";

        public static string PreOutBound_Confirm_Return(string ContainerBarCode, int Code, string ItemBarCode)
        {
            if (ContainerBarCode == "") ContainerBarCode = "无 ";
            if (ItemBarCode != "") ItemBarCode = " 第一物料条码：" + ItemBarCode;
            return "容器编号：" + ContainerBarCode + ItemBarCode + " 状态：" + WorkFlow_ReturnCode(Code);
        }

        public const string PreOutBound_NeedBarCode = "未满足条码限制，无法确认。";

        public static string PreOutBound_MaxPriority(int PreOutBoundPriority, int OutTaskPriority)
        {
            return "当前下架中最高优先级为" + PreOutBoundPriority.ToString() + "，出库任务中最高优先级为" + OutTaskPriority.ToString() + "。";
        }
        #endregion

        #region PreInBound
        public const string PreInBound_UpdateFailed = "修改操作执行成功！";

        public const string PreInBound_UpdateFailedPart = "部分上架修改操作执行成功！";
        public const string PreInBound_UpdateOK = "上架修改操作执行成功！";
        public const string PreInBound_DeleteFailed = "删除操作执行成功！";
        public const string PreInBound_DeleteFailedPart = "部分上架删除操作执行，请注意自动生成的上架不能删除。";
        public const string PreInBound_DeleteOK = "上架删除成功。";
        public const string PreInBound_NotSelect = "请先选择需要处理的上架！";
        public const string PreInBound_Container_ISNULL = "未指定";

        public const string PreInBound_MoreThanOneWindow = "您不能打开超过一个上架编辑窗口。";

        public const string PreInBound_Edit_NoContainer = "该上架没有包含任何容器。";
        public const string PreInBound_Edit_CreateFailed = "操作执行失败，无法添加上架！";
        public const string PreInBound_Edit_Created = "上架添加完成。";
        public const string PreInBound_Edit_UpdateFailed = "操作执行失败，无法修改上架！";

        public const string PreInBound_Edit_NotEnoughData = "请输入组编号和顺序号。！";
        public const string PreInBound_CannotEdit = "操作执行失败，无法修改或删除上架！";
        public const string PreInBound_Edit_NoQty = "请输入合法的数量值！";
        public static string PreInBound_Confirm_Return(string ContainerBarCode, int Code, string ItemBarCode)
        {
            if (ContainerBarCode == "") ContainerBarCode = "无 ";
            if (ItemBarCode != "") ItemBarCode = " 第一物料条码：" + ItemBarCode;
            return "容器编号：" + ContainerBarCode + ItemBarCode + " 状态：" + WorkFlow_ReturnCode(Code);
        }

        public const string PreInBound_NeedBarCode = "未满足条码限制，无法确认。";

        public static string PreInBound_GenerateLanawayText(string ChangeTime, string ItemCode, string ItemExteriorCode, string ItemGradeText, string EachFill, string CatalogName, string CatalogTypeText)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("修改时间：");
            sb.AppendLine(ChangeTime);
            sb.Append("物料编码：");
            sb.AppendLine(ItemCode);
            sb.Append("外部编码：");
            sb.AppendLine(ItemExteriorCode);
            sb.Append("码盘方式：");
            sb.AppendLine(ItemGradeText);
            sb.Append("单托盘载量：");
            sb.AppendLine(EachFill);
            //sb.Append("品种：");
            //sb.AppendLine(CatalogName);
            //sb.Append("品种类型：");
            //sb.Append(CatalogTypeText);
            return sb.ToString();
        }

        public const string PreInBound_SetLanawayError = "设置分流时发生错误。";
        public const string PreInBound_SetLanawayDone = "设置分流成功！";
        public const string PreInBound_SetLanawayWCSError = "设置分流时与WCS通信错误。";
        #endregion

        #region Task
        public const string Task_NoSelect = "请选择要处理的任务！";
        public const string Task_DeleteFailed = "操作执行失败，无法删除此记录,请检查是否仍在使用中。";
        public const string Task_DeleteFailedPart = "部分任务删除成功。";
        public const string Task_DeleteOK = "任务删除成功。";
        //public const string Task_Date = "对不起，完成日期不能小于开始日期。";
        public const string Task_HaveTaskExecuting = "您确认要强制所选择的纪录为执行中状态吗？";
        public const string Task_HaveTaskExecuting_Title = "强制执行中";
        public const string Task_HaveTaskNotExecute = "您确认要强制所选择的纪录为未执行状态吗？";
        public const string Task_HaveTaskNotExecute_Title = "强制未执行";
        public const string Task_HaveTaskPause = "您确认要强制所选择的纪录为暂停状态吗？";
        public const string Task_HaveTaskPause_Title = "强制暂停";
        public const string Task_HaveFinishedTheTaskWarning = "您确认要强制完成所选择的纪录吗？";
        public const string Task_HaveFinishedTheTaskWarning_Title = "强制完成";

        public const string Task_DeleteTheTaskWarning = "您确认要删除所选择的纪录吗？";
        public const string Task_DeleteTheTaskWarning_Title = "删除任务";

        #endregion

        #region NoteBook
        public const string NoteBook_InsertWithNoMessage = "对不起,记事本信息不能为空。";
        public const string NoteBook_UpdateWithNoMessage = "对不起,记事本信息不能为空。";
        public const string NoteBook_UpdateSuccess = "修改成功。";
        public const string NoteBook_DeleteSuccess = "删除成功。";
        public const string NoteBook_Exist = "对不起,此记录已添加,请使用修改。";
        #endregion

        #region Count
        public const string Count_NeedSelect = "请先选择需要处理的盘点单。";

        public const string Count_NoSelectItem = "未指定";
        public const string CountDetail_NeedSelectItem = "请选择物料。";
        public const string CountDetail_NeedSelectLocationNumber = "请选择货位。";
        public const string Count_NoWorkFlowHead = "请先配置业务流！";
        public const string CountHead_InsertFailed = "操作执行失败，无法添加新盘点单！";
        public const string CountHead_UpdateFailed = "操作执行失败，无法修改此盘点单！";
        public const string CountHead_DeleteFailed = "操作执行失败，无法删除此盘点单！";
        public const string CountHead_LowHighError = "对不起,盘点日期不能小于添加日期。";
        public const string CountDetail_Overflow = "您输入的数据不符合系统要求，请重新输入！";
        public const string CountDetail_EditFailed_Proceeded = "本盘点单不能修改。";
        public const string CountDetail_InsertFailed = "操作执行失败，无法插入该项！";
        public const string CountDetail_UpdateFailed = "操作执行失败，无法修改该项！";
        public const string CountHead_NeedSelect = "请先选择要处理的盘点！";
        public const string CountDetail_DeleteFailed = "操作执行失败，无法删除该盘点。\n可能原因：盘点非手工录入。";
        public const string Count_Confirm_NotSelect = "请先选择需要处理的盘点单！";
        public const string Count_DataOverflow = "您输入的数据过大，或者小于0。";
        public static string Count_Confirm_Return(string CountNumber, int Code)
        {
            return "盘点单：" + CountNumber + " 状态：" + WorkFlow_ReturnCode(Code);
        }
        public const string Count_Existed = "对不起,已存在！";
        #endregion

        #region ShortInventory

        public const string ShortInventory_NotSelect = "请先选择需要处理的缺货单！";
        public const string ShortInventory_DeleteFailed = "操作执行失败，无法删除。\n注意：您不能删除未完成的缺货单。";

        #endregion

        #region Wave
        public const string Wave_Item_NotSpecified = "(非拣选区)";
        public const string Wave_Item_NotCatalog = "(非相同品规)";
        public const string Wave_NoDock = "未找到可用的月台。";

        public const string Wave_Select_All = "（不使用筛选）";
        public const string Wave_Select_Filter = "筛选：";
        public const string Wave_Select_Selector = "选择：";
        public const string Wave_Open = "当前仓库波次功能未配置成功。";

        public const string Wave_Confirm = "您确认要进行波次运算？";


        #endregion

        #region Report
        public const string Report_Select_Item = "未选择";
        public const string Report_Year_Err = "由于本年度生产任务未完成,无法生成年报表,如果想查询本年度已完成生产任务统计数据,请到报表查询功能查看";
        public const string Report_Month_Err = "由于本月生产任务未完成,无法生成年报表,如果想查询本月已完成生产任务统计数据,请到报表查询功能查看";
        public const string Report_Day_Err = "由于本日生产任务未完成,无法生成年报表,如果想查询本日已完成生产任务统计数据,请到报表查询功能查看";

        public const string Report_Select_Err = "请选择正确的日期";
        #endregion

        #region Runs

        public const string Runs_Empty = "当前您没有处于班次之中。";
        public static string Runs_Status(string Name, int RunsDate, string TypeDescription, int SignStart, int SignStop, int DateAdd)
        {
            StringBuilder sb = new StringBuilder("班次：");
            sb.AppendLine(Name);
            sb.Append("所属日期：");
            sb.AppendLine(Class_TimeToString.TimeToString(RunsDate));
            sb.AppendLine("  ------");
            sb.Append("班次类别：");
            sb.AppendLine(TypeDescription);
            sb.Append("登记开始时间：");
            sb.AppendLine(Class_TimeToString.SecondToString(SignStart));
            sb.Append("登记结束时间：");
            sb.AppendLine(Class_TimeToString.SecondToString(SignStop));
            sb.Append("日期调整：");
            sb.AppendLine(DateAdd.ToString());
            return sb.ToString();
        }

        #endregion

        #region Check
        public const string InBoundCheck_DeleteFailed = "操作执行失败，无法删除检验项目。";
        public const string InBoundCheck_Not_SelectItem = "你没有选择要确认的纪录。";
        public const string InBoundCheck_DeleteOK = "检验项目删除成功。";
        public const string InBoundCheck_DeleteFailedPart = "部分检验项目删除成功。";
        public const string InBoundCheck_Not_exit = "请选择质检出库口";
        public const string InBoundCheck_NewOK = "质检下架添加完成";
        public const string InBoundCheck_OK = "确认";
        #endregion

        #region Combination
        public const string Container_Failed = "操作失败！";
        public const string Container_Exist = "已经存在该条记录。";
        public const string Container_PleaseSelect = "请选择行数据！";
        public const string Container_OnlyOne = "一次只能修改一行数据。";
        public const string Container_PleaseFillQty = "请填写数据信息！";
        public const string Container_QtyIllegal = "数字信息非法。";
        public const string Container_NeedSelectItem = "请选择产品名称和配盘模式！";
        public const string Container_QtyEmpty = "数量不能为空。";
        public const string Container_QtyNegative = "数量不能为负数。";
        public const string Container_NeedItems = "请选择产品名称、机台型号、区段、配盘单状态。";
        public const string Container_QtyIsZone = "数量不能为零。";
        public const string Container_Version = "版本不能为空。";
        public const string Container_IsDefault = "默认牌号不能删除！";
        public const string Container_EquipmentTypeIsDefault = "默认机台型号不能删除！";
        #endregion

        #region MachineDock
        public const string MachineDock_NotExist = "不存在该条记录。";
        public const string MachineDock_NoDockName = "站台名称不能为空。";
        public const string MachineDock_NeedSelectDockNumber = "请选择机台号和站台号！";
        public const string MachineDock_NeedSelectCombinationType = "请选择配盘模式！";
        public const string MachineDock_NoWorkshopDockCode = "车间类型和机台编号不能为空";
        public const string MachineDock_NoDockCode = "站台编号不能为空";
        public const string MachineDock_NoStateSection = "站台状态和区段不能为空";
        #endregion

        #region MachineProductPlan
        public const string MachineProductPlan_NotPlan = "无计划";
        public const string MachineProductPlan_NeedSelectMachineNo = "请选择机台号！";
        public const string MachineProductPlan_NeedSelectPlanID = "请选择计划编号！";
        public const string MachineProductPlan_NeedSelectProductName = "请选择产品名称！";
        public const string MachineProductPlan_PlanHasBeUser = "请选择其它计划，该计划已经被安排，！";
        public const string MachineProductPlan_MachineNoHasBeUser = "请选择其它机台号，该机台号已经被安排！";

        public const string MachineProductPlan_MachineIsUsing = "机台号已经设置生产牌号，请将其完成之后再重新设置！";
        public const string MachineProductPlan_MachineNotUsing = "机台号未设置生产牌号，不能执行完成操作！";
        #endregion

        #region ItemInventory
        public const string ItemInventory = "物料库存";
        public const string ItemInventory_Success = "成功。";
        public const string ItemInventory_Failed = "失败。";
        public const string ItemInventory_NotMinus = "数量不能为负值！";
        public const string ItemInventory_Invalid = "数量信息非法！";
        public const string ItemInventory_CompareUsedandTotalQty = "已用数量不能超过总数量！";
        #endregion

        #region ProcessBOM

        public const string ProcessBom_ErrorUpdateType = "只能修改该记录的模式描述,数量,备注。";
        public const string ProcessBom_ErrorUpdateItem = "只能修改该记录的数量,描述。";
        public const string ProcessBom_NoVersion = "版本不能为空！";
        #endregion
        public const string ContainerType_NA = "不适用";
        public const string ListViewFiltered_RegionSelect_Selector_GrammarError = "您当前的设定不符合语法规范。";

        #region WorkFlow
        public const string UPDATESTRATEGY_SUCCESS = "操作执行失败，修改策略配置文件操作成功";
        public const string UPDATESTRATEGY_ERROR = "操作执行失败，无法修改策略配置文件";
        public const string INSERTSTRATEGY_SUCCESS = "保存策略配置文件操作成功";
        public const string INSERTSTRATEGY_ERROR = "操作执行失败，无法保存策略配置文件";

        public const string CONFIGFILE_ERROR = "操作执行失败，无法验证活动策略配置，请检查。";
        #endregion

        #region CallTerminals
        /// <summary>
        /// 呼叫终端相关.PC
        /// </summary>

        public const string HAVING_NOT_PERMISSIONS = "对不起,您没有权限进行此操作,请与管理员联系";
        public const string SYSTEM_HINT = "系统提示";
        public const string NOOPERATORMES = "对不起,您不能进行";
        public const string OPERATORSUCCEED = "操作执行成功!";
        public const string OPERATORFAIL = "操作执行失败!";
        public const string OPERATORSERVERALSUCC = "操作部分成功!";
        public const string RETURN_COUNT_OVER_ERROR = "退料总量不能超出BOM的数量,请重新输入!";
        public const string RETURN_COUNT_OVER_GET_ERROR = "退料总量超出领料总量,请重新输入!";
        public const string HAVING_LGV_ERROR = "此操作必须要有LGV机器!";
        public const string GET_COUNT_ERROR = "领料量不能超过库存量!";
        public const string SEND_COUNT_ERROR = "发料量不能超过库存量";
        public const string ODD_OVER_ERROR = "零头总量不能超过零头库存总量";
        public const string COUNT_ZERO_ERROR = "领料量为零,不能产生要料单!";
        public const string RETURN_BILL_ZERO_ERROR = "退料量不能为0,请重新输入!";


        public const string DOCK_ERROR = "本机台没有设置人工站台!";
        public const string DOCK_REJECT_ERROR = "本机台没有设置退残烟站台!";
        public const string DOCK_WAST_ERROR = "本机台没有设置退废料站台!";
        public const string NO_TARGET_DESTINATION = "没能获取到目的区域";
        public const string NUMBER_DIGIT_OVER_ERROR = "您输入的数值位数太长,请重新输入!";
        public const string DATA_CHANGE_HINT = "您确认进行以下数据修改?";
        public const string ENSURE_SEND_COUNT = "您确认按以下数量进行发料操作?";

        public const string DOCK_TASK_ERROR = "站台存在未完成任务, 不允许进行操作!";

        #region 要料
        public const string NO_DOCK = "该机台还没有设置站台位";
        public const string DOCKTASKERROR = "对不起,您不允许执行要料操作,本站台存在未完成任务;如果当前机台是成型机台请确认使用位是否已经要料，请先在使用位要料，并确定当前站台不是2号站台。";
        public const string REFRESH_MESSAGE = "对不起,您的刷新请求不能过频繁.";
        #endregion

        #region 换牌
        public const string CHANGE_CARD_HINT = "机台将执行“换牌”操作,请确认!";
        #endregion

        #region 交接班
        public const string CHANGE_CLASS_HINT = "机台将执行“交接班”操作,请确认!";
        public const string CHANGE_CLASS_RECIEVE = "将执行“接收交接班”操作,请确认!";
        #endregion
        #region 人工退料
        public const string RETURN_MATERIALS_HINT = "您将执行“人工退料”操作,请确认!";
        #endregion

        #region 人工领料
        public const string HAND_MATERIALS_BILL = "您将创建一份领料单,请确认!";
        public const string BILL_ERROR = "创建要料单操作失败,请重试!";
        public const string BILL_SUCC = "创建要料单操作成功!";
        public const string BILL_RECIEVE_HINT = "您将接收一份领料单的物料,请确认!";
        public const string BILL_RECIEVE_ALL_HINT = "您将接收全部领料单(已发料)的物料,请确认!";
        public const string ACCEPT_SUCC = "接收物料操作成功";
        public const string ACCEPT_ERROR = "接收物料操作失败,请重试!";
        #endregion

        #region 确认退料单
        public const string CHANGE_COUNT_HINT = "您将执行物料数量的“修改”操作,请确认!";
        public const string ENSURE_BILL_HINT = " 站台将执行“确认退料单”操作,请确认!";
        public const string ENSURE_BILL_ALL_HINT = "您将执行“确认全部退料单”操作,请确认!";
        public const string RETURN_BILL_HINT = " 站台将执行“退回此单”操作,请确认!";
        #endregion

        #region 退坏料
        public const string RETURN_BAD_MATERIALS_HINT = "您将执行“退坏料”操作,请确认!";
        public const string RETURN_BAD_RESULT_ERROR = "对不起,您至少选择一项退坏料原因!";

        #endregion

        #region 退余料
        public const string RETURN_MATERIALS_TRUE_HINT = " 站台将执行“真退”操作,请确认!";
        public const string RETURN_MATERIALS_FALSE_HINT = " 站台将执行“假退”操作,请确认!";
        #endregion

        #region 数据输入
        public const string DECIMAL_INPUT_ERROR = "数据输入形式错误,存在多个小数点!请重新输入!";
        public const string DATA_INPUT_ERROR = "数据输入形式错误!请重新输入!";
        #endregion
        #endregion
    }
}