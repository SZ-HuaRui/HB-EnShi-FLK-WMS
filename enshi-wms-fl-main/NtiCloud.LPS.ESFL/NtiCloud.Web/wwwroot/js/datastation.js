$(function () {    
    var jishu_station = $("#jishu_station").offset().top;
    var yewu_station = $("#yewu_station").offset().top;
    var data_station = $("#data_station").offset().top;
    var ai_station = $("#ai_station").offset().top;
    var iot_station = $("#iot_station").offset().top;
    $(window).scroll(function () {
        var scroH = $(this).scrollTop();
        if ($(window).scrollTop() >= $(".container-banner").height()) {
            $(".container_menu_d").css({
                'position': 'fixed',
                'top': $(".header").height(),
                'left': 0,
                'z-index': 1000,
                'width': '100%'
            })
            //$(".container-cloud_d,.container-service_d,.info_bg").css({
            //    'top': $(".product_title_d").innerHeight()
            //})
        } else {
            $(".container_menu_d").css({
                'position': 'static'
            })
            //$(".container-cloud_d,.container-service_d,.info_bg").css({
            //    'top': 0
            //})
        }
        if (scroH >= iot_station - $(".container_menu_d").innerHeight() - $(".container_menu_d").innerHeight() - $(".header").innerHeight()) {
            set_cur(".iot_station");
        } else if (scroH >= ai_station - $(".container_menu_d").innerHeight() - $(".container_menu_d").innerHeight() - $(".header").innerHeight()) {
            set_cur(".ai_station");
        } else if (scroH >= data_station - $(".container_menu_d").innerHeight() - $(".container_menu_d").innerHeight() - $(".header").innerHeight()) {
            set_cur(".data_station");
        } else if (scroH >= yewu_station - $(".container_menu_d").innerHeight() - $(".container_menu_d").innerHeight() - $(".header").innerHeight()) {
            set_cur(".yewu_station");
        } else if (scroH >= jishu_station - $(".container_menu_d").innerHeight() - $(".container_menu_d").innerHeight() - $(".header").innerHeight()) {
            set_cur(".jishu_station");
        }
    });
    $('.container_menu_d .product_menu_li').on("click", function () {
        var el = $(this).attr('class').split(" ")[1];
        console.log($("#" + el).offset().top)
        $(window).scrollTop($("#" + el).offset().top - $(".container_menu_d").innerHeight() - $(".header").innerHeight()+1)
        //$(this).children().addClass("title_active").parent().siblings().children().removeClass("title_active");
        set_cur('.' + el);
    });
    function set_cur(n) {
        console.log(n)
        if ($(".product_menu_li span").hasClass("menu_active")) {
            $(".product_menu_li span").removeClass("menu_active");
        }
        $(n).children().addClass("menu_active");
    }
    $('#jishu_station .product_title_li').on("click", function () {
        var index = $(this).index();
        $(this).children('span').addClass('title_active').parent().siblings().children('span').removeClass('title_active');
        $('#jishu_station .station_d img').eq(index).show().siblings().hide();
    });
})
