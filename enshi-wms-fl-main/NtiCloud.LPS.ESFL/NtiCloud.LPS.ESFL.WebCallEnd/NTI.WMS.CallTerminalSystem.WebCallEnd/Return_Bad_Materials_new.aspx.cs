using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Data;
using System.Collections;
using WMSInterface;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class Return_Bad_Materials_new : System.Web.UI.Page
    {
        private static string target_Area_Des;
        private static string targe_Area_Number;
        private ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        private static DataTable containerDetail;
        //private static DataSet target_Dock;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AddHeader("P3P", "CP=CAO PSAOUR");
            if (Session["MachineNumber"] == null)
            {
                Response.Redirect("Call_Terminal_Log.aspx");
                return;
            }

            Response.Expires = -1;

            if (Session["MachineNumber"] == null)
            {
                string Url = "Control_Panel.aspx";
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('无机台信息，请先到机台设置中设置机台信息!');window.location='" + Url + "';</script>");
                return;
            }

            if (!Page.IsPostBack)
            {
                InitDataSet();
                BindingListview();
                InitTextBlock();
                BindingCombobox();
            }
        }

        #region 获取页面数据
        /// <summary>
        /// 获取页面数据
        /// </summary>
        private void InitDataSet()
        {
            containerDetail = new DataTable();
            containerDetail.Clear();
            containerDetail = rpb.BadMaterialGetContainerDetailByDockNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.DockNumber, LoginUser.Token);
            if (containerDetail != null)
            {
                DataTable dtTaskItemInfo = rpb.GetTaskByDock(LoginUser.Url,"0", LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.DockNumber, LoginUser.Token);
                for (int i = 0; i < dtTaskItemInfo.Rows.Count; i++)
                {
                    for (int k = 0; k < containerDetail.Rows.Count; k++)
                    {
                        if (dtTaskItemInfo.Rows[i]["container_d_id"].ToString() == containerDetail.Rows[k]["containerDetailId"].ToString())
                        {
                            containerDetail.Rows[k]["planQty"] = dtTaskItemInfo.Rows[i]["plan_qty"].ToString();
                        }
                    }
                }
                //containerDetail.Tables[0].Columns.Add("ENABLEDODD");

                //foreach (DataRow row in containerDetail.Tables[0].Rows)
                //{
                //    row["FIELD4"] = row["QTY"];
                //    row["ENABLEDODD"] = Properties.Settings.Default.EnabledODD;
                //    row["QTY"] = 0;
                //}

                //containerDetail.Columns.Add("ENABLEDODD");
                //foreach (DataRow row in containerDetail.Rows)
                //{
                //    row["ENABLEDODD"] = Properties.Settings.Default.EnabledODD;
                //}
            }
            //target_Dock = new DataSet();
            //target_Dock.Clear();
            //target_Dock = rpb.GetReturnPalletDestination();
        }
        #endregion

        #region 绑定界面数据
        /// <summary>
        /// 绑定界面数据
        /// </summary>
        private void BindingListview()
        {
            if (containerDetail == null || containerDetail.Rows.Count == 0)
            {
                lv_Return_Bad.DataSource = null;
                lv_Return_Bad.DataBind();
                return;
            }
            //设置默认的退料的数量为0  
            DataTable dsContainerDetails = this.GetReturnItem(0, containerDetail, "");//containerDetail
            lv_Return_Bad.DataSource = dsContainerDetails.DefaultView;
            lv_Return_Bad.DataBind();
        }
        #endregion

        #region 绑定站台信息
        /// <summary>
        /// 绑定站台信息
        /// </summary>
        public void InitTextBlock()
        {
            bool flag = false;
            if (containerDetail != null && containerDetail.Rows.Count > 0)
            {
                flag = true;
            }
            txtblk_Sock_Code.Text = flag ? containerDetail.Rows[0]["containerBarcode"].ToString() : "未设置";//托盘条码
            txtblk_Platform_Num.Text = LoginUser.DockNumber;//站台编号
            txtblk_Brand.Text = LoginUser.ItemName;//牌号
            txtblk_Section.Text = flag ? containerDetail.Rows[0]["sectionCode"].ToString() : "未设置";//区段
        }
        #endregion

        #region 设置目的区域
        /// <summary>
        /// 设置目的区域
        /// </summary>
        private void BindingCombobox()
        {
            //ListItem listItem = new ListItem("高架库", "3033");
            //target_Area.Items.Add(listItem);
            try
            {
                DataTable targetAreaDataSource = rpb.GetTargetAreaOnBadMaterial(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.DockNumber, LoginUser.Token);
                target_Area.DataSource = targetAreaDataSource.DefaultView;
                target_Area.DataTextField = "EQUIPMENT_CODE";
                target_Area.DataValueField = "t_id";
                target_Area.DataBind();
            }
            catch(Exception ex)
            {
                target_Area.DataSource = null;
                target_Area.DataBind();
                ClientScript.RegisterStartupScript(Page.GetType(), "", $"<script>alert('没能获取到目的区域,原因：{ex.Message}!');</script>");
            }
        } 
        #endregion

        protected void target_Area_SelectedIndexChanged(object sender, EventArgs e)
        {
            target_Area_Des = target_Area.SelectedItem.Text.ToString();
            targe_Area_Number = target_Area.SelectedItem.Value.ToString();
        }

        protected void lv_Return_Bad_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            lv_Return_Bad.PageIndex = e.NewPageIndex;
            CheckQty();
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Finish_Click(object sender, EventArgs e)
        {
            if (TextBox1.Text == "")
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请输入密码！');</script>");
                return;
            }
            //string pwd = "";//CommonBLL.ReadStrategy(LoginUser.WarehouseID, "WMS.WebCallEnd.txtReturnBadPwd");
            //if (string.IsNullOrEmpty(pwd))
            //{
            //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('读取系统配置密码失败！');</script>");
            //    return;
            //}
            //if (!TextBox1.Text.Equals(pwd))
            //{
            //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('密码错误，请重新输入！');</script>");
            //    return;
            //}
            if (!TextBox1.Text.ToString().Trim().Equals(Properties.Settings.Default.AdminPassword))
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('密码错误，请重新输入！');</script>");
                return;
            }
            CheckQty();
            //退坏料信息
            JsonObject remnantMaterialInfo = new JsonObject();
            DataRow rowBom = containerDetail.Rows[0];
            string dockId = rowBom["dockId"].ToString();
            //退坏料物料信息
            JsonArray details = new JsonArray();
            bool isAllZero = true;
            foreach (DataRow row in containerDetail.Rows)
            {
                decimal qty = decimal.Parse(row["qty"].ToString());

                if (qty >= 0)
                {
                    isAllZero = false;
                    decimal planQty = Convert.ToDecimal(row["planQty"]);

                    if (qty > planQty)
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('“" + row["itemName"] + "”的退料数量不能超出标准数量 " + planQty + " , 请重新输入!');</script>");
                        return;
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('“" + row["itemName"] + "”的退料数量不能为负数, 请重新输入!');</script>");
                    return;
                }

                JsonObject obj = new JsonObject();
                obj.Add("containerDetailId", row["containerDetailId"].ToString());//容器明细id
                obj.Add("itemId", row["itemId"].ToString());//物料id
                obj.Add("itemName", row["itemName"].ToString());//物料名称
                obj.Add("planQty", row["planQty"].ToString());//要料时物料的数量
                obj.Add("qty", qty);//退坏料的数量
                obj.Add("lotNo", row["lotNo"].ToString());//批次
                obj.Add("ownerId", row["ownerId"].ToString());//供应商
                details.Add(obj);
            }


            if (isAllZero == true)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('所有物料的退坏料的数量全为0，请退空托盘！');</script>");
                return;
            }

            remnantMaterialInfo.Add("ReturnItems", details);
            //remnantMaterialInfo.Add("FullPallet", fullPallet);

            string msg = rpb.ClientReturnBadPallet(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, dockId, LoginUser.SchemeId, LoginUser.RunsCodeId, LoginUser.RunsGroupId, remnantMaterialInfo.ToString(), LoginUser.HasLGV.ToString(), LoginUser.Token);
            if (string.IsNullOrEmpty(msg))
            {
                Session["rb_ReturnBadMaterials"] = true;
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退坏料成功。');window.location='Call_Terminal.aspx';</script>");
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退坏料失败！原因：" + msg + "');</script>");
            }
        }

        public void TextBox_Preview(TextBox textbox, DataTable dt, string ContainerDetailID)
        {
            decimal text_initCount;
            text_initCount = decimal.Parse(textbox.Text);

            if (textbox.Text == "")
            {
                textbox.Text = text_initCount.ToString();
            }
            //Check_input(textbox, text_initCount);
            //取消退坏料数量的限制
            CompareItemQtyToBom(textbox, dt, ContainerDetailID);
        }

        private void CompareItemQtyToBom(TextBox tb, DataTable dt, string ContainerDetailID)
        {
            if (dt != null)
            {
                //string s = tb.Tag.ToString();
                foreach (DataRow row in dt.Rows)
                {
                    if (row["containerDetailId"].ToString() == ContainerDetailID)
                    {
                        string value = row[tb.ID].ToString();

                        if (tb.Text.Length > 10)
                        {
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('您输入的数值位数太长,请重新输入!');</script>");
                            tb.Text = value;
                            return;
                        }

                        row[tb.ID] = tb.Text;     //写入修改数据到DataSet

                        //if (row["sectionCode"].ToString().Equals(Properties.Settings.Default.ContainerTypeCode_Handle))
                        //{
                        //    return;
                        //}
                        decimal qty = decimal.Parse(row["qty"].ToString());
                        if (!rpb.CompareItemQtyToBom(row["itemId"].ToString(), qty, decimal.Parse(row["ODD1"].ToString()), decimal.Parse(row["ODD2"].ToString()), decimal.Parse(row["ODD3"].ToString()), row["BOMID"].ToString()))
                        {
                            //MessageBoxTimeOutStatic.show(NTIMessage.SYSTEM_HINT, NTIMessage.RETURN_COUNT_OVER_ERROR, Hint.ERROR);
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料总量不能超出BOM的数量,请重新输入!');</script>");
                        }
                    }
                }
            }
        }

        protected void lv_Return_Bad_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //if (e.CommandName == "reason")
            //{
            //    string s = e.CommandArgument.ToString();
            //    if (containerDetail != null)
            //    {
            //        foreach (DataRow row in containerDetail.Rows)
            //        {
            //            if (row["CONTAINERDETAILID"].ToString() == s)
            //            {
            //                //string ss = this.@return.Value.ToString();
            //                if (Session["returnValue"] != null)
            //                {
            //                    row["FIELD3"] = Session["returnValue"].ToString();
            //                    Session["returnValue"] = null;
            //                }
            //                //else
            //                //{

            //                //}
            //            }
            //        }
            //    }
            //}
            //BindingListview();
        }

        #region 取消
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Call_Terminal.aspx");
        } 
        #endregion

        #region 检查退坏料数量是否符合格式要求
        /// <summary>
        /// 检查退坏料数量是否符合格式要求
        /// </summary>
        /// <returns>false：不符合；true：符合</returns>
        private bool CheckQty()
        {
            bool bFind = false;
            foreach (GridViewRow row in lv_Return_Bad.Rows)
            {
                if (bFind)
                {
                    break;
                }
                TextBox txtContainerDetailID = (TextBox)row.FindControl("txtContainerDetailID");
                string ContainerDetailID = txtContainerDetailID.Text.Trim();
                TextBox textbox = (TextBox)row.FindControl("QTY");
                if (!string.IsNullOrEmpty(textbox.Text))
                {
                    if (CommonFuctions.CheckDecemal(textbox.Text.Trim()))
                    {
                        this.UpdateContainerDetailsQty(textbox, ContainerDetailID);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('数据格式不正确,请重新输入!');</script>");
                        bFind = true;
                        break;
                    }
                }
            }
            if (bFind)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 重新绑定退坏料界面数据源
        /// <summary>
        /// 重新绑定退坏料界面数据源
        /// </summary>
        /// <param name="tb">输入坏料数量的控件</param>
        /// <param name="containerDetailId">容器明细id</param>
        private void UpdateContainerDetailsQty(TextBox tb, string containerDetailId)
        {
            if (string.IsNullOrEmpty(containerDetailId)) return;
            //否则直接修改containerdetail的数据
            //获取textbox中的数值
            string count = tb.Text;
            if (lv_Return_Bad.Rows == null) return;
            containerDetail = this.GetReturnItem(decimal.Parse(count), containerDetail, containerDetailId);//containerDetail
            this.lv_Return_Bad.DataSource = containerDetail;//containerDetail
            this.lv_Return_Bad.DataBind();
        }
        #endregion

        #region 根据容器明细id，修改退坏料界面数据源中退坏料中物料的数量
        /// <summary>
        /// 根据容器明细id，修改退坏料界面数据源中退坏料中物料的数量
        /// </summary>
        /// <param name="qty">退坏料数量</param>
        /// <param name="dtContainerDetails">退坏料界面数据源</param>
        /// <param name="containerDetailId">容器明细id</param>
        /// <returns></returns>
        public DataTable GetReturnItem(decimal qty, DataTable dtContainerDetails, string containerDetailId)
        {
            DataTable dtItem = new DataTable();
            try
            {
                if (dtContainerDetails.Rows.Count <= 0) return null;
                dtItem = dtContainerDetails.Clone();
                DataRow drItem = null;

                //如果containerdetailid为空则修改所有的containerdetail的数据
                if (string.IsNullOrEmpty(containerDetailId))
                {
                    foreach (DataRow dr in dtContainerDetails.Rows)
                    {
                        dr.BeginEdit();
                        dr["qty"] = qty;
                        dr.EndEdit();
                        drItem = dtItem.NewRow();
                        drItem = dr;
                        dtItem.Rows.Add(drItem.ItemArray);
                    }
                }
                else
                {
                    foreach (DataRow dr in dtContainerDetails.Rows)
                    {
                        if (dr["containerDetailId"] != null && dr["containerDetailId"].ToString().Equals(containerDetailId))
                        {
                            dr.BeginEdit();
                            dr["qty"] = qty;
                            dr.EndEdit();

                        }
                        drItem = dtItem.NewRow();
                        drItem = dr;
                        dtItem.Rows.Add(drItem.ItemArray);
                    }
                }

            }
            catch { return null; }
            return dtItem;
        }
        #endregion
    }
}