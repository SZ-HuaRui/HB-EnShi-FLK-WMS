
$(function () {
    if ($("#encrypt").val()) {
        sessionStorage["encrypt"] = $("#encrypt").val();
    };
    if ($(window).width() < 992) {
        var userAgent = navigator.userAgent;
        if (userAgent.indexOf("DingTalk") != -1) {//通过钉钉打开
            var titleContent = this.title;
            var urlContent = common.Encrypt(window.location.toString().replace(window.location.protocol + "//" + window.location.hostname,""));
            var app_id = 0, corpid = "ding5033bbcf5247e61b35c2f4657eb6378f";
            if (common.getUrlParam("AppCode") == "SMS") {
                app_id = 216834455;
            }
            else if (common.getUrlParam("AppCode") == "COST") {
                app_id = 264780061;
            }
            else if (common.getUrlParam("AppCode") == "CRM") {
                app_id = 613842423;
            }
            else if (common.getUrlParam("AppCode") == "BIGSCREEN") {
                app_id = 671250504;
            }
            else if (common.getUrlParam("AppCode") == "Work") {
                app_id = 789468342;
            }
            else if (common.getUrlParam("AppCode") == "BI") {
                app_id = 262769154;
            }
            else if (common.getUrlParam("AppCode") == "File") {
                app_id = 834698097;
            }
            else if (common.getUrlParam("AppCode") == "SPM") {
                app_id = 841277114;
            }
            if (common.getUrlParam("corpid") != null) corpid = common.getUrlParam("corpid");
            var dingUrl = "dingtalk://dingtalkclient/action/openapp?corpid=" + corpid + "&container_type=work_platform&app_id=0_" + app_id + "&redirect_type=jump&redirect_url=" + window.location.protocol.replace(":","") + "%3a%2f%2f" + window.location.host + "%2fdingding%2finterface%2f" + urlContent + "%3fcorpid%3d" + corpid;
            dd.biz.navigation.setRight({
                show: true,//控制按钮显示， true 显示， false 隐藏， 默认true
                control: true,//是否控制点击事件，true 控制，false 不控制， 默认false
                text: '分享',//控制显示文本，空字符串表示显示默认文本
                onSuccess: function (result) {                    
                    dd.biz.util.share({
                        type: 1,//分享类型，0:全部组件 默认；1:只能分享到钉钉；2:不能分享，只有刷新按钮
                        url: dingUrl,
                        title: titleContent,
                        content: "欢迎访问" + titleContent,
                        image: "https://cloud.nti56.com/img/wechat_qr.png",
                        onSuccess: function () {
                            //onSuccess将在调起分享组件成功之后回调
                            /**/
                        },
                        onFail: function (err) { }
                    })
                },
                onFail: function (err) { }
            });
        }

        $('#pc_menu').hide();
        $('.tab_content').css('height', $(window).height() - 50);
        //初始化页面元素宽高
        $("#iframeinitTab").css({ "min-height": $(window).height() - 50 });
        $(".phone_area").css("min-height", $(window).height() - 50);
 
        var tempPage = common.getUrlParam('Page');
        if (tempPage) {
            $('#phone_menu').hide();
            $('.tab_content .show_iframe').children('iframe').attr('src', common.Decrypt(tempPage));
            var titleStr = decodeURI(window.location.search).split('&')[2].split('=')[1];
            $('.phone_header_title').html(titleStr);
            if (titleStr == '个人中心') {//个人中心
                $(window).scrollTop(0);
                $('.phone_header .set_btn').removeClass('person_set').addClass('action_set');
                $('.phone_header .phone_header_btn').addClass('block_btn').addClass('toPerson').show();
            } else if (titleStr == '帮助') { //帮助文档
                $('.phone_header .set_btn').hide();
                $('.phone_header .phone_header_btn').addClass('block_btn').addClass('toIframe').show();
            }else {//其他菜单项
                $(window).scrollTop(0);
                $('.phone_header .phone_header_btn').addClass('block_btn').addClass('toIframe').show();
            };
        } else {
            $('.tab_content .show_iframe').removeClass('show_iframe');
        };

    };
    //请求菜单接口数据
    common.postGateway('platform/system/menuconfig',{
        id: "-1"
    }, common.getToken(), function (data) {
            if (data.isSucceed == true && data.errcode == 0) {
                if (data.json == "noLogin") {
                    common.removeCookie("sessionKey");
                    common.setCookie("sessionKey", "noLogin");
                    window.localStorage.removeItem("token");
                    var dbCode = "";
                    if (common.getUrlParam("dbCode") != null) {
                        dbCode = "&dbCode=" + common.getUrlParam("dbCode");
                    }
                    if (common.getUrlParam("ClientId") == null || common.getUrlParam("ClientId") == "") {
                        window.location.href = "/Admin/Login?AppCode=" + common.getUrlParam("AppCode") + dbCode;
                    }
                    else {
                        window.location.href = '/Admin/Login/Index/' + common.getUrlParam("ClientId") + "?AppCode=" + common.getUrlParam("AppCode") + dbCode;
                    }
                    return false;
                }
                else {
                    data = JSON.parse(data.json);
                }
            }
            else {
                return false;
            }

        //移动端向下展开的菜单功能实现
        var winWidth = $(window).width();
        if (winWidth < 992){

            //移动端的事件
            //点击进入个人中心/设置
            $('.phone_header .set_btn').click(function () {
                if (!($('.tab_content .content').hasClass('show_iframe'))){
                    $('.tab_content .content').addClass('show_iframe');
                };
                if ($(this).hasClass('person_set')) {//个人中心
                    var nextUrl = encodeURI(window.location.href.split('&')[0] + '&Page='+common.Encrypt('/Admin/System/Personalinfo')+'&Name=个人中心');
                    window.location.href = nextUrl;
                } else {//设置
                    //console.log(222)
                    $('.phone_area').show().find('.action_set_area').show();
                    $('.phone_header_title').html('设置');
                    $('.phone_header_btn.block_btn').addClass('toSet');
                    $(this).hide();
                };
            });
            //点击进入修改密码
            $('#develop_main').on('click', '.to_modify', function () {
                $('.modify_psword').show().siblings().hide();
                $('.modify_psword input').css('width', $(window).width() - 32 - 80 - 2).val('');
                $('.phone_header_title').html('修改密码');
                $('.set_btn').hide();
                $('.phone_header_btn.block_btn').addClass('toModify');
            });
            //点击进入帮助文档
            $('#develop_main').on('click', '.get_help', function () {
                //$('.modify_psword').show().siblings().hide();
                //$('.modify_psword input').css('width', $(window).width() - 32 - 80 - 2).val('');
                //$('.phone_header_title').html('帮助文档');
                //$('.set_btn').hide();
                //$('.phone_header_btn.block_btn').addClass('toModify');

                var nextUrl = encodeURI(window.location.href.split('&')[0] + '&Page='+common.Encrypt('/Admin/Page/PageHelp')+'&Name=帮助');
                window.location.href = nextUrl;
            });

           
            //var menuType = 'tree';
            var menuType = 'block';
            if (menuType == 'tree') {
                $('.phone_header_btn').show();
                //加载树状菜单组件
                $('#phone_menu').menuDown({
                    data: data,
                    iconStr: 'iconCls',
                    hrefStr: 'urladdress',
                    textStr: 'text',
                    idStr: 'id',
                    childStr: 'children',
                    isnavigateStr: 'isnavigate',
                    onSelectMenu: function (data) {
                        //console.log(data);
                        if (data.url != ''){
                            $('.phone_header_btn').click();
                            $(window).scrollTop(0);
                            $('.tab_content .content').addClass('show_iframe');
                            $('.tab_content .show_iframe').children('iframe').attr('src', data.url);
                            //console.log(data)
                            //var nextUrl = encodeURI(window.location.href + '&Page=' + data.url+'&Name=aa');
                            //window.location.href = nextUrl;

                        };
                        return false;
                    }
                });
                //修改滚动条样式
                //$('#phone_menu').perfectScrollbar();
                //点击显示、隐藏菜单栏    
                $('.phone_header_btn').click(function () {
                    if ($(this).hasClass('close')) {
                        $(this).removeClass('close');
                        $('#phone_menu').fadeOut(350);
                    } else {
                        $(this).addClass('close');
                        $('#phone_menu').fadeIn(350);
                    };
                });
            } else {
                if (!tempPage) {
                    //加载块状菜单
                    $('#phone_menu').css({ 'backgroundColor': '#fff', 'min-height': $(window).height() - 50 }).show();
                    if (userAgent.indexOf("DingTalk") != -1) { //钉钉环境才有退出按钮
                        $('.phone_header .phone_header_btn').addClass('close').show();
                    } else {
                        $('.phone_header .phone_header_btn').addClass('block_btn');
                    }

                    var blockMaxNum = parseInt(($(window).width() - 20) / (60 + 24));
                    eachLoadMenu(data, blockMaxNum, 'first');

                    //点击展开或者隐藏内容块
                    $('#phone_menu').on('click', '.span_btn', function () {
                        if ($(this).hasClass('click_show')) {//展开
                            $(this).parent().siblings('.block_area').find('.over').css('display', 'inline-block');
                            $(this).fadeOut().siblings('.click_hide').fadeIn();
                        } else {//收起
                            $(this).parent().siblings('.block_area').find('.over').css('display', 'none');
                            $(this).fadeOut().siblings('.click_show').fadeIn();
                        };
                        return false;
                    });
                    //点击菜单项
                    $('#phone_menu').on('click', '.block ', function () {
                        if (userAgent.indexOf("DingTalk") != -1) { //钉钉环境才有退出按钮
                            $('.phone_header_btn.close').addClass('block_btn').removeClass('close').show();
                        } else {
                            $('.phone_header_btn.block_btn').show();
                        }
                        if (!$(this).hasClass('hasChild')) {//菜单跳转
                            var nextUrl = encodeURI(window.location.href.split('&')[0] + '&Page=' +common.Encrypt($(this).attr('data-url')) + '&Name=' + $(this).find('span').html());
                            window.location.href = nextUrl +"&dd_orientation=auto&dd_full_screen=true";
                        } else {//加载下级菜单
                            var titleName = $(this).parent().parent().find('.title_name').html() + '/' + $(this).find('span').html();
                            $('.phone_header_title').attr('data-firstTab', titleName).html(titleName);
                            var index_1 = $(this).parent().parent().attr('data-index');
                            var index_2 = $(this).attr('data-index');
                            $('#phone_menu .first_menu').hide();
                            $('.phone_header_btn.block_btn').addClass('toSubMenu');
                            $(window).scrollTop(0);
                            eachLoadMenu(data[index_1].children[index_2].children, blockMaxNum, 'second');
                        };
                    });

                };
                //点击首页的退出按钮
                $('.phone_header').on('click', '.phone_header_btn.close', function () {
                    if (userAgent.indexOf("DingTalk") != -1) { //钉钉环境才有退出按钮
                        dd.biz.navigation.close({
                            onSuccess: function (result) {
                                /*result结构
                                {}
                                */
                            },
                            onFail: function (err) { }
                        })
                    }
                });


                //点击返回按钮返回上一个页面
                $('.phone_header').on('click', '.block_btn', function () {
                    //从修改密码返回设置
                    if ($(this).hasClass('toModify')) {
                        $(this).removeClass('toModify');
                        $('.phone_header_title').html('设置');
                        $('.modify_psword').hide().siblings('.action_set_area').show();

                        //从设置返回个人中心
                    } else if ($(this).hasClass('toSet')) {
                        $(this).removeClass('toSet');
                        $('.phone_header .set_btn').addClass('action_set').show();
                        $('.phone_header_title').html('个人中心');
                        $('.phone_area').hide().find('.action_set_area').hide();
                        //从个人中心返回
                    } else if ($(this).hasClass('toPerson')) {
                        window.history.back();
                        //从第二个菜单页进入的iframe页面返回
                    } else if ($(this).hasClass('toSubMenu') && $(this).hasClass('toIframe')) {
                        $(this).removeClass('toIframe');
                        $('#phone_menu').fadeIn();
                        $('.phone_header_title').html($('.phone_header_title').attr('data-firstTab'));
                        //从第二个菜单页返回
                    } else if ($(this).hasClass('toSubMenu')) {
                        $(this).removeClass('toSubMenu');
                        if (userAgent.indexOf("DingTalk") != -1) { //钉钉环境才有退出按钮
                            $('.phone_header_btn.block_btn').addClass('close').removeClass('block_btn')
                        } else {
                            $('.phone_header_btn.block_btn').hide();
                        }
                        $('#phone_menu .first_menu').show();
                        $('#phone_menu .second_menu').remove();
                        $('.phone_header_title').html('今天云');
                        //从第一个菜单页进入的iframe页面返回
                    } else if ($(this).hasClass('toIframe')) {
                        $(this).removeClass('toIframe');
                        window.history.back();
                    };

                    return false;
                });


            };

        } else {//PC端事件
            //设置页码初始化样式
            setStyle();
            //加载PC端菜单项和对应的事件
            loadPcMenu(data);
            //点击收缩左侧菜单
            hideMenu();

            //点击刷新当前iframe标签页面
            $('.settings_refresh').click(function () {
                var iframeID = $('.tab_content .show_iframe').children('iframe').attr('id');
                window.frames[iframeID].location.reload();
                $.messageMsg('页面刷新中……', { time: 900, icon: 6, shade: [0.1, '#fff'] });


            });
            //点击设置图标
            var isSetting = true;
            $('.settings_group').on('click', function () {
                if (isSetting == true) {
                    isSetting = false;
                    $('.settings ul').show();
                } else {
                    isSetting = true;
                    $('.settings ul').hide();
                }
            });
            $(document).on('click', function () {
                if (isSetting == false) {
                    isSetting = true;
                    $('.settings ul').hide();
                }
            });
            $(".settings_group").on('click', function () {
                return false;
            });
            //点击个人信息 显示对应的tab项
            $("#Personal").on('click', function () {
                var personal = {
                    tid: "-1",
                    dataIndex: "-1",
                    icon: "",
                    menu: "个人信息",
                    isnavigate: "0",
                    url: "/Admin/System/Personalinfo",
                    dataArr: [],
                    indexArr: []
                };
                addTab(personal);
            });
            //判断登录时候登录密码是否达到三级等保要求
            if (window.localStorage.getItem('intensityReg') == 'false') {                
                $.messageBoxOpenBtn('修改密码', ['350px', '250px'], '#modalpassword', [
                    { text: '修改密码', handler: function (index) { common.modifyPsword([$('#Password1'), $('#Password2'), $('#Password3')]); } }
                ],0);
                $.messageBoxAlert(0, "登录密码不符合三级等保要求，请修改密码！<br>1、密码长度大于等于8个字符；<br>2、必须是大写字母，小写字母，数字，特殊字符(_!@#$%^&*`~()-+=/)中任意三个组合!");
            };

            //PC端显示修改密码弹窗
            $("#UserPswd").click(function () {
                $.messageBoxOpenBtn('修改密码', ['350px', '250px'], '#modalpassword', [
                    //{ text: '修改密码', handler: function (index) { modifyPsword([$('#old_psword'), $('#new_psword'), $('#confirm_psword')]); }},
                    { text: '修改密码', handler: function (index) { common.modifyPsword([$('#Password1'), $('#Password2'), $('#Password3')]); } },
                ]);
            });
        };

    });
    //切换主题
    layui.use('layer', function () {
        $("#LayUiSetThemes").on("click", function () {
            var nticloudCSS = common.getCookie("NticloudCSS");
            if (nticloudCSS == null || nticloudCSS == "" || nticloudCSS.indexOf(":") == -1) {
                common.setCookie("NticloudCSS", "1:bluelight");
            }
            var nticloudFontSize = nticloudCSS.split(':')[0];
            var nticloudStyle = nticloudCSS.split(':')[1];
            $("input:radio[name='themes'][value='" + nticloudStyle + "']").prop("checked", true);
            $("input:radio[name='fontsize'][value='" + nticloudFontSize + "']").prop("checked", true);
            $.messageBoxOpenBtn('个性化设置', ['500px', '360px'], '#ModalStyle', [
                {
                    text: '保 存', handler: function (index) {
                        var theme = $('input[name="themes"]:checked').val();
                        var fontsize = $('input[name="fontsize"]:checked').val();
                        common.setCookie("NticloudCSS", fontsize + ":" + theme);
                        location.reload();
                    }
                },
                { text: '取 消', handler: function (index) { } },
            ]);
        });
    });
});

//退出登录
function loginOut() {
    $.messageBoxConfirm('确认要退出系统吗？', ['确定', '取消'], function () {
        common.postGateway('platform/login/getlogout', { "sessionKey": common.getCookie("sessionKey") }, common.getToken(), function () {
            common.removeNoLogin();
            var dbCode = "";
            if (common.getUrlParam("dbCode") != null) {
                dbCode = "&dbCode=" + common.getUrlParam("dbCode");
            }
            if (common.getCookie("warehouseId") != null && common.getCookie("warehouseId") != "") {
                if (common.getUrlParam("ClientId") == null || common.getUrlParam("ClientId") == "") {
                    window.location.href = "/Admin/Login/WMS?AppCode=" + common.getUrlParam("AppCode") + dbCode;
                }
                else {
                    window.location.href = '/Admin/Login/WMS/' + common.getUrlParam("ClientId") + "?AppCode=" + common.getUrlParam("AppCode") + dbCode;
                }
            }
            else {
                if (common.getUrlParam("ClientId") == null || common.getUrlParam("ClientId") == "") {
                    window.location.href = "/Admin/Login?AppCode=" + common.getUrlParam("AppCode") + dbCode;
                }
                else {
                    window.location.href = '/Admin/Login/Index/' + common.getUrlParam("ClientId") + "?AppCode=" + common.getUrlParam("AppCode") + dbCode;
                }
            }
        });
    });
};

//遍历加载移动端菜单项
function eachLoadMenu(data, blockMaxNum, type) {
    //var colorArr = ['#3EB9E8', '#F4A953', '#66A2E3', '#E3856C', '#81BE69', '#6A677D', '#D75D5B', '#EFB250', '#53BEB7', '#7678D5'];
    var colorArr = ['#3396FB', 'linear-gradient(180deg,rgba(98,199,254,1) 0%,rgba(51,150,251,1) 100%)', 'linear-gradient(180deg,rgba(250,201,0,1) 0%,rgba(243,152,0,1) 100%)', 'linear-gradient(180deg,rgba(9,212,250,1) 0%,rgba(4,169,244,1) 100%)', '#FF943C', '#06C1B3', 'linear-gradient(180deg,rgba(0,160,231,1) 0%,rgba(0,102,203,1) 100%)','linear-gradient(180deg,rgba(98,224,147,1) 0%,rgba(51,190,90,1) 100%)'];
    //console.log(data);
    var menuIndex = 0;
    $.each(data, function (i) {
        var divDom = '<div class="phone_block ' + type + '_menu" data-index="' + i + '">' +
            '<p class="title"><span class="title_name">' + this.text + '</span>' +
            '<span class="span_btn click_show">展开</span><span class="span_btn click_hide">收起</span></p>'+
            '<div class="block_area"></div></div >';
        $('#phone_menu').append(divDom);
        if (data[i].children.length == 0) {
            if (this.iconImg == '' || this.iconImg == null) {
                $('#phone_menu .block_area').eq(-1).append('<div class="block isParent" data-url="' + this.urladdress + '"><p style="background:' + this.iconColor + ';"><i class="' + this.iconCls + '"></i></p><span>' + this.text + '</span></div>')
            } else {
                $('#phone_menu .block_area').eq(-1).append('<div class="block isParent" data-url="' + this.urladdress + '"><p style="background:' + this.iconColor + ';"><span class= "img_span" style = "background:url(../../../img/icon/' + this.iconImg + ') no-repeat center;background-size:cover;" ></span ></p><span>' + this.text + '</span></div>')
            };
        } else {
            if (data[i].children.length > blockMaxNum){
                $('#phone_menu .phone_block').eq(-1).find('.click_hide').show();
            };
            $.each(data[i].children, function (j) {
                var className_1 = 'over', className_2 = '';
                if (j < blockMaxNum) { className_1 = ''; };
                if (data[i].children[j].children.length != 0) { className_2 = 'hasChild'; }
                if (this.iconImg == '' || this.iconImg == null) {
                    $('#phone_menu .block_area').eq(-1).append('<div class="block ' + className_2 + ' ' + className_1 + '" data-index="' + j + '" data-url="' + this.urladdress + '"><p style="background:' + this.iconColor + ';"><i class="' + this.iconCls + '"></i></p><span>' + this.text + '</span></div>')
                } else {
                    $('#phone_menu .block_area').eq(-1).append('<div class="block ' + className_2 + ' ' + className_1 + '" data-index="' + j + '" data-url="' + this.urladdress + '"><p style="background:' + this.iconColor + ';"><span class= "img_span" style = "background:url(../../../img/icon/' + this.iconImg + ') no-repeat center;background-size:cover;" ></span></p><span>' + this.text + '</span></div>')
                };
            });
        };
        menuIndex += data[i].children.length;
    });
};


//设置页码初始化样式
function setStyle() {
    $(".mainBody").css("height", $(window).height());
    //$('body').css('min-width', $('.left_bar').width() + $('.t_con_detail').width() + 'px');
    $('body').css('min-width', $(window).width() + 'px');
    $('.first_tab').css({ "max-height": $(window).height() - 70, "overflow": "hidden" });
    $('.t_con_detail').css({
        'height': $(window).height() + 'px',
        'width': $('.mainBody').width() - $('.left_bar').width() + 'px'
    }).show();
    $('.tab_content').css('height', $('.t_con_detail').height() - $('.t_con_detail .tabs').outerHeight() + 'px');
    //头部元素
    var settings_width = $('.settings').width() + 1;
    $('.tab_pages').css('width', $('.tabs').width() - settings_width + 'px');
    $('.tabs_main').css('width', $('.tab_pages').width()-32 + 'px');
    $(window).resize(function () {
        $(".mainBody").css("height", $(window).height());
        $('.first_tab').css({ "max-height": $(window).height() - 70, "overflow": "auto" });
        var cm_width = $('.left_bar').width();
        $('.t_con_detail').css({
            'height': $(window).height() + 'px',
            'width': $('.mainBody').width() - cm_width + 'px'
        }).show();
        if ($('.skullselect').css('display') == 'block') {
            $('.tab_pages').css('width', $('.tabs').width() - $('.skullselect').width() - 15 - settings_width + 'px');
            $('.tabs_main').css('width', $('.tabs').width() - $('.btn_arrow').outerWidth() * 2 - $('.skullselect').width() - 15 - settings_width-32 + 'px');
            $('.tabs_container').css('left', '0px');
        } else {
            $('.tab_pages').css('width', $('.tabs').width() - settings_width + 'px');
            $('.tabs_main').css('width', $('.tab_pages').width()-32 + 'px');
        }
        $('.tab_content').css('height', $('.t_con_detail').height() - $('.t_con_detail .tabs').outerHeight() + 'px');
        //禁用默认滚动条的样式
        $('.first_tab').css('overflow', 'hidden');
    });


    //修改滚动条的样式
    $('.first_tab').perfectScrollbar();
    $('#ascrail2000').css('z-index', '1');
    //监听滚动条滚动事件 同时防抖动
    $('.first_tab').scroll(function () {
        var timer;
        return function () {
            clearTimeout(timer);
            $('#ascrail2000').css('z-index', '100003');
            timer = setTimeout(function () {
                $('#ascrail2000').css('z-index', '1');
            }, 150);
        };
    }());


};
//加载PC端菜单项和对应的事件
function loadPcMenu(data) {
    //初始化页面数据
    var isFirst = true;
    $.each(data, function (index_1) {
        //加载第一级菜单
        $('.first_tab').append('<li class="con_tab" data-tid=' + this.id + ' data-dataIndex=' + index_1 + ' data-isnavigate=' + this.isnavigate + ' data-url="' + this.urladdress + '" data-menu=' + this.text + ' data-icon="' + this.iconCls + '" ><span class="iconSmall ' + this.iconCls + '"></span><span class="first_con_tab" title=' + this.text + '>' + this.text + '</span><em></em><i class="mdi mdi-chevron-right"></i></li>');
        if (this.children && (this.children.length != 0)) {
            $('.first_tab li.con_tab').eq(index_1).css('cursor', 'pointer').find('i').show();
        } else {
            $('.first_tab li.con_tab').eq(index_1).css('cursor', 'default').find('i').hide();
        };

        if (isFirst) {
            isFirst = false;
            //一级菜单的li的事件
            var time = null;
            $("ul.first_tab").on('mouseenter', function () {
                $('.second_tab').hide();
                $('.third_tab').hide();
            }).on('mouseenter', 'li.con_tab', function () {
                $(this).addClass('first_tab_active').siblings().removeClass('first_tab_active');
                if ($(this).attr('data-dataIndex') == $('.second_tab .level03').attr('data-dataIndex')) { $('.second_tab').show(); return false; }
                value = $(this).attr('data-menu');
                $('.second_tab .level03').html('');
                $('.third_tab').hide();
                $('.second_tab p').html(value);
                var self = $(this);
                $('.second_tab .level03').attr('data-dataIndex', self.attr('data-dataIndex'));
                var data_2 = data[self.attr('data-dataIndex')]['children'];
                var isChildBool = (data_2 && data_2.length != 0);
                if (isChildBool) {
                    //加载二级菜单li
                    $.each(data_2, function (index_2) {
                        $('.second_tab .level03').append('<li class="second_con_tab" data-tid=' + this.id + ' data-dataIndex=' + index_2 + ' data-isnavigate=' + this.isnavigate + ' data-url="' + this.urladdress + '" data-menu=' + this.text + ' data-icon="" ><span class=""></span><span class="first_con_tab" title="' + this.text + '">' + this.text + '</span><em></em><i class="mdi mdi-chevron-right"></i></li>');
                        if (this.children && (this.children.length != 0)) {
                            $('.level03 li.second_con_tab').eq(index_2).css('cursor', 'pointer').find('i').show();
                        } else {
                            $('.level03 li.second_con_tab').eq(index_2).css('cursor', 'default').find('i').hide();
                        };
                    });
                    clearTimeout(time);
                    $('.second_tab').show();
                } else {
                    self.find('i').hide();
                    clearTimeout(time);
                    $('.second_tab').hide();
                };
                return false
            }).on('mouseleave', function () {
                time = setTimeout(function () {
                    $(".second_tab").hide()
                    if ($('.second_tab').css('display') === 'none') {
                        $("li.con_tab.first_tab_active").removeClass('first_tab_active')
                    }
                }, 50);
            });
            $('.left_menu .second_tab').css('left', $('.first_tab').width() - 1);
            $('.left_menu .third_tab').css('left', $('.first_tab').width() + $('.third_tab').width());
            //二级菜单li的事件
            $(".left_main_tab").on('mouseenter', 'li.second_con_tab', function () {
                $(this).addClass('other_active').siblings().removeClass('other_active');
                var valueN = $(this).attr('data-menu');
                $('.third_tab .leve203').html('');
                $('.third_tab p').html(valueN);
                var self = $(this);
                var data_3 = data[$('.second_tab .level03').attr('data-dataIndex')]['children'][self.attr('data-dataIndex')]['children'];
                var isChildBool = (data_3 && data_3.length != 0);
                //加载第三级
                if (isChildBool) {
                    $('.third_tab .leve203').attr('data-dataIndex', self.attr('data-dataIndex'));
                    $.each(data_3, function (index_3) {
                        $('.third_tab .leve203').append('<li class="third_con_tab" data-tid=' + this.id + ' data-dataIndex=' + index_3 + ' data-isnavigate=' + this.isnavigate + ' data-url="' + this.urladdress + '" data-menu=' + this.text + ' data-icon="" ><span class=""></span><span class="first_con_tab" title="' + this.text + '">' + this.text + '</span><em></em><i class="mdi mdi-chevron-right"></i></li>');
                        var jqObj = $('.leve203 li.third_con_tab').eq(index_3);
                        if (this.children && (this.children.length != 0)) {
                            jqObj.append('<ul></ul>');
                            //加载第四级
                            var data_4 = this.children;
                            $.each(data_4, function (index_4) {
                                jqObj.find('ul').append('<li class="fourth_con_tab" data-tid=' + this.id + ' data-dataIndex=' + index_4 + ' data-isnavigate=' + this.isnavigate + ' data-url="' + this.urladdress + '" data-menu=' + this.text + ' data-icon="" ><span class=""></span><span class="first_con_tab" title="' + this.text + '">' + this.text + '</span><em></em><i class="mdi mdi-chevron-right"></i></li>');
                            });
                            jqObj.css('cursor', 'pointer').find('i').attr('class', 'mdi mdi-chevron-down').show();
                            jqObj.find('ul li').find('i').hide();
                            jqObj.find('ul').hide();
                        } else {
                            jqObj.css('cursor', 'default').find('i').hide();
                        };
                    });
                    clearTimeout(time);
                    $('.third_tab').show();
                } else {
                    self.find('i').hide();
                    self.css('cursor', 'default');
                    $('.third_tab .leve203').attr('data-dataIndex', self.attr('data-tid'));
                    clearTimeout(time);
                    $('.third_tab').hide();
                };
            }).mouseleave(function () {
                time = setTimeout(function () { $(".third_tab").hide() }, 50);
            });

            $(".left_main_tab").on('mouseenter', '.second_tab', function () {
                clearTimeout(time); $(".second_tab").show();
            }).mouseleave(function () {
                time = setTimeout(function () { $(".second_tab").hide() }, 50);
            });

            $(".left_main_tab").on('mouseenter', '.third_tab', function () {
                clearTimeout(time); $(".third_tab").show();
            }).mouseleave(function () {
                time = setTimeout(function () { $(".third_tab").hide() }, 50);
            });
            $(".left_main_tab").on('mouseleave', '.second_tab,.third_tab', function () {
                time = setTimeout(function () {
                    if ($('.second_tab').css('display') === 'none') {
                        $("li.con_tab.first_tab_active").removeClass('first_tab_active')
                    } }, 150);
                
            })
        };
    });

    //点击添加选项卡
    var is_ulShow = 1;
    $('div.left_main_tab').on('click', '.first_tab li,.second_tab li,.third_tab li', function (e) {
        e.stopPropagation();
        var self = $(this);
        if (self.attr('data-url') != '#' && self.attr('data-url') != '') {
            //console.log(111, self.attr('data-url'))
            var dataArr = [], indexArr = [];
            if (self.hasClass('fourth_con_tab')) {//第四级菜单
                var index_1 = $('ul.level03').attr('data-dataIndex'),
                    index_2 = $('ul.leve203').attr('data-dataIndex'),
                    index_3 = self.attr('data-dataIndex');
                indexArr = [index_3, index_2, index_1];
                dataArr = [data[index_1]['children'][index_2]['children'][index_3], data[index_1]['children'][index_2], data[index_1]];

            } else if (self.hasClass('third_con_tab')) {//第三级
                var index_1 = $('ul.level03').attr('data-dataIndex'),
                    index_2 = $('ul.leve203').attr('data-dataIndex');
                indexArr = [index_2, index_1];
                dataArr = [data[index_1]['children'][index_2], data[index_1]];
            } else if (self.hasClass('second_con_tab')) {//第二级
                var index_1 = $('ul.level03').attr('data-dataIndex'),
                    indexArr = [index_1];
                dataArr = [data[index_1]];
            } else if (self.hasClass('con_tab')) {//第一级
            };
            var tabObj = {
                tid: self.attr('data-tid'),
                dataIndex: self.attr('data-dataIndex'),
                icon: self.attr('data-icon'),
                menu: self.attr('data-menu'),
                isnavigate: self.attr('data-isnavigate'),
                url: self.attr('data-url'),
                dataArr: dataArr,
                indexArr: indexArr
            };
            $('.second_tab,.third_tab').trigger('mouseleave');
            addTab(tabObj);
        } else {
            if (self.attr("class") == "third_con_tab") {
                if (is_ulShow == 1) {
                    is_ulShow = 0;
                    self.find('i').removeClass('mdi mdi-chevron-down').addClass('mdi mdi-chevron-up');
                    self.find('ul').show();
                } else {
                    is_ulShow = 1;
                    self.find('i').removeClass('mdi mdi-chevron-up').addClass('mdi mdi-chevron-down');
                    self.find('ul').hide();
                };
            };
        };
        $(this).trigger('mouseenter');
    });

    $('.tab_content').on('mouseenter', '.location_content li', function () {
        if ($(this).attr('data-url') != '#' && $(this).attr('data-url') != '') {
            $(this).css('cursor', 'pointer');
        }
    }).on('click', '.location_content li', function () {
        var self = $(this);
        if (self.attr('data-url') != '#' && self.attr('data-url') != '') {
            var dataArr = [], indexArr = [];
            if (self.hasClass('fourth_con_tab')) {//第四级菜单
                var index_1 = $('ul.level03').attr('data-dataIndex'),
                    index_2 = $('ul.leve203').attr('data-dataIndex'),
                    index_3 = self.attr('data-dataIndex');
                indexArr = [index_3, index_2, index_1];
                dataArr = [data[index_1]['children'][index_2]['children'][index_3], data[index_1]['children'][index_2], data[index_1]];

            } else if (self.hasClass('third_con_tab')) {//第三级
                var index_1 = $('ul.level03').attr('data-dataIndex'),
                    index_2 = $('ul.leve203').attr('data-dataIndex');
                indexArr = [index_2, index_1];
                dataArr = [data[index_1]['children'][index_2], data[index_1]];
            } else if (self.hasClass('second_con_tab')) {//第二级
                var index_1 = $('ul.level03').attr('data-dataIndex'),
                    indexArr = [index_1];
                dataArr = [data[index_1]];
            } else if (self.hasClass('con_tab')) {//第一级
            };
            var tabObj = {
                tid: self.attr('data-tid'),
                dataIndex: self.attr('data-dataIndex'),
                icon: self.attr('data-icon'),
                menu: self.attr('data-menu'),
                isnavigate: self.attr('data-isnavigate'),
                url: self.attr('data-url'),
                dataArr: dataArr,
                indexArr: indexArr
            };
            addTab(tabObj);
        };
    });

    //选中选项卡
    $('.tabs_main').on('click', 'ul.tabs_main_ul li', function () {
        $(this).addClass('active').siblings('li').removeClass('active');
        showTab($(this));
        showIframe($(this));
        $('.main_tab').trigger('mouseleave');
    });
    //单击删除选项卡
    $('ul.tabs_main_ul').on('click', 'div.xbtn', function () {
        if ($(this).parent().is('.active')) {
            if ($(this).parent().is('ul.tabs_main_ul li:last-child')) {
                $('ul.tabs_main_ul').children('li').eq(-2).addClass('active');
                showIframe($('ul.tabs_main_ul').children('li').eq(-2));
            } else {
                $(this).parent().next().addClass('active');
                showIframe($(this).parent().next());
            };
        };
        var $self = $(this).parent();
        $('.tab_content .content').each(function () {
            if ($self.attr('data-tid') == $(this).attr('data-tid')) {
                $(this).remove();
                return false;
            };
        });
        $('li.show_tab').each(function () {
            if ($(this).attr('data-tid') == $self.attr('data-tid')) {
                $(this).remove();
                return false;
            };
        });
        $self.remove();
        var settings_width = $('.settings').width() + 1;
        if ($('.tabs_main_ul').width() >= $('.tabs_main').width()) {
            $('.tabs_container').css('left', -$('.tabs_main_ul').width() + $('.tabs_main').width() + 'px');
        } else {
            $('.btn_arrow').hide();
            $('.skullselect').hide();
            $('.tab_pages').css('width', $('.tabs').width() - settings_width + 'px');
            $('.tabs_main').css('width', $('.tabs').width() - settings_width-32 + 'px');
            $('.tabs_container').css('left', '0px');
        };
    });
    //双击删除选项卡
    $('.tabs_main').on('dblclick', 'ul.tabs_main_ul li', function () {
        if (!($(this).is('ul.tabs_main_ul li:first-child'))) {
            $(this).children('div.xbtn').trigger('click');
        };
    });

    //左右箭头查询
    $('.tabs').on('mousedown', '.tab_pages .left,.tab_pages .right', function (e) {
        var container_add_dis = 0;
        var downTime = 0;
        var conDisLeft = -$('.tabs_container').position().left;
        $('.tabs_container').css('transition', '');
        var self = e.target;
        self.timer = setInterval(function () {
            downTime++;
            container_add_dis = downTime * 20;
            if ($(self).is('.tab_pages .left')) {
                if (-conDisLeft + container_add_dis < 0) {
                    $('.tabs_container').css('left', -conDisLeft + container_add_dis + 'px');

                } else {
                    $('.tabs_container').css('left', '0px');
                    $('.tabs .left').addClass('left_grey');
                    $('.tabs .left').css('color', '#a7a6aa');
                };
                $('.tabs .right').removeClass('right_grey');
                $('.tabs .right').css('color', '#ffffff');
            } else {
                if (-conDisLeft - container_add_dis > -$('.tabs_main_ul').width() + $('.tabs_main').width()) {
                    $('.tabs_container').css('left', -conDisLeft - container_add_dis + 'px');
                } else {
                    $('.tabs_container').css('left', -$('.tabs_main_ul').width() + $('.tabs_main').width() + 'px');
                    $('.tabs .right').addClass('right_grey');
                    $('.tabs .right').css('color', '#a7a6aa');
                };
                $('.tabs .left').removeClass('left_grey');
                $('.tabs .left').css('color', '#ffffff');
            };
        }, 10);
    });
    $('body').on('mouseup', '.tab_pages .left,.tab_pages .right', function (e) {
        var self = e.target;
        clearInterval(self.timer);
        $('.tabs_container').css('transition', 'all 0.3s linear');
    });
    //所有tab整体查询
    //点击显示ul
    $('.skullselect').click(function () {
        if ($(this).children('.arrow_box').css('display') == 'none') {
            $(this).children().show();
        } else {
            $(this).children().hide();
        };
    });
    //点击删除所有tab
    $('.skullselect').on('click', 'ul.arrow_box li', function () {
        if ($(this).attr('class') == 'close_tabs') {
            $('div.xbtn').trigger('click');

        } else {
            var $self = $(this);
            $('ul.tabs_main_ul li').each(function () {
                if ($(this).attr('data-tid') == $self.attr('data-tid')) {
                    $(this).trigger('click');
                };
            });
        };
        $(this).parent().hide();
        return false;
    });



};
//添加选项卡的方法
function addTab(tabObj) {
    var isLiBool = true;
    $('.tabs_main li').each(function () {
        if ($(this).attr('data-tid') == tabObj.tid) {
            isLiBool = false;
            $(this).addClass('active').siblings('li').removeClass('active');
            showTab($(this));
            showIframe($(this));
            $.messageMsg('该页面已打开!', { time: 900, icon: 1, shade: [0.1, '#fff'] });
            return false;
        };
    });
    var settings_width = $('.settings').width() + 1;
    if (isLiBool) {
        var ifrHeight = $(window).height() - 40 - $('.t_con_detail .tabs').outerHeight();
        if (tabObj.isnavigate == "0") ifrHeight = $(window).height() - $('.t_con_detail .tabs').outerHeight();
        $('.tabs_main_ul').append('<li data-tid=' + tabObj.tid + ' data-thirdDataIndex=' + $('.third_tab>ul').eq(0).attr('data-dataIndex') + '><div class="tab_left"></div><div class="tab_right"></div><div class="tab_img" style="background-image: url(' + tabObj.icon + ')"></div><div class="tab_menu">' + tabObj.menu + '</div><div class="xbtn mdi mdi-close"></div></li>');
        $('.arrow_box').append('<li data-tid=' + tabObj.tid + ' class="show_tab"><span>' + tabObj.menu + '</span><em>&gt;</em></li>');
        $('.tab_content').append('<div class="content" data-tid=' + tabObj.tid + '>' + locationUrl(tabObj) + '<iframe name="iframe' + tabObj.tid + '" id="iframe' + tabObj.tid + '" style="height:' + ifrHeight + 'px" src="' + tabObj.url + '" frameborder="0" scrolling="auto"></iframe></div>');
        $('.tab_content .content').eq(-1).addClass('show_iframe').siblings('div').removeClass('show_iframe');
        if ($('.tabs_main_ul').width() > $('.tabs_main').width() - 20) {
            $('.btn_arrow').show();
            $('.skullselect').show();
            $('.tab_pages').css('width', $('.tabs').width() - $('.skullselect').width() - 15 - settings_width + 'px');
            $('.tabs_main').css('width', $('.tabs').width() - $('.btn_arrow').outerWidth() * 2 - $('.skullselect').width() - 15 - settings_width-32 + 'px');
            $('.tabs_container').css('left', -$('.tabs_main_ul').width() + $('.tabs_main').width() + 'px');
            $('.tabs .left').removeClass('left_grey');
            $('.tabs .left').css('color', '#ffffff');
            $('.tabs .right').addClass('right_grey');
            $('.tabs .right').css('color', '#a7a6aa');
        };
        $('ul.tabs_main_ul li').eq(-1).addClass('active').siblings('li').removeClass('active');
        //$('ul.tabs_main_ul li').eq(-1).siblings('li').children('.xbtn').hide();
        $('ul.tabs_main_ul li').children('.xbtn').show();
    };
};
//导航路径
function locationUrl(tabObj) {
    if (tabObj.isnavigate == "0") return "";
    var titleData = [{ 'text': tabObj.menu, 'url': tabObj.url, 'tid': tabObj.tid, 'isnavigate': tabObj.isnavigate, 'dataIndex': tabObj.dataIndex }];
    var thisDataIndex = tabObj.dataArr;
    if (thisDataIndex.length != 0) {
        for (var i = 0; i < thisDataIndex.length; i++) {
            titleData.unshift({ 'text': thisDataIndex[i].text, 'url': thisDataIndex[i].urladdress, 'tid': thisDataIndex[i].id, 'isnavigate': tabObj.isnavigate, 'dataIndex': tabObj['indexArr'][i] });
        };
    };
    var locationDiv = '<div class="location_content"><ul class="location_ul">';
    var classNameArr = ['con_tab', 'second_con_tab', 'third_con_tab', 'fourth_con_tab'];
    for (var i = 0; i < titleData.length; i++) {
        locationDiv += '<li class="' + classNameArr[i] + '" data-tid="' + titleData[i].tid + '" data-dataIndex="' + titleData[i].dataIndex + '" data-isnavigate="' + titleData[i].isnavigate + '"  data-url="' + titleData[i].url + '" data-menu="' + titleData[i].text + '"><a><span>' + titleData[i].text + '</span><span>></span></a></li>';
    };
    locationDiv += '</ul></div>';
    return locationDiv;
};
//显示iframe的内容
function showIframe(jqObjLi) {
    $('.tab_content .content').each(function () {
        if (jqObjLi.attr('data-tid') == $(this).attr('data-tid')) {
            $(this).addClass('show_iframe').siblings('div').removeClass('show_iframe');
            return false;
        };
    });
};
//点击移动container显示选项卡
function showTab(jqObjLi) {
    var liObj = {
        'width': jqObjLi.width(),
        'posiLeft': jqObjLi.position().left,
        'margin': parseInt(jqObjLi.css('margin-left')) + parseInt(jqObjLi.css('margin-right'))
    };
    var conDisLeft = -$('.tabs_container').position().left;
    var showAreaWidth = $('.tabs_main').width();
    if (liObj.posiLeft >= conDisLeft && liObj.posiLeft + liObj.width <= conDisLeft + showAreaWidth) {
        //console.log('in');
    } else if (liObj.posiLeft < conDisLeft && liObj.width + liObj.posiLeft > conDisLeft) {
        //console.log('left_in');
        $('.tabs_container').css('left', -liObj.posiLeft + 'px');
    } else if (liObj.posiLeft + liObj.width < conDisLeft + showAreaWidth && liObj.posiLeft + liObj.width > conDisLeft + showAreaWidth) {
        //console.log('right_in');
        $('.tabs_container').css('left', -(liObj.posiLeft + liObj.width + 2 * liObj.margin - showAreaWidth + 13) + 'px');
    } else if (liObj.width + liObj.posiLeft < conDisLeft) {
        //console.log('left_out');
        $('.tabs_container').css('left', -liObj.posiLeft + 'px');
    } else {
        //console.log('right_out');
        $('.tabs_container').css('left', -(liObj.posiLeft + liObj.width + 2 * liObj.margin - showAreaWidth - 1) + 'px');
    };
    setTimeout(function () {
        var con_left = parseFloat($('.tabs_container').css('left'));
        if (con_left == 0) {
            $('.tabs .left').addClass('left_grey');
            $('.tabs .right').removeClass('right_grey');
        } else {
            $('.tabs .left').removeClass('left_grey');
        };
        if (-con_left + $('.tabs_main').width() >= $('.tabs_main_ul').width() - 2) {
            $('.tabs .right').addClass('right_grey');
            $('.tabs .left').removeClass('left_grey');
        } else {
            $('.tabs .right').removeClass('right_grey');
        };
    }, 500);
};
//点击收缩左侧菜单
function hideMenu() {
    var left_click = 1;
    var imgSrc = $('.left_top_logo .logo').attr('src');
    $('.btn_left_menu').on('click', function () {
        var settings_width = $('.settings').width() + 1;
        left_click *= -1;
        if (left_click < 0) {
            $(this).animate({
                'left': '15px'
            }, 200).removeClass('mdi-format-indent-decrease').addClass('mdi mdi-format-indent-increase');
            $('.left_bar').animate({ 'width': 50 + 'px' }, 400);
            $('.left_top_logo .logo').attr('src', '/areas/admin/img/logo_small.png').css({ 'width': '42px', 'height': '28px' });
            $('.left_top_logo .logo_item').css({ 'padding': '0', 'text-align': 'center' });
            $('.first_tab .first_con_tab').css({ 'display': 'none' });
            $('.first_tab li').css({ 'padding': '0', 'text-align': 'center' });
            $('.left_menu .second_tab').css('left', 50);
            $('.left_menu .third_tab').css('left', 50 + $('.second_tab').width() + 1);
            $('.t_con_detail').animate({ 'width': $('.mainBody').width() - 50 + 'px' }, 400);
            setTimeout(function () {
                if ($('.skullselect').css('display') == 'block') {
                    $('.tab_pages').css('width', $('.t_con_detail').width() - $('.skullselect').width() - 15 - settings_width + 'px');
                    $('.tabs_main').css('width', $('.t_con_detail').width() - $('.btn_arrow').outerWidth() * 2 - $('.skullselect').width() - 15 - settings_width-32 + 'px');
                    $('.tabs_container').css('left', '0px');
                    $('.tabs .left').addClass('left_grey');
                    $('.tabs .right').removeClass('right_grey');
                } else {
                    $('.tab_pages').css('width', $('.t_con_detail').width() - settings_width + 'px');
                    $('.tabs_main').css('width', $('.t_con_detail').width() - settings_width-32 + 'px');
                }
            }, 410);
        } else {
            $(this).animate({
                'left': '190px'
            }, 200).removeClass('mdi mdi-format-indent-increase').addClass('mdi mdi-format-indent-decrease');
            $('.left_bar').animate({ 'width': 220 + 'px' }, 400);
            $('.left_top_logo .logo').attr('src', imgSrc).css({ 'width': 'auto', 'height': '32px' });
            //$('.left_top_logo .logo_item').css({ 'padding': '0 16px', 'text-align': 'unset' });
            $('.first_tab .first_con_tab').css({ 'display': 'inline-block' });
            $('.first_tab li').css({ 'padding': '0 20px 0', 'text-align': 'left' });
            $('.left_menu .second_tab').css('left', 220);
            $('.left_menu .third_tab').css('left', 220 + $('.second_tab').width() + 1);
            $('.t_con_detail').animate({ 'width': $('.mainBody').width() - 220 + 'px' }, 400);
            if ($('.skullselect').css('display') == 'block') {
                $('.tab_pages').css('width', $('.t_con_detail').width() - 220 - $('.skullselect').width() - 15 - settings_width + 'px');
                $('.tabs_main').css('width', $('.t_con_detail').width() - 220 - $('.btn_arrow').outerWidth() * 2 - $('.skullselect').width() - 15 - settings_width-32 + 'px');
            } else {
                $('.tab_pages').css('width', $('.t_con_detail').width() - 220 - settings_width + 'px');
                $('.tabs_main').css('width', $('.t_con_detail').width() - 220 - settings_width-32 + 'px');
            }
        }
    });

}







