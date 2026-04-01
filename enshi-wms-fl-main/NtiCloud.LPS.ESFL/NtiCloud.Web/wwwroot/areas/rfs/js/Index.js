$(function() {
    var cache = JSON.parse(common.Decrypt(sessionStorage["encrypt"]));
    var userId = cache.UserId;
    $(".content_d").html('');
    getMenu(userId);
});

function getMenu(userId) {
    var search = window.location.search;
    var menuDict = {
        RF_Compose: ["配盘", "/RFS/Main/ReceiptFL"],
        RF_Group: ["组盘", "/RFS/Main/ReceiptZP"],
        RF_ZP_Barcode: ["成品组盘（恩施）", "/RFS/Main/ReceiptZP_Barcode"],
        RF_ZP_WithoutBarcode: ["成品无码组盘(恩施)", "/RFS/Main/ReceiptZP_WithoutBarcode"],
        RF_PutawayTaskWithBarcode: ["上架任务(有码)", "/RFS/Main/PutawayDoing"],
        RF_PutawayTask: ["上架任务(无码)", "/RFS/Main/PutawayDoingWithoutSn"],
        RF_Putaway: ["直接上架", "/RFS/Main/PutawayDoingWithoutTask"],
        RF_PutawayByCompose: ["组盘上架", "/RFS/Main/PutawayPallet"],
        RF_PickOnline: ["在线拣选(无码)", "/RFS/Main/LKPickNoBN"],
        RF_PickOnlineWithBarcode: ["在线拣选(有码)", "/RFS/Main/LKPickBN"],
        RF_PickInWarehouse: ["在库拣选(无码)", "/RFS/Main/PKPickNoBNList"],
        RF_PickInWarehouseWithBarcode: ["在库拣选(有码)", "/RFS/Main/PKPickBNList"],
        PKPickForContainerList: ["在库拣选(容器)", "/RFS/Main/PKPickForContainerList"],
        RF_QC: ["质检", "/RFS/Main/QcInbound"],
        RF_Count: ["盘点", "/RFS/Main/LKCountSingle"],
        RF_AdjustmentByContainer: ["托盘转移", "/RFS/Main/RF_AdjustmentByContainer"],
        RF_ItemReplacement: ["烟包替换", "/RFS/Main/ItemReplacement"],
        RF_ApplyTask: ["模拟WCS申请入库", "/RFS/Main/ApplyTask"],
        RF_ItemReplaceGJ: ["替换烟包入库", "/RFS/Main/ItemReplaceGJ"]
    };
    var token = common.getToken();
    var sql = "select modulecode " +
        "from root.prod_app_module module " +
        "join root.prod_permission_menu menu " +
        "on module.t_id = menu.menu_id " +
        "join root.prod_permission_user permission_user " +
        "on menu.role_id = permission_user.role_id " +
        "join root.prod_system_user system_user " +
        "on system_user.t_id = permission_user.user_id " +
        "where system_user.userid = '" + userId + "' " +
        "order by module.moduleseq";
    common.postGateway('platform/database/selectsql', {
        "sql": common.Encrypt(sql),
        "parameters": []
    }, token, function(data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            var result = JSON.parse(data.json);
            var len = result.length;
            var menuHtml = "";
            if (len <= 0) {
                alert("该账号无权限，请联系管理员！");
            } else {
                for (var i = 0; i < len; i++) {
                    if (menuDict.hasOwnProperty(result[i].modulecode)) {
                        menuHtml += '<a class="content_a" href="' + menuDict[result[i].modulecode][1] + search + '">' + menuDict[result[i].modulecode][0] + '</a>';
                    }
                }
                if (menuHtml == "") {
                    alert("该账号无权限，请联系管理员！");
                    return;
                }
                $(".content_d").append(menuHtml);
            }
        } else {
            alert("查询失败！");
        }
    });
}