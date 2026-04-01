$(function(){
    var selectLocation = "select loc.location_number,"+
                                 "loc.location_line,"+
                                 "loc.location_column,"+
                                 "loc.location_layer,"+
                                 "(floor(location_line / 2) * 5 + floor((location_line - 1) / 2) * 4 + loc.location_layer +mod(location_line, 2) * (4 - 2 * loc.location_layer)) index_no,"+
                                 "floor((7 - location_line) / 6) * 5 -floor(location_line / 6) * 7 + 7 + loc.location_column index_col,"+
                                 "loc.location_state stock_sts,"+
                                 "loc.location_flag useable_sts,"+
                                 "cap.max_qty - nvl(inv.qty, 0) remain_qty "+
                            "from swms_wi_location loc "+
                            "left join swms_m_location_inventory inv "+
                              "on loc.t_id = inv.location_id "+
                            "left join swms_bi_location_type cap "+
                              "on loc.location_type = cap.t_id "+
                           "where loc.warehouse_id = '1016319470013779968' "+
                             "and loc.location_number <> '0001' "+
                        "order by loc.location_number";
    common.postGateway('platform/database/selectsql', {
        "sql": common.Encrypt(selectLocation),
        "parameters": []
    }, common.getToken(),function (data) {
            if (data.errcode == 0 && data.errmsg == "OK") {
                var result = JSON.parse(data.json);
                var len = result.length;
                if (len > 0) {
                    matchData(result);
                }
            }
    });
    function matchData(rowObj) {
       
        var arrRes=[[],[],[],[],[]];
        for(var i=0;i<rowObj.length;i++){
            var that=rowObj[i];
            if(that.stock_sts=="LocationState_Empty"&&that.useable_sts=="Enable"){ 
                arrRes[0].push([parseInt(that.index_col),that.index_no,that.location,that.remain_qty,'LocationState_Empty-Enable']);
            }else if(that.stock_sts=="LocationState_Partial"&&that.useable_sts=="Enable"){
                arrRes[1].push([parseInt(that.index_col),that.index_no,that.location,that.remain_qty,'LocationState_Partial-Enable']);
            }else if(that.stock_sts=="LocationState_Full"&&that.useable_sts=="Enable"){
                arrRes[2].push([parseInt(that.index_col),that.index_no,that.location,that.remain_qty,'LocationState_Full-Enable']);
            }else if(that.stock_sts=="LocationState_Empty"&&that.useable_sts=="Disable"){
                arrRes[3].push([parseInt(that.index_col),that.index_no,that.location,that.remain_qty,'LocationState_Empty-Disable']);
            }else if(that.stock_sts=="LocationState_Partial"&&that.useable_sts=="Disable"){
                arrRes[4].push([parseInt(that.index_col),that.index_no,that.location,that.remain_qty,'LocationState_Partial-Disable']);
            };
        };
        var name_arr=[arrRes[0].length,arrRes[1].length,arrRes[2].length,arrRes[3].length,arrRes[4].length];
        var xy_arr=setXY(52,26);
        var obj={
            size:20
        };
        var myEchart=echarts.init(document.getElementById('chart_1'));
        myEchart.setOption(getOption(arrRes, xy_arr, name_arr,obj));
        myEchart.on('brushSelected', function(params){
            renderBrushed(params,myEchart,arrRes);        
        });
    
    };
    function renderBrushed(params,myEchart,arrRes) {
        var brushed = [];
        var brushComponent = params.batch[0];
        var nameArr=['空 可 用','部分可用','满 可 用','空 禁 用','部分禁用'];
        for (var sIdx = 0; sIdx < brushComponent.selected.length; sIdx++) {
            var rawIndices = brushComponent.selected[sIdx].dataIndex;
            var remainTotal=0;
            for(var i=0;i<rawIndices.length;i++){
                remainTotal+=arrRes[sIdx][rawIndices[i]][3];
            };
            brushed.push(nameArr[sIdx] +'：' + remainTotal);
        };
    
        myEchart.setOption({
            title: {
                backgroundColor: '#333',
                text: '已选货位剩余数量统计   ' + brushed.join('    '),
                x:'center',
                bottom: 20,
                width: 100,
                textStyle: {
                    fontSize: 14,
                    color: '#fff'
                }
            }
        });
    };
    function setXY(xNum,yNum){
        var arr=[[],[],[]];
        for(var i=0;i<=xNum;i++){
            arr[0].push(i);
        };
        for(var j=0;j<=yNum;j++){
            arr[1].push(j);
        };
        for(var i=0;i<=xNum;i++){
            if(i<13){
                arr[2].push('');
            }else{
                arr[2].push(i-12);
            }
        };
        return arr;
    };

    function getOption(arr, xy_arr, name_arr,obj) {
        schema = ['列', '层','货位编码' ,'剩余数量', '状态','托盘条码','物料名称', '工艺', '入库时间', '存储时间（分）', '预计出库时间', '出库标识'];
        return option = {
            color: ['#92d931','yellow', 'blue','red','lightcoral'],
            itemStyle: {
                emphasis: {
                    shadowBlur: 20,
                    shadowColor: 'rgba(0, 0, 0, 1)',
                    textStyle:{
                        fontSize:30,
                        fontWeight:'bold'
                    }
                }
            },
            tooltip: {
                trigger: 'item',
                axisPointer: {
                    type: 'cross'
                },
                formatter: function (params){
                    var value = params.value;
                    return setFormatter(schema,value);
                },
            },
            axisPointer: {
                label:{
                    backgroundColor:'red',
                    color:'purple'
                }
            },
            brush: {
                toolbox: ['rect','lineX', 'lineY', 'keep', 'clear'],
                throttleType: 'debounce',
                throttleDelay: 100,
                xAxisIndex: 0,
                brushStyle: {
                    borderWidth: 1,
                    color: 'rgba(120,140,180,0.3)',
                    borderColor: 'rgba(120,140,180,0.8)'
                },
                outOfBrush: {
                    opacity:0.3
                }
            },
            toolbox: {
                right:20,
                iconStyle: {
                    normal: {
                        borderColor: '#666'
                    },
                    emphasis: {
                        borderColor: 'green'
                    }
                }
            },
            legend: {
                y: 'top',
                top:'5%',
                right: '10%',
                itemHeight: 25,
                selectedMode:'multiple' ,
                data: [
                    {
                        name: '空可用：' + name_arr[0],
                    },
                    {
                        name: '部分可用：' + name_arr[1],
                    },
                    {
                        name: '满可用：' + name_arr[2],
                    },
                    {
                        name: '空禁用：' + name_arr[3],
                    },
                    {
                        name: '部分禁用：' + name_arr[4],           
                    }
                ],
                textStyle: {
                    fontSize: 15,
                    color:'#000'
                }
            },
            grid: {
                left: '3%',
                top:'20%',
                height: '70%',
                width: '90%',
                y: '2%',
                x: '1%'
            },
            xAxis: [
                {
                    type: 'category',
                    name: '货位/列',
                    nameGap: 5,
                    position:'top',
                    min:1,
                    data: xy_arr[0],
                    splitArea: {
                        show: false
                    },
                    axisLine: {
                        lineStyle: {
                            color: '#000'
                        }
                    },
                    axisLabel: {
                        textStyle: {
                            color: '#000'
                        },
                        interval: 0,
                    },     
                    axisTick: {
                        lineStyle: {
                            color: '#000'
                        }
                    },
                    textStyle: {
                        fontSize: 8
                    },
                    offset: 0
                },
                {
                    type: 'category',
                    name: '货位/列',
                    nameGap: 5,
                    position:'bottom',
                    min:1,
                    data: xy_arr[2],
                    splitArea: {
                        show: false
                    },
                    axisLine: {
                        lineStyle: {
                            color: '#000'
                        }
                    },
                    axisLabel: {
                        textStyle: {
                            color: '#000'
                        },
                        interval: 0,
                    },     
                    axisTick: {
                        lineStyle: {
                            color: '#000'
                        }
                    },
                    textStyle: {
                        fontSize: 8
                    },
                    offset: 0
                },
            ],
            yAxis: {
                type: 'category',
                data: xy_arr[1],
                min: 1,
                name: ' ',
                splitArea: {
                    show: false
                },
                textStyle: {
                    fontSize: 8
                },
                axisLine: {
                    lineStyle: {
                        color: '#000'
                    }
                },
                axisLabel: {
                    textStyle: {
                        color: '#000'
                    },
                    interval: 0,
                    formatter: function (val) {
                        var yLabel=['一','号','架','二','三','四','五','六'];
                        if(val==1){
                            return yLabel[0];
                        }else if(val==2||val==7||val==11||val==16||val==20||val==25){
                            return yLabel[1];
                        }else if(val==3||val==8||val==12||val==17||val==21||val==26){
                            return yLabel[2];
                        }else if(val==6){
                            return yLabel[3];
                        }else if(val==10){
                            return yLabel[4];
                        }else if(val==15){
                            return yLabel[5];
                        }else if(val==19){
                            return yLabel[6];
                        }else if(val==24){
                            return yLabel[7];
                        };
                    }
                },
                axisTick: {
                    lineStyle: {
                        color: '#000'
                    }
                },
                z: 10
            },
            animation: false,
            series: [
                {
                    name: '空可用：' + name_arr[0],
                    type: 'scatter',
                    symbol: 'image://../../areas/wms/img/green.png',
                    symbolSize:obj.size,
                    label: {
                        emphasis: {
                            show: false,
                            position: 'right',
                            textStyle: {
                                color: 'red',
                                fontSize: 16
                            }
                        }
                    },
                    data: arr[0]
                },
                {
                    name: '部分可用：' + name_arr[1],
                    type: 'scatter',
                    symbol: 'image://../../areas/wms/img/blue.png',
                    symbolSize:obj.size,
                    data: arr[1]
                },
                {
                    name: '满可用：' + name_arr[2],
                    type: 'scatter',
                    symbol: 'image://../../areas/wms/img/yellow.png',
                    symbolSize:obj.size,
                    data: arr[2]
                },
                {
                    name: '空禁用：' + name_arr[3],
                    type: 'scatter',
                    symbol: 'image://../../areas/wms/img/red.png',
                    symbolSize:obj.size,
                    data: arr[3]
                },
                {
                    name: '部分禁用：' + name_arr[4],
                    type: 'scatter',
                    symbol: 'image://../../areas/wms/img/pink.png',
                    symbolSize:obj.size,
                    data: arr[4]
                },
            
            ]
        };

    };
    function setFormatter(schema,value){
        var letterArr=value[2].split('');
        return '<div style="padding:10px;font-size:14px;">'
            + '<span style="color:#E8DB81;margin-left:20px;">' + letterArr[0] + '</span>' +'号货架'+ '<br>'
            + '第<span style="color:#E8DB81">' + parseInt(letterArr[1]+letterArr[2]) + '</span>' + schema[0] + '&nbsp;&nbsp;'
            + '第<span style="color:#E8DB81">' + letterArr[3] + '</span>' + schema[1] + '</div>'
            + schema[2] + ' : ' + '<span style="color:#E8DB81">' + value[2] + '</span>' + '<br>'
            + schema[3] + ' : ' + '<span style="color:#E8DB81">' + value[3] + '</span>' + '<br>'
            + schema[4] + ' : ' + '<span style="color:#E8DB81">' + value[4] + '</span>' + '<br>';
    };
})