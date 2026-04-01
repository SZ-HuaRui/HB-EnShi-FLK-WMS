<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Return_Bad_Materials.aspx.cs" Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Return_Bad_Materials" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>退坏料</title>
    <base target="_self" />
    <link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/keyboard.js"></script>
    <link href="Scripts/RFJScript.js" type="text/javascript" />
    <script type="text/javascript" language="javascript">
        function OpenPage() {
            var url = "Return_Materials_Result.aspx";
            var result = window.showModalDialog(url, window, "DialogHeight=380px;dialogWidth:900px;status:no;scrollbars:auto;Resizable=no;");
//            if (result != null) {
//                document.getElementsByName("return").value = result;
//                return result;
//            }
        }  
    </script> 
    
<script language="javascript" language="javascript" type="text/javascript">
    function firm() {
        if (confirm("确定进行退坏料操作？")) {//如果是true
            return true;
        }
        else {
            return false;
        }
    }
</script>
    <%--<script language="javascript" type="text/javascript">
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
    </script> --%>
    <style type="text/css">
        .style2
        {
            font-size: 12px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main tablebg">
    	<div class="table_middle">
          <div class="sitebg">当前所在位置：退坏料</div>
            <div class="tabletitle">
                <ul>
                    <li><img src="/images/table_pic01.gif" /></li>
                    <li>机台信息</li>
                </ul>
            </div>
    <div class="table_module1bg">
                <div class="table_module1">
                    <ul>
                        <li>托盘条码:</li>
                        <li>
                            <asp:Label ID="txtblk_Sock_Code" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                    <ul>
                        <li>站台编号:</li>
                        <li>
                            <asp:Label ID="txtblk_Platform_Num" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                   
              <ul>
                        <li></li>
                        <li></li>
                    </ul>
                    <ul>
                        <li>牌&nbsp;&nbsp;号:</li>
                        <li>
                            <asp:Label ID="txtblk_Brand" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                    <ul>
                        <li>区&nbsp;&nbsp;段:</li>
                        <li>
                            <asp:Label ID="txtblk_Section" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                </div>
            </div>
            <div class="tabletitle">
                <ul>
                    <li><img src="/images/table_pic01.gif" /></li>
                    <li>退料信息</li>
                </ul>
            </div>
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" >
                    <tr class="list_title">
                        <td>
                        <asp:GridView ID="lv_Return_Bad" runat="server" AllowSorting="True"
                        AutoGenerateColumns="False" PageSize="12" 
                        DataKeyNames="CONTAINERDETAILID" OnPageIndexChanging="lv_Return_Bad_PageIndexChanging"
                        BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                        CellPadding="3" Font-Size="12px" Width="100%" 
            AllowPaging="True" onrowcommand="lv_Return_Bad_RowCommand" >
                        <RowStyle ForeColor="#000066" Wrap="false" Font-Size="12px"/>
                        <emptydatarowstyle 
                                    forecolor="#000066" Font-Size="14px" BorderColor="#CCCCCC" BorderWidth="1px"/>
                                    <EmptyDataTemplate>
                                        <table class="sublist" width="100%">
                                            <tr>
                                                <th>
                                                    物料
                                                </th>
                                                <th>
                                                    批次号
                                                </th>
                                                <th>
                                                    标准计量
                                                </th>
                                                <th>
                                                    退料量
                                                </th>
                                                <th>
                                                    单位
                                                </th>
                                                <th>
                                                    原因
                                                </th>
                                            </tr>
                                            <tr>
                                                <td></td><td></td><td></td><td></td><td></td><td></td>
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
                            <asp:TemplateField HeaderText="CURCONTAINERID" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtCURCONTAINERID" Text='<%# Eval("CURCONTAINERID") %>' Visible="false"
                                        ReadOnly="true" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CURPALLETNUMBER" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtCURPALLETNUMBER" Text='<%# Eval("CURPALLETNUMBER") %>' Visible="false"
                                        ReadOnly="true" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="MODELCODE" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtMODELCODE" Text='<%# Eval("MODELCODE") %>' Visible="false" ReadOnly="true"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="BOMID" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtBOMID" Text='<%# Eval("BOMID") %>' Visible="false" ReadOnly="true"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="RATE" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtRATE" Text='<%# Eval("RATE") %>' Visible="false" ReadOnly="true"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="物料" ItemStyle-Height="40px">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <ItemTemplate>
                                    <asp:Label ID="item" Text='<%# Eval("ITEMNAME") %>'  ReadOnly="true"
                                        runat="server" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="批次号">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <ItemTemplate>
                                    <asp:Label ID="LOTNUMBER" Text='<%# Eval("LOTNUMBER") %>' runat="server" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                           <asp:TemplateField HeaderText="标准计量">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <ItemTemplate>
                                    <asp:Label ID="BOMQTY" Text='<%# Eval("BOMQTY") %>' runat="server" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="退料量">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <ItemTemplate>
                                    <asp:TextBox ID="QTY" Text='<%# Eval("QTY") %>' runat="server" CssClass="style2" AutoPostBack="true" onclick="Method2(this.id)"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                                  
                            <asp:TemplateField HeaderText="零头1" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <ItemTemplate>
                                    <asp:TextBox ID="ODD1" Text='<%# Eval("ODD1") %>' runat="server" Width="50px" AutoPostBack="true" onclick="Method2(this.id)"/>
                                        <%--onkeyup="javascript:clearNoNum(this)" onblur="CheckQtyPatten(this,3)"--%> 
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="零头2" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <ItemTemplate>
                                    <asp:TextBox ID="ODD2" Text='<%# Eval("ODD2") %>' runat="server" Width="50px" AutoPostBack="true" onclick="Method2(this.id)"/>
                                        <%--onkeyup="javascript:clearNoNum(this)" onblur="CheckQtyPatten(this,3)"--%> 
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="零头3" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <ItemTemplate>
                                    <asp:TextBox ID="ODD3" Text='<%# Eval("ODD3") %>' runat="server" Width="50px" AutoPostBack="true" onclick="Method2(this.id)"/>
                                        <%--onkeyup="javascript:clearNoNum(this)" onblur="CheckQtyPatten(this,3)" --%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="单位">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <ItemTemplate>
                                    <asp:Label ID="check" Text='<%# Eval("CNAME") %>' runat="server" Width="50px" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="增加原因" Visible="false">
                                <ItemTemplate>
                                    <asp:Button ID="btn_Add" Text='增加' runat="server" CommandName="reason" CommandArgument='<%# Eval("CONTAINERDETAILID") %>' 
                                    OnClientClick="OpenPage()" Width="60px" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="原因">
                                <ItemTemplate>
                                    <asp:TextBox ID="field3" Text='<%# Eval("FIELD3") %>' runat="server" CssClass="style2" Enabled="true"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <FooterStyle BackColor="White" ForeColor="#000066" />
                        <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Center" 
                            Font-Bold="True" />
                        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                    </asp:GridView>
                        </td>
                    </tr>
                </table>
                    <div class="list_page">
                    </div>
            </div>
            <div class="list_options">
            	<div class="left">操作类别</div>
                    <ul>
                        <li>目的区域</li>
                        <li>
                            <asp:DropDownList ID="target_Area" runat="server" Height="35px" 
                                onselectedindexchanged="target_Area_SelectedIndexChanged" Width="140px" 
                                Font-Size="11pt">
                            </asp:DropDownList>
                        </li>
                    </ul>
            	</div>
            <div class="table_bottombg">
                <ul>
                    <li>
                        <asp:Button ID="btn_Finish" runat="server" Text="" Width="98px" Height="33px" Style="background-image: url(/images/btn_complete.gif);
                            background-position: center; background-repeat: no-repeat" OnClick="btn_Finish_Click" OnClientClick="return firm();"/>
                    </li>
                    <li>
                        <asp:Button ID="btn_Cancel" runat="server" Text="" Width="98px" Height="33px" Style="background-image: url(/images/btn_cancel.gif);
                            background-position: center; background-repeat: no-repeat" 
                            onclick="btn_Cancel_Click" /></li>
                </ul>
            </div>
            </div>
        </div>
        
        
    </form>
</body>
</html>
