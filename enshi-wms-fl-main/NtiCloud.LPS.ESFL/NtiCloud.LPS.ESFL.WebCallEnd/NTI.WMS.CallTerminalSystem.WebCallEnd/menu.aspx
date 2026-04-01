<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="menu.aspx.cs" Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.html.menu" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>无标题文档</title>
    <link href="Styles/index.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="index_menu">
	<div>
        <asp:Button ID="Button1" runat="server" Text="" CssClass="index_btn01off" 
            onMouseover='index_btn01on' onMouseout='index_btn01off' 
            onclick="Button1_Click"/>
    </div>
    <div>
        <asp:Button ID="Button2" runat="server" Text="" CssClass="index_btn02off" 
            onMouseover='index_btn02on' onMouseout='index_btn02off' 
            onclick="Button2_Click"/>
    </div>
    <div>
        <asp:Button ID="Button3" runat="server" Text="" CssClass="index_btn03off" 
            onMouseover='index_btn03on' onMouseout='index_btn03off' 
            onclick="Button3_Click"/>
    </div>
    <div>
        <asp:Button ID="Button4" runat="server" Text="" CssClass="index_btn04off" 
            onMouseover='index_btn04on' onMouseout='index_btn04off' 
            onclick="Button4_Click"/>
    </div>
    <div>
        <asp:Button ID="Button5" runat="server" Text="" CssClass="index_btn05off" 
            onMouseover='index_btn05on' onMouseout='index_btn05off' 
            onclick="Button5_Click"/>
    </div>
    <div>
        <asp:Button ID="Button6" runat="server" Text="" CssClass="index_btn06off" onMouseover='index_btn06on' onMouseout='index_btn06off'/>
    </div>
    <div>
        <asp:Button ID="Button7" runat="server" Text="" CssClass="index_btn07off" onMouseover='index_btn07on' onMouseout='index_btn07off'/>
    </div>
    <div>
        <asp:Button ID="Button8" runat="server" Text="" CssClass="index_btn08off" onMouseover='index_btn08on' onMouseout='index_btn08off'/>
    </div>
</div>
    </form>
</body>
</html>
