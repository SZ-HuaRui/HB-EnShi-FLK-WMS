$(function () {
    var warehouseData = [];
    common.postGateway("website/selectdata", {
        "type": "warehouse",
        "parameters": [{
            "clientId": (common.getUrlParam("clientId") == null ? "" : common.getUrlParam("clientId")),
            "appCode": common.getUrlParam("appCode")
        }]
    }, null, function (data) {
        warehouseData = JSON.parse(data.json);
        $.each(warehouseData, function (index, item) {
            $('#warehouse').append(new Option(item.text, item.id));
        })
        if (common.getCookie("warehouseId") != null && common.getCookie("warehouseId") != "") {
            $("#warehouse").find("option[value=" + common.getCookie("warehouseId") + "]").prop("selected", true);
        }
    });

    $("#account").focus();
    $("#account").keydown(function (event) {
        if (event.which == 13) {
            loginBtn(warehouseData);      
            return;
        }
    });
    $("#password").keydown(function (event) {
        if (event.which == 13) {
            loginBtn(warehouseData);
            return;
        }
    });
    $(".rf_login").keydown(function (event) {
        if (event.which == 13) {
            loginBtn(warehouseData);
            return;
        }
    });
    $(".rf_login").on('click', function () {
        loginBtn(warehouseData);
    })
})

function loginBtn(data) {
    var warehouseId = $('#warehouse').val();
    var account = $('#account').val();
    var password = $('#password').val();
    var clientId = $("#clientId").val();
    //common.removeCookie("clientId");
    //common.setCookie("clientId", clientId);
    if (!account) {
        alert('请输入账号!');
        $("#account").focus();
        return;
    }
    else if (!password) {
        alert('请输入密码!');
        $("#password").focus();
        return;
    }
    else if (!warehouseId) {
        alert('请选择仓库!');
        return;
    }
    else {
        common.setCookie("warehouseId", warehouseId);
        var JumpUrl = "/RFS/Main";
        var searchWarehouse = false;
        for (var i = 0; i < data.length; i++) {
            if (data[i].id == warehouseId) {
                JumpUrl = JumpUrl + "?AppCode=" + data[i].code;
                common.removeCookie("appCode");
                common.setCookie("appCode", data[i].code);
                searchWarehouse = true;
                break;
            }
        }
        if (!searchWarehouse) { return;}
        if (clientId != null && clientId != "") {//隔移企业数据，自动定位企业ClientId
            $.messageLoad();
            $.ajaxSettings.async = false;
            common.postGateway("website/selectdata", {
                "type": "userClientId",
                "parameters": [{ userid: account, clientid: clientId }]
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
        CheckLogin(account, password, '#account', '#password', JumpUrl);        
    }
}