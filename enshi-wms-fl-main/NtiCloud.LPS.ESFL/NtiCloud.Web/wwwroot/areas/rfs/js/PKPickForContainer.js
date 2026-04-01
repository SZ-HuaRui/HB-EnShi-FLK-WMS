var curPage = 1;

$(function () {

    var orderCode = getUrlParam("orderCode");
    var locationCode = getUrlParam("ContainerCode");

    $("#outboundCode").val(orderCode);
    $("#ContainerCode").val(locationCode);

    GetOrderInfo(orderCode, locationCode, curPage,"");

    $("#pev").on("click", function () {
    curPage = curPage - 1;
        var orderCode = $("#outboundCode").val();
        var locationCode = $("#ContainerCode").val();

        GetOrderInfo(orderCode, locationCode, curPage,"pev");
    });

    $("#next").on("click", function () {
    curPage = curPage + 1;
        var orderCode = $("#outboundCode").val();
        var locationCode = $("#ContainerCode").val();
        GetOrderInfo(orderCode, locationCode, curPage,"next");
    });

    $("#submit").on("click", function () {
    SaveSubmit();
    });

});

function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) {
        return unescape(r[2]);
    }
    return null; //返回参数值
}

//扫描托盘码，绑定下拉框
function GetOrderInfo(orderCode, locationCode, page,pageType) {
    //传入model的货位编码-LocationCode、用户选择的单据-Order、页码-Page（默认为1）
    common.postGateway('wms/RFPickService/PK_ContainerPickInfo', {
        "ContainerCode": locationCode,
        "Order": orderCode,
        "Page": page
    }, common.getToken(), function (data) {
        if (data.isSucceed) {
            if (data.json != {} && data.json != null && data.json != "") {
                $("#t_taskCode").text(data.json.任务编号);
                $("#t_sourceCode").text(data.json.来源单号);
                $("#t_itemCode").text(data.json.物料编号);
                $("#t_itemName").text(data.json.物料名称);
                $("#t_lot").text(data.json.批次);
                $("#t_num").text(data.json.物料数量);
                $("#t_overPickNum").text(data.json.已拣);
                $("#t_needPickNum").text(data.json.应拣);
                $("#t_TaskDetailID").text(data.json.taskDetailID);
                $("#PickNum").val(parseInt(data.json.应拣) - parseInt(data.json.已拣));
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

//提交数据
function SaveSubmit() {
    /// TaskDetailID 任务明细ID
    /// PickNum      拣选数量
    /// NeedPick     本次应拣数量
    /// BNList       箱码(有箱码则填，无箱码不填)

    var TaskDetailID = $("#t_TaskDetailID").text();
    var PickNum = $("#PickNum").val();
    var NeedPick = $("#t_needPickNum").text();
    var ReadyPickNum = $("#t_overPickNum").text();
    var BNList = [];

    if (isNaN(parseInt(PickNum))) {
        alert("拣选数量不是数字");
        return;
    }

    if (PickNum.indexOf(".")>-1) {
        alert("拣选数量不是整数");
        return;
    }

    if (parseInt(PickNum) == 0) {
        alert("拣选数量不能为0");
        return;
    }

    if (parseInt(PickNum) < 0) {
        alert("拣选数量不能为负数");
        return;
    }

    common.postGateway('wms/RFPickService/PK_PickSubmit', {
    "TaskDetailID": TaskDetailID,
        "PickNum": PickNum,
        "NeedPick": NeedPick,
        "BNList": BNList,
        "ReadyPickNum": ReadyPickNum
    }, common.getToken(), function (data) {
            if (data.isSucceed) {
                alert("保存成功");
                var orderCode = $("#outboundCode").val();
                var locationCode = $("#ContainerCode").val();
                GetOrderInfo(orderCode, locationCode, curPage, "");
            }
            else {
                alert(data.errmsg);
            }
    });
}
