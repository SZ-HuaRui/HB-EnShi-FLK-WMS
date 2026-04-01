using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using NTI.WMS.CallTerminalSystem.BLL;
using WMSInterface.Entity50;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class ManPowerReturnMaterials : System.Web.UI.Page
    {
        string UserName, UserPassword, WareHouseID, ItemName, ProductItemId, returnBomID;
        string Equipment, WorkShop, Run, MachineNumber, RunsGroup, MachineName, hasLGV, EquipmentNumber, DockNumber, Schemeid;
        private string target_Area_Des;
        private string targe_Area_Number;
        private DataSet containerDetail, containerDetail1;
        private DataSet return_Destination;
        private DataTable lastDetail;
        int modifyBomQty = 0;
        TaskEntity taskentity = new TaskEntity();
        WMSInterface.EntityDefination.Container_View containerinfo = new WMSInterface.EntityDefination.Container_View();
        List<WMSInterface.Entity50.ContainerDetailN> details = new List<WMSInterface.Entity50.ContainerDetailN>();

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AddHeader("P3P", "CP=CAO PSAOUR");
            if (Session["productitemname"] == null || Session["WorkShop"] == null || Session["Equipment"] == null || Session["UserName"] == null || Session["WarehouseID"] == null || Session["RunsGroup"] == null || Session["MachineNumber"] == null)
            {
                Response.Redirect("pass.aspx");
            }
            else
            {
                EquipmentNumber = LoginUser.EquipmentNumber;
                UserName = Session["UserName"].ToString();
                WareHouseID = Session["WarehouseID"].ToString();
                Equipment = Session["Equipment"].ToString();
                WorkShop = Session["WorkShop"].ToString(); //车间 
                Run = Session["Runs"].ToString(); //班组 
                RunsGroup = Session["RunsGroup"].ToString();
                hasLGV = Session["HasLGV"].ToString();
                DockNumber = Session["DockNumber"].ToString();
                ItemName = Session["productitemname"].ToString();
                Response.Expires = -1;
                MachineName = LoginUser.MachineName;
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
                if (Session["MachineName"] != null) MachineName = Session["MachineName"].ToString();
                switch (Run)
                {
                    case "1":
                        Run = "后夜班";
                        break;
                    case "2":
                        Run = "上午班";
                        break;
                    case "3":
                        Run = "下午班";
                        break;
                    case "4":
                        Run = "前夜班";
                        break;
                }
                LoginUser.RunsCode = Run;
                switch (RunsGroup)
                {
                    case "1":
                        RunsGroup = "甲班";
                        break;
                    case "2":
                        RunsGroup = "乙班";
                        break;
                    case "3":
                        RunsGroup = "丙班";
                        break; ;
                    case "4":
                        RunsGroup = "丁班";
                        break;
                }
                if (!IsPostBack)
                {
                    InitData();
                    InitTextBlock();
                    BindingListview();
                }
            }
        }
        private DataSet InitData()
        {
            ReturnPalletBLL rpb = new ReturnPalletBLL(EquipmentNumber, UserName, UserPassword, WareHouseID);
            containerDetail = rpb.GetManPowerReturnDetailByDockNumber(EquipmentNumber, UserName, "", WareHouseID, MachineNumber, DockNumber);
            CommonBLL bll = new CommonBLL(LoginUser.EquipmentNumber, UserName, LoginUser.Password, LoginUser.WarehouseID);
            DataSet ds = null; // bll.GetMachineCurPlanByMachineNumber(LoginUser.Url, LoginUser.SessionKey, MachineNumber);
            if (ds != null && ds.Tables != null)
            {
                Schemeid = ds.Tables[0].Rows[0]["Schemeid"].ToString().Trim();
                ItemName = ds.Tables[0].Rows[0]["name"].ToString();
                ProductItemId = ds.Tables[0].Rows[0]["Itemid"].ToString().Trim();
            }
            if (containerDetail != null)
            {
                containerDetail.Tables[0].Columns.Add("qty");
                containerDetail.Tables[0].Columns.Add("Schemeid");
                foreach (DataRow row in containerDetail.Tables[0].Rows)
                {
                    row["qty"] = 0.000;
                    row["Schemeid"] = Schemeid;
                }
                returnBomID = containerDetail.Tables[0].Rows[0]["Bomid"].ToString().Trim();
                containerDetail1 = containerDetail.Copy();
            }
            //添加containerDetail中
            return containerDetail1;
        }

        private void InitTextBlock()
        {
            bool flag = false;
            if (containerDetail1 != null && containerDetail1.Tables[0].Rows.Count > 0)
            {
                flag = true;
            }
            txtblk_Platform_Num.Text = DockNumber;
            txtblk_Brand.Text = ItemName;
            txtblk_Section.Text = flag ? containerDetail1.Tables[0].Rows[0]["MODELCODE"].ToString() : "未设置";//containerDetail
            txtblk_Platform_Name.Text = flag ? DockNumber : "未设置";//containerDetail
        }

        private void BindingListview()
        {
            if (containerDetail1 == null || containerDetail1.Tables[0].Rows.Count == 0)//containerDetail
            {
                lv_ReturnMaterial.DataSource = null;
                lv_ReturnMaterial.DataBind();
                return;
            }

            lv_ReturnMaterial.DataSource = containerDetail1;
            lv_ReturnMaterial.DataBind();
        }

        protected void lv_ReturnMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private bool CheckQty()
        {
            bool bFind = false;
            lastDetail = InitData().Tables[0];
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
                        this.UpdateContainerDetailsQty(textbox, ContainerDetailID, ref lastDetail);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('数据格式不正确,请重新输入!');</script>");
                        bFind = true;
                        break;
                    }
                }
            }
            this.lv_ReturnMaterial.DataSource = lastDetail;
            this.lv_ReturnMaterial.DataBind();
            if (bFind)
            {
                return false;
            }
            return true;
        }

        private void UpdateContainerDetailsQty(TextBox tb, string containerDetailId, ref DataTable dt)
        {
            if (string.IsNullOrEmpty(containerDetailId)) return;
            if (lv_ReturnMaterial.Rows == null) return;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Containerdetailid"] != null && dr["Containerdetailid"].ToString().Equals(containerDetailId))
                {
                    dr["QTY"] = Convert.ToDecimal(tb.Text.ToString().Trim());
                }
            }
        }

        protected void btn_Finish_Click(object sender, EventArgs e)
        {
            ReturnPalletBLL rpb = new ReturnPalletBLL(EquipmentNumber, UserName, UserPassword, WareHouseID);
            if (lv_ReturnMaterial.Rows != null)
            {
                if (!CheckQty())
                {
                    return;
                }
            }
            string tasknumber = string.Empty;
            if (BindEntity())
            {
                tasknumber = rpb.CreateManPowerCallMaterTask(EquipmentNumber, UserName, UserPassword, WareHouseID, taskentity, containerinfo, details);
                if (!String.IsNullOrEmpty(tasknumber))
                {
                    Session["rb_ManPowerReturnMaterials"] = true;
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('人工退料，操作执行成功!');window.location='ManReturn_Terminal.aspx';</script>");
                }
                else
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('人工退料，操作执行失败!');</script>");
                }
            }
        }

        private bool BindEntity()
        {
            details.Clear();
            string containerid = System.Guid.NewGuid().ToString("N");
            string containerbarcode = "88888888";
            taskentity.WAREHOUSEID = WareHouseID;
            taskentity.TASKTYPECODE = Properties.Settings.Default.TaskType_ManPower_Return;
            taskentity.PRIORITY = 0;
            taskentity.CONTAINERID = containerid;
            taskentity.CONTAINERBARCODE = containerbarcode;
            taskentity.ITEMID = ProductItemId;
            taskentity.OPERATIONQTY = Convert.ToDecimal(1.000);
            taskentity.BOMCODE = lastDetail.Rows[0]["Bomcode"].ToString();
            //if (this.Type.ToString().Equals("Task_ManPower_Return"))
            //{
            taskentity.TARGETADDRESSID = "14060cc623cb4a2186f1263a0bbae004";
            taskentity.TARGETADDRESSTEXT = "暂存区";
            taskentity.SOURCEADDRESSTEXT = DockNumber;
            //  taskentity.SOURCEADDRESSID=
            //}
            //else
            //{
            //    taskentity.TARGETADDRESSID = cmnMachineDock1.SelectedValue.ToString();
            //    taskentity.TARGETADDRESSTEXT = cmnMachineDock1.SelectedText.ToString();
            //    taskentity.SOURCEADDRESSTEXT = "暂存区";
            //}
            taskentity.STARTDATE = DateTime.Now;
            taskentity.STATECODE = Properties.Settings.Default.TaskNoExecuteState;
            taskentity.USERID = LoginUser.UserName;
            taskentity.CREATETIME = DateTime.Now;
            taskentity.LGVEXIST = 0;
            taskentity.FIELD1 = RunsGroup + "-" + Run;
            taskentity.FIELD2 = MachineNumber;
            taskentity.FIELD3 = Schemeid;

            containerinfo.WarehouseId = WareHouseID;
            containerinfo.ContainerId = containerid;
            containerinfo.ContainerBarCode1 = containerbarcode;
            containerinfo.BomId = returnBomID;
            containerinfo.ModelCode = lastDetail.Rows[0]["Modelcode"].ToString();//containerDetail1.Tables[0].Rows[0]["Modelcode"].ToString();
            containerinfo.LotNumber = DateTime.Now.ToString("yyyyMMdd");
            containerinfo.FullPallet = 0;
            containerinfo.Qty = Convert.ToDecimal(0.000);
            containerinfo.CreateDate = DateTime.Now;
            containerinfo.ModifyDate = DateTime.Now;
            containerinfo.CheckState = "CheckState_CheckAvoid";
            containerinfo.Source = "暂存区";
            containerinfo.ReturnUserId = LoginUser.UserName;
            containerinfo.FirstInTime = DateTime.Now;
            containerinfo.Length = 0;
            containerinfo.Width = 0;
            containerinfo.Height = 0;
            containerinfo.Weight = Convert.ToDecimal(0.000);

            for (int i = 0; i < lastDetail.Rows.Count; i++)
            {
                DataRow row = lastDetail.Rows[i];
                decimal qty = decimal.Parse(row["QTY"].ToString()) * decimal.Parse(row["RATE"].ToString());
                decimal bomqty = decimal.Parse(row["Bomqty"].ToString());
                if (bomqty < qty)
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料总量不能超出BOM的数量,请重新输入!');</script>");
                    return false;
                }
                ContainerDetailN containerdetail = new ContainerDetailN();
                containerdetail.WarehouseID = WareHouseID;
                containerdetail.ContainerDetailID = System.Guid.NewGuid().ToString("N");
                containerdetail.ContainerID = containerid;
                containerdetail.ItemID = row["Itemid"].ToString();
                containerdetail.ItemName = row["Itemname"].ToString();
                containerdetail.Qty = Convert.ToDecimal(row["qty"]);
                containerdetail.CheckState = "CheckState_CheckWaiting";
                containerdetail.FullStatus = "0";
                containerdetail.ODD1 = "0.000";
                containerdetail.ODD2 = "0.000";
                containerdetail.ODD3 = "0.000";
                containerdetail.Itemsupplier = row["Itemsupplier"].ToString().Trim();
                details.Add(containerdetail);
            }
            return true;
        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            target_Area_Des = string.Empty;
            targe_Area_Number = string.Empty;
            containerDetail = null;
            return_Destination = null;
            lastDetail = null;

            Response.Redirect("ManReturn_Terminal.aspx");
        }
    }
}