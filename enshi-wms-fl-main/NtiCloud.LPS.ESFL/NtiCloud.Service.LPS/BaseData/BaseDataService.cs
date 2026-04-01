using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NtiCloud.Interface.Core;
using NtiCloud.Interface.Core;
using NtiCloud.Interface.WMS;
using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using NtiCloud.Model.WMS;
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
using System.Reflection;
using System.Text;
using System.Text.Json;
using NtiCloud.Kernel.Core;
using NtiCloud.Service.LPS.Common;
using Microsoft.AspNetCore.Components;

namespace NtiCloud.Service.WMS.BaseData
{
    public class BaseDataService : IBaseDataService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IKernelCachingProvider _redis;
        private IConfiguration _configuration;
        private readonly Func<string, IDBContextProvider> _dbFactory;
        private readonly ILogger<BaseDataService> _logger;
        private readonly BaseData _baseData;
        public BaseDataService(IServiceProvider _serviceProvider, ILogger<BaseDataService> logger, IConfiguration configuration, Func<string, IDBContextProvider> dbFactory)
        {
            serviceProvider = _serviceProvider;
            _redis = serviceProvider.GetService<KernelRedisProvider>();
            _configuration = configuration;
            _dbFactory = dbFactory;
            _logger = logger;
            Context.Init(serviceProvider, _dbFactory);

            _baseData = new BaseData();
        }

        public WebJsonInfo AddItemUomWithItem(dynamic param)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IBaseDataService), "BaseDataService", "AddItemUomWithItem", param);

            return _baseData.AddItemUomWithItem(param, log);
        }

        public WebJsonInfo DelItemUomWithItem(dynamic param)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IBaseDataService), "BaseDataService", "DelItemUomWithItem", param);

            return _baseData.DelItemUomWithItem(param, log);
        }

        public WebJsonInfo AddItemUomWithCata(dynamic param)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IBaseDataService), "BaseDataService", "AddItemUomWithCata", param);

            return _baseData.AddItemUomWithCata(param, log); ;
        }

        public WebJsonInfo DelItemUomWithCata(dynamic param)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IBaseDataService), "BaseDataService", "DelItemUomWithCata", param);

            return _baseData.DelItemUomWithCata(param, log);
        }

        public WebJsonInfo BomProductionOpt(dynamic param, int optType)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IBaseDataService), "BaseDataService", "BomProductionOpt", new
            {
                param = param,
                optType = optType
            });

            return _baseData.BomProductionOpt(param, optType, log);
        }

        public WebJsonInfo BomChangeItemOpt(dynamic param, int optType)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IBaseDataService), "BaseDataService", "BomChangeItemOpt", new
            {
                param = param,
                optType = optType
            });

            return _baseData.BomChangeItemOpt(param, optType, log);
        }

        public WebJsonInfo BatchAddLoc(dynamic param)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IBaseDataService), "BaseDataService", "BatchAddLoc", param);

            return _baseData.BatchAddLoc(param, log);
        }

        public WebJsonInfo UpdateZoneSqlWhere(dynamic param)
        {
            UserLogModel log = WmsLog.GetUserLog(typeof(IBaseDataService), "BaseDataService", "UpdateZoneSqlWhere", param);

            return _baseData.UpdateZoneSqlWhere(param, log);
        }
    }
}
