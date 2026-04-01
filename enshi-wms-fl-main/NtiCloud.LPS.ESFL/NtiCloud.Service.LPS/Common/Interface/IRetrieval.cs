using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.LPS.Common.Interface
{
    public interface IRetrieval
    {
        /// <summary>
        /// 下架单提交下架
        /// </summary>
        /// <param name="RetrievalID">下架单ID</param>
        /// <returns></returns>
        WebJsonInfo RetrievalDownCommit(string RetrievalID);
        
        /// <summary>
        /// 下架单提交下架
        /// </summary>
        /// <param name="TaskHID">下架任务主档ID</param>
        /// <returns></returns>
        WebJsonInfo RetrievalDownTaskOver(string TaskHID);
    }
}
