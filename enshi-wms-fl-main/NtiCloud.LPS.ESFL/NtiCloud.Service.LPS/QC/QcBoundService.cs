using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NtiCloud.Interface.Core;
using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Service.LPS.Common;
using NtiCloud.Service.LPS.Inbound;
using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Common;
using NtiCloud.Service.WMS.Inventory;
using NtiCloud.Service.WMS.Model;
using NtiCloud.Service.WMS.Tasking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


namespace NtiCloud.Service.WMS.QC
{
    public class QcBoundService : IQcBoundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IKernelCachingProvider _redis;
        private IConfiguration _configuration;
        private readonly Func<string, IDBContextProvider> _dbFactory;
        private readonly ILogger<QcBoundService> _logger;
        private readonly QcBound _qcBound;
        private readonly IWorkTask _workTask;
        private readonly IWorkTaskItem _workTaskItem;
        public QcBoundService(IServiceProvider _serviceProvider, ILogger<QcBoundService> logger, IConfiguration configuration, Func<string, IDBContextProvider> dbFactory)
        {
            serviceProvider = _serviceProvider;
            _redis = serviceProvider.GetService<KernelRedisProvider>();
            _configuration = configuration;
            _dbFactory = dbFactory;
            _logger = logger;
            Context.Init(serviceProvider, _dbFactory);

            _workTask = new WorkTask();
            _workTaskItem = new WorkTaskItem();
            _qcBound = new QcBound(_workTask, _workTaskItem, new Container());   
        }

        public WebJsonInfo ConfirmQcRunByType(dynamic data)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IQcBoundService), "QcBoundService", "ConfirmQcRunByType", data);

            return _qcBound.ConfirmQcRunByType(data, log);
        }

        public WebJsonInfo DelQcDetail(dynamic param)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IQcBoundService), "QcBoundService", "DelQcDetail", param);

            return _qcBound.DelQcDetail(param, log);
        }

        public WebJsonInfo StartInQcTask(string containerCode)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IQcBoundService), "QcBoundService", "StartInQcTask", new
            {
                containerCode = containerCode
            });

            return _qcBound.StartInQcTask(containerCode, log);
        }

        public WebJsonInfo UpdateQcResult(dynamic param)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IQcBoundService), "QcBoundService", "UpdateQcResult", param);

            return _qcBound.UpdateQcResult(param, log);
        }

        public WebJsonInfo ConfirmQcResult(dynamic param)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IQcBoundService), "QcBoundService", "ConfirmQcResult", param);

            return _qcBound.ConfirmQcResult(param, log);
        }

        public WebJsonInfo RunInQcTaskByWcs(NtiCloud.Model.WMS.WorkTaskModel.ApplyTaskInParam applyInfo)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IQcBoundService), "QcBoundService", "RunInQcTaskByWcs", new
            {
                applyInfo = applyInfo
            });

            return _qcBound.RunInQcTaskByWcs(applyInfo, log);
        }

        public WebJsonInfo GetQcInfo(string barcode)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IQcBoundService), "QcBoundService", "GetQcInfo", new
            {
                barcode = barcode
            });

            return _qcBound.GetQcDetail(barcode, log);
        }

        public WebJsonInfo StartInQcTaskWithItem(string containerCode, dynamic items)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IQcBoundService), "QcBoundService", "StartInQcTaskWithItem", items);

            return _qcBound.StartInQcTask(containerCode, log, items);
        }

        public WebJsonInfo CheckQcResult(dynamic param)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IQcBoundService), "QcBoundService", "CheckQcResult", param);

            return _qcBound.CheckQcResult(param, log);
        }
    }
}
