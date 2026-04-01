$(function () {
    getBillInfo();
    $("#receipt_no").on('change', function () {
        InventoryChange();
    });       
})
//获取单号数据
function getBillInfo() {
    var sql = "select 'r' bill_type,t_id,receiving_number bill_no from wms.swms_b_receiving_h where state_code <> 'Executed' "
        +"union all " 
        +"select 'o' bill_type,t_id,order_no bill_no from wms.swms_b_order_h where state_code <> 'Executed'";
    var token = common.getToken();
    common.postGateway('platform/database/selectsql', {
        "sql": common.Encrypt(sql),
        "parameters": []
    }, token, function (data) {
            if (data.errcode == 0 && data.errmsg == "OK") {
                var result = JSON.parse(data.json);
                var len = result.length;
                if (len <= 0) {
                    alert("查询不到单号!");
                }
                else {
                    var Order_code = '';
                    $('#receipt_no').html('');
                    if (len > 1) {
                        Order_code = '<option value="">请选择</option>';
                        for (var i = 0; i < len; i++) {
                            Order_code += '<option bill_type="' + result[i].bill_type + '" value="' + result[i].t_id + '">' + result[i].bill_no + '</option>';
                        }
                        $('#receipt_no').append(Order_code);
                    } else {
                        Order_code = '<option bill_type="' + result[0].bill_type + '"  value="' + result[0].t_id + '">' + result[0].bill_no + '</option>';
                        $('#receipt_no').append(Order_code);
                        InventoryChange();
                    }
                }
            }
            else {
                alert("查询失败！");
            }
    });
}
//获取单号数据信息
function InventoryChange() {
    var t_id = $('#receipt_no').val();
    if (t_id == '' || t_id == null) {
        alert("请选择订单!");
        return;
    }
    var sql =""
    var bill_type = $('#receipt_no option:selected').attr('bill_type');
    if(bill_type == 'o'){
        sql ="select '发货' bill_type,i.item_code,i.item_name,d.plan_qty request_qty,nvl(finished_qty,0) actual_qty,(plan_qty-nvl(finished_qty,0)) left_qty from wms.swms_b_order_d d left join wms.swms_wi_item i on d.item_id = i.t_id  where d.order_id = '"+t_id+"'";
    }
    else{
        sql ="select '收货' bill_type,i.item_code,i.item_name,d.actual_qty request_qty,(nvl(putaway_qty,0)+nvl(temporary_qty,0)+nvl(virtual_qty,0)) actual_qty,(d.actual_qty-nvl(putaway_qty,0)-nvl(temporary_qty,0)-nvl(virtual_qty,0)) left_qty from wms.swms_b_receiving_d d left join wms.swms_wi_item i on d.item_id = i.t_id where d.receiving_id = '"+t_id+"'";
    }
    var token = common.getToken()
    common.postGateway('platform/database/selectsql', {
        "sql": common.Encrypt(sql),
        "parameters": []
    }, token, function (data) {
            if (data.errcode == 0 && data.errmsg == "OK") {
                var result = JSON.parse(data.json);
                var len = result.length;
                if (len <= 0) {
                    alert("查询不到订单明细!");
                    return;
                }
                else {
                    var table_loc_tr = '';
                    for (var r = 0; r < len; r++) {
                        table_loc_tr += '<tr><td>' + result[r].bill_type + '</td><td>' + result[r].item_code + '</td><td>' + result[r].item_name + '</td><td>' + result[r].request_qty + '</td><td>' + result[r].actual_qty + '</td><td>' + result[r].left_qty + '</td></tr>';
                    };
                    $('.table_loc tbody').html('');
                    $('.table_loc tbody').append(table_loc_tr);
                }
            }
            else {
                alert("查询失败!");
            }
    });
}
