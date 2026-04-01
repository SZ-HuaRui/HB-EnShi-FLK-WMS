<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Materials_Hand.aspx.cs" Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Materials_Hand" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>人工领料</title>
    <base target="_self" />
    <link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <script language="JavaScript" src="Scripts/RFJScript.js" type="text/javascript"></script>
    <script type="text/javascript" src="Scripts/keyboard.js"></script>
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
        .style1
        {
            background: url('images/sitebg.gif') repeat-x;
            width: auto;
            height: 38px;
            color: #000000;
            font-size: 20px;
            padding-top: 10px;
            padding-left: 10px;
        }
        .style2
        {
            font-size: 14px;
        }
    </style>
    
</head>
<body>
    <form id="form1" runat="server">
    <div class="main">
    <div class="style1">当前所在位置：人工领料</div>
    	<div class="table_middle">
        <div class="tabletitle">
                <ul>
                    <li><img src="/images/table_pic01.gif" /></li>
                    <li>机台信息</li>
                </ul>
            </div>
    <div class="table_module1bg">
                <div class="table_module1">
                    <ul>
                        <li>机台编号:</li>
                        <li><asp:Label ID="Label2" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                    <ul>
                        <li>机台名称:</li>
                        <li><asp:Label ID="labdocknumber" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                   
              <ul>
                        <li>班&nbsp;&nbsp;时:</li>
                        <li><asp:Label ID="labbc" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                    <ul>
                        <li>生产牌号:</li>
                        <li><asp:Label ID="Label3" runat="server" Text="未设置" Width="165px"></asp:Label></li>
                    </ul>
                    <ul>
                        <li></li>
                        <li></li>
                    </ul>
                </div>
            </div>
            <div class="tabletitle">
                <ul>
                    <li><img src="/images/table_pic01.gif" /></li>
                    <li>
                        <asp:Label ID="Label1" runat="server" Text="未设置"></asp:Label></li>
                </ul>
            </div>
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" >
                    <tr>
                        <td><asp:GridView ID="lv_Materials_Hand" runat="server" AllowSorting="True"
                        AutoGenerateColumns="False" PageSize="10" 
                        DataKeyNames="CONTAINERDETAILID" OnPageIndexChanging="lv_Materials_Hand_PageIndexChanging"
                        BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                        CellPadding="3" Font-Size="12pt" Width="100%" 
            AllowPaging="True" >
                        <RowStyle ForeColor="#000066" Wrap="false" Font-Size="12pt"/>
                        <emptydatarowstyle backcolor="LightBlue"
                                    forecolor="#000066" Font-Size="12pt" BorderColor="#CCCCCC" BorderWidth="1px"/>
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
                                                    领料人
                                                </th>
                                                <th>
                                                    领料数量
                                                </th>
                                                <th>
                                                    实发料量
                                                </th>
                                                <th>
                                                    状态
                                                </th>
                                                <th>
                                                    领料时间
                                                </th>
                                            </tr>
                                            <tr>
                                                <td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td>
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
                            <asp:TemplateField HeaderText="ItemID" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtItemID" Text='<%# Eval("ITEMID") %>' Visible="false" ReadOnly="true"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="BillID" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="BILLID" Text='<%# Eval("BILLID") %>' Visible="false" ReadOnly="true"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="STATECODE" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="STATECODE" Text='<%# Eval("STATECODE") %>' Visible="false" ReadOnly="true"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="RATE" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtRATE" Text='<%# Eval("RATE") %>' Visible="false" ReadOnly="true"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="选择" ItemStyle-Height="40px">
                                            <HeaderTemplate>
                                                <input id="ckbAll" type="checkbox" onclick="CheckAll('grvHandWorkBill',this)" />
                                            </HeaderTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkSelect" runat="server" OnCheckedChanged="chkSelect_select" AutoPostBack="true"/>
                                            </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="物料代码">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="CItemCode" Text='<%# Eval("ITEMCODE") %>' ReadOnly="true"
                                        runat="server" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="物料名称">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:TextBox ID="CItemName" Text='<%# Eval("CITEMNAME") %>' ReadOnly="true"
                                        runat="server" Width="100px" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="领料人">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="CUser" Text='<%# Eval("USERID") %>' ReadOnly="true" runat="server"
                                        Width="90px" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="领料数量">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="CCount" Text='<%# Eval("R_QTY") %>' ReadOnly="true" runat="server"
                                        Width="90px" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>  
                            <asp:TemplateField HeaderText="实发数量">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:TextBox ID="CFactCount" Text='<%# Eval("S_QTY") %>' runat="server" width="40px" ReadOnly="true" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>                      
                            <asp:TemplateField HeaderText="零头1" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <ItemTemplate>
                                    <asp:TextBox ID="CODD1" Text='<%# Eval("ODD1") %>' runat="server" Width="50px" ReadOnly="true"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="零头2" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <ItemTemplate>
                                    <asp:TextBox ID="CODD2" Text='<%# Eval("ODD2") %>' runat="server" Width="50px" ReadOnly="true"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="零头3" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <ItemTemplate>
                                    <asp:TextBox ID="CODD3" Text='<%# Eval("ODD3") %>' runat="server" Width="50px" ReadOnly="true"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="状态">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="CStatus" Text='<%# Eval("BILLSTATENAME") %>' runat="server" Width="50px" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="领料时间">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="CTime" Text='<%# Eval("CREATETIME") %>' runat="server" CssClass="style2"/>
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
                    <asp:Button ID="btn_Create" runat="server" Text="" Width="140px" Height="33px" 
                        Style="background-image: url(/images/btn_create.gif); background-position:center;background-repeat: no-repeat" 
                        onclick="btn_Create_Click"/>
                </li>

                <li>
                    <asp:Button ID="btn_Fresh" runat="server" Text="" Width="98px" Height="33px" 
                        Style="background-image: url(/images/btn_refresh.gif); background-position:center;background-repeat: no-repeat" 
                        onclick="btn_Fresh_Click"/></li>

                <li>
                    <asp:Button ID="btn_Ensure" runat="server" Text="" Width="98px" Height="33px" 
                        Style="background-image: url(/images/btn_receive.gif); background-position:center;background-repeat: no-repeat" 
                        onclick="btn_Ensure_Click"/></li>

                <li>
                    <asp:Button ID="btn_All_Ensure" runat="server" Text="" Width="98px" 
                        Height="33px" 
                        Style="background-image: url(/images/btn_receiveall.gif); background-position:center;background-repeat: no-repeat" 
                        onclick="btn_All_Ensure_Click"/></li>

                <li>
                    <asp:Button ID="btn_Cancle" runat="server" Text="" Width="98px" Height="33px" 
                        Style="background-image: url(/images/btn_back.gif); background-position:center;background-repeat: no-repeat" 
                        onclick="btn_Cancle_Click"/></li>
            </ul>
        </div>        
        </div>      
        
    </div>
    </form>
</body>
</html>
