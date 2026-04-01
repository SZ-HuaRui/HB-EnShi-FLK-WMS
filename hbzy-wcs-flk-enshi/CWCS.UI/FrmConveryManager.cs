using C1.Win.C1Ribbon;
using CWCS.Entity.Convery;
using CWCS.UIDesigner.ConveryControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CWCS.BLL;
using CWCS.Repository;

namespace CWCS.UI
{
    public partial class FrmConveryManager : C1RibbonForm
    {
        public DataTable dtSource = new DataTable();
        public DataTable dtConfig = new DataTable("ConveryStations");
        public ConveryBLL bll = null;
        public Dictionary<string, Color> listColor = new Dictionary<string, Color>();
        private Dictionary<ConveryStationInfo, ConveryStationsList> LstInfo = new Dictionary<ConveryStationInfo, ConveryStationsList>();
        public Dictionary<string, bool> dicEquipStatus = new Dictionary<string, bool>();
        private delegate void LabelCallBack(string ip, string time, bool b_ConnectStatus);
        public bool NeedCheckUser = false;
        public OracleRepository _repository = new OracleRepository();

        public FrmConveryManager()
        {
            InitializeComponent();
            this.converyPanel1.MouseWheel += new MouseEventHandler(converyPanel1_OnMouseWheel);
        }

        private void FrmConveryManager_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            bll = new ConveryBLL();
            LoadingData();
            bll.RefreshStation += ShowConveryStatus;
            bll.WriteToLeftLog += WriteToLeftLog;
            bll.WriteToDowntLog += WriteToDownLog;
            bll.ShowStationPLCStatus += ShowScanTime;
            bll.EventInit();
            bll.ConveyorInit();
            tabTray.SelectedIndex = 0;
        }

      


        private void LoadingData()
        {
            dtSource = _repository.GetOPCItem();
            if (File.Exists("ConveryStations.xml"))
            {
                try
                {
                    dtConfig.ReadXmlSchema("ConveryStations.xml");
                    dtConfig.ReadXml("ConveryStations.xml");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("加载配置文件失败，详细信息：" + ex.Message, "提示");
                    return;
                }
            }
            if (dtConfig.Columns.Count == 0)
            {
                dtConfig.Columns.Add("StationNo");//站台编号
                dtConfig.Columns.Add("StationType");//站台类型
                dtConfig.Columns.Add("locationx");//X
                dtConfig.Columns.Add("locationY");//Y
                dtConfig.Columns.Add("Width");//宽
                dtConfig.Columns.Add("Height");//高
                dtConfig.Columns.Add("Direction");//方向
                dtConfig.Columns.Add("DB");//DB块
                dtConfig.Columns.Add("Length");//DB块长度
                dtConfig.Columns.Add("Value");//DB值
                dtConfig.Columns.Add("IP");//IP地址
                dtConfig.Columns.Add("Belong");//所属线程
                dtConfig.Columns.Add("ZoneCode");//所属区域
            }
            InitConveryStations();
        }


        void InitConveryStations()
        {
            try
            {
                List<DataRow> drNew = new List<DataRow>();
                Color defaultcolor = new ConveryUnitRect().BackColor;
                listColor.Clear();
                listColor.Add("", defaultcolor);
                int count = 0;
                foreach (DataRow dr in dtSource.Select("status = '1'")) 
                {
                    ConveryStationInfo csi = new ConveryStationInfo() { X = dr["locationx"].ToString(), Y = dr["locationy"].ToString(), Width = dr["width"].ToString(), Height = dr["height"].ToString(), ConveryNu = dr["stationno"].ToString() };
                    string converyStationNu = dr["stationno"].ToString();
                    if (LstInfo.FirstOrDefault(v => v.Key.Equals(csi)).Value == null)
                    {
                        count++;

                        DataRow[] drs = (dtConfig.Rows.Count > 0 ? dtConfig.Select("stationno='" + dr["stationno"] + "'") : dtSource.Select("1=2"));
                        ConveryUnitRect u;
                        if (drs.Count() > 0)
                        {
                            u = new ConveryUnitRect();
                            u.Visible = false;
                            u.XIndex = Convert.ToInt32(dr["locationx"]);
                            u.YIndex = Convert.ToInt32(dr["locationy"]);
                            u.WidthColumns = Convert.ToInt32(dr["width"]);
                            u.HeightRows = Convert.ToInt32(dr["height"]);

                            u.Data = dr;
                            if (dr["stationtype"].ToString().Trim() == "1")//入库申请站台
                                u.StationType = ConveryUnit.UnitType.InFeeding;
                            else if (dr["stationtype"].ToString().Trim() == "2")//入库到位站台
                                u.StationType = ConveryUnit.UnitType.InFeeded;
                            else if (dr["stationtype"].ToString().Trim() == "3")//入库到位站台
                                u.StationType = ConveryUnit.UnitType.OutFeeding;
                            else if (dr["stationtype"].ToString().Trim() == "2")//入库到位站台
                                u.StationType = ConveryUnit.UnitType.OutFeeded;
                            else
                                u.StationType = ConveryUnit.UnitType.Normal;
                            if (dr["direction"].ToString().Trim() == "1")//输送机方向
                            {
                                u.Direct = ConveryUnit.UnitOrientation.Vertical;
                            }
                            else if (dr["direction"].ToString().Trim() == "2")
                            {
                                u.Direct = ConveryUnit.UnitOrientation.Horizontal;
                            }
                        }
                        else
                        {
                            u = new ConveryUnitRect();
                            string status = dr["status"].ToString();
                            if (string.IsNullOrEmpty(status))
                                status = "0";
                            if (int.Parse(status) == 1)
                                u.Visible = true;
                            else
                                u.Visible = false;
                            u.XIndex = Convert.ToInt32(dr["locationx"]);
                            u.YIndex = Convert.ToInt32(dr["locationy"]);
                            u.WidthColumns = Convert.ToInt32(dr["width"]);
                            u.HeightRows = Convert.ToInt32(dr["height"]);
                            u.TextDescription = dr["TextDescription"].ToString();
                            u.Text = dr["stationno"].ToString();
                            u.ConveryNumber = u.Text;
                            u.Data = dr;
                            u.BackColor = Color.Gainsboro;
                            if (dr["stationtype"].ToString().Trim() == "1")//入库申请站台
                                u.StationType = ConveryUnit.UnitType.InFeeding;
                            else if (dr["stationtype"].ToString().Trim() == "2")//入库到位站台
                                u.StationType = ConveryUnit.UnitType.InFeeded;
                            else if (dr["stationtype"].ToString().Trim() == "3")//入库到位站台
                                u.StationType = ConveryUnit.UnitType.OutFeeding;
                            else if (dr["stationtype"].ToString().Trim() == "2")//入库到位站台
                                u.StationType = ConveryUnit.UnitType.OutFeeded;
                            else
                                u.StationType = ConveryUnit.UnitType.Normal;
                            if (dr["direction"].ToString().Trim() == "2")//输送机方向
                            {
                                u.Direct = ConveryUnit.UnitOrientation.Vertical;
                            }
                            else if (dr["direction"].ToString().Trim() == "1")
                            {
                                u.Direct = ConveryUnit.UnitOrientation.Horizontal;
                            }
                            if (dr["arrowdirection"].ToString().Trim() == "1")
                                u.Arrow = ConveryUnit.UnitArrowDirection.Right;
                            if (dr["arrowdirection"].ToString().Trim() == "2")
                                u.Arrow = ConveryUnit.UnitArrowDirection.Left;
                            if (dr["arrowdirection"].ToString().Trim() == "3")
                                u.Arrow = ConveryUnit.UnitArrowDirection.Down;
                            if (dr["arrowdirection"].ToString().Trim() == "4")
                                u.Arrow = ConveryUnit.UnitArrowDirection.Up;
                            if (dr["arrowdirection"].ToString().Trim() == "5")
                                u.Arrow = ConveryUnit.UnitArrowDirection.LeftRight;
                            if (dr["arrowdirection"].ToString().Trim() == "6")
                                u.Arrow = ConveryUnit.UnitArrowDirection.UpDown;
                            if (dr["groupname"].ToString().Trim() == "Convery")
                            {
                                u.EquipmentType = ConveryUnit.UnitEquipmentType.Convery;
                            }
                            if (dr["groupname"].ToString().Trim() == "Text")
                            {
                                u.EquipmentType = ConveryUnit.UnitEquipmentType.Text;
                            }
                        }

                        if (!converyPanel1.Units.Exists(it => it.Data == u.Data))
                            converyPanel1.Add(u);

                    }
                 
                }
                converyPanel1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Source);
            }
        }

        public struct ConveryStationInfo
        {
            public string X;
            public string Y;
            public string Width;
            public string Height;
            public string ConveryNu;

            public override bool Equals(object obj)
            {
                ConveryStationInfo csi = (ConveryStationInfo)obj;
                return csi.ConveryNu == this.ConveryNu;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public override string ToString()
            {
                return this.ConveryNu;
            }

        }

        public class ConveryStationsList
        {
            public List<DataRow> rows = new List<DataRow>();
        }


        private void converyPanel1_OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                converyPanel1.UnitLW++;
            }
            else
            {
                if (converyPanel1.UnitLW >= 25)
                    converyPanel1.UnitLW--;
            }
            Invalidate();
        }

        public void WriteToDownLog(string message)
        {
            if (txtWriteLog.InvokeRequired)
            {
                Action<string> callback = WriteToDownLog;
                Invoke(callback,new object[] {message });
            }
            else
            {
                txtWriteLog.AppendText($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}:{message}\r\n"); ;
                if (txtWriteLog.TextLength > 2000)
                {
                    txtWriteLog.Clear();
                }
            }
        }

        public void WriteToLeftLog(string message)
        {
            if (txtLog.InvokeRequired)
            {
                Action<string> callback = WriteToDownLog;
                Invoke(callback, new object[] { message });
            }
            else
            {
                txtLog.AppendText($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}:{message}\r\n"); ;
                if (txtLog.TextLength > 2000)
                {
                    txtLog.Clear();
                }
            }
        }

     

        private void ShowConveryStatus(List<ConveryEntity> lstConvery)
        {
            foreach (ConveryEntity convery in lstConvery)
            {
                ConveryUnit[] unit = converyPanel1.Units.Where(it => it.ConveryNumber == convery.StationNo.ToString() && convery.config.Equals("Convery")).ToArray();
                if (unit.Count() > 0)
                {   
                    if (convery.C_TaskInfo.TaskNumber > 0)
                    {
                        switch (convery.C_TaskInfo.GoodsType)
                        {
                            case 1: unit[0].GoodsType = ConveryUnit.UnitGoodsType.RealPallet; break;
                            case 2: unit[0].GoodsType = ConveryUnit.UnitGoodsType.SingleNullPallet; break;
                            case 3: unit[0].GoodsType = ConveryUnit.UnitGoodsType.ExceptionPallet; break;
                            case 10: unit[0].GoodsType = ConveryUnit.UnitGoodsType.NullPalletGroup; break;
                            default: unit[0].GoodsType = ConveryUnit.UnitGoodsType.unknown; break;
                        }
                    }
                    else
                        unit[0].GoodsType = ConveryUnit.UnitGoodsType.Empty;
                    if (convery.C_TaskInfo.CheckInfo > 0 || convery.C_TaskInfo.SystemError > 0)
                        unit[0].EuipmentState = ConveryUnit.UnitEuipmentState.Error;
                    else
                        unit[0].EuipmentState = ConveryUnit.UnitEuipmentState.OK;
                    unit[0].Data = convery.C_TaskInfo;
                 

                    if (convery.config.Equals("Other"))
                    {
                        unit[0].EuipmentState = ConveryUnit.UnitEuipmentState.Other;
                    }

                    if (unit[0].EuipmentState == ConveryUnit.UnitEuipmentState.Error)
                    {
                        dicEquipStatus[unit[0].ConveryNumber] = false;
                    }
                    else if (unit[0].EuipmentState == ConveryUnit.UnitEuipmentState.OK)
                    {
                        dicEquipStatus[unit[0].ConveryNumber] = true;
                    }
                }
            }
            converyPanel1.Invalidate();
        }

        private void ShowScanTime(string strIP, string strDuration, bool b_ConnectStatus)
        {
            if (txt_scantime.InvokeRequired)
            {
                LabelCallBack lcb = new LabelCallBack(ShowScanTime);
                this.Invoke(lcb, new object[] { strIP, strDuration, b_ConnectStatus });
            }
            else
            {
                if (b_ConnectStatus)
                    txt_scantime.BackColor = txt_scantime.BackColor == Color.Lime ? Color.DarkGray : Color.Lime;
                else
                    txt_scantime.BackColor = Color.Red;
                txt_scantime.Text = strDuration;
            }
        }

        private void converyPanel1_UnitMouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    ConveryUnit u = (ConveryUnit)sender;
                    if (u != null)
                    {
                        ConveryTaskInfo cs = u.Data as ConveryTaskInfo;
                        if (cs == null)return;
                        txtCheckInfo.Text = string.Empty;
                        txtStationNum.Text = cs.StationNo;
                        txtTaskNum.Text = cs.TaskNumber.ToString();
                        txtBarCode.Text = cs.Barcode ??= "0";
                        txtGoodsType.Text = cs.GoodsType.ToString();
                        txtFromStation.Text = cs.FromStation.ToString();
                        txtToStation.Text = cs.ToStation.ToString();
                        txtQty.Text = cs.Qty.ToString();
                        txtRFIDFlag.Text = cs.RfidFlag.ToString();
                        if (cs.CheckInfo > 0) txtCheckInfo.Text = TransErrorInfo(cs.CheckInfo);
                        txtSystemError.Text = cs.SystemError.ToString();
                    }

                }
            }
            catch(Exception ex)
            {
                txtWriteLog.AppendText(ex.Message+ex.StackTrace);
            }

        }

        private string TransErrorInfo(int checkinfo)
        {
            string _ledtext = "故障:";
            string errorcode = Convert.ToString(checkinfo, 2).PadLeft(16, '0');
            char[] ErrorCode = errorcode.ToCharArray();
            for (int j = 0; j < 16; j++)
            {
                if (ErrorCode[j] == '1')
                {
                    switch (j)
                    {
                        case 0:
                            _ledtext = _ledtext + "";
                            break;
                        case 1:
                            _ledtext = _ledtext + "";
                            break;
                        case 2:
                            _ledtext = _ledtext + "右超宽;";
                            break;
                        case 3:
                            _ledtext = _ledtext + "左超宽;";
                            break;
                        case 4:
                            _ledtext = _ledtext + "后超长;";
                            break;
                        case 5:
                            _ledtext = _ledtext + "前超长;";
                            break;
                        case 6:
                            _ledtext = _ledtext + "超高;";
                            break;
                        case 7:
                            _ledtext = _ledtext + "故障;";
                            break;
                        case 15:
                            _ledtext = _ledtext + "烟包条码读取错误;";
                            break;
                        case 14:
                            _ledtext = _ledtext + "托盘条码读取错误;";
                            break;
                        case 13:
                            _ledtext = _ledtext + "重量读取错误;";
                            break;
                        default:
                            _ledtext = _ledtext + "未知异常;";
                            break;
                    }
                }
            }
            return _ledtext;
        }

    

        private void btnClean_Click(object sender, EventArgs e)
        {
            txtTaskNum.Text = "0";
            txtBarCode.Text = "0";
            txtGoodsType.Text = "0";
            txtFromStation.Text = "0";
            txtToStation.Text = "0";
            txtQty.Text = "0";
            txtRFIDFlag.Text = "0";
            txtSystemError.Text = "0";
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
                try
                {
                    if (!string.IsNullOrEmpty(txtStationNum.Text.Trim()))
                    {
                        string[] info = bll.ReadStationInfo(txtStationNum.Text.Trim());
                        info[0] = txtTaskNum.Text.Trim();
                        info[1] = txtBarCode.Text.Trim().ToString();
                        info[2] = txtGoodsType.Text.Trim();
                        info[3] = txtFromStation.Text.Trim();
                        info[4] = txtToStation.Text.Trim();
                        info[5] = txtQty.Text.Trim();
                        info[7] = txtRFIDFlag.Text.Trim();
                        info[9] = txtSystemError.Text.Trim();

                        if (bll.WriteStationInfo(info, txtStationNum.Text.Trim()))
                        {
                            MessageBox.Show("修改成功！");
                            _repository.InsertEventlog($"用户修改站台信息。站台号：{txtStationNum.Text},任务号：{txtTaskNum.Text},货物类型：{txtGoodsType.Text},起始地址：{txtFromStation.Text},目标地址：{txtToStation.Text}");
                        }
                        else
                        {
                            MessageBox.Show("修改失败！");
                        }
                    }
                }
                catch
                {

                }
            }

        private void btnOpenLog_Click(object sender, EventArgs e)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                System.Diagnostics.Process.Start(path);
            }
            catch
            {

            }
          
        }
    }
}
