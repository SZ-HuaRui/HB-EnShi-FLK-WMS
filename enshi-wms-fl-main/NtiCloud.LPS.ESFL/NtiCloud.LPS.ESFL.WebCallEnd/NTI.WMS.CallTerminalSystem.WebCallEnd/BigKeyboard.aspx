<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BigKeyboard.aspx.cs" Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.BigKeyboard" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title>JS虚拟键盘</title>
<base target="_self" />
<link href="Styles/virtualKeyboard.css" rel="stylesheet" type="text/css" />
<script src="Styles/virtualKeyboard.js" type="text/javascript" />
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
//        var winWidth = document.body.scrollWidth * 1.1;
//        var winHeight = document.body.scrollHeight * 1.2;
//        var availWidth = screen.availWidth;
//        var availHeight = screen.availHeight;

//        // 设置最小宽度、高度，最大宽度、高度 （根据屏幕宽度高度的比例计算）    
//        var minWidth = availWidth * 0.3;
//        var minHeight = availHeight * 0.3;
//        var maxWidth = availWidth;
//        var maxHeight = availHeight * 0.85;

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
<form id="Form1" runat="server">
<div id="searchForm">
    
    <asp:TextBox ID="PostVal" runat="server" CssClass="searchBar" Height="46px" 
        Width="132px"></asp:TextBox>
    <asp:Button ID="ButtonPost" runat="server" Text=""  CssClass="searchButton" 
        onclick="ButtonPost_Click"/>
&nbsp;
</div>

<div id="virtualKeyboard">
    <div>
    <asp:Button ID="kbtn__" CssClass="inputButton" runat="server" Text="`" />
    <asp:Button ID="Button1" CssClass="inputButton" runat="server" Text="1" 
            onclick="Button1_Click" />
    <asp:Button ID="Button2" CssClass="inputButton" runat="server" Text="2" 
            onclick="Button2_Click" />
    <asp:Button ID="Button3" CssClass="inputButton" runat="server" Text="3" 
            onclick="Button3_Click" />
    <asp:Button ID="Button4" CssClass="inputButton" runat="server" Text="4" 
            onclick="Button4_Click" />
    <asp:Button ID="Button5" CssClass="inputButton" runat="server" Text="5" 
            onclick="Button5_Click" />
    <asp:Button ID="Button6" CssClass="inputButton" runat="server" Text="6" 
            onclick="Button6_Click" />
    <asp:Button ID="Button7" CssClass="inputButton" runat="server" Text="7" 
            onclick="Button7_Click" />
    <asp:Button ID="Button8" CssClass="inputButton" runat="server" Text="8" 
            onclick="Button8_Click" />
    <asp:Button ID="Button9" CssClass="inputButton" runat="server" Text="9" 
            onclick="Button9_Click" />
    <asp:Button ID="Button0" CssClass="inputButton" runat="server" Text="0" 
            onclick="Button0_Click" />
    
    <asp:Button ID="ButtonBack" CssClass="inputButton backButton" runat="server" Text="" />
    <asp:Button ID="ButtonTab" CssClass="inputButton tabButton line2" runat="server" Text="" />
    <asp:Button ID="ButtonQ" CssClass="inputButton" runat="server" Text="Q" />
    <asp:Button ID="ButtonW" CssClass="inputButton" runat="server" Text="W" />
    <asp:Button ID="ButtonE" CssClass="inputButton" runat="server" Text="E" />
    <asp:Button ID="ButtonR" CssClass="inputButton" runat="server" Text="R" />
    <asp:Button ID="ButtonT" CssClass="inputButton" runat="server" Text="T" />
    <asp:Button ID="ButtonY" CssClass="inputButton" runat="server" Text="Y" />
    <asp:Button ID="ButtonU" CssClass="inputButton" runat="server" Text="U" />
    <asp:Button ID="ButtonI" CssClass="inputButton" runat="server" Text="I" />
    <asp:Button ID="ButtonO" CssClass="inputButton" runat="server" Text="O" />
    <asp:Button ID="ButtonP" CssClass="inputButton" runat="server" Text="P" />
    <asp:Button ID="ButtonXing" CssClass="inputButton" runat="server" Text="*" />
    <asp:Button ID="ButtoncapsLock" CssClass="inputButton capsLockButton line3" 
            runat="server" Text="" onclick="ButtoncapsLock_Click" />
    <asp:Button ID="ButtonA" CssClass="inputButton" runat="server" Text="A" />
    <asp:Button ID="ButtonS" CssClass="inputButton" runat="server" Text="S" />
    <asp:Button ID="ButtonD" CssClass="inputButton" runat="server" Text="D" />
    <asp:Button ID="ButtonF" CssClass="inputButton" runat="server" Text="F" />
    <asp:Button ID="ButtonG" CssClass="inputButton" runat="server" Text="G" />
    <asp:Button ID="ButtonH" CssClass="inputButton" runat="server" Text="H" />
    <asp:Button ID="ButtonJ" CssClass="inputButton" runat="server" Text="J" />
    <asp:Button ID="ButtonK" CssClass="inputButton" runat="server" Text="K" />
    <asp:Button ID="ButtonL" CssClass="inputButton" runat="server" Text="L" />
    <asp:Button ID="ButtonEnter" CssClass="inputButton enterButton" runat="server" Text="" />
    <asp:Button ID="ButtonShift" CssClass="inputButton leftShiftButton line4" 
            runat="server" Text="" onclick="ButtonShift_Click1" />
    <asp:Button ID="ButtonZ" CssClass="inputButton" runat="server" Text="Z" />
    <asp:Button ID="ButtonX" CssClass="inputButton" runat="server" Text="X" />
    <asp:Button ID="ButtonC" CssClass="inputButton" runat="server" Text="C" />
    <asp:Button ID="ButtonV" CssClass="inputButton" runat="server" Text="V" />
    <asp:Button ID="ButtonB" CssClass="inputButton" runat="server" Text="B" />
    <asp:Button ID="ButtonN" CssClass="inputButton" runat="server" Text="N" />
    <asp:Button ID="ButtonM" CssClass="inputButton" runat="server" Text="M" />
    <asp:Button ID="Buttondian" CssClass="inputButton" runat="server" Text="." />
    <asp:Button ID="Button11" CssClass="inputButton" runat="server" Text="/" />
    <asp:Button ID="ButtonSpace" CssClass="inputButton" runat="server" Text="" />
    </div>

</div>
</form>
</body>
</html>
