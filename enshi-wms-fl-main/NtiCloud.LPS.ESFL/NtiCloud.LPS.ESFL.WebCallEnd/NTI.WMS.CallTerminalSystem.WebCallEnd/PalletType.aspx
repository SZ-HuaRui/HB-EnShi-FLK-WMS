<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PalletType.aspx.cs" Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.PalletType" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>设置托盘类型</title>
    <base target="_self" />
    <link href="Styles/master.css" rel="Stylesheet" type="text/css" />
    <%--<script type="text/javascript" src="Scripts/keyboard.js" language="javascript"></script>--%>
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
        function firm() {
            if (confirm("确定进行换牌操作？")) {//如果是true
                return true;
            }
            else {
                return false;
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
     <div class="sitebg">当前所在位置：设置托盘类型</div>
    	<div class="table_middle">
          
            <div class="tabletitle">
                <ul>
                    <li><img src="/images/table_pic01.gif" /></li>
                    <li>托盘类型信息</li>
                </ul>
            </div>
    <div class="table_module1bg">
                <div class="table_module1">
                    <ul>
                        <li>机台编号:</li>
                        <li><asp:Label ID="Label1" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                    <ul>
                        <li>机台名称:</li>
                        <li><asp:Label ID="labdocknumber" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                   
              <ul>
                        <li>班&nbsp;&nbsp;次:</li>
                        <li><asp:Label ID="labbc" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                    <ul>
                        <li>生产牌号:</li>
                        <li><asp:Label ID="Label2" runat="server" Text="未设置" Width="165px"></asp:Label></li>
                    </ul>
                    <ul>
                        <li>
                            <asp:Label ID="Label4" runat="server" Text="前一牌号:" Visible="false"></asp:Label>
                        </li>
                        <li></li>
                        <asp:Label ID="Label5" runat="server" Text="未设置" Visible="false"></asp:Label>
                    </ul>
                </div>
            </div>
            <div class="tabletitle">
                <ul>
                    <li><img src="/images/table_pic01.gif" /></li>
                    <li>设置托盘类型</li>
                </ul>
            </div>
    <div>
    <table width="100%" border="0" cellpadding="0" cellspacing="0" >
                    <tr>
                        <td>
                        <asp:GridView ID="gvChangeItem" runat="server" AllowSorting="True"
                                    AutoGenerateColumns="False" PageSize="7" BackColor="White" BorderColor="#CCCCCC"
                                    BorderStyle="None" BorderWidth="1px" CellPadding="3" OnPageIndexChanging="gvChangeItem_PageIndexChanging"
                                    Font-Size="Medium" AllowPaging="True" Width="100%" 
                                ShowHeaderWhenEmpty="True">
                                   <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" 
                                       Font-Size="Medium"/>
                         <emptydatarowstyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="14px"/> 
                            <EmptyDataTemplate>
                                <table class="sublist" width="100%">
                                    <%--<tr>
                                        <th>
                                            设置托盘类型
                                        </th>
                                        <th>
                                            机台号
                                        </th>
                                        <th>
                                            站台编号
                                        </th>
                                        <th>
                                            站台名称
                                        </th>
                                        <th>
                                            托盘类型
                                        </th>
                                        <th>
                                            机台类型
                                        </th>
                                    </tr>--%>
                                    <tr>
<%--                                        <td>
                                        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                                        </td>
                                        <td><asp:TextBox ID="TextBox2" runat="server"></asp:TextBox></td>
                                        <td><asp:TextBox ID="TextBox3" runat="server"></asp:TextBox></td>
                                        <td><asp:TextBox ID="TextBox4" runat="server"></asp:TextBox></td>
                                        <td><asp:TextBox ID="TextBox5" runat="server"></asp:TextBox></td>
                                        <td><asp:TextBox ID="TextBox6" runat="server"></asp:TextBox></td>
                                        <td><asp:TextBox ID="TextBox7" runat="server"></asp:TextBox></td>
                                        <td><asp:TextBox ID="TextBox8" runat="server"></asp:TextBox></td>
                                        <td><asp:TextBox ID="TextBox9" runat="server"></asp:TextBox></td>--%>
                                    </tr>
                                </table>
                            </EmptyDataTemplate>
                                    <Columns>
                                        <asp:TemplateField HeaderText="ContainerDetailID" Visible="false">
                                          <ItemTemplate>
                                          <asp:TextBox ID="txtContainerDetailID" Text='<%# Eval("machinedockid") %>' Visible="false"
                                             ReadOnly="true" runat="server" />
                                          </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="ContainerDetailID" Visible="false">
                                          <ItemTemplate>
                                          <asp:TextBox ID="txtCURCONTAINERID" Text='<%# Eval("CURCONTAINERID") %>' Visible="false"
                                             ReadOnly="true" runat="server" />
                                          </ItemTemplate>
                                        </asp:TemplateField>

                                       <asp:TemplateField HeaderText="ContainerDetailID" Visible="false">
                                          <ItemTemplate>
                                          <asp:TextBox ID="machinenumber" Text='<%# Eval("machinenumber") %>' Visible="false"
                                             ReadOnly="true" runat="server" />
                                          </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="ContainerDetailID" Visible="false">
                                          <ItemTemplate>
                                          <asp:TextBox ID="machinetype" Text='<%# Eval("machinetype") %>' Visible="false"
                                             ReadOnly="true" runat="server" />
                                          </ItemTemplate>
                                        </asp:TemplateField>

                                       <asp:TemplateField HeaderText="ContainerDetailID" Visible="false">
                                          <ItemTemplate>
                                          <asp:TextBox ID="dockstatus" Text='<%# Eval("dockstatus") %>' Visible="false"
                                             ReadOnly="true" runat="server" />
                                          </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="ContainerDetailID" Visible="false">
                                          <ItemTemplate>
                                          <asp:TextBox ID="itemsource" Text='<%# Eval("itemsource") %>' Visible="false"
                                             ReadOnly="true" runat="server" />
                                          </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="ContainerDetailID" Visible="false">
                                          <ItemTemplate>
                                          <asp:TextBox ID="modelid" Text='<%# Eval("modelid") %>' Visible="false"
                                             ReadOnly="true" runat="server" />
                                          </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="ContainerDetailID" Visible="false">
                                          <ItemTemplate>
                                          <asp:TextBox ID="predocknumber" Text='<%# Eval("perdocknumber") %>' Visible="false"
                                             ReadOnly="true" runat="server" />
                                          </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="设置托盘类型"  ItemStyle-Height="60px" ItemStyle-Width="100px">
                                            <HeaderTemplate>
                                                <input id="ckbAll" type="checkbox" onclick="CheckAll('gvChangeItem',this)" />
                                            </HeaderTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" CssClass="list_text"  />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:CheckBox ID="ckbSelect" runat="server" OnCheckedChanged="ckbSelect_CheckedChanged"
                                                    AutoPostBack="true" Checked="false" Style="zoom: 50%;" Text="勾选" Font-Bold="True" Font-Size="X-Large" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="机台号" HeaderStyle-Width="120px">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblDocNumber" Font-Size="14px" runat="server" Text='<%# Eval("machinenumber") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="站台编号" HeaderStyle-Width="100px">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblCurPalletNumber" Font-Size="14px"  runat="server" Text='<%# Eval("docknumber") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="站台名称" HeaderStyle-Width="100px">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblCurPalletNumber1" Font-Size="14px"  runat="server" Text='<%# Eval("dockname") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="托盘类型">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="TxtItemName" Font-Size="14px"  runat="server" Text='<%# Eval("modelcode") %>' ReadOnly="true"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="机台类型">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblModelCode" runat="server" Font-Size="14px"  Text='<%# Eval("type_desc") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <FooterStyle  ForeColor="#000066" />
                                    <PagerStyle BackColor="#f3f8fa" ForeColor="#000066" HorizontalAlign="Center" Font-Bold="True" Height="30px"  />
                                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                                    <HeaderStyle BackColor="#c7e0ee" Font-Bold="True" ForeColor="#4" Height="40" />
                                </asp:GridView>
                        </td>
                        </tr>
                        <div><asp:HiddenField ID="hfAllowODD" runat="server" /></div>
                </table>
    </div>
    <div class="table_bottombg">
                <ul>
                    <li>
                        <asp:Button ID="btn_Ensure" runat="server" Text="" Width="98px" Height="33px" Style="background-image: url(/images/btn_enter.gif);
                            background-position: center;border:0px; background-repeat: no-repeat" 
                            OnClientClick="return firm();" onclick="btn_Ensure_Click"/></li>
                    <li>
                        <asp:Button ID="btn_Return" runat="server" Text="" Width="98px" Height="33px" Style="background-image: url(/images/btn_back.gif);
                            background-position: center;border:0px; background-repeat: no-repeat" 
                            onclick="btn_Return_Click" /></li>
                </ul>
                <asp:Label ID="Label3" runat="server" Text="托盘类型"></asp:Label>
                <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" 
                    Height="25px" Width="274px" 
                    onselectedindexchanged="DropDownList1_SelectedIndexChanged" Font-Size="Large">
                </asp:DropDownList>
            </div>
            </div>
            </div>
    </form>
</body>
</html>
