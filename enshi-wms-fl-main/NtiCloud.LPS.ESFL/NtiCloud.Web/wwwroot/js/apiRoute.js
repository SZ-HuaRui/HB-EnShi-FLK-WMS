var apiRoute = (function () {
    return {
        selectDataEvent: function (listSql, parameters, queryStr, listFun) {
            common.postGateway('platform/database/selectsql', {
                "sql": common.Encrypt(listSql),
                "parameters": parameters,
                "queryStr": queryStr
            }, common.getToken(), function (data) {
                if (listFun != null) listFun(JSON.parse(data.json));
            })
        },
        selectEasyuiTable: function (dataSourceCode,listSql, parameters, listFun) {
            var SelectMicroServices = common.Decrypt(listSql);
            if (SelectMicroServices.toString().indexOf("SelectMicroServices(") == 0) {
                var microService = SelectMicroServices.toString().replace("SelectMicroServices(", "").replace(")", "").split(",");
                if (microService.length >= 1) parameters.serviceArg = microService[1];
                parameters.isData = true;
                parameters.dataSource = dataSourceCode;
                common.postGateway(microService[0], parameters, common.getToken(), function (resultitem) {
                    if (listFun != null) {
                        var json;
                        try {
                            if (resultitem.errcode == 401) {
                                location.reload();
                                return;
                            }
                            json = JSON.parse(resultitem.json);
                        }
                        catch (e) {
                            json = resultitem.json;
                        }
                        listFun(json);
                    }
                });
            }
            else {
                parameters.sql = listSql;
                parameters.dataSource = dataSourceCode;
                common.postGateway('platform/database/selectdatapage', parameters
                    , common.getToken(), function (tableData) {
                        if (tableData.errcode == 401) {
                            location.reload();
                        }
                        else if (listFun != null) listFun(JSON.parse(tableData.json));
                });
            }
        },
        selectTreeEvent: function (dataSourceCode, sql, id, pid, text, pidValue, parameters, listFun) {
            common.postGateway('platform/database/selecttree', {
                "dataSource": dataSourceCode,
                "sql": common.Encrypt(sql),
                "id": id,
                "pid": pid,
                "text": text,
                "pidValue": pidValue,
                "parameters": parameters
            }, common.getToken(), function (tableData) {
                    if (listFun != null) listFun(JSON.parse(tableData.json));
            });
        },
        runTaskEvent: function (dataSourceCode,listSql, listJson, listFun) {
            if (listSql.toString().indexOf("RunTransFlow(") == 0) {
                var transFlow = listSql.toString().replace("RunTransFlow(", "").replace(")", "").split(",");
                var transFlowParameters = [];
                for (var i = 1; i < transFlow.length; i++) {
                    if (transFlow[i].indexOf("@") != -1) {
                        transFlowParameters.push(listJson[0][transFlow[i].replace(/@/g, "").trim()]);
                    }
                    else {
                        transFlowParameters.push(transFlow[i].trim());
                    }
                }
                common.postGateway('platform/transflows/runtransflow', {
                    "transflowCode": transFlow[0].trim(),
                    "parameters": transFlowParameters,
                    "commit": true
                }, common.getToken(), function (result) {
                    if (result.ErrMsg && result.ErrMsg != "") {
                        $.messageBoxCloseLoading();
                        $.messageBoxBottom(0, result.ErrMsg);
                    }
                    else if (result.errcode != 0 && result.errmsg && result.errmsg != "") {
                        $.messageBoxCloseLoading();
                        $.messageBoxBottom(0, result.errmsg);
                    }
                    else if (listFun != null) listFun(result);
                });
            }
            else if (listSql.toString().indexOf("RunMicroServices(") == 0) {
                var microService = listSql.toString().replace("RunMicroServices(", "").replace(")", "").split(",");
                var microServiceParameters = [];
                for (var i = 0; i < listJson.length; i++) {
                    var microServiceParameter = {};
                    for (var j = 1; j < microService.length; j++) {
                        var key = microService[j].replace(/@/g, "").trim();
                        microServiceParameter[key] = listJson[i][key];
                    }
                    microServiceParameters.push(microServiceParameter);
                }
                common.postGateway(microService[0], microServiceParameters, common.getToken(), function (result) {
                    if (result.errcode != 0) {
                        $.messageBoxCloseLoading();
                        $.messageBoxBottom(0, result.errmsg);
                    }
                    else if (listFun != null) listFun(result);
                });
            }
            else {
                common.postGateway("platform/database/updatesql", {
                    "dataSource": dataSourceCode,
                    "sql": common.Encrypt(listSql),
                    "json": listJson
                }, common.getToken(), function (result) {
                    if (result.resMsg != "") {
                        $.messageBoxCloseLoading();
                        $.messageBoxBottom(0, result.resMsg);
                    }
                    else if (listFun != null) listFun(result);
                });
            }
        },
        dictionaryConfig: function (dictionaryCodes, isTable, dictionaryFun) {
            common.postGateway('platform/system/dictionaryconfig', {
                "dictionaryCodes": dictionaryCodes,
                "isTable": isTable
            }, common.getToken(), function (tableData) {
                if (dictionaryFun != null) dictionaryFun(JSON.parse(tableData.json));
            });
        },
        pageDictionary: function (dictionaryorsql, dictionaryFun) {
            common.postGateway('platform/system/pagedictionary', {
                code: dictionaryorsql
            }, common.getToken(), function (tableDataSet) {
                if (dictionaryFun != null) dictionaryFun(JSON.parse(tableDataSet.json));
            });
        },
        initializationSystem: function (num,returnFun) {
            common.postGateway('platform/system/initialization', {
                num: num
            }, common.getToken(), function (userinfo) {
                if (returnFun != null) returnFun(userinfo);
            });
        },
        fieldList: function (treeService, returnFun) {
            if (treeService.toString().indexOf("SelectMicroServices(") == 0) {
                var microService = treeService.toString().replace("SelectMicroServices(", "").replace(")", "").split(",");
                var microServiceParameters = {};
                if (microService.length >= 1) microServiceParameters.serviceArg = microService[1];
                microServiceParameters.isData = false;
                common.postGateway(microService[0], microServiceParameters, common.getToken(), function (resultitem) {
                    if (returnFun != null) returnFun(resultitem);
                });
            }
            else {
                common.postGateway('platform/table/fieldlist', {
                    sql: treeService
                }, common.getToken(), function (resultitem) {
                    if (returnFun != null) returnFun(resultitem);
                });
            }
        }
    }
})();