using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface MessengerWS : _KeepAlive
    {
        [System.ServiceModel.OperationContract()]
        int InsertEvent(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Sender, string Receiver, string Message, int Priority, DateTime SendTime, string StateCode);
        [System.ServiceModel.OperationContract()]
        int UpdateEvent(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string EventID, string Message, int Priority, DateTime SendTime, string StateCode);
        [System.ServiceModel.OperationContract()]
        int Update2Event(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, int EventID, string Receiver, int Priority);
        [System.ServiceModel.OperationContract()]
        int DeleteEvent(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string EventID);
        [System.ServiceModel.OperationContract()]
        DataSet SelectEvent(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceEvent(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Receiver, DateTime SendTime);
        [System.ServiceModel.OperationContract()]
        int InsertNoteBook(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string UserID, DateTime WriteDate, string Content, string StateCode);
        [System.ServiceModel.OperationContract()]
        int UpdateNoteBook(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string NoteBookID, DateTime WriteDate, string Content, string StateCode);
        [System.ServiceModel.OperationContract()]
        int Update2NoteBook(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string NoteBookID, string Content);
        [System.ServiceModel.OperationContract()]
        int DeleteNoteBook(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, DateTime WriteDate);
        [System.ServiceModel.OperationContract()]
        DataSet SelectNoteBook(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, DateTime WriteDate);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceNoteBook(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, DateTime WriteDate);
        [System.ServiceModel.OperationContract()]
        DataSet Choice2NoteBook(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, DateTime StartDate, DateTime EndDate);
        [System.ServiceModel.OperationContract()]
        new void KeepAlive();
    }
}
