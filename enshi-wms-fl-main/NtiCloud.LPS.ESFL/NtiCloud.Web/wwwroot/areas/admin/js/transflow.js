var property = {
    width: $(window).width() - 10,
    height: $(window).height() - 15,
    toolBtns: ["start round mix", "task", "state round mix", "end round mix"],//, "node", "chat", "plug", "join", "fork", "complex"
    haveHead: true,
    headLabel: true,
    headBtns: ["new", "open", "reload", "save", "run", "zoom", "shrink", "undo", "redo", "print"], //如果haveHead=true，则定义HEAD区的按钮
    haveTool: true,
    haveDashed: false,
    haveGroup: false,
    useOperStack: true
};
/*
* 全局的控件颜色自定义设置
*/
GooFlow.prototype.color = {
    main: "#20A0FF",
    font: "#15428B",
    node: "#C0CCDA",
    line: "#1D8CE0",
    lineFont: "#ff6600",
    mark: "#ff8800",
    mix: "#B6F700",
    mixFont: "#777"
};
//取代setNodeRemarks方法，采用更灵活的注释配置
GooFlow.prototype.remarks.toolBtns = {
    cursor: "选择指针",
    direct: "结点连线",
    dashed: "关联虚线",
    start: "开始结点",
    "end": "结束结点",
    "task": "任务结点",
    node: "自动结点",
    chat: "决策结点",
    state: "回滚结点",
    plug: "附加插件",
    fork: "分支结点",
    "join": "联合结点",
    "complex": "复合结点",
    group: "组织划分框编辑开关"
};
GooFlow.prototype.remarks.headBtns = {
    "new": "新建流程",
    open: "打开流程",
    save: "保存流程",
    run: "运行调试",
    zoom: "放大",
    shrink: "缩小",
    undo: "撤销",
    redo: "恢复",
    reload: "流程属性",
    print: "打印流程图"
};
GooFlow.prototype.remarks.extendRight = "工作区向右扩展";
GooFlow.prototype.remarks.extendBottom = "工作区向下扩展";

var jsondata, Tflow, LoadFlow = true, TransFlowDictionary = [], editorSqlContent, isDataSource='0';
$(document).ready(function () {
    $.ajaxSettings.async = false;
    apiRoute.dictionaryConfig('1045235280879161344', true, function (result) {
        for (var i = 0; i < result.length; i++) {
            TransFlowDictionary.push({ id: result[i].id, text: result[i].text });
        }
    });
    var editorSqltables = {};
    common.postGateway('platform/table/DbTable', {
        "tableName": ""
    }, common.getToken(), function (data) {
        data = JSON.parse(data.json);
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
            if (data.rows[i].column_name) {
                editorSqltables[data.rows[i].table_name] = data.rows[i].column_name.split(",");
                editorSqltables["&DataSource&" + data.rows[i].table_name] = data.rows[i].column_name.split(",");
            }
        }
    });

    editorSqlContent = CodeMirror.fromTextArea(document.getElementById("Taction"), {
        lineNumbers: true,  //显示行数
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
    editorSqlContent.setSize(330, 40);
    // 将自动提示绑定到change事件上，这样输入的时候就可以看到联想的关键词
    editorSqlContent.on('change', function (instance, change) {
        // 自动补全的时候，也会触发change事件，所有坐下判断，以免死循环，正则是为了不让空格，换行之类的也提示
        // 通过change对象你可以自定义一些规则去判断是否提示
        if (change.origin !== 'complete' && /\w|\./g.test(change.text[0])) {
            instance.showHint();
        }
    })
    $.ajaxSettings.async = true;

    layui.use('form', function () {
        apiRoute.selectDataEvent("select DBCODE id,DBNOTE text from prod_system_database", [], "1=1", function (tableData) {
            $.each(tableData, function (index, item) {
                $('#dataSource').append(new Option(item.text, item.id));// 下拉菜单里添加元素
            });
            layui.form.render();//下拉菜单渲染 把内容加载进去
        });

        layui.form.on('switch(switchData)', function (data) {
            if (this.checked) {
                isDataSource = '1';
                layer.tips('开启从数据源获取事务流列表数据', data.othis);
            }
            else {
                isDataSource = '0';
                layer.tips('以下事务流列表来源当前数据库', data.othis);
            }
            layui.table.reload('activityList', {
                where: {
                    "dataSource": (isDataSource == "0" ? "" : $("#dataSource").val()),
                }
                , page: {
                    curr: 1 //重新从第 1 页开始
                }
            });
        });
        layui.form.on('select(dataSource)', function (data) {
            if (isDataSource == "1") {
                layui.table.reload('activityList', {
                    where: {
                        "dataSource": $("#dataSource").val(),
                    }
                    , page: {
                        curr: 1 //重新从第 1 页开始
                    }
                });
            }
            layui.form.render('select');//select是固定写法 不是选择器
        });
    });


    Tflow = $.createGooFlow($("#Tflow"), property);

    Tflow.onBtnNewClick = function () {
        if (Tflow.exportData().initNum <= 1) {
            newFlow();
        }
        else {
            $.messageBoxOpenBtn("提示", ['350px', '160px'],
                "<div style='padding:20px;'>请确认是否保存当前事务流？</div>", [
                    {
                        text: '保存',
                        handler: function (index) {
                            SaveTransFlow();
                            $.messageBoxClose(index);
                        }
                    }, {
                        text: '不保存',
                        handler: function () {
                            newFlow();
                        }
                    }, {
                        text: '复制',
                        handler: function () {
                            $("#activityid").val("");
                            $.messageBoxBottom(1, "事流流复制成功，请修改属性后保存！");
                        }
                    }, {
                        text: '取消'
                    }
                ]);
        }
        function newFlow() {
            LoadFlow = false;
            Tflow.$max = 1;
            Tflow.clearData();
            LoadFlow = true;
            $("#activityid").val("");
            $("#start_input").val("[]");
            $("#end_output").val("");
            $("#activitycode").val("");
            $("#activityname").val("");
            $("#description").val("");
            $("#isModule").switchbutton({ checked: false });
            add_edit_json("new", 0, "new", null);

            $("#Tactivity option").each(function () {
                $(this).removeAttr("disabled");
            });
        }
    }
    Tflow.onBtnOpenClick = function () {
        $.messageBoxOpenID('<span class="ico_flowopen"></span>选择事务流程图', ['650px', '380px'], "#list", openGrid);
    }
    Tflow.onBtnSaveClick = function () {
        SaveTransFlow();
    }
    Tflow.onFreshClick = function () {
        add_edit_json("new", 0, "open", null);
    }
    Tflow.onBtnRunClick = function () {
        TflowRun();
    }
    Tflow.onPrintClick = function () {
        Tflow.print(1);
    }
    Tflow.onItemDel = function (id, type) {
        if (LoadFlow && id == "start") {
            $.messageBoxBottom(0, "对不起，开始结点禁止删除！");
            return false;
        }
        else if (LoadFlow) {
            return confirm("确定要删除该单元吗?");
        }
        return true;
    }
    Tflow.onItemAdd = function (id, type, json) {
        if (LoadFlow) {
            if (JSON.stringify(Tflow.exportData().nodes).indexOf(",\"type\":\"start round mix\"") == -1 && json.type != "start round mix") {
                $.messageBoxAlert(0, "请先在事务流编辑器中拖放开始结点！");
                Tflow.$max -= 1;
                return false;
            }
            else if (JSON.stringify(Tflow.exportData().nodes).indexOf(",\"type\":\"start round mix\"") != -1 && json.type == "start round mix") {
                $.messageBoxAlert(0, "事务流编辑器中开始结点已存在！");
                Tflow.$max -= 1;
                return false;
            }
            else if (type == "line" && json.from == json.to) { //线画好后，该事件会重复执行,点击node会产生bug，影响双击事件
                Tflow.$max -= 1;
                return false;
            }
            else {
                add_edit_json("add", id, type, json);
            }
        }
        return true;
    }
    Tflow.onItemDbClick = function (id, type) {
        LoadFlow = false; //不是新增，通过双击加载
        add_edit_json("edit", id, type, (type == "node" ? this.$nodeData[id] : this.$lineData[id])); //Tflow.exportData()[type + "s"][id]
    }

    layui.use(['form', 'element'], function () {
        var form = layui.form;
        var element = layui.element; //Tab的切换功能，切换事件监听等，需要依赖element模块
        element.on('tab(mainTab)', function (elem) {
            var index = $(this).attr('lay-id');
            if (index == "tab1") {
                for (var n = 0; $("#output_arg").length > 0 && n < $("#output_arg").datagrid("getData").rows.length; n++) {
                    $("#output_arg").datagrid("endEdit", n);
                }
                $("#input_arg").datagrid();
            }
            else if (index == "tab2") {
                for (var n = 0; $("#input_arg").length > 0 && n < $("#input_arg").datagrid("getData").rows.length; n++) {
                    $("#input_arg").datagrid("endEdit", n);
                }
                $("#output_arg").datagrid();
            }
            else if (index == "tab4") {
                for (var n = 0; $("#input_arg").length > 0 && n < $("#input_arg").datagrid("getData").rows.length; n++) {
                    $("#input_arg").datagrid("endEdit", n);
                }
                dg_in_output_arg("2", $("#input_arg").datagrid("getData").rows);
            }
        });

        form.on('select(sqltype)', function (data) {
            var newValue = data.value; //得到被选中的值
            if (newValue == "sqlTransflow") {
                $("#sqlShow").hide();
                $("#procedureShow").hide();
                $("#transflowShow").show();
                $("#MicroServiceShow").hide();
            }
            else if (newValue == "sqlProcedure") {
                $("#sqlShow").hide();
                $("#procedureShow").show();
                $("#transflowShow").hide();
                $("#MicroServiceShow").hide();
            } else if (newValue == "MicroServiceGet" || newValue == "MicroServicePost") {
                $("#sqlShow").hide();
                $("#procedureShow").hide();
                $("#transflowShow").hide();
                $("#MicroServiceShow").show();
            }
            else {
                $("#sqlShow").show();
                $("#procedureShow").hide();
                $("#transflowShow").hide();
                $("#MicroServiceShow").hide();
                setTimeout(function () {
                    editorSqlContent.refresh();
                }, 100); 
            }
        });
        var Tactivity = [];
        apiRoute.selectDataEvent("select transflow_code id,transflow_name text,description, input,output from prod_config_transflow",
            [], "1=1", function (tableData) {
            Tactivity = tableData;
            $.each(Tactivity, function (index, item) {
                /*var option = document.createElement("option"); // 创建添加option属性 
                option.setAttribute("value", item.id); // 给option的value添加值
                option.innerText = item.text; // 打印option对应的纯文本
                document.getElementById("Tactivity").appendChild(option); //给select添加option子标签 
                */
                $('#Tactivity').append(new Option(item.text, item.id));//往下拉菜单里添加元素
            });
            form.render(null, 'nodeflow');//菜单渲染 把内容加载进去
        });

        form.on('select(Tactivity)', function (data) {
            for (var i = 0; i < Tactivity.length; i++) {
                if (data.value == Tactivity[i].id) {
                    $("#input_arg").datagrid("loadData", JSON.parse(Tactivity[i].input));
                    $("#output_arg").datagrid("loadData", JSON.parse(Tactivity[i].output));
                    break;
                }
            }
        });
    });
    /*
    $("#Open_Sql1").click(function () {
        $.messageBoxOpenBtn("<span class='mdi mdi-playlist-plus'></span>SQL语句", ['500px', '350px'], "#SQLList1",
            [{
                text: '确 定',
                handler: function (index) {
                    $("#Taction").val('editorSqlContent.getValue()')
                    $.messageBoxClose(index);
                }
            }, {
                text: '取 消',
                handler: function () {

                }
            }]
        );
        editorSqlContent.setValue($("#Taction").val());
    });
    $("#Open_Sql2").click(function () {
        $.messageBoxOpenBtn("<span class='mdi mdi-playlist-plus'></span>微服务地址", ['500px', '350px'], "#SQLList2",
            [{
                text: '确 定',
                handler: function (index) {
                    $("#MicroServiceUrl").val($("#sqlContent2").val())
                    $.messageBoxClose(index);
                }
            }, {
                text: '取 消',
                handler: function () {

                }
            }]
        );
        $("#sqlContent2").val($("#MicroServiceUrl").val());
    });
    */
    $('#txtQuery').on('keydown', function (event) { if (event.keyCode == 13) { search(); } });
});
function SaveTransFlow() {
    if ($("#activitycode").val() == "") {
        $.messageBoxAlert(0, "请输入流程代码！", function () { $("#activitycode").focus() });
    }
    else if (/^[a-zA-Z\$_][a-zA-Z\d\.\-_]*$/.test($("#activitycode").val()) == false) {
        $.messageBoxAlert(0, "流程代码验证不通过！", function () { $("#activitycode").focus() });
    }
    else if ($("#activityname").val() == "") {
        $.messageBoxAlert(0, "请输入流程名称！", function () { $("#activityname").focus() });
    }
    else if ($("#description").val() == "") {
        $.messageBoxAlert(0, "请输入流程描述！", function () { $("#description").focus() });
    }
    else if (JSON.stringify(Tflow.exportData().nodes) == "{}" || JSON.stringify(Tflow.exportData().lines) == "{}") {
        $.messageBoxAlert(0, "流程图不完整，不能保存！");
    }
    else {
        $("#TransFlowSet").hide();
        apiRoute.selectDataEvent("select * from prod_config_transflow where t_id<>'" + $("#activityid").val() + "' and transflow_code='" + $("#activitycode").val() + "'",
            [], "1=1", function (result) {
            if (result.length != 0) {
                $.messageBoxBottom(0, "检测到事务流代码[" + $("#activitycode").val() + "]在数据库中重复！");
                $("#activitycode").focus();
            }
            else {
                var saveJson = {}, sql = "";
                saveJson.t_id = $("#activityid").val();
                saveJson.transflow_code = $("#activitycode").val();
                saveJson.transflow_name = $("#activityname").val();
                saveJson.input = $("#start_input").val();
                saveJson.output = $("#end_output").val();
                saveJson.json = JSON.stringify(Tflow.exportData());
                saveJson.description = $("#description").val();
                saveJson.appcode = common.getUrlParam("AppCode");
                saveJson.ismodule = ($("#isModule").switchbutton("options").checked ? "1" : "0");

                if ($("#activityid").val() == "" || $("#activityid").val() == "0") {
                    sql = "insert into prod_config_transflow(t_id,appcode,ismodule,transflow_code,transflow_name,input,output,json,description,create_date,created_by,modify_ip,act_status) values "
                        + "(@t_id,@appcode,@ismodule,@transflow_code,@transflow_name,@input,@output,@json,@description,now(),@created_by,@modify_ip,'1')"
                }
                else {
                    sql = "update prod_config_transflow set "
                        + "appcode=@appcode,"
                        + "ismodule=@ismodule,"
                        + "transflow_code=@transflow_code,"
                        + "transflow_name=@transflow_name,"
                        + "input=@input,"
                        + "output=@output,"
                        + "json=@json,"
                        + "description=@description,"
                        + "modify_date=now(),"
                        + "last_modified_by=@last_modified_by,"
                        + "modify_ip=@modify_ip,"
                        + "act_status='1' where t_id=@t_id";
                }
                apiRoute.runTaskEvent("",sql, [saveJson], function (result) {
                    if (result.resMsg == "") {
                        $("#activityid").val(result.resId);
                        $.messageBoxBottom(1, "事流流保存成功！");
                    }
                    else {
                        $.messageBoxBottom(0, result.resMsg);
                    }
                });
            }
        });
    }
}
function add_edit_json(opt, id, type, json) {
    $("#sid").val(id);
    if (opt == "new") {
        $.messageBoxOpenBtn("<span class='ico_attribe'></span>事务流程属性设置", ['900px', '520px'], "#TransFlowSet", [
            {
                text: '确　定',
                handler: function (index) { save_json('save', index); }
            }, {
                text: '取　消',
                handler: function (index) { save_json('close', index); }
            }]);
    
        $('#mainTab .layui-tab-title li').eq(0).attr("class", "layui-this").click();
        $('#mainTab .layui-tab-title li').eq(1).attr("class", "layui-hide");
        $('#mainTab .layui-tab-title li').eq(2).attr("class", "layui-hide");
        $('#mainTab .layui-tab-title li').eq(3).attr("class", "layui-hide");

        $("#startflow").show();
        $("#lineflow").hide();
        $("#nodeflow").hide();
        startGrid(type);
        return;
    }
    else if (opt == "add") {
        $("#ele_id").val(type + Tflow.$max);
    }
    else {
        $("#ele_id").val(id);
    }
    $("#ele_model").val(type);
    $("#ele_name").val(json.name);
    $("#lineflow").show();
    if (type == "node") {
        $.messageBoxOpenBtn("<span class='ico_attribe'></span>节点属性设置", ['920px', '520px'], "#TransFlowSet", [
            {
                text: '确　定',
                handler: function (index) { save_json('save', index); }
            }, {
                text: '取　消',
                handler: function (index) { save_json('close', index); }
            }]);
        $('#mainTab .layui-tab-title li').eq(0).attr("class", "layui-this").click();
        $('#mainTab .layui-tab-title li').eq(1).removeAttr("class");
        $('#mainTab .layui-tab-title li').eq(2).attr("class", "layui-hide");
        $("#startflow").hide();
        if (json.type == "start round mix") {
            $("#ele_id").val("start");
            $("#nodeflow").hide();
            $("#sqltype").val('selectFirst');
            $.setValue(editorSqlContent, "");
            $('#mainTab .layui-tab-title li').eq(3).attr("class", "layui-hide");
        }
        else if (json.type == "end round mix" || json.type == "state round mix") {
            if (opt == "add") $("#ele_id").val("end" + Tflow.$max);
            $("#nodeflow").hide();
            $("#sqltype").val('selectFirst');
            $.setValue(editorSqlContent, "");
            $('#mainTab .layui-tab-title li').eq(3).attr("class", "layui-hide");
        }
        else {
            $('#mainTab .layui-tab-title li').eq(3).removeAttr("class");
            $("#nodeflow").show();
            if (json.sqltype) $("#sqltype").val(json.sqltype);
            if (json.sqltype == "sqlProcedure") {
                $("#sqlShow").hide();
                $("#procedureShow").show();
                $("#transflowShow").hide();
                $("#MicroServiceShow").hide();
                $("#MicroServiceUrl").val("");
                $.setValue(editorSqlContent, "");
                $("#Tprocedure").val(json.action);
                layui.form.val("nodeflow", { "Tactivity": "" });
            }
            else if (json.sqltype != "sqlTransflow") {
                $("#sqlShow").show();
                $("#procedureShow").hide();
                $("#transflowShow").hide();
                $.setValue(editorSqlContent, json.action);
                $("#Tprocedure").val("");
                if (json.sqltype == "MicroServiceGet" || json.sqltype == "MicroServicePost") {
                    $("#sqlShow").hide();
                    $("#procedureShow").hide();
                    $("#transflowShow").hide();
                    $("#MicroServiceShow").show();
                    $("#MicroServiceUrl").val(json.action);
                    $.setValue(editorSqlContent, '');
                }
                layui.form.val("nodeflow", { "Tactivity": "" });
            }
            else {
                $("#sqlShow").hide();
                $("#procedureShow").hide();
                $("#transflowShow").show();
                $("#MicroServiceShow").hide();
                $("#MicroServiceUrl").val("");
                $.setValue(editorSqlContent, '');
                $("#Tprocedure").val("");
                layui.form.val("nodeflow", { "Tactivity": json.action });
            }
            dg_in_output_arg(2, json.input == null ? [] : json.input);
        }
        layui.use('form', function () {
            var form = layui.form;
            //form.render(null, 'nodeflow');
            form.val("nodeflow", { "sqltype": $("#sqltype").val() });
        });
        inAndOut_load(json);
    }
    else if (type == "line") {
        $.messageBoxOpenBtn("<span class='ico_attribe'></span>条件属性设置", ['900px', '520px'], "#TransFlowSet", [
            {
                text: '确　定',
                handler: function (index) { save_json('save', index); }
            }, {
                text: '取　消',
                handler: function (index) { save_json('close', index); }
            }]);
        $('#datagrid').datagrid('reload');
        $('#mainTab .layui-tab-title li').eq(0).attr("class", "layui-hide");
        $('#mainTab .layui-tab-title li').eq(1).attr("class", "layui-hide");
        $('#mainTab .layui-tab-title li').eq(2).attr("class", "layui-this").click();
        $('#mainTab .layui-tab-title li').eq(3).attr("class", "layui-hide");
        $("#startflow").hide();
        $("#nodeflow").hide();
        datagrid_load(id, json);
    }
}
function save_json(opt,index) {
    if (LoadFlow && opt == "close") {
        LoadFlow = false;
        Tflow.$max -= 1;
        Tflow.delNode($("#sid").val(), true);
        Tflow.delLine($("#sid").val());
    }
    else if (opt == "close") {

    }
    else if ($("#sid").val() == 0) {//属性设置
        for (var n = 0; n < $("#input_start").datagrid("getData").rows.length; n++) {
            $("#input_start").datagrid("endEdit", n);
        }
        if ($("#activitycode").val() == "") {
            $.messageBoxAlert(0, "请输入流程代码！", function () { $("#activitycode").focus() });
            return;
        }
        else if (/^[a-zA-Z\$_][a-zA-Z\d\.\-_]*$/.test($("#activitycode").val()) == false) {
            $.messageBoxAlert(0, "流程代码验证不通过！", function () { $("#activitycode").focus() });
            return;
        }
        else if ($("#activityname").val() == "") {
            $.messageBoxAlert(0, "请输入流程名称！", function () { $("#activityname").focus() });
            return;
        }
        else if ($("#description").val() == "") {
            $.messageBoxAlert(0, "请输入流程描述！", function () { $("#description").focus() });
            return;
        }
        else {
            Tflow.$title = $("#activityname").val();
            $("#start_input").val(JSON.stringify($('#input_start').datagrid("getData").rows));
        }
    }
    else {
        jsondata = Tflow.exportData();
        if ($("#ele_model").val() == "node") {
            for (var n = 0; n < $("#input_arg").datagrid("getData").rows.length; n++) {
                $("#input_arg").datagrid("endEdit", n);
                if ($("#sqlShow").is(":visible") && common.patch($("#input_arg").datagrid("getData").rows[n].fldkey, editorSqlContent.getValue()) == 0) {
                    $.messageBoxAlert(0, "输入参数[" + $("#input_arg").datagrid("getData").rows[n].fldkey + "]在SQL中出现次数有误!", function () { editorSqlContent.focus(); });
                    return;
                }
            }
            for (var n = 0; n < $("#output_arg").datagrid("getData").rows.length; n++) {
                $("#output_arg").datagrid("endEdit", n);
            }

            if (jsondata.nodes[$("#sid").val()].type == "end round mix" || jsondata.nodes[$("#sid").val()].type == "state round mix") {
                $("#end_output").val(JSON.stringify($('#output_arg').datagrid("getData").rows));
            }

            jsondata.nodes[$("#sid").val()].name = $("#ele_name").val();
            jsondata.nodes[$("#sid").val()].sqltype = $("#sqltype").val();
            if ($("#sqltype").val() == "sqlProcedure") {
                jsondata.nodes[$("#sid").val()].action = $("#Tprocedure").val();
            }
            else if ($("#sqltype").val() != "sqlTransflow") {
                jsondata.nodes[$("#sid").val()].action = editorSqlContent.getValue();
                if ($("#sqltype").val() == "MicroServiceGet" || $("#sqltype").val() == "MicroServicePost") {
                    jsondata.nodes[$("#sid").val()].action = $("#MicroServiceUrl").val();
                }
            }
            else {
                jsondata.nodes[$("#sid").val()].action = $("#Tactivity").val();
            }
            jsondata.nodes[$("#sid").val()].input = $("#input_arg").datagrid("getData").rows;
            jsondata.nodes[$("#sid").val()].output = $("#output_arg").datagrid("getData").rows;

            if ($("#sid").val() != $("#ele_id").val() && jsondata.nodes[$("#ele_id").val()] == null) {
                jsondata.nodes[$("#ele_id").val()] = jsondata.nodes[$("#sid").val()];
                delete jsondata.nodes[$("#sid").val()];
                for (var key in jsondata.lines) {
                    if (jsondata.lines[key].from == $("#sid").val()) {
                        jsondata.lines[key].from = $("#ele_id").val();
                    }
                    if (jsondata.lines[key].to == $("#sid").val()) {
                        jsondata.lines[key].to = $("#ele_id").val();
                    }
                } //Tflow.transNewId($("#sid").val(), $("#ele_id").val(), "node");
            }
            else if ($("#sid").val() != $("#ele_id").val()) {
                Tflow.$max += 1;
                $.messageBoxAlert(0, "事务流编辑器中节点主键已存在！", function () { $("#ele_id").focus(); });
                return;
            }
        }
        else if ($("#ele_model").val() == "line") {
            for (var i = 0; i < $('#datagrid').datagrid("getData").total; i++) {
                $("#datagrid").datagrid("endEdit", i);
            }
            jsondata.lines[$("#sid").val()].name = $("#ele_name").val();
            jsondata.lines[$("#sid").val()].condition = $('#datagrid').datagrid("getData").rows;
            if ($("#sid").val() != $("#ele_id").val() && jsondata.lines[$("#ele_id").val()] == null) {
                jsondata.lines[$("#ele_id").val()] = jsondata.lines[$("#sid").val()];
                delete jsondata.lines[$("#sid").val()];
            }
            else if ($("#sid").val() != $("#ele_id").val()) {
                Tflow.$max += 1;
                $.messageBoxAlert(0, "事务流编辑器中连线主键已存在！", function () { $("#ele_id").focus(); });
                return;
            }
        }
        LoadFlow = false;
        Tflow.clearData();
        Tflow.loadData(jsondata);
    }
    LoadFlow = true;
    $.messageBoxClose(index);
}

function startGrid(type) {
    $("#input_data").html("<table id=\"input_start\"></table>");
    $("#input_start").datagrid({
        title: "事务开始输入参数" + JQueryEasyUI.gridToolsMenu("#input_start", ''),
        nowrap: true,
        striped: true,
        height: 280,
        fitColumns: true,
        remoteSort: false,
        animate: true,
        singleSelect: true,
        rownumbers: true,//行号
        columns: [[
            {
                field: 'fldkey', title: '参数代码', width: 50, align: 'center', editor: {
                    type: "combobox", options: {
                        data: TransFlowDictionary, valueField: 'id', textField: 'text', panelHeight: 'auto', required: true, onSelect: function (data) {
                            var row = $("#input_start").datagrid('getSelected');
                            var rowIndex, rows = $("#input_start").datagrid('getEditingRowIndexs');
                            if (rows.length == 1) {
                                rowIndex = rows[0];
                            }
                            else {
                                rowIndex = $("#input_start").datagrid('getRowIndex', row); //获取行号
                            }
                            var target = $("#input_start").datagrid('getEditor', { 'index': rowIndex, 'field': 'fldname' }).target;
                            target.textbox('setValue', data.text);
                        }
                    }
                }
            },
            { field: 'fldname', title: '参数名称', width: 50, align: 'center', editor: 'textbox' },
            { field: 'fldvalue', title: '默认值', width: 50, align: 'center', editor: 'textbox' }
        ]],
        onClickRow: function (rowData) {
            for (var n = 0; n < $("#input_start").datagrid("getData").rows.length; n++) {
                $("#input_start").datagrid("endEdit", n);
            }
        },
        onDblClickRow: function (rowIndex, rowData) {
            for (var n = 0; n < $("#input_start").datagrid("getData").rows.length; n++) {
                $("#input_start").datagrid("endEdit", n);
            }
            var rows = $("#input_start").datagrid('getEditingRowIndexs');
            if (rows.length == 0) {//单击开启编辑行
                $("#input_start").datagrid("beginEdit", rowIndex);
            }
        }
    });
    if (type == "new") $("#input_start").datagrid("loadData", []);
    else $("#input_start").datagrid("loadData", JSON.parse($("#start_input").val()));
}
function search() {
    layui.form.on('submit(transFlowHeader)', function (data) {
        layui.table.reload('activityList', {
            where: { //请求参数（注意：这里面的参数可任意定义，并非下面固定的格式）
                "queryStr": "TRANSFLOW_CODE like '%" + $('#txtQuery').val() + "%'  or TRANSFLOW_NAME like '%" + $('#txtQuery').val() + "%' or DESCRIPTION like '%" + $('#txtQuery').val() + "%'"
            }
            , page: {
                curr: 1 //重新从第 1 页开始
            }
        });
        return false;
    });
}
function openGrid(index) {
    layui.use('table', function () {
        var table = layui.table;
        var tableIns = table.render({
            elem: '#activityList'
            , url: common.gatewayUrl()+ 'api/platform/database/selectlayuipage'
            , width: 640
            , height: 278
            , method: "post"
            , even: true
            , where: {
                "dataSource": (isDataSource == "0" ? "" : $("#dataSource").val()),
                "sql": common.Encrypt("select * from prod_config_transflow where ISMODULE='1' or AppCode='" + common.getUrlParam("AppCode") + "'")
            }
            , headers: { Authorization: sessionStorage.getItem("Authorization") }
            , autoSort: false
            , contentType: 'application/json'
            , parseData: function (response) { //res 即为原始返回的数据
                return {
                    "code": response.errcode, //解析接口状态
                    "msg": "", //解析提示文本
                    "count": JSON.parse(response.json).total, //解析数据长度
                    "data": JSON.parse(response.json).rows //解析数据列表
                };
            }
            , request: {
                pageName: 'page', //页码的参数名称，默认：page
                limitName: 'limit' //每页数据量的参数名，默认：limit
            }
            , response: {
                statusName: 'code' //规定数据状态的字段名称，默认：code
                , statusCode: 0 //规定成功的状态码，默认：0
                , msgName: 'msg' //规定状态信息的字段名称，默认：msg
                , countName: 'count' //规定数据总数的字段名称，默认：count
                , dataName: 'data' //规定数据列表的字段名称，默认：data
            }
            , cols: [[
                { type: 'numbers', title: '', width: '5%' },
                {
                    field: 't_id', align: 'center', title: "操作", width: '10%', sort: false, event: 'delTransFlow',
                    templet: function (res) {
                        return "<a title=\"删除\" class=\"btnline\"><span class=\"mdi mdi-trash-can-outline\"></span></a>";
                    }
                },
                { field: 'transflow_code', title: '事务流代码', width: '24%', align: 'left', sort: true, event: 'loadTransFlow' },
                { field: 'transflow_name', title: '事务流名称', width: '23%', align: 'left', sort: true, event: 'loadTransFlow' },
                { field: 'description', title: '描述', width: '28%', halign: 'center', sort: true, event: 'loadTransFlow' },
                { field: 'json', title: 'JSON', width: 50, align: 'center', hide: true },
                {
                    field: 't_id', align: 'center', title: "同步", width: '10%', sort: false, event: 'copyTransFlow',
                    templet: function (res) {
                        return "<a title=\"同步\" class=\"btnline\"><span class=\"mdi mdi-access-point-network\"></span></a>";
                    }
                },
            ]]
            , limit: 10
            , limits: [10, 20, 30, 40, 50, 100, 200, 500]
            , page: true
        });
        table.on('sort(list)', function (obj) {
            table.reload('activityList', {
                initSort: obj //记录初始排序，如果不设的话，将无法标记表头的排序状态。
                , where: { //请求参数（注意：这里面的参数可任意定义，并非下面固定的格式）
                    "field": obj.field, //排序字段
                    "order": obj.type == null ? "" : obj.type //排序方式
                }
            });
        });
        table.on('row(list)', function (obj) {

        });
        table.on('tool(list)', function (obj) {
            var data = obj.data;
            if (obj.event === 'delTransFlow') {
                if (isDataSource == "1") {
                    $.messageBoxBottom(0, "对不起，没有权限操作远端事务流！");
                    return;
                }
                $.messageBoxConfirm('你要删除所选择的记录吗？', null, function () {
                    apiRoute.runTaskEvent("","delete from prod_config_transflow where t_id=@tid",[{ tid: data.t_id }], function (result) {
                        if (result.resMsg == "") {
                            $.messageBoxBottom(1, "共删除" + result.resCount + "条记录！");
                            obj.del();
                            tableIns.reload();
                            if ($("#activityid").val() == data.t_id) {
                                Tflow.clearData();
                                Tflow.resetScale(1);
                                $("#activityid").val("");
                            }
                        }
                        else {
                            $.messageBoxBottom(0, result.resMsg);
                        }
                    });
                });
            }
            else if (obj.event === 'copyTransFlow') {
                var titleTip = "即将从本地源上传事务流到选择数据源？", select_dtSource, insert_dtSource;
                if ($("#dataSource").val() == "") {
                    $.messageBoxBottom(0, "当前数据源下事务流无需同步！");
                    return;
                }
                else if (isDataSource == "1") {
                    titleTip = "即将从选择数据源下载事务流到本地源？"
                    select_dtSource = $("#dataSource").val();
                    insert_dtSource = "";
                }
                else {
                    select_dtSource = "";
                    insert_dtSource = $("#dataSource").val();
                }
                $.messageBoxConfirm(titleTip, null, function () {
                    var json = [{ tid: data.t_id }];
                    common.postGateway('platform/database/selectsql', {
                        "dataSource": select_dtSource,
                        "sql": common.Encrypt("select * from prod_config_transflow where t_id=@tid"),
                        "parameters": json,
                        "queryStr": "1=1"
                    }, common.getToken(), function (data) {
                            var selectData = JSON.parse(data.json);
                            if (selectData.length > 0) {
                                json.push(selectData[0]);
                                apiRoute.runTaskEvent(insert_dtSource, "delete from prod_config_transflow where t_id=@tid;"
                                    + "insert into PROD_CONFIG_TRANSFLOW(T_ID,CLIENT_ID,ORG_ID,APPCODE,ISMODULE,TRANSFLOW_CODE,TRANSFLOW_NAME,INPUT,OUTPUT,JSON,DESCRIPTION,CREATE_DATE,CREATED_BY,MODIFY_IP,MODIFY_DATE,LAST_MODIFIED_BY,ACT_STATUS) values "
                                    +"(@t_id,@client_id,@org_id,@appcode,@ismodule,@transflow_code,@transflow_name,@input,@output,@json,@description,@create_date,@created_by,@modify_ip,now(),@last_modified_by,@act_status)", json, function (result) {
                                    $.messageBoxBottom(1, "当前选择的事务流同步成功！");
                                });
                            }
                            else {
                                $.messageBoxBottom(0, "查询数据出来，同步失败！");
                            }
                    })
                });
            }
            else {
                var rowData = obj.data
                LoadFlow = false;
                Tflow.clearData();
                Tflow.resetScale(1);
                $("#activityid").val(rowData.t_id);
                $("#activitycode").val(rowData.transflow_code);
                $("#activityname").val(rowData.transflow_name);
                $("#description").val(rowData.description);
                $("#isModule").switchbutton({ checked: ((rowData.ismodule == "1") ? true : false) });
                Tflow.loadData(rowData.json == "" ? {} : JSON.parse(rowData.json));
                $("#start_input").val(rowData.input);
                $("#end_output").val(rowData.output);

                $("#Tactivity option").each(function () {
                    if (rowData.transflow_code == $(this).attr("value")) {
                        $(this).attr("disabled", "disabled");
                    }
                    else {
                        $(this).removeAttr("disabled");
                    }
                });
                LoadFlow = true;
                $('#list').hide();
                $.messageBoxClose(index);
            }
        });
    });
}

function inAndOut_load(json) {
    $("#input_data").html("<table id=\"input_arg\"></table>");
    $("#output_data").html("<table id=\"output_arg\"></table>");
    if (json.type == "start round mix") {
        $("#input_arg,#output_arg").datagrid({
            data: JSON.parse($("#start_input").val()),
            nowrap: true,
            striped: true,
            height: 280,
            fitColumns: true,
            remoteSort: false,
            animate: true,
            singleSelect: true,
            rownumbers: true,//行号
            columns: [[
                { field: 'fldkey', title: '参数代码', width: 50, align: 'center' },
                { field: 'fldname', title: '参数名称', width: 50, align: 'center' },
                { field: 'fldvalue', title: '默认值', width: 50, align: 'center' }
            ]]
        });
    }
    else {
        var all_node = [];
        for (var key in Tflow.$nodeData) {
            if (Tflow.$nodeData[key].type != "end round mix" && Tflow.$nodeData[key].type != "state round mix" && key != $("#sid").val() && key != $("#ele_id").val()) all_node.push({ "NodeID": key, "NodeText": Tflow.$nodeData[key].name });
        }
        $("#input_arg").datagrid({
            data: (json.input != null ? json.input : []),
            title: "输入参数设置" + JQueryEasyUI.gridToolsMenu("#input_arg", ''),
            nowrap: true,
            striped: true,
            height: 280,
            fitColumns: true,
            remoteSort: false,
            animate: true,
            singleSelect: true,
            rownumbers: true,//行号
            columns: [[
                {
                    field: 'fldkey', title: '输入参数', width: 50, align: 'center', editor: {
                        type: "combobox", options: {
                            data: TransFlowDictionary, valueField: 'id', textField: 'text', panelHeight: 'auto', required: true, onSelect: function (data) {
                                var row = $("#input_arg").datagrid('getSelected');
                                var rowIndex, rows = $("#input_arg").datagrid('getEditingRowIndexs');
                                if (rows.length == 1) {
                                    rowIndex = rows[0];
                                }
                                else {
                                    rowIndex = $("#input_arg").datagrid('getRowIndex', row); //获取行号
                                }
                                var target = $("#input_arg").datagrid('getEditor', { 'index': rowIndex, 'field': 'fldname' }).target;
                                target.textbox('setValue', data.text);
                            }
                        }
                    }
                },
                { field: 'fldname', title: '参数名称', width: 50, align: 'center', editor: 'textbox' },
                {
                    field: 'fldnode', title: '对应结点', width: 50, align: 'center', editor: {
                        type: 'combobox', options: {
                            editable: false,
                            data: all_node, valueField: 'NodeID', textField: 'NodeText', panelHeight: 'auto', panelMaxHeight: 250, missingMessage: '来源节点必选!',
                            onSelect: function (propertydata) {
                                var row = $('#input_arg').datagrid('getSelected');
                                var rowIndex = $('#input_arg').datagrid('getRowIndex', row)
                                var target = $('#input_arg').datagrid('getEditor', { 'index': rowIndex, 'field': 'fldmapping' }).target;
                                var value = target.combobox('getValue');
                                target.combobox('clear'); //清除原来的数据
                                var jsondata = [];
                                if (Tflow.$nodeData[propertydata.NodeID].output != undefined) {
                                    for (var key = 0; key < Tflow.$nodeData[propertydata.NodeID].output.length; key++) {
                                        jsondata.push({ "fldmapping": Tflow.$nodeData[propertydata.NodeID].output[key].fldkey });
                                    }
                                }
                                target.combobox("loadData", jsondata);
                            }
                        }
                    }, formatter: function (value, row, index) { if (row && value != undefined && Tflow.$nodeData[value] != undefined) return Tflow.$nodeData[value].name; else return value; }
                },
                { field: 'fldmapping', title: '对应参数', width: 50, align: 'center', editor: { type: 'combobox', options: { editable: false, valueField: 'fldmapping', textField: 'fldmapping', missingMessage: '来源字段必选!', panelHeight: 'auto', panelMaxHeight: 250 } } }
            ]],
            onClickRow: function (rowData) {
                for (var n = 0; n < $("#input_arg").datagrid("getData").rows.length; n++) {
                    $("#input_arg").datagrid("endEdit", n);
                }
            },
            onDblClickRow: function (rowIndex, rowData) {
                for (var n = 0; n < $("#input_arg").datagrid("getData").rows.length; n++) {
                    $("#input_arg").datagrid("endEdit", n);
                }
                var rows = $("#input_arg").datagrid('getEditingRowIndexs');
                if (rows.length == 0) {//单击开启编辑行
                    $("#input_arg").datagrid("beginEdit", rowIndex);
                }
            }
        });
        var outputArr = [{ "fldkey": "resCount", "fldname": "记录行数" }, { "fldkey": "resMsg", "fldname": "提示消息" }];
        $("#output_arg").datagrid({
            data: (json.output != null ? json.output : outputArr),
            title: "输出参数设置" + JQueryEasyUI.gridToolsMenu("#output_arg", ''),
            nowrap: true,
            striped: true,
            height: 280,
            fitColumns: true,
            remoteSort: false,
            animate: true,
            singleSelect: true,
            rownumbers: true,//行号
            columns: [[
                { field: 'fldkey', title: '输出参数', width: 50, align: 'center', editor: { type: "textbox", options: { required: true } } },
                { field: 'fldname', title: ((json.type == "end round mix" || json.type == "state round mix") ? '输出提示信息' : '参数名称'), width: 50, align: 'center', editor: 'textbox' },
                { field: 'fldvalue', title: '默认值', hidden: ((json.type == "end round mix" || json.type == "state round mix") ? false : true), width: 50, align: 'center', editor: 'textbox' }
            ]],
            onClickRow: function (rowData) {
                for (var n = 0; n < $("#output_arg").datagrid("getData").rows.length; n++) {
                    $("#output_arg").datagrid("endEdit", n);
                }
            },
            onDblClickRow: function (rowIndex, rowData) {
                for (var n = 0; n < $("#output_arg").datagrid("getData").rows.length; n++) {
                    $("#output_arg").datagrid("endEdit", n);
                }
                var rows = $("#output_arg").datagrid('getEditingRowIndexs');
                if (rows.length == 0) {//单击开启编辑行
                    $("#output_arg").datagrid("beginEdit", rowIndex);
                }
            }
        });
    }
}

function datagrid_load(id, json) {
    $("#table_data").html("<table id=\"datagrid\"></table>");
    var Property = [];
    if (Tflow.$nodeData[json.from].output != null) {
        for (var key = 0; key < Tflow.$nodeData[json.from].output.length; key++) {
            Property.push({ "property": Tflow.$nodeData[json.from].output[key].fldkey });
        }
    }
    var datagrid = $('#datagrid').datagrid({
        data: json.condition == null ? [] : Tflow.$lineData[id].condition,
        title: '事务流条件设置' + JQueryEasyUI.gridToolsMenu("#datagrid", ''),
        striped: true,
        height: 280,
        fitColumns: true,
        loadMsg: '正在加载数据...',
        singleSelect: true,
        columns: [[
            {
                field: 'property', title: '属性', width: 120, align: 'center', editor: {
                    type: 'combobox', options: {
                        editable: false, required: true,
                        data: Property, valueField: 'property', textField: 'property', panelHeight: 'auto', missingMessage: '属性必选!',
                        onSelect: function (propertydata) {
                            var rowIndex, rows = datagrid.datagrid('getEditingRowIndexs');
                            if (rows.length == 1) {
                                rowIndex = rows[0];
                            }
                            else {
                                var row = datagrid.datagrid('getSelected');
                                rowIndex = datagrid.datagrid('getRowIndex', row); //获取行号
                            }
                            var target = datagrid.datagrid('getEditor', { 'index': rowIndex, 'field': 'variable' }).target;
                            var value = target.combobox('getValue');
                            //target.combobox('clear'); //清除原来的数据
                            var jsondata = [{ "propertyid": "", "propertytxt": "无" }];
                            for (var i = 0; i < Property.length; i++) {
                                if (Property[i].property != propertydata.propertyid) {
                                    jsondata.push({ "propertyid": Property[i].property, "propertytxt": Property[i].property });
                                }
                            }
                            target.combobox("loadData", jsondata);
                            if (value == propertydata.propertyid) target.combobox('setValue', '');
                        }
                    }
                }
            },
            {
                field: 'term', title: '条件', width: 80, align: 'center', editor: {
                    type: 'combobox', options: {
                        editable: false, required: true,
                        data: [{ "value": "=", "term": "等于" }, { "value": ">", "term": "大于" }, { "value": ">=", "term": "大于等于" }, { "value": "<", "term": "小于" }, { "value": "<=", "term": "小于等于" }, { "value": "<>", "term": "不等于" }, { "value": "is null", "term": "为空" }, { "value": "is not null", "term": "不为空" }, { "value": "like", "term": "属于" }, { "value": "not like", "term": "不属于" }],
                        valueField: 'value', textField: 'term', panelHeight: 'auto', missingMessage: '条件必选!'
                    }
                }
            },
            {
                field: 'variable', title: '变量', width: 120, align: 'center', editor: {
                    type: 'combobox', options: {
                        editable: false, required: true,
                        valueField: 'propertyid', textField: 'propertytxt', missingMessage: '变量必选!', panelHeight: 'auto'
                    }
                }
            },
            { field: 'compare', title: '值', editor: 'textbox', width: 100, align: 'center' }
        ]],
        onClickRow: function (rowIndex, rowData) {
            for (var n = datagrid.datagrid("getData").rows.length - 1; n >= 0; n--) {
                datagrid.datagrid("endEdit", n);
            }
            var rows = datagrid.datagrid('getEditingRowIndexs');
            if (rows.length == 0) {//单击开启编辑行
                datagrid.datagrid("beginEdit", rowIndex);
            }
        }
    });
}
function TflowRun() {
    $.messageBoxOpenID("<span class='ico_flowrun'></span>运行事务流程图", ['860px', '350px'], "#TransFlowRun");
    dg_in_output_arg(1, JSON.parse($("#start_input").val()));
}
function dg_in_output_arg(typeid, jsondata) {
    var limit = jsondata.length;
    layui.use('table', function () {
        var table = layui.table;
        table.render({
            elem: '#dg_input_arg' + typeid
            , toolbar: '#toolbarRun'
            , defaultToolbar:[]
            , data: jsondata
            , width: 410
            , height: 265
            , even: true
            , limit: limit
            , cols: [[
                { type: 'numbers', title: '', width: '10%' },
                { field: 'fldkey', title: '输入参数', width: '30%', align: 'left' },
                { field: 'fldname', title: '参数名称', width: '30%', align: 'center' },
                { field: 'fldvalue', title: '输入数值', width: '30%', align: 'center', edit: 'text' }
            ]]
        });

        table.on("toolbar(dg_input_arg" + typeid + ")", function (obj) {
            if (obj.event == "transt") {
                Activity_Run(typeid, false);
            }
            else {
                Activity_Run(typeid, true);
            }
        });
    });
}
function Activity_Run(typeid, commit) {
    var ActivityData = layui.table.cache["dg_input_arg" + typeid];
    if (typeid == 1 || $("#sqltype").val() == "sqlTransflow") {
        var parameters = [];
        for (var i = 0; i < ActivityData.length; i++) {
            parameters.push(ActivityData[i].fldvalue);
        }
        common.postGateway('platform/transflows/runtransflow', {
            "transflowCode": $("#activitycode").val(),
            "parameters": parameters,
            "commit": commit
        }, common.getToken(), function (result) {
            $("#jsonResult1").val(JSON.stringify(result.json) + "\n\n" + $("#jsonResult1").val());
        });
    }
    else if (typeid == 2) {
        var parameters = {}, sqltype = $("#sqltype").val(), sql = editorSqlContent.getValue();
        for (var i = 0; i < ActivityData.length; i++) {
            parameters[ActivityData[i].fldkey] = ActivityData[i].fldvalue;
            if (sqltype == "selectFirst" || sqltype == "selectAll" || sqltype == "sqlDML") {
                sql = sql.replace(new RegExp("@" + ActivityData[i].fldkey, "gm"), parameters[ActivityData[i].fldkey]);
            }
        }
        if (sqltype == "selectOne") {
            common.postGateway('platform/database/selectone', {
                "sql": common.Encrypt(sql),
                "parameters": parameters
            }, common.getToken(), function (result) {
                result = JSON.parse(result.json);
                $("#jsonResult2").val(JSON.stringify(result) + "\n\n" + $("#jsonResult2").val());
            });
        }
        else if (sqltype == "selectFirst" || sqltype == "selectFirstArg" || sqltype == "selectAll" || sqltype == "selectAllArg") {
            apiRoute.selectDataEvent(sql,[parameters],"1=1", function (result) {
                $("#jsonResult2").val(JSON.stringify(result) + "\n\n" + $("#jsonResult2").val());
            });
        }
        else if (sqltype == "MicroServiceGet" || sqltype == "MicroServicePost") {
            $.ajax({
                url: $("#MicroServiceUrl").val(),
                type: (sqltype == "MicroServiceGet"?'get':'post'),
                contentType: 'application/json; charset=UTF-8',
                dataType: 'json',
                data: JSON.stringify(parameters),
                success: function (response) {
                    $("#jsonResult2").val(JSON.stringify(response.json) + "\n\n" + $("#jsonResult2").val());
                }
            });
        }
        else {
            common.postGateway('platform/database/updatesql', {
                "sql": common.Encrypt(sql),
                "json": [parameters],
                "isEncrypt": true,
                "commit": commit
            }, common.getToken(), function (result) {
                $("#jsonResult2").val(JSON.stringify(result) + "\n\n" + $("#jsonResult2").val());
            });
        }
    }
}