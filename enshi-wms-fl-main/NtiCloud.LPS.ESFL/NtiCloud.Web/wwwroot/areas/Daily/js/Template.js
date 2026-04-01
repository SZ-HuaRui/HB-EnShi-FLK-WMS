$(function () {
    var fieldType = { 'text': '文本型', 'number': '数字型', 'select': '下拉选择项', 'date': '日期型', 'radio': '单选型', 'checkbox': '多选型', 'switch': '开关型', 'grid': '表格型'};
    var fieldIndex,pageData=[];
    //点击新建模板
    $('#addTemp').on('click', function () {
        $('.add_template').show();
    })
    //点击关闭新建模板
    $('#close_img').on('click', function () {
        $('.add_template').hide();
    })
    //点击添加字段
    $('#add_field').on('click', function () {
        var fieldStr = '<div class="add_field_c field_d"><img class="delete_field" src = "/areas/Daily/img/icon-delete.png" /><input type="text" class="" placeholder="请先填写字段名称" /><div class="add_type"><span>文本型</span><img src="/areas/Daily/img/icon-right.png" /></div></div >';
        $(this).before(fieldStr);
    })
    //点击删除字段
    $('.add_field_t').on('click','.delete_field', function () {
        $(this).parent().remove(); 
    })
    //点击类型
    $('.add_field_t').on('click', '.add_type', function () {
        fieldIndex=$(this).parent().index();
        $(this).parent().attr('active', 'click_active').siblings().attr('active', '');
        $('.field_edit').hide();
        $('.field_type_edit').show();
    })
    //点击字段属性
    $('.field_type_edit .prop_d').on('click', function () {
        $(this).children().children('div').addClass('prop_img');
        $(this).siblings().children().children('div').removeClass('prop_img');
        $('.save_btn').attr({ 'field_type': $(this).attr('field_type'), 'isrequired': new Boolean($("#isRequired").switchbutton("options").checked) });
        if ($(this).attr('is_edit') == 'true') {
            var this_type = $(this).attr('field_type');
            $('.field_type_edit').hide();  
            $('.type_child').show();
            if (this_type == 'select') {                              
                $('.edit_select').show();
                $('.edit_switch').hide();
                $('.edit_grid').hide();
            } else if (this_type == 'switch') {
                $('.edit_select').hide();
                $('.edit_switch').show();
                $('.edit_grid').hide();
            } else if (this_type == 'grid') {
                $('.edit_select').hide();
                $('.edit_switch').hide();
                $('.edit_grid').show();
            }            
        }
    })
    //点击保存字段类型
    $('.field_type_edit .save_btn').on('click', function () {
        var this_type = $(this).attr('field_type');
        var this_required = $(this).attr('isrequired');
        if (this_type) {
            $('.field_type_edit').hide();
            $('.field_edit').show();
            $('.field_edit .field_d[active="click_active"]').children('.add_type').children('span').text(fieldType[this_type]);
            $('.field_edit .field_d[active="click_active"]').attr({ 'field_type': this_type, 'isrequired': this_required });
        }
    })
    //点击返回更改类型
    $('.type_child .back_btn').on('click', function () {
        $('.field_type_edit').show();
        $('.type_child').hide();
    })
});