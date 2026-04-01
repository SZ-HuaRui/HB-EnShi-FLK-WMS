var formid_config = common.getUrlParam("formid"), formContent, dataSourceCode = '', formUserParam = null;//通过ID获取模版的配置信息
$(function () {
    if (formid_config != null && formid_config != "") {
        common.postGateway('platform/permission/pagedataset', {
            "menuId": formid_config
        }, common.getToken(), function (data) {
            if (data.errcode == "0") {
                var webPageData = JSON.parse(data.json);
                $("title").html(webPageData.Table2[0].text);
                dataSourceCode = webPageData.Table2[0].datasourcecode;
                formContent = JSON.parse(common.Decrypt(webPageData.Table2[0].modulejson));
            }
            else if (data.errcode == "400") {
                common.removeCookie("sessionKey");
                common.setCookie("sessionKey", "noLogin");
                window.localStorage.removeItem("token");
                $.messageBoxBottom(0, "对不起，登录超时！");
            }
            else {
                $.messageBoxBottom(0, data.errmsg);
            }

            $("#StructTree").tree({
                loader: function (param, success, error) {
                    apiRoute.selectTreeEvent(dataSourceCode,formContent.treeData.treeSql,
                        formContent.treeData.treeId,formContent.treeData.treePid,formContent.treeData.treeText,"0", [], function (tableData) {
                            success(tableData);
                    });
                },
                onLoadSuccess: function (node, data) {
                    if (formContent.treeData.treeControlInitMode == "CollapseAll" || formContent.treeData.treeControlInitMode == "ExpandFirstSubNode") {
                        $("#StructTree").tree("collapseAll"); //全部折叠
                    }
                    if (formContent.treeData.treeControlInitMode == "ExpandFirstSubNode" && data.length > 0) {
                        var node = $("#StructTree").tree('find', data[0].id);
                        $("#StructTree").tree('expand', node.target); //展开第一个节点
                    }
                    for (var i = 0; i < 1 && i < formContent.treeData.pageWebData.length; i++) {
                        var urlParam = common.Encrypt(formContent.treeData.pageWebData[0].treeBindField + "='" + data[0].id + "'");
                        $("#listmain").append('<iframe id="' + formContent.treeData.pageWebData[i].treeNode + '" src="' + formContent.treeData.pageWebData[i].treeUrl + "&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId") + "&urlParam=" + urlParam + '" frameborder="0" scrolling="auto" style="width:100%; height:100%; overflow: hidden;display:' + (i == 0 ? "block" : "none") + '"></iframe>');
                    }
                },
                onContextMenu: function (e, node) {
                    e.preventDefault();
                    $(this).tree('select', node.target);
                    if (node.id != "" && document.getElementById("treeMenu").getElementsByTagName("div").length > 1) {
                        $('#treeMenu').menu('show', {
                            left: e.pageX,
                            top: e.pageY
                        });
                    }
                }
            });
            $('.easyui-layout').layout('panel', 'west').panel({ title: (formContent.tableData[0].pageTitle ? formContent.tableData[0].pageTitle : $("title").html()) + '分类' });
            $('#treeleft').css({ "height": SetGridHeightSub(35), "overflow-y": "auto" });

        });
    }

    $('.easyui-layout').layout('panel', 'center').panel({
        onResize: function (width, height) {
            try {
                for (var i = 0; i < formContent.treeData.pageWebData.length; i++) {
                    $("#" + formContent.treeData.pageWebData[i].treeNode).height(height);
                    $("#" + formContent.treeData.pageWebData[i].treeNode).width(width - 3);
                }
                $('#treeleft').css({ "height": SetGridHeightSub(35), "overflow-y": "auto" });
            }
            catch (e) { }
        }
    });
});

function getSelected() {    //树结点关联查询
    var node = $('#StructTree').tree('getSelected');
    if (node) {
        for (var i = 0; i < formContent.treeData.pageWebData.length; i++) {
            if (formContent.treeData.pageWebData[i].treeNode == node.treecode) {
                if ($("#" + formContent.treeData.pageWebData[i].treeNode).length != 0) {
                    $("#" + formContent.treeData.pageWebData[i].treeNode).show();
                    var framesContent = document.getElementById(formContent.treeData.pageWebData[i].treeNode).contentWindow.formContent;
                    document.getElementById(formContent.treeData.pageWebData[i].treeNode).contentWindow.$('#List0').datagrid("load", {
                        sql: common.Encrypt(framesContent.tableData[0].tableSql),
                        queryStr: formContent.treeData.pageWebData[i].treeBindField + "='" + node[formContent.treeData.treeId]+"'"
                    });
                }
                else if (formContent.treeData.pageWebData[i].treeUrl) {
                    var urlParam = common.Encrypt(formContent.treeData.pageWebData[i].treeBindField + "='" + node[formContent.treeData.treeId] + "'");
                    $("#listmain").append('<iframe id="' + formContent.treeData.pageWebData[i].treeNode + '" src="' + formContent.treeData.pageWebData[i].treeUrl + "&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId")+ "&urlParam=" + urlParam+ '" frameborder="0" scrolling="auto" style="width:100%; height:100%; overflow: hidden;"></iframe>');
                    $("#" + formContent.treeData.pageWebData[i].treeNode).load(function () {
                    });
                }
            }
            else if ($("#" + formContent.treeData.pageWebData[i].treeNode).length != 0) {
                $("#" + formContent.treeData.pageWebData[i].treeNode).hide();
            }
        }
    }
}