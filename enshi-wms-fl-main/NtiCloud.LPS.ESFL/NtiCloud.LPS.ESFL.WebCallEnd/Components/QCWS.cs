using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface QCWS : _KeepAlive
    {
        #region New

        [System.ServiceModel.OperationContract()]

        string QCHeadInsServer(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TypeCode, string Reason);

        [System.ServiceModel.OperationContract()]

        DataSet ChioceQcServer(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string SQLCommon);

       
        [System.ServiceModel.OperationContract()]

        DataSet ChioceQCDetailServer(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string QCID, string SQLCommon);

        [System.ServiceModel.OperationContract()]

        DataSet ChioceLocationServer(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string SQLCommon);

        [System.ServiceModel.OperationContract()]
        DataSet GetLocationExecute(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string SQLCommon);

        [System.ServiceModel.OperationContract()]

        WorkFlowResult QC_Del_Work(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string QCID);
        [System.ServiceModel.OperationContract()]

        WorkFlowResult QC_DirectQC_Work(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string QCID, string ContainerID);

        [System.ServiceModel.OperationContract()]

        WorkFlowResult QC_Result_Work(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string QCDetailID, string CheckStateCode, string Reason);

        [System.ServiceModel.OperationContract()]

        WorkFlowResult QC_OutBoundQC_Work(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string QCID, string ContainerID, string TargetAddressID, string TargetAddressText, string TargetAddressType, int Priority);

        [System.ServiceModel.OperationContract()]

        WorkFlowResult QCDetail_Del_Work(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string QCDetailID);

        [System.ServiceModel.OperationContract()]

        WorkFlowResult QC_Finished_Work(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string QCID);

        [System.ServiceModel.OperationContract()]

        bool QCHeadFinishedDel(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string QCID);

        [System.ServiceModel.OperationContract()]

        string ChioceWarehouseTypeCodeServer(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);

        #endregion


        [System.ServiceModel.OperationContract()]

        DataSet QCFromItem_ConIDServer(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ContainerID);

        [System.ServiceModel.OperationContract()]

        int Ch_TaskServer(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ContainerID);

        [System.ServiceModel.OperationContract()]

        bool QCllLLServer(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string QCID, string QCDetailID, string ContainerID, string ContainerDetailID, decimal LLQty, ref string ReturnStr);


        [System.ServiceModel.OperationContract()]

        bool QCllGhServer(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string QCID, string QCDetailID, string ContainerID, string QCFromItemID, decimal GhQty, ref string ReturnStr);

        [System.ServiceModel.OperationContract()]
        int Get_LocationDepthServer(string EquipmentNumber, string UserName, string UserPassword, string WareHouseID, string ContainerID);


        [System.ServiceModel.OperationContract()]
        new void KeepAlive();
    }
}
