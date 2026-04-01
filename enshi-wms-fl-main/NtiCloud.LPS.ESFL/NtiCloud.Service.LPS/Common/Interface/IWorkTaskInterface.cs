using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface IWorkTaskInterface
    {
        WebJsonInfo NotifyWaveFinished(string waveNo);

        WebJsonInfo NotifyWavePause(string waveNo);

        string Recycle(string taskid);

        WebJsonInfo Recycle(List<string> ids);

        WebJsonInfo Recycle(DataRow[] taskRows);

        WebJsonInfo GetBatchNo(string warehouseID, string destAddrNo, string batchNo);

        WebJsonInfo Send(List<string> ids);

        /// <summary>
        /// 要料终端发送入库任务
        /// </summary>
        /// <param name="ids">任务ID或任务号，如有多个，用英文逗号隔开</param>
        /// <returns></returns>
        WebJsonInfo SendInBonud(List<string> ids);
    }
}
