var listData;
$(function () {
    var formid_config = common.getUrlParam("id");
    var sql = "select * from  prod_rf_config where t_id=" + formid_config;
    $.ajaxSettings.async = false;
    apiRoute.selectDataEvent(sql, [], "1=1", function (data) {
        $('#rftitle').text(data[0].rfname);
        if (!data[0].rfjson) { return false }
        listData = JSON.parse(data[0].rfjson);    
        })
    console.log(listData);
    $.ajaxSettings.async = true;
    if (!listData) { return false }
    if (listData.list.length > 0) {        
        for (var i = 0; i < listData.list.length; i++) {
            var htmlDom = '';
            if (listData.list[i].type == 'select') {
                htmlDom += '<p>';
                if (listData.list[i].options.required == true) {
                    htmlDom += '<span style="color:red;" class="required">*</span>';
                }
                htmlDom += '<label for=' + listData.list[i].name + '>' + listData.list[i].name + '：</label>';
                htmlDom += '<select id="' + listData.list[i].id + '"';
                if (listData.list[i].options.width != '') {
                    if(listData.list[i].options.width.indexOf('%') > 0 ||listData.list[i].options.width.indexOf('px') > 0 ){
                        htmlDom += ' style="width:' + listData.list[i].options.width + '"';
                    }else {
                        htmlDom += ' style="width:' + listData.list[i].options.width + 'px"';
                    }
                }
                var isDisabled = listData.list[i].options.disabled == true ? 'disabled="disabled"' : '';
                htmlDom += ' ' + isDisabled +'"></select></p>';
                $('.rf_content').append(htmlDom);
                getDataSelect(listData.list[i]);
            } else if (listData.list[i].type == 'input') {
                htmlDom += '<p>';
                if (listData.list[i].options.required == true) {
                    htmlDom += '<span style="color:red;" class="required">*</span>';
                }
                htmlDom += '<label for=' + listData.list[i].name + '>' + listData.list[i].name + '：</label><input id="' + listData.list[i].id + '" type="text"';
                if (listData.list[i].options.width != '') {
                    if(listData.list[i].options.width.indexOf('%') > 0 ||listData.list[i].options.width.indexOf('px') > 0 ){
                        htmlDom += ' style="width:' + listData.list[i].options.width + '"';
                    }else {
                        htmlDom += ' style="width:' + listData.list[i].options.width + 'px"';
                    }
                }
                var isPlaceholder=listData.list[i].options.placeholder != '' ? listData.list[i].options.placeholder:'';
                var isDefaultValue = listData.list[i].options.defaultValue != '' ? listData.list[i].options.defaultValue : '';
                var isScan = listData.list[i].options.isScan == true ? '<span class="scanCode mdi mdi-barcode-scan" onclick="scanQRCode(this)"></span>' : '';
                htmlDom += ' placeholder="' + isPlaceholder + '" value="' + isDefaultValue + '">'+isScan+'</p>';
                $('.rf_content').append(htmlDom);
                if(listData.list[i].dataBind!=''|| listData.list[i].dataBindPara!=''){
                    var listArrr=listData.list[i];
                    $("#" + listArrr.id).keydown(listArrr, function (event) {
                        if (event.which == 13) {
                            var inputVal = $('#'+listArrr.id).val();
                            if (inputVal != '' && inputVal != null) {
                                getNextData(listArrr.type,listArrr.dataBind, listArrr.dataBindPara, inputVal)
                            } else {
                                alert(listArrr.name+"不能为空！！！");
                            }
                        }
                    });
                }
            }else if (listData.list[i].type == 'textarea') {
                htmlDom += '<p>';
                if (listData.list[i].options.required == true) {
                    htmlDom += '<span style="color:red;" class="required">*</span>';
                }
                htmlDom += '<label for=' + listData.list[i].name + '>' + listData.list[i].name + '：</label><textarea id="' + listData.list[i].id + '"';
                if (listData.list[i].options.width != '') {
                    if(listData.list[i].options.width.indexOf('%') > 0 ||listData.list[i].options.width.indexOf('px') > 0 ){
                        htmlDom += ' style="width:' + listData.list[i].options.width + '"';
                    }else {
                        htmlDom += ' style="width:' + listData.list[i].options.width + 'px"';
                    }                    
                }
                var isPlaceholder=listData.list[i].options.placeholder != '' ? listData.list[i].options.placeholder:'';
                var isDefaultValue = listData.list[i].options.defaultValue != '' ? listData.list[i].options.defaultValue : '';
                var isDisabled = listData.list[i].options.disabled == true ? 'disabled="disabled"' : '';
                htmlDom += ' placeholder="' + isPlaceholder + '" ' + isDisabled +'">' + isDefaultValue + '</textarea></p>';     
                $('.rf_content').append(htmlDom);           
            }else if (listData.list[i].type == 'radio') {
                htmlDom += '<p>';
                if (listData.list[i].options.required == true) {
                    htmlDom += '<span style="color:red;" class="required">*</span>';
                }
                htmlDom += '<label for=' + listData.list[i].name + '>' + listData.list[i].name + '：</label>';
                htmlDom += '<span id="' + listData.list[i].id + '" style="';
                if (listData.list[i].options.width != '') {
                    if(listData.list[i].options.width.indexOf('%') > 0 ||listData.list[i].options.width.indexOf('px') > 0 ){
                        htmlDom += 'width:' + listData.list[i].options.width + '';
                    }else {
                        htmlDom += 'width:' + listData.list[i].options.width + 'px;';
                    }
                }
                var isBlock=listData.list[i].options.inline == false?'block':'';                                
                htmlDom += 'display:' + isBlock + '">';  


                //if (listData.list[i].options.options.length > 0) {
                //    for (var j = 0; j < listData.list[i].options.options.length; j++) {
                //        var isDefault=listData.list[i].options.defaultValue == listData.list[i].options.options[j].value?'checked':'';
                //        htmlDom += '<label style="display:'+isBlock+'" for="' + listData.list[i].options.options[j].value + '"><input name="' + listData.list[i].name + '" '+isDefault+' type="radio" value="' + listData.list[i].options.options[j].value + '" />' + listData.list[i].options.options[j].value + '</label>';
                //    }
                //}


                htmlDom += '</span></p>';
                $('.rf_content').append(htmlDom);
                getDataSelect(listData.list[i]);
            }else if (listData.list[i].type == 'checkbox') {
                htmlDom += '<p>';
                if (listData.list[i].options.required == true) {
                    htmlDom += '<span style="color:red;" class="required">*</span>';
                }
                htmlDom += '<label for=' + listData.list[i].name + '>' + listData.list[i].name + '：</label>';
                htmlDom += '<span id="' + listData.list[i].id + '" style="';
                if (listData.list[i].options.width != '') {
                    if(listData.list[i].options.width.indexOf('%') > 0 ||listData.list[i].options.width.indexOf('px') > 0 ){
                        htmlDom += 'width:' + listData.list[i].options.width + '';
                    }else {
                        htmlDom += 'width:' + listData.list[i].options.width + 'px;';
                    }
                }
                var isBlock=listData.list[i].options.inline == false?'block':'';                                
                htmlDom += 'display:' + isBlock + '">';   


                //if (listData.list[i].options.options.length > 0) {                    
                //    for (var j = 0; j < listData.list[i].options.options.length; j++) {    
                //        var isEqual=0;
                //        for(var h = 0; h < listData.list[i].options.defaultValue.length; h++){  
                //            if(listData.list[i].options.defaultValue[h] == listData.list[i].options.options[j].value){
                //                htmlDom += '<label style="display:'+isBlock+'" for="' + listData.list[i].options.options[j].value + '"><input name="' + listData.list[i].name + '" checked type="checkbox" value="' + listData.list[i].options.options[j].value + '" />' + listData.list[i].options.options[j].value + '</label>';
                //                isEqual=1;
                //            }                            
                //        }
                //        if(isEqual==0){
                //            htmlDom += '<label style="display:'+isBlock+'" for="' + listData.list[i].options.options[j].value + '"><input name="' + listData.list[i].name + '"  type="checkbox" value="' + listData.list[i].options.options[j].value + '" />' + listData.list[i].options.options[j].value + '</label>';
                            
                //        }
                //    }
                //}


                htmlDom += '</span></p>';
                $('.rf_content').append(htmlDom);
                getDataSelect(listData.list[i]);
            } else if (listData.list[i].type == 'text') {
                htmlDom += '<p>';
                if (listData.list[i].options.required == true) {
                    htmlDom += '<span style="color:red;" class="required">*</span>';
                }
                htmlDom += '<label for=' + listData.list[i].name + '>' + listData.list[i].name + '：</label><span id="' + listData.list[i].id + '"';
                if (listData.list[i].options.width != '') {
                    if(listData.list[i].options.width.indexOf('%') > 0 ||listData.list[i].options.width.indexOf('px') > 0 ){
                        htmlDom += ' style="width:' + listData.list[i].options.width + '"';
                    }else {
                        htmlDom += ' style="width:' + listData.list[i].options.width + 'px"';
                    }
                }
                var isDefaultValue=listData.list[i].options.defaultValue != ''?listData.list[i].options.defaultValue:'';
                htmlDom += '">'+isDefaultValue+'</span></p>';
                $('.rf_content').append(htmlDom);
            } else if (listData.list[i].type == 'button') {
                htmlDom += '<a id="' + listData.list[i].id + '"';
                if (listData.list[i].options.width != '') {
                    if(listData.list[i].options.width.indexOf('%') > 0 ||listData.list[i].options.width.indexOf('px') > 0 ){
                        htmlDom += ' style="width:' + listData.list[i].options.width + '"';
                    }else {
                        htmlDom += ' style="width:' + listData.list[i].options.width + 'px"';
                    }
                }
                htmlDom += '">'+listData.list[i].name+'</a>';
                $('.button_con').append(htmlDom);
                buttonAction(listData.list[i]);
            }
        }
    }

    $(".rf_home").click(function () {
        location.href = "/RF/Main/RFMain?AppCode=" + common.getUrlParam("AppCode");
    });
    $(".rf_out").click(function () {
        location.href = "/RF/Login?AppCode=" + common.getUrlParam("AppCode");
    });
})

//微服务数据获取
function GetMicroservices(type, dataBind, dataBindPara, fun) {
    //$.ajaxSettings.async = false;
    var json = JSON.parse(dataBindPara);
    if (json.sql) {
        json.sql = json.sql;
    }
    apiRoute.selectDataEvent(json.sql, [], "1=1", function (data) {
        if (fun != null) fun(data);
    });
    //$.ajaxSettings.async = true;
}

//下列类型数据绑定
function getDataSelect(list) {
    //alert(JSON.stringify(list));
    if (!list.id) { return false }
    $("#" + list.id).empty(); 
    var isBlock = list.options.inline == false ? 'block' : ''; 
    //if (list.options.remote == false) {
    if (!list.options.props.value) {
        if (list.type == 'select') {
            for (var i = 0; i < list.options.options.length; i++) {
                var label = list.options.options[i].label ? list.options.options[i].label : list.options.options[i].value;
                $("#" + list.id).append("<option value='" + list.options.options[i].value + "'>" + label + "</option>");
            }
            if ($("#" + list.id + " option").length == "0") {
                $("#" + list.id).append("<option value='0'>==请选择==</option>");
            }
        } else if (list.type == 'radio') {
            for (var i = 0; i < list.options.options.length; i++) {
                var isDefault = list.options.defaultValue == list.options.options[i].value ? 'checked' : '';
                var label = list.options.options[i].label == "" ? list.options.options[i].value : list.options.options[i].label;
                $("#" + list.id).append('<label style="display: ' + isBlock + '" for="' + list.options.options[i].value + '"><input id="' + list.options.options[i].value + '" name="' + list.id + '" ' + isDefault + ' type="radio" value="' + list.options.options[i].value + '" />' + label + '</label>');
            }
        } else if (list.type == 'checkbox') {
            for (var i = 0; i < list.options.options.length; i++) {
                var isEqual = 0;
                var label = list.options.options[i].label ? list.options.options[i].label : list.options.options[i].value;
                for (var h = 0; h < list.options.defaultValue.length; h++) {
                    if (list.options.defaultValue[h] == list.options.options[i].value) {
                        $("#" + list.id).append('<label style="display:' + isBlock + '" for="' + list.options.options[i].value + '"><input id="' + list.options.options[i].value + '" name="' + list.id + '" checked type="checkbox" value="' + list.options.options[i].value + '" />' + label + '</label>');
                        isEqual = 1;
                    }
                }
                if (isEqual == 0) {
                    $("#" + list.id).append('<label style="display:' + isBlock + '" for="' + list.options.options[i].value + '"><input id="' + list.options.options[i].value + '" name="' + list.id + '"  type="checkbox" value="' + list.options.options[i].value + '" />' + label + '</label>');
                }
            }
        }        
    }
    else {
        GetMicroservices(list.type, list.options.remoteFunc, list.options.props.value, function (data) {
            if (list.type == 'select') {
                for (var i = 0; i < data.length; i++) {
                    $("#" + list.id).append("<option value='" + data[i].id + "'>" + data[i].text + "</option>");
                }
                if ($("#" + list.id + " option").length == "0") {
                    $("#" + list.id).append("<option value='0'>==请选择==</option>");
                }
            } else if (list.type == 'radio') {
                for (var i = 0; i < data.length; i++) {
                    $("#" + list.id).append('<label style="display: ' + isBlock + '" for="' + data[i].id + '"><input id="' + data[i].id + '" name="' + list.id + '" type="radio" value="' + data[i].id + '" />' + data[i].text + '</label>');
                }
            } else if (list.type == 'checkbox') {
                for (var i = 0; i < data.length; i++) {
                    $("#" + list.id).append('<label style="display:' + isBlock + '" for="' + data[i].id + '"><input id="' + data[i].id + '" name="' + list.id + '"  type="checkbox" value="' + data[i].id + '" />' + data[i].text + '</label>');
                }
            }            
        });
    }
    if (list.options.props.label) { //联动其它控件        
        if (list.type == 'radio') {
            $('body').on('click', "input:radio[name='" + list.id + "']", function () {
                for (var n = 0; n < listData.list.length; n++) {
                    if (listData.list[n].id == list.options.props.label) {
                        var json = JSON.parse(JSON.stringify(listData.list[n]));
                        json.options.props.value = listData.list[n].options.props.value.replace('@' + list.id, "'" + $(this).val() + "'");
                        if (listData.list[n].type == 'select') {
                            getDataSelect(json);
                        }                        
                        break;
                    }
                }
            })
        } else if (list.type == 'select') {
            $("#" + list.id).off('change').on('change', function () {
                for (var n = 0; n < listData.list.length; n++) {
                    if (listData.list[n].id == list.options.props.label) {
                        var json = JSON.parse(JSON.stringify(listData.list[n]));
                        json.options.props.value = listData.list[n].options.props.value.replace('@' + list.id, "'" + $('#' + list.id).val() + "'");
                        if (listData.list[n].type == 'select') {
                            getDataSelect(json);
                        }                        
                        break;
                    }
                }
            });
        }
    }
    if (list.dataBind) { //联动数据              
        if (list.type == 'radio') {
            $('body').on('click', "input:radio[name='" + list.id + "']", function () {
                getNextData(list.type, list.dataBind, list.dataBindPara, $(this).val());
            })
        }
        else if (list.type == 'select') {
            $("#" + list.id).off('change').on('change', function () {
                getNextData(list.type, list.dataBind, list.dataBindPara, $(this).val());
            });
        }
    }
}
//按钮绑定事件
function buttonAction(list) {
    if (!list.id) { return false }
    $("#" + list.id).off('click').on('click', function () {
        var jsonData = {},jsonAll=[];
        if (listData.list.length > 0) {
            for (var i = 0; i < listData.list.length; i++) {
                if (listData.list[i].type == 'select' || listData.list[i].type == 'textarea' || listData.list[i].type == 'input') {
                    jsonData[listData.list[i].id] = $('#' + listData.list[i].id).val();
                }  else if (listData.list[i].type == 'radio') {
                    jsonData[listData.list[i].id] = $("input[name='" + listData.list[i].id +"']:checked").val();
                } else if (listData.list[i].type == 'checkbox') {
                    var checkboxAarr = [];
                    $("input[name='" + listData.list[i].id +"']:checked").each(function (i) {
                        checkboxAarr[i] = $(this).val();
                    });
                    var vals = checkboxAarr.join(",");
                    jsonData[listData.list[i].id] = vals;
                } else if (listData.list[i].type == 'text') {
                    jsonData[listData.list[i].id] = $('#' + listData.list[i].id).text();
                }
            }
        }
        jsonAll[0] = jsonData;
        //console.log(list)
        if (!list.dataSourse || !list.dataSoursePara) return;
        var json = JSON.parse(list.dataSoursePara);
        for (var key in json) {
            if (key == "sql") {
                var jsonSql = common.Encrypt(json[key]);
                json[key] = jsonSql;
            } else if (json[key] == "jsonData") {
                json[key] = jsonAll;
            } else {
                for (var i = 0; i < listData.list.length; i++) {
                    if (key == listData.list[i].id) {
                        if (listData.list[i].type == 'select' || listData.list[i].type == 'textarea' || listData.list[i].type == 'input') {
                            json[key] = $('#' + listData.list[i].id).val();
                        } else if (listData.list[i].type == 'radio') {
                            json[key] = $("input[name='" + listData.list[i].id + "']:checked").val();
                        } else if (listData.list[i].type == 'checkbox') {
                            var checkboxAarr = [];
                            $("input[name='" + listData.list[i].id + "']:checked").each(function (i) {
                                checkboxAarr[i] = $(this).val();
                            });
                            var vals = checkboxAarr.join(",");
                            json[key] = vals;
                        } else if (listData.list[i].type == 'text') {
                            json[key] = $('#' + listData.list[i].id).text();
                        }

                    }
                }

            }
        }
        
        //for (var key in json) {
        //    if (json[key] == "jsonData") {
        //        json[key] = jsonAll;
        //    }
        //}        
        //console.log("json", json);
        common.postGateway(list.dataSourse, json, common.getToken(), function (data) {
            //console.log(data)
            if (data.errcode == 0) {
                alert(list.name + "成功！！！");
                if (list.url) {
                    location.href = list.url;
                }

            } else {
                alert(data.errmsg);
            }
        })
        //apiRoute.runTaskEvent("",json.sql, JSON.stringify(jsonAll), function (data) {
        //    console.log(data)
        //});
    })
    
    
}

//联动数据
function getNextData(type, dataBind, dataBindPara, lastVal) {
    //$.ajaxSettings.async = false;
    GetMicroservices(type,dataBind, dataBindPara.replace(/\@S0X/g, "'" + lastVal + "'"), function (data) {
        if (data.length > 0) {
            for (var key in data[0]) {
                if ($("#" + key).length > 0) {
                    $("#" + key).val(data[0][key]);
                    $("#" + key).text(data[0][key]);
                }
            }
        }
    });
    //$.ajaxSettings.async = true;
}

function scanQRCode(obj) {
    dd.ready(function () {
        dd.biz.util.scan({
            type: "all", // type 为 all、qrCode、barCode，默认是all。
            onSuccess: function (data) {
                //onSuccess将在扫码成功之后回调
                /* data结构
                  { 'text': String}
                */
                //alert(JSON.stringify(data))
                $(obj).prev().val(data.text);
            },
            onFail: function (err) {
                alert(JSON.stringify(err))
            }
        })
    });
}