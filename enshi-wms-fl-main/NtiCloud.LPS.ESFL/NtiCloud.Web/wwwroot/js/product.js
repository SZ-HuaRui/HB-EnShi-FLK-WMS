$(function () {    
    $('.product_menu_ul .product_menu_li').on('click', function () {
        var categorycode = $(this).attr('categorycode');
        $(this).addClass('menu_active').siblings().removeClass('menu_active');
        showProd(categorycode);
    })
    $('.product_title_li .product_title_li_d').on('click', function () {
        var appcode = $(this).parent().attr('appcode');
        $(this).addClass('title_active').parent().siblings().children('.product_title_li_d').removeClass('title_active');
        $(this).children('span').css('color', '#ffffff').parent().parent().siblings().children().children('span').css('color', '#666666');
        $.each($('.product_title .product_title_li_d').children('img'), function () {
            $(this).attr('src', $(this).attr('src').replace(/pre/, "nor"));
            $(this).attr('src', $(this).attr('src').replace(/hov/, "nor"));
        })
        var newSrc = $(this).children('img').attr('src');
        $(this).children('img').attr("src", newSrc.replace(/nor/, "pre"));
        showApp(appcode);
    })
    $('.product_menu_ul .product_menu_li').eq(0).trigger("click");
})

function showProd(id) {
    var clickOne = false;
    for (var i = 0; i < $('.product_title_ul .product_title_li').length; i++) {
        if (id == $('.product_title_ul .product_title_li').eq(i).attr('categorycode')) {
            $('.product_title_ul .product_title_li').eq(i).show();
            if (clickOne == false) {
                clickOne = true;
                $('.product_title_ul .product_title_li').eq(i).find('.product_title_li_d').trigger("click");
            }
        } else {
            $('.product_title_ul .product_title_li').eq(i).hide();
        }
    }
}
function showApp(typeid) {
    for (var i = 0; i < $('.container_content_change .div_info_d').length; i++) {
        if (typeid == $('.container_content_change .div_info_d').eq(i).attr('appcode')) {
            $('.container_content_change .div_info_d').eq(i).show();
        } else {
            $('.container_content_change .div_info_d').eq(i).hide();
        }
    }
}