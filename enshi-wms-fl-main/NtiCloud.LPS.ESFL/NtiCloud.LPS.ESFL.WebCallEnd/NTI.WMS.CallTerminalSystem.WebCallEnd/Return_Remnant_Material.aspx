<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Return_Remnant_Material.aspx.cs"
    Inherits="NTI.WMS.CallTerminalSystem.WebCallEnd.Return_Remnant_Material" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>退余料</title>
    <link href="Styles/master.css" rel="Stylesheet" type="text/css" />
    <script src="js/jquery.js" type="text/javascript"></script>
    <script src="js/jquery-ui.js" type="text/javascript"></script>
    <script src="js/common.js" type="text/javascript"></script>
    <link href="css/common.css" rel="stylesheet" type="text/css" />
    <%--<script type="text/javascript" src="Scripts/keyboard.js" language="javascript"></script>--%>
    <script language="javascript" type="text/javascript">
        //    window.onload = function adjustWindowSizeAndPos() {
        //        var WshShell = new ActiveXObject("WScript.Shell");
        //        WshShell.SendKeys('{F11}');
        //    }

        //    window.onload = function adjustWindowSizeAndPos() {
        //        // 设置页面的初始大小，if条件判断是否是模式窗口    
        //        if (typeof window.dialogArguments != 'undefined') {
        //            window.dialogHeight = 0 + "px";
        //            window.dialogWidth = 0 + "px";
        //        } else {
        //            window.resizeTo(0, 0);
        //        }

        //        // 定义窗口宽度、窗口高度、屏幕宽度、屏幕高度    
        //        var winWidth = document.body.scrollWidth * 1.2;
        //        var winHeight = document.body.scrollHeight * 1.4;
        //        var availWidth = screen.availWidth;
        //        var availHeight = screen.availHeight;

        //        // 设置最小宽度、高度，最大宽度、高度 （根据屏幕宽度高度的比例计算）    
        //        var minWidth = availWidth * 0.5;
        //        var minHeight = availHeight * 0.5;
        //        var maxWidth = availWidth;
        //        var maxHeight = availHeight * 0.9;

        //        // 设置窗口的宽度高度在最小及最大值之间    
        //        winWidth = winWidth < minWidth ? minWidth : winWidth;
        //        winHeight = winHeight < minHeight ? minHeight : winHeight;
        //        winWidth = winWidth > maxWidth ? maxWidth : winWidth;
        //        winHeight = winHeight > maxHeight ? maxHeight : winHeight;

        //        // 设置窗口的宽度和高度，设置窗口的位置（if中为模式窗口设置，else中为活动窗口设置）    
        //        if (typeof window.dialogArguments != 'undefined') {
        //            window.dialogTop = ((availHeight - winHeight) / 2) + "px";
        //            window.dialogLeft = ((availWidth - winWidth) / 2) + "px";
        //            window.dialogHeight = winHeight + "px";
        //            window.dialogWidth = winWidth + "px";
        //        } else {
        //            window.moveTo((availWidth - winWidth) / 2, (availHeight - winHeight) / 2);
        //            window.resizeTo(winWidth, winHeight);
        //        }
        //    }  
    </script>
    <script language="javascript" type="text/javascript">

        function Method2(id) {
            var reValue = showModalDialog("NumKey.aspx", window, "dialogWidth=880px;dialogHeight=200px;dialogTop:600px;help:no;resizable:no;status:no;scroll:yes;center:yes;scroll:yes;"); //保存窗体的返回值  
            var array = reValue; //保存返回值  
            var uname = document.getElementById(id);
            if (reValue != null) {
                uname.value = array;
            }
        }
        function Method(id) {
            var reValue = showModalDialog("KeyBoard.aspx", window, "dialogWidth=880px;dialogHeight=340px;dialogTop:600px;help:no;resizable:no;status:no;scroll:yes;center:yes;scroll:yes;location:no;"); //保存窗体的返回值  
            var array = reValue; //保存返回值  
            var uname = document.getElementById(id);
            if (reValue != null) {
                uname.value = array;
            }
        }

        function CheckSubmit() {
            var tatol = 0;
            $("[type='text']").each(function () {
                if ($(this).attr("id") != "number_text_in") {
                    tatol += $(this).val() - 0;
                }
            });
            if (tatol < 0) {
                alert("退料数量不能小于0！");
                return false;
            }
            if (tatol == 0) {
                alert("退料数量为0，请进行退空托盘！");
                return false;
            }
            //!$("#CkbCanCallOut").attr("checked") && 
            if ($("#TxtPwd").val() == "") {
                alert("请输入密码！");
                return false;
            }

            hideNumberInput();
            hideNumberInput_p();
            //$("#submitWatting").show();
            return true;
        }
    </script>
    <style type="text/css">
        .style2 {
            font-size: 12px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="main">
            <div class="table_middle">
                <div class="sitebg">
                    当前所在位置：退余料
                </div>
                <div class="tabletitle">
                    <ul>
                        <li>
                            <img src="images/table_pic01.gif" /></li>
                        <li>机台信息</li>
                    </ul>
                </div>
                <div class="table_module1bg">
                    <div class="table_module1">
                        <ul>
                            <li>
                                <asp:Label runat="server" Text="托盘条码:"></asp:Label>
                            </li>
                            <li>
                                <%--<asp:TextBox ID="txtblk_Sock_Code" runat="server"></asp:TextBox>--%>
                                <asp:Label ID="LblSockCode" runat="server" Text="未设置"></asp:Label>
                            </li>
                        </ul>
                        <ul>
                            <li>
                                <asp:Label runat="server" Text="站台编号:"></asp:Label>
                            </li>
                            <li>
                                <%--<asp:TextBox ID="txtblk_Platform_Num" runat="server"></asp:TextBox>--%>
                                <asp:Label ID="LblPlatformNum" runat="server" Text="未设置"></asp:Label>
                            </li>
                        </ul>
                        <ul>
                            <li>
                                <asp:Label runat="server" Text="区段:"></asp:Label>
                            </li>
                            <li>
                                <%--<asp:TextBox ID="txtblk_Section" runat="server"></asp:TextBox>--%>
                                <asp:Label ID="LblSection" runat="server" Text="未设置"></asp:Label>
                            </li>
                        </ul>
                        <ul>
                            <li>
                                <asp:Label runat="server" Text="生产牌号:"></asp:Label>
                            </li>
                            <li>
                                <%--<asp:TextBox ID="txtblk_Brand" runat="server"></asp:TextBox>--%>
                                <asp:Label ID="LblBrand" runat="server" Text="未设置"></asp:Label>
                            </li>
                        </ul>
                        <ul>
                            <li>
                                <asp:Label runat="server" Text="站台名称:"></asp:Label>
                            </li>
                            <li>
                                <%--<asp:TextBox ID="txtblk_Platform_Name" runat="server"></asp:TextBox>--%>
                                <asp:Label ID="LblPlatformName" runat="server" Text="未设置"></asp:Label>
                            </li>
                        </ul>
                    </div>
                </div>
                <div class="tabletitle">
                    <ul>
                        <li>
                            <img src="images/table_pic01.gif" /></li>
                        <li>退料信息</li>
                    </ul>
                </div>
                <div>
                    <table width="100%" border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <asp:GridView ID="lv_ReturnMaterial" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                                PageSize="7" OnPageIndexChanging="GridViewReturnBill_ToNext_PageIndexChanging"
                                BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                                CellPadding="3" Font-Size="12px" Width="100%" AllowPaging="True">
                                <RowStyle ForeColor="#000066" BackColor="#eff5f8" Wrap="false" Font-Size="12px" />
                                <EmptyDataRowStyle BackColor="#eff5f8" ForeColor="#000066" Font-Size="12px" BorderColor="#CCCCCC"
                                    BorderWidth="1px" />
                                <EmptyDataTemplate>
                                    <table class="sublist" width="100%">
                                        <tr>
                                            <th>物料
                                            </th>
                                            <th>标准计量
                                            </th>
                                            <th>退料量
                                            </th>
                                            <th>单位
                                            </th>
                                            <th>供应商
                                            </th>
                                            <th>批次
                                            </th>
                                            <th>规格
                                            </th>
                                        </tr>
                                        <tr>
                                            <td></td>
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
                                            <%--<asp:Label ID="item" Text='' ReadOnly="true" runat="server"
                                            CssClass="style2" />--%>
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
                                    <asp:TemplateField HeaderText="退料量">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtQTY" Text='<%# Eval("qty") %>' runat="server" AutoPostBack="true"
                                                Width="40px" onclick="showNumberInput(this)" CssClass="style2" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="单位">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <%# Eval("uomQty") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="目的地">
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                    <ItemTemplate>
                                        <asp:Label ID="targetAddress" Text='<%# Eval("FIELD2") %>' ReadOnly="true" runat="server"
                                            Width="60px" CssClass="style2" />
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="供应商">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <%# Eval("owner") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="批次">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                        <ItemTemplate>
                                            <%# Eval("lotNo") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="规格">
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="list_text" />
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="false" CssClass="border" />
                                    <ItemTemplate>
                                        <%# Eval("ITEMCATALOGNAME")%>
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                                </Columns>
                                <FooterStyle BackColor="White" ForeColor="#000066" />
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
                            </asp:GridView>
                        </tr>
                    </table>
                </div>
                <div class="list_options" style="padding-top: 10px; height: 30px;">
                    <div class="left">
                        <%--操作类别--%>
                    </div>
                    <ul>
                        <li>
                            <asp:Label runat="server" Text="目的区域" Visible="true"></asp:Label>
                        </li>
                        <li>
                            <asp:DropDownList ID="target_Area" runat="server" AutoPostBack="True" OnSelectedIndexChanged="target_Area_SelectedIndexChanged"
                                Width="92px" Visible="true">
                            </asp:DropDownList>
                        </li>
                        <li></li>
                        <li>
                            <asp:RadioButton ID="rb_True_Return" runat="server" Text="退余料(退回仓库)" AutoPostBack="True"
                                GroupName="return" OnCheckedChanged="RadioButton_Click" Checked="true" Visible="false"
                                Enabled="true" />
                        </li>
                        <li>
                            <asp:RadioButton ID="rb_F_Return" runat="server" Text="假退(退下一班)" AutoPostBack="True"
                                GroupName="return" OnCheckedChanged="RadioButton_Click" Visible="false" />
                        </li>
                        <li>
                             <asp:CheckBox ID="CkbCanCallOut" runat="server" Text="再次要料" Visible="false" OnCheckedChanged="CkbCanCallOut_CheckedChanged" />

                             <asp:CheckBox ID="IsCkbCanBeCallOut" runat="server" Checked="true" Text="是否允许被再次呼出" Visible="true"/>
                        </li>
                    </ul>
                </div>
                <div style="text-align: center; line-height: 30px; color: Red; font-weight: bold; background-color: Yellow;">
                    <asp:Label ID="LblHasLGV" runat="server" Text="当前机台设置为不产生 AGV 任务！"></asp:Label>
                </div>
                <div class="table_bottombg">
                    <ul>
                        <li>
                            <asp:Label ID="LblPwdMsg" runat="server" Text="请输入密码:" Style="line-height: 35px; font-weight: bold;"></asp:Label>
                        </li>
                        <li>
                            <asp:TextBox ID="TxtPwd" runat="server" Height="26px" Width="188px" onclick="showNumberInput_p(this)"
                                CssClass="style2" TextMode="Password"></asp:TextBox>
                        </li>
                        <%--AutoPostBack="true"  --%>
                        <li>
                            <asp:Button ID="btn_Finish" runat="server" Text="" Style="background-image: url(/images/btn_complete.gif); background-position: center; border: 0px; background-repeat: no-repeat"
                                Height="33px"
                                Width="98px" OnClientClick="return CheckSubmit();" OnClick="btn_Finish_Click" /></li>
                        <%----%>
                        <li>
                            <asp:Button ID="btn_Cancel" runat="server" Text="" Style="background-image: url(/images/btn_cancel.gif); background-position: center; border: 0px; background-repeat: no-repeat"
                                Height="33px"
                                Width="98px" OnClick="btn_Cancel_Click" /></li>
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
