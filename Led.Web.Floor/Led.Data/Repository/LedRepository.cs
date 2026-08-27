using DataAccess;
using Led.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Data.Repository
{
    /*WCS_ONECODETOITEMCODE表是入库记录 WCS_ONECODETOITEMCODE_CLONE是出库记录 */
    public class LedRepository:RepositoryBase<ItemProduction>,ILedRepository
    {
        OracleHelper oracleHelper = new OracleHelper("DBContext");

        public ClassProduction GetLastClassQty()
        {
            ClassProduction production = new ClassProduction();
            List<ItemProduction> list = new List<ItemProduction>();
            int timeHH = Convert.ToInt32(DateTime.Now.ToString("HH"));
            string startTime = string.Empty;
            string endTime = string.Empty;
            if (timeHH >= 8 && timeHH <17)
            {
                return production;
            }
            if (timeHH >= 0 && timeHH < 8)
            {
                startTime = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd") + " 08:00:00";
                endTime = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd") + " 17:12:00";
            }
            else if (timeHH == 17)
            {
                int timeMM = Convert.ToInt32(DateTime.Now.ToString("mm"));
                if (timeMM >= 12)
                {
                    startTime = DateTime.Now.ToString("yyyy/MM/dd") + " 08:00:00";
                    endTime = DateTime.Now.ToString("yyyy/MM/dd") + " 17:12:00";
                }
                else
                {
                    return production;
                }
            }else if (timeHH > 17 && timeHH < 24)
            {
                startTime = DateTime.Now.ToString("yyyy/MM/dd") + " 08:00:00";
                endTime = DateTime.Now.ToString("yyyy/MM/dd") + " 17:12:00";
            }
            production.IsShow = true;
            string sql = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(startTime))
                {
                    sql = $@"SELECT 
    sum(cc.count_clone) AS inqty,  -- WCS_ONECODETOITEMCODE_CLONE 的计数
    sum(c.count_original) AS outqty   -- wcs_onecodetoitemcode 的计数
FROM 
    swms_wi_item i
LEFT JOIN (
    SELECT 
        itemid, 
        COUNT(*) AS count_clone
    FROM 
        WCS_ONECODETOITEMCODE_CLONE
     WHERE createtime >= TO_DATE('{startTime}', 'yyyy/mm/dd hh24:mi:ss')
       AND createtime <= TO_DATE('{endTime}', 'yyyy/mm/dd hh24:mi:ss')
    GROUP BY 
        itemid
) cc ON i.t_id = cc.itemid
LEFT JOIN (
    SELECT 
        itemid, 
        COUNT(*) AS count_original
    FROM 
        wcs_onecodetoitemcode
     WHERE createtime >= TO_DATE('{startTime}', 'yyyy/mm/dd hh24:mi:ss')
       AND createtime <= TO_DATE('{endTime}', 'yyyy/mm/dd hh24:mi:ss')
    GROUP BY 
        itemid
) c ON i.t_id = c.itemid
WHERE 
    1=1 and (cc.count_clone<> 0 or c.count_original<> 0)";
                }
                else
                {
                    sql = $@"SELECT 
    sum(cc.count_clone) AS inqty,  -- WCS_ONECODETOITEMCODE_CLONE 的计数
    sum(c.count_original) AS outqty   -- wcs_onecodetoitemcode 的计数
FROM 
    swms_wi_item i
LEFT JOIN (
    SELECT 
        itemid, 
        COUNT(*) AS count_clone
    FROM 
        WCS_ONECODETOITEMCODE_CLONE
    GROUP BY 
        itemid
) cc ON i.t_id = cc.itemid
LEFT JOIN (
    SELECT 
        itemid, 
        COUNT(*) AS count_original
    FROM 
        wcs_onecodetoitemcode
    GROUP BY 
        itemid
) c ON i.t_id = c.itemid
WHERE 
    1=1 and (cc.count_clone<> 0 or c.count_original<> 0)";
                }
                DataTable dt = oracleHelper.ExecuteQueryBySql(sql);
                production.inQty = dt.Rows[0]["inqty"].ToString();
                production.outQty = dt.Rows[0]["outqty"].ToString();
                return production;
            }
            catch
            {
                return production;
            }
        }

        public List<ItemProduction> GetTodayInBoundInfo()
        {
            List<ItemProduction> list = new List<ItemProduction>();
            int timeHH = Convert.ToInt32(DateTime.Now.ToString("HH"));
            string startTime = string.Empty;
            string endTime = string.Empty;
            if (timeHH >= 0 && timeHH < 8)
            {
                startTime = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd") + " 17:12:00";
                endTime = DateTime.Now.ToString("yyyy/MM/dd") + " 07:59:59";
            }else if(timeHH >= 8 && timeHH < 17)
            {
                startTime = DateTime.Now.ToString("yyyy/MM/dd") + " 08:00:00";
                endTime = DateTime.Now.ToString("yyyy/MM/dd") + " 17:12:00";
            }else if (timeHH == 17)
            {
                int timeMM = Convert.ToInt32(DateTime.Now.ToString("mm"));
                if (timeMM >= 12)
                {
                    startTime = DateTime.Now.ToString("yyyy/MM/dd") + " 17:12:00";
                    endTime = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";
                }
                else
                {
                    startTime = DateTime.Now.ToString("yyyy/MM/dd") + " 08:00:00";
                    endTime = DateTime.Now.ToString("yyyy/MM/dd") + " 17:12:00";
                }
            }else if(timeHH > 17 && timeHH < 24)
            {
                startTime = DateTime.Now.ToString("yyyy/MM/dd") + " 17:12:00";
                endTime = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";
            }
            string sql = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(startTime))
                {
                    //sql = $@"select i.item_code,i.exterior_code,i.item_name,count(1) Qty
                    //        from WCS_ONECODETOITEMCODE h
                    //        inner join swms_wi_item i on h.itemid = i.t_id
                    //        where h.createtime >= to_date('{startTime}','yyyy/mm/dd hh24:mi:ss')
                    //        and h.createtime <= to_date('{endTime}','yyyy/mm/dd hh24:mi:ss')
                    //        group by i.item_code,i.exterior_code,i.item_name";

                    sql = $@"SELECT 
    i.item_code, 
    i.exterior_code, 
    i.item_name, 
    cc.count_clone AS inqty,  -- WCS_ONECODETOITEMCODE_CLONE 的计数
    c.count_original AS outqty   -- wcs_onecodetoitemcode 的计数
FROM 
    swms_wi_item i
LEFT JOIN (
    SELECT 
        itemid, 
        COUNT(*) AS count_clone
    FROM 
        WCS_ONECODETOITEMCODE_CLONE
     WHERE createtime >= TO_DATE('{startTime}', 'yyyy/mm/dd hh24:mi:ss')
       AND createtime <= TO_DATE('{endTime}', 'yyyy/mm/dd hh24:mi:ss')
    GROUP BY 
        itemid
) cc ON i.t_id = cc.itemid
LEFT JOIN (
    SELECT 
        itemid, 
        COUNT(*) AS count_original
    FROM 
        wcs_onecodetoitemcode
     WHERE createtime >= TO_DATE('{startTime}', 'yyyy/mm/dd hh24:mi:ss')
       AND createtime <= TO_DATE('{endTime}', 'yyyy/mm/dd hh24:mi:ss')
    GROUP BY 
        itemid
) c ON i.t_id = c.itemid
WHERE 
    1=1 and (cc.count_clone<> 0 or c.count_original<> 0)";
                }
                else
                {
                    sql = $@"SELECT 
    i.item_code, 
    i.exterior_code, 
    i.item_name, 
    cc.count_clone inqty,  -- WCS_ONECODETOITEMCODE_CLONE 的计数
    c.count_original outqty   -- wcs_onecodetoitemcode 的计数
FROM 
    swms_wi_item i
LEFT JOIN (
    SELECT 
        itemid, 
        COUNT(*) AS count_clone
    FROM 
        WCS_ONECODETOITEMCODE_CLONE
    GROUP BY 
        itemid
) cc ON i.t_id = cc.itemid
LEFT JOIN (
    SELECT 
        itemid, 
        COUNT(*) AS count_original
    FROM 
        wcs_onecodetoitemcode
    GROUP BY 
        itemid
) c ON i.t_id = c.itemid
WHERE 
    1=1 and (cc.count_clone<> 0 or c.count_original<> 0)";
                }
                DataTable dt = oracleHelper.ExecuteQueryBySql(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ItemProduction item = new ItemProduction();
                        item.ItemCode = dr["item_code"].ToString();
                        item.ItemName = dr["item_name"].ToString();
                        item.inQty = dr["inqty"].ToString();
                        item.outQty = dr["outqty"].ToString();
                        list.Add(item);
                    }
                }
                return list;
            }
            catch
            {
                return list;
            }
        }

    }
}
