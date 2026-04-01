$(function () {
    $('body').css('height', $(window).height());
    $('.account_info_d input[type="text"]').css('width', $('.account_info').width() - 24 * 2 - 100 - 2);
    $(window).resize(function () {
        $('body').css('height', $(window).height());
        $('.account_info_d input[type="text"]').css('width', $('.account_info').width() - 24 * 2 - 100 - 2);
    })
    var searchSql = "select USERID,JOBNUM,DingJSON,USERSEX, DINGID,WEIXINID,QQ,OFFICETEL,FAX,AREA,ADDRESS,ENGLISHNAME,BIRTHDAY,SCHOOL,EDUCATION,PROFESSION,BIRTHPLACE,USERNAME,USERMOB,USEREMAIL from prod_system_user where t_id='&User_Tid&'";
    if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "Oracle" || 
        JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "PostgreSql") {
        searchSql = "select USERID,JOBNUM,DingJSON,USERSEX, DINGID,WEIXINID,QQ,OFFICETEL,FAX,AREA,ADDRESS,ENGLISHNAME,to_char(BIRTHDAY,'yyyy-mm-dd') BIRTHDAY,SCHOOL,EDUCATION,PROFESSION,BIRTHPLACE,USERNAME,USERMOB,USEREMAIL from prod_system_user where t_id='&User_Tid&'";
    }
    else if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "MySql") {
        searchSql = "select USERID,JOBNUM,DingJSON,USERSEX, DINGID,WEIXINID,QQ,OFFICETEL,FAX,AREA,ADDRESS,ENGLISHNAME,date_format(BIRTHDAY,'%Y-%m-%d') BIRTHDAY,SCHOOL,EDUCATION,PROFESSION,BIRTHPLACE,USERNAME,USERMOB,USEREMAIL from prod_system_user where t_id='&User_Tid&'"
    }
    apiRoute.selectDataEvent(searchSql,[],"1=1", function (data) {
            if (data.length > 0) {
                var positionData = parseFloat(data[0].jobnum.replace(/[^0-9]/ig, ""));//入职日期
                $('.userName').text(data[0].userid == null ? "未填写" : data[0].userid);
                $('.userpswd').text(data[0].userpswd == null ? "未填写" : data[0].userpswd);
                $('.workNum').text(data[0].jobnum == null ? "未填写" : data[0].jobnum);
                $('.position').text(JSON.parse(data[0].dingjson).position == null ? "未填写" : JSON.parse(data[0].dingjson).position);
                $('.positionData').text(positionData == null ? "未填写" : positionData);
                $('.type').text(data[0].usergroup == null ? "未填写" : data[0].usergroup);
                $('.external_email_content').text(data[0].useremail == null ? "未填写" : data[0].useremail);
                $('.true_name_content').text(data[0].nickname == null ? "未填写" : data[0].nickname);
                $('.office_phone_content').text(data[0].officetel == null ? "未填写" : data[0].officetel);
                $('.fax_number_content').text(data[0].fax == null ? "未填写" : data[0].fax);
                $('.phone_number_content').text(data[0].usermob == null ? "未填写" : data[0].usermob);
                $('.qq_number_content').text(data[0].qq == null ? "未填写" : data[0].qq);
                $('.wechat_number_content').text(data[0].weixinid == null ? "未填写" : data[0].weixinid);
                $('.ding_number_content').text(data[0].dingid == null ? "未填写" : data[0].dingid);
                $('.area_content').text(data[0].area == null ? "未填写" : data[0].area);
                $('.detail_address_content').text(data[0].address == null ? "未填写" : data[0].address);
                $('.true_name_content').text(data[0].username == null ? "未填写" : data[0].username);
                $('.english_name_content').text(data[0].englishname == null ? "未填写" : data[0].englishname);
                $('.sex_content').text((data[0].usersex == 1 || data[0].usersex == null) ? "男" : "女");
                $('.school_content').text(data[0].school == null ? "未填写" : data[0].school);
                $('.birthday_content').text(data[0].birthday == null ? "未填写" : data[0].birthday);
                $('.major_content').text(data[0].profession == null ? "未填写" : data[0].profession);
                $('.native_place_content').text(data[0].birthplace == null ? "未填写" : data[0].birthplace);
                $('.education_content').text(data[0].education == null ? "未填写" : data[0].education);
            }
    })
    $('.edit').on('click', function () {
        $(this).hide();
        $(this).next().show();
        $(this).parent().next().find('.lable_content').hide();
        $(this).parent().next().find('input').show();
        $(this).parent().next().find('label').show();
        //给input框赋值 或默认选中
        $('.account_info_d input[type="text"]').each(function () {
            if ($(this).siblings('.lable_content').html() != '未填写') {
                $(this).val($(this).siblings('.lable_content').html());
            };
        });
        $('.account_info_d input[type="radio"]').each(function () {
            if ($(this).val() == $(this).parent().siblings('.lable_content').html()) {
                $(this).attr('checked', 'checked');
            };
        });
    })
    $('.finish').on('click', function () {
        $(this).hide();
        $(this).prev().show();
        $(this).parent().next().find('.lable_content').show();
        $(this).parent().next().find('input').hide();
        $(this).parent().next().find('label').hide();
        var sex = $("input[name='sex']:checked").val() == "男" ? 1 : 0;
        var jsonData = [{
            //"userid": $('.userName').text(),
            //"userpswd": $('.userpswd').text(),
            "dingid": $('.ding_number').val(),
            "weixinid": $('.wechat_number').val(),
            "qq": $('.wechat_number').val(),
            "officetel": $('.office_phone').val(),
            "fax": $('.fax_number').val(),
            "area": $('.area').val(),
            "address": $('.detail_address').val(),
            "englishname": $('.english_name').val(),
            "birthday": $('.birthday').val(),
            "school": $('.school').val(),
            "education": $('.education').val(),
            "profession": $('.major').val(),
            "birthplace": $('.native_place').val(),
            "username": $('.true_name').val(),
            "usermob": $('.phone_number').val(),
            "useremail": $('.external_email').val(),
            "usersex": sex
        }];
        $('.office_phone_content').text($('.office_phone').val());
        $('.fax_number_content').text($('.fax_number').val());
        $('.phone_number_content').text($('.phone_number').val());
        $('.qq_number_content').text($('.wechat_number').val());
        $('.wechat_number_content').text($('.wechat_number').val());
        $('.ding_number_content').text($('.ding_number').val());
        $('.external_email_content').text($('.external_email').val()); 
        //$('.internal_email_content').text($('.internal_email').val());
        $('.area_content').text($('.area').val());
        $('.detail_address_content').text($('.detail_address').val());
        $('.true_name_content').text($('.true_name').val());
        $('.english_name_content').text($('.english_name').val());
        $('.school_content').text($('.school').val());
        $('.birthday_content').text($('.birthday').val());
        $('.major_content').text($('.major').val());
        $('.native_place_content').text($('.native_place').val());
        $('.education_content').text($('.education').val());
        $('.sex_content').text($("input[name='sex']:checked").val());
        var birthdaySql = "";
        if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "Oracle") {
            birthdaySql = "BIRTHDAY =case when @birthday='' then null else to_date(@birthday, 'yyyy-MM-dd') end";
        }
        else if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "MySql") {
            birthdaySql = "BIRTHDAY =case when @birthday='' then null else str_to_date(@birthday, '%Y-%m-%d') end";
        }
        else {
            birthdaySql = "BIRTHDAY =case when @birthday='' then null else @birthday end";
        }
        var searchSql = "update prod_system_user set DINGID=@dingid,WEIXINID=@weixinid,QQ=@qq,USERSEX=@usersex,OFFICETEL=@officetel,FAX=@fax,AREA=@area,ADDRESS=@address,ENGLISHNAME=@englishname," + birthdaySql+",SCHOOL=@school,EDUCATION=@education,PROFESSION=@profession,BIRTHPLACE=@birthplace,USERNAME=@username,USERMOB=@usermob,USEREMAIL=@useremail where  t_id='&User_Tid&'";
        apiRoute.runTaskEvent("",searchSql, jsonData, function (data) {
            $.messageBoxAlert(data.resCount, "个人资料保存成功！！！");
        });
    })
})