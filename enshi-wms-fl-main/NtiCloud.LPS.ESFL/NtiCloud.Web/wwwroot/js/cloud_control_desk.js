$(function () {    
    $(".content").css("height", 'auto');
    //$('.container_product').css("height", $(".content").height() - $('.navbar').height());
    //changeDevice();
    //$(window).resize(function () {
    //    $(".content").css("height", 'auto');
    //    changeDevice();
    //})
    //pc端时展开与收起
    $('.btn_pic').click(function () {
        var that = $(this).parent().parent().next();
        if ($(this).hasClass('btn_pic_up')) {
            $(this).removeClass('btn_pic_up').addClass('btn_pic_down');
        } else {
            $(this).removeClass('btn_pic_down').addClass('btn_pic_up');
        }
        if (that.hasClass('product_body_show')) {
            that.removeClass('product_body_show');
        } else {
            that.addClass('product_body_show');
        }        
    })
    //点击切换产品和项目
    $('.product_project li').click(function () {
        var index = $(this).index();
        $(this).addClass('li_active').siblings().removeClass('li_active');
        $('.container_d').eq(index).show().siblings().hide();
    })
    if ($('.con_product .content_c').length == 0) {
        $('.product_project li').eq(0).hide().removeClass('li_active');
        $('.product_project li').eq(1).hide().removeClass('li_active');
        $('.container_d').eq(0).hide();
        $('.container_d').eq(1).show();
    } else {
        $('.product_project li').eq(1).show().removeClass('li_active');
        $('.product_project li').eq(0).show().addClass('li_active');
        $('.container_d').eq(1).hide();
        $('.container_d').eq(0).show();
    }
    //点击创建应用
    $('.addBtn_d').click(function () {
        $('.choose_pro').fadeIn();
    })
    //点击弹窗的关闭
    $('.close_b').click(function () {
        $(this).parent().parent().parent().parent().fadeOut();
    })
    //点击弹窗的关闭
    $('.close_b').click(function () {
        $(this).parent().parent().parent().parent().fadeOut();
    })

    $(".login_out").click(function () {
        LogOut();
    });
    
    //手机端时展开与收起
    $('.head_pc .btn_word').click(function () {
        if ($(this).text() == "展开") {
            $(this).text("收起");
            $(this).parent().parent().next().children('.work_content:gt(1)').fadeIn();
        } else {
            $(this).text("展开");
            $(this).parent().parent().next().children('.work_content:gt(1)').fadeOut();
        }
    })
    //弹窗时二级的展开与收起			
    $('.product_top').on('click', function (e) {
        if (e.target.className.indexOf("choose") == 0) return;
        if ($(this).next().css('display') == 'none') {
            $(this).next().show();
        } else {
            $(this).next().hide();
        }
    })
    //点击下一步按钮
    $('.next_step').on('click', function () {
        $('.choose_pro').fadeOut();
        $('.choose_clo').fadeIn();
    })

    //弹框时判断应用下是否有子集
    //var thatChild = $(".product_body .product_bottom .product_bottom_title");
    var thatChild = $(".choose_pro .product_body");
    for (var i = 0; i < thatChild.length; i++) {
        //alert(thatChild.eq(i).find('.product_bottom_title').length);
        if (thatChild.eq(i).find('.product_bottom_title').length == 0) {
            thatChild.eq(i).find('.choose').removeClass('partChoosed').addClass('unchoosed');
        }
    }
    //构建选择,点击勾选图标
    $('.choose').on('click', function () {
        if ($(this).hasClass('choosed')) {
            $(this).removeClass('choosed').addClass('unchoosed');
            $(this).parent().next().children('.product_bottom_title').children('.level_3').removeClass('choosed').addClass('unchoosed');
        } else {
            $(this).removeClass('unchoosed').addClass('choosed');
            console.log($(this).parent().next().children('.product_bottom_title').children('.level_3'));
            $(this).parent().next().children('.product_bottom_title').children('.level_3').removeClass('unchoosed').addClass('choosed');
        }
    })
    //$('.level_3').on('click', function () {
    //    if ($(this)) {

    //    }
    //    $(this).parent().next().children('.product_bottom_title').children('.level_3').removeClass('unchoosed').addClass('choosed');
    //})    

    

    function changeDevice() {
        var screenWidth = $(window).width();//获取屏幕宽度
        if (screenWidth <= 768) {
            for (var i = 0; i < $(".product_body").length; i++) {
                // alert($(".product_body").eq(i).children('.work_content').length);
                $(".product_body").eq(i).children('.work_content:gt(1)').hide();
                if ($(".product_body").eq(i).hasClass('product_body_show')) {
                    $(".product_body").eq(i).removeClass('product_body_show');
                }
            }
            // $(".product_type .work_content:gt(1)").hide();
        } else {
            $(".product_body").children('.work_content:gt(1)').show();
        }
    }
    //选择图标
    var selectPart = true;
    $(".icon_down").on('click', function (event) {
        if (selectPart == true) {
            selectPart = false;
            $('.icon_down .process_select').show();

        } else {
            selectPart = true;
            $('.icon_down .process_select').hide();
        }
        event.stopPropagation();
    })
    $(".department_d").on('click', '.department_select_li', function () {
        $(this).addClass('active_li').siblings().removeClass('active_li');
        $(".icon_down input").val($(this).html());
        $(".icon_down .process_select").hide();
        //selectPart = true;
    })
    //显示项目阶段
    var selectShow = true;
    $(".appType").on('click', function (event) {
        if (selectShow == true) {
            selectShow = false;
            $('.appType .process_select').show();

        } else {
            selectShow = true;
            $('.appType .process_select').hide();
        }
        event.stopPropagation();
    })
    $(".process_d").on('click', '.process_select_li', function () {
        $(this).addClass('active_li').siblings().removeClass('active_li');
        $(".appType input").val($(this).html());
        $(".appType .process_select").hide();
        //selectShow = true; 
    })
    $(document).on('click', function () {
        //if (selectShow == false) {
        //    selectShow = true;
        $('.appType .process_select').hide();
        $('.icon_down .process_select').hide();
        //}
    })
});
