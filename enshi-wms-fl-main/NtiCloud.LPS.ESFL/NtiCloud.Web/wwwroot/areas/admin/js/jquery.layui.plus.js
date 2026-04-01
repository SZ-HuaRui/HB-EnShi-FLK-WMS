$.extend({
    messageLoad: function () {
        layui.use('layer', function () {
            var layer = layui.layer;
            layer.load(2, { shade: [0.1, '#000'] });
        });
    },
    messageTips: function (msg, id, tips) {
        layui.use('layer', function () {
            var layer = layui.layer;
            layer.tips(msg, id, { tips: tips });
        });
    },
    messageMsg: function (msg, json) {
        layui.use('layer', function (){
            var layer = layui.layer;
            layer.msg(msg, json);
        });
    },
    messageBoxAlert: function (status, msg, fun) {
        var icon = 1, title = '成功';
        if (status == 1) {
            icon = 1;
            title = '成功';
        }
        else {
            icon = 2;
            title = '失败';
        }
        layui.use('layer', function () {
            var layer = layui.layer;
            var index = layer.alert(msg, { title: title, icon: icon, resize: false }, function () {
                if (fun) { fun() };
                layer.close(index);
            });
        });
    },
    //不含按钮
    messageBoxOpenID: function (title, area, id, fun){
        layui.use('layer', function () {
            var layer = layui.layer;
            var index = layer.open({
                type: 1,
                title: title,
                area: area,
                resize: false,
                content: (id.indexOf("#") == 0 ? $(id) : id),
                cancel: function (){
                    if (id.indexOf("#") == 0) {
                        $(id).hide();
                    }
                }
            });
            if (fun) { fun(index);}
        });
    },
    //含按钮
    messageBoxOpenBtn: function (title, area, id, setArr, closeBtn) {
        if (closeBtn === "" || closeBtn === undefined || closeBtn === null) { closeBtn = 1 };//可通过配置1和2来展示，如果不显示，则closeBtn: 0
        layui.use('layer', function () {    
            var layer = layui.layer;
            var layObj = {
                type: 1,
                title: title,
                area: area,
                closeBtn: closeBtn,
                resize: false,
                content: (id.indexOf("#") == 0 ? $(id):id),
                btn: [],
                cancel: function () {
                    if (id.indexOf("#") == 0) {
                        $(id).hide();
                    }
                }
            };
            for (var i = 0; i < setArr.length; i++) {
                var keyName = i != 0 ? ('btn' + (i + 1)) : 'yes';
                (function (i) {
                    layObj.btn.push(setArr[i].text);
                    layObj[keyName] = setArr[i].handler;
                })(i);
            };
            layer.open(layObj);
        });
    },
    messageBoxConfirm: function (msg, btn, fun1,fun2) {
        layui.use('layer', function () {
            var layer = layui.layer;
            var json = {
                resize: false,
                closeBtn: 0,
                title: '信息',
                icon: 3
            }
            if (btn != null) {
                json.btn = btn;
            }
            layer.confirm(msg, json, function (index) {
                if (fun1) { fun1() };
                layer.close(index);
            }, function () {
                if (fun2) { fun2() };
            });
        });
    },
    messageBoxBottom: function (status, msg) {
        var icon = 1, title = '成功';
        if (status == 1) {
            icon = 1;
            title = '成功';
        }
        else {
            icon = 2;
            title = '失败';
        }
        layui.use('layer', function () {
            var layer = layui.layer;
            layer.alert(msg, { title: title, icon: icon, offset: 'rb', resize: false, time: 3000, shadeClose:true });
        });
    },
    messageBoxClose: function (index) {
        layui.use('layer', function () {
            var layer = layui.layer;
            layer.close(index);
        });
    },
    messageBoxCloseLoading: function () {
        layui.use('layer', function () {
            var layer = layui.layer;
            layer.closeAll('loading');
        });
    },
    messageBoxCloseAll: function () {
        layui.use('layer', function () {
            var layer = layui.layer;
            layer.closeAll();
        });
    }
});

var BasedLayUi = (function () {
    return {
        baseTableRender: function (tableData) {//layui表格及事件监听函数
            layui.use(['jquery', 'form', 'element', 'table', 'layer'], function () {
                var $ = layui.$;
                var element = layui.element;
                var layer = layui.layer;
                var form = layui.form;
                var table = layui.table;
                var indexChecked;
                //折叠面板渲染
                element.render();
                //form表单的渲染
                form.render();
                var tableJson = {
                    elem: "#" + tableData.elem,
                    even: true,
                    cols: tableData.cols
                };
                if (tableData.height) tableJson.tableJson = tableData.height;
                if (tableData.data) tableJson.data = tableData.data;
                if (tableData.method) tableJson.method = tableData.method;
                if (tableData.url) {
                    tableJson.url = tableData.url;
                    tableJson.contentType = 'application/json';
                    tableJson.parseData = function (response) { //res 即为原始返回的数据
                        return {
                            "code": 0, //解析接口状态
                            "msg": "", //解析提示文本
                            "count": JSON.parse(response.Entity).total, //解析数据长度
                            "data": JSON.parse(response.Entity).rows //解析数据列表
                        };
                    };
                    tableJson.request = {
                        pageName: 'page', //页码的参数名称，默认：page
                        limitName: 'limit' //每页数据量的参数名，默认：limit
                    };
                    tableJson.response = {
                        statusName: 'code' //规定数据状态的字段名称，默认：code
                        , statusCode: 0 //规定成功的状态码，默认：0
                        , msgName: 'msg' //规定状态信息的字段名称，默认：msg
                        , countName: 'count' //规定数据总数的字段名称，默认：count
                        , dataName: 'data' //规定数据列表的字段名称，默认：data
                    };
                }
                if (tableData.where) tableJson.where = tableData.where;
                if (tableData.page) {
                    tableJson.page = tableData.page;
                    tableJson.limit = 10;
                    tableJson.limits = [10, 20, 30, 40, 50, 100, 200, 500];
                }
                else {
                    tableJson.page = false;
                    tableJson.limit = 100000;
                }
                if (tableData.toolbar) tableJson.toolbar = tableData.toolbar;
                if (tableData.defaultToolbar) tableJson.defaultToolbar = tableData.defaultToolbar;

                //数据表格的渲染
                var baseTable = table.render(tableJson);
                //头部工具栏事件
                table.on('toolbar(' + tableData.elem + ')', function (obj) {
                    var oldData = table.cache[tableData.elem];
                    switch (obj.event) {
                        case 'addRow'://新增一行
                            if (!indexChecked) indexChecked = oldData.length;
                            oldData.splice(indexChecked, 0, {});
                            baseTable.reload({
                                url:"",
                                data: oldData,
                                done: function () {
                                    //bindTableToolbar();
                                }
                            });
                            break;
                        case 'upRow'://上移按钮
                            BasedLayUi.upData(oldData, indexChecked);
                            baseTable.reload({
                                url:'',
                                data: newArr,
                                done: function () {
                                    //bindTableToolbar();		
                                    //radioChecked();					
                                    if (indexChecked <= 0) {
                                        return false;
                                    } else {
                                        indexChecked--;
                                    };
                                }
                            });
                            break;
                        case 'downRow'://下移按钮
                            BasedLayUi.downData(oldData, indexChecked);
                            baseTable.reload({
                                url: '',
                                data: newArr,
                                done: function () {
                                    //bindTableToolbar();		
                                    //radioChecked();					
                                    if (indexChecked > (oldData.length - 2)) {
                                        return false;
                                    } else {
                                        indexChecked++;
                                    };
                                }
                            });
                            break;
                        case 'refresh'://刷新按钮
                            baseTable.reload({
                                data: oldData,
                                done: function () {
                                    //bindTableToolbar();
                                    var index = layer.load(1, {
                                        shade: [0.1, '#fff'],
                                        time: 500
                                    });
                                }
                            });
                            break;
                    };
                });
                //监听表格内（右侧）的工具条
                table.on('tool(' + tableData.elem + ')', function (obj) { //tool是工具条事件名，test是table原始容器的属性 lay-filter="对应的值"
                    var data = obj.data //获得当前行数据
                        , event = obj.event, tr = obj.tr, index = obj.tr.index();
                    var oldData = table.cache[tableData.elem];
                    switch (event) {
                        // case 'upRow':
                        // 	upData(oldData, index);
                        // 	tableT.reload({
                        // 		data: newArr,
                        // 		done: function () {
                        // 			bindTableToolbar(); 
                        // 		}
                        // 	});
                        // 	break;
                        case "del":
                            layer.confirm('真的删除行么', function (indexClose) {
                                obj.del(); //删除对应行tr的DOM结构
                                layer.close(indexClose);
                                BasedLayUi.removeEmptyTableCache(oldData);
                                baseTable.reload({
                                    data: oldData,
                                    done: function () {
                                        var index = layer.load(1, {
                                            shade: [0.1, '#fff'],
                                            time: 500
                                        });
                                    }
                                });
                            });
                            break;
                        // case 'downRow':
                        // 	downData(oldData, index);
                        // 	tableT.reload({
                        // 		data: newArr,
                        // 		done: function () {
                        // 			bindTableToolbar();
                        // 		}
                        // 	});
                        // 	break;
                        case 'dataSource':
                            layer.open({
                                type: 1,
                                title: '数据源配置',
                                shade: 0.5,
                                area: ['600px', '330px'], //宽高
                                content: $('#dataSourceOpen'),
                                btn: ['确定', '取消'],
                                yes: function () {
                                    alert(321);
                                }
                            });
                            break;
                        case 'radio':
                            indexChecked = index;
                            break;
                    }
                });
                //监听单元格编辑
                table.on('edit(' + tableData.elem + ')', function (obj) {
                    var value = obj.value //得到修改后的值
                        , data = obj.data //得到所在行所有键值
                        , field = obj.field; //得到字段
                    layer.msg('[ID: ' + data.id + '] ' + field + ' 字段更改为：' + value);
                });
            });
        },
        upData: function (arr, index) {//上移按钮函数
            if (arr.length > 1 && index > 0) {
                newArr = BasedLayUi.swapItems(arr, parseInt(index), parseInt(index) - 1);
            }
            else {
                newArr = arr;
            }
        },
        downData: function (arr, index) {//下移按钮函数
            if (arr.length > 1 && index < (arr.length - 1)) {
                newArr = BasedLayUi.swapItems(arr, parseInt(index), parseInt(index) + 1);
            }
            else {
                newArr = arr;
            }
        },
        swapItems: function (arr, index1, index2) {//上移下移时改变table的数据顺序
            arr[index1] = arr.splice(index2, 1, arr[index1])[0];
            return arr;
        },
        renderSelectOptions: function (data, settings) {//对表格里的某个单元格下拉数据进行解析
            settings = settings || {};
            var valueField = settings.valueField || 'value',
                textField = settings.textField || 'text',
                selectedValue = settings.selectedValue || "";
            var html = [];
            for (var i = 0, item; i < data.length; i++) {
                item = data[i];
                html.push('<option value="');
                html.push(item[valueField]);
                html.push('"');
                if (selectedValue && item[valueField] == selectedValue) {
                    html.push(' selected="selected"');
                }
                html.push('>');
                html.push(item[textField]);
                html.push('</option>');
            }
            return html.join('');
        },
        removeEmptyTableCache: function (oldData) {
            for (var i = 0, row; i < oldData.length; i++) {
                row = oldData[i];
                if (!row || row.length == 0) {
                    oldData.splice(i, 1);//删除一项
                    i--;
                }

                continue;
            }
        },
        radioChecked: function () {
            $('.layui-form-radio').on('click', function () {
                var index = $(this).parent().parent().parent().attr('data-index');
                indexChecked = index;
            })
        }
    }
})();