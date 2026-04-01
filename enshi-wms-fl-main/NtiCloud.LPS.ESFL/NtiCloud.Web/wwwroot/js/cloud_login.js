$(function () {
    common.removeNoLogin();
    var userAgent = navigator.userAgent;
    if (userAgent.indexOf("DingTalk") != -1) {//通过钉钉登录
        DingOpenLogin("/Member/ControlDesk", 'ding5033bbcf5247e61b35c2f4657eb6378f', false);
    }

    $(".content").css("height", $(window).height());
    $(window).resize(function () {
        $(".content").css("height", $(window).height());
    });
    $('.main_ul li').on('click', function () {
        var index = $(this).index();
        $(this).addClass('active_title').siblings().removeClass('active_title');
        if (index == 0) {
            $('#username').attr('placeholder', "输入用户名");
            $('.user').show();
            $('.password').show();
            $('.valida').hide();
        } else {
            $('#username').attr('placeholder', "输入手机号码");
            $('#username').val('');
            $('.user_tip').text('');
            $('.user').show();
            $('.password').hide();
            $('.valida').show();
        }
    });
    $(".code_text").on('click', function () {
        Phone("#username");
    });
    var dingdingScan = common.DingDingScan();
    var url = encodeURIComponent('https://' + dingdingScan.host + '/Main/Login');
    var gotodd = encodeURIComponent('https://oapi.dingtalk.com/connect/oauth2/sns_authorize?appid=' + dingdingScan.appId + '&response_type=code&scope=snsapi_login&state=STATE&redirect_uri=') + url;
    var objdd = DDLogin({
        "id": "login_container",//这里需要你在自己的页面定义一个HTML标签并设置id，例如<div id="login_container"></div>或<span id="login_container"></span>
        "goto": gotodd, //请encodeURIComponent
        "style": "border:none;background-color:#FFFFFF;",
        "width": "280",
        "height": "315"
    });
    var hanndleMessage = function (event) {
        var origin = event.origin;
        if (origin == "https://login.dingtalk.com") { //判断是否来自ddLogin扫码事件。
            var loginTmpCode = event.data; //拿到loginTmpCode后就可以在这里构造跳转链接进行跳转了
            location.href = "https://oapi.dingtalk.com/connect/oauth2/sns_authorize?appid=" + dingdingScan.appId + "&response_type=code&scope=snsapi_login&state=STATE&redirect_uri=" + url + "&loginTmpCode=" + loginTmpCode;
        }
    };
    if (typeof window.addEventListener != 'undefined') {
        window.addEventListener('message', hanndleMessage, false);
    } else if (typeof window.attachEvent != 'undefined') {
        window.attachEvent('onmessage', hanndleMessage);
    }

    if (common.getUrlParam("user") == "WX" && common.getUrlParam("code") != null) {//企业微信
        WXScanLogin('#username', "/Member/ControlDesk");
    }
    if (common.getUrlParam("user") == "WeiXin" && common.getUrlParam("code") != null) {//微信
        WeiXinScanLogin('#username', "/Member/ControlDesk");
    }
    else if (common.getUrlParam("code") != null) {//钉钉
        DingScanLogin('#username', "/Member/ControlDesk");
    }
    //微信
    var objwx = new WxLogin({
        self_redirect: false,
        id: "login_weixin_wx",
        appid: "wx343971ab1ec05617",
        scope: "snsapi_login",
        redirect_uri: "http://cloud.nti56.com/Main/Login?user=WeiXin",
        state: "",
        style: "border:none;background-color:#FFFFFF;",
        href: ""
    });
    //企业微信
    window.WwLogin({
        id: "login_weixin",
        appid: "wx659d1718e214b6c9",
        agentid: "1000005",
        redirect_uri: "http://cloud.nti56.com/Main/Login?user=WX",
        state: "",
        style: "border:none;background-color:#FFFFFF;",//样式
        href: ""  //样式    样式查阅开发文档即可
    });
});
function ohterLogin(obj) {
    if (obj == '.ding_login' || obj == '.wechat_login' || obj == '.wechat_login_wx') {
        $(obj).show();
        $('.delete').show();
        $('.phone_login').hide();
    } else {
        $('.qrcode_login').hide();
        $('.delete').hide();
        $('.phone_login').show();
    }
}

function CheckUser(userNameId, userPswdId) {
    var username = $("#" + userNameId).val();
    var userpswd = $("#" + userPswdId).val();
    var usercode = $("#VlidataCode").val();
    if (!username && $('.valida').is(":visible")) {
        document.getElementById(userNameId).style.borderStyle = "solid";//边框样式
        document.getElementById(userNameId).style.borderColor = "#C8161D";//边框颜色
        document.getElementById(userNameId).style.borderWidth = "1px";//边框宽度
        $(".user_tip").html("<font color='#C8161D'>请输入手机号</font>");
        $("#" + userNameId).focus();
    }
    else if (!username) {
        document.getElementById(userNameId).style.borderStyle = "solid";//边框样式
        document.getElementById(userNameId).style.borderColor = "#C8161D";//边框颜色
        document.getElementById(userNameId).style.borderWidth = "1px";//边框宽度
        $(".user_tip").html("<font color='#C8161D'>请输入用户名</font>");
        $("#" + userNameId).focus();
    }
    else if (!userpswd && $('.valida').is(":visible") == false) {
        $("#" + userPswdId).css('border', "1px solid #C8161D");
        $(".password_tip").html("<font color='#C8161D'>请输入密码</font>");
        $("#" + userPswdId).focus();
    }
    else if (!usercode && $('.valida').is(":visible")) {
        $("#VlidataCode").css('border', "1px solid #C8161D");
        $(".user_tip").html("<font color='#C8161D'>请填写验证码</font>");
        $("#VlidataCode").focus();
    }
    else if ($('.valida').is(":visible") && usercode != common.Decrypt($("#hiddenVlidataCode").val())) {
        $("#VlidataCode").css('border', "1px solid #C8161D");
        $(".user_tip").html("<font color='#C8161D'>验证码不正确</font>");
        $("#VlidataCode").focus();
    }
    else if ($('.valida').is(":visible")) {
        SmSLogin(username, '#username', "/Member/ControlDesk");
    }
    else {
        CheckLogin(username, userpswd, '#username', '#userpswd', "/Member/ControlDesk");
    }
}