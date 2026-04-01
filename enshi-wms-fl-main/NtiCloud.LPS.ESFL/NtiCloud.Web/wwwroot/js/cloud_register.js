//勾选已阅读
var isRead = true, isDisplay = true, isCheckOk = false;
$(function () {
    $(".content").css("height", $(window).height());
    $(".back_main p").eq(1).hide();
    $(window).resize(function () {
        $(".content").css("height", $(window).height());
    });
    $('.isRead').on('click', function () {
        if (isRead) {
            $(this).removeClass('isRead').addClass('read_active');
            isRead = false;
        } else {
            $(this).removeClass('read_active').addClass('isRead');
            isRead = true;
        }
    });

    $('.input_d,.img_d').on('click', function () {
        if (isDisplay) {
            $('.data_select').fadeIn(0);
            isDisplay = false;
        } else {
            $('.data_select').fadeOut(0);
            isDisplay = true;
        }
    });
    $('.select_product_d').on('click', '.data_select li', function () {
        $('#prodcode').val($(this).attr("dataId"));
        $('.input_d').val($(this).text());
        $('.data_select').fadeOut(0);
        isDisplay = true;
    });

    var dingdingScan = common.DingDingScan();
    var url = encodeURIComponent('https://' + dingdingScan.host + '/Main/Register');
    var gotodd = encodeURIComponent('https://oapi.dingtalk.com/connect/oauth2/sns_authorize?appid=' + dingdingScan.appId + '&response_type=code&scope=snsapi_login&state=STATE&redirect_uri=') + url;
    var obj = DDLogin({
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
    if (common.getUrlParam("code") != null) {
        var timestamp = Date.parse(new Date());
        var code = common.getUrlParam("code");
        $.messageLoad();
        common.postGateway("platform/login/dingscanloginorregister", {
            host: window.location.host,
            code: code,
            timestamp: timestamp,
            action: 'register'
        }, null, function (checkLogin) {
            $.messageBoxCloseLoading();
            if (checkLogin.errcode != 0) {
                $.messageTips(checkLogin.json, '#usermob', [1, '#c8161d']);
            }
            else {
                $("#usermob").val(checkLogin.json.mobile);
                $("#usermob").attr("disabled", true);
                $(".input_phone").hide();
                $(".second_s").show();
                $("#username").val(checkLogin.json.name);
                $("#useremail").val(checkLogin.json.email);
            }
        });
    }

    $(".code_text").on('click', function () {
        var myreg = /^(((13[0-9]{1})|(14[0-9]{1})|(17[0-9]{1})|(15[0-3]{1})|(15[4-9]{1})|(18[0-9]{1})|(199))+\d{8})$/;
        var phone = $("#usermob").val();
        if (phone == '') {
            $(".phone_tip").html("<font color='#C8161D'>手机号码不能为空</font>");
            $("#usermob").focus();
        } else if (phone.length != 11) {
            $(".phone_tip").html("<font color='#C8161D'>请输入11位手机号码</font>");
            $("#usermob").focus();
        } else if (!myreg.test(phone)) {
            $(".phone_tip").html("<font color='#C8161D'>请输入有效的手机号码</font>");
            $("#usermob").focus();
        } else if ($(".code_text").html() == "获取验证码") {
            $(".code_text").html("正在发送……");
            common.postGateway("AliDingTalk/smssender/SMSSenderCode", {
                "phoneNumber": phone,
                "signName": "今天云平台",
                "templateCode": "SMS_192375272",
                "templateParam": JSON.stringify({ code: "@code" })
            }, null, function (msg) {
                if (msg.errcode == 0) {
                    $("#hiddenVlidataCode").val(msg.json);
                    settime($(".code_text"));
                }
                else {
                    $(".code_tip").html("<font color='#C8161D'>" + msg.errmsg + "</font>");
                    $(".code_text").html("发送失败");
                }
            });
        }
        var countdown = 60;
        function settime(obj) { //发送验证码倒计时
            if (countdown == 0) {
                //obj.attr('disabled', false);
                obj.html("获取验证码");
                countdown = 60;
                return;
            } else if (countdown) {
                //obj.attr('disabled', true);
                obj.html("重新发送(" + countdown + ")");
                countdown--;
            }
            setTimeout(function () {
                settime(obj);
            }, 1000);
        }
    });
});

function slider() {
    //一、定义一个获取DOM元素的方法
    var $ = function (selector) {
        return document.querySelector(selector);
    },
        box = $(".drag"),//容器
        bg = $(".drag_bg"),//背景
        text = $(".drag_text"),//文字
        btn = $(".drag_btn"),//滑块
        success = false,//是否通过验证的标志
        distance = box.offsetWidth - btn.offsetWidth;//滑动成功的宽度（距离）

    //二、给滑块注册鼠标按下事件
    btn.onmousedown = function (e) {
        //1.鼠标按下之前必须清除掉后面设置的过渡属性
        btn.style.transition = "";
        bg.style.transition = "";

        //说明：clientX 事件属性会返回当事件被触发时，鼠标指针向对于浏览器页面(或客户区)的水平坐标。

        //2.当滑块位于初始位置时，得到鼠标按下时的水平位置
        var ex = e || window.event;
        var downX = ex.clientX;

        //三、给文档注册鼠标移动事件
        document.onmousemove = function (e) {

            var ey = e || window.event;
            //1.获取鼠标移动后的水平位置
            var moveX = ey.clientX;

            //2.得到鼠标水平位置的偏移量（鼠标移动时的位置 - 鼠标按下时的位置）
            var offsetX = moveX - downX;

            //3.在这里判断一下：鼠标水平移动的距离 与 滑动成功的距离 之间的关系
            if (offsetX > distance) {
                offsetX = distance;//如果滑过了终点，就将它停留在终点位置
            } else if (offsetX < 0) {
                offsetX = 0;//如果滑到了起点的左侧，就将它重置为起点位置
            }

            //4.根据鼠标移动的距离来动态设置滑块的偏移量和背景颜色的宽度
            btn.style.left = offsetX + "px";
            bg.style.width = offsetX + "px";

            //如果鼠标的水平移动距离 = 滑动成功的宽度
            if (offsetX == distance) {

                //1.设置滑动成功后的样式
                text.innerHTML = "验证通过";
                text.style.color = "#fff";
                btn.style.background = "url(/areas/index/img/login_slide_right.png) center no-repeat";
                //btn.style.color = "green";
                bg.style.backgroundColor = "#ec928d";

                //2.设置滑动成功后的状态
                success = true;
                //成功后，清除掉鼠标按下事件和移动事件（因为移动时并不会涉及到鼠标松开事件）
                btn.onmousedown = null;
                document.onmousemove = null;

                //3.成功解锁后的回调函数
                setTimeout(function () {
                    isCheckOk = true;
                }, 100);
            }
        };

        //四、给文档注册鼠标松开事件
        document.onmouseup = function (e) {

            //如果鼠标松开时，滑到了终点，则验证通过
            if (success) {
                return;
            } else {
                //反之，则将滑块复位（设置了1s的属性过渡效果）
                btn.style.left = 0;
                bg.style.width = 0;
                btn.style.transition = "left 1s ease";
                bg.style.transition = "width 1s ease";
            }
            //只要鼠标松开了，说明此时不需要拖动滑块了，那么就清除鼠标移动和松开事件。
            document.onmousemove = null;
            document.onmouseup = null;
        };
    };
}
function ohterRegister(obj) {
    if (obj == '.ding_login' || obj == '.wechat_login') {
        $(obj).show();
        $('.delete').show();
        $('#register1').hide();
    } else {
        $('.qrcode_login').hide();
        $('.delete').hide();
        $('#register1').show();
    }
}
function registerMob() {
    $("#usermob").css('border', "1px solid #e2e2e2");
    $(".phone_tip").html("");
    var regmob = /^(((13[0-9]{1})|(14[0-9]{1})|(17[0-9]{1})|(15[0-3]{1})|(15[4-9]{1})|(18[0-9]{1})|(199))+\d{8})$/;
    if (!$("#usermob").val()) {
        $("#usermob").css('border', "1px solid #C8161D");
        $(".phone_tip").html("<font color='#C8161D'>请输入手机号码</font>");
        $("#usermob").focus();
    }
    else if (!regmob.test($("#usermob").val())) {
        $("#usermob").css('border', "1px solid #C8161D");
        $(".phone_tip").html("<font color='#C8161D'>请输入有效的手机号码</font>");
        $("#usermob").focus();
    }
    else {
        $.ajaxSettings.async = false;
        var userid = "";
        common.postGateway("platform/login/checkphone", {
            "userMob": $("#usermob").val()
        }, null, function (data) {
            userid = data;
        });
        $.ajaxSettings.async = true;
        if (userid != "NoUser") {
            $("#usermob").css('border', "1px solid #C8161D");
            $(".phone_tip").html("<font color='#C8161D'>该手机号已注册，请登录</font>");
            $("#usermob").focus();
        }
        else {
            $("#usermob").attr("disabled", true);
            $(".input_phone").hide();
            $(".second_s").show();
            $(".back_main p").eq(0).hide();
            $(".back_main p").eq(1).show();
        }
    }
}
function register() {
    $("#username").css('border', "1px solid #e2e2e2");
    $(".user_tip").html("");
    $("#userpswd1").css('border', "1px solid #e2e2e2");
    $(".password1_tip").html("");
    $("#userpswd2").css('border', "1px solid #e2e2e2");
    $(".password2_tip").html("");
    $(".check_tip").html("");
    $(".read_tip").html("");
    var reg1 = /^[a-zA-Z\$_][a-zA-Z\d\.\-_]*$/;
    if (!$("#VlidataCode").val()) {
        $("#VlidataCode").css('border', "1px solid #C8161D");
        $(".code_tip").html("<font color='#C8161D'>请输入验证码</font>");
        $("#VlidataCode").focus();
    }
    else if ($("#VlidataCode").val() != common.Decrypt($("#hiddenVlidataCode").val())) {
        $("#VlidataCode").css('border', "1px solid #C8161D");
        $(".code_tip").html("<font color='#C8161D'>验证码不正确</font>");
        $("#VlidataCode").focus();
    }
    else if (!$("#username").val()) {
        $("#username").css('border', "1px solid #C8161D");
        $(".user_tip").html("<font color='#C8161D'>请输入用户名</font>");
        $("#username").focus();
    }
    else if (!reg1.test($("#username").val())) {
        $("#username").css('border', "1px solid #C8161D");
        $(".user_tip").html("<font color='#C8161D'>请输入4到16位字母</font>");
        $("#username").focus();
    }
    else if (!$("#userpswd1").val()) {
        $("#userpswd1").css('border', "1px solid #C8161D");
        $(".password1_tip").html("<font color='#C8161D'>请输入密码</font>");
        $("#userpswd1").focus();
    }
    else if (!$("#userpswd2").val()) {
        $("#userpswd2").css('border', "1px solid #C8161D");
        $(".password2_tip").html("<font color='#C8161D'>请确认密码</font>");
        $("#userpswd2").focus();
    }
    else if ($("#userpswd1").val() != $("#userpswd2").val()) {
        $("#userpswd2").css('border', "1px solid #C8161D");
        $(".password2_tip").html("<font color='#C8161D'>两次输入的密码不一致</font>");
        $("#userpswd2").focus();
    }
    else if (!common.intensityReg($("#userpswd1").val())) {
        $("#userpswd1").css('border', "1px solid #C8161D");
        $(".password1_tip").html("<font color='#C8161D'>密码强度不够，请重新输入</font>");
        $("#userpswd1").focus();
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
    $("#adminusername").css('border', "1px solid #e2e2e2");
    $("#projectcode").css('border', "1px solid #e2e2e2");
    $("#projectname").css('border', "1px solid #e2e2e2");
    $("#useremail").css('border', "1px solid #e2e2e2");
    var reg1 = /^[a-zA-Z\$_][a-zA-Z\d\.\-_]*$/;
    var reg2 = /^[A-Za-z\d]+([-_.][A-Za-z\d]+)*@([A-Za-z\d]+[-.])+[A-Za-z\d]{2,4}$/;
    if (!$("#adminusername").val()) {
        $("#adminusername").css('border', "1px solid #C8161D");
        $(".adminuser_tip").html("<font color='#C8161D'>用户姓名不能为空</font>");
    }
    else if (!$("#projectcode").val()) {
        $("#projectcode").css('border', "1px solid #C8161D");
        $(".project_code_tip").html("<font color='#C8161D'>企业代码不能为空</font>");
    }
    else if (!reg1.test($("#projectcode").val())) {
        $("#projectcode").css('border', "1px solid #C8161D");
        $(".project_code_tip").html("<font color='#C8161D'>请输入字母或数字组合的企业代码</font>");
    }
    else if (!$("#projectname").val()) {
        $("#projectname").css('border', "1px solid #C8161D");
        $(".project_name_tip").html("<font color='#C8161D'>企业名称不能为空</font>");
    }
    else if (!$("#useremail").val()) {
        $("#useremail").css('border', "1px solid #C8161D");
        $(".project_mail_tip").html("<font color='#C8161D'>邮箱地址不能为空</font>");
    }
    else if (!reg2.test($("#useremail").val())) {
        $("#useremail").css('border', "1px solid #C8161D");
        $(".project_mail_tip").html("<font color='#C8161D'>请输入有效的邮箱地址</font>");
    }
    else {
        $.messageLoad();
        common.postGateway('platform/user/registeruser', {
            userid: $("#username").val(),
            userpswd: $("#userpswd1").val(),
            username: $("#adminusername").val(),
            usermob: $("#usermob").val(),
            projectcode: $("#projectcode").val(),
            projectname: $("#projectname").val(),
            useremail: $("#useremail").val(),
            defaultApp: [(common.getUrlParam("AppCode") == null ? "DWMS" : common.getUrlParam("AppCode"))]
        }, null, function (checkLogin) {
            $.messageBoxCloseLoading();
            if (checkLogin.userlogin != "OK") {
                if (checkLogin.id == 1) {
                    $.messageBoxAlert(0, checkLogin.msg, function () {
                        $("#register1").show();
                        $("#register2").hide();
                    });
                }
                else {
                    $.messageBoxAlert(0, checkLogin.msg);
                }
            }
            else {
                $.messageBoxAlert(1, "用户注册成功！", function () {
                    common.removeCookie("sessionKey");
                    common.setCookie("sessionKey", checkLogin.guid);
                    localStorage.setItem("token", common.Encrypt(checkLogin.guid));
                    sessionStorage.setItem("encrypt", checkLogin.encrypt);
                    location.href = "/Member/ControlDesk";
                });
            }
        });
    }
}