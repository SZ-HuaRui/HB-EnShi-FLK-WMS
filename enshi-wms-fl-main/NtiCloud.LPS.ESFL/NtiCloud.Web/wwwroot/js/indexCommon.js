$(function () {
    // 显示移动端菜单
    function showMobileHeaderMenu() {
        var $menuItems = $('.menu > .menu-list', '.header').clone(),
            $menuBtn = $('.menu > .right-wrapper > .menu-item', '.header').clone(),
            $switchLang = $('.lang-switch', '.header').clone(),
            $mask = $('<div class="x-window-mask header-menu-mask"/>').click(function (e) {
                var $target = $(e.target);
                if ($target.closest('.lang-switch').length) {
                    showLangSwitchPane();
                    return;
                }
                if ($target.hasClass('x-window-mask')) {
                    $mask.removeClass('dark-in');
                    $menuPane.removeClass('active');
                    setTimeout(function () {
                        $mask.remove();
                    }, 218);
                }
            }).appendTo('body'),
            $menuPane = $('<div class="header-menu-pane"/>')
                .append($('<div class="menu-wrapper"/>').append($menuItems))
                .append($('<div class="menu-btn"/>').append($menuBtn))
                .append($('<div class="lang-wrapper"/>').append($switchLang))
                .appendTo($mask);
        setTimeout(function () {
            $mask.addClass('dark-in');
            $menuPane.addClass('active');
        }, 100);
    }
    // header点击
    $('.header').on('click', function (e) {
        var $target = $(e.target);
        if ($target.closest('.menu-icon').length) {
            showMobileHeaderMenu();
            return;
        }
        var $langSwitch = $target.closest('.lang-switch');
        if ($langSwitch.length) {
            showLangSwitchPane($langSwitch);
        }
    });
    $(document).on('mousedown', function (e) {
        var $langList = $('.lang-list', '.header'),
            $target = $(e.target).closest('.lang-item', $langList);
        if ($target.length === 0) {
            $langList.remove();
        }
    });
    $('body').on('click', '.menu-wrapper .menu_li', function (e) {
        e.stopPropagation();
        $(this).siblings().children('.menu-list-select').fadeOut();
        if ($(this).children('.menu-list-select').css('display') == 'none') {
            $(this).children('.menu-list-select').fadeIn();
        } else {
            $(this).children('.menu-list-select').fadeOut();
        }
    })
    $('body').on('click', '.menu-wrapper .menu-list', function (e) {
        e.stopPropagation();
        if ($(this).children('ul').length > 0) {
            if ($(this).children('ul').css('display') == 'none') {
                $(this).children('ul').show();
            } else {
                $(this).children('ul').hide();
            }            
        }
    })
    if (sessionStorage["encrypt"] == null) {
        common.postGateway("platform/user/gettoken", {}, common.getToken(), function (userinfo) {
            if (userinfo.hasOwnProperty("userLoginInfo") && userinfo.userLoginInfo.hasOwnProperty("userTid") && userinfo.userLoginInfo.userTid!=null && userinfo.userLoginInfo.userTid != "") {
                $(".right-wrapper").html("<a class=\"menu-item signin\">" + userinfo.userLoginInfo.userName + "</a>"
                    + "<a class=\"menu-item register\" href=\"/Member/ControlDesk\">控制台</a>");
            }
            else {
                $(".right-wrapper").html("<a class=\"menu-item signin\" href=\"/Main/Register\">注册</a>"
                    + "<a class=\"menu-item register\" href=\"/Main/Login\">登录</a>");
            }
        })
    }
    else {
        $(".right-wrapper").html("<a class=\"menu-item signin\">" + JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserName + "</a>"
            + "<a class=\"menu-item register\" href=\"/Member/ControlDesk\">控制台</a>");
    }
})