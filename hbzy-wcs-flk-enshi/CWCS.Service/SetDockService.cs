using CWCS.Core.Library.PLCManager;
using CWCS.Entity.Convery;
using CWCS.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CWCS.Service
{
    /// <summary>
    /// Ahthor:zpw
    /// Description:Logic of Seting Dock
    /// </summary>
    public class SetDockService
    {
        public OracleRepository _repository = new OracleRepository();
        public DataTable GetAllBrandInfo()
        {
          return _repository.GetAllBrandInfo();
        }

        public DataTable GetSomeBrandInfo(string itemCode)
        {
            return _repository.GetSomeBrandInfo(itemCode);
        }
        public bool SetDockBrand(string dockNO,string itemCode)
        {
            return _repository.SetDockBrand(dockNO,itemCode);
        }

        public bool IsExistsExceptionBrand(string itemCode)
        {
            return _repository.IsExistsExceptionBrand(itemCode);
        }

        public bool WriteBoxType(string dockNO,string itemCode)
        {
            try
            {
                DataTable dt = _repository.GetBrandInfoByDock(dockNO);
                if (dt == null || dt.Rows.Count != 1)
                {
                    return false;
                }
                DataTable InfoTable = _repository.GetItemBoxTypeInfo(itemCode);
                if (InfoTable == null || InfoTable.Rows.Count != 1)
                {
                    return false;
                }
                string stationNO = dt.Rows[0]["stationno"].ToString();
                string boxType = InfoTable.Rows[0]["box_type"].ToString();
                string palletQty = InfoTable.Rows[0]["each_pallet_fillqty"].ToString();
                if (!string.IsNullOrEmpty(stationNO) || ConveryPLC.StaticBoxTypeList.Count > 0 && ConveryPLC.StaticBoxTypeList.Exists(x => x.StationNo.Equals(stationNO)))
                {
                    ConveryEntity entity = ConveryPLC.StaticBoxTypeList.Where(x => x.StationNo.Equals(stationNO)).ToList()[0];
                    if (entity != null)
                    {
                        byte[] array = new byte[38];
                        if (!ConveryPLC.PLCList[entity.Key_ip].Write(entity.DBAddress, entity.value, entity.length, array)) return false;
                        array[0] = Convert.ToByte(boxType);
                        array[1] = Convert.ToByte(palletQty);
                        Array.Copy(Encoding.Default.GetBytes(itemCode), 0, array, 2, Encoding.Default.GetBytes(itemCode).Length);
                        if (!ConveryPLC.PLCList[entity.Key_ip].Write(entity.DBAddress, entity.value, entity.length, array)) return false;
                    }
                }

            }
            catch(Exception)
            {
                return false;
            }
            return true;

          
        }



        public DataTable GetAllDockInfo()
        {
            return _repository.GetAllDockInfo();
        }


        public bool InsertWcsLog(string message)
        {
            return _repository.InsertEventlog(message);
        }

    }
}
