var newSql = "";
$(function () {
    //修改滚动条的样式
    $('#deskTop').perfectScrollbar();
    var searchSql = "select ifnull(sum(case when IS_CLOSED='0' then 1 else 0 end),0) upcoming,ifnull(sum(case when IS_CLOSED='1' then 1 else 0 end),0) done from prod_workflow_task_action where CLIENT_ID='&CLIENT_ID&'  and user_id='&User_Tid&'"
        + ";select count(t_id) product from prod_app_product union all select count(t_id) project from prod_common_project";
    if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "Oracle") {
        searchSql += ";select count(*) alluser, nvl(sum(case when userlogindt > (SYSDATE -interval '3' hour) then 1 else 0 end),0) on_line from prod_system_user"
        //searchSql += ";select count(t_id) m1,nvl(sum(case when userlogindt > (SYSDATE -interval '24' hour) then 1 else 0 end),0) m2,nvl(sum(case when userlogindt > (SYSDATE -interval '240' hour) then 1 else 0 end),0) m3,sum(case when USERLOGINNUM=0 then 1 else 0 end) n1,sum(case when USERLOGINNUM>0 and USERLOGINNUM<=10  then 1 else 0 end) n2,sum(case when USERLOGINNUM>10 and USERLOGINNUM<=100  then 1 else 0 end) n3,sum(case when USERLOGINNUM>100 and USERLOGINNUM<=1000  then 1 else 0 end) n4,sum(case when USERLOGINNUM>1000 and USERLOGINNUM<10000  then 1 else 0 end) n5,sum(case when USERLOGINNUM>10000  then 1 else 0 end) n6 from prod_system_user "
    }
    else if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "MySql") {
        searchSql += ";select count(*) alluser, ifnull(sum(case when userlogindt > DATE_SUB(NOW(),INTERVAL 3 HOUR) then 1 else 0 end),0) on_line from prod_system_user where CLIENT_ID='&CLIENT_ID&'";
        //searchSql += ";select count(t_id) m1,ifnull(sum(case when userlogindt > DATE_SUB(NOW(),INTERVAL 24 HOUR) then 1 else 0 end),0) m2,ifnull(sum(case when userlogindt > DATE_SUB(NOW(),INTERVAL 240 HOUR) then 1 else 0 end),0) m3,sum(case when USERLOGINNUM=0 then 1 else 0 end) n1,sum(case when USERLOGINNUM>0 and USERLOGINNUM<=10  then 1 else 0 end) n2,sum(case when USERLOGINNUM>10 and USERLOGINNUM<=100  then 1 else 0 end) n3,sum(case when USERLOGINNUM>100 and USERLOGINNUM<=1000  then 1 else 0 end) n4,sum(case when USERLOGINNUM>1000 and USERLOGINNUM<10000  then 1 else 0 end) n5,sum(case when USERLOGINNUM>10000  then 1 else 0 end) n6 from prod_system_user "
    }
    else if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "PostgreSql") {
        searchSql += ";select count(*) alluser, coalesce(sum(case when userlogindt > CURRENT_TIMESTAMP::TIMESTAMP - INTERVAL '3 HOUR' then 1 else 0 end),0) on_line from prod_system_user where CLIENT_ID='&CLIENT_ID&'";
        //searchSql += ";select count(t_id) m1,coalesce(sum(case when userlogindt > CURRENT_TIMESTAMP::TIMESTAMP - INTERVAL '24 HOUR' then 1 else 0 end),0) m2,coalesce(sum(case when userlogindt > CURRENT_TIMESTAMP::TIMESTAMP - INTERVAL '240 HOUR' then 1 else 0 end),0) m3,sum(case when USERLOGINNUM=0 then 1 else 0 end) n1,sum(case when USERLOGINNUM>0 and USERLOGINNUM<=10  then 1 else 0 end) n2,sum(case when USERLOGINNUM>10 and USERLOGINNUM<=100  then 1 else 0 end) n3,sum(case when USERLOGINNUM>100 and USERLOGINNUM<=1000  then 1 else 0 end) n4,sum(case when USERLOGINNUM>1000 and USERLOGINNUM<10000  then 1 else 0 end) n5,sum(case when USERLOGINNUM>10000  then 1 else 0 end) n6 from prod_system_user "
    }
    else {
        searchSql += ";select count(*) alluser, isnull(sum(case when userlogindt > DATEADD(hh,-3,GETDATE()) then 1 else 0 end),0) on_line from prod_system_user where CLIENT_ID='&CLIENT_ID&'";
        //searchSql += ";select count(t_id) m1,isnull(sum(case when userlogindt > DATEADD(hh,-24,GETDATE()) then 1 else 0 end),0) m2,isnull(sum(case when userlogindt > DATEADD(hh,-240,GETDATE()) then 1 else 0 end),0) m3,sum(case when USERLOGINNUM=0 then 1 else 0 end) n1,sum(case when USERLOGINNUM>0 and USERLOGINNUM<=10  then 1 else 0 end) n2,sum(case when USERLOGINNUM>10 and USERLOGINNUM<=100  then 1 else 0 end) n3,sum(case when USERLOGINNUM>100 and USERLOGINNUM<=1000  then 1 else 0 end) n4,sum(case when USERLOGINNUM>1000 and USERLOGINNUM<10000  then 1 else 0 end) n5,sum(case when USERLOGINNUM>10000  then 1 else 0 end) n6 from prod_system_user "
    }
    apiRoute.selectDataEvent(searchSql, [], "1=1", function (data) {
        $('.unfinish').text(data.Table1[0].upcoming);
        $('.finish').text(data["Table1"][0].done);
        $('.product').text(data.Table2[0].product + "/" + data.Table2[1].product);
        $('.online').text(data.Table3[0].on_line + "/" + data.Table3[0].alluser);
    })
    for (var i = 0; i < parseInt($("#newCount").val()); i++) {
        if ($("#newSql_" + i).val() != "") {
            newSql += $("#newSql_" + i).val();
        }
        else {
            newSql += "SELECT t_id,titleinfo title,CONTENTDETAIL note, create_date FROM prod_desktop_detail where p_id='" + $("#t_id_" + i).val()+"' order by create_date desc"
        }
        newSql += ";";
    }
    
    newSql += "select '' title from dual";
    apiRoute.selectDataEvent(newSql, [], "1=1", function (data) {
        for (var i = 0; i < parseInt($("#newCount").val()); i++) {
            if (data["Table" + (i + 1)].length == 0) {
                $("#html_" + i).html("<p><span>暂无任何信息！</span></p>");
            }
            else {
                $("#html_" + i).html('');
                for (var j = 0; j < data["Table" + (i + 1)].length; j++) {
                    if (data["Table" + (i + 1)][0].t_id && data["Table" + (i + 1)][0].title && data["Table" + (i + 1)][0].create_date) {
                        $("#html_" + i).append("<p><span title=\"" + data["Table" + (i + 1)][j].title + "\" onclick=detail('" + data["Table" + (i + 1)][j].t_id + "'," + i + ")>·" + data["Table" + (i + 1)][j].title + "</span><span>" + data["Table" + (i + 1)][j].create_date + "</span></p>");
                    }
                    else {
                        $("#html_" + i).append("<p><span title=\"" + (data["Table" + (i + 1)][j].title || data["Table" + (i + 1)][j].lefttext) + "\">·" + data["Table" + (i + 1)][j].lefttext + "</span><span>" + data["Table" + (i + 1)][j].righttext + "</span></p>");
                    }
                }
                $(".title_num_" + i).text("数量：" + data["Table" + (i + 1)].length);
                if (data["Table" + (i + 1)].length > 6) {
                    movedome("#html_" + i);
                }
            }
        }
    });
});

function detail(id, n) {
    apiRoute.selectDataEvent(newSql.split(';')[n], [],"t_id='"+id+"'", function (data) {
        $.messageBoxOpenID(data[0].title, ['90%', '90%'],'<div style="color: rgb(102, 102, 102); line-height:180%;padding:10px;">'+ data[0].note+'</div>');
    });
}
//循环垂直向上滚动
function movedome(objD) {
    var margintop = 0;//上边距的偏移量
    var stop = false;
    setInterval(function () {
        if (stop == true) {
            return;
        }
        $(objD).children("p").first().animate({ "margin-top": margintop-- }, 0, function () {
            var $p = $(this);
            if (!$p.is(":animated")) {//第一个p的动画结束时
                if (-margintop > $p.height()) {
                    $p.css("margin-top", "0px").appendTo($(objD));
                    margintop = 0;
                }
            }
        });
    }, 50);
    //鼠标放到信息上时
    $(objD).hover(function () {
        $(this).css({ "cursor": "pointer" });
        $(this).perfectScrollbar();
        stop = true;//停止动画
    }, function () {
        $(this).css("overflow", "hidden");
        stop = false;//开始动画
    });
}     