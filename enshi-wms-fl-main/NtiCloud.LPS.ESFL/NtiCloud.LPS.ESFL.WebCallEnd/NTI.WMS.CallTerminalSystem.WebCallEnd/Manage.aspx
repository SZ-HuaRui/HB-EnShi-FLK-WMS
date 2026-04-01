<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Manage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <link href="Styles/master%201.css" rel="stylesheet" type="text/css" />
    <base target="_self" />
    <script id="Back" language="javascript" type="text/javascript"></script>
    <%--<script language="javascript" type="text/javascript">
        window.onload = function adjustWindowSizeAndPos() {
            // 设置页面的初始大小，if条件判断是否是模式窗口    
            if (typeof window.dialogArguments != 'undefined') {
                window.dialogHeight = 0 + "px";
                window.dialogWidth = 0 + "px";
            } else {
                window.resizeTo(0, 0);
            }

            // 定义窗口宽度、窗口高度、屏幕宽度、屏幕高度    
            var winWidth = document.body.scrollWidth * 1.2;
            var winHeight = document.body.scrollHeight * 1.4;
            var availWidth = screen.availWidth;
            var availHeight = screen.availHeight;

            // 设置最小宽度、高度，最大宽度、高度 （根据屏幕宽度高度的比例计算）    
            var minWidth = availWidth * 0.5;
            var minHeight = availHeight * 0.5;
            var maxWidth = availWidth;
            var maxHeight = availHeight * 0.9;

            // 设置窗口的宽度高度在最小及最大值之间    
            winWidth = winWidth < minWidth ? minWidth : winWidth;
            winHeight = winHeight < minHeight ? minHeight : winHeight;
            winWidth = winWidth > maxWidth ? maxWidth : winWidth;
            winHeight = winHeight > maxHeight ? maxHeight : winHeight;

            // 设置窗口的宽度和高度，设置窗口的位置（if中为模式窗口设置，else中为活动窗口设置）    
            if (typeof window.dialogArguments != 'undefined') {
                window.dialogTop = ((availHeight - winHeight) / 2) + "px";
                window.dialogLeft = ((availWidth - winWidth) / 2) + "px";
                window.dialogHeight = winHeight + "px";
                window.dialogWidth = winWidth + "px";
            } else {
                window.moveTo((availWidth - winWidth) / 2, (availHeight - winHeight) / 2);
                window.resizeTo(winWidth, winHeight);
            }
        }  
    </script>--%>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main tablebg">
    	<div class="table_middle">
        	<div class="sitebg">当前所在位置：库管</div>
            <div class="tabletitle">
                <ul>
                    <li><img src="/images/table_pic01.gif" /></li>
                    <li>
                        <asp:Label ID="txtblock_UserName" runat="server" Text=""></asp:Label>，欢迎进入管理界面</li>
                </ul>
            </div>
            <div class="managemen_modulebg ">
                <ul>
                	<li>
                        <asp:Button ID="btn_Getting" runat="server" Text="" Width="240px" 
                            Height="100px" 
                            Style="background-image: url(/images/management_btn1.jpg); background-position:center;background-repeat: no-repeat" 
                            onclick="btn_Getting_Click"/></li>
                    <li>
                        <asp:Button ID="btn_Return_Bill" runat="server" Text="" Width="240px" 
                            Height="100px" 
                            Style="background-image: url(/images/management_btn2.jpg); background-position:center;background-repeat: no-repeat" 
                            onclick="btn_Return_Bill_Click"/></li>
                </ul>
            </div>
            
        </div>
        
        <div class="table_bottombg">
        	<ul>
            	
                <li>
                    <asp:Button ID="btn_Return" runat="server" Text="" Width="98px" 
                        Height="33px" 
                        Style="background-image: url(/images/btn_back.gif); background-position:center;background-repeat: no-repeat" 
                        onclick="btn_Return_Click"/></li>
            </ul>
        </div>
    </div>
    </form>
</body>
</html>
