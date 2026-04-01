var menuJson = [];
$(function () {
    var iconList = [];
    $('.easyui-layout').layout('panel', 'center').panel({
        onResize: function (width, height) {
            try {
                $('#treeleft').css({ "height": SetGridHeightSub(30), "overflow-y": "auto" });
                $('#ifameright').css({ "height": SetGridHeightSub(30), "overflow-y": "auto" });
                $("#iframeinitTab").css("min-height", SetGridHeightSub(40));
            }
            catch (e) { }
        }
    });
    $('#treeleft').css({ "height": SetGridHeightSub(30), "overflow-y": "auto" });
    $('#ifameright').css({ "height": SetGridHeightSub(30), "overflow-y": "auto" });
    $("#iframeinitTab").css("min-height", SetGridHeightSub(40));
    //获取图标、菜单数据
    $.ajaxSettings.async = false;
    apiRoute.selectDataEvent("select iconcss from prod_system_icon", [], "1=1", function (result) {
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
        text: "添 加",
        iconCls: "mdi mdi-plus",
        onclick: function () {
            var node = $('#TreeList').tree('getSelected');
            $("#tId").val("0");
            $("#ParentId").combotree("setValue", node.id);
            $("#rfCode").textbox("setValue", "");
            $("#rfName").textbox("setValue", "");
            $("#rfIcon").combo('setValue', "");
            $("#sortNo").numberbox("setValue", "");
            $("#isConfig").switchbutton({ checked: false });
            $("#saveIcon").attr("class", "mdi mdi-plus");
            $("#saveIcon").html("&nbsp;&nbsp;添加菜单配置");
        }
    });
    $('#TreeMenu').menu('appendItem', {
        separator: true
    });
    $('#TreeMenu').menu('appendItem', {
        text: "修 改",
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
        text: "删 除",
        iconCls: "mdi mdi-trash-can-outline",
        onclick: function () {
            $.messageBoxConfirm('你要删除所选择的记录吗？', ['确定', '取消'], function () {
                var node = $('#TreeList').tree('getSelected');
                apiRoute.runTaskEvent("","delete from prod_rf_config where t_id=@t_id", [{ "t_id": node.id }],
                    function (result) {
                        if (result.resMsg == "") {
                            $.messageBoxBottom(1, "所选节点删除成功！");
                            var nodeParent = $('#TreeList').tree('getParent', node.target);
                            frameReturnByMenuLoad(nodeParent.id);
                        }
                        else {
                            $.messageBoxBottom(0, result.resMsg);
                        }
                });
            });
        }
    });

    //右侧控件 
    $("#ParentId").combotree({
        data: menuJson,
        required: true,
        showItemIcon: true,
        editable: false,
        panelHeight: 'auto',
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

    $("#rfIcon").combo({
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
    $('#picCtrl').appendTo($('#rfIcon').combo('panel'));
    $('#picCtrl').delegate('a', 'click', function () {
        var value = $(this).attr('value');
        var text = $(this).attr('text');
        $("#rfIcon").combo({ buttonIcon: text, prompt: '            点击更换图标' }).combo('setValue', value).combo('hidePanel');
    });

    $("#openRfConfig").click(function () {
        if ($("#saveIcon").attr("class") != "mdi mdi-pencil" || $("#tId").val() == "0") {
            $.messageBoxAlert(0, "请先保存菜单后再进行模版页面配置！");
        }
        else if (!$("#isConfig").switchbutton("options").checked) {
            $.messageBoxAlert(0, "该节点不是子节点配置节点！");
        }
        else {
            $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='/RF/Main/Config?id=" + $("#tId").val() + "'></iframe>");
            $("#modalwindow").window({ title: "配置页面-" + $("#rfName").val(), maximized: true, width: SetGridWidthSub(100), height: SetGridHeightSub(50), iconCls: "mdi mdi-settings-outline" }).window('open');
        }
    });

    //保存操作
    $("#btnSave").click(function () {
        if ($("#saveIcon").attr("class") == "mdi mdi-content-save-outline") {
            $.messageBoxBottom(0, "请在左侧树节点上右击添加！");
            return;
        }
        else if ($("#tId").val() != "0" && $("#tId").val() == $("#ParentId").combotree("getValue")) {
            $.messageBoxBottom(0, "当前菜单和上级菜单冲突，无法保存！");
            return;
        }
        else if (!$("#rfCode").textbox("getValue")) {
            $.messageBoxBottom(0, "模块代码不能为空！");
            return;
        }
        else if (!$("#rfName").textbox("getValue")) {
            $.messageBoxBottom(0, "模块名称不能为空！");
            return;
        }
        else {
            $.messageLoad();
            var sql = "";
            if ($("#tId").val() == "0") {
                sql = "insert into prod_rf_config(t_id,p_id,appcode,rfcode,rfname,rfjson,rficon,isconfig,sortno,create_date,created_by,modify_ip,act_status) values "
                    + "(@t_id,@p_id,@appcode,@rfcode,@rfname,'',@rficon,@isconfig,@sortno,now(),@created_by,@modify_ip,'1')"
            }
            else {
                sql = "update prod_rf_config set p_id=@p_id,appcode=@appcode,rfcode=@rfcode,rfname=@rfname,rficon=@rficon,isconfig=@isconfig,sortno=@sortno,MODIFY_DATE=now(),LAST_MODIFIED_BY=@created_by,modify_ip=@modify_ip where t_id=@t_id";
            }
            apiRoute.runTaskEvent("",sql,[{
                    "t_id": $("#tId").val(),
                    "p_id": $("#ParentId").combotree("getValue"),
                    "appcode": common.getUrlParam("AppCode"),
                    "rfcode": $("#rfCode").textbox("getValue"),
                    "rfname": $("#rfName").textbox("getValue"),
                    "rficon": $("#rfIcon").combo("getValue"),
                    "isconfig": ($("#isConfig").switchbutton("options").checked ? "1" : "0"),
                    "sortno": ($("#sortNo").numberbox("getValue") ? $("#sortNo").numberbox("getValue") : 0)
                }], function (result) {
                if (result.resMsg == "") {
                    frameReturnByMenuLoad(result.resId);
                }
                else {
                    $.messageBoxBottom(0, result.resMsg);
                    $.messageBoxCloseLoading();
                }
            });
        }
    });
});

function loadMenu(menuFun) {
    apiRoute.selectTreeEvent("","select t_id id,p_id pid,rficon,appcode,rfcode,rfname,isconfig,rfjson,sortno from prod_rf_config where appcode='" + common.getUrlParam("AppCode")+"'",
        "id", "pid","rfname", "0", [], function (result) {
        for (var i = 0; i < result.length; i++) {
            result[i].iconCls = result[i].rficon;
        }
        menuJson = [{
            "id": "0",
            "text": common.getUrlParam("AppCode") + "应用菜单",
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
        $("#ParentId").combotree("setValue", "");
        $("#rfCode").textbox("setValue", "");
        $("#rfName").textbox("setValue", "");
        $("#rfIcon").combo('setValue', "");
        $("#sortNo").numberbox("setValue", "");
        $("#isConfig").switchbutton({ checked: false });
        $("#saveIcon").attr("class", "mdi mdi-content-save-outline");
        $("#saveIcon").html("&nbsp;&nbsp;菜单配置");
        $("#iframeinitTab").attr("src", "about:blank");
    }
    else {
        $("#tId").val(node.id);
        $("#ParentId").combotree("setValue", node.pid);
        $("#rfCode").textbox("setValue", node.rfcode);
        $("#rfName").textbox("setValue", node.text);
        $("#rfIcon").combo({ buttonIcon: node.rficon, prompt: '            点击更换图标' }).combo('setValue', node.rficon);
        document.getElementById("Span_rfIcon").className = (node.rficon ? node.rficon :"mdi mdi-more");
        $("#sortNo").numberbox("setValue", node.sortno);
        $("#isConfig").switchbutton({ checked: ((node.isconfig == "1") ? true : false) });
        $("#saveIcon").attr("class", "mdi mdi-pencil");
        $("#saveIcon").html("&nbsp;&nbsp;修改菜单配置");
        refersh(false);
    }
}

function mdi_Open(id) {
    $("#" + id).combo('hidePanel');
    $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='/lib/MaterialDesign-Webfont-master/preview.html?id=" + id + "'></iframe>");
    $("#modalwindow").window({ title: '图标选择', width: SetGridWidthSub(200), height: SetGridHeightSub(100), iconCls: "mdi mdi-more" }).window('open');
}

function refersh(isRefersh) {
    if (isRefersh) {
        if ($("#tId").val() == "0") {
            $.messageBoxBottom(0, "没有需要浏览的界面！");
        }
        else if ($("#isConfig").switchbutton("options").checked == false) {
            $.messageBoxBottom(0, "当前菜单不是配置节点！");
        }
        else {
            $("#iframeinitTab").attr("src", "/rf/main/rfpagelist?AppCode=" + common.getUrlParam("AppCode") + "&id=" + $("#tId").val());
        }
    }
    else if ($("#tId").val() != "0" && $("#isConfig").switchbutton("options").checked == true) {
        $("#iframeinitTab").attr("src", "/rf/main/rfpagelist?AppCode=" + common.getUrlParam("AppCode") + "&id=" + $("#tId").val());
    }
}