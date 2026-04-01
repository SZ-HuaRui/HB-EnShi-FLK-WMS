$(function () {
    $(".container_code").focus();
    $(".container_code").keydown(function (event) {
        if (event.which == 13) {
            var container_code = $('.container_code').val();
            if (container_code != '' && container_code != null) {
                getInfo();
            } else {
                alert("托盘条码不能为空！！！");
            }
        }
    });
    $(".conform_btn").on('click', function () {
        outConfirm();
    })
})
//获取任务
function getInfo() {
    var container_code = $(".container_code").val();
    if (container_code == null || container_code == "") {
        alert("托盘条码不能为空！！！");
        return;
    }
    $("#actual_num").val(24);
    var sql = "select h.order_no,item.item_code,item.item_name,con_h.qty "+
                "from wms.swms_b_order_h h "+
                "join wms.swms_b_order_d d "+
                  "on h.t_id = d.order_id "+
                "join wms.swms_wi_item item "+
                  "on d.item_id = item.t_id " +
                "join wms.swms_m_container_h con_h "+
                  "on d.item_id = con_h.item_id "+
               "where h.warehouse_id = '1016319470013779968' "+
                 "and h.state_code <> 'Executed' "+
                 "and d.state_code <> 'Executed' "+
                 "and con_h.container_barcode = '" + container_code + "'";
    var token = common.getToken();
    common.postGateway('platform/database/selectsql', {
        "sql": common.Encrypt(sql),
        "parameters": []
    }, token, function (data) {
            if (data.errcode == 0 && data.errmsg == "OK") {
                var result = JSON.parse(data.json);
                var len = result.length;
                var orderHtml = "";
                if (len <= 0) {
                    alert("没有此物料的出库单!");
                    $("#item_name").text('');
                    $("#item_code").text('');
                    $(".OrderNos").val('');
                    return;
                }
                else if (len == 1) {
                    orderHtml += '<option>' + result[0].order_no + '</option>';
                    $("#number").text(result[0].qty);
                }
                else {
                    orderHtml = '<option>请选择</option>';
                    for (var i = 0; i < len; i++) {
                        orderHtml += '<option>' + result[i].order_no + '</option>'
                    }
                }
                $("#item_code").text(result[0].item_code);
                $("#item_name").text(result[0].item_name);
                $(".OrderNos").append(orderHtml);
            }
            else {
                alert("查询失败！");
            }
    });           
}
//下架确定按钮
function outConfirm() {
    var container = $(".container_code").val();
    if (container == null || container == "") {
        alert("托盘条码不能为空！！！");
        return;
    }
    var orderNo = $(".OrderNos  option:selected").text();
    var itemCode = $("#item_code").text();
    if (orderNo == null || orderNo == "") {
        alert("出库单号不能为空！！！");
        return;
    }
    var actual_num = $("#actual_num").val();
    if (actual_num < 0 && actual_num > parseInt($("#number").text())) {
        alert("实出数量不能超过件烟数量！！！");
        return;
    }
    var token = common.getToken();
    common.postGateway('cpfk/outbound/takeoutconfirm', {
        "orderNo": orderNo,
        "container": container,
        "item": itemCode,
        "shipmentQty": actual_num
    }, token, function (data) {
        if (data.errcode == 0) {
            alert("下架成功！");
            $(".container_code").val('');
            $(".OrderNos").val('');
            $("#item_name").text('');
            $("#item_code").text('');
            $("#number").text('');
            $("#actual_num").val('');
        }
        else {
            alert(data.errmsg);
            return;
        }
    });
}
