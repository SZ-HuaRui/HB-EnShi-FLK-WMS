$(function () {
    layui.use(['layer', 'form','carousel'], function () {
        var layer = layui.layer ,
            form = layui.form ,
            carousel = layui.carousel;
        //建造轮播实例
        carousel.render({
            elem: '#carousel_top'
            , width: '100%' //设置容器宽度
            , arrow: 'always' //始终显示箭头
            //,anim: 'updown' //切换动画方式
        });
        carousel.render({
            elem: '#carousel_bottom'
            , width: '100%' //设置容器宽度
            , arrow: 'always' //始终显示箭头
            //,anim: 'updown' //切换动画方式
        });
    });
    var swiper = new Swiper('.banner', {
        loop: true,
        autoplay: true,//可选选项，自动滑动
        pagination: {
            el: '.swiper-pagination',
            clickable: true,
        },
        navigation: {
            nextEl: '.swiper-button-next',
            prevEl: '.swiper-button-prev',
        }
    });
    var swiper = new Swiper('.swiper-service', {
        speed: 1000,
        autoplay: {
            delay: 500
        },
        initialSlide: 0,
        slidesOffsetBefore: 0,
        slidesPerView: 4,
        spaceBetween: 32,
        slidesPerGroup: 1,
        loop: true,
        loopFillGroupWithBlank: false,
        //pagination: {
        //    el: '.swiper-pagination',
        //    clickable: true,
        //},
        navigation: {
            nextEl: '.swiper-button-next',
            prevEl: '.swiper-button-prev',
        },
        breakpoints: {
            //当宽度小于等于320
            576: {
                slidesPerView: 1,
                slidesPerGroup: 1,
                spaceBetween: 10
            },
            //当宽度小于等于320
            768: {
                slidesPerView: 2,
                slidesPerGroup: 1,
                spaceBetween: 10
            },
            //当宽度小于等于480
            992: {
                slidesPerView: 3,
                slidesPerGroup: 1,
                spaceBetween: 20
            },
            //当宽度小于等于640
            1160: {
                slidesPerView: 3,
                slidesPerGroup: 1,
                spaceBetween: 25
            }
        }
    });

    changePic();
    $(window).resize(function () {
        changePic();
    })
    function changePic() {
        var screen_width = $(window).width();
        var count = 0;
        if (screen_width >= 1280) {
            $('.swiper-slide').css('height', '560px');
            $('.banner_top .swiper-slide').each(function (i) {
                //count++;
                var attrCla = $(this).children('a').attr('class');
                if (attrCla == 'img1') {
                    $(this).css('background', '#060e30');
                    $(this).html('').append('<a  href="/Member/ControlDesk" class="img1"></a> ');
                } else if (attrCla == 'img2') {                    
                    $(this).css('background', 'linear-gradient(90deg,#3B1C71 0%,#FDAD7F 100%)');
                    $(this).html('').append('<a  href="/Member/ControlDesk" class="img2"></a> ');
                } else if (attrCla == 'img3') {
                    $(this).css('background', '#111A38');
                    $(this).html('').append('<a  href="/Member/ControlDesk" class="img3"></a> ');
                }
                //if (i % 2 == 0) {
                //    $(this).html('').append('<a  href="/Member/ControlDesk" class="img1"></a> ');
                //} else {
                //    $(this).html('').append('<a  href="/Member/ControlDesk" class="img2"></a> ');
                //}
            })
        } else if (screen_width > 751 && screen_width < 1280) {
            $('.swiper-slide').css('height', '100%');
            $('.banner_top .swiper-slide').each(function (i) {
                //count++;
                var attrCla = $(this).children('a').attr('class');
                if (attrCla == 'img1') {
                    $(this).html('').append('<a href="/Member/ControlDesk"><img src="../img/index_banner1.png"></a>');
                } else if (attrCla == 'img2') {
                    $(this).html('').append('<a href="/Member/ControlDesk"><img src="../img/index_banner2.png"></a>');
                } else if (attrCla == 'img3') {
                    $(this).html('').append('<a href="/Member/ControlDesk"><img src="../img/index_banner3.png"></a>');
                }
                //if (i % 2 == 0) {
                //    $(this).html('').append('<a href="/Member/ControlDesk"><img src="../img/index_banner1.png"></a>');
                //} else {
                //    $(this).html('').append('<a href="/Member/ControlDesk"><img src="../img/index_banner2.png"></a>');
                //}
                $(this).children('a').css('height', 'auto');
            })
        } else if (screen_width <= 751) {
            $('.swiper-slide').css('height', '100%');
            $('.banner_top .swiper-slide').each(function (i) {
                //count++;
                var attrCla = $(this).children('a').attr('class');
                if (attrCla == 'img1') {
                    $(this).html('').append('<a href="/Member/ControlDesk"><img src="../img/phone_banner1.png"></a>');
                } else if (attrCla == 'img2') {
                    $(this).html('').append('<a href="/Member/ControlDesk"><img src="../img/phone_banner2.png"></a>');
                } else if (attrCla == 'img3') {
                    $(this).html('').append('<a href="/Member/ControlDesk"><img src="../img/phone_banner3.png"></a>');
                }
                //if (i % 2 == 0) {
                //    $(this).html('').append('<a href="/Member/ControlDesk"><img src="../img/phone_banner1.png"></a>');
                //} else {
                //    $(this).html('').append('<a href="/Member/ControlDesk"><img src="../img/phone_banner2.png"></a>');
                //}
                $(this).children('a').css('height', 'auto');
            })
        }
    }
})