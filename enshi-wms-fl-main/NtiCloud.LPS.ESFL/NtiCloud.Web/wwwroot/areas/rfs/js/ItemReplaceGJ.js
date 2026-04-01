var TaskList = [];//烟包替换任务列表
var table;
var posting = false;
$(function () {
    init();
})
function init() {
    layui.use('table', function () {
        table = layui.table;
        table.init("tbTaskList", {
            height: 100,
            cols: [
                [{ type: 'radio' },
                {
                    field: "item_name", title: "物料"
                }, {
                    field: "spec", title: "规格"
                }, {
                    field: "create_date", title: "霉变日期"
                }
                ]
            ],
            data: TaskList,
            even: true
        });
    });
    LoadTask();
    $("#btnSubmit").bind("click", function () {
        if (posting) {
            return;
        }
        var checkStatus = table.checkStatus('tbTaskList');
        data = checkStatus.data;
        if (data.length != 1) {
            alert("请选中一笔待替换物料");
        } else {
            if ($("#txtResendbarcode").val() == "") {
                alert("请扫替换入库烟包条码");
                return;
            }            
            if (confirm("确认替换入库吗?")) {
                var postData = data[0];
                postData.new_container_code = $("#txtResendbarcode").val();
                if (posting) {
                    return;
                }
                $.messageLoad();
                common.postGateway('wms/PutawayService/SubmitReplace', postData, common.getToken(),
                    function (data) {
                        $.messageBoxCloseLoading();
                        posting = false;
                        if (data.isSucceed) {
                            alert("替换成功");
                            $("#txtResendbarcode").val("");
                            LoadTask();
                        }
                        else {
                            alert(data.errmsg);
                        }
                    }
                ); 
            }
        }
    });
}

/*
加载可替换入库任务列表
*/
function LoadTask() {
    common.postGateway('wms/PutawayService/GetItemReplaceList', {}, common.getToken(),
        function (data) {
            if (data.isSucceed) {
                TaskList = data.json;
                doShowSnTable();
            }
            else {
                alert(data.errmsg);
            }
        }
    );
}

function doShowSnTable() {
    layui.use('table', function () {
        var table = layui.table;
        table.reload("tbTaskList", {
            height: 100,
            limit: Number.MAX_VALUE,
            data: TaskList
        });
    });
}

