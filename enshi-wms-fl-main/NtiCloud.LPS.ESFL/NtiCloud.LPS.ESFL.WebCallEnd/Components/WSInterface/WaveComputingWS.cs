using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using WMSInterface.EntityDefination;
using System.ServiceModel;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract(CallbackContract = typeof(WaveComputingCallBack))]
    public interface WaveComputingWS
    {
        [System.ServiceModel.OperationContract(IsOneWay = true)]
        void KeepAlive();

        [System.ServiceModel.OperationContract(IsOneWay = true)]
        void AutoCompute(String equipmentNumber, String userId, String userPassword, String warehouseId, String waveId, String locationAssign_WorkflowHeadId);

    }
}
