var FormData = {};
var PutawayDetails = [];
var ShowTaskIndex = 0;//显示第几个任务
$(function () {
    layui.use(['laydate'], function () {
        layui.laydate.render({
            elem: '#txtProduceDate'
        })
    });
    $('#txtPutawayNo').focus();

    $("#btnNext").bind("click", doNext);
    $("#btnPriority").bind("click", doPriority);

    $("#txtPutawayNo").bind("keypress", function (event) {
        if (event.keyCode == 13 && $('#txtPutawayNo').val() != "") {
            GetPutawayInfo();
            $("#txtToLocation").focus();
        }
    });

    $("#txtToLocation").bind("keypress", function (event) {
        if (event.keyCode == 13 && $('#txtToLocation').val() != "") {
            //$("#txtItemCode").focus();
            $("#txtLotno").focus();
        }
    });

    $("#txtItemCode").bind("keypress", function (event) {
        /*if (event.keyCode == 13 && $('#txtItemCode').val() != "") {
            GetPutawayInfo();
            $("#txtLotno").focus();
        }*/
    });
    $("#txtLotno").bind("keypress", function (event) {
        if (event.keyCode == 13) {
            $("#txtProduceDate").focus();
        }
    });
    $("#txtProduceDate").bind("keypress", function (event) {
        if (event.keyCode == 13) {
            $("#txtInQty").focus();
        }
    });

});

//查询上架单信息
function GetPutawayInfo() {
    var putawayno = $("#txtPutawayNo").val();
    var itemcode = $("#txtItemCode").val();
    if (putawayno !== "") {
        clearData();
        var token = common.getToken();
        common.postGateway('wms/PutawayService/GetPutawayItemListForDoing', {
            "PutawayNo": putawayno,
            "ItemCode": itemcode
        }, token, function (data) {
            if (data.isSucceed) {
                var result = data.json;
                console.log(result);
                var len = result.length;
                if (len > 0) {
                    PutawayDetails = result;
                    ShowTaskIndex = 0;
                    showDetail();
                }
                else {
                    alert(data.errmsg);
                }
            }
            else {
                alert(data.errmsg);
            }
        });

    } else {
        if (putawayno === "") {
            $("#txtPutawayNo").focus();
            return;
        }
        if (itemcode === "") {
            $("#txtItemCode").focus();
            return;
        }
    }
}
//下一笔上架单信息
function doNext() {
    if (ShowTaskIndex < PutawayDetails.length - 1) {
        ShowTaskIndex = ShowTaskIndex + 1;
        showDetail();
    }
}
//上一笔上架单信息
function doPriority() {
    if (ShowTaskIndex > 0) {
        ShowTaskIndex = ShowTaskIndex - 1;
        showDetail();
    }
}
//显示数据
function showDetail() {
    FormData = jQuery.extend({}, PutawayDetails[ShowTaskIndex]);
    $("#txtItemCode").val(FormData.item_code);
    $("#txtItemName").text(FormData.item_name);
    $("#txtLotno").val(FormData.lot_no);
    $("#txtProduceDate").val(FormData.production_date_str);
    $("#txtProcessQty").text(FormData.processed_qty);
    $("#txtInQty").val(FormData.plan_qty - FormData.processed_qty);
    $("#lbtaskIndo").text((ShowTaskIndex + 1) + "/" + PutawayDetails.length);

    $("#txtToLocation").val("");
}
//清空数据
function clearData() {
    PutawayDetails = [];
    ShowTaskIndex = 0;
    FormData = {};

    $("#txtItemCode").val("");
    $("#txtItemName").val("");
    $("#txtLotno").val("");
    $("#txtProduceDate").val("");
    $("#txtProcessQty").text("");
    $("#txtInQty").val("");
    $("#lbtaskIndo").text("");
    $("#txtToLocation").val("");
}
//提交上架信息
function DoSubmit() {
    var putawayno = $("#txtPutawayNo").val();
    var InQty = $("#txtInQty").val();
    var reg = /^([0-9]+\\.?)?[0-9]+$/;
    if (!reg.test(InQty)) {
        alert("请输入正确的上架量");
        return;
    }
    if (!FormData.t_id) {
        alert("请先获取到上架信息");
        return;
    }
    FormData.production_date = $("#txtProduceDate").val();
    FormData.lot_no = $("#txtLotno").val();
    FormData.to_location = $("#txtToLocation").val();
    if (Number(InQty) == 0) {
        alert("上架量必须大于0");
        return;
    }
    if (FormData.to_location == "") {
        alert("上架货位不能为空");
        return;
    }

    if (putawayno !== "") {
        var token = common.getToken();
        common.postGateway('wms/PutawayService/SubmitPutawayWithoutTask', {
            "PutawayNo": putawayno,
            "PutawayId": FormData.inbound_id,
            "ItemCode": FormData.item_code,
            "PutawayDetailId": FormData.t_id,
            "ProduceDate": FormData.production_date,
            "LotNo": FormData.lot_no,
            "Qty": InQty,
            "ToLocation": FormData.to_location,
        }, token, function (data) {
            if (data.isSucceed) {
                if (data.errmsg === "整单完成") {
                    clearData();
                    $("#txtPutawayNo").val("");
                    $("#txtPutawayNo").focus();
                }
                else {
                    GetPutawayInfo();
                    $("#txtToLocation").focus();
                }
            }
            else {
                alert(data.errmsg);
            }
        }
        );

    } else {
        if (putawayno === "") {
            $("#txtPutawayNo").focus();
            return;
        }
    }
}