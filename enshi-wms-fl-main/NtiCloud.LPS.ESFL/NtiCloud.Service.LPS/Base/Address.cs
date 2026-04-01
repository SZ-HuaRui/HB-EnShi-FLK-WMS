using NtiCloud.Service.WMS.Common;
using NtiCloud.Services.WMS.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Policy;
using System.Text;

namespace NtiCloud.Service.WMS.Base
{
    public class Address : WmsObject
    {
        public string ID { get; set; }

        public string Type { get; set; }

        public string Text { get; set; }

        [Obsolete]
        public Address(string id)
        {
            //try
            //{
            //    DataRow dockRow = Dock.GetDock(id);
            //    ID = id;
            //    Type = "Address_Dock";
            //    Text = dockRow["dock_number"].ToString();
            //    return;
            //}
            //catch (WmsException ex)
            //{
            //    if (ex.ErrorCode != DBHelper.NotFound)
            //    {
            //        throw;
            //    }
            //}

            //DataRow equipRow = Equipment.GetEquipment(id);
            //ID = id;
            //Type = "Address_Station";
            //Text = equipRow["equipment_number"].ToString();
            DataRow locRow = Location.Get(id);
            ID = id;
            Type = "AddressType_Location";
            Text = locRow["location_no"].ToString();
        }

        public Address()
        {
        }

        public Address(string id, string type, string text)
        {
            ID = id;
            Type = type;
            Text = text;
        }

        public Address(AddressType type, string id) : this(type.ToString(), id)
        {
        }

        public Address(string type, string id)
        {
            Type = type;
            ID = id;
            if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(id))
            {
                return;
            }
            if (type == AddressType.AddressType_Dock.ToString())
            {
                DataRow row = Dock.GetDock(id);
                Text = row["dock_code"].ToString();
            }
            else if (type == AddressType.AddressType_Location.ToString())
            {
                DataRow locRow = Location.Get(id);
                Text = locRow["location_no"].ToString();
            }    
            else if (type == AddressType.AddressType_Station.ToString())
            {
                DataRow row = Equipment.GetEquipment(id);
                Text = row["equipment_code"].ToString();
            }
        }

        public bool IsSame(Address addr)
        {
            if (Type.Trim() == addr.Type.Trim() && ID.Trim() == addr.ID.Trim())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 如果类型一致且typeAddr中ID为空则返回true
        /// </summary>
        /// <param name="typeAddr"></param>
        /// <returns></returns>
        public bool IsType(Address typeAddr)
        {
            if (string.IsNullOrWhiteSpace(typeAddr.ID) && Type == typeAddr.Type)
            {
                return true;
            }
            return false;
        }

        public bool IsMatched(Address addr)
        {
            return IsSame(addr) || BelongsTo(addr) || IsType(addr);
        }

        public bool BelongsTo(Address zoneAddr)
        {
            if (Type != AddressType.AddressType_Location.ToString())
            {
                return false;
            }
            if (zoneAddr.Type != AddressType.AddressType_Zone.ToString())
            {
                return false;
            }
            DataRow loc = Location.Get(ID);
            Zone zone = new Zone();
            string zoneID = loc[l.ZoneID].ToString();

            while (!string.IsNullOrWhiteSpace(zoneID))
            {
                if (zoneID == zoneAddr.ID)
                {
                    return true;
                }
                zoneID = zone.GetParent(zoneID)["p_id"].ToString();
            }

            return false;
        }

    }
}
