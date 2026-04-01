using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using WMSInterface.Entity50;

namespace WMSInterface
{
    [System.ServiceModel.ServiceContract]
    public interface ProducingPrintWS : _KeepAlive
    {
        #region Label
        [System.ServiceModel.OperationContract()]
        object InsertBarcodeReceivingNumber(LoginInfo loginInfo, BarcodeRecevingNumber barcodeReceivingNumber);
        [System.ServiceModel.OperationContract()]
        int UpdateLabel(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LabelID, string ItemID, string LabelNumber, string ItemDescription1, string ItemDescription1Code, string ItemDescription2, string ItemDescription2Code, string ItemDescription3, string ItemDescription3Code, string ItemDescription4, string ItemDescription4Code, string ItemDescription5, string ItemDescription5Code, string ItemDescription6, string ItemDescription6Code, string ItemDescription7, string ItemDescription7Code, string ItemDescription8, string ItemDescription8Code, string Field1, string Field2);
        [System.ServiceModel.OperationContract()]
        string InsertLabel(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID, string LabelNumber, string ItemDescription1, string ItemDescription1Code, string ItemDescription2, string ItemDescription2Code, string ItemDescription3, string ItemDescription3Code, string ItemDescription4, string ItemDescription4Code, string ItemDescription5, string ItemDescription5Code, string ItemDescription6, string ItemDescription6Code, string ItemDescription7, string ItemDescription7Code, string ItemDescription8, string ItemDescription8Code, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        DataSet SelectLabel(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet ChoiceLabel(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LabelNumber);
        [System.ServiceModel.OperationContract()]
        int DeleteLabel(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string LabelID);
        [System.ServiceModel.OperationContract()]
        DataTable GetLabelInfo(LoginInfo loginInfo, Dictionary<string, string> Params);
        [System.ServiceModel.OperationContract()]
        DataSet GetData(string WarehouseID, string[] ItemIDs);
        [System.ServiceModel.OperationContract()]
        void GetBarCode(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string[] ItemIDs, ref string[] BarCode1, ref string[] BarCode2);
        #endregion

        #region Template
        [System.ServiceModel.OperationContract()]
        int InsertTemplate(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TemplateName, string Description, byte[][] Picture_Data, int[] Picture_x, int[] Picture_y, string[] Text_Text, bool[] Text_DirectMode, string[] Text_FontCommand, string[] Text_FontName, decimal[] Text_Size, bool[] Text_Bold, bool[] Text_Italic, bool[] Text_StrikeOut, bool[] Text_Underline, int[] Text_x, int[] Text_y, string[] BarCode_Code, string[] BarCode_Command, string[] BarCode_Parameter, int[] BarCode_x, int[] BarCode_y, string[] BarCode_Front, string Field1, string Field2, string Field3, string Field4, string Field5);
        [System.ServiceModel.OperationContract()]
        int UpdateTemplate(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TemplateID, string TemplateName, string Description, byte[][] Picture_Data, int[] Picture_x, int[] Picture_y, string[] Text_Text, bool[] Text_DirectMode, string[] Text_FontCommand, string[] Text_FontName, decimal[] Text_Size, bool[] Text_Bold, bool[] Text_Italic, bool[] Text_StrikeOut, bool[] Text_Underline, int[] Text_x, int[] Text_y, string[] BarCode_Code, string[] BarCode_Command, string[] BarCode_Parameter, int[] BarCode_x, int[] BarCode_y, string[] BarCode_Front);
        [System.ServiceModel.OperationContract()]
        DataSet SelectTemplate(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID);
        [System.ServiceModel.OperationContract()]
        DataSet GetOneTemplate(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TemplateID);
        [System.ServiceModel.OperationContract()]
        int DeleteTemplate(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string TemplateID);
        #endregion


        //[System.ServiceModel.OperationContract()]
        //void CreateIntoContainerDetail(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string[] ItemID, string[] ContainerDetailID, bool[] NeedBarCode1, bool[] NeedBarCode2);
        [System.ServiceModel.OperationContract()]
        DataSet GetItemSetting(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, string ItemID);

        [System.ServiceModel.OperationContract()]
        void AutoImportLabel(string EquipmentNumber, string UserName, string UserPassword, string WarehouseID, ref Int32 total, ref Int32 ifSucceed);
        [System.ServiceModel.OperationContract()]
        new void KeepAlive();
    }
}
