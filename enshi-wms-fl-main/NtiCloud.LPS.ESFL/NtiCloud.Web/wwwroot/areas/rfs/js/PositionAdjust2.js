$(function () {
    $("#new_location").focus();
    $("#new_location").keydown(function (event) {
        if (event.which == 13) {
            var new_location = $('#new_location').val();
            if (new_location != '' && new_location != null) {
                getItem();
            } else {
                alert("目标货位不能为空！！！");
            }
        }
    });
    $(".getInfo").on('click',function () {
        getInfo();
        $("#new_location").focus();
    })
    $(".conform_btn").on('click', function () {
        putOnConfirm();
    })
    //$('.container_code').on('change', function () {
    //barCodeChange($('.container_code').val());
    //})        
})
//车载终端上架获取任务按钮
function getInfo() {
    common.postGateway('wms/cpf/adjustlocation/refreshdata', {
        "sessionKey": common.getCookie("sessionKey"),
        "Warehouse": "CPF"
    }, function (data) {
        console.log(data);
        if (Array.isArray(data)) {
            if (data.length > 0) {
                $("#old_location").text(data[0].location);
                $("#item_name").text(data[0].name);
                $("#item_code").text(data[0].item);
                $("#batch_number").text(data[0].lot);
                $("#shelve_data").text(data[0].on_shelf_time);
                $("#container_id").text(data[0].container_id);
            }
            else {
                alert("获取任务失败！");
            }
        } else {
            alert(data);
        }
    });
}
//根据目标货位查询
function getItem() {
    var new_location = $("#new_location").val();
    if (new_location == null || new_location == "") {
        alert("目标货位不能为空！！！");
        return;
    }
    common.postGateway('wms/cpf/adjustlocation/getlocationtdata', {
        "sessionKey": common.getCookie("sessionKey"),
        "Warehouse": "CPF",
        "ToLocation": new_location
    }, function (data) {
        if (Array.isArray(data)) {
            if (data.length > 0) {
                console.log(data);
                $("#item_name2").text(data[0].name);
                $("#item_code2").text(data[0].item);
                $("#batch_number2").text(data[0].lot);
                $("#shelve_data2").text(data[0].on_shelf_time);
            }
            else {
                alert("数据加载失败,请重新加载！");
                return;
            }
        } else {
            alert(data);
        }
    });
}
//车载终端上架确定按钮
function putOnConfirm() {
    var old_location=$("#old_location").text();
    var new_location = $("#new_location").val();
    if (new_location == null || new_location == "") {
        alert("目标货位不能为空！！！");
        return;
    }
    var container_id = $("#container_id").text();
    //if ($("#item_code").text() != $("#item_code2").text()) {
        //alert("两个物料编码不一致！！！");
        //return;
    //}
    common.postGateway('wms/cpf/adjustlocation/createadjustlocation', {
        "sessionKey": common.getCookie("sessionKey"),
        "Warehouse": "CPF",
        "SourceLocation": old_location,
        "ToLocation": new_location,
        "PalletCode": container_id,
        "Action":"0"
    }, function (data) {
        if (data == "Success") {
            alert(data);
            $("#old_location").text('');
            $("#item_name").text('');
            $("#item_code").text('');
            $("#batch_number").text('');
            $("#shelve_data").text('');
            $("#container_id").text('');
            $("#new_location").val('');
            $("#item_name2").text('');
            $("#item_code2").text('');
            $("#batch_number2").text('');
            $("#shelve_data2").text('');
        }
        else {
            alert(data);
            return;
        }
    });
}