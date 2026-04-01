var tableId = 0, dataMainField = [], gridId, NtiCloudDbName = "", editorSql = {}, editorSqltables = {};
function functionEditorSql(id, w, h) {
    CodeMirror.keywords = "&DataSource& &AppCode& &CLIENT_ID& &ORG_ID& &WAREHOUSE_ID& &User_Tid& &User_Id& &CREATED_BY& &MODIFY_IP&";  
    editorSql[id] = CodeMirror.fromTextArea(document.getElementById(id), {
        //lineNumbers: true,  //显示行数
        lineWrapping: true, //自动换行
        styleActiveLine: true,   //当前行背景高亮
        matchBrackets: true,    //括号匹配
        continueComments: "Enter",
        mode: "text/x-sql",       //模式
        gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter", "CodeMirror-lint-markers"],
        lint: true,
        hintOptions: {
            completeSingle: false,// 当匹配只有一项的时候是否自动补全
            tables: editorSqltables
        },
        extraKeys: {
            "Ctrl": "autocomplete",
            "Ctrl-/": "toggleComment",
            "F11": function (cm) {
                cm.setOption("fullScreen", !cm.getOption("fullScreen"));
            },
            "Esc": function (cm) {
                if (cm.getOption("fullScreen")) cm.setOption("fullScreen", false);
            }
        }
    });
    editorSql[id].setSize(w,h);
    // 将自动提示绑定到change事件上，这样输入的时候就可以看到联想的关键词
    editorSql[id].on('change', function (instance, change) {
        // 自动补全的时候，也会触发change事件，所有坐下判断，以免死循环，正则是为了不让空格，换行之类的也提示
        // 通过change对象你可以自定义一些规则去判断是否提示
        if (change.origin !== 'complete' && /\w|\./g.test(change.text[0])) {
            instance.showHint();
        }
    })
}

$(function () {
    functionEditorSql("sqlContent", 460, 90);
    $('#conDiv').css({
        'height': SetGridHeightSub(80),
        'overflow-y': 'auto'
    });
    var buttonArray = [], styleArray = [], contorlArray = [], alignArray = [], SHN_Array = [], RHEN_Array = [], validataArray = [], allTableName = [], controlType =[];
    var styleDataID_Text = {}, editControlID_Text = {}, alignTypeID_Text = {},  validatDataID_Text = {};
    $.ajaxSettings.async = false;
    common.postGateway('platform/system/buttonconfig',{
        "category": 'Normally'
    }, common.getToken(), function (data) {
        data = JSON.parse(data.json);
        for (var i = 0; i < data.length; i++){
            buttonArray.push(data[i]);
            buttonArray[i].iconCls = buttonArray[i].button_icon;
        }
    });
    
    apiRoute.dictionaryConfig("WebStyle;WebHtml;align;MainShow;ControlShow;validata", false, function (result) {
        styleArray = result.webstyle;
        contorlArray = result.webhtml;
        alignArray = result.align;
        SHN_Array = result.mainshow;
        RHEN_Array = result.controlshow;
        validataArray = result.validata;
        for (var i = 0; i < styleArray.length; i++) {
            styleDataID_Text[styleArray[i].id] = styleArray[i].text;
        }
        for (var i = 0; i < contorlArray.length; i++) {
            if (contorlArray[i].description) controlType.push(contorlArray[i])
            editControlID_Text[contorlArray[i].id] = contorlArray[i].text;
        }
        for (var i = 0; i < alignArray.length; i++) {
            alignTypeID_Text[alignArray[i].id] = alignArray[i].text;
        }
        for (var i = 0; i < validataArray.length; i++) {
            validatDataID_Text[validataArray[i].id] = validataArray[i].text;
        }
    });

    common.postGateway('platform/table/DbTable', {
        "tableName": ""
    }, common.getToken(), function (data) {
        data = JSON.parse(data.json);
        //NtiCloudDbName = data.dbname;
        NtiCloudDbName = "&DataSource&";
        editorSqltables["&DataSource&"] = [];
        editorSqltables["&AppCode&"] = [];
        editorSqltables["&CLIENT_ID&"] = [];
        editorSqltables["&ORG_ID&"] = [];
        editorSqltables["&WAREHOUSE_ID&"] = [];
        editorSqltables["&User_Tid&"] = [];
        editorSqltables["&User_Id&"] = [];
        editorSqltables["&CREATED_BY&"] = [];
        editorSqltables["&MODIFY_IP&"] = [];
        for (var i = 0; i < data.rows.length; i++) {
            allTableName.push({ "id": data.rows[i].t_id, "text": data.rows[i].table_name, "description": data.rows[i].table_comment });
            if (data.rows[i].column_name) {
                editorSqltables[data.rows[i].table_name] = data.rows[i].column_name.split(",");
                editorSqltables["&DataSource&" + data.rows[i].table_name] = data.rows[i].column_name.split(",");
            }
        }
    });

    $("#printFileName").combobox({
        loader: function (param, success, error) {
            common.postGateway('platform/file/filelist', { from: "Web", fileName: "FastReport" },
                common.getToken(), function (result) {
                    var printFileName = [];
                    for (var i = 0; i < result.json.length; i++) {
                        printFileName.push({id:'FastReport/' + result.json[i].filename});
                    }
                    success(printFileName);
            });
        },
        editable: false ,
        panelHeight: 'auto',
        panelMaxHeight: 150,
        valueField: 'id',
        textField: 'id'
    });

    $.ajaxSettings.async = true;
    if ($("#TemplateID").val() != 6) {
        addGridAction('');
    }

    if ($("#TemplateID").val() == 3 || $("#TemplateID").val() == 5) {//左侧有树时
        $("#treeSetPanelA").show();
        functionEditorSql("treeSql", 900, 55);
        $("#treeSetPanelB").show();
        $("#pageWebPagePanel").hide();
    } else if ($("#TemplateID").val() == 6) {//左侧是树右侧是页面时
        $("#treeSetPanelA").show();
        functionEditorSql("treeSql", 900, 55);
        $("#treeSetPanelB").hide();
        $(".ntiCloud_addGrid").hide();
    }
    else {
        $("#treeSetPanelA").hide();
        $("#treeSetPanelB").hide();
    }
    if ($("#TemplateID").val() == 1 ||$("#TemplateID").val() == 4 || $("#TemplateID").val() == 5) {//主从表结构有从表
        addGridAction('');
        $(".ntiCloud_addGrid").show();
        $(".ntiCloud_addGrid").show();
    }
    else {
        $(".ntiCloud_addGrid").hide();
        $(".ntiCloud_addGrid").hide();
    }

    $("#addGridBtn").on('click', function () { addGridAction('') });
    $("#addTableBtn").on('click', function () { addGridAction('openTable') });
    function addGridAction(tableType) {
        var titleName = '';
        if (tableType == 'openTable') {
            titleName = "批量弹框表";            
        } else {
            titleName = parseInt(tableId) == 0 ? "主表" : "从表" + tableId;
        }
        var GridCon = "";
        GridCon += '<div><div id="gridTable' + tableId + '" class="easyui-panel" title=' + titleName + ' style="width:100%;margin-bottom:10px;" data-options="collapsible:true,closable:' + (parseInt(tableId) < 2 ? "false" : "true") + ',onClose:function(){removeID(' + tableId + ')}"><div style="padding:20px;">';
        GridCon += '<div id="mainPanel' + tableId + '" class="easyui-panel" title="主界面配置" style="width:100%;padding:10px;" data-options="collapsible:true,closable:false">';
        GridCon += '<input id="inputType' + tableId + '" style="display:none;" type="text" value="' + tableType+'" /><span style="display: inline-block;margin-bottom: 20px;">操作表名：</span><select class="easyui-combobox" id="tableName' + tableId + '" data-options="editable: true,width:150,panelHeight: \'auto\',panelMaxHeight: 250"></select>';
        GridCon += '<span style="display: inline-block;margin-left: 20px;">数据查询：</span><span style="display:inline-block;"><textarea id="tableSql' + tableId + '"></textarea></span>';
        GridCon += '<span class="mdi mdi-av-timer easyui-tooltip" title="读取字段信息" onclick="ReadMainSql(' + tableId + ',[],[])" style="font-size:18px;padding:5px;cursor:pointer;"></span><br/>';
        GridCon += '<span>页面名称：</span><input id="pageTitle' + tableId + '" class="easyui-textbox" data-options="width:150" >';
        GridCon += '<span style="display: inline-block;margin-left: 20px;">主键字段：</span><select class="easyui-combobox" id="tableIdField' + tableId + '" data-options="editable:false,width:110,panelHeight: \'auto\',panelMaxHeight: 250" ></select >';
        GridCon += '<span style="display: inline-block;margin-left: 20px;">宽度自适应：</span><input id="isFitColumns' + tableId + '" class="easyui-switchbutton" data-options="onText:\'是\',offText:\'否\',width:45,height:23" checked>';
        GridCon += '<span style="display: inline-block;margin-left: 20px;">显示序号：</span><input id="isRownumber' + tableId + '" class="easyui-switchbutton" data-options="onText:\'是\',offText:\'否\',width:45,height:23" >';
        GridCon += '<span style="display: inline-block;margin-left: 20px;">分页：</span><input id="isPagination' + tableId + '" class="easyui-switchbutton" data-options="onText:\'是\',offText:\'否\',width:45,height:23" checked>';
        GridCon += '<span style="display: inline-block;margin-left: 20px;">多选：</span><input id="isMultipleSelected' + tableId + '" class="easyui-switchbutton" data-options="onText:\'是\',offText:\'否\',width:45,height:23" ><br />';
        GridCon += '<span style="display: inline-block;margin-top: 20px;">排序字段：</span><select class="easyui-combobox" id="tableSortName' + tableId + '" data-options="editable: false,width:150,panelHeight: \'auto\',panelMaxHeight: 250" ></select >';
        GridCon += '<span style="display: inline-block;margin-left: 20px;">排序方式：</span><select class="easyui-combobox" id="tableSortOrder' + tableId + '" data-options="editable: false,width:110,panelHeight: \'auto\',panelMaxHeight: 250" ><option value="asc" selected>升序</option><option value="desc">降序</option><option value="none">不排序</option></select>';
        GridCon += '<span style="display: inline-block;margin-left: 20px;">分页记录数：</span><select class="easyui-combobox" id="pageNum' + tableId + '" data-options="editable: true,width:55,panelHeight: \'auto\',panelMaxHeight: 250" ><option value="10">10</option><option value="20" selected>20</option><option value="30">30</option><option value="40">40</option><option value="50">50</option><option value="100">100</option><option value="200">200</option><option value="500">500</option></select>';

        GridCon += '<span style="display: inline-block;margin-left: 20px;">子页面列数：</span><select class="easyui-combobox" id="multipleColumn' + tableId + '" data-options="editable: false,width:55,panelHeight: \'auto\',panelMaxHeight: 250" ><option value="1" selected>1</option><option value="2">2</option><option value="3">3</option></select>';
        if (tableId == 0) {
            GridCon += '<span id="treeSetPanelB"><span style="display: inline-block;margin-left: 20px;"> 左侧树绑定字段：</span ><select class="easyui-combobox" id="treeBindData" data-options="editable: false,width:120,panelHeight: \'auto\',panelMaxHeight: 250"></select></span>';
        }
        GridCon += '<div style="margin-top: 20px;"><span style="display: inline-block;">页脚sql：</span><span style="display:inline-block;"><textarea id="footerSql' + tableId + '"></textarea></span></div>';
        GridCon += '</div><br />';
        if (tableId > 0) {
            GridCon += '<table id="mainSubLine' + tableId + '" title="" style="width:100%;/*height:100%;*/overflow-y:auto"></table><br />';
        }
        GridCon += '<table id="buttonData' + tableId + '"></table><br /><table id="tableData' + tableId + '"></table></div></div></div>';

        $("#addGridBtn").parent('.ntiCloud_addGrid').before(GridCon);
        //$.parser.parse($(".ntiCloud_addGrid").parent());
        $.parser.parse($("#gridTable" + tableId).parent());

        functionEditorSql("tableSql" + tableId, 660, 50);
        functionEditorSql("footerSql" + tableId, 925, 50);

        button_table(tableId);
        tableId++;
    }
    $("#pageWebPage").datagrid({
        nowrap: true,
        striped: true,
        fitColumns: true,
        remoteSort: false,
        collapsible: true,
        title: "页面配置" + JQueryEasyUI.gridToolsMenu("#pageWebPage", ''),
        animate: true,
        singleSelect: true,
        rownumbers: true,//行号
        columns: [[
            {
                field: 'treeNode', title: '树节点代码', width: 180, align: 'center', editor: {
                    type: "textbox", options: { required: true }
                }
            },
            { field: 'treeBindField', title: '树绑定字段', width: 180, align: 'center', editor: "textbox" },
            {
                field: 'treeUrl', title: '自定义页面', width: 100, align: 'center', editor: {
                    type: 'combobox', options: {
                        loader: function (param, success, error) {
                            apiRoute.selectDataEvent("select a.t_id,a.moduleurl menuurl,a.modulename menuname from prod_app_module a left join prod_app_product_module b on a.modulecode=b.modulecode where a.moduleurl is not null and a.ismenu='0' and (b.APPCODE is null or b.APPCODE='" + common.getUrlParam("AppCode") + "')",
                                 [], "1=1", function (tableData) {
                                for (var i = 0; i < tableData.length; i++) {
                                    if (tableData[i].menuurl.indexOf("Page") == 0) tableData[i].menuurl = "/Admin/" + tableData[i].menuurl;
                                    if (tableData[i].menuurl.indexOf("?") == -1) {
                                        tableData[i].id = tableData[i].menuurl + "?formid=" + tableData[i].t_id;
                                    }
                                    else {
                                        tableData[i].id = tableData[i].menuurl + "&formid=" + tableData[i].t_id;
                                    }
                                }
                                success(tableData);
                            });
                        },
                        panelHeight: 'auto',
                        panelMaxHeight: 250,
                        valueField: 'id',
                        textField: 'menuname'
                    }
                }
            }
        ]],
        onClickRow: function (rowIndex, rowData) {
            for (var n = $("#pageWebPage").datagrid("getData").rows.length - 1; n >= 0; n--) {
                $("#pageWebPage").datagrid("endEdit", n);
            }
            var rows = $("#pageWebPage").datagrid('getEditingRowIndexs');
            if (rows.length == 0) {//单击开启编辑行
                $("#pageWebPage").datagrid("beginEdit", rowIndex);
            }
        }
    });
    function button_table(id) {
        $("#tableName" + id).combobox({
            data: allTableName,
            panelHeight: 'auto',
            panelMaxHeight: 250,
            valueField: 'id',
            textField: 'text',
            formatter: JQueryEasyUI.dataDictionaryFormat
        });

        mainAndSub(id, [], []);

        $("#buttonData" + id).datagrid({
            nowrap: true,
            striped: true,
            fitColumns: true,
            remoteSort: false,
            collapsible: true,
            title: "按钮配置" + JQueryEasyUI.gridToolsMenu("#buttonData" + id, ''),
            animate: true,
            singleSelect: true,
            rownumbers: true,//行号
            columns: [[
                {
                    field: 'btnId', title: '按钮选择', width: 100, align: 'center', editor: {
                        type: 'combobox', options: {
                            editable: false, data: buttonArray, required: true,
                            valueField: 'button_code', textField: 'button_name',
                            showItemIcon: true,
                            panelHeight: 'auto',
                            panelMaxHeight: 240,
                            onSelect: function (data) {
                                var row = $("#buttonData" + id + "").datagrid('getSelected');
                                //row.btnText = data.button_name;
                                var rowIndex, rows = $("#buttonData" + id + "").datagrid('getEditingRowIndexs');
                                if (rows.length == 1) {
                                    rowIndex = rows[0];
                                }
                                else {
                                    rowIndex = $("#buttonData" + id).datagrid('getRowIndex', row); //获取行号
                                }
                                var icon = $("#buttonData" + id).datagrid('getEditor', { 'index': rowIndex, 'field': 'btnIcon' }).target;
                                if (data.btnId != "Config" && data.btnId != "Run") icon.combobox('setValue', data.button_icon)
                                var target = $("#buttonData" + id).datagrid('getEditor', { 'index': rowIndex, 'field': 'btnText' }).target;
                                //var value = target.textbox('getValue');
                                if (data.btnId != "Config" && data.btnId != "Run") target.textbox('setValue', data.button_name)
                            }
                        }
                    }, formatter: function (value, row, index) {
                        for (var i = 0; i < buttonArray.length; i++) {
                            if (buttonArray[i].button_code == value) {
                                return "<span class='" + buttonArray[i].button_icon + "'></span>&nbsp;&nbsp;" + buttonArray[i].button_name
                            }
                        }
                        return value;
                    }
                },
                {
                    field: 'btnIcon', title: '按钮图标', width: 80, align: 'center', editor: {
                        type: 'combobox',
                        options: {
                            loader: function (param, success, error) {
                                apiRoute.selectDataEvent("select iconcss,icontype from prod_system_icon",
                                    [], "1=1", function (tableData) {
                                    for (var i = 0; i < tableData.length; i++) {
                                        tableData[i].iconCls = tableData[i].iconcss;
                                    }
                                    success(tableData);
                                });
                            },
                            editable: false,
                            panelWidth: 300,
                            panelMaxHeight: 250,
                            valueField: 'iconcss',
                            textField: 'iconcss',
                            groupField: 'icontype',
                            showItemIcon: true,
                            groupFormatter: function(group){
                                return '<span style="color:red">' + group + '</span>';
                            }
                        }
                    }, formatter: function (value, row, index) {
                        return "<span class='" + value + "'></span>"
                    }
                },
                { field: 'btnText', title: '按钮名称', width: 80, align: 'center', editor: "textbox" },
                {
                    field: 'btnAlign', title: '按钮位置', width: 100, align: 'center', editor: {
                        type: 'combobox',
                        options: {
                            editable: false,
                            data: [{ id: "left", text: "工具条左边" }, { id: "right", text: "工具条右边" }, { id: "list", text: "记录行按钮" }, { id: "rightlist", text: "列表右击菜单" }, { id: "table", text: "批量操作" }],
                            panelHeight: 'auto',
                            valueField: 'id',
                            textField: 'text',
                            panelMaxHeight: 260,
                            panelWidth: 100
                        }
                    }, align: 'center', formatter: function (value, row, index) {
                        if (row && value != undefined && value == 'left') return '工具条左边';
                        else if (row && value != undefined && value == 'right') return '工具条右边';
                        else if (row && value != undefined && value == 'list') return '记录行按钮';
                        else if (row && value != undefined && value == 'rightlist') return '列表右击菜单';
                        else if (row && value != undefined && value == 'table') return '批量操作';
                        else return value;
                    }
                },
                { field: 'btnSql', title: '按钮SQL', width: 300, halign: 'center', align: 'left', editor: { type: 'codemirrorSqlEdit', options: { tableId: id} }, formatter: function (value, row, index) { if (row && value != undefined && value.length > 50) return value.substring(0, 50) + "..."; else return value;}},
                { field: 'btnUrl', title: '自定义页面', width: 100, align: 'center', editor: {
                    type: 'combobox', options: {
                        loader: function (param, success, error) {
                            apiRoute.selectDataEvent("select a.t_id,a.moduleurl menuurl,a.modulename menuname from prod_app_module a left join prod_app_product_module b on a.modulecode=b.modulecode where a.moduleurl is not null and a.ismenu='0' and (b.APPCODE is null or b.APPCODE='" + common.getUrlParam("AppCode") + "')",
                                [], "1=1", function (tableData) {
                                for (var i = 0; i < tableData.length; i++) {
                                    if (tableData[i].menuurl.indexOf("Page") == 0) tableData[i].menuurl = "/Admin/" + tableData[i].menuurl;
                                    if (tableData[i].menuurl.indexOf("?") == -1) {
                                        tableData[i].id = tableData[i].menuurl + "?formid=" + tableData[i].t_id;
                                    }
                                    else {
                                        tableData[i].id = tableData[i].menuurl + "&formid=" + tableData[i].t_id;
                                    }
                                }
                                success(tableData);
                            });
                        },
                        panelHeight: 'auto',
                        panelMaxHeight: 250,
                        valueField: 'id',
                        textField: 'menuname'
                    }
                } 
            }
            ]],
            tools: [{
                iconCls: 'mdi mdi-autorenew',
                handler: function () {
                    for (var n = $("#buttonData" + id).datagrid("getData").rows.length - 1; n >= 0; n--) {
                        $("#buttonData" + id).datagrid("endEdit", n);
                    }
                    var sql_insert_arg = "'&CLIENT_ID&','&ORG_ID&',";
                    var sql_insert = "INSERT INTO " + NtiCloudDbName + $("#tableName" + id).textbox("getValue") + " (CLIENT_ID,ORG_ID,";
                    var sql_update = "UPDATE " + NtiCloudDbName + $("#tableName" + id).textbox("getValue") + " SET CLIENT_ID= '&CLIENT_ID&',ORG_ID= '&ORG_ID&',";
                    var sql_delete = "DELETE FROM " + NtiCloudDbName + $("#tableName" + id).textbox("getValue") + " WHERE "
                    for (var i = 0; i < $("#tableData" + id).datagrid("getData").rows.length; i++) {
                        if ($("#tableData" + id).datagrid("getData").rows[i].isAllowAdd != "N") {
                            sql_insert += " " + $("#tableData" + id).datagrid("getData").rows[i].field + " ,";
                            if ($("#tableData" + id).datagrid("getData").rows[i].editControl == "datebox") {
                                sql_insert_arg += "case when @" + $("#tableData" + id).datagrid("getData").rows[i].field +"='' then null else @" + $("#tableData" + id).datagrid("getData").rows[i].field+" end,";
                            }
                            else {
                                sql_insert_arg += "@" + $("#tableData" + id).datagrid("getData").rows[i].field+",";
                            }
                        }
                        if ($("#tableData" + id).datagrid("getData").rows[i].isAllowEdit != "N" && $("#tableData" + id).datagrid("getData").rows[i].field != $("#tableIdField" + id).combobox("getValue")) {
                            if ($("#tableData" + id).datagrid("getData").rows[i].editControl == "datebox") {
                                sql_update += $("#tableData" + id).datagrid("getData").rows[i].field + "=case when @" + $("#tableData" + id).datagrid("getData").rows[i].field +"='' then null else @" + $("#tableData" + id).datagrid("getData").rows[i].field +" end,";
                            }
                            else {
                                sql_update += $("#tableData" + id).datagrid("getData").rows[i].field + "=@" + $("#tableData" + id).datagrid("getData").rows[i].field +",";
                            }
                        }
                    }
                    sql_insert = sql_insert + "CREATED_BY,MODIFY_IP,CREATE_DATE,ACT_STATUS) VALUES (" + sql_insert_arg + "@created_by, @modify_ip,now(),'1')";
                    sql_update = sql_update + "LAST_MODIFIED_BY=@last_modified_by,MODIFY_IP=@modify_ip,MODIFY_DATE=now() WHERE " + $("#tableIdField" + id).combobox("getValue") + "=@" + $("#tableIdField" + id).combobox("getValue");
                    for (var i = 0; i < $("#buttonData" + id).datagrid("getData").rows.length; i++) {
                        if ($("#buttonData" + id).datagrid("getData").rows[i].btnId == "Create" || $("#buttonData" + id).datagrid("getData").rows[i].btnId == "CreateItem") {
                            $("#buttonData" + id).datagrid("getData").rows[i].btnSql = sql_insert;
                        }
                        else if ($("#buttonData" + id).datagrid("getData").rows[i].btnId == "Edit") {
                            $("#buttonData" + id).datagrid("getData").rows[i].btnSql = sql_update;
                        }
                        else if ($("#buttonData" + id).datagrid("getData").rows[i].btnId == "Delete" && $("#buttonData" + id).datagrid("getData").rows[i].btnAlign == "table") {
                            var sqlmainSub = "";
                            for (var j = 0; j < $("#mainSubLine" + id).datagrid("getData").rows.length; j++) {
                                sqlmainSub += " and " + $("#mainSubLine" + id).datagrid("getData").rows[j].subGridInput+"=@" + $("#mainSubLine" + id).datagrid("getData").rows[j].subGridInput;
                            }
                            $("#buttonData" + id).datagrid("getData").rows[i].btnSql = sql_delete + $("#tableIdField" + id).combobox("getValue") + " not in ('@" + $("#tableIdField" + id).combobox("getValue") + "')" + sqlmainSub;
                        }
                        else if ($("#buttonData" + id).datagrid("getData").rows[i].btnId == "Delete") {
                            $("#buttonData" + id).datagrid("getData").rows[i].btnSql = sql_delete + $("#tableIdField" + id).combobox("getValue") + "=@" + $("#tableIdField" + id).combobox("getValue");
                        }
                        $("#buttonData" + id).datagrid("refreshRow", i);
                    }
                }
            }],
            //工具栏
            onClickRow: function (rowIndex, rowData) {
                for (var n = $("#buttonData" + id).datagrid("getData").rows.length - 1; n >= 0; n--) {
                    $("#buttonData" + id).datagrid("endEdit", n);
                }
                var rows = $("#buttonData" + id).datagrid('getEditingRowIndexs');
                if (rows.length == 0) {//单击开启编辑行
                    $("#buttonData" + id).datagrid("beginEdit", rowIndex);
                }
            }
        });

        $("#tableData" + id).datagrid({
            nowrap: true,
            striped: true, //奇偶行是否区分
            title: "字段配置" + JQueryEasyUI.gridToolsMenu("#tableData" + id, ''),
            data: [],
            fitColumns: true,
            collapsible: true,
            remoteSort: false,
            animate: true,
            singleSelect: true,
            loadMsg: "loading...",
            rownumbers: true,//行号
            columns: [
                [
                    { field: 'field', title: '字段名称', width: 100, halign: 'center', editor: 'text', "rowspan": 2 },
                    { field: 'title', title: '标题', width: 75, halign: 'center', editor: 'text', "rowspan": 2 },
                    { field: 'note', title: '备注<br>说明', width: 65, halign: 'center', editor: 'text', "rowspan": 2 },
                    { field: 'mainPage', title: '主界面', align: 'center', "colspan": 5 },
                    { field: 'show', title: '显示', align: 'center', "colspan": 4 },
                    { field: 'validate', title: '数据验证', align: 'center', "colspan": 2 },
                    {
                        field: 'editControl', title: '编辑控件', width: 70, align: 'center', "rowspan": 2,
                        editor: {
                            type: 'combobox',
                            options: {
                                editable: false,
                                data: contorlArray,
                                panelHeight: 'auto',
                                valueField: 'id',
                                textField: 'text',
                                panelMaxHeight: 260,
                                panelWidth: 100
                            }
                        }, align: 'center', formatter: function (value, row, index) { if (row && value != undefined) return editControlID_Text[value]; else return value; }
                    },
                    { field: 'dataSource', title: '数据源', align: 'center', width: 100, "rowspan": 2, editor: { type: 'seltext', options: { multiline: true } }, formatter: function (value, row, index) { if (row && value != undefined && value.length > 12) return value.substring(0, 12) + "..."; else return value; } },
                    { field: 'isGeneralQuery', title: '普通<br>查询', width: 45, "rowspan": 2, align: 'center', editor: { type: 'checkbox', options: { on: '1', off: '0' } }, formatter: function (value) { return IfFormatter(value) } },
                    { field: 'isAdvanceQuery', title: '高级<br>查询', width: 45, "rowspan": 2, align: 'center', editor: { type: 'checkbox', options: { on: '1', off: '0' } }, formatter: function (value) { return IfFormatter(value) } }
                ],
                [
                    { field: 'isFrozen', title: '固定列', width: 55, align: 'center', editor: { type: 'checkbox', options: { on: '1', off: '0' } }, formatter: function (value) { return IfFormatter(value) } },
                    {
                        field: 'style', title: '样式', align: 'center', width: 65, editor: 'text',
                        editor: {
                            type: 'combobox',
                            options: {
                                editable: false,
                                data: styleArray,
                                panelHeight: 'auto',
                                panelWidth: 100,
                                valueField: 'id',
                                textField: 'text',
                                formatter: JQueryEasyUI.dataDictionaryFormat
                            }
                        }, align: 'center', formatter: function (value, row, index) { if (row && value != undefined) return styleDataID_Text[value]; else return value; }
                    },
                    { field: 'width', title: '宽度', editor: 'numberbox', width: 40, align: 'center' },
                    {
                        field: 'align', title: '对齐', width: 40, align: 'center',
                        editor: {
                            type: 'combobox',
                            options: {
                                editable: false,
                                data: alignArray,
                                panelHeight: 'auto',
                                valueField: 'id',
                                textField: 'text',
                                formatter: JQueryEasyUI.dataDictionaryFormat
                            }
                        }, align: 'center', formatter: function (value, row, index) { if (row && value != undefined) return alignTypeID_Text[value]; else return value; }
                    },
                    {
                        field: 'isMainPageShow', title: '显示', align: 'center', width: 50,
                        editor: {
                            type: 'combobox',
                            options: {
                                editable: false,
                                data: SHN_Array,
                                panelHeight: 'auto',
                                valueField: 'id',
                                textField: 'text',
                                formatter: JQueryEasyUI.dataDictionaryFormat
                            }
                        }, align: 'center', formatter: ShowFormatter
                    },
                    {
                        field: 'isAllowShow', title: '查看', align: 'center', width: 50,
                        editor: {
                            type: 'combobox',
                            options: {
                                editable: false,
                                data: SHN_Array,
                                panelHeight: 'auto',
                                valueField: 'id',
                                textField: 'text',
                                formatter: JQueryEasyUI.dataDictionaryFormat
                            }
                        }, align: 'center', formatter: ShowFormatter
                    },
                    {
                        field: 'isAllowAdd', title: '新增', width: 50, align: 'center',
                        editor: {
                            type: 'combobox',
                            options: {
                                editable: false,
                                data: RHEN_Array,
                                panelHeight: 'auto',
                                valueField: 'id',
                                textField: 'text',
                                formatter: JQueryEasyUI.dataDictionaryFormat
                            }
                        }, align: 'center', formatter: ShowFormatter
                    },
                    {
                        field: 'isAllowEdit', title: '修改', width: 50, align: 'center',
                        editor: {
                            type: 'combobox',
                            options: {
                                editable: false,
                                data: RHEN_Array,
                                panelHeight: 'auto',
                                valueField: 'id',
                                textField: 'text',
                                formatter: JQueryEasyUI.dataDictionaryFormat
                            }
                        }, align: 'center', formatter: ShowFormatter
                    },
                    {
                        field: 'controlType', title: '控件', width: 60, align: 'center',
                        editor: {
                            type: 'combobox',
                            options: {
                                editable: false,
                                data: controlType,
                                panelHeight: 'auto',
                                panelWidth: 100,
                                valueField: 'id',
                                textField: 'text'
                            }
                        }, align: 'center', formatter: function (value, row, index) {
                            if (row && value != undefined) return editControlID_Text[value]; else return value;
                        }
                    },
                    { field: 'isRequired', title: '必填', width: 50, align: 'center', editor: { type: 'checkbox', options: { on: '1', off: '0' } }, formatter: function (value) { return IfFormatter(value) } },
                    {
                        field: 'dataValidate', title: '验证规则', width: 65, align: 'center',
                        editor: {
                            type: 'combobox',
                            options: {
                                editable: false,
                                data: validataArray,
                                panelHeight: 'auto',
                                valueField: 'id',
                                textField: 'text',
                                formatter: JQueryEasyUI.dataDictionaryFormat
                            }
                        }, align: 'center', formatter: function (value, row, index) { if (row && value != undefined) return validatDataID_Text[value]; else return value; }
                    }
                ]],
            onClickRow: function (rowIndex, rowData) {
                //单击开启编辑行
                for (var n = $("#tableData" + id).datagrid("getData").rows.length - 1; n >= 0; n--) {
                    $("#tableData" + id).datagrid("endEdit", n);
                }
                var rows = $("#tableData" + id).datagrid('getEditingRowIndexs');
                if (rows.length == 0) {//单击开启编辑行
                    $("#tableData" + id).datagrid("beginEdit", rowIndex);
                }
            },
            onBeginEdit: function (index, row) {
                gridId = "#tableData" + id;
            }
        });

        $("#pageNum" + id).combobox({
            onChange: function (newValue, oldValue) {
                if (newValue) {
                    newValue = newValue.replace(/\D/gi, '');
                    if (newValue == '') newValue = oldValue;
                    $("#pageNum" + id).combobox("setValue", newValue);
                }
                else {
                    $("#pageNum" + id).combobox("setValue", oldValue);
                }
            }
        });
    }

    var PageContent = window.parent.$("#listJson").val();
    if (PageContent != "") {
        var menuJson = JSON.parse(PageContent);
        $("#isPrintPage").switchbutton({ checked: (menuJson.isPrintPage && menuJson.isPrintPage == "1" ? true : false) });
        if (menuJson.isPrintPage === "1") {
            $('#frxTemplate').show()
        }
        if (menuJson.printBtnName && menuJson.printBtnName != "") {
            $("#printBtnName").textbox("setValue", menuJson.printBtnName);
        }
        $("#printFileName").combobox("setValue", menuJson.printFileName);
        
        if (($("#TemplateID").val() == 3 || $("#TemplateID").val() == 5 || $("#TemplateID").val() == 6) && menuJson.treeData) {//左侧有树时
            $.setValue(editorSql["treeSql"], menuJson.treeData.treeSql);
            $("#treeId").combobox("setValue", menuJson.treeData.treeId);
            $("#treePid").combobox("setValue", menuJson.treeData.treePid);
            $("#treeText").combobox("setValue", menuJson.treeData.treeText);
            $("#treeControlInitMode").combobox("setValue", menuJson.treeData.treeControlInitMode);

            if ($("#TemplateID").val() == 6) {
                $("#pageWebPage").datagrid("loadData", menuJson.treeData.pageWebData);
            }
            else {
                $("#treeBindData").combobox("setValue", menuJson.treeData.treeBindData);
            }
            ReadTreeSql();
        }
        for (var i = 0; i < menuJson.tableData.length; i++) {
            if (tableId == i) {
                if (menuJson.tableData[i].tableType == '') {
                    addGridAction('');
                } else {
                    addGridAction('openTable');
                }
            } 
            $("#inputType" + i).val(menuJson.tableData[i].tableType);
            $("#pageTitle" + i).textbox("setValue", menuJson.tableData[i].pageTitle);
            $("#tableName" + i).combobox("setValue", menuJson.tableData[i].tableName);
            $.setValue(editorSql["tableSql" + i], menuJson.tableData[i].tableSql);
            $("#tableIdField" + i).combobox("setValue", menuJson.tableData[i].tableIdField);
            $("#tableSortName" + i).combobox("setValue", menuJson.tableData[i].tableSortName);
            $("#tableSortOrder" + i).combobox("setValue", menuJson.tableData[i].tableSortOrder);
            $("#isFitColumns" + i).switchbutton({ checked: menuJson.tableData[i].isFitColumns });
            $("#isRownumber" + i).switchbutton({ checked: menuJson.tableData[i].isRownumber });
            $("#isPagination" + i).switchbutton({ checked: menuJson.tableData[i].isPagination });
            $("#isMultipleSelected" + i).switchbutton({ checked: menuJson.tableData[i].isMultipleSelected });
            $("#multipleColumn" + i).combobox("setValue", menuJson.tableData[i].multipleColumn);
            $("#buttonData" + i).datagrid("loadData", menuJson.tableData[i].buttonData);
            $("#pageNum" + i).combobox("setValue", menuJson.tableData[i].pageNum);
            $.setValue(editorSql["footerSql" + i], menuJson.tableData[i].footerSql);
            if (i > 0) {
                ReadMainSql(i, menuJson.tableData[i].fieldData, menuJson.tableData[i].mainSubLine);
            }
            else {
                ReadMainSql(i, menuJson.tableData[i].fieldData, []);
            }
        }
    }

    //返回按钮
    $("#btnReturn").click(function () {
        window.parent.frameReturnByClose();
    });

    $('#btnSave,#btnSaveClose').on('click', function () {
        var jsonData = {};
        jsonData.isPrintPage = (new Boolean($("#isPrintPage").switchbutton("options").checked) == true ? "1" : "0");
        jsonData.printBtnName = $("#printBtnName").textbox("getValue");
        jsonData.printFileName = $("#printFileName").combobox("getValue");
        if ($("#TemplateID").val() == 3 || $("#TemplateID").val() == 5 || $("#TemplateID").val() == 6) {//左侧有树时
            jsonData.treeData = {};
            jsonData.treeData.treeSql = editorSql["treeSql"].getValue();
            jsonData.treeData.treeId = $("#treeId").combobox("getValue");
            jsonData.treeData.treePid = $("#treePid").combobox("getValue");
            jsonData.treeData.treeText = $("#treeText").combobox("getValue");
            jsonData.treeData.treeControlInitMode = $("#treeControlInitMode").combobox("getValue");
            
            if ($("#TemplateID").val() == 6) {
                for (var n = 0; n < $("#pageWebPage").datagrid("getData").rows.length; n++) {
                    $("#pageWebPage").datagrid("endEdit", n);
                }
                jsonData.treeData.pageWebData = $("#pageWebPage").datagrid("getData").rows;
            } else {
                jsonData.treeData.treeBindData = $("#treeBindData").combobox("getValue");
            }
        }

        jsonData.tableData = [],tableAll=[],openAll=[];
        buttonConfig = [], fieldConfig = [];
        for (var x = 0; x < tableId; x++) {
            if ($("#mainPanel" + x).length == 0) continue;
            for (var n = 0; n < $("#buttonData" + x).datagrid("getData").rows.length; n++) {
                $("#buttonData" + x).datagrid("endEdit", n);
                buttonConfig.push({
                    "t_id": "0",
                    "menu_id": $("#formId").val(),
                    "button_owner": x,
                    "button_code": $("#buttonData" + x).datagrid("getData").rows[n].btnId+"_"+n,
                    "button_caption": ($("#buttonData" + x).datagrid("getData").rows[n].btnText ? $("#buttonData" + x).datagrid("getData").rows[n].btnText : $("#buttonData" + x).datagrid("getData").rows[n].btnId)
                });
            }
            for (var n = 0; n < $("#tableData" + x).datagrid("getData").rows.length; n++) {
                $("#tableData" + x).datagrid("endEdit", n);
                fieldConfig.push({
                    "t_id": "0",
                    "menu_id": $("#formId").val(),
                    "table_name": $("#tableName" + x).combobox("getValue"),
                    "field_name": $("#tableData" + x).datagrid("getData").rows[n].field,
                    "field_title": ($("#tableData" + x).datagrid("getData").rows[n].title ? $("#tableData" + x).datagrid("getData").rows[n].title : $("#tableData" + x).datagrid("getData").rows[n].field)
                });
            }
            if ($("#inputType" + x).val()=='') {
                tableAll.push({});
                tableAll[tableAll.length - 1].tableType = $("#inputType" + x).val();
                tableAll[tableAll.length - 1].pageTitle = $("#pageTitle" + x).textbox("getValue");
                tableAll[tableAll.length - 1].tableName = $("#tableName" + x).combobox("getValue");
                tableAll[tableAll.length - 1].tableSql = editorSql["tableSql" + x].getValue();
                tableAll[tableAll.length - 1].tableIdField = $("#tableIdField" + x).combobox("getValue");
                tableAll[tableAll.length - 1].isFitColumns = new Boolean($("#isFitColumns" + x).switchbutton("options").checked);
                tableAll[tableAll.length - 1].isRownumber = new Boolean($("#isRownumber" + x).switchbutton("options").checked);
                tableAll[tableAll.length - 1].isPagination = new Boolean($("#isPagination" + x).switchbutton("options").checked);
                tableAll[tableAll.length - 1].isMultipleSelected = new Boolean($("#isMultipleSelected" + x).switchbutton("options").checked);
                tableAll[tableAll.length - 1].multipleColumn = $("#multipleColumn" + x).combobox("getValue");
                tableAll[tableAll.length - 1].tableSortName = $("#tableSortName" + x).combobox("getValue");
                tableAll[tableAll.length - 1].tableSortOrder = $("#tableSortOrder" + x).combobox("getValue");
                tableAll[tableAll.length - 1].pageNum = $("#pageNum" + x).combobox("getValue");
                tableAll[tableAll.length - 1].footerSql = editorSql["footerSql" + x].getValue();
                tableAll[tableAll.length - 1].buttonData = $("#buttonData" + x).datagrid("getData").rows;
                tableAll[tableAll.length - 1].fieldData = $("#tableData" + x).datagrid("getData").rows;
                if (x > 0) {
                    for (var n = 0; n < $("#mainSubLine" + x).datagrid("getData").rows.length; n++) {
                        $("#mainSubLine" + x).datagrid("endEdit", n);
                    }
                    tableAll[tableAll.length - 1].mainSubLine = $("#mainSubLine" + x).datagrid("getData").rows;
                }
            }
            if ($("#inputType" + x).val() == 'openTable') {
                openAll.push({});
                openAll[openAll.length - 1].tableType = $("#inputType" + x).val();
                openAll[openAll.length - 1].pageTitle = $("#pageTitle" + x).textbox("getValue");
                openAll[openAll.length - 1].tableName = $("#tableName" + x).combobox("getValue");
                openAll[openAll.length - 1].tableSql = editorSql["tableSql" + x].getValue();
                openAll[openAll.length - 1].tableIdField = $("#tableIdField" + x).combobox("getValue");
                openAll[openAll.length - 1].isFitColumns = new Boolean($("#isFitColumns" + x).switchbutton("options").checked);
                openAll[openAll.length - 1].isRownumber = new Boolean($("#isRownumber" + x).switchbutton("options").checked);
                openAll[openAll.length - 1].isPagination = new Boolean($("#isPagination" + x).switchbutton("options").checked);
                openAll[openAll.length - 1].isMultipleSelected = new Boolean($("#isMultipleSelected" + x).switchbutton("options").checked);
                openAll[openAll.length - 1].multipleColumn = $("#multipleColumn" + x).combobox("getValue");
                openAll[openAll.length - 1].tableSortName = $("#tableSortName" + x).combobox("getValue");
                openAll[openAll.length - 1].tableSortOrder = $("#tableSortOrder" + x).combobox("getValue");
                openAll[openAll.length - 1].pageNum = $("#pageNum" + x).combobox("getValue");
                openAll[openAll.length - 1].footerSql = editorSql["footerSql" + x].getValue();
                openAll[openAll.length - 1].buttonData = $("#buttonData" + x).datagrid("getData").rows;
                openAll[openAll.length - 1].fieldData = $("#tableData" + x).datagrid("getData").rows;
                if (x > 0) {
                    for (var n = 0; n < $("#mainSubLine" + x).datagrid("getData").rows.length; n++) {
                        $("#mainSubLine" + x).datagrid("endEdit", n);
                    }
                    openAll[openAll.length - 1].mainSubLine = $("#mainSubLine" + x).datagrid("getData").rows;
                }
            }
        }
        jsonData.tableData = tableAll.concat(openAll);
        var ctrlID = this.id;
        var jsonArray = [{
            t_id: $("#formId").val(),
            modulejson: JSON.stringify(jsonData),
            modulehtml: "",
            modulejscript: parent.editorJs.getValue()
        },
            { t_id: $("#formId").val() },
            { t_id: $("#formId").val() }];
        for (var i = 0; i < buttonConfig.length; i++) {
            jsonArray.push(buttonConfig[i]);
        }
        for (var i = 0; i < fieldConfig.length; i++) {
            jsonArray.push(fieldConfig[i]);
        }
        common.postGateway('platform/menu/savewebconfig', {
            json: jsonArray,
            projectSet: parent.$("#projectSet").combobox('getValue'),
            buttonCount: buttonConfig.length,
            fieldCount: fieldConfig.length
        }, common.getToken(), function (result) {
            if (result.json == "OK") {
                apiRoute.initializationSystem(4,  function (userinfo) {
                    sessionStorage["encrypt"] = userinfo.json.encrypt;
                });
                window.parent.$("#listJson").val(JSON.stringify(jsonData));
                window.parent.frameReturnByMenuLoad($("#formId").val());
                if (ctrlID == "btnSaveClose") {
                    window.parent.frameReturnByClose();
                }
            }
            else {
                $.messageBoxBottom(0,result.json);
            }
        });
    });
    $('#isPrintPage').switchbutton({
        onChange: function (checked) {
            if (checked == true) {
                $('#frxTemplate').show()
            } else {
                $('#frxTemplate').hide()
            }
        }
    })
    //点击编辑SQL的取消与确定
    $("#sqlCancleBtn").on('click', function () {
        $("#SQLList").window('close');
    });
    $("#sqlSureBtn").on('click', function () {
        $("#SQLList").window('close');
        var row = $(gridId).datagrid('getRowIndex', $(gridId).datagrid('getSelected'));
        var sqlData = editorSql["sqlContent"].getValue();
        var selectPage = $("#openSelectPage").combobox("getValue");
        var bindField = $("#bindField").combobox("getValue");
        var sqlWhere = $("#sqlWhere").textbox("getValue");
        if ($("#isEditable").is(":checked") && ($(gridId).datagrid('getSelected').editControl == "selected" || $(gridId).datagrid('getSelected').editControl == "multipleselect" || $(gridId).datagrid('getSelected').editControl == "tree")) {
            if ($(gridId).datagrid('getSelected').editControl == "selected" || $(gridId).datagrid('getSelected').editControl == "tree") {
                sqlData = sqlData + ";true" + ";" + bindField + ";" + sqlWhere;
            }
            else if ($(gridId).datagrid('getSelected').editControl == "multipleselect") {
                sqlData = sqlData + ";true";
            }
        }
        else if ($(gridId).datagrid('getSelected').editControl == "selected" || $(gridId).datagrid('getSelected').editControl == "tree") {
            sqlData = sqlData + ";;" + bindField + ";" + sqlWhere;
        }
        else if ($(gridId).datagrid('getSelected').editControl == "openselect" || $(gridId).datagrid('getSelected').editControl == "leftlist") {
            sqlData = sqlData + ";" +selectPage;
        }
        $(gridId).datagrid("updateRow", {
            index: row,
            row: {
                dataSource: sqlData
            }
        });
    });
    $("#logicId").combobox({
        data: [{ value: "0", "text": "默认否" }, { value: "1", "text": "默认是" }],
        onClick: function (data) {
            $.setValue(editorSql["sqlContent"], data.value);
        }
    });
    $("#sequencenoId").combobox({
        loader: function (param, success, error) {
            apiRoute.selectDataEvent("select seq_no_name from prod_config_sequence", [], "1=1", function (tableData) {
                success(tableData);
            });
        },
        required: true,
        editable: false,
        panelHeight: 'auto',
        panelMaxHeight: 250,
        valueField: 'seq_no_name',
        textField: 'seq_no_name',
        onClick: function (record) {
            $.setValue(editorSql["sqlContent"], "GetSequenceNo('" + record.seq_no_name + "')");
        }
    });
    $("#dictionaryId").combotreegrid({
        loader: function (param, success, error) {
            apiRoute.selectTreeEvent("","select t_id ,p_id ,dict_code ,dict_name,description ,sort_no from prod_system_dictionary a where (a.appcode='' or a.appcode='" + common.getUrlParam("AppCode")+"') and exists(select * from prod_system_dictionary b where a.t_id=b.p_id and (b.appcode='' or b.appcode='" + common.getUrlParam("AppCode")+"')) order by sort_no",
                "t_id", "p_id", "", "0", [], function (tableData) {
                    success(tableData);
            });
        },
        required: true,
        panelWidth: 500,
        panelHeight: 300,
        idField: 'id',
        treeField: 'dict_name',
        columns: [[
        { field: 'dict_name', title: '字典名称', width: 200, halign: 'center' },
        { field: 'dict_code', title: '字典编码', width: 200, halign: 'center' },
        { field: 'description', title: '描述', width: 150, halign: 'center' }
        ]],
        fitColumns: true,
        onClickRow: function (row) {
            $.setValue(editorSql["sqlContent"], "GetDictionary('" + row.id + "')");
        }
    });
    $('#openSelectPage').combobox({
        loader: function (param, success, error) {
            apiRoute.selectDataEvent("select a.t_id,a.moduleurl menuurl,a.modulename menuname from prod_app_module a left join prod_app_product_module b on a.modulecode=b.modulecode where a.moduleurl is not null and a.ismenu='0' and (b.APPCODE is null or b.APPCODE='" + common.getUrlParam("AppCode") + "')",
                 [], "1=1", function (tableData) {
                for (var i = 0; i < tableData.length; i++) {
                    if (tableData[i].menuurl.indexOf("Page") == 0) tableData[i].menuurl = "/Admin/" + tableData[i].menuurl;
                    if (tableData[i].menuurl.indexOf("?") == -1) {
                        tableData[i].id = tableData[i].menuurl + "?formid=" + tableData[i].t_id;
                    }
                    else {
                        tableData[i].id = tableData[i].menuurl + "&formid=" + tableData[i].t_id;
                    }
                }
                success(tableData);
            });
        },
        panelHeight: 'auto',
        panelMaxHeight: 250,
        valueField: 'id',
        textField: 'menuname'
    })
});

function ReadTreeSql() {
    if (!editorSql["treeSql"].getValue()) return;
    apiRoute.fieldList(editorSql["treeSql"].getValue(), function (resultitem) {
        if (resultitem.isSucceed == false) {
            $.messageBoxBottom(0, resultitem.errmsg);
            return;
        }
        var tableData = [], result = resultitem.json.fieldName;
        for (var i = 0; i < result.length; i++) {
            var fieldTitle = resultitem.json.fieldNameTitle[result[i]];
            if (fieldTitle == null || fieldTitle == "") fieldTitle = result[i];
            tableData.push({ "value": result[i], "text": fieldTitle });
        }
        var treeId = $("#treeId").combobox("getValue");
        var treePid = $("#treePid").combobox("getValue");
        var treeText = $("#treeText").combobox("getValue");
        $("#treeId,#treePid,#treeText").combobox({
            data: tableData
        });
        $("#treeId").combobox("setValue", treeId);
        $("#treePid").combobox("setValue", treePid);
        $("#treeText").combobox("setValue", treeText);
    });
}

function ReadMainSql(id, gridData, mainSubGrid) {
    var editorSql_id = editorSql["tableSql" + id].getValue();
    if ((!$("#tableName" + id).textbox("getValue")) && (!editorSql_id)) return;
    if (!editorSql_id) {
        $.setValue(editorSql["tableSql" + id], "select * from " + NtiCloudDbName + $("#tableName" + id).textbox("getValue") + " where CLIENT_ID='&CLIENT_ID&' and ORG_ID='&ORG_ID&'");
    }
    editorSql_id = editorSql["tableSql" + id].getValue();
    apiRoute.fieldList(editorSql_id, function (resultitem) {
        if (resultitem.isSucceed == false) {
            $.messageBoxBottom(0, resultitem.errmsg);
            return;
        }
        var tableData = [], fieldData = [], dataSubField = [], result = resultitem.json.fieldName;
        for (var i = 0; i < result.length; i++) {
            var fieldTitle = resultitem.json.fieldNameTitle[result[i]];
            if (fieldTitle == null || fieldTitle == "") fieldTitle = result[i];
            fieldData.push({ "fieldName": result[i], "fieldTitle": fieldTitle });
            if (gridData.length == 0) {
                var findField = true, tableField = {
                    "field": result[i],
                    "title": resultitem.json.fieldNameTitle[result[i]],
                    "isMainPageShow": "S",
                    "isAllowShow": "S",
                    "isAllowAdd": "E",
                    "isAllowEdit": "E"
                };
                if (result[i] == "t_id") {
                    tableField.isMainPageShow = "H";
                    tableField.isAllowShow = "H";
                    tableField.isAllowAdd = "H";
                    tableField.isAllowEdit = "H";
                    tableField.editControl = "text";
                }
                else if (result[i] == "client_id" || result[i] == "org_id" || result[i] == "create_date" || result[i] == "created_by" || result[i] == "modify_ip" || result[i] == "modify_date" || result[i] == "last_modified_by" || result[i] == "act_status") {
                    tableField.isMainPageShow = "N";
                    tableField.isAllowShow = "N";
                    tableField.isAllowAdd = "N";
                    tableField.isAllowEdit = "N";
                }
                else {
                    tableField.editControl = "text";
                    tableField.isAdvanceQuery = 1;
                }

                for (var j = 0; j < $("#tableData" + id).datagrid("getData").rows.length; j++) {
                    if (result[i] == $("#tableData" + id).datagrid("getData").rows[j].field) {
                        findField = false;
                        tableData.push($("#tableData" + id).datagrid("getData").rows[j]);
                        break;
                    }
                }
                if (findField) {
                    tableData.push(tableField);
                }
            }
        }
        if (id == 0) {
            dataMainField = fieldData;
        }
        else {
            dataSubField = fieldData;
        }
        if (tableData.length == 0) tableData = gridData;
        $("#tableData" + id).datagrid("loadData", tableData);

        var tableIdField = $("#tableIdField" + id).combobox("getValue");
        var tableSortName = $("#tableSortName" + id).combobox("getValue");
        if (!tableIdField && fieldData.length > 0) {
            tableIdField = fieldData[0].fieldName;
        }
        if (!tableSortName && fieldData.length > 0) {
            tableSortName = fieldData[0].fieldName;
        }

        $("#tableIdField" + id + ",#tableSortName" + id).combobox({
            data: fieldData,
            editable: false,
            panelHeight: 'auto',
            panelMaxHeight: 250,
            valueField: 'fieldName',
            textField: 'fieldTitle'
        });

        $("#tableIdField" + id).combobox("setValue", tableIdField);
        $("#tableSortName" + id).combobox("setValue", tableSortName);

        if (id == 0 && $("#treeSetPanelA").is(':visible') && $("#treeSetPanelB").is(':visible')) {
            var treeBindData = $("#treeBindData").combobox("getValue");
            $("#treeBindData").combobox({
                data: fieldData,
                editable: false,
                panelHeight: 'auto',
                panelMaxHeight: 250,
                valueField: 'fieldName',
                textField: 'fieldTitle'
            });
            $("#treeBindData").combobox("setValue", treeBindData);
        }

        if (id > 0) { mainAndSub(id, dataSubField, mainSubGrid); }
    });
}

function mainAndSub(id, dataSubField, mainSubData) {
    if (id != 0) {
        $("#mainSubLine" + id).datagrid({
            nowrap: true,
            striped: true,
            title: "关联字段" + JQueryEasyUI.gridToolsMenu("#mainSubLine" + id, ''),
            //fitColumns: true,
            remoteSort: false,
            animate: true,
            singleSelect: true,
            rownumbers: true,//行号
            columns: [[
                {
                    field: 'mainGridOutput', title: '主表字段', width: 200, align: 'center', editor: {
                        type: 'combobox', options: {
                            editable: false, required: true,
                            data: dataMainField, valueField: 'fieldName', textField: 'fieldName',
                            panelHeight: 'auto', missingMessage: '必选属性!'
                        }
                    }
                },
                {
                    field: 'subGridInput', title: '从表字段', width: 200, align: 'center', editor: {
                        type: 'combobox', options: {
                            editable: false, required: true,
                            data: dataSubField, valueField: 'fieldName', textField: 'fieldName',
                            panelHeight: 'auto', missingMessage: '必选属性!'
                        }
                    }
                }
            ]],
            onClickRow: function (rowIndex, rowData) {
                //单击开启编辑行
                for (var n = $("#mainSubLine" + id).datagrid("getData").rows.length - 1; n >= 0; n--) {
                    $("#mainSubLine" + id).datagrid("endEdit", n);
                }
                var rows = $("#mainSubLine" + id).datagrid('getEditingRowIndexs');
                if (rows.length == 0) {//单击开启编辑行
                    $("#mainSubLine" + id).datagrid("beginEdit", rowIndex);
                }
            }
        });

        if (mainSubData.length == 0) {
            for (var i = 0; i < dataSubField.length; i++) {
                if (dataSubField[i].fieldName == "create_date" || dataSubField[i].fieldName == "created_by" || dataSubField[i].fieldName == "modify_ip" || dataSubField[i].fieldName == "modify_date" || dataSubField[i].fieldName == "last_modified_by" || dataSubField[i].fieldName == "act_status") {
                    continue;
                }
                for (var j = 0; j < dataMainField.length; j++) {
                    if (dataMainField[j].fieldName == "create_date" || dataMainField[j].fieldName == "created_by" || dataMainField[j].fieldName == "modify_ip" || dataMainField[j].fieldName == "modify_date" || dataMainField[j].fieldName == "last_modified_by" || dataMainField[j].fieldName == "act_status") {
                        continue;
                    }
                    else if (dataSubField[i].fieldName == dataMainField[j].fieldName) {
                        mainSubData.push({ mainGridOutput: dataMainField[j].fieldName, subGridInput: dataSubField[i].fieldName });
                        break;
                    }
                }
            }
        }
        $("#mainSubLine" + id).datagrid("loadData", mainSubData);
    }
}

function removeID(tableId) {
    if (tableId > 1) {
        $("#gridTable" + tableId).panel('destroy');
    }
}

//按钮点击编辑js脚本
function SelDetails() {
    var fieldData = $(gridId).datagrid('getRows');
    var stringFieldData = JSON.stringify(fieldData);
    var arrFieldData = JSON.parse(stringFieldData);
    arrFieldData.unshift({ "field": "AllFieldName", "title": "自动联动字段" });
    arrFieldData.unshift({ "field": "", "title": "无" });
    $("#bindField").combobox({
        data: arrFieldData,
        editable: false,
        panelHeight: 'auto',
        panelMaxHeight: 250,
        valueField: 'field',
        textField: 'title'
    });
    var rows = $(gridId).datagrid('getEditingRowIndexs');
    if (rows.length != 0) {//单击开启编辑行
        $(gridId).datagrid("endEdit", rows[0]);
        $(gridId).datagrid('selectRow', rows[0]);
        $(gridId).datagrid("beginEdit", rows[0]);
    }
    var row = $(gridId).datagrid('getSelected');
    if (row.editControl == "logic") {
        $("#logicHtml").show();
        $("#sequencenoHtml").hide();
        $("#dictionaryHtml").hide();
        $("#userEditable").hide();
        $("#openSelect").hide();
        $("#bindFieldPar").hide();
        $("#logicId").combobox("setValue", row.dataSource);
        $("#SqlNote").html('请选择布尔默认值[1默认是、0默认否]');
    }
    else if (row.editControl == "text" && row.isAllowAdd == "R") {
        $("#logicHtml").hide();
        $("#sequencenoHtml").show();
        $("#dictionaryHtml").hide();
        $("#userEditable").hide();
        $("#openSelect").hide();
        $("#bindFieldPar").hide();
        if (row.dataSource.indexOf('GetSequenceNo') == 0) {
            $("#sequencenoId").combobox('setValue', row.dataSource.replace('GetSequenceNo(\'', '').replace('\')', ''));
        }
        $("#SqlNote").html('请选择生成序列号规则代码');
    }
    else if (row.editControl == "radio" || row.editControl == "checkbox" || row.editControl == "selected" || row.editControl == "multipleselect" || row.editControl == "grid") {
        $("#logicHtml").hide();
        $("#sequencenoHtml").hide();
        $("#openSelect").hide();
        $("#dictionaryHtml").show();
        $("#bindFieldPar").hide();
        if (row.editControl == "selected" || row.editControl == "multipleselect") {
            $("#userEditable").show();
            if (row.dataSource.split(";")[1] && row.dataSource.split(";")[1]=="true") {
                $("#isEditable").prop("checked", true);//选中
            }
            else {
                $("#isEditable").removeAttr("checked");//取消选中 
            }
            if (row.editControl == "selected") {
                $("#bindFieldPar").show();
                if (row.dataSource.split(";")[2]) {
                    $("#bindField").combobox('setValue', row.dataSource.split(";")[2]);
                } else {
                    $("#bindField").combobox('setValue', '');
                }
                $("#sqlWhere").textbox('setValue', row.dataSource.split(";")[3]);                               
            } else {
                $("#bindFieldPar").hide();
            }
        }
        else {
            $("#userEditable").hide();
        }
        if (row.dataSource.indexOf('GetDictionary') == 0) {
            $("#dictionaryId").combotreegrid('setValue', row.dataSource.replace('GetDictionary(\'', '').replace('\')', '').split(";")[0]);
        }
        $("#SqlNote").html('请选择数据字典或输入id,text,selected三个字段的SQL');
    }
    else if (row.editControl == "tree") {
        $("#logicHtml").hide();
        $("#sequencenoHtml").hide();
        $("#dictionaryHtml").show();
        $("#openSelect").hide();

        $("#userEditable").show();
        if (row.dataSource.split(";")[1] && row.dataSource.split(";")[1] == "true") {
            $("#isEditable").prop("checked", true);//选中
        }
        else {
            $("#isEditable").removeAttr("checked");//取消选中 
        }
        $("#bindFieldPar").show();
        if (row.dataSource.split(";")[2]) {
            $("#bindField").combobox('setValue', row.dataSource.split(";")[2]);
        } else {
            $("#bindField").combobox('setValue', '');
        }
        $("#sqlWhere").textbox('setValue', row.dataSource.split(";")[3]);

        if (row.dataSource.indexOf('GetDictionary') == 0) {
            $("#dictionaryId").combotreegrid('setValue', row.dataSource.replace('GetDictionary(\'', '').replace('\')', ''));
        }
        $("#SqlNote").html('请选择数据字典或输入id,pid,text,selected四个字段的SQL');
    }
    else if (row.editControl == "gridTree") {
        $("#logicHtml").hide();
        $("#sequencenoHtml").hide();
        $("#dictionaryHtml").show();
        $("#openSelect").hide();
        $("#bindFieldPar").hide();
        if (row.dataSource.indexOf('GetDictionary') == 0) {
            $("#dictionaryId").combotreegrid('setValue', row.dataSource.replace('GetDictionary(\'', '').replace('\')', ''));
        }
        $("#SqlNote").html('请选择数据字典或输入id,pid,text,selected四个字段的SQL');
    }
    else if (row.editControl == "openselect" || row.editControl == "leftlist") {
        $("#openSelect").show();
        $("#logicHtml").hide();
        $("#sequencenoHtml").hide();
        $("#dictionaryHtml").hide();
        $("#userEditable").hide();
        $("#bindFieldPar").hide();
        if (row.dataSource.split(";")[1]) {
            $("#openSelectPage").combobox('setValue', row.dataSource.split(";")[1]);
        } else {
            $("#openSelectPage").combobox('setValue', '');
        }
    }
    else {
        $.messageBoxBottom(0, "该控件不需要设置数据源！");
        return;
    }
    $("#SQLList").window('open');
    $.setValue(editorSql["sqlContent"], row.dataSource.split(";")[0]);
}

function fileUpload() {
    $('#dialogFileUpload').dialog('close');
    var serviceName = "platform/file/uploadfile";
    var serviceJson = { "uploadFormId": "uploadForm", fileDir: "FastReport" };
    var serviceTitle = { title: "上传", msg: "正在上传打印模版文件，请稍后……", "text": "文件上传中" };
    common.postFileGateway(serviceName, serviceJson, serviceTitle, common.getToken(), function (data) {
        $("#filedir").val('');
        var printFileName = $("#printFileName").combobox("getData");
        printFileName.push({ id: data.json });
        $("#printFileName").combobox("loadData", printFileName);
        $("#printFileName").combobox("setValue", data.json);
    });
}

function deleteFile() {
    if (!$("#printFileName").combobox("getValue")) {
        $.messageBoxBottom(0, "对不起，需要删除的打印模版文件不存在！");
    }
    else {
        common.postGateway("platform/file/deletefile", { fileName: $("#printFileName").combobox("getValue") }, common.getToken(), function (data) {
            $.messageBoxBottom(1, data.json);
            var printFileName = $("#printFileName").combobox("getData");
            for (var i = printFileName.length - 1; i >= 0; i--) {
                if (printFileName[i].id == $("#printFileName").combobox("getValue")) {
                    printFileName.splice(i, 1);
                    break;
                }
            }
            $("#printFileName").combobox("loadData", printFileName);
            $("#printFileName").combobox("setValue", "");
        });
    }
}