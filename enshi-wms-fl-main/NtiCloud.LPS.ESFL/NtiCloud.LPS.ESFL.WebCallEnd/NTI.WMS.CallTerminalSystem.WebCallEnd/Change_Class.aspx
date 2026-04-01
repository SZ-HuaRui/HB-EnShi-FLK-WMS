<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Change_Class.aspx.cs" Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Change_Class" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>交接班</title>
    <link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.js" type="text/javascript"></script>
    <script src="js/jquery-ui.js" type="text/javascript"></script>
    <script src="js/common.js" type="text/javascript"></script>
    <link href="css/common.css" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript" src="Scripts/RFJScript.js"></script>
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
    <script language="javascript" type="text/javascript">
        function firm() {
            var tatol = 0;
            $("[type='text']").each(function () {
                if ($(this).attr("id") != "number_text_in") {
                    tatol += $(this).val() - 0;
                }
            });
            if (tatol < 0) {
                alert("物料数量不能小于0！");
                return false;
            }
            if (tatol == 0) {
                alert("物料数量为0，请进行退空托盘！");
                return false;
            }
            if (confirm("确定进行交班操作？")) {
                hideNumberInput();
                hideNumberInput_p();
                $("#submitWatting").show();
                return true;
            } else {
                return false;
            }
        }

        function CheckModify() {
            var tatol = 0;
            $("[type='text']").each(function () {
                if ($(this).attr("id") != "number_text_in") {
                    tatol += $(this).val() - 0;
                }
            });
            if (tatol <= 0) {
                alert("输入的数量不能全部为 0，请重新输入！");
                return false;
            }
            if (confirm('确定要修改交班数据吗？')) {
                hideNumberInput();
                hideNumberInput_p();
                $("#submitWatting").show();
                return true;
            } else {
                return false;
            }
        }
    </script>
    <script language="javascript" type="text/javascript">
        function firm1() {//利用对话框返回的值（true 或者 false）
            if (confirm("确定当前班次消耗数量？")) {
                return true;
            }
            else {
                return false;
            }
        }
    </script>
    <style type="text/css">
        .style1 {
            color: #FF0000;
            font-size: large;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="main">
            <div class="sitebg">
                当前所在位置：交班操作
            </div>
            <%--<div class="table_middle2">--%>
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
                            <asp:Label ID="label1" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                    <ul>
                        <li>机台名称:</li>
                        <li>
                            <asp:Label ID="labdocknumber" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                    <ul>
                        <li>班&nbsp;&nbsp;次:</li>
                        <li>
                            <asp:Label ID="labbc" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                    <ul style="width: 636px;">
                        <li>生产牌号:</li>
                        <li style="width: 500px;">
                            <asp:Label ID="label2" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                    <ul>
                        <li>
                            <asp:Label ID="Label4" runat="server" Text="前一牌号:" Visible="false"></asp:Label></li>
                        <li>
                            <asp:Label ID="label3" runat="server" Text="未设置" Width="165px" Visible="false"></asp:Label>
                        </li>
                    </ul>
                </div>
            </div>
            <%--            <div class="tabletitle">
                <ul>
                    <li><img src="/images/table_pic01.gif" /></li>
                    <li>
                        <asp:Label ID="Label5" runat="server" Text="当前班次领料确认"></asp:Label></li>
                </ul>
            </div>
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr >
                        <td><asp:GridView ID="GridViewUsedMaterialConfirm" runat="server" AllowSorting="True"
                        AutoGenerateColumns="False" PageSize="12" 
                        DataKeyNames="USEDMATERIALSDETAILID" 
                        BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="0px"
                        CellPadding="0" Font-Size="12pt" Width="100%" 
                        AllowPaging="True" ShowHeaderWhenEmpty="True" EmptyDataText="本机台没有要料消耗数据" 
                                onpageindexchanged="GridViewUsedMaterialConfirm_PageIndexChanged">
                        <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="12px"/>
                         <emptydatarowstyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="12px"/> 
                        <EmptyDataTemplate>
                        <table class="sublist" width="100%">
                            <tr>
                                <td>
                                    托盘类型
                                </td>
                                <td>
                                    物料名称
                                </td>
                                <td>
                                    要料数量
                                </td>
                                <td>
                                    退料数量
                                </td>
                                <td>
                                  单位
                                </td>
                                <td>
                                  物料规格
                                </td>
                                <td>单位转换率</td>
                                <td>领料统计</td>
                            </tr>
                            <tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>
                        </table>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderText="USEDMATERIALSDETAILID" Visible="false" ItemStyle-Height="30px">
                                <ItemTemplate>
                                    <asp:TextBox ID="USEDMATERIALSDETAILID" Text='<%# Eval("USEDMATERIALSDETAILID") %>' Visible="false"
                                        ReadOnly="true" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ItemID" Visible="false" ItemStyle-Height="30px">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtitemid" Text='<%# Eval("itemid") %>' Visible="false"
                                        ReadOnly="true" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="MachineNumber" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="MachineNumber" Text='<%# Eval("MachineNumber") %>' Visible="false" 
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>                                      
                            <asp:TemplateField HeaderText="托盘类型" ItemStyle-Height="30px">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" Font-Size="14px" />
                                <ItemTemplate>
                                    <asp:Label ID="TxtModel" Text='<%# Eval("MODELCODE") %>' ReadOnly="true" runat="server"
                                        Width="70px" Font-Size="12px"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="物料代码" ItemStyle-Height="30px" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" Font-Size="14px"/>
                                <ItemTemplate>
                                    <asp:Label ID="txtItemCode" Text='<%# Eval("ITEMCODE") %>' ReadOnly="true" Visible="false"
                                        runat="server" Width="0px" Font-Size="12px"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="物料名称">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" Font-Size="14px"/>
                                <ItemTemplate>
                                    <asp:Label ID="txtITEMNAME" Text='<%# Eval("ITEMNAME") %>' ReadOnly="true" runat="server" Width="300px" Font-Size="12px" TextMode="MultiLine"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="要料数量">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" Font-Size="14px"/>
                                <ItemTemplate>
                                    <asp:TextBox ID="TxtCallQty" Text='<%# Eval("CALLQTY") %>' runat="server" 
                                        Width="40px" Font-Size="12px" onclick="Method2(this.id)"/>
                                </ItemTemplate>
                            </asp:TemplateField>                                                      
                            <asp:TemplateField HeaderText="退料数量">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" Font-Size="14px"/>
                                <ItemTemplate>
                                    <asp:TextBox ID="txtReturnqty" Text='<%# Eval("RETURNQTY") %>' runat="server" width="50px" 
                                    Font-Size="12px" onclick="Method2(this.id)"/>
                                </ItemTemplate>
                          </asp:TemplateField>   
                       <asp:TemplateField HeaderText="实际要料数量">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" Font-Size="14px"/>
                                <ItemTemplate>
                                    <asp:TextBox ID="txtUsedqty" Text='<%# Eval("USEDQTY") %>' runat="server" width="50px" ReadOnly="true" Font-Size="12px"/>
                                </ItemTemplate>
                            </asp:TemplateField>                 
                            <asp:TemplateField HeaderText="物流单位">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" Font-Size="14px"/>
                                <ItemTemplate>
                                    <asp:Label ID="txtunitname" Text='<%# Eval("UNITNAME") %>' runat="server" Width="25px" Font-Size="12px"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="领料统计">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" Font-Size="14px"/>
                                <ItemTemplate>
                                    <asp:Label ID="txtUsedCount" Text='<%# Eval("USEDCOUNT") %>' runat="server" Width="80px" Font-Size="12px" ReadOnly="true"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="统计单位">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" Font-Size="14px"/>
                                <ItemTemplate>
                                    <asp:Label ID="txtusedunitname" Text='<%# Eval("USEDUNITNAME") %>' runat="server" Width="25px" Font-Size="12px"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="物料规格" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" Font-Size="14px"/>
                                <ItemTemplate>
                                    <asp:Label ID="txtmemo" Text='' runat="server" Width="60px" CssClass="txt100" Font-Size="12px"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="供应商名称" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:Label ID="txtShipperName" Text='<%# Eval("SHIPPERNAME") %>' runat="server" Width="0px" Font-Size="12px"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="供应商ID" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                <ItemTemplate>
                                    <asp:TextBox ID="txtShipperid" Text='<%# Eval("Shipperid") %>' runat="server" Width="0px" Font-Size="12px" Visible="false"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="转换率" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" Font-Size="14px"/>
                                <ItemTemplate>
                                    <asp:Label ID="txtRate" Text='<%# Eval("Rate") %>' runat="server" Width="30px" Font-Size="12px"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="要料数量" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" Font-Size="14px"/>
                                <ItemTemplate>
                                    <asp:TextBox ID="Field1" Text='<%# Eval("CALLQTY") %>' runat="server" ReadOnly="true"
                                        Width="40px" Font-Size="12px" Visible="false"/>
                                </ItemTemplate>
                            </asp:TemplateField>                                                      
                            <asp:TemplateField HeaderText="退料数量" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" Font-Size="14px"/>
                                <ItemTemplate>
                                    <asp:TextBox ID="Field2" Text='<%# Eval("RETURNQTY") %>' runat="server" width="50px" ReadOnly="true" Font-Size="12px"/>
                                </ItemTemplate>
                          </asp:TemplateField>   
                       <asp:TemplateField HeaderText="消耗数量" Visible="false">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false"  CssClass="list_text"/>
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" Font-Size="14px"/>
                                <ItemTemplate>
                                    <asp:TextBox ID="Field3" Text='<%# Eval("USEDQTY") %>' runat="server" width="50px" ReadOnly="true" Font-Size="12px"/>
                                </ItemTemplate>
                            </asp:TemplateField>             
                        </Columns>
                        <FooterStyle BackColor="White" ForeColor="#000066" />
                           
                        <PagerStyle BackColor="#f3f8fa" ForeColor="#000066" HorizontalAlign="Center" Font-Bold="True" Height="30px"  />
                        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#c7e0ee" Font-Bold="True" ForeColor="#4" Height="34" />

                        <PagerTemplate>
                第<asp:Label ID="lblPageIndex" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex + 1 %>' />页
                共/<asp:Label ID="lblPageCount" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageCount %>' />页
                <asp:LinkButton ID="lbnFirst" runat="Server" Text="首页" Enabled='<%# ((GridView)Container.NamingContainer).PageIndex != 0 %>'
                    CommandName="Page" CommandArgument="First" ></asp:LinkButton>
                <asp:LinkButton ID="lbnPrev" runat="server" Text="上一页" Enabled='<%# ((GridView)Container.NamingContainer).PageIndex != 0 %>'
                    CommandName="Page" CommandArgument="Prev" ></asp:LinkButton>
                <asp:LinkButton ID="lbnNext" runat="Server" Text="下一页" Enabled='<%# ((GridView)Container.NamingContainer).PageIndex != (((GridView)Container.NamingContainer).PageCount - 1) %>'
                    CommandName="Page" CommandArgument="Next"  ></asp:LinkButton>
                <asp:LinkButton ID="lbnLast" runat="Server" Text="尾页" Enabled='<%# ((GridView)Container.NamingContainer).PageIndex != (((GridView)Container.NamingContainer).PageCount - 1) %>'
                    CommandName="Page" CommandArgument="Last"  ></asp:LinkButton>
            </PagerTemplate>  
                    </asp:GridView>
                    </td>
                    </tr>                   
                </table>
                    <div><asp:HiddenField ID="HiddenField1" runat="server" Visible="false"/></div>
            </div>
            <div class="table_bottombg">
                <ul>
                    <li>
                        <asp:Button ID="BtnUsed_Ensure" runat="server" Text="" Width="98px" Height="33px"
                            Style="background-image: url(/images/btn_enter.gif); border:0px; background-position: center;
                            background-repeat: no-repeat" OnClientClick="return firm1();" 
                            onclick="BtnUsed_Ensure_Click"/></li>
                </ul>
                <strong><span class="style1">先确认当前班次的物料领料数量(如需修改请联系系统管理员或带班长)，再进行交班操作。</span></strong></div>--%>
            <div class="tabletitle">
                <ul>
                    <li>
                        <img src="/images/table_pic01.gif" /></li>
                    <li>
                        <asp:Label ID="LabHeader" runat="server" Text="交班信息"></asp:Label></li>
                </ul>
            </div>
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:GridView ID="GridViewReturnBill_ToNext" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                                PageSize="7" DataKeyNames="CONTAINERDETAILID" OnPageIndexChanging="GridViewReturnBill_ToNext_PageIndexChanging"
                                BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="0px"
                                CellPadding="0" Font-Size="13pt" Width="100%" AllowPaging="True" ShowHeaderWhenEmpty="True"
                                EmptyDataText="没有交接班的数据">
                                <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="13px" />
                                <EmptyDataRowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="13px" />
                                <EmptyDataTemplate>
                                    <table class="sublist" width="100%">
                                        <tr>
                                            <%--
                                        <th>
                                            托盘号
                                        </th>--%>
                                            <th>站台位
                                            </th>
                                            <th>托盘类型
                                            </th>
                                            <th>物料名称
                                            </th>
                                            <th>数量
                                            </th>
                                            <th>单位
                                            </th>
                                            <th>生产厂家
                                            </th>
                                            <th>物料规格
                                            </th>
                                        </tr>
                                        <tr>
                                            <td colspan="7" style="line-height: 50px; text-align: center;">当前机台暂无数据可用于交班。
                                            </td>
                                        </tr>
                                    </table>
                                </EmptyDataTemplate>
                                <Columns>
                                    <asp:TemplateField HeaderText="ContainerDetailID" Visible="false" ItemStyle-Height="30px">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtContainerDetailID" Text='<%# Eval("containerDetailId") %>' Visible="false"
                                                ReadOnly="true" runat="server" />
                                        </ItemTemplate>
                                        <ItemStyle Height="25px"></ItemStyle>
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="ItemID" Visible="false">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtItemID" Text='<%# Eval("ITEMID") %>' Visible="false" ReadOnly="true"
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Itemsupplier" Visible="false">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtItemsupplier" Text='<%# Eval("SHIPPER_ID") %>' Visible="false"
                                            ReadOnly="true" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="MODELCODE" Visible="false">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtMODELCODE" Text='<%# Eval("MODELCODE") %>' Visible="false" ReadOnly="true"
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="RATE" Visible="false">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtRATE" Text='<%# Eval("RATE") %>' Visible="false" ReadOnly="true"
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="站台" ItemStyle-Height="30px">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <asp:Label ID="txtDOCKNUMBER" Text='<%# Eval("dockName") %>' ReadOnly="true" runat="server"
                                                Width="50px" Font-Size="13px" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="托盘类型">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <asp:Label ID="TxtModel" Text='<%# Eval("sectionCode") %>' ReadOnly="true" runat="server"
                                                Width="80px" Font-Size="13px" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="物料名称">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <%# Eval("itemName") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="标准计量">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <%# Eval("planQty") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="数量">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtQty" Text='<%# Eval("qty") %>' runat="server" Width="40px" onclick="showNumberInput(this)"
                                                Font-Size="13px" />
                                            <asp:HiddenField ID="hfItemID" runat="server" Value='<%# Eval("itemId") %>' />
                                            <asp:HiddenField ID="hfRate" runat="server" Value='<%# Eval("ratio") %>' />
                                            <asp:HiddenField ID="hfCurRowIndex" runat="server" Value='<%# Container.DataItemIndex%> ' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="单位">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <%# Eval("uomName") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="批次">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <%# Eval("lotNo")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="生产厂家">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <%# Eval("owner")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="物料规格" Visible="false">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <%# Eval("spec") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <FooterStyle BackColor="White" ForeColor="#000066" />
                                <PagerStyle BackColor="#f3f8fa" ForeColor="#000066" HorizontalAlign="Center" Font-Bold="True"
                                    Height="30px" />
                                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#c7e0ee" Font-Bold="True" ForeColor="#4" Height="34" />
                                <PagerTemplate>
                                    第<asp:Label ID="lblPageIndex" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex + 1 %>' />页
                                共/<asp:Label ID="lblPageCount" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageCount %>' />页
                                <asp:LinkButton ID="lbnFirst" runat="Server" Text="首页" Enabled='<%# ((GridView)Container.NamingContainer).PageIndex != 0 %>'
                                    CommandName="Page" CommandArgument="First"></asp:LinkButton>
                                    <asp:LinkButton ID="lbnPrev" runat="server" Text="上一页" Enabled='<%# ((GridView)Container.NamingContainer).PageIndex != 0 %>'
                                        CommandName="Page" CommandArgument="Prev"></asp:LinkButton>
                                    <asp:LinkButton ID="lbnNext" runat="Server" Text="下一页" Enabled='<%# ((GridView)Container.NamingContainer).PageIndex != (((GridView)Container.NamingContainer).PageCount - 1) %>'
                                        CommandName="Page" CommandArgument="Next"></asp:LinkButton>
                                    <asp:LinkButton ID="lbnLast" runat="Server" Text="尾页" Enabled='<%# ((GridView)Container.NamingContainer).PageIndex != (((GridView)Container.NamingContainer).PageCount - 1) %>'
                                        CommandName="Page" CommandArgument="Last"></asp:LinkButton>
                                </PagerTemplate>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
                <div>
                    <asp:HiddenField ID="hfAllowODD" runat="server" Visible="false" />
                    <asp:HiddenField ID="HfBillId" runat="server" />
                </div>
            </div>
            <div class="table_bottombg">
                <ul>
                    <li style="line-height: 35px;">
                        <asp:Label ID="LblChangeMsg" runat="server" Text="（温馨提示：已提交了交班数据，请确认是否需要修改）" Visible="false"></asp:Label>
                    </li>
                     <li>
                        <asp:CheckBox ID="ckbSelect" runat="server" OnCheckedChanged="ckbSelect_CheckedChanged"
                                                AutoPostBack="true" Checked="false" Enabled="true" Text="需要交接人工领料"
                                                Font-Bold="True" Font-Size="X-Large" Visible="false"/>
                    </li>
                    <li>
                        <asp:Button ID="btn_Ensure" runat="server" Text="" Width="98px" Height="33px" OnClick="btn_Ensure_Click"
                            Style="background-image: url(/images/btn_enter.gif); border: 0px; background-position: center; background-repeat: no-repeat"
                            OnClientClick="return firm();" /></li>
                    <li>
                        <asp:Button ID="btn_Modify" runat="server" Text="" Height="33px" Width="98px" OnClientClick="return CheckModify();"
                            OnClick="btn_Modify_Click" Style="background-image: url(/images/btn_xg.gif); background-position: center; border: 0px; background-repeat: no-repeat"
                            Visible="false" /></li>
                    <li>
                        <asp:Button ID="btn_Return" runat="server" Text="" Width="98px" Height="33px" OnClick="btn_Return_Click"
                            Style="background-image: url(/images/btn_back.gif); border: 0px; background-position: center; background-repeat: no-repeat" /></li>
                </ul>
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
    <div id="number_input" class="number_bg">
        <div class="number_text">
            <input type="text" id="number_text_in" class="number_text_in" />
        </div>
        <ul>
            <li>1</li>
            <li>2</li>
            <li>3</li>
            <li>←</li>
        </ul>
        <ul>
            <li>4</li>
            <li>5</li>
            <li>6</li>
            <li>0</li>
        </ul>
        <ul>
            <li>7</li>
            <li>8</li>
            <li>9</li>
            <li>.</li>
        </ul>
        <ul>
            <li></li>
            <li></li>
            <li>关闭</li>
            <li>确认</li>
        </ul>
    </div>
</body>
</html>
