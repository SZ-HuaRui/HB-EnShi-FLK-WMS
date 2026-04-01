$(function () {
    init()
});

function init() {
    $(".station").focus();
    $(".station").keydown(function (e) {
        if (e.which == 13) {
            $(".pallet_barcode").focus();
        }
    });
    $(".pallet_barcode").keydown(function (e) {
        if (e.which == 13) {
            $(".item_barcode").focus();
        }
    });
    $(".item_barcode").keydown(function (e) {
        if (e.which == 13) {
            addBarcodeToContainer($(".item_barcode").val())
        }
    });
}
function commit_btn() {
    var token = common.getToken();
    var station = $(".station").val();
    var pallet_barcode = $(".pallet_barcode").val();
    if (isNullOrEmpty(station) || isNullOrEmpty(pallet_barcode)) {
        alert("组盘站台或托盘条码为空！");
        return;
    }
    var barcodes = getAllContainerBarcode(0, 32);
    if (barcodes.length == 0) {
        alert("请扫描件烟条码！");
        return;
    }
    var param = {
        "DockNumber": station,
        "ContainerBarcode": pallet_barcode,
        "BarcodeList": barcodes
    };
    common.postGateway("wms/GroupContainer/GroupProduct", param, token, function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            alert('组盘成功！');
            $('.pallet_barcode').val('').focus;
            $('.item_barcode').val('');
            $('.container').html('');
            $(".qty").html('0');
        }
    });
}
function addBarcodeToContainer(barcode) {
    //验证32位数字
    var reg = /^\d{32}$/;
    if (!reg.test(barcode)) {
        alert('请扫描32位件烟条码！');
        $(".item_barcode").val('').focus();
        return;
    }
    var item_code = barcode.substring(2, 5);
    //验证货品是否相同
    if (!itemCodeIsSame(item_code,2,5)) {
        alert("该条码解析出的货品和其他不同，请重新扫描！");
        $(".item_barcode").val('').focus();
        return;
    }
    var barcodes = getAllContainerBarcode(0, 32);
    if (barcodes.length >= 30) {
        alert("扫描件数超过30条！");
        return;
    }
    //存在重复的barcode
    if (barcodes.indexOf(barcode) != -1) {
        var isDelete = confirm("存在重复条码，是否删除该条码！");
        if (isDelete) {
            removeBarcode(barcode);
            updateQty();
        }
        $(".item_barcode").val('').focus();
        return;
    }
    //添加barcode到container
    $(".container").append('<p>' + barcode + '</p>');
    $(".item_barcode").val('').focus();
    updateQty();
}

function updateQty() {
    var qty = $(".container p").length;
    $(".qty").html(qty);
}
function getAllContainerBarcode(start, end) {
    var barcodes = [];
    $(".container p").each(function () {
        barcodes.push($(this).text().substring(start, end));
    });
    return barcodes;
}
function removeBarcode(barcode) {
    $(".container p").each(function () {
        if (barcode == $(this).text()) {
            $(this).remove();
        }
    });
}
function itemCodeIsSame(item,start,end) {
    var items = getAllContainerBarcode(start, end);
    for (var i = 0; i < items.length; i++) {
        if (item != items[i]) {
            return false;
        }
    }
    return true;
}
function isNullOrEmpty(i) {
    if (i == 'undefined' || !i || !/[^\s]/.test(i)) {
        return true;
    } else {
        return false;
    }
}