<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Call_Terminal_Log.aspx.cs"
    Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Call_Terminal_Log" %>

<%--<%@ Register assembly="C1.Web.Wijmo.Controls.4" namespace="C1.Web.Wijmo.Controls.C1SuperPanel" tagprefix="wijmo" %>--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <%--<base target="_self" />--%>
    <title>呼叫终端登录</title>
    <%--<link href="Styles/master 1.css" rel="Stylesheet" type="text/css"/>--%>
    <link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <%--<script type="text/javascript" src="Scripts/keyboard.js"  language="javascript"></script>--%>
    <script src="js/jquery.js" type="text/javascript"></script>
    <script src="js/jquery-ui.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">

        //    window.onload = function adjustWindowSizeAndPos() {
        //        var WshShell = new ActiveXObject("WScript.Shell");
        //        WshShell.SendKeys('{F11}');
        //    }


        //window.onload = function adjustWindowSizeAndPos() {
        //    //显示登录 或 自动登录提示
        //    var url = location.href;
        //    if (url.indexOf("usercode") == -1 || url.indexOf("SiteEQCODE") == -1 || url.indexOf("TASKCODE") == -1
        //        || url.indexOf("SiteCLASSTIMECODE") == -1 || url.indexOf("SiteSHIFTCLASSCODE") == -1
        //        || url.indexOf("SiteMATERIALCODE") == -1) {
        //        document.getElementById("divLogin").style.display = "block";
        //        document.getElementById("divAutoLogin").style.display = "none";
        //    }

        //    document.getElementById("hifIp").value = "127.0.0.1";
        //}



        function Method(id) {
            var reValue = showModalDialog("KeyBoard.aspx", window, "dialogWidth=860px;dialogHeight=380px;dialogTop:600px;help:no;resizable:no;status:no;scroll:yes;center:yes;scroll:yes;location:no;"); //保存窗体的返回值  
            var array = reValue; //保存返回值  
            var uname = document.getElementById(id);
            if (reValue != null) {
                uname.value = array;
            }
        }


        $(function () {
            var url = location.href;
            if (url.indexOf("username") == -1 || url.indexOf("pwd") == -1 || url.indexOf("machine") == -1) {
                $("#divLogin").show();
                $("#divAutoLogin").hide();
            }
            //getIPs(function (ip) {
            //    if (ip != "") {
            //        console.log("ip=" + ip);
            //    }
            //});
        });

        function checkLogin() {
            if ($("#txtUserName").val() == "") {
                $("#txtUserName").focus();
                alert("请输入用户名！");
                return false;
            }
            if ($("#txtPwd").val() == "") {
                alert("请输入密码！");
                $("#txtPwd").focus();
                return false;
            }

            $("#btn_Login").focus();
            $("#chekLogin").show();
            return true;
        }
    </script>
    <style type="text/css">
        .style1
        {
            font-size: 14px;
        }
        
    </style>
</head>
<body>
    <form id="form1" runat="server">
<iframe id="iframe" style="display:none"></iframe>
    <div class="main">
        <div class="loginbg2">
            <div>
                <img alt="" src="images/login_title.gif" /></div>
            <div id="divLogin" class="login_text">
                <ul>
                    <li>帐号:</li>
                    <li>
                        <asp:TextBox ID="txtUserName" runat="server" CssClass="style1"></asp:TextBox><%--onclick="Method(this.id)"--%></li>
                </ul>
                <ul>
                    <li>密码:</li>
                    <li>
                        <asp:TextBox ID="txtPwd" runat="server" TextMode="Password" CssClass="style1" Text="admin" onclick="Method(this.id)"></asp:TextBox><%-- onclick="Method(this.id)"--%></li>
                </ul>
                <div style="padding-top: 20px">
                    <asp:Button ID="btn_Login" runat="server" CssClass="login_btn" Text="" OnClientClick="return checkLogin();"
                        OnClick="btn_Login_Click" />
                    <asp:Button ID="btn_Cancle" runat="server" Text="" CssClass="exit_btn" OnClick="btn_Cancle_Click" />
                </div>
                <asp:HiddenField ID="hifIp" runat="server" />
            </div>
            <div id="divAutoLogin" style="line-height: 200px; font-size: 22px; "><%--display: none;--%>
                正在跳转，请稍候...
            </div>
        </div>
    </div>
    <div id="chekLogin" style="display: none;">
        <div class="div_Watting"></div>
        <div id="Div1" class="div_Model">
            <div id="divWatting" style="font-size:large; width: 100%; text-align: center;">
                <img alt="" src="images/loading.gif" />
            </div>
        </div>
    </div>
    </form>
</body>
</html>
