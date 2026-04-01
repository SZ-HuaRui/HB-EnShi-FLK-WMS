$(function () {
    var curUserId, curModuleId, curRoleId, cPermissionId, curRoleName = "?", curModuleName = "?";//选择的模块ID,选中的角色ID
    $(window).resize(function () {
        $('#moduleList').treegrid('resize', {
            height: SetGridHeightSub(70)
        });
        $('#buttonList,#fieldList').treegrid('resize', {
            height: SetGridHeightSub(70) / 2
        });
    });

    $('#roleList').datagrid({
        loader: function (param, success, error) {
            apiRoute.selectDataEvent("select * from prod_system_role where CLIENT_ID='&CLIENT_ID&' and appcode='&AppCode&'", [],"1=1", function (tableData) {
                success(tableData);
            });
        },
        width: SetGridWidthSub(680),
        methord: 'post',
        height: SetGridHeightSub(70),
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
        rownumbers: true,//行号
        title: '角色列表',
        onLoadSuccess: function (data) {
            var allCheck = $("#moduleList").treegrid("getAllChecked", -1);
            for (var i = 0; i < allCheck.length; i++) {
                $("#moduleList").treegrid("uncheckNode", allCheck[i]);
            }
            $('#buttonList').treegrid('loadData', { rows: [] });
            $('#fieldList').treegrid('loadData', { rows: [] });
        },
        columns: [[
            { field: 't_id', title: '', width: 80, hidden: true },
            { field: 'role_name', title: '角色名称', halign: "center", width: 80, sortable: true },
            { field: 'created_by', title: '创建人', halign: "center", width: 80, sortable: true },
            { field: 'create_date', title: '创建时间', align: "center", width: 100, sortable: true },
            { field: 'userid', title: 'userid', hidden: true }
        ]],
        onClickRow: function (index, data) {
            curRoleId = data.t_id;
            curRoleName = data.role_name;
            curUserId = data.userid;
            var allCheck = $("#moduleList").treegrid("getAllChecked", -1);
            for (var i = 0; i < allCheck.length; i++) {
                $("#moduleList").treegrid("uncheckNode", allCheck[i]);
            }
            $('#moduleList').treegrid({
                loader: function (param, success, error) {
                    common.postGateway('platform/permission/menupermission', {
                        "roleid": curRoleId
                    }, common.getToken(), function (tableData) {
                        success(JSON.parse(tableData.json));
                    });
                }
            });
        }
    }).datagrid('getPager').pagination({ showPageList: false, showRefresh: false });

    $('#moduleList').treegrid({
        width: 320,
        methord: 'post',
        height: SetGridHeightSub(70),
        checkbox: true,
        fitColumns: true,
        treeField: 'text',
        idField: 'id',
        sortName: 'id',
        sortOrder: 'asc',
        pagination: false,
        onlyLeafCheck: true,//是否只在末级节点之前显示复选框
        striped: true, //奇偶行是否区分
        singleSelect: true,//单选模式
        title: '模块列表',
        columns: [[
            { field: 'id', title: '资源ID', width: 120, hidden: true },
            { field: 'text', title: '菜单名称', halign: "center", width: 220, sortable: false },
            { field: 'permission', title: '权限ID', width: 120, hidden: true },
            { field: 'checked', title: '是否有权限', width: 120, hidden: true },
            { field: 'isreadonly', title: '是否可改', width: 120, hidden: true }
        ]],
        onLoadSuccess: function (row, data) {
            //$(this).treegrid("collapseAll");
            if (curModuleId) {
                var node = $('#moduleList').treegrid('find', curModuleId);
                if (node != null) {
                    cPermissionId = node.permission;
                }
            }
            var noCheckNodes = $(this).treegrid("getPanel").find(".tree-checkbox0");
            for (var i = 0; i < noCheckNodes.length; i++) {
                var keyValue = $($(noCheckNodes[i]).closest('tr')[0]).attr("node-id");
                if ($($(noCheckNodes[i]).closest('tr')[0]).children('td[field="isreadonly"]').text() == 1) {
                    $(noCheckNodes[i]).removeClass("tree-checkbox0").addClass("tree-checkbox3");
                    $(noCheckNodes[i]).unbind().click(function () {
                        return false;
                    });
                }
            }
            var checkNodes = $(this).treegrid("getPanel").find(".tree-checkbox1");
            for (var i = 0; i < checkNodes.length; i++) {
                var keyValue = $($(checkNodes[i]).closest('tr')[0]).attr("node-id");
                if ($($(checkNodes[i]).closest('tr')[0]).children('td[field="isreadonly"]').text() == 1) {
                    $(checkNodes[i]).removeClass("tree-checkbox1").addClass("tree-checkbox4");
                    $(checkNodes[i]).unbind().click(function () {
                        return false;
                    });
                }
            }
            reloadButtonField();
        },
        onClickRow: function (data) {
            var allModuleId = $('#moduleList').treegrid("getAllChecked", -1);
            if (curModuleId && curUserId == JSON.parse(common.Decrypt(sessionStorage["encrypt"])).CurrentUserID && $.inArray(curModuleId, allModuleId) != -1) {//有变化则保存 
                var saveButtonField = false;
                var buttonList = $("#buttonList").treegrid("getData"); //这段代码是获取当前页的所有行。
                for (var i = 0; i < buttonList.length; i++) {
                    for (var j = 0; j < buttonList[i].children.length; j++) {
                        var setFlag = $("#isDenyView" + buttonList[i].children[j].button_owner + buttonList[i].children[j].button_code).prop("checked");
                        if (buttonList[i].children[j].is_deny_view != setFlag) {
                            saveButtonField = true;
                            break;
                        }
                    }
                }

                var fieldList = $('#fieldList').treegrid("getData");
                for (var i = 0; i < fieldList.length; i++) {
                    for (var j = 0; j < fieldList[i].children.length; j++) {
                        if (fieldList[i].children[j].is_deny_view != $("#isDenyView" + fieldList[i].children[j].table_name + fieldList[i].children[j].field_name).prop("checked") ||
                            fieldList[i].children[j].is_deny_new != $("#isDenyNew" + fieldList[i].children[j].table_name + fieldList[i].children[j].field_name).prop("checked") ||
                            fieldList[i].children[j].is_deny_edit != $("#isDenyEdit" + fieldList[i].children[j].table_name + fieldList[i].children[j].field_name).prop("checked")) {
                            saveButtonField = true;
                            break;
                        }
                    }
                }
                if (saveButtonField) $("#btnSave").trigger("click");
            }

            curModuleName = data.text;
            curModuleId = data.id;
            cPermissionId = data.permission;
            $("#moduleList").treegrid("checkNode", curModuleId);//根据ID勾选节点
            if (curRoleId == null && curModuleId) {
                $.messageBoxBottom('0', "请再选择一个角色！");
                return;
            }
            else {
                reloadButtonField();
            }
        }
    });

    $('#buttonList').treegrid({
        width: 345,
        height: SetGridHeightSub(70) / 2,
        methord: 'post',
        fitColumns: true,
        treeField: 'button_code',
        idField: 't_id',
        sortName: 't_id',
        sortOrder: 'asc',
        pagination: false,
        striped: true, //奇偶行是否区分
        singleSelect: true,//单选模式
        title: '按钮权限',
        //rownumbers: true,//行号
        columns: [[
            { field: 'button_owner', title: '表', align: "center", width: 30, hidden: true },
            { field: 'button_code', title: '按钮', align: "left", halign: "center", width: 65 },
            { field: 'button_caption', title: '标题', halign: "center", width: 60 },
            {
                field: 'is_deny_view', title: "禁用", align: 'center', width: 35, formatter: function (value, row, index) {
                    var moduleData = $("#moduleList").treegrid("getSelected");
                    if (row.button_owner == -1) {
                        return "";
                    }
                    else if ((value == 1 && (moduleData != null && moduleData.isreadonly == 1)) || row.isreadonly == 1) {
                        return "<input id='isDenyView" + row.button_owner + row.button_code + "' disabled type='checkbox' checked='checked' value=" + value + " />";
                    }
                    else if (moduleData != null && moduleData.isreadonly == 1) {
                        return "<input id='isDenyView" + row.button_owner + row.button_code + "' disabled type='checkbox' value=" + value + " />";
                    }
                    else if (value == 1) {
                        return "<input id='isDenyView" + row.button_owner + row.button_code + "' type='checkbox' checked='checked' value=" + value + " />";
                    } else {
                        return "<input id='isDenyView" + row.button_owner + row.button_code + "' type='checkbox' value=" + value + " />";
                    }
                },
            }
        ]],
        onLoadSuccess: function (row, data) {
        }
    });

    $('#fieldList').treegrid({
        width: 345,
        height: SetGridHeightSub(70) / 2,
        methord: 'post',
        fitColumns: true,
        treeField: 'field_name',
        idField: 't_id',
        sortName: 't_id',
        sortOrder: 'asc',
        pagination: false,
        striped: true, //奇偶行是否区分
        singleSelect: true,//单选模式
        title: '字段权限',
        //rownumbers: true,//行号
        columns: [[
            {
                field: 'table_name', title: '表名', halign: "center", width: 50, hidden: true
            },
            {
                field: 'field_name', title: '字段', halign: "center", align: "left", width: 85
            },
            {
                field: 'is_deny_view', title: "禁止<br>查看", align: 'center', width: 25, formatter: function (value, row, index) {
                    var moduleData = $("#moduleList").treegrid("getSelected");
                    if (row.table_name == "") {
                        return "";
                    }
                    else if ((value == 1 && (moduleData != null && moduleData.isreadonly == 1)) || row.isreadonlyview == 1) {
                        return "<input id='isDenyView" + row.table_name + row.field_name + "' disabled type='checkbox' checked='checked' value=" + value + " />";
                    }
                    else if (moduleData != null && moduleData.isreadonly == 1) {
                        return "<input id='isDenyView" + row.table_name + row.field_name + "' disabled type='checkbox' value=" + value + " />";
                    }
                    else if (value == 1) {
                        return "<input id='isDenyView" + row.table_name + row.field_name + "' type='checkbox' checked='checked' value=" + value + " />";
                    }
                    else {
                        return "<input id='isDenyView" + row.table_name + row.field_name + "' type='checkbox' value=" + value + " />";
                    }
                },
            },
            {
                field: 'is_deny_new', title: "禁止<br>新增", align: 'center', width: 25, formatter: function (value, row, index) {
                    var moduleData = $("#moduleList").treegrid("getSelected");
                    if (row.table_name == "") {
                        return "";
                    }
                    else if ((value == 1 && (moduleData != null && moduleData.isreadonly == 1)) || row.isreadonlynew == 1) {
                        return "<input id='isDenyNew" + row.table_name + row.field_name + "' disabled type='checkbox' checked='checked' value=" + value + " />";
                    }
                    else if (moduleData != null && moduleData.isreadonly == true) {
                        return "<input id='isDenyNew" + row.table_name + row.field_name + "' disabled type='checkbox' value=" + value + " />";
                    }
                    else if (value) {
                        return "<input id='isDenyNew" + row.table_name + row.field_name + "' type='checkbox' checked='checked' value=" + value + " />";
                    }
                    else {
                        return "<input id='isDenyNew" + row.table_name + row.field_name + "' type='checkbox' value=" + value + " />";
                    }
                },
            },
            {
                field: 'is_deny_edit', title: "禁止<br>修改", align: 'center', width: 25, formatter: function (value, row, index) {
                    var moduleData = $("#moduleList").treegrid("getSelected");
                    if (row.table_name == "") {
                        return "";
                    }
                    else if ((value == 1 && (moduleData != null && moduleData.isreadonly == 1)) || row.isreadonlyedit == 1) {
                        return "<input id='isDenyEdit" + row.table_name + row.field_name + "' disabled type='checkbox' checked='checked' value=" + value + " />";
                    }
                    else if (moduleData != null && moduleData.isreadonly == 1) {
                        return "<input id='isDenyEdit" + row.table_name + row.field_name + "' disabled type='checkbox' value=" + value + " />";
                    }
                    else if (value == 1) {
                        return "<input id='isDenyEdit" + row.table_name + row.field_name + "' type='checkbox' checked='checked' value=" + value + " />";
                    }
                    else {
                        return "<input id='isDenyEdit" + row.table_name + row.field_name + "' type='checkbox' value=" + value + " />";
                    }
                },
            }
        ]],
        onLoadSuccess: function (row, data) {
        }
    });

    function reloadButtonField() {
        if ((!curModuleId) || (curModuleId == "")) {
            $("#buttonList").treegrid("getPanel").panel("setTitle", "请勾选<font color='red'>禁用</font>按钮-角色:" + curRoleName + " > 模块:" + (curModuleId ? curModuleName : "[请选择表单模块]"));
            $("#fieldList").treegrid("getPanel").panel("setTitle", "请勾选<font color='red'>隐藏</font>字段-角色:" + curRoleName + " > 模块:" + (curModuleId ? curModuleName : "[请选择表单模块]"));
            $('#buttonList').treegrid('loadData', { rows: [] });
            $('#fieldList').treegrid('loadData', { rows: [] });
            return;
        }
        else {
            $('#buttonList').treegrid({
                loader: function (param, success, error) {
                    common.postGateway('platform/permission/buttonpermission', {
                        "menuId": curModuleId,
                        "permissionId": cPermissionId
                    }, common.getToken(), function (tableData) {
                        success(JSON.parse(tableData.json));
                    });
                },
                'title': "请勾选<font color='red'>禁用</font>按钮-角色:" + curRoleName + " > 模块:" + (curModuleId ? curModuleName : "[请选择表单模块]")
            });
            $('#fieldList').treegrid({
                loader: function (param, success, error) {
                    common.postGateway('platform/permission/fieldpermission', {
                        "menuId": curModuleId,
                        "permissionId": cPermissionId
                    }, common.getToken(), function (tableData) {
                        success(JSON.parse(tableData.json));
                    });
                },
                'title': "请勾选<font color='red'>隐藏</font>字段-角色:" + curRoleName + " > 模块:" + (curModuleId ? curModuleName : "[请选择表单模块]")
            });
        }
    }

    $("#btnSave").click(function () {
        if (curUserId != JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserTid) {
            $.messageBoxBottom('0', "该角色非本人创建！");
            return;
        }
        else if (curRoleId == null) {
            $.messageBoxBottom('0', "请再选择一个角色！");
            return;
        }
        else {
            var SavePermissionAction = {}, allModuleId = $('#moduleList').treegrid("getAllChecked", -1);
            var checkModuleId = $('#moduleList').treegrid("getAllChecked", 1), C_PERMISSION = [];
            SavePermissionAction.menu_id = curModuleId;
            SavePermissionAction.clickmenunumber = $.inArray(curModuleId, allModuleId);  //-1不需要保存按钮、字段信息
            SavePermissionAction.role_id = curRoleId;
            for (var i = 0; i < checkModuleId.length; i++) {
                C_PERMISSION.push({
                    "menuId": checkModuleId[i]
                });
            }
            SavePermissionAction.permissionMenu = C_PERMISSION;

            var buttonList = $("#buttonList").treegrid("getData"), buttonListHidden = []; //这段代码是获取当前页的所有行。
            for (var i = 0; i < buttonList.length; i++) {
                for (var j = 0; j < buttonList[i].children.length; j++) {
                    var setFlag = $("#isDenyView" + buttonList[i].children[j].button_owner + buttonList[i].children[j].button_code).prop("checked");
                    buttonList[i].children[j].is_deny_view = setFlag;
                    if (setFlag == true) {
                        buttonListHidden.push(buttonList[i].children[j]);
                    }
                }
            }

            var fieldList = $('#fieldList').treegrid("getData"), fieldListHidden = [];
            for (var i = 0; i < fieldList.length; i++) {
                for (var j = 0; j < fieldList[i].children.length; j++) {
                    fieldList[i].children[j].is_deny_view = $("#isDenyView" + fieldList[i].children[j].table_name + fieldList[i].children[j].field_name).prop("checked");
                    fieldList[i].children[j].is_deny_new = $("#isDenyNew" + fieldList[i].children[j].table_name + fieldList[i].children[j].field_name).prop("checked");
                    fieldList[i].children[j].is_deny_edit = $("#isDenyEdit" + fieldList[i].children[j].table_name + fieldList[i].children[j].field_name).prop("checked");
                    if (fieldList[i].children[j].is_deny_view == true || fieldList[i].children[j].is_deny_new == true || fieldList[i].children[j].is_deny_edit == true) {
                        fieldListHidden.push(fieldList[i].children[j]);
                    }
                }
            }

            SavePermissionAction.permissionButton = buttonListHidden;
            SavePermissionAction.permissionField = fieldListHidden;
            $.messager.progress({ title: "保存", msg: "正在保存权限信息，请稍后……", "text": "数据保存中" });

            common.postGateway('platform/permission/savepermission', {
                "json": JSON.stringify(SavePermissionAction)
            }, common.getToken(), function (result) {
                if (result.json == "OK") {
                    $.messageBoxBottom("1", "菜单权限保存成功！");
                    $('#moduleList').treegrid({
                        loader: function (param, success, error) {
                            common.postGateway('platform/permission/menupermission', {
                                "roleid": curRoleId
                            }, common.getToken(), function (tableData) {
                                success(JSON.parse(tableData.json));
                            });
                        }
                    });
                }
                else {
                    $.messageBoxBottom("0", result.json);
                }
                $.messager.progress('close');
            });
        }
    });
});