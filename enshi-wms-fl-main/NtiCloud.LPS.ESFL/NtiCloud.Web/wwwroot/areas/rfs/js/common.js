$(function () {
    $(".rf_out").click(function () {
        var r = confirm("确定退出系统吗？");
        if (r) {
            common.postGateway('platform/login/getlogout?sessionKey=' + common.getCookie("sessionKey"),
                {}, common.getToken(), function (response) {
                    common.removeCookie("sessionKey");
                    common.setCookie("sessionKey", "noLogin");
                    var urlpath = window.location.href.replace("http://", "").replace("https://", "").split("/");
                    window.location.href = '/' + urlpath[1] + '/Login';
            });
        }
    });
})