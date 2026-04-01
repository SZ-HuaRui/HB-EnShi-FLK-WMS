var openData = [], cellIndex;
$(function () {
    var formid_config = window.parent.formid_config, btnSql = window.parent.btnSql;
    var tableid_config = common.getUrlParam("tableid"); //第几个表格
    var buttonid_config = common.getUrlParam("buttonid"); //第几个按钮
    var formContent = window.parent.formContent;//配置数据

    //此处增加左侧列表选项的iframe加载逻辑
    var target_table_config = formContent.tableData[tableid_config];
    var target_field_obj = {};
    var isShow_leftlist = false;

    $("#pageFormTableContainer").layout('resize', {
        width: '100%',
        height: SetGridHeightSub($("#buttonList").height() + 6)
    });
    $("#left_wrapper").layout('resize', {
        width: '100%',
        height: SetGridHeightSub($("#buttonList").height() + 41)
    })

    for (var i = 0; i < target_table_config.fieldData.length; i++) {
        if (target_table_config.fieldData[i].controlType == 'leftlist') { //存在左侧列表
            var dataSource_arr = target_table_config.fieldData[i].dataSource.split(';');
            target_field_obj.dataSource = dataSource_arr[dataSource_arr.length - 1];
            target_field_obj.field = target_table_config.fieldData[i].field;
            target_field_obj.width = (dataSource_arr[0] == "" ? "450" : dataSource_arr[0]);

            $('#pageFormTableContainer').layout('panel', 'west').panel('resize', { width: target_field_obj.width});
            $('#pageFormTableContainer').layout('resize');
            $('#left_wrapper').layout('panel', 'west').panel('resize', { width: $('#pageFormTableContainer').layout('panel', 'west')[0].clientWidth - 43 });
            $('#left_wrapper').layout('resize');

            //显示左侧列表和操作框
            $('#pageFormTableContainer').addClass('show_leftlist');
            var iframeSrcStr = target_field_obj.dataSource + "&ctrlName=" + target_field_obj.field + "&leftlist=listOpen&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId");
            $('#leftlist_table').attr('src', iframeSrcStr);

            //左侧表添加数据加载成功的回调，对已选数据进行打钩
            var subIframeObj = document.getElementById("leftlist_table")
            subIframeObj.onload = function () {
                subIframeObj.contentWindow.$('#List0').datagrid({
                    onLoadSuccess: function (data) {
                        matchAndCheck();
                        //添加鼠标拖拽多选
                        var containObj = subIframeObj.contentWindow.$('.layout-panel-center .datagrid-view2 .datagrid-body');
                        var targetObj = subIframeObj.contentWindow.$('.layout-panel-center .datagrid-view2 .datagrid-body tr');
                        $.DragToSelect(containObj, targetObj,'moveSelected');
                    }
                })
            }
            isShow_leftlist = true;

            break;
        }
    }
    if (!isShow_leftlist) {
        $('#pageFormTableContainer').layout('remove', 'west');
    }
    //点击按钮从左侧列表选择
    $('#pageFormTableContainer #select_act').click(function () {
        var select_options = document.getElementById("leftlist_table").contentWindow.$('#List0').datagrid("getSelections");
        var fields_arr = [];
        select_options.forEach(function (item, index) {
            if (index == 0) {
                fields_arr = findSameFields(item, target_table_config.fieldData);
            }
            var right_table_data = $('#List').datagrid("getData").rows;
            for (var i = 0; i < right_table_data.length; i++) {
                if (right_table_data[i][target_field_obj.field] == item[target_field_obj.field]) {
                    return;
                }
            }

            var row_obj = {};
            fields_arr.forEach(function (subItem) {
                row_obj[subItem] = item[subItem];
            })

            $('#List').datagrid('insertRow', {
                index: 0,   // 索引从0开始
                row: row_obj
            });
        });
        matchAndCheck();
    });

    //点击全选按钮
    $('#pageFormTableContainer #select_all').click(function () {
        document.getElementById("leftlist_table").contentWindow.$('#List0').datagrid("checkAll");
        $('#pageFormTableContainer #select_act').click();
    });

    //点击按钮从右侧表中删除
    $('#pageFormTableContainer #cancel_act').click(function () {
        var select_option = $('#List').datagrid("getSelections"),selectRows=[];
        if (select_option.length > 0) {
            for (var i = select_option.length - 1; i >= 0; i--) {
                var rowIndex = $('#List').datagrid('getRowIndex', select_option[i]);
                selectRows.push(rowIndex);
            }
            for (var i = 0; i < selectRows.length; i++) {
                $('#List').datagrid("deleteRow", selectRows[i]);
            }
            matchAndCheck();
        } else {
            $.messageBoxBottom(0, "请先从右侧表中选中一行！");
        };
    });

    //点击全部取消
    $('#pageFormTableContainer #cancel_all').click(function () {
        $('#List').datagrid("clearSelections");
        $('#List').datagrid("selectAll");
        $('#pageFormTableContainer #cancel_act').click();
    });

    //比对并自动勾选
    function matchAndCheck() {
        document.getElementById("leftlist_table").contentWindow.$('#List0').datagrid("clearSelections");
        $('#List').datagrid("clearSelections");
        for (var n = $('#List').datagrid("getData").rows.length - 1; n >= 0; n--) {
            $('#List').datagrid("endEdit", n);
        }
        var left_table_data = document.getElementById("leftlist_table").contentWindow.$('#List0').datagrid("getData").rows;
        var right_table_data = $('#List').datagrid("getData").rows;
        left_table_data.forEach(function (item, index) {;
            for (var i = 0; i < right_table_data.length; i++) {
                if (item[target_field_obj.field] == right_table_data[i][target_field_obj.field]) {
                    document.getElementById("leftlist_table").contentWindow.$('#List0').datagrid("checkRow", index);
                    return;
                }
            }
            document.getElementById("leftlist_table").contentWindow.$('#List0').datagrid("uncheckRow", index);
        })
    }

    //查找相同字段
    function findSameFields(objA, keyArr) {
        var fields_arr = [];
        Object.keys(objA).forEach(function (key) {
            for (var i = 0; i < keyArr.length; i++) {
                if (keyArr[i].field == key) {
                    fields_arr.push(key);
                    break;
                }
            }
        })
        return fields_arr;
    }


    var dataSourceCode = window.parent.dataSourceCode;//配置数据源
    var tableNum = 0, btnNum = 0;
    for (var i = 0; i < formContent.tableData.length; i++) {
        if (formContent.tableData[i].tableType == "") {
            tableNum++;
        }
        for (var j = 0; i <= tableid_config && j < formContent.tableData[i].buttonData.length && j <= buttonid_config; j++) {
            if (formContent.tableData[i].buttonData[j].btnId == "Table") {
                btnNum++;
            }
        }
    }
    if (tableNum < formContent.tableData.length && btnNum <= (formContent.tableData.length - tableNum)) {
        tableid_config = tableNum + btnNum - 1;
    }
    var formPermission = window.parent.formPermission;//权限数据
    
    var pageListTemp = [10, 20, 30, 40, 50, 100, 200, 500];
    for (var i = 0; $.inArray(formContent.tableData[tableid_config].pageNum, pageListTemp) == -1 && i < pageListTemp.length; i++) {
        if (parseInt(formContent.tableData[tableid_config].pageNum) < parseInt(pageListTemp[i])) {
            pageListTemp.splice(i, 0, formContent.tableData[tableid_config].pageNum);
            break;
        }
    }
    if ($.inArray(formContent.tableData[tableid_config].pageNum, pageListTemp) == -1) pageListTemp.push(formContent.tableData[tableid_config].pageNum);

    if (tableid_config == "0") {
        var sql = common.Encrypt(formContent.tableData[tableid_config].tableSql.replace(/@t_id/g, "'" + common.getUrlParam("id") + "'"));
        var queryStr = "1=1";
        ListTable(tableid_config, sql, queryStr);
    }
    else {
        var click_main_id = window.parent.click_main_id;
        var gridRow = window.parent.$('#List0').datagrid("getData").rows[click_main_id];//获取选中的数据;
        var queryStr = "1=1";
        for (n = 0; n < formContent.tableData[tableid_config].mainSubLine.length; n++) {
            queryStr += " and " + formContent.tableData[tableid_config].mainSubLine[n].subGridInput + "='" + gridRow[formContent.tableData[tableid_config].mainSubLine[n].mainGridOutput] + "'";
        }
        ListTable(tableid_config, common.Encrypt(formContent.tableData[tableid_config].tableSql), queryStr);
    }

    //数据区表格
    function ListTable(x, sql, queryStr) {
        var allWidth = -100000000, columnsArr = [], frozenColumnsArr = [], codeDictionary = {}, pageDictionary = {};
        //性能优化数据字典相关数据查询
        var isDictionary = false;
        for (var n = 0; n < formContent.tableData[x].fieldData.length; n++) {
            if (formContent.tableData[x].fieldData[n].dataSource != undefined && parseFloat(formContent.tableData[x].fieldData[n].dataSource).toString() == "NaN"&& formContent.tableData[x].fieldData[n].dataSource != "" && formContent.tableData[x].fieldData[n].editControl != "text" && formContent.tableData[x].fieldData[n].editControl != "logic") {
                isDictionary = true;
                codeDictionary[formContent.tableData[x].fieldData[n].field] = common.Encrypt(formContent.tableData[x].fieldData[n].dataSource);
            }
        }
        if (isDictionary) {
            $.ajaxSettings.async = false;
            apiRoute.pageDictionary(codeDictionary, function (tableData) {
                pageDictionary = tableData;
            });
            $.ajaxSettings.async = true;
        }
        for (var n = 0; n < formContent.tableData[x].fieldData.length; n++) {
            if (n > 0 && formContent.tableData[x].fieldData[n].isMainPageShow == "N") continue;

            var fieldShow = true;        //字段是否显示权限
            for (var m = 0; formPermission && m < formPermission.fieldpermission.length; m++) {//验证字段权限
                if (formContent.tableData[x].tableName == formPermission.fieldpermission[m].table_name &&
                    formContent.tableData[x].fieldData[n].field == formPermission.fieldpermission[m].field_name) {
                    fieldShow = (formPermission.fieldpermission[m].is_deny_view == 1 ? false : true);
                    break;
                }
            };

            var columnsJson = {};
            columnsJson.field = formContent.tableData[x].fieldData[n].field;
            if (formContent.tableData[x].fieldData[n].title && common.getCookie("localLanguage") != "en-US") {
                columnsJson.title = formContent.tableData[x].fieldData[n].title;
            }
            else {
                formContent.tableData[x].fieldData[n].title = formContent.tableData[x].fieldData[n].field;
                columnsJson.title = formContent.tableData[x].fieldData[n].field;
            }
            if (formContent.tableData[x].fieldData[n].width) {
                columnsJson.width = parseInt(formContent.tableData[x].fieldData[n].width);
            }
            else {
                columnsJson.width = 100;
            }
            if (formContent.tableData[x].fieldData[n].align) {
                columnsJson.align = formContent.tableData[x].fieldData[n].align;
            }
            columnsJson.halign = 'center';

            if (fieldShow == false) {//没有权限显示
                columnsJson.permission = "none";
                columnsJson.hidden = true;
            }
            else if (formContent.tableData[x].fieldData[n].isMainPageShow != undefined) {
                columnsJson.hidden = (formContent.tableData[x].fieldData[n].isMainPageShow == "H" || formContent.tableData[x].fieldData[n].isAllowAdd == "H" || formContent.tableData[x].fieldData[n].isAllowEdit == "H");//批量操作时主界面、新增、修改只要有一个设置为隐藏，该字段就隐藏
                columnsJson.permission = "show";
            }
            else {
                columnsJson.permission = "show";
                columnsJson.hidden = true;
            }
            columnsJson.sortable = (formContent.tableData[x].tableSortOrder == "none" ? false : true);
            if (formContent.tableData[x].fieldData[n].style != undefined && formContent.tableData[x].fieldData[n].style != "") {
                columnsJson.style = formContent.tableData[x].fieldData[n].style;
                columnsJson.formatter = function (value) { return eval(this.style)(value) }
            }
            //遍历添加编辑器
            var dataObj = formContent.tableData[x].fieldData[n];
            if (dataObj.controlType) {
                if (dataObj.controlType == 'text') {
                    columnsJson.editor = {
                        type: 'textbox',
                        options: { required: dataObj.isRequired }
                    };
                } else if (dataObj.controlType == 'checkbox') {
                    columnsJson.editor = {
                        type: 'checkbox',
                        options: { on: '1', off: '0' } 
                    };
                } else if (dataObj.controlType == 'number' && dataObj.dataValidate == "Integer") {
                    columnsJson.editor = {
                        type: 'numberbox',
                        options: { required: dataObj.isRequired, min: 0}
                    };
                } else if (dataObj.controlType == 'number') {
                    columnsJson.editor = {
                        type: 'numberbox',
                        options: { required: dataObj.isRequired, min: 0, precision: 5 }
                    };
                } else if (dataObj.controlType == 'selected') {
                    (function (dataObj) {
                        columnsJson.editor = {
                            type: 'combobox',
                            options: {
                                data: pageDictionary[dataObj.field],
                                required: dataObj.isRequired,
                                editable: false,
                                panelHeight: 'auto',
                                panelMaxHeight: 250,
                                valueField: 'id',
                                textField: 'text',
                                onSelect: function (val) {
                                    //console.log(val, dataObj)
                                },
                                onChange: function (newValue, oldValue) {
                                    //console.log(newValue, oldValue)
                                    var row = $('#List').datagrid('getSelected');
                                    var rowIndex = $('#List').datagrid('getRowIndex', row);//获取行号  
                                    var QueryInput=formContent.tableData[x].fieldData
                                    var dataSourceArr = dataObj.dataSource.split(";")
                                    if (dataSourceArr.length > 2 && dataSourceArr[2].length > 0) {
                                        for (var n = 0; dataSourceArr[2] != "AllFieldName" && n < QueryInput.length; n++) {
                                            if (QueryInput[n].field == dataSourceArr[2]) {
                                                var newdataSource = QueryInput[n].dataSource.replace(new RegExp("@" + dataObj.field, "g"), "'" + newValue + "'");
                                                //if (QueryInput[n].editControl == "selected") loadCombobox(n, newdataSource);
                                                var target = $('#List').datagrid('getEditor', { 'index': rowIndex, 'field': QueryInput[n].field }).target;
                                                //target.combobox('reload', url);//联动下拉列表重载
                                                target.combobox({
                                                    loader: function (param, success, error) {
                                                        var whereSql = "";
                                                        if (newdataSource.split(";").length > 3) {
                                                            whereSql = newdataSource.split(";")[3];
                                                        }
                                                        if (newdataSource.split(";").length > 3 && whereSql != "") {
                                                            var sqldict = newdataSource.split(";")[0] + " " + whereSql;
                                                            if (sqldict.indexOf('@') != -1) return;
                                                            apiRoute.pageDictionary({ "dictionaryorsql": common.Encrypt(sqldict) }
                                                                , function (tableDataSet) {
                                                                    var tableData = tableDataSet.dictionaryorsql;
                                                                    success(tableData);
                                                                });
                                                        }
                                                        else {
                                                            success(pageDictionary[QueryInput[n].field]);
                                                        }
                                                    }
                                                });
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        };
                    })(dataObj);
                } else if (dataObj.controlType == 'datebox') {
                    columnsJson.editor = {
                        type: 'datebox',
                        options: { required: dataObj.isRequired }
                    };
                } else if (dataObj.controlType == 'openselect' && dataObj.editControl == 'openselect') {
                    columnsJson.editor = {
                        type: 'seltext',
                        options: { required: dataObj.isRequired, multiline: true, colField: dataObj.field },
                        formatter: function (value, row, index) { if (row && value != undefined && value.length > 12) return value.substring(0, 12) + "..."; else return value; }
                    };
                    openData.push(dataObj);
                };                
            };

            if (formContent.tableData[x].fieldData[n].dataSource != undefined && formContent.tableData[x].fieldData[n].dataSource != "" && formContent.tableData[x].fieldData[n].editControl != "text" && formContent.tableData[x].fieldData[n].editControl != "logic") {
                //性能优化数据字典相关数据从缓存中查询
                SetDictionary(pageDictionary[formContent.tableData[x].fieldData[n].field] ||[])
                function SetDictionary(tableData) {
                    var tableDictionary = {};
                    for (var i = 0; i < tableData.length; i++) {
                        tableDictionary[tableData[i].id] = tableData[i].text;
                    }
                    columnsJson.formatter = function (value, row, index) {
                        if (tableDictionary[value]) {
                            return tableDictionary[value];
                        }
                        else {
                            return value;
                        }
                    }

                    if ($("#General_" + formContent.tableData[x].fieldData[n].field).length > 0) {
                        tableData.unshift({ "id": "*", "text": "==全部==", "selected": true });
                        $("#General_" + formContent.tableData[x].fieldData[n].field).combobox({
                            data: tableData,
                            panelHeight: 'auto',
                            panelMaxHeight: 300,
                            valueField: 'id',
                            textField: 'text',
                            onLoadSuccess: function (data) {
                                $(this).combobox("setValue", "*");
                            }
                        });
                    }
                }
            }
            columnsArr.push(columnsJson);
        }
        var pageListTemp = [10, 20, 30, 40, 50, 100, 200, 500];
        for (var i = 0; $.inArray(parseInt(formContent.tableData[x].pageNum), pageListTemp) == -1 && i < pageListTemp.length; i++) {
            if (parseInt(formContent.tableData[x].pageNum) < parseInt(pageListTemp[i])) {
                pageListTemp.splice(i, 0, formContent.tableData[x].pageNum);
                break;
            }
        }
        if ($.inArray(parseInt(formContent.tableData[x].pageNum), pageListTemp) == -1) pageListTemp.push(formContent.tableData[x].pageNum);

        $(window).resize(function () {
            $('#pageFormTableContainer').layout('resize');
        });
        $('#pageFormTableContainer').layout('panel', 'center').panel({
            onResize: function (width, height) {
                try {
                    $('#left_wrapper').layout('panel', 'west').panel('resize', { width: $('#pageFormTableContainer').layout('panel', 'west')[0].clientWidth - 43 });
                    $('#left_wrapper').layout('resize');

                    $('#List').datagrid('resize', {

                    }).datagrid('resize', {
                        width: width - 3,
                        height: height
                    });
                }
                catch (e) { }
            }
        });

        $('#List').datagrid({
            loader: function (param, success, error) {
                if (param.sql) sql = param.sql;
                if (param.queryStr) queryStr = param.queryStr;
                apiRoute.selectEasyuiTable(dataSourceCode,sql, {
                    "queryStr": queryStr,
                    "pager": param
                }, function (tableData) {
                    success(tableData);
                });
            },
            title: "操作按钮" + JQueryEasyUI.gridToolsMenu("#List", ''),
            width: ($('#pageFormTableContainer').layout('panel', 'center')[0].clientWidth - 3),
            height: SetGridHeightSub($("#buttonList").height()+6),
            methord: 'post',
            //remoteSort: false,
            fitColumns: (formContent.tableData[x].isFitColumns != null ? formContent.tableData[x].isFitColumns : true),
            sortName: formContent.tableData[x].tableSortName,
            sortOrder: (formContent.tableData[x].tableSortOrder == 'asc' ? 'asc' : 'desc'),
            idField: formContent.tableData[x].tableIdField,
            pageSize: formContent.tableData[x].pageNum,
            pageList: pageListTemp,
            pagination: formContent.tableData[x].isPagination,  //是否分页
            striped: true, //奇偶行是否区分
            multiSort: false, //多列排序
            checkOnSelect: !formContent.tableData[x].isMultipleSelected,//如果设置为 true，当用户点击某一行时，则会选中/取消选中复选框。如果设置为 false 时，只有当用户点击了复选框时，才会选中/取消选中复选框。
            selectOnCheck: true,//如果设置为 true，点击复选框将会选中该行。如果设置为 false，选中该行将不会选中复选框
            singleSelect: (!formContent.tableData[x].isMultipleSelected),//单选模式
            rownumbers: formContent.tableData[x].isRownumber,//行号
            rownumbers: true,//行号
            //frozenColumns: [frozenColumnsArr],//设置冻结列
            columns: [columnsArr],
            //工具栏
            onClickRow: function (rowIndex, rowData) {
                for (var n = $("#List").datagrid("getData").rows.length - 1; n >= 0; n--) {
                    $("#List").datagrid("endEdit", n); //结束编辑行
                    if (rowIndex!==n)$("#List").datagrid("cancelEdit", n); //取消编辑行
                }
                $("#List").datagrid("beginEdit", rowIndex);
                //var rows = $("#List").datagrid('getEditingRowIndexs');
                //if (rows.length == 0) {//单击开启编辑行
                //    $("#List").datagrid("beginEdit", rowIndex);
                //}
                //else {
                //    $.messageBoxConfirm("当前行存在必填项，确认取消吗？", ["确定", "取消"], function () {
                //        for (var n = $("#List").datagrid("getData").rows.length - 1; n >= 0; n--) {
                //            $("#List").datagrid("cancelEdit", n); //结束编辑行
                //        }
                //        $("#List").datagrid("beginEdit", rowIndex);
                //    }, function () {
                //        $("#List").datagrid("selectRow", rows[0]);
                //        $("#List").datagrid("beginEdit", rows[0]);
                //    });
                //}
            },
            onClickCell: function (index, field, value) {
                cellIndex = index;
            },
            onBeforeEdit: function (index, row) {
                cellIndex = index;
            }
        });
    }

    $("#btnReturn").click(function () {
        window.parent.frameReturnByClose();
    });

    $("#btnSave,#btnSaveClose").click(function () {
        if (window.RunCheckFormTable && typeof (RunCheckFormTable) == 'function') {
            var checkForm = RunCheckFormTable();
            if (checkForm == false) {
                return;
            }
        }
        $.messageLoad();
        var ctrlID = this.id;
        var btnSqlArr = [], btnJsonArr = [], tIdArr = [];
        var mainSubArr = {}, mainTableRow = window.parent.$('#List0').datagrid('getSelected');  //获取主表关联值
        for (var i = 0; i < formContent.tableData[tableid_config].mainSubLine.length; i++) {
            mainSubArr[formContent.tableData[tableid_config].mainSubLine[i].subGridInput] = mainTableRow[formContent.tableData[tableid_config].mainSubLine[i].mainGridOutput];
        }
        for (var n = $("#List").datagrid("getData").rows.length - 1; n >= 0; n--) {//完成编辑
            $("#List").datagrid("endEdit", n);
        }
        var formJson = $('#List').datagrid("getData").rows;
        if (formJson.length <= 0) {
            $.messageBoxBottom(0, "请填入一条记录！");
            $.messageBoxCloseLoading();
            return false;
        }
        for (var n = formJson.length - 1; n >= 0; n--) {
            for (var m = 0; m < formContent.tableData[tableid_config].fieldData.length; m++) {
                if (formContent.tableData[tableid_config].fieldData[m].isRequired &&
                    (formJson[n][formContent.tableData[tableid_config].fieldData[m].field]==undefined ||
                        formJson[n][formContent.tableData[tableid_config].fieldData[m].field] == "")) {
                    $.messageBoxBottom(0, "字段[" + formContent.tableData[tableid_config].fieldData[m].title + "]必填！");
                    $.messageBoxCloseLoading();
                    return false;
                }
            }//验证必填字段

            if (!formJson[n].t_id) {
                formJson[n].t_id = "";
            }
            else if (formJson[n].t_id != "" && formJson[n].t_id!="0"){
                tIdArr.push(formJson[n].t_id);
            }
            if (JSON.stringify(formJson[n]) === "{\"t_id\":\"\"}") {//移除输入非法字段
                formJson.splice(n, 1);
            }
            for (var key in mainSubArr) {//为每一行赋主表关联数值 
                formJson[n][key] = mainSubArr[key];
            }
            for (var i = 0; i < formContent.tableData[tableid_config].fieldData.length; i++) {  //将数字控件默认为0
                if (formContent.tableData[tableid_config].fieldData[i].controlType && formContent.tableData[tableid_config].fieldData[i].controlType == 'number'
                    && formJson[n][formContent.tableData[tableid_config].fieldData[i].field] == "") {
                    formJson[n][formContent.tableData[tableid_config].fieldData[i].field] = "0";
                }
            }
        }
        if (btnSql == "") {//批量配置增删改
            var insertSql = "", updateSql = "", deleteSql = "";
            var tableid = common.getUrlParam("tableid");
            for (var i = 0; i < buttonid_config; i++) {
                if (formContent.tableData[tableid].buttonData[i].btnId == "Create" && formContent.tableData[tableid].buttonData[i].btnAlign == "table") {
                    insertSql = formContent.tableData[tableid].buttonData[i].btnSql;
                }
                if (formContent.tableData[tableid].buttonData[i].btnId == "Edit" && formContent.tableData[tableid].buttonData[i].btnAlign == "table") {
                    updateSql = formContent.tableData[tableid].buttonData[i].btnSql;
                }
                if (formContent.tableData[tableid].buttonData[i].btnId == "Delete" && formContent.tableData[tableid].buttonData[i].btnAlign == "table") {
                    deleteSql = formContent.tableData[tableid].buttonData[i].btnSql;
                }
            }
            if (deleteSql != "") {
                for (var i = 0; i < deleteSql.split(";").length; i++) {
                    btnJsonArr.push(mainSubArr);
                    var deleteId = tIdArr.join("','");
                    if (tIdArr.length == 0) {
                        deleteId = -1;  //解决Oracle问题
                    }
                    btnSqlArr.push(deleteSql.split(";")[i].replace(/@t_id/g, deleteId));
                }
            }
            if (insertSql != "") {
                for (var i = 0; i < insertSql.split(";").length; i++) {
                    for (var j = 0; j < formJson.length; j++) {
                        if (formJson[j].t_id == "" || formJson[j].t_id == "0") {
                            btnJsonArr.push(formJson[j]);
                            btnSqlArr.push(insertSql.split(";")[i])
                        }
                    }
                }
            }
            if (updateSql != "") {
                for (var i = 0; i < updateSql.split(";").length; i++) {
                    for (var j = 0; j < formJson.length; j++) {
                        if (formJson[j].t_id != "") {
                            btnJsonArr.push(formJson[j]);
                            btnSqlArr.push(updateSql.split(";")[i])
                        }
                    }
                }
            }
        }
        else if (btnSql.split(";").length == 1) {
            for (var i = 0; i < formJson.length; i++) {
                btnJsonArr.push(formJson[i]);
                btnSqlArr.push(btnSql);
            }
        }
        else {
            for (var i = 0; i < formJson.length; i++) {
                for (var j = 0; j < btnSql.split(";").length; j++) {
                    btnJsonArr.push(formJson[i]);
                    btnSqlArr.push(btnSql.split(";")[j]);
                }
            }
        }
        apiRoute.runTaskEvent(dataSourceCode, btnSqlArr.join(";"), btnJsonArr, function (result) {
            var jsonLog = {
                "tableId": parseInt(tableid_config) + 1,
                "formid": formid_config,
                "btnText": '批量操作',
                "json": JSON.stringify({
                    "dataSourceCode": dataSourceCode,
                    "btnSql": btnSqlArr.join(";"),
                    "formJson": btnJsonArr
                })
            };
            common.postGateway("platform/menu/MenuLog", jsonLog, common.getToken());

            if (result.resMsg == "") {
                var count = $('#List').datagrid("getData").rows.length;
                if (result.resCount >= count) {
                    $.messageBoxBottom(1, "共成功操作" + count + "条记录！");
                }
                else {
                    $.messageBoxBottom(1, "共成功操作" + result.resCount + "条记录！");
                }
                window.parent.frameReturnByReload($("#TemplateID").val() == "0" ? tableid_config : false);
                if (ctrlID == "btnSave") {
                    $("#List").datagrid('load');
                }                
                else if (ctrlID == "btnSaveClose") {
                    window.parent.frameReturnByClose();
                }
            }
            else {
                $.messageBoxBottom(0, result.resMsg);
            }
            $.messageBoxCloseLoading();
        });
    });

});
//当数据表格的控件为点击弹框时
function SelDetails(colField) {
    for (var i = 0; i < openData.length; i++) {
        if (openData[i].field == colField) {
            OpenSelectList(openData[i]);
        }      
    }
    function OpenSelectList(data) {
        var dataSource = data.dataSource.split(';')[1] + "&pagelist=selectOpen&tablelist=focusOpen&ctrlName=" + data.field + "&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId");
        $("#modalwindow").html("<iframe id='selectopen_" + data.field + "' width='100%' height='100%'  frameborder='0' src='" + dataSource + "'></iframe>");
        $("#modalwindow").dialog({
            width: 900,
            height: SetGridHeightSub(100),
            closed: false,
            cache: false,
            modal: true,
            buttons: [{
                iconCls: 'icon-save',
                text: ' 选 择 ',
                handler: function () {
                    var userId = [], selectArr = document.getElementById("selectopen_" + data.field).contentWindow.$('#List0').datagrid("getSelections");
                    
                    for (var i = 0; i < selectArr.length; i++) {
                        userId.push(selectArr[i].id);
                    }
                    if (userId.length == 0) {
                        $.messageBoxAlert(0, "对不起，没有选中任何数据！");
                    }
                    else {
                        var parSelect = $('#List').datagrid("getSelections"), parKey = {};
                        var columnFields = $('#List').datagrid("getColumnFields");//获取所有列的字段
                        for (var key in selectArr[0]) {
                            for (var i = 0; i < columnFields.length;i++) {
                                if (key == columnFields[i]) {
                                    parKey[key] = selectArr[0][key];
                                    break;
                                }
                            }
                        }
                        parKey[colField] = userId.join(",").toString();
                        $("#List").datagrid("endEdit", cellIndex);
                        $("#List").datagrid("updateRow", {
                            index: cellIndex,
                            row: parKey
                        });
                        $("#List").datagrid("beginEdit", cellIndex);
                        $("#modalwindow").dialog('close');
                    }
                }
            }, {
                iconCls: 'icon-cancel',
                text: ' 取 消 ',
                handler: function () {
                    $("#modalwindow").dialog('close');
                }
            }]
        });
    }
}
