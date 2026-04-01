/**
 * RF上架任务执行JS:无码
 * */
var TASKS = [];//任务列表
var taskObj = {};//当前任务
var SNList = [];//扫描的物料条码
var showTaskIndex = 0;//显示第几个任务

$(function () {    
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
                    ClearData();
                    GetTaskDetails($("#txtItemCode").val());
                    $("#txtInQty").val("");
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
//任务体骄傲后清空界面
function ClearData() {
    showTaskIndex = 0;
    TASKS = [];
    taskObj = {};
    SNList = [];
    $("#txtItemName").text("");
    $("#txtToLocation").text("");
    $("#txtPutawayNo").text("");
    $("#txtTotalQty").text("");
    $("#txtWaitingProcessQty").text(""); 
    $("#lbtaskIndo").text("");
    $("#txtInQty").val("")
}
  
 
