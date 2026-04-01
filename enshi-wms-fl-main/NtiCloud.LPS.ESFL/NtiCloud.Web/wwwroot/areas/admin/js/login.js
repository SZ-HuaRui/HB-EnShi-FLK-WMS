$(function () {
    if (window != top || parent.document != this) { // 判断当前的window对象是否是top对象  || 让login页面始终在最上层，不嵌入到别的frame中
        top.location.href = window.location.href; // 如果不是，将top对象的网址自动导向被嵌入网页的网址  
    };
    var userAgent = navigator.userAgent;
    if (userAgent.indexOf("DingTalk") != -1) {//通过钉钉登录
        window.localStorage.removeItem("token");
        common.postGateway('website/selectdata', {
            "type": "clientId",
            "parameters": [{ "clientid": $("#clientId").val() }]
        }, null, function (tableData) {
            tableData = JSON.parse(tableData.json);
            if (tableData.length == 0) {
                $.messageBoxAlert(0, "钉钉企业信息非法，请更新后重试！");
            }
            else {
                var JumpUrl = "/" + $("#userarea").val() + "/Main";
                if ($("#AppCode").val() != null && $("#AppCode").val() != "") JumpUrl = JumpUrl + "?AppCode=" + $("#AppCode").val();
                if (common.getUrlParam("dbCode") != null) JumpUrl = JumpUrl + "&dbCode=" + common.getUrlParam("dbCode");
                JumpUrl = JumpUrl + "&ClientId=" + $("#clientId").val();
                DingOpenLogin(JumpUrl, common.Decrypt(tableData[0].ding_corpid), false);
            }
        });
    }
    else {
        common.removeNoLogin();
    }

    $("#admin_login").css('height', $(window).height());
    $('.login').css('top', ($(window).height()) / 2 - $('.login').innerHeight());
    $('.system_info').css('top', ($(window).height() - $('.system_info').innerHeight() - 232) / 2 - 65);
    $(window).resize(function () {
        $("#admin_login").css('height', $(window).height());
        $('.login').css('top', ($(window).height()) / 2 - $('.login').innerHeight());
        $('.system_info').css('top', ($(window).height() - $('.system_info').innerHeight() - 232) / 2 - 65);
    });
    common.postGateway("website/selectdata", {
        "type": "language",
        "parameters": []
    }, null, function (data) {
        data = JSON.parse(data.json);
        if (data.length > 1) {
            $(".local_ul").html("");
            for (var i = 0; i < data.length; i++) {
                $(".local_ul").append('<li data_id="' + data[i].languagecode + '">' + data[i].languagename + '</li>');
                if (common.getCookie("localLanguage") == data[i].languagecode) {
                    $('.local_span').text(data[i].languagename);
                }
                else if (data[i].isdefault == "1" && common.getCookie("localLanguage") == null) {
                    $('.local_span').text(data[i].languagename);
                    common.setCookie("localLanguage", data[i].languagecode);
                }
            }
            //点击选择语言
            $('.local_ul li').on('click', function () {
                common.setCookie("localLanguage", $(this).attr("data_id"));
                $('.local_span').text($(this).text());
            });
        }
        else {
            //点击选择语言
            $('.local_ul li').on('click', function () {
                common.setCookie("localLanguage", $(this).attr("data_id"));
                $('.local_span').text($(this).text());
            });
        }
    });
    //点击语言栏显示或者隐藏下拉列表
    $('.language .local_span,.language .icon').click(function () {
        if ($('.language .local_ul').hasClass('hide')) {
            $('.language .local_ul').fadeIn().removeClass('hide');
            $('.language .icon').addClass('icon_up').removeClass('icon_down');
        } else {
            $('.language .local_ul').addClass('hide').fadeOut();
            $('.language .icon').removeClass('icon_up').addClass('icon_down');
        };
        return false;
    });

    //隐藏语言选择栏
    $('#admin_login').click(function () {
        if (!$('.language .local_ul').hasClass('hide')) {
            $('.language .local_ul').addClass('hide').fadeOut();
            $('.language .icon').removeClass('icon_up').addClass('icon_down');
        };
    });
    //enter提交input框数据
    $("#username_input,#password_input").keydown(function (e){
        if (e.which == 13) {
            document.getElementById("login_act").click();
            //jq的表示方式失效 不能触发layui的事件
            //$('#login_act').click();
            //$('#login_act').trigger('click');
            return false;
        };
    });
    //layui抓取页面数据和对应的提交事件
    layui.use('form', function(){
        var form = layui.form;  
        //监听提交
        form.on('submit(formLogin)', function (data){
            LoginSys(data.field);
            return false;
        });
    });
    $('#reg_act').click(function () {
        var AppCode = common.getUrlParam("AppCode");
        if (AppCode != null && AppCode !== "") {
            AppCode = "?AppCode=" + AppCode
        }
        else {
            AppCode = "";
        }
        location.href = "/Main/Register" + AppCode;
        /*
        $.messageBoxOpenBtn("用户注册", ['300px', '300px'], "<div style='padding:20px;line-height:135%'>"
            + "方式一：<br>通过平台注册企业帐号，由管理员添加用户分配权限使用；<br><br>"
            + "方式二：<br>通过钉钉注册，由钉钉同步组织及人员，直接从钉钉入口登录。"
            +"</div > ", [
            {
                text: '平台注册',
                    handler: function (index) {
                    location.href = "/Main/Register" + AppCode;
                    $.messageBoxClose(index);
                }
            }, {
                text: '钉钉入口',
                handler: function (index) {
                    location.href = "/DingDing/Register" + AppCode;
                    $.messageBoxClose(index);
                }
            }
        ]);
        */
    });
});


function LoginSys(data) {
    var userName = $.trim(data.username);
    var passWord = $.trim(data.password);
    var clientId = $.trim(data.clientId);
    var userNameId = '#username_input';
    var passWordId = '#password_input';
    common.setCookie("warehouseId", "");
    common.removeCookie("warehouseId");
    var JumpUrl = "/" + data.userarea + "/Main";
    if ($("#AppCode").val() != null && $("#AppCode").val() != "") JumpUrl = JumpUrl + "?AppCode=" + $("#AppCode").val();
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
