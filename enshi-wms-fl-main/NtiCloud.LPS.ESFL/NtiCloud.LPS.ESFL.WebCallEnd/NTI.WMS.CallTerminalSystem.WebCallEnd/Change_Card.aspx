<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Change_Card.aspx.cs" Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Change_Card" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>批量退料</title>
    <%--<link href="Styles/master 1.css" rel="Stylesheet" type="text/css"/>--%>
    <link href="Styles/master.css" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript" src="Scripts/RFJScript.js"></script>
    <script src="js/jquery.js" type="text/javascript"></script>
    <script src="js/jquery-ui.js" type="text/javascript"></script>
    <script src="js/common.js" type="text/javascript"></script>
    <link href="css/common.css" rel="stylesheet" type="text/css" />
    <%--<script type="text/javascript" src="Scripts/keyboard.js"  language="javascript"></script>--%>
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
            if (document.getElementById("TextBox1").value == "") {
                alert("请输入密码！");
                return false;
            }
            if (confirm("确定进行退料操作？")) {
                hideNumberInput();
                hideNumberInput_p();
                $("#submitWatting").show();
                return true;
            }
            else {
                return false;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="main">
            <div class="sitebg">
                当前所在位置：批量退料
            </div>
            <div class="table_middle">
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
                                <asp:Label ID="Label1" runat="server" Text="未设置"></asp:Label></li>
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
                                <asp:Label ID="Label2" runat="server" Text="未设置"></asp:Label></li>
                        </ul>
                        <ul style="display: none;">
                            <li>
                                <asp:Label ID="Label3" runat="server" Text="前一牌号:" Visible="false"></asp:Label>
                            </li>
                            <li>
                                <asp:Label ID="Label4" runat="server" Text="未设置" Visible="false"></asp:Label>
                            </li>
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
                        <tr>
                            <td>
                                <asp:GridView ID="gvChangeItem" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                                    PageSize="7" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                                    CellPadding="3" OnPageIndexChanging="gvChangeItem_PageIndexChanging" Font-Size="Medium"
                                    AllowPaging="True" Width="100%" ShowHeaderWhenEmpty="True">
                                    <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="Medium" />
                                    <EmptyDataRowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="12px" />
                                    <EmptyDataTemplate>
                                        <table class="sublist" width="100%">
                                            <tr>
                                                <td>站台号
                                                </td>
                                                <td>托盘号
                                                </td>
                                                <td>托盘类型
                                                </td>
                                                <td>物料名称
                                                </td>
                                                <td>数量
                                                </td>
                                                <td>单位
                                                </td>
                                                <td>批次号
                                                </td>
                                                <td>生产厂家
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="8" style="line-height: 50px; text-align: center;">当前机台暂无数据可用于退料。
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
                                        <asp:TemplateField HeaderText="DOCKNUMBER" Visible="false">
                                            <ItemTemplate>
                                                <asp:TextBox ID="DOCKNUMBER" Text='<%# Eval("dockCode") %>' Visible="false" ReadOnly="true"
                                                    runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Rate" Visible="false">
                                            <ItemTemplate>
                                                <asp:TextBox ID="RATE" Text='<%# Eval("itemName") %>' Visible="false" ReadOnly="true"
                                                    runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ItemID" Visible="false">
                                            <ItemTemplate>
                                                <asp:TextBox ID="ITEMID" Text='<%# Eval("item_id") %>' Visible="false" ReadOnly="true"
                                                    runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--<asp:TemplateField HeaderText="全选" ItemStyle-Height="40px" Visible="false">
                                        <HeaderTemplate>
                                            <input id="ckbAll" type="checkbox" title="全选" onclick="CheckAll('gvChangeItem',this)" />
                                        </HeaderTemplate>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <asp:CheckBox ID="ckbSelect" runat="server" OnCheckedChanged="ckbSelect_CheckedChanged"
                                                Visible="true" AutoPostBack="true" Enabled="true" Text="勾选" Style="zoom: 50%;"
                                                Font-Bold="True" Font-Size="X-Large" />
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                        <asp:TemplateField HeaderText="站台号" HeaderStyle-Width="80px">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblDocNumber" Font-Size="12px" runat="server" Text='<%# Eval("dockCode") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="托盘号" HeaderStyle-Width="100px">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblCurPalletNumber" Font-Size="12px" runat="server" Text='<%# Eval("sectionCode") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="托盘类型">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblModelCode" runat="server" Font-Size="12px" Text='<%# Eval("sectionDes") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--<asp:TemplateField HeaderText="生产牌号">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblproductname" Font-Size="12px" runat="server" Text='<%# Eval("Field41") %>'
                                                ReadOnly="true"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                        <asp:TemplateField HeaderText="物料">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="TxtItemName" Font-Size="12px" runat="server" Text='<%# Eval("itemName") %>'
                                                    ReadOnly="true"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="标准计量">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <%--<asp:HiddenField ID="hidBomQty" runat="server" Value='<%# Eval("BOMQTY") %>' />--%>
                                                <asp:Label ID="txtPlanQty" Font-Size="12px" runat="server" Text='<%# Eval("planQty") %>'
                                                    Width="30px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="退料量">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <%--<asp:HiddenField ID="hidBomQty" runat="server" Value='<%# Eval("BOMQTY") %>' />--%>
                                                <asp:TextBox ID="txtQty" Text='<%# Eval("qty") %>' runat="server" Width="40px" onclick="showNumberInput(this)"
                                                    Font-Size="13px" />
                                                <%-- <%# Eval("uomName") %>--%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="单位">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblUnitName" Font-Size="12px" runat="server" Text='<%# Eval("uomName") %>'
                                                    Width="30px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="批次">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblLotNumber" Font-Size="12px" runat="server" Text='<%# Eval("lot_no") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="供应商">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" CssClass="list_text" />
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblShipper" Font-Size="12px" runat="server" Text='<%# Eval("ownerName") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <FooterStyle ForeColor="#000066" />
                                    <PagerStyle BackColor="#f3f8fa" ForeColor="#000066" HorizontalAlign="Center" Font-Bold="True"
                                        Height="30px" />
                                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                                    <HeaderStyle BackColor="#c7e0ee" Font-Bold="True" ForeColor="#4" Height="40" />
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
                                        <%-- <asp:TextBox ID="txtNewPageIndex" runat="server" Width="20px" Text='<%# ((GridView)Container.Parent.Parent).PageIndex + 1 %>' />--%>
                                        <%--<asp:LinkButton ID="btnGo" runat="server" CausesValidation="False" CommandArgument="-1"
                    CommandName="Page" Text="GO" />--%><!-- here set the CommandArgument of the Go Button to '-1' as the flag -->
                                    </PagerTemplate>
                                </asp:GridView>
                            </td>
                        </tr>
                        <div>
                            <asp:HiddenField ID="hfAllowODD" runat="server" />
                        </div>
                    </table>
                </div>
                <div class="list_options" style="padding-top: 10px; height: 30px;">
                    <div class="left">
                        <%--操作类别--%>
                    </div>
                    <ul>
                        <li>
                            <asp:Label ID="labtarget_area" runat="server" Font-Bold="True" Text="目的区域:" Visible="false"></asp:Label></li>
                        <li>
                            <asp:DropDownList ID="target_Area" runat="server" Height="22px" Width="140px" OnSelectedIndexChanged="target_Area_SelectedIndexChanged"
                                Visible="false" Style="font-size: medium">
                            </asp:DropDownList>
                        </li>
                        <li>
                            <asp:CheckBox ID="IsCkbCanBeCallOut" runat="server" Checked="true" Text="是否允许被再次呼出" Visible="true" />
                        </li>
                    </ul>
                </div>
                <div class="table_bottombg">
                    <ul>
                        <li style="line-height: 35px; font-weight: bold;">
                            <asp:Label ID="LblMsg" runat="server" Font-Bold="True" Text="请联系高架库工作人员输入退料密码: "></asp:Label>
                        </li>
                        <li>
                            <asp:TextBox ID="TextBox1" runat="server" onclick="showNumberInput_p(this)" Height="26px"
                                Width="188px" CssClass="style2" Visible="true" TextMode="Password"></asp:TextBox></li>
                        <li>
                            <asp:Button ID="btn_Ensure" runat="server" Text="" Width="98px" Height="33px" Style="background-image: url(/images/btn_enter.gif); background-position: center; border: 0px; background-repeat: no-repeat"
                                OnClick="btn_Ensure_Click"
                                OnClientClick="return firm();" /></li>
                        <li>
                            <asp:Button ID="btn_Return" runat="server" Text="" Width="98px" Height="33px" Style="background-image: url(/images/btn_back.gif); background-position: center; border: 0px; background-repeat: no-repeat"
                                OnClick="btn_Return_Click" /></li>
                    </ul>
                </div>
            </div>
        </div>
        <%-- <div class="table_bottombg">
        	<ul>
            <li><asp:Button ID="btn_Ensure" runat="server" Text="" Width="98px" Height="33px" Style="background-image: url(/images/btn_enter.gif); background-position:center;background-repeat: no-repeat"
            onclick="btn_Ensure_Click" /></li>	
            <li><asp:Button ID="btn_Return" runat="server" Text="" Width="98px" Height="33px" Style="background-image: url(/images/btn_back.gif); background-position:center;background-repeat: no-repeat"
            onclick="btn_Return_Click" /></li>
            </ul>
        </div>--%>
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
