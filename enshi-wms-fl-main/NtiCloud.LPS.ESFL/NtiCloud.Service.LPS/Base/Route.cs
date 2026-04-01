using NtiCloud.Service.WMS.Common;
using NtiCloud.Services.WMS.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NtiCloud.Service.WMS.Base
{
    public class Route : WmsObject
    {
        private List<DataRow[]> RouteList = new List<DataRow[]>();

        private DataRow[] GetByPurpose(string purpose)
        {
            Dictionary<string, object> param = new Dictionary<string, object> {
                { "warehouse", Context.WarehouseID },
                { "purpose", purpose },
            };
            string sql;
            //and warehouse_id = @warehouse 
            sql = @"
select * from &DataSource&wms_s_task_route  
 where client_id='&CLIENT_ID&' and org_id='&ORG_ID&' 
   and warehouse_id='&WAREHOUSE_ID&'
   and purpose = @purpose
 order by seq_no, src_addr_type, src_addr_id
";
            DataTable dt = DBHelper.Select(sql, param, "查询路径表", false);

            return dt.AsEnumerable().ToArray();
        }

        public DataRow[] GetBy(string itemCatalogID, string purpose)
        {
            DataRow[] routes = GetByPurpose(purpose);
            DataRow[] selected;

            while (true)
            {
                selected = routes.Where(r =>
                    r["item_catalog_id"].ToString() == itemCatalogID).ToArray();
                if (selected.Length > 0)
                {
                    break;
                }
                DataRow catalogRow = ItemCatalog.GetParent(itemCatalogID);
                if (catalogRow == null)
                {
                    break;
                }
                itemCatalogID = catalogRow["t_id"].ToString();
            }

            return selected;
        }

        public Route() { }


        public Route(string itemCatalogID, string purpose)
        {
            Init(itemCatalogID, purpose);
        }

        private bool CanJoin(DataRow fore, DataRow back)
        {
            if (fore["dest_addr_type"].ToString() == back["src_addr_type"].ToString())
            {
                if (fore["dest_addr_id"].ToString() == back["src_addr_id"].ToString())
                {
                    return true;
                }
            }
            return false;
        }

        private void Add(DataRow[] group)
        {
            bool added = false;

            List<DataRow[]> newRouteList = new List<DataRow[]>();
            List<DataRow[]> toBeRemoved = new List<DataRow[]>();
            foreach (DataRow[] route in RouteList)
            {
                if (CanJoin(route[route.Length - 1], group[0]))
                {
                    //route.Append(group[0]);
                    //DataRow[] newRoute = route.Append(group[0]).ToArray();
                    //newRouteList.Add(newRoute);
                    foreach (DataRow row in group)
                    {
                        newRouteList.Add(route.Append(row).ToArray());
                        if (AreSrcAndDestTheSame(row))
                        {
                            toBeRemoved.Add(route);
                        }
                    }
                    added = true;
                }
            }
            toBeRemoved.ForEach(r => RouteList.Remove(r));
            RouteList.AddRange(newRouteList.ToArray());

            if (!added)
            {
                foreach (DataRow row in group)
                {
                    RouteList.Add(new DataRow[1] { row });
                }
            }
        }

        private void AddV1(DataRow[] group)
        {
            bool added = false;

            List<DataRow[]> newRouteList = new List<DataRow[]>();
            foreach (DataRow[] route in RouteList)
            {
                if (CanJoin(route[route.Length - 1], group[0]))
                {
                    route.Append(group[0]);
                    DataRow[] newRoute = route.Append(group[0]).ToArray();
                    newRouteList.Add(newRoute);
                    added = true;
                }
            }
            RouteList.AddRange(newRouteList.ToArray());

            if (!added)
            {
                foreach (DataRow row in group)
                {
                    RouteList.Add(new DataRow[1] { row });
                }
            }
        }

        public void Init(string itemCatalogID, string purpose)
        {
            DataRow[] routeRows = GetBy(itemCatalogID, purpose);
            Init(routeRows);
        }

        private void Init(string purpose)
        {
            DataRow[] routeRows = GetByPurpose(purpose);
            Init(routeRows);
        }

        private void Init(DataRow[] routeRows)
        {
            List<List<DataRow>> sameSrc = new List<List<DataRow>>();
            bool added = false;

            foreach (DataRow row in routeRows)
            {
                added = false;
                foreach (List<DataRow> group in sameSrc)
                {
                    //if (CanJoin(group.Last(), row))
                    if (IsSameSrc(group.First(), row))
                    {
                        group.Add(row);
                        added = true;
                        break;
                    }
                }
                if (!added)
                {
                    sameSrc.Add(new List<DataRow>() { row });
                }
            }
            foreach (List<DataRow> group in sameSrc)
            {
                Add(group.ToArray());
            }
        }

        private bool AreSrcAndDestTheSame(DataRow row)
        {
            if (row["src_addr_type"].ToString() == row["dest_addr_type"].ToString())
            {
                if (row["src_addr_id"].ToString() == row["dest_addr_id"].ToString())
                {
                    return true;
                }
            }

            return false;
        }
        private bool IsSameSrc(DataRow group, DataRow row)
        {
            if (group["src_addr_type"].ToString() == row["src_addr_type"].ToString())
            {
                if (group["src_addr_id"].ToString() == row["src_addr_id"].ToString())
                {
                    return true;
                }
            }

            return false;
        }

        public bool MatchRoute(Address src, Address dest, out DataRow[] route)
        {
            route = null;
            List<DataRow[]> candidate = new List<DataRow[]>();
            foreach (DataRow[] group in RouteList)
            {
                Address head = new Address()
                {
                    Type = group.First()[r.SrcAddrType].ToString(),
                    ID = group.First()[r.SrcAddrID].ToString(),
                };
                Address last = new Address()
                {
                    Type = group.Last()[r.DestAddrType].ToString(),
                    ID = group.Last()[r.DestAddrID].ToString(),
                };
                if (src.IsMatched(head) && dest.IsMatched(last))
                {
                    candidate.Add(group);
                }
            }

            if (candidate.Count() <= 0)
            {
                return false;
            }

            if (candidate.Count() == 1)
            {
                route = new DataRow[candidate[0].Count()];
                candidate[0].CopyTo(route, 0);
                return true;
            }

            foreach (DataRow[] group in candidate)
            {
                if (group[0][r.IsDefault].ToString() == YN.Yes)
                {
                    route = new DataRow[group.Count()];
                    group.CopyTo(route, 0);
                    return true;
                }
            }

            Random random = new Random();
            int i = random.Next(candidate.Count() - 1);
            route = new DataRow[candidate[i].Count()];
            candidate[i].CopyTo(route, 0);

            return true;
        }
        public bool MatchRouteV1(Address src, Address dest, out DataRow[] route)
        {
            route = null;
            List<DataRow[]> candidate = new List<DataRow[]>();
            foreach (DataRow[] group in RouteList)
            {
                Address head = new Address()
                {
                    Type = group[0][r.SrcAddrType].ToString(),
                    ID = group[0][r.SrcAddrID].ToString(),
                };
                Address last = new Address()
                {
                    Type = group.Last()[r.DestAddrType].ToString(),
                    ID = group.Last()[r.DestAddrID].ToString(),
                };
                if (head.IsSame(src) && last.IsSame(dest))
                {
                    candidate.Add(group);
                }

                if (group[0][r.SrcAddrType].ToString() == AddressType.AddressType_Zone.ToString()
                    && src.Type == AddressType.AddressType_Location.ToString()
                    && src.BelongsTo(head)
                    && last.IsSame(dest))
                {
                    candidate.Add(group);
                }
            }

            if (candidate.Count() <= 0)
            {
                return false;
            }

            if (candidate.Count() == 1)
            {
                route = new DataRow[candidate[0].Count()];
                candidate[0].CopyTo(route, 0);
                return true;
            }

            foreach (DataRow[] group in candidate)
            {
                if (group[0][r.IsDefault].ToString() == YN.Yes)
                {
                    route = new DataRow[group.Count()];
                    group.CopyTo(route, 0);
                    return true;
                }
            }

            Random random = new Random();
            int i = random.Next(candidate.Count() - 1);
            route = new DataRow[candidate[i].Count()];
            candidate[i].CopyTo(route, 0);

            return true;
        }
    }
}
