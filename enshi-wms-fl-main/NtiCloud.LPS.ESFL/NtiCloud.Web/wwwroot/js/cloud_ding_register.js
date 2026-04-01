//勾选已阅读
var isRead = true;
$(function () {
    $(".content").css("height", $(window).height());
	$(".back_main p").eq(1).hide();
    $(window).resize(function () {
        $(".content").css("height", $(window).height());
    })
    $('.isRead').on('click', function () {
        if (isRead) {
            $(this).removeClass('isRead').addClass('read_active');
            isRead = false;
        } else {
            $(this).removeClass('read_active').addClass('isRead');
            isRead = true;
        };
    });
    //监听设置管理员输入框的输入事件
    $(".content").on('input  propertychange', '.user #userId', function () {
        $('.user #userId').attr('data-tid', '').css('border','1px solid #C8161D');
        getMembers($(this).val())
        return false;
    });
    $(".content").on('click', '.user #userLists li', function () {
        if ($(this).attr('data-tid') != '') {
            $('.user #userId').val($(this).html());
            $('.user #userId').attr('data-tid', $(this).attr('data-tid')).css('border', '1px solid #e2e2e2');
        };
        $('.user #userLists').hide();
        return false;
    });
    $('body').on('click', function () {
        $('.user #userLists').hide();
    });
});

//模糊查询匹配用户名
function getMembers(inputStr) {
    common.postGateway("website/selectdata", {
        "type": "userSearch",
        "parameters": [{ "client_id": $("#t_id").val() ,"username": "%"+inputStr+"%"}]
    }, null, function (data) {
        data = JSON.parse(data.json);
        $('.user #userLists').show();
        $('.user #userLists').html('');
        if (data.length != 0) {
            for (var i = 0; i < data.length; i++) {
                $('.user #userLists').append('<li data-tid=' + data[i].t_id + '>' + data[i].username + '</li>');
            };
        } else {
            $('.user #userLists').append('<li data-tid="" >暂无匹配数据</li>');
        };
    });
};

function registerCompany() {
    $("#corpid").css('border', "1px solid #e2e2e2");
    $(".phone_tip").html("");
    if (!$("#corpid").val()) {
        $("#corpid").css('border', "1px solid #C8161D");
        $(".phone_tip").html("<font color='#C8161D'>请输入钉钉开放平台企业CorpId</font>");
        $("#usermob").focus();
    }
    else {
        $.messageLoad();
        common.postGateway("website/selectdata", {
            "type": "dingCompany",
            "parameters": [{ "ding_corpid": common.Encrypt($("#corpid").val())}]
        }, null, function (data) {
            data = JSON.parse(data.json);
            $.messageBoxCloseLoading();
            $("#corpid").attr("disabled", true);
            $(".input_phone").hide();
            $(".second_s").show();
            $(".register_read").show();
            if (data.length == 1) {//已注册
                $("#t_id").val(data[0].t_id);
                $("#AgentId").val(data[0].ding_appid);
                $("#AppKey").val(data[0].ding_appkey);
                $("#AppSecret").val(data[0].ding_appsecret);
                $('.user #userId').attr('data-tid', data[0].userid);
                $('#userId').val(data[0].username);
                $("#dingUrl").val(window.location.protocol + "//" + window.location.host + "/DingDing/Interface/" + common.Encrypt(window.location.protocol + "//" + window.location.host + "/Admin/Main?AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + data[0].t_id));
                dingApp();
            }
            else {
                $("#dingUrl").val(window.location.protocol + "//" + window.location.host + "/DingDing/Interface/" + common.Encrypt(window.location.protocol + "//" + window.location.host + "/Admin/Main?AppCode=" + common.getUrlParam("AppCode") + "&corpId=" + $("#corpid").val()));
            }
        });
    }
}
function register() {
    $("#AgentId").css('border', "1px solid #e2e2e2");
    $(".agentId_tip").html("");
    $("#AppKey").css('border', "1px solid #e2e2e2");
    $(".key_tip").html("");
    $("#AppSecret").css('border', "1px solid #e2e2e2");
    $(".password_tip").html("");
    $(".read_tip").html("");
    if (!$("#AgentId").val()) {
        $("#AgentId").css('border', "1px solid #C8161D");
        $(".agentId_tip").html("<font color='#C8161D'>请输入钉钉应用对应的AgentId</font>");
        $("#AgentId").focus();
    }
    else if (!$("#AppKey").val()) {
        $("#AppKey").css('border', "1px solid #C8161D");
        $(".key_tip").html("<font color='#C8161D'>请输入钉钉应用对应的AppKey</font>");
        $("#AppKey").focus();
    }
    else if (!$("#AppSecret").val()) {
        $("#AppSecret").css('border', "1px solid #C8161D");
        $(".password_tip").html("<font color='#C8161D'>请输入钉钉应用对应的AppSecret</font>");
        $("#AppSecret").focus();
    }
    else if (isRead) {
        $(".read_tip").html("<font color='#C8161D'>请勾选服务协议</font>");
    }
    else {
        $("#register1").hide();
        $("#register2").show();
    }
}
function registerOk() {
    $("#projectcode").css('border', "1px solid #e2e2e2");
    $("#projectname").css('border', "1px solid #e2e2e2");
    $("#usermob").css('border', "1px solid #e2e2e2");
    $("#useremail").css('border', "1px solid #e2e2e2");
    $(".project_code_tip").html("");
    $(".project_name_tip").html("");
    $(".project_mob_tip").html("");
    $(".project_mail_tip").html("");
    var reg0 = /^(((13[0-9]{1})|(14[0-9]{1})|(17[0-9]{1})|(15[0-3]{1})|(15[4-9]{1})|(18[0-9]{1})|(199))+\d{8})$/;
    var reg1 = /^[a-zA-Z\$_][a-zA-Z\d\.\-_]*$/;
    var reg2 = /^[A-Za-z\d]+([-_.][A-Za-z\d]+)*@([A-Za-z\d]+[-.])+[A-Za-z\d]{2,4}$/;
    if (!$("#projectcode").val()) {
        $("#projectcode").css('border', "1px solid #C8161D");
        $(".project_code_tip").html("<font color='#C8161D'>企业代码不能为空</font>") 
    }
    else if (!reg1.test($("#projectcode").val())) {
        $("#projectcode").css('border', "1px solid #C8161D");
		$(".project_code_tip").html("<font color='#C8161D'>请输入字母+数字组合的项目代码</font>")
    }
    else if (!$("#projectname").val()) {
        $("#projectname").css('border', "1px solid #C8161D");
        $(".project_name_tip").html("<font color='#C8161D'>企业名称不能为空</font>")
    }
    else if (!$("#usermob").val()) {
        $("#usermob").css('border', "1px solid #C8161D");
        $(".project_mob_tip").html("<font color='#C8161D'>手机号码不能为空</font>")
    }
    else if ($("#usermob").val().length != 11) {
        $(".project_mob_tip").html("<font color='#C8161D'>请输入11位手机号码</font>");
        $("#usermob").focus();
    }
    else if (!reg0.test($("#usermob").val())) {
        $("#usermob").css('border', "1px solid #C8161D");
        $(".project_mob_tip").html("<font color='#C8161D'>请输入有效的手机号码</font>")
    }
    else if (!$("#useremail").val()) {
        $("#useremail").css('border', "1px solid #C8161D");
        $(".project_mail_tip").html("<font color='#C8161D'>邮箱地址不能为空</font>")
    }
    else if (!reg2.test($("#useremail").val())) {
        $("#useremail").css('border', "1px solid #C8161D");
        $(".project_mail_tip").html("<font color='#C8161D'>请输入有效的邮箱地址</font>")
    }
    else {
        $.messageLoad();
        common.postGateway('platform/user/companydinginfo', {
            "json": JSON.stringify([{
                "t_id": "",
                ding_corpid: $("#corpid").val(),
                ding_appid: $("#AgentId").val(),
                ding_appkey: $("#AppKey").val(),
                ding_appsecret: $("#AppSecret").val(),
                client_code: $("#projectcode").val(),
                client_name: $("#projectname").val(),
                phone_no: $("#usermob").val(),
                email: $("#useremail").val(),
                appcode: (common.getUrlParam("AppCode") == null ? "DWMS" : common.getUrlParam("AppCode"))
            }])
        },null, function (result) {
            if (result.resMsg == "") {
                $("#t_id").val(result.resId)
                $("#register1").show();
                $("#register2").hide();
                dingApp();
            }
            else {
                $.messageBoxBottom(0, result.resMsg);
            }
            $.messageBoxCloseLoading();
        });
    }
}

function dingApp() {
    $(".register_btn").hide();
    $(".register_read").show();
    $(".second_r").show();
}

function saveOrg() {
    $.messageLoad();
    common.postGateway('AliDingTalk/dingding/getaccess_token', {
        "appId": $("#AgentId").val(),
        "appKey": $("#AppKey").val(),
        "appSecret": $("#AppSecret").val()
    },null, function (result) {
        if (result.isSucceed == true) {
            common.postGateway('AliDingTalk/dingding/getdepartmentinfo', {
                "accessToken": result.json,
                "clientId": $("#t_id").val(),
                "departmentId": "1"
            },null, function (data) {
                if (data == "OK") {
                    $.messageBoxBottom(1, "钉钉组织架构同步完成！");
                }
                else {
                    $.messageBoxAlert(0, data);
                }
                $.messageBoxCloseLoading();
            });
        }
        else {
            $.messageBoxAlert(0, result.json);
            $.messageBoxCloseLoading();
        }
    });
}
function saveUser() {
    $.messageLoad();
    common.postGateway('AliDingTalk/dingding/getaccess_token', {
        "appId": $("#AgentId").val(),
        "appKey": $("#AppKey").val(),
        "appSecret": $("#AppSecret").val()
    }, null, function (result) {
        if (result.isSucceed == true) {
            common.postGateway('AliDingTalk/dingding/getdepartmentuserinfo', {
                "accessToken": result.json,
                "clientId": $("#t_id").val(),
                "departmentId": "1"
            },null, function (data) {
                if (data == "OK") {
                    $.messageBoxBottom(1, "钉钉员工信息同步完成！");
                }
                else {
                    $.messageBoxAlert(0, data);
                }
                $.messageBoxCloseLoading();
            });
        }
        else {
            $.messageBoxAlert(0, result.json);
            $.messageBoxCloseLoading();
        }
    });
}
function saveOk() {
    if ((!$("#userId").val()) || $('.user #userId').attr('data-tid') == "") {
        $("#userId").css('border', "1px solid #C8161D");
        $.messageBoxBottom(0, "请设置钉钉应用管理员！");
    }
    else {
        $.messageLoad();
        common.postGateway('platform/user/companydinginfo', {
            "json": JSON.stringify([{
                "t_id": $("#t_id").val(),
                ding_corpid: common.Encrypt($("#corpid").val()),
                ding_appid: $("#AgentId").val(),
                ding_appkey: $("#AppKey").val(),
                ding_appsecret: $("#AppSecret").val(),
                userid: $('.user #userId').attr('data-tid')
            }])
        },null, function (result) {
            if (result.resMsg == "") {
                $.messageBoxBottom(1, "钉钉应用设置完成！");
            }
            else {
                $.messageBoxBottom(0, result.resMsg);
            }
            $.messageBoxCloseLoading();
        });
    }
}