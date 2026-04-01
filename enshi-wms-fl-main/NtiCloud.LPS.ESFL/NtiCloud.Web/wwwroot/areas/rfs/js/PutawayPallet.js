var PalletInfo = {};
var Submitting = false;
$(function () {

    $('#txtPalletNo').focus();

    $("#txtPalletNo").bind("keypress", function (event) {
        if (event.keyCode == 13 && $('#txtPalletNo').val() != "") {
            GetPalletInfo();
        }
    });

    $("#txtToLocation").bind("keypress", function (event) {
        if (event.keyCode == 13 && $('#txtToLocation').val() != "" && $('#txtPalletNo').val() != "") {
            DoSubmit();
        }
    });

    $("#btnSubmitWithoutSn").bind("click", function () { 
            DoSubmit(); 
    });

    BindItemDetail([]);

    $("#btnSubmitWithoutSn").css("padding", "10px");
});
//显示物料明细
function BindItemDetail(items) {
    layui.use('table', function () {
        var table = layui.table;
        table.init("tbCartonDetails", {
            height: 100,
            cols: [
                [
                    { field: "item_name", title: "物料编码" },
                    { field: "lot_no", title: "批次" },
                    { field: "qty", title: "数量" }
                ]
            ],
            data: items,
            even: true
        });
    });
}
//查询上架单信息
function GetPalletInfo() {
    var palletNo = $("#txtPalletNo").val();
    if (palletNo !== "") {
        $.messageLoad();
        var token = common.getToken();
        common.postGateway('wms/PutawayService/GetPalletInfoForIn', {
            "containerNo": palletNo
        }, token, function (data) {
                $.messageBoxCloseLoading();
            if (data.isSucceed) {
                PalletInfo = data.json;
                $("#txtSuggestLocation").text(PalletInfo.suggestLocatio);
                BindItemDetail(PalletInfo.details);
                $("#txtToLocation").val("");
                $("#txtToLocation").focus();
            }
            else {
                alert(data.errmsg);
            }
        });

    }
}
//提交
function DoSubmit() {
    if (Submitting) {
        return;
    }
    Submitting = true;
    var palletNo = $("#txtPalletNo").val();
    var locationcode = $("#txtToLocation").val();
    debugger;
    if (palletNo !== "" && locationcode !== "" && PalletInfo.containerNo == palletNo) {
        $.messageLoad();
        Submitting = false;
        var token = common.getToken();
        common.postGateway('wms/PutawayService/PutawayPalletToLocation', {
            "PalletNo": palletNo,
            "containerId": PalletInfo.containerId,
            "toLocation": locationcode
        }, token, function (data) {
            $.messageBoxCloseLoading();
            if (data.isSucceed) {
                ClearForm();
                $("#txtPalletNo").focus();
            }
            else {
                alert(data.errmsg);
            }
        });

    }
}
//清空界面
function ClearForm() {
    $("#txtPalletNo").val("");
    $("#txtSuggestLocation").text("");
    $("#txtToLocation").val("");
    BindItemDetail([]);
}