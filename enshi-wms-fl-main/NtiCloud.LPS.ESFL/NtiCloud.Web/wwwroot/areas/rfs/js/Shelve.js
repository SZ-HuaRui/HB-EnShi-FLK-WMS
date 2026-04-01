$(function () {
    $("#locationCode").focus();
    $(".getWork").on('click', function () {
        getWork();
    })
    $(".conform_btn").on('click', function () {
        putOnConfirm();
    })      
})
function getWork() {//获取任务按钮
    var getWorkSql = "select c.t_id,h.receiving_number receipt_no,i.item_code,c.container_id,i.item_name,c.receipt_qty qty from wms.swms_b_receiving_container c join wms.swms_b_receiving_h h on c.receiving_id = h.t_id join wms.swms_wi_item i on c.item_id = i.t_id  where c.warehouse_id = '1016319470013779968' and c.state_code = 'NotExecute'";
    var token = common.getToken();
    common.postGateway('platform/database/selectsql', {
        "sql": common.Encrypt(getWorkSql),
        "parameters": []
    }, token, function (data) {
            if (data.errcode == 0 && data.errmsg == "OK") {
                var result = JSON.parse(data.json);
                var len = result.length;
                if (len == 1) {
                    $("#receipt_no").text(result[0].receipt_no);
                    $("#container_id").text(result[0].container_id);
                    $("#item_name").text(result[0].item_name);
                    $("#item_code").text(result[0].item_code);
                    $("#receipt_qty").text(result[0].qty);
                    $("#t_id").text(result[0].t_id);
                }
                else {
                    alert("获取任务失败！");
                }
            }
            else {
                alert("获取任务失败！");
            }
    });
}
function putOnConfirm() {//确定按钮
    var locationCode = $("#locationCode").val();
    if (locationCode == null || locationCode == "") {
        alert("货位条码不能为空！！！");
        return;
    }
    if (locationCode == "0001") {
        alert("请选择入暂存区！！！");
        return;
    }
    var tId = $("#t_id").text();
    var item = $("#item_code").text();
    if (item == null || item == "") {
        alert("物料编码不能为空！！！");
        return;
    }
    common.postGateway('cpfk/inbound/putawayconfirm', {
        "receiptContainerTid": tId,
        "location": locationCode
    }, common.getToken(), function (data) {
        if (data.errcode == 0) {
            $("#receipt_no").text('');
            $("#container_id").text('');
            $("#item_name").text('');
            $("#item_code").text('');
            $("#receipt_qty").text('');
            $("#t_id").text('');
            $("#locationCode").val('');
            location.href = "/RFS/Main/Receive";
        }
        else {
            alert(data.errmsg);
            return;
        }
    });    
}