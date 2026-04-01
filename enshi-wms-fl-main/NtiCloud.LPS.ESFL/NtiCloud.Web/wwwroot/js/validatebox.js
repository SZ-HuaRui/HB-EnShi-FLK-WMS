//扩展easyui表单的自定义验证  
$.extend($.fn.validatebox.defaults.rules, {
    //验证邮箱地址  
    MailBox: {//value值为文本框中的值  
        validator: function (value) {
            var reg = /^[A-Za-z\d]+([-_.][A-Za-z\d]+)*@([A-Za-z\d]+[-.])+[A-Za-z\d]{2,4}$/;
            return reg.test(value);
        },
        message: '请输入合法的邮箱地址'
    },
    //移动手机号码验证  
    MobileNo: {//value值为文本框中的值  
        validator: function (value) {
            var reg = /^(((13[0-9]{1})|(14[0-9]{1})|(17[0-9]{1})|(15[0-3]{1})|(15[4-9]{1})|(18[0-9]{1})|(199))+\d{8})$/;
            return reg.test(value);
        },
        message: '请输入合法的手机号码'
    },
    //非负整数
    Integer: {
        validator: function (value) {
            var reg = /^[1-9]\d*|0$/;
            return reg.test(value);
        },
        message: '请输入合法的非负整数'
    },
    //验证汉字  
    CHS: {
        validator: function (value) {
            return /^[\u0391-\uFFE5]+$/.test(value);
        },
        message: '请输入合法的汉字'
    },
    //国内邮编验证  
    ZipCode: {
        validator: function (value) {
            var reg = /^[0-9]\d{5}$/;
            return reg.test(value);
        },
        message: '请输入合法的邮政编码'
    },
    //数字  
    Number: {
        validator: function (value) {
            var reg = /^[0-9]*$/;
            return reg.test(value);
        },
        message: '请输入合法的数字'
    },
    English: {
        validator: function (value) {
            var reg = /^[a-zA-Z\$_][a-zA-Z\d\.\-_]*$/;
            return reg.test(value);
        },
        message: '请输入合法的英文字母'
    },
    PassWordCheck: {
        validator: function (value) {
            //var reg = new RegExp('(?=.*[0-9])(?=.*[a-zA-Z])(?=.*[^a-zA-Z0-9]).{8,30}');
            var reg = /^(?![a-zA-Z]+$)(?![A-Z0-9]+$)(?![A-Z\W_!@#$%^&*`~()-+=\/]+$)(?![a-z0-9]+$)(?![a-z\W_!@#$%^&*`~()-+=\/]+$)(?![0-9\W_!@#$%^&*`~()-+=\/]+$)[a-zA-Z0-9\W_!@#$%^&*`~()-+=\/]{8,30}$/;
            return reg.test(value);
        },
        message: '密码强度不够，不符合三级等保要求！<br>1、密码长度大于等于8个字符；<br>2、必须是大写字母，小写字母，数字，特殊字符任意三个组合!'
    }
})