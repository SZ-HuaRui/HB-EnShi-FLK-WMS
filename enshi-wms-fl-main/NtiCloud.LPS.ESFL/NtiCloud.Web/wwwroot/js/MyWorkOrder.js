var whereQuery = "1=1";
function data_from_to() {
    var data_from_p = $('.data_from_p').val();//开始时间
    var data_to_p = $('.data_to_p').val();//结束时间
    if (data_from_p && data_to_p) {
        if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "Oracle") {
            whereQuery = "1=1 and create_date>=to_date(" + data_from_p + ",'yyyy-mm-dd hh24:mi:ss') AND create_date<=to_date(" + data_to_p + ",'yyyy-mm-dd hh24:mi:ss')";
        }
        else {
            whereQuery = "1=1 and create_date>='" + data_from_p + "' AND create_date<='" + data_to_p + " 23:59:59'";
        }
    }
    else {
        whereQuery = "1=1"
    }
}


$(function () {
    layui.use('laydate', function () {
        var laydate = layui.laydate;
          //执行一个laydate实例
        laydate.render({
            elem: '#bdate' //指定元素
        });
        laydate.render({
            elem: '#edate' //指定元素
        });
    });


    data_from_to();
    $(".login_out").click(function () {
        LogOut();
    });

    DataList(1, whereQuery);

    $("#PrevPage").off('click').on('click', function () {
        var CurPage = $("#CurPage").text();
        if (CurPage > 1) {
            CurPage--;
            $("#NextPage").removeClass('disabled');
            $("#last").removeClass('disabled');
            if (CurPage == 1) {
                $("#PrevPage").addClass('disabled');
                $("#first").addClass('disabled');
            };
            $("#CurPage").text(CurPage);
            DataList(CurPage, whereQuery);
        };
    });

    $('#first').off('click').on('click', function () {
        //var totalData = $("#total").text();//总数据条数
        //var rowsNum = $("#rows").text();//每页数据条数
        //var pageNum = Math.ceil(totalData / rowsNum);//共有多少页
        $("#PrevPage").addClass('disabled');
        $("#first").addClass('disabled');
        $("#NextPage").removeClass('disabled');
        $("#last").removeClass('disabled');
        if ($("#CurPage").text() != 1) {
            DataList(1, whereQuery);
        }
        $("#CurPage").text(1);
        //if (pageNum > 1) {
        //    $("#NextPage").removeClass('disabled');
        //    $("#last").removeClass('disabled');
        //    DataList(1, whereQuery);
        //} else {
        //    $("#PrevPage").addClass('disabled');
        //    $("#first").addClass('disabled');
        //    $("#NextPage").addClass('disabled');
        //    $("#last").addClass('disabled');
        //}
        
        
        
    });

    $("#NextPage").off('click').on('click', function () {
        var CurPage = $("#CurPage").text();//当前第几页
        var totalData = $("#total").text();//总数据条数
        var rowsNum = $("#rows").text();//每页数据条数
        var pageNum = Math.ceil(totalData/rowsNum);//共有多少页
        if (CurPage < pageNum) {
            CurPage++;  
            $("#PrevPage").removeClass('disabled');
            $("#first").removeClass('disabled');
            if (CurPage == pageNum) {
                $("#NextPage").addClass('disabled');
                $("#last").addClass('disabled');
            };
            $("#CurPage").text(CurPage);
            DataList(CurPage, whereQuery);
        };
        
    });
    $('#last').off('click').on('click', function () {
        
        
        var totalData = $("#total").text();//总数据条数
        var rowsNum = $("#rows").text();//每页数据条数
        var pageNum = Math.ceil(totalData / rowsNum);//共有多少页
        $("#PrevPage").removeClass('disabled');
        $("#first").removeClass('disabled');
        $("#NextPage").addClass('disabled');
        $("#last").addClass('disabled');
        if ($("#CurPage").text() != pageNum) {
            DataList(pageNum, whereQuery);
        }
        $("#CurPage").text(pageNum);
        //if (pageNum > 1) {
        //    $("#PrevPage").removeClass('disabled');
        //    $("#first").removeClass('disabled');
        //    DataList(pageNum, whereQuery);
        //} else {
        //    $("#NextPage").addClass('disabled');
        //    $("#last").addClass('disabled');
        //    $("#PrevPage").addClass('disabled');
        //    $("#first").addClass('disabled');
        //}
        
    });
    //点击查询按钮
    $('.top_up').on('click', function () {
        var data_from_p = $('.data_from_p').val();//开始时间
        var data_to_p = $('.data_to_p').val();//结束时间
        if (data_from_p && (!data_to_p)) {
            $.messageBoxAlert(0, "请选择结束日期！");
        }
        else if ((!data_from_p) && data_to_p) {
            $.messageBoxAlert(0, "请选择开始日期！");
        }
        else {
            data_from_to();
            DataList(1, whereQuery);
        }
    })
    //点击关键字的查询图
    $('.key_word_img').on('click', function () {
        var key_word = $('.key_word').val();//输入关键字
        data_from_to();
        whereQuery = whereQuery + " And (appname like '%" + key_word + "%' or workorder like '%" + key_word + "%' or qatype like '%" + key_word + "%')";
        DataList(1, whereQuery);
    })

});

function DataList(page, queryStr) {
    $.messageLoad();
    apiRoute.selectEasyuiTable("",common.Encrypt("select a.t_id,a.orderid,b.APPNAME, c.dict_name QATYPE ,a.WORKORDER,a.WORKFLOW, d.dict_name WORKFLOWText,a.CREATE_DATE from prod_workorder_header a "
        + " left join prod_app_product b on a.PRODCODE = b.APPCODE "
        + " left join(select dict_code, dict_name from prod_system_dictionary where p_id = "
        + " (select t_id from prod_system_dictionary where dict_code = 'QATYPE')) c on a.QATYPE = c.dict_code "
        + " left join(select dict_code, dict_name from prod_system_dictionary where p_id ="
        + " (select t_id from prod_system_dictionary where dict_code = 'workOrder')) d on a.WORKFLOW = d.dict_code where a.userid='&User_Tid&'"), {
        "queryStr": queryStr,
        "pager": {
            sort: "t_id",
            order: "desc",
            page: page,
            rows: $("#rows").html()
        }
    }, function (gridList) {
        $.messageBoxCloseLoading();
        if (gridList.rows.length != $('#table_order tbody tr').length) {
            for (var i = 0; i < $('#table_order tbody tr').length; i++) {
                $('#table_order tbody tr').eq(i).find('td span').eq(0).html('');
                $('#table_order tbody tr').eq(i).find('td span').eq(1).html('');
                $('#table_order tbody tr').eq(i).find('td span').eq(2).html('');
                $('#table_order tbody tr').eq(i).find('td span').eq(3).attr('class', '').html('');
                $('#table_order tbody tr').eq(i).find('td span').eq(4).html('');
                $('#table_order tbody tr').eq(i).find('td').eq(5).html('');
            };
        };
        if (Math.ceil(gridList.total / (1 * $("#rows").html())) <=1) {//总页数为1 向下翻页也禁用
            $("#NextPage").addClass('disabled');
            $("#last").addClass('disabled');
        } else {
            //$("#NextPage").removeClass('disabled');
            //$("#last").removeClass('disabled');
        };
        for (var r = 0; r < gridList.rows.length; r++) {
            $('#table_order tbody tr').eq(r).find('td span').eq(0).html(gridList.rows[r].orderid);
            $('#table_order tbody tr').eq(r).find('td span').eq(1).html(gridList.rows[r].appname + "/" + gridList.rows[r].qatype);
            $('#table_order tbody tr').eq(r).find('td span').eq(2).html(gridList.rows[r].workorder);
            $('#table_order tbody tr').eq(r).find('td span').eq(3).attr('class', 'class_' + (gridList.rows[r].workflow * 1 + 1)).html(gridList.rows[r].workflowtext);
            $('#table_order tbody tr').eq(r).find('td span').eq(4).html(gridList.rows[r].create_date);
            $('#table_order tbody tr').eq(r).find('td').eq(5).html("<a href=\"/Member/MyWorkOrderDetail/" + gridList.rows[r].orderid + "\">查看</a>|<a href=\"javascript:;\" onclick=\"deleteOrder('" + gridList.rows[r].orderid + "')\">删除</a>");
        };
        $("#total").html(gridList.total);
        $("#CurPage").text(page);
    });
}

function deleteOrder(orderId) {
    $.messageLoad();
    apiRoute.runTaskEvent("","delete from prod_workorder_detail where orderid='" + orderId + "';delete from prod_workorder_header where orderid='" + orderId + "'", []
    , function () {
        DataList($("#CurPage").text(), whereQuery)
    });
}