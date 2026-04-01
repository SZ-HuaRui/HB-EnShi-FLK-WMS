using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using WMSInterface.EntityDefination;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface WaveWS : _KeepAlive
    {
        [System.ServiceModel.OperationContract()]
        new void KeepAlive();

        [System.ServiceModel.OperationContract()]
        Collection<OrderHead_View_ForWave> SelectOrderHeadForWave(String equipmentNumber, String userId, String userPassword, String warehouseId);
        [System.ServiceModel.OperationContract()]
        byte[] byteSelectOrderHeadForWave(String equipmentNumber, String userId, String userPassword, String warehouseId);
        
         [System.ServiceModel.OperationContract()]
        DataTable dtSelectOrderHeadForWave(String equipmentNumber, String userId, String userPassword, String warehouseId);

        #region 查找波茨订单号 2011-11-29 YYL
        [System.ServiceModel.OperationContract()]
        DataSet DsSelectOrderHeadForWave(String equipmentNumber, String userId, String userPassword, String warehouseId);
        #endregion


        [System.ServiceModel.OperationContract()]
        Collection<OrderHead_View_ForWave> SelectUserDefinedOrderHeadForWave(String equipmentNumber, String userId, String userPassword, String warehouseId, String OrderTypeFlag);
        [System.ServiceModel.OperationContract()]
        DataTable dtSelectUserDefinedOrderHeadForWave(String equipmentNumber, String userId, String userPassword, String warehouseId, String OrderTypeFlag);

        [System.ServiceModel.OperationContract()]
        Collection<WaveHead_View> SelectWaveHead(String equipmentNumber, String userId, String userPassword, String warehouseId, SelectStateScope stateScope);
        [System.ServiceModel.OperationContract()]
         DataTable dtSelectWaveHead(String equipmentNumber, String userId, String userPassword, String warehouseId, WMSInterface.SelectStateScope stateScope);

        [System.ServiceModel.OperationContract()]
        String InsertWaveHead(String equipmentNumber, String userId, String userPassword, String warehouseId, WaveHead entity);
        [System.ServiceModel.OperationContract()]
        Int32 DeleteWaveHead(String equipmentNumber, String userId, String userPassword, String warehouseId, String waveId);
        [System.ServiceModel.OperationContract()]
        String ReleaseWaveHead(String equipmentNumber, String userId, String userPassword, String warehouseId, String waveId, String flag);
        [System.ServiceModel.OperationContract()]
        String FinishWaveHead(String equipmentNumber, String userId, String userPassword, String warehouseId, String waveId);

        [System.ServiceModel.OperationContract()]
        Collection<WaveDetail_View> SelectWaveDetail(String equipmentNumber, String userId, String userPassword, String warehouseId, String waveId);
        [System.ServiceModel.OperationContract()]
        List<String> InsertWaveDetail(String equipmentNumber, String userId, String userPassword, String warehouseId, String waveId, WaveDetail entity);
        [System.ServiceModel.OperationContract()]
        Int32 DeleteWaveDetail(String equipmentNumber, String userId, String userPassword, String warehouseId, String waveId, String waveDetailId);
        [System.ServiceModel.OperationContract()]
        Int32 UpdateWaveDetailSequence(String equipmentNumber, String userId, String userPassword, String warehouseId, String waveId, String waveDetailId, String orderSequence, String detailSequence);
        [System.ServiceModel.OperationContract()]
        Int32 UpdateWaveDetailQty(String equipmentNumber, String userId, String userPassword, String warehouseId, String waveId, String waveDetailId, Decimal inWaveQty);

        [System.ServiceModel.OperationContract()]
        Collection<WaveTask_View> SelectWaveTask(String equipmentNumber, String userId, String userPassword, String warehouseId, String waveId);
        [System.ServiceModel.OperationContract()]
        Int32 DeleteWaveTask(String equipmentNumber, String userId, String userPassword, String warehouseId, String waveId, String taskNumber);

        [System.ServiceModel.OperationContract()]
        Collection<WaveInventoryCheck> CheckWaveInventory(String equipmentNumber, String userId, String userPassword, String warehouseId, String waveId);

        [System.ServiceModel.OperationContract()]
        Collection<WaveFillingRequired> GetWaveFillingRequired(String equipmentNumber, String userId, String userPassword, String warehouseId, String waveId);

        [System.ServiceModel.OperationContract()]
        String CreateOutboundTask(String equipmentNumber, String userId, String userPassword, String warehouseId, String waveId, String waveDetailId, String locationId);
        [System.ServiceModel.OperationContract()]
        String CreateFillingTask(String equipmentNumber, String userId, String userPassword, String warehouseId, String waveId, String waveDetailId, String locationId);
        [System.ServiceModel.OperationContract()]
        String CreatePickingTask(String equipmentNumber, String userId, String userPassword, String warehouseId, String waveId, String waveDetailId);

        [System.ServiceModel.OperationContract()] //只创建拣选任务不创建补货任务
        string CreateOnlyPickingTask(string equipmentNumber, string userId, string userPassword, string warehouseId, string waveId, string waveDetailId, string LocationID, string ContainerID);

        [System.ServiceModel.OperationContract()]
        String InsertDockControl(String equipmentNumber, String userId, String userPassword, String warehouseId, String dockId, String waveDetailId);
        [System.ServiceModel.OperationContract()]
        List<WaveDetail_View> GetByDock(String equipmentNumber, String userId, String userPassword, String warehouseId, String dockId);
        [System.ServiceModel.OperationContract()]
        Int32 BatchOrderDeliveryModify(String equipmentNumber, String userId, String userPassword, String warehouseId, String newDeliveryMan, String oldDeliveryMan);
    }
}
