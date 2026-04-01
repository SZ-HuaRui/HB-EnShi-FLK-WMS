<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Return_Bad_Materials_new.aspx.cs"
    Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Return_Bad_Materials_new" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>退坏料</title>
    <base target="_self" />
    <script src="js/jquery.js" type="text/javascript"></script>
    <script src="js/jquery-ui.js" type="text/javascript"></script>
    <link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/keyboard.js"></script>
    <link href="Scripts/RFJScript.js" type="text/javascript" />
    <script src="js/common.js" type="text/javascript"></script>
    <link href="css/common.css" rel="stylesheet" type="text/css" />
    <script language="javascript" language="javascript" type="text/javascript">
        function fullscreen() {
            var iWidth = window.screen.availWidth;
            var iHeight = window.screen.availHeight;
            window.moveTo(0, 0);
            window.resizeTo(iWidth, iHeight);
        }
        function firm() {
            var tatol = 0;
            $("[type='text']").each(function () {
                if ($(this).attr("class") == "style2") {
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
            if ($("#TextBox1").val() == "") {
                alert("请输入密码！");
                return false;
            }

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
        .style2 {
            font-size: 12px;
        }
    </style>
</head>
<body onload="fullscreen()">
    <form id="form1" runat="server">
        <div class="main tablebg">
            <div class="table_middle">
                <div class="sitebg">
                    当前所在位置：退坏料
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
                        <li>
                            <img src="/images/table_pic01.gif" /></li>
                        <li>退料信息</li>
                    </ul>
                </div>
                <div>
                    <table width="100%" border="0" cellpadding="0" cellspacing="0">
                        <tr class="list_title">
                            <td>
                                <asp:GridView ID="lv_Return_Bad" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                                    PageSize="7" DataKeyNames="CONTAINERDETAILID" OnPageIndexChanging="lv_Return_Bad_PageIndexChanging"
                                    BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                                    CellPadding="3" Font-Size="12px" Width="100%" AllowPaging="True" OnRowCommand="lv_Return_Bad_RowCommand">
                                    <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="12px" />
                                    <EmptyDataRowStyle BackColor="#eff5f8" ForeColor="#000066" Font-Size="12px" BorderColor="#CCCCCC" BorderWidth="1px" />
                                    <EmptyDataTemplate>
                                        <table class="sublist" width="100%">
                                            <tr>
                                                <th>物料
                                                </th>
                                                <th>批次号
                                                </th>
                                                <th>标准计量
                                                </th>
                                                <th>退料量
                                                </th>
                                                <th>单位
                                                </th>
                                                <th>原因
                                                </th>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
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
                                        <asp:TemplateField HeaderText="物料" ItemStyle-Height="40px">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="item" Text='<%# Eval("itemName") %>' ReadOnly="true" runat="server"
                                                    CssClass="style2" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="标准计量">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="BOMQTY" Text='<%# Eval("planQty") %>' runat="server" CssClass="style2" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="退坏料量">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:TextBox ID="QTY" Text='<%# Eval("qty") %>' runat="server" CssClass="style2"
                                                    Width="50px" AutoPostBack="true" onclick="showNumberInput(this)" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="单位">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="check" Text='<%# Eval("uomQty") %>' runat="server" Width="80px" CssClass="style2" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="批次">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="LOTNUMBER" Text='<%# Eval("lotNo") %>' runat="server" CssClass="style2" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="供应商">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="SHIPPERNAME" Text='<%# Eval("owner") %>' runat="server" CssClass="style2" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <FooterStyle BackColor="White" ForeColor="#000066" />
                                    <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Center" Font-Bold="True" />
                                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
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
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
                   <%-- <div class="list_page">
                    </div>--%>
                </div>
                <div class="list_options" style="padding-top: 10px; height: 30px;">
                    <div class="left">
                    </div>
                    <ul>
                        <li>
                            <asp:Label runat="server" Text="目的区域" Visible="false"></asp:Label>
                        </li>
                        <li>
                            <asp:DropDownList ID="target_Area" runat="server" AutoPostBack="True" OnSelectedIndexChanged="target_Area_SelectedIndexChanged"
                                Width="92px" Visible="false">
                            </asp:DropDownList>
                        </li>
                        <li>&nbsp;&nbsp;</li>
                        <%--<li>退坏料原因：</li>
                        <li>
                            <asp:TextBox ID="txtDesc" runat="server" Width="555"></asp:TextBox>
                        </li>--%>
                    </ul>
                </div>
                <div class="table_bottombg">
                    <ul>
                        <li>
                            <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Size="14pt" Text="请输入密码:"
                                Visible="true" Height="27px"></asp:Label>
                        </li>
                        <li>
                            <asp:TextBox ID="TextBox1" runat="server" Height="24px" Width="188px" onclick="showNumberInput_p(this)"
                                CssClass="style2" Visible="true" TextMode="Password"></asp:TextBox>
                        </li>
                        <li>
                            <asp:Button ID="btn_Finish" runat="server" Text="" Width="98px" Height="33px" Style="background-image: url(/images/btn_complete.gif); background-position: center; background-repeat: no-repeat"
                                OnClick="btn_Finish_Click"
                                OnClientClick="return firm();" />
                        </li>
                        <li>
                            <asp:Button ID="btn_Cancel" runat="server" Text="" Width="98px" Height="33px" Style="background-image: url(/images/btn_cancel.gif); background-position: center; background-repeat: no-repeat"
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
    <div id="number_input_p" class="number_bg_p">
        <div class="number_text">
            <input type="password" id="number_text_in_p" class="number_text_in" />
        </div>
        <ul>
            <li>l</li>
            <li>n</li>
            <li>m</li>
            <li>←</li>
        </ul>
        <ul>
            <li>s</li>
            <li>3</li>
            <li>6</li>
            <li>h</li>
        </ul>
        <ul>
            <li>2</li>
            <li>t</li>
            <li>9</li>
            <li>r</li>
        </ul>
        <ul>
            <li>i</li>
            <li>5</li>
            <li>关闭</li>
            <li>确认</li>
        </ul>
    </div>
</body>
</html>
