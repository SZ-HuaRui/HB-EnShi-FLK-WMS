using Newtonsoft.Json.Linq;
using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;

namespace NtiCloud.Service.WMS.BaseData
{
    public class BaseData
    {
        /// <summary>
        /// 按物料添加物料单位
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public WebJsonInfo AddItemUomWithItem(dynamic param, UserLogModel log)
        {
            return WmsHelper.ExcuteWithExceDo((info) =>
            {
                string strSql, t_id;
                Dictionary<string, object> dicParam = new Dictionary<string, object>();
                t_id = AddOrUpdateUom(param);

                if (param.is_default.ToString() == "1")//默认单位
                {
                    if (param.uom_type.ToString() == "UomType_Pack")//物流单位
                    {
                        //物料单位表
                        strSql = @"update &DataSource&WMS_M_ITEM_UOM  a set a.is_default = '0' 
                                         where a.item_id=@item_id 
                                         and  a.uom_type='UomType_Pack' 
                                         and a.t_id!=@t_id and a.act_status=1  
                                         and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&' 
                                         and a.warehouse_id = @warehouse_id";
                        dicParam.Clear();
                        dicParam.Add("item_id", param.p_t_id.ToString());
                        dicParam.Add("t_id", t_id);
                        dicParam.Add("warehouse_id", Context.WarehouseID);
                        DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);

                        //供应商
                        strSql = @"update &DataSource&wms_m_item_supplier  b set b.default_pack_uom_id=@default_pack_uom_id  where b.item_id=@item_id
                                   and b.client_id='&CLIENT_ID&' and b.org_id='&ORG_ID&' 
                                       ";
                        dicParam.Clear();
                        dicParam.Add("default_pack_uom_id", t_id);
                        dicParam.Add("item_id", param.p_t_id.ToString());
                        dicParam.Add("warehouse_id", Context.WarehouseID);
                        DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);

                        //物料表
                        strSql = @"update &DataSource&WMS_M_ITEM  a set a.default_pack_uom_id=@default_pack_uom_id where a.t_id=@t_id
                                         and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&' 
                                         and a.warehouse_id = @warehouse_id";
                        dicParam.Clear();
                        dicParam.Add("t_id", param.p_t_id.ToString());
                        dicParam.Add("default_pack_uom_id", t_id);
                        dicParam.Add("warehouse_id", Context.WarehouseID);
                        DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                    }
                    else
                    {
                        //物料单位表
                        strSql = @"update &DataSource&WMS_M_ITEM_UOM  a set a.is_default = '0' 
                                        where a.item_id=@item_id 
                                        and  a.uom_type='UomType_Stat' 
                                        and a.t_id!=@t_id and a.act_status=1
                                        and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&' 
                                        and a.warehouse_id = @warehouse_id";
                        dicParam.Clear();
                        dicParam.Add("item_id", param.p_t_id.ToString());
                        dicParam.Add("t_id", t_id);
                        dicParam.Add("warehouse_id", Context.WarehouseID);
                        DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);

                        //供应商             
                        strSql = @"update &DataSource&wms_m_item_supplier  b set b.default_stat_uom_id=@default_stat_uom_id  where b.item_id=@item_id
                            and b.client_id='&CLIENT_ID&' and b.org_id='&ORG_ID&' 
                            ";
                        dicParam.Clear();
                        dicParam.Add("default_stat_uom_id", t_id);
                        dicParam.Add("item_id", param.p_t_id.ToString());
                        dicParam.Add("warehouse_id", Context.WarehouseID);
                        DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);

                        //物料表
                        strSql = @"update &DataSource&WMS_M_ITEM  a set a.default_stat_uom_id=@default_stat_uom_id where a.t_id=@t_id
                         and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&' 
                                         and a.warehouse_id = @warehouse_id";
                        dicParam.Clear();
                        dicParam.Add("t_id", param.p_t_id.ToString());
                        dicParam.Add("default_stat_uom_id", t_id);
                        dicParam.Add("warehouse_id", Context.WarehouseID);
                        DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                    }
                }
                else
                {
                    UpdataItemAndSupplier(param.p_t_id.ToString());
                }

                return info;
            },log);
        }

        /// <summary>
        /// 按物料删除物料单位
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public WebJsonInfo DelItemUomWithItem(dynamic param, UserLogModel log)
        {
            return WmsHelper.ExcuteWithExceDo((info) =>
            {
                if (string.IsNullOrEmpty(param.r1_t_id.ToString()))
                {
                    // 出错
                    info.errcode = 500;
                    info.errmsg = "t_id不能为空，停止处理";
                    return info;
                }
                string strSql;
                Dictionary<string, object> dicParam = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(param.default_pack_uom_id.ToString())
                    && param.r1_t_id.ToString() == param.default_pack_uom_id.ToString()
                    && param.uom_type.ToString() == "UomType_Pack")
                {
                    strSql = @"update &DataSource&WMS_M_ITEM  a set a.default_pack_uom_id=null where a.t_id=@t_id  
                              and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id = @warehouse_id";
                    dicParam.Add("t_id", param.r0_t_id.ToString());
                    dicParam.Add("warehouse_id", Context.WarehouseID);
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                    strSql = @"update &DataSource&wms_m_item_supplier  a set a.default_pack_uom_id=null where a.item_id=@t_id 
                    and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id = @warehouse_id";
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                }
                if (!string.IsNullOrEmpty(param.default_stat_uom_id.ToString())
                    && param.r1_t_id.ToString() == param.default_stat_uom_id.ToString()
                    && param.uom_type.ToString() == "UomType_Stat")
                {
                    dicParam.Clear();
                    strSql = @"update &DataSource&WMS_M_ITEM  a set a.default_stat_uom_id=null where a.t_id=@t_id
                               and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id = @warehouse_id";
                    dicParam.Add("t_id", param.r0_t_id.ToString());
                    dicParam.Add("warehouse_id", Context.WarehouseID);
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                    strSql = @"update &DataSource&wms_m_item_supplier  a set a.default_stat_uom_id=null where a.item_id=@t_id
                             and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id = @warehouse_id";
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                }

                dicParam.Clear();
                strSql = @"update &DataSource&WMS_M_ITEM_UOM  a set a.ACT_STATUS=0 where a.t_id=@t_id
                and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id = @warehouse_id";
                dicParam.Add("t_id", param.r1_t_id.ToString());
                dicParam.Add("warehouse_id", Context.WarehouseID);
                DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);

                return info;
            },log);
        }

        /// <summary>
        /// 按物料种类添加物料单位
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public WebJsonInfo AddItemUomWithCata(dynamic param, UserLogModel log)
        {
            return WmsHelper.ExcuteWithExceDo((info) =>
            {
                string strSql, t_id;
                Dictionary<string, object> dicParam = new Dictionary<string, object>();
                t_id = AddOrUpdateUom(param);

                if (param.is_default.ToString() == "1")//默认单位
                {
                    if (param.uom_type.ToString() == "UomType_Pack")//物流单位
                    {
                        //物料单位表
                        strSql = @"update &DataSource&WMS_M_ITEM_UOM  a set a.is_default = '0'
                                 where a.item_catalog_id=@item_catalog_id 
                                and  a.uom_type='UomType_Pack' 
                                and a.t_id!=@t_id and a.act_status=1
                                and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id = @warehouse_id";
                        dicParam.Add("item_catalog_id", param.p_t_id.ToString());
                        dicParam.Add("t_id", t_id);
                        dicParam.Add("warehouse_id", Context.WarehouseID);
                        DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);

                        //物料种类表
                        strSql = @"update &DataSource&WMS_M_ITEM_CATALOG  a set a.default_pack_uom_id=@default_pack_uom_id where a.t_id=@t_id
                                   and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id = @warehouse_id";
                        dicParam.Clear();
                        dicParam.Add("default_pack_uom_id", t_id);
                        dicParam.Add("t_id", param.p_t_id.ToString());
                        dicParam.Add("warehouse_id", Context.WarehouseID);
                        DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                    }
                    else
                    {
                        //物料单位表
                        strSql = @"update &DataSource&WMS_M_ITEM_UOM  a set a.is_default='0'  where a.item_catalog_id=@item_catalog_id and  a.uom_type='UomType_Stat' and a.t_id!=@t_id and a.act_status=1
                                 and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id = @warehouse_id";
                        dicParam.Add("item_catalog_id", param.p_t_id.ToString());
                        dicParam.Add("t_id", t_id);
                        dicParam.Add("warehouse_id", Context.WarehouseID);
                        DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);

                        //物料种类表
                        strSql = @"update &DataSource&WMS_M_ITEM_CATALOG  a set a.default_stat_uom_id=@default_stat_uom_id where a.t_id=@t_id
                                and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id = @warehouse_id";
                        dicParam.Clear();
                        dicParam.Add("default_stat_uom_id", t_id);
                        dicParam.Add("t_id", param.p_t_id.ToString());
                        dicParam.Add("warehouse_id", Context.WarehouseID);
                        DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                    }
                }
                else
                {
                    //物料单位表
                    strSql = @"update &DataSource&WMS_M_ITEM_CATALOG  a set a.default_pack_uom_id=null where a.default_pack_uom_id=@default_pack_uom_id
                              and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id = @warehouse_id";
                    dicParam.Add("default_pack_uom_id", t_id);
                    dicParam.Add("warehouse_id", Context.WarehouseID);
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);

                    //物料种类表
                    strSql = @"update &DataSource&WMS_M_ITEM_CATALOG  a set a.default_stat_uom_id=null where a.default_stat_uom_id=@default_stat_uom_id
                               and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id = @warehouse_id";
                    dicParam.Clear();
                    dicParam.Add("default_stat_uom_id", t_id);
                    dicParam.Add("warehouse_id", Context.WarehouseID);
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                }

                return info;
            },log);
        }

        /// <summary>
        /// 按物料种类删除物料单位
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public WebJsonInfo DelItemUomWithCata(dynamic param, UserLogModel log)
        {
            return WmsHelper.ExcuteWithExceDo((info) =>
            {
                if (string.IsNullOrEmpty(param.r1_t_id.ToString()))
                {
                    // 出错
                    info.errcode = 500;
                    info.errmsg = "t_id不能为空，停止处理";
                    return info;
                }
                string strSql;
                Dictionary<string, object> dicParam = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(param.default_pack_uom_id.ToString())
                    && param.r1_t_id.ToString() == param.default_pack_uom_id.ToString()
                    && param.uom_type.ToString() == "UomType_Pack")
                {
                    strSql = @"update &DataSource&WMS_M_ITEM_CATALOG  a set a.default_pack_uom_id=null where a.t_id=@t_id
                               and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id = @warehouse_id";
                    dicParam.Add("t_id", param.r0_t_id.ToString());
                    dicParam.Add("warehouse_id", Context.WarehouseID);
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                }
                if (!string.IsNullOrEmpty(param.default_stat_uom_id.ToString())
                    && param.r1_t_id.ToString() == param.default_stat_uom_id.ToString()
                    && param.uom_type.ToString() == "UomType_Stat")
                {
                    dicParam.Clear();
                    strSql = @"update &DataSource&WMS_M_ITEM_CATALOG  a set a.default_stat_uom_id=null where a.t_id=@t_id
                            and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id = @warehouse_id";
                    dicParam.Add("t_id", param.r0_t_id.ToString());
                    dicParam.Add("warehouse_id", Context.WarehouseID);
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                }

                dicParam.Clear();
                strSql = @"update &DataSource&WMS_M_ITEM_UOM  a set a.ACT_STATUS=0 where a.t_id=@t_id
                         and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id = @warehouse_id";
                dicParam.Add("t_id", param.r1_t_id.ToString());
                dicParam.Add("warehouse_id", Context.WarehouseID);
                DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);

                return info;
            },log);
        }

        /// <summary>
        /// 配方单操作
        /// </summary>
        /// <param name="param"></param>
        /// <param name="optType"></param>
        /// <returns></returns>
        public WebJsonInfo BomProductionOpt(dynamic param, int optType, UserLogModel log)
        {
            return WmsHelper.ExcuteWithExceDo((info) =>
            {
                string p = JsonSerializer.Serialize(param);
                param = JObject.Parse(p);
                string strSql;

                Dictionary<string, object> dicParam = new Dictionary<string, object>();
                if (optType == 1) //删除配方明细
                {
                    if (string.IsNullOrEmpty(param.t_id.ToString()))
                    {
                        // 出错
                        info.errcode = 500;
                        info.errmsg = "t_id不能为空，停止处理";
                        return info;
                    }

                    strSql = @"select count(*) cnt from &DataSource&wms_m_bom_d  where bom_d_id=@t_id and act_status=1
                               and client_id='&CLIENT_ID&' and org_id='&ORG_ID&'  and warehouse_id = @warehouse_id";
                    dicParam.Add("warehouse_id", Context.WarehouseID);
                    dicParam.Add("t_id", param.t_id.ToString());
                    dicParam.Add("h_t_id", param.h_t_id.ToString());

                    DataTable dt = DBHelper.Select(strSql, dicParam);

                    if (decimal.Parse("0" + dt.Rows[0]["cnt"]) > 0)
                    {
                        info.errcode = 500;
                        info.errmsg = "该记录存在替换物料明细，不能删除！";

                        return info;
                    }
                    else
                    {
                        strSql = @"UPDATE &DataSource&wms_m_bom_d SET ACT_STATUS= 0 where t_id=@t_id
                                 and client_id='&CLIENT_ID&' and org_id='&ORG_ID&'  and warehouse_id = @warehouse_id";
                        DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);

                        strSql = @"update &DataSource&wms_m_bom_d dd
                                   set dd.seq_no =
                                       (select num
                                          from (select rownum num, t_id
                                                  from (select t_id
                                                          from &DataSource&wms_m_bom_d
                                                         where act_status = 1
                                                           and bom_h_id = @h_t_id  and client_id='&CLIENT_ID&' and org_id='&ORG_ID&'  and warehouse_id=@warehouse_id
                                                         order by seq_no) d0) d
                                         where d.t_id = dd.t_id)
                                 where dd.act_status = 1
                                   and dd.bom_h_id = @h_t_id";
                        DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                    }
                }
                else if (optType == 2) //上移
                {
                    dicParam.Add("pre_seq_no", param.pre_seq_no.ToString());
                    dicParam.Add("pre_t_id", param.pre_t_id.ToString());
                    dicParam.Add("cur_seq_no", param.cur_seq_no.ToString());
                    dicParam.Add("cur_t_id", param.cur_t_id.ToString());
                    dicParam.Add("warehouse_id", Context.WarehouseID);

                    strSql = @"UPDATE &DataSource& wms_m_bom_d d SET d.seq_no = @pre_seq_no where d.t_id =@cur_t_id
                                and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&'  and d.warehouse_id=@warehouse_id";
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                    strSql = @"UPDATE &DataSource& wms_m_bom_d d SET d.seq_no = @cur_seq_no where d.t_id =@pre_t_id
                               and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&'  and d.warehouse_id=@warehouse_id";
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                }
                else if (optType == 3)//下移
                {
                    dicParam.Add("next_seq_no", param.next_seq_no.ToString());
                    dicParam.Add("next_t_id", param.next_t_id.ToString());
                    dicParam.Add("cur_seq_no", param.cur_seq_no.ToString());
                    dicParam.Add("cur_t_id", param.cur_t_id.ToString());
                    dicParam.Add("warehouse_id", Context.WarehouseID);

                    strSql = @"UPDATE &DataSource& wms_m_bom_d d SET d.seq_no = @next_seq_no where d.t_id =@cur_t_id
                            and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&'  and d.warehouse_id=@warehouse_id";
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                    strSql = @"UPDATE &DataSource& wms_m_bom_d d SET d.seq_no = @cur_seq_no where d.t_id =@next_t_id
                            and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&'  and d.warehouse_id=@warehouse_id";
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                }

                return info;
            },log);
        }

        /// <summary>
        /// 配方单替换物料操作
        /// </summary>
        /// <param name="param"></param>
        /// <param name="optType"></param>
        /// <returns></returns>
        public WebJsonInfo BomChangeItemOpt(dynamic param, int optType, UserLogModel log)
        {
            return WmsHelper.ExcuteWithExceDo((info) =>
            {
                string p = JsonSerializer.Serialize(param);
                param = JObject.Parse(p);
                string strSql;

                Dictionary<string, object> dicParam = new Dictionary<string, object>();
                if (optType == 1) //删除配方明细
                {
                    if (string.IsNullOrEmpty(param.t_id.ToString()))
                    {
                        // 出错
                        info.errcode = 500;
                        info.errmsg = "t_id不能为空，停止处理";
                        return info;
                    }
                    dicParam.Add("t_id", param.t_id.ToString());
                    dicParam.Add("bom_d_id", param.bom_d_id.ToString());
                    dicParam.Add("warehouse_id", Context.WarehouseID);

                    strSql = @"UPDATE &DataSource&wms_m_bom_d SET ACT_STATUS= 0 where t_id=@t_id
                             and client_id='&CLIENT_ID&' and org_id='&ORG_ID&'  and warehouse_id=@warehouse_id ";
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);

                    strSql = @"update  &DataSource&wms_m_bom_d dd
                               set dd.level_no =
                                  ( 
                                     select num from ( 
                                         select rownum num,t_id
                                            from (
                                             select t_id from  &DataSource&wms_m_bom_d  where act_status=1  
                                             and  bom_d_id=@bom_d_id  and client_id='&CLIENT_ID&' and org_id='&ORG_ID&'  and warehouse_id=@warehouse_id  order by seq_no 
                                          ) d0
                                     )d
                                     where d.t_id = dd.t_id
                                   )
                               where dd.act_status=1 and dd.bom_d_id=@bom_d_id";
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);

                }
                else if (optType == 2) //上移
                {
                    dicParam.Add("pre_level_no", param.pre_level_no.ToString());
                    dicParam.Add("pre_t_id", param.pre_t_id.ToString());
                    dicParam.Add("cur_level_no", param.cur_level_no.ToString());
                    dicParam.Add("cur_t_id", param.cur_t_id.ToString());
                    dicParam.Add("warehouse_id", Context.WarehouseID);

                    strSql = @"UPDATE &DataSource&wms_m_bom_d d SET d.level_no = @pre_level_no where d.t_id =@cur_t_id
                     and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&'  and d.warehouse_id=@warehouse_id";
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                    strSql = @"UPDATE &DataSource&wms_m_bom_d d SET d.level_no = @cur_level_no where d.t_id =@pre_t_id
                    and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&'  and d.warehouse_id=@warehouse_id";
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                }
                else if (optType == 3)//下移
                {
                    dicParam.Add("next_level_no", param.next_level_no.ToString());
                    dicParam.Add("next_t_id", param.next_t_id.ToString());
                    dicParam.Add("cur_level_no", param.cur_level_no.ToString());
                    dicParam.Add("cur_t_id", param.cur_t_id.ToString());
                    dicParam.Add("warehouse_id", Context.WarehouseID);

                    strSql = @"UPDATE &DataSource&wms_m_bom_d d SET d.level_no = @next_level_no where d.t_id =@cur_t_id
                             and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&'  and d.warehouse_id=@warehouse_id";
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                    strSql = @"UPDATE &DataSource&wms_m_bom_d d SET d.level_no = @cur_level_no where d.t_id =@next_t_id
                             and d.client_id='&CLIENT_ID&' and d.org_id='&ORG_ID&'  and d.warehouse_id=@warehouse_id";
                    DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
                }

                return info;
            },log);
        }

        public WebJsonInfo BatchAddLoc(dynamic param, UserLogModel log)
        {
            return WmsHelper.ExcuteWithExceDo((info) =>
            {
                if (string.IsNullOrEmpty(param.limitParam.ToString())
                || string.IsNullOrEmpty(param.strFormat.ToString())
                || string.IsNullOrEmpty(param.locInfo.ToString()))
                {
                    // 出错
                    info.errcode = 500;
                    info.errmsg = "存在货位参数为空";

                    return info;
                }

                LocationParam limitParam = Newtonsoft.Json.JsonConvert.DeserializeObject<LocationParam>(param.limitParam.ToString());
                string strFormat = param.strFormat.ToString();
                LocationModel locInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<LocationModel>(param.locInfo.ToString());

                BatchCreateLoc batchCreateLoc = new BatchCreateLoc();
                DataRow dr = DBHelper.Select(@"select l.no_format,l.code_format,l.name_format from &DataSource&wms_m_location_format l where  l.t_id=@t_id 
                    and l.client_id='&CLIENT_ID&' and l.org_id='&ORG_ID&'", new Dictionary<string, object>() {
                    //{"warehouse_id",Context.WarehouseID },
                    {"t_id",strFormat }
                }, "货位编码格式", true).Rows[0];

                Tuple<bool, string> check = batchCreateLoc.CheckLocFormat(dr["code_format"].ToString(), dr["no_format"].ToString(), dr["name_format"].ToString());//校验格式

                if (!check.Item1)
                {
                    // 出错
                    info.errcode = 500;
                    info.errmsg = check.Item2;

                    return info;
                }

                List<Dictionary<string, object>> parameters = batchCreateLoc.CreateLocParam(limitParam,
                    dr["code_format"].ToString(),
                    dr["no_format"].ToString(),
                    dr["name_format"].ToString(),locInfo);
                List<string> sqls = new List<string>();
                for (int i = 0; i < parameters.Count; i++)
                {
                    sqls.Add(batchCreateLoc.InsLocSql());
                }

                DBHelper.RunSqls(sqls, parameters);

                return info;
            },log);
        }

        public WebJsonInfo UpdateZoneSqlWhere(dynamic param, UserLogModel log)
        {
            return WmsHelper.ExcuteWithExceDo((info) =>
            {
                if (string.IsNullOrEmpty(param.t_id.ToString()))
                {
                    // 出错
                    info.errcode = 500;
                    info.errmsg = "参数t_id为空";

                    return info;
                }

                DataTable dt = DBHelper.Select(@"select * from &DataSource&WMS_SI_ZONESQL_D where CLIENT_ID='&CLIENT_ID&' and ORG_ID='&ORG_ID&' 
                                    and warehouse_id=@warehouse_id
                                    and ZONE_SQL_ID=@t_id order by create_date ", new Dictionary<string, object>() {
                                { "warehouse_id",Context.WarehouseID },
                                { "t_id",param.t_id.ToString()}
                    }, "区域策略明细", true);

                string strWhere = string.Empty;

                foreach (DataRow item in dt.Rows)
                {
                    string temp = $" {item["logic"].ToString()} {item["columns_name"].ToString()}{item["condition"].ToString()} '{item["rule_value"].ToString()}'";

                    strWhere += $"{temp}";
                }

                strWhere = $"(1=1 {strWhere})";

                DBHelper.RunSql(@"update  &DataSource&WMS_SI_ZONESQL_H h set h.zone_sql=@zone_sql where  h.CLIENT_ID='&CLIENT_ID&' and h.ORG_ID='&ORG_ID&'
                                 and h.t_id=@t_id 
                                 and h.warehouse_id=@warehouse_id", new Dictionary<string, object>() {
                          { "warehouse_id",Context.WarehouseID },
                          { "t_id",param.t_id.ToString()},
                          { "zone_sql",strWhere }
                    }, "更新区域策略sql值");

                return info;
            }, log);
        }

        #region 私有成员

        /// <summary>
        /// 更新物料和供应商默认单位字段
        /// </summary>
        /// <param name="p_t_id">物料单位id</param>
        private void UpdataItemAndSupplier(string p_t_id)
        {
            Dictionary<string, object> dicParam = new Dictionary<string, object>
            {
                 { "default_pack_uom_id", p_t_id },
                 { "warehouse_id", Context.WarehouseID }
            };
            string strSql = @"update &DataSource&wms_m_item_supplier  a set a.default_pack_uom_id=null where a.default_pack_uom_id=@default_pack_uom_id
                        and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id=@warehouse_id";
            DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
            strSql = @"update &DataSource&WMS_M_ITEM  a set a.default_pack_uom_id=null where a.default_pack_uom_id=@default_pack_uom_id
                    and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id=@warehouse_id";
            DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);

            dicParam.Clear();
            dicParam.Add("default_stat_uom_id", p_t_id);
            dicParam.Add("warehouse_id", Context.WarehouseID);
            strSql = @"update &DataSource&wms_m_item_supplier  a set a.default_stat_uom_id=null where a.default_stat_uom_id=@default_stat_uom_id
               and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id=@warehouse_id";
            DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
            strSql = @"update &DataSource&WMS_M_ITEM  a set a.default_stat_uom_id=null where a.default_stat_uom_id=@default_stat_uom_id
               and a.client_id='&CLIENT_ID&' and a.org_id='&ORG_ID&'  and a.warehouse_id=@warehouse_id ";
            DBHelper.RunSql(strSql, dicParam, string.Empty, DBHelper.OperatedNumType.ZeroOrMore);
        }

        /// <summary>
        /// 更新或添加物料单位
        /// </summary>
        /// <param name="param"></param>
        private string AddOrUpdateUom(dynamic param)
        {
            string strSql, t_id;
            if (!string.IsNullOrEmpty(param.t_id.ToString()))
            {
                strSql = ItemUomCurdSql(SqlType.U);
                t_id = param.t_id.ToString();
            }
            else
            {
                strSql = ItemUomCurdSql(SqlType.C);
                t_id = SnowflakeID.Guid().ToString();
            }

            Dictionary<string, object> dicParam = new Dictionary<string, object>() {
                    { "child_id", param.child_id.ToString() },
                    { "is_default", param.is_default.ToString() },
                    { "is_disabled", param.is_disabled.ToString() },
                    { "is_min_io_pack_uom", param.is_min_io_pack_uom.ToString() },
                    { "is_min_uom",  param.is_min_uom.ToString() },
                    { "is_visible", param.is_visible.ToString() },
                    { "item_catalog_id", param.item_catalog_id.ToString() },
                    { "item_id", param.item_id.ToString() },
                    { "rate",param.rate.ToString() },
                    { "seq_no", param.seq_no.ToString() },
                    { "spec", param.spec.ToString() },
                    { "supplier_id", param.supplier_id.ToString() },
                    { "t_id", t_id },
                    { "tag", param.tag.ToString() },
                    { "uom_category", param.uom_category.ToString() },
                    { "uom_desc", param.uom_desc.ToString() },
                    { "uom_id", param.uom_id.ToString() },
                    { "uom_name", param.uom_name.ToString() },
                    { "uom_type", param.uom_type.ToString() },
                    { "warehouse_id", param.warehouse_id.ToString() },
                    { "last_modified_by", Context.UserInfo.UserName },
                    { "modify_ip", Context.UserInfo.UserName },
                    { "created_by", Context.UserInfo.UserName },
                  };

            DBHelper.RunSql(strSql, dicParam);

            return t_id;
        }

        private string ItemUomCurdSql(SqlType type)
        {
            string strSql = string.Empty;

            if (type == SqlType.C)
            {
                strSql = @" INSERT INTO &DataSource&WMS_M_ITEM_UOM
                              (CLIENT_ID, ORG_ID, t_id, warehouse_id,
                               item_id,
                               item_catalog_id,
                               tag,
                               uom_id,
                               uom_name,
                               uom_desc,
                               uom_type,
                               uom_category,
                               spec,
                               is_visible,
                               is_default,
                               is_min_io_pack_uom,
                               is_min_uom,
                               seq_no,
                               rate,
                               supplier_id,
                               child_id,
                               is_disabled,
                               CREATED_BY,
                               MODIFY_IP,
                               CREATE_DATE,
                               ACT_STATUS)VALUES
                              ('&CLIENT_ID&',
                               '&ORG_ID&',
                               @t_id,
                               '&WAREHOUSE_ID&',
                               @item_id,
                               @item_catalog_id,
                               @tag,
                               @uom_id,
                              (select uom_name from &DataSource&wms_m_uom where t_id= @uom_id),
                               @uom_desc,
                               @uom_type,
                               @uom_category,
                               @spec,
                               @is_visible,
                               @is_default,
                               @is_min_io_pack_uom,
                               @is_min_uom,
                               @seq_no,
                               @rate,
                               @supplier_id,
                               @child_id,
                               @is_disabled,
                               @created_by,
                               @modify_ip,
                               now(),
                               '1')";
            }
            else if (type == SqlType.U)
            {
                strSql = @"UPDATE &DataSource&WMS_M_ITEM_UOM
                              SET CLIENT_ID          = '&CLIENT_ID&',
                                  ORG_ID             = '&ORG_ID&',
                                  item_id            = @item_id,
                                  item_catalog_id    = @item_catalog_id,
                                  tag                = @tag,
                                  uom_id             = @uom_id,
                                  uom_name           = (select uom_name from &DataSource&wms_m_uom where t_id=@uom_id),
                                  uom_desc           = @uom_desc,
                                  uom_type           = @uom_type,
                                  uom_category       = @uom_category,
                                  spec               = @spec,
                                  is_visible         = @is_visible,
                                  is_default         = @is_default,
                                  is_min_io_pack_uom = @is_min_io_pack_uom,
                                  is_min_uom         = @is_min_uom,
                                  seq_no             = @seq_no,
                                  rate               = @rate,
                                  supplier_id        = @supplier_id,
                                  child_id           = @child_id,
                                  is_disabled        = @is_disabled,
                                  LAST_MODIFIED_BY   = @last_modified_by,
                                  MODIFY_IP          = @modify_ip,
                                  MODIFY_DATE        = now()
                            WHERE t_id = @t_id";
            }

            return strSql;
        }

        #endregion
    }
}