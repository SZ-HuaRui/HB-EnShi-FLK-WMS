$(function () {

    var iwmsData = [
        {
            title: '基础管理',
            test: ['仓库/区域/库位', '用户/角色/权限', '贸易伙伴', '货类', '货品'],
            standard: ['体验版功能', '客户', '供应商', '承运商'],
            enterprise: ['标准版功能']
        },
        {
            title: '入库管理',
            test: ['到货通知单', '按单收货'],
            standard: ['体验版功能', '采购订单', '容器上架', '货品上架'],
            enterprise: ['标准版功能']
        },
        {
            title: '出库管理',
            test: ['出库订单', '按单出库'],
            standard: ['体验版功能', '库存分配', '拣货', '订单打印', '包装复核'],
            enterprise: ['标准版功能', '波次机会', '复核称重', '包装校验']
        },
        {
            title: '库存查询',
            test: ['库存查询'],
            standard: ['库存查询'],
            enterprise: ['库存查询']
        },
        {
            title: '统计分析',
            test: ['入库报表', '出库报表', '库存报表'],
            standard: ['体验版功能'],
            enterprise: ['标准版功能', '绩效报表']
        },
        {
            title: '任务管理',
            test: ['任务管理'],
            standard: ['任务管理'],
            enterprise: ['任务管理']
        },
        {
            title: '库存管理',
            test: [],
            standard: ['库存调整', '库存移动', '库存冻结', '库存释放'],
            enterprise: ['标准版功能', '盘点管理', '批次库存', '效期预警']
        },
        {
            title: '业务规则',
            test: [],
            standard: [],
            enterprise: ['上架规则', '周转规则', '分配规则', '波次规则', '补货规则']
        },
        {
            title: '增值服务',
            test: [],
            standard: [],
            enterprise: ['加工单', '月台管理', '员工绩效考核', '电子看板', '图形化库存']
        },
        {
            title: '接口管理',
            test: [],
            standard: [],
            enterprise: ['ERP系统对接', '面单系统对接', '物流设备接口']
        }
    ];
    //遍历加载数据
    $.each(iwmsData, function (i) {
        var firstClass = '';
        if (i == 0) { firstClass = 'first'; }
        $('.con_wrapper').append('<ul class="ul_con ' + firstClass + '"><li class="li_title"><span>' + iwmsData[i].title + '</span></li></ul>');
        var newObj = this;
        for (var key in newObj) {
            if (key != 'title') {
                var centerClass = '';
                if (key == 'standard') { centerClass = 'center'; }
                $('.con_wrapper .ul_con').eq(-1).append('<li class="li_con ' + centerClass + '"></li>');
                if (newObj[key].length != 0) {
                    $.each(newObj[key], function () {
                        $('.con_wrapper .ul_con').eq(-1).find('.li_con').eq(-1).append('<span>' + this + '</span>');
                    });
                };
            };
        };
    });


    //调整内容的垂直显示位置
    $('.ul_con').each(function () {
        var ul_height = $(this).height();
        $(this).find('li').each(function () {
            $(this).css('margin-top', (ul_height - $(this).height()-30)/2 + 'px')
        })

    })


})

