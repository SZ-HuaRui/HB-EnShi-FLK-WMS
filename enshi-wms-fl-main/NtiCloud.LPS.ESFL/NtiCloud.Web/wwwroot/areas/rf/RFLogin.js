$(function () {
    $('.rf_main').css('height', $(window).height());
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
    $(".rf_button").on('click', function () {
        loginBtn();
    })
    if ($("#url").val()) {
        $("#ware_House").hide();
    }
    $(".warehouse_ul").html('');
    apiRoute.selectDataEvent("select appcode PRODUCTCODE,appname PRODUCTNAME from prod_app_product where p_id<>0",
         [], "1=1", function (tableData) {
        for (var i = 0; i < tableData.length; i++) {
            //$("#wareHouse").append("<option value='" + tableData[i].productcode + "'>" + tableData[i].productname + "</option>");
            $(".warehouse_ul").append("<li class='warehouse_li' value='" + tableData[i].productcode + "'>" + tableData[i].productname + "</li>");
        }
        if (common.getUrlParam("AppCode") != null) {
            $("#wareHouse").attr('value',common.getUrlParam("AppCode"));
            $("#wareHouse").val(common.getUrlParam("AppCode"));
            $("#wareHouse").attr("disabled", true);
        }
    });

    function loginBtn() {
        var userName = $.trim($("#account").val());
        var passWord = $.trim($("#password").val());
        var userNameId = '#account';
        var passWordId = '#password';
        var JumpUrl = "/RF/Main/RFMain";
        if (common.getUrlParam("AppCode") != null) {
            JumpUrl = JumpUrl + "?AppCode=" + common.getUrlParam("AppCode");
        }
        else if ($("#url").val()) {
            JumpUrl = common.Decrypt($("#url").val()) + "?sessionKey=";
        }
        else {
            JumpUrl = JumpUrl + "?AppCode=" + $("#wareHouse").val();
        }
        if (common.getUrlParam("dbCode") != null) JumpUrl = JumpUrl + "&dbCode=" + common.getUrlParam("dbCode");
        CheckLogin(userName, passWord, userNameId, passWordId, JumpUrl);
    }
    var isShow = 0;
    $("#wareHouse,.down_img").on('click', function () {
        if (isShow == 0) {
            $('.warehouse_ul').show();
            isShow = 1;
        } else {
            $('.warehouse_ul').hide();
            isShow = 0;
        }
    })
    $(".warehouse_ul").on('click', ' .warehouse_li', function () {
        $('.warehouse_ul').hide();
        isShow = 0;
        $("#wareHouse").val($(this).attr('value'));
        $("#wareHouse").attr('value', $(this).attr('value'));

    })
});
