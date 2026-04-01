var formContent, dataSourceCode = '', QueryInput = [], formPermission, btnSql, workFlowData;
var tableid_config = common.getUrlParam("tableid"); //第几个表格
var addPermission = [], editPermission = [];        //控制增加、修改的权限
var editorSqltables = null, editorSql = {};
$(function () {
    //workFlowData = common.workFlowHtml();
    //var workflowPermission = workFlowData.queryWfStepField ? workFlowData.queryWfStepField : [];
    var formid_config = common.getUrlParam("formid");   //表单ID
    var btcode_config = common.getUrlParam("btCode");   //操作码
    var editid_config = common.getUrlParam("id");       //编辑ID
    var load_config = common.getUrlParam("Load");
    var PageContent, iconList = [];
    $.ajaxSettings.async = false;
    common.postGateway('platform/system/iconconfig', {}, common.getToken(), function (result) {
        iconList = JSON.parse(result.json);
    });
    $.ajaxSettings.async = true;
    $('#conDiv').css({ 'height': SetGridHeightSub(66), 'overflow-y': 'auto' });
    $(window).resize(function () {
        $('#conDiv').css({ 'height': SetGridHeightSub(66), 'overflow-y': 'auto' });
    });
    if (tableid_config == null) tableid_config = 0;
    if (btcode_config == null) btcode_config = "Create";
    //Load=Complete,从父页取配置信息
    if (load_config == "Complete") {
        formContent = window.parent.formContent;
        dataSourceCode = window.parent.dataSourceCode;
        formPermission = window.parent.formPermission;
        btnSql = window.parent.btnSql;
        if (btcode_config != "Create") {
            PageContent = window.parent.$('#List' + tableid_config).datagrid('getSelected');
        }
        ReadWriteCtrl();
    }
    else if (formid_config != null && formid_config != "") {//需要后台查询配置信息
        //优化平台配置及权限相关信息
        $.ajaxSettings.async = false;
        formPermission = {};
        common.postGateway('platform/permission/pagedataset', {
            "menuId": formid_config
        }, common.getToken(), function (data) {
            if (data.errcode == "0") {
                var webPageData = JSON.parse(data.json);
                dataSourceCode = webPageData.Table2[0].datasourcecode;
                formContent = JSON.parse(common.Decrypt(webPageData.Table2[0].modulejson));

                formPermission.buttonpermission = webPageData.Table3;//获取访问按钮权限
                formPermission.fieldpermission = webPageData.Table4;//获取访问字段权限
                var queryStr = "1=1";
                if (webPageData.Table2[0].urladdress.indexOf('/Admin/Page/PageForm/1') == 0 && formContent.tableData[tableid_config].buttonData.length > 0) { //录入模块
                    btcode_config = "Edit";                    
                    btnSql = formContent.tableData[tableid_config].buttonData[0].btnSql;
                    $("#buttonList").html("<a onclick=\"SaveOneRecord()\" title=\"保存\" class=\"btn btn-default easyui-tooltip\"><span class=\"mdi mdi-content-save\">&nbsp;保存</span></a>&nbsp;");
                }
                else {
                    queryStr = formContent.tableData[tableid_config].tableIdField + "=" + editid_config
                }
                for (var i = 0; i < formContent.tableData[tableid_config].buttonData.length; i++) {
                    if (btcode_config == formContent.tableData[tableid_config].buttonData[i].btnId) {
                        btnSql = formContent.tableData[tableid_config].buttonData[i].btnSql;
                        break;
                    }
                }
                if (btcode_config != "Create") {
                    apiRoute.selectDataEvent(formContent.tableData[tableid_config].tableSql,[], queryStr, function (listData) {
                        PageContent = listData[0];
                        ReadWriteCtrl();
                    });
                }
                else {
                    ReadWriteCtrl();
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
    }

    function ReadWriteCtrl() {
        if (window.RunChildOnLoad && typeof (RunChildOnLoad) == 'function') {
            window.RunChildOnLoad();
        }

        for (var m = 0; m < formPermission.fieldpermission.length; m++) {//隐藏没有权限的字段
            if (formContent.tableData[tableid_config].tableName == formPermission.fieldpermission[m].table_name
                && formPermission.fieldpermission[m].is_deny_new == 1) {
                addPermission.push(formPermission.fieldpermission[m].field_name);
            }
            if (formContent.tableData[tableid_config].tableName == formPermission.fieldpermission[m].table_name
                && formPermission.fieldpermission[m].is_deny_edit == 1) {
                editPermission.push(formPermission.fieldpermission[m].field_name);
            }
        }
        /*
        for (var i = 0; i < workflowPermission.length; i++) { //工作流字段权限
            if (workflowPermission[i].field_edit_mode == "E") continue;
            for (var j = 0; j < formContent.tableData.length; j++) {
                if (workflowPermission[i].table_name == formContent.tableData[j].tableName) {
                    for (var k = 0; k < formContent.tableData[j].fieldData.length; k++) {
                        if (workflowPermission[i].field_name == formContent.tableData[j].fieldData[k].field) {
                            formContent.tableData[j].fieldData[k].isAllowAdd = workflowPermission[i].field_edit_mode;
                            formContent.tableData[j].fieldData[k].isAllowEdit = workflowPermission[i].field_edit_mode;
                            break;
                        }
                    }
                    break;
                }
            }
        }
        */
        if ($("#TemplateID").val() == "2" && formContent.tableData.length > 1) {
            $("#tableList").show();
            for (var i = 1; i < formContent.tableData.length; i++) {
                ListTable(i);
            }
            function ListTable(x) {//数据区表格
                $("#tableList").show();
                $("#tableList").append("<table id=\"List" + x + "\"></table><br>");
                var allWidth = -100000000, columnsArr = [],codeDictionary = {}, pageDictionary = {};
                //性能优化数据字典相关数据查询
                var isDictionary = false;
                for (var n = 0; n < formContent.tableData[x].fieldData.length; n++) {
                    if (formContent.tableData[x].fieldData[n].dataSource != undefined && formContent.tableData[x].fieldData[n].dataSource != "" && formContent.tableData[x].fieldData[n].editControl != "text" && formContent.tableData[x].fieldData[n].editControl != "logic") {
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
                    for (var m = 0; m < formPermission.fieldpermission.length; m++) {//验证字段权限
                        if (formContent.tableData[x].tableName == formPermission.fieldpermission[m].table_name &&
                            formContent.tableData[x].fieldData[n].field == formPermission.fieldpermission[m].field_name) {
                            fieldShow = (formPermission.fieldpermission[m].is_deny_view == 1 ? false : true);
                            break;
                        }
                    }

                    columnsArr.push({});
                    columnsArr[columnsArr.length - 1].field = formContent.tableData[x].fieldData[n].field;
                    if (formContent.tableData[x].fieldData[n].title && common.getCookie("localLanguage") != "en-US") {
                        columnsArr[columnsArr.length - 1].title = formContent.tableData[x].fieldData[n].title;
                    }
                    else {
                        formContent.tableData[x].fieldData[n].title = formContent.tableData[x].fieldData[n].field;
                        columnsArr[columnsArr.length - 1].title = formContent.tableData[x].fieldData[n].field;
                    }
                    if (formContent.tableData[x].fieldData[n].width) {
                        columnsArr[columnsArr.length - 1].width = parseInt(formContent.tableData[x].fieldData[n].width);
                    }
                    else {
                        columnsArr[columnsArr.length - 1].width = 100;
                    }
                    if (formContent.tableData[x].fieldData[n].align) {
                        columnsArr[columnsArr.length - 1].align = formContent.tableData[x].fieldData[n].align;
                    }
                    columnsArr[columnsArr.length - 1].halign = 'center';

                    if (fieldShow == false) {//没有权限显示
                        columnsArr[columnsArr.length - 1].permission = "none";
                        columnsArr[columnsArr.length - 1].hidden = true;
                    }
                    else if (formContent.tableData[x].fieldData[n].isMainPageShow != undefined) {
                        columnsArr[columnsArr.length - 1].hidden = (formContent.tableData[x].fieldData[n].isMainPageShow == "H");
                        columnsArr[columnsArr.length - 1].permission = "show";
                    }
                    else {
                        columnsArr[columnsArr.length - 1].permission = "show";
                        columnsArr[columnsArr.length - 1].hidden = true;
                    }
                    columnsArr[columnsArr.length - 1].sortable = (formContent.tableData[x].tableSortOrder == "none" ? false : true);
                    if (formContent.tableData[x].fieldData[n].style != undefined && formContent.tableData[x].fieldData[n].style != "") {
                        columnsArr[columnsArr.length - 1].style = formContent.tableData[x].fieldData[n].style;
                        columnsArr[columnsArr.length - 1].formatter = function (value) { return eval(this.style)(value) }
                    }
                    if (formContent.tableData[x].fieldData[n].dataSource != undefined && formContent.tableData[x].fieldData[n].dataSource != "") {
                        var tableData = pageDictionary[formContent.tableData[x].fieldData[n].field];
                        var tableDictionary = {};
                        for (var i = 0; i < tableData.length; i++) {
                            tableDictionary[tableData[i].id] = tableData[i].text;
                        }
                        columnsArr[columnsArr.length - 1].formatter = function (value, row, index) {
                            if (tableDictionary[value]) {
                                return tableDictionary[value];
                            }
                            else {
                                return value;
                            }
                        }
                    }

                    if (columnsArr.length == 1 && formContent.tableData[x].isMultipleSelected) {
                        columnsArr[columnsArr.length - 1].hidden = false;
                        columnsArr[columnsArr.length - 1].checkbox = true;
                    }

                    if (!columnsArr[columnsArr.length - 1].hidden && allWidth >= 0) {
                        allWidth += (formContent.tableData[x].fieldData[n].width ? parseInt(formContent.tableData[x].fieldData[n].width) : 100);
                    }

                    if (columnsArr.length == 1) {
                        allWidth = 0;
                        columnsArr.push({
                            field: 'allStyleList', halign: 'right', title: "<a title=\"添加\" onclick=\"gridButton('" + x + "',0,'insertRow')\" class=\"btnline\"><span class=\"fa fa-plus\"></span></a>&nbsp;&nbsp;&nbsp;", width: 80, sortable: false, align: 'center',
                            formatter: function (value, row, index) {
                                if (row) {
                                    return "<a title=\"详细\" onclick=\"gridButton('" + x + "'," + index + ",'detailRow')\" class=\"btnline\"><span class=\"fa fa-list\"></span></a>"
                                        + "<a title=\"编辑\" onclick=\"gridButton('" + x + "'," + index + ",'editRow')\" class=\"btnline\"><span class=\"fa fa-pencil\"></span></a>"
                                        + "<a title=\"删除\" onclick=\"gridButton('" + x + "'," + index + ",'deleteRow')\" class=\"btnline\"><span class=\"fa fa-trash\"></span></a>";
                                } else {
                                    return value;
                                }
                            }
                        });
                    }
                }
                if (allWidth > 0) {//自动计算记录行操作按钮宽度
                    var thisWidth = 150, gridWidth = SetGridWidthSub(80);
                    thisWidth = thisWidth / (gridWidth / allWidth);
                    columnsArr[1].width = thisWidth;
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
                        apiRoute.selectEasyuiTable(dataSourceCode,common.Encrypt(formContent.tableData[x].tableSql), {
                            "pager": param
                        }, function (tableData) {
                            success(tableData);
                        });
                    },
                    width: SetGridWidthSub(80),
                    methord: 'post',
                    fitColumns: true,
                    sortName: formContent.tableData[x].tableSortName,
                    sortOrder: (formContent.tableData[x].tableSortOrder == 'asc' ? 'asc' : 'desc'),
                    idField: formContent.tableData[x].tableIdField,
                    pageSize: formContent.tableData[x].pageNum,
                    pageList: pageListTemp,
                    pagination: formContent.tableData[x].isPagination,  //是否分页
                    striped: true, //奇偶行是否区分
                    multiSort: false, //多列排序
                    checkOnSelect: true,//如果设置为 true，当用户点击某一行时，则会选中/取消选中复选框。如果设置为 false 时，只有当用户点击了复选框时，才会选中/取消选中复选框。
                    selectOnCheck: true,//如果设置为 true，点击复选框将会选中该行。如果设置为 false，选中该行将不会选中复选框
                    singleSelect: (!formContent.tableData[x].isMultipleSelected),//单选模式
                    rownumbers: formContent.tableData[x].isRownumber,//行号
                    columns: [columnsArr],
                    onHeaderContextMenu: function (e, field) {
                        e.preventDefault();
                        $(this).datagrid('columnMenu').menu('show', {
                            left: e.pageX,
                            top: e.pageY
                        });
                    },
                    onRowContextMenu: function (e, rowIndex, rowData) { //右键时触发事件
                        if ($('#rightMenu' + x).children('div').length > 1) {
                            e.preventDefault(); //阻止浏览器捕获右键事件
                            $(this).datagrid("clearSelections"); //取消所有选中项
                            $(this).datagrid("selectRow", rowIndex); //根据索引选中该行
                            if (rowData != null) {
                                $('#rightMenu' + x).menu('show', {
                                    //显示右键菜单
                                    left: e.pageX,//在鼠标点击处显示菜单
                                    top: e.pageY
                                });
                            }
                            e.preventDefault();  //阻止浏览器自带的右键菜单弹出
                        }
                    }
                });
            }
        }
        else {
            $("#tableList").hide();
        }

        QueryInput = formContent.tableData[tableid_config].fieldData;
        var form_n = 0, widthSet = 300, titleWidth = '100px;';
        if (window.screen.width <= 1366 && formContent.tableData[tableid_config].multipleColumn == "3") {
            formContent.tableData[tableid_config].multipleColumn = "2"
        }
        if (formContent.tableData[tableid_config].multipleColumn == "" || formContent.tableData[tableid_config].multipleColumn == "1") {
            formContent.tableData[tableid_config].multipleColumn = 1;
            widthSet = 300;
            titleWidth='auto'
            $("#multipleColumn").html('<tr><th width="30%"></th><td width="40%"></td><td width="30%"></td></tr>');
        }
        else if (formContent.tableData[tableid_config].multipleColumn == "2") {
            widthSet = 220;
            $("#multipleColumn").html('<tr><th width="15%"></th><td width="20%"></td><td width="15%"></td><th width="15%"></th><td width="20%"></td><td width="15%"></td></tr>');
        }
        else if (formContent.tableData[tableid_config].multipleColumn == "3") {
            widthSet = 180;
            $("#multipleColumn").html('<tr><th width="10%"></th><td width="13%"></td><td width="10%"></td><th width="10%"></th><td width="14%"></td><td width="10%"></td><th width="10%"></th><td width="13%"></td><td width="10%"></td></tr>');
        }
        var inputhtml = "";
        for (var i = 0; i < QueryInput.length; i++) {
            if (common.getUrlParam("ViewFlow") == "true" && QueryInput[i].title && QueryInput[i].isAllowEdit == "H") {
                QueryInput[i].isAllowEdit = "E";//将工作流查看字段全设为可见
            }
            if (i > 0 && (  //第一列为主键
                ((!QueryInput[i].editControl) || QueryInput[i].editControl == "") ||
                (QueryInput[i].isAllowAdd == "N" && QueryInput[i].isAllowEdit == "N"))) { //删除无效属性
                QueryInput.splice(i, 1);
                i--;
                continue;
            }
            if ((btcode_config == "Create" && (QueryInput[i].isAllowAdd == "H" || $.inArray(QueryInput[i].field, addPermission) != -1)) || //添加时动态加载隐藏控件
                (btcode_config == "Edit" && (QueryInput[i].isAllowEdit == "H" || $.inArray(QueryInput[i].field, editPermission) != -1))) {//修改时动态加载隐藏控件
                $(".formtable").append("<input id=\"" + QueryInput[i].field + "\" type=\"hidden\" value=\"\" />");
                continue;
            }
            if ((form_n % parseInt(formContent.tableData[tableid_config].multipleColumn)) == 0) {
                inputhtml += "<tr>";
            }

            if (QueryInput[i].editControl == "html") {
                if ((form_n % parseInt(formContent.tableData[tableid_config].multipleColumn)) == 0) {
                    inputhtml += "<th><label for=\"" + QueryInput[i].field + "\">" + QueryInput[i].title + "：</label></th>";
                }
                else {
                    for (var j = (form_n % parseInt(formContent.tableData[tableid_config].multipleColumn)); j < parseInt(formContent.tableData[tableid_config].multipleColumn); j++) {
                        inputhtml += "<th></th><td></td><td></td>";
                    }
                    inputhtml += "</tr>";
                    form_n = 0;
                    inputhtml += "<tr>";
                    inputhtml += "<th><label for=\"" + QueryInput[i].field + "\">" + QueryInput[i].title + "：</label></th>";
                }
                inputhtml += "<td colspan=\"" + (2 + 3 * (parseInt(formContent.tableData[tableid_config].multipleColumn) - 1)) + "\">";
                inputhtml += "<textarea name=\"" + QueryInput[i].field + "\" id=\"" + QueryInput[i].field + "\" style=\"width:95%; height:200px; \"></textarea>";
                inputhtml += "<script type=\"text/javascript\">$(function() {UE.getEditor(\"" + QueryInput[i].field + "\", { elementPathEnabled: false, wordCount: false, maximumWords: 1000000000, autoHeightEnabled: false";
                if ((btcode_config == "Create" && QueryInput[i].isAllowAdd == "R") || (btcode_config == "Edit" && QueryInput[i].isAllowEdit == "R")) {
                    inputhtml += ", readonly: true";
                }
                inputhtml += "});});</script><br></td></tr>";
            }
            else {
                inputhtml += "<th style=\"width:"+ titleWidth+"\"><label for=\"" + QueryInput[i].field + "\">" + QueryInput[i].title + "：</label></th>";
                inputhtml += "<td>";
                var htmlNote = "";
                if (QueryInput[i].note) {
                    if (QueryInput[i].note.length < 6) {
                        htmlNote = "<span style='color:#ff0000'>" + QueryInput[i].note + "</span>";
                    } else {
                        htmlNote = "<span class='mdi mdi-comment-question-outline' style='color:#ff0000;font-size: 20px;' title='" + QueryInput[i].note + "'></span>";
                    }
                }
                if ((btcode_config == "Create" && QueryInput[i].isAllowAdd == "R") || (btcode_config == "Edit" && QueryInput[i].isAllowEdit == "R")) {
                    inputhtml += "<input style=\"width:" + widthSet + "px;background: #e7eaec;cursor: no-drop;\" class=\"text-box single-line\" readonly disabled id=\"" + QueryInput[i].field + "\" name=\"" + QueryInput[i].field + "\" type=\"text\" />";
                    inputhtml += "</td><td>" + htmlNote + "</td>";
                }
                else if (QueryInput[i].editControl == "number") {
                    inputhtml += "<input style=\"width:" + widthSet + "px\" class=\"text-box single-line\" data-val=\"true\" data-val-number=\"字段 " + QueryInput[i].title + " 必须是一个数字。\" id=\"" + QueryInput[i].field + "\" name=\"" + QueryInput[i].field + "\" type=\"" + QueryInput[i].editControl + "\" onkeyup='this.value=this.value.replace(/\\D/gi,\"\")' />";
                    inputhtml += "</td><td><span class=\"field-validation-valid\" data-valmsg-for=\"" + QueryInput[i].field + "\" data-valmsg-replace=\"true\"></span>" + htmlNote + "</td>";
                }
                else if (QueryInput[i].editControl == "password") {
                    inputhtml += "<input style=\"width:" + widthSet + "px\"  class=\"text-box single-line\" data-val=\"true\" id=\"" + QueryInput[i].field + "\" name=\"" + QueryInput[i].field + "\" type=\"password\" /></td><td>" + htmlNote + "</td>";
                }
                else if (QueryInput[i].editControl == "datebox") {
                    inputhtml += "<input style=\"width:" + widthSet + "px\"  data-val=\"true\" class=\"Wdate\" readonly data-val-date=\"字段 " + QueryInput[i].title + " 必须是日期。\" id=\"" + QueryInput[i].field + "\" name=\"" + QueryInput[i].field + "\" type=\"text\" onclick=\"WdatePicker()\" />";
                    inputhtml += "</td><td>" + htmlNote + "<span class=\"field-validation-valid\" data-valmsg-for=\"" + QueryInput[i].field + "\" data-valmsg-replace=\"true\"></span></td>";
                }
                else if (QueryInput[i].editControl == "datetime") {
                    inputhtml += "<input style=\"width:" + widthSet + "px\"  data-val=\"true\" class=\"Wdate\" readonly data-val-date=\"字段 " + QueryInput[i].title + " 必须是日期。\" id=\"" + QueryInput[i].field + "\" name=\"" + QueryInput[i].field + "\" type=\"text\" onclick=\"WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})\" />";
                    inputhtml += "</td><td>" + htmlNote + "<span class=\"field-validation-valid\" data-valmsg-for=\"" + QueryInput[i].field + "\" data-valmsg-replace=\"true\"></span></td>";
                }
                else if (QueryInput[i].editControl == "texteara" || QueryInput[i].editControl == "editorSql") {
                    inputhtml += "<textarea cols=\"20\" id=\"" + QueryInput[i].field + "\" name=\"" + QueryInput[i].field + "\" rows=\"2\" style=\"width:" + widthSet + "px;height:50px;\"></textarea></td><td>" + htmlNote + "</td>";
                }
                else if (QueryInput[i].editControl == "radio") {  //单选
                    inputhtml += "<span id=\"" + QueryInput[i].field + "\"></span></td><td>" + htmlNote + "</td>";
                }
                else if (QueryInput[i].editControl == "checkbox") {  //复选框
                    inputhtml += "<span id=\"" + QueryInput[i].field + "\"></span></td><td>" + htmlNote + "</td>";
                }
                else if (QueryInput[i].editControl == "selected" || QueryInput[i].editControl == "multipleselect") { //下拉
                    inputhtml += "<input style=\"width:" + (widthSet + 8) + "px\"  data-val=\"true\" id=\"" + QueryInput[i].field + "\" name=\"" + QueryInput[i].field + "\" type=\"text\" />";
                    inputhtml += "</td><td>" + htmlNote + "</td>";
                }
                else if (QueryInput[i].editControl == "openselect") { //弹框下拉
                    inputhtml += "<input style=\"width:" + (widthSet - 40) + "px\"  data-val=\"true\" id=\"" + QueryInput[i].field + "\" name=\"" + QueryInput[i].field + "\" type=\"text\" />";
                    inputhtml += "<span id=\"Open_" + QueryInput[i].field + "\" class=\"mdi mdi-cloud-search\" style=\"font-size:18px;padding:0 5px;cursor:pointer;color: #989898;\"></span>"
                    inputhtml += "</td><td>" + htmlNote + "</td>";
                }
                else if (QueryInput[i].editControl == "orgRadioCtrl" || QueryInput[i].editControl == "orgCheckCtrl" ||
                    QueryInput[i].editControl == "areaSelect") { //用户控件下拉
                    inputhtml += "<input style=\"width:" + (widthSet - 40) + "px\"  data-val=\"true\" id=\"" + QueryInput[i].field + "\" name=\"" + QueryInput[i].field + "\" type=\"text\" />";
                    inputhtml += "<span id=\"Open_" + QueryInput[i].field + "\" class=\"mdi mdi-plus-circle-outline\" style=\"font-size:18px;padding:0 5px;cursor:pointer;color: #989898;\"></span>"
                    inputhtml += "</td><td>" + htmlNote + "</td>";
                }
                else if (QueryInput[i].editControl == "tree") { //树下拉
                    inputhtml += "<input style=\"width:" + (widthSet + 8) + "px\"  data-val=\"true\" id=\"" + QueryInput[i].field + "\" name=\"" + QueryInput[i].field + "\" type=\"text\" />";
                    inputhtml += "</td><td>" + htmlNote + "</td>";
                }
                else if (QueryInput[i].editControl == "logic") { //逻辑是否
                    inputhtml += "<input class=\"easyui-switchbutton\" style=\"width:50px;\" value=\"true\" id=\"" + QueryInput[i].field + "\" name=\"" + QueryInput[i].field + "\" ontext=\"\" offtext=\"\" checked />";
                    inputhtml += "</td><td>" + htmlNote + "<span class=\"field-validation-valid\" data-valmsg-for=\"" + QueryInput[i].field + "\" data-valmsg-replace=\"true\"></span></td>";
                }
                else if (QueryInput[i].editControl == "picCtrl") {
                    inputhtml += "<input style=\"width:" + (widthSet - 10) + "px\"  class=\"text-box single-line\" data-val=\"true\" id=\"" + QueryInput[i].field + "\" name=\"" + QueryInput[i].field + "\" type=\"text\" />"
                        + htmlNote + "<span class=\"mdi mdi-more\" style=\"font-size:14px;cursor:pointer;color: #989898;\" id=\"Span_" + QueryInput[i].field + "\" onclick=\"mdi_Open(\'" + QueryInput[i].field + "\')\"></span></td><td></td>";
                }
                else if (QueryInput[i].editControl == "upLoadFile") {
                    inputhtml += "<input style=\"width:" + (widthSet - 80) + "px\" readonly data-val=\"true\" id=\"" + QueryInput[i].field + "\" name=\"" + QueryInput[i].field + "\" type=\"text\" />";
                    inputhtml += "<span id=\"file_upload" + QueryInput[i].field + "\" title=\"上传文件\" class=\"mdi mdi-cloud-upload\" style=\"font-size:18px;padding:0 5px;cursor:pointer;color: #989898;\"></span>"
                    inputhtml += "<span id=\"file_preview" + QueryInput[i].field + "\" title=\"预览文件\" class=\"mdi mdi-folder-search\" style=\"font-size:18px;padding:0 5px;cursor:pointer;color: #989898;\"></span>"
                    inputhtml += "<span id=\"file_delete" + QueryInput[i].field + "\" title=\"删除文件\" class=\"mdi mdi-delete-restore\" style=\"font-size:18px;padding:0 5px;cursor:pointer;color: #989898;\"></span>"
                    inputhtml += "</td><td>" + htmlNote + "</td>";
                }
                else if (QueryInput[i].isRequired == 1) {
                    inputhtml += "<input style=\"width:" + widthSet + "px\"  class=\"text-box single-line\" data-val=\"true\" data-val-required=\"" + QueryInput[i].title + " 字段是必需的。\" id=\"" + QueryInput[i].field + "\" name=\"" + QueryInput[i].field + "\" type=\"" + QueryInput[i].editControl + "\" value=\"\"/>";
                    inputhtml += "</td><td>" + htmlNote + "<span class=\"field-validation-valid\" data-valmsg-for=\"" + QueryInput[i].field + "\" data-valmsg-replace=\"true\"></span></td>";
                }
                else {
                    inputhtml += "<input style=\"width:" + widthSet + "px\"  class=\"text-box single-line\" data-val=\"true\" id=\"" + QueryInput[i].field + "\" name=\"" + QueryInput[i].field + "\" type=\"text\" /></td><td>" + htmlNote + "</td>";
                }
                if ((form_n % parseInt(formContent.tableData[tableid_config].multipleColumn)) == (parseInt(formContent.tableData[tableid_config].multipleColumn) - 1) || (i == (QueryInput.length - 1))) {
                    if ((i == (QueryInput.length - 1)) && ((form_n % parseInt(formContent.tableData[tableid_config].multipleColumn)) != (parseInt(formContent.tableData[tableid_config].multipleColumn) - 1))) {
                        for (var j = (form_n % parseInt(formContent.tableData[tableid_config].multipleColumn)); j < (parseInt(formContent.tableData[tableid_config].multipleColumn) - 1); j++) {
                            inputhtml += "<th></th><td></td><td></td>";
                        }
                    }
                    inputhtml += "</tr>";
                }
                form_n++;
            }
        }
        $("table tbody tr").eq(0).before(inputhtml);

        //性能优化数据字典相关数据查询
        var codeDictionary = {}, pageDictionary = {}, isDictionary = false;
        for (var n = 0; n < QueryInput.length; n++) {
            if (QueryInput[n].dataSource != undefined && QueryInput[n].dataSource != "" && QueryInput[n].editControl != "text" && QueryInput[n].editControl != "logic") {
                isDictionary = true;
                codeDictionary[QueryInput[n].field] = common.Encrypt(QueryInput[n].dataSource);
            }
        }
        if (isDictionary) {
            $.ajaxSettings.async = false;
            apiRoute.pageDictionary(codeDictionary, function (tableData) {
                pageDictionary = tableData;
            });
            $.ajaxSettings.async = true;
        }

        for (var i = 0; i < QueryInput.length; i++) {
            if ((btcode_config == "Create" && (QueryInput[i].isAllowAdd == "H" || $.inArray(QueryInput[i].field, addPermission) != -1)) || //添加时动态加载隐藏控件
                (btcode_config == "Edit" && (QueryInput[i].isAllowEdit == "H" || $.inArray(QueryInput[i].field, editPermission) != -1))) {//是隐藏控件
                continue;
            }
            //动态加载EasyUi控件
            else if (QueryInput[i].editControl == "text" && btcode_config == "Create" && QueryInput[i].isAllowAdd == "R" && QueryInput[i].dataSource != "") {
                if (QueryInput[i].dataSource.indexOf("GetSequenceNo") == 0) {
                    $.ajaxSettings.async = false;
                    common.postGateway('platform/sequence/getsequenceno', {
                        "seqNameOrId": QueryInput[i].dataSource.replace('GetSequenceNo(\'', '').replace('\')', '')
                    }, common.getToken(), function (sequenceNo) {
                        $("#" + QueryInput[i].field).val(sequenceNo.json);
                    });
                    $.ajaxSettings.async = true;
                }
                else {
                    $("#" + QueryInput[i].field).val(QueryInput[i].dataSource);
                }
            }
            else if (QueryInput[i].editControl == "radio" || QueryInput[i].editControl == "checkbox") {//单选控件数据加载,多选控件数据加载
                var tableData = pageDictionary[QueryInput[i].field];
                for (var j = 0; j < tableData.length; j++) {
                    var checkedHtml = "<input type=\"" + QueryInput[i].editControl + "\" value=\"" + tableData[j].id + "\" name=\"" + QueryInput[i].field + "\" id=\"" + QueryInput[i].field + "_" + j + "\"";
                    if (tableData[j].selected && tableData[j].selected == 1) {
                        checkedHtml += " checked";
                    }
                    checkedHtml += "><label for=\"" + QueryInput[i].field + "_" + j + "\">" + tableData[j].text + "</label>&nbsp;&nbsp;"
                    $("#" + QueryInput[i].field).append(checkedHtml);
                }
            }
            else if (QueryInput[i].editControl == "selected" || QueryInput[i].editControl == "multipleselect") {//下拉控件数据加载
                loadCombobox(i, QueryInput[i].dataSource);
                function loadCombobox(x, dataSource) {
                    $("#" + QueryInput[x].field).combobox({
                        loader: function (param, success, error) {
                            var whereSql = "";
                            if (dataSource.split(";").length > 3) {
                                whereSql = dataSource.split(";")[3];
                            }
                            if (tableid_config != 0 && whereSql.indexOf("@t_id") > 0) {
                                var parTid = window.parent.$('#List0').datagrid("getData").rows[window.parent.click_main_id].t_id;
                                whereSql = whereSql.replace('@t_id', "'" + parTid + "'");
                            }
                            if (dataSource.split(";").length > 3 && whereSql != "") {
                                var sqldict = dataSource.split(";")[0] + " " + whereSql;
                                if (sqldict.indexOf('@') != -1) return;
                                apiRoute.pageDictionary({ "dictionaryorsql": common.Encrypt(sqldict) }
                                    , function (tableDataSet) {
                                        var tableData = tableDataSet.dictionaryorsql;
                                        success(tableData);
                                    });
                            }
                            else {
                                success(pageDictionary[QueryInput[x].field]);
                            }
                        },
                        showItemIcon: true,
                        editable: (dataSource.split(";").length > 1 && dataSource.split(";")[1] == "true" ? true : false),
                        multiple: (QueryInput[x].editControl == "multipleselect" ? true : false),
                        panelHeight: 'auto',
                        width: widthSet + 8,
                        panelMaxHeight: 250,
                        onLoadSuccess: function (data) {
                            for (var n = 0; n < data.length; n++) {
                                if (data[n].selected && data[n].selected == 1) {
                                    $(this).combobox("setValue", data[n].id);
                                }
                            }
                        },
                        onHidePanel: function () {
                            var _options = $(this).combobox('options');
                            var _data = $(this).combobox('getData');/* 下拉框所有选项 */
                            var _value = $(this).combobox('getValue');/* 用户输入的值 */
                            var _b = false;/* 标识是否在下拉列表中找到了用户输入的字符 */
                            for (var i = 0; i < _data.length; i++) {
                                if (_data[i][_options.valueField].indexOf(_value) != -1) {
                                    _b = true;
                                    break;
                                }
                            }
                            if (!_b) {
                                $(this).combobox('setValue', '');
                            }
                        },
                        onSelect: function (record) {//是否自动联动字段控件
                            if (dataSource.split(";").length > 2 && dataSource.split(";")[2].length > 0 && dataSource.split(";")[2] == "AllFieldName") {
                                for (var key in record) {
                                    if ($("#" + key).length > 0 && key != "id" && key != "text") {
                                        $("#" + key).val(record[key]);
                                    }
                                }
                            }
                        },
                        onChange: function (newValue, oldValue) {
                            if (dataSource.split(";").length > 2 && dataSource.split(";")[2].length > 0) {
                                for (var n = 0; dataSource.split(";")[2] != "AllFieldName" && n < QueryInput.length; n++) {
                                    if (QueryInput[n].field == dataSource.split(";")[2]) {
                                        var newdataSource = QueryInput[n].dataSource.replace(new RegExp("@" + QueryInput[x].field, "g"), "'" + newValue + "'");
                                        if (QueryInput[n].editControl == "selected" || QueryInput[n].editControl == "multipleselect") loadCombobox(n, newdataSource);
                                        if (QueryInput[n].editControl == "tree") loadCombotree(n, newdataSource);
                                        break;
                                    }
                                }
                            }
                        },
                        valueField: 'id',
                        textField: 'text',
                        formatter: JQueryEasyUI.dataDictionaryFormat
                    });
                };
            }
            else if (QueryInput[i].editControl == "tree") {//下拉树数据加载
                $.ajaxSettings.async = false;
                loadCombotree(i, QueryInput[i].dataSource);
                function loadCombotree(x, dataSource) {
                    $("#" + QueryInput[x].field).combotree({
                        loader: function (param, success, error) {
                            var whereSql = "", sqldict;
                            if (dataSource.split(";").length > 3) {
                                whereSql = dataSource.split(";")[3];
                            }
                            if (tableid_config != 0 && whereSql.indexOf("@t_id") > 0) {
                                var parTid = window.parent.$('#List0').datagrid("getData").rows[window.parent.click_main_id].t_id;
                                whereSql = whereSql.replace('@t_id', "'" + parTid + "'");
                            }
                            if (dataSource.split(";").length > 3 && whereSql != "") {
                                sqldict = dataSource.split(";")[0] + " " + whereSql;
                            }
                            else {
                                sqldict = dataSource.split(";")[0];
                            }
                            apiRoute.selectTreeEvent(dataSourceCode, sqldict,
                                "id", "pid", "text", "0", [], function (tableData) {
                                    success(tableData);
                                });
                        },
                        showItemIcon: true,
                        editable: (dataSource.split(";").length > 1 && dataSource.split(";")[1] == "true" ? true : false),
                        panelHeight: 'auto',
                        panelMaxHeight: 250,
                        //formatter: JQueryEasyUI.treeDictionaryFormat,
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
                        },
                        onSelect: function (record) {//是否自动联动字段控件
                            if (dataSource.split(";").length > 2 && dataSource.split(";")[2].length > 0 && dataSource.split(";")[2] == "AllFieldName") {
                                for (var key in record) {
                                    if ($("#" + key).length > 0 && key != "id" && key != "text") {
                                        $("#" + key).val(record[key]);
                                    }
                                }
                            }
                        },
                        onChange: function (newValue, oldValue) {
                            if (dataSource.split(";").length > 2 && dataSource.split(";")[2].length > 0) {
                                for (var n = 0; dataSource.split(";")[2] != "AllFieldName" && n < QueryInput.length; n++) {
                                    if (QueryInput[n].field == dataSource.split(";")[2]) {
                                        var newdataSource = QueryInput[n].dataSource.replace(new RegExp("@" + QueryInput[x].field, "g"), "'" + newValue + "'");
                                        if (QueryInput[n].editControl == "selected" || QueryInput[n].editControl == "multipleselect") loadCombobox(n, newdataSource);
                                        if (QueryInput[n].editControl == "tree") loadCombotree(n, newdataSource);
                                        break;
                                    }
                                }
                            }
                        }
                    });
                }
                $.ajaxSettings.async = true;
                if (common.getUrlParam("treeid") != null && btcode_config == "Create") {
                    $("#" + QueryInput[i].field).combotree("setValues", common.getUrlParam("treeid").toString());
                }
            }
            else if (QueryInput[i].editControl == "logic") {//是否逻辑值控件
                $("#" + QueryInput[i].field).switchbutton({ checked: (QueryInput[i].dataSource == "0" ? false : true) });
            }
            else if (QueryInput[i].editControl == "picCtrl") {//图标选择控件
                PicCtrlList(i);
                function PicCtrlList(x) {
                    $("#" + QueryInput[x].field).combo({
                        editable: false,
                        panelWidth: 460,
                        prompt: '            点击选择图标',
                        panelMaxHeight: 250,
                        hasDownArrow: false,
                        //width: widthSet + 8,
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
                    $('#picCtrl').appendTo($('#' + QueryInput[x].field).combo('panel'));
                    $('#picCtrl').delegate('a', 'click', function () {
                        var value = $(this).attr('value');
                        var text = $(this).attr('text');
                        $("#" + QueryInput[x].field).combo({ buttonIcon: text, prompt: '            点击更换图标' }).combo('setValue', value).combo('hidePanel');
                    });
                }
            }
            else if (QueryInput[i].editControl == "orgRadioCtrl" || QueryInput[i].editControl == "orgCheckCtrl") {
                $("#" + QueryInput[i].field).combobox({
                    loader: function (param, success, error) {
                        apiRoute.selectDataEvent("select a.t_id,userid,username,b.org_id from prod_system_user a,prod_common_org_user b where a.t_id=b.usertid and b.client_id='&CLIENT_ID&'",
                            [], "1=1", function (tableData) {
                                success(tableData);
                            });
                    },
                    editable: false,
                    hasDownArrow: false,
                    valueField: 't_id',
                    textField: 'userid',
                    panelHeight: 0,
                    panelMaxHeight: 0,
                    multiple: true,
                    readonly: true
                });

                UserCtrlList(i);
                function UserCtrlList(x) {
                    $("#Open_" + QueryInput[x].field).click(function () {
                        $("#modalwindow").html("<iframe id='user_" + QueryInput[x].field + "' width='100%' height='100%'  frameborder='0' src='/Admin/Page/UserCtrl?tableid=" + tableid_config + "&ctrlParam=" + QueryInput[x].editControl + "&ctrlName=" + QueryInput[x].field + "'></iframe>");
                        $("#modalwindow").dialog({
                            width: 450,
                            height: 370,
                            closed: false,
                            cache: false,
                            modal: true,
                            buttons: [{
                                text: ' 确 定 ',
                                handler: function () {
                                    var userId = [], selectUser = document.getElementById("user_" + QueryInput[x].field).contentWindow.$('#selectlist').datagrid("getData").rows;
                                    for (var i = 0; i < selectUser.length; i++) {
                                        if (selectUser[i].userid.indexOf("√") == 0) {
                                            userId.push(selectUser[i].t_id);
                                        }
                                    }
                                    if (userId.length == 0) {
                                        $.messageBoxAlert(0, "对不起，没有选中任何数据！");
                                    }
                                    else {
                                        $("#" + QueryInput[x].field).combobox("setValues", userId.join(",").toString());
                                        $("#modalwindow").dialog('close');
                                    }
                                }
                            }, {
                                text: ' 取 消 ',
                                handler: function () {
                                    $("#modalwindow").dialog('close');
                                }
                            }]
                        });
                    });
                }
            }
            else if (QueryInput[i].editControl == "upLoadFile") {
                $("#file_upload" + QueryInput[i].field).click(function () {
                    if ((!$("#" + this.id.replace('file_upload', '')).val()) || $("#" + this.id.replace('file_upload', '')).val() == 'NoFile') {
                        $("#fileid").val(this.id.replace('file_upload', ''));
                        $("#dialogFileUpload").dialog('open').dialog('center');
                    }
                    else {
                        $.messageBoxBottom(0, "请先删除该文件再上传！");
                    }
                });
                $("#file_preview" + QueryInput[i].field).click(function () {
                    let fileName = $("#" + this.id.replace('file_preview', '')).val()
                    if (fileName) {
                        filePreview(fileName)
                    }
                    else {
                        $.messageBoxBottom(0, "请先上传文件！");
                    }
                });
                $("#file_delete" + QueryInput[i].field).click(function () {
                    $("#fileid").val(this.id.replace('file_delete', ''));
                    deleteFile();
                });
            }
            else if (QueryInput[i].editControl == "openselect") {
                $("#" + QueryInput[i].field).combobox({
                    loader: function (param, success, error) {
                        success(pageDictionary[QueryInput[i].field]);
                    },
                    editable: false,
                    hasDownArrow: false,
                    panelHeight: 0,
                    panelMaxHeight: 0,
                    multiple: true,
                    valueField: 'id',
                    textField: 'text',
                    readonly: true
                });
                OpenSelectList(i);
                function OpenSelectList(x) {
                    var dataSource = QueryInput[x].dataSource.split(';')[1] + "&pagelist=selectOpen&ctrlName=" + QueryInput[x].field + "&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId");
                    $("#Open_" + QueryInput[x].field).click(function () {
                        $("#modalwindow").html("<iframe id='selectopen_" + QueryInput[x].field + "' width='100%' height='100%'  frameborder='0' src='" + dataSource + "'></iframe>");

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
                                    var userId = [], selectArr = document.getElementById("selectopen_" + QueryInput[x].field).contentWindow.$('#List0').datagrid("getSelections");
                                    for (var i = 0; i < selectArr.length; i++) {
                                        userId.push(selectArr[i].id);
                                    }
                                    if (userId.length == 0) {
                                        $.messageBoxAlert(0, "对不起，没有选中任何数据！");
                                    }
                                    else {
                                        for (var key in selectArr[0]) {
                                            for (var i = 0; i < QueryInput.length; i++) {
                                                if (key == QueryInput[i].field) {
                                                    if (QueryInput[i].editControl == "text" || QueryInput[i].editControl == "number" || QueryInput[i].editControl == "password" || QueryInput[i].editControl == "texteara" || QueryInput[i].editControl == "datetime") {
                                                        $("#" + QueryInput[i].field).val(selectArr[0][key]);
                                                    } else if (QueryInput[i].editControl == "selected" || QueryInput[i].editControl == "multipleselect") {
                                                        var dataSource = QueryInput[i].dataSource;
                                                        if (dataSource.split(';').length > 3 && dataSource.split(';')[3].indexOf("@" + QueryInput[i].field) != -1) {
                                                            var newdataSource = dataSource.replace(new RegExp("@" + QueryInput[i].field, "g"), "'" + selectArr[0][key] + "'");
                                                            loadCombobox(i, newdataSource);
                                                        }
                                                        else {
                                                            $("#" + QueryInput[i].field).combobox("setValues", selectArr[0][key]);
                                                        }
                                                    }
                                                    break
                                                }
                                            }
                                        }
                                        $("#" + QueryInput[x].field).combobox("setValues", userId.join(",").toString());  //userId.join(",").toString()
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
                    });
                }
            }
            else if (QueryInput[i].editControl == "editorSql") {
                if (editorSqltables == null) {
                    editorSqltables = {};
                    $.ajaxSettings.async = false;
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
                    $.ajaxSettings.async = true;
                }
                editorSql[QueryInput[i].field] = CodeMirror.fromTextArea(document.getElementById(QueryInput[i].field), {
                    lineNumbers: false,  //不显示行数
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
                editorSql[QueryInput[i].field].setSize($("#" + QueryInput[i].field).width()+8, 100);
                // 将自动提示绑定到change事件上，这样输入的时候就可以看到联想的关键词
                editorSql[QueryInput[i].field].on('change', function (instance, change) {
                    // 自动补全的时候，也会触发change事件，所有坐下判断，以免死循环，正则是为了不让空格，换行之类的也提示
                    // 通过change对象你可以自定义一些规则去判断是否提示
                    if (change.origin !== 'complete' && /\w|\./g.test(change.text[0])) {
                        instance.showHint();
                    }
                })
                
            }
            //动态加载需要增加JS验证
            if (QueryInput[i].isRequired) {
                $("#" + QueryInput[i].field).rules("add", { required: true, messages: { required: QueryInput[i].title + " 字段是必需的。" } });
            }
            if (QueryInput[i].editControl == "number") {
                $("#" + QueryInput[i].field).rules("add", { number: true, messages: { required: "字段 " + QueryInput[i].title + " 必须是一个数字。" } });
            }
            if (QueryInput[i].dataValidate && QueryInput[i].dataValidate != "None") {
                $("#" + QueryInput[i].field).validatebox({
                    required: QueryInput[i].isRequired,
                    validType: QueryInput[i].dataValidate
                });
            }
        }
        if (tableid_config > 0) {//从表关联
            if (window.parent.$('#List0').length > 0) {//主从表列表方式
                var gridRow = window.parent.$('#List0').datagrid("getData").rows[window.parent.click_main_id];//获取选中的数据
                for (var i = 0; i < formContent.tableData[tableid_config].mainSubLine.length; i++) {
                    $("#" + formContent.tableData[tableid_config].mainSubLine[i].subGridInput).val(gridRow[[formContent.tableData[tableid_config].mainSubLine[i].mainGridOutput]]);
                    for (var j = 0; j < QueryInput.length; j++) {
                        if (QueryInput[j].field == formContent.tableData[tableid_config].mainSubLine[i].subGridInput) {
                            if (QueryInput[j].editControl == "selected") {
                                $("#" + formContent.tableData[tableid_config].mainSubLine[i].subGridInput).combobox("setValue", gridRow[[formContent.tableData[tableid_config].mainSubLine[i].mainGridOutput]]).combo('readonly', true);//主表ID自动设到从表PID中
                            }
                            else if (QueryInput[j].editControl == "multipleselect") {
                                $("#" + formContent.tableData[tableid_config].mainSubLine[i].subGridInput).combobox("setValues", gridRow[[formContent.tableData[tableid_config].mainSubLine[i].mainGridOutput].toString()]).combo('readonly', true);//主表ID自动设到从表PID中
                            }
                            else if (QueryInput[j].editControl == "tree") {
                                $("#" + formContent.tableData[tableid_config].mainSubLine[i].subGridInput).combotree("setValue", gridRow[[formContent.tableData[tableid_config].mainSubLine[i].mainGridOutput]]).combo('readonly', true);
                            }
                            break;
                        }
                    }
                }
            }
            else {//主从表表单方式
                for (var i = 0; i < formContent.tableData[tableid_config].mainSubLine.length; i++) {
                    if (window.parent.$("#" + formContent.tableData[tableid_config].mainSubLine[i].mainGridOutput).length == 0) continue;
                    $("#" + formContent.tableData[tableid_config].mainSubLine[i].subGridInput).val(window.parent.$("#" + formContent.tableData[tableid_config].mainSubLine[i].mainGridOutput).val());
                    for (var j = 0; j < QueryInput.length; j++) {
                        if (QueryInput[j].field == formContent.tableData[tableid_config].mainSubLine[i].subGridInput) {
                            if (QueryInput[j].editControl == "selected") {
                                $("#" + formContent.tableData[tableid_config].mainSubLine[i].subGridInput).combobox("setValue", window.parent.$("#" + formContent.tableData[tableid_config].mainSubLine[i].mainGridOutput).val()).combo('readonly', true);//主表ID自动设到从表PID中
                            }
                            else if (QueryInput[j].editControl == "multipleselect") {
                                $("#" + formContent.tableData[tableid_config].mainSubLine[i].subGridInput).combobox("setValues", window.parent.$("#" + formContent.tableData[tableid_config].mainSubLine[i].mainGridOutput).val().toString()).combo('readonly', true);//主表ID自动设到从表PID中
                            }
                            else if (QueryInput[j].editControl == "tree") {
                                $("#" + formContent.tableData[tableid_config].mainSubLine[i].subGridInput).combotree("setValue", window.parent.$("#" + formContent.tableData[tableid_config].mainSubLine[i].mainGridOutput).val()).combo('readonly', true);
                            }
                            break;
                        }
                    }
                }
            }
        }

        if (btcode_config == "Create" && tableid_config == 0) {
            if (parent.$("#TemplateID").length != 0 && parseInt(parent.$("#TemplateID").val()) % 2 == 0) {
                var treeData = parent.$("#StructTree").tree("getSelected");
                if (treeData != null) {
                    $("#" + formContent.treeData.treeBindData).val(treeData.id);
                    $("#" + formContent.treeData.treeBindData).combotree("setValues", treeData.id.toString());
                }
                else {
                    $.messageBoxBottom(0,"未选中任何树结点！");
                }
            }
            else if (parent.parent.$("#StructTree").length != 0) {//考虑左侧树右侧页面情况
                var treeData = parent.parent.$("#StructTree").tree("getSelected");
                if (treeData != null) {
                    var treeNodeCategory = parent.parent.formContent.treeData.pageWebData;
                    for (var i = 0; i < treeNodeCategory.length; i++) {
                        if ($("#" + treeNodeCategory[i].treeNode).length == 1 && treeNodeCategory[i].treeUrl.indexOf(formid_config) != -1) {
                            $("#" + treeNodeCategory[i].treeNode).val(treeData.id);
                            $("#" + treeNodeCategory[i].treeNode).combobox("setValue", treeData.id);
                            break;
                        }
                    }
                }
            }
        }
        else if (btcode_config == "Edit") {
            for (var field in PageContent) {//赋值
                if (PageContent[field]) {
                    PageContent[field] = PageContent[field].toString().replace(/&lt;\/script/ig, '<\/script').replace(/&lt;script/ig, '<script').replace(/script&gt;/ig, 'script>').replace(/&lt;/ig, '<').replace(/&gt;/ig, '>');
                    //解密存储型跨站脚本攻击
                }
                $("#" + field).val(PageContent[field]);
            }
            for (var i = 0; PageContent && i < QueryInput.length; i++) {
                if ((btcode_config == "Create" && (QueryInput[i].isAllowAdd == "H" || $.inArray(QueryInput[i].field, addPermission) != -1)) || //添加时动态加载隐藏控件
                    (btcode_config == "Edit" && (QueryInput[i].isAllowEdit == "H" || $.inArray(QueryInput[i].field, editPermission) != -1))) {//是隐藏控件
                    continue;
                }
                else if (QueryInput[i].editControl == "password") {//密码控件解密
                    $.ajaxSettings.async = false;
                    common.postGateway('platform/user/decryptpassword', {
                        "pswd": $("#" + QueryInput[i].field).val(),
                        "opt": "get"
                    }, common.getToken(), function (pswd) {
                        $("#" + QueryInput[i].field).val(pswd.json);
                    });
                    $.ajaxSettings.async = true;
                }
                else if (QueryInput[i].editControl == "radio") {//单选控件数据加载
                    $("input[name='" + QueryInput[i].field + "'][value='" + PageContent[QueryInput[i].field] + "']").prop("checked", true);
                }
                else if (QueryInput[i].editControl == "checkbox") {//多选控件数据加载
                    $("input[name='" + QueryInput[i].field + "']").prop("checked", false);
                    var id_value = PageContent[QueryInput[i].field].split(',');
                    for (var k = 0; k < id_value.length; k++) {
                        $("input[name='" + QueryInput[i].field + "'][value='" + id_value[k] + "']").prop("checked", true);
                    }
                }
                else if (QueryInput[i].editControl == "selected" || QueryInput[i].editControl == "multipleselect" ||
                    QueryInput[i].editControl == "orgRadioCtrl" || QueryInput[i].editControl == "orgCheckCtrl" || QueryInput[i].editControl == "openselect") {//下拉控件、多选下拉、组织选择、批量操作数据加载
                    $("#" + QueryInput[i].field).combobox("setValues", (PageContent[QueryInput[i].field] ? PageContent[QueryInput[i].field].toString() : []));
                }
                else if (QueryInput[i].editControl == "tree" || QueryInput[i].editControl == 'areaSelect') {//下拉树数据加载
                    $("#" + QueryInput[i].field).combotree("setValues", (PageContent[QueryInput[i].field] ? PageContent[QueryInput[i].field].toString() : ""));
                }
                else if (QueryInput[i].editControl == "logic") {//是否逻辑值控件
                    $("#" + QueryInput[i].field).switchbutton({
                        checked: (!PageContent ? true : (PageContent[QueryInput[i].field] == 1 || PageContent[QueryInput[i].field]=='true'))
                    });
                }
                else if (QueryInput[i].editControl == "picCtrl") {//图片控件
                    var value = PageContent[QueryInput[i].field];
                    $("#" + QueryInput[i].field).combo({ buttonIcon: value, prompt: '            点击更换图标' }).combo('setValue', value);
                    if (value) document.getElementById("Span_" + QueryInput[i].field).className = value;
                }
                else if (QueryInput[i].editControl == "datebox" && PageContent[QueryInput[i].field]) {//日期控件
                    $("#" + QueryInput[i].field).val(PageContent[QueryInput[i].field].split(' ')[0]);
                }
                else if (QueryInput[i].editControl == "editorSql") {//Sql编辑器
                    $.setValue(editorSql[QueryInput[i].field], PageContent[QueryInput[i].field]);
                }
            }
        }

        if (window.RunChildOnFinish && typeof (RunChildOnFinish) == 'function') {
            window.RunChildOnFinish();
        }
    }
    $("#btnSave,#btnSaveClose").click(function () {
        $.messageLoad();
        var ctrlID = this.id;
        SaveForm(function (result) {
            if (result.resMsg && result.resMsg != "") {
                $.messageBoxBottom(0, result.resMsg);
            }
            else if (result.errcode != 0 && result.errmsg) {
                $.messageBoxBottom(0, result.errmsg);
            }
            else {
                var msg = "";
                if (result.resCount <= 1 && result.resMsg == "") {
                    msg = "共成功操作" + result.resCount + "条记录！";
                    $.messageBoxBottom(1, msg);
                }
                else if (result.resCount > 1 && result.resMsg == "") {
                    msg = "操作成功，明细合计" + result.resCount + "条记录！";
                    $.messageBoxBottom(1, msg);
                }
                else {
                    msg = "微服务执行成功！";
                    $.messageBoxBottom(1, msg);
                }
                window.parent.frameReturnByReload($("#TemplateID").val() == "0" ? tableid_config : false);
                if (ctrlID == "btnSaveClose") {
                    window.parent.frameReturnByClose();
                    window.parent.$.messageBoxBottom(1, msg);
                }
                else if (btcode_config == "Create") {
                    for (var i = 0; i < QueryInput.length; i++) {
                        if (QueryInput[i].editControl != "radio" && QueryInput[i].editControl != "checkbox" && QueryInput[i].editControl != "selected" && QueryInput[i].editControl != "multipleselect" && QueryInput[i].editControl != "tree" && QueryInput[i].editControl != "logic"
                            && QueryInput[i].editControl != "orgRadioCtrl" && QueryInput[i].editControl != "orgCheckCtrl" && QueryInput[i].editControl != 'areaSelect' && QueryInput[i].isAllowAdd == "E") {
                            $("#" + QueryInput[i].field).val('');
                        }
                    }
                }
            }
            $.messageBoxCloseLoading();
        });
        return false;
    });

    $("#btnReturn").click(function () {
        window.parent.frameReturnByClose();
    });
});

function SaveOneRecord() {
    $.messageLoad();
    SaveForm(function (result) {
        if (result.resMsg && result.resMsg == "") {
            $.messageBoxBottom(1, "共成功操作" + result.resCount + "条记录！");
        }
        else if (result.resMsg) {
            $.messageBoxBottom(0, result.resMsg);
        }
        else if (result.errcode != 0) {
            $.messageBoxBottom(0, result.errmsg);
        }
        else {
            $.messageBoxBottom(1, "微服务执行成功！");
        }
        $.messageBoxCloseLoading();
    });
}
function SaveForm(fun) {
    if (window.RunCheckForm && typeof (RunCheckForm) == 'function') {
        var checkForm = RunCheckForm();
        if (checkForm == false) {
            $.messageBoxCloseLoading();
            return;
        }
    }
    var formJson = {};
    for (var i = 0; i < QueryInput.length; i++) {
        if (QueryInput[i].editControl == "logic" && QueryInput[i].isAllowAdd != 'H' && QueryInput[i].isAllowEdit != 'H') {
            formJson[QueryInput[i].field] = (new Boolean($("#" + QueryInput[i].field).switchbutton("options").checked)==true ? "1" : "0");
        }
        else if (QueryInput[i].editControl == "password") {//密码控件加密
            if ($("#" + QueryInput[i].field).val()) {
                $.ajaxSettings.async = false;
                common.postGateway('platform/user/decryptpassword', {
                    "pswd": $("#" + QueryInput[i].field).val(),
                    "opt": "set" 
                }, common.getToken(), function (pswd) {
                    formJson[QueryInput[i].field] = pswd.json;
                });
                $.ajaxSettings.async = true;
            }
            else {
                formJson[QueryInput[i].field] = "";
            }
        }
        else if (QueryInput[i].editControl == "radio" || QueryInput[i].editControl == "checkbox") {
            var chk_value = [];
            $("input[name='" + QueryInput[i].field + "']:checked").each(function () {
                chk_value.push($(this).val());
            });
            /*$(":checked[name='" + QueryInput[i].field + "']", $("#" + QueryInput[i].field)).each(function () {
                alert($(this).val() || "");
            });*/
            formJson[QueryInput[i].field] = chk_value.join(',');
        }
        else if (QueryInput[i].editControl == "number" && (!QueryInput[i].isRequired) && $("#" + QueryInput[i].field).val()=="") {
            formJson[QueryInput[i].field] = 0;
        }
        else if (QueryInput[i].editControl == "number" || QueryInput[i].controlType =="number" ) {
            formJson[QueryInput[i].field] = parseFloat($("#" + QueryInput[i].field).val());
        }
        else if (QueryInput[i].editControl == "html" && QueryInput[i].isAllowAdd != 'H' && QueryInput[i].isAllowEdit != 'H') {
            formJson[QueryInput[i].field] = UE.getEditor(QueryInput[i].field).getContent();
        }
        else if (QueryInput[i].editControl == "editorSql" && QueryInput[i].isAllowAdd != 'H' && QueryInput[i].isAllowEdit != 'H') {//Sql编辑器
            formJson[QueryInput[i].field] = editorSql[QueryInput[i].field].getValue();
        }
        else {
            //防存储型跨站脚本攻击
            formJson[QueryInput[i].field] = $("#" + QueryInput[i].field).val().replace(/<\/script/ig, '&lt;\/script').replace(/<script/ig, '&lt;script').replace(/script>/ig, 'script&gt;').replace(/</ig, '&lt;').replace(/>/ig, '&gt;');
        }
        if (QueryInput[i].editControl == "tree" && $("#" + QueryInput[i].field).val() == $("#" + QueryInput[0].field).val()) {
            $.messageBoxBottom(0,"检测到[" + QueryInput[i].title + "]和当前记录在同一个节点上，无法生成树型结构！");
            $("#" + QueryInput[i].field).focus();
            $.messageBoxCloseLoading();
            return;
        }

        if (QueryInput[i].dataValidate && QueryInput[i].dataValidate == "CheckOnly") {
            $.ajaxSettings.async = false;
            var checkOnly = false;
            var DataSource = "";
            if (formContent.tableData[tableid_config].tableName.indexOf("prod_") != 0) {
                DataSource = "&DataSource&";
            }
            apiRoute.selectDataEvent("select * from "+DataSource + formContent.tableData[tableid_config].tableName + " where " + formContent.tableData[tableid_config].tableIdField + "<>ifnull('" + formJson[formContent.tableData[tableid_config].tableIdField] + "','0') and " + QueryInput[i].field + "='" + formJson[QueryInput[i].field] + "'",
                 [], "1=1", function (result) {
                if (result.length != 0) {
                    $.messageBoxBottom(0,"检测到[" + QueryInput[i].title + "]在数据库中重复！");
                    $("#" + QueryInput[i].field).focus();
                    checkOnly = true;
                    $.messageBoxCloseLoading();
                    return;
                }
            });
            $.ajaxSettings.async = true;
            if (checkOnly) return;
        }
        else if (QueryInput[i].dataValidate && QueryInput[i].dataValidate == "CheckNodeOnly") {
            $.ajaxSettings.async = false;
            var checkOnly = false;
            var DataSource = "";
            if (formContent.tableData[tableid_config].tableName.indexOf("prod_") != 0) {
                DataSource = "&DataSource&";
            }
            apiRoute.selectDataEvent("select * from "+DataSource + formContent.tableData[tableid_config].tableName + " where " + formContent.tableData[tableid_config].tableIdField + "<>ifnull('" + formJson[formContent.tableData[tableid_config].tableIdField] + "','0') and " + QueryInput[i].field + "='" + formJson[QueryInput[i].field] + "' and p_id='" + $("#p_id").val() + "'",
                [], "1=1", function (result) {
                if (result.length != 0) {
                    $.messageBoxBottom(0,"检测到[" + QueryInput[i].title + "]在数据库该节点下重复！");
                    $("#" + QueryInput[i].field).focus();
                    checkOnly = true;
                    $.messageBoxCloseLoading();
                    return;
                }
            });
            $.ajaxSettings.async = true;
            if (checkOnly) return;
        }
        else if (QueryInput[i].dataValidate && QueryInput[i].dataValidate == "CheckViewOnly") {
            $.ajaxSettings.async = false;
            var checkOnly = false;
            apiRoute.selectDataEvent("select * from (" + formContent.tableData[tableid_config].tableSql + ") tb where " + formContent.tableData[tableid_config].tableIdField + "<>ifnull('" + formJson[formContent.tableData[tableid_config].tableIdField] + "','0') and " + QueryInput[i].field + "='" + formJson[QueryInput[i].field] + "'",
                [], "1=1", function (result) {
                    if (result.length != 0) {
                        $.messageBoxBottom(0, "检测到[" + QueryInput[i].title + "]在数据库指定视图下重复！");
                        $("#" + QueryInput[i].field).focus();
                        checkOnly = true;
                        $.messageBoxCloseLoading();
                        return;
                    }
                });
            $.ajaxSettings.async = true;
            if (checkOnly) return;
        }
        else if (QueryInput[i].dataValidate && QueryInput[i].dataValidate != "None" && $("#" + QueryInput[i].field).validatebox("isValid") == false) {
            $("#" + QueryInput[i].field).focus();
            $.messageBoxCloseLoading();
            return;
        }
        if (QueryInput[i].isRequired && formJson[QueryInput[i].field] == "") {
            $("#" + QueryInput[i].field).focus();
            $.messageBoxBottom(0,"[" + QueryInput[i].title + "]不能为空！");
            $.messageBoxCloseLoading();
            return;
        }
        var integer = /^-?[1-9]\d*$/;//整数
        if (QueryInput[i].dataValidate == "Integer" && (!integer.test(formJson[QueryInput[i].field]))) {
            $("#" + QueryInput[i].field).focus();
            $.messageBoxBottom(0, "[" + QueryInput[i].title + "]字段需要填入整数！");
            $.messageBoxCloseLoading();
            return;
        }
    }
    apiRoute.runTaskEvent(dataSourceCode, btnSql, [formJson], function (result) {
        var jsonLog = {
            "tableId": parseInt(tableid_config) + 1,
            "formid": common.getUrlParam("formid"),
            "btnText": window.parent.btnText || '保存',
            "json": JSON.stringify({
                "dataSourceCode": dataSourceCode,
                "formJson": JSON.stringify({
                    "dataSourceCode": dataSourceCode,
                    "btnSql": btnSql,
                    "formJson": [formJson]
                })
            })
        };
        common.postGateway("platform/menu/MenuLog", jsonLog, common.getToken());

        if (fun != null) {
            fun(result);
        }
    });
}

function fileUpload() {
    $('#dialogFileUpload').dialog('close');
    var serviceName = "platform/file/uploadfile";
    var serviceJson = { "uploadFormId": "uploadForm", fileDir: "upload" };
    var serviceTitle = { title: "上传", msg: "正在上传文件，请稍后……", "text": "文件上传中" };
    common.postFileGateway(serviceName, serviceJson, serviceTitle, common.getToken(), function (data) {
        $("#" + $("#fileid").val()).val(data.json);
    });
}
function filePreview(fileName) {//打开预览
    var srcs = getFileUrl(fileName);
    $("#modalwindow").html("<iframe width='100%' height='99%'  id='filemodalwindow' frameborder='0' src='" + srcs + "'></iframe>");
    $("#modalwindow").window({ title: '文件浏览', maximized: true, width: SetGridWidthSub(100), height: SetGridHeightSub(50), iconCls: 'mdi mdi-access-point' }).window('open');
}

function getFileUrl(url) {//获取预览路径
    var onlineViewType = ['doc', 'docx', 'xls', 'xlsx', 'xlsm', 'ppt', 'pptx'];
    var fileTypeName = url.substring(url.lastIndexOf('.') + 1, url.length).split('?')[0].toLowerCase();//获取文件类型
    var isWord = 0;
    for (var i = 0; i < onlineViewType.length; i++) {
        if (fileTypeName == onlineViewType[i]) {
            isWord = 0;
            break;
        } else {
            isWord = -1;
        }
    }
    var preUrl = window.location.protocol + "//" + window.location.hostname + "/";
    var isOther = false;
    var isPicture = false;
    if (isWord != -1) {//word文件
        url = preUrl + url;
        url = encodeURIComponent(url);
        url = window.location.protocol + '//docview.mingdao.com/op/view.aspx?src=' + url; // 明道

    } else if (fileTypeName == "pdf") {
        url = preUrl + url;
        url = encodeURIComponent(url);
        url = '/lib/pdf/web/viewer.html?file=' + url;
    } else if (fileTypeName == "jpg" || fileTypeName == "png" || fileTypeName == "jpeg" || fileTypeName == "gif" || fileTypeName == "tif") {
        url = preUrl + url;
        isPicture = true;
    } else {//zip rar 
        isOther = true;
        url = window.location.protocol + "//" + window.location.hostname + "/file/downfile?fileName=Cost%2F" + url;
    }
    return url;
}
function deleteFile() {
    if (!$("#" + $("#fileid").val()).val()) {
        $.messageBoxBottom(0,"对不起，需要删除的文件不存在！");
    }
    else {
        common.postGateway("platform/file/deletefile", { fileName: $("#" + $("#fileid").val()).val() },common.getToken(), function(data) {
            $.messageBoxBottom(1, data.json);
        });
    }
    $("#" + $("#fileid").val()).val("");
}

function gridButton(tableId, rowId, optCode) {
    var PageUrl, btText, btIcon;
    if (optCode == "detailRow") {
        pageUrl = "Page/PageDetail?Load=Complete&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId")+"&btCode=Detail&formid=" + common.getUrlParam("formid") + "&tableid=" + tableId;
        btText = "查看明细";
        btIcon = "mdi mdi-file-document-outline";
    }
    else if (optCode == "editRow") {
        pageUrl = "Page/PageForm/0?Load=Complete&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId")+"&btCode=Edit&formid=" + common.getUrlParam("formid") + "&tableid=" + tableId;
        btText = "修改";
        btIcon = "mdi mdi-pencil";
    }
    else if (optCode == "insertRow") {
        pageUrl = "Page/PageForm/0?Load=Complete&AppCode=" + common.getUrlParam("AppCode") + "&ClientId=" + common.getUrlParam("ClientId")+"&btCode=Create&formid=" + common.getUrlParam("formid") + "&tableid=" + tableId;
        btText = "添加";
        btIcon = "mdi mdi-plus";
    }
    else if (optCode == "deleteRow") {
        //var row = $('#List' + tableId).datagrid('getSelected');
        //var rowIndex = $('#List' + tableId).datagrid('getRowIndex', row);
        $.messageBoxConfirm('你要删除所选择的记录吗？', ['确定', '取消'], function () {
            $("#List" + tableId).datagrid(optCode, rowId);
            $("#List" + tableId).datagrid('load');
        });
        return;
    }
    $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='/" + pageUrl + "'></iframe>");
    $("#modalwindow").window({ title: btText, maximized: true, width: SetGridWidthSub(100), height: SetGridHeightSub(50), iconCls: btIcon }).window('open');
}

function mdi_Open(id) {
    $("#"+id).combo('hidePanel');
    $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='/lib/MaterialDesign-Webfont-master/preview.html?id="+id+"'></iframe>");
    $("#modalwindow").window({ title: '图标选择', width: SetGridWidthSub(200), height: SetGridHeightSub(100), iconCls: "mdi mdi-more" }).window('open');
}

function frameReturnByReload(tableId) {
    $("#List" + tableId).datagrid('load');
}