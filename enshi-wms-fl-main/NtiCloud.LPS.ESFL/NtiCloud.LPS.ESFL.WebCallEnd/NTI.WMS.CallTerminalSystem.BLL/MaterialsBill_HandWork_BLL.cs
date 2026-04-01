using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTI.WMS.CallTerminalSystem.DAL;
using WMSInterface;
using System.Data;

namespace NTI.WMS.CallTerminalSystem.BLL
{
    /// <summary>
    /// 手工领、退、接收
    /// </summary>
    public class MaterialsBill_HandWork_BLL
    {
        string _equipmentNumber;
        string _userName;
        string _userPassword;
        string _warehouseID;


        public MaterialsBill_HandWork_BLL(string equipmentNumber, string userName, string userPassword, string warehouseId)
        {
            this._equipmentNumber = equipmentNumber;
            this._userName = userName;
            this._userPassword = userPassword;
            this._warehouseID = warehouseId;
        }

        /// <summary>
        /// 根据ContainerDetailID 修改手工领料单中的已发料量
        /// </summary>
        /// <param name="ContainerDetailID">容器明细ID</param>
        /// <param name="Qty">数量</param>
        /// <param name="odd1">零头1</param>
        /// <param name="odd2">零头2</param>
        /// <param name="odd3">零头3</param>
        /// <returns>大于1: 修改成功</returns>
        public int UpdateSentItemQty(string containerDetailID, decimal qty, decimal odd1, decimal odd2, decimal odd3)
        {
            int flag = 0;
            try
            {
                flag = MaterialsBill_HandWork_DAL.UpdateSentItemQty(_equipmentNumber, _userName, _userPassword, _warehouseID, containerDetailID, qty, odd1, odd2, odd3);
            }
            catch { flag = 0; }
            finally { }

            return flag;
        }

        /// <summary>
        /// 仓库人员获取已完成（车间人员已经确认接收的单据）的手工领料单数据 QWH 2009/07/03
        /// </summary>
        /// <returns></returns>  
        public DataSet GetFinishedMaterialsBill_HandWork()
        {
            DataSet ds = null;
            try
            {
                ds = MaterialsBill_HandWork_DAL.GetFinishedMaterialsBill_HandWork(_equipmentNumber, _userName, _userPassword, _warehouseID);
            }
            catch
            {
                ds = null;
            }
            finally { }

            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0) ds = null;

            return ds;
        }


        /// <summary>
        /// 获取暂存区有库存的物料数据
        /// </summary>
        /// <returns></returns>        
        public DataSet GetItemDataByZoneID(string machineNumber, string runsTypeCode)
        {
            DataSet ds = null;
            try
            {
                //查询人工领料数据源
                //ds = MaterialsBill_HandWork_DAL.GetItemDataByZoneID(_equipmentNumber, _userName, _userPassword, _warehouseID);
                ds = MaterialsBill_HandWork_DAL.GetItemDataByZoneID(_equipmentNumber, _userName + "," + machineNumber, _userPassword, _warehouseID + "," + runsTypeCode);

            }
            catch
            {
                ds = null;
            }
            finally { }

            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0) ds = null;

            return ds;
        }

        /// <summary>
        /// 获取手工领料单所有状态
        /// </summary>
        /// <returns>包含状态数据的DATATABLE（状态有：已完成、已发料、未处理）</returns>
        public DataTable GetBillState()
        {
            DataTable dt = null;
            try
            {
                DataSet ds = MaterialsBill_HandWork_DAL.GetBillState(_equipmentNumber, _userName, _userPassword, _warehouseID);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    dt = ds.Tables[0];
            }
            catch
            {
                dt = null;
            }
            finally { }

            return dt;
        }


        /// <summary>
        /// 仓库人员获取未完成的手工领料单数据 QWH 2009/06/17
        /// </summary>
        /// <returns></returns>        
        public DataSet GetMaterialsBill_HandWork()
        {
            DataSet ds = null;
            try
            {
                ds = MaterialsBill_HandWork_DAL.GetMaterialsBill_HandWork(_equipmentNumber, _userName, _userPassword, _warehouseID);
            }
            catch
            {
                ds = null;
            }
            finally { }

            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0) ds = null;

            return ds;
        }

        /// <summary>
        /// 创建手工领料单
        /// </summary>
        /// <param name="machineNumber">机台号</param>
        /// <param name="dockNumber">站台位号</param>
        /// <param name="containerEntity">包含领料明细数据的实体类</param>
        /// <returns>大于0: 操作成功</returns>
        public int CreateHandWorkMaterialBill(string machineNumber, string dockNumber, ContainerEntity containerEntity)
        {
            int flag = 0;
            try
            {
                flag = MaterialsBill_HandWork_DAL.CreateHandWorkMaterialBill(_equipmentNumber, _userName, _userPassword, _warehouseID, machineNumber, dockNumber,containerEntity);
            }
            catch { }
            finally { }

            return flag;
        }


         /// <summary>
        /// 车间人员获取手工领料单数据 QWH 2009/06/17 
         /// </summary>
         /// <param name="machineNumber">机台号</param>
         /// <returns></returns>        
        public DataSet GetMaterialsBill_HandWork_Warehouse(string machineNumber)
        {
            DataSet ds = null;
            try
            {
                ds = MaterialsBill_HandWork_DAL.GetMaterialsBill_HandWork_Warehouse(_equipmentNumber, _userName, _userPassword, _warehouseID, machineNumber);
            }
            catch
            {
                ds = null;
            }
            finally { }

            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0) ds = null;

            return ds;
        }

        /// <summary>
        /// 车间人员接收物料（手工发料） QWH 2009/06/19
        /// </summary>
        /// <param name="machineNumber">机台号</param>
        /// <param name="dockNumber">站台位号</param>
        /// <param name="billID">领料单ID</param>
        /// <returns>大于0:操作成功</returns>        
        public int Client_AcceptMaterials_HandWork(string machineNumber, string dockNumber, string billID, string Runs)
        {
            int flag = 0;
            try
            {
                flag = MaterialsBill_HandWork_DAL.Client_AcceptMaterials_HandWork(_equipmentNumber, _userName, _userPassword, _warehouseID, machineNumber, dockNumber, billID,Runs);
            }
            catch
            { flag = 0; }
            finally { }

            return flag;
        }

        /// <summary>
        /// 仓库人员确认发料（手工领料单） QWH 2009/06/19
        /// </summary>
        /// <param name="billID">领料单ID</param>
        /// <param name="containerEntity">包含具体发料信息的实体类（物料、数量等）</param>
        /// <returns>大于0:操作成功</returns>
        public int Client_SentMaterials_HandWork(string billID, ContainerEntity containerEntity)
        {
            int flag = 0;
            try
            {
                flag = MaterialsBill_HandWork_DAL.Client_SentMaterials_HandWork(_equipmentNumber, _userName, _userPassword, _warehouseID, billID, containerEntity);
            }
            catch
            { flag = 0; }
            finally { }

            return flag;
 
        }
        public bool FinishManPowerCallTask(string taskNumber)
        {
            bool res = false;
            try
            {
                res = MaterialsBill_HandWork_DAL.FinishManPowerCallTask(_equipmentNumber, _userName, _userPassword, _warehouseID, taskNumber);
            }
            catch
            {
                res = false;
            }
            finally { }
            return res;
        }

       
    }
}
