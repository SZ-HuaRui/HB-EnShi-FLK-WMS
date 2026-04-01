$(function () {
    var table = layui.table;
    table.init('tb_qc_d', {

    });
    $('.container_code').bind('keypress', function (event) {
        if (event.keyCode == "13") {
            if ($('.container_code').val() == '') {
                layer.msg('请先扫托盘条码');
                return;
            }
            if ($("qc_no").val() != '') {
                queryQcDetail();
            }
        }
    });

    $(".btn_inbound").bind("click", function () {
        addQcTask();
    })

    $(".btn_run").bind("click", function () {
        runQcTask();
    })
})

function addQcTask() {
    if ($(".container_code").val() == null || $(".container_code").val() == '') {
        layer.msg('请先扫托盘条码');
        return;
    }
    var formJson = {
        containerCode: $(".container_code").val(),
        items: layui.table.cache["tb_qc_d"]
    };
    common.postGateway("wms/QcBound/StartInQcTaskWithItem", formJson, common.getToken(), function (result) {
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

function runQcTask() {
    if ($(".container_code").val() == null || $(".container_code").val() == '') {
        layer.msg('请先扫托盘条码');
        return;
    }
    var formJson = {
        barcode: $(".container_code").val()
    };
    common.postGateway("wms/QcBound/RunInQcTaskByWcs", formJson, common.getToken(), function (result) {
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
                msg = "调用成功！";
                $.messageBoxBottom(1, msg);
            }
        }
    });
}

function queryQcDetail() {
    if ($(".container_code").val() == null || $(".container_code").val() == '') {
        layer.msg('请先扫托盘条码');
        return;
    }
    var formJson = {
        barcode: $(".container_code").val()
    };
    common.postGateway("wms/QcBound/GetQcInfo", formJson, common.getToken(), function (result) {
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
                var qcDetail = result.json.qcDetail;
                var table = layui.table;
                // var tabledata = table.cache["tb_qc_d"]; //获取现有数据
                var tabledata = [];
                for (var i = 0; i < qcDetail.length; i++) {
                    tabledata.push({
                        "id": qcDetail[i].id
                        , "item": qcDetail[i].itemName
                        , "qty": qcDetail[i].qty
                        , "statQty": qcDetail[i].statQty
                    })
                }
                table.reload("tb_qc_d", {
                    data: tabledata,
                })

            }
        }
    });
}