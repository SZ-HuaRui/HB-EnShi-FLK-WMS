$(function () {
    
    var formContent, tableid_config = common.getUrlParam("tableid"), btcode_config = common.getUrlParam("btCode");
    var load_config = common.getUrlParam("Load"), PageContent, formPermission;
    $('#conDiv').css({ 'height': SetGridHeightSub(66), 'overflow-y': 'auto' });
    $(window).resize(function () {
        $('#conDiv').css({ 'height': SetGridHeightSub(66), 'overflow-y': 'auto' });
    });
    var formid_config = common.getUrlParam("formid");
    if (load_config == "Complete") {
        formContent = window.parent.formContent.tableData[tableid_config];
        PageContent = window.parent.$('#List' + tableid_config).datagrid('getSelected');
        formPermission = window.parent.formPermission;
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
                $("title").html(webPageData.Table2[0].text);
                formContent = JSON.parse(common.Decrypt(webPageData.Table2[0].modulejson)).tableData[tableid_config];
                formPermission.buttonpermission = webPageData.Table3;//获取访问按钮权限
                formPermission.fieldpermission = webPageData.Table4;//获取访问字段权限
                var queryStr = formContent.tableIdField + "='" + common.getUrlParam("id")+"'";
                apiRoute.selectDataEvent(formContent.tableSql, [], queryStr, function (listData) {
                    PageContent = listData[0];
                });
                $("#btnReturn").hide();
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

    if (btcode_config == "Detail") {
        var fieldPermission = [];
        for (var m = 0; formPermission && m < formPermission.fieldpermission.length; m++) {
            if (formContent.tableName == formPermission.fieldpermission[m].table_name && formPermission.fieldpermission[m].is_deny_view == 1) {
                fieldPermission.push(formPermission.fieldpermission[m].field_name);
            }
        }

        //性能优化数据字典相关数据查询
        var codeDictionary = {}, pageDictionary = {},isDictionary = false;
        for (var n = 0; n < formContent.fieldData.length; n++) {
            if (formContent.fieldData[n].editControl == "radio" || formContent.fieldData[n].editControl == "checkbox" ||
                formContent.fieldData[n].editControl == "selected" || formContent.fieldData[n].editControl == "multipleselect" ||
                formContent.fieldData[n].editControl == "tree") {
                isDictionary = true;
                codeDictionary[formContent.fieldData[n].field] = common.Encrypt(formContent.fieldData[n].dataSource);
            }
        }
        if (isDictionary) {
            $.ajaxSettings.async = false;
            apiRoute.pageDictionary(codeDictionary, function (tableData) {
                pageDictionary = tableData;
            });
            $.ajaxSettings.async = true;
        }
        if (formContent.multipleColumn == "" || formContent.multipleColumn == "1") {
            $("#multipleColumn").html('<tr><th width="40%"></th><td width="60%"></td></tr>');
            var inputhtml = "";
            for (var i = 0; i < formContent.fieldData.length; i++) {  
                var content = PageContent[formContent.fieldData[i].field];
                if (formContent.fieldData[i].editControl == "radio" || formContent.fieldData[i].editControl == "checkbox" ||
                    formContent.fieldData[i].editControl == "selected" || formContent.fieldData[i].editControl == "multipleselect" ||
                    formContent.fieldData[i].editControl == "tree") {

                    var tableData = pageDictionary[formContent.fieldData[i].field]
                    for (var j = 0; j < tableData.length; j++) {
                        if (content == tableData[j].id) {
                            content = tableData[j].text;
                            break;
                        }
                    }
                }
                
                if (formContent.fieldData[i] && formContent.fieldData[i].isAllowShow == "S" && $.inArray(formContent.fieldData[i].field, fieldPermission) == -1) {
                    inputhtml += "<tr><th><b>" + formContent.fieldData[i].title + "：</b></th>";
                    if (formContent.fieldData[i].style == "IconFormatter") {
                        inputhtml += "<td><span class=\"" + content + "\"></span></td></tr>";
                    }
                    else if (formContent.fieldData[i].editControl == "texteara") {
                        inputhtml += "<td>" + common.HTMLEncode(content) + "</td>";
                    }
                    else {
                        inputhtml += "<td>" + content + "</td>";
                    }
                }
            }
            $("table tbody tr").eq(0).after(inputhtml);
        }
        else if (formContent.multipleColumn == "2") {
            $("#multipleColumn").html('<tr><th width="20%"></th><td width="30%"></td><th width="20%"></th><td width="30%"></td></tr>');
            var inputhtml = "", form_n = 0;
            for (var i = 0; i < formContent.fieldData.length; i++) {
                var content = PageContent[formContent.fieldData[i].field];
                if (formContent.fieldData[i].editControl == "radio" || formContent.fieldData[i].editControl == "checkbox" ||
                    formContent.fieldData[i].editControl == "selected" || formContent.fieldData[i].editControl == "multipleselect" ||
                    formContent.fieldData[i].editControl == "tree") {

                    var tableData = pageDictionary[formContent.fieldData[i].field]
                    for (var j = 0; j < tableData.length; j++) {
                        if (content == tableData[j].id) {
                            content = tableData[j].text;
                            break;
                        }
                    }
                }

                if (formContent.fieldData[i].isAllowShow == "S" && $.inArray(formContent.fieldData[i].field, fieldPermission) == -1) {
                    if (form_n % 2 == 0) inputhtml += "<tr>"
                    inputhtml += "<th><b>" + formContent.fieldData[i].title + "：</b></th>";
                    if (formContent.fieldData[i].style == "IconFormatter") {
                        inputhtml += "<td><span class=\"" + content + "\"></span></td>";
                    }
                    else if (formContent.fieldData[i].editControl == "texteara") {
                        inputhtml += "<td>" + common.HTMLEncode(content) + "</td>";
                    }
                    else {
                        inputhtml += "<td>" + content + "</td>";
                    }
                    if (form_n % 2 == 1) inputhtml += "</tr>"
                    form_n++;
                }
            }
            if (form_n % 2 == 1) inputhtml += "<th></th><td></td></tr>"
            $("table tbody tr").eq(0).after(inputhtml);
        }
        else if (formContent.multipleColumn == "3") {
            $("#multipleColumn").html('<tr><th width="13%"></th><td width="20%"></td><th width="14%"></th><td width="20%"></td><th width="13%"></th><td width="20%"></td></tr>');
            var inputhtml = "", form_n = 0;
            for (var i = 0; i < formContent.fieldData.length; i++) {
                var content = PageContent[formContent.fieldData[i].field];
                if (formContent.fieldData[i].editControl == "radio" || formContent.fieldData[i].editControl == "checkbox" ||
                    formContent.fieldData[i].editControl == "selected" || formContent.fieldData[i].editControl == "multipleselect" ||
                    formContent.fieldData[i].editControl == "tree") {

                    var tableData = pageDictionary[formContent.fieldData[i].field]
                    for (var j = 0; j < tableData.length; j++) {
                        if (content == tableData[j].id) {
                            content = tableData[j].text;
                            break;
                        }
                    }
                }

                if (formContent.fieldData[i].isAllowShow == "S" && $.inArray(formContent.fieldData[i].field, fieldPermission) == -1) {
                    if (form_n % 3 == 0) inputhtml += "<tr>"
                    inputhtml += "<th><b>" + formContent.fieldData[i].title + "：</b></th>";
                    if (formContent.fieldData[i].style == "IconFormatter") {
                        inputhtml += "<td><span class=\"" + content + "\"></span></td>";
                    }
                    else if (formContent.fieldData[i].editControl == "texteara"){
                        inputhtml += "<td>" + common.HTMLEncode(content) + "</td>";
                    }
                    else {
                        inputhtml += "<td>" + content + "</td>";
                    }
                    if (form_n % 3 == 2) inputhtml += "</tr>"
                    form_n++;
                }
            }
            if (form_n % 3 == 1) inputhtml += "<th></th><td></td><th></th><td></td></tr>"
            if (form_n % 3 == 2) inputhtml += "<th></th><td></td></tr>"
            $("table tbody tr").eq(0).after(inputhtml);
        }
        $("table tbody td").css({ "word-break": "break-all", "padding": "0 12px" });

        if (window.RunDetailOnFinish && typeof (RunDetailOnFinish) == 'function') {
            window.RunDetailOnFinish();
        }

        var userAgent = navigator.userAgent;
        if (userAgent.indexOf("DingTalk") != -1) {//通过钉钉打开
            var titleContent = this.title;
            var urlContent = common.Encrypt(window.location.toString().replace(window.location.protocol + "//" + window.location.hostname, ""));
            var app_id = 264781010, corpid = "ding5033bbcf5247e61b35c2f4657eb6378f";
            if (common.getUrlParam("corpid") != null) corpid = common.getUrlParam("corpid");
            var dingUrl = "dingtalk://dingtalkclient/action/openapp?corpid=" + corpid + "&container_type=work_platform&app_id=0_" + app_id + "&redirect_type=jump&redirect_url=" + window.location.protocol.replace(":", "") + "%3a%2f%2f" + window.location.host + "%2fdingding%2finterface%2f" + urlContent + "%3fcorpid%3d" + corpid;
            dd.biz.navigation.setRight({
                show: true,//控制按钮显示， true 显示， false 隐藏， 默认true
                control: true,//是否控制点击事件，true 控制，false 不控制， 默认false
                text: '分享',//控制显示文本，空字符串表示显示默认文本
                onSuccess: function (result) {
                    dd.biz.util.share({
                        type: 1,//分享类型，0:全部组件 默认；1:只能分享到钉钉；2:不能分享，只有刷新按钮
                        url: dingUrl,
                        title: titleContent,
                        content: "欢迎访问" + titleContent,
                        image: "https://cloud.nti56.com/img/wechat_qr.png",
                        onSuccess: function () {
                            //onSuccess将在调起分享组件成功之后回调
                            /**/
                        },
                        onFail: function (err) { }
                    })
                },
                onFail: function (err) { }
            });
        }
    }

    $("#btnReturn").click(function () {
        window.parent.frameReturnByClose();
    });
});