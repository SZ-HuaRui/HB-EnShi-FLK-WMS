using C1.Win.C1Ribbon;
using CWCS.Entity;
using CWCS.Repository;
using CWCS.UIDesigner.ConveryControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CWCS.UIControl.Convery
{
    public partial class FrmConveryDesigner : C1RibbonForm
    {
        DataTable dtSource = new DataTable();
        DataTable dtConfig = new DataTable("ConveryStations");
        Dictionary<string, Point> Fulllocations = new Dictionary<string, Point>();
        Dictionary<ConveryStationInfo, ConveryStationsList> LstInfo = new Dictionary<ConveryStationInfo, ConveryStationsList>();
        Dictionary<ConveryUnit, DataRow> UnitsConfig = new Dictionary<ConveryUnit, DataRow>();
        List<DataRow> list = new List<DataRow>();
        public OracleRepository _repository = new OracleRepository();
        public FrmConveryDesigner()
        {
            InitializeComponent();
        }

        private void FrmConveryDesigner_Load(object sender, EventArgs e)
        {
            LoadingData();
            LoadWcsParamConfig();
        }

        private void LoadWcsParamConfig()
        {
            DataTable dt = _repository.QueryTableData(new WCS_PARAMETER_CONFIG());
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    WCS_PARAMETER_CONFIG item = new WCS_PARAMETER_CONFIG();
                    item.T_ID = dr["t_id"].ToString();
                    item.PARAM_CODE = dr["param_code"].ToString();
                    item.PARAM_NAME = dr["param_name"].ToString();
                    item.PARAM_VALUE = dr["param_value"].ToString();
                    item.PARAM_TYPE = dr["param_type"].ToString();
                    item.CAN_EDIT = dr["can_edit"].ToString();
                    StaticCommon.WcsParamConfigList.Add(item);
                }
            }

        }
        private void btnBatchInsert_Click(object sender, EventArgs e)
        {
                FrmAddConvery frm = new FrmAddConvery();
                frm.ShowDialog();
                LoadingData();
        }
        public void LoadingData()
        {
            dtSource = _repository.GetOPCItem();
            InitConveryStations();
        }

        void InitConveryStations()
        {
            try
            {
                if (dtSource.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtSource.AsEnumerable().Where(x=>x["status"].Equals("1")))
                    {
                        if (converyPanel1.Units.Exists(x => x.Text.Equals(dr["stationno"].ToString())))
                        {
                            List<ConveryUnit> listUnits = converyPanel1.Units.Where(x => x.Text.Equals(dr["stationno"].ToString())).ToList();
                            foreach (ConveryUnit cu in listUnits)
                            {
                                cu.XIndex = Convert.ToInt32(dr["locationx"].ToString());
                                cu.YIndex = Convert.ToInt32(dr["locationy"].ToString());
                                cu.WidthColumns = Convert.ToInt32(dr["width"]);
                                cu.HeightRows = Convert.ToInt32(dr["height"]);
                                cu.TextDescription = dr["TextDescription"].ToString();
                                if (dr["arrowdirection"].ToString().Trim() == "1")
                                    cu.Arrow = ConveryUnit.UnitArrowDirection.Right;
                                if (dr["arrowdirection"].ToString().Trim() == "2")
                                    cu.Arrow = ConveryUnit.UnitArrowDirection.Left;
                                if (dr["arrowdirection"].ToString().Trim() == "3")
                                    cu.Arrow = ConveryUnit.UnitArrowDirection.Down;
                                if (dr["arrowdirection"].ToString().Trim() == "4")
                                    cu.Arrow = ConveryUnit.UnitArrowDirection.Up;
                                if (dr["arrowdirection"].ToString().Trim() == "5")
                                    cu.Arrow = ConveryUnit.UnitArrowDirection.LeftRight;
                                if (dr["arrowdirection"].ToString().Trim() == "6")
                                    cu.Arrow = ConveryUnit.UnitArrowDirection.UpDown;
                                if (dr["direction"].ToString().Trim() == "2")
                                {
                                    cu.Direct = ConveryUnit.UnitOrientation.Vertical;
                                }
                                else if (dr["direction"].ToString().Trim() == "1")
                                {
                                    cu.Direct = ConveryUnit.UnitOrientation.Horizontal;
                                }
                                cu.Data = dr;
                            }
                        }
                        else
                        {
                            ConveryUnitRect u = new ConveryUnitRect();
                            u.Visible = true;
                            u.XIndex = Convert.ToInt32(dr["locationx"]);
                            u.YIndex = Convert.ToInt32(dr["locationy"]);
                            u.WidthColumns = Convert.ToInt32(dr["width"]);
                            u.HeightRows = Convert.ToInt32(dr["height"]);
                            u.TextDescription = dr["TextDescription"].ToString();
                            u.ConveryNumber = dr["stationno"].ToString();
                            u.Text = u.ConveryNumber;
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
                                u.EquipmentType = ConveryUnit.UnitEquipmentType.Convery;
                            if (dr["groupname"].ToString().Trim() == "Text")
                            {
                                u.EquipmentType = ConveryUnit.UnitEquipmentType.Text;
                            }

                            if (!converyPanel1.Units.Exists(it => it.Data == u.Data))
                                converyPanel1.Add(u);
                        }
                    }

                    converyPanel1.Invalidate();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Source);
            }
        }

        struct ConveryStationInfo
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
            public override string ToString()
            {
                return this.ConveryNu;
            }

        }

        class ConveryStationsList
        {
            public List<DataRow> rows = new List<DataRow>();
            public int total;
            public int fullConveryStations;
            public ConveryUnitRect unit;
            public string memo;
            public ConveryStationInfo lcd;
        }

        private void btnModity_Click(object sender, EventArgs e)
        {
            WCS_OPCITEM item = new WCS_OPCITEM();

            item.Stationno = txtStationNum.Text;
            item.Stationtype = txtStationType.Text.Trim();
            item.Width = txtWidth.Text.Trim();
            item.Height = txtHight.Text.Trim();
            item.Locationx = txtLocationX.Text.Trim();
            item.Locationy = txtLocationY.Text.Trim();
            item.Direction = txtOrientation.Text.Trim();
            item.Signaltype = txtDB.Text.Trim();
            item.Value = txtLength.Text.Trim();
            item.Belong = txtBelong.Text.Trim();
            item.Groupname = txtGroupName.Text.Trim();
            item.ArrowDirection = txtArrowDirection.Text.Trim();
            item.TextDescription = txtTextDescription.Text.Trim();
            item.Status = txtStatus.Text.Trim();
            if (_repository.UpdateStationInfo(item))
            {
                MessageBox.Show("更新成功！", "提示", MessageBoxButtons.OK);
            }
            if (item.Status.Equals("0"))
            {
                if (converyPanel1.Units.Exists(x => x.Text.Equals(item.Stationno)))
                {
                    ConveryUnit unit = converyPanel1.Units.FirstOrDefault(x => x.Text.Equals(item.Stationno));
                    if (unit != null)
                    {
                        converyPanel1.Units.Remove(unit);
                    }
                }
            }
           
            btnRefresh_Click(null, null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var unitmove = converyPanel1.Units.Where(it => it.MoveFlag).ToArray();
            if (unitmove.Length > 0)
            {
                string move = "";
                foreach(ConveryUnit u in unitmove)
                {
                    move +="编号："+u.ConveryNumber+",";
                   
                   if(u.Data!=null)
                   {
                       WCS_OPCITEM item = new WCS_OPCITEM();
                       DataRow dr= (DataRow)(u.Data);
                       item.Stationno = dr["Stationno"].ToString().Trim();
                       item.Stationtype = dr["Stationtype"].ToString().Trim();
                       item.Width = dr["Width"].ToString().Trim();
                       item.Height = dr["Height"].ToString().Trim();
                       item.Locationx = u.XIndex.ToString();
                       item.Locationy = u.YIndex.ToString();
                       item.Direction = dr["Direction"].ToString().Trim();
                       item.Signaltype = dr["Signaltype"].ToString().Trim();
                       item.Value = dr["Value"].ToString().Trim();
                       item.Belong = dr["Belong"].ToString().Trim();
                       item.ArrowDirection = dr["ArrowDirection"].ToString().Trim();
                       item.TextDescription = dr["TextDescription"].ToString().Trim();
                        if (_repository.UpdateStationInfo(item))
                       {
                           move += "设备编号："+item.Stationno;
                           //ConveryMove(item);
                           u.Visible = true;
                       }
                   }
                }
                if(!string.IsNullOrEmpty(move))
                  MessageBox.Show(move);
                converyPanel1.ClearMoveFlag();
            }
        }

        private void ConveryMove(WCS_OPCITEM entity)
        {
            var q = converyPanel1.Units.Where(v => ((DataRow)v.Data)["itemname"].ToString() == entity.Stationno.Trim());//.ToArray();
            if (q != null) converyPanel1.Units.Remove(q.FirstOrDefault());
            //if (q.Length == 1)
            //{
            //    converyPanel1.Units.Remove((ConveryUnitRect)q);
            //    foreach (ConveryUnit u in q)
            //    {
            //        //u.XIndex = Convert.ToInt32(entity.Locationx);
            //        //u.YIndex = Convert.ToInt32(entity.Locationy);
            //        u.WidthColumns = Convert.ToInt32(entity.Width);
            //        u.HeightRows = Convert.ToInt32(entity.Height);
            //        u.Visible = false;
            //    }
            //}
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadingData();
        }

        private void converyPanel1_UnitMouseClick(object sender, MouseEventArgs e)
        {
            ConveryUnit u = (ConveryUnit)sender;
            if (u != null)
            {
                DataRow dr = (DataRow)u.Data;
                if (dr != null)
                {
                    txtStationNum.Text = dr["stationno"].ToString().Trim();
                    txtStationType.Text = dr["stationtype"].ToString().Trim();
                    txtWidth.Text = dr["width"].ToString().Trim();
                    txtHight.Text = dr["height"].ToString().Trim();
                    txtLocationX.Text = u.XIndex.ToString();

                    txtLocationY.Text = u.YIndex.ToString();
                    txtOrientation.Text = dr["DIRECTION"].ToString().Trim();
                    txtDB.Text = dr["signaltype"].ToString().Trim();
                    txtLength.Text = dr["value"].ToString().Trim();
                    txtBelong.Text = dr["Belong"].ToString().Trim();
                    txtArrowDirection.Text = dr["ArrowDirection"].ToString().Trim();
                    txtGroupName.Text = dr["groupname"].ToString().Trim();
                    txtTextDescription.Text = dr["textdescription"].ToString().Trim();
                    txtStatus.Text = dr["status"].ToString().Trim();
                    c1DockingTabPage1.TabVisible =true;
                    c1DockingTab3.SlideShowPage(c1DockingTabPage1);
                }
            }
        }

        private void btnCreateRoute_Click(object sender, EventArgs e)
        {
            FrmAddRoute frm = new FrmAddRoute();
            frm.ShowDialog();
        }

        private void btnAddTextStation_Click(object sender, EventArgs e)
        {
            FormAddTextStation form = new FormAddTextStation();
            form.ShowDialog();
            LoadingData();
        }
    }
}
