<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Set_Number_new.aspx.cs" Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Set_Number_new" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>机台设置</title>
    <link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .style1 {
            font-size: medium;
            font-weight: bold;
        }

        .style2 {
            width: 118px;
        }

        .style3 {
            width: 118px;
            font-weight: bold;
        }
    </style>
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
    <script language="javascript" type="text/javascript">
        function changetext(obj) {
            var drr4 = document.getElementById("txtBlock_MachineNam");
            drr4.value = obj.value;
        }
        function checkForm() {
            var machine = document.getElementById("ddlMachine");
            if (machine.value == "") {
                alert("请选择机台！");
                return false;
            }
            //var runscode = document.getElementById("ddlRunsCode");
            //if (runscode.value == "") {
            //    alert("请选择班次！");
            //    return false;
            //}
            //var runsgroup = document.getElementById("ddlRunsGroup");
            //if (runsgroup.value == "") {
            //    alert("请选择班组！");
            //    return false;
            //}
            return true;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="main tablebg1">
            <div class="table_middle">
                <div class="sitebg">
                    当前所在位置：机台设置
                </div>
                <div class="tabletitle">
                    <ul>
                        <li>
                            <img src="/images/table_pic01.gif" /></li>
                        <li>机台设置</li>
                    </ul>
                </div>
                <div class="set_text">
                    <div style="width: 366px; height: 15px; background-image: url(/images/set_titlebg.gif);">
                        <%--<img src="/images/set_titlebg.gif" />--%>
                    </div>
                    <div class="set_modulebg">
                        <table style="width: 100%; height: 100%;">
                            <%--<tr>
                            <td style="width: 60px;">
                                车间：
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlWorkShop" runat="server" Width="155px" Height="25px" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlWorkShop_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>--%>
                            <tr>
                                <td>机台：
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlMachine" runat="server" Width="155px" Height="25px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <%--<tr>
                                <td>班次：
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlRunsCode" runat="server" Width="155px" Height="25px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>班组：
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlRunsGroup" runat="server" Width="155px" Height="25px">
                                    </asp:DropDownList>
                                </td>
                            </tr>--%>
                            <tr>
                                <td colspan="2">
                                    <asp:CheckBox ID="checkBox_LGV" runat="server" Text="产生AGV任务" />
                                </td>
                            </tr>
                        </table>

                    </div>
                    <div style="margin: -8px 0 0 0;">
                        <img src="/images/set_bottom.gif" />
                    </div>
                </div>
                <div class="table_bottombg">
                    <ul>
                        <li>
                            <asp:Button ID="btn_Ensure" runat="server" Text="" Width="98px" Height="33px" OnClick="btn_Ensure_Click"
                                Style="background-image: url(/images/btn_enter.gif); border: 0px; background-position: center; background-repeat: no-repeat"
                                OnClientClick="return checkForm();" />
                        </li>
                        <li>
                            <asp:Button ID="btn_Cancle" runat="server" Text="" Width="98px" Height="33px" OnClick="btn_Cancle_Click"
                                Style="background-image: url(/images/btn_back.gif); border: 0px; background-position: center; background-repeat: no-repeat"
                                Visible="false" />
                        </li>
                        <li>
                            <asp:Button ID="btn_Exit" runat="server" Text="" Width="98px" Height="33px" OnClick="btn_Exit_Click"
                                Style="background-image: url(/images/btn_Exit.gif); border: 0px; background-position: center; background-repeat: no-repeat; cursor: pointer;" />
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </form>
</body>
<%--<body>
    <form id="form1" runat="server">
    <div>
    <table width="400" align="center" style="height: 180px">
    <tr><td colspan="2" align="left" class="style1">请选择机台号:</td></tr>
    <tr><td class="style3" align="right">机台编号:</td><td>
        <asp:DropDownList ID="cb_MachineNumber" runat="server" Height="21px" 
            Width="155px" AutoPostBack="True" 
            onselectedindexchanged="cb_MachineNumber_SelectedIndexChanged">
        </asp:DropDownList>
        </td></tr>
    <tr><td class="style3" align="right">机台名称:</td><td>
        <asp:TextBox ID="txtBlock_MachineName" runat="server" Width="144px"></asp:TextBox>
        </td></tr>
    <tr><td class="style2"></td><td>
        <asp:CheckBox ID="checkBox_LGV" runat="server" Text="产生LGV任务" />
        </td></tr>
    <tr><td align="right" class="style2">
        <asp:Button ID="btn_Ensure" runat="server" Text="确定" Width="97px" 
            onclick="btn_Ensure_Click" />
        </td><td align="center">
            <asp:Button ID="btn_Cancle" runat="server" Text="取消" Width="97px" 
                onclick="btn_Cancle_Click" />
        </td></tr>
    </table>
    </div>
    </form>
</body>--%>
</html>
