$(function () {

    $(".product_project").hide(); 

    //点击选择框区域 切换下拉框显示和隐藏
    $('#ai_content').on('click','.select_area',function (){
        if ($(this).hasClass('select_active')){
            $(this).removeClass('select_active');
        } else {
            $(this).addClass('select_active');
        };
        return false;
    });

    //点击下拉框选择内容项
    $('#ai_content').on('click', '.select_li', function () {
        if (!($(this).hasClass('active'))) {
            $(this).addClass('active').siblings().removeClass('active').parent('.select_ul').siblings('.select_con').attr('data-value', $(this).attr('data-value')).html($(this).html());
        };
    });

    //点击页面 隐藏下拉框
    $('body').click(function () {
        $('.select_area').removeClass('select_active');
    });

    //点击上传按钮
    $('.upload_act').click(function (){
        $('#files').click();
    });

    //点击具选择识别类型
    $('.work_content').click(function (){
        //$('.header-item a').hide();
        $('.content_c').hide();
        $('#ai_content').show().find('.ai_breadcrumb').append('<span class="return_control">控制台</span>' +' / <span class="return_span">AI技术</span>' + ' / <span class="now_span" data-value="' + $(this).find('.pro_name').attr('data-value')+'">' + $(this).find('.pro_name').html()+'</span>');
        $('.select_area').removeClass('show');
        if ($(this).find('.pro_name').attr('data-value') == 'idcard') {//身份证识别
            $('.select_area').eq(0).addClass('show');
        } else if ($(this).find('.pro_name').attr('data-value') == 'vehicle_license') {//行驶证驾驶证识别
            $('.select_area').eq(1).addClass('show');
        };
        $(window).scrollTop(0);
        return false;
    });
    //点击面包屑
    $('.ai_breadcrumb').on('click', 'span', function () {
        if ($(this).hasClass('return_span')) {
            window.location.reload();
        } else if ($(this).hasClass('return_control')) {
            window.location.href = "/Member/ControlDesk";
        } else {
            $.messageMsg('该页面已打开!', { time: 900, icon: 1, shade: [0.1, '#fff'] });
        };
    });
});

// 选择图片显示
function imgChange(obj) {
    //获取点击的文本框
    var file = $("#files")[0];
    window.URL = window.URL || window.webkitURL;
    var imgUrl = window.URL.createObjectURL(file.files[0]);
    var imgObj = $('#t_img');
    imgObj.attr('src', imgUrl);
    //释放内存
    imgObj.onload = function (){
        window.URL.revokeObjectURL(imgUrl);
    };
};

//上传识别
function upLoad() {
    var reader = new FileReader();
    var file = $("#files")[0].files[0];
    if (file) {
        //将文件以Data URL形式读入页面  
        reader.readAsDataURL(file);
        reader.onload = function (e) {
            console.log(reader.result);
            var imageStr = reader.result.split(',')[1];
            var dataObj = {
                image_base64: imageStr,
                type: $('.ai_breadcrumb .now_span').attr('data-value')
            };
            //var nameStr = $('.ai_act .show').attr('data-string');
            //if (nameStr) {
            //    dataObj[nameStr] = $('.ai_act .show .select_con').attr('data-value');
            //};
            $.messageLoad(); //设定最长等待10秒 
            common.postGateway("AliDingTalk/DingAI/OCR", dataObj, null, function (res) {
                $.messageBoxCloseLoading();
                if (res.errcode == 0) {
                    $('#conList p').remove();
                    var typeStr = dataObj.type;
                    var resObj = JSON.parse(res.json.data);
                    for (var i in resObj) {
                        $('#conList').append('<p><span class="name">' + i + ':</span><span> ' + resObj[i] + '</span></p>');
                    }
                } else {
                    $.messageBoxAlert(0, res.json);
                };
            });
        };
    };
};












