
$(function () {
    $("#actual_num").val(24);
    getReceiptNo();
    $("#container_code").focus();

    $(".conform_btn").off('click').on('click', function () {
        composingContainer();
    });
    //物料名称的选择事件
    $("#item_name").off('change').on('change', function () {
        var obj = $('#item_name option:selected');
        $("#left_num").text(obj.attr('data-remain'));
    });
})
//获取物料
function getReceiptNo(isReload) {
    var searchSql = "select rpt.receiving_id,h.receiving_number,it.item_code,it.item_name,rpt.actual_qty,rpt.actual_qty - nvl(rpt.has_received_qty, 0) remain_qty from wms.swms_b_receiving_d rpt left join wms.swms_wi_item it on rpt.item_id = it.t_id left join wms.swms_b_receiving_h h on rpt.receiving_id = h.t_id where rpt.warehouse_id = '1016319470013779968'and nvl(rpt.actual_qty, 0) - nvl(rpt.has_received_qty, 0) > 0 and rpt.state_code <> 'Executed' and exists (select t_id from wms.swms_b_receiving_h h where rpt.receiving_id = h.t_id and h.state_code <> 'Executed')";
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
                    $("#left_num").text('');
                    return;
                };
                $('#orderNo').html('');
                $('#orderNo').append('<option>请选择</option>');
                var obj = {};
                $.each(result, function (i) {
                    if (obj[result[i].receiving_number] == undefined) {
                        var receiptNo = '';
                        receiptNo += '<option value="' + result[i].receiving_number + '">' + result[i].receiving_number + '</option>';
                        $('#orderNo').append(receiptNo);
                        obj[result[i].receiving_number] = [[result[i].item_code, result[i].item_name, result[i].actual_qty, result[i].remain_qty]];
                    } else {
                        obj[result[i].receiving_number].push([result[i].item_code, result[i].item_name, result[i].actual_qty, result[i].remain_qty]);
                    };
                });
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
    } else {
        $("#left_num").text('');
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
    var actual_num = $("#actual_num").val();
    if (actual_num == null || actual_num == "") {
        alert("实际数量不能为空！！！");
        return;
    }
    if (actual_num.indexOf(".") != -1) {
        alert("实际数量不能为小数！！！");
        return;
    }
    var actual_num = parseInt(actual_num);
    if (actual_num < 0 || actual_num > 24) {
        alert("实际数量不能小于零或大于24！！！");
        return;
    }
    if (actual_num > $("#left_num").text()) {
        alert("实际数量不大于剩余数量！！！");
        return;
    }
    var item_state = $(".item_state").val();
    var item_code = $('#item_name').val();
    var list_data = ['1'];
    common.postGateway('cpfk/inbound/composecontainer', {
        "type":"0",
        "receiptNo": orderNo,
        "item": item_code,
        "container": container_code,
        "receiptQty": actual_num,
        "checkState": item_state,
        "barcode": list_data,
        "produceTime": list_data
    }, common.getToken(), function (data) {
        if (data.errcode == 0) {
            alert("组盘成功！");
            $("#container_code").val('');
            $("#container_code").focus();
            if ($("#left_num").text() - actual_num <= 0) {
                alert("此货品已组盘完成！！！");
                $('#orderNo').attr('data-no', orderNo);
                getReceiptNo(true);
                return;
            };
            var searchSql = "select compose_qty remain from wms.swms_receive_v where item_code='" + item_code + "' and receiving_number='" + orderNo + "'";
            common.postGateway('platform/database/selectsql', {
                "sql": common.Encrypt(searchSql),
                "parameters": []
            }, common.getToken(), function (res) {
                    if (res.errcode == 0 && res.errmsg == "OK") {
                        var result = JSON.parse(res.json);
                        $("#left_num").text(result[0].remain);
                        $("#item_name option[value='" + item_name + "']").attr("data-remain", result[0].remain);
                    } else {
                        alert("查询剩余数量失败！");
                    }
            });
        }
        else {
            alert(data.errmsg);
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