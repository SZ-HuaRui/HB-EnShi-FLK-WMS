var formResult = [], workFlowData, dataSourceCode="";
$(function () {
    $('#conDiv').css({
        'height': SetGridHeightSub(30),
        'overflow-y': 'auto'
    })
});

function LoadFinish() {
    if (common.getUrlParam("id") != null) {
        $.ajaxSettings.async = false;
        apiRoute.selectDataEvent(formattributeJSON.pageService,[], formattributeJSON.pageIDField + "='" + common.getUrlParam("id") + "'", function (result) {
            formResult = result;
            for (var field in result[0]) {
                var isCtrl = 0;
                for (var i = 0; result && i < allctrl.length; i++) {
                    if (field == allctrl[i].id) {
                        if (allctrl[i].type == "button" || allctrl[i].type == "label") {
                            continue;
                        } else if (allctrl[i].type == "radio") {
                            $("input[name='" + allctrl[i].id + "'][value='" + result[0][field] + "']").prop("checked", true);

                        } else if (allctrl[i].type == "checkbox") {
                            $("input[name='" + allctrl[i].id + "']").prop("checked", false);
                            var id_value = result[0][field].split(',');
                            for (var k = 0; k < id_value.length; k++) {
                                $("input[name='" + allctrl[i].id + "'][value='" + id_value[k] + "']").prop("checked", true);
                            }
                        } else if (allctrl[i].type == "dict") {
                            $("#" + allctrl[i].id).switchbutton({ checked: (result[0][field] == "0" ? false : true) });
                        } else {
                            $("#" + allctrl[i].id).val(result[0][field]);
                        }
                        isCtrl = 1;
                        break;
                    }
                }
                if (isCtrl == 0) {
                    allctrl.push({ id: field, type: "text" });
                    $("#conDiv").append('<input id="' + field + '" type="hidden" value="' + result[0][field] + '">');
                }
            }
        });
        $.ajaxSettings.async = true;
    }
} 

function RadioOrCheckboxHtml(htmlid, sql, id, text, type, defaultvalue, eventArrs) {
    apiRoute.pageDictionary({ "dictionaryorsql": common.Encrypt(sql) }, function (tableDataSet) {
        var result = tableDataSet.dictionaryorsql;
        for (var i = 0; i < result.length; i++) {
            var value = result[i][id];
            var title = result[i][text];
            $("#" + htmlid).append('<input type="' + type + '" name="' + htmlid + '" id="' + htmlid + '_' + i.toString() + '" value="' + $.trim(value) + '" ' + (defaultvalue == $.trim(value) ? 'checked' : '') + ' style="vertical-align:middle;" isflow="1" type1="flow_' + type + '"' + eventArrs + '/>');
            $("#" + htmlid).append('<label for="' + htmlid + '_' + i.toString() + '" style="vertical-align:middle;margin-right:3px;">' + $.trim(title) + '</label>');
        }
        if (type == 'checkbox') {//radio 同样适用
            $("input[name='" + htmlid + "']").prop("checked", false);
            var id_value;
            if (formResult.length == 0) {
                id_value = defaultvalue.split(',');
            }
            else {
                id_value = formResult[0][htmlid].split(',');
            }
            for (var k = 0; k < id_value.length; k++) {
                $("input[name='" + htmlid + "'][value='" + id_value[k] + "']").prop("checked", true);
            }
        }
    });
}
function SelectHtml(htmlid, sql, id, text, defaultvalue) {
    apiRoute.pageDictionary({ "dictionaryorsql": common.Encrypt(sql) }, function (tableDataSet) {
        var result = tableDataSet.dictionaryorsql;
        for (var i = 0; i < result.length; i++) {
            var value = result[i][id];
            var title = result[i][text];
            $("#" + htmlid).append('<option value="' + value + '" ' + (defaultvalue == $.trim(value) ? 'selected="selected"' : '') + '>' + title + '</option>');
        }
        if (formResult.length > 0) {
            $("#" + htmlid).val(formResult[0][htmlid]);
        }
    });
}

function ComboxHtml(htmlid, sql, id, pid, text, defaultvalue, allselect, treelist, treeselect, events) {
    var eventArr = JSON.parse(events);
    apiRoute.pageDictionary({ "dictionaryorsql": common.Encrypt(sql) }, function (tableDataSet) {
        var result = tableDataSet.dictionaryorsql;
        if (treelist == "0") {
            $("#" + htmlid).combobox({
                data: result,
                editable: false,
                multiple: allselect == "1",
                valueField: id,
                textField: text,
                panelHeight: 'auto',
                onChange: function (srcObj) {
                    eventAction(eventArr, srcObj);
                },
                onLoadSuccess: function (data) {
                    if (formResult.length > 0) {
                        $("#" + htmlid).combobox("setValues", formResult[0][htmlid].toString());
                    }
                    else {
                        $("#" + htmlid).combobox("setValues", defaultvalue.toString());
                    }
                },
                onClick: function (srcObj) {
                    eventAction(eventArr, srcObj);
                }
            });
        }
        else {
            $("#" + htmlid).combotree({
                data: JQueryEasyUI.toTreeData(result, id, pid, text),
                editable: false,
                multiple: allselect == "1",
                onlyLeafCheck: treeselect == "0",
                panelHeight: 'auto',
                onShowPanel: function () {  //显示下拉列表展开到当前节点
                    var t = $(this).combotree('tree');
                    t.tree("collapseAll");
                    var node = t.tree('getSelected');
                    if (node) {
                        t.tree('expand', node.target); //选中节点展开
                        t.tree('expandTo', node.target);
                    }
                    else {
                        t.tree("expandAll");
                    }
                },
                onLoadSuccess: function (data) {
                    if (formResult.length > 0) {
                        $("#" + htmlid).combotree("setValues", formResult[0][htmlid].toString());
                    }
                    else {
                        $("#" + htmlid).combotree("setValues", defaultvalue.toString());
                    }
                },
                onChange: function (srcObj) {
                    eventAction(eventArr, srcObj);
                },
                onClick: function (srcObj) {
                    eventAction(eventArr, srcObj);
                }
            });
        }        
    })
}
function eventAction(eventArr, srcObj) {
    for (var i = 0; i < eventArr.length; i++) {
        if (eventArr[i].name == "onclick") {            
            return eval(eventArr[i].script.replace('srcObj.value', 'srcObj'));
        } else if (eventArr[i].name == "onchange") {
            return eval(eventArr[i].script.replace('srcObj.value', 'srcObj'));
        }
    }
};
function SerialNumberHtml(htmlid, sql) {
    if (!$("#" + htmlid).val() && common.getUrlParam("btCode") != "Edit") {
        common.postGateway('platform/sequence/getsequenceno', {
            "seqNameOrId": sql.replace('GetSequenceNo(\'', '').replace('\')', '')
        }, common.getToken(), function (sequenceNo) {
            $("#" + htmlid).val(sequenceNo.json);
        });
    }
}
function SubTableHtml(subtableData,id) {
    var subtableJSON = JSON.parse(subtableData);
    var columnsArr = [], showIndex = "1" == subtableJSON.showindex;
    for (var n = 0; n < subtableJSON.colnums.length; n++) {
        if (n > 0 && subtableJSON.colnums[n].show == "N") continue;

        columnsArr.push({});
        columnsArr[columnsArr.length - 1].field = subtableJSON.colnums[n].fieldName;
        if (subtableJSON.colnums[n].title && common.getCookie("localLanguage") != "en-US") {
            columnsArr[columnsArr.length - 1].title = subtableJSON.colnums[n].title;
        } else {
            subtableJSON.colnums[n].title = subtableJSON.colnums[n].fieldName;
            columnsArr[columnsArr.length - 1].title = subtableJSON.colnums[n].fieldName;
        }
        if (subtableJSON.colnums[n].width) {
            columnsArr[columnsArr.length - 1].width = parseInt(subtableJSON.colnums[n].width);
        }
        else {
            columnsArr[columnsArr.length - 1].width = 100;
        }
        if (subtableJSON.colnums[n].alignType) {
            columnsArr[columnsArr.length - 1].align = subtableJSON.colnums[n].alignType;
        }
        columnsArr[columnsArr.length - 1].halign = 'center';
        if (subtableJSON.colnums[n].show == "N" || subtableJSON.colnums[n].show == "H") {
            columnsArr[columnsArr.length - 1].hidden = true;
        }
        else {
            columnsArr[columnsArr.length - 1].hidden = false;
        }
        columnsArr[columnsArr.length - 1].sortable = true;
        if (subtableJSON.colnums[n].style != undefined && subtableJSON.colnums[n].style != "") {
            columnsArr[columnsArr.length - 1].style = subtableJSON.colnums[n].style;
        }
        if (subtableJSON.editmodel == "0") {
            if (subtableJSON.colnums[n].dataSource != undefined && subtableJSON.colnums[n].dataSource != "" && subtableJSON.colnums[n].editControl == "selected") {
                columnsArr[columnsArr.length - 1].editor = {
                    type: "combobox",
                    options: {
                        loader: function (param, success, error) {
                            apiRoute.selectDataEvent(subtableJSON.colnums[n].dataSource, [],"1=1", function (tableData) {
                                success(tableData);
                            });
                        },
                        editable: true,
                        //data: selectData,
                        panelHeight: 'auto',
                        valueField: 'id',
                        textField: 'text'
                    }
                }
            } else {
                columnsArr[columnsArr.length - 1].editor = {
                    type: 'text',
                    options: {      
                        editable: true,
                        data: '',
                        panelHeight: 'auto',
                        valueField: 'id',
                        textField: 'text'
                    }
                }
            }
        }
    }

    var editRow = undefined
    var datagrid = $("#" + id).datagrid({
        loader: function (param, success, error) {
            apiRoute.selectEasyuiTable(dataSourceCode,common.Encrypt(subtableJSON.secondtable), {
                "pager": param
            }, function (tableData) {
                success(tableData);
            });
        },
        //width: SetGridWidthSub(' + parseInt(subtableJSON.width) + '),
        methord: 'post',
        //height: SetGridHeightSub(' + parseInt(subtableJSON.height) + '),
        fitColumns: true,
        sortName: columnsArr[1].field,
        sortOrder: 'asc',
        pageSize: 15,
        pageList: [15, 20, 30, 40, 50],
        pagination: true,
        striped: true, //奇偶行是否区分\
        singleSelect: true,//单选模式
        rownumbers: showIndex == "1" ? true : false,//行号
        columns: [columnsArr],
        toolbar: [
            {
                text: '添 加', iconCls: 'fa fa-plus color-green', handler: function (){
                    if (editRow != undefined) {datagrid.datagrid("endEdit", editRow);}
                    if (editRow == undefined) {
                        datagrid.datagrid("insertRow", {index: 0,row: {}});
                        datagrid.datagrid("beginEdit", 0);
                        datagrid.datagrid('selectRow', 0);
                            editRow = 0;
                    }
                }
            },'-',
            {
                text: '删 除', iconCls: 'fa fa-minus color-red', handler: function (){
                    var rowindex = datagrid.datagrid('getRowIndex', datagrid.datagrid('getSelected'));
                    if (editRow != undefined) {datagrid.datagrid('selectRow', editRow);rowindex = editRow;}
                    if (rowindex == -1) { $.messageBoxAlert(0, "请选择要删除的行!");return;}
                    $.messageBoxConfirm("你确定要删除吗?", ['确定', '取消'], function () {
                        datagrid.datagrid('cancelEdit', rowindex).datagrid('deleteRow', rowindex);
                        editRow = undefined;
                    })
                }
            }
        ],
        onAfterEdit: function (rowIndex, rowData, changes) {editRow = undefined;},
        onClickRow: function (rowIndex, rowData) {
            if (editRow != undefined) {datagrid.datagrid("endEdit", editRow);}
            if (editRow == undefined) {
                datagrid.datagrid("beginEdit", rowIndex);
                editRow = rowIndex;
            }
        }
            
    })
}
function SaveForm(fun) {//allctrl, formattributeJSON, 
    var jsonSave = {};
    var fieldJson = formattributeJSON.fields;
    for (var n = 0; n < fieldJson.length; n++) {
        if ((fieldJson[n].field == "CLIENT_ID" || fieldJson[n].field == "ORG_ID" || fieldJson[n].field == "CREATE_DATE" || fieldJson[n].field == "CREATED_BY" || fieldJson[n].field == "MODIFY_IP" || fieldJson[n].field == "MODIFY_DATE" || fieldJson[n].field == "LAST_MODIFIED_BY" || fieldJson[n].field == "ACT_STATUS") && $("#"+fieldJson[n].field).length==0) continue;
        if ($("#" + fieldJson[n].field).length == 0) {
            jsonSave[fieldJson[n].field] = "";
        }
        else {
            jsonSave[fieldJson[n].field] = $("#" + fieldJson[n].field).val();
        }
        for (var i = 0; i < allctrl.length; i++) {
            if (allctrl[i].id == fieldJson[n].field) {
                if (allctrl[i].type == "button" || allctrl[i].type == "label") {
                    continue;
                }else if (allctrl[i].type == "radio" || allctrl[i].type == "checkbox") {
                    var chk_value = [];
                    $("input[name='" + allctrl[i].id + "']:checked").each(function () {
                        chk_value.push($(this).val());
                    });
                    jsonSave[fieldJson[n].field] = chk_value.join(',');
                } else if (allctrl[i].type == "dict") {
                    jsonSave[fieldJson[n].field] = (new Boolean($("#" + fieldJson[n].field).switchbutton("options").checked) == true ? "1" : "0");
                }else if (allctrl[i].type == "html") {
                    jsonSave[fieldJson[n].field] = UE.getEditor(fieldJson[n].field).getContent();
                } else {
                    jsonSave[fieldJson[n].field] = $("#"+fieldJson[n].field).val();
                }
                break;
            }
        }
    }
    apiRoute.runTaskEvent(dataSourceCode,(common.getUrlParam("id") == null ? formattributeJSON.addService : formattributeJSON.editService), [jsonSave], function (result) {
        if (fun != null) {
            fun(result);
        }
    });
}

$(function () {
    //workFlowData = common.workFlowHtml();
    //var workflowPermission = workFlowData.queryWfStepField ? workFlowData.queryWfStepField : [];
    $("#btnSave,#btnSaveClose").click(function () {
        var ctrlID = this.id;
        SaveForm(function (result) {
            if (result.resMsg == '') {
                $.messageBoxBottom(1,"页面配置保存成功！");
                window.parent.frameReturnByReload(false);
                if (ctrlID == "btnSaveClose") {
                    window.parent.frameReturnByClose();
                }
            }
            else {
                $.messageBoxBottom(0,result.resMsg);
            }
        });
        return false;
    });

    $("#btnReturn").click(function () {
        window.parent.frameReturnByClose();
    });
});
