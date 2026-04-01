using NtiCloud.Kernel.Core;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NtiCloud.Service.WMS.BaseData
{
    /// <summary>
    /// 批量生成货位处理
    /// </summary>
    public class BatchCreateLoc
    {

        /// <summary>
        /// 校验货位格式串
        /// </summary>
        /// <param name="strCode">编码</param>
        /// <param name="strNo">编号</param>
        /// <param name="strName">名称</param>
        /// <returns></returns>
        public Tuple<bool, string> CheckLocFormat(string strCode, string strNo, string strName)
        {
            string strMatch = "{}FJHLCSK";

            bool isExist = true;
            string msg=string.Empty;

            if (string.IsNullOrEmpty(strNo) || string.IsNullOrEmpty(strName))
            {
                return new Tuple<bool, string>(false, "货位编号、名称格式存在为空");
            }

            for (int i = 0; i < strNo.Length; i++)
            {
                if (!strMatch.Contains(strNo[i].ToString()))
                {
                    isExist = false;
                    msg = "货位编号错误";
                    break;
                }
            }

            return new Tuple<bool, string>(isExist, msg);
        }

        /// <summary>
        /// 生成货位参数值
        /// </summary>
        /// <param name="param"></param>
        /// <param name="strCodeFormat">货位编码</param>
        /// <param name="strNoFormat">货位编号</param>
        /// <param name="strNameFormat">货位名称</param>
        /// <param name="locationInfo"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> CreateLocParam(LocationParam param, string strCodeFormat, string strNoFormat, string strNameFormat, LocationModel locationInfo)
        {
            Tuple<int, int, bool, string> indexs1 = GetStartEndIndex(param.F_Limit);
            if (!indexs1.Item3)
            {
                throw new WmsException(500, indexs1.Item4);
            }
            Tuple<int, int, bool, string> indexs2 = GetStartEndIndex(param.J_Limit);
            if (!indexs2.Item3)
            {
                throw new WmsException(500, indexs2.Item4);
            }
            Tuple<int, int, bool, string> indexs3 = GetStartEndIndex(param.H_Limit);
            if (!indexs3.Item3)
            {
                throw new WmsException(500, indexs3.Item4);
            }
            Tuple<int, int, bool, string> indexs4 = GetStartEndIndex(param.L_Limit);
            if (!indexs4.Item3)
            {
                throw new WmsException(500, indexs4.Item4);
            }
            Tuple<int, int, bool, string> indexs5 = GetStartEndIndex(param.C_Limit);
            if (!indexs5.Item3)
            {
                throw new WmsException(500, indexs5.Item4);
            }
            Tuple<int, int, bool, string> indexs6 = GetStartEndIndex(param.S_Limit);
            if (!indexs6.Item3)
            {
                throw new WmsException(500, indexs6.Item4);
            }
            Tuple<int, int, bool, string> indexs7 = GetStartEndIndex(param.K_Limit);
            if (!indexs7.Item3)
            {
                throw new WmsException(500, indexs7.Item4);
            }

            List<Dictionary<string, object>> locParam = new List<Dictionary<string, object>>();

            List<LocationModel> location_F = new List<LocationModel>();
            List<LocationModel> location_J = new List<LocationModel>();
            List<LocationModel> location_K = new List<LocationModel>();
            List<LocationModel> location_H = new List<LocationModel>();
            List<LocationModel> location_L = new List<LocationModel>();
            List<LocationModel> location_C = new List<LocationModel>();
            List<LocationModel> location_S = new List<LocationModel>();
            Tuple<bool, List<string>, List<string>, List<string>, string> strChilds = GetFormatInfo(strNoFormat, strCodeFormat, strNameFormat);
            if (!strChilds.Item1)
            {
                throw new WmsException(500, strChilds.Item5);
            }

            List<string> noChilds = strChilds.Item2;
            List<string> nameChilds = strChilds.Item4;

            Tuple<string, string> noName = FindChildNoName(nameChilds, noChilds, "F");//楼层
            location_F = GetPlaceData(new Tuple<int, int>(indexs1.Item1, indexs1.Item2), noName.Item1, noName.Item2);

            noName = FindChildNoName(nameChilds, noChilds, "J");//仓间
            location_J = GetPlaceData(new Tuple<int, int>(indexs2.Item1, indexs2.Item2), noName.Item1, noName.Item2);

            noName = FindChildNoName(nameChilds, noChilds, "H");//行
            location_H = GetPlaceData(new Tuple<int, int>(indexs3.Item1, indexs3.Item2), noName.Item1, noName.Item2);

            noName = FindChildNoName(nameChilds, noChilds, "L");//列
            location_L = GetPlaceData(new Tuple<int, int>(indexs4.Item1, indexs4.Item2), noName.Item1, noName.Item2);

            noName = FindChildNoName(nameChilds, noChilds, "C");//层
            location_C = GetPlaceData(new Tuple<int, int>(indexs5.Item1, indexs5.Item2), noName.Item1, noName.Item2);

            noName = FindChildNoName(nameChilds, noChilds, "S");//深
            location_S = GetPlaceData(new Tuple<int, int>(indexs6.Item1, indexs6.Item2), noName.Item1, noName.Item2);

            noName = FindChildNoName(nameChilds, noChilds, "K");//单元号
            location_K = GetPlaceData(new Tuple<int, int>(indexs7.Item1, indexs7.Item2), noName.Item1, noName.Item2);

            //笛卡尔积
            var data = from f in location_F
                       from j in location_J
                       from k in location_K
                       from h in location_H
                       from l in location_L
                       from c in location_C
                       from s in location_S
                       select new { f, j, k, h, l, c, s };

            foreach (var item in data)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{(item.f.seq_no != 0 ? item.f.location_no : string.Empty)}");
                sb.Append($"{(item.j.seq_no != 0 ? item.j.location_no : string.Empty)}");
                sb.Append($"{(item.k.seq_no != 0 ? item.k.location_no : string.Empty)}");
                sb.Append($"{(item.h.seq_no != 0 ? item.h.location_no : string.Empty)}");
                sb.Append($"{(item.l.seq_no != 0 ? item.l.location_no : string.Empty)}");
                sb.Append($"{(item.c.seq_no != 0 ? item.c.location_no : string.Empty)}");
                sb.Append($"{(item.s.seq_no != 0 ? item.s.location_no : string.Empty)}");
                locationInfo.location_no = sb.ToString();

                sb.Clear();
                sb.Append($"{(item.f.seq_no != 0 ? item.f.location_name : string.Empty)}");
                sb.Append($"{(item.j.seq_no != 0 ? item.j.location_name : string.Empty)}");
                sb.Append($"{(item.k.seq_no != 0 ? item.k.location_name : string.Empty)}");
                sb.Append($"{(item.h.seq_no != 0 ? item.h.location_name : string.Empty)}");
                sb.Append($"{(item.l.seq_no != 0 ? item.l.location_name : string.Empty)}");
                sb.Append($"{(item.c.seq_no != 0 ? item.c.location_name : string.Empty)}");
                sb.Append($"{(item.s.seq_no != 0 ? item.s.location_name : string.Empty)}");
                locationInfo.location_name = sb.ToString();

                locParam.Add(GetLocationModel(item, locationInfo));
            }

            return locParam;
        }

        /// <summary>
        /// 根据索引范围和子串获取货位子串数据
        /// </summary>
        /// <param name="indexs"></param>
        /// <param name="strNo"></param>
        /// <returns></returns>
        private List<LocationModel> GetPlaceData(Tuple<int, int> indexs, string strNo, string strName)
        {
            List<LocationModel> locations = new List<LocationModel>();
            if (string.IsNullOrEmpty(strNo))//货位子串不存在
            {
                locations.Add(new LocationModel()
                {
                    seq_no = 0,
                    location_no = "",
                    location_name = ""
                });
                return locations;
            }

            for (int i = indexs.Item1; i <= indexs.Item2; i++)
            {
                Tuple<string, string> nameAndNo = CreateNameAndNoByChildStr(i, strNo, strName);
                LocationModel item = new LocationModel()
                {
                    seq_no = i,
                    location_no = nameAndNo.Item1,
                    location_name = nameAndNo.Item2
                };

                locations.Add(item);
            }

            return locations;
        }

        /// <summary>
        /// 获取货位格式中所有子串信息
        /// </summary>
        /// <param name="strNoFormat"></param>
        /// <returns></returns>
        public Tuple<bool, List<string>, List<string>, List<string>, string> GetFormatInfo(string strNoFormat, string strCodeFormat, string strNameFormat)
        {
            string temp = string.Empty;
            List<string> listNo = new List<string>();
            List<string> listCode = new List<string>();
            List<string> listName = new List<string>();

            for (int i = 0; i < strNoFormat.Length; i++)
            {
                temp += strNoFormat[i].ToString();

                if (strNoFormat[i].ToString() == "}")
                {
                    listNo.Add(temp);
                    temp = string.Empty;
                }
            }
            for (int i = 0; i < strCodeFormat.Length; i++)
            {
                temp += strCodeFormat[i].ToString();

                if (strCodeFormat[i].ToString() == "}")
                {
                    listCode.Add(temp);
                    temp = string.Empty;
                }
            }

            for (int i = 0; i < listNo.Count; i++)
            {
                Tuple<bool, string> ret = CheckLocNoFormatChild(listNo[i]);
                if (!ret.Item1)
                {
                    return new Tuple<bool, List<string>, List<string>, List<string>, string>(false, null, null, null, "解析货位编号错误");
                }
            }
            for (int i = 0; i < listCode.Count; i++)
            {
                Tuple<bool, string> ret = CheckLocNoFormatChild(listCode[i]);
                if (!ret.Item1)
                {
                    return new Tuple<bool, List<string>, List<string>, List<string>, string>(false, null, null, null, "解析货位编码错误");
                }
            }

            string strName = strNameFormat;

            string[] nameArray = strName.Split("{");

            if (nameArray.Length != (listNo.Count + 1))
            {
                return new Tuple<bool, List<string>, List<string>, List<string>, string>(false, null, null, null, "解析货位名称错误");
            }

            return new Tuple<bool, List<string>, List<string>, List<string>, string>(true, listNo, listCode, nameArray.ToList(), "");
        }

        /// <summary>
        /// 校验货位编号格式子串
        /// </summary>
        /// <param name="strFormat">货位格式串</param>
        /// <returns></returns>
        private Tuple<bool, string> CheckLocNoFormatChild(string strFormat)
        {
            if (strFormat.Length <= 2)
            {
                return new Tuple<bool, string>(false, "货位格式的子串错误");
            }
            else
            {
                if (strFormat[0] != '{' || strFormat[strFormat.Length - 1] != '}')
                {
                    return new Tuple<bool, string>(false, "货位格式的子串错误");
                }
                else
                {
                    string temp = strFormat.TrimStart('{').TrimEnd('}');
                    if (!IsCharEq(temp))
                    {
                        return new Tuple<bool, string>(false, "货位格式的子串错误");
                    }
                    else
                    {
                        return new Tuple<bool, string>(true, "");
                    }
                }
            }
        }

        private Tuple<string, string> FindChildNoName(List<string> listName, List<string> listNo, string code)
        {
            string tempName = listName.Find(p => p.Contains(code));
            string no = listNo.Find(p => p.Contains(code));
            string name = string.Empty;

            if (!string.IsNullOrEmpty(no))
            {
                string[] array = tempName.Split("}");
                name = array[1];
            }


            return new Tuple<string, string>(no, name);
        }

        /// <summary>
        /// 判断字符串是否为同一个字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool IsCharEq(string str)
        {
            string temp = string.Empty;
            bool ret = true;

            for (int i = 0; i < str.Length; i++)
            { 
                if (!str[i].ToString().Equals(temp)&&!string.IsNullOrEmpty(temp))
                {
                    ret = false;
                    break;
                }
                temp = str[i].ToString();
            }

            return ret;
        }

        /// <summary>
        /// 生成位置编号和名称
        /// </summary>
        /// <param name="num">序号</param>
        /// <param name="strNo">货位子串</param>
        /// <returns></returns>
        private Tuple<string, string> CreateNameAndNoByChildStr(int num, string strNo,string strName)
        {
            string strFormat = strNo.TrimStart('{').TrimEnd('}');

            int preLength = strFormat.Length - num.ToString().Length;//补0个数
            string strZero = "";

            for (int i = 0; i < preLength; i++)
            {
                strZero += "0";
            }

            string no = $"{strZero}{num}";
            string name = $"{no}{strName}";

            return new Tuple<string, string>(no, name);
        }

        private Tuple<int, int, bool, string> GetStartEndIndex(string param)
        {
            if (string.IsNullOrEmpty(param)) { return new Tuple<int, int, bool, string>(0, 0, true, ""); }

            string[] strs = param.Split(',');
            if (strs.Length != 2)
            {
                return new Tuple<int, int, bool, string>(0, 0, false, "输入范围格式错误");
            }
            int startIndex = 0, endIndex = 0;

            if (!int.TryParse(strs[0], out startIndex) || !int.TryParse(strs[1], out endIndex))
            {
                return new Tuple<int, int, bool, string>(0, 0, false, "输入范围格式错误");
            }
            if (startIndex > endIndex)
            {
                return new Tuple<int, int, bool, string>(0, 0, false, "起始数字应小于结束数字");
            }

            return new Tuple<int, int, bool, string>(startIndex, endIndex, true, "");
        }

        private Dictionary<string, object> GetLocationModel(dynamic item, LocationModel locationInfo)
        {
            Dictionary<string, object> param = new Dictionary<string, object>() {
                   { "t_id",  SnowflakeID.Guid().ToString() },
                   {"warehouse_id",Context.WarehouseID},
                   {"location_no",locationInfo.location_no},
                   {"location_code",locationInfo.location_no},
                   {"location_name",locationInfo.location_name},
                   {"zone_id",locationInfo.zone_id},
                   {"zone_id2",""},
                   {"zone_id3",""} ,
                   {"zone_id4",""},
                   {"location_row",item.h.seq_no.ToString()},
                   {"location_column",item.l.seq_no.ToString()} ,
                   {"location_layer",item.c.seq_no.ToString()},
                   {"location_depth",item.s.seq_no.ToString()},
                   {"floor_no",item.f.seq_no.ToString()} ,
                   {"room_no",item.j.seq_no.ToString()},
                   {"unit_no",item.k.seq_no.ToString()},
                   {"tags",locationInfo.tags} ,
                   {"location_type",locationInfo.location_type},
                   {"location_status",locationInfo.location_status} ,
                   {"location_flag",locationInfo.location_flag},
                   {"process_status",locationInfo.process_status} ,
                   {"usage",locationInfo.usage},
                   {"owner_id",locationInfo.owner_id} ,
                   {"container_catalog_id",locationInfo.container_catalog_id},
                   {"is_mix_item",locationInfo.is_mix_item} ,
                   {"is_mix_lot",locationInfo.is_mix_lot},
                   {"is_mix_grade",locationInfo.is_mix_grade},
                   {"length",locationInfo.length.ToString()},
                   {"width",locationInfo.width.ToString()},
                   {"height",locationInfo.height.ToString()},
                   {"uom_id",locationInfo.uom_id},
                   {"seq_no",locationInfo.seq_no.ToString()},
                   {"volume_limit",locationInfo.volume_limit.ToString()},
                   {"weight_limit",locationInfo.weight_limit.ToString()},
                   {"qty_limit",locationInfo.qty_limit.ToString()},
                   {"container_limit",locationInfo.container_limit.ToString()} ,
                   {"uom_qtyid",locationInfo.uom_qtyid} ,
                   {"weightid",locationInfo.weightid} ,
                   {"description",locationInfo.description} ,
                   {"xcoord",locationInfo.xcoord.ToString()} ,
                   {"ycoord",locationInfo.ycoord.ToString()} ,
                   {"zcoord",locationInfo.zcoord.ToString()},
                   {"xdistance",locationInfo.xdistance.ToString()} ,
                   {"ydistance",locationInfo.ydistance.ToString()} ,
                   {"inbound_date",locationInfo.inbound_date.ToString()} ,
                   {"packing_type",locationInfo.packing_type} ,
                   {"status_reason",locationInfo.status_reason} ,
                   {"flag_reason",locationInfo.flag_reason} ,
                   {"create_date",locationInfo.create_date.ToString()} ,
                   {"created_by",Context.UserInfo.UserName} ,
                   {"modify_ip",Context.UserInfo.LoginIP}
            };

            return param;
        }

        public string InsLocSql()
        {
            string sql = @"INSERT INTO &DataSource&WMS_M_LOCATION(CLIENT_ID,
                       ORG_ID,
                       t_id,
                       warehouse_id,
                       location_no,
                       location_code,
                       location_name,
                       zone_id,
                       zone_id2,
                       zone_id3,
                       zone_id4,
                       location_row,
                       location_column,
                       location_layer,
                       location_depth,
                       floor_no,
                       room_no,
                       unit_no,
                       tags,
                       location_type,
                       location_status,
                       location_flag,
                       process_status,
                       usage,
                       owner_id,
                       container_catalog_id,
                       is_mix_item,
                       is_mix_lot,
                       is_mix_grade,
                       length,
                       width,
                       height,
                       uom_id,
                       seq_no,
                       volume_limit,
                       weight_limit,
                       qty_limit,
                       container_limit,
                       uom_qtyid,
                       weightid,
                       description,
                       xcoord,
                       ycoord,
                       zcoord,
                       xdistance,
                       ydistance,
                       inbound_date,
                       packing_type,
                       status_reason,
                       flag_reason,
                       CREATED_BY,
                       MODIFY_IP,
                       CREATE_DATE,
                       ACT_STATUS) VALUES('&CLIENT_ID&',
              '&ORG_ID&',
              @t_id,
             '&WAREHOUSE_ID&',
              @location_no,
              @location_code,
              @location_name,
              @zone_id,
              @zone_id2,
              @zone_id3,
              @zone_id4,
              @location_row,
              @location_column,
              @location_layer,
              @location_depth,
              @floor_no,
              @room_no,
              @unit_no,
              @tags,
              @location_type,
              @location_status,
              @location_flag,
              @process_status,
              @usage,
              @owner_id,
              @container_catalog_id,
              @is_mix_item,
              @is_mix_lot,
              @is_mix_grade,
              @length,
              @width,
              @height,
              @uom_id,
              @seq_no,
              @volume_limit,
              @weight_limit,
              @qty_limit,
              @container_limit,
              @uom_qtyid,
              @weightid,
              @description,
              @xcoord,
              @ycoord,
              @zcoord,
              @xdistance,
              @ydistance,
              @inbound_date,
              @packing_type,
              @status_reason,
              @flag_reason,
              @created_by,
              @modify_ip,
              now(),
              '1')";

            return sql;
        }
    }
    public class LocationParam
    {
        /// <summary>
        /// 楼层
        /// </summary>
        public string F_Limit { get; set; }

        /// <summary>
        /// 仓间
        /// </summary>
        public string J_Limit { get; set; }

        /// <summary>
        /// 行
        /// </summary>
        public string H_Limit { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        public string L_Limit { get; set; }

        /// <summary>
        /// 层
        /// </summary>
        public string C_Limit { get; set; }

        /// <summary>
        /// 深
        /// </summary>
        public string S_Limit { get; set; }

        /// <summary>
        /// 单元
        /// </summary>
        public string K_Limit { get; set; }
    }
}
