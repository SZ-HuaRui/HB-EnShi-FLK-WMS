$(function () {
    receiptNo();
    $("#actual_num").val(24);
    $(".container_code").focus();
    $(".container_code").keydown(function (event) {
        if (event.which == 13) {
            var receipt_no = $("#receipt_no").val();
            $("#actual_num").val('24');
            var container_code = $('.container_code').val();
            if (container_code != '' && container_code != null && receipt_no != '' && receipt_no != null && container_code.length == 8) {
                var searchSql = "select count(1) num from wms.swms_b_receiving_download where warehouse_id = '1016319470013779968' and receipt_no = '" + receipt_no + "' and container_id = '" + container_code + "'";
                //查询本地是否有数据
                common.postGateway('platform/database/selectsql', {
                    "sql": common.Encrypt(searchSql),
                    "parameters": []
                }, common.getToken(), function (data) {
                    if (data.errcode == 0 && data.errmsg == "OK") {
                        var result = JSON.parse(data.json);
                        var len = result.length;
                        if (len <= 0) {
                            alert("查询数据失败!");
                        }
                        else {
                            //从上位系统获取数据
                            if (parseInt(result[0].num) == 0) {
                                common.postGateway('platform/transflows/runtransflow', {
                                    "transflowCode": "GetContainerInfo",
                                    "parameters": [receipt_no, container_code],
                                    "commit": true
                                }, common.getToken(), function (res) {
                                    console.log(res);
                                    if (res.resMsg == null || res.resMsg == '') {
                                        barCodeChange(receipt_no, container_code);
                                    }
                                    else {
                                        alert("从上位系统获取托盘信息失败！");
                                    }
                                });
                            }
                            else {
                                barCodeChange(receipt_no, container_code);
                            }
                        }
                    }
                    else {
                        alert("查询数据失败!");
                    }
                });
                //从上位系统中获取信息
            } else {
                alert("入库单号或托盘条码不能为空或长度不为8！！！");
            }
        }
    });
    $(".conform_btn").on('click', function () {
        receiptConfirm();
    })
    $(".storage_area").on('click', function () {
        inTemporaryArea();
    })
    $("#receipt_no").off('change').on('change', function () {
        $(".container_code").focus();
    });
})
//获取入库单号数据
function receiptNo() {
    var searchSql = "select receiving_number receipt_no from wms.swms_b_receiving_h where warehouse_id = '1016319470013779968' and  state_code in ('NotExecute','Executing')";
    common.postGateway('platform/database/selectsql', {
        "sql": common.Encrypt(searchSql),
        "parameters": []
    }, common.getToken(), function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            var result = JSON.parse(data.json);
            var len = result.length;
            if (len <= 0) {
                alert("查询不到任务!");
            }
            else {
                var Order_code = '';
                $('#receipt_no').html('');
                if (len > 1) {
                    Order_code = '<option value="">请选择</option>';
                    for (var i = 0; i < len; i++) {
                        Order_code += '<option value="' + result[i].receipt_no + '">' + result[i].receipt_no + '</option>';
                    }
                } else {
                    Order_code = '<option value="' + result[0].receipt_no + '">' + result[0].receipt_no + '</option>';
                }
                $('#receipt_no').append(Order_code);
            }
        }
        else {
            alert("查询失败！");
        }
    });
}
//扫描托盘条码文本框改变事件
function barCodeChange(rpt_no, barCode) {
    //清空所有控件信息。
    var searchSql = "select d.receipt_no,d.item_code,i.item_name,d.container_id,d.bar_code,v.qty from wms.swms_b_receiving_download d left join wms.swms_wi_item i on d.item_code = i.item_code left join wms.swms_receive_v v on v.receiving_number = d.receipt_no and v.item_code = d.item_code where d.container_id = '" + barCode + "'and d.interface_status = 'NotExecute'and d.receipt_no = '" + rpt_no + "'";
    common.postGateway('platform/database/selectsql', {
        "sql": common.Encrypt(searchSql),
        "parameters": []
    }, common.getToken(), function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            var result = JSON.parse(data.json);
            var len = result.length;
            if (len <= 0) {
                alert("查询不到任务!");
            }
            else {
                $("#item_name").text(result[0].item_name);
                $("#item_code").text(result[0].item_code);
                $("#number").text(len);
                $("#left_num").text(result[0].qty);
                $('.between_d').html('');
                var bar_code = '';
                for (var i = 0; i < len; i++) {
                    var bar = result[i].bar_code
                    if (bar == null || bar == "") {
                        continue;
                    }
                    bar_code += '<p class="bar_code">' + bar + '</p>';
                }
                $('.between_d').append(bar_code);
                if ($("#item_code").text() == null || $("#item_code").text() == "") {
                    alert("上位系统无托盘信息，请组盘！");
                }
            }
        }
        else {
            alert("查询失败！");
        }
    });
}
//收货确认
function receiptConfirm() {
    var actual_num = $("#actual_num").val();
    if (actual_num < 0 || actual_num > 24 || actual_num.indexOf(".") != -1) {
        alert("实际数量不能小于零或大于24或小数！！！");
        return;
    }
    var container_code = $('.container_code').val();
    if (container_code == '' || container_code == null) {
        alert("请输入托盘号！！！");
        return;
    }
    var receiptNo = $("#receipt_no").val();
    if (receiptNo == '' || receiptNo == null) {
        alert("收货单不能为空！！！");
        return;
    }
    var itemCode = $("#item_code").text();
    if (itemCode == null || itemCode == "") {
        alert("物料编码不能为空！！！");
        return;
    }
    var item_state = $(".item_state").val();
    common.postGateway('cpfk/inbound/receiptconfirm', {
        "receiptNo": receiptNo,
        "item": itemCode,
        "receiptQty": actual_num,
        "container": container_code,
        "checkState": item_state
    }, common.getToken(), function (data) {
        if (data.errcode == 0) {
            $("#item_name").text('');
            $("#item_code").text('');
            $("#number").text('');
            $("#actual_num").val('24');
            $("#left_num").text('');
            $('.container_code').val('');
            $('.between_d').html('');
            location.href = "/RFS/Main/Shelve";
        }
        else {
            alert(data.errmsg);
            return;
        }
    });
}
//入暂存区
function inTemporaryArea() {
    var actual_num = $("#actual_num").val();
    if (actual_num < 0 || actual_num > 24 || actual_num.indexOf(".") != -1) {
        alert("实际数量不能小于零或大于24或小数！！！");
        return;
    }
    var container_code = $('.container_code').val();
    if (container_code == '' || container_code == null) {
        alert("请输入托盘号！！！");
        return;
    }
    var receiptNo = $("#receipt_no").val();
    if (receiptNo == '' || receiptNo == null) {
        alert("收货单不能为空！！！");
        return;
    }
    var itemCode = $("#item_code").text();
    if (itemCode == null || itemCode == "") {
        alert("物料编码不能为空！！！");
        return;
    }
    var item_state = $(".item_state").val();
    common.postGateway('cpfk/inbound/intemporaryconfirm', {
        "receiptNo": receiptNo,
        "item": itemCode,
        "receiptQty": actual_num,
        "container": container_code,
        "checkState": item_state
    }, common.getToken(), function (data) {
        if (data.errcode == 0) {
            alert("入暂存区成功！");
            $("#item_name").text('');
            $("#item_code").text('');
            $("#actual_num").val('24');
            $("#number").text('');
            $("#actual_num").val('');
            $("#left_num").text('');
            $('.between_d').html('');
            $('.container_code').val('');
            $(".container_code").focus();
        }
        else {
            alert(data.errmsg);
            return;
        }
    });
}