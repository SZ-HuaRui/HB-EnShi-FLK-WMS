using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using NTI.WMS.CallTerminalSystem.BLL;
using WMSInterface;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class Materials_Select : System.Web.UI.Page
    {
        private static string dockName;
        private static string dockNumber;
        private static DataSet itemData;
        private static DataTable changed_DataTable;
        //private DataSet changed_DataSet = new DataSet();
        private static GridViewRow drv;
        private MaterialsBill_HandWork_BLL mhb = new MaterialsBill_HandWork_BLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        private static string MachineNumber, UserName, UserPassword, WareHouseID, Equipment, WorkShop, Run, RunsGroup, hasLGV;
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Session["Equipment"] == null || Session["UserName"] == null || Session["WarehouseID"] == null || Session["RunsGroup"] == null)
            //{
            //    Response.Redirect("pass.aspx");
            //}
            //else
            //{
                UserName = Session["UserName"].ToString();
           //   UserPassword = Session["PassWord"].ToString();
                WareHouseID = Session["WarehouseID"].ToString();
                Equipment = Session["Equipment"].ToString();
                WorkShop = Session["WorkShop"].ToString(); //车间 
                Run = Session["Runs"].ToString(); //班组 
                //MachineNumber = Session["MachineNumber"].ToString();
                RunsGroup = Session["RunsGroup"].ToString();
                hasLGV = Session["HasLGV"].ToString();
         //  }
            if (Session["MachineNumber"] == null)
            {
                string Url = "Control_Panel.aspx";
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('无机台信息，请先到机台设置中设置机台信息!');window.location='" + Url + "';</script>");
                return;
            }
            else
            {
                MachineNumber = Session["MachineNumber"].ToString();
            }
            if (!IsPostBack)
            {
                dockName = Request.QueryString["DockName"].ToString();
                dockNumber=Request.QueryString["DockNumber"].ToString();
                InitData();
                if (itemData == null)
                {
                    lv_Create_Bill.DataSource = null;
                    lv_Create_Bill.DataBind();
                    return;
                }
                //lv_Create_Bill.DataContext = itemData.Tables[0].DefaultView;
                BindingData();
                //lb_ProductName.Text = (LoginUser.ItemName == null) ? "牌  号：未设置" : "牌  号：" + LoginUser.ItemName;
                tb_ItemCode.Attributes.Add("onclick", "Method(this.id);");
                //tb_Get_Count.Attributes.Add("onclick", "showNumberInput(this)");
            }
        }

        #region 获取数据
        private void InitData()
        {
            //itemData = mhb.GetItemDataByZoneID();
            itemData = mhb.GetItemDataByZoneID(MachineNumber, LoginUser.RunsTypeCode);
            changed_DataTable = new DataTable();
            changed_DataTable.Clear();
            changed_DataTable = this.GetCreateBillDataSource(itemData).Tables[0];
        }
        private void BindingData()
        {
            if (changed_DataTable.Rows == null || changed_DataTable.Rows.Count <= 0)
            {
                lv_Create_Bill.DataSource = null;
                lv_Create_Bill.DataBind();
            }
            else
            {
                lv_Create_Bill.DataSource = changed_DataTable;
                lv_Create_Bill.DataBind();
            }
            foreach (DataRow dr in changed_DataTable.Rows)
            {
                if (dr["select"].ToString().ToLower().Equals("true"))
                {
                    string itemcode1 = dr["ITEMCODE"].ToString();
                    foreach (GridViewRow grv in lv_Create_Bill.Rows)
                    {
                        string itemCode = ((TextBox)grv.FindControl("CItemCode")).Text.ToString();
                        CheckBox cbk = (CheckBox)grv.FindControl("chkSelect");
                        if (itemcode1 == itemCode)
                        {
                            cbk.Checked = true;
                        }
                    }
                }
            }
        }
        //处理重复物料的问题  peq 2011/7/28
        private DataSet GetCreateBillDataSource(DataSet dsSource)
        {
            try
            {
                if (dsSource == null || dsSource.Tables.Count <= 0 || dsSource.Tables[0].Rows.Count <= 0)
                    return null;

                string itemid = string.Empty;
                DataSet dsGreateSource = new DataSet();
                DataTable dtGreateSource = dsSource.Tables[0].Clone();

                dsSource.Tables[0].DefaultView.Sort = "itemid desc";
                DataTable dtLoop = new DataTable();
                dtLoop = dsSource.Tables[0].Clone();
                dtLoop = dsSource.Tables[0].DefaultView.ToTable();

                foreach (DataRow dr in dtLoop.Rows)//dsSource.Tables[0].Rows
                {
                    dr["QTY"] = 0;
                    //如果相同继续循环
                    if (dr["itemid"].ToString().Equals(itemid))
                    {
                        continue;
                    }
                    else
                    {
                        //如果不同引入row
                        itemid = dr["itemid"].ToString();
                        //lotNumber = dr["LOTNUMBER"].ToString();
                        dtGreateSource.ImportRow(dr);
                    }
                }
                dtGreateSource.Columns.Add("select", typeof(string));
                dsGreateSource.Tables.Add(dtGreateSource);
                return dsGreateSource;
            }
            catch { return null; }
        }
        #endregion

        protected void Button_Click(object sender, EventArgs e)
        {
            if (tb_ItemCode.Text == string.Empty || tb_ItemCode.Text == "")
            {
                if (rb_ItemCode.Checked == true)
                {
                    DataSet_Oper(itemData, "ITEMCODE", false);
                }
                if (rb_ItemName.Checked == true)
                {
                    DataSet_Oper(itemData, "ITEMNAME", false);
                    itemData.Tables[0].DefaultView.Sort = "ITEMNAME";
                }
            }
            else
            {
                DataSet_Oper(itemData, tb_ItemCode.Text, true);
            }
        }
        private void DataSet_Oper(DataSet ds, string condition, bool isUseText)
        {
            if (ds == null)
            {
                return;
            }

            DataTable dt = new DataTable();
            DataView dv = new DataView();

            dv.Table = ds.Tables[0];
            if (isUseText)
            {
                dv.RowFilter = string.Format("ITEMCODE like '%" + condition + "%'");
            }
            else
            {
                dv.Sort = string.Format(condition + " DESC");
            }
            dt = dv.ToTable();

            if (dt == null || dt.Rows.Count <= 0)
            {
                return;
            }
            lv_Create_Bill.DataSource = dt.DefaultView;
            lv_Create_Bill.DataBind();
        }

        protected void chkSelect_select(object sender, EventArgs e)
        {
            CheckBox cbk = sender as CheckBox;
            drv = (GridViewRow)((CheckBox)sender).Parent.Parent;
            string itemcode = ((TextBox)drv.FindControl("CItemCode")).Text.ToString();
            if (cbk.Checked == true)
            {
                foreach (GridViewRow grv in lv_Create_Bill.Rows)
                {
                    string othercode = ((TextBox)grv.FindControl("CItemCode")).Text.ToString();
                    if (itemcode == othercode)
                    {
                        ((CheckBox)grv.FindControl("chkSelect")).Checked = true;
                        ChangeDataTable(itemcode, true);
                    }
                    else
                    {
                        ((CheckBox)grv.FindControl("chkSelect")).Checked = false;
                        ChangeDataTable(othercode, false);
                    }
                }
            }
            else if(cbk.Checked == false)
            {
                ChangeDataTable(itemcode, false);
            }
            //tblock_ItemName.Text = ((TextBox)drv.FindControl("CItemName")).Text.ToString();
            //tblock_ItemCode.Text = ((TextBox)drv.FindControl("CItemCode")).Text.ToString();
            //tb_Get_Count.Text = "0";
        }

        private void ChangeDataTable(string itemcode,bool bools)
        {
            foreach (DataRow datar in changed_DataTable.Rows)
            {
                if (itemcode == datar["ITEMCODE"].ToString())
                {
                    datar["select"] = bools.ToString();
                }
            }
        }

        protected void btn_Ensure_Click(object sender, EventArgs e)
        {
            CheckQty();
            int counts = 0;
            for (int i = 0; i < changed_DataTable.Rows.Count; i++)
            {
                if (changed_DataTable.Rows[i]["select"].ToString().ToLower().Equals("true"))
                {
                    counts++;
                }
            }
            if (counts == 0)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请选择新建要料单物料！');</script>");
                return;
            }
            foreach (DataRow dr in changed_DataTable.Rows)
            {
                if (dr["select"].ToString()=="True")
                {
                    string itemID = dr["ITEMID"].ToString();
                    string itemName = dr["ItemName"].ToString();
                    string itemCode = dr["ItemCode"].ToString();

                    decimal count = Convert.ToDecimal(dr["QTY"].ToString());
                    string lotNumber = string.Empty;
                    decimal rate = 1;
                    decimal countRated = count * rate;
                    if (countRated == 0)
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('领料量不能为0，请重新输入数量！');</script>");
                        return;
                    }
                    ContainerEntity ce = new ContainerEntity();
                    ce.FirstInTime = DateTime.Now;
                    ce.Modelcode = Properties.Settings.Default.ContainerTypeCode_Handle;
                    ce.CONTAINERID = string.Empty;
                    ce.WAREHOUSEID = LoginUser.WarehouseID;
                    ce.AddContainerDetail_ItemData(itemID, itemName, countRated, itemCode, lotNumber, 0, 0, 0, false, "", rate);      //count->countRated PC 2008/08/04

                    if (mhb.CreateHandWorkMaterialBill(MachineNumber, dockNumber, ce) > 0)
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('创建要料单操作成功!');</script>");
                        InitData();
                        BindingData();
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('创建要料单操作失败,请重试');</script>");
                    }
                }
            }
                #region
            //    foreach (GridViewRow grv in lv_Create_Bill.Rows)
            //    {
            //        CheckBox cb = (CheckBox)grv.FindControl("chkSelect");
            //        if (cb.Checked == true)
            //        {
            //            if (count > 0)
            //            {
            //                string itemID = ((TextBox)grv.FindControl("ITEMID")).Text.ToString();//drv.Row["ITEMID"].ToString();
            //                string itemName = ((TextBox)grv.FindControl("CItemName")).Text.ToString();
            //                string itemCode = ((TextBox)grv.FindControl("CItemCode")).Text.ToString();
            //                string lotNumber = string.Empty;
            //                decimal rate = 1;
            //                decimal countRated = count * rate;
            //                decimal qty_OnHand = decimal.Parse(((TextBox)grv.FindControl("CQty")).Text.ToString());

            //                ContainerEntity ce = new ContainerEntity();
            //                ce.FirstInTime = DateTime.Now;
            //                ce.Modelcode = Properties.Settings.Default.ContainerTypeCode_Handle;
            //                ce.CONTAINERID = string.Empty;
            //                ce.WAREHOUSEID = LoginUser.WarehouseID;
            //                ce.AddContainerDetail_ItemData(itemID, itemName, countRated, itemCode, lotNumber, 0, 0, 0, false, "");      //count->countRated PC 2008/08/04

            //                if (mhb.CreateHandWorkMaterialBill(MachineNumber, dockNumber, ce) > 0)
            //                {
            //                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('创建要料单操作成功!');</script>");
            //                    //Page_Load(null, null);
            //                }
            //                else
            //                {
            //                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('创建要料单操作失败,请重试');</script>");
            //                }
            //            }
            //            else
            //            {
            //                return;
            //            }
            //        }
            //        else
            //        {
            //            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请选择新建要料单物料！');</script>");
            //        }
            //    }
            #endregion
        }

        protected void lv_Create_Bill_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            lv_Create_Bill.PageIndex = e.NewPageIndex;
            CheckQty();
            
            BindingData();
        }

        private void CheckQty()
        {
            foreach (GridViewRow grv in lv_Create_Bill.Rows)
            {
                CheckBox cbk = (CheckBox)grv.FindControl("chkSelect");
                if (cbk.Checked == true)
                {
                    string itemCode = ((TextBox)grv.FindControl("CItemCode")).Text.ToString();
                    TextBox getCount = (TextBox)grv.FindControl("GetCount");
                    if (getCount.Text == null || getCount.Text == string.Empty)
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('领料数量不能为空，请输入数量!');</script>");
                        return;
                    }
                    string Txt = ((TextBox)grv.FindControl("GetCount")).Text.ToString().Trim();
                    string strtempa = ".";
                    int IndexofA = Txt.IndexOf(strtempa);
                    int tt = Txt.Length - (IndexofA + 1);
                    if (tt > 4)
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('只能输入4位小数!');</script>");
                        return;
                    }
                    decimal count = decimal.Parse(Txt);
                    if (count != 0)
                    {
                        ChangeDataQty(itemCode, count);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('领料量不能为0!');</script>");
                        return;
                    }
                }
            }
        }

        private void ChangeDataQty(string itemcode, decimal count)
        {
            foreach (DataRow dr in changed_DataTable.Rows)
            {
                if (dr["ITEMCODE"].ToString() == itemcode)
                {
                    dr["QTY"] = count.ToString();
                }
            }
        }

        protected void btn_Return_Click(object sender, EventArgs e)
        {
            Response.Redirect("Materials_Hand.aspx");
        }
    }
}