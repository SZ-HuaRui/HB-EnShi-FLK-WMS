<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManReturn_Terminal.aspx.cs" Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.ManReturn_Terminal" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self" />
    <title>要料</title>
    <link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <%--<script type="text/javascript" src="Scripts/keyboard.js"></script>--%>
    <%--<script language="javascript" type="text/javascript">
        function firm() {//利用对话框返回的值（true 或者 false）
            if (confirm("确定进行此操作？")) {
                return true;
            }
            else {
                return false;
            }
        }

        function firm1(id) {
            if (confirm("是否对备用站台进行退空托盘操作？")) {
                document.getElementById("btn_Platform" + id).click();
            }
            else {
                return false;
            }
        }

    </script>--%>
    <style type="text/css">
        .style1
        {
            font-size: 12px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main tablebg">
        <div class="table_middle">
            <div class="sitebg">
                当前所在位置：人工领退料<asp:Timer ID="Timer1" runat="server" 
                    Interval="2000" Enabled="false">
                </asp:Timer>
            </div>
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
                        <li>机台编号：</li>
                        <li>
                            <asp:Label ID="lb_MachineNumber" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                    <ul>
                        <li>机台名称：</li>
                        <li>
                            <asp:Label ID="lb_MachineName" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                    <ul>
                        <li>操作人员:</li>
                        <li>
                            <asp:Label ID="txtblock_UserName" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                    <ul>
                        <li>生产牌号:</li>
                        <li>
                            <asp:Label ID="lb_ProductName" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                    <ul>
                        <li>班次:</li>
                        <li>
                            <asp:Label ID="Label3" runat="server" Text="未设置"></asp:Label></li>
                        <li>
                        </li>
                    </ul>
                    <ul>
                        <li>
                            <asp:Button ID="btn_Refresh" runat="server" Text="刷  新" CssClass="list_btn01off"
                                    BorderStyle="None" OnClick="btn_Refresh_Click" Width="140px" Height="40px" ValidationGroup="0"/>
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
                    <div id="Div1" runat="server">
                        <table width="100%" border="0" cellpadding="0" cellspacing="0">
                            <tr class="list_title">
                                <td valign="top">
                                    <asp:GridView ID="lv_ClientCallTask" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                                        PageSize="7" OnPageIndexChanging="GridViewReturnBill_ToNext_PageIndexChanging"
                                        BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                                        CellPadding="3" Font-Size="Large" Width="100%" AllowPaging="True">
                                        <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="12px" />
                                        <EmptyDataRowStyle BackColor="LightBlue" ForeColor="#000066" Font-Size="12px" BorderColor="#CCCCCC"
                                            BorderWidth="1px" />
                                        <EmptyDataTemplate>
                                            <table class="sublist" width="100%">
                                                <tr>
                                                    <th>
                                                        任务号
                                                    </th>
                                                    <th>
                                                        操作类型
                                                    </th>
                                                    <th>
                                                        站台位
                                                    </th>
                                                    <th>
                                                        托盘类型
                                                    </th>
                                                    <th>
                                                        配盘单号
                                                    </th>
                                                    <th>
                                                        创建时间
                                                    </th>
                                                    <th>
                                                        状态
                                                    </th>
                                                </tr>
                                                <tr>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                        </EmptyDataTemplate>
                                        <Columns>
                                            <asp:TemplateField HeaderText="任务号" ItemStyle-Height="40px">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                <ItemTemplate>
                                                    <asp:Label ID="task" Text='<%# Eval("TASKNUMBER") %>' ReadOnly="true" runat="server"
                                                        Width="120px" CssClass="style1" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="操作类型">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                <ItemTemplate>
                                                    <asp:Label ID="oper" Text='<%# Eval("TASKTYPENAME") %>' ReadOnly="true" runat="server"
                                                        Width="110px" CssClass="style1" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="站台位">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                <ItemTemplate>
                                                    <asp:Label ID="txtDOCKNUMBER" Text='<%# Eval("DOCKNUMBER") %>' ReadOnly="true" runat="server"
                                                        Width="100px" CssClass="style1" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="托盘类型">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                <ItemTemplate>
                                                    <asp:Label ID="TxtModel" Text='<%# Eval("MODELCODE") %>' ReadOnly="true" runat="server"
                                                        Width="100px" CssClass="style1" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="配盘单号">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                <ItemTemplate>
                                                    <asp:Label ID="bomcode" Text='<%# Eval("BOMCODE") %>' ReadOnly="true" runat="server"
                                                        Width="110px" CssClass="style1" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="创建时间">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                <ItemTemplate>
                                                    <asp:Label ID="createtime" Text='<%# Eval("createtime") %>' ReadOnly="true" runat="server"
                                                        Width="120px" CssClass="style1" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="状态">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                <ItemTemplate>
                                                    <asp:Label ID="columnState" Text='<%# Eval("TASKSTATENAME") %>' ReadOnly="true" runat="server"
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
                        <div class="left" style="padding-top: 10px;">
                            操作类别</div>
                        <ul style="padding-top: 10px;">
                            <li>
                                <asp:RadioButton ID="rb_ManPowerCallMaterials" runat="server" CssClass="style6" Text="人工领料"
                                    GroupName="radiobutton" AutoPostBack="True" 
                                    oncheckedchanged="rb_ManPowerCallMaterials_CheckedChanged"  /></li>
                            <li></li>
                            <li>
                                <asp:RadioButton ID="rb_ReturnNullPallt" runat="server" CssClass="style6" Text="退空托盘"
                                    OnCheckedChanged="radioButton_Click" GroupName="radiobutton" AutoPostBack="True" /></li>
                            <li></li>
                            <li>
                                <asp:RadioButton ID="rb_ManPowerReturnMaterials" runat="server" CssClass="style6"
                                    Text="人工退料" OnCheckedChanged="rb_ManPowerReturnMaterials_CheckedChanged" GroupName="radiobutton"
                                    AutoPostBack="True" /></li>
                            <li></li>

                        </ul>
                    </div>
                    <div class="list_options2">
                        <div class="left" style="padding-top: 5px; padding-right: 10px; width: 70px;">
                            托盘类型</div>
                        <ul>
                            <li style="padding-left: 0px;">
                                <asp:Button ID="textblock_Section1" runat="server" Text="" CssClass="list_btn01off"
                                    BorderStyle="None" OnClick="textblock_click" Width="140px" Height="40px" ValidationGroup="0">
                                </asp:Button></li>
                            <li style="padding-left: 0px;">
                                <asp:Button ID="textblock_Section2" runat="server" Text="" CssClass="list_btn01off"
                                    BorderStyle="None" OnClick="textblock_click" Width="140px" Height="40px" ValidationGroup="0">
                                </asp:Button></li>
                            <li>
                                <asp:Button ID="textblock_Section3" runat="server" Text="" CssClass="list_btn01off"
                                    BorderStyle="None" OnClick="textblock_click" Width="140px" Height="40px" ValidationGroup="0">
                                </asp:Button></li>
                            <li>
                                <asp:Button ID="textblock_Section4" runat="server" Text="" CssClass="list_btn01off"
                                    BorderStyle="None" OnClick="textblock_click" Width="140px" Height="40px">
                                </asp:Button></li>
                            <li>
                                <asp:Button ID="textblock_Section5" runat="server" Text="" CssClass="list_btn01off"
                                    BorderStyle="None" OnClick="textblock_click" Width="140px" Height="40px" >
                                </asp:Button></li>
                            <li>
                                <asp:Button ID="textblock_Section6" runat="server" Text="" CssClass="list_btn01off"
                                    BorderStyle="None" OnClick="textblock_click" Width="140px" Height="40px" >
                                </asp:Button></li>
                        </ul>
                    </div>
                    <div class="list_options2">
                        <div class="left" style="padding-top: 5px; padding-right: 10px; width: 70px;">
                            站台位</div>
                        <ul>
                            <li>
                                <asp:Button ID="btn_Platform1" runat="server" Text="" CssClass="list_btn01off" OnClick="button_Click"
                                    Width="140px" Height="40px" /></li>
                            <li>
                                <asp:Button ID="btn_Platform2" runat="server" Text="" CssClass="list_btn01off" OnClick="button_Click"
                                    Width="140px" Height="40px"  Visible="true"/></li>
                            <li>
                                <asp:Button ID="btn_Platform3" runat="server" Text="" CssClass="list_btn01off" OnClick="button_Click"
                                    Width="140px" Height="40px" /></li>
                            <li>
                                <asp:Button ID="btn_Platform4" runat="server" Text="" CssClass="list_btn01off" OnClick="button_Click"
                                    Width="140px" Height="40px"/></li><%--return firm();--%>
                            <li>
                                <asp:Button ID="btn_Platform5" runat="server" Text="" CssClass="list_btn01off" OnClick="button_Click"
                                    Width="140px" Height="40px"/></li>
                            <li>
                                <asp:Button ID="btn_Platform6" runat="server" Text="" CssClass="list_btn01off" OnClick="button_Click"
                                    Width="140px" Height="40px"/></li>
                        </ul>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="textblock_Section1" />
                    <asp:PostBackTrigger ControlID="textblock_Section2" />
                    <asp:PostBackTrigger ControlID="textblock_Section3" />
                    <asp:PostBackTrigger ControlID="textblock_Section4" />
                    <asp:PostBackTrigger ControlID="textblock_Section5" />
                    <asp:PostBackTrigger ControlID="textblock_Section6" />
                    <asp:PostBackTrigger ControlID="btn_Platform1" />
                    <asp:PostBackTrigger ControlID="btn_Platform2" />
                    <asp:PostBackTrigger ControlID="btn_Platform3" />
                    <asp:PostBackTrigger ControlID="btn_Platform4" />
                    <asp:PostBackTrigger ControlID="btn_Platform5" />
                    <asp:PostBackTrigger ControlID="btn_Platform6" />
                </Triggers>
            </asp:UpdatePanel>
            <div class="table_bottombg">
                <ul>
                    <li>
                        <asp:Button ID="btn_PalletType" runat="server" Text="" Width="120px" Height="33px" Enabled="false"
                            Style="background-image: url(/images/btn_set.gif); border: 0px;" OnClick="btn_PalletType_Click" /></li>
                    <%--<li>
                        <asp:Button ID="btn_Switch" runat="server" Text="" OnClick="btn_Switch_Click" Width="98px" 
                            Height="33px" Style="background-image: url(/images/btn_tab.gif); border: 0px;" /></li>--%>
                    <li>
                        <asp:Button ID="btn_Exit" runat="server" Text=" " Width="98px" Height="33px" Style="background-image: url(/images/btn_Exit.gif);
                            border: 0px;" OnClick="btn_Exit_Click" Visible="false" /></li>
                    <li>
                        <asp:Button ID="btn_Returns" runat="server" Text="" Width="98px" Height="33px" Style="background-image: url(/images/btn_back.gif);
                            border: 0px;" OnClick="btn_Returns_Click" /></li><%--onclick="btn_Returns_Click"--%>
                    </ul>
                <asp:Label ID="Label4" runat="server" Text="Label" Visible="true"
                    Font-Bold="true"></asp:Label>
            </div>
            <asp:GridView ID="ItemDetail" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                                        PageSize="12" BackColor="White" 
                BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                                        CellPadding="3" Font-Size="Smaller" Width="100%" 
                AllowPaging="True" 
                onselectedindexchanging="ItemDetail_SelectedIndexChanging">
                                        <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="12px" />
                                        <EmptyDataRowStyle BackColor="LightBlue" ForeColor="#000066" Font-Size="12px" BorderColor="#CCCCCC"
                                            BorderWidth="1px" />
                                        <EmptyDataTemplate>
                                            <table class="sublist" width="100%">
                                                <tr>
                                                    <th>
                                                        任务号
                                                    </th>
                                                    <th>
                                                        托盘类型
                                                    </th>
                                                    <th>
                                                        物料名称
                                                    </th>
                                                    <th>
                                                        生产厂家
                                                    </th>
                                                    <th>数量</th>
                                                    <th>单位</th>
                                                </tr>
                                            </table>
                                        </EmptyDataTemplate>
                                        <Columns>
                                            <asp:TemplateField HeaderText="站台位" ItemStyle-Height="15px">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                <ItemTemplate>
                                                    <asp:Label ID="task" Text='<%# Eval("Docknumber") %>' ReadOnly="true" runat="server"
                                                        Width="90px" CssClass="style1" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="托盘类型">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                <ItemTemplate>
                                                    <asp:Label ID="TxtModel" Text='<%# Eval("MODELCODE") %>' ReadOnly="true" runat="server"
                                                        Width="80px" CssClass="style1" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="物料名称">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                <ItemTemplate>
                                                    <asp:Label ID="itemname" Text='<%# Eval("ITEMNAME") %>' ReadOnly="true" runat="server"
                                                        Width="110px" CssClass="style1" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="生产厂家">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                <ItemTemplate>
                                                    <asp:Label ID="shippername" Text='<%# Eval("SHIPPERNAME") %>' ReadOnly="true" runat="server"
                                                        Width="150px" CssClass="style1" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="数量">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                <ItemTemplate>
                                                    <asp:Label ID="qty" Text='<%# Eval("QTY") %>' ReadOnly="true" runat="server"
                                                        Width="30px" CssClass="style1" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="单位">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                                <ItemTemplate>
                                                    <asp:Label ID="unitname" Text='<%# Eval("UNITNAME") %>' ReadOnly="true" runat="server"
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
        </div>
    </div>
    </form>
</body>
</html>