$(function () { 
    init();
})
function init() {
    //getReceiptNo();
    //$("#receipt_no").change(function () { receiptOnChange() });
    //$("#item_name").change(function () { itemOnChange()});

    $("#btnSubmit").bind("click", function () {
        addItemReplacement();
    });

    $('.item_barcode').bind('keypress', function (event) {
        if (event.keyCode == "13") {
            if ($('.item_barcode').val() == '') {
                layer.msg('请先扫烟包条码');
                return;
            }
            if ($("txtItemBarcode").val() != '') {
                queryQcDetail();
            }
        }
    });
}

function addItemReplacement() {
    if ($(".item_barcode").val() == null || $(".item_barcode").val() == '') {
        layer.msg('请先扫托盘条码');
        return;
    }
    var formJson = {
        ItemBarcode: $(".item_barcode").val(),
        Reason: $("#selReason").val(),
        AbnormalWeight: $("#txtAbnormalWeight").val()
    };
    common.postGateway("wms/WorkTask/AddItemReplacement", formJson, common.getToken(), function (result) {
        if (result.resMsg && result.resMsg != "") {
            $.messageBoxBottom(0, result.resMsg);
        } else if (result.errcode != 0) {
            $.messageBoxBottom(0, result.errmsg);
        } else {
            var msg = "";
            if ((result.resMsg && result.resCount && result.resMsg == "")) {
                msg = "共成功操作" + result.resCount + "条记录！";
                $.messageBoxBottom(1, msg);
            } else {
                $("#qc_no").val(result.json.qcNo)
                msg = "入库调用成功！";
                $.messageBoxBottom(1, msg);
            }
        }
    });
}

function queryQcDetail() {
    if ($(".item_barcode").val() == null || $(".item_barcode").val() == '') {
        layer.msg('请先扫托盘条码');
        return;
    }
    var formJson = {
        barcode: $(".item_barcode").val()
    };

    apiRoute.selectDataEvent(
        ["select cd.* from wms_b_container_d cd left join wms_b_container_h ch on cd.container_id = ch.t_id where cd.client_id='&CLIENT_ID&' and cd.org_id='&ORG_ID&' and cd.warehouse_id='&WAREHOUSE_ID&' and ch.container_barcode = @item_barcode "], 
        [{ item_barcode : $(".item_barcode").val() }], "",
        function (data) {
            if (data == null) {
                $.messageBoxBottom(2, "系统出错，无法取得返回值！");
                return;
            }
            if (data.length == 1 && data[0].error) {
                $.messageBoxBottom(2, data[0].error);
                return;
            }

            $('#txtItemName').val(data[0].item_name);
            $('#txtPrepLotNo').val(data[0].lot_no);
            $('#txtProdLotNo').val(data[0].owner_lot_no);
            $('#txtWeight').val(data[0].stat_qty);
            $('txtAbnormalWeight').focus();
        });
}