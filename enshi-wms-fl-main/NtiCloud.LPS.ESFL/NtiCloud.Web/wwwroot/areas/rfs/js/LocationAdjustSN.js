/**
 * RF货位调整JS:有码
 * */
var adjustInfo = {};
var itemInfo = {};

$(function () {
    init();
})
function init() {
    var token = common.getToken();
    if (!token) {
        alert('请重新登陆！');
        return;
    }
    token.clientId = '1010203040506070809';
    token.appCode = 'WMS';
    getAdjust();
    $("#txtAdjustNo").change(function () { OutAdjustNoChange() }); 
    $("#txtItemName").change(function () { ItemOnChange() });
    
}
function OutAdjustNoChange() {
    var adjustno = $('#txtAdjustNo option:checked').val();
    if (adjustInfo.hasOwnProperty(adjustno)) {
        render('adjustNo', adjustInfo[adjustno]);        

    } 
}

function ItemOnChange() {
    var itemcode = $('#txtItemName option:checked').val();
    if (itemInfo.hasOwnProperty(itemcode)) {
        render('itemcode', itemInfo[itemcode]);

    }
}

function render(type, obj) {
    switch (type) {
        case 'adjustNo':
            renderAdjustNo(obj)
            break;
        case 'itemcode':
            renderItem(obj)
            break;
        default:
    }
}

function renderAdjustNo(adjustnoArray) {
    $('#txtAdjustNo').html('');
    var selectHtml = '';
    for (var i = 0; i < adjustnoArray.length; i++) {
        selectHtml += '<option value="' + adjustnoArray[i] + '"></option>';
    }
    $('#txtAdjustNo').append(selectHtml);
}

function renderItem(itemArray) {
    $('#txtItemName').html('');
    var selectHtml = '';
    for (var i = 0; i < itemArray.length; i++) {
        selectHtml += '<option value="' + itemArray[i] + '">' + itemInfo[itemArray[i]].itemname + '</option>';
    }
    $('#txtItemName').append(selectHtml);
}

//获取库位调整单号
function getAdjustno() {
    var token = common.getToken();
    if (!token) {
        alert('请重新登陆！');
        return;
    }
    token.clientId = '1010203040506070809';
    token.appCode = 'WMS';
    common.postGateway("wms/LocationAdjust/GetAdjustNo", {
            }, token, function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            var result = JSON.parse(data.json);
            var len = result.length;
            var selectHtml = '';
            var isContinue = false;
            if (len >= 1) {
                for (var i = 0; i < len; i++) {
                    selectHtml += '<option value="' + result[i].adjust_no + '"></option>';
                }
                isContinue = true;
            }
            if (isContinue) {
                $('#txtAdjustNo').append(selectHtml);
                var itemId = $('#txtAdjustNo option:checked').val();
                
            }
        }
    });
}
function confirm() {
    var Outlocationno = $('#txtToOutLocation').val();
    if (!Outlocationno) {
        alert('请输入移出库位条码！');
        return;
    }
    var OutContainerno = $('#txtOutTray').val();
    if (!OutContainerno) {
        alert('请扫描托盘条码！');
        return;
    }
 
    var Inlocationno = $('#txtInLocation').val();
    if (!Inlocationno) {
        alert('请扫描移入库位条码！');
        return;
    }
    var InContainerno = $('#txtToInContainer').val();
    if (!InContainerno) {
        alert('请扫描移入容器条码！');
        return;
    }

    var len = $('#bom_table tr').length;
    if (len <= 1) {
        alert('该移入库位下无物料条码！');
        return;
    }
   
    //获取数据
    var token = common.getToken();
    if (!token) {
        alert('请重新登陆！');
        return;
    }
    token.clientId = '1010203040506070809';
    token.appCode = 'WMS';
    var json = getJsonFromTable();
    var param = {
        "AdjustNo": adjustno,
        "ContainerBarcode": barcode,
        ContainerDetailList: json
    };
    common.postGateway("wms/LocationAdjust/Compose", param, token, function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            alert('配盘成功！');
            $('#barcode').val('');
            $('#barcode').focus();
        } else {
            alert(data.errmsg);
        }
    });
}
