using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Xml;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Data;


namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class Call_Terminal_Log : System.Web.UI.Page
    {
        #region Load  自动登录到控制面板
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string warehouseId = Properties.Settings.Default.WareHouseID,
                    userCode, itemId, itemCode, itemName;
                // LogWriter.WriterSystemLogs("URL：" + Request.QueryString["cgrcode"]);
                string shift = "", team = "", planNum = "";
                //Add by wd 2017

                Response.AddHeader("P3P", "CP=CAO PSAOUR");

                try
                {
                    // SiteEQCODE：机台编号（equipment表field1字段)
                    // TASKCODE：工单号
                    // SiteCLASSTIMECODE：班次（早中晚班）
                    // SiteSHIFTCLASSCODE：班组（甲乙丙丁班）
                    // usercode：用户编码（userid）
                    // SiteMATERIALCODE：牌号code

                    if (string.IsNullOrEmpty(Request.QueryString["SiteEQCODE"])
                        || string.IsNullOrEmpty(Request.QueryString["TASKCODE"])
                        || string.IsNullOrEmpty(Request.QueryString["SiteCLASSTIMECODE"])
                        || string.IsNullOrEmpty(Request.QueryString["SiteSHIFTCLASSCODE"])
                        || string.IsNullOrEmpty(Request.QueryString["usercode"])
                        || string.IsNullOrEmpty(Request.QueryString["SiteMATERIALCODE"])
                        )
                    {
                        //LogWriter.WriterSystemLogs("URL参数不足，URL：" + Request.Url.OriginalString);
                        Page_Load_Hand(null, null); //跳转到手动登录模式
                        //ClientScript.RegisterStartupScript(Page.GetType(), "", SetInnerHtml("URL参数有误，自动登录失败！")); //参数不足
                        return;
                    }
                    //LogWriter.WriterSystemLogs("URL2：url传过来的key-value满足条件!");
                    #region 注释掉通过URL传参登录；传入的参数必须包含用户名和密码
                    //string url = Request.Url.OriginalString;
                    //userCode = Request.QueryString["usercode"].ToString().Trim('"');//工号
                    //string ecode = Request.QueryString["SiteEQCODE"].ToString().Trim('"');//设备编码
                    //shift = Request.QueryString["SiteCLASSTIMECODE"].ToString().Trim('"');//班次
                    //team = Request.QueryString["SiteSHIFTCLASSCODE"].ToString().Trim('"');//班别
                    //planNum = Request.QueryString["TASKCODE"].ToString().Trim('"');//工单
                    //itemCode = Request.QueryString["SiteMATERIALCODE"].ToString().Trim('"');//成品牌号code
                    //if (planNum.Contains("*"))
                    //{
                    //    planNum = planNum.Replace('*', '#');
                    //}

                    //LogWriter.WriterSystemLogs(string.Format("执行自动登录，工号：{0}，设备编码：{1}，班次：{2}，班别：{3}，工单：{4}，牌号：{5}。",
                    //    userCode, ecode, shift, team, planNum, itemCode), true);
                    //LogWriter.WriterSystemLogs("URL：" + url);

                    ////验证用户
                    //string userName = string.Empty;
                    //try
                    //{
                    //    if (!DataDisposal.CheckAutoLoginUser(warehouseId, userCode, out userName))
                    //    {
                    //        ClientScript.RegisterStartupScript(this.GetType(), "", SetInnerHtml("工号“" + userCode + "”未匹配到用户信息！"));
                    //        return;
                    //    }
                    //}
                    //catch (Exception)
                    //{
                    //    ClientScript.RegisterStartupScript(this.GetType(), "", SetInnerHtml("连接服务器失败！"));
                    //    return;
                    //}

                    //LoginUser.UserName = userName;
                    //Session["UserName"] = LoginUser.UserName;
                    //LoginUser.WarehouseID = warehouseId;
                    //LoginUser.EquipmentNumber = ecode;
                    //Session["WarehouseID"] = warehouseId;
                    //Session["PassWord"] = "x";
                    //txtPwd.Text = "admin";
                    //LoginUser.Password = "x";
                    //LoginUser.HasLGV = XmlReader.GetHasLGV();
                    //Session["HasLGV"] = LoginUser.HasLGV;
                    //Session["Used_Ensure"] = "no_haved";

                    //string banbie = "";
                    //string banci = "";

                    //#region 班次班组
                    //switch (shift)
                    //{
                    //    case "01":
                    //        banci = "早班";
                    //        break;
                    //    case "02":
                    //        banci = "中班";
                    //        break;
                    //    case "03":
                    //        banci = "晚班";
                    //        break;
                    //}
                    //Session["Runs"] = banci;
                    //switch (team)
                    //{
                    //    case "01":
                    //        banbie = "甲班";
                    //        break;
                    //    case "02":
                    //        banbie = "乙班";
                    //        break;
                    //    case "03":
                    //        banbie = "丙班";
                    //        break;
                    //    case "04":
                    //        banbie = "丁班";
                    //        break;
                    //}
                    //Session["RunsGroup"] = banbie;
                    //#endregion

                    //LoginUser.RunsTypeCode = banbie + "-" + banci;

                    ////获取equipmentid
                    //CommonBLL bll = new CommonBLL(LoginUser.EquipmentNumber, userCode, LoginUser.Password, LoginUser.WarehouseID);
                    //DataSet dsEqut = bll.GetEquimentid(ecode);
                    //if (dsEqut == null || dsEqut.Tables.Count == 0 || dsEqut.Tables[0].Rows.Count == 0)
                    //{
                    //    LogWriter.WriterErrorLogs("EQUIPMENT 的 FIELD1 字段未存储设备编码：" + ecode);
                    //    ClientScript.RegisterStartupScript(this.GetType(), "", SetInnerHtml("未能匹配到机台信息！设备编码：" + ecode));
                    //    return;
                    //}
                    //DataTable dtEquit = dsEqut.Tables[0];
                    //DataRow dr = dtEquit.Rows[0];
                    //LoginUser.EquipmentNumber = dr["EQUIPMENTNUMBER"].ToString().Trim();
                    //itemId = dr["ITEMID"].ToString().Trim();
                    //itemName = dr["ITEMNAME"].ToString().Trim();
                    //Session["schemeid"] = dr["SCHEMEID"] == DBNull.Value ? "" : dr["SCHEMEID"].ToString().Trim();
                    //Session["LoginUser.EquipmentNumber"] = LoginUser.EquipmentNumber;
                    //Session["productitemname"] = itemName;
                    //Session["WorkShop"] = dr["ZONECODE"].ToString().Trim();
                    //Session["WorkShopName"] = dr["ZONENAME"].ToString().Trim();
                    //Session["MachineName"] = dr["DESCRIPTION"].ToString().Trim();
                    #endregion
                }
                catch (Exception exception)
                {
                    LogWriter.WriterErrorLogs("自动登录出现异常：" + exception.Message);
                    ClientScript.RegisterStartupScript(this.GetType(), "", SetInnerHtml("连接服务器失败！"));
                    return;
                }

                ////转入控制面板
                //LogWriter.WriterSystemLogs(string.Format("机台 {0} 自动登录验证通过，跳转至控制面板。", LoginUser.EquipmentNumber));
                //Response.Redirect("Control_Panel.aspx");
                ////Response.Write("<script type='text/javascript'>window.location='Control_Panel.aspx';</script>");
            }
        }

        //获取车间
        public bool GetWorkShop()
        {
            try
            {
                CommonBLL bll = new CommonBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
                DataSet ds = bll.GetZoneByMachineNumber(LoginUser.EquipmentNumber);

                if (ds == null || ds.Tables.Count == 0)
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", SetInnerHtml("获取机台信息连接要料终端服务器失败，自动登录失败！"));
                    return false;
                }
                DataTable datatabl = ds.Tables[0];
                if (datatabl == null || datatabl.Rows.Count == 0)
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", SetInnerHtml("根据编号“" + LoginUser.EquipmentNumber + "”未匹配到机台信息，自动登录失败！"));
                    return false;
                }
                Session["WorkShopName"] = datatabl.Rows[0]["name"].ToString();
                LoginUser.WorkShop = datatabl.Rows[0]["CODE"].ToString();//获取车间代号
                LoginUser.MachineName = datatabl.Rows[0]["DESCRIPTION"].ToString();
                Session["MachineName"] = LoginUser.MachineName;
                LogWriter.WriterSystemLogs("机台 " + LoginUser.EquipmentNumber + " 获取车间信息成功。");
                return true;
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("机台 " + LoginUser.EquipmentNumber + " 获取车间信息出现异常：" + ex.Message);
                LogWriter.WriterSystemLogs("机台 " + LoginUser.EquipmentNumber + " 获取车间信息失败！");
                ClientScript.RegisterStartupScript(this.GetType(), "", SetInnerHtml("连接要料终端服务器失败！"));
            }
            return false;
        }

        //获取机台生产牌号
        public bool GetMachineCurPlanByMachineNumber()
        {
            try
            {
                CommonBLL bll = new CommonBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
                DataSet ds = null; // bll.GetMachineCurPlanByMachineNumber(LoginUser.EquipmentNumber);
                if (ds == null || ds.Tables.Count == 0)
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", SetInnerHtml("获取生产牌号连接要料终端服务器失败，自动登录失败！"));
                    return false;
                }
                DataTable datatabl = ds.Tables[0]; ;
                if (datatabl == null || datatabl.Rows.Count == 0)
                {
                    LogWriter.WriterSystemLogs("机台 " + LoginUser.EquipmentNumber + " 未设置生产牌号！");
                    ClientScript.RegisterStartupScript(Page.GetType(), "", SetInnerHtml("机台“" + LoginUser.EquipmentNumber + "”未设置生产牌号！"));
                    return false;
                }
                LoginUser.ItemID = ds.Tables[0].Rows[0]["ITEMID"].ToString();
                LoginUser.ItemName = ds.Tables[0].Rows[0]["NAME"].ToString();
                Session["productitemname"] = LoginUser.ItemName;


                LogWriter.WriterSystemLogs("机台 " + LoginUser.EquipmentNumber + " 获取生成牌号信息成功。");
                return true;
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("机台 " + LoginUser.EquipmentNumber + " 获取生成牌号信息出现异常：" + ex.Message);
                LogWriter.WriterSystemLogs("机台 " + LoginUser.EquipmentNumber + " 获取生成牌号信息失败！");
                ClientScript.RegisterStartupScript(this.GetType(), "", SetInnerHtml("连接要料终端服务器失败！"));
            }
            return false;
        }

        private bool SetPlan(string eqno, string plan, string state)
        {
            string msg = "";
            try
            {
                //CommonBLL bll = new CommonBLL(LoginUser.EquipmentNumber, UserName, LoginUser.Password, LoginUser.WarehouseID);
                if (DataDisposal.UpdateMachineProductPlan(LoginUser.WarehouseID, eqno, plan, state, out msg))
                {
                    LogWriter.WriterSystemLogs("机台 " + LoginUser.EquipmentNumber + " 校验工单信息成功。");
                    return true;
                }
                LogWriter.WriterSystemLogs("机台 " + LoginUser.EquipmentNumber + " 校验工单信息失败：" + msg);
                ClientScript.RegisterStartupScript(this.GetType(), "", SetInnerHtml(msg));
            }
            catch (Exception ex)
            {
                LogWriter.WriterErrorLogs("机台 " + LoginUser.EquipmentNumber + " 校验工单信息出现异常：" + ex.Message);
                LogWriter.WriterSystemLogs("机台 " + LoginUser.EquipmentNumber + " 校验工单信息失败！错误信息：" + ex.Message);
                ClientScript.RegisterStartupScript(this.GetType(), "", SetInnerHtml("连接要料终端服务器失败！"));
            }
            return false;
        }


        #endregion

        /// <summary>
        /// 手动登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load_Hand(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoginUser.Url = Properties.Settings.Default.NtiCloudUrl;
                LoginUser.WarehouseID = Properties.Settings.Default.WareHouseID;
                LoginUser.ClientId = Properties.Settings.Default.ClientId;
                LoginUser.AppCode = Properties.Settings.Default.AppCode;
                txtUserName.Focus();

                //txtUserName.Text = "houmingli";
                //txtPwd.Focus();
            }
        }

        #region 登录
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Login_Click(object sender, EventArgs e)
        {
            LoginUser.WarehouseID = Properties.Settings.Default.WareHouseID;
            LoginUser.ClientId = Properties.Settings.Default.ClientId;
            LoginUser.AppCode = Properties.Settings.Default.AppCode;
            string userName = txtUserName.Text.Trim();
            string passWord = txtPwd.Text;
            if (string.IsNullOrEmpty(userName))
            {
                MessageBox.alert(this, "用户名不能为空！");
                return;
            }
            if (string.IsNullOrEmpty(passWord))
            {
                MessageBox.alert(this, "密码不能为空！");
                return;
            }
            //string ip = hidIp.Value;
            string sessionKey = string.Empty;
            string token = string.Empty;
            try
            {
                string ip = hifIp.Value == "" ? "1" : hifIp.Value;
                int login_Result = new LoginBLL().WorkShop_Loging(LoginUser.Url, userName, passWord, ip, out sessionKey);
                if (login_Result == -1)//表示用户名或密码错误
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "", SetAlertMsg("用户名或密码错误！"));
                    txtPwd.Focus();
                    return;
                }
                else if (login_Result == 1)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "", SetAlertMsg("密码尝试超过限制，该用户已锁定！"));
                    txtPwd.Focus();
                    return;
                }
                else if (login_Result == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('无法访问服务器！');</script>");
                    txtPwd.Focus();
                    return;
                }
            }
            catch
            {
                ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('登录失败！');</script>");
                txtPwd.Focus();
                return;
            }

            try
            {
                bool getTokenSucc = new LoginBLL().WorkShop_GetToken(LoginUser.Url, sessionKey,LoginUser.ClientId, LoginUser.WarehouseID,LoginUser.AppCode, out token);
                if (getTokenSucc == false)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('获取token失败！');</script>");
                    txtPwd.Focus();
                    return;
                }
            }
            catch
            {
                ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('获取token失败！');</script>");
                txtPwd.Focus();
                return;
            }

            LoginUser.SessionKey = sessionKey;
            LoginUser.Token = token;
            LoginUser.UserName = userName;
            LoginUser.HasLGV = XmlReader.GetHasLGV();

            //最初始的代码
            Response.Redirect("Set_Number_new.aspx");

        } 
        #endregion

        protected void btn_Cancle_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Write("<script language='javascript'>window.opener=null;window.close()</script>");
        }

        private string SetAlertMsg(string msg)
        {
            return "<script language='javascript'>alert('" + msg + "');</script>";
        }

        private string SetInnerHtml(string msg)
        {
            return "<script language='javascript'>document.getElementById('divAutoLogin').innerHTML = '" + msg + "';</script>";
        }
    }
}