<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Control_Panel.aspx.cs"
    Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Control_Panel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>控制面板</title>
    <link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <link href="Styles/master%201.css" rel="stylesheet" type="text/css" />
    <link href="css/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.js" type="text/javascript"></script>
    <script src="js/jquery-ui.js" type="text/javascript"></script>
    <script src="js/common.js" type="text/javascript"></script>
    <script id="Back" language="javascript" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">

        //        window.onload = function adjustWindowSizeAndPos() {
        //            var WshShell = new ActiveXObject("WScript.Shell");
        //            WshShell.SendKeys('{F11}');
        //        }

        //        window.onload = function adjustWindowSizeAndPos() {
        //            // 设置页面的初始大小，if条件判断是否是模式窗口    
        //            if (typeof window.dialogArguments != 'undefined') {
        //                window.dialogHeight = 0 + "px";
        //                window.dialogWidth = 0 + "px";
        //            } else {
        //                window.resizeTo(0, 0);
        //            }

        //            // 定义窗口宽度、窗口高度、屏幕宽度、屏幕高度    
        //            var winWidth = document.body.scrollWidth * 1.2;
        //            var winHeight = document.body.scrollHeight * 1.4;
        //            var availWidth = screen.availWidth;
        //            var availHeight = screen.availHeight;

        //            // 设置最小宽度、高度，最大宽度、高度 （根据屏幕宽度高度的比例计算）    
        //            var minWidth = availWidth * 0.5;
        //            var minHeight = availHeight * 0.5;
        //            var maxWidth = availWidth;
        //            var maxHeight = availHeight * 0.9;

        //            // 设置窗口的宽度高度在最小及最大值之间    
        //            winWidth = winWidth < minWidth ? minWidth : winWidth;
        //            winHeight = winHeight < minHeight ? minHeight : winHeight;
        //            winWidth = winWidth > maxWidth ? maxWidth : winWidth;
        //            winHeight = winHeight > maxHeight ? maxHeight : winHeight;

        //            // 设置窗口的宽度和高度，设置窗口的位置（if中为模式窗口设置，else中为活动窗口设置）    
        //            if (typeof window.dialogArguments != 'undefined') {
        //                window.dialogTop = ((availHeight - winHeight) / 2) + "px";
        //                window.dialogLeft = ((availWidth - winWidth) / 2) + "px";
        //                window.dialogHeight = winHeight + "px";
        //                window.dialogWidth = winWidth + "px";
        //            } else {
        //                window.moveTo((availWidth - winWidth) / 2, (availHeight - winHeight) / 2);
        //                window.resizeTo(winWidth, winHeight);
        //            }
        //        }  
    </script>
    <style type="text/css">
        .style1
        {
            color: #0066CC;
            font-size: 21px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main">
        <div class="sitebg">
            当前所在位置：控制面板</div>
        <div class="panelbg">
            <div class="panel_top">
                <div class="left" style="padding-top: 7px;">
                    <img src="images/panel_pic01.gif" /><asp:Label ID="Label1" runat="server" Text="Label"
                        Font-Size="Large" ForeColor="White" Style="padding-left: 10px;"></asp:Label></div>
                <ul>
                    <li>
                        <asp:Button ID="btn_Setting" runat="server" Text="" Height="40px" Width="130px" OnClick="btn_Setting_Click"
                            Style="background-image: url(/images/panel_bt02.gif); background-position: center;
                            border: 0px; background-repeat: no-repeat; cursor: pointer;" /></li>
                    <li>
                        <asp:Button ID="btn_LogOff" runat="server" Text="" Height="40px" Width="130px" OnClick="btn_LogOff_Click"
                            Style="background-image: url(/images/panel_bt03.gif); border: 0px; background-position: center;
                            background-repeat: no-repeat; cursor: pointer;" /></li><!-- Visible="False"-->
                </ul>
            </div>
            <div class="panel_text" style="padding-top: 10px;">
                <ul>
                    <li>
                        <asp:Button ID="btn_Call_Timinal" runat="server" Height="100px" Text="" Width="240px"
                            OnClick="btn_Call_Timinal_Click" Style="background-image: url(/images/panel_tc.jpg);
                            border: 0px; background-position: center; background-repeat: no-repeat" /></li>
                    <li>
                        <asp:Button ID="btn_Change_Card" runat="server" Height="100px" Text="" Width="240px"
                            OnClick="btn_Change_Card_Click" Style="background-image: url(/images/panel_cb_01.jpg);
                            background-position: center; border: 0px; background-repeat: no-repeat" /></li>
                   <%-- <li>
                        <asp:Button ID="btn_SetProduct" runat="server" Text="" Height="100px" Width="240px"
                            Style="background-image: url(/images/set_btn.jpg); border: 0px; background-position: center;
                            background-repeat: no-repeat" OnClick="btn_SetProduct_Click" /></li>--%>
                </ul>
                <ul>
                    <li>
                        <asp:Button ID="btn_Change_Class" runat="server" Height="100px" Text="" Width="240px"
                            OnClick="btn_Change_Class_Click" Style="background-image: url(/images/panel_r.jpg);
                            border: 0px; background-position: center; background-repeat: no-repeat" /></li>
                    <%--  <li>
                            <asp:Button ID="btn_ManPower_Fetch_Materials" runat="server" Text="" 
                            Height="100px" Width="240px" 
                            Style="background-image: url(/images/ManCallConfirm.jpg); border:0px;background-position:center;background-repeat: no-repeat" 
                            onclick="btn_ManPower_Fetch_Materials_Click"/></li>--%>
                    <%-- <li><asp:Button ID="btn_Artificial_Return" runat="server" Text="" Height="100px" 
                Width="240px" 
                            Style="background-image: url(/images/panel_sba.jpg); background-position:center; border:0px; background-repeat: no-repeat" 
                            onclick="btn_Artificial_Return_Click"/></li>--%>
                    <%--    <li><asp:Button ID="btn_Return_Bill" runat="server" Text="" Width="240px" 
                            Height="100px" Visible="true"
                            Style="background-image: url(/images/ManCallReturn.jpg); background-position:center; border:0px; background-repeat: no-repeat" 
                            onclick="btn_Return_Bill_Click"/></li>--%>
                    <li>
                        <asp:Button ID="btn_ClassDetails" runat="server" Text="" Height="100px" Visible="true"
                            Width="240px" OnClick="btn_ClassDetails_Click" Style="background-image: url(/images/management_btn4.jpg);
                            border: 0px; background-position: center; background-repeat: no-repeat" /></li>
                    <%-- <li><asp:Button ID="btn_Return_Wastes" runat="server" Text="" Height="100px" Visible="false"
                      Width="240px" onclick="btn_Return_Wastes_Click" Style="background-image: url(/images/panel_rw.jpg); border:0px; 
                	        background-position:center;background-repeat: no-repeat"/></li>--%>
                </ul>
                <ul>
                    <%--     <li><asp:Button ID="btn_Return_Manage" runat="server" Text="" Height="100px" Visible="true"
                            Width="240px" 
                            Style="background-image: url(/images/return_manage.jpg); border:0px; background-position:center;background-repeat: no-repeat" 
                            onclick="btn_Return_Manage_Click"/></li>--%>
                    <%-- <li><input type="button" id="comfirnpwd1" value='显示' /></li>--%>
                </ul>
                <%--<ul>
                      
                	<li><asp:Button ID="btn_Dottels" runat="server" Text="" Height="100px" Visible="false"
                      Width="240px" onclick="btn_Dottels_Click" Style="background-image: url(/images/panel_dc.jpg); border:0px; 
                	        background-position:center;background-repeat: no-repeat"/></li>
                            
                    <li><asp:Button ID="btn_Warehouse" runat="server" Text="" Height="100px" 
                      Width="240px" Visible="false"
                            Style="background-image: url(/images/panel_m.jpg); border:0px; background-position:center;background-repeat: no-repeat" 
                            onclick="btn_Warehouse_Click"/></li>
                </ul>--%>
                <div class="modifypwd" title="正在努力生成交班数据，请稍候..." style="width: 410px; height: 200px;
                    overflow: hidden; display: none;">
                    <br />
                    <h2>
                        请您注意</h2>
                    <hr />
                    &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp; 1、工单开始前请接班，工单结束后请交班。<br />
                    <br />
                    &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp; 2、实退余料时请通知高架库操作人员到场进行点数。
                </div>
            </div>
            <%--<div style="padding-top: 30px; height: 30px;">
                <ul>
                    <li class="style1"><strong>注:若无法进入交接班界面，请等待所有的任务完成之后,再进行交接班及领料确认操作！</strong></li></ul>
            </div>--%>
        </div>
    </div>
    </form>
</body>
</html>
