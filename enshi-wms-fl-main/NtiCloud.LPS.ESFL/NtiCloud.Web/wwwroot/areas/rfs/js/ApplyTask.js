/**
 * 模拟WCS申请入库任务
 * */
 

$(function () {  

        
    $("#btnSubmit").bind("click",function(){
            doSubmit();
    })   

})

function doSubmit() {     
    common.postGateway('wms/WorkTask/ApplyInTask', { 
            "WarehouseCode": "WMS",
            "ApplyType": $("#ddlApplyType").val(),
            "ContainerNo":  $("#txtContainerNo").val(), 
            "ApplyStationCode": $("#txtStationCode").val()
    }, common.getToken(), function (data) {
                if (data.isSucceed) {
                     alert(data.json.task_no);
                }
        });
} 