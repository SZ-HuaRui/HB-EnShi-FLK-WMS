using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;
using WMSInterface.Entity50;


namespace WMSInterface
{

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICallBack))]
    public interface IGetData
    {
        [OperationContract]
        DataTable GetInventoryInfo(LoginInfo loginInfo, Dictionary<String, String> Params);

        [OperationContract]
        DataTable GetLotInventory(LoginInfo loginInfo, Dictionary<String, String> Params);

        [OperationContract]
        DataTable GetLog(LoginInfo loginInfo, Dictionary<String, String> Params);

        [OperationContract]
        DataTable GetProductBOMCompute(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable GetProductBOMComputeDetailN(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable SelectContainerInventoryN(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable GetFinishedTaskFL(LoginInfo loginInfo, Dictionary<String, String> Params);
       
        [System.ServiceModel.OperationContract()]
        DataTable GetFinishedTaskNotFL(LoginInfo loginInfo, Dictionary<String, String> Params);
    }

    public interface ICallBack
    {
        //[OperationContract(IsOneWay = true)]
        [OperationContract]
        int SendDataRow(DataTable values,bool IsLast);

        [OperationContract]
        int NeedMore();
    }

    [ServiceContract]
    public interface IControlData : _KeepAlive
    {
        [OperationContract]
        DataTable GetControlDataTitle(LoginInfo loginInfo);

        [OperationContract]
        DataTable GetControlDataDetail(LoginInfo loginInfo);

        [OperationContract]
        bool SaveControlData(LoginInfo loginInfo, Dictionary<string, string> Title, List<Dictionary<string, string>> Detail);
    }
  
}
