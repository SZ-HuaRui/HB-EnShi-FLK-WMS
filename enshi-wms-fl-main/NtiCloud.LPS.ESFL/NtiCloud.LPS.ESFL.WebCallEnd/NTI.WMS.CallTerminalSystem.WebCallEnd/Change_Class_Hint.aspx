<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Change_Class_Hint.aspx.cs"
    Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Change_Class_Hint" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>交接班处理</title>
    <link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript">

        //    window.onload = function adjustWindowSizeAndPos() {
        //        // 设置页面的初始大小，if条件判断是否是模式窗口    
        //        if (typeof window.dialogArguments != 'undefined') {
        //            window.dialogHeight = 0 + "px";
        //            window.dialogWidth = 0 + "px";
        //        } else {
        //            window.resizeTo(0, 0);
        //        }

        //        // 定义窗口宽度、窗口高度、屏幕宽度、屏幕高度    
        //        var winWidth = document.body.scrollWidth * 1.2;
        //        var winHeight = document.body.scrollHeight * 1.4;
        //        var availWidth = screen.availWidth;
        //        var availHeight = screen.availHeight;

        //        // 设置最小宽度、高度，最大宽度、高度 （根据屏幕宽度高度的比例计算）    
        //        var minWidth = availWidth * 0.5;
        //        var minHeight = availHeight * 0.5;
        //        var maxWidth = availWidth;
        //        var maxHeight = availHeight * 0.9;

        //        // 设置窗口的宽度高度在最小及最大值之间    
        //        winWidth = winWidth < minWidth ? minWidth : winWidth;
        //        winHeight = winHeight < minHeight ? minHeight : winHeight;
        //        winWidth = winWidth > maxWidth ? maxWidth : winWidth;
        //        winHeight = winHeight > maxHeight ? maxHeight : winHeight;

        //        // 设置窗口的宽度和高度，设置窗口的位置（if中为模式窗口设置，else中为活动窗口设置）    
        //        if (typeof window.dialogArguments != 'undefined') {
        //            window.dialogTop = ((availHeight - winHeight) / 2) + "px";
        //            window.dialogLeft = ((availWidth - winWidth) / 2) + "px";
        //            window.dialogHeight = winHeight + "px";
        //            window.dialogWidth = winWidth + "px";
        //        } else {
        //            window.moveTo((availWidth - winWidth) / 2, (availHeight - winHeight) / 2);
        //            window.resizeTo(winWidth, winHeight);
        //        }
        //    }  

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main tablebg1">
        <div class="table_middle">
            <div class="sitebg">
                当前所在位置：交接班提示</div>
            <div class="tabletitle">
                <ul>
                    <li>
                        <img src="/images/table_pic01.gif" /></li>
                    <li>
                        <asp:Label ID="tb_User" runat="server" Text=""></asp:Label></li>
                </ul>
            </div>
            <div class="set_text">
                <div>
                    <img src="images/set_titlebg2.jpg"></div>
                <div class="set_modulebg">
                    <ul>
                        <li>机台
                            <asp:Label ID="LblMachine" runat="server" Text=""></asp:Label>
                            存在交接班未处理：</li>
                    </ul>
                    <ul style="padding-top: 40px; padding-left: 10px;">
                        <li style="padding-left: 16px;">
                            <asp:Button ID="tb_oper" runat="server" Text="" CssClass="tips_btnenter" OnClick="tb_oper_Click" /></li>
                        <li>
                            <%--<asp:Button ID="tb_Close" runat="server" Text="" CssClass="tips_btncancel" OnClick="tb_Close_Click" />--%>
                            <asp:Button ID="btn_Setting" runat="server" Text="" Width="140px" Height="50px" OnClick="btn_Setting_Click"
                                Style="background-image: url(/images/panel_bt02.gif); background-position: center;
                                border: 0px; background-repeat: no-repeat; cursor: pointer;" /></li>
                    </ul>
                </div>
                <div>
                    <img src="/images/set_bottom.gif" /></div>
            </div>
        </div>
        <div class="table_bottombg">
        </div>
    </div>
    </form>
</body>
<%--<body>
    <form id="form1" runat="server">
    <div>
    <table style="height: 104px; width: 240px" align="center">
    <tr>
    <td colspan="2" align="center">
        <asp:Label ID="tb_User" runat="server" Font-Bold="True" Text="Label"></asp:Label>
        </td>
    </tr>
    <tr><td colspan="2" align="center">
        <asp:Label ID="tb_Text" runat="server" Font-Bold="True" Text="存在交接班任务未处理!"></asp:Label>
        </td></tr>
    <tr><td align="center">
        <asp:Button ID="tb_oper" runat="server" Text="处  理" onclick="tb_oper_Click" />
        </td><td align="center">
            <asp:Button ID="tb_Close" runat="server" Text="关  闭" onclick="tb_Close_Click" />
        </td></tr>
    </table>
    </div>
    </form>
</body>--%>
</html>
