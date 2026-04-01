
$(function () {
    //绑定出库单下拉框
    GetAllPickOrder();

    //更改下拉框单据
    $('#orderCode').change(function (e) {
        var curordercode = $("#orderCode").val();
        $("#locationCode").val("");
        //使用选择的单据，填充拣选列表
        GetPickInfo(curordercode);
    });

    //扫描货位
    $('#locationCode').keydown(function (e) {
        if (e.keyCode == 13) {
            var locationcode = $("#locationCode").val();
            if (locationcode != "") {
    //突出背景
    GetLocationPickInfo(locationcode);
            }
        }
    });

    //下一步按钮
    $("#submit").on("click", function () {
        var ordercode = $("#orderCode").val();
        var locationcode = $("#locationCode").val();
        NextBtn(ordercode, locationcode);
    });

});

//绑定出库单下拉框
function GetAllPickOrder() {
    var token = common.getToken();
    common.postGateway('wms/RFPickService/PK_GetOutBoundInfo', {}, token, function (data) {
        if (data.isSucceed) {
            var curordercode = "";
            $.each(data.json, function (i, n) {
    $("#orderCode").append("<option value='" + n + "'>" + n + "</option>");
                if (i == 0) {
    curordercode = n;
                }
            });

            //使用第一个单据填充拣选列表
            if (curordercode != "") {
    GetPickInfo(curordercode);
            }
        }
        else {
    alert(data.errmsg);
        }
    });
}

//填充拣选列表
function GetPickInfo(ordercode) {
    common.postGateway('wms/RFPickService/PK_PickLocationInfo', { "Order": ordercode }, common.getToken(), function (data) {
        $("#databody").empty();
        if (data.isSucceed) {
            $.each(data.json, function (i, n) {
                $("#databody").append("<tr><th>" + n.task_no + "</th><th class='LocationClass'>" + n.locationcode + "</th><th>" + n.qtysum + "</th></tr>");
            });
        }
        else {
            alert(data.errmsg);
        }
    });
}

//扫描货位后突出背景，显示对应的拣选数据
function GetLocationPickInfo(locationcode) {
    $("#databody tr").each(function (i, n) {
        var curText = $(n).find(".LocationClass").text();
        if (curText == locationcode) {
            $(n).css("background-color", "#FF0000");
        }
        else {
            $(n).css("background-color", "#FFFFFF");
        }
    });
}

//下一步按钮
function NextBtn(orderCode, locationCode) {
    if (locationCode == "") {
    alert("请先扫描货位");
        return;
    }
    location.href = "PKPickBN?orderCode=" + orderCode + "&locationCode=" + locationCode;
}
