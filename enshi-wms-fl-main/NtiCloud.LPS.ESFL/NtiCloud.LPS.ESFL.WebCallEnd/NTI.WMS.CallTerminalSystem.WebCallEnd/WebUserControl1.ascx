<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebUserControl1.ascx.cs" Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.WebUserControl1" %>
<head>

<meta http-equiv="Content-Type" content="text/html; charset=gb2312" />

<meta http-equiv="X-UA-Compatible" content="IE=EmulateIE8" />

<title>JS虚拟键盘</title>

<link href="css/virtualKeyboard.css" rel="stylesheet" type="text/css" />

<script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>

<script src="js/virtualKeyboard.js" type="text/javascript"></script>

<script type="text/javascript">
    window.onload = function () {
        virtualKeyboard("searchBar");
        $("#searchForm .searchButton").click(function () {			//普通按键输入功能
            //alert("您输入的内容是：" + $("#searchForm #searchBar").val());
            window.returnValue=$("#searchForm #searchBar").val();
            window.close();
        });
    }

</script>

</head>



<body>

<div id="searchForm">

	<form><input type="text" name="keyword" class="searchBar" id="searchBar" value="" /><input type="button" class="searchButton" value=""/></form>

</div>

<div id="virtualKeyboard">

	<%--<div class="inputButton line1" id="inputButton">`</div>--%>

	<div class="inputButton" id="inputButton">1</div>

    <div class="inputButton" id="inputButton">2</div>

    <div class="inputButton" id="inputButton">3</div>

    <div class="inputButton" id="inputButton">4</div>

    <div class="inputButton" id="inputButton">5</div>

    <div class="inputButton" id="inputButton">6</div>

    <div class="inputButton" id="inputButton">7</div>

    <div class="inputButton" id="inputButton">8</div>

    <div class="inputButton" id="inputButton">9</div>

    <div class="inputButton" id="inputButton">0</div>

    <%--<div class="inputButton" id="inputButton">-</div>

    <div class="inputButton" id="inputButton">=</div>--%>

    <div class="inputButton backButton" id="backButton"><!--退格键--></div>

   <%-- <div class="inputButton tabButton line2" id="tabButton"><!--Tab键--></div>

    <div class="inputButton" id="inputButton">q</div>

    <div class="inputButton" id="inputButton">w</div>

    <div class="inputButton" id="inputButton">e</div>

    <div class="inputButton" id="inputButton">r</div>

    <div class="inputButton" id="inputButton">t</div>

    <div class="inputButton" id="inputButton">y</div>

    <div class="inputButton" id="inputButton">u</div>

    <div class="inputButton" id="inputButton">i</div>

    <div class="inputButton" id="inputButton">o</div>

    <div class="inputButton" id="inputButton">p</div>

    <div class="inputButton" id="inputButton">[</div>

    <div class="inputButton" id="inputButton">]</div>

    <div class="inputButton inputButton70" id="inputButton">\</div>

    <div class="inputButton capsLockButton line3" id="capsLockButton"><!--大小写切换键--></div>

    <div class="inputButton" id="inputButton">a</div>

    <div class="inputButton" id="inputButton">s</div>

    <div class="inputButton" id="inputButton">d</div>

    <div class="inputButton" id="inputButton">f</div>

    <div class="inputButton" id="inputButton">g</div>

    <div class="inputButton" id="inputButton">h</div>

    <div class="inputButton" id="inputButton">j</div>

    <div class="inputButton" id="inputButton">k</div>

    <div class="inputButton" id="inputButton">l</div>

    <div class="inputButton" id="inputButton">;</div>

    <div class="inputButton" id="inputButton">'</div>

    <div class="inputButton enterButton" id="enterButton"><!--回车键--></div>

    <div class="inputButton leftShiftButton line4" id="leftShiftButton"><!--左侧Shift键--></div>

    <div class="inputButton" id="inputButton">z</div>

    <div class="inputButton" id="inputButton">x</div>

    <div class="inputButton" id="inputButton">c</div>

    <div class="inputButton" id="inputButton">v</div>

    <div class="inputButton" id="inputButton">b</div>

    <div class="inputButton" id="inputButton">n</div>

    <div class="inputButton" id="inputButton">m</div>

    <div class="inputButton" id="inputButton">,</div>--%>

    <div class="inputButton" id="inputButton">.</div>

    <%--<div class="inputButton" id="inputButton">/</div>

    <div class="inputButton rightShiftButton" id="rightShiftButton"><!--右侧Shift键--></div>

    <div class="inputButton ctrlButton line5" id="ctrlButton"><!--左侧Ctrl键--></div>

    <div class="inputButton" id="inputButton"></div>

    <div class="inputButton altButton" id="altButton"><!--左侧Alt键--></div>

    <div class="inputButton spaceButton" id="spaceButton"><!--空格键--></div>

    <div class="inputButton altButton" id="altButton"><!--右侧Alt键--></div>

    <div class="inputButton" id="inputButton"></div>

    <div class="inputButton ctrlButton" id="ctrlButton"><!--右侧Ctrl键--></div>

    <div class="inputButton delButton" id="delButton"><!--Del键--></div>--%>

</div>



<div id="test" style="color:#FFF;"></div>

</body>
