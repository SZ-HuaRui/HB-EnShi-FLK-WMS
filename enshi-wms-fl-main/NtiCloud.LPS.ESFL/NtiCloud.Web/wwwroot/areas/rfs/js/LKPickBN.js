var curPage = 1;
$(function () {

    $('#container_code').keydown(function (e) {
        if (e.keyCode == 13) {
            var barcode = $("#container_code").val();
            if (barcode != "") {
                ScanContainerCode(barcode);
            }
        }
    });

    //扫描箱码
    $("#snCode").keydown(function (e) {
        if (e.keyCode == 13) {
            var snCode = $("#snCode").val();
            if (snCode != "") {
    //增加箱码列表
    AddSnTable(snCode);
            }
        }
    });

    $("#pev").on("click", function () {
    curPage = curPage - 1;
        var orderCode = $("#orderCode").val();
        var container_code = $("#container_code").val();

        GetPickInfo(container_code,orderCode, curPage, "pev");
    });

    $("#next").on("click", function () {
    curPage = curPage + 1;
        var orderCode = $("#orderCode").val();
        var container_code = $("#container_code").val();
        GetPickInfo(container_code,orderCode, curPage, "next");
    });

    $("#submit").on("click", function () {
    SaveSubmit();
    });
});

//扫描托盘码，绑定下拉框
function ScanContainerCode(barcode) {
    var token = common.getToken();
    common.postGateway('wms/RFPickService/LK_ScanContainerCode', {
    "ContainerCode": barcode
    }, token, function (data) {
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
    GetPickInfo(barcode, curordercode, curPage);
                }
            }
            else {
    alert(data.errmsg);
            }
    });
}

//校验并增加箱码到列表
function AddSnTable(snCode) {
    common.postGateway('wms/RFPickService/BNInfo', {
        "BNStr": snCode
    }, common.getToken(), function (data) {
        if (data.isSucceed) {
            var curItemCode = $("#t_itemCode").text();
            if (data.json.item_code != curItemCode) {
                alert("当前箱码不存在该物料");
                $("#snCode").val("");
                return;
            }

            var needPickNum = parseInt($("#t_needPickNum").text());
            var alreadynum = parseInt($("#t_scanSumNum").text());

            if (alreadynum + parseInt(data.json.qty) > needPickNum) { //超出应收数量，需拆箱
                alert("超出应收数量，需拆箱");
                return;
            }
            else {
                $("#t_scanSumNum").text(parseInt(alreadynum) + parseInt(data.json.qty));
                var rownum = $("#bodydata").find("tr").length + 1;
                $("#bodydata").append("<tr><th>" + rownum + "</th><th class='bn'>" + data.json.item_sn + "</th><th>" + data.json.qty + "</th><th><span><a href='#' onclick='deleteRow(this," + data.json.qty + ")'>删除</a></span></th></tr >");
                $("#snCode").val("");
            }
        }
        else {
            alert(data.errmsg);
        }
    });
}


//填充拣选列表
function GetPickInfo(ContainerCode, Order, Page, pageType) {
    //托盘号 - ContainerCode、用户选择的单据 - Order、页码 - Page（默认为1）
    common.postGateway('wms/RFPickService/LK_ContainerPickInfo', { "ContainerCode": ContainerCode, "Order": Order, "Page": Page }, common.getToken(), function (data) {
        if (data.isSucceed) {
            if (data.json != {} && data.json != null && data.json != "") {
                $("#t_sourceCode").text(data.json.来源单号);
                $("#t_itemCode").text(data.json.物料编号);
                $("#t_itemName").text(data.json.物料名称);
                $("#t_lot").text(data.json.批次);
                $("#t_num").text(data.json.物料数量);
                $("#t_overPickNum").text(data.json.已拣);
                $("#t_needPickNum").text(data.json.应拣);
                $("#t_TaskDetailID").text(data.json.TaskDetailID);
            }
            else {
                if (pageType == "pev") {
                    curPage = curPage + 1;
                }
                if (pageType == "next") {
                    curPage = curPage - 1;
                }
                alert("没有相应数据");
            }
        }
        else {
            alert(data.errmsg);
        }
    });
}

//删除行
function deleteRow(row, delqty) {
    var alreadynum = parseInt($("#t_scanSumNum").text());
    $("#t_scanSumNum").text(parseInt(alreadynum) - parseInt(delqty));
    $(row).parent().parent().parent().remove();
}

//提交数据
function SaveSubmit() {
    /// TaskDetailID 任务明细ID
    /// PickType     拣选模式  (1为默认，2为交换托盘)
    /// OrderContainerCode  空托盘条码
    /// PickNum      拣选数量
    /// NeedPick     本次应拣数量
    /// BNList       箱码(有箱码则填，无箱码不填)

    var TaskDetailID = $("#t_TaskDetailID").text();
    var PickType = $("#isChangeCon").prop("checked") == false ? "1" : "2";
    var OrderContainerCode = $("#container_empcode").val();
    var PickNum = $("#t_scanSumNum").text();
    var NeedPick = $("#t_needPickNum").text();
    var BNList = [];

    $("#bodydata").find("tr").each(function (i, n) {
        var BN = $(n).find(".bn").text();
        BNList.push(BN);
    });

    if (OrderContainerCode == "") {
    alert("未指定目标容器");
        return;
    }

    if (parseInt(PickNum) == 0) {
    alert("未拣选任务物料");
        return;
    }

    common.postGateway('wms/RFPickService/LK_PickSubmit', {
    "TaskDetailID": TaskDetailID,
        "PickType": PickType,
        "OrderContainerCode": OrderContainerCode,
        "PickNum": PickNum,
        "NeedPick": NeedPick,
        "BNList": BNList
    }, common.getToken(), function (data) {
        if (data.isSucceed) {
    alert("保存成功");
        }
        else {
    alert(data.errmsg);
        }
    });
}

