var menuJson = [],editorJs;
$(function () {
    editorJs = CodeMirror.fromTextArea(document.getElementById("listJscript"), {
        lineNumbers: true,  //显示行数
        lineWrapping: true, //自动换行
        indentUnit: 3,   //缩进单位3
        styleActiveLine:true,   //当前行背景高亮
        matchBrackets: true,    //括号匹配
        continueComments: "Enter",
        mode: "javascript",       //模式
        foldGutter: true,//代码折叠
        gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter", "CodeMirror-lint-markers"],
        lint: true,
        hintOptions: {
            completeSingle: false// 当匹配只有一项的时候是否自动补全
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
    editorJs.setSize('90%', SetGridHeightSub(400)); 
    /*
    editorJs.on('cursorActivity', function () {
        editorJs.showHint();
    })
    */
    // 将自动提示绑定到change事件上，这样输入的时候就可以看到联想的关键词
    editorJs.on('change', function(instance, change)  {
        // 自动补全的时候，也会触发change事件，所有坐下判断，以免死循环，正则是为了不让空格，换行之类的也提示
        // 通过change对象你可以自定义一些规则去判断是否提示
        if (change.origin !== 'complete' && /\w|\./g.test(change.text[0])) {
            instance.showHint();
        }
    })

    var iconList = [];
    //页面窗口大小发现变化
    $(window).resize(function () {
        
    });
    $('.easyui-layout').layout('panel', 'center').panel({
        onResize: function (width, height) {
            try {
                $('#treeleft').css({ "height": SetGridHeightSub(40), "overflow-y": "auto" });
            }
            catch (e) { }
        }
    });

    $('.easyui-layout').layout('panel', 'west').panel({ title: '菜单列表' });
    $('#treeleft').css({ "height": SetGridHeightSub(40), "overflow-y": "auto" });
    //if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).CLIENT_ID != "") $("#projectMenu").show();
    //获取图标、菜单数据
    $.ajaxSettings.async = false;
    apiRoute.selectDataEvent("select iconcss from prod_system_icon",[], "1=1", function (result) {
        iconList = result;
    });
    loadMenu();
    $.ajaxSettings.async = true;
    //左侧菜单树
    $("#TreeList").tree({
        data: menuJson,
        onContextMenu: function (e, node) {
            e.preventDefault();
            $(this).tree('select', node.target);
            $('#TreeMenu').menu('show', {
                left: e.pageX,
                top: e.pageY
            });
        }
    });
    $("#TreeList").tree("collapseAll"); //全部折叠
    var node = $("#TreeList").tree('find', menuJson[0].id);
    $("#TreeList").tree('expand', node.target); //展开第一个节点

    $('#TreeMenu').menu('appendItem', {
        text: "添 加 菜 单",
        iconCls: "mdi mdi-plus",
        onclick: function () {
            var node = $('#TreeList').tree('getSelected');
            $("#tId").val("0");
            $("#menuParent").combotree("setValue", node.id);
            $("#moduleCode").textbox("setValue", "");
            $("#dataSource").combobox('setValue', "");
            $("#projectSet").combobox('setValue', "-1");
            $("#menuName").textbox("setValue", "");
            $("#menuIcon").combo('setValue', "");
            $("#imgBg").combobox('setValue', "");
            $("#menuImg").combobox('setValue', "");
            $("#menuIndex").numberbox("setValue", "");
            $("#menuUrl").combobox("setValue", "");
            
            $("#listJson").val("");
            $.setValue(editorJs, "");
            if (node.ismenu == "0") {
                $("#isMenu").switchbutton({ checked: false });
            }
            else {
                $("#isMenu").switchbutton({ checked: true });
            }
            $("#isWorkflow").switchbutton({ checked: false });
            $("#isNavigate").switchbutton({ checked: true });
            $("#isModule").switchbutton({ checked: false });
            $("#saveIcon").attr("class", "mdi mdi-plus");
            if (common.getUrlParam("ClientId") != "") {
                $("#saveIcon").html("&nbsp;&nbsp;添加项目菜单配置");
            }
            else {
                $("#saveIcon").html("&nbsp;&nbsp;添加产品菜单配置");
            }
            $("#clientId").val(common.getUrlParam("ClientId"));
        }
    });
    $('#TreeMenu').menu('appendItem', {
        separator: true
    });
    $('#TreeMenu').menu('appendItem', {
        text: "修 改 菜 单",
        iconCls: "mdi mdi-pencil",
        onclick: function () {
            var node = $('#TreeList').tree('getSelected');
            getSelected(node);
        }
    });
    $('#TreeMenu').menu('appendItem', {
        separator: true
    });
    $('#TreeMenu').menu('appendItem', {
        text: "删 除 菜 单",
        iconCls: "mdi mdi-trash-can-outline",
        onclick: function () {
            var node = $('#TreeList').tree('getSelected');
            if (node.client_id != common.getUrlParam("ClientId") && common.getUrlParam("ClientId")!="") {
                $.messageBoxBottom(0, "项目中无法删除产品菜单！");
                return;
            }
            $.messageBoxConfirm('你要删除所选择的记录吗？',['确定', '取消'], function () {
                if (node.ismodule == "0") {
                    common.postGateway("platform/menu/menudetele", {
                        appcode: common.getUrlParam("AppCode"),
                        id: node.id
                    }, common.getToken(), function (result) {
                        $.messageBoxBottom(1, result.json);
                        var nodeParent = $('#TreeList').tree('getParent', node.target);
                        apiRoute.initializationSystem(4, function (userinfo) {
                            sessionStorage["encrypt"] = userinfo.json.encrypt;
                            frameReturnByMenuLoad(nodeParent.id);
                        });
                    });
                }
                else {
                    $.messageBoxBottom(0, "请先关闭共享模块再删除！");
                }
            });
        }
    });
    $('#TreeMenu').menu('appendItem', {
        separator: true
    });
    $('#TreeMenu').menu('appendItem', {
        text: "复 制 配 置",
        iconCls: "mdi mdi-content-cut",
        onclick: function () {
            var node = $('#TreeList').tree('getSelected');
            $.messageLoad();
            apiRoute.selectDataEvent("select * from prod_app_module  where t_id='" + node.id + "'", [], "1=1", function (tableData) {
                var textareaEl = document.createElement('textarea');
                textareaEl.setAttribute('readonly', 'readonly'); // 防止手机上弹出软键盘
                textareaEl.value = JSON.stringify(tableData);
                document.body.appendChild(textareaEl);
                textareaEl.select();
                var res = document.execCommand('copy');
                document.body.removeChild(textareaEl);
                $.messageBoxCloseLoading();
            });
        }
    });
    $('#TreeMenu').menu('appendItem', {
        separator: true
    });
    $('#TreeMenu').menu('appendItem', {
        text: "粘 贴 配 置",
        iconCls: "mdi mdi-content-copy",
        onclick: function () {
            var node = $('#TreeList').tree('getSelected');
            if (node.client_id != common.getUrlParam("ClientId") && common.getUrlParam("ClientId") != "") {
                $.messageBoxBottom(0, "项目中无法粘贴菜单配置！");
                return;
            }
            $.messageBoxOpenBtn("粘贴菜单配置", ['300px', '300px'], "<div style='padding:20px;line-height:135%'>"
                + "<textarea id=\"menuJson\" style=\"width:260px;height:150px\" rows=\"8\"></textarea>"
                + "</div > ", [
                {
                    text: '提交',
                    handler: function (index) {
                        var json = $("#menuJson").val().trim();
                        if (!json) {
                            $("#menuJson").focus();
                            $.messageBoxBottom(0, "请按Ctrl+V粘贴配置信息！");
                        }
                        if (json.indexOf("[{") != 0) {
                            $("#menuJson").focus();
                            $.messageBoxBottom(0, "菜单配置信息有误，请检查！");
                        }
                        else {
                            $.messageLoad();
                            apiRoute.runTaskEvent("","update prod_app_module set moduleurl=@moduleurl,modulejson=@modulejson,modulehtml=@modulehtml,modulejscript=@modulejscript,description=@description where t_id='" + node.id+"'", JSON.parse(json), function (result) {
                                apiRoute.initializationSystem(4, function (userinfo) {
                                    sessionStorage["encrypt"] = userinfo.json.encrypt;
                                    frameReturnByMenuLoad(node.id);
                                });
                                $.messageBoxClose(index);
                            });
                        }
                    }
                }, {
                    text: '取消',
                    handler: function (index) {
                        $.messageBoxClose(index);
                    }
                }
            ]);
        }
    });
    //右侧控件 
    $("#menuParent").combotree({
        data: menuJson,
        required: true,
        showItemIcon: true,
        editable: false,
        panelMaxHeight: 250,
        onShowPanel: function () {  //显示下拉列表展开到当前节点                    
            var t = $(this).combotree('tree');
            t.tree("collapseAll");
            var node = t.tree('getSelected');
            if (node) {
                t.tree('expand', node.target); //选中节点展开
                t.tree('expandTo', node.target); //从根部展开一个指定的节点
                t.tree('scrollTo', node.target);
            }
            else {
                t.tree("expandAll");
            }
        }
    });
    apiRoute.dictionaryConfig("1059998107359645696", false, function (result) {
        var imgBg = result["1059998107359645696"];
        $("#imgBg").combobox({
            data: imgBg,
            editable: false,
            panelWidth: 265,
            panelMaxHeight: 250,
            hasDownArrow: false,
            width: 265,
            valueField: 'id',
            textField: 'text',
            formatter: JQueryEasyUI.dataDictionaryFormat
        });
    });

    common.postGateway('platform/file/filelist', { from: "Web", fileName: "img/icon" }, common.getToken(), function (result) {
        result = result.json;
        result.unshift({filename:""});
        $("#menuImg").combo({
            editable: false,
            panelWidth: 480,
            prompt: '                点击选择图片',
            panelMaxHeight: 250,
            hasDownArrow: false,
            width: 265,
            onShowPanel: function () {
                var picHtml = '';
                for (var k = 0; k < result.length; k++) {
                    if ($(this).combo('getValue') == result[k].filename) {
                        picHtml += '<a value="' + result[k].filename + '" text="' + result[k].filename + '" style="width:30px;height:30px;padding: 5px;position: relative;background-color: #39CAFF;" class="btn btn-primary"><span style="background:url(../../../img/icon/' + result[k].filename + ') no-repeat center;background-size:cover;position: absolute;top: 0;left: 0;height: 100%;width: 100%;" class="' + result[k].filename + '"></span></a>';
                    }
                    else {
                        picHtml += '<a value="' + result[k].filename + '" text="' + result[k].filename + '" style="width:30px;height:30px;padding: 5px;position: relative;background-color: #39CAFF;" class="btn btn-default"><span style="background:url(../../../img/icon/' + result[k].filename + ') no-repeat center;background-size:cover;position: absolute;top: 0;left: 0;height: 100%;width: 100%;" class="' + result[k].filename + '"></span></a>';
                    }
                }
                $('#imgCtrl').html(picHtml);
            }
        });
        $('#imgCtrl').appendTo($('#menuImg').combo('panel'));
        $('#imgCtrl').delegate('a', 'click', function () {
            var value = $(this).attr('value');
            var text = $(this).attr('text');
            $("#menuImg").combo({ buttonIcon: text, prompt: '            点击更换图片' }).combo('setValue', value).combo('hidePanel');
            $("#menuImg").next('.combo').children('a').css({ "background": "#39CAFF", "width": "40px" });
            $("#menuImg").next('.combo').children().children('span').css({ "background": "url(../../../img/icon/" + value+") no-repeat center","background-size": "cover","top": 0,"left": 0, "height": "100%","width": "100%"})
        });
    })
    $("#menuIcon").combo({
        editable: false,
        panelWidth: 480,
        prompt: '            点击选择图标',
        panelMaxHeight: 250,
        hasDownArrow: false,
        width: 265,
        onShowPanel: function () {
            var picHtml = '';
            for (var k = 0; k < iconList.length; k++) {
                if ($(this).combo('getValue') == iconList[k].iconcss) {
                    picHtml += '<a value="' + iconList[k].iconcss + '" text="' + iconList[k].iconcss + '" style="width:20px;" class="btn btn-primary"><span class="' + iconList[k].iconcss + '"></span></a>';
                }
                else {
                    picHtml += '<a value="' + iconList[k].iconcss + '" text="' + iconList[k].iconcss + '" style="width:20px" class="btn btn-default"><span class="' + iconList[k].iconcss + '"></span></a>';
                }
            }
            $('#picCtrl').html(picHtml);
        }
    });
    $('#picCtrl').appendTo($('#menuIcon').combo('panel'));
    $('#picCtrl').delegate('a', 'click', function () {
        var value = $(this).attr('value');
        var text = $(this).attr('text');
        $("#menuIcon").combo({ buttonIcon: text, prompt: '            点击更换图标' }).combo('setValue', value).combo('hidePanel');
    });

    $("#menuUrl").combobox({
        data: [{ "id": "/Admin/Page/PageForm/1", "text": "简单录入页模版" },
                { "id": "/Admin/Page/PageForm/2", "text": "主从表录入页模版" },
                { "id": "/Admin/Page/PageList/1", "text": "简单列表页模版" },
                { "id": "/Admin/Page/PageList/2", "text": "左侧树右侧列表模版" },
                { "id": "/Admin/Page/PageList/3", "text": "主从表列表页模版" },
                { "id": "/Admin/Page/PageList/4", "text": "左侧树右侧主从表模版" },
                { "id": "/Admin/Page/PageWebPage", "text": "左侧树右侧页面" },
                { "id": "/Admin/Page/PageFree", "text": "自由表单设计器" },
                { "id": "/Admin/Page/PageChart", "text": "图表模版设计器" }],
        panelHeight: 'auto',
        panelMaxHeight: 250,
        valueField: 'id',
        textField: 'text'
    });


    $("#openWebConfig").click(function () {
        var pageArray = ["/Admin/Page/PageForm/1", "/Admin/Page/PageForm/2", "/Admin/Page/PageList/1", "/Admin/Page/PageList/2", "/Admin/Page/PageList/3", "/Admin/Page/PageList/4", "/Admin/Page/PageWebPage", "/Admin/Page/PageFree", "/Admin/Page/PageChart"];
        var TemplateID = $.inArray($("#menuUrl").combobox("getValue"), pageArray);
        if ($("#saveIcon").attr("class") != "mdi mdi-pencil" || $("#tId").val() == "0") {
            $.messageBoxAlert(0, "请先保存菜单后再进行模版页面配置！");
        }
        else if (TemplateID == -1) {
            $.messageBoxAlert(0, "未选择指定的模版无法进行配置！");
        }
        else if (common.getUrlParam("ClientId") != $("#clientId").val()) {
            $.messageBoxAlert(0, "项目中无法修改产品页面配置！");
        }
        else if (TemplateID == 7) {
            $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='/Admin/System/FormDesigner/" + $("#tId").val() + "?AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId") +"'></iframe>");
            $("#modalwindow").window({ title: "自由表单设计器-" + $("#menuName").val(), maximized: true, width: SetGridWidthSub(100), height: SetGridHeightSub(50), iconCls: "mdi mdi-settings-outline" }).window('open');
        }
        else if (TemplateID == 8) {
            $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='/Admin/System/ChartsConfig/" + $("#tId").val() + "?AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId") +"'></iframe>");
            $("#modalwindow").window({ title: "图表设计器-" + $("#menuName").val(), maximized: true, width: SetGridWidthSub(100), height: SetGridHeightSub(50), iconCls: "mdi mdi-settings-outline" }).window('open');
        }
        else {
            $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='/Admin/System/WebConfig/" + $("#tId").val() + "?templateId=" + TemplateID + "&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId") +"'></iframe>");
            $("#modalwindow").window({ title: "配置页面-" + $("#menuName").val(), maximized: true, width: SetGridWidthSub(100), height: SetGridHeightSub(50), iconCls: "mdi mdi-settings-outline" }).window('open');
        }
    });

    //保存操作
    $("#btnSave").click(function () {
        if ($("#isMenu").switchbutton("options").checked) {
            var node = $('#TreeList').tree('getSelected');
            var pnode = $('#TreeList').tree('getParent', node.target);
            if (node.ismenu == "0" || (pnode != null && pnode.ismenu == "0")) {
                $.messageBoxBottom(0, "该功能为表单，不能设为菜单！");
                return;
            }
        }
        if ($("#isModule").switchbutton("options").checked) {
            var node = $('#TreeList').tree('getSelected');
            var pnode = $('#TreeList').tree('getParent', node.target);
            if (pnode != null && pnode.ismodule == "0") {
                $.messageBoxBottom(0, "该菜单模块不符合共享规则！");
                return;
            }
        }
        if ($("#saveIcon").attr("class") == "mdi mdi-content-save-outline") {
            $.messageBoxBottom(0, "请在左侧树节点上右击添加！");
            return;
        }
        else if ($("#tId").val() != "0" && $("#tId").val().trim() == $("#menuParent").combotree("getValue").trim()) {
            $.messageBoxBottom(0, "当前菜单和上级菜单冲突，无法保存！");
            return;
        }
        else if (!$("#moduleCode").textbox("getValue")) {
            $.messageBoxBottom(0, "模块代码不能为空！");
            return;
        }
        else if (!$("#menuName").textbox("getValue")) {
            $.messageBoxBottom(0, "模块名称不能为空！");
            return;
        }
        else if (common.getUrlParam("ClientId") != $("#clientId").val()) {
            $.messageBoxBottom(0, "项目中无法修改产品菜单！");
            return;
        }
        else {
            $.messageLoad();
            common.postGateway("platform/menu/menusave", {
                json: [{
                    "t_id": $("#tId").val(),
                    "p_id": $("#menuParent").combotree("getValue"),
                    "appcode": common.getUrlParam("AppCode"),
                    "modulecode": $("#moduleCode").textbox("getValue"),
                    "datasource": $("#dataSource").combobox('getValue'),
                    "modulename": $("#menuName").textbox("getValue"),
                    "moduleurl": $("#menuUrl").combobox("getValue"),
                    "projectset": $("#projectSet").combobox('getValue'),
                    "moduleseq": ($("#menuIndex").numberbox("getValue") ? parseFloat($("#menuIndex").numberbox("getValue")) : 0),
                    "moduleicon": $("#menuIcon").combobox("getValue"),
                    "moduleimg": $("#menuImg").combobox("getValue"),
                    "modulecolor": $("#imgBg").combo("getValue"),
                    "modulejson": $("#listJson").val(),
                    "modulejscript": editorJs.getValue(),//$("#listJscript").textbox("getValue")
                    "ismenu": ($("#isMenu").switchbutton("options").checked ? "1" : "0"),
                    "isworkflow": ($("#isWorkflow").switchbutton("options").checked ? "1" : "0"),
                    "isnavigate": ($("#isNavigate").switchbutton("options").checked ? "1" : "0"),
                    "ismodule": ($("#isModule").switchbutton("options").checked ? "1" : "0")
                }]
            }, common.getToken(), function (menuid) {
                if (menuid.errcode != 0) {
                    $.messageBoxBottom(0, menuid.json);
                    $.messageBoxCloseLoading();
                }
                else {
                    apiRoute.initializationSystem(4,  function (userinfo) {
                        sessionStorage["encrypt"] = userinfo.json.encrypt;
                        frameReturnByMenuLoad(menuid.json);
                    });
                }
            });
        }
    });

    $("#dataBase").combobox({
        loader: function (param, success, error) {
            apiRoute.selectDataEvent("select DBCODE id,DBNOTE text from prod_system_database", [], "1=1", function (tableData) {
                success(tableData);
            });
        },
        editable: false,
        panelHeight: 'auto',
        valueField: 'id',
        textField: 'text'
    });
    $("#dataSource").combobox({
        loader: function (param, success, error) {
            apiRoute.selectDataEvent("select DBCODE id,DBCODE text,DBNOTE description from prod_system_database", [], "1=1", function (tableData) {
                tableData.unshift({ "id": "", "text": "当前数据源", "description": "默认数据源" });
                success(tableData);
            });
        },
        onLoadSuccess: function (data) {
            $(this).combobox("setValue", "");
        },
        editable: false,
        panelMaxHeight: 250,
        panelHeight: 'auto',
        valueField: 'id',
        textField: 'text',
        formatter: JQueryEasyUI.dataDictionaryFormat
    });
    $("#projectSet").combobox({
        data: [{ "id": "-1", "text": "使用产品菜单" }, { "id": "0", "text": "隐藏该产品菜单" }, { "id": "1", "text": "复制产品菜单到项目" }],
        onLoadSuccess: function (data) {
            if (common.getUrlParam("ClientId") && common.getUrlParam("ClientId") != "") {
                $(this).combobox('enable');   //可用
            }
            else {
                $(this).combobox('disable');   //不可用
            }            
        },
        onChange: function (newValue, oldValue) {
            if (common.getUrlParam("ClientId") && common.getUrlParam("ClientId") != "") {
                if (newValue == 1) {
                    $("#saveIcon").html("&nbsp;&nbsp;修改项目菜单配置");
                    $("#clientId").val(common.getUrlParam("ClientId"));
                }
                else {
                    $("#saveIcon").html("&nbsp;&nbsp;无法修改产品菜单");
                    if ($("#tId").val() != "" && $("#tId").val() != "0") {
                        var sql = "delete from prod_app_project_module where p_id='" + $("#tId").val() + "' and appcode='" + common.getUrlParam("AppCode") + "' and CLIENT_ID='" + common.getUrlParam("ClientId") +"' "
                        if (newValue == 0) {
                            sql += ";insert into prod_app_project_module(t_id,CLIENT_ID,P_ID,APPCODE,CREATE_DATE,created_by,modify_ip,ACT_STATUS) values(@t_id,'&CLIENT_ID&',@p_id,'" + common.getUrlParam("AppCode") +"',now(),@created_by,@modify_ip,'0')"
                        }
                        $.messageLoad();
                        apiRoute.runTaskEvent("", sql, [{}, {
                            "t_id": "", "p_id": $("#tId").val()
                        }], function (result) {
                            apiRoute.initializationSystem(4, function (userinfo) {
                                sessionStorage["encrypt"] = userinfo.json.encrypt;
                                loadMenu(function () {
                                    $("#TreeList").tree("loadData", menuJson);
                                    var node = $('#TreeList').tree('find', $("#tId").val());
                                    $("#TreeList").tree("collapseAll");
                                    $("#TreeList").tree('expandTo', node.target);
                                    $('#TreeList').tree('select', node.target);
                                });
                            });
                            $.messageBoxCloseLoading();
                        });
                    }
                }
            }
        },
        editable: false,
        panelMaxHeight: 250,
        panelHeight: 'auto',
        valueField: 'id',
        textField: 'text'
    });
    function CheckDataBase(text,opt) {
        if ($("#tId").val() == "0") {
            $.messageBoxAlert(0, "对不起，未选择任何菜单！");
        }
        else if (!$("#dataBase").combobox("getValue")) {
            $.messageBoxAlert(0, "对不起，请选择即将操作的数据库！");
        }
        else {
            $.messageBoxConfirm(text, ['确定', '取消'], function () {
                $.messageLoad();
                common.postGateway('platform/menu/menusynchronous', {
                    "t_id": $("#tId").val(),
                    "dbCode": $("#dataBase").combobox("getValue"),
                    "isSynchronous": opt
                }, common.getToken(), function (tableData) {
                    if (tableData.json == "OK") {
                        $.messageBoxAlert(1, "两个数据库配置同步成功！");
                    }
                    else {
                        $.messageBoxAlert(0, tableData);
                    }
                    $.messageBoxCloseLoading();
                });
            });
        }
    }
    $("#btnUp").click(function () {
        CheckDataBase("确定要上传该配置到选择数据库", true);
    });
    $("#btnDown").click(function () {
        CheckDataBase("确定要从选择数据库下载配置", false);
    });
});

function loadMenu(menuFun) {
    common.postGateway('platform/system/menuconfig', {
        id: "0"
    }, common.getToken(), function (result) {
        result = JSON.parse(result.json);
        if (result == "noLogin") {
            common.removeCookie("sessionKey");
            common.setCookie("sessionKey", "noLogin");
            window.localStorage.removeItem("token");
            $.messageBoxAlert(0, "对不起，登录超时！");
            return;
        }
        menuJson = [{
            "id": "0",
            "text": "顶级菜单",
            "iconCls": "mdi mdi-database",
            "children": result
        }];
        if (menuFun != null) menuFun();
    });
}

function frameReturnByMenuLoad(menuid) {
    loadMenu(function () {
        $("#TreeList").tree("loadData", menuJson);
        var node = $('#TreeList').tree('find', menuid);
        getSelected(node);
        $("#TreeList").tree("collapseAll");
        $("#TreeList").tree('expandTo', node.target);
        $('#TreeList').tree('select', node.target);
        $("#tId").val(menuid);
        $("#saveIcon").attr("class", "mdi mdi-pencil");
        $("#saveIcon").html("&nbsp;&nbsp;修改菜单配置");
        $.messageBoxBottom(1, "菜单管理操作成功！");
        $.messageBoxCloseLoading();
    });
}

//点击树节点时加载
function getSelected(node) {
    if (node.id == 0) {
        $("#tId").val(node.id);
        $("#menuParent").combotree("setValue", "");
        $("#moduleCode").textbox("setValue", "");
        $("#dataSource").combobox('setValue', "");
        $("#projectSet").combobox('setValue', "-1");
        $("#menuName").textbox("setValue", "");
        $("#menuIcon").combo('setValue', "");
        $("#imgBg").combobox('setValue', "");
        $("#menuImg").combobox('setValue', "");
        $("#menuIndex").numberbox("setValue", "");
        $("#menuUrl").combobox("setValue", "");
        $("#listJson").val("");
        $.setValue(editorJs, "");
        $("#isMenu").switchbutton({ checked: true });
        $("#isWorkflow").switchbutton({ checked: false });
        $("#isNavigate").switchbutton({ checked: true });
        $("#isModule").switchbutton({ checked: false });
        $("#saveIcon").attr("class", "mdi mdi-content-save-outline");
        $("#saveIcon").html("&nbsp;&nbsp;菜单配置");
    }
    else {
        $("#tId").val(node.id);
        $("#menuParent").combotree("setValue", node.parentid);
        $("#moduleCode").textbox("setValue", node.modulecode);
        $("#dataSource").combobox('setValue', node.datasourcecode);
        $("#menuName").textbox("setValue",node.text);
        $("#menuIcon").combo({ buttonIcon: node.iconCls, prompt: '            点击更换图标' }).combo('setValue', node.iconCls);
        document.getElementById("Span_menuIcon").className = (node.iconCls ? node.iconCls : "mdi mdi-more");
        $("#imgBg").combobox('setValue', node.imgColor);
        //$("#menuImg").combobox('setValue', node.iconImg);
        $("#menuImg").combo({ buttonIcon: node.iconImg, prompt: '            点击更换图片' }).combo('setValue', node.iconImg);
        $("#menuImg").next('.combo').children('a').css({ "background": "#39CAFF", "width": "40px" });
        $("#menuImg").next('.combo').children().children('span').css({ "background": "url(../../../img/icon/" + node.iconImg + ") no-repeat center", "background-size": "cover", "top": 0, "left": 0, "height": "100%", "width": "100%" })
        $("#menuIndex").numberbox("setValue", node.moduleseq);
        $("#menuUrl").combobox("setValue", node.moduleurl);
        $("#projectSet").combobox('setValue', node.projectset);
        $.messageLoad();
        apiRoute.selectDataEvent("select ifnull(b.modulejscript,a.modulejscript) modulejscript,ifnull(b.modulejson,a.modulejson) modulejson from prod_app_module a left join prod_app_project_module b on (a.t_id=b.p_id and b.CLIENT_ID='" + common.getUrlParam("ClientId") + "' and b.APPCODE='" + common.getUrlParam("AppCode")+"') where a.t_id='" + node.id + "'", [], "1=1", function (tableData) {
            $("#listJson").val(tableData[0].modulejson);
            $.setValue(editorJs, tableData[0].modulejscript);
            $.messageBoxCloseLoading();
        });

        $("#isMenu").switchbutton({ checked: ((node.ismenu == "1") ? true : false) });
        $("#isWorkflow").switchbutton({ checked: ((node.isworkflow == "1") ? true : false) });
        $("#isNavigate").switchbutton({ checked: ((node.isnavigate == "1") ? true : false) });
        $("#isModule").switchbutton({ checked: ((node.ismodule == "1") ? true : false) });
        $("#saveIcon").attr("class", "mdi mdi-pencil");
        if (node.client_id == common.getUrlParam("ClientId") && node.client_id != null && node.client_id != "" && common.getUrlParam("ClientId") != "") {
            $("#saveIcon").html("&nbsp;&nbsp;修改项目菜单配置");
        }
        else if ((node.client_id == null || node.client_id == "") && common.getUrlParam("ClientId") == "") {
            $("#saveIcon").html("&nbsp;&nbsp;修改产品菜单配置");
        }
        else {
            $("#saveIcon").html("&nbsp;&nbsp;无法修改产品菜单");
        }
        $("#clientId").val(node.client_id);
    }
}

function mdi_Open(id) {
    $("#" + id).combo('hidePanel');
    $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='/lib/MaterialDesign-Webfont-master/preview.html?id=" + id + "'></iframe>");
    $("#modalwindow").window({ title: '图标选择', width: SetGridWidthSub(200), height: SetGridHeightSub(100), iconCls: "mdi mdi-more" }).window('open');
}