using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Data;
using WMSInterface;
using System.Text;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    /// <summary>
    /// 退余料
    /// </summary>
    public partial class Return_Remnant_Material : System.Web.UI.Page
    {
        private string target_Area_Des;//static
        private string targe_Area_Number;//static
        private ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        private DataTable containerDetail;//static
        private DataSet return_Destination;//static
        private DataSet lastReturn;//static
        int modifyBomQty = 0;

        private string target_Area_Des1
        {
            set { ViewState["target_Area_Des1"] = value; }
            get { return ViewState["target_Area_Des1"] as string; }
        }

        private string targe_Area_Number1
        {
            set { ViewState["targe_Area_Number1"] = value; }
            get { return ViewState["targe_Area_Number1"] as string; }
        }

        private DataTable containerDetail1
        {
            set { ViewState["containerDetail1"] = value; }
            get { return ViewState["containerDetail1"] as DataTable; }
        }

        private DataSet return_Destination1
        {
            set { ViewState["return_Destination1"] = value; }
            get { return ViewState["return_Destination1"] as DataSet; }
        }

        private DataSet lastReturn1
        {
            set { ViewState["lastReturn1"] = value; }
            get { return ViewState["lastReturn1"] as DataSet; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SessionKey"] == null)
            {
                Response.Redirect("Call_Terminal_Log.aspx");
                return;
            }

            Response.AddHeader("P3P", "CP=CAO PSAOUR");

            if (!IsPostBack)
            {
                InitData();
                InitTextBlock();
                BindingListview();
                BindingCombobox();
            }

            //重新刷新生产牌号  add by pengenqi 2012/8/6
            //this.SetTowProductName();

            LblHasLGV.Visible = LoginUser.HasLGV == 0;
        }

        #region 加载界面数据
        /// <summary>
        /// 加载界面数据
        /// </summary>
        private void InitData()
        {
            containerDetail = rpb.GetContainerDetailByDockNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.DockNumber, LoginUser.Token);
            if (containerDetail != null)
            {
                //containerDetail.Columns.Add("ENABLEDODD");
                //foreach (DataRow row in containerDetail.Rows)
                //{
                //    row["ENABLEDODD"] = Properties.Settings.Default.EnabledODD;
                //}
                //DataTable dtTaskItemInfo = rpb.GetTheNewItemNumByPlanOutId(LoginUser.Url,"0", LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.SchemeId, LoginUser.Token);
                DataTable dtTaskItemInfo = rpb.GetTaskByDock(LoginUser.Url, "0", LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.DockNumber, LoginUser.Token);
                if (dtTaskItemInfo==null)
                {
                    containerDetail1 = null;
                    return;
                }
                for (int i = 0; i < dtTaskItemInfo.Rows.Count; i++)
                {
                    for (int k = 0; k < containerDetail.Rows.Count; k++)
                    {
                        if (dtTaskItemInfo.Rows[i]["container_d_id"].ToString()== containerDetail.Rows[k]["containerDetailId"].ToString())
                        {
                            containerDetail.Rows[k]["planQty"] = dtTaskItemInfo.Rows[i]["plan_qty"].ToString();
                        }
                    }
                }
            }

            //添加containerDetail中
            containerDetail1 = containerDetail;

            //return_Destination = rpb.GetReturnPalletDestination();

            //return_Destination添加到viewstate中
            //return_Destination1 = return_Destination;
        }
        #endregion

        #region 显示机台信息
        /// <summary>
        /// 显示机台信息
        /// </summary>
        private void InitTextBlock()
        {
            bool flag = false;
            if (containerDetail1 != null && containerDetail1.Rows.Count > 0)//containerDetail
            {
                flag = true;
            }

            LblSockCode.Text = flag ? containerDetail1.Rows[0]["containerBarcode"].ToString() : "未设置";//托盘条码
            LblPlatformNum.Text = LoginUser.DockNumber;//站台编号
            LblBrand.Text = LoginUser.ItemName;//生产牌号
            LblSection.Text = flag ? containerDetail1.Rows[0]["sectionCode"].ToString() : "未设置";//区段
            LblPlatformName.Text = flag ? containerDetail1.Rows[0]["dockName"].ToString() : "未设置";//站台名称
        }
        #endregion

        #region 设置默认退余料的数量为0
        /// <summary>
        /// 设置默认退余料的数量为0
        /// </summary>
        private void BindingListview()
        {
            if (containerDetail1 == null || containerDetail1.Rows.Count == 0)//containerDetail
            {
                lv_ReturnMaterial.DataSource = null;
                lv_ReturnMaterial.DataBind();
                return;
            }
            //设置默认的退料的数量为0  
            DataTable dsContainerDetails = this.GetReturnItem(0, containerDetail1, "");//containerDetail

            lv_ReturnMaterial.DataSource = dsContainerDetails;
            lv_ReturnMaterial.DataBind();
        } 
        #endregion

        #region 根据容器明细id，修改退余料界面数据源中退余料中物料的数量
        /// <summary>
        /// 根据容器明细id，修改退余料界面数据源中退余料中物料的数量
        /// </summary>
        /// <param name="qty">退余料数量</param>
        /// <param name="dtContainerDetails">退余料界面数据源</param>
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
                        dr["QTY"] = qty;
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
                            dr["QTY"] = qty;
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

        #region 设置退料目的区域
        /// <summary>
        /// 设置退料目的区域
        /// </summary>
        private void BindingCombobox()
        {
            try
            {
                //绑定退料的目的地  所有的数据默认退往暂存区   
                //配置退料目的地  
                //ListItem myListItem1 = new ListItem("暂存区", "暂存区");
                //target_Area.Items.Add(myListItem1);
                ListItem myListItem = new ListItem("平衡库", Properties.Settings.Default.ReturnPallet_TargetAddress);
                //return_Destination1.Tables[0].DefaultView.Sort = "EQUIPMENTNUMBER desc";//return_Destination
                //DataTable dtDestination = return_Destination1.Tables[0].DefaultView.ToTable();//return_Destination
                //target_Area.DataSource = return_Destination1.Tables[0].DefaultView;//return_Destination
                //target_Area.DataTextField = "description";
                //target_Area.DataValueField = "equipmentnumber";                                
                //target_Area.DataBind();
                target_Area.Items.Add(myListItem);
                target_Area.SelectedIndex = 0;

                //设置退料目的地不可选  
                if (!string.IsNullOrEmpty(Properties.Settings.Default.UnSetReturnItemPlace) && (Properties.Settings.Default.UnSetReturnItemPlace.Equals("true")))
                {
                    this.target_Area.Enabled = false;
                }
            }
            catch
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('没能获取到目的区域!');</script>");
            }
        }
        #endregion

        #region 翻页事件
        /// <summary>
        /// 翻页事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridViewReturnBill_ToNext_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            lv_ReturnMaterial.PageIndex = e.NewPageIndex;
            CheckQty();
        } 
        #endregion

        #region 目的区域下拉框选择事件
        /// <summary>
        /// 目的区域下拉框选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void target_Area_SelectedIndexChanged(object sender, EventArgs e)
        {
            target_Area_Des = target_Area.SelectedItem.Text.ToString();
            targe_Area_Number = target_Area.SelectedValue.ToString();

            //添加target_Area_Des到viewstate中
            target_Area_Des1 = target_Area_Des;
            targe_Area_Number1 = targe_Area_Number;

            foreach (DataRow row in containerDetail1.Rows)//containerDetail
            {
                row["FIELD2"] = target_Area_Des;        //row[19] -> row["FIELD2"] 
            }
            foreach (GridViewRow row in lv_ReturnMaterial.Rows)
            {
                Label textbox = (Label)row.FindControl("targetAddress");
                textbox.Text = target_Area_Des;
            }
        }
        #endregion

        #region 退余料(退回仓库)单选按钮选择事件
        protected void RadioButton_Click(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            string operName = rb.Text.ToString();
            ChangeReturn(operName);
        }

        private void ChangeReturn(string operName)
        {
            switch (operName)
            {
                case "真退(退回仓库)":
                    target_Area.Enabled = true;
                    foreach (DataRow row in containerDetail1.Rows)//containerDetail
                    {
                        row["FIELD2"] = target_Area.SelectedItem.Text.ToString();  //row[19] -> row["FIELD2"]  
                    }
                    break;
                case "假退(退下一班)":
                    target_Area.Enabled = false;
                    foreach (DataRow row in containerDetail1.Rows)//containerDetail
                    {
                        row["FIELD2"] = row["DOCKNUMBER"];          //row[19] -> row["FIELD2"]  
                        if (targe_Area_Number1 != row["FIELD2"].ToString())      //row[19] -> row["FIELD2"]    targe_Area_Number
                        {
                            targe_Area_Number1 = row["FIELD2"].ToString();       //row[19] -> row["FIELD2"]   targe_Area_Number
                        }
                    }
                    break;
            }
        } 
        #endregion

        #region 检查退余料数量是否符合格式要求
        /// <summary>
        /// 检查退余料数量是否符合格式要求
        /// </summary>
        /// <returns>false：不符合；true：符合</returns>
        private bool CheckQty()
        {
            bool bFind = false;
            foreach (GridViewRow row in lv_ReturnMaterial.Rows)
            {
                if (bFind)
                {
                    break;
                }
                TextBox txtContainerDetailID = (TextBox)row.FindControl("txtContainerDetailID");
                string ContainerDetailID = txtContainerDetailID.Text.Trim();
                TextBox textbox = (TextBox)row.FindControl("txtQTY");
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

        #region 完成
        /// <summary>
        /// 完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Finish_Click(object sender, EventArgs e)
        {
            //TxtPwd.Text = "ntinti56";
            if (!CkbCanCallOut.Checked && TxtPwd.Text.ToString() == "")
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请输入密码！');</script>");
                return;
            }
            if (!CkbCanCallOut.Checked && !TxtPwd.Text.ToString().Trim().Equals(Properties.Settings.Default.AdminPassword))
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('密码错误，请重新输入！');</script>");
                return;
            }

            if (lv_ReturnMaterial.Rows != null)
            {
                if (!CheckQty())
                {
                    return;
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('没有物料，无需退余料！');</script>");
                return;
            }
            //targe_Area_Number = target_Area.SelectedItem.Value.ToString();
            DataTable change_DataSet = new DataTable();
            ContainerEntity ce = new ContainerEntity();
            change_DataSet = containerDetail1;//containerDetail
            string fullPallet = "1";
            //string canUse = CkbCanCallOut.Checked ? "1" : "0";
            //退余料信息
            JsonObject remnantMaterialInfo = new JsonObject();
            DataRow rowBom = change_DataSet.Rows[0];
            string dockId = rowBom["dockId"].ToString();
            //退余料物料信息
            JsonArray details = new JsonArray();
            bool isAllZero = true;
            foreach (DataRow row in change_DataSet.Rows)
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
                obj.Add("qty", qty);//退余料的数量
                obj.Add("lotNo", row["lotNo"].ToString());//批次
                obj.Add("ownerId", row["ownerId"].ToString());//供应商
                details.Add(obj);
            }


            if (isAllZero == true)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('所有物料的退余料的数量全为0，请退空托盘！');</script>");
                return;
            }

            remnantMaterialInfo.Add("ReturnItems", details);
            //remnantMaterialInfo.Add("FullPallet", fullPallet);
            string errcode = string.Empty;
            string msg = rpb.ClientReturnPallet(LoginUser.Url, out errcode,CkbCanCallOut.Checked, IsCkbCanBeCallOut.Checked, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, dockId, LoginUser.SchemeId, LoginUser.RunsCodeId, LoginUser.RunsGroupId, remnantMaterialInfo.ToString(), LoginUser.HasLGV.ToString(), LoginUser.Token);
            //if (string.IsNullOrEmpty(msg))
            //{
            //    Session["rb_ReturnRemnantsMaterials"] = true;
            //    //ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退余料成功。');window.location='Call_Terminal.aspx';</script>");
            //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('操作成功。');window.location='Call_Terminal.aspx';</script>");
            //}
            //else
            //{
            //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退余料失败！原因：" + msg + "');</script>");
            //}
            if (string.Equals(errcode, "0"))
            {
                //ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('操作成功。');window.location='Call_Terminal.aspx';</script>");
                if (string.Equals(msg,"OK"))
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('操作成功。');window.location='Call_Terminal.aspx';</script>");
                }
                else
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退余料成功，但要料失败，原因：" + msg + "，请进入呼叫终端进行要料');window.location='Call_Terminal.aspx';</script>");
                }
               
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退余料失败！原因：" + msg + "');</script>");
            }
            //else //if (string.Equals(errcode, "2"))
            //{
            //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退余料成功，但要料失败，原因：" + msg + "，请进入呼叫终端进行要料');window.location='Call_Terminal.aspx';</script>");
            //}
            //else
            //{
            //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退余料出现异常！');</script>");
            //}
        } 
        #endregion

        #region 重新绑定退余料界面数据源
        /// <summary>
        /// 重新绑定退余料界面数据源
        /// </summary>
        /// <param name="tb">输入余料数量的控件</param>
        /// <param name="containerDetailId">容器明细id</param>
        private void UpdateContainerDetailsQty(TextBox tb, string containerDetailId)
        {
            if (string.IsNullOrEmpty(containerDetailId)) return;
            //否则直接修改containerdetail的数据
            //获取textbox中的数值
            string count = tb.Text;
            if (lv_ReturnMaterial.Rows == null) return;
            containerDetail1 = this.GetReturnItem(decimal.Parse(count), containerDetail1, containerDetailId);//containerDetail
            this.lv_ReturnMaterial.DataSource = containerDetail1;//containerDetail
            this.lv_ReturnMaterial.DataBind();
        }
        #endregion

        #region 取消
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            target_Area_Des = string.Empty;
            targe_Area_Number = string.Empty;
            containerDetail = null;
            return_Destination = null;
            lastReturn = null;

            Response.Redirect("Call_Terminal.aspx");
        } 
        #endregion

        private void SetTowProductName()
        {
            int twoProducts = 0;
            string ddProductName = string.Empty;
            string onlyOne = string.Empty;


            DataSet twoProductNames = rpb.GetTwoProductName(LoginUser.WarehouseID, LoginUser.MachineNumber);
            //如果同一个机台存在两个生产牌号的情况下  退料的时候进行比较显示前一个生产牌号
            if (twoProductNames != null && twoProductNames.Tables.Count > 0 && twoProductNames.Tables[0].Rows.Count > 1)
            {
                for (int z = 0; z < twoProductNames.Tables[0].Rows.Count; z++)
                {
                    if (twoProductNames.Tables[0].Rows[z]["name"] != null && twoProductNames.Tables[0].Rows[z]["name"].ToString().Equals(ddProductName))
                    {
                        continue;
                    }
                    else
                    {
                        ddProductName = twoProductNames.Tables[0].Rows[z]["name"].ToString();
                        twoProducts++;
                    }
                }

                if (twoProducts > 1)
                {
                    string beforeProductName = string.Empty;
                    string productdockname = string.Empty;
                    for (int x = 0; x < twoProductNames.Tables[0].Rows.Count; x++)
                    {
                        beforeProductName = twoProductNames.Tables[0].Rows[x]["name"].ToString();
                        productdockname = twoProductNames.Tables[0].Rows[x]["dockname"].ToString();
                        //containerDetail1.Tables[0].Rows[0]["DOCKNAME"].ToString()
                        if (!string.IsNullOrEmpty(beforeProductName))
                        {
                            if (containerDetail1 != null && containerDetail1.Rows.Count > 0)
                            {
                                if (containerDetail1.Rows[0]["DOCKNAME"].ToString().Equals(productdockname))
                                {
                                    LblBrand.Text = beforeProductName;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (twoProductNames != null && twoProductNames.Tables.Count > 0 && twoProductNames.Tables[0].Rows.Count > 0)
                {
                    onlyOne = twoProductNames.Tables[0].Rows[0]["name"].ToString();
                    if (!string.IsNullOrEmpty(onlyOne))
                    {
                        if (!onlyOne.Equals(LoginUser.ItemName))
                        {
                            this.LblBrand.Text = onlyOne;
                        }
                    }
                }
            }
        }

        #region 再次要料
        /// <summary>
        /// 再次要料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CkbCanCallOut_CheckedChanged(object sender, EventArgs e)
        {
            if (CkbCanCallOut.Checked)
            {
                //TxtPwd.Visible = false;
                //LblPwdMsg.Visible = false;

                //DataTable dt = containerDetail1;
                //dt.DefaultView.RowFilter = "bomqty > 0";
                //lv_ReturnMaterial.DataSource = dt.DefaultView;
                //lv_ReturnMaterial.DataBind();
            }
            else
            {
                //TxtPwd.Visible = true;
                //LblPwdMsg.Visible = true;

                //DataTable dt = containerDetail1;
                //lv_ReturnMaterial.DataSource = dt;
                //lv_ReturnMaterial.DataBind();
            }
        } 
        #endregion
    }
}