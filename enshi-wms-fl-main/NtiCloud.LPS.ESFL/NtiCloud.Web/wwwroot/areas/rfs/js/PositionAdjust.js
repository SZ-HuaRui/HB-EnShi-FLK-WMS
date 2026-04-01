$(function () {
    $(".container_code").focus();
    $(".container_code").keydown(function (event) {
        if (event.which == 13) {
            var container_code = $('.container_code').val();
            if (container_code != '' && container_code != null) {
                barCodeChange(container_code);
            } else {
                alert("托盘条码不能为空！！！");
            }
        }
    });
    $(".conform_btn").on('click',function () {
        adjustConfirm();
    })
    $(".storage_area").on('click', function () {
        inTemporaryArea();
    })
    //$('.container_code').on('change', function () {
    //barCodeChange($('.container_code').val());
    //})        
})
//扫描托盘条码事件
function barCodeChange(barCode) {
    common.postGateway('wms/cpf/adjustlocation/selectpalletdata', {
        "sessionKey": common.getCookie("sessionKey"),
        "Warehouse": "CPF",
        "PalletCode": barCode
    }, function (data) {
        if (Array.isArray(data)) {
            var len = data.length;
            if (len <= 0) {
                alert("查询不到任务!");
            }
            else {
                $("#item_name").text(data[0].name);
                $("#item_code").text(data[0].item);
                $("#location_code").text(data[0].location);
                $("#batch_number").text(data[0].lot);
                $("#shelve_data").text(data[0].on_shelf_time);
            }
        } else {
            alert(data);
        }
    });
}
//确认
function adjustConfirm() {  
    var container_code = $('.container_code').val();
    if (container_code == '' || container_code == null) {
        alert("请输入托盘号！！！");
        return;
    }
    var location_code = $('#location_code').text();
    if (location_code == '' || location_code == null) {
        alert("无货位等信息，请先扫描托盘条码！！！");
        return;
    }
    common.postGateway('wms/cpf/adjustlocation/submitpalletdata', {
        "sessionKey": common.getCookie("sessionKey"),
        "Warehouse": "CPF",
        "PalletCode": container_code
    }, function (data) {
        if (Array.isArray(data)) {
            if (data.length > 0) {
                alert("货位调整成功！！！");
                $("#location_code").text('');
                $("#item_name").text('');
                $("#item_code").text('');
                $("#batch_number").text('');

                $("#shelve_data").text('');
                $(".container_code").val('');
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
//入暂存区
function inTemporaryArea() {
    var location_code = $("#location_code").text();
    if (location_code == null || location_code == "") {
        alert("货位编码不能为空！！！");
        return;
    }
    var container_code = $(".container_code").val();
    if (container_code == null || container_code == "") {
        alert("托盘条码不能为空！！！");
        return;
    }
    common.postGateway('platform/transflows/runtransflow', {
        "sessionKey": common.getCookie("sessionKey"),
        "transflowCode":"m_adjust_temp",
        "parameters": [container_code,'rf']
    },
        function(res){
            //alert(JSON.stringify(res));
            var resMsg = res.OutPuts[1]["FldValue"];
            alert(resMsg);
            if(resMsg == "成功"){
                 $("#location_code").text('');
                $("#item_name").text('');
                $("#item_code").text('');
                $("#batch_number").text('');
                $("#shelve_data").text('');
                $(".container_code").val('');
                $(".container_code").focus();
            }          
        }
    );  
    // common.postGateway('wms/cpf/adjustlocation/createadjustlocation', {
    //     "sessionKey": common.getCookie("sessionKey"),
    //     "Warehouse": "CPF",
    //     "SourceLocation": location_code,
    //     "ToLocation": "",
    //     "PalletCode": container_code,
    //     "Action": "1"
    // }, function (data) {
    //     if (data =="Success") {
    //         alert("入暂存区成功！！！");
    //         $("#location_code").text('');
    //         $("#item_name").text('');
    //         $("#item_code").text('');
    //         $("#batch_number").text('');
    //         $("#shelve_data").text('');
    //         $(".container_code").val('');
    //     }
    //     else {
    //         alert(data);
    //         return;
    //     }
    // });
}