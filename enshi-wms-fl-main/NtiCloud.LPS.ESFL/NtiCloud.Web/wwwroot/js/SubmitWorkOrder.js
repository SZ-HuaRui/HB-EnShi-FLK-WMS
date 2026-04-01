$(function () {
    //$(".content").css("height", 'auto');
    //$(".container_left").css("height", $(window).height() - $('.header').height());
    //$('.container_main').css({ "height": 'auto', "width": $(window).width() - $('.container_left').width()});
    //$(window).resize(function () {
    //    $(".content").css("height", 'auto');
    //    $(".container_left").css("height", $(window).height() - $('.header').height());
    //    $('.container_main').css({ "height": 'auto', "width": $(window).width() - $('.container_left').width() });
    //})
    
    $(".login_out").click(function () {
        LogOut();
    });

    //选择产品
    $('.main_product .right_btn').on('click', function () {
        var product_title = $(this).prev().children('.center_title').text();
        var product_code = $(this).prev().children('.center_code').text();
        $('.step_three .product_t').text(product_title).attr('product-code', product_code);
        $(this).parent().parent().parent().parent().parent().hide().next().show();
        $('.right_main_top .number_p').eq(0).children('.number').removeClass('number_color');
        $('.right_main_top .number_p').eq(0).children('.number_title').removeClass('title_color')
        $('.right_main_top .number_p').eq(1).children('.number').addClass('number_color');
        $('.right_main_top .number_p').eq(1).children('.number_title').addClass('title_color');
    })

    //选择产品的问题
    $('.step_two .choose').on('click', function () {
        var issue_title = $(this).prev().text();
        var issue_code = $(this).attr('id');
        $('.step_three .product_d').text(issue_title).attr('issue-code', issue_code);
        $(this).parent().parent().parent().parent().parent().hide().next().show();

        $('.right_main_top .number_p').eq(1).children('.number').removeClass('number_color');
        $('.right_main_top .number_p').eq(1).children('.number_title').removeClass('title_color')
        $('.right_main_top .number_p').eq(2).children('.number').addClass('number_color');
        $('.right_main_top .number_p').eq(2).children('.number_title').addClass('title_color');
    })

    $("input:text,textarea").keyup(function () {
        if (!$('.step_three .issue_text').val() || !$('.step_three .issue_text_t').val() || !$('.step_three .mail').val()) {
            $('.step_three .submis').attr("disabled", true);
        } else {
            $('.step_three .submis').attr("disabled", null);
            $('.step_three .submis').css({ 'background': '#B5272D', 'color': '#ffffff' });
        }
    })
    $('.step_three .submis').on('click', function () {
        var issue_text = $('.step_three .issue_text').val();
        var issue_text_t = $('.step_three .issue_text_t').val();
        var mail = $('.step_three .mail').val();
        var product_code = $('.step_three .title_p').children('.product_t').attr('product-code');
        var issue_code = $('.step_three .title_p').children('.product_d').attr('issue-code');

        $.messageLoad();
        common.postGateway('platform/sequence/getsequenceno', {
            "seqNameOrId": "WorkOrderNo"
        }, common.getToken(), function (sequenceNo) {
            apiRoute.runTaskEvent("","insert into prod_workorder_header(T_ID,CLIENT_ID,ORG_ID,ORDERID,PRODCODE,QATYPE,WORKORDER,WORKFLOW,USERID,CREATE_DATE,CREATED_BY,MODIFY_IP) values "
                    + "(@t_id,'','',@orderId,@prodcode,@qatype,@workorder,'0','&User_Tid&',now(),@created_by,@modify_ip);"
                    + "INSERT INTO prod_workorder_detail (t_id,CLIENT_ID,ORG_ID,orderid,workorder,CREATE_DATE,CREATED_BY,MODIFY_IP) VALUES "
                    + "(@t_id, '', '',@orderId, @workorder, now(),@created_by,@modify_ip)", [{
                    t_id: "",
                    orderId: sequenceNo.json,
                    prodcode: product_code,
                    qatype: issue_code,
                    workorder: issue_text
                }, {
                    t_id: "",
                    orderId: sequenceNo.json,
                    workorder: issue_text_t
                }], function (data) {
                $.messageBoxCloseLoading();
                if (data.resMsg == "") {
                    location.href = "/Member/MyWorkOrder";
                }
                else {
                    $.messageBoxAlert(0,data.resMsg);
                }
            });
        });
    })
});
