using NtiCloud.Kernel.Core;
using NtiCloud.Model.WMS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Interface.WMS.HF
{
    [Post("/api/wms/{ServiceName}/{MethodName}")]
    public   interface IRFOperateService
    {
        /// <summary>
        /// RF登录
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="pwd">密码</param>
        /// <returns>用户信息</returns>
        [Description("RF登录", "", "")]
        public WebJsonInfo Login(string account, string pwd);

        /// <summary>
        /// 获取用户有权限的仓库列表
        /// </summary>
        /// <param name="account">账号</param> 
        /// <returns>用户信息</returns>
        [Description("获取用户有权限的仓库列表", "", "")]
        public WebJsonInfo GetWarehouseList(UserInfo userinfo);


        /// <summary>
        /// 获取用户有权限的货位列表
        /// </summary>
        /// <param name="account">账号</param> 
        /// <returns>用户信息</returns>
        [Description("获取用户有权限的货位列表", "", "")]
        public WebJsonInfo GetLocationList(UserInfo userinfo);


        /// <summary>
        /// 获取基础数据
        /// </summary>
        /// <param name="account">账号</param> 
        /// <returns>用户信息</returns>
        [Description("获取基础数据", "", "")]
        public WebJsonInfo GetSysTypeList(string flag);

        /// <summary>
        /// 获取任务列表
        /// </summary> 
        [Description("获取任务列表", "", "")]
        public WebJsonInfo GetOutboundTaskList(UserInfo userinfo, SynOutboundTaskIn searchInfo);
        /// <summary>
        /// 获取任务列表：不获取条码
        /// </summary> 
        [Description("获取任务列表:不获取条码", "", "")]
        public WebJsonInfo GetOutboundTaskListNew(UserInfo userinfo, SynOutboundTaskIn searchInfo);
        /// <summary>
        /// 获取任务行对应的可用条码信息
        /// </summary> 
        [Description("获取任务列表", "", "")]
        public WebJsonInfo GetOutboundTaskDetail(string outboundOrdedetailid);


        /// <summary>
        /// 获取楼层信息
        /// </summary> 
        [Description("获取楼层信息", "", "")]
        public WebJsonInfo GetWarehouseFloor(string warehouseId);

        /// <summary>
        /// 获取仓间信息
        /// </summary> 
        [Description("获取仓间信息", "", "")]
        public WebJsonInfo GetRoomByFloor(string warehouseId,string floor);

        /// <summary>
        /// 提交出库扫描信息
        /// </summary> 
        [Description("提交出库扫描信息", "", "")]
        public WebJsonInfo SubmitOutboundTask(SubmitOutboudDetailIn pickedInfo);
        /// <summary>
        /// 批量提交出库扫描信息
        /// </summary> 
        [Description("批量提交出库扫描信息", "", "")]
        public WebJsonInfo SubmitOutboundTaskList(List<SubmitOutboudDetailIn> pickedInfos);

        /// <summary>
        /// 批量提交出库扫描信息2:后台自动分配条码对应的任务行
        /// </summary> 
        [Description("批量提交出库扫描信息2", "", "")]
        public WebJsonInfo SubmitOutboundTaskList2(List<SubmitOutboudDetailIn> pickedInfos);

        /// <summary>
        /// 批量提交出库扫描信息3:将提交信息按条码提交返回
        /// </summary> 
        [Description("批量提交出库扫描信息3", "", "")]
        public WebJsonInfo SubmitOutboundTaskList3(List<SubmitOutboudDetailIn> pickedInfos);

        /// <summary>
        /// 合包时，获取出库扫描的条码信息
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        [Description("获取已出库条码信息","扫描的出库条码","")]
        public WebJsonInfo GetShippedbarcode(string barcode);
        /// <summary>
        /// 提交合包信息
        /// </summary>
        /// <returns></returns>
        [Description("提交合包信息", "扫描的新条码与原条码", "")]
        public WebJsonInfo SubmitMergedCarcode(MergebarcodeIn mergeBarcode);
         
        /// <summary>
        /// 按出库单生成新条码
        /// </summary>
        /// <returns></returns>
        [Description("提交合包信息", "扫描的新条码与原条码", "")]
        public WebJsonInfo MakeNewBoxNumberByOrder(string orderno);

        /// <summary>
        /// 获取在库烟包条码信息
        /// </summary>
        /// <returns></returns>
        [Description("获取在库烟包条码信息", "烟包条码", "")]
        public WebJsonInfo GetInvBarCode(string barcode);

        /// <summary>
        /// 提交拆包信息
        /// </summary>
        /// <returns></returns>
        [Description("提交拆包信息", "", "")]
        public WebJsonInfo SubmitSplitInfo(BarcodeSplitIn splitInfo);

        /// <summary>
        /// 批量提交拆包信息
        /// </summary>
        /// <returns></returns>
        [Description("批量提交拆包信息", "", "")]
        public WebJsonInfo SubmitSplitInfos(List<BarcodeSplitIn> splitInfo);

        /// <summary>
        /// 按备料出库计划单维度获取权限范围内的待出库任务:整料任务
        /// </summary>
        /// <returns></returns>
        [Description("按备料出库计划单维度获取权限范围内的待出库任务:整料任务", "", "")]
        public WebJsonInfo GetTasklistThrowPlanNo(UserInfo userinfo);

        /// <summary>
        /// 按备料出库计划单维度获取权限范围内的待出库任务:零散料任务
        /// </summary>
        /// <returns></returns>
        [Description("按备料出库计划单维度获取权限范围内的待出库任务:零散料任务", "", "")]
        public WebJsonInfo GetTasklistThrowPlanNo2(UserInfo userinfo);

        /// <summary>
        /// 醇化库RF获取待合包任务明细
        /// </summary>
        /// <returns></returns>
        [Description("醇化库RF获取待合包任务明细", "", "")]
        WebJsonInfo GetTasklistForCombine(UserInfo userinfo);

        /// <summary>
        /// 批量提交合包信息  
        /// </summary>
        /// <returns></returns>
        [Description("批量提交合包信息", "", "")]
        public WebJsonInfo SubmitMergedCarcodeNew(List<MergebarcodeNewIn> lstBarcodes);
    }
}
