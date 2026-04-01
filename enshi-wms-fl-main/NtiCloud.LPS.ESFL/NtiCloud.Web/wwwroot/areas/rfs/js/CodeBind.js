$(function () {
    $(".one_code").focus();
    $(".one_code").keydown(function (event) {
        if (event.which == 13) {
            var one_code = $(".one_code").val();
            if (one_code == '' || one_code == null || one_code.length != 32) {
                alert("一号工程码不能为空或者一号工程码必须为32位！！！");
                return;
            }
            //查询后台一号工程码是否重复
            var searchSql = "select count(1) count from swms_b_barcode where trunc(create_date) = trunc(sysdate) and bar_code = '" + one_code+"'";
            var token = common.getToken();
            common.postGateway('platform/database/selectsql', {
                "sql": common.Encrypt(searchSql),
                "parameters": []
            }, token, function (data) {
                if (data.errcode == 0 && data.errmsg == "OK") {
                    var result = JSON.parse(data.json);
                    var len = result.length;
                    if (len <= 0) {
                        alert("查询数据失败!");
                    }
                    else {
                        var count = parseInt(result[0].count);
                        if (count >= 1) {
                            alert("该条码已经扫描,不需要重复扫描！");
                        }
                        else if (count == 0) {
                            var sqlArray = ["insert into swms_b_barcode(t_id,client_id,warehouse_id,bar_code,created_by,create_date,act_status) values(@t_id,'1010203040506070809','1016319470013779968',@bar_code,@created_by,sysdate,1)"];
                            var jsonArray = [{ "t_id": "", "bar_code": one_code, "created_by": JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserName }];
                            common.postGateway("platform/database/updatesql", {
                                "sql": common.Encrypt(sqlArray.join(";")),
                                "json": jsonArray
                            }, common.getToken(), function (res) {
                                if (res.errcode == 0 && res.errmsg == "OK") {
                                    $(".one_code").val('');
                                }
                                else {
                                    alert("系统忙，请稍后再试！");
                                }
                            });
                        }
                        else {
                            alert("查询数据失败!");
                        }
                    }
                }  
                else {
                    alert("查询数据失败!");
                }
            });
            
        }
    });
})
