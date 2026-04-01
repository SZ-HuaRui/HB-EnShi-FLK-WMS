<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Change_Class_Accept.aspx.cs"
    Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Change_Class_Accept" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>接班</title>
    <link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <%--<script type="text/javascript" src="Scripts/keyboard.js"></script>--%>
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
    <script type="text/javascript" language="javascript">
        function Method2(id) {
            var reValue = showModalDialog("NumKey.aspx", window, "dialogWidth=880px;dialogHeight=200px;dialogTop:600px;help:no;resizable:no;status:no;scroll:yes;center:yes;scroll:yes;"); //保存窗体的返回值  
            var array = reValue; //保存返回值  
            var uname = document.getElementById(id);
            if (reValue != null) {
                uname.value = array;
            }
        }

        function checkForm() {
            if (confirm('请确认数据无误，是否执行接班操作？')) {
                $("#submitWatting").show();
                return true;
            }
            return false;
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
        <div class="main">
            <div class="sitebg">
                当前所在位置：接班操作
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
                        <li>班次班组:</li>
                        <li>
                            <asp:Label ID="labbc" runat="server" Text="未设置"></asp:Label></li>
                    </ul>
                    <ul>
                        <li>生产牌号:</li>
                        <li>
                            <asp:Label ID="label2" runat="server" Text="未设置" Width="165px"></asp:Label></li>
                    </ul>
                    <ul>
                        <li>交班时间:</li>
                        <li>
                            <asp:Label ID="label3" runat="server" Text="无"></asp:Label></li>
                        <li>
                            <asp:Label ID="lblRuns" runat="server" Text="" Style="position: absolute;"></asp:Label>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="table_middle">
                <div class="tabletitle">
                    <ul>
                        <li>
                            <img src="/images/table_pic01.gif" /></li>
                        <li>接班明细</li>
                    </ul>
                </div>
                <div>
                    <table width="100%" border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <asp:GridView ID="lv_Change_Class" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                                    PageSize="7" DataKeyNames="CONTAINERDETAILID" OnPageIndexChanging="GridViewReturnBill_ToNext_PageIndexChanging"
                                    BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                                    CellPadding="3" Font-Size="12px" Width="100%" AllowPaging="True">
                                    <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="12px" />
                                    <EmptyDataRowStyle BackColor="#eff5f8" ForeColor="#000066" Font-Size="12px" BorderColor="#CCCCCC"
                                        BorderWidth="1px" />
                                    <EmptyDataTemplate>
                                        <table class="sublist" width="100%">
                                            <tr>
                                                <th>站台位
                                                </th>
                                                <th>托盘号
                                                </th>
                                                <th>物料
                                                </th>
                                                <th>数量
                                                </th>
                                                <th>单位
                                                </th>
                                                <th>供应商
                                                </th>
                                                <th>批次
                                                </th>
                                                <th>物料规格
                                                </th>
                                            </tr>
                                            <tr>
                                                <td colspan="7" style="line-height: 50px; text-align: center;">当前机台暂无接班数据。
                                                </td>
                                            </tr>
                                        </table>
                                    </EmptyDataTemplate>
                                    <Columns>
                                        <asp:TemplateField HeaderText="ContainerDetailID" Visible="false">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtContainerDetailID" Text='<%# Eval("containerDetailId") %>' Visible="false"
                                                    ReadOnly="true" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ItemID" Visible="false">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtItemID" Text='<%# Eval("itemId") %>' Visible="false" ReadOnly="true"
                                                    runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--<asp:TemplateField HeaderText="RATE" Visible="false">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtRATE" Text='<%# Eval("ITEM_RATE") %>' Visible="false" ReadOnly="true"
                                                runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                        <asp:TemplateField HeaderText="站台位" ItemStyle-Height="33px">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="txtDOCKNUMBER" Text='<%# Eval("dockName") %>' ReadOnly="true" runat="server"
                                                    CssClass="style1" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="托盘号">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="TxtPallet" Text='<%# Eval("sectionCode") %>' ReadOnly="true" runat="server"
                                                    CssClass="style1" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="物料">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <%--<asp:Label ID="txtITEMNAME" Text='' ReadOnly="true" runat="server"
                                                CssClass="style1" />--%>
                                                <%# Eval("itemName")%>
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
                                                <asp:TextBox ID="txtQty" Text='<%# Eval("qty") %>' ForeColor="Red" runat="server"
                                                    CssClass="style1" Enabled="false" Width="50px" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="单位">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <%--<asp:Label ID="txtCname" Text='' runat="server" CssClass="style1" />--%>
                                                <%# Eval("uomName") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="供应商">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <%--<asp:Label ID="txtShippername" Text='' runat="server" />--%>
                                                <%# Eval("owner") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="批次">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <%--<asp:Label ID="txtLOTNUMBER" Text='' runat="server" />--%>
                                                <%# Eval("lotNo") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="物料规格">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <%--<asp:Label ID="TextBox1" Text='' runat="server" CssClass="txt100" />--%>
                                                <%# Eval("spec") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <FooterStyle BackColor="White" ForeColor="#000066" Font-Size="Medium" />
                                    <PagerStyle BackColor="#f3f8fa" ForeColor="#000066" HorizontalAlign="Center" Font-Bold="True"
                                        Height="40px" />
                                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" Font-Size="Medium" />
                                    <HeaderStyle BackColor="#c7e0ee" Font-Bold="True" ForeColor="#4" Height="34" />
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
                                    </PagerTemplate>
                                    <%--<SortedAscendingCellStyle BackColor="#F1F1F1" />
                                <SortedAscendingHeaderStyle BackColor="#007DBB" />
                                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                <SortedDescendingHeaderStyle BackColor="#00547E" />--%>
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="table_bottombg">
                    <ul>
                        <li>
                            <asp:Button ID="btn_Ensure_Next" runat="server" Text="" Width="98px" Height="33px"
                                Style="background-image: url(/images/btn_qrreceive.gif); background-position: center; border: 0px; background-repeat: no-repeat"
                                OnClick="btn_Ensure_Next_Click" OnClientClick="return ();" /></li>
                        <li>
                            <asp:Button ID="btn_Modify" runat="server" Text="" Height="33px" Width="98px" OnClick="btn_Modify_Click"
                                Style="background-image: url(/images/btn_xg.gif); background-position: center; border: 0px; background-repeat: no-repeat" /></li>
                        <li>
                            <asp:Button ID="btn_Return" runat="server" Text="" Height="33px" Width="98px" OnClick="btn_Return_Click"
                                Style="background-image: url(/images/btn_back.gif); background-position: center; border: 0px; background-repeat: no-repeat" /></li>
                    </ul>
                </div>
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
