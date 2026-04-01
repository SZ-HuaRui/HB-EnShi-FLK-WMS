using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WMSInterface.EntityDefination
{
    [Serializable()]
    public class BOMHead
    {
        public String BomId; //PK
        public String ItemId; //主表物料
        public String ItemName;
        public String BomNumber; //BOM编号
        public String Priority; //优先级
        /// <summary>
        /// BOMNumber(配方单) State
        /// </summary>
        public String Bom1Status;
        /// <summary>
        /// BOMCode(配盘单) State
        /// </summary>
        public String Bom2Status;
        public String StateCode; //类型，可用
        public String Description; //描述
        public String BomCode; //BOM编码
        public String Version; //版本
        public Decimal Qty; //主表物料数量
        public String ModelCode;//区段
        public String EquipmentTypeCode;//机型
        /// <summary>
        /// 配盘单名称
        /// </summary>
        public String Field1;
        /// <summary>
        /// 单据类型：BOM_Process配盘单；BOM_Prduction配方单
        /// </summary>
        public String BomType;
        public Decimal BomUnitRate;//单位转换率
        public String BomUnitCName;//单位名称

    }

     [Serializable()]
    public class BOMHeadAndDetails :BOMHead
    {
        public List<BOMDetailItem> Details = new List<BOMDetailItem>();
        public override string ToString()
        {
            return String.Format(@"BOMID：{0} ITEMID：{1} 物料名称：{2} 配方单号：{3} 配方单状态：{4} 配盘单号：{5} 配盘单名称：{6} 配盘单描述：{7} 配盘单状态：{8} 区段：{9} 
机型：{10} 版本：{11} 优先级：{12} 数量：{13} 单据类型:{14}",BomId, ItemId, ItemName, BomNumber, Bom1Status, BomCode, Field1, Description, Bom2Status, ModelCode, EquipmentTypeCode, Version, Priority, Qty, BomType);
        }
    }

    [Serializable()]
    public class BOMDetailItem
    {
        public String BomId;
        public String DetailId;
        public String ItemId; //物料
        public String ItemName;
        public Decimal Qty; //数量
        public String Remark; //备注
        public Decimal ItemUnitRate;//单位转换率
        public String ItemUnitCName;//单位名称
    }
    [Serializable()]
    public class BOMInfo
    {
        public String BomId; //PK
        public String ItemId; //主表物料
        public String ItemName;
        public String BomNumber; //BOM编号
        public String Priority; //优先级
        /// <summary>
        /// BOMNumber(配方单) State
        /// </summary>
        public String Bom1Status;
        /// <summary>
        /// BOMCode(配盘单) State
        /// </summary>
        public String Bom2Status;
        public String StateCode; //类型，可用
        public String Description; //描述
        public String BomCode; //BOM编码
        public String Version; //版本
        public Decimal TotalQty; //主表物料数量
        public String ModelCode;//区段
        public String EquipmentTypeCode;//机型
        /// <summary>
        /// 配盘单名称
        /// </summary>
        public String Field1;
        /// <summary>
        /// 单据类型：BOM_Process配盘单；BOM_Prduction配方单
        /// </summary>
        public String BomType;
        public String DetailId;
        public String DetailItemId; //物料
        public String DetailItemName;
        public Decimal Qty; //数量
        public String Remark; //备注
        public Decimal DetailItemUnitRate;//单位转换率
        public String DetailItemUnitCName;//单位名称
    }

    [Serializable()]
    [KnownType(typeof(BomQueryNoCondition))]
    [KnownType(typeof(BomQueryByQueryConditions))]
    [KnownType(typeof(BomQueryByBomId))]
    [DataContract]
    public abstract class BomQueryConditionBase
    {
        public abstract String CreateCondition();
        public abstract String CreateJoin();
    }

    [Serializable()]
    public class BomQueryNoCondition : BomQueryConditionBase
    {

        public override string CreateCondition()
        {
            return "";
        }

        public override string CreateJoin()
        {
            return "";
        }
    }

    [Serializable()]
    public class BomQueryByQueryConditions : BomQueryConditionBase
    {
        public String condition = null;

        public override string CreateCondition()
        {
            return condition.ToString();
        }

        public override string CreateJoin()
        {
            return "";
        }
    }
    [Serializable()]
    public class BomQueryByBomId : BomQueryConditionBase
    {
        public String BomId = null;

        public override string CreateCondition()
        {
            return "and bomid = '" + BomId.Replace("'", "''") + "'";
        }

        public override string CreateJoin()
        {
            return "";
        }
    }
}
