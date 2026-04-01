$(function () {
    getBorrowOrder();
    $("#orderNo").off('change').on('change', function () {
        getItemsInOrder();
    });
    $("#container_code").keydown(function (event) {
        if (event.which == 13) {
            var container_code = $('#container_code').val();
            if (container_code != '' && container_code != null) {
                getLocationByContainer(container_code);
            } else {
                alert("托盘条码不能为空！！！");
            }
        }
    });
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
    $(".between_d").off('click').on('click', '.bar_code', function () {
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
        if ($('.bar_code').length <= 0) {
            $('#actual_num').text(24);
            $('#one_code').text('');
        } else {
            $('#actual_num').text($('.bar_code').length);
        }
    });
    //点击确定按钮
    $(".conform_btn").off('click').on('click', function () {
        borrowConform();
    });
})
//获取借出单号
function getBorrowOrder() {
    //oh.client_id='&CLIENT_ID&' and oh.org_id='&ORG_ID&' and
    var searchSql = "select * from wms_t_order_h oh where  oh.warehouse = 'CPF' and oh.type = 'Order_Lend'  and status != 'Order_Complete'";
    common.postGateway('platform/database/selectsql', {
        "sessionKey": common.getCookie("sessionKey"),
        "sql": common.Encrypt(searchSql)
    }, function (data) {
        if (data == null) {
            alert("系统出错，无法取得返回值！");
            return;
        }
        if (data.length <= 0) {
            alert("查询借出单失败！！！");
            $('#orderNo').html('');
            $('#item_name').html('');
            $("#actual_num").text('');
            return;
        };
        var len = data.length;
        var Order_code = '';
        $('#orderNo').html('');
        if (len > 1) {
            Order_code = '<option value="">请选择</option>';
            for (var i = 0; i < len; i++) {
                Order_code += '<option value="' + data[i].t_id + '">' + data[i].ordernumber + '</option>';
            }
            $('#orderNo').append(Order_code);
        } else {
            Order_code = '<option value="' + data[0].t_id + '">' + data[0].ordernumber + '</option>';
            $('#orderNo').append(Order_code);
            getItemsInOrder();
        }
    });
}
//获取物料名称
function getItemsInOrder() {
    //client_id='&CLIENT_ID&' and org_id='&ORG_ID&' and
    var order_id = $('#orderNo').val();
    if (order_id == '' ||order_id == null || order_id.length <= 0) {
        $('#item_name').html('');
        return;
    }
    var sql = "select item, item_name from wms_t_order_d od where od.t_order_h_id = '" + order_id + "'";
    common.postGateway('platform/database/selectsql', {
        "sessionKey": common.getCookie("sessionKey"),
        "sql": common.Encrypt(sql)
    }, function (data) {
        if (data == null) {
            alert("系统出错，无法取得返回值！");
            return;
        }
        if (data.length <= 0) {
            alert("查询借出单失败！");
            return;
        }
        var len = data.length;
        var item_name = '';
        $('#item_name').html('');
        if (len > 1) {
            item_name = '<option value="">请选择</option>';
            for (var i = 0; i < len; i++) {
                item_name += '<option value="' + data[i].item + '">' + data[i].item_name + '</option>';
            }
        } else {
            item_name = '<option value="' + data[0].item + '">' + data[0].item_name + '</option>';
        }
        $('#item_name').append(item_name);
    });
}
//扫描托盘条码
function getLocationByContainer(container_id) {
    //client_id='&CLIENT_ID&' and org_id='&ORG_ID&' and
    var sql = "select location from wms_container_inventory ci where container_id = '" + container_id + "'";
    common.postGateway('platform/database/selectsql', {
        "sessionKey": common.getCookie("sessionKey"),
        "sql": common.Encrypt(sql)
    }, function (data) {
        if (data == null) {
            alert("系统出错，无法取得返回值！");
            return;
        }
        if (data.length <= 0) {
            alert("查询货位失败！");
            return;
        }
        $('#location_code').text(data[0].location);
    });
}
//点击确定按钮
function borrowConform() {
    var order_no = $('#orderNo option:selected').text();
    if (order_no == '' || order_no == null) {
        alert("请选择借出单号！！！");
        return;
    }
    var pallet = $('#container_code').val();
    if (pallet == '' || pallet == null) {
        alert("托盘条码不能为空！！！");
        return;
    }
    var location = $('#location_code').text();
    if (location == '' || location == null) {
        alert("货位编码不能为空！！！");
        return;
    }
    var itemname = $('#item_name').val();
    if (itemname == '' || itemname == null) {
        alert("请选择物料名称！！！");
        return;
    }
    var qty = $('#actual_num').text();
    var barcode = [];
    if ($('.bar_code').length <= 0) {
        barcode = [];
    } else {
        for (var i = 0, j = $('.bar_code').length; i < j; i++) {
            barcode.push($('.bar_code').eq(i).text());
        };
    }
    common.postGateway('wms/cpf/outbound/lendout', {
        "sessionKey": common.getCookie("sessionKey"),
        "warehouse": "CPF",
        "orderNo": order_no,
        "palletCode": pallet,
        "location": location,
        "item": itemname,
        "qty": qty,
        "barcodes": barcode
    }, function (data) {
        if (data == null) {
            alert("调用服务失败，请重试!");
            return;
        }
        if (data.ErrorCode != 0) {
            alert(data.ErrMsg);
            return;
        }
        alert("借出成功！");
    });
}
