$(function () { 
    init();
    ReceivingIteminfo = null;
})
function init() {
    getReceiptNo();
    $("#receipt_no").change(function () { receiptOnChange() });
    $("#item_name").change(function () { itemOnChange()});
}
function getReceiptNo() {
    var token = common.getToken();
    common.postGateway("wms/GroupContainer/GetReceiptNo", {
        "warehouseId": common.getCookie("warehouseId")
    }, token, function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            var result = JSON.parse(data.json);
            var len = result.length;
            var selectHtml = '';
            var isContinue = false;
            if (len >= 1) {
                for (var i = 0; i < len; i++) {
                    selectHtml += '<option value="' + result[i].t_id + '">' + result[i].doc_no + '</option>';
                }
                isContinue = true;
            }
            if (isContinue) {
                $('#receipt_no').append(selectHtml);
                getReceiptDetail()
            }
        }
    });
}
function getReceiptDetail() {
    var receiptId = $('#receipt_no option:checked').val();
    if (isNullOrEmpty(receiptId)) {
        return;
    }
    var token = common.getToken();
    common.postGateway("wms/GroupContainer/GetItemsByReceiptId", {
        "receiptId": receiptId
    }, token, function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            var result = JSON.parse(data.json);
            var len = result.length;
            var selectHtml = '';
            var isContinue = false;
            if (len >= 1) {
                for (var i = 0; i < len; i++) {
                    selectHtml += '<option value="' + result[i].item_id + '">' + result[i].item_name + '</option>';
                }
                isContinue = true;
            }
            if (isContinue) {
                $('#item_name').html('');
                $('#item_name').append(selectHtml);
                render()
            }
        }
    });
}

function confirm_btn() {
    var token = common.getToken();
    var qty = $('#item_qty').val();
    if (!isPositiveInt(qty)) {
        alert('请输入正确的数量！');
        return;
    }
    if (parseInt(qty) > parseInt($('#max_container_qty').val())) {
        alert('实际数量大于托盘载量！');
        return;
    }
    if (parseInt(qty) > parseInt($('#item_finished_qty').val())) {
        alert('实际数量大于待收数量！');
        return;
    }
    var list =
    {
        "ReceiptId": [$('#receipt_no').val(), '收货单号为空，请检查！'],
        "ItemId": [$('#item_name').val(), '物料为空，请检查！'],
        "ItemName": [$('#item_name').find("option:selected").text(), '物料为空，请检查！'],
        "Qty": [qty,'数量为空，请检查！'],
        "UomId": [$('#item_uom_name').attr('item_uom_id'), '单位为空，请检查！'],
        "ContainerCapacity" :[$('#max_container_qty').val(), '托盘载量为空，请检查！'],
        "SupplierId" :[$('#item_supplier').val(), '供应商为空，请检查！'],
        "Lot": [$('#item_lot').val(), '批次号为空，请检查！'],
        "QCStatus": [$('#qc_status').val(), '质检状态为空，请检查！'],
        "ContainerBarcode": [$('#barcode').val(),'托盘条码为空，请检查！']
    };
    for (var key in list) {
        if (isNullOrEmpty(list[key][0])) {
            alert(list[key][1]);
            return;
        }
    }
    var param = {
        "ReceiptId": list.ReceiptId[0],
        "ItemId": list.ItemId[0],
        "ItemName": list.ItemName[0],  
        "Qty": list.Qty[0],
        "UomId": list.UomId[0],
        "ContainerCapacity": list.ContainerCapacity[0],
        "SupplierId": list.SupplierId[0],
        "Lot": list.Lot[0],
        "QCStatus": list.QCStatus[0],
        "ContainerBarcode": list.ContainerBarcode[0],
        "ItemLabel": $('#label_code').val(),
        "Spec": ReceivingIteminfo.spec,
        "StaUomId": ReceivingIteminfo.sta_uom_id,
        "Ratio": ReceivingIteminfo.rate
    };
    common.postGateway("wms/GroupContainer/Group", param, token, function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            alert('组盘成功！');
            $('#barcode').val('');
            common.postGateway("wms/GroupContainer/GetItemByItemId",
                { "receiptId": list.ReceiptId[0], "itemId": list.ItemId[0] },
                token, function (data) {
                if (data.errcode == 0 && data.errmsg == "OK") {
                    var result = JSON.parse(data.json);
                    var len = result.length;
                    if (len == 1) {
                        $("#item_finished_qty").val(result[0].finished_qty - result[0].processed_qty);
                        $("#item_qty").val(result[0].max_container_qty);
                    }
                }
            });
        }
    });
}

function receiptOnChange() {
    clear();
    getReceiptDetail();
}
function itemOnChange() {
    render();
}
function render() {
    var receiptId = $('#receipt_no option:checked').val();
    var itemId = $('#item_name option:checked').val();
    var param = { "receiptId": receiptId, "itemId": itemId };
    var token = common.getToken();
    if (isNullOrEmpty(receiptId) || isNullOrEmpty(itemId)) {
        return;
    }
    ReceivingIteminfo = null;
    common.postGateway("wms/GroupContainer/GetItemByItemId", param, token, function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            var result = JSON.parse(data.json);
            var len = result.length;
            if (len == 1) {
                ReceivingIteminfo = result[0];
                $("#item_code").val(result[0].item_code);
                $("#max_container_qty").val(result[0].max_container_qty);
                $("#item_uom_name").val(result[0].uom_name);
                $("#item_uom_name").attr('item_uom_id', result[0].uom_id);
                $("#item_finished_qty").val(result[0].finished_qty - result[0].processed_qty);
                $("#item_qty").val(result[0].max_container_qty);
                $("#item_lot").empty();
                $("#item_lot").append($("<option>").val(result[0].lot_no).text(result[0].lot_no));
                $("#item_supplier").empty();
                $("#item_supplier").append($("<option>").val(result[0].supplier_id).text(result[0].full_name));
            }
        }
    });
}
function clear() {
    $("#item_code").val('');
    $("#item_name").empty();
    $("#max_container_qty").val('');
    $("#item_uom_name").empty();
    $("#item_finished_qty").val('');
    $("#item_qty").val('');
    $("#item_lot").empty();
    $("#item_supplier").empty();
}
function isNullOrEmpty(item) {
    return (item == null || item == "");
}
function isPositiveInt(number) {
    var reg = /^[0-9]*[1-9][0-9]*$/;
    return reg.test(number);
}
