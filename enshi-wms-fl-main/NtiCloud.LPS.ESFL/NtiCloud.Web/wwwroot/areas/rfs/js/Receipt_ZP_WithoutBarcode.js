$(function () {
    init()
});
//初始化
function init() {
    //加载牌号
    common.postGateway("wms/GroupContainer/GetProductList", {}, common.getToken(), function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            var result = JSON.parse(data.json);
            var len = result.length;
            var brandHtml = '';
            var isContinue = false;
            if (len >= 1) {
                for (var i = 0; i < len; i++) {
                    brandHtml += '<option value="' + result[i].item_id + '">' + result[i].item_name + '</option>';
                }
                isContinue = true;
            }
            if (isContinue) {
                $('.brand').append(brandHtml);
            } else {
                alert("加载牌号失败！");
            }
        }
    });
    $(".station").focus();
    $(".quantity").val('30');
    $(".station").keydown(function (e) {
        if (e.which == 13) {
            $(".quantity").focus();
        }
    });
    $(".quantity").keydown(function (e) {
        if (e.which == 13) {
            if (!validate($(this).val())) {
                alert("请输入正确的数量！");
                $(this).val('').focus();
                return;
            }
            else {
                $(".pallet_barcode").focus();
            }
        }
    });
}
function commit_btn() {
    var token = common.getToken();
    var station = $(".station").val();
    var pallet_barcode = $(".pallet_barcode").val();
    var brand_name = $('.brand').find("option:selected").text();
    var qty = $('.quantity').val();
    if (isNullOrEmpty(station)
        || isNullOrEmpty(pallet_barcode)
        || isNullOrEmpty(brand_name)) {
        alert("页面存在空值，请检查！");
        return;
    }
    if (!validate(qty)) {
        alert("请输入正确的数量！");
        return;
    }
    var item_id = $('.brand').val();
    var param = {
        "DockNumber": station,
        "ContainerBarcode": pallet_barcode,
        "ItemId": item_id,
        "Qty": parseFloat(qty)
    };
    common.postGateway("wms/GroupContainer/GroupProduct", param, token, function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            alert('组盘成功！');
            $('.pallet_barcode').val('').focus;
            $(".quantity").val('30');
        }
    });
}
function validate(num) {
    var reg = /^\d+(?=\.{0,1}\d+$|$)/;
    if (reg.test(num)) return true;
    return false;
}
function isNullOrEmpty(i) {
    if (i == 'undefined' || !i || !/[^\s]/.test(i)) {
        return true;
    } else {
        return false;
    }
}
