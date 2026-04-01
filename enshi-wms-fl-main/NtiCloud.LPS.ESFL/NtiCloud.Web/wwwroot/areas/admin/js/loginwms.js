$(function () {
    var warehouseData = [];
    common.postGateway("website/selectdata", {
        "type": "warehouse",
        "parameters": [{
            "clientId": $("#clientId").val(),
            "appCode": $("#AppCode").val()
        }]
    }, null, function (data) {
        warehouseData = JSON.parse(data.json);
        $.each(warehouseData, function (index, item) {
            $('#warehouseId').append(new Option(item.text, item.id));// 下拉菜单里添加元素
        })
        if (warehouseData.length==1) {
            $("#warehouseId").find("option[value=" + warehouseData[0].id + "]").prop("selected", true);
        }
        else if (common.getCookie("warehouseId") != null && common.getCookie("warehouseId") != "") {
            $("#warehouseId").find("option[value=" + common.getCookie("warehouseId") + "]").prop("selected", true);
        }
        layui.form.render();//下拉菜单渲染 把内容加载进去
    });

    //layui抓取页面数据和对应的提交事件
    layui.use('form', function () {
        var form = layui.form;
        //监听提交
        form.on('submit(formLoginWMS)', function (data) {
            LoginSys(data.field, warehouseData);
            return false;
        });
    });
});


function LoginSys(data, warehouseData) {
    var warehouseId = $.trim(data.warehouseId);
    var userName = $.trim(data.username);
    var passWord = $.trim(data.password);
    var clientId = $.trim(data.clientId);
    var userNameId = '#username_input';
    var passWordId = '#password_input';
    if (!warehouseId) {
        $.messageTips('请选择仓库!', ".layui-select-title", [3, '#c8161d']);
        return false;
    }
    common.setCookie("warehouseId", warehouseId);

    var JumpUrl = "/" + data.userarea + "/Main/WMS";
    var searchWarehouse = false;
    for (var i = 0; i < warehouseData.length; i++) {
        if (warehouseData[i].id == warehouseId) {
            JumpUrl = JumpUrl + "?AppCode=" + warehouseData[i].code;
            searchWarehouse = true;
            break;
        }
    }
    if (searchWarehouse == false && $("#AppCode").val() != null && $("#AppCode").val() != "") JumpUrl = JumpUrl + "?AppCode=" + $("#AppCode").val();
    if (common.getUrlParam("dbCode") != null) JumpUrl = JumpUrl + "&dbCode=" + common.getUrlParam("dbCode");
    
    if (clientId != null && clientId != "") {//隔移企业数据，自动定位企业ClientId
        $.messageLoad();
        $.ajaxSettings.async = false;
        common.postGateway("website/selectdata", {
            "type": "userClientId",
            "parameters": [{ userid: userName, clientid: clientId }]
        }, null, function (data) {
            data = JSON.parse(data.json);
            if (data.length == 1) {
                JumpUrl = JumpUrl + "&ClientId=" + data[0].client_id;
            }
            else {
                JumpUrl = JumpUrl + "&ClientId=" + clientId;
            }
        });
        $.ajaxSettings.async = true;
    }
    CheckLogin(userName, passWord, userNameId, passWordId, JumpUrl);
}