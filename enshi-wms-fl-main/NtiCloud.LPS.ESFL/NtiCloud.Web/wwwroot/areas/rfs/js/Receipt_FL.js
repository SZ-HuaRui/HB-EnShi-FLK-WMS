var brandInfo = {};
var bomInfo = {};
var supplierInfo = {};
//记录所有拆分物料的数据
var itemSelect = {
    data: [],
    index: 0
}
//记录当前供应商列表数据
var temSupplierData = []

$.fn.extend({
    setHighLight: function () {
        $('tr', this).click(function () {
            $(this).css('background', '#dedede').siblings('tr').css('background', '');
            $(this).attr('isSelected', '1').siblings('tr').attr('isSelected', '0');
        });
    }
});
$(function () {
    init();
})
function init() {
    var token = common.getToken();
    if (!token) {
        alert('请重新登陆！');
        return;
    }
    getBrand();
    //获取区域信息
    getZoneData()
    $("#brand_name").change(function () { brandOnChange() });
    $("#bom_code").change(function () { bomOnChange() });
    $("#item_qty").change(function () { QtyOnChange() });
    $("#supplier").change(function () {
        supplierOnChange('supplier')
    });
    $("#select_supplier").change(function () {
        supplierOnChange('select_supplier', 'select_')
    });
}
//获取区域信息
function getZoneData() {
    var token = common.getToken();
    common.postGateway("wms/ComposeContainer/GetZone", {}, token,
        function (data) {
            if (data.errcode == 0 && data.errmsg == "OK") {
                var result = JSON.parse(data.json);
                var len = result.length;
                var selectHtml = '';
                if (len >= 1) {
                    for (var i = 0; i < len; i++) {
                        selectHtml += '<option value="' + result[i].t_id + '">' + result[i].zone_name
                            + '</option>';
                    }
                    $('#zone_id').html('');
                    $('#zone_id').append(selectHtml);
                }
            }

        })

}
//供应商号变化 导致转换率变化
function supplierOnChange(domId, prevStr) {
    //var supplierId = $('#'+domId).val();
    //var rate = $('#' + domId +'  option:checked').attr('rate')
    renderConvertText(prevStr)
}
//生产牌号变化
function brandOnChange() {
    var itemId = $('#brand_name option:checked').val();
    brandInfo = {}
    itemSelect = {
        data: [],
        index: 0
    }
    $('#bom_table').html('')
    if (brandInfo.hasOwnProperty(itemId)) {
        render('brand', brandInfo[itemId]);
        render('bom', $('#bom_code option:checked').val());

    } else {
        getBomHeader(itemId)
    }
}
//配盘单号变化
function bomOnChange() {
    var bomId = $('#bom_code option:checked').val();
    $('#bom_table').html('')
    itemSelect = {
        data: [],
        index: 0
    }
    if (bomInfo.hasOwnProperty(bomId)) {
        render('bom', bomId);
    } else {
        alert('error');
    }
}



function QtyOnChange() {
    renderConvertText();
}

function render(type, obj) {
    switch (type) {
        case 'brand':
            renderBrand(obj)
            break;
        case 'bom':
            renderBom(obj)
            break;
        default:
    }
}
function renderBrand(bomArray) {
    $('#bom_code').html('');
    var selectHtml = '';
    for (var i = 0; i < bomArray.length; i++) {
        selectHtml += '<option value="' + bomArray[i] + '">' + bomInfo[bomArray[i]].bom_code + '</option>';
    }
    $('#bom_code').append(selectHtml);
}
function renderBom(bomId) {
    if (bomId) {
        $('#brand_code').val(bomInfo[bomId].brand_code);
        $('#model_name').val(bomInfo[bomId].section_code);
        renderBomDetail(bomId)
    } else {
        $('#brand_code').val('');
        $('#model_name').val('')
    }
}
//渲染物料详情
function renderBomDetail(id) {
    delete bomInfo[id].json
    if (bomInfo[id].hasOwnProperty('json')) {
        renderTable(bomInfo[id].json)   
    } else {
        getBomDetail(id);
    }
}
//渲染表格的行
function renderTable(json) {
    $('#bom_table').html('');
    var tbHtml = '<tr id="header"  supplier_id="default"><th>物料编码</th><th>物料名称</th><th>数量</th><th>单位</th><th>供应商</th><th>批次</th><th>转换率</th></tr>';
    if (json && json.length > 0) {
        for (var i = 0; i < json.length; i++) {
            tbHtml += '<tr id ="' + json[i].item_id +
                '" supplier_id="' + json[i].supplier +
                '" bom_qty="' + json[i].qty +
                '" index="' + i +
                '" uom_id="' + json[i].uom_id + '"><td>' +
                json[i].item_code + '</td><td>' +
                json[i].item_name + '</td><td>' +
                json[i].qty + '</td><td>' +
                json[i].uom_name + '</td><td>' +
                json[i].supplier_name + '</td><td>' +
                json[i].lot + '</td><td>' +
                json[i].rate + '</td></tr>'
        }
    }
    $('#bom_table').append(tbHtml);
    $("#bom_table").setHighLight();
}
function renderEditTab(data) {
    var doms = $('.tab2 .main_panel .bom_text,.tab2 .main_panel .bom_select')
    compareAndAssign(data, doms)

    $('.tab').css("display", "none");
    $('.tab2').css("display", "block");

    var itemId = data.item_id;
    getLotData('lot', itemId, data.lot)
    getSupplierData('supplier', itemId, '', data.supplier)
}

/**
 * 自动根据id匹配data中的属性赋值
 * @param {any} data    数据对象
 * @param {any} doms    节点数组
 * @param {any} isSplit 布尔变量 是否需要根据字段名去除第一个下划线以及其前面部分的字符
 */
function compareAndAssign(data, doms, isSplit) {
    for (var i = 0, j = doms.length; i < j - 1; i++) {
        var idStr = doms[i].getAttribute('id')
        var fieldStr = idStr
        if (isSplit) {
            fieldStr = idStr.replace(/^\w+?_/, '')
        }
        if (data[fieldStr]) {
            $('#' + idStr).val(data[fieldStr])
        }
    }
}

/**
 * 异步获取供应商号数据并赋值
 * @param {any} domId   节点id
 * @param {any} itemId  物料id
 * @param {any} prevStr  prevStr为id属性的前缀
 * @param {any} selectedData  默认选中的value
 */
function getSupplierData(domId, itemId, prevStr, selectedData) {
    domId = '#' + domId
    $(domId).html('');

    var token = common.getToken();
    if (!token) {
        alert('请重新登陆！');
        return;
    }

    common.postGateway("wms/ComposeContainer/GetItemSupplier", {
        "itemId": itemId
    }, token, function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            var result = JSON.parse(data.json);
            temSupplierData = result  //记录当前供应商的下拉数据
            var len = result.length;
            var selectHtml = '';
            if (len >= 1) {
                for (var i = 0; i < len; i++) {
                    var selectedStr = result[i].supplier_id == selectedData ? 'selected=true' : ''
                    selectHtml += '<option value="' + result[i].supplier_id + '"' + selectedStr + ' rate="' + result[i].rate + '">' + result[i].full_name + '</option>';
                    if (!supplierInfo.hasOwnProperty(itemId)) {
                        supplierInfo[itemId] = {};
                    }
                    if (!supplierInfo[itemId].hasOwnProperty(result[i].supplier_id)) {
                        supplierInfo[itemId][result[i].supplier_id] = result[i].rate;
                    }
                }
                $(domId).append(selectHtml);
                renderConvertText(prevStr)
            }
        }
    });
}

/**
 * 异步获取批次号数据并赋值
 * @param {any} domId 节点id
 * @param {any} itemId  物料id
 * @param {any} selectedData  默认选中的value
 */
function getLotData(domId, itemId, selectedData) {
    domId = '#' + domId
    $(domId).html('');

    var token = common.getToken();
    if (!token) {
        alert('请重新登陆！');
        return;
    }
    common.postGateway("wms/ComposeContainer/GetItemLot", {
        "itemId": itemId
    }, token, function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            var result = JSON.parse(data.json);
            var len = result.length;
            var selectHtml = '';
            if (len >= 1) {
                for (var i = 0; i < len; i++) {
                    var selectedStr = result[i].lot_no == selectedData ? 'selected=true' : ''
                    selectHtml += '<option value="' + result[i].lot_no + '"' + selectedStr + '>' + result[i].lot_no + '</option>';
                }
                $(domId).html('');
                $(domId).append(selectHtml);
            }
        }
    });
}

//渲染对应配盘单号的物料数据 便于拆分
function renderItemSelect(data) {
    var doms = $('#tab3 .main_panel .bom_text,#tab3 .main_panel .bom_select')
    //自动匹配赋值
    compareAndAssign(data, doms, true)

    var itemId = data.item_id;
    getLotData('select_lot', itemId)
    getSupplierData('select_supplier', itemId, 'select_')

}

//拆分时点击下一物料执行的方法
function goNext(isOver) {
    var actionType = isOver ? 'over' : 'next'
    var temObj = collectData(actionType)
    if (!temObj) {
        return false
    }

    if (!isOver) {
        itemSelect.index++
        goItem(itemSelect.index)
    }
}
//完成拆分 返回配盘初始页面
function goOver() {
    goNext(true)
    cancel()
    renderTable(itemSelect.data)
}
//批次拆分按钮的点击事件
function logSplit() {

    var temObj = collectData('split')
    if (!temObj) {
        return false
    }

    var leftQty = computeMaxQty(temObj.item_id, parseFloat($('#select_bom_qty').val()))
    $('#select_max_qty').val(leftQty)
    $('#select_item_qty').val(leftQty)
}
//切换到物料拆分页面
function goItem(index) {
    itemSelect.index = index

    $('.tab').css("display", "none");
    $('#tab3').css("display", "block");
    var bom_id = $('#bom_code option:checked').val();
    var data = bomInfo[bom_id] ? bomInfo[bom_id].json : null
    if (!data) { return false; }
    if (index == data.length - 1) {
        $('#goOver').show().siblings('#goNext').hide()
    } else {
        $('#goNext').show().siblings('#goOver').hide()
    }
    if (index == 0) {
        itemSelect = {
            data: [],
            index: 0
        }
    }
    var targetData = data[index]
    targetData.bom_qty = targetData.qty
    targetData.max_qty = computeMaxQty(targetData.item_id, targetData.qty)
    targetData.item_qty = targetData.max_qty
    targetData.item_uom = targetData.uom_name

    renderItemSelect(targetData)  
}
//统计对比修改数量和标配数量 计算最大可修改数量
function computeMaxQty(itemId, bomQty) {
    var maxNum = 0;
    $.each(itemSelect.data, function (i, val) {
        if (val.item_id == itemId) {
            maxNum += val.qty
        }
    })
    if (bomQty - maxNum < 0) {
        alert('该物料修改数量总和大于标配数量!')
        return 0
    }
    return bomQty - maxNum
}
//判断并收集物料拆分页面数据
function collectData(actionType) {
    var select_supplier_id = $('#select_supplier').val()
    var select_supplier_name = ''
    $.each(temSupplierData, function (i, item) {
        if (item.supplier_id == select_supplier_id) {
            select_supplier_name = item.full_name
        }
    })

    var temObj = {
        item_id: $('#select_item_id').val(),
        uom_id: $('#select_uom_id').val(),
        uom_name: $('#select_item_uom').val(),
        item_code: $('#select_item_code').val(),
        item_name: $('#select_item_name').val(),
        qty: parseFloat($('#select_item_qty').val()),
        supplier: select_supplier_id,
        supplier_name: select_supplier_name,
        lot: $('#select_lot').val(),
        rate: parseFloat($('#select_rate').attr('rate'))
    }
    //判断修改数量未填
    if (isNaN(temObj.qty)) {
        alert('请填写修改数量！')
        return false
    }
    //判断修改数量不得大于该物料的未修改数量
    var leftQty = parseFloat($('#select_max_qty').val())
    if (temObj.qty > leftQty) {
        alert('修改数量应小于等于该物料的未修改数量(' + leftQty + ')！')
        return false
    }

    if (temObj.qty == 0) {  //判断修改数量为0
        if (actionType == 'next') {
            itemSelect.index++
            goItem(itemSelect.index)
            return false
        } else if (actionType == 'split') {
            alert('用户数量为0，不可拆分！')
        }
        return false
    }
    //判断供应商号和批次号码是否完全重复
    if (judgeRepeat(temObj)) {
        return false
    }

    itemSelect.data.push(temObj)
    return temObj
}

//遍历判断 批次号码和供应商号都相等  targetIdx为自身在数组中的位置，为了排除自身
function judgeRepeat(temObj, targetIdx) {
    var isRepeat = false
    $.each(itemSelect.data, function (i, val) {
        if (i == targetIdx) {
            return false
        }
        if (temObj.item_id == val.item_id && temObj.lot == val.lot && temObj.supplier == val.supplier) {
            isRepeat = true
        }
    })
    if (isRepeat) {
        alert('该物料已存在批次号码和供应商号都相等的批次！')
    }
    return isRepeat
}

//算转换率 prevStr为id属性的前缀
function renderConvertText(prevStr) {
    prevStr = prevStr ? prevStr : ''
    //获取供应商id
    var item_qty = parseFloat($('#' + prevStr + 'item_qty').val());
    var bom_qty = parseFloat($('#' + prevStr + 'bom_qty').val());
    if (!(/(^[1-9]\d*$)/.test(item_qty))) {
        alert("请输入正确的数量！");
        return;
    }
    if (item_qty > bom_qty) {
        alert("输入数量不能大于配盘数量！");
        return;
    }
    var itemId = $('#' + prevStr + 'item_id').val();
    var supplierId = $('#' + prevStr + 'supplier').val();
    var itemUom = $('#' + prevStr + 'item_uom').val();
    if (itemId && supplierId && itemUom) {
        var rate = parseFloat(supplierInfo[itemId][supplierId]);
        var str = (item_qty * rate) + ' ' + itemUom + ' （转换率：' + rate + '）';
        $('#' + prevStr + 'rate').attr('rate', rate).val(str);
    }
}
//获取所有牌号
function getBrand() {
    var token = common.getToken();
    if (!token) {
        alert('请重新登陆！');
        return;
    }
    common.postGateway("wms/ComposeContainer/GetItem", {
        "warehouseId": "1016319470013779968"
    }, token, function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            var result = JSON.parse(data.json);
            var len = result.length;
            var selectHtml = '';
            var isContinue = false;
            if (len >= 1) {
                for (var i = 0; i < len; i++) {
                    selectHtml += '<option value="' + result[i].item_id + '">' + result[i].item_name + '</option>';
                }
                isContinue = true;
            }
            if (isContinue) {
                $('#brand_name').append(selectHtml);
                var itemId = $('#brand_name option:checked').val();
                getBomHeader(itemId)
            }
        }
    });
}
//获取该牌号下所有配盘单
function getBomHeader(item) {
    var token = common.getToken();
    if (!token) {
        alert('请重新登陆！');
        return;
    }
    common.postGateway("wms/ComposeContainer/GetBomHeaderByItem", {
        "itemId": item
    }, token, function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            var result = JSON.parse(data.json);
            var len = result.length;
            var selectHtml = '';
            var isContinue = false;
            if (len >= 1) {
                for (var i = 0; i < len; i++) {
                    var itemId = result[i].item_id;
                    var tId = result[i].t_id;
                    if (!bomInfo.hasOwnProperty(tId)) {
                        bomInfo[tId] = {
                            "bom_code": result[i].bom_code,
                            "bom_name": result[i].bom_name,
                            "brand_code": result[i].item_code,
                            "brand_name": result[i].item_name,
                            "section_code": result[i].section_code
                        };
                    }
                    if (!brandInfo.hasOwnProperty(itemId)) {
                        brandInfo[itemId] = [];
                        brandInfo[itemId].push(tId);
                    }
                    else {
                        if (brandInfo[itemId].indexOf(tId) == -1) {
                            brandInfo[itemId].push(tId);
                        }
                    }
                    selectHtml += '<option value="' + result[i].t_id + '">' + result[i].bom_code + '</option>';
                }
                isContinue = true;
            } else {
                $('#bom_code').html('');
                renderBom('')
            }
            if (isContinue) {
                $('#bom_code').html('');
                $('#bom_code').append(selectHtml);
                var bom_id = $('#bom_code option:checked').val();
                renderBom(bom_id)
            }
        }
    });
}
//获取配盘单明细
function getBomDetail(bomId) {
    var token = common.getToken();
    if (!token) {
        alert('请重新登陆！');
        return;
    }
    common.postGateway("wms/ComposeContainer/GetBomDetailByItem", {
        "bomHeaderId": bomId
    }, token, function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            var result = JSON.parse(data.json);
            var len = result.length;
            if (len > 0) {
                if (!bomInfo[bomId].hasOwnProperty('json')) {
                    bomInfo[bomId].json = result;
                }
            }
            //renderTable(result)
        }
    });
}


function edit() {
    //获取选中行的内容
    var selectRow = $("tr[isSelected='1']");
    if (selectRow.length == 0) {
        alert("请先选中一行数据！")
        return false
    }
    var rowIdx = selectRow.attr('index')
    $('#select_index').val(rowIdx)

    var data = {};
    if (selectRow) {
        data.item_id = selectRow.attr('id');
        data.item_code = selectRow.children('td:eq(0)').html();
        data.item_name = selectRow.children('td:eq(1)').html();
        data.bom_qty = selectRow.attr('bom_qty');
        data.item_qty = selectRow.children('td:eq(2)').html();
        data.item_uom = selectRow.children('td:eq(3)').html();
        data.supplier = itemSelect.data[rowIdx].supplier
        data.lot = itemSelect.data[rowIdx].lot

        renderEditTab(data)
    }
}
function confirm_btn() {
    var barcode = $('#barcode').val();
    if (!barcode) {
        alert('请输入托盘条码！');
        return;
    }
    var zoneId = $("#zone_id").find("option:selected").val();
    if (!zoneId) {
        alert('请选择入库区域！');
        return;
    }

    var bomId = $('#bom_code').val();
    if (!bomId) {
        alert('请选择配盘单号！');
        return;
    }
    var len = $('#bom_table tr').length;
    if (len <= 1) {
        alert('该配盘单下无明细！');
        return;
    }
    var isEmpty = false;
    $('#bom_table tr').each(function () {
        var attr = $(this).attr('supplier_id');
        if (typeof (attr) == typeof undefined) {
            isEmpty = true;
            return false;
        }
    });
    if (isEmpty) {
        alert('请检查明细，存在批次号或供应商为空！');
        return;
    }
    //获取数据
    var token = common.getToken();
    if (!token) {
        alert('请重新登陆！');
        return;
    }
    var json = getJsonFromTable();
    var param = {
        "BomId": bomId,
        "ZoneId": zoneId,
        "ContainerBarcode": barcode,
        ContainerDetailList: json
    };

    common.postGateway("wms/ComposeContainer/Compose", param, token, function (data) {
        if (data.errcode == 0 && data.errmsg == "OK") {
            alert('配盘成功！');
            $('#barcode').val('');
            $('#barcode').focus();
        } else {
            alert(data.errmsg);
        }
    });
}
function getJsonFromTable() {
    var json = [];
    $('#bom_table tr').each(function (i) {
        if (i == 0) return true;
        var row = {};
        row["ItemId"] = $(this).attr('id');
        row["ItemCode"] = $(this).children('td:eq(0)').html();
        row["ItemName"] = $(this).children('td:eq(1)').html();
        row["Qty"] = parseFloat($(this).children('td:eq(2)').html());
        row["UomId"] = $(this).attr('uom_id');
        row["SupplierId"] = $(this).attr('supplier_id');
        row["Lot"] = $(this).children('td:eq(5)').html();
        row["Rate"] = parseFloat($(this).children('td:eq(6)').html());
        json.push(row);
    });
    return json;
}
function update() {
    //获取参数值
    var itemId = $('#item_id').val();
    var supplierId = $('#supplier').val();
    var lot = $('#lot').val();
    var supplierName = $('#supplier option:checked').text();
    var item_qty = $('#item_qty').val();
    //赋值
    if (!itemId || !supplierId || !lot || !supplierName) {
        alert('请填写完整记录！');
        return;
    }
    var temObj = {
        item_id: itemId,
        lot: lot,
        supplier: supplierId
    }
    //判断供应商号和批次号码是否完全重复
    if (judgeRepeat(temObj, $('#select_index').val())) {
        return false
    }

    var rate = supplierInfo[itemId][supplierId];
    var selectRow = $("tr[isSelected='1']");
    selectRow.attr('supplier_id', supplierId);
    selectRow.children('td:eq(2)').html(item_qty);
    selectRow.children('td:eq(4)').html(supplierName);
    selectRow.children('td:eq(5)').html(lot);
    selectRow.children('td:eq(6)').html(rate);

    var selectedRowIdx = $('#select_index').val()
    itemSelect.data[selectedRowIdx].qty = item_qty
    itemSelect.data[selectedRowIdx].lot = lot
    itemSelect.data[selectedRowIdx].supplier = supplierId
    itemSelect.data[selectedRowIdx].supplier_name = supplierName
    itemSelect.data[selectedRowIdx].rate = rate

    cancel()
}
function cancel() {
    $('.tab').css("display", "none");
    $('.tab1').css("display", "block");
}