using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface ContainerInfoListWS : _KeepAlive
    {
        [System.ServiceModel.OperationContract()]
        EntityDefination.Container_View ShowContainer(String warehouseId, String containerId);

        [System.ServiceModel.OperationContract()]
        EntityDefination.Container_View ShowContainerFinished(String warehouseId, String containerId);
        
                
        [System.ServiceModel.OperationContract()]
        new void KeepAlive();
    }
}
