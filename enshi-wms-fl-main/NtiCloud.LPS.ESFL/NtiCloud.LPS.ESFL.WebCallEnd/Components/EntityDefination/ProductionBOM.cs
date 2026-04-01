using System;
using System.Collections.Generic;
using System.Text;

namespace WMSInterface.EntityDefination
{



    [Serializable()]
    public class ProductionBOM
    {
        public String BomId; //PK
        public String ItemId; //主表物料
        public String BomNumber; //BOM编号
        public String Priority; //优先级
        //public String StateCode; //类型，可用
        public String Description; //描述
        public String BomCode; //BOM编码
        public String Version; //版本
        public Decimal Qty; //主表物料数量
        public String ItemName;
        //public List<ProductionBOMDetail> Details; //无序列表，根据选定主表再读取
        //public String ItemId_Code;
        //public String ItemId_Name;
        //public String ItemId_Description;
        //public Decimal ItemId_ItemUnitRate;
        //public String ItemId_ItemUnitName;
    }
    /// <summary>
    /// 用来插入时服务端和客户端通信用。by xsj,2011-11-02
    /// </summary>
    [Serializable()]
    public class ProductionBOMDetailSingle
    {
        public String WarehouseID;
        public String BOMID;
        public String DetailID; 
        public String ItemID;
        public String ItemName;
        public decimal Qty;
        public decimal QtyPercent;
        public String Remark;
        public String UserID;
        public String SequenctNumber;
        public String Field1;
        public String Field2;
        public String Field3;
        public int InsertType;
    }

    [Serializable()]
    public class ProductionBOMDetail
    {
        public String DetailId; //PK
        public String SequenceNumber; //排序号 nvarchar 50
        public List<ProductionBOMItem> Items; //有序列表
    }

    [Serializable()]
    public class ProductionBOMItem
    {
        public String ItemId; //物料
        public Decimal Qty; //数量
        public String Remark; //备注
        public String ItemId_Code;
        public String ItemId_Name;
        public String ItemId_Description;
        public Decimal ItemId_ItemUnitRate;
        public String ItemId_ItemUnitName;
    }





    [Serializable()]
    public class ProductionBOMSelectedItem
    {
        public String DetailId; //PK @ Detail
        public String ItemId; //物料
    }

}
