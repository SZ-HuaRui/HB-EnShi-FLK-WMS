$(function () {
    
    $(".login_out").click(function () {
        LogOut();
    });

    $('.submis').on('click', function () {
        var issue_text = $('.issue_text').val();
        $.messageLoad();
        apiRoute.runTaskEvent("","INSERT INTO prod_workorder_detail (t_id,CLIENT_ID,ORG_ID,orderid,workorder,CREATE_DATE,CREATED_BY,MODIFY_IP) VALUES "
                + "(@t_id, '', '',@orderId, @workorder,now(),@created_by, @modify_ip)",[{
                t_id: "",
                orderId: $("#OrderID").html(),
                workorder: issue_text
            }], function (data) {
            $.messageBoxCloseLoading();
            if (data.resMsg == "") {
                window.location.reload();
            }
            else {
                $.messageBoxAlert(0,data.resMsg);
            }
        });
    });
});

function workOrderList() {
    $("#MyWorkOrderList").show();
}
