$(function () {
    //点击个人资料
    $('#person_info').on('click', function () {
        $('#div1').css({ "display": "block" });
        $('.main_content,.product_project').css({ "display": "none" });
        $('.main_content,.div_d,.pagination_d,.search_b').css({ "display": "none" });
    })
    //点击个人资料中的关闭
    $('.close_d').on('click', function () {
        $('#div1').css({ "display": "none" });
        $('.main_content,.product_project').css({ "display": "block" });
        $('.main_content,.div_d,.pagination_d,.search_b').css({ "display": "block" });
    })
    //判断通过账号密码登录  且登录密码是否达到三级等保要求
    if (window.localStorage.getItem('intensityReg') == 'false') {
        $.messageBoxOpenBtn('修改密码', ['350px', '250px'], '#pc_mdfpassword', [
            { text: '修改密码', handler: function (index) { common.modifyPsword([$('#Password1'), $('#Password2'), $('#Password3')]); } }
        ], 0);
        $.messageBoxAlert(0, "登录密码不符合三级等保要求，请修改密码！<br>1、密码长度大于等于8个字符；<br>2、必须是大写字母，小写字母，数字，特殊字符(_!@#$%^&*`~()-+=/)中任意三个组合!");
    };
    //点击修改密码
    $('#layout_mdf').on('click', function () {
        $.messageBoxOpenBtn('修改密码', ['350px', '250px'], '#pc_mdfpassword', [
            { text: '修改密码', handler: function (index) { common.modifyPsword([$('#Password1'), $('#Password2'), $('#Password3')]); } }
        ]);
    });
})
//帐号密码方式登录
function CheckLogin(userName, passWord, userNameId, passWordId, JumpUrl) {
    if (!userName) {
        $.messageTips('请输入用户名!', userNameId, [3, '#c8161d']);
        $.messageBoxCloseLoading();
        return false;
    } else if (!passWord) {
        $.messageTips('请输入登录密码!', passWordId, [3, '#c8161d']);
        $.messageBoxCloseLoading();
        return false;
    } else {
        apilogin(false);
        function apilogin(islogin){
            $.messageLoad();
            var jsonLogin = {
                "username": userName,
                "userpswd": common.Encrypt(passWord),
                "islogin": islogin
            };
            common.postGateway('platform/login/getlogin', jsonLogin, null, function (checkLogin) {
                $.messageBoxCloseLoading();
                if (checkLogin.isSucceed == true && checkLogin.errcode != 0) {
                    $.messageBoxConfirm(checkLogin.json, ['继续登录', '取消登录'], function () {
                        apilogin(true);
                    });
                }
                else if (checkLogin.isSucceed == false ) {
                    $.messageBoxAlert(0, checkLogin.json ? checkLogin.json : checkLogin.errmsg);
                    return;
                } else {
                    //加入登录密码是否符合三级等保要求的检验 校验结果存入localstorage
                    if (!common.intensityReg(passWord)) {
                        window.localStorage.setItem('intensityReg', 'false');
                    } else {
                        window.localStorage.setItem('intensityReg', 'true');
                    };
                    common.removeCookie("sessionKey");
                    common.setCookie("sessionKey", checkLogin.json.guid);
                    localStorage.setItem("token", common.Encrypt(checkLogin.json.guid));
                    sessionStorage["encrypt"] = checkLogin.json.encrypt;
                    if (JumpUrl.indexOf("?sessionKey=") != -1) {
                        JumpUrl += checkLogin.json.guid;
                    }
                    location.href = JumpUrl;
                    //location.replace(JumpUrl);
                }
            });
        }
    }
}

//手机验证码方式
function SmSLogin(userName, userNameId, JumpUrl) {
    $.messageLoad();
    common.postGateway('platform/login/postsmslogin', {
        "userMob": userName,
    },null, function (checkLogin) {
        $.messageBoxCloseLoading();
        if (checkLogin.errcode != 0) {
            $.messageTips(checkLogin.json, userNameId, [3, '#c8161d']);
        }
        else {
            common.removeCookie("sessionKey");
            common.setCookie("sessionKey", checkLogin.json.guid);
            localStorage.setItem("token", common.Encrypt(checkLogin.json.guid));
            sessionStorage["encrypt"] = checkLogin.json.encrypt;
            location.href = JumpUrl;
        }
    });
}

//钉钉扫描方式登录
function DingScanLogin(userNameId, JumpUrl) {
    var timestamp = Date.parse(new Date());
    var code = common.getUrlParam("code");
    $.messageLoad();
    common.postGateway("platform/login/dingscanloginorregister", {
        host: window.location.host,
        code: code,
        timestamp: timestamp,
        action:'login'
    },null, function (checkLogin) {
        $.messageBoxCloseLoading();
        if (checkLogin.errcode != 0) {
            $.messageTips(checkLogin.json, userNameId, [3, '#c8161d']);
        }
        else {
            common.removeCookie("sessionKey");
            common.setCookie("sessionKey", checkLogin.json.guid);
            localStorage.setItem("token", common.Encrypt(checkLogin.json.guid));
            sessionStorage["encrypt"] = checkLogin.json.encrypt;
            location.href = JumpUrl;
        }
    })
}

//钉钉内打开应用方式
function DingOpenLogin(JumpUrl, corpId, isExternal) {
    var appCode = "";
    if (JumpUrl.indexOf("?") == -1) {
        JumpUrl = JumpUrl + "?corpId=" + corpId;
    }
    var splitUrl = JumpUrl.split('?');
    var parmUrl = splitUrl[1].split('&');
    JumpUrl = splitUrl[0];
    for (var i = 0; i < parmUrl.length; i++) {
        if (parmUrl[i].toLowerCase().indexOf("clientid") != 0 && parmUrl[i].toLowerCase().indexOf("corpid") != 0 && parmUrl[i].toLowerCase().indexOf("sessionKey") != 0) {
            if (JumpUrl.indexOf("?") == -1) {
                JumpUrl += "?" + parmUrl[i];
            }
            else {
                JumpUrl += "&" + parmUrl[i];
            }
        }
    }
    var reg = new RegExp("(^|&)AppCode=([^&]*)(&|$)", "i"); //构造一个含有目标参数的正则表达式对象
    var r = splitUrl[1].match(reg);  //匹配目标参数
    if (r != null) { appCode = unescape(r[2]); } //返回参数值
    if (appCode == "DWMS") appCode = "DDWMS";

    dd.ready(function () {
        dd.runtime.permission.requestAuthCode({
            corpId: corpId,
            onSuccess: function (result) {
                var code = result.code;
                $.messageLoad();
                common.postGateway('platform/login/dingopenlogin', {
                    corpId: common.Encrypt(corpId),
                    userCode: code,
                    appCode: appCode
                }, null, function (checkLogin) {
                    $.messageBoxCloseLoading();
                    if (checkLogin.errcode == 200 && checkLogin.errmsg.indexOf("首次登录") != -1) {
                        DingOpenLogin(JumpUrl, corpId, isExternal);//首次登录新增用户
                    }
                    else if (checkLogin.errcode != 0) {
                        if (appCode == "COST") {
                            $.messageMsg('首次添加应用，正在同步数据……', { time: 1800, icon: 6, shade: [0.1, '#fff'] });
                            common.postGateway('AliDingTalk/DingSubscribe/DCostCompanyInit', {
                                authCorpid: corpId,
                                authCode: code
                            }, null, function (res) {
                                DingOpenLogin(JumpUrl, corpId, isExternal);//首次报价管家
                            });
                        }
                        else if (appCode == "DDWMS") {
                            $.messageMsg('首次添加应用，正在同步数据……', { time: 1800, icon: 6, shade: [0.1, '#fff'] });
                            dd.runtime.permission.requestAuthCode({
                                corpId: corpId,
                                onSuccess: function (result) {
                                    var code = result.code;
                                    common.postGateway('dwms/Init/DDCompanyInit', {
                                        authCorpid: corpId,
                                        authCode: code,
                                        appCode: "DDWMS"
                                    }, null, function (res) {
                                        DingOpenLogin(JumpUrl, corpId, isExternal);//首次今天云仓
                                    });
                                }
                            })
                        }
                        else {
                            $.messageBoxAlert(0, checkLogin.json);
                        }
                    }
                    else {
                        common.removeCookie("sessionKey");
                        common.setCookie("sessionKey", checkLogin.json.guid);
                        localStorage.setItem("token", common.Encrypt(checkLogin.json.guid));
                        sessionStorage["encrypt"] = checkLogin.json.encrypt;
                        if (JumpUrl.indexOf("?") == -1) {
                            JumpUrl = JumpUrl + "?ClientId=" + checkLogin.clientId + "&corpId=" + corpId;
                        }
                        else {
                            JumpUrl = JumpUrl + "&ClientId=" + checkLogin.clientId + "&corpId=" + corpId;
                        }
                        if (isExternal) {
                            JumpUrl = JumpUrl + "&sessionKey=" + checkLogin.json.guid;
                        }
                        location.href = JumpUrl;
                        //location.replace(JumpUrl);
                    }
                });
            },
            onFail: function (err) {
                $.messageBoxCloseLoading();
                $.messageBoxAlert(0, JSON.stringify(err));
            }
        });
    });
}

//手机验证码
function Phone(phoneID) {
    var myreg = /^(((13[0-9]{1})|(14[0-9]{1})|(17[0-9]{1})|(15[0-3]{1})|(15[4-9]{1})|(18[0-9]{1})|(199))+\d{8})$/;
    var phone = $(phoneID).val();
    if (phone == '') {
        $(".user_tip").html("<font color='#ff0000'>手机号码不能为空</font>");
        $(phoneID).focus();
    } else if (phone.length != 11) {
        $(".user_tip").html("<font color='#ff0000'>请输入11位手机号码</font>");
        $(phoneID).focus();
    } else if (!myreg.test(phone)) {
        $(".user_tip").html("<font color='#ff0000'>请输入有效的手机号码</font>");
        $(phoneID).focus();
    } else if ($(".code_text").html() == "获取验证码") {
        $(".code_text").html("正在发送……");
        common.postGateway("AliDingTalk/smssender/SMSSenderCode", {
            "phoneNumber": phone,
            "signName": "今天云平台",
            "templateCode": "SMS_192375274",
            "templateParam": JSON.stringify({ code: "@code" })
        }, null, function (msg) {
            if (msg.errcode == 0) {
                $("#hiddenVlidataCode").val(msg.json);
                settime($(".code_text"));
            }
            else {
                $(".user_tip").html("<font color='#ff0000'>" + msg.errmsg + "</font>");
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
            settime(obj)
        }, 1000)
    }
}

//退出登录
function LogOut() {
    $.messageBoxConfirm('确认要退出系统吗？', ['确定', '取消'], function () {
        common.postGateway('platform/login/getlogout', { "sessionKey": common.getCookie("sessionKey") }, common.getToken(), function () {
            common.removeNoLogin();
            window.location.href = '/';
        });
    });
}


//企业微信扫描方式登录
function WXScanLogin(userNameId, JumpUrl) {
    var timestamp = Date.parse(new Date());
    var code = common.getUrlParam("code");
    $.messageLoad();
    common.postGateway("platform/weixin/wxscanloginorregister", {
        host: window.location.host,
        code: code,
        timestamp: timestamp,
        action: 'login'
    }, null, function (checkLogin) {
        $.messageBoxCloseLoading();
        if (checkLogin.errcode != 0) {
            $.messageTips(checkLogin.json, userNameId, [3, '#c8161d']);
        }
        else {
            common.removeCookie("sessionKey");
            common.setCookie("sessionKey", checkLogin.json.guid);
            localStorage.setItem("token", common.Encrypt(checkLogin.json.guid));
            sessionStorage["encrypt"] = checkLogin.json.encrypt;
            location.href = JumpUrl;
        }
    })
}

//微信扫描方式登录
function WeiXinScanLogin(userNameId, JumpUrl) {
    var code = common.getUrlParam("code");
    $.messageLoad();
    common.postGateway("platform/weixin/wxscanloginorregister_wx", {
        host: window.location.host,
        code: code,
        action: 'login'
    }, null, function (checkLogin) {
        $.messageBoxCloseLoading();
        if (checkLogin.errcode != 0) {
            $.messageTips(checkLogin.json, userNameId, [3, '#c8161d']);
        }
        else {
            common.removeCookie("sessionKey");
            common.setCookie("sessionKey", checkLogin.json.guid);
            localStorage.setItem("token", common.Encrypt(checkLogin.json.guid));
            sessionStorage["encrypt"] = checkLogin.json.encrypt;
            location.href = JumpUrl;
        }
    })
}