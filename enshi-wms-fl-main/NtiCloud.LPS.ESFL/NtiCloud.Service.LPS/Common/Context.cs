using Microsoft.Extensions.DependencyInjection;
using NtiCloud.Interface.Core;
using NtiCloud.Kernel;
using NtiCloud.Kernel.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace NtiCloud.Service.WMS.Common
{
    public sealed class Context
    {
        [ThreadStatic]
        private static string _sessionKey = string.Empty;
        [ThreadStatic]
        private static UserMode _userMode = null;
        [ThreadStatic]
        private static UserLoginInfo _userLoginInfo = null;
        [ThreadStatic]
        private static string _appcode = null;
        [ThreadStatic]
        private static string _clientID = "";

        private Context() { }

        public static UserMode User
        {
            get
            {
                if (_userMode == null)
                {
                    //throw new Exception("用户信息未能正确初始化");
                }
                return _userMode;
            }
            set
            {
                _userMode = value;
                _userLoginInfo = value.userLoginInfo;
            }
        }

        public static UserLoginInfo UserInfo
        {
            get
            {
                if (_userLoginInfo == null)
                {
                    throw new Exception("用户信息未能正确初始化");
                }
                return _userLoginInfo;
            }
        }

        public static string AppCode
        {
            get
            {
                return _appcode;
            }
        }

        public static string ClientID
        {
            get { return _clientID; }
            set { _clientID = value; }
        }

        private static void GetWarehouseV1()
        {
            //_warehouse = WmsConfig.GetOrDefault("CurrentWarehouse", "");

            //if (!string.IsNullOrEmpty(_warehouse.Trim()))
            //{
            //    return;
            //}
            DataTable dt = null;

            //if (string.IsNullOrEmpty(ClientID))
            //{
            //    //GetWarehouse_bak();
            //    return;
            //}
            //string sql = $"select t_id from &DataSource&swms_wi_warehouse where Code = @appCode ";
            string warehouseIDConst = "&WAREHOUSE_ID&";
            string sql = $"select '{warehouseIDConst}' t_id from dual ";
            //string sql = $"select t_id from wms.wms_m_warehouse where warehouse_code = @appCode ";
            //string sql = $"select t_id from &DataSource&wms_m_warehouse where warehouse_code = @appCode ";
            try
            {
                //dt = DBHelper.Select(sql, null);
                dt = DBHelper.Select(sql, new Dictionary<string, object> {
                    { "appCode", AppCode } 
                    //{ "appCode", "LB" } 
                });
                _warehouse = dt.Rows[0]["t_id"].ToString();
                if (_warehouse == warehouseIDConst)
                {
                    LogHelper.LogError("没有传入仓库ID，程序将自动查找");
                    throw new WmsException(100, "无法确定仓库！");
                }
            }
            catch
            {
                //sql = $"select t_id from isa_bi_warehouse where client_id = '{ClientID}'";
                sql = $"select t_id from &DataSource&wms_m_warehouse where client_id='{ClientID}' and act_status = '1' and warehouse_code = '{AppCode}' ";
                try
                {
                    dt = DBHelper.Select(sql, null);
                }
                catch
                {
                    sql = $"select t_id from &DataSource&wms_warehouse where client_id = '{ClientID}'";
                    try
                    {
                        dt = DBHelper.Select(sql, null);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogError(ex.Message);
                        //throw new WmsException(100, "查询仓库出错!", ex);
                        throw new WmsException(100, $"查询仓库出错!client_id={ClientID}, AppCode={AppCode}!" + ex.Message, ex);
                    }
                }
            }
            if (dt.Rows.Count > 1)
            {
                throw new WmsException(100, "无法确定仓库！");
            }
            _warehouse = dt.Rows[0]["t_id"].ToString();
        }

        private static void GetWarehouse()
        {
            //_warehouse = WmsConfig.GetOrDefault("CurrentWarehouse", "");

            //if (!string.IsNullOrEmpty(_warehouse.Trim()))
            //{
            //    return;
            //}
            DataTable dt = null;

            //if (string.IsNullOrEmpty(ClientID))
            //{
            //    //GetWarehouse_bak();
            //    return;
            //}
            //string sql = $"select t_id from &DataSource&swms_wi_warehouse where Code = @appCode ";
            string warehouseIDConst = "&WAREHOUSE_ID&";
            string sql = $"select '{warehouseIDConst}' t_id from dual ";
            //string sql = $"select t_id from wms.wms_m_warehouse where warehouse_code = @appCode ";
            //string sql = $"select t_id from &DataSource&wms_m_warehouse where warehouse_code = @appCode ";
            try
            {
                //dt = DBHelper.Select(sql, null);
                dt = DBHelper.Select(sql, new Dictionary<string, object> {
                    { "appCode", AppCode } 
                    //{ "appCode", "LB" } 
                });
                _warehouse = dt.Rows[0]["t_id"].ToString();
                if (_warehouse == warehouseIDConst)
                {
                    LogHelper.LogError("没有传入仓库ID，程序将自动查找");
                    //throw new WmsException(100, "无法确定仓库！");
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex.Message);
            }
            //sql = $"select t_id from isa_bi_warehouse where client_id = '{ClientID}'";
            sql = $"select t_id from &DataSource&wms_m_warehouse where client_id='{ClientID}' and act_status = '1' and warehouse_code = '{AppCode}' ";
            try
            {
                dt = DBHelper.Select(sql, null);
                _warehouse = dt.Rows[0]["t_id"].ToString();
                return;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex.Message);
            }
            sql = $"select t_id from &DataSource&wms_warehouse where client_id = '{ClientID}'";
            try
            {
                dt = DBHelper.Select(sql, null);
                _warehouse = dt.Rows[0]["t_id"].ToString();
                return;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex.Message);
                //throw new WmsException(100, "查询仓库出错!", ex);
                //throw new WmsException(100, $"查询仓库出错!client_id={ClientID}, AppCode={AppCode}!" + ex.Message, ex);
            }
            //if (dt.Rows.Count > 1)
            {
                //throw new WmsException(100, "无法确定仓库！");
            }
            //_warehouse = dt.Rows[0]["t_id"].ToString();
            _warehouse = "";
        }

        //private static void GetWarehouse_bak()
        //{
        //    LogHelper.LogDebug($"warehouseCode={AppCode}");
        //    DataTable dt = null;
        //    if (string.IsNullOrWhiteSpace(AppCode))
        //    {
        //        var service = ServiceProvider.GetService<ISystemService>();

        //        string pid = "1144424800303845376";
        //        //var dict = service.DictionaryConfig(_sessionKey, pid, false);
        //        var dict = service.DictionaryConfig(pid, false);
        //        DataSet ds = JsonConvert.DeserializeObject<DataSet>(dict.Result);
        //        dt = ds.Tables[pid];
        //        DataRow[] houses = dt.AsEnumerable().Where(r => r["selected"].ToString() == "1").ToArray();
        //        if (houses.Length != 1)
        //        {
        //            throw new WmsException(99, "当前仓库定义错误");
        //        }
        //        _warehouse = houses[0]["id"].ToString();
        //        return;
        //    }
        //    //IsaResultInfo info = new ResultInfo();
        //    string sql = $"select t_id from isa_bi_warehouse where wh_code = '{AppCode}'";
        //    try
        //    {
        //        dt = DBHelper.Select(sql, null);
        //    }
        //    catch
        //    {
        //        sql = $"select t_id from wms_warehouse where productcode = '{AppCode}'";
        //        try
        //        {
        //            dt = DBHelper.Select(sql, null);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogHelper.LogError(ex.Message);
        //            //throw new WmsException(100, "查询仓库出错!", ex);
        //            throw new WmsException(100, $"查询仓库出错!client_id={ClientID}, AppCode={AppCode}!", ex);
        //        }
        //    }
        //    _warehouse = dt.Rows[0]["t_id"].ToString();
        //    /*
        //    var dict = service.DictionaryConfig(_sessionKey, "1144424800303845376");
        //    List<string> results = JsonConvert.DeserializeObject<List<string>>(dict.Result);
        //    var definition = new { id = "", pid = "", selected = "" };
        //    foreach (string s in results)
        //    {
        //        var row = JsonConvert.DeserializeAnonymousType(s, definition);
        //        if (row.selected == "1")
        //        {
        //            _warehouse = row.id;
        //        }
        //    }
        //    */
        //}

        [ThreadStatic]
        private static string _warehouse = string.Empty;
        public static string WarehouseID
        {
            get
            {
                //if (string.IsNullOrWhiteSpace(_warehouse))
                //{
                //    GetWarehouse();
                //}
                return _warehouse;
            }
            set
            {
                _warehouse = value;
            }
        }

        public static UserLoginInfo GetInfo(string sessionKey, string warehouseCode = "")
        {
            if (!sessionKey.Equals(_sessionKey) || _userLoginInfo == null)
            {
                _sessionKey = sessionKey;
                //_userLoginInfo = new User().GetUserInfo(_sessionKey);

                //var service = ServiceLocator.GetService<ILoginService>();
                //_userLoginInfo = service.GetUserInfo(sessionKey, "", "").Result;

                //_userLoginInfo = _service.GetUserInfo(sessionKey, "", "").Result;

                var service = ServiceProvider.GetService<IUserService>();
                if (service == null)
                {
                    throw new Exception("Service is null");
                }

                //_userLoginInfo = service.GetUserInfo(sessionKey).Result;
                //_userMode = service.GetToken().Result;
                //_userMode = service.GetToken().GetAwaiter().GetResult();
                _userMode = service.GetToken();
                _userLoginInfo = _userMode.userLoginInfo;

                if (_userMode.token.ContainsKey("appCode"))
                {
                    _appcode = _userMode.token["appCode"];
                    WmsFactory.ConfigRoot = _appcode.Trim();
                }
                if (_userMode.token.ContainsKey("clientId"))
                {
                    _clientID = _userMode.token["clientId"];
                }
                if (_userMode.token.ContainsKey("sessionKey"))
                {
                    _sessionKey = _userMode.token["sessionKey"];
                }


                //if (!string.IsNullOrWhiteSpace(warehouseCode))
                //{
                //    Info.WareHouse = warehouseCode;
                //}
                //LogHelper.LogDebug($"warehouseCode={warehouseCode}");

                GetWarehouse();

                return _userLoginInfo;
            }
            return _userLoginInfo;
        }

        public static UserLoginInfo GetUserInfo()
        {
            var service = ServiceProvider.GetService<IUserService>();
            if (service == null)
            {
                throw new Exception("Service is null");
            }

            //_userLoginInfo = service.GetUserInfo(sessionKey).Result;
            //_userMode = service.GetToken().Result;
            _userMode = service.GetToken();
            _userLoginInfo = _userMode.userLoginInfo;

            if (_userMode.token.ContainsKey("appCode"))
            {
                _appcode = _userMode.token["appCode"];
                WmsFactory.ConfigRoot = _appcode.Trim();
            }
            if (_userMode.token.ContainsKey("clientId"))
            {
                _clientID = _userMode.token["clientId"];
            }
            if (_userMode.token.ContainsKey("sessionKey"))
            {
                _sessionKey = _userMode.token["sessionKey"];
            }



            UserLoginInfo userLoginInfo;
            userLoginInfo = new UserLoginInfo()
            {
                UserDbConnR = new DbInfo
                {
                    dbType = UserInfo.UserDbConnR.dbType,
                    dbIp = UserInfo.UserDbConnR.dbIp,
                    dbPort = UserInfo.UserDbConnR.dbPort,
                    database = UserInfo.UserDbConnR.database,
                    dbUid = UserInfo.UserDbConnR.dbUid,
                    dbPwd = UserInfo.UserDbConnR.dbPwd
                },
                UserDbConnW = new DbInfo
                {
                    dbType = UserInfo.UserDbConnW.dbType,
                    dbIp = UserInfo.UserDbConnW.dbIp,
                    dbPort = UserInfo.UserDbConnW.dbPort,
                    database = UserInfo.UserDbConnW.database,
                    dbUid = UserInfo.UserDbConnW.dbUid,
                    dbPwd = UserInfo.UserDbConnW.dbPwd
                }
            };
            return userLoginInfo;
        }
        /*
        public static UserLoginInfo GetUserInfo()
        {
            UserLoginInfo userLoginInfo;
            userLoginInfo = new UserLoginInfo()
            {
                UserDbConnR = new DbInfo
                {
                    dbType = UserInfo.UserDbConnR.dbType,
                    dbIp = UserInfo.UserDbConnR.dbIp,
                    dbPort = UserInfo.UserDbConnR.dbPort,
                    database = UserInfo.UserDbConnR.database,
                    dbUid = UserInfo.UserDbConnR.dbUid,
                    dbPwd = UserInfo.UserDbConnR.dbPwd
                },
                UserDbConnW = new DbInfo
                {
                    dbType = UserInfo.UserDbConnW.dbType,
                    dbIp = UserInfo.UserDbConnW.dbIp,
                    dbPort = UserInfo.UserDbConnW.dbPort,
                    database = UserInfo.UserDbConnW.database,
                    dbUid = UserInfo.UserDbConnW.dbUid,
                    dbPwd = UserInfo.UserDbConnW.dbPwd
                }
            };
            return userLoginInfo;
        }
        */

        [ThreadStatic]
        private static IServiceProvider serviceProvider = null;
        public static IServiceProvider ServiceProvider
        {
            get
            {
                if (serviceProvider == null)
                {
                    throw new Exception("请初始化用户环境");
                }
                return serviceProvider;
            }
        }

        [ThreadStatic]
        private static string _dataSource = null;
        public static string DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                _dataSource = value;
            }
        }
        public const string DATA_SOURCE = "&DataSource&";

        [ThreadStatic]
        private static IKernelGloballyVariable _globalVariable = null;
        public static IKernelGloballyVariable GlobalVariable
        {
            get
            {
                if (_globalVariable == null)
                {
                    throw new Exception("请初始化用户环境");
                }
                //globalVariable = serviceProvider.GetService<IKernelGloballyVariable>();
                return _globalVariable;
            }
        }

        public static void Init(IServiceProvider provider)
        {
            serviceProvider = provider;

            GetInfo("none");

            SetCulture();
        }

        private static void SetCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-CN", true)
            {
                DateTimeFormat = {
                    ShortDatePattern = "yyyy-MM-dd",
                    FullDateTimePattern = "yyyy-MM-dd HH:mm:ss",
                    LongTimePattern = "HH:mm:ss"
                }
            };
        }

        public static void Init(IServiceProvider proxy, string sessionKey)
        {
            serviceProvider = proxy;
            GetInfo(sessionKey);

            SetCulture();
        }

        public static void Init(IServiceProvider proxy, string sessionKey, string appCode)
        {
            //_proxy = proxy;
            //_appcode = appCode;
            //WmsFactory.ConfigRoot = appCode.Trim();
            //GetInfo(sessionKey);
            _appcode = appCode;
            WmsFactory.ConfigRoot = appCode.Trim();
            Init(proxy, sessionKey);
        }

        public static void Init(IServiceProvider proxy, string sessionKey, string appCode, string clientID)
        {
            //_proxy = proxy;
            //_appcode = appCode;
            //_clientID = clientID.Trim();
            //WmsFactory.ConfigRoot = appCode.Trim();
            //GetInfo(sessionKey);

            _clientID = clientID.Trim();
            Init(proxy, sessionKey, appCode);
        }

        public static void Init(IServiceProvider provider,
            Func<string, IDBContextProvider> _dbFactory)
        {
            serviceProvider = provider;

            SaveUserInfo();

            DBHelper.Init(_dbFactory(_userMode?.userLoginInfo.UserDbConnR.dbType));
            //DBHelper.Init(_dbFactory("Oracle"));

            _globalVariable = serviceProvider.GetService<IKernelGloballyVariable>();
            GloballyDBChange globallyDB = _globalVariable.Globally_SQL(_userMode, DATA_SOURCE);
            _dataSource = globallyDB.sql;
            //GetInfo("none");

            SetCulture();

            //_warehouse = "1016319470013779968";
            if (string.IsNullOrEmpty(_warehouse))
            {
                GetWarehouse();
            }
        }

        private static void SaveUserInfo()
        {
            var service = ServiceProvider.GetService<IUserService>();
            if (service == null)
            {
                throw new Exception("Service is null");
            }

            _userMode = service.GetUserMode();
            if (_userMode == null)
            {
                //throw new WmsException(500, "请先登录");
                throw new WmsException(500, "无法获得数据库连接信息");
            }
            _userLoginInfo = _userMode.userLoginInfo;

            UserMode userMode = service.GetToken();
            if (userMode == null)
            {
                //throw new WmsException(400, "无法获得令牌");
                return;
            }
            _userMode = userMode;
            _userLoginInfo = userMode.userLoginInfo;
            if (userMode.token.ContainsKey("appCode"))
            {
                _appcode = userMode.token["appCode"];
                LogHelper.LogInformation("appcode=[{0}]", _appcode);
                WmsFactory.ConfigRoot = _appcode.Trim();
            }
            if (userMode.token.ContainsKey("clientId"))
            {
                _clientID = userMode.token["clientId"];
            }
            if (userMode.token.ContainsKey("sessionKey"))
            {
                _sessionKey = userMode.token["sessionKey"];
            }
            if (userMode.token.ContainsKey("warehouseId"))
            {
                _warehouse = userMode.token["warehouseId"];
            }
        }

        /*
        public static void Init(string sessionKey)
        {
            GetInfo(sessionKey);
            BeginWork();
        }
        */

        public static void Init(string sessionKey, string warehouseCode)
        {
            GetInfo(sessionKey, warehouseCode);
        }
    }
}
