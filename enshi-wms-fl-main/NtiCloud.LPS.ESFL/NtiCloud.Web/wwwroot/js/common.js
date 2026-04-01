var common = (function () {
    return {
        _KEY: "abcdefghijklmnopqrstuvwxyz012345",//32位
        _IV: "9876543210abcdef",//16位
        /**************************************************************
        *字符串加密
        *   str：需要加密的字符串
        ****************************************************************/
        Encrypt: function (str, userkey) {
            var key = CryptoJS.enc.Utf8.parse(this._KEY);
            var iv = CryptoJS.enc.Utf8.parse(this._IV);
            if (userkey) {
                key = CryptoJS.enc.Utf8.parse(userkey);
            }

            var encrypted = '';

            var srcs = CryptoJS.enc.Utf8.parse(str);
            encrypted = CryptoJS.AES.encrypt(srcs, key, {
                iv: iv,
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            });

            return encrypted.ciphertext.toString();
        },

        /**************************************************************
        *字符串解密
        *   str：需要解密的字符串
        ****************************************************************/
        Decrypt: function (str, userkey) {
            var key = CryptoJS.enc.Utf8.parse(this._KEY);
            var iv = CryptoJS.enc.Utf8.parse(this._IV);
            if (userkey) {
                key = CryptoJS.enc.Utf8.parse(userkey);
            }
            var encryptedHexStr = CryptoJS.enc.Hex.parse(str);
            var srcs = CryptoJS.enc.Base64.stringify(encryptedHexStr);
            var decrypt = CryptoJS.AES.decrypt(srcs, key, {
                iv: iv,
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            });
            try {
                var decryptedStr = decrypt.toString(CryptoJS.enc.Utf8);
                return decryptedStr.toString();
            }
            catch (e) {
                return "";
            }
        },
        newGuid: function () {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        },
        gatewayUrl: function () {
            var url = window.location.protocol + "//" + window.location.hostname + ":9080/";
            return url;
        },
        getToken: function (json) {
            var josnToken = {
                "sessionKey": ((this.getCookie("sessionKey") == null || this.getCookie("sessionKey") == "") ? "noLogin" : this.getCookie("sessionKey")),
                "appCode": (this.getUrlParam("AppCode") == null ? "" : this.getUrlParam("AppCode")),
                "clientId": (this.getUrlParam("ClientId") == null ? "" : this.getUrlParam("ClientId")),
                "localLanguage": ((this.getCookie("localLanguage") == null || this.getCookie("localLanguage") == "") ? "zh-CN" : this.getCookie("localLanguage"))
            };
            if (this.getCookie("warehouseId") != null && this.getCookie("warehouseId") != "") {
                josnToken.warehouseId = this.getCookie("warehouseId");
            }
            for (var key in json) {
                josnToken[key] = json[key];
            }
            if (josnToken.sessionKey == "noLogin")
                return null;
            else
                return josnToken;
        },
        postToken: function (serviceToken, serviceFun) {
            $.ajax({
                type: "post",
                async: false,  //同步请求
                url: common.gatewayUrl() + 'api/oauth2/token',
                dataType: 'json',
                data: JSON.stringify(serviceToken),
                contentType: 'application/json; charset=UTF-8',
                success: function (data) {
                    if (data.isSucceed == true && data.errcode == 0) {
                        sessionStorage.setItem("Authorization", data.json);
                    }
                    if (serviceFun != null) {
                        serviceFun(data);
                    }
                }
            });
        },
        postGateway: function (serviceName, serviceJson, serviceToken, serviceFun) {
            if ((serviceToken != null && serviceToken.hasOwnProperty("sessionKey") && serviceToken.sessionKey != "noLogin" && serviceToken.sessionKey != "") &&
                (sessionStorage.getItem("Authorization") == null || sessionStorage.getItem("token") == null || JSON.stringify(serviceToken) != this.Decrypt(sessionStorage.getItem("token"), serviceToken.sessionKey))) {
                sessionStorage.setItem("token", this.Encrypt(JSON.stringify(serviceToken), serviceToken.sessionKey));
                this.postToken(serviceToken, function (data) {
                    if (data.errcode == 401) {
                        if (serviceName != "platform/user/gettoken") {
                            window.sessionStorage.removeItem("Authorization");
                            location.reload();
                            $.messageBoxAlert(0, "用户鉴权认证失败！" + data.Message);
                        }
                    }
                    else {
                        common.postApiGateway(serviceName, serviceJson, serviceToken, serviceFun, "application/json");
                    }
                });
            }
            else {
                this.postApiGateway(serviceName, serviceJson, serviceToken, serviceFun, "application/json");
            }
        },
        postApiGateway: function (serviceName, serviceJson, serviceToken, serviceFun, contentType) {
            var postData = serviceJson;
            if (contentType == "application/json") postData = JSON.stringify(serviceJson)
            $.ajax({
                url: common.gatewayUrl() + 'api/' + serviceName,
                type: 'post',
                contentType: contentType + "; charset=UTF-8",
                //async: true,
                dataType: 'json',
                data: postData,
                beforeSend: function (request) {
                    request.setRequestHeader("Authorization", sessionStorage.getItem("Authorization"));
                },
                success: function (response) {
                    if (response.isSucceed == true && (response.errcode == 0 || response.errcode == 200)) {
                        if (serviceFun != null) serviceFun(response);
                    }
                    else if (contentType == "application/json" &&response.errcode == 401 && serviceToken != null && serviceName != "oauth2/token" && serviceToken.hasOwnProperty("sessionKey") && serviceToken.sessionKey != "noLogin" && serviceToken.sessionKey != "") {
                        window.sessionStorage.removeItem("Authorization");
                        window.sessionStorage.removeItem("token");
                        common.postGateway(serviceName, serviceJson, serviceToken, serviceFun);
                    }
                    else if ((response.errcode == 401 || serviceName == "platform/user/gettoken") && serviceFun != null) {
                        serviceFun(response);
                    }
                    else {
                        $.messageBoxAlert(0, "调用" + serviceName + "服务出错！" + response.errmsg);
                        if (serviceFun != null) serviceFun(response);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    if (contentType == "application/json" && XMLHttpRequest.readyState == 0 && XMLHttpRequest.statusText == "error") {//防止网络字符过滤，连接被断开
                        common.postApiGateway(serviceName, { NtiCloudEncryptKey: common.Encrypt(JSON.stringify(serviceJson)) }, serviceToken, serviceFun, "application/x-www-form-urlencoded");
                    }
                    else {
                        $.messageBoxAlert(0, serviceName + "请求数据网络出错！" + JSON.stringify(XMLHttpRequest) + textStatus + errorThrown, function () {
                            $.messageBoxCloseLoading();
                        });
                    }
                }
            });
        },
        postFileGateway: function (serviceName, serviceJson, serviceTitle, serviceToken, serviceFun) {
            if (serviceToken != null && serviceToken.hasOwnProperty("sessionKey")) {// && sessionStorage.getItem("Authorization") == null
                this.postToken(serviceToken);
            }
            $.messager.progress(serviceTitle);
            var formData = new FormData($("#" + serviceJson.uploadFormId)[0]);
            formData.append('path', "/api/" + serviceName);
            for (var key in serviceJson) {
                if (key == "uploadFormId") continue;
                formData.append(key, serviceJson[key]);
            }
            $.ajax({
                url: common.gatewayUrl() + "Service/File",
                type: 'POST',
                cache: false,
                data: formData,
                processData: false,
                contentType: false,
                beforeSend: function (request) {
                    request.setRequestHeader("Authorization", sessionStorage.getItem("Authorization"));
                },
                success: function (result) {
                    $.messager.progress('close');
                    if (result.isSucceed == true && result.errcode == 0 && serviceFun != null) {
                        serviceFun(result);
                    }
                    else {
                        $.messageBoxBottom(0, "对不起，上传文件出错" + result.errmsg);
                    }
                },
                error: function (err) {
                    $.messageBoxBottom(0, JSON.stringify(err));
                }
            });
        },
        postWebGateway: function (serviceName, serviceJson, serviceToken, serviceFun) {
            $.ajax({
                url: '/Gateway/PostData',
                type: 'post',
                data: {
                    serviceName: serviceName,
                    postData: JSON.stringify(serviceJson),
                    token: serviceToken
                },
                headers: { VerificationToken: $("input[name='Web_Token_XSRF_CSRF']").val() },
                success: function (response) {
                    if (response == null || response == "") {
                        if (serviceName != "platform/user/gettoken") {
                            $.messageBoxAlert(0, "调用" + serviceName + "服务出错！");
                        }
                    }
                    else if (serviceFun != null) {
                        var json;
                        try {
                            json = JSON.parse(response);
                        }
                        catch (e) {
                            json = response;
                        }
                        serviceFun(json);
                    }
                }
            });
        },
        postWebSocket: function (serviceName, serviceJson, serviceToken, serviceFun) {
            var wsuri = window.location.protocol.replace("http", "ws") + "//" + window.location.hostname + ":9080/ws";
            sock = new WebSocket(wsuri);
            sock.onopen = function () {
                sock.send(JSON.stringify(
                    {
                        "sessionKey": ((common.getCookie("sessionKey") == null || common.getCookie("sessionKey") == "") ? "noLogin" : common.getCookie("sessionKey")),
                        "sendToken": sessionStorage.getItem("Authorization"),
                        "sendCommand": "/api/" + serviceName,
                        "sendParameter": serviceJson
                    }));
            }
            sock.onclose = function (e) {
                sock = null;
            }
            sock.onmessage = function (e) {
                //console.log("message received: " + e.data);
                var response = JSON.parse(e.data);
                if (response.isSucceed == true && (response.errcode == 0 || response.errcode == 200)) {
                    if (serviceFun != null) serviceFun(response);
                }
                else {
                    $.messageBoxAlert(0, "调用" + serviceName + "服务出错！" + response.errmsg);
                    if (serviceFun != null) serviceFun(response);
                }
                sock.close();
                sock = null;
            }
        },
        setCookie: function (name, value) {
            /*
            var Days = 30;
            var exp = new Date();
            exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
            document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString();
            */
            document.cookie = name + "=" + escape(value) + ";path=/;";
        },
        getCookie: function (name) {
            var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
            if (arr = document.cookie.match(reg))
                return unescape(arr[2]);
            else
                return "";
        },
        removeCookie: function (name) {
            var exp = new Date();
            exp.setTime(exp.getTime() - 1);
            var cval = this.getCookie(name);
            if (cval != null)
                document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
        },
        removeNoLogin: function () {
            this.setCookie("sessionKey", "noLogin");
            window.sessionStorage.removeItem("encrypt");
            window.sessionStorage.removeItem("Authorization");
            this.removeCookie("sessionKey");
            if ((this.getCookie("sessionKey") == null || this.getCookie("sessionKey") == "" || this.getCookie("sessionKey") == "noLogin") && localStorage.getItem("token") != null) {
                this.postGateway("website/clearredis", { key: ["User:" + common.Decrypt(localStorage.getItem("token"))] }, null, function () {
                    window.localStorage.removeItem("token");
                });
            }
            else {
                window.localStorage.removeItem("token");
            }
        },
        getUrlParam: function (name) {//获取url中的参数
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i"); //构造一个含有目标参数的正则表达式对象
            var UrlQuery = window.location.search.substr(1);
            var r = UrlQuery.match(reg);  //匹配目标参数
            if (r != null) return decodeURI(r[2]); return null; //返回参数值
        },
        HTMLEncode: function (html) {
            if (html && html.toString().indexOf("<") != -1 && html.toString().indexOf(">") != -1) {
                var temp = document.createElement("div");
                (temp.textContent != null) ? (temp.textContent = html) : (temp.innerText = html);
                var output = temp.innerHTML;
                temp = null;
                return output;
            }
            else {
                return html;
            }
        },
        DingDingScan: function () {
            var host = window.location.host;
            if (host == "nti.eapps.dingtalkcloud.com") {
                return {
                    host: host, appId: "dingoawmhei7mhqbhxkyre"
                };
            }
            else if (host == "cloud.nti56.com") {
                return {
                    host: host, appId: "dingoavoelafcddmqojubv"
                };
            }
            else if (host == "yun.nti56.com") {
                return {
                    host: host, appId: "dingoal5wj8ixprybl0zem"
                };
            }
            else {
                return {
                    host: host, appId: "Error"
                };
            }
        },
        patch: function (re, s) {//判断re字符串在s中出现的次数!
            var count;
            var re = eval("/" + re + "/gi")
            if (s.match(re) == null) {
                count = 0;
            }
            else {
                count = s.match(re).length;
            }
            return count;
        },
        isExitsFunction: function (funcName) {//是否存在指定函数 
            try {
                if (typeof (eval(funcName)) == "function") {
                    return true;
                }
            } catch (e) { }
            return false;
        },
        isExitsVariable: function (variableName) {//是否存在指定变量 
            try {
                if (typeof (variableName) == "undefined") {
                    //alert("value is undefined"); 
                    return false;
                } else {
                    //alert("value is true"); 
                    return true;
                }
            } catch (e) { }
            return false;
        },
        intensityReg: function (value) {//校验登录密码规则 是否符合三级等保要求
            var reg = /^(?![a-zA-Z]+$)(?![A-Z0-9]+$)(?![A-Z\W_!@#$%^&*`~()-+=\/]+$)(?![a-z0-9]+$)(?![a-z\W_!@#$%^&*`~()-+=\/]+$)(?![0-9\W_!@#$%^&*`~()-+=\/]+$)[a-zA-Z0-9\W_!@#$%^&*`~()-+=\/]{8,30}$/;
            return reg.test(value);
        },
        //点击修改密码
        modifyPsword: function (arr, deviceType) {
            var jqObj_1 = arr[0], jqObj_2 = arr[1], jqObj_3 = arr[2];
            if ($.trim(jqObj_1.val()).length == 0) {
                $.messageBoxAlert(0, "请输入原密码 !");
                jqObj_1.focus();
                return;
            };
            if ($.trim(jqObj_2.val()).length == 0) {
                $.messageBoxAlert(0, "请输入新密码 !");
                jqObj_2.focus();
                return;
            };
            if (!this.intensityReg($.trim(jqObj_2.val()))) {
                $.messageBoxAlert(0, "新密码强度不够，不符合三级等保要求！<br>1、密码长度大于等于8个字符；<br>2、必须是大写字母，小写字母，数字，特殊字符(_!@#$%^&*`~()-+=/)中任意三个组合!");
                jqObj_2.focus();
                return;
            };
            if ($.trim(jqObj_2.val()) != $.trim(jqObj_3.val())) {
                $.messageBoxAlert(0, "两次密码不一致 !");
                jqObj_3.focus();
                return;
            };
            this.postGateway('platform/user/modifypassword', {
                pswdOld: jqObj_1.val(),
                pswdNew: jqObj_2.val()
            }, common.getToken(), function (msg) {
                if (msg.errcode == 0) {
                    $.messageBoxAlert(1, "修改成功 !");
                    window.localStorage.setItem('intensityReg', 'true');
                    setTimeout(function () {
                        $.messageBoxCloseAll();
                        $('#pc_mdfpassword').hide();
                        $('#modalpassword').hide();
                        if (deviceType == 'phone') {
                            window.history.back();
                        };
                    }, 1500);
                    jqObj_1.val('');
                    jqObj_2.val('');
                    jqObj_3.val('');
                } else {
                    $.messageBoxAlert(0, msg.errmsg);
                };
            });
        },
    }
})();


Date.prototype.format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1,                 //月份 
        "d+": this.getDate(),                    //日 
        "h+": this.getHours(),                   //小时 
        "m+": this.getMinutes(),                 //分 
        "s+": this.getSeconds(),                 //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds()             //毫秒 
    };
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        }
    }
    return fmt;
}