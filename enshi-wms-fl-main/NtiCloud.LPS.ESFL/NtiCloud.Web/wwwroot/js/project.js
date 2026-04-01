var fun = function (doc, win) {
    var docEl = doc.documentElement,
        resizeEvt = 'orientationchange' in window ? 'orientationchange' : 'resize',
        recalc = function () {
            var clientWidth = docEl.clientWidth;
            if (!clientWidth) return;

            //这里是假设在1920px宽度设计稿的情况下，1rem = 100px；
            //可以根据实际需要修改
            docEl.style.fontSize = 100 * (clientWidth / 1920) + 'px';
        };
    if (!doc.addEventListener) return;
    win.addEventListener(resizeEvt, recalc, false);
    doc.addEventListener('DOMContentLoaded', recalc, false);
}
//fun(document, window);
$(function () {
    //var img_direct = 1;
    //$('.price_product .title img').on('click', function () {
    //    if ($(this).attr('src') == '/areas/index/img/product_down.png') {
    //        $('.price_product .title img').attr('src', '/areas/index/img/product_down.png');
    //        $(this).attr('src', '/areas/index/img/product_up.png');
    //    }
    //    else {
    //        $('.price_product .title img').attr('src', '/areas/index/img/product_down.png');
    //        $(this).attr('src', '/areas/index/img/product_down.png');
    //    }
    //})
    //$('.product_title .product_title_li_d').hover(function () {
    //    if (!$(this).hasClass('title_active')) {
    //        var newSrc = $(this).children('img').attr('src');
    //        $(this).children('img').attr("src", newSrc.replace(/nor/, "hov"));
    //        $(this).children('span').css('color', '#C8161D');
    //    }
    //}, function () {
    //    if (!$(this).hasClass('title_active')) {
    //        var newSrc = $(this).children('img').attr('src');
    //        $(this).children('img').attr("src", newSrc.replace(/hov/, "nor"));
    //        $(this).children('span').css('color', '#666666');
    //    }
    //})
    $('.product_title .product_title_li_d').on('click', function () {
        $(this).addClass('title_active').parent().siblings().children('.product_title_li_d').removeClass('title_active');
        $(this).children('span').css('color', '#ffffff').parent().parent().siblings().children().children('span').css('color', '#666666');
        $.each($('.product_title .product_title_li_d').children('img'), function () {
            $(this).attr('src', $(this).attr('src').replace(/pre/, "nor"));
            $(this).attr('src', $(this).attr('src').replace(/hov/, "nor"));
        })
        var newSrc = $(this).children('img').attr('src');
        $(this).children('img').attr("src", newSrc.replace(/nor/, "pre"));

        $('.container-cloud .container_content').eq($(this).parent().index()).show().siblings().hide();
    })
    if ($(window).width() < 1218) {
        $('.trangle').css('left', ($('.container_content').width() - 16) / 2);
    }
    bannerHeight();
})

function bannerHeight() {
    var windowWidth = $(window).width();
    //$(".container-banner").height(windowWidth / 1920 * 270);
    //$(".container-banner .banner-content").css({ "top": windowWidth / 1920 * 140, "right": windowWidth / 1920 * 160 });
    //$(".container-banner .banner-content").css({ "padding-top": windowWidth / 1920 * 55 })
    //$(".container-banner .banner-content .description").css({ "font-size": windowWidth / 1920 * 14 });
    //$(".container-banner .banner-content .banner-title").css({ "font-size": windowWidth / 1920 * 36, "margin-bottom": windowWidth / 1920 * 20 });
    //$(".container-cloud-content").css({ "top": -windowWidth / 1920 * 60 });
}

$(window).resize(function () {
    bannerHeight();
    if ($(window).width() < 1218) {
        $('.trangle').css('left', ($('.container_content').width() - 16) / 2);
    }
});