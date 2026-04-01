using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    class PermissionString
    {
        public const string WORKSHOP_CALLPALLET = "WorkShop.CallPallet";   //要料呼叫
        public const string WorkShop_ReceiveReturnBill = "WarehouseManage.ConfirmReturnBill";   //接收机台退料单
        public const string WorkShop_ReturnNullPallet = "WorkShop.ReturnNullPallet";//退空托盘
        public const string WorkShop_ReturnReject = "WorkShop.ReturnMaterial.ReturnReject";        //退残烟
        public const string WorkShop_ReturnWaste = "WorkShop.ReturnMaterial.ReturnWaste";          //退废料
        public const string WorkShop_UpdateReturnItemQty = "WorkShop.UpdateReturnItemQty"; //修改领/退料量
        public const string WarehouseManage_SentItemByBill = "WarehouseManage.SentItemByBill";  //仓库人员发手工领料单
        public const string WorkShop_ReturnBill = "WorkShop.ReturnMaterial";            //退料  WorkShop.ReturnMaterial.ReturnBill
        public const string WorkShop_CurMachineNumberSetting = "WorkShop.CurMachineNumberSetting";   //允许设置机台编号
        public const string WorkShop_ChangeClassAdmin = "WorkShop.ChangeClassUpdateItemQty";//交接班管理员模块

        //显示机台设置按钮 PEQ 20110409
        public const string WorkShop_ShowSetMachine = "WorkShop.Show_SetMachine";
    }

    class DockState
    {
        public const string DOCK_STATUS_OK = "EquipmentStatus_OK";
        public const string DOCK_STATUS_USING = "EquipmentStatus_Using";
    }

    class PermissionConst
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string Task = "Task";
    }

    class RadioButtonOperator
    {
        public const string CallPallet = "要料+退空托盘";
        public const string ProductTransfer = "成品转移";
        public const string ReturnNullPallet = "退空托盘";
        public const string ReturnNullPalletAndCallPallet = "退空托盘 + 要料";
        public const string ReturnExcrescentMaterials = "退 余 料";
        public const string ReturnBadMaterials = "退 坏 料";
        public const string ReturnWasteMaterials = "退废料";
        public const string ManPowerReturnMaterials = "人工退料";
        public const string OnlyCallPallet = "要料";
        public const string ManPowerCallMaterials = "人工领料";
    }

    class Hint
    {
        public const string ERROR = "error";
        public const string RIGHT = "right";
        public const string WARNING = "warming";
    }

    class TaskType
    {
        public const string TASK_CALLTERMINAL_CALL = "Task_CallTerminal_Call";
        public const string TASK_RFSENDITEM = "Task_RFSendItem";
        public const string TASK_CALLTERMINAL_RETURNBADPALLET = "Task_CallTerminal_ReturnBadPallet";
        public const string TASK_CALLTERMINAL_RETURNNULLPALLET = "Task_CallTerminal_ReturnNullPallet";
        public const string TASK_CALLTERMINAL_RETURNPALLET = "Task_CallTerminal_ReturnPallet";
        public const string TASK_CALLTERMINAL_RETURNWASTEPALLET = "Task_CallTerminal_ReturnWastePallet";
        public const string TASK_MANPOWER_RETURN = "Task_ManPower_Return";
    }

    class HandStates
    {
        public const string HandStates_Send = "PreOutBound_Sent";
        public const string HandStates_New = "PreOutBound_New";
        public const string HandStates_Finished = "PreOutBound_Finished";
    }

    class OperType
    {
        public const string CLIENT_CALLTERMINAL_RETURNBADPALLET = "Client_CallTerminal_ReturnBadPallet";
        public const string CLIENT_ACCEPTMATERIALS = "Client_AcceptMaterials";
        public const string CLIENT_RETURNNULLPALLET = "Client_ReturnNullPallet";
        public const string CLIENT_CALLPALLET_DEPENDENCELGV = "Client_CallPallet_DependenceLGV";
        public const string CLIENT_SENTMATERIALS = "Client_SentMaterials";
        public const string CLIENT_CREATEMATERIALSBILL_HAND = "Client_CreateMaterialsBill_Hand";
        public const string CLIENT_RETURNMATERIALDETAIL = "Client_ReturnMaterialDetail";
        public const string CLIENT_CONFIRM_RETURNMATERIAL_NEXT = "Client_Confirm_ReturnMaterial_Next";
        public const string CLIENT_RETURNMATERIAL_FALSE_THIS = "Client_ReturnMaterial_False_This";
        public const string CLIENT_CONFIRM_RETURNMATERIALBILL = "Client_Confirm_ReturnMaterialBill";
        public const string CLIENT_CALLPALLET = "Client_CallPallet";
    }
}