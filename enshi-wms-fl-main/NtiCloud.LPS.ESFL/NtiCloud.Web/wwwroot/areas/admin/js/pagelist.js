var btnList = {}, btnSql, click_main_id = 0, dataSourceCode = '', btnText, formContent, formPermission, formid_config = common.getUrlParam("formid");//通过ID获取模版的配置信息
$(function () {
    var openSelectData = [];
    if (common.getUrlParam("pagelist") == "selectOpen" || common.getUrlParam("leftlist")=="listOpen" || common.getUrlParam("tablelist") == "focusOpen") {//作为下列控件弹出
        $('.pageList_layout').layout('remove', 'north');
        $("#searchList").show();
        if (common.getUrlParam("leftlist") != "listOpen") {  //listOpen为左侧选择列表
            var ctrlName = common.getUrlParam("ctrlName");
            if (common.getUrlParam("tablelist") == "focusOpen") { //批量操作列表页
                if (parent.$('#List').datagrid('getRows')[parent.cellIndex][ctrlName]) {
                    openSelectData = parent.$('#List').datagrid('getRows')[parent.cellIndex][ctrlName].split(',');
                }
            } else {  //pageform中下拉弹列表
                openSelectData = parent.$("#" + ctrlName).combobox("getValues");
            }
        }
    }
    if (parseInt($("#TemplateID").val()) % 2 == 1) {//没有左侧树
        $('.pageList_layout').layout('remove', 'west');
    }
    if (parseInt($("#TemplateID").val()) <= 2) {//没有从表
        $('#tabs').hide();
    }
    else {
        $("#tabs").tabs({
            width: $('.pageList_layout').layout('panel', 'center')[0].clientWidth - 8,
            height: (SetGridHeightSub($('.pageList_layout').layout('panel', 'north')[0].clientHeight+3) - ($("#searchList").is(":visible") ? $("#searchList").height() : 0)) / 2 - 20,
            onSelect: function (title, index) {
                for (var i = 1; i < formContent.tableData.length; i++) {
                    if (formContent.tableData[i].pageTitle == title) {
                        $("#btSubList" + i).show();
                        $('#List' + i).datagrid();
                    }
                    else {
                        $("#btSubList" + i).hide();
                    }
                }
            }
        });
    }
    //$('#txtQuery').searchbox().textbox('addClearBtn', 'icon-clear');
    $("#btnSearch").click(function () {
        if ($(this).find("span").attr("class") == "mdi mdi-chevron-double-down") {
            $(this).find("span").attr("class", "mdi mdi-chevron-double-up");
            $("#searchList").show();
        }
        else {
            $(this).find("span").attr("class", "mdi mdi-chevron-double-down");
            $("#searchList").hide();
        }
        $('#List0').datagrid('resize', {
            height: ((parseInt($("#TemplateID").val()) <= 2) ? (SetGridHeightSub($('.pageList_layout').layout('panel', 'north')[0].clientHeight + 3) - ($("#searchList").is(":visible") ? $("#searchList").height() : 0)) : (SetGridHeightSub($('.pageList_layout').layout('panel', 'north')[0].clientHeight+3) - ($("#searchList").is(":visible") ? $("#searchList").height() : 0)) / 2 + 20)
        });
        if (parseInt($("#TemplateID").val()) > 2) {
            $("#tabs").tabs({
                height: (SetGridHeightSub($('.pageList_layout').layout('panel', 'north')[0].clientHeight+3) - ($("#searchList").is(":visible") ? $("#searchList").height() : 0) - 3) / 2 - 20
            });
            for (var i = 1; i <= $('#tabs').tabs('tabs').length; i++) {
                $('#List' + i).datagrid('resize', {
                    height: (SetGridHeightSub($('.pageList_layout').layout('panel', 'north')[0].clientHeight + 3) - ($("#searchList").is(":visible") ? $("#searchList").height() : 0) - 3) / 2 - 20 - 35//(SetGridHeightSub($('.pageList_layout').layout('panel', 'north')[0].clientHeight+3) - ($("#searchList").is(":visible") ? $("#searchList").height() : 0)) / 2 - 20 - 35
                });
            }
        }
        $('#treeleft').css({ "height": SetGridHeightSub(100), "overflow-y": "auto" });
    });
    if (formid_config != null && formid_config != "") {
        var dataEditButton = [];
        //优化平台配置及权限相关信息
        $.ajaxSettings.async = false;
        formPermission = {};
        common.postGateway('platform/permission/pagedataset', {
            "menuId": formid_config
        }, common.getToken(), function (data) {
            if (data.errcode == "0") {
                var webPageData = JSON.parse(data.json);
                for (var i = 0; i < webPageData.Table1.length; i++) {
                    btnList[webPageData.Table1[i].button_code] = {};
                    btnList[webPageData.Table1[i].button_code].code = webPageData.Table1[i].button_code;
                    btnList[webPageData.Table1[i].button_code].js = webPageData.Table1[i].button_js;
                    btnList[webPageData.Table1[i].button_code].name = webPageData.Table1[i].button_name;
                    btnList[webPageData.Table1[i].button_code].icon = webPageData.Table1[i].button_icon;
                }
                $("title").html(webPageData.Table2[0].text);
                dataSourceCode = webPageData.Table2[0].datasourcecode;
                formContent = JSON.parse(common.Decrypt(webPageData.Table2[0].modulejson));
                formPermission.buttonpermission = webPageData.Table3;//获取访问按钮权限
                formPermission.fieldpermission = webPageData.Table4;//获取访问字段权限

                if (formContent.isPrintPage && formContent.isPrintPage == "1" && formContent.printFileName && formContent.printFileName!="") {//显示打印按钮
                    if (formContent.printBtnName && formContent.printBtnName != "") {
                        $(".mdi-printer").html("&nbsp;&nbsp;" + formContent.printBtnName);
                    }
                    $("#btnPrint").show();
                }
            }
            else if (data.errcode == "400") {
                common.removeCookie("sessionKey");
                common.setCookie("sessionKey", "noLogin");
                window.localStorage.removeItem("token");
                $.messageBoxBottom(0, "对不起，登录超时！");
            }
            else {
                $.messageBoxBottom(0, data.errmsg);
            }
        });
        $.ajaxSettings.async = true;

        if (window.RunOnLoad && typeof (RunOnLoad) == 'function') {
            RunOnLoad();
        }
        GeneralQuery(0);

        if (parseInt($("#TemplateID").val()) % 2 == 0) {//加载左侧树
            $("#StructTree").tree({
                loader: function (param, success, error) {
                    apiRoute.selectTreeEvent(dataSourceCode, formContent.treeData.treeSql,
                        formContent.treeData.treeId, formContent.treeData.treePid, formContent.treeData.treeText,"0", [], function (tableData) {
                        success(tableData);
                    });
                },
                onLoadSuccess: function (node, data) {
                    if (formContent.treeData.treeControlInitMode == "CollapseAll" || formContent.treeData.treeControlInitMode == "ExpandFirstSubNode") {
                        $("#StructTree").tree("collapseAll"); //全部折叠
                    }
                    if (formContent.treeData.treeControlInitMode == "ExpandFirstSubNode" && data.length > 0) {
                        var node = $("#StructTree").tree('find', data[0].id);
                        $("#StructTree").tree('expand', node.target); //展开第一个节点
                    }
                },
                onContextMenu: function (e, node) {
                    e.preventDefault();
                    $(this).tree('select', node.target);
                    if (node.id != "" && document.getElementById("treeMenu").getElementsByTagName("div").length > 1) {
                        $('#treeMenu').menu('show', {
                            left: e.pageX,
                            top: e.pageY
                        });
                    }
                }
            });
            $('.pageList_layout').layout('panel', 'west').panel({ title: (formContent.tableData[0].pageTitle ? formContent.tableData[0].pageTitle : $("title").html()) + '分类' });
            $('#treeleft').css({ "height": SetGridHeightSub(100), "overflow-y": "auto" });
            for (var i = 0; i < formContent.tableData[0].buttonData.length; i++) {
                if (formContent.tableData[0].buttonData[i].btnId == "Create" || formContent.tableData[0].buttonData[i].btnId == "Edit") {
                    ButtonTree(formContent.tableData[0].buttonData[i].btnId, formContent.tableData[0].buttonData[i].btnSql);
                }
            }
            function ButtonTree(code, action) {
                $('#treeMenu').menu('appendItem', {
                    text: btnList[code].name,
                    iconCls: btnList[code].icon,
                    onclick: function () {
                        $('#treeMenu').menu('hide');
                        var pageUrl = "/Admin/Page/PageForm?btCode=" + code + "&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId")+"&formid=" + formid_config, node = $('#StructTree').tree('getSelected');
                        btnSql = action;
                        if (code == "Create") {
                            pageUrl = pageUrl + "&Load=Complete&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId")+"&treeid=" + node.id
                        }
                        else {
                            pageUrl = pageUrl + "&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId")+"&id=" + node.id
                        }
                        $("#modalwindow").html("<iframe width='100%' height='99%' id='modalwindow' frameborder='0' src='" + pageUrl + "'></iframe>");
                        $("#modalwindow").window({ title: btnList[code].name, maximized: true, width: SetGridWidthSub(100), height: SetGridHeightSub(50), iconCls: btnList[code].icon }).window('open');
                    }
                });
                $('#treeMenu').menu('appendItem', {
                    separator: true
                });
            }
        }

        for (var i = 0; i < formContent.tableData.length; i++) {//对配置中的按钮、表进行处理
            if (formContent.tableData[i].tableType == 'openTable') break;
            dataEditButton.push("");
            $(".mvctool").append('<span style="display:none"><div id="rightMenu' + i + '" class="easyui-menu"></div></span>');
            $.parser.parse($("#rightMenu" + i).parent());
            for (var j = 0; j < formContent.tableData[i].buttonData.length; j++) {
                ButtonList(i, j);
            }
            ButtonSearch(i);
            ListTable(i);
        }

        function ButtonList(x, y) {//工具栏按钮或菜单
            var btnInfo = btnList[formContent.tableData[x].buttonData[y].btnId];
            var btCode = btnInfo.code, btJs = btnInfo.js, btText = btnInfo.name, btIcon = btnInfo.icon;
            if (formContent.tableData[x].buttonData[y].btnText) btText = formContent.tableData[x].buttonData[y].btnText;
            if (formContent.tableData[x].buttonData[y].btnIcon) btIcon = formContent.tableData[x].buttonData[y].btnIcon;

            for (var m = 0; formPermission && m < formPermission.buttonpermission.length; m++) {//验证按钮权限
                if (x == formPermission.buttonpermission[m].button_owner && btCode+"_"+y == formPermission.buttonpermission[m].button_code) {
                    return;//无该按钮权限
                }
            }

            if (formContent.tableData[x].buttonData[y].btnAlign == "left") {
                if (x == 0) {
                    $(".mvctool").append("<a id=\"btn" + btCode + x + "_" + y + "\" class=\"btn btn-default\"><span class=\"" + btIcon + "\">&nbsp;&nbsp;" + btText + "</span></a>&nbsp;");
                    $("#btn" + btCode + x + "_" + y).tooltip({ content: btText });
                    $("#btn" + btCode + x + "_" + y).click(function () {
                        ButtonRun(x, y, btJs, btText, btIcon, -1);
                    });
                }
                else {
                    if ($("#btSubList" + x).length==0) {
                        $(".mvctool").append("<span id=\"btSubList" + x + "\" style=\"display:none;border-left:1px solid #b0a2a2\">&nbsp;</span>");
                    }
                    $("#btSubList" + x).append("<a id=\"btn" + btCode + x + "_" + y + "\" class=\"btn btn-default\"><span class=\"" + btIcon + "\">&nbsp;&nbsp;" + btText + "</span></a>&nbsp;");
                    $("#btn" + btCode + x + "_" + y).tooltip({ content: btText });
                    $("#btn" + btCode + x + "_" + y).click(function () {
                        ButtonRun(x, y, btJs, btText, btIcon, -1);
                    });
                }
            }
            else if (formContent.tableData[x].buttonData[y].btnAlign == "right") {
                $('#MoreMenu').menu('appendItem', {
                    text: btText,
                    iconCls: btIcon,
                    onclick: function () {
                        ButtonRun(x, y, btJs, btText, btIcon, -1);
                    }
                });
                $('#MoreMenu').menu('appendItem', {
                    separator: true
                });
            }
            else if (formContent.tableData[x].buttonData[y].btnAlign == "rightlist") {
                $('#rightMenu' + x).menu('appendItem', {
                    text: btText,
                    iconCls: btIcon,
                    onclick: function () {
                        ButtonRun(x, y, btJs, btText, btIcon, -1);
                    }
                });
                $('#rightMenu' + x).menu('appendItem', {
                    separator: true
                });
            }
            else if (formContent.tableData[x].buttonData[y].btnAlign == "list"){
                dataEditButton[x] += "<a onclick=\"gridButton('rowindex','" + x + "','" + y + "','" + btJs + "','" + btText + "','" + btIcon + "')\" title=\"" + btText + "\" class=\"btnline\"><span class=\"" + btIcon + "\"></span></a>";
            }
        }
        function ButtonSearch(x) {
            var text = '高级查询';
            if (x == 0 && formContent.tableData.length > 1) {
                text = '主表高级查询';
            }
            else if (x == 0) {
                text = '高级查询';
            }
            else {
                text = '从表' + x + "高级查询";
            }

            $('#MoreMenu').menu('appendItem', {
                text: text,
                iconCls: "mdi mdi-magnify",
                onclick: function () {
                    AdvSearch(x, formContent.tableData[x].pageTitle + '高级查询');
                }
            });
            $('#MoreMenu').menu('appendItem', {
                separator: true
            });
        }
        $('#MoreMenu').menu('appendItem', {
            text: "帮助",
            iconCls: "icon-help",
            onclick: function () {
                $("#modalwindow").html("<iframe width='100%' height='99%' id='modalwindow'  frameborder='0' src='/Admin/Page/PageHelp?formid=" + formid_config + "'></iframe>");
                $("#modalwindow").window({ title: '帮助文档', maximized: true, width: SetGridWidthSub(100), height: SetGridHeightSub(50), iconCls: 'mdi mdi-help' }).window('open');
            }
        });

        function ListTable(x) {//数据区表格
            var columnsArr = [], frozenColumnsArr = [],codeDictionary = {}, pageDictionary = {};
            //性能优化数据字典相关数据查询
            var isDictionary = false;
            for (var n = 0; n < formContent.tableData[x].fieldData.length; n++) {
                if (formContent.tableData[x].fieldData[n].dataSource != undefined && parseFloat(formContent.tableData[x].fieldData[n].dataSource).toString() == "NaN" && formContent.tableData[x].fieldData[n].dataSource != "" && formContent.tableData[x].fieldData[n].editControl != "text" && formContent.tableData[x].fieldData[n].editControl != "logic") {
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
                }

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
                    columnsJson.hidden = (formContent.tableData[x].fieldData[n].isMainPageShow == "H");
                    columnsJson.permission = "show";
                }
                else {
                    columnsJson.permission = "show";
                    columnsJson.hidden = true;
                }
                columnsJson.sortable = (formContent.tableData[x].tableSortOrder == "none" ? false : true);

                if (formContent.tableData[x].fieldData[n].style != undefined && formContent.tableData[x].fieldData[n].style != "") {
                    columnsJson.style = formContent.tableData[x].fieldData[n].style;
                    columnsJson.formatter = function (value, row, index) {
                        if (row.allstylelist == "1") {
                            return value;
                        }
                        else {
                            return eval(this.style)(value, row, index, this.field);
                        }
                    }
                }
                if (formContent.tableData[x].fieldData[n].dataSource != undefined && formContent.tableData[x].fieldData[n].dataSource != "" && formContent.tableData[x].fieldData[n].editControl != "text" && formContent.tableData[x].fieldData[n].editControl != "logic") {
                    //性能优化数据字典相关数据从缓存中查询
                    SetDictionary(pageDictionary[formContent.tableData[x].fieldData[n].field]||[]);
                    function SetDictionary(tableData) {
                        var tableDictionary = {};
                        for (var i = 0; i < tableData.length; i++) {
                            tableDictionary[tableData[i].id] = tableData[i].text;
                        }
                        columnsJson.formatter = function (value, row, index) {
                            if (row.allstylelist == "1") {
                                return value;
                            }
                            else if ((value || value==0) && tableDictionary[value]) {   //解决为0时，条件不满足
                                return tableDictionary[value];
                            }
                            else if (value && value.toString().indexOf(",") != -1) {
                                var moreValue = [];
                                var moreArray = value.split(",");
                                for (var n = 0; n < moreArray.length; n++) {
                                    if (tableDictionary[moreArray[n]]) {
                                        moreValue.push(tableDictionary[moreArray[n]]);
                                    }
                                    else {
                                        moreValue.push(moreArray[n]);
                                    }
                                }
                                return moreValue.join(",");
                            }
                            else {
                                return value;
                            }
                        }
                        if (x==0 && $("#General_" + formContent.tableData[x].fieldData[n].field).length > 0) {
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

                if (frozenColumnsArr.length == 0 || formContent.tableData[x].fieldData[n].isFrozen) {
                    frozenColumnsArr.push(columnsJson);
                }
                else {
                    columnsArr.push(columnsJson);
                }

                if (frozenColumnsArr.length == 1 && (formContent.tableData[x].isMultipleSelected || common.getUrlParam("pagelist") == "selectOpen" || common.getUrlParam("leftlist") == "listOpen")) {
                    frozenColumnsArr[frozenColumnsArr.length - 1].hidden = false;
                    frozenColumnsArr[frozenColumnsArr.length - 1].checkbox = true;
                }

                if (frozenColumnsArr.length == 1 && dataEditButton[x] != "") {
                    frozenColumnsArr.push({
                        field: 'allStyleList', title: Lang.Operating, width: dataEditButton[x].match(/gridButton/ig).length * 40 + 10, sortable: false, align: 'center',
                        formatter: function (value, row, index) {
                            if (row.allstylelist == "1") {
                                return;
                            }
                            else if (row) {
                                return dataEditButton[x].replace(/gridButton\('rowindex'/gi, "gridButton(" + index + "");
                            } else {
                                return value;
                            }
                        }
                    });
                }
            }
            if (x > 0) { //添加从表选项卡
                $("#tabs").tabs('add', {
                    fit: true,
                    title: formContent.tableData[x].pageTitle,
                    selected: (x == 1 ? true : false),
                    content: '<div style="padding:3px"><table id="List' + x + '"></table></div>'
                });
            }
            var pageListTemp = [10, 20, 30, 40, 50, 100, 200, 500];
            for (var i = 0; $.inArray(parseInt(formContent.tableData[x].pageNum), pageListTemp) == -1 && i < pageListTemp.length; i++) {
                if (parseInt(formContent.tableData[x].pageNum) < parseInt(pageListTemp[i])) {
                    pageListTemp.splice(i, 0, formContent.tableData[x].pageNum);
                    break;
                }
            }
            if ($.inArray(parseInt(formContent.tableData[x].pageNum), pageListTemp) == -1) pageListTemp.push(formContent.tableData[x].pageNum);
            $('#List' + x).datagrid({
                loader: function (param, success, error) {
                    var tid = common.getUrlParam("id");
                    if (formContent.tableData[0].tableSql.indexOf("@t_id")!=-1 && (tid == '' || tid == null)) {
                        tid = window.parent.$('#List0').datagrid("getData").rows[window.parent.click_main_id].t_id;
                    }
                    var sql = '';
                    if (param.sql) {
                        sql = common.Encrypt(common.Decrypt(param.sql).replace(/@t_id/g, "'" + tid + "'"));
                    } else {
                        sql = common.Encrypt(formContent.tableData[x].tableSql.replace(/@t_id/g, "'" + tid + "'"));
                    }
                    var queryStr = "1=1";
                    if (param.queryStr) queryStr = param.queryStr;
                    if (x == 0 && queryStr == "1=1" && common.getUrlParam("urlParam") != null) queryStr = common.Decrypt(common.getUrlParam("urlParam"));
                    if (common.Decrypt(sql) == "" || (x != 0 && queryStr == "1=1")) return;//由主表去触发从表加载数据
                    apiRoute.selectEasyuiTable(dataSourceCode, sql, {
                        "queryStr": queryStr,
                        "footer": (formContent.tableData[x].footerSql ? common.Encrypt(formContent.tableData[x].footerSql) : ""),
                        "pager": param
                    }, function (tableData) {
                        success(tableData);
                    });
                },
                width: (x == 0 ? $('.pageList_layout').layout('panel', 'center')[0].clientWidth - 8 : $('.pageList_layout').layout('panel', 'center')[0].clientWidth - 6 - 8),
                methord: 'post',
                height: ((common.getUrlParam("pagelist") == "selectOpen" || common.getUrlParam("leftlist") == "listOpen") ? SetGridHeightSub(4 + $("#searchList").height()) : ((parseInt($("#TemplateID").val()) <= 2) ? (SetGridHeightSub($('.pageList_layout').layout('panel', 'north')[0].clientHeight + 3) - ($("#searchList").is(":visible") ? $("#searchList").height() : 0)) : (x == 0 ? (SetGridHeightSub($('.pageList_layout').layout('panel', 'north')[0].clientHeight + 3) - ($("#searchList").is(":visible") ? $("#searchList").height() : 0)) / 2 + 20 : (SetGridHeightSub($('.pageList_layout').layout('panel', 'north')[0].clientHeight+3) - ($("#searchList").is(":visible") ? $("#searchList").height() : 0)) / 2 - 20 - 35))),
                fitColumns: (formContent.tableData[x].isFitColumns != null ? formContent.tableData[x].isFitColumns : true),
                sortName: formContent.tableData[x].tableSortName,
                sortOrder: formContent.tableData[x].tableSortOrder,
                idField: formContent.tableData[x].tableIdField,
                pageSize: formContent.tableData[x].pageNum,
                pageList: pageListTemp,
                pagination: formContent.tableData[x].isPagination,  //是否分页
                striped: true, //奇偶行是否区分
                multiSort: false, //多列排序
                showFooter: (formContent.tableData[x].footerSql && formContent.tableData[x].footerSql!=""), //是否显示页脚
                checkOnSelect: true,//!formContent.tableData[x].isMultipleSelected,如果设置为 true，当用户点击某一行时，则会选中/取消选中复选框。如果设置为 false 时，只有当用户点击了复选框时，才会选中/取消选中复选框。
                selectOnCheck: true,//如果设置为 true，点击复选框将会选中该行。如果设置为 false，选中该行将不会选中复选框
                singleSelect: (!formContent.tableData[x].isMultipleSelected),//单选模式
                rownumbers: formContent.tableData[x].isRownumber,//行号
                frozenColumns: [frozenColumnsArr],//设置冻结列
                columns: [columnsArr],
                onLoadSuccess: function (gridData) {
                    if (formContent.tableData[x].isMultipleSelected) {//多选时支持框选
                        var containObj = $('#List' + x).siblings('.datagrid-view2').find('.datagrid-body');
                        var targetObj = containObj.find('tr');
                        $.DragToSelect(containObj, targetObj, 'moveSelected1_'+x);
                        //if ($('#List' + x).siblings('.datagrid-view1').length != 0) {
                        //    var containObj = $('#List' + x).siblings('.datagrid-view1').find('.datagrid-body');
                        //    var targetObj = containObj.find('tr');
                        //    $.DragToSelect(containObj, targetObj,'moveSelected2_'+x);
                        //}
                    }

                    if (x == 0 && gridData.rows.length >= 0 && parseInt($("#TemplateID").val()) > 1) {
                        if (click_main_id >= gridData.rows.length) click_main_id = 0;
                        for (var m = 1; m < formContent.tableData.length; m++) {
                            $('#List' + m).datagrid('clearSelections');
                            var queryStr = "1=1";
                            if (gridData.rows.length == 0 && formContent.tableData[m].mainSubLine.length!=0) {
                                queryStr = "1<>1";
                            }
                            else if (formContent.tableData[m].mainSubLine.length == 0) {
                                queryStr = "1<>0";
                            }
                            else {
                                for (n = 0; n < formContent.tableData[m].mainSubLine.length; n++) {
                                    queryStr += " and " + formContent.tableData[m].mainSubLine[n].subGridInput + "='" + gridData.rows[click_main_id][formContent.tableData[m].mainSubLine[n].mainGridOutput] + "'";
                                }
                            }
                            $('#List' + m).datagrid("load", {
                                sql: common.Encrypt(formContent.tableData[m].tableSql),
                                queryStr: queryStr
                            });
                        }
                    }
                    if (common.getUrlParam("pagelist") == "selectOpen") {
                        for (var i = 0; i < openSelectData.length; i++) {
                            for (var j = 0; j < gridData.rows.length; j++) {
                                if (openSelectData[i] == gridData.rows[j].id) {
                                    $('#List' + x).datagrid('checkRow', j);
                                    break;
                                }
                            }
                        }
                    }
                    $("#btnRefresh").attr("disabled", false).css("pointer-events", "auto");
                },
                onClickRow: function (index, row) {
                    if (x == 0 && parseInt($("#TemplateID").val()) > 1) {
                        if (row.is_allow_view == "0") return;
                        //if (row.is_allow_add) $("#is_allow_add").val(row.is_allow_add);
                        click_main_id = index;
                        for (var m = 1; m < formContent.tableData.length; m++) {
                            $('#List' + m).datagrid('clearSelections');
                            var queryStr = "1<>0";
                            for (n = 0; n < formContent.tableData[m].mainSubLine.length; n++) {
                                queryStr += " and " + formContent.tableData[m].mainSubLine[n].subGridInput + "='" + row[formContent.tableData[m].mainSubLine[n].mainGridOutput] + "'";
                            };
                            $('#List' + m).datagrid("load", {
                                sql: common.Encrypt(formContent.tableData[m].tableSql),
                                queryStr: queryStr
                            });
                        };
                    }else {
                        return;
                    }
                },
                onDblClickRow: function (rowIndex, rowData) {
                    $('#List' + x).datagrid('clearSelections');
                    $('#List' + x).datagrid('selectRow', rowIndex);
                    ButtonRun(x, -1, "Detail", btnList.Detail.name, btnList.Detail.icon, rowIndex)
                },
                onHeaderContextMenu: function (e, field) {
                    e.preventDefault();
                    $(this).datagrid('columnMenu').menu('show', {
                        left: e.pageX,
                        top: e.pageY
                    });
                },
                onRowContextMenu: function (e, rowIndex, rowData) { //右键时触发事件
                    if ($('#rightMenu' + x).children('div').length > 1){
                        e.preventDefault(); //阻止浏览器捕获右键事件
                        $(this).datagrid("clearSelections"); //取消所有选中项
                        $(this).datagrid("selectRow", rowIndex); //根据索引选中该行
                        if (rowData != null) {
                            $('#rightMenu' + x).menu('show', {
                                //显示右键菜单
                                left: e.pageX,//在鼠标点击处显示菜单
                                top: e.pageY
                            });
                        };
                        e.preventDefault();  //阻止浏览器自带的右键菜单弹出
                    }
                }
            });
            if (x != 0) {//拖拽从表，改变从表的高度
                var allTableHeight = $(window).height() - $('.panel.layout-panel.layout-panel-north.panel-htop').height() - ($("#searchList").is(":visible") ? $("#searchList").height() : 0);
                $('.easyui-resizable').resizable({
                    handles: "n", //北方出现拖拽线条
                    edge: 20,     //默认5，边框边缘触发大小
                    onResize: function () {
                        for (var m = 1; m < formContent.tableData.length; m++) {
                            $('#List' + m).datagrid('resize', {
                                height: $('.easyui-resizable').height() - 3 * 2 - 1 * 2 - 34 + 'px',
                            });
                        }
                        $('.easyui-resizable').find('.tabs-panels').css('height', $('.easyui-resizable').height() - 34 + 'px').find('.panel-body').css('height', $('.easyui-resizable').height() - 34 + 'px');
                    },
                    onStopResize: function () {
                        $('.easyui-tabs.easyui-resizable').css({ 'top': '0px' });
                        $('#List0').datagrid('resize', {
                            height: allTableHeight - $('.easyui-resizable').height() + 'px',
                        });
                        for (var m = 1; m < formContent.tableData.length; m++) {
                            $('#List'+m).datagrid('resize', {
                                height: $('.easyui-resizable').height() - 3 * 2 - 1 * 2 - 34 + 'px',
                            });
                        }
                    }
                });
            };
        }
    }
    //Right click on the header of DataGrid to display context menu
    (function ($) {
        function buildMenu(target){
            var state = $(target).data('datagrid');
            if (!state.columnMenu) {
                state.columnMenu = $('<div></div>').appendTo('body');
                state.columnMenu.menu({
                    onClick: function (item) {
                        if (item.iconCls == 'tree-checkbox1') {
                            $(target).datagrid('hideColumn', item.name);
                            $(this).menu('setIcon', {
                                target: item.target,
                                iconCls: 'tree-checkbox0'
                            });
                        } else {
                            $(target).datagrid('showColumn', item.name);
                            $(this).menu('setIcon', {
                                target: item.target,
                                iconCls: 'tree-checkbox1'
                            });
                        }
                    }
                })
                var fields = $(target).datagrid('getColumnFields', true).concat($(target).datagrid('getColumnFields', false));
                for (var i = 0; i < fields.length; i++) {
                    var field = fields[i];
                    var col = $(target).datagrid('getColumnOption', field);
                    if (col.permission == "none") continue;
                    state.columnMenu.menu('appendItem', {
                        text: col.title,
                        name: field,
                        iconCls: (col.hidden ? 'tree-checkbox0':'tree-checkbox1')
                    });
                }
            }
            return state.columnMenu;
        }
        $.extend($.fn.datagrid.methods, {
            columnMenu: function (jq) {
                return buildMenu(jq[0]);
            }
        });
    })(jQuery);

    $("#btnRefresh").click(function () {
        //window.parent.$("#tab_menu-tabrefresh").trigger("click");
        $("#btnRefresh").attr("disabled", true).css("pointer-events", "none");
        search();
        /*
        for (var i = 0; i < formContent.tableData.length; i++) {
            $("#List" + i).datagrid("load", {
                sql: common.Encrypt(formContent.tableData[i].tableSql)
            });
            $('#List' + i).datagrid();
        }
        */
    });

    $("#btnPrint").click(function () {
        var tableid = 0;
        var rows = $('#List' + tableid).datagrid('getSelections');  //取得所有选中行数据，返回元素记录的数组数据。
        if (rows.length == 0) {
            $.messageBoxBottom(0, CommonLang.PleaseSelectTheOperatingRecord);
            return;
        }
        else {
            var ids = [];
            for (var i = 0; i < rows.length; i++) {
                ids.push(rows[i][$('#List' + tableid).datagrid('getColumnFields', true)[0]]);
            }
            $("#modalwindow").html("<iframe width='100%' height='99%'  id='modalwindow' frameborder='0' src='" + $("#ReportUrl").val() + "Report/Index?fileName=" + formContent.printFileName + "&tIds=" + ids.join(",") + "'></iframe>");
            $("#modalwindow").window({ title: "打印浏览", maximized: false, width: 800, height: SetGridHeightSub(50), iconCls: "mdi mdi-printer" }).window('open');
        }
    });

    //$("#btnSearch").click(function () { AdvSearch(0, '高级查询') });
    function GeneralQuery(tableId) {
        var inputhtml1 = "", inputhtml2 = "", GeneralId = [];
        for (var i = 0; i < formContent.tableData[tableId].fieldData.length; i++) {
            var thisFieldInfo = formContent.tableData[tableId].fieldData[i];
            if (!thisFieldInfo.isGeneralQuery) continue;
            if (thisFieldInfo.editControl == "logic" && thisFieldInfo.title.length <= 4) {
                inputhtml1 += "<p>";
                inputhtml1 += "<span class='generalquery'>" + thisFieldInfo.title + "：</span>";
                inputhtml1 += "<input id=\"General_" + thisFieldInfo.field + "\" data-options=\"panelHeight:'auto',width: 150,data: [{ id: -1, text: '==全部==',selected:true }, { id: 1, text: '是' }, { id: 0, text: '否' }],valueField: 'id',textField: 'text'\" class=\"easyui-combobox\">";
                inputhtml1 += "</p>";
            }
            else if (thisFieldInfo.editControl == "logic") {
                inputhtml2 += "<p>";
                inputhtml2 += "<span class='generalquery'>" + thisFieldInfo.title + "：</span>";
                inputhtml2 += "<input id=\"General_" + thisFieldInfo.field + "\" data-options=\"panelHeight:'auto',width: 150,data: [{ id: -1, text: '==全部==',selected:true }, { id: 1, text: '是' }, { id: 0, text: '否' }],valueField: 'id',textField: 'text'\" class=\"easyui-combobox\">";
                inputhtml2 += "</p>";
            }
            else if (thisFieldInfo.editControl == "datebox") {
                inputhtml2 += "<p>";
                inputhtml2 += "<span class='generalquery'>" + thisFieldInfo.title + "：</span>";
                inputhtml2 += "<input id=\"General_" + thisFieldInfo.field + "_1\" class=\"Wdate\" style=\"width:120px;border:1px #ccc solid;border-radius:4px;height:26px\" readonly prompt=\"请选择起始日期\" onclick=\"WdatePicker()\"> — ";
                inputhtml2 += "<input id=\"General_" + thisFieldInfo.field + "_2\" class=\"Wdate\" style=\"width:120px;border:1px #ccc solid;border-radius:4px;height:26px\" readonly prompt=\"请选择终止日期\" onclick=\"WdatePicker()\">";
                inputhtml2 += "</p>";
            }
            else if (thisFieldInfo.editControl == "datetime") {
                inputhtml2 += "<p>";
                inputhtml2 += "<span class='generalquery'>" + thisFieldInfo.title + "：</span>";
                inputhtml2 += "<input id=\"General_" + thisFieldInfo.field + "_1\" class=\"easyui-datetimebox\" prompt=\"请选择起始日期\" data-options=\"width: 150\"> — ";
                inputhtml2 += "<input id=\"General_" + thisFieldInfo.field + "_2\" class=\"easyui-datetimebox\" prompt=\"请选择终止日期\" data-options=\"width: 150\">";
                inputhtml2 += "</p>";
            }
            else if ((thisFieldInfo.editControl == "selected" || thisFieldInfo.editControl == "radio" || thisFieldInfo.editControl == "checkbox") && thisFieldInfo.title.length <= 4 && thisFieldInfo.dataSource) {
                inputhtml1 += "<p>";
                inputhtml1 += "<span class='generalquery'>" + thisFieldInfo.title + "：</span>";
                inputhtml1 += "<input id=\"General_" + thisFieldInfo.field + "\" data-options=\"panelHeight:'auto',panelMaxHeight: 250,width: 150\" class=\"easyui-combobox\">";
                inputhtml1 += "</p>";
            }
            else if ((thisFieldInfo.editControl == "selected" || thisFieldInfo.editControl == "radio" || thisFieldInfo.editControl == "checkbox") && thisFieldInfo.dataSource) {
                inputhtml2 += "<p>";
                inputhtml2 += "<span class='generalquery'>" + thisFieldInfo.title + "：</span>";
                inputhtml2 += "<input id=\"General_" + thisFieldInfo.field + "\" data-options=\"panelHeight:'auto',panelMaxHeight: 250,width: 150\" class=\"easyui-combobox\">";
                inputhtml2 += "</p>";
            }
            else if (thisFieldInfo.editControl == "number" && thisFieldInfo.title.length <= 4) {
                inputhtml1 += "<p>";
                inputhtml1 += "<span class='generalquery'>" + thisFieldInfo.title + "：</span>";
                inputhtml1 += "<input id=\"General_" + thisFieldInfo.field + "\" class=\"easyui-numberbox\" prompt=\"请输入" + thisFieldInfo.title + "\" data-options=\"width: 150\">";
                inputhtml1 += "</p>";
                GeneralId.push("General_" + thisFieldInfo.field);
            }
            else if (thisFieldInfo.editControl == "number") {
                inputhtml2 += "<p>";
                inputhtml2 += "<span class='generalquery'>" + thisFieldInfo.title + "：</span>";
                inputhtml2 += "<input id=\"General_" + thisFieldInfo.field + "\" class=\"easyui-numberbox\" prompt=\"请输入" + thisFieldInfo.title + "\" data-options=\"width: 150\">";
                inputhtml2 += "</p>";
                GeneralId.push("General_" + thisFieldInfo.field);
            }
            else if (thisFieldInfo.title.length <= 4) {
                inputhtml1 += "<p>";
                inputhtml1 += "<span class='generalquery'>" + thisFieldInfo.title + "：</span>";
                inputhtml1 += "<input id=\"General_" + thisFieldInfo.field + "\" class=\"easyui-textbox\" prompt=\"请输入" + thisFieldInfo.title + "\" data-options=\"width: 150\">";
                inputhtml1 += "</p>";
                GeneralId.push("General_" + thisFieldInfo.field);
            }
            else {
                inputhtml2 += "<p>";
                inputhtml2 += "<span class='generalquery'>" + thisFieldInfo.title + "：</span>";
                inputhtml2 += "<input id=\"General_" + thisFieldInfo.field + "\" class=\"easyui-textbox\" prompt=\"请输入" + thisFieldInfo.title + "\" data-options=\"width: 150\">";
                inputhtml2 += "</p>";
                GeneralId.push("General_" + thisFieldInfo.field);
            }
        }
        $("#searchList").append(inputhtml1 + inputhtml2);
        $.parser.parse($("#searchList").parent());
        if (inputhtml1=="" && inputhtml2=="") $("#btnSearch").hide();
        for (var i = 0; i < GeneralId.length; i++) {
            var t = $('#' + GeneralId[i]);
            t.textbox('textbox').bind('keydown', function (e) {
                if (e.keyCode == 13) {	// 当按下回车键时接受输入的值。
                    search();
                }
            });
        }
    }
    function AdvSearch(tableId, title) {
        //if (tableId == 0) $("#btnSearch").hide();
        $('.pageList_layout').layout('remove', 'east');
        $('.pageList_layout').layout('add', {
            region: 'east',
            width: 250,
            title: title,
            split: false,
            collapsible: false,
            content: "<div id=\"advanceQuery\" style=\"padding:5px;\"></div>",
            tools: [{
                iconCls: 'mdi mdi-magnify',
                handler: function () {
                    var queryStr = "1=1";
                    for (var i = 0; i < formContent.tableData[tableId].fieldData.length; i++) {
                        var thisFieldInfo = formContent.tableData[tableId].fieldData[i];
                        if (!thisFieldInfo.isAdvanceQuery) continue;
                        if (thisFieldInfo.editControl == "logic") {
                            if ($("#" + thisFieldInfo.field).val() == -1) continue;
                            queryStr += " AND " + thisFieldInfo.field + "='" + $("#" + thisFieldInfo.field).val() + "'";
                            if (thisFieldInfo.isMainPageShow != 'N') {
                                $('#List' + tableId).datagrid("getColumnOption", thisFieldInfo.field).title = thisFieldInfo.title + "&nbsp;<span class='icon-filter' title='" + ($("#" + thisFieldInfo.field).val() == 1) + "'>&nbsp;&nbsp;&nbsp;&nbsp;</span>";
                            }
                            continue;
                        }
                        else if (thisFieldInfo.editControl == "datebox" || thisFieldInfo.editControl == "datetime") {
                            if ($("#" + thisFieldInfo.field + "1").val() && $("#" + thisFieldInfo.field + "2").val()) {
                                if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "Oracle") {
                                    queryStr += " AND " + thisFieldInfo.field + ">=to_date('" + $("#" + thisFieldInfo.field + "1").val() + "','yyyy-mm-dd hh24:mi:ss') AND " + thisFieldInfo.field + "<=to_date('" + $("#" + thisFieldInfo.field + "2").val() + "','yyyy-mm-dd hh24:mi:ss')";
                                }
                                else {
                                    queryStr += " AND " + thisFieldInfo.field + ">='" + $("#" + thisFieldInfo.field + "1").val() + "' AND " + thisFieldInfo.field + "<='" + $("#" + thisFieldInfo.field + "2").val() + "'";
                                }
                                if (thisFieldInfo.isMainPageShow != 'N') {
                                    $('#List' + tableId).datagrid("getColumnOption", thisFieldInfo.field).title = thisFieldInfo.title + "&nbsp;<span class='icon-filter' title='从" + $("#" + thisFieldInfo.field + "1").val() + "到" + $("#" + thisFieldInfo.field + "2").val() + "'>&nbsp;&nbsp;&nbsp;&nbsp;</span>";
                                }
                            }
                            else if($("#" + thisFieldInfo.field + "1").val()) {
                                if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "Oracle") {
                                    queryStr += " AND " + thisFieldInfo.field + ">=to_date('" + $("#" + thisFieldInfo.field + "1").val() + "','yyyy-mm-dd hh24:mi:ss')";
                                }
                                else {
                                    queryStr += " AND " + thisFieldInfo.field + ">='" + $("#" + thisFieldInfo.field + "1").val() + "'";
                                }
                                if (thisFieldInfo.isMainPageShow != 'N') {
                                    $('#List' + tableId).datagrid("getColumnOption", thisFieldInfo.field).title = thisFieldInfo.title + "&nbsp;<span class='icon-filter' title='开始于" + $("#" + thisFieldInfo.field + "1").val() + "'>&nbsp;&nbsp;&nbsp;&nbsp;</span>";
                                }
                            }
                            else if ($("#" + thisFieldInfo.field + "2").val()) {
                                if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "Oracle") {
                                    queryStr += " AND " + thisFieldInfo.field + "<=to_date('" + $("#" + thisFieldInfo.field + "2").val() + "','yyyy-mm-dd hh24:mi:ss')";
                                }
                                else {
                                    queryStr += " AND " + thisFieldInfo.field + "<='" + $("#" + thisFieldInfo.field + "2").val() + "'";
                                }
                                if (thisFieldInfo.isMainPageShow != 'N') {
                                    $('#List' + tableId).datagrid("getColumnOption", thisFieldInfo.field).title = thisFieldInfo.title + "&nbsp;<span class='icon-filter' title='结果于" + $("#" + thisFieldInfo.field + "2").val() + "'>&nbsp;&nbsp;&nbsp;&nbsp;</span>";
                                }
                            }
                            continue;
                        }

                        if ($("#" + thisFieldInfo.field).val()) {
                            if ($("#Select_" + thisFieldInfo.field).combobox("getValue") == "StartsWith") {
                                queryStr += " AND lower(" + thisFieldInfo.field + ") like lower('" + $("#" + thisFieldInfo.field).val() + "%')";
                            }
                            else if ($("#Select_" + thisFieldInfo.field).combobox("getValue") == "Contains") {
                                queryStr += " AND lower(" + thisFieldInfo.field + ") like lower('%" + $("#" + thisFieldInfo.field).val() + "%')";
                            }
                            else if ($("#Select_" + thisFieldInfo.field).combobox("getValue") == "EndsWith") {
                                queryStr += " AND lower(" + thisFieldInfo.field + ") like (lower'%" + $("#" + thisFieldInfo.field).val() + "')";
                            }
                            else if ($("#Select_" + thisFieldInfo.field).combobox("getValue") == "NoContains") {
                                queryStr += " AND lower(" + thisFieldInfo.field + ") not like lower('%" + $("#" + thisFieldInfo.field).val() + "%')";
                            }
                            else if ($("#Select_" + thisFieldInfo.field).combobox("getValue") == "Equals") {
                                if ($("#" + thisFieldInfo.field).val() == "*") continue;
                                queryStr += " AND " + thisFieldInfo.field + " = '" + $("#" + thisFieldInfo.field).val() + "'";
                            }
                            else if ($("#Select_" + thisFieldInfo.field).combobox("getValue") == "NoEquals") {
                                if ($("#" + thisFieldInfo.field).val() == "*") continue;
                                queryStr += " AND " + thisFieldInfo.field + " <> '" + $("#" + thisFieldInfo.field).val() + "'";
                            }
                            if (thisFieldInfo.isMainPageShow != 'N') {
                                $('#List' + tableId).datagrid("getColumnOption", thisFieldInfo.field).title = thisFieldInfo.title + "&nbsp;<span class='icon-filter' title='" + $("#Select_" + thisFieldInfo.field).combobox("getText") + $("#" + thisFieldInfo.field).val() + "'>&nbsp;&nbsp;&nbsp;&nbsp;</span>";
                            }
                        }
                        else if (thisFieldInfo.isMainPageShow != 'N') {
                            $('#List' + tableId).datagrid("getColumnOption", thisFieldInfo.field).title = thisFieldInfo.title;
                        }
                    }
                    $("#List" + tableId).datagrid("load", {
                        sql: common.Encrypt(formContent.tableData[tableId].tableSql),
                        queryStr: queryStr
                    });
                    if (tableId == 0) click_main_id = 0;
                    $('#List' + tableId).datagrid('clearSelections');
                    $('#List' + tableId).datagrid();
                }
            }, {
                iconCls: 'mdi mdi-close',
                handler: function () {
                    $('.pageList_layout').layout('remove', 'east');
                    //$("#btnSearch").show();
                }
            }]
        });
        for (var i = 0; i < formContent.tableData[tableId].fieldData.length; i++) {
            var thisFieldInfo = formContent.tableData[tableId].fieldData[i], inputhtml = "";
            if (!thisFieldInfo.isAdvanceQuery) continue;
            if (thisFieldInfo.editControl == "logic") {
                inputhtml += "<div style=\"float:left\"><label for=\"" + thisFieldInfo.field + "\">" + thisFieldInfo.title + "</label>：</div>";
                inputhtml += "<div style=\"float:right;padding-right:30px;\"><input class=\"easyui-combobox\" id=\"" + thisFieldInfo.field + "\"></div><br><br>";
            }
            else if (thisFieldInfo.editControl == "datebox") {
                inputhtml += "<div style=\"float:left\"><label for=\"" + thisFieldInfo.field + "\">" + thisFieldInfo.title + "</label>：</div><br>";
                inputhtml += "从<input class=\"Wdate\" readonly style=\"width:88px\" id=\"" + thisFieldInfo.field + "1\" type=\"text\" onclick=\"WdatePicker()\" />到";
                inputhtml += "<input class=\"Wdate\" readonly style=\"width:88px\" id=\"" + thisFieldInfo.field + "2\" type=\"text\" onclick=\"WdatePicker()\" /><br><br>";
            }
            else if (thisFieldInfo.editControl == "datetime") {
                inputhtml += "<div style=\"float:left\"><label for=\"" + thisFieldInfo.field + "\">" + thisFieldInfo.title + "</label>：</div><br>";
                inputhtml += "从<input class=\"Wdate\" readonly style=\"width:200px\" id=\"" + thisFieldInfo.field + "1\" type=\"text\" onclick=\"WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})\" /><br><br>";
                inputhtml += "到<input class=\"Wdate\" readonly style=\"width:200px\" id=\"" + thisFieldInfo.field + "2\" type=\"text\" onclick=\"WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})\" /><br><br>";
            }
            else {
                inputhtml += "<div style=\"float:left\"><label for=\"" + thisFieldInfo.field + "\">" + thisFieldInfo.title + "</label>：</div>";
                inputhtml += "<div style=\"float:right;padding-right:30px;\"><input class=\"easyui-combobox\" id=\"Select_" + thisFieldInfo.field + "\"></div>";
                inputhtml += "<input class=\"easyui-textbox\" style=\"width:200px\" type=\"text\" id=\"" + thisFieldInfo.field + "\"><br><br>";
            }
            $("#advanceQuery").append(inputhtml);
            var SelectData = [{ id: "Contains", text: "包含" }, { id: "StartsWith", text: "开头包含" }, { id: "EndsWith", text: "结尾包含" }, { id: "NoContains", text: "不包含" }, { id: "Equals", text: "相等" }, { id: "NoEquals", text: "不相等" }];
            $("#Select_" + thisFieldInfo.field).combobox({
                editable: false,
                width: 80,
                data: SelectData,
                panelHeight: 'auto',
                valueField: 'id',
                textField: 'text'
            });
            if (thisFieldInfo.editControl == "logic") {
                $("#" + thisFieldInfo.field).combobox({
                    data: [{ id: -1, text: '无' }, { id: 1, text: '是' }, { id: 0, text: '否' }],
                    panelHeight: 'auto',
                    width: 50,
                    valueField: 'id',
                    textField: 'text'
                });
                $("#" + thisFieldInfo.field).combobox("setValue", -1);
            }
            //以下是下拉控件
            if ((thisFieldInfo.editControl == "selected" || thisFieldInfo.editControl == "radio" || thisFieldInfo.editControl == "checkbox") && thisFieldInfo.dataSource) {
                $("#" + thisFieldInfo.field).combobox({
                    loader: function (param, success, error) {
                        apiRoute.pageDictionary({ "dictionaryorsql": common.Encrypt(thisFieldInfo.dataSource) }, function (tableDataSet) {
                            var tableData = tableDataSet.dictionaryorsql;
                            tableData.unshift({ id: "*", text: "==全部==" });
                            success(tableData);
                        });
                    },
                    panelHeight: 'auto',
                    panelMaxHeight: 300,
                    valueField: 'id',
                    textField: 'text',
                    onLoadSuccess: function (data) {
                        $(this).combobox("setValue", "*");
                    }
                });
                $("#Select_" + thisFieldInfo.field).combobox("setValue", "Equals");
            }
            else if (thisFieldInfo.editControl == "number") {
                $("#Select_" + thisFieldInfo.field).combobox({
                    data: [{ id: "Equals", text: "相等" }],
                    panelHeight: 'auto',
                    valueField: 'id',
                    textField: 'text',
                    onLoadSuccess: function (data) {
                        $(this).combobox("setValue", "Equals");
                    }
                });
            }
            else {
                $("#Select_" + thisFieldInfo.field).combobox("setValue", "Contains");
            }
        }
        $('#advanceQuery').css({ "height": SetGridHeightSub(67), "overflow-y": "auto" });
    }

    $('.pageList_layout').layout('panel', 'center').panel({
        onResize: function (width, height) {
            try {
                $('#List0').datagrid('resize', {

                }).datagrid('resize', {
                    width: width - 8,
                    height: ((common.getUrlParam("pagelist") == "selectOpen" || common.getUrlParam("leftlist") == "listOpen") ? SetGridHeightSub(4 + $("#searchList").height()) : ((parseInt($("#TemplateID").val()) <= 2) ? (SetGridHeightSub($('.pageList_layout').layout('panel', 'north')[0].clientHeight + 3) - ($("#searchList").is(":visible") ? $("#searchList").height() : 0)) : (SetGridHeightSub($('.pageList_layout').layout('panel', 'north')[0].clientHeight+3) - ($("#searchList").is(":visible") ? $("#searchList").height() : 0)) / 2 + 20))
                });

                if (parseInt($("#TemplateID").val()) > 2) {
                    $("#tabs").tabs({
                        width: width - 8,
                        height: (height - ($("#searchList").is(":visible") ? $("#searchList").height() : 0) - 3) / 2 - 20
                    });
                    for (var i = 1; i <= $('#tabs').tabs('tabs').length; i++) {
                        $('#List' + i).datagrid('resize', {

                        }).datagrid('resize', {
                            width: width - 6 - 8,
                            height: (height - ($("#searchList").is(":visible") ? $("#searchList").height() : 0) - 3) / 2 - 20 - 35//(SetGridHeightSub($('.pageList_layout').layout('panel', 'north')[0].clientHeight+3) - ($("#searchList").is(":visible") ? $("#searchList").height() : 0)) / 2 - 20 - 35
                        });
                    }
                }
                $('#treeleft').css({ "height": SetGridHeightSub(100), "overflow-y": "auto" });
            }
            catch (e) { }
        }
    });
});
function gridButton(rowIndex, tableid, buttonid, btJs, btText, btIcon) {
    $('#List' + tableid).datagrid('clearSelections');
    if (formContent.tableData[tableid].isMultipleSelected != true || btJs == "Delete") {
        $('#List' + tableid).datagrid('selectRow', rowIndex);
    }
    ButtonRun(tableid, buttonid, btJs, btText, btIcon, rowIndex);
}
function ButtonRun(tableid, buttonid, btJs, btText, btIcon, rowIndex) {
    var pageUrl;
    if (tableid >= 0 && buttonid >= 0) {
        btnText = btText;       //用户操作行为日记记录具体操和按钮
        btnSql = formContent.tableData[tableid].buttonData[buttonid].btnSql;
        pageUrl = formContent.tableData[tableid].buttonData[buttonid].btnUrl;
    }
	var rowsMainTable = $('#List0').datagrid('getSelections');
    var rows = $('#List' + tableid).datagrid('getSelections');  //取得所有选中行数据，返回元素记录的数组数据。//getChecked取得checkbox选择行的数据，返回元素记录的数组数据

    if (btJs == "Table") {
        rows = rowsMainTable;
    }

	if (rowsMainTable.length != 1 && tableid > 0 && btJs == "Create"){ //从表添加按钮
		$.messageBoxBottom(0, CommonLang.PleaseSelectOneOperatingMainRecord);
		return;
	}
    else if (rowIndex == -1 && btJs != "Create" && btJs != "Config" && btJs != "Export" && btJs != "Import") {//不是点击添加操作按钮
        if (rows.length == 0) {
            $.messageBoxBottom(0, CommonLang.PleaseSelectTheOperatingRecord);
            return;
        }
        else if (rows.length != 1 && btJs != "Delete" && btJs !="Custom") {//删除、自定义按钮不限多选
            $.messageBoxBottom(0, CommonLang.PleaseSelectOneOperatingRecord);
            return;
        }
    }

    if (pageUrl) {//配置页面有设地址
        //var row = $('#List' + tableid).datagrid('getSelected');//取得第一个选中行数据，如果没有选中行，则返回 null，否则返回记录
        var ListID = "";
        if (btJs != "Create") {
            ListID = "&id="+ rows[0][$('#List' + tableid).datagrid('getColumnFields', true)[0]];
        }
        if (pageUrl.indexOf("?") != -1) {
            pageUrl = pageUrl + "&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId")+"&btCode=" + btJs + ListID;
        }
        else {
            pageUrl = pageUrl + "?AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId")+"&btCode=" + btJs + ListID;
        }
    }
    else if (btJs == "Detail") {//查询明细
        pageUrl = "/Admin/Page/PageDetail?Load=Complete&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId")+"&btCode=Detail&formid=" + formid_config + "&tableid=" + tableid;
    }
    else if (btJs == "OpenFlow" || btJs == "ViewFlow") {//打开工作流
        $.ajaxSettings.async = false;
        $.post("/Admin/WorkFlow/EngineUrl", { workflow_task_id: rows[0].workflow_task_id, document_id: rows[0].document_id, workflow_id: rows[0].workflow_id, stepid: rows[0].wf_step_id }, function (result) {
            pageUrl = result;
            if (btJs == "ViewFlow") pageUrl = pageUrl + "&ViewFlow=true";
        });
        $.ajaxSettings.async = true;
    }
    else if (btJs == "ViewReport") {
        var frxPath = rows[0];
        var sourceName = rows[0].frxdatasourcename;
        var frxPath = rows[0].filepath;
        var tid = rows[0].t_id;
        pageUrl = $("#ReportUrl").val()+"Report/ViewReport?sourceName=" + sourceName + "&tid=" + tid + "&frxPath=" + frxPath + "&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId") + "&btCode=Detail&formid=" + formid_config + "&tableid=" + tableid;
    }
    else if (!pageUrl && btJs == "Table") {
        pageUrl = "/Admin/Page/PageFormTable?Load=Complete&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId")+ "&btCode=" + btJs + "&formid=" + formid_config + "&tableid=" + tableid + "&buttonid=" + buttonid;
    }
    else if (!pageUrl) {
        pageUrl = "/Admin/Page/PageForm?Load=Complete&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId")+ "&btCode=" + btJs + "&formid=" + formid_config + "&tableid=" + tableid;
    }
    if (btJs == "Export") {
        $.messageLoad();
        var parameters = [];
        for (var i = 0; i < formContent.tableData[tableid].fieldData.length; i++) {
            var thisFieldInfo = formContent.tableData[tableid].fieldData[i];
            if (!thisFieldInfo.isGeneralQuery) continue;            
            if (thisFieldInfo.editControl == "datebox" || thisFieldInfo.editControl == "datetime") {
                parameters.push({ "field": thisFieldInfo.field + "_1", "text": thisFieldInfo.title, "editControl": thisFieldInfo.editControl, "value": $("#General_" + thisFieldInfo.field + "_1").val() });
                parameters.push({ "field": thisFieldInfo.field + "_2", "text": thisFieldInfo.title, "editControl": thisFieldInfo.editControl, "value": $("#General_" + thisFieldInfo.field + "_2").val() });
            }
            else {
                parameters.push({ "field": thisFieldInfo.field, "text": thisFieldInfo.title, "editControl": thisFieldInfo.editControl, "value": $("#General_" + thisFieldInfo.field).val() });
            }
        }
        common.postGateway("platform/excel/export", {
            "dataSource": dataSourceCode,
            "strSql": common.Encrypt(btnSql),
            "rows": rows,
            "parameters": parameters
        }, common.getToken(), function (result) {
            $.messageBoxCloseLoading();
            if (result.isSucceed) {
                /*
                var $eleForm = $("<form target=\"down-file-iframe\" method='get'></form>");
                $eleForm.attr("action", result.json);
                $(document.body).append($eleForm);
                $eleForm.submit();//提交表单，实现下载
                */
                window.location = common.gatewayUrl() + "api/file/downfile?fileName=" + encodeURIComponent(".." + result.json);
            }
            else {
                $.messageBoxBottom(0, result.errmsg);
            }
        });
    }
    else if (btJs == "Import") {
        $("#excelsql").val(common.Encrypt(btnSql));
        $("#ExcelImport").dialog('open').dialog('center');
    }
    else if (btJs == "DownFile") {
        if (rows[0].is_allow_view == "0") {
            $.messageBoxBottom(0, "权限不足，无法查看！");
        }
        else {
            var doc_dir = "doc_dir";
            for (var file = 0; file < formContent.tableData[tableid].fieldData.length; file++) {
                if (formContent.tableData[tableid].fieldData[file].editControl == "upLoadFile") {
                    doc_dir = formContent.tableData[tableid].fieldData[file].field;
                    break;
                }
            }
            var $eleForm = $("<form target=\"down-file-iframe\" method='get'></form>");
            $eleForm.attr("action", "/" + rows[0][doc_dir]);
            $(document.body).append($eleForm);
            $eleForm.submit();//提交表单，实现下载
            //window.location = common.gatewayUrl() + "api/file/downfile?from=Web&fileName=" + rows[0][doc_dir];
        }
    }
    else if (btJs == "Interface") {
        var interfaceId = rows[0][formContent.tableData[tableid].tableIdField];
        $.messageBoxConfirm('你要确定执行所选记录接口吗？', ['确定', '取消'], function () {
            common.postGateway('platform/interface/RunInterface', {
                interfaceId: interfaceId
            }, common.getToken(), function (data) {
                $.messageBoxBottom(1, data.resMsg);
            });
        });
    }
    else if (btJs == "Custom" || btJs == "Config") {
        eval(btnSql);
    }
    else if (btJs == "Delete") {
        var sqlCount = btnSql.split(";").length;
        var listJson = [], listSql = [];
        if (rows.length == 0 && rowIndex != -1) {
            rows = $('#List' + tableid).datagrid('getSelections');
        }
        for (var row_m = 0; row_m < rows.length; row_m++) {
            if (rows[row_m].is_allow_delete == 0) {//不允许删除
                if (rows[row_m].disallow_delete_msg && rows[row_m].disallow_delete_msg != "") {
                    $.messageBoxBottom(0, rows[row_m].disallow_delete_msg);
                }
                else {
                    $.messageBoxBottom(0, "选择行条件不满足，无法删除！");
                }
                return;
            }
            for (var row_n = 0; row_n < sqlCount; row_n++) { //看有几个SQL
                listJson.push(rows[row_m]);
            }
            listSql.push(btnSql);
        }
        var runBtCount = rows.length;
        $.messageBoxConfirm('你要' + btText + '所选择的记录吗？', ['确定', '取消'], function () {
            apiRoute.runTaskEvent(dataSourceCode, listSql.join(";"), listJson, function (result) {

                var jsonLog = {
                    "tableId": parseInt(tableid) + 1,
                    "formid": formid_config,
                    "btnText": btText,
                    "json": JSON.stringify({
                        "dataSourceCode": dataSourceCode,
                        "btnSql": listSql.join(";"),
                        "formJson": listJson
                    })
                };
                common.postGateway("platform/menu/MenuLog", jsonLog, common.getToken());

                $("#List" + tableid).datagrid('clearSelections');
                $("#List" + tableid).datagrid('reload');
                if (result.resMsg && result.resMsg != "") {
                    $.messageBoxBottom(0, result.resMsg);
                }
                else if (result.errcode != 0 && result.errmsg) {
                    $.messageBoxBottom(0, result.errmsg);
                }
                else if (result.errcode == 0 && result.errmsg && result.errmsg != "" && result.errmsg.toUpperCase() != "OK") {
                    $.messageBoxBottom(1, result.errmsg);
                }
                else if (result.resCount && result.resCount < runBtCount) {
                    $.messageBoxBottom(0, btText + "操作实际只" + result.resCount + "条记录！");
                }
                else {
                    $.messageBoxBottom(1, btText + "操作" + runBtCount + "条记录成功！");
                }
                frameReturnByTreeLoad();
            });
        });
    }
    else {
        if (btJs == "Create") {
            if (tableid == 0 && $("#is_allow_add").val() == "0") {
                $.messageBoxBottom(0, "选择行不满足添加操作条件！");
                return;
            }
            else if(tableid > 0){
                if (rowsMainTable[0].is_allow_add == 0) {//不允许添加明细
                    if (rowsMainTable[0].disallow_add_msg && rowsMainTable[0].disallow_add_msg != "") {
                        $.messageBoxBottom(0, rowsMainTable[0].disallow_add_msg);
                    }
                    else {
                        $.messageBoxBottom(0, "选择行不满足添加明细条件！");
                    }
                    return;
                }
            }
        }
        else if (btJs == "Edit" || btJs == "Table") {
            if (rows.length == 0 && rowIndex != -1) {   //修复多选时rows为空
                rows = [$('#List' + tableid).datagrid('getRows')[rowIndex]];
            }
            for (var row_m = 0; row_m < rows.length; row_m++) {
                if (rows[row_m].is_allow_edit == 0) {//不允许修改
                    if (rows[row_m].disallow_edit_msg && rows[row_m].disallow_edit_msg != "") {
                        $.messageBoxBottom(0, rows[row_m].disallow_edit_msg);
                    }
                    else {
                        $.messageBoxBottom(0, "选择行不满足编辑操作条件！");
                    }
                    return;
                }
            }
        }
        $("#modalwindow").html("<iframe width='100%' height='99%'  id='modalwindow' frameborder='0' src='" + pageUrl + "'></iframe>");
        $("#modalwindow").window({ title: btText, maximized: true, width: SetGridWidthSub(100), height: SetGridHeightSub(50), iconCls: btIcon }).window('open');
    }
}

function ExcelImport() {
    $('#ExcelImport').dialog('close');
    var serviceName = "platform/excel/Import";
    var serviceJson = { "uploadFormId": "uploadForm", "dataSource": dataSourceCode };
    var serviceTitle = { title: "导入", msg: "正在导入Excel数据，请稍后……", "text": "数据保存中" };
    common.postFileGateway(serviceName, serviceJson, serviceTitle, common.getToken(), function (data) {

        var jsonLog = {
            "tableId": 1,
            "formid": formid_config,
            "btnText": btnText,
            "json": JSON.stringify({
                "dataSourceCode": dataSourceCode,
                "btnSql":btnSql,
                "formJson": "导入Excel数据:platform/excel/Import"
            })
        };
        common.postGateway("platform/menu/MenuLog", jsonLog, common.getToken());

        if (data.isSucceed == true) {
            $("#excelfile").val("");
            $("#excelsql").val("");
            $("#btnRefresh").click();
            $.messageBoxBottom(1, "数据导入成功！");
        }
        else {
            $.messageBoxBottom(0, data.errmsg);
        }
    });
}
function search() { //普通查询
    var tableId = 0;
    var queryStr = "1=1";
    for (var i = 0; i < formContent.tableData[tableId].fieldData.length; i++) {
        var thisFieldInfo = formContent.tableData[tableId].fieldData[i];
        if (!thisFieldInfo.isGeneralQuery) continue;
        if (thisFieldInfo.isMainPageShow != 'N') {
            $('#List' + tableId).datagrid("getColumnOption", thisFieldInfo.field).title = thisFieldInfo.title;
        }
        //防存储型跨站脚本攻击
        var valText = $("#General_" + thisFieldInfo.field).val();
        if (valText) {
            valText = valText.toString().replace(/'/ig, '');
        }
        if (thisFieldInfo.editControl == "logic") {
            if (valText != -1) {
                if (formContent.tableData[tableId].tableSql.indexOf("SelectMicroServices(") == 0) {
                    queryStr += " AND " + thisFieldInfo.field + "=" + (valText=='1');
                }
                else {
                    queryStr += " AND " + thisFieldInfo.field + "='" + valText + "'";
                }
                if (thisFieldInfo.isMainPageShow != 'N') {
                    $('#List' + tableId).datagrid("getColumnOption", thisFieldInfo.field).title = thisFieldInfo.title + "&nbsp;<span class='icon-filter' title='" + (valText == 1) + "'>&nbsp;&nbsp;&nbsp;&nbsp;</span>";
                }
            }
        }
        else if (thisFieldInfo.editControl == "datebox" || thisFieldInfo.editControl == "datetime") {
            if ($("#General_" + thisFieldInfo.field + "_1").val() && $("#General_" + thisFieldInfo.field + "_2").val()) {
                if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "Oracle") {
                    queryStr += " AND " + thisFieldInfo.field + ">=to_date('" + $("#General_" + thisFieldInfo.field + "_1").val() + "','yyyy-mm-dd hh24:mi:ss') AND " + thisFieldInfo.field + "<=to_date('" + $("#General_" + thisFieldInfo.field + "_2").val() + "','yyyy-mm-dd hh24:mi:ss')";
                }
                else {
                    queryStr += " AND " + thisFieldInfo.field + ">='" + $("#General_" + thisFieldInfo.field + "_1").val() + "' AND " + thisFieldInfo.field + "<='" + $("#General_" + thisFieldInfo.field + "_2").val() + "'";
                }
                if (thisFieldInfo.isMainPageShow != 'N') {
                    $('#List' + tableId).datagrid("getColumnOption", thisFieldInfo.field).title = thisFieldInfo.title + "&nbsp;<span class='icon-filter' title='从" + $("#General_" + thisFieldInfo.field + "_1").val() + "到" + $("#General_" + thisFieldInfo.field + "_2").val() + "'>&nbsp;&nbsp;&nbsp;&nbsp;</span>";
                }
            }
        }
        else if ((thisFieldInfo.editControl == "selected" || thisFieldInfo.editControl == "tree" ||thisFieldInfo.editControl == "radio" || thisFieldInfo.editControl == "checkbox") && thisFieldInfo.dataSource) {
            if (valText != "*") {
                queryStr += " AND " + thisFieldInfo.field + " = '" + valText + "'";
                if (thisFieldInfo.isMainPageShow != 'N') {
                    $('#List' + tableId).datagrid("getColumnOption", thisFieldInfo.field).title = thisFieldInfo.title + "&nbsp;<span class='icon-filter' title='" + $("#General_" + thisFieldInfo.field).combobox("getText") + valText + "'>&nbsp;&nbsp;&nbsp;&nbsp;</span>";
                }
            }
        }
        else if (thisFieldInfo.editControl == "number") {
            if (valText != "") {
                queryStr += " AND " + thisFieldInfo.field + " = '" + valText + "'";
                if (thisFieldInfo.isMainPageShow != 'N') {
                    $('#List' + tableId).datagrid("getColumnOption", thisFieldInfo.field).title = thisFieldInfo.title + "&nbsp;<span class='icon-filter' title='" + valText + "'>&nbsp;&nbsp;&nbsp;&nbsp;</span>";
                }
            }
        }
        else {
            if (valText != "") {
                queryStr += " AND lower(" + thisFieldInfo.field + ") like lower('%" + valText + "%')";
                if (thisFieldInfo.isMainPageShow != 'N') {
                    $('#List' + tableId).datagrid("getColumnOption", thisFieldInfo.field).title = thisFieldInfo.title + "&nbsp;<span class='icon-filter' title='" + valText + "'>&nbsp;&nbsp;&nbsp;&nbsp;</span>";
                }
            }
        }
    }
    $("#List" + tableId).datagrid("load", {
        sql: common.Encrypt(formContent.tableData[tableId].tableSql),
        queryStr: queryStr,
        parameters:[]
    });
    if (tableId == 0) click_main_id = 0;
    $('#List' + tableId).datagrid('clearSelections');
    $('#List' + tableId).datagrid();
}

function getSelected() {    //树结点关联查询
    var node = $('#StructTree').tree('getSelected');
    var parent = $('#StructTree').tree('getParent', node.target);
    if (node.is_allow_add == "0") {
        $("#is_allow_add").val("0");
    }
    else {
        $("#is_allow_add").val("1");
    }
    if (parent == null && $('#StructTree').tree('getRoots').length == 1) {
        $("#List0").datagrid("load", {
            sql: common.Encrypt(formContent.tableData[0].tableSql)
        });
    }
    else if (node) {
        var queryStr = formContent.treeData.treeBindData + "='" + node.id + "'";
        $("#List0").datagrid("load", {
            sql: common.Encrypt(formContent.tableData[0].tableSql),
            queryStr: queryStr,
            parameters:[]
        });
    }
}

//ifram 返回
function frameReturnByReload(flag) {
    if (common.getUrlParam("formid") == "1054270510264880006") {//更新内存数据库中按钮信息
        apiRoute.initializationSystem(1,  function (userinfo) {
            sessionStorage["encrypt"] = userinfo.json.encrypt;
        });
    }
    else if (common.getUrlParam("formid") == "1054270510264880008") {//更新内存数据库中数据字典信息
        apiRoute.initializationSystem(2,  function (userinfo) {
            sessionStorage["encrypt"] = userinfo.json.encrypt;
        });
    }
    else if (common.getUrlParam("formid") == "1054270510264880007") {//更新内存数据库中图标信息
        apiRoute.initializationSystem(3,  function (userinfo) {
            sessionStorage["encrypt"] = userinfo.json.encrypt;
        });
    }
    else if (common.getUrlParam("formid") == "1054270510264880004") {//更新内存数据库中数据字段信息
        apiRoute.initializationSystem(5,  function (userinfo) {
            sessionStorage["encrypt"] = userinfo.json.encrypt;
        });
    }
    if (flag)
        $("#List0").datagrid('load');
    else
        $("#List0").datagrid('reload');
    if (parseInt($("#TemplateID").val()) > 2) {//主从表、左侧树右侧主从列表
        var tab = $('#tabs').tabs('getSelected');
        var index = $('#tabs').tabs('getTabIndex', tab);
        index = parseInt(index) + 1;
        if (flag)
            $("#List" + index).datagrid('load');
        else
            $("#List" + index).datagrid('reload');
    }
    frameReturnByTreeLoad();
}

function frameReturnByTreeLoad() {
    if (parseInt($("#TemplateID").val()) % 2 == 0) {//刷新左侧树
        var selectTreeData = $("#StructTree").tree("getSelected");
        $("#StructTree").tree({
            loader: function (param, success, error) {
                apiRoute.selectTreeEvent(dataSourceCode,formContent.treeData.treeSql,
                    formContent.treeData.treeId,formContent.treeData.treePid, formContent.treeData.treeText,"0", [], function (tableData) {
                        success(tableData);
                });
            },
            onLoadSuccess: function (node, data) {
                if (formContent.treeData.treeControlInitMode == "CollapseAll" || formContent.treeData.treeControlInitMode == "ExpandFirstSubNode") {
                    $("#StructTree").tree("collapseAll"); //全部折叠
                }
                if (formContent.treeData.treeControlInitMode == "ExpandFirstSubNode" && data.length > 0) {
                    var node = $("#StructTree").tree('find', data[0].id);
                    $("#StructTree").tree('expand', node.target); //展开第一个节点
                }
                if (selectTreeData != null) {
                    var node = $("#StructTree").tree('find', selectTreeData.id);
                    $("#StructTree").tree('expand', node.target);
                    $("#StructTree").tree('expandTo', node.target);
                    $("#StructTree").tree('select', node.target);
                }
            }
        });
    }
}