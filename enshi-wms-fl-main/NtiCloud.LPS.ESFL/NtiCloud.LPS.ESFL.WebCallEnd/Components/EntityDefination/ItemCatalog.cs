using System;
using System.Collections.Generic;
using System.Text;

namespace WMSInterface.EntityDefination
{
    [Serializable]
    public class ItemCatalog
    {
        public String WarehouseId;
        public String CatalogId;
        public String ParentCatalogId;
        public String Name;
        public String Description;
        public String TypeCode;
        public String StateCode;       
        public String UserId;
        public DateTime CreateTime;
        public String Field1;//code
        public String Field2;
        public String Field3;

        public override string ToString()
        {
            return String.Format("种类ID：{0} ParentCatalogId：{1} 名称：{2} 描述：{3} 类型：{4} 状态：{5} 编码：{6}",CatalogId,ParentCatalogId,Name,Description,TypeCode,StateCode,Field1);
        }
    }

    [Serializable]
    public class ItemCatalog_View
    {
        public String WarehouseId;
        public String CatalogId;
        public String ParentCatalogId;
        public String Name;
        public String Description;
        public String TypeCode;
        public String StateCode;        
        public String UserId;
        public DateTime CreateTime;        
        public String Type_Description;
        public String State_Description;
    }
}
