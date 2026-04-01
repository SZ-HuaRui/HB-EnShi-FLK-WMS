//ifram 返回
function frameReturnByClose() {
    $("#modalwindow").window('close');
}
/*
原理很简单，使用一个死循环来遍历数组，循环跳出的条件是数组的长度为0，也就是说，循环内部会引起数组长度的改变。这里就几个关键点做一下说明。
1.为什么要用死循环？顺序混乱，如果单次循环，子节点出现在父节点之前，子节点不好处理，这里做一个死循环相当于先把父节点全部找出，但是这里又不是简单的先把所有的父节点找出，找的同时，如果这个节点父节点已经找到，那么可以继续做后续操作;
2.如何建立层级关系？代码中有一个变量pos，这个用于保存每个已添加到tree中的节点在tree中位置信息，比如上面测试数据父节点A添加到tree后，那么pos中增加一条数据，pos={”1“:[0]}，1就是父节点A的id，这样写便于查找，[0]表示父节点A在tree的第一个元素，即tree[0]，如果某个位置信息为[1,2,3]，那么表示这个节点在tree[1].children[2].children[3]，这里的位置关系其实就是父子的层级关系。
*/
var JQueryEasyUI = (function () {
    return {
        toTreeData: function (data, idName, pidName, textName) {
            var pos = {};
            var tree = [];
            var i = 0;
            while (data.length != 0) {
                if (data[i][pidName] == 0 || data[i][pidName] == "" || data[i][pidName] == null) {
                    //delete data[i][pidName];
                    if (data[i].id == undefined) data[i].id = data[i][idName];
                    var texttemp = data[i][textName];
                    delete data[i][textName];
                    data[i].text = texttemp;
                    tree.push(data[i]);
                    pos[data[i][idName]] = [tree.length - 1];
                    data.splice(i, 1);
                    i--;
                } else {
                    var posArr = pos[data[i][pidName]];
                    if (posArr != undefined) {
                        var obj = tree[posArr[0]];
                        for (var j = 1; j < posArr.length; j++) {
                            obj = obj.children[posArr[j]];
                        }
                        if (obj.children == undefined) obj.children = [];
                        //delete data[i][pidName];
                        if (data[i].id == undefined) data[i].id = data[i][idName];;
                        var texttemp = data[i][textName];
                        delete data[i][textName];
                        data[i].text = texttemp;
                        obj.children.push(data[i]);
                        pos[data[i][idName]] = posArr.concat([obj.children.length - 1]);
                        data.splice(i, 1);
                        i--;
                    }
                }
                i++;
                if (i > data.length - 1) {
                    i = 0;
                }
            }
            return tree;
        },
        toGridTreeData: function (data, idName, pidName) {
            var pos = {};
            var tree = [];
            var i = 0;
            while (data.length != 0) {
                if (data[i][pidName] == 0 || data[i][pidName] == "" || data[i][pidName] == null) {
                    //delete data[i][pidName];			
                    tree.push(data[i]);
                    pos[data[i][idName]] = [tree.length - 1];
                    data.splice(i, 1);
                    i--;
                } else {
                    var posArr = pos[data[i][pidName]];
                    if (posArr != undefined) {
                        var obj = tree[posArr[0]];
                        for (var j = 1; j < posArr.length; j++) {
                            obj = obj.children[posArr[j]];
                        }
                        if (obj.children == undefined) obj.children = [];
                        //delete data[i][pidName];
                        obj.children.push(data[i]);
                        pos[data[i][idName]] = posArr.concat([obj.children.length - 1]);
                        data.splice(i, 1);
                        i--;
                    }
                }
                i++;
                if (i > data.length - 1) {
                    i = 0;
                }
            }
            return tree;
        },
        toGridData: function (data, total) {
            var griddata = { "total": total, "rows": data };
            return griddata;
        },
        dataDictionaryFormat: function (row) {//数据字典下拉样式
            if (row.description) {
                var opts = $(this).combobox('options');
                if (row.description.indexOf('linear-gradient') != -1 || (row.description.indexOf('#') != -1 && row.description.length < 10) || row.description.indexOf('rgb') != -1) {
                    var s = '<span style="font-weight:bold">' + row[opts.textField] + '</span><br/>' +
                        '<span style="color:#ffffff;background:' + row.description + '">' + row.description + '</span>';
                } else {
                    var s = '<span style="font-weight:bold">' + row[opts.textField] + '</span><br/>' +
                        '<span style="color:#888">' + row.description + '</span>';
                }
                return s;
            }
            else {
                var opts = $(this).combobox('options');
                return row[opts.textField];
            }
        },
        treeDictionaryFormat: function (row) {//数据字典下拉样式
            if (row.description) {
                var s = '<div class="easyui-tooltip" title="' + row.description + '">' + row.text + '</div>';
                return s;
            }
            else {
                //var opts = $(this).combobox('options');
                //return row[opts.textField];
                return row.text;
            }
        },
        gridToolsMenu: function (gridname, insertCode) {
            return "&nbsp;&nbsp;&nbsp;&nbsp;<a title=\"添加\" onclick=\"JQueryEasyUI.gridDataButton('" + gridname + "','insertRow','" + insertCode + "')\" class=\"btnline\"><span class=\"mdi mdi-plus\"></span></a>"
                + "<a title=\"删除\" onclick=\"JQueryEasyUI.gridDataButton('" + gridname + "','deleteRow')\" class=\"btnline\"><span class=\"mdi mdi-playlist-remove\"></span></a>"
                + "<a title=\"上移\" onclick=\"JQueryEasyUI.gridDataButton('" + gridname + "','MoveUp')\" class=\"btnline\"><span class=\"mdi mdi-cloud-upload\"></span></a>"
                + "<a title=\"下移\" onclick=\"JQueryEasyUI.gridDataButton('" + gridname + "','MoveDown')\" class=\"btnline\"><span class=\"mdi mdi-cloud-download\"></span></a>";
        },
        gridDataButton: function (gridname, opt, insertCode) {
            var rowData = $(gridname).datagrid('getSelected');
            var rowIndex = $(gridname).datagrid('getRowIndex', rowData);
            //$(gridname).datagrid("endEdit", rowIndex);
            //$(gridname).datagrid('cancelEdit', rowIndex);
            if ("MoveUp" == opt) {
                if (rowIndex != 0 && rowIndex != -1) {
                    var toup = $(gridname).datagrid('getData').rows[rowIndex];
                    var todown = $(gridname).datagrid('getData').rows[rowIndex - 1];
                    $(gridname).datagrid("endEdit", rowIndex);
                    $(gridname).datagrid('getData').rows[rowIndex] = todown;
                    $(gridname).datagrid('getData').rows[rowIndex - 1] = toup;
                    $(gridname).datagrid('refreshRow', rowIndex);
                    $(gridname).datagrid('refreshRow', rowIndex - 1);
                    $(gridname).datagrid('selectRow', rowIndex - 1);
                }
            } else if ("MoveDown" == opt) {
                var len = $(gridname).datagrid('getRows').length;
                if (rowIndex != len - 1 && rowIndex != -1) {
                    var todown = $(gridname).datagrid('getData').rows[rowIndex];
                    var toup = $(gridname).datagrid('getData').rows[rowIndex + 1];
                    $(gridname).datagrid("endEdit", rowIndex);
                    $(gridname).datagrid('getData').rows[rowIndex + 1] = todown;
                    $(gridname).datagrid('getData').rows[rowIndex] = toup;
                    $(gridname).datagrid('refreshRow', rowIndex);
                    $(gridname).datagrid('refreshRow', rowIndex + 1);
                    $(gridname).datagrid('selectRow', rowIndex + 1);
                }
            }
            else if (opt == "insertRow") {
                //添加时先判断是否有开启编辑的行，如果有则把开户编辑的那行结束编辑
                for (var n = $(gridname).datagrid("getData").rows.length - 1; n >= 0; n--) {
                    $(gridname).datagrid("endEdit", n);
                }
                var rows = $(gridname).datagrid('getEditingRowIndexs');
                if (rows.length == 0) {
                    $(gridname).datagrid(opt, {
                        index: rowIndex + 1, // index start with 0
                        row: (insertCode == "defaultTable" ? {
                            "fldkey": "否", 'fldnull': 'Y'
                        } : {})
                    });
                    //将新插入的那一行开户编辑状态
                    $(gridname).datagrid("beginEdit", rowIndex + 1);
                    $(gridname).datagrid('selectRow', rowIndex + 1);
                }
            }
            else if (rowIndex != -1) {
                $(gridname).datagrid(opt, rowIndex);
            }
        }
    };
})();

/**
* 在iframe中调用，在父窗口中出提示框(herf方式不用调父窗口)
*/
$.extend($.fn.datagrid.defaults.editors, {
    codemirrorSqlEdit: {
        init: function (container, options) {
            //var talbeEditId = "configSql" + new Date().getTime();
            var talbeEditId = "configSql_" + options.tableId; 
            var codemirrorSql = $('<textarea id="' + talbeEditId+'" rows="2"></textarea>');
            codemirrorSql.appendTo(container);
            functionEditorSql(talbeEditId, SetGridWidthSub(140)*0.39, 35);
            return talbeEditId;
        },
        getValue: function (target) {
            //return $(target).val();
            return editorSql[target].getValue();
        },
        setValue: function (target, value) {
            //$(target).val(value); 
            $.setValue(editorSql[target], value);
        }
    }
});

$.extend({
    messageBox5s: function (title, msg) {
        $.messager.show({
            title: '<span class="mdi mdi-information-outline">&nbsp;&nbsp;' + title + '</span>', msg: msg, timeout: 5000, showType: 'slide', style: {
                left: '',
                right: 5,
                top: '',
                bottom: 5
            },
            width: 250,
            height: 125
        });
    }
});
$.extend({
    messageBox10s: function (title, msg) {
        $.messager.show({
            title: '<span class="mdi-information-outline">&nbsp;&nbsp;' + title + '</span>', msg: msg, height: 'auto', width: 350, timeout: 10000, showType: 'slide', style: {
                left: '',
                right: 5,
                top: '',
                bottom: -document.body.scrollTop - document.documentElement.scrollTop + 5
            }
        });
    }
});
$.extend({
    show_alert: function (strTitle, strMsg) {
        $.messager.alert(strTitle, strMsg);
    }
});





/** 
* panel关闭时回收内存，主要用于layout使用iframe嵌入网页时的内存泄漏问题
*/
$.fn.panel.defaults.onBeforeDestroy = function () {

    var frame = $('iframe', this);
    try {
        // alert('销毁，清理内存');
        if (frame.length > 0) {
            for (var i = 0; i < frame.length; i++) {
                frame[i].contentWindow.document.write('');
                frame[i].contentWindow.close();
            }
            frame.remove();
            if ($.browser.msie) {
                CollectGarbage();
            }
        }
    } catch (e) {
    }
};


var oriFunc = $.fn.datagrid.defaults.view.onAfterRender;
$.fn.datagrid.defaults.view.onAfterRender = function (tgt) {
    if ($(tgt).datagrid("getRows").length > 0) {

        $(tgt).datagrid("getPanel").find("div.datagrid-body").find("div.datagrid-cell").each(function () {
            var $Obj = $(this)
            $Obj.attr("title", $Obj.text());
        })
    }
};


var oriFunctree = $.fn.treegrid.defaults.view.onAfterRender;
$.fn.treegrid.defaults.view.onAfterRender = function (tgt) {
    if ($(tgt).treegrid("getRoots").length > 0) {

        $(tgt).treegrid("getPanel").find("div.datagrid-body").find("div.datagrid-cell").each(function () {
            var $Obj = $(this)
            $Obj.attr("title", $Obj.text());
        })
    }
};

/**
* 防止panel/window/dialog组件超出浏览器边界
* @param left
* @param top
*/

var easyuiPanelOnMove = function (left, top) {
    var l = left;
    var t = top;
    if (l < 1) {
        l = 1;
    }
    if (t < 1) {
        t = 1;
    }
    var width = parseInt($(this).parent().css('width')) + 14;
    var height = parseInt($(this).parent().css('height')) + 14;
    var right = l + width;
    var buttom = t + height;
    var browserWidth = $(window).width();
    var browserHeight = $(window).height();
    if (right > browserWidth) {
        l = browserWidth - width;
    }
    if (buttom > browserHeight) {
        t = browserHeight - height;
    }
    $(this).parent().css({/* 修正面板位置 */
        left: l,
        top: t
    });
};
//$.fn.dialog.defaults.onMove = easyuiPanelOnMove;
//$.fn.window.defaults.onMove = easyuiPanelOnMove;
//$.fn.panel.defaults.onMove = easyuiPanelOnMove;
//让window居中
var easyuiPanelOnOpen = function (left, top) {

    var iframeWidth = $(this).parent().parent().width();

    var iframeHeight = $(this).parent().parent().height();

    var windowWidth = $(this).parent().width();
    var windowHeight = $(this).parent().height();

    var setWidth = (iframeWidth - windowWidth) / 2;
    var setHeight = (iframeHeight - windowHeight) / 2;
    $(this).parent().css({/* 修正面板位置 */
        left: setWidth,
        top: setHeight
    });

    if (iframeHeight < windowHeight) {
        $(this).parent().css({/* 修正面板位置 */
            left: setWidth,
            top: 0
        });
    }
    $(".window-shadow").hide();
    //修复被撑大的问题
    if ($(".window-mask") != null) {
        try{
            if ($(".window-mask").size() > 1) {
                $(".window-mask")[0].remove();
            }
        }
        catch(e){}
    }
    $(".window-mask").attr("style", "display: block; z-index: 9002; width: " + iframeWidth - 200 + "px; height: " + iframeHeight - 200 + "px;");

    //$(".window-mask").hide().width(1).height(3000).show();
};
$.fn.window.defaults.onOpen = easyuiPanelOnOpen;
var easyuiPanelOnClose = function (left, top) {


    $(".window-mask").hide();

    //$(".window-mask").hide().width(1).height(3000).show();
};

$.fn.window.defaults.onClose = easyuiPanelOnClose;
//var easyuiPanelOnResize = function (left, top) {


//    var iframeWidth = $(this).parent().parent().width();

//    var iframeHeight = $(this).parent().parent().height();

//    var windowWidth = $(this).parent().width();
//    var windowHeight = $(this).parent().height();


//    var setWidth = (iframeWidth - windowWidth) / 2;
//    var setHeight = (iframeHeight - windowHeight) / 2;
//    $(this).parent().css({/* 修正面板位置 */
//        left: setWidth-6,
//        top: setHeight-6
//    });

//    if (iframeHeight < windowHeight) {
//        $(this).parent().css({/* 修正面板位置 */
//            left: setWidth,
//            top: 0
//        });
//    }
//    $(".window-shadow").hide();
//    //$(".window-mask").hide().width(1).height(3000).show();
//};
//$.fn.window.defaults.onResize = easyuiPanelOnResize;


/**
* 
* @requires jQuery,EasyUI
* 
* 扩展tree，使其支持平滑数据格式
*/
$.fn.tree.defaults.loadFilter = function (data, parent) {
    var opt = $(this).data().tree.options;
    var idFiled, textFiled, parentField;
    //alert(opt.parentField);
    if (opt.parentField) {
        idFiled = opt.idFiled || 'id';
        textFiled = opt.textFiled || 'text';
        parentField = opt.parentField;
        var i, l, treeData = [], tmpMap = [];
        for (i = 0, l = data.length; i < l; i++) {
            tmpMap[data[i][idFiled]] = data[i];
        }
        for (i = 0, l = data.length; i < l; i++) {
            if (tmpMap[data[i][parentField]] && data[i][idFiled] != data[i][parentField]) {
                if (!tmpMap[data[i][parentField]]['children'])
                    tmpMap[data[i][parentField]]['children'] = [];
                data[i]['text'] = data[i][textFiled];
                tmpMap[data[i][parentField]]['children'].push(data[i]);
            } else {
                data[i]['text'] = data[i][textFiled];
                treeData.push(data[i]);
            }
        }
        return treeData;
    }
    return data;
};

/**

* @requires jQuery,EasyUI
* 
* 扩展combotree，使其支持平滑数据格式
*/
$.fn.combotree.defaults.loadFilter = $.fn.tree.defaults.loadFilter;

//如果datagrid过长显示...截断(格式化时候，然后调用resize事件)
//$.DataGridWrapTitleFormatter("值",$("#List"),"字段");
//onResizeColumn:function(field,width){ var refreshFieldList = ["字段名称","字段名称","字段名称"]; if(refreshFieldList.indexOf(field)>=0){$("#List").datagrid("reload");})}
$.extend({
    DataGridWrapTitleFormatter: function (value, obj, fidld) {
        if (value == undefined || value == null || value == "") {
            return "";
        }
        var options = obj.datagrid('getColumnOption', field);
        var cellWidth = 120;
        if (options != undefined) {
            cellWidth = options.width - 10;
        }
        return "<div style='width:" + cellWidth + "px;padding:0px 6px;line-height:25px;height:25px;margin-top:1px;cursor:pointer;white-space:nowrap:overflow:hidden;text-overflow:ellipsis;' title='" + value + "'>" + value + "</div>";
    }
});
//替换字符串
/* 
 * 功    能：替换字符串中某些字符
 * 参    数：sInput-原始字符串  sChar-要被替换的子串 sReplaceChar-被替换的新串
 * 返 回 值：被替换后的字符串
 */
$.extend({
    ReplaceStrAll: function (sInput, sChar, sReplaceChar) {
        if (sInput == "" || sInput == undefined) {
            return "";
        }
        var oReg = new RegExp(sChar, "g");
        return sInput.replace(oReg, sReplaceChar);

    }
});

/*
 * 功    能：替换字符串中某些字符（只能是第一个被替换掉）
 * 参    数：sInput-原始字符串  sChar-要被替换的子串 sReplaceChar-被替换的新串
 * 返 回 值：被替换后的字符串
 */
$.extend({
    ReplaceOne: function (sInput, sChar, sReplaceChar) {
        if (sInput == "" || sInput == undefined) {
            return "";
        }
        return sInput.replace(sChar, sReplaceChar);
    }
});

/**
 * 鼠标拖拽实现多选
 * @containerObj 父级容器对象 父级容器为可选域
 * @targetObj   选中目标容器对象
 * 
 * 
 * 
 */

$.extend({
    DragToSelect: function (containerObj, targetObj, idStr) {
            //添加一个鼠标拖拽出的遮罩
        var moveSelected = containerObj.find('#' + idStr);
        moveSelected.remove()

        containerObj.css({
            position: 'relative'
        }).append('<div id="' + idStr+'"></div>')
        moveSelected = containerObj.find('#' + idStr);

        //var moveSelected = containerObj.find('#moveSelected');
        moveSelected.css({
            position: 'fixed',
            backgroundColor: 'blue',
            opacity: '0.3',
            border: '1px dashed #d9d9d9',
            top: '0',
            left: '0'
        })

        moveSelected = moveSelected[0]  //下面设置style属性需要转为数组中的单个

        var flag = false;//是搜开启拖拽的标志
        //var timerArr = []; //延时器数组
        var clickTimeArr = [];//点击时间数组
        var oldLeft = 0;//鼠标按下时的left,top
        var oldTop = 0;
        var firstTime = 0;  //鼠标按下的时间
        var lastTime = 0;   //鼠标松开的时间
        var selectedList = [];//拖拽多选选中的块集合

        // 鼠标按下时开启拖拽多选，将遮罩定位并展现
        containerObj.mousedown(function (event) {
            firstTime = new Date().getTime();
            clickTimeArr.push(firstTime);
            flag = true;
            moveSelected.style.top = event.pageY + 'px';
            moveSelected.style.left = event.pageX + 'px';
            oldLeft = event.pageX;
            oldTop = event.pageY;
            event.preventDefault();  // 阻止默认行为
            event.stopPropagation(); // 阻止事件冒泡
        });
        // 鼠标移动时计算遮罩的位置，宽 高
        containerObj.mousemove(function (event) {
            if (!flag) return;//只有开启了拖拽，才进行mouseover操作
            if (event.pageX < oldLeft) {//向左拖
                moveSelected.style.left = event.pageX + 'px';
                moveSelected.style.width = (oldLeft - event.pageX) + 'px';
            } else {
                moveSelected.style.width = (event.pageX - oldLeft) + 'px';
            }
            if (event.pageY < oldTop) {//向上
                moveSelected.style.top = event.pageY + 'px';
                moveSelected.style.height = (oldTop - event.pageY) + 'px';
            } else {
                moveSelected.style.height = (event.pageY - oldTop) + 'px';
            }
            event.preventDefault();  // 阻止默认行为
            event.stopPropagation(); // 阻止事件冒泡
        });
        //鼠标抬起时计算遮罩的right 和 bottom，找出遮罩覆盖的块，关闭拖拽选中开关，清除遮罩数据
        containerObj.mouseup(function (event) {
            lastTime = new Date().getTime();
            moveSelected.style.bottom = Number(moveSelected.style.top.split('px')[0]) + Number(moveSelected.style.height.split('px')[0]) + 'px';
            moveSelected.style.right = Number(moveSelected.style.left.split('px')[0]) + Number(moveSelected.style.width.split('px')[0]) + 'px';
            findSelected();
            flag = false;
            if ((lastTime - firstTime) > 200) {
                event.preventDefault();  // 阻止默认行为
                event.stopPropagation(); // 阻止事件冒泡
            } else {
                clearDragData();
            }
        });
        containerObj.mouseleave(function (event) {
            flag = false;
            moveSelected.style.width = 0;
            moveSelected.style.height = 0;
            moveSelected.style.top = 0;
            moveSelected.style.left = 0;
            event.preventDefault();  // 阻止默认行为
            event.stopPropagation(); // 阻止事件冒泡
        });

        //获取已选的元素
        function findSelected() {
            var blockList = targetObj;
            selectedList = []
            for (var i = 0; i < blockList.length; i++) {
                //计算每个块的定位信息
                var left = $(blockList[i]).offset().left;
                var right = $(blockList[i]).width() + left;
                var top = $(blockList[i]).offset().top;
                var bottom = $(blockList[i]).height() + top;

                var left_move = parseInt(moveSelected.style.left.split('px')[0]);
                var right_move = parseInt(moveSelected.style.right.split('px')[0]);
                var top_move = parseInt(moveSelected.style.top.split('px')[0]);
                var bottom_move = parseInt(moveSelected.style.bottom.split('px')[0]);

                //判断每个块是否被遮罩盖住（即选中）
                var collisionFlag = !(left > right_move || right < left_move || top > bottom_move || bottom < top_move)
                if (collisionFlag) {
                    selectedList.push(blockList[i]);
                    if ($(blockList[i]).hasClass('dragSelected')) {
                        $(blockList[i]).removeClass('dragSelected');
                    } else {
                        $(blockList[i]).addClass('dragSelected');
                    }

                    var isDblclick = false; //判断是否双击
                    if (clickTimeArr.length > 1) {
                        clickTimeArr = clickTimeArr.slice(clickTimeArr.length - 2)
                        var duringTime = clickTimeArr[1] - clickTimeArr[0]
                        //console.log('time', clickTimeArr)
                        //console.log('duringTime', duringTime)
                        if (duringTime < 250) {
                            isDblclick=true
                            //clearTimeout(timerArr[i])
                            $(blockList[i]).dblclick()
                        }
                    }
                    if (!isDblclick) {
                        $(blockList[i]).click()
                        //timerArr[i] = setTimeout(
                        //    (function (i) {
                        //        return function () {
                        //            $(blockList[i]).click()
                        //        }
                        //    })(i),
                        //250)
                    }
                }
            }
            //console.log(selectedList);
            //从单行中去寻找单个选中td
            if (selectedList.length === 1) {
                getOnlyChildContent($(selectedList[0]))
            }
            //清除已选
            clearDragData()
        }

        //清除已选
        function clearDragData() {
            moveSelected.style.width = 0;
            moveSelected.style.height = 0;
            moveSelected.style.top = 0;
            moveSelected.style.left = 0;
            moveSelected.style.bottom = 0;
            moveSelected.style.right = 0;
        }

        //选取单元格的内容
        function getOnlyChildContent(targetObj) {
            var blockList = targetObj.find('td');
            selectedTdList = []
            for (var i = 0; i < blockList.length; i++) {
                if (selectedTdList.length > 1) { //超过一个则不需要再遍历
                    return false
                }
                //计算每个块的定位信息
                var left = $(blockList[i]).offset().left;
                var right = $(blockList[i]).width() + left;
                var top = $(blockList[i]).offset().top;
                var bottom = $(blockList[i]).height() + top;

                var left_move = parseInt(moveSelected.style.left.split('px')[0]);
                var right_move = parseInt(moveSelected.style.right.split('px')[0]);
                var top_move = parseInt(moveSelected.style.top.split('px')[0]);
                var bottom_move = parseInt(moveSelected.style.bottom.split('px')[0]);

                //判断每个块是否被遮罩盖住（即选中）
                var collisionFlag = !(left > right_move || right < left_move || top > bottom_move || bottom < top_move)
                if (collisionFlag) {
                    selectedTdList.push(blockList[i]);
                }

            }
            if (selectedTdList.length === 1) {
                doCopy(selectedTdList[0].innerText)
            }
        }

        /**
        * @param {String} text 需要复制的内容
        * @return {Boolean} 复制成功:true或者复制失败:false  执行完函数后，按ctrl + v试试
        */
        function doCopy(text) {
            var textareaEl = document.createElement('textarea');
            textareaEl.setAttribute('readonly', 'readonly'); // 防止手机上弹出软键盘
            textareaEl.value = text;
            document.body.appendChild(textareaEl);
            textareaEl.select();
            var res = document.execCommand('copy');
            document.body.removeChild(textareaEl);
            //console.log("复制成功");
            return res;
        }

    }
})


function myformatter(date) {
    var dateArray = date.split(" ");
    return dateArray[0].replace("/", "-").replace("/", "-");
}

function myparser(s) {
    if (!s) return new Date();
    var ss = (s.split('-'));
    var y = parseInt(ss[0], 10);
    var m = parseInt(ss[1], 10);
    var d = parseInt(ss[2], 10);
    if (!isNaN(y) && !isNaN(m) && !isNaN(d)) {
        return new Date(y, m - 1, d);
    } else {
        return new Date();
    }
}

function SetGridWidthSub(w) {
    return $(window).width() - w;
}
function SetGridHeightSub(h) {
    return $(window).height() - h
}


function SubStrYMD(value) {
    if (value == null || value == "") {
        return "";
    } else {
        return value.substr(0, value.indexOf(' '))
    }
}
function CustomFormatter(value, success, error) {
    if (value) {
        return "<span class='label label-success'>" + success + "</span>";
    } else {
        return "<span class='label label-error'>" + error + "</span>";
    }
}
function EnableFormatterMes(value, mes) {
    if (value) {
        return "<span class='label label-success'>" + mes + "</span>";
    } else {
        return "<span class='label label-error'>" + mes + "</span>";
    }
}

//圆点状态设置，只有蓝和绿色
function EnableFormatter(value) {
    if (value) {
        return "<span class='label label-success'>启用</span>";
    } else {
        return "<span class='label label-error'>禁用</span>";
    }
}
function CircleFormatter(value) {
    if (value) {
        return "<span class='color-green mdi mdi-checkbox-blank-circle'></span>";
    } else {
        return "<span class='color-red mdi mdi-checkbox-blank-circle'></span>";
    }
}
function SexFormatter(value) {
    if (value) {
        return "<span class='color-green mdi mdi-checkbox-blank-circle'>男</span>";
    } else {
        return "<span class='color-red fmdi mdi-checkbox-blank-circle'>女</span>";
    }
}
//树控件，节点中展开，还是收起
function OpenFormatter(value) {
    if (value =="closed") {
        return "<span class='label label-success'>收起</span>";
    } else {
        return "<span class='label label-error'>展开</span>";
    }
}

function IconFormatter(value) {
    return "<span class='" + value +"'></span>&nbsp;&nbsp;<span class='label label-success'>" + value + "</span>";
}

function CheckFormatter(value) {
    if (value) {
        return "<span class='label label-success'>已审核</span>";
    } else {
        return "<span class='label label-info'>未审核</span>";
    }
}

function IfFormatter(value) {
    if (value == "1" || value == true) {
        return "<span class='label label-success'>是</span>";
    } else {
        return "<span class='label label-info'>否</span>";
    }
}
function ShowFormatter(value) {
    if (value == "S") {
        return "<span class='label label-success'>显示</span>";
    }
    else if (value == "R") {
        return "<span class='label label-info'>只读</span>";
    }
    else if (value == "H") {
        return "<span class='label label-info'>隐藏</span>";
    }
    else if (value == "E") {
        return "<span class='label label-success'>编辑</span>";
    }
    else if (value == "N") {
        return "<span class='label label-error'>去除</span>";
    }
    else {
        return "";
    }
}
function ImgFormatter(value) {
    if (value) {
        return "<img height='55' src='" + value + "' />"
    } else {
        return ""
    }
}
function MoneyFormatter(value) {
    if (value) {
        return "￥" + value
    } else {
        return ""
    }
}


//ComboTree数据过滤
function queryComboTree(q, comboid, roots) {
    var datalist = [];//过滤后的数据源
    var childrenlist = [];//子节点数据

    $(comboid).combotree('setText', q);
    var entertext = $(comboid).combotree('getText');
    if (entertext == null || entertext == "") {
        //清空值之后重新加载数据
        $(comboid).combotree("loadData", roots).combotree("clear");
        return;
    }
    //循环数组
    for (var i = 0; i < roots.length; i++) {
        var org = {
            'id': roots[i].id,
            'text': roots[i].text,
            'children': []
        };
        if (q.toLowerCase() == roots[i].text.toLowerCase()) {
            $(comboid).combotree('setValue', roots[i].id);
        }
        var childrens = [];//查询到的子节点
        //递归找子节点
        childrensTree(comboid, roots[i], childrens, q);
        if (childrens.length > 0) {
            org.children = childrens;
            datalist.push(org);
        }
        else if (org.text.toLowerCase().indexOf(q.toLowerCase()) >= 0 && org.text != "") {
            //没有子节点，但是根节点符合要求
            datalist.push(org);
        }
    }

    if (datalist.length > 0) {
        $(comboid).combotree("loadData", datalist);
        $(comboid).combotree('setText', q);
        datalist = [];//初始化
        return;
    }

}
//组件ID，根节点，需要填充的数组，查询值
function childrensTree(comboid, roots, datalist, q) {
    var roots = roots.children;
    if (roots != undefined) {
        for (var j = 0; j < roots.length; j++) {
            var org = {
                'id': roots[j].id,
                'text': roots[j].text,
                'children': []
            };
            if (q.toLowerCase() == roots[j].text.toLowerCase()) {
                $(comboid).combotree('setValue', roots[j].id);
            }
            var childrens = [];//查询到的子节点
            //递归找子节点
            childrensTree(comboid, roots[j], childrens, q);
            if (childrens.length > 0) {
                org.children = childrens;
                datalist.push(org);
            }
            else if (org.text.toLowerCase().indexOf(q.toLowerCase()) >= 0 && org.text != "") {
                //没有子节点，但是根节点符合要求
                datalist.push(org);
            }
        }
    }
}

//合并列
//onLoadSuccess: function (data) {
//    $(this).datagrid("autoMergeCells", ['Area', 'PosCode']);
//},
$.extend($.fn.datagrid.methods, {
    autoMergeCells: function (jq, fields) {
        return jq.each(function () {
            var target = $(this);
            if (!fields) {
                fields = target.datagrid("getColumnFields");
            }
            var rows = target.datagrid("getRows");
            var i = 0,
			j = 0,
			temp = {};
            for (i; i < rows.length; i++) {
                var row = rows[i];
                j = 0;
                for (j; j < fields.length; j++) {
                    var field = fields[j];
                    var tf = temp[field];
                    if (!tf) {
                        tf = temp[field] = {};
                        tf[row[field]] = [i];
                    } else {
                        var tfv = tf[row[field]];
                        if (tfv) {
                            tfv.push(i);
                        } else {
                            tfv = tf[row[field]] = [i];
                        }
                    }
                }
            }
            $.each(temp, function (field, colunm) {
                $.each(colunm, function () {
                    var group = this;

                    if (group.length > 1) {
                        var before,
						after,
						megerIndex = group[0];
                        for (var i = 0; i < group.length; i++) {
                            before = group[i];
                            after = group[i + 1];
                            if (after && (after - before) == 1) {
                                continue;
                            }
                            var rowspan = before - megerIndex + 1;
                            if (rowspan > 1) {
                                target.datagrid('mergeCells', {
                                    index: megerIndex,
                                    field: field,
                                    rowspan: rowspan
                                });
                            }
                            if (after && (after - before) != 1) {
                                megerIndex = after;
                            }
                        }
                    }
                });
            });
        });
    }
});


$.extend($.fn.datagrid.defaults.editors, {
    textreadonly: {
        init: function (t, a) {
            var i = $('<input type="text" class="datagrid-editable-input bgcolor-gray" readonly="readonly"/>').appendTo(t);

            return i
        },
        destroy: function (e) {
            $(e).remove()
        },
        getValue: function (e) {
            return $(e).val()
        },
        setValue: function (e, t) {
            $(e).val(t);
            setTimeout(function () {
                e.focus()
            },
            100)
        },
        resize: function (e, t) {
            $(e[0]).width(t - 15);
        }
    },
    textevent: {
        init: function (t, a) {
            //setKeyUpValue自行逻辑
            var i = $('<input type="text" class="datagrid-editable-input" onkeyup="setKeyUpValue()"/>').appendTo(t);
            return i
        },
        destroy: function (e) {
            $(e).remove()
        },
        getValue: function (e) {
            return $(e).val()
        },
        setValue: function (e, t) {
            $(e).val(t);
            setTimeout(function () {
                e.focus()
            },
            100)
        },
        resize: function (e, t) {
            $(e[0]).width(t - 15);
        }
    },
    seltext: {
        init: function (t, a) {
            //设置一个输入框和一个扩大镜图标
            var i = $('<input type="text" class="datagrid-editable-input bgcolor-gray"  readonly="readonly" />&nbsp;<a href="javascript:SelDetails(\'' + a.colField+'\')" class="mdi mdi-map-search-outline color-black"></a>').appendTo(t);
            if (a != undefined && a != null && a._medg) {
                i.keydown(function (t) {
                    e(t, a._medg)
                })
            }
            return i
        },
        destroy: function (e) {
            //销毁
            $(e).remove()
        },
        getValue: function (e) {
            //datagrid 结束编辑模式，通过该方法返回编辑最终值
            return $(e).val()
        },
        setValue: function (e, t) {
            //datagrid 进入编辑器模式，通过该方法为编辑赋值
            $(e).val(t);
            setTimeout(function () {
                e.focus()
            },
            100)
        },
        resize: function (e, t) {
            //列宽改变后调整编辑器宽度
            $(e[0]).width(t - 30);
        }
    },
    textarea: {
        init: function (t, a) {
            var i = $('<textarea class="datagrid-editable-input"></textarea>').appendTo(t);
            if (a != undefined && a != null && a._medg) {
                i.keydown(function (t) {
                    e(t, a._medg)
                })
            }
            return i
        },
        destroy: function (e) {
            $(e).remove()
        },
        getValue: function (e) {
            return $(e).val()
        },
        setValue: function (e, t) {
            $(e).val(t);
            setTimeout(function () {
                e.focus()
            },
            100)
        },
        resize: function (e, t) {
            e.outerWidth(t);
            e.outerHeight(e.parents("td[field]").height())
        }
    },
    checkbox: {//调用名称
        init: function (container, options) {
            //container 用于装载编辑器 options,提供编辑器初始参数
            var input = $('<input type="checkbox" class="datagrid-editable-input">').appendTo(container);
            //这里我把一个 checkbox类型的输入控件添加到容器container中
            // 需要渲染成easyu提供的控件，需要时用传入options,我这里如果需要一个combobox，就可以 这样调用 input.combobox(options);
            return input;
        },
        getValue: function (target) {
            //datagrid 结束编辑模式，通过该方法返回编辑最终值
            //这里如果用户勾选中checkbox返回1否则返回0
            return $(target).prop("checked") ? 1 : 0;
        },
        setValue: function (target, value) {
            //datagrid 进入编辑器模式，通过该方法为编辑赋值
            //我传入value 为0或者1，若用户传入1则勾选编辑器
            if (value)
                $(target).prop("checked", "checked")
        },
        resize: function (target, width) {
            //列宽改变后调整编辑器宽度
            var input = $(target);
            if ($.boxModel == true) {
                input.width(width - (input.outerWidth() - input.width()));
            } else {
                input.width(width);
            }
        }
    }

});
/*
*  datagrid 获取正在编辑状态的行，使用如下：
*  $('#id').datagrid('getEditingRowIndexs'); //获取当前datagrid中在编辑状态的行编号列表
*/
$.extend($.fn.datagrid.methods, {
    getEditingRowIndexs: function (jq) {
        var rows = $.data(jq[0], "datagrid").panel.find('.datagrid-row-editing');
        var indexs = [];
        rows.each(function (i, row) {
            var index = row.sectionRowIndex;
            if (indexs.indexOf(index) == -1) {
                indexs.push(index);
            }
        });
        return indexs;
    }
});

//带清除图标的文本框
$.extend($.fn.textbox.methods, {
    addClearBtn: function (jq, iconCls) {
        return jq.each(function () {
            var t = $(this);
            var opts = t.textbox('options');
            opts.icons = opts.icons || [];
            opts.icons.unshift({
                iconCls: iconCls,
                handler: function (e) {
                    $(e.data.target).textbox('clear').textbox('textbox').focus();
                    $(this).css('visibility', 'hidden');
                }
            });
            t.textbox();
            if (!t.textbox('getText')) {
                t.textbox('getIcon', 0).css('visibility', 'hidden');
            }
            t.textbox('textbox').bind('keyup', function () {
                var icon = t.textbox('getIcon', 0);
                if ($(this).val()) {
                    icon.css('visibility', 'visible');
                } else {
                    icon.css('visibility', 'hidden');
                }
            });
        });
    }
});


$.extend($.fn.treegrid.methods, {
    //iscontains == 1返回真选节点
    //iscontains == 0即子节点被选中时是否也取未全选的父节点
    //iscontains ==-1是否包含父节点（即子节点被选中时是否也取父节点）
    getAllChecked: function (jq, iscontains) {
        var keyValues = new Array();
        /*
          tree-checkbox4 有子节点被选中的css（禁用状态）
          tree-checkbox3 有子节点未选中的css（禁用状态）
          tree-checkbox2 有子节点被选中的css
          tree-checkbox1 节点被选中的css
          tree-checkbox0 节点未选中的css
        */
        if (iscontains == 1 || iscontains==-1) {
            var checkNodes = jq.treegrid("getPanel").find(".tree-checkbox1");
            for (var i = 0; i < checkNodes.length; i++) {
                var keyValue1 = $($(checkNodes[i]).closest('tr')[0]).attr("node-id");
                keyValues.push(keyValue1);
            }
        }
        if (iscontains == 0 || iscontains == -1) {
            var childCheckNodes = jq.treegrid("getPanel").find(".tree-checkbox2");
            for (var i = 0; i < childCheckNodes.length; i++) {
                var keyValue2 = $($(childCheckNodes[i]).closest('tr')[0]).attr("node-id");
                keyValues.push(keyValue2);
            }
        }
        if (iscontains == -1) {
            var checkNodes = jq.treegrid("getPanel").find(".tree-checkbox4");
            for (var i = 0; i < checkNodes.length; i++) {
                var keyValue3 = $($(checkNodes[i]).closest('tr')[0]).attr("node-id");
                keyValues.push(keyValue3);
            }
        }
        return keyValues;
    }
});