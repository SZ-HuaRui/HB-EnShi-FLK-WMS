<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <title>要料终端</title>
</head>
<%--<body>
<iframe name="" width="100% "height="100%" marginwidth="0" marginheight="0" frameborder="0" scrolling="no" src="header.html" style="height:117px;" > </iframe>

	<iframe class="left" style="width:14%" name="myframe2" width="100%" height="100%" marginwidth="0" marginheight="0" frameborder="0" scrolling="no" src="menu.aspx" onload="document.all['myframe2'].style.height=myframe.document.body.scrollHeight" > </iframe>
    <iframe class="right" style="width:86%" name="myframe" width="100%" marginwidth="0" marginheight="0" frameborder="0" scrolling="no" src="Call_Terminal_Log.aspx" onload="document.all['myframe'].style.height=myframe.document.body.scrollHeight"> </iframe>
    
<%--<iframe  width="100%" height="100%" marginwidth="0" marginheight="0" frameborder="0" src="bottom.html" > </iframe>
</body>--%>
<frameset rows="117,*" frameborder="no" border="0" framespacing="0">
  <frame src="header.html" name="topFrame" scrolling="auto" noresize="noresize" id="topFrame" title="topFrame" />
  <frameset cols="14%,*" frameborder="no" border="0" framespacing="0">
    <frame src="menu.aspx" name="leftFrame" scrolling="No" noresize="noresize" id="leftFrame" title="leftFrame" />
    <frame src="Call_Terminal_Log.aspx" name="mainFrame" id="mainFrame" title="mainFrame" />
  </frameset>
</frameset>
<noframes>
<body>
<p>你的浏览器不支持框架，请升级你的浏览器</p>
</body>
</noframes>
</html>
