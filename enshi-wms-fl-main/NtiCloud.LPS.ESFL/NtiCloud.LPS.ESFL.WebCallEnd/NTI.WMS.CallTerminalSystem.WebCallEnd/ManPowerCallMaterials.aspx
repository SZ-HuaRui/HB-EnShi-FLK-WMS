<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManPowerCallMaterials.aspx.cs" Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.ManPowerCallMaterials" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>人工退料</title>
<base target="_self" />
    <link href="Styles/master.css" rel="Stylesheet" type="text/css" />
        <script src="js/jquery.js" type="text/javascript"></script>
    <script src="js/jquery-ui.js" type="text/javascript"></script>
    <script src="js/common.js" type="text/javascript"></script>
     <link href="css/common.css" rel="stylesheet" type="text/css" />
<%--<script type="text/javascript" src="Scripts/keyboard.js" language="javascript"></script>--%>
<script language="javascript" type="text/javascript">
</script>
    <script language="javascript" type="text/javascript">
        function firm() {
            if (confirm("请代班长输入退料密码!")) {//如果是true
                return true;
            }
            else {
                return false;
            }
        }
   </script>
    <script type="text/javascript" language="javascript">
        function Method2(id) {
            var reValue = showModalDialog("NumKey.aspx", window, "dialogWidth=880px;dialogHeight=200px;dialogTop:600px;help:no;resizable:no;status:no;scroll:yes;center:yes;scroll:yes;"); //保存窗体的返回值  
            var array = reValue; //保存返回值  
            var uname = document.getElementById(id);
            if (reValue != null) {
                uname.value = array;
            }
        } 
    </script>
    <script type="text/javascript" language="javascript">
        function Method(id) {
            var reValue = showModalDialog("KeyBoard.aspx", window, "dialogWidth=880px;dialogHeight=340px;dialogTop:600px;help:no;resizable:no;status:no;scroll:yes;center:yes;scroll:yes;location:no;"); //保存窗体的返回值  
            var array = reValue; //保存返回值  
            var uname = document.getElementById(id);
            if (reValue != null) {
                uname.value = array;
            }
        }
    </script>
    <style type="text/css">
        .style2
        {
            font-size: 12px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main">
    	<div class="table_middle">
            <div class="sitebg">当前所在位置：人工领料</div>
            <div class="tabletitle">
                <ul>
                    <li><img src="images/table_pic01.gif" /></li>
                    <li>机台信息</li>
                </ul>
            </div>
    <div class="table_module1bg">
                <div class="table_module1">
                    <ul>
                        <li>
                            <asp:Label ID="Label2" runat="server" Text="站台编号:"></asp:Label>
                        </li>
                        <li>
                            <asp:TextBox ID="txtblk_Platform_Num" runat="server"></asp:TextBox>
                        </li>
                    </ul> 
                   <ul>
                        <li>
                            <asp:Label ID="Label3" runat="server" Text="区段:"></asp:Label>
                        </li>
                        <li>
                            <asp:TextBox ID="txtblk_Section" runat="server"></asp:TextBox>
                        </li>
                    </ul>
                    <ul>
                        <li>
                            <asp:Label ID="Label4" runat="server" Text="生产牌号:"></asp:Label>
                        </li>
                        <li>
                            <asp:TextBox ID="txtblk_Brand" runat="server"></asp:TextBox>
                        </li>
                    </ul>
                    <ul>
                        <li>
                            <asp:Label ID="Label5" runat="server" Text="站台名称:"></asp:Label>
                        </li>
                        <li>
                            <asp:TextBox ID="txtblk_Platform_Name" runat="server"></asp:TextBox>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="tabletitle">
                <ul>
                    <li><img src="images/table_pic01.gif" /></li>
                    <li>领料信息</li>
                </ul>
            </div>
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" >
                    <tr>
                        <asp:GridView ID="lv_CallMaterial" runat="server" AllowSorting="True"
                        AutoGenerateColumns="False" PageSize="7" 
                        BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                        CellPadding="3" Font-Size="12px" Width="100%" 
                        AllowPaging="True">
                        <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="12px"/>
                        <emptydatarowstyle BackColor="#eff5f8"
                                    forecolor="#000066" Font-Size="12px" BorderColor="#CCCCCC" BorderWidth="1px"/>
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
                                                    配盘单号
                                                </th>
                                            </tr>
                                            <tr>
                                                <td></td><td></td><td></td><td></td><td></td><td></td><td></td>
                                            </tr>
                                        </table>
                        </EmptyDataTemplate>
                        <Columns>
                            <%--<asp:TemplateField HeaderText="ContainerDetailID" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtContainerDetailID" Text='<%# Eval("CONTAINERDETAILID") %>' Visible="false"
                                        ReadOnly="true" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>--%>

                            <asp:TemplateField HeaderText="工单号" ItemStyle-Height="40px">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="Schemeid" Text='<%# Eval("Schemeid") %>' ReadOnly="true"
                                        runat="server" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Itemid" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtItemId" Text='<%# Eval("Itemid") %>' Visible="false"
                                        ReadOnly="true" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="物料" ItemStyle-Height="40px">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="item" Text='<%# Eval("ITEMNAME") %>' ReadOnly="true"
                                        runat="server" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--<asp:TemplateField HeaderText="批次号" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="lotNumber" Text='<%# Eval("LOTNUMBER") %>' runat="server" ReadOnly="true" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                            <asp:TemplateField HeaderText="标准计量">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="bomqty" Text='<%# Eval("BOMQTY") %>' ReadOnly="true" runat="server" CssClass="style2"
                                        />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="领料量">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:TextBox ID="txtQTY" Text='<%# Eval("QTY") %>' runat="server" AutoPostBack="true"
                                        Width="80px" onclick="showNumberInput(this)" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="单位">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="check" Text='<%# Eval("CNAME") %>' runat="server" Width="40px" ReadOnly="true" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="供应商">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="ITEMSUPPLIER" Text='<%# Eval("ShipperName") %>' ReadOnly="true" runat="server"
                                        Width="140px" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="规格">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="Description" Text='<%# Eval("DESCRIPTION") %>' ReadOnly="true" runat="server"
                                        Width="60px" CssClass="style2"/>
                                </ItemTemplate>
                            </asp:TemplateField>                       
                        </Columns>
                        <FooterStyle BackColor="White" ForeColor="#000066" />
                        <PagerStyle BackColor="#f3f8fa" ForeColor="#000066" HorizontalAlign="Center" Font-Bold="True" Height="40px"  />
                        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" Font-Size="Medium" />
                        <HeaderStyle BackColor="#c7e0ee" Font-Bold="True" ForeColor="#4" Height="34" />
                    </asp:GridView>
                    </tr>
                </table>
            </div>
            <div class="list_options" style=" padding-top:10px; height:30px;">
            	<div class="left"><%--操作类别--%></div>
                    <%--<ul>
                        <li>
                            <asp:Label ID="Label6" runat="server" Text="目的区域" Visible="true"></asp:Label>
                            ：</li>
                        <li>
                            暂存区</li>
                        <li></li>
                    </ul>--%>
            	</div>
            <div class="table_bottombg">
                <ul>
                    <li>
                        <asp:Button ID="btn_Finish" runat="server" Text="" Style="background-image: url(/images/btn_complete.gif);
                            background-position: center;border:0px; background-repeat: no-repeat" Height="33px" Width="98px"
                            OnClick="btn_Finish_Click"/></li><%--OnClientClick="return firm();"--%>
                    <li>
                        <asp:Button ID="btn_Cancel" runat="server" Text="" Style="background-image: url(/images/btn_cancel.gif);
                            background-position: center;border:0px; background-repeat: no-repeat" Height="33px" Width="98px"
                            OnClick="btn_Cancel_Click" /></li>
                </ul>
            </div>
            </div>
        </div>
    </form>
     <div id="number_input" class="number_bg">
			<div class="number_text">
				<input type="text" id="number_text_in" class="number_text_in" />
			</div>
			<ul><li>1</li><li>2</li><li>3</li><li>←</li></ul>
			<ul><li>4</li><li>5</li><li>6</li><li>0</li></ul>
			<ul><li>7</li><li>8</li><li>9</li><li>.</li></ul>
			<ul><li></li><li></li><li>关闭</li><li>确认</li></ul>
		</div>
</body>
</html>
