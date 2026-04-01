$(function () {
    $('#List').treegrid({
        loader: function (param, success, error) {
            common.postGateway('platform/file/filelist', { from: "Server", fileName: "Modules" }, common.getToken(), function (tableData) {
                tableData = tableData.json;
                apiRoute.selectDataEvent("select MICROSERVICE_NAME filename,MODIFY_IP createdate,CREATE_DATE modifydate,CREATED_BY note from prod_microserviceslibs",
                     [], "1=1", function (userlogData) {
                    for (var i = 0; i < tableData.length; i++) {
                        tableData[i].children = [];
                        for (var j = userlogData.length - 1; j >= 0; j--) {
                            if (tableData[i].filename == userlogData[j].filename) {
                                tableData[i].state = 'closed';
                                tableData[i].children.push(userlogData[j]);
                                userlogData.splice(j, 1);
                            }
                        }
                    }
                    success(tableData);
                });
            });
        },
        methord: 'post',
        width: SetGridWidthSub(10),
        methord: 'post',
        height: SetGridHeightSub(70),
        fitColumns: true,
        treeField: 'filename',
        idField: 'filename',
        pagination: false,
        striped: true, //奇偶行是否区分
        singleSelect: true,//单选模式
        rownumbers: true,//行号

        columns: [[
            { field: 'filename', title: '文件名', width: 200, halign: 'center', align: 'left' },
            { field: 'createdate', title: '创建时间', width: 120, align: 'center' },
            { field: 'modifydate', title: '最后修改时间', width: 120, align: 'center' },
            { field: 'note', title: '备注说明', width: 160, halign:'center', align: 'left' }
        ]]
    });

    $("#btnSave").click(function () {
        $("#fileList").html("");
        $("#MicroServicesUpload").dialog('open').dialog('center');
    });

    $("#btnBak").click(function () {
        var row = $('#List').datagrid('getSelected');
        if (row == null) {
            $.messageBoxBottom(0, CommonLang.PleaseSelectTheOperatingRecord);
            return;
        }
        else {
            window.location = common.gatewayUrl() + "api/file/downfile?from=Server&fileName=" + row.filename;
        }
    });

    $("#btnDelete").click(function () {
        var row = $('#List').datagrid('getSelected');
        if (row == null) {
            $.messageBoxBottom(0, CommonLang.PleaseSelectTheOperatingRecord);
            return;
        }
        else {
            common.postGateway('platform/file/deletefile', { fileName: "Modules/" + row.filename, from: "Server" }, common.getToken(), function (data) {
                $.messageBoxBottom(1, data.json);
                $('#List').treegrid('load');
            });
        }
    });

    $("#btnRefresh").click(function () {
        $('#List').treegrid('load');
    });
});

function MicroServicesUpload() {
    if ($("#fileList").html() == "") {
        $.messageBoxBottom(0, "请选择要上传的微服务！");
        return;
    }
    $('#MicroServicesUpload').dialog('close');
    var serviceName = "platform/file/uploadfile";
    var serviceJson = { "uploadFormId": "uploadForm", fileDir: "Modules", from: "Server" };
    var serviceTitle = { title: "上传", msg: "正在上传微服务，请稍后……", "text": "服务上传中" };
    common.postFileGateway(serviceName, serviceJson, serviceTitle, common.getToken(), function (data) {
        if (data.json.indexOf("Modules") == 0) {
            $("#btnRefresh").click();
            $.messageBoxBottom(1, "微服务上传成功！");
            $("#microServicesUpload").val("");
            apiRoute.runTaskEvent("","insert into prod_microserviceslibs (client_id,org_id, t_id ,MICROSERVICE_NAME,created_by,modify_ip,create_date,act_status) values ('&CLIENT_ID&','&ORG_ID&',@t_id,@microservice_name,@created_by, @modify_ip,now(),'1')",
                [{
                    "t_id": "",
                    "microservice_name": data.json.substr(data.json.lastIndexOf("\/") + 1)
                }], function (result) {
                if (result.resMsg == "") {
                    $.messageBoxBottom(1, "共" + btText + result.resCount + "条记录！");
                    frameReturnByTreeLoad();
                }
                else {
                    $.messageBoxBottom(0, result.resMsg);
                }
                $('#List').treegrid('load');
            });
        }
        else {
            $.messageBoxBottom(0, data);
        }
    });
}
function onchageFile() {
    $("#fileList").html("<br>即将上传文件列表<br>");
    var files = document.getElementById("microServicesUpload").files;
    for (var i = 0; i < files.length; i++) {
        $("#fileList").html($("#fileList").html() + files[i].name + "<br>");
    }
}