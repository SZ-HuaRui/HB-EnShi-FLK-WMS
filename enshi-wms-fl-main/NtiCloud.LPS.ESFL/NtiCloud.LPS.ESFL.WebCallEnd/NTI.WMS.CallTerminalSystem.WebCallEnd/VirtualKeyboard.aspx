<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VirtualKeyboard.aspx.cs" Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.VirtualKeyboard" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>JS虚拟键盘</title>
<base target="_self" />
<link href="Styles/virtualKeyboard.css" rel="stylesheet" type="text/css" />
<%--<script src="Styles/virtualKeyboard.js" type="text/javascript"></script>--%>
<%--<script type="text/javascript">
     function post(){
	$("#searchForm .searchButton").(function(){			//普通按键输入功能
	    //		alert("您输入的内容是：" + $("#searchForm #searchBar").val());
     window.returnValue=$("#searchForm #searchBar").val();
     window.close();
}
}
</script>--%>
    <style type="text/css">
        #form1
        {
            height: 506px;
            width: 1732px;
            margin-left: 0px;
        }
    </style>
</head>
<body>
<form id="form1" runat="server" >
<div id="searchForm">
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:TextBox ID="PostVal" runat="server" CssClass="searchBar" Height="46px" 
        Width="132px"></asp:TextBox>
    <asp:Button ID="ButtonPost" runat="server" Text=""  CssClass="searchButton" 
        onclick="ButtonPost_Click"/>
&nbsp;
</div>
<div id="virtualKeyboard">
    <div>
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
    <asp:Button ID="Button10" CssClass="inputButton" runat="server" Text="清空" 
            onclick="Button10_Click" /></div>
</div>
</form>
</body>
</html>
