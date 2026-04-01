<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Return_Materials_Confirm.aspx.cs" Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Return_Materials_Confirm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>退料确认</title>
    <link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <link href="Scripts/RFJScript.js" type="text/javascript" />
    <script type="text/javascript" src="Scripts/keyboard.js"></script>
    <script type="text/javascript">
        function Check() {
            if (confirm("站台将执行“退回此单”操作,请确认!")) {//如果是true
                return true;
            }
            else {
                return false;
            }
        }
        function Ensure() {
            if (confirm("站台将执行“确认退料单”操作,请确认!")) {//如果是true
                return true;
            }
            else {
                return false;
            }
        }
        function AllConfirm() {
            if (confirm("您将执行“确认全部退料单”操作,请确认!")) {//如果是true
                return true;
            }
            else {
                return false;
            }
        }
    </script> 
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
    <div class="sitebg">
            	当前所在位置：退料确认
            </div>
    	<div class="table_middle">
       	  
        	<div class="tabletitle">
                <ul>
                    <li><img src="/images/table_pic01.gif" /></li>
                    <li>查询设置</li>
                </ul>
            </div>
   			<div class="creat_set">
               
                <ul>
                    <li>机台号</li>
                    <li>
                        <asp:DropDownList ID="cb_MachineNumber" runat="server" 
                            onselectedindexchanged="cb_MachineNumber_SelectedIndexChanged" 
                            Width="90px">
                        </asp:DropDownList>
                    </li>
                </ul>
                <ul>
                	<li>站台编号</li>
                    <li>
                        <asp:DropDownList ID="cb_DockNumber" runat="server" Width="90px">
                        </asp:DropDownList>
                    </li>
                    <li>
                        <asp:Button ID="btn_Query" runat="server" Text="查 询" CssClass="btn" 
                            onclick="btn_Query_Click" Font-Size="Large"/></li>
                    <li>
                        <asp:Button ID="btn_requery" runat="server" Text="刷 新" CssClass="btn" 
                            onclick="btn_requery_Click" Font-Size="Large"/></li>
                </ul>
            </div>
            <div class="tabletitle">
                <ul>
                    <li><img src="/images/table_pic01.gif" /></li>
                    <li>退料单</li>
                </ul>
            </div>
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" >
                    <tr>
                        <td><asp:GridView ID="lv_Return_Bill" runat="server" AllowSorting="True"
                        AutoGenerateColumns="False" PageSize="7" 
                        OnPageIndexChanging="lv_Create_Bill_PageIndexChanging"
                        BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                        CellPadding="3" Font-Size="14pt" Width="100%" 
            AllowPaging="True" >
                         <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="14px" Height="40px"/>
                        <emptydatarowstyle 
                                    forecolor="#000066" Font-Size="14pt" BorderColor="#eff5f8" BorderWidth="1px"/>
                                    <EmptyDataTemplate>
                                        <table class="sublist" width="100%">
                                            <tr>
                                                <th>
                                                    选择
                                                </th>
                                                <th>
                                                    机台号
                                                </th>
                                                <th>
                                                    站台位
                                                </th>
                                                <th>
                                                    类型
                                                </th>
                                                <th>
                                                    创建时间
                                                </th>
                                                <th>
                                                    区段/模式
                                                </th>
                                                <th>
                                                    配盘单号
                                                </th>
                                            </tr>
                                            <tr>
                                                <td></td><td></td><td></td><td></td><td></td><td></td><td></td>
                                            </tr>
                                        </table>
                        </EmptyDataTemplate>
                        <Columns>
                            
                            <%--<asp:TemplateField HeaderText="ItemID" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtItemID" Text='<%# Eval("ITEMID") %>' Visible="false" ReadOnly="true"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                            <asp:TemplateField HeaderText="BILLID" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="BILLID" Text='<%# Eval("BILLID") %>' Visible="false"
                                        ReadOnly="true" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="选择查看退料明细" ItemStyle-Height="40px" Visible="false">
                                            <HeaderTemplate>
                                                <input id="ckbAll" type="checkbox" onclick="CheckAll" />
                                            </HeaderTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" />
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkSelect" runat="server" OnCheckedChanged="ChkSelect_select" AutoPostBack="true"/>
                                            </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="机台号">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="CMachineNumber" Text='<%# Eval("MACHINENUMBER") %>' ReadOnly="true"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="站台位">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="CDock" Text='<%# Eval("DOCKNUMBER") %>' ReadOnly="true"
                                        runat="server" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="类型">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="CType" Text='<%# Eval("BILLTYPENAME") %>' ReadOnly="true" runat="server"
                                         CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="确认此单">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Button ID="CEnsure" Text='确 认' runat="server" CssClass="style2"
                                        Width="80px" OnClick="btn_Confirm_Item_Click" OnClientClick="return Ensure();"/>
                                </ItemTemplate>
                            </asp:TemplateField>  
                            <asp:TemplateField HeaderText="退回此单">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Button ID="CReturn" Text='退 单' runat="server" CssClass="style2"
                                        Width="80px" OnClick="btn_Return_Item_Click" OnClientClick="return Check();"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="创建时间">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="CTime" Text='<%# Eval("CREATETIME") %>' runat="server" ReadOnly="true" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>                    
                            <asp:TemplateField HeaderText="区段/模式">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="CModel" Text='<%# Eval("CONTAINERTYPE") %>' runat="server" Width="50px"
                                        ReadOnly="true" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="配盘单号">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="BOMCODE" Text='<%# Eval("BOMCODE") %>' runat="server" 
                                         ReadOnly="true" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <FooterStyle BackColor="White" ForeColor="#000066" />
                        <PagerStyle BackColor="#f3f8fa" ForeColor="#000066" HorizontalAlign="Center" Font-Bold="True" Height="40px"  />
                        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#c7e0ee" Font-Bold="True" ForeColor="#4" Height="34" />
                    </asp:GridView></td>
                    </tr>
                </table>
         
            </div>
             <div class="list_page"></div>
             <div class="tabletitle">
                <ul>
                    <li><img src="/images/table_pic01.gif" /></li>
                    <li>退料明细</li>
                </ul>
            </div>
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" >
                    <tr>
                        <td>
                        <asp:GridView ID="lv_Return_Materials_list" runat="server" AllowSorting="True"
                        AutoGenerateColumns="False" PageSize="7" 
                        DataKeyNames="CONTAINERDETAILID" 
                        BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                        CellPadding="3" Font-Size="14pt" Width="100%" 
            AllowPaging="True" onselectedindexchanged="lv_Return_Materials_list_SelectedIndexChanged" 
                                ShowHeaderWhenEmpty="True" >
                         <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="14px"/>
                        <emptydatarowstyle 
                                    forecolor="#000066" Font-Size="14pt" BorderColor="#eff5f8" BorderWidth="1px"/>
                                    <EmptyDataTemplate>
                                        <table class="sublist" width="100%">
                                            <tr>
                                                <th>
                                                    站台位
                                                </th>
                                                <th>
                                                    物料名称
                                                </th>
                                                <th>
                                                    数量
                                                </th>
                                                <th>
                                                    修改数量
                                                </th>
                                                <th>
                                                    是否锁定
                                                </th>
                                            </tr>
                                            <tr>
                                                <td></td><td></td><td></td><td></td><td></td><td></td><td></td>
                                            </tr>
                                        </table>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderText="ContainerDetailID" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtContainerDetailID" Text='<%# Eval("CONTAINERDETAILID") %>' Visible="false"
                                        ReadOnly="true" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="站台位" ItemStyle-Height="40px">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="CDockNumber" Text='<%# Eval("DOCKNUMBER") %>' ReadOnly="true"
                                        runat="server" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="物料名称">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="CItemName" Text='<%# Eval("ITEMNAME") %>' ReadOnly="true" runat="server" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                              
                            <asp:TemplateField HeaderText="数  量">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:TextBox ID="QTY1" Text='<%# Eval("QTY1") %>'  runat="server" AutoPostBack="true" 
                                    onkeyup="javascript:clearNoNum(this)" onclick="Method2(this.id)" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>                    
                            <asp:TemplateField HeaderText="零头1" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <ItemTemplate>
                                    <asp:TextBox ID="ODD1" Text='<%# Eval("ODD1") %>' runat="server" 
                                          AutoPostBack="true" Width="50px" onkeyup="javascript:clearNoNum(this)" onblur="CheckQtyPatten(this,3)" onclick="Method2(this.id)"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="零头2" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <ItemTemplate>
                                    <asp:TextBox ID="ODD2" Text='<%# Eval("ODD2") %>' runat="server" 
                                          AutoPostBack="true" Width="50px" onkeyup="javascript:clearNoNum(this)" onblur="CheckQtyPatten(this,3)" onclick="Method2(this.id)"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="零头3" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <ItemTemplate>
                                    <asp:TextBox ID="ODD3" Text='<%# Eval("ODD3") %>' runat="server" Width="50px"
                                        onkeyup="javascript:clearNoNum(this)" onblur="CheckQtyPatten(this,3)" AutoPostBack="true" onclick="Method2(this.id)"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="修改数量">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Button ID="CButton" Text='修 改'  runat="server"
                                        OnClick="btn_Change_Click" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="是否锁定">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:CheckBox ID="CCheck" CssClass="style2" runat="server" Text="锁定" OnCheckedChanged="CheckBox_Checked" AutoPostBack="true"/>
                                            </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="批 次" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="CLotNumber" Text='<%# Eval("LOTNUMBER") %>' runat="server" CssClass="style2" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="备注" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtBz" Text='<%# Eval("FIELD31") %>' runat="server" Width="50px" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <FooterStyle BackColor="White" ForeColor="#000066" />
                        <PagerStyle BackColor="#f3f8fa" ForeColor="#000066" HorizontalAlign="Center" Font-Bold="True" Height="40px"  />
                        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#c7e0ee" Font-Bold="True" ForeColor="#4" Height="34" />
                    </asp:GridView></td>
                    </tr>
                </table>
                
            </div>
            <div class="table_bottombg">
                <ul>
                    <li>
                        <asp:Button ID="btn_All_confirm" runat="server" Text="" Width="98px" Height="33px"
                            Style="background-image: url(/images/btn_enter.gif); border:0px; background-position: center;
                            background-repeat: no-repeat"  OnClick="btn_All_confirm_Click"  OnClientClick="return AllConfirm();" /></li>
                    <li>
                        <asp:Button ID="btn_Quit" runat="server" Text="" Width="98px" Height="33px" Style="background-image: url(/images/btn_back.gif);
                            background-position: center; border:0px; background-repeat: no-repeat" OnClick="btn_Quit_Click" /></li>
                </ul>
            </div>
        </div>
        
        
    </div>
    </form>
</body>
</html>
