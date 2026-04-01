
$(function () {
    $(".content").css("height", $(window).height());
    //$('.main_content').css('margin-top', ($(window).height() - $('.navbar').height() - $('.main_content').innerHeight()) / 2 - $('.navbar').height() / 2);
    $(window).resize(function () {
        $(".content").css("height", $(window).height());
        //$('.main_content').css('margin-top', ($(window).height() - $('.navbar').height() - $('.main_content').innerHeight()) / 2 - $('.navbar').height() / 2);
    });

    $(".input_phone").show();
    $(".reset_phone").hide();

    $("#usermob").focus();
    $(".register_btn").click(function () {
        var myregPhone = /^(((13[0-9]{1})|(14[0-9]{1})|(17[0-9]{1})|(15[0-3]{1})|(15[4-9]{1})|(18[0-9]{1})|(199))+\d{8})$/;
        var mail = /^[A-Za-z\d]+([-_.][A-Za-z\d]+)*@([A-Za-z\d]+[-.])+[A-Za-z\d]{2,4}$/;
        var usermob = $("#usermob").val();
        if (usermob == '') {
			$("#usermob").css('border', "1px solid #C8161D");
            $(".user_tip").html("<font color='#C8161D'>手机号或邮箱不能为空</font>");
            $("#usermob").focus();
        }
        else if (!myregPhone.test(usermob) && !mail.test(usermob)) {
			$("#usermob").css('border', "1px solid #C8161D");
            $(".user_tip").html("<font color='#C8161D'>请输入有效的手机号码或邮箱</font>");
            $("#usermob").focus();
        }
        else if (!$("#VlidataCode").val()) {
            $("#VlidataCode").css('border', "1px solid #C8161D");
            $(".var_code").html("<font color='#C8161D'>请填写验证码</font>");
            $("#VlidataCode").focus();
        }
        else if ($("#VlidataCode").val() != common.Decrypt($("#hiddenVlidataCode").val())) {
            $("#VlidataCode").css('border', "1px solid #C8161D");
            $(".var_code").html("<font color='#C8161D'>验证码不正确</font>");
            $("#VlidataCode").focus();
        }
        else {
            $(".input_phone").hide();
            $(".reset_phone").show();
        }
    });

    $(".code_text").on('click', function () {
        var myregPhone = /^(((13[0-9]{1})|(14[0-9]{1})|(17[0-9]{1})|(15[0-3]{1})|(15[4-9]{1})|(18[0-9]{1})|(199))+\d{8})$/;
        var mail = /^[A-Za-z\d]+([-_.][A-Za-z\d]+)*@([A-Za-z\d]+[-.])+[A-Za-z\d]{2,4}$/;
        var usermob = $("#usermob").val();
        if (usermob == '') {
			$("#usermob").css('border', "1px solid #C8161D");
            $(".user_tip").html("<font color='#C8161D'>手机号或邮箱不能为空</font>");
            $("#usermob").focus();
        }
        else if (!myregPhone.test(usermob) && !mail.test(usermob)) {
			$("#usermob").css('border', "1px solid #C8161D");
            $(".user_tip").html("<font color='#C8161D'>请输入有效的手机号码或邮箱</font>");
            $("#usermob").focus();
        }
        else if (myregPhone.test(usermob) && $(".code_text").html() == "获取验证码") {
            $(".code_text").html("正在发送……");
            common.postGateway("AliDingTalk/smssender/SMSSenderCode", {
                "phoneNumber": usermob,
                "signName":"今天云平台",
                "templateCode": "SMS_192375271",
                "templateParam": JSON.stringify({ code: "@code" })
            },null, function (msg) {
                if (msg.errcode == 0) {
                    $("#hiddenVlidataCode").val(msg.json);
                    settime($(".code_text"));
                }
                else {
                    $(".user_tip").html("<font color='#C8161D'>" + msg.errmsg + "</font>");
                    $(".code_text").html("发送失败");
                }
            });
        }
        else if (mail.test(usermob) && $(".code_text").html() == "获取验证码") {
            $(".code_text").html("正在发送……");
            common.postGateway("platform/mailsender/sendmail", {
                "mailFrom": "*",
                "mailPwd": "*",
                "mailSubject":"今天云平台密码重置",
                "mailBody": "您好！<br>今天云平台密码重置验证码为<div style=\"color:red\">@code</div>请于30分钟内填写。如非本人操作，请忽略本邮件。<br>动态验证码，请勿泄漏他人！",
                "isBodyHtml": true,
                "host": "*",
                "mailTo": usermob,
                "mailCC": "*"
            },null, function (msg) {
                if (msg.isSucceed == true) {
                    $("#hiddenVlidataCode").val(msg.json);
                    settime($(".code_text"));
                }
                else {
                    $(".user_tip").html("<font color='#C8161D'>" + msg.json + "</font>");
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
    })
    $(".botton_p").click(function () {
        var password = $(".password").val();
        var password_c = $(".password_c").val();
        if (password == '') {
			$("#pwd1").css('border', "1px solid #C8161D");
            $(".password_tip").html("<font color='#C8161D'>密码不能为空</font>");
            $(".password").focus();
        }
        else if (password_c == '') {
			$("#pwd2").css('border', "1px solid #C8161D");
            $(".password_c_tip").html("<font color='#C8161D'>确认密码不能为空</font>");
            $(".password_c").focus();
        }
        else if (password != password_c) {
			$("#pwd2").css('border', "1px solid #C8161D");
            $(".password_c_tip").html("<font color='#C8161D'>两次输入密码不一致，请重新输入</font>"); 
            return false;
        }
        else if (!common.intensityReg(password)) {
            $("#pwd1").css('border', "1px solid #C8161D");
            $(".password_tip").html("<font color='#C8161D'>密码强度不够，请重新输入</font>");
            $(".password").focus();
        }
		else{
			common.postGateway("platform/user/resetpassword", {
				"mailOrPhone": $("#usermob").val(),
				"pswdNew": password_c
			},null, function (msg) {
				//alert(JSON.stringify(msg))
                if (msg.isSucceed == true) {
					location.href = "/Main/Login";
				}
				else {
					$.messageMsg(msg.json);
				}
			});	
		}
    });
});

