using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using WMSInterface.Entity50;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface LogWS : _KeepAlive
    {
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceLog(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);

        [System.ServiceModel.OperationContract()]
        DataSet SelectLog(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, String condition);

        [System.ServiceModel.OperationContract()]
        DataTable GetLogUserIDN(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable GetLogTypeN(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable GetLogCommNameN(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable GetLogCriticalLevelN(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        DataTable GetLogClientNumberN(LoginInfo loginInfo, Dictionary<String, String> Params);

        [System.ServiceModel.OperationContract()]
        new void KeepAlive();

        [System.ServiceModel.OperationContract()]
        DataTable GetLogs(WMSInterface.Entity50.LoginInfo loginInfo, Dictionary<string, string> Params);
    }
}
