//文本框中只能输入数字或小数点
function clearNoNum(obj) 
{
    //先把非数字的都替换掉，除了数字和.
    obj.value = obj.value.replace(/[^\d.]/g, "");
    //必须保证第一个为数字而不是.
    obj.value = obj.value.replace(/^\./g, "");
    //保证只有出现一个.而没有多个.
    obj.value = obj.value.replace(/\.{2,}/g, ".");
    //保证.只出现一次，而不能出现两次以上
    obj.value = obj.value.replace(".", "$#$").replace(/\./g, "").replace("$#$", ".");
}
//隐藏地址栏、状态栏
function HiddenAddressState() 
{

}
//将弹出页面值传递给服务器端
function postvalue()
 {
    var a = document.getElementById("").value;    
    window.opener.document.frm1.all("").value = a;      //将值传递给客户端
}

//检查数量输入是否合法
function CheckQtyPatten(qty, type) {
    var formatQty1 = /^\d+$/; //"^([1-9][0-9]*)|(0)$"; //0 or 正整数
    var formatQty2 = /^(([1-9][0-9]*|(0))([.][0-9]{1,3})?)$/; //0  or 正小数
    var formatOdd = /^0([.][0-9]{1,3})?$/; //0  or 0.?
    var txtQty = qty.value;
    if (type == 1) {

        if (!formatQty1.test(txtQty)) {
            alert("只能输入大于等于0的正整数");                        
            return false;
        }
    }
    else if (type == 2) {

        if (!formatQty2.test(txtQty)) {
            alert("输入格式不正确");
            return false;
        }

    }
    else if (type == 3) //验证零头
    {

        if (!formatOdd.test(txtQty)) {
            alert("只能输入纯小数！");
            qty.value = "0";
            return false;
        }
    }

    else {
        window.close();
    }
}


//GridView 全选功能
function CheckAll(gridviewID, oCheckbox)//
{ 
    var gvResult = document.getElementById(gridviewID);
    for (i = 1; i < gvResult.rows.length; i++) {
        var obj = gvResult.rows[i].cells[0].getElementsByTagName("INPUT")[0];
        if (obj != null) {
            if (!obj.disabled) {

                obj.checked = oCheckbox.checked;
            }
        }
    }
}

function Chec_kAll(gridviewID, oCheckbox)//
{
    var gvResult = document.getElementById(gridviewID);
    alert(gvResult.rows.count);  
   
}




//在没有选中记录时的确认操作
function ShowConfirm(gridviewID, isConfirmRecord, selectedRecordToOpration) {
    var gvResult = document.getElementById(gridviewID);
    if (gvResult == null) {
        return false;
    }

    var flag = false;
    for (i = 1; i < gvResult.rows.length; i++) {
        if (gvResult.rows[i].cells[0].getElementsByTagName("INPUT")[0] != null) {
            if (gvResult.rows[i].cells[0].getElementsByTagName("INPUT")[0].checked) {
                flag = true;
            }
        }
    }

    if (flag) {
        //是否确认所选记录?
        return window.confirm(isConfirmRecord);
    }
    else {
        //请至少选择一条要确认的记录进行操作!
        alert(selectedRecordToOpration);
        return false;
    }
}

//在没有选中记录时,确认操作不可点击
function ShowCheckConfirm(oCheckBox, gridviewID, confirmbtnID, selectedRecordToOpration) {
    var gvResult = document.getElementById(gridviewID);
    var btnObj = document.getElementById(confirmbtnID);
    if (gvResult == null || btnObj == null) {
        return false;
    }
    if (oCheckBox.checked) {
        btnObj.disabled = false;
    }
    var flag = false;
    for (i = 1; i < gvResult.rows.length; i++) {
        if (gvResult.rows[i].cells[0].getElementsByTagName("INPUT")[0] != null) {
            if (gvResult.rows[i].cells[0].getElementsByTagName("INPUT")[0].checked) {
                flag = true;
                break;
            }
        }
    }
    if (flag) {
        btnObj.disabled = false;
    }
    else {
        btnObj.disabled = true;
    }
}


function aa()//
{
   
   var xmlHttp = new ActiveXObject("MSXML2.XMLHTTP");
   
    if (window.confirm('该站台存在未完成任务或未执行的退料单,你是否要退空托盘？') == true) //
    {
//        var xmlHttp = new ActiveXObject("MSXML2.XMLHTTP");
        var url = "GoOnitem.aspx?id=" + 1;  
     
    }
    else 
     {
         var url = "GoOnitem.aspx?id=" + 0;      
     }
        
       xmlHttp.open("POST", url, false);
          xmlHttp.send("");

          if (xmlHttp.readyState == 4 && xmlHttp.status == 200)//
          {
              var _Str = xmlHttp.responseText;
              // alert(_Str);

              document.getElementById('TextBox1').value = _Str;
              
          }
}


/// 实现单选
var ClickNum = 0; //判断同一checkbox连续点击次数   
var PreCheckboxID = ""; //记录点击checkbox的ID   
//listId为控制checkbox的范围   
//SelectRadio是点击checkbox自己
function SelectRadio(listId, SelectRadio) {
   
    ///判断是否连击同一个checkbox   
    if (PreCheckboxID == SelectRadio.id) {
        ClickNumClickNum = ClickNum + 1;
    }
    else {
        PreCheckboxID = SelectRadio.id;
        ClickNum = 0;
    }
    //找到控制范围   
    var GridViewtableSearchList = document.getElementById(listId);
    //找到控制范围下所有input   
    var objs = GridViewtableSearchList.getElementsByTagName("input");
    //找到控制范围下所有checkbox并都变为false   
    for (var i = 0; i < objs.length; i++) {
        if (objs[i].type.toLowerCase() == "checkbox")
            objs[i].checked = false;
    }
    var SelectRadioSelectRadioID = SelectRadio.id;
    //如果连击次数为奇次checked为true偶次为false   
    if (ClickNum % 2 != 1) {
        document.getElementById(PreCheckboxID).checked = true;
        
       
    }
    else {
        document.getElementById(PreCheckboxID).checked = false;
    }
} 