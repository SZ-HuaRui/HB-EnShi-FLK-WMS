<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Call_Terminal.aspx.cs"
    Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Call_Terminal" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self" />
    <title>要料</title>
    <link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.js" type="text/javascript"></script>
    <script src="js/jquery-ui.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        //        window.onload=function adjustWindowSizeAndPos() {
        //            // 设置页面的初始大小，if条件判断是否是模式窗口    
        //            if (typeof window.dialogArguments != 'undefined') {
        //                window.dialogHeight = 0 + "px";
        //                window.dialogWidth = 0 + "px";
        //            } else {
        //                window.resizeTo(0, 0);
        //            }

        //            // 定义窗口宽度、窗口高度、屏幕宽度、屏幕高度    
        //            var winWidth = document.body.scrollWidth * 1.1;
        //            var winHeight = document.body.scrollHeight * 1.2;
        //            var availWidth = screen.availWidth;
        //            var availHeight = screen.availHeight;

        //            // 设置最小宽度、高度，最大宽度、高度 （根据屏幕宽度高度的比例计算）    
        //            var minWidth = availWidth * 0.3;
        //            var minHeight = availHeight * 0.3;
        //            var maxWidth = availWidth;
        //            var maxHeight = availHeight * 0.85;

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
        function firm(btn) {//利用对话框返回的值（true 或者 false）
            var excuteType = "";
            if (document.getElementById("rb_OnlyCallMaterials").checked == true) {
                excuteType = "要料";
            } else if (document.getElementById("rb_ReturnNullPallt").checked == true) {
                excuteType = "退空托盘";
            } else if (document.getElementById("rb_ReturnRemnantsMaterials").checked) {
                excuteType = "退余料";
            } else if (document.getElementById("rb_ReturnBadMaterials").checked == true) {
                excuteType = "退坏料";
            } else if (document.getElementById("rb_ReturnNull_Call").checked == true) {
                excuteType = "退空托盘+要料";
            } else if (document.getElementById("rb_ReturnWasteMaterials").checked == true) {
                excuteType = "退垃圾";
            } else {
                excuteType = "其他";
            }
            var msg = "";
            if (document.getElementById("txtHavLGV").value == "0") {
                msg = "\n（当前机台设置为不产生 AGV 任务）";
            }
            //return (confirm("确认对站台 " + btn.value + " 进行“" + excuteType + "”操作吗？" + msg));
            if (confirm("确认对站台 " + btn.value + " 进行“" + excuteType + "”操作吗？" + msg)) {
                $("#submitWatting").show();
                return true;
            }
            return false;
        }

        function firm1(id) {
            if (confirm("是否对备用站台进行退空托盘操作？")) {
                document.getElementById("btn_Platform" + id).click();
            }
            else {
                return false;
            }
        }
        function firm99() {
            if (confirm("请确认是否进行退空托盘操作？") == true) {
                //                document.getElementById("btn_Platform" + id).click();
                return true;

            }
            else {
                return false;
            }
        }

    </script>
    <style type="text/css">
        .style1 {
            font-size: 12px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="main tablebg">
            <div class="table_middle">
                <%--<div class="sitebg">
                当前所在位置：要料<asp:Timer ID="Timer1" runat="server" Interval="2000" Enabled="false">
                </asp:Timer>
            </div>--%>
                <div class="tabletitle">
                    <ul>
                        <li>
                            <img src="/images/table_pic01.gif" /></li>
                        <li>机台信息</li>
                    </ul>
                </div>

                <div class="table_module1bg">
                    <div class="table_module1">
                        <ul>
                            <li>机台编号:</li>
                            <li>
                                <asp:Label ID="lb_MachineNumber" runat="server" Text="未设置"></asp:Label>
                                &nbsp;&nbsp; </li>
                        </ul>
                        <ul>
                            <li>机台名称:</li>
                            <li>
                                <asp:Label ID="lb_MachineName" runat="server" Text="未设置"></asp:Label></li>
                        </ul>
                        <ul>
                            <li>操作人员:</li>
                            <li>
                                <asp:Label ID="txtblock_UserName" runat="server" Text="未设置"></asp:Label>
                                （<asp:Label ID="Label3" runat="server" Text="未设置"></asp:Label>）</li>
                        </ul>
                        <ul style="width: 636px;">
                            <li>生产牌号:</li>
                            <li style="width: 500px;">
                                <asp:Label ID="lb_ProductName" runat="server" Text="未设置"></asp:Label></li>
                        </ul>
                        <%--<ul>
                        <li>班次:</li>
                        <li>
                            </li>
                        <li></li>
                    </ul>--%>
                        <ul>
                            <li>
                                <asp:Button ID="btn_Refresh" runat="server" Text="刷  新" CssClass="list_btn01off"
                                    BorderStyle="None" OnClick="btn_Refresh_Click" Width="140px" Height="40px" ValidationGroup="0" />
                            </li>
                            <li></li>
                        </ul>
                    </div>
                </div>

                <div class="tabletitle">
                    <ul>
                        <li>
                            <img src="/images/table_pic01.gif" /></li>
                        <li>机台操作记录</li>
                    </ul>
                </div>
                <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
                </asp:ScriptManager>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="True" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div runat="server">
                            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                <tr class="list_title">
                                    <td valign="top">
                                        <asp:GridView ID="lv_ClientCallTask" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                                            PageSize="12"
                                            BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                                            CellPadding="3" Font-Size="Large" Width="100%" AllowPaging="True">
                                            <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="12px" />
                                            <EmptyDataRowStyle BackColor="LightBlue" ForeColor="#000066" Font-Size="12px" BorderColor="#CCCCCC"
                                                BorderWidth="1px" />
                                            <EmptyDataTemplate>
                                                <table class="sublist" width="100%">
                                                    <tr>
                                                        <th>任务号
                                                        </th>
                                                        <th>操作类型
                                                        </th>
                                                        <th>站台位
                                                        </th>
                                                        <th>托盘类型
                                                        </th>
                                                        <th>配盘单号
                                                        </th>
                                                        <th>创建时间
                                                        </th>
                                                        <th>状态
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td></td>
                                                        <td></td>
                                                        <td></td>
                                                        <td></td>
                                                        <td></td>
                                                        <td></td>
                                                    </tr>
                                                </table>
                                            </EmptyDataTemplate>
                                            <Columns>
                                                <asp:TemplateField HeaderText="任务号" ItemStyle-Height="40px">
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="task" Text='<%# Eval("task_no") %>' ReadOnly="true" runat="server"
                                                            Width="100px" CssClass="style1" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="操作类型">
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="oper" Text='<%# Eval("ref_doc_type") %>' ReadOnly="true" runat="server"
                                                            Width="110px" CssClass="style1" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="站台位">
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="txtDOCKNUMBER" Text='<%# Eval("dock_name") %>' ReadOnly="true" runat="server"
                                                            Width="100px" CssClass="style1" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="托盘类型">
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="TxtModel" Text='<%# Eval("section_code") %>' ReadOnly="true" runat="server"
                                                            Width="100px" CssClass="style1" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="配盘单号">
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="bomcode" Text='<%# Eval("bom_code") %>' ReadOnly="true" runat="server"
                                                            Width="110px" CssClass="style1" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="创建时间">
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="createtime" Text='<%# Eval("create_date") %>' ReadOnly="true" runat="server"
                                                            Width="120px" CssClass="style1" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="状态">
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="columnState" Text='<%# Eval("task_status") %>' ReadOnly="true" runat="server"
                                                            Width="80px" CssClass="style1" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle BackColor="White" ForeColor="#000066" />
                                            <PagerStyle BackColor="#f3f8fa" ForeColor="#000066" HorizontalAlign="Center" Font-Bold="True" />
                                            <SelectedRowStyle BackColor="#669999" Font-Bold="false" ForeColor="White" />
                                            <HeaderStyle BackColor="#c7e0ee" Font-Bold="True" ForeColor="#4" Height="34" BorderColor="#c7e0ee"
                                                BorderStyle="None" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="tabletitle">
                            <ul>
                                <li>
                                    <img src="/images/table_pic01.gif" /></li>
                                <li>机台操作</li>
                            </ul>
                        </div>
                        <div class="list_options">
                            <div class="left" style="padding-top: 10px; width: 38px;">
                                操作
                            </div>
                            <ul style="padding-top: 10px;">
                                <li>
                                    <asp:RadioButton ID="rb_OnlyCallMaterials" runat="server" CssClass="style6" Text="要料"
                                        OnCheckedChanged="radioButton_Click" GroupName="radiobutton" AutoPostBack="true" 
                                        Checked="true" /></li>
                                <li style="width: 70px;"></li>
                                <li>
                                    <asp:RadioButton ID="rb_ReturnNullPallt" runat="server" CssClass="style6" Text="退空托盘"
                                        OnCheckedChanged="radioButton_Click" GroupName="radiobutton" AutoPostBack="True" /></li>
                                <li style="width: 70px;"></li>

                                <li>
                                    <asp:RadioButton ID="rb_ReturnNull_Call" runat="server" CssClass="style6" Text="退空托盘+要料"
                                        OnCheckedChanged="radioButton_Click" GroupName="radiobutton" AutoPostBack="True" Visible="false"/></li>
                                <li style="width: 70px;"></li>

                                <li>
                                    <asp:RadioButton ID="rb_ReturnRemnantsMaterials" runat="server" CssClass="style6"
                                        Text="退余料" OnCheckedChanged="rb_ReturnRemnantsMaterials_CheckedChanged" GroupName="radiobutton"
                                        AutoPostBack="True" /></li>
                                <li style="width: 70px;"></li>

                                <li>
                                    <asp:RadioButton ID="rb_ReturnBadMaterials" runat="server" CssClass="style6" Text="退坏料"
                                        OnCheckedChanged="radioButton_Click" GroupName="radiobutton" AutoPostBack="True" /></li>
                                <li style="width: 70px;"></li>
                                <li>
                                    <asp:RadioButton ID="rb_ReturnWasteMaterials" runat="server" CssClass="style6" Text="退垃圾"
                                        OnCheckedChanged="radioButton_Click" GroupName="radiobutton" AutoPostBack="True"  Visible="false" /></li>
                                <li></li>
                                <li style="display: none;">
                                    <asp:TextBox ID="txtHavLGV" runat="server"></asp:TextBox></li>
                            </ul>
                        </div>
                        <div>
                            <table style="width: 100%; text-align: left; border-style: none; border-width: 0px; background-color: #e8f0f5;">
                                <tr>
                                    <td style="width: 40px; text-align: right;">托盘
                                    </td>
                                    <td>
                                        <asp:Button ID="textblock_Section1" runat="server" Text="" CssClass="list_btn01off"
                                            BorderStyle="None" Width="130px" Height="40px" ValidationGroup="0" OnClick="textblock_click"></asp:Button>
                                    </td>
                                    <td>
                                        <asp:Button ID="textblock_Section2" runat="server" Text="" CssClass="list_btn01off"
                                            BorderStyle="None" Width="130px" Height="40px" ValidationGroup="0" OnClick="textblock_click"></asp:Button>
                                    </td>
                                    <td>
                                        <asp:Button ID="textblock_Section3" runat="server" Text="" CssClass="list_btn01off"
                                            BorderStyle="None" Width="130px" Height="40px" ValidationGroup="0" OnClick="textblock_click"></asp:Button>
                                    </td>
                                    <td>
                                        <asp:Button ID="textblock_Section4" runat="server" Text="" CssClass="list_btn01off"
                                            BorderStyle="None" Width="130px" Height="40px" ValidationGroup="0" OnClick="textblock_click"></asp:Button>
                                    </td>
                                    <td>
                                        <asp:Button ID="textblock_Section5" runat="server" Text="" CssClass="list_btn01off"
                                            BorderStyle="None" Width="130px" Height="40px" ValidationGroup="0" OnClick="textblock_click"></asp:Button>
                                    </td>
                                    <td>
                                        <asp:Button ID="textblock_Section6" runat="server" Text="" CssClass="list_btn01off"
                                            BorderStyle="None" Width="130px" Height="40px" ValidationGroup="0" OnClick="textblock_click"></asp:Button>
                                    </td>
                                    <td>
                                        <asp:Button ID="textblock_Section7" runat="server" Text="" CssClass="list_btn01off"
                                            BorderStyle="None" Width="130px" Height="40px" ValidationGroup="0" OnClick="textblock_click"></asp:Button>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right;">站台
                                    </td>
                                    <td>
                                        <asp:Button ID="btn_Platform1" runat="server" Text="" CssClass="list_btn01off_60"
                                            OnClick="button_Click" Width="130px" Height="60px" OnClientClick="return firm(this);"
                                            Font-Bold="False" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btn_Platform2" runat="server" Text="" CssClass="list_btn01off_60"
                                            OnClick="button_Click" Width="130px" Height="60px" OnClientClick="return firm(this);" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btn_Platform3" runat="server" Text="" CssClass="list_btn01off_60"
                                            OnClick="button_Click" Width="130px" Height="60px" OnClientClick="return firm(this);" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btn_Platform4" runat="server" Text="" CssClass="list_btn01off_60"
                                            OnClick="button_Click" Width="130px" Height="60px" OnClientClick="return firm(this);" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btn_Platform5" runat="server" Text="" CssClass="list_btn01off_60"
                                            OnClick="button_Click" Width="130px" Height="60px" OnClientClick="return firm(this);" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btn_Platform6" runat="server" Text="" CssClass="list_btn01off_60"
                                            OnClick="button_Click" Width="130px" Height="60px" OnClientClick="return firm(this);" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btn_Platform7" runat="server" Text="" CssClass="list_btn01off_60"
                                            OnClick="button_Click" Width="130px" Height="60px" OnClientClick="return firm(this);" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="textblock_Section1" />
                        <asp:PostBackTrigger ControlID="textblock_Section2" />
                        <asp:PostBackTrigger ControlID="textblock_Section3" />
                        <asp:PostBackTrigger ControlID="textblock_Section4" />
                        <asp:PostBackTrigger ControlID="textblock_Section5" />
                        <asp:PostBackTrigger ControlID="textblock_Section6" />
                        <asp:PostBackTrigger ControlID="textblock_Section7" />
                        <asp:PostBackTrigger ControlID="btn_Platform1" />
                        <asp:PostBackTrigger ControlID="btn_Platform2" />
                        <asp:PostBackTrigger ControlID="btn_Platform3" />
                        <asp:PostBackTrigger ControlID="btn_Platform4" />
                        <asp:PostBackTrigger ControlID="btn_Platform5" />
                        <asp:PostBackTrigger ControlID="btn_Platform6" />
                        <asp:PostBackTrigger ControlID="btn_Platform7" />
                    </Triggers>
                </asp:UpdatePanel>

                <div class="table_bottombg">
                    <ul>
                        <li>
                            <asp:Button ID="btn_PalletType" runat="server" Text="" Width="120px" Height="33px"
                                Enabled="false" Style="background-image: url(/images/btn_set.gif); border: 0px;"
                                OnClick="btn_PalletType_Click" Visible="False" /></li>
                        <%--<li>
                        <asp:Button ID="btn_Switch" runat="server" Text="" OnClick="btn_Switch_Click" Width="98px" 
                            Height="33px" Style="background-image: url(/images/btn_tab.gif); border: 0px;" /></li>--%>
                        <li>
                            <asp:Button ID="btn_Exit" runat="server" Text=" " Width="98px" Height="33px" Style="background-image: url(/images/btn_Exit.gif); border: 0px;"
                                OnClick="btn_Exit_Click" Visible="false" /></li>
                        <li></li>
                        <li>
                            <asp:Button ID="btn_Returns" runat="server" Text="" Width="98px" Height="33px" Style="background-image: url(/images/btn_back.gif); border: 0px;"
                                OnClick="btn_Returns_Click" /></li>
                    </ul>
                    <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                </div>


                <asp:GridView ID="ItemDetail" runat="server" AllowSorting="True" AutoGenerateColumns="False" Visible="false"
                    PageSize="12" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                    CellPadding="3" Font-Size="Smaller" Width="100%" AllowPaging="True" OnSelectedIndexChanging="ItemDetail_SelectedIndexChanging">
                    <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="12px" />
                    <EmptyDataRowStyle BackColor="LightBlue" ForeColor="#000066" Font-Size="12px" BorderColor="#CCCCCC"
                        BorderWidth="1px" />
                    <EmptyDataTemplate>
                        <table class="sublist" width="100%">
                            <tr>
                                <th>站台位
                                </th>
                                <th>托盘类型
                                </th>
                                <th>物料名称
                                </th>
                                <th>生产厂家
                                </th>
                                <th>数量
                                </th>
                                <th>单位
                                </th>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="站台位" ItemStyle-Height="15px">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="task" Text='<%# Eval("dock_name") %>' ReadOnly="true" runat="server"
                                    Width="50px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="托盘类型">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="TxtModel" Text='<%# Eval("section_code") %>' ReadOnly="true" runat="server"
                                    Width="80px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="物料名称">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="itemname" Text='<%# Eval("item_name") %>' ReadOnly="true" runat="server"
                                    Width="110px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="供应商">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="shippername" Text='<%# Eval("short_name") %>' ReadOnly="true" runat="server"
                                    Width="150px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="数量">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="qty" Text='<%# Eval("qty") %>' ReadOnly="true" runat="server" Width="30px"
                                    CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="单位">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="unitname" Text='<%# Eval("uom_name") %>' ReadOnly="true" runat="server"
                                    Width="30px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle BackColor="White" ForeColor="#000066" />
                    <PagerStyle BackColor="#f3f8fa" ForeColor="#000066" HorizontalAlign="Center" Font-Bold="True" />
                    <SelectedRowStyle BackColor="#669999" Font-Bold="false" ForeColor="White" />
                    <HeaderStyle BackColor="#c7e0ee" Font-Bold="True" ForeColor="#4" Height="34" BorderColor="#c7e0ee"
                        BorderStyle="None" />
                </asp:GridView>

                <div class="tabletitle">
                    <ul>
                        <li>
                            <img src="/images/table_pic01.gif" /></li>
                        <li>机台早班已完成操作记录</li>
                    </ul>
                </div>
                <asp:GridView ID="FinsishTask" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                    PageSize="10" OnPageIndexChanging="GridViewReturnBill_ToNext_PageIndexChanging"
                    BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                    CellPadding="3" Font-Size="Large" Width="100%" AllowPaging="True">
                    <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="12px" />
                    <EmptyDataRowStyle BackColor="LightBlue" ForeColor="#000066" Font-Size="12px" BorderColor="#CCCCCC"
                        BorderWidth="1px" />
                    <EmptyDataTemplate>
                        <table class="sublist" width="100%">
                            <tr>
                                <th>任务号
                                </th>
                                <th>操作类型
                                </th>
                                <th>站台位
                                </th>
                                <th>托盘类型
                                </th>
                                <th>配盘单号
                                </th>
                                <th>创建时间
                                </th>
                                <th>状态
                                </th>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="任务号" ItemStyle-Height="40px">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="_task" Text='<%# Eval("task_no") %>' ReadOnly="true" runat="server"
                                    Width="100px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="操作类型">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="_oper" Text='<%# Eval("ref_doc_type") %>' ReadOnly="true" runat="server"
                                    Width="50px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="站台位">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="_txtDOCKNUMBER" Text='<%# Eval("dock_name") %>' ReadOnly="true" runat="server"
                                    Width="80px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="托盘类型">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="_TxtModel" Text='<%# Eval("section_code") %>' ReadOnly="true" runat="server"
                                    Width="100px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="配盘单号">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="_bomcode" Text='<%# Eval("bom_code") %>' ReadOnly="true" runat="server"
                                    Width="150px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="创建时间">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="_createtime" Text='<%# Eval("create_date") %>' ReadOnly="true" runat="server"
                                    Width="120px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="状态">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="_columnState" Text='<%# Eval("task_status") %>' ReadOnly="true" runat="server"
                                    Width="80px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle BackColor="White" ForeColor="#000066" />
                    <PagerStyle BackColor="#f3f8fa" ForeColor="#000066" HorizontalAlign="Center" Font-Bold="True" />
                    <SelectedRowStyle BackColor="#669999" Font-Bold="false" ForeColor="White" />
                    <HeaderStyle BackColor="#c7e0ee" Font-Bold="True" ForeColor="#4" Height="34" BorderColor="#c7e0ee"
                        BorderStyle="None" />
                    <PagerTemplate>
                        第<asp:Label ID="lblPageIndex" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex + 1 %>' />页
                                    共<asp:Label ID="lblPageCount" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageCount %>' />页
                                    <asp:LinkButton ID="lbnFirst" runat="Server" Text="首页" Enabled='<%# ((GridView)Container.NamingContainer).PageIndex != 0 %>'
                                        CommandName="Page" CommandArgument="First"></asp:LinkButton>
                        <asp:LinkButton ID="lbnPrev" runat="server" Text="上一页" Enabled='<%# ((GridView)Container.NamingContainer).PageIndex != 0 %>'
                            CommandName="Page" CommandArgument="Prev"></asp:LinkButton>
                        <asp:LinkButton ID="lbnNext" runat="Server" Text="下一页" Enabled='<%# ((GridView)Container.NamingContainer).PageIndex != (((GridView)Container.NamingContainer).PageCount - 1) %>'
                            CommandName="Page" CommandArgument="Next"></asp:LinkButton>
                        <asp:LinkButton ID="lbnLast" runat="Server" Text="尾页" Enabled='<%# ((GridView)Container.NamingContainer).PageIndex != (((GridView)Container.NamingContainer).PageCount - 1) %>'
                            CommandName="Page" CommandArgument="Last"></asp:LinkButton>
                        <%-- <asp:TextBox ID="txtNewPageIndex" runat="server" Width="20px" Text='<%# ((GridView)Container.Parent.Parent).PageIndex + 1 %>' />--%>
                        <%--<asp:LinkButton ID="btnGo" runat="server" CausesValidation="False" CommandArgument="-1"
                    CommandName="Page" Text="GO" />--%><!-- here set the CommandArgument of the Go Button to '-1' as the flag -->
                    </PagerTemplate>
                </asp:GridView>
                <div class="tabletitle">
                    <ul>
                        <li>
                            <img src="/images/table_pic01.gif" /></li>
                        <li>机台中班已完成操作记录</li>
                    </ul>
                </div>
                <asp:GridView ID="MorFinsishTask" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                    PageSize="10" 
                    BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                    CellPadding="3" Font-Size="Large" Width="100%" AllowPaging="True">
                    <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="12px" />
                    <EmptyDataRowStyle BackColor="LightBlue" ForeColor="#000066" Font-Size="12px" BorderColor="#CCCCCC"
                        BorderWidth="1px" />
                    <EmptyDataTemplate>
                        <table class="sublist" width="100%">
                            <tr>
                                <th>任务号
                                </th>
                                <th>操作类型
                                </th>
                                <th>站台位
                                </th>
                                <th>托盘类型
                                </th>
                                <th>配盘单号
                                </th>
                                <th>创建时间
                                </th>
                                <th>状态
                                </th>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="任务号" ItemStyle-Height="40px">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="_Mortask" Text='<%# Eval("task_no") %>' ReadOnly="true" runat="server"
                                    Width="100px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="操作类型">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="_Moroper" Text='<%# Eval("ref_doc_type") %>' ReadOnly="true" runat="server"
                                    Width="50px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="站台位">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="_MortxtDOCKNUMBER" Text='<%# Eval("dock_name") %>' ReadOnly="true" runat="server"
                                    Width="80px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="托盘类型">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="_MorTxtModel" Text='<%# Eval("section_code") %>' ReadOnly="true" runat="server"
                                    Width="100px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="配盘单号">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="_Morbomcode" Text='<%# Eval("bom_code") %>' ReadOnly="true" runat="server"
                                    Width="150px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="创建时间">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="_Morcreatetime" Text='<%# Eval("create_date") %>' ReadOnly="true" runat="server"
                                    Width="120px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="状态">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                            <ItemTemplate>
                                <asp:Label ID="_MorcolumnState" Text='<%# Eval("task_status") %>' ReadOnly="true" runat="server"
                                    Width="80px" CssClass="style1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle BackColor="White" ForeColor="#000066" />
                    <PagerStyle BackColor="#f3f8fa" ForeColor="#000066" HorizontalAlign="Center" Font-Bold="True" />
                    <SelectedRowStyle BackColor="#669999" Font-Bold="false" ForeColor="White" />
                    <HeaderStyle BackColor="#c7e0ee" Font-Bold="True" ForeColor="#4" Height="34" BorderColor="#c7e0ee"
                        BorderStyle="None" />
                    <PagerTemplate>
                        第<asp:Label ID="_lblPageIndex" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex + 1 %>' />页
                                    共<asp:Label ID="_lblPageCount" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageCount %>' />页
                                    <asp:LinkButton ID="_lbnFirst" runat="Server" Text="首页" Enabled='<%# ((GridView)Container.NamingContainer).PageIndex != 0 %>'
                                        CommandName="Page" CommandArgument="First"></asp:LinkButton>
                        <asp:LinkButton ID="_lbnPrev" runat="server" Text="上一页" Enabled='<%# ((GridView)Container.NamingContainer).PageIndex != 0 %>'
                            CommandName="Page" CommandArgument="Prev"></asp:LinkButton>
                        <asp:LinkButton ID="_lbnNext" runat="Server" Text="下一页" Enabled='<%# ((GridView)Container.NamingContainer).PageIndex != (((GridView)Container.NamingContainer).PageCount - 1) %>'
                            CommandName="Page" CommandArgument="Next"></asp:LinkButton>
                        <asp:LinkButton ID="_lbnLast" runat="Server" Text="尾页" Enabled='<%# ((GridView)Container.NamingContainer).PageIndex != (((GridView)Container.NamingContainer).PageCount - 1) %>'
                            CommandName="Page" CommandArgument="Last"></asp:LinkButton>
                    </PagerTemplate>
                </asp:GridView>
            </div>
        </div>
        <div id="submitWatting" style="display: none;">
            <div class="div_Watting">
            </div>
            <div id="Div1" class="div_Model">
                <div id="divWatting" style="font-size: large; width: 100%; text-align: center;">
                    <img alt="" src="images/loading.gif" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
