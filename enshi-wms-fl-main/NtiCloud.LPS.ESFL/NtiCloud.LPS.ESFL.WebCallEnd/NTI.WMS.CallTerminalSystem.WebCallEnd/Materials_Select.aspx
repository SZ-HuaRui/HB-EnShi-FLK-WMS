<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Materials_Select.aspx.cs" Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Materials_Select" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>创建人工领料</title>
    <link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <base target="_self" />
    <script type="text/javascript" src="Scripts/keyboard.js"></script>
    <style type="text/css">
        .style1
        {
            font-size: medium;
        }
    </style>
    <script language="javascript" type="text/javascript">
        window.onload = function adjustWindowSizeAndPos() {
            // 设置页面的初始大小，if条件判断是否是模式窗口    
            if (typeof window.dialogArguments != 'undefined') {
                window.dialogHeight = 0 + "px";
                window.dialogWidth = 0 + "px";
            } else {
                window.resizeTo(0, 0);
            }

            // 定义窗口宽度、窗口高度、屏幕宽度、屏幕高度    
            var winWidth = document.body.scrollWidth * 1.2;
            var winHeight = document.body.scrollHeight * 1.4;
            var availWidth = screen.availWidth;
            var availHeight = screen.availHeight;

            // 设置最小宽度、高度，最大宽度、高度 （根据屏幕宽度高度的比例计算）    
            var minWidth = availWidth * 0.5;
            var minHeight = availHeight * 0.5;
            var maxWidth = availWidth;
            var maxHeight = availHeight * 0.9;

            // 设置窗口的宽度高度在最小及最大值之间    
            winWidth = winWidth < minWidth ? minWidth : winWidth;
            winHeight = winHeight < minHeight ? minHeight : winHeight;
            winWidth = winWidth > maxWidth ? maxWidth : winWidth;
            winHeight = winHeight > maxHeight ? maxHeight : winHeight;

            // 设置窗口的宽度和高度，设置窗口的位置（if中为模式窗口设置，else中为活动窗口设置）    
            if (typeof window.dialogArguments != 'undefined') {
                window.dialogTop = ((availHeight - winHeight) / 2) + "px";
                window.dialogLeft = ((availWidth - winWidth) / 2) + "px";
                window.dialogHeight = winHeight + "px";
                window.dialogWidth = winWidth + "px";
            } else {
                window.moveTo((availWidth - winWidth) / 2, (availHeight - winHeight) / 2);
                window.resizeTo(winWidth, winHeight);
            }
        }  
    </script>
    <style type="text/css">
        .style2
        {
            font-size: 14px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">         
          <div class="main">
          <div class="table_middle">
          <div class="sitebg">当前所在位置：创建人工领料单</div>
            <div class="tabletitle">
                <ul>
                    <li><img src="/images/table_pic01.gif" /></li>
                    <li>条件设置</li>
                </ul>
            </div>
   			<div class="creat_set">
               
                <ul>
                    <li style="margin-top:4px;">
                        <asp:RadioButton ID="rb_ItemCode" runat="server" Text="按物料代码排序" 
                            style="font-size: medium" /></li>
                </ul>
                <ul>
                	<li style="margin-top:4px;">
                        <asp:RadioButton ID="rb_ItemName" runat="server" Text="按物料名称排序" 
                            style="font-size: medium" /></li>
                </ul>
                <ul>
                	<li><span class="style1">物料代理</span>
                        <asp:TextBox ID="tb_ItemCode" runat="server" onclick="Method(this.id)"></asp:TextBox></li>
                </ul>
                <ul>
                <li>
                    <asp:Button ID="Button" runat="server" Text="查  询" CssClass="btn" 
                        onclick="Button_Click" Font-Size="Large" /></li>
                </ul>
               
            </div>
            <div class="tabletitle">
                <ul>
                    <li><img src="/images/table_pic01.gif" /></li>
                    <li>物料列表</li>
                </ul>
            </div>
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" >
                    <tr>
                    <td>
                        <asp:GridView ID="lv_Create_Bill" runat="server" AllowSorting="True"
                        AutoGenerateColumns="False" PageSize="7" 
                        OnPageIndexChanging="lv_Create_Bill_PageIndexChanging"
                        BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                        CellPadding="3" Font-Size="14pt" Width="100%" 
            AllowPaging="True" >
                         <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="14px"/>
                        <emptydatarowstyle backcolor="#eff5f8"
                                    forecolor="#000066" Font-Size="14pt" BorderColor="#CCCCCC" BorderWidth="1px"/>
                                    <EmptyDataTemplate>
                                        <table class="sublist" width="100%">
                                            <tr>
                                                <th>
                                                    选择
                                                </th>
                                                <th>
                                                    物料代码
                                                </th>
                                                <th>
                                                    物料名称
                                                </th>
                                                <th>
                                                    单位
                                                </th>
                                            </tr>
                                            <tr>
                                                <td></td><td></td><td></td><td></td>
                                            </tr>
                                        </table>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderText="ITEMID" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="ITEMID" Text='<%# Eval("ITEMID") %>' Visible="false"
                                        ReadOnly="true" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="选择">
                                            <HeaderTemplate>
                                                <input id="ckbAll" type="checkbox" onclick="CheckAll('grvHandWorkBill',this)" />
                                            </HeaderTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" />
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkSelect" runat="server" OnCheckedChanged="chkSelect_select" AutoPostBack="true"/>
                                            </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CQty" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="CQty" Text='<%# Eval("qty") %>' Visible="false"
                                        ReadOnly="true" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="物料代码" HeaderStyle-Width="200px" ItemStyle-Height="40px">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:TextBox ID="CItemCode" Text='<%# Eval("ITEMCODE") %>'  ReadOnly="true" runat="server" Width="180px" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="物料名称" HeaderStyle-Width="200px">
                               <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="CItemName" Text='<%# Eval("ITEMNAME") %>' ReadOnly="true"
                                        runat="server" Width="180px" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="领料量" HeaderStyle-Width="200px">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:TextBox ID="GetCount" runat="server" Text='<%# Eval("QTY") %>' AutoPostBack="true" 
                                    onclick="Method2(this.id)" CssClass="style2"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="单位">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="CUnitName" Text='<%# Eval("CNAME") %>' ReadOnly="true" runat="server"
                                        Width="90px" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                           
                        </Columns>
                        <FooterStyle BackColor="White" ForeColor="#000066" />
                        <PagerStyle BackColor="#f3f8fa" ForeColor="#000066" HorizontalAlign="Center" Font-Bold="True" Height="40px"  />
                        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#c7e0ee" Font-Bold="True" ForeColor="#4" Height="34" />
                    </asp:GridView>
                    </td>
                    </tr>
                </table>
                   
                    <%--<div class="table_module1" style="padding-top:20px;">
                        <ul>
                            <li>物料名称:</li>
                            <li>
                                <asp:Label ID="tblock_ItemName" runat="server" Text="" Visible="false"></asp:Label></li>
                        </ul>
                        
                        <ul>
                            <li>物料代码:</li>
                            <li>
                                <asp:Label ID="tblock_ItemCode" runat="server" Text="" Visible="false"></asp:Label>
                            </li>
                        </ul>
                        <ul><li></li></ul>
                        <ul>
                            <li>领&nbsp;料量:</li>
                            <li>
                                <asp:TextBox ID="tb_Get_Count" runat="server" Text="0" AutoPostBack="true" onclick="Method2(this.id)" CssClass="style2" Visible="false"></asp:TextBox>
                            </li>
                        </ul>
                        <ul>
                            <li><asp:Label ID="lb_ProductName" runat="server" Text="牌&nbsp;&nbsp;号:" Visible="false"></asp:Label></li>
                            <li>
                        </ul>                      
                	</div>--%>
            </div>
            <div class="table_bottombg">
        	<ul>
                <li>
                    <asp:Button ID="btn_Ensure" runat="server" Text="" Width="98px" Height="33px" 
                        Style="background-image: url(/images/btn_enter.gif); background-position:center; border=0; background-repeat: no-repeat" 
                        onclick="btn_Ensure_Click"/></li>
                <li>
                    <asp:Button ID="btn_Return" runat="server" Text="" Width="98px" Height="33px" 
                        Style="background-image: url(/images/btn_back.gif); background-position:center; border=0; background-repeat: no-repeat" 
                        onclick="btn_Return_Click"/></li>
            </ul>
        </div>
        </div>
        
    </div>
    </form>
</body>
</html>
