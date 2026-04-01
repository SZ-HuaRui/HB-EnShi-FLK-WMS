/**
 * RF上架任务执行JS，包括有码，无码两个界面
 * */
var TASKS = [];//任务列表
var taskObj = {};//当前任务
var SNList = [];//扫描的物料条码
var showTaskIndex = 0;//显示第几个任务

$(function () {   
    layui.use('table', function () {
        var table = layui.table;
        table.init("tbSNLits", { 
            height:190,
            cols: [
                [{
                    field: "SN", title: "物料条码"
                }, {
                        templet: function (d) { return "<a href='#' onclick='doDeleteSn(" + d.LAY_TABLE_INDEX+")'  lay-event='del' >删除</a>"; }
                }
                ]
            ],
            data: SNList,
            even: true
        });
    }); 
    //下一项
    $("#btnNext").bind("click", function () {
        if (showTaskIndex < TASKS.length - 1) {
            showTaskIndex = showTaskIndex + 1;
        } else {
            showTaskIndex = TASKS.length - 1;
        }
        showtaskdetail();
        doShowSnTable();
        console.log("btnNext");
    }); 
    //上一项
    $("#btnPriority").bind("click", function () {
        if (showTaskIndex >0) {
            showTaskIndex = showTaskIndex - 1;
        } else {
            showTaskIndex = 0;
        }
        showtaskdetail();
        doShowSnTable();
        console.log("btnPriority");
    });
    //扫描物料编码
    $("#txtItemCode").bind("keypress", function (event) {
        if (event.keyCode == 13 && $('#txtorderno').val() != "") {
            GetTaskDetails($('#txtItemCode').val());
        }
    }); 
    //扫描SN箱码
    $("#txtItemSn").bind("keypress", function (event) {
        if (event.keyCode == 13 && $('#txtItemSn').val() != "") {
            AddSN($('#txtItemSn').val());
            $("#txtItemSn").val(""); 
        }
    });
    //提交上架(有码)
    $("#btnSubmit").bind("click", function () {
        debugger;
        if (SNList.length == 0) {
            return;
        }
        if (SNList.length < taskObj.plan_qty) {
            if (!confirm("确认部分上架?")) { 
                return;
            }
        }
        var token = common.getToken();

        common.postGateway('wms/PutawayService/DoPutawayToLocation', { 
            "TaskId": taskObj.task_id,
            "TaskItemId": taskObj.t_id,
            "ToLocationCode": taskObj.to_location,
            "InQty": SNList.length,
            "LstSN": SNList,
        }, token, function (data) {
                if (data.isSucceed) {
                    GetTaskDetails($("#txtItemCode").val());
                }
        });

    })

    //提交上架(无码)
    $("#btnSubmitWithoutSn").bind("click", function () {
        debugger;
        var thisQty = $("#txtInQty").val();
        var reg = /^[0-9]*$/;
        if (!reg.test(thisQty)) {
            alert("上架数量必须为合法数字");
            return;
        }
         
        var token = common.getToken();

        common.postGateway('wms/PutawayService/DoPutawayToLocation', {
            "TaskId": taskObj.task_id,
            "TaskItemId": taskObj.t_id,
            "ToLocationCode": taskObj.to_location,
            "InQty": thisQty,
            "LstSN": [],
        }, token, function (data) {
            if (data.isSucceed) {
                var result = data.json;
                console.log(result);
                var len = result.length;
                if (len > 0) {
                    TASKS = result;
                    showTaskIndex = 0;
                    showtaskdetail();
                    $("#txtItemSn").focus();
                }
                else {
                    alert(data.errmsg);
                }
            }
            else {
                alert(data.errmsg);
            }
        });

    })


    $("#txtItemCode").focus();

})

///扫描物料编码，加载任务信息
function GetTaskDetails(itemcode) {
    TASKS = [];
    taskObj = {};
    var token = common.getToken();
    common.postGateway('wms/PutawayService/GetTaskListFordoing_RF', {
        "WarehouseId":"1016319470013779968",
        "ItemCode": itemcode
    }, token, function (data) {
            if (data.isSucceed) {
                var result = data.json;
                console.log(result);
                var len = result.length;
                if (len > 0) {
                    TASKS = result;
                    showTaskIndex = 0;
                    showtaskdetail();
                    if ($("#txtItemSn").length > 0) {
                        $("#txtItemSn").focus();
                    }
                    else {
                        $("#txtInQty").focus();
                    }
                }
                else {
                    alert(data.errmsg);
                }
            }
            else {
                alert(data.errmsg);
            }
    });

}
//显示单个任务的详细信息
function showtaskdetail() {
    taskObj = TASKS[showTaskIndex];
    SNList = [];
    $("#txtItemName").text(taskObj.item_name);
    $("#txtToLocation").text(taskObj.to_location);
    $("#txtPutawayNo").text(taskObj.ref_doc_no);
    $("#txtTotalQty").text(taskObj.plan_qty);
    $("#txtWaitingProcessQty").text(taskObj.waiting_qty); 

    $("#lbtaskIndo").text((showTaskIndex + 1) + "/" + TASKS.length);
}
 
//将扫描ode条码添加到列表中
function AddSN(sn) {
    for (var i = 0; i < SNList.length;i++) {
        if (SNList[i].SN == sn) {
            alert("此条码已扫描,不能再次扫描");
            return;
        }
    }
    debugger;
    SNList.push({ "SN": sn, "Qty": 1 });  
    doShowSnTable();
    $("#txtInQty").text(SNList.length);
}
function doDeleteSn(snindex) { 
    if (confirm("确认删除此条码？")) {
        SNList.splice(snindex, 1);
        doShowSnTable();
        $("#txtInQty").text(SNList.length);
    }
}

function doShowSnTable() {
    layui.use('table', function () {
        var table = layui.table;
        table.reload("tbSNLits",{ 
            height: 100,
            limit: Number.MAX_VALUE,
            data: SNList
        });
    });
}
 
