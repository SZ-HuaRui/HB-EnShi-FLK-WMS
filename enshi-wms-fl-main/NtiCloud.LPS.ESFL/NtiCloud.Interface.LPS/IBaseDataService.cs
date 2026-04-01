using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Interface.WMS
{
    [Post("/api/wms/{ServiceName}/{MethodName}")]
    public interface IBaseDataService
    {
        /// <summary>
        /// 按物料添加物料单位
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Description(description: "按物料添加物料单位",
         inputDescription: "",
         outputDescription: "")]
        WebJsonInfo AddItemUomWithItem(dynamic param);

        /// <summary>
        /// 按物料删除物料单位
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Description(description: "按物料删除物料单位",
         inputDescription: @"{
                              r0_t_id: '物料id',
                              r1_t_id: '物料单位id',
                              default_stat_uom_id: '默认库存统计单位id',
                              default_pack_uom_id: '默认库存物流单位id',
                              supplier_id: '供应商id',
                             }",
         outputDescription: "")]
        WebJsonInfo DelItemUomWithItem(dynamic param);

        /// <summary>
        /// 按物料种类添加物料单位
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Description(description: "按物料种类添加物料单位",
         inputDescription: "",
         outputDescription: "")]
        WebJsonInfo AddItemUomWithCata(dynamic param);

        /// <summary>
        /// 按物料种类删除物料单位
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Description(description: "按物料种类删除物料单位",
         inputDescription: @"{
                              r0_t_id: '物料id',
                              r1_t_id: '物料单位id',
                              default_stat_uom_id: '默认库存统计单位id',
                              default_pack_uom_id: '默认库存物流单位id'
                             }",
         outputDescription: "")]
        WebJsonInfo DelItemUomWithCata(dynamic param);

        /// <summary>
        /// 配方单操作
        /// </summary>
        /// <param name="param"></param>
        /// <param name="optType">操作类别</param>
        /// <returns></returns>
        [Description(description: "配方单操作",
         inputDescription: @"param:{ t_id:'配方明细id' ,h_t_id:'配方单主档id'} 
                                    or {pre_seq_no: '前一个行序号',
                                        pre_t_id: '前一个行id',
                                        cur_seq_no: '当前行序号',
                                        cur_t_id:'当前行id'} 
                                    or {next_seq_no: '下一个行序号',
                                        next_t_id: '下一个行id',
                                        cur_seq_no: '当前行序号',
                                        cur_t_id:'当前行id'} 
                             optType:1.删除配方单明细 2.上移 3.下移",
        outputDescription: "")]
        WebJsonInfo BomProductionOpt(dynamic param, int optType);

        /// <summary>
        /// 配方单替换物料操作
        /// </summary>
        /// <param name="param"></param>
        /// <param name="optType">操作类别</param>
        /// <returns></returns>
        [Description(description: "配方单替换物料操作",
         inputDescription: "param:参数  optType:1.删除替换物料明细 2.上移 3.下移",
         outputDescription: "")]
        WebJsonInfo BomChangeItemOpt(dynamic param, int optType);

        /// <summary>
        /// 批量添加货位
        /// </summary>
        /// <returns></returns>
        [Description(description: "批量添加货位",
         inputDescription: "",
         outputDescription: "param:参数  limitParam:限制参数范围 strFormat:货位格式  locInfo:货位信息")]
        WebJsonInfo BatchAddLoc(dynamic param);

        /// <summary>
        /// 生成区域策略定义的zone_sql
        /// </summary>
        /// <returns></returns>
        [Description(description: "生成区域策略定义的zone_sql",
         inputDescription: "",
         outputDescription: "param:参数  t_id:主表id")]
        WebJsonInfo UpdateZoneSqlWhere(dynamic param);
    }
}
