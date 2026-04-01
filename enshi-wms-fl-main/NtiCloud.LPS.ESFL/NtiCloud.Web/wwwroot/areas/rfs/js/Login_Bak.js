$(function () {
    $("#account").focus();
    $("#account").keydown(function (event) {
        if (event.which == 13) {
            loginBtn();      
            return false;
        }
    });
    $("#password").keydown(function (event) {
        if (event.which == 13) {
            loginBtn();
            return false;
        }
    });
    $(".rf_login").keydown(function (event) {
        if (event.which == 13) {
            loginBtn();
            return false;
        }
    });
    $(".rf_login").on('click', function () {
        loginBtn();
    })
})

function loginBtn() {
    var account = $('#account').val();
    var password = $('#password').val();
    var userip = $("#userip").val();
    var clientId = $("#clientId").val();
    common.removeCookie("clientId");
    common.setCookie("clientId", clientId);
    if (!account) {
        alert('请输入账号!');
        $("#account").focus();
        return false;
    }
    else if (!password) {
        alert('请输入密码!');
        $("#password").focus();
        return false;
    }
    else {
        CheckLogin(account, password, '#account', '#password', "/RFS/Main");        
    }
}