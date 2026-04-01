
$(function () {
    getReceiptNo();
    $("#one_code").focus();
    $("#one_code").keydown(function (event) {
        if (event.which == 13) {
            var one_code = $('#one_code').val();
            var produce_data = one_code.substring(16, 22);
            if (one_code != '' && one_code != null && one_code.length == 32) {
                var isAdd = true;
                for (var i = 0, j = $('.bar_code').length; i < j; i++) {
                    if (one_code == $('.bar_code').eq(i).text()) {
                        alert("该一号工程码已扫描，请勿重复扫描！！！");
                        isAdd = false;
                        break;
                    }
                };           
                if (isAdd) {
                    $('.between_d').append('<p class="bar_code" produce_data="' + produce_data + '">' + one_code + '</p>');
                    $('#actual_num').text($('.bar_code').length);
                }
            } else {
                alert("一号工程码不能为空或者长度必须是32！！！");
            }
            $(this).val('');
        }
    });
    //点击一号工程码
    $(".between_d").off('click').on('click','.bar_code', function () {
        $(this).addClass('active_one').siblings().removeClass('active_one');
    });
    //点击删除按钮
    $(".delete_btn").off('click').on('click', function () {
        for (var i = 0, j = $('.bar_code').length; i < j; i++) {
            if ($('.bar_code').eq(i).hasClass('active_one')) {
                $('.bar_code').eq(i).remove();
                break;
            }
        };        
        $('#actual_num').text($('.bar_code').length);
    });
    $(".conform_btn").off('click').on('click', function () {
        composingContainer();
    });
    //物料名称的选择事件
    $("#item_name").off('change').on('change', function () {
        var obj = $('#item_name option:selected');
        $("#left_num").text(obj.attr('data-remain'));
        $("#item_code").text(obj.attr('value'));
    });
})
//获取物料
function getReceiptNo(isReload) {
    var searchSql = "select receiving_number m_receipt_h_id,item_code item,item_name name,actual_qty receipt_qty,compose_qty remain_qty from wms.swms_receive_v";
    var token = common.getToken();
    common.postGateway('platform/database/selectsql', {
        "sql": common.Encrypt(searchSql),
        "parameters": []
    }, token, function (data) {
            if (data.errcode == 0 && data.errmsg == "OK") {
                var result = JSON.parse(data.json);
                var len = result.length;
                if (len <= 0) {
                    alert("找不到收货单号！！！");
                    $('#orderNo').html('');
                    $('#item_name').html('');
                    $("#actual_num").text('');
                    $("#left_num").text('');
                    return;
                };

                $('#orderNo').html('');
                $('#orderNo').append('<option>请选择</option>');
                var obj = {};
                $.each(result, function (i) {
                    if (obj[result[i].m_receipt_h_id] == undefined) {
                        var receiptNo = '';
                        receiptNo += '<option value="' + result[i].m_receipt_h_id + '">' + result[i].m_receipt_h_id + '</option>';
                        $('#orderNo').append(receiptNo);
                        obj[result[i].m_receipt_h_id] = [[result[i].item, result[i].name, result[i].receipt_qty, result[i].remain_qty]];
                    } else {
                        obj[result[i].m_receipt_h_id].push([result[i].item, result[i].name, result[i].receipt_qty, result[i].remain_qty]);
                    };
                });
                //重载页面
                if (isReload == true) {
                    refreshData();
                    loadItemname(obj);
                };
                //收货单号的选择事件
                $("#orderNo").off('change').on('change', function () {
                    loadItemname(obj);
                });
            }
            else {
                alert("查询失败！");
            }
    });
}

//加载物料名称
function loadItemname(obj) {
    $('#item_name').html('');
    var item_code = $('#orderNo option:selected').text();
    if (obj[item_code].length == 1) {
        $('#item_name').append('<option value="' + obj[item_code][0][0] + '">' + obj[item_code][0][1] + '</option>');
        $("#left_num").text(obj[item_code][0][3]);
        $("#item_code").text(obj[item_code][0][0]);
    } else {
        $("#left_num").text('');
        $("#item_code").text('');
        $('#item_name').append('<option>请选择</option>');
        $.each(obj[item_code], function (i) {
            $('#item_name').append('<option value="' + obj[item_code][i][0] + '" data-receipt=' + obj[item_code][i][2] + ' data-remain=' + obj[item_code][i][3] + '>' + obj[item_code][i][1] + '</option>');
        });
    };
};

//确定按钮
function composingContainer() {
    var orderNo = $("#orderNo").val();
    if (orderNo == null || orderNo == "") {
        alert("收货单号不能为空！！！");
        return;
    }   
    var item_name = $("#item_name").val();
    if (item_name == null || item_name == "") {
        alert("物料名称不能为空！！！");
        return;
    }
    var container_code = $("#container_code").val();
    if (container_code == null || container_code == "") {
        alert("托盘条码不能为空！！！");
        return;
    }
    var actual_num = parseInt($("#actual_num").text());
    if (actual_num < 0 || actual_num > 24) {
        alert("已增数量不能小于零或大于24！！！");
        return;
    }
    if (actual_num > $("#left_num").text()) {
        alert("已增数量不能大于剩余数量！！！");
        return;
    }
    var onecode_data = [], produceTime=[];
    for (var i = 0, j = $('.bar_code').length; i < j; i++) {
        onecode_data.push($('.bar_code').eq(i).text());
        produceTime.push($('.bar_code').eq(i).attr('produce_data'));
    };
    var item_state = $(".item_state").val();
    var token = common.getToken();
    common.postGateway('cpfk/inbound/composecontainer', {
        "type": "1",
        "receiptNo": orderNo,
        "item": item_name,
        "container": container_code,
        "receiptQty": actual_num,
        "checkState": item_state,
        "barcode": onecode_data,
        "produceTime": produceTime
    }, token, function (data) {
            if (data.errcode == 0) {
            alert("组盘成功！");
            $("#container_code").val('');
            $('#one_code').val('');
            $('.between_d').html('');
            $("#one_code").focus();
            if ($("#left_num").text() - actual_num <= 0) {
                alert("此货品已组盘完成！！！");
                $('#orderNo').attr('data-no', orderNo);
                getReceiptNo(true);
                return;
            };
            var searchSql = "select compose_qty, remain_qty from wms.swms_receive_v where receiving_number = '" + orderNo + "' and item_code = '" + item_name+"'";
                common.postGateway('platform/database/selectsql', {
                    "sql": common.Encrypt(searchSql),
                    "parameters": []
                }, token, function (res) {
                    if (res.errcode == 0 && res.errmsg == "OK") {
                        var result = JSON.parse(data.json);
                        $("#left_num").text(result[0].remain_qty);
                        $("#item_name option[value='" + item_name + "']").attr("data-remain", result[0].remain_qty);
                    }
                    else {
                        alert("获取组盘剩余数量失败！")
                    }
                });
            } else {
                alert(data.errmsg);
                return;
            }
    });
}

//重载页面
function refreshData() {
    var orderNo = $('#orderNo').attr('data-no');
    $("#left_num").text('');
    for (var i = 0; i < $("#orderNo option").length; i++) {
        if (orderNo == $("#orderNo option").eq(i + 1).text()) {
            console.log('选择:');
            $("#orderNo option").eq(i + 1).attr('selected', true);
            return;
        };
    };
};