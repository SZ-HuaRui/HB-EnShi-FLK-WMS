using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;


namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface LoginWS : _KeepAlive
    {
        [System.ServiceModel.OperationContract()]
        bool SelectLogin(string WarehouseID, string Number, string UserID, string Password);

        [System.ServiceModel.OperationContract()]
        bool CheckUser(string WarehouseID, string Number, string UserID, string Password);

        //if (Class_CheckUser.CheckUser(WarehouseID, EquipmentNumber, UserName, UserPassword))
        [System.ServiceModel.OperationContract()]
        bool CheckUserByName(string WarehouseID, string Number, string UserID, string Password,string CheckUserName);


        [System.ServiceModel.OperationContract()]

        string ReadUserXM(string WarehouseID, string Number, string UserID, string Password);
        [System.ServiceModel.OperationContract()]
        new void KeepAlive();        

    }

    //public class PermissionListClass : List<string>
    //{
    //    public new bool Contains(string Item)
    //    {
    //        if (PermissionChecked != null) PermissionChecked();
    //        if (base.Contains("*")) return true;
    //        else return base.Contains(Item);
    //    }

    //    public delegate void PermissionCheckedDelegate();
    //    public event PermissionCheckedDelegate PermissionChecked;
    //}	
}
