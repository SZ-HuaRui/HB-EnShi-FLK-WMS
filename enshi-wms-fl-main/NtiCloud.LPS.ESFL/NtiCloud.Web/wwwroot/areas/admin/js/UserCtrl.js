$(function () {
    var ctrlParam = common.getUrlParam("ctrlParam"), ctrlName = common.getUrlParam("ctrlName");
    var clickMore;
    var tableid_config = common.getUrlParam("tableid");
    if (tableid_config == null) tableid_config = 0;
    if (ctrlParam == "orgRadioCtrl") {
        clickMore = 0;
    }
    else {
        clickMore = 1;
    }

    var selectUserListData = [], selectUserListId = parent.$("#" + ctrlName).combobox("getValues");
    var allUserListData;
    $.ajaxSettings.async = false;
    apiRoute.selectDataEvent("select a.t_id,userid,username,b.org_id from prod_system_user a,prod_common_org_user b where a.t_id=b.usertid and b.client_id='&CLIENT_ID&'",
         [], "1=1", function (userData) {
        allUserListData = userData;
    });
    $.ajaxSettings.async = true;

    for (var i = 0; i < selectUserListId.length; i++) {
        for (var j = 0; j < allUserListData.length; j++) {
            if (selectUserListId[i] == allUserListData[j].t_id) {
                selectUserListData.push({ "t_id": selectUserListId[i], "userid": "√ " + allUserListData[j].userid, "username": allUserListData[j].username });
                break;
            }
        }
    }

    if (selectUserListData.length > 0) {
        $("#UserCtrl").tabs("select", 0);
    }
    else {
        $("#UserCtrl").tabs("select", 1);
    }
    
    $("#UserCtrl").tabs({
        onSelect: function (title, index) {
            if (index == 0) {
                var selectUser = $('#selectlist').datagrid("getData").rows;
                for (var i = selectUser.length - 1; i >= 0; i--) {
                    if (selectUser[i].userid.indexOf("√") != 0) {
                        $('#selectlist').datagrid("deleteRow", i);
                    }
                }
                $('#selectlist').datagrid();
            }
            else {
                $('#orglist').treegrid("load");
                $('#userlist').datagrid("load");
                selectUpdateUser();
            }
        }
    });
    function selectUpdateUser() {
        var selectUser = $('#selectlist').datagrid("getData").rows;
        var allUser = $('#userlist').datagrid("getData").rows;
        for (var i = 0; i < allUser.length; i++) {
            for (var j = 0; j < selectUser.length; j++) {
                if (selectUser[j].t_id == allUser[i].t_id) {
                    allUser[i].userid = selectUser[j].userid;
                    $("#userlist").datagrid("refreshRow", i);
                    break;
                }
            }
        }
    }

    $('#selectlist').datagrid({
        data: selectUserListData,
        width: 492,
        height: 280,
        methord: 'post',
        fitColumns: true,
        pagination: false,
        striped: true, //奇偶行是否区分
        singleSelect: true,//单选模式
        rownumbers: true,//行号
        onLoadSuccess: function (data) {
        },
        columns: [[
            { field: 't_id', title: '', width: 80, hidden: true },
            { field: 'userid', title: '选中用户列表', halign: "center", width: 100, sortable: true },
            { field: 'username', title: '用户姓名', halign: "center", width: 100, sortable: true }
        ]],
        onClickRow: function (index, data) {
            if (data.userid.indexOf("√") != 0) {
                data.userid = "√ " + data.userid;
                $("#selectlist").datagrid("refreshRow", index);
            }
            else {
                data.userid = data.userid.substring(2);
                $("#selectlist").datagrid("refreshRow", index);
            }
        }
    });

    $('#orglist').treegrid({
        loader: function (param, success, error) {
            apiRoute.selectTreeEvent("","select T_ID id,ORG_PID pid,ORG_NAME text from prod_common_org",
                "id","pid","text", "1", [], function (tableData) {
                    success(tableData);
            });
        },
        width: 220,
        height: 280,
        methord: 'post',
        fitColumns: true,
        treeField: 'text',
        idField: 'id',
        sortName: 'id',
        sortOrder: 'asc',
        checkbox:(clickMore==0 ? false: true),
        singleSelect: true,//单选模式
        onlyLeafCheck: true,
        pagination: false,
        striped: true, //奇偶行是否区分
        columns: [[
            { field: 'id', title: '唯一标识', width: 120, hidden: true },
            { field: 'text', title: '组织名称', halign: "center", width: 220, sortable: false }
        ]],
        onLoadSuccess: function (row, data) {
            if (parent.$("#" + ctrlName + "_Org").length > 0) {
                var orgCheckId = parent.$("#" + ctrlName + "_Org").val().split(',');
                for (var i = 0; i < orgCheckId.length; i++) {
                    if (orgCheckId[i] == "") continue;
                    $("#orglist").treegrid("checkNode", orgCheckId[i]);
                }
            }
        },
        onSelect: function (node) {
            var parent = $('#orglist').treegrid("getParent", node.id);
            $("#userlist").datagrid("load", {
                sql: common.Encrypt("select a.t_id,userid,username,b.org_id from prod_system_user a,prod_common_org_user b where a.t_id=b.usertid"),
                queryStr: ((parent == null && $('#orglist').treegrid('getRoots').length == 1) ? "" : "org_id='" + node.id + "'")
            });
        },
        onCheckNode: function (row, checked) {
            if (checked) {
                for (var i = 0; i < allUserListData.length; i++) {
                    if (allUserListData[i].org_id == row.id) {
                        var selectUser = false;
                        for (var j = 0; j < selectUserListData.length; j++) {
                            if (selectUserListData[j].t_id == allUserListData[i].t_id) {
                                selectUser = true;
                                break;
                            }
                        }
                        if (selectUser == false) {
                            selectUserListData.push({ "t_id": allUserListData[i].t_id, "userid": "√ " + allUserListData[i].userid, "username": allUserListData[i].username });
                        }
                    }
                }
            }
            else {
                for (var i = 0; i < allUserListData.length; i++) {
                    if (allUserListData[i].org_id == row.id) {
                        for (var j = 0; j < selectUserListData.length; j++) {
                            if (selectUserListData[j].t_id == allUserListData[i].t_id) {
                                selectUserListData.splice(j, 1);
                                break;
                            }
                        }
                    }
                }
            }
            $("#userlist").datagrid("load", {
                sql: common.Encrypt("select a.t_id,userid,username,b.org_id from prod_system_user a,prod_common_org_user b where a.t_id=b.usertid"),
                queryStr: ((parent == null && $('#orglist').treegrid('getRoots').length == 1) ? "" : "org_id='" + row.id + "'")
            });
        }
    });


    $('#userlist').datagrid({
        loader: function (param, success, error) {
            var sql = common.Encrypt("select a.t_id,userid,username,b.org_id from prod_system_user a,prod_common_org_user b where a.t_id=b.usertid");
            var queryStr = "1=1";
            if (param.sql) sql = param.sql;
            if (param.queryStr) queryStr = param.queryStr;
            apiRoute.selectEasyuiTable("",sql, {
                "queryStr": queryStr,
                "pager": param
            }, function (tableData) {
                success(tableData);
            });
        },
        width: 215,
        height: 280,
        methord: 'post',
        fitColumns: true,
        sortName: 't_id',
        sortOrder: 'asc',
        idField: 't_id',
        pageSize: 15,
        pageList: [15, 20, 30, 40, 50],
        showPageList: false,
        showRefresh: false,
        displayMsg: '',
        pagination: false,
        striped: true, //奇偶行是否区分
        singleSelect: true,//单选模式
        //rownumbers: true,//行号
        onLoadSuccess: function (data) {
            selectUpdateUser();
        },
        columns: [[
            { field: 't_id', title: '', width: 80, hidden: true },
            { field: 'userid', title: '用户名', halign: "center", width: 100, sortable: true },
            { field: 'username', title: '姓名', halign: "center", width: 100, sortable: true }
        ]],
        onClickRow: function (index, data) {
            if (clickMore == 0) {//单选
                var curlist = $('#userlist').datagrid("getData").rows;
                for (var i = 0; i < $('#selectlist').datagrid("getData").rows.length; i++) {
                    $('#selectlist').datagrid("deleteRow", i);
                }
                for (var i = 0; i < curlist.length; i++) {
                    if (curlist[i].userid.indexOf("√") == 0) {
                        curlist[i].userid = curlist[i].userid.substring(2);
                        curlist[i].username = curlist[i].username;
                        $("#userlist").datagrid("refreshRow", i);
                    }
                }
            }

            if (data.userid.indexOf("√") != 0) {
                data.userid = "√ " + data.userid;
                data.username = data.username;
                $("#userlist").datagrid("refreshRow", index);
            }
            else {
                data.userid = data.userid.substring(2);
                data.username = data.username;
                $("#userlist").datagrid("refreshRow", index);
            }
            var selectUser = $('#selectlist').datagrid("getData").rows, listId = -1;
            for (var i = 0; i < selectUser.length; i++) {
                if (selectUser[i].t_id == data.t_id) {
                    listId = i;
                    selectUser[i].userid = data.userid;
                    selectUser[i].username = data.username;
                    $("#selectlist").datagrid("refreshRow", i);
                    break;
                }
            }
            if (listId != -1 && data.userid.indexOf("√") != 0) {
                $('#selectlist').datagrid("deleteRow", listId);
            }
            else if (listId == -1 && data.userid.indexOf("√") == 0) {
                $('#selectlist').datagrid("insertRow", {
                    index: selectUser.length,	// 索引从0开始
                    row: {
                        t_id: data.t_id,
                        userid: data.userid,
                        username:data.username
                    }
                });
            }
        }
    }).datagrid('getPager').pagination({ showPageList: false, showRefresh: false });

});