using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface IWorkTaskItem
    {
        /// <summary>
        /// 根据taskRow下面的所有明细的数量更新波次中的已处理数量
        /// </summary>
        /// <param name="taskRow"></param>
        /// <param name="sign">符号，1表示正数，-1表示负数</param>
        /// <returns></returns>
        IWorkTaskItem AddWaveDetailProcessQty(DataRow taskRow, decimal sign);
        /// <summary>
        /// 根据taskID下面的所有明细的数量更新波次中的已处理数量
        /// </summary>
        /// <param name="taskRow"></param>
        /// <param name="sign">符号，1表示正数，-1表示负数</param>
        IWorkTaskItem AddWaveDetailProcessQty(string taskID, decimal sign);

        DataRow Get(string id);

        DataRow[] GetByOrderNoOrID(string orderNoOrID);

        DataRow[] GetByRefDocID(string refDocType, string refDocID);

        DataRow[] GetByTaskID(string taskID);

        IWorkTaskItem Insert(WorkTaskItemModel model);

        /// <summary>
        /// 根据任务物料实体类，插入任务物料历史表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IWorkTaskItem InsertFinishedTaskItem(WorkTaskItemModel model);

        void UpdateActStatus(string id, string actStatus, string oldActStatus = null);
    }
}
