$(function () {

    $("#container_code").keydown(function (event) {
        if (event.which == 13) {
            var container_code = $('#container_code').val();
            if (container_code != '' && container_code != null) {
                getByContainer(container_code);
            } else {
                alert("托盘条码不能为空");
            }
        }
    });

    //快速盘点提交，只产生盘点回库任务
    $("#submit").on("click", function () {
        var container_code = $('#container_code').val();
        var countHeadID = $("#t_CountID").text();
        if (container_code != '' && container_code != null) {
            common.postGateway('wms/CountContainerService/RFSingleGenInCountTask', {
                "ContainerCode": container_code,
                "CountHeadID": countHeadID
            }, common.getToken(), function (data) {
                if (data.isSucceed) {
                    $.each(data.json, function (i, n) {
                        alert("盘点回库成功");
                        $('#container_code').val("");
                        $("#t_CountCode").text("");
                        $("#t_CountID").text("");
                    });
                }
                else {
                    alert(data.errmsg);
                }
            });
        } else {
            alert("托盘条码不能为空");
        }
    });


    //模拟执行wcs回库任务
    $("#submit_wcs").on("click", function () {
        var container_code = $('#container_code').val();

        if (container_code != '' && container_code != null) {
            common.postGateway('wms/CountContainerService/RunInCountTaskByWcs', {
                ContainerNo: container_code,
                ApplyType: "TallyedContainer",
                WarehouseCode: "WMS",
                ApplyStationCode:"C"
            }, common.getToken(), function (data) {
                if (data.isSucceed) {
                    $.each(data.json, function (i, n) {
                        alert("盘点wcs执行成功");
                        $('#container_code').val("");
                        $("#t_CountCode").text("");
                        $("#t_CountID").text("");
                    });
                }
                else {
                    alert(data.errmsg);
                }
            });
        } else {
            alert("托盘条码不能为空");
        }
    });

    //扫描托盘条码
    function getByContainer(container_code) {

        common.postGateway('wms/CountContainerService/CountGetConInfo', {
            "ContainerCode": container_code
        }, common.getToken(), function (data) {
                if (data.isSucceed) {
                    $("#bodydata").html("");
                    $.each(data.json, function (i, n) {
                        $("#t_CountCode").text(n.countCode);
                        $("#t_CountID").text(n.countHeadID);
                    $("#bodydata").append("<tr><th style='display: none'>" + n.detailID + "</th><tr><th>" + n.itemCode + "</th><th class='LocationClass'>" + n.itemName + "</th><th>" + n.qty + "</th></tr>");
                });
            }
            else {
                alert(data.errmsg);
            }
        });
    }
});