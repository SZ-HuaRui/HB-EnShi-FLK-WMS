$(function () {
    getItems();
    $(".conform_btn").on('click', function () {
        outConfirm();
    });
    $("#item_name").on('change', function () {
        getOrders();
    });
    $("#OrderNos").on('change', function () {
        getShouldNum();
    });
})
//获取物料
function getItems() {
    var sql = "select i.t_id,i.item_code, i.item_name "+
                "from wms.swms_wi_item i "+
               "where i.t_id in (select li.item_id "+
                "from wms.swms_m_location_inventory li "+
           "left join wms.swms_wi_location l "+
                  "on li.location_id = l.t_id "+
               "where li.warehouse_id = '1016319470013779968' "+
                 "and li.location_id = '1186101878480572416') "+
                 "and i.t_id in (select item_id "+
                "from swms_b_order_d od "+
                "join swms_b_order_h oh "+
                  "on od.order_id = oh.t_id "+
               "where od.warehouse_id = '1016319470013779968' "+
               "and oh.state_code <> 'Executed')";
    var token = common.getToken();
    common.postGateway('platform/database/selectsql', {
        "sql": common.Encrypt(sql),
        "parameters": []
    }, token, function (data) {
            if (data.errcode == 0 && data.errmsg == "OK") {
                var result = JSON.parse(data.json);
                var len = result.length;
                if (len <= 0) {
                    alert("找不到可供出库的物料！！！");
                    $('#OrderNos').html('');
                    $('#item_name').html('');
                    $("#item_code").text('');
                    $("#storage_num").text('');
                    $("#actual_num").val('');
                    return;
                }
                var optionList = '';
                for (var i = 0; i < result.length; i++) {
                    optionList += '<option item_id = "' + result[i].t_id + '" value="' + result[i].item_code + '">' + result[i].item_name + '</option>'
                }
                $('#item_name').append(optionList);
                $("#item_name").change();
            }
            else {
                alert("查询失败！");
            }
    });
}
//根据物料编码获取出库单号
function getOrders() {
    var item_code = $('#item_name').val();
    if (item_code == null) {
        return;
    }
    var item_id = $('#item_name').find("option:selected").attr("item_id");
    var sql = "select h.order_no orderno,"+ 
                      "nvl(inv.qty, 0) temp_qty,"+
                      "(nvl(d.plan_qty, 0) - nvl(d.finished_qty, 0)) order_qty " +
                "from wms.swms_b_order_h h " +
                "join wms.swms_b_order_d d " +
                  "on d.order_id = h.t_id " +
                "join wms.swms_m_location_inventory inv " +
                  "on d.item_id = inv.item_id " +
               "where h.warehouse_id = '1016319470013779968' " +
                 "and d.item_id = '" + item_id + "' " +
                 "and h.state_code <> 'Executed' " +
                 "and inv.location_id = '1186101878480572416' " +
                 "and inv.item_id = '" + item_id + "' ";
    var token = common.getToken();
    common.postGateway('platform/database/selectsql', {
        "sql": common.Encrypt(sql),
        "parameters": []
    }, token, function (data) {
            var optiondata = '';
            $('#OrderNos').html('');
            if (data.errcode == 0 && data.errmsg == "OK") {
                var result = JSON.parse(data.json);
                var len = result.length;
                if (len <= 0) {
                    alert("没有此物料的出库单!");
                    return;
                }
                else if (len == 1) {
                    optiondata += '<option>' + result[0].orderno + '</option>'
                    $('#OrderNos').append(optiondata);
                    $("#OrderNos").change();
                }
                else {
                    optiondata = '<option>请选择</option>';
                    for (var i = 0; i < len; i++) {
                        optiondata += '<option>' + result[i].orderno + '</option>'
                    }
                    $('#OrderNos').append(optiondata);
                }
                $("#item_code").text(item_code);
                $("#storage_num").text(result[0].temp_qty);
                $("#actual_num").val('');
            }
            else {
                alert("查询失败！");
            }
    });
}
//根据物料和出库单号获取应出数量
function getShouldNum() {
    var item_id = $('#item_name').find("option:selected").attr("item_id");
    var OrderNos = $('#OrderNos').val();
    var item_code = $('#item_name').val();
    if (item_code == null || OrderNos == null) {
        return;
    }
    var sql = "select (nvl(d.plan_qty, 0) - nvl(d.finished_qty, 0)) order_qty "+
                "from wms.swms_b_order_d d "+
                "join wms.swms_b_order_h h "+
                  "on d.order_id = h.t_id "+
               "where d.warehouse_id = '1016319470013779968' "+
                 "and h.order_no = '" + OrderNos + "' "+
                 "and d.item_id = '" + item_id + "'";
    var token = common.getToken();
    common.postGateway('platform/database/selectsql', {
        "sql": common.Encrypt(sql),
        "parameters": []
    }, token, function (data) {
            if (data.errcode == 0 && data.errmsg == "OK") {
                var result = JSON.parse(data.json);
                var len = result.length;
                if (len <= 0) {
                    alert("查询不到应出数量!");
                    return;
                } else {
                    $("#should_num").text(result[0].order_qty);
                }
            }
            else {
                alert("查询不到应出数量!");
            }
    });
}
//确定按钮
function outConfirm() {
    var storage_num = parseInt($("#storage_num").text());
    var item_code = $("#item_code").text();
    var actual_num = parseInt($("#actual_num").val());
    var should_num = parseInt($("#should_num").text());
    if (actual_num <= 0) {
        alert("实出数量不能小于零！！！");
        return;
    }
    if (actual_num > storage_num || actual_num > should_num) {
        alert("实出数量不能超出暂存区数量且不能超过应出数量！！！");
        return;
    }
    var OrderNos = $("#OrderNos  option:selected").text();
    if (OrderNos == null || OrderNos == "") {
        alert("出库单号不能为空！！！");
        return;
    }
    var cache = JSON.parse(common.Decrypt(sessionStorage["encrypt"]));
    var userName = cache.UserName;
    common.postGateway('platform/transflows/runtransflow', {
        "transflowCode": "outbound_cpf_temp_inv",
        "parameters": [OrderNos, item_code, actual_num, userName],
        "commit": true
    }, common.getToken(), function (res) {
            if (res.json[1].fldValue == '成功') {
                alert("下架成功！！！");
                $('#item_name').html('');
                getItems();
            }
            else {
                alert(res.json[1].FldValue)
            } 
    });
}
