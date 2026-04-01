using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface PermissionManagerWS : _KeepAlive
    {
        [System.ServiceModel.OperationContract()]
        int InsertUser(string EquipmentNumber, string WarehouseID, string UserID, string UserPassword, string TargetID, string CName, string EName, string Email, string Birthday, string EquipmentID, string Password, string Memo, string Field1, string Field2, string Field3, string Field4, string Field5);

        [System.ServiceModel.OperationContract()]
        DataSet SelectUserByCondition(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string SqlCondition);

        [System.ServiceModel.OperationContract()]
        int UpdateUser(string EquipmentNumber, string WarehouseID, string UserID, string UserPassword, string TargetID, string CName, string EName, string Email, string Birthday, string EquipmentID, string Memo);
        [System.ServiceModel.OperationContract()]
        int Update2User(string EquipmentNumber, string WarehouseID, string UserID, string Password, string TargetID, string NewPassword);
        [System.ServiceModel.OperationContract()]
        DataSet SelectUser(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        string GetUserName(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string UserID);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceUser(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID,string UserID);
        [System.ServiceModel.OperationContract()]
        DataSet Choice2User(string EquipmentNumber, string UserPassword, string WarehouseID, string UserID);
        [System.ServiceModel.OperationContract()]
        int DeleteUser(string EquipmentNumber, string WarehouseID, string UserID, string UserPassword, string TargetID);
        [System.ServiceModel.OperationContract()]
        string InsertRole(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string Name, string Description, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        int UpdateRole(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string RoleID, string Name, string Description);
        [System.ServiceModel.OperationContract()]
        DataSet SelectRole(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceRole(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID,string RoleID);
        [System.ServiceModel.OperationContract()]
        int DeleteRole(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string RoleID);
        [System.ServiceModel.OperationContract()]
        //int InsertPermission(string EquipmentNumber, string UserName, string UserPassword, string ParentPermissionID, string Name, string Description, string Field1, string Field2, string Field3, string Field4, string Field5);
        string InsertPermission(string EquipmentNumber, string UserName, string UserPassword, string ParentPermissionID, string Name, string Description, string Field1, string Field2, string Field3, string Field4, string Field5, int serial);
        [System.ServiceModel.OperationContract()]
        int UpdatePermission(string EquipmentNumber, string UserName, string UserPassword, string PermissionID, string Name, string Description,string foreignName,int serial,string NodeDisplay,string NodeType);
        //[System.ServiceModel.OperationContract()]
        //DataSet SelectPermission(string EquipmentNumber, string UserName, string UserPassword, string PermissionID);
        [System.ServiceModel.OperationContract()]//构造功能树也要用到此方法
        DataSet ChoicePermission(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID,string type);
        [System.ServiceModel.OperationContract()]
        DataSet Choice2Permission(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ParentPermissionID);
        [System.ServiceModel.OperationContract()]
        int DeletePermission(string EquipmentNumber, string UserName, string UserPassword, string PermissionID);
        [System.ServiceModel.OperationContract()]
        string InsertUserRole(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string RoleID, string UserID, string Description, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        int UpdateUserRole(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string UserRoleID, string RoleID, string UserID, string Description);
        [System.ServiceModel.OperationContract()]
        DataSet SelectUserRole(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);

        [System.ServiceModel.OperationContract()]
        DataSet SelectUserRoleByCondition(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string SqlCondition);

        [System.ServiceModel.OperationContract()]
        DataSet ChoiceUserRole(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID,string UserRoleID);
        [System.ServiceModel.OperationContract()]
        int DeleteUserRole(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string UserRoleID);
        [System.ServiceModel.OperationContract()]
        int UpdateRolePermission(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string RoleID, string[] PermissionID, string Description, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet SelectRolePermission(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string RoleID);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceRolePermission(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string RoleID);
        [System.ServiceModel.OperationContract()]
        DataSet ChoicePermission_ByPermissionId(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword,string permissionid);
        [System.ServiceModel.OperationContract()]
        DataSet ChoicePermissionID_ByPermissionName(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string permissionName);
        [System.ServiceModel.OperationContract()]
        DataSet FindPermission_ByParentPermissionId(string WarehouseID, string EquipmentNumber, string UserName, string UserPassword, string permissionId);

        [System.ServiceModel.OperationContract()]
        bool ModifyPwd(string warehouseId, string userId, string oldPwd, string newPwd, out string msg);
        new void KeepAlive();
    }
}
