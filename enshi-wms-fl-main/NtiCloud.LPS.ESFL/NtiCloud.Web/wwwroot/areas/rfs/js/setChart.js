$(function(){
    $.getJSON('data3.json',function(res){
        console.log(res);
        matchData(res);

    });
    
    //数据对应
    function matchData(rowObj) {
        //数据遍历对数据进行排序 空可用 部分可用 满可用 空禁用 部分禁用
        var arrRes=[[],[],[],[],[]];
        for(var i=0;i<rowObj.length;i++){
            var that=rowObj[i];
            // console.log(that.stock_sts+'+'+that.useable_sts);
            if(that.stock_sts=="Empty"&&that.useable_sts=="Enable"){//空可用 
                //列 层 货位编号 剩余数量 状态名
                arrRes[0].push([parseInt(that.index_col),that.index_no,that.location,that.remain_qty,'Empty-Enable']);
            }else if(that.stock_sts=="Partial"&&that.useable_sts=="Enable"){//部分可用 
                arrRes[1].push([parseInt(that.index_col),that.index_no,that.location,that.remain_qty,'Partial-Enable']);
            }else if(that.stock_sts=="Full"&&that.useable_sts=="Enable"){//满可用 
                arrRes[2].push([parseInt(that.index_col),that.index_no,that.location,that.remain_qty,'Full-Enable']);
            }else if(that.stock_sts=="Empty"&&that.useable_sts=="Disable"){//空禁用 
                arrRes[3].push([parseInt(that.index_col),that.index_no,that.location,that.remain_qty,'Empty-Disable']);
            }else if(that.stock_sts=="Partial"&&that.useable_sts=="Disable"){//部分禁用 
                arrRes[4].push([parseInt(that.index_col),that.index_no,that.location,that.remain_qty,'Partial-Disable']);
            };
        };
        // console.log('arrRes:');
        // console.log(arrRes);
        //各类数据对应的个数数组
        var name_arr=[arrRes[0].length,arrRes[1].length,arrRes[2].length,arrRes[3].length,arrRes[4].length];
        //x轴、y轴数组
        var xy_arr=setXY(52,26);
        //x轴文字旋转和图标大小对象
        var obj={
            // rotate:0,
            // bool:false,
            size:16
        };
        var myEchart=echarts.init(document.getElementById('chart_1'));
        myEchart.setOption(getOption(arrRes, xy_arr, name_arr,obj));
        myEchart.on('brushSelected', function(params){
            renderBrushed(params,myEchart,arrRes);        
        });
    
    };

    //渲染数量统计栏
    function renderBrushed(params,myEchart,arrRes) {
        // console.log(params);
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
                // text: '已选货位剩余数量统计: \n' + brushed.join('\n'),
                // bottom: 0,
                // right: 0,
                // width: 100,
                // textStyle: {
                //     fontSize: 12,
                //     color: '#fff'
                // }

                text: '已选货位剩余数量统计   ' + brushed.join('    '),
                x:'center',
                bottom: 20,
                //right: 10,
                width: 100,
                textStyle: {
                    fontSize: 14,
                    color: '#fff'
                }
            }
        });
    };

    //构建x轴和y轴数组
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

    //设置图表option
    function getOption(arr, xy_arr, name_arr,obj) {
        //console.log(arr);
        schema = ['列', '层','货位编码' ,'剩余数量', '状态','托盘条码','物料名称', '工艺', '入库时间', '存储时间（分）', '预计出库时间', '出库标识'];
        return option = {
            //color: ['#ec6941', '#e60012', '#52abe3', '#cfa972'],
            // color: ['#92d931','yellow', 'blue','#fe007f','lightcoral'],
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
                // trigger: 'axis',
                formatter: function (params){
                    var value = params.value;
                    return setFormatter(schema,value);
                },
                // // textStyle:{
                // //     color:'red'
                // // },
                // axisPointer: {
                //     show: false,
                //     type: 'cross',
                //     lineStyle: {
                //         type: 'dashad',
                //         width: 1
                //     },
                //     // label:{
                //     //     backgroundColor:'red',
                //     //     color:'purple'
                //     // }
                // }
            },
            axisPointer: {
                // show: false,
                // type: 'cross',
                // lineStyle: {
                //     type: 'solid',
                //     width: 1
                // },
                label:{
                    backgroundColor:'red',
                    color:'purple'
                }
            },
            brush: {
                // toolbox: ['rect', 'polygon', 'lineX', 'lineY', 'keep', 'clear'],
                // toolbox: ['rect','lineX', 'lineY', 'keep', 'clear'],
                // throttleType: 'debounce',//选框效果延迟 避免频繁刷新数据和页面的闪动效果 
                // throttleDelay: 200,//选框反应时间延迟
                // xAxisIndex: 0
                
                toolbox: ['rect','lineX', 'lineY', 'keep', 'clear'],
                throttleType: 'debounce',//选框效果延迟 避免频繁刷新数据和页面的闪动效果 
                throttleDelay: 100,//选框反应时间延迟
                xAxisIndex: 0,
                brushStyle: {
                    borderWidth: 1,
                    color: 'rgba(120,140,180,0.3)',
                    borderColor: 'rgba(120,140,180,0.8)'
                },
                outOfBrush: {
                    //colorAlpha: 0.1,
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
                        // borderColor: '#b1e4ff'
                        borderColor: 'green'
                    }
                }
            },
            legend: {//顶部指示
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
                {//x轴坐标
                    type: 'category',//category value
                    name: '货位/列',
                    nameGap: 5,
                    position:'top',
                    //max:30,
                    min:1,
                    data: xy_arr[0],
                    splitArea: {
                        //show: true
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
                {//x轴坐标
                    type: 'category',//category value
                    name: '货位/列',
                    nameGap: 5,
                    position:'bottom',
                    //max:30,
                    min:1,
                    data: xy_arr[2],
                    splitArea: {
                        //show: true
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
            yAxis: {//y轴坐标
                type: 'category',//category
                data: xy_arr[1],
                //max:8,
                min: 1,
                name: ' ',
                splitArea: {
                    //show: true
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
                    symbol: 'rect',
                    // symbol: 'image://./img/loc_out.png',
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
                    // itemStyle: {
                    //     emphasis: {
                    //         shadowBlur: 20,
                    //         shadowColor: 'rgba(0, 0, 0, 1)'
                    //     }
                    // },
                    // tooltip: {
                    //     trigger: 'axis',
                    //     formatter: function (params){
                    //         var value = params.value;
                    //         return setFormatter(schema,value);
                    //     },
                    //     axisPointer: {
                    //         show: false,
                    //         type: 'cross',
                    //         lineStyle: {
                    //             type: 'solid',
                    //             width: 1
                    //         },
                    //         label:{
                    //             backgroundColor:'red',
                    //             color:'purple'
                    //         }
                    //     }
                    // },
                    data: arr[0]
                },
                {
                    name: '部分可用：' + name_arr[1],
                    type: 'scatter',
                    symbol: 'rect',
                    // symbol: 'image://./img/loc_in.png',
                    symbolSize:obj.size,
                    data: arr[1]
                },
                {
                    name: '满可用：' + name_arr[2],
                    type: 'scatter',
                    symbol: 'rect',
                    // symbol: 'image://./img/loc_cell_stored.png',
                    symbolSize:obj.size,
                    data: arr[2]
                },
                {
                    name: '空禁用：' + name_arr[3],
                    type: 'scatter',
                    symbol: 'rect',
                    // symbol: 'image://./img/loc_pallet_stored.png',
                    symbolSize:obj.size,
                    data: arr[3]
                },
                {
                    name: '部分禁用：' + name_arr[4],
                    type: 'scatter',
                    symbol: 'rect',
                    // symbol: 'image://./img/loc_forbid.png',  
                    symbolSize:obj.size,
                    data: arr[4]
                },
            
            ]
        };

    };

    //设置悬浮详情提示框的内容
    function setFormatter(schema,value){
        // console.log(value);
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