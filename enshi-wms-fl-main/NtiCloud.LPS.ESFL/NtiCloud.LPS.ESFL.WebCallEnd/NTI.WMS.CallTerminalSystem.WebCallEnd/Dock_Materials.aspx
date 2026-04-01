<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dock_Materials.aspx.cs"
    Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Dock_Materials" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>托盘明细</title>
    <link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <base target="_self" />
    <script language="javascript" type="text/javascript">
        function closeWindow() {
            window.close();
        } 
    </script>
    <style type="text/css">
        .style1
        {
            font-size: 12px;
        }
    </style>
    <script language="javascript" type="text/javascript">
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main1 tablebg2" style=" width: 650px;">
        <div class="table_middle2">
            <div class="tabletitle">
                <ul>
                    <li>
                        <img src="/images/table_pic01.gif" /></li>
                    <li><asp:Label ID="LblBomCode" runat="server" Text=""></asp:Label></li>
                </ul>
            </div>
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:GridView ID="lv_Dock_Materials" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                                PageSize="9" OnPageIndexChanging="GridViewReturnBill_ToNext_PageIndexChanging"
                                BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                                CellPadding="3" Font-Size="14pt" Width="100%" AllowPaging="True">
                                <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="14px" />
                                <EmptyDataRowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="14px"
                                    BorderWidth="1px" />
                                <EmptyDataTemplate>
                                    <table class="sublist" width="100%">
                                        <tr>
                                            <th>
                                                物料名称
                                            </th>
                                            <th>
                                                数量
                                            </th>
                                            <th>
                                                配盘状态
                                            </th>
                                            <th>
                                                托盘类型
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
                                        </tr>
                                    </table>
                                </EmptyDataTemplate>
                                <Columns>
                                    <asp:TemplateField HeaderText="物料名称" ItemStyle-Height="40px">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <asp:Label ID="CMName" Text='<%# Eval("item_name") %>' ReadOnly="true" runat="server"
                                                Width="230px" CssClass="style1" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="数量">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <asp:Label ID="CCount" Text='<%# Eval("qty") %>' ReadOnly="true" runat="server" Width="60px"
                                                CssClass="style1" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="单位">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <asp:Label ID="uomName" Text='<%# Eval("uom_name") %>' ReadOnly="true" runat="server" Width="60px"
                                                CssClass="style1" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="配盘状态">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <asp:Label ID="CStatus" Text='<%# Eval("bom_status") %>' ReadOnly="true" runat="server"
                                                Width="90px" CssClass="style1" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="托盘类型">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <asp:Label ID="CModelCode" Text='<%# Eval("section_code") %>' ReadOnly="true" runat="server"
                                                CssClass="style1" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <FooterStyle BackColor="White" ForeColor="#000066" />
                                <PagerStyle BackColor="#f3f8fa" ForeColor="#000066" HorizontalAlign="Center" Font-Bold="True" />
                                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" Font-Size="Medium" />
                                <HeaderStyle BackColor="#c7e0ee" Font-Bold="True" ForeColor="#4" Height="34" />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="table_bottombg">
                <ul>
                    <li>
                        <input id="Button2" type="button" value="" onclick="window.close()" class="dock_btn" />
                    </li>
                </ul>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
