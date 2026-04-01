var warehouseData = [];
$(function () {
    layui.use('form', function () {
        common.postGateway("website/selectdata", {
            "type": "warehouse",
            "parameters": [{
                "clientId": (common.getUrlParam("clientId") == null ? "" : common.getUrlParam("clientId")),
                "appCode": common.getUrlParam("appCode")
            }]
        }, null, function (data) {
            warehouseData = JSON.parse(data.json);
            $.each(warehouseData, function (index, item) {
                $('#warehouseId').append(new Option(item.text, item.id));// 下拉菜单里添加元素
            })
            if (common.getCookie("warehouseId") != null && common.getCookie("warehouseId") != "") {
                $("#warehouseId").find("option[value=" + common.getCookie("warehouseId") + "]").prop("selected", true);
            }
            layui.form.render();//下拉菜单渲染 把内容加载进去
        });
    });

    //PC端显示切换仓库弹窗
    layui.use('layer', function () {
        $("#LayUiWarehouse").on("click", function () {
            var nticloudCSS = common.getCookie("NticloudCSS");
            if (nticloudCSS == null || nticloudCSS == "" || nticloudCSS.indexOf(":") == -1) {
                common.setCookie("NticloudCSS", "1:bluelight");
            }
            var nticloudFontSize = nticloudCSS.split(':')[0];
            var nticloudStyle = nticloudCSS.split(':')[1];
            $("input:radio[name='themes'][value='" + nticloudStyle + "']").prop("checked", true);
            $.each($("input[name='fontsize']"), function (index, data) {//index是索引,从0开始
                if (data.value == nticloudFontSize) {
                    $(data).prop('checked', true);//data表示数据的内容
                }
            })
            $.messageBoxOpenBtn('基础设置', ['500px', '410px'], '#SwitchWarehouse', [
                {
                    text: '保 存', handler: function (index) {
                        var warehouseId = $.trim($('#warehouseId').val());
                        if (!warehouseId) {
                            $.messageTips('请选择需要切换的仓库!', ".layui-select-title", [3, '#c8161d']);
                            return false;
                        }
                        var theme = $('input[name="themes"]:checked').val();
                        var fontsize = $('input[name="fontsize"]:checked').val();
                        common.setCookie("warehouseId", warehouseId);
                        common.setCookie("NticloudCSS", fontsize + ":" + theme);
                        var url = window.location.href;
                        var appcode = common.getUrlParam("AppCode");
                        for (var i = 0; i < warehouseData.length; i++) {
                            if (warehouseData[i].id == warehouseId) {
                                url = url.replace("AppCode=" + encodeURI(appcode), "AppCode=" + encodeURI(warehouseData[i].code));
                                window.location.replace(url);
                                break;
                            }
                        }
                    }
                },
                { text: '取 消', handler: function (index) { } },
            ]);
        });
    });
});

