using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BLL;
using CWCS.Repository;
using DataGridViewAutoFilter;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Configuration;
using CWCS.Core.Library.BDManager;
using CWCS.Core.Library.AGVManager;
using CWCS.Entity;


namespace WCS
{
    public partial class FormBDManager : Form
    {
        OracleRepository _repository = new OracleRepository();
        string BdNo = "";
        string Ip = "";
        string staJZ = "";
        string staPZ = "";
        public FormBDManager()
        {
            InitializeComponent();
            
        }

        private void FormBDEquip_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable DT = _repository.GetBdInfo();
                if (DT != null)
                {
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        BdNo = DT.Rows[i]["BDNO"].ToString();
                        Ip = DT.Rows[i]["BDIP"].ToString();
                        staJZ = DT.Rows[i]["STAJZ"].ToString();
                        staPZ = DT.Rows[i]["STAPZ"].ToString();
                        initBD(BdNo, Ip, staJZ, staPZ);
                    }
                }
            }
            catch (Exception ex)
            {
                rtb_info.Text += $"Bd门信息初始化异常：{ex.Message}";
            }
            
            //initBD("1", "10.157.170.245", "9091", "9092");
            //initBD("2", "10.157.170.246", "9101", "9102");
            //initBD("3", "10.157.170.240", "9111", "9112");
            //initBD("4", "10.157.170.237", "9121", "9122");
        }
        private void initBD(string name, string ip, string staJZ, string staPZ)
        {
            BDControl bd = new BDControl(ip, 1205);
            bd.ShowConn += OnConnect;
            bd.OnConfirmCMD += (showip, bt) =>
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    rtb_info.Text += $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}:{showip}确认收到{bt}指令{Environment.NewLine}";

                }));
            };
            bd.OnRecDoorStatus += (showip, which, cmd) =>
            {
                try
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        rtb_info.Text += $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}:{showip}收到，{BDDoorStatus.WhichDoorDesc[which]}状态，{BDDoorStatus.DoorStatusDesc[cmd]}{Environment.NewLine}";
                        if (dv_BDView.SelectedRows.Count > 0)
                        {
                            var ip = (string)dv_BDView.SelectedRows[0].Cells[2].Value;
                            if (ip != showip) return;
                            switch (which)
                            {
                                case BDRecCMD.RecDoorJZ:
                                    lbl_JZ.Text = BDDoorStatus.DoorStatusDesc[cmd];
                                    break;
                                case BDRecCMD.RecDoorPZ:
                                    lbl_PZ.Text = BDDoorStatus.DoorStatusDesc[cmd];
                                    break;
                                default:
                                    break;
                            }
                        }
                    }));
                }
                catch (Exception)
                {

                }
            };
            var workStaJZ = new WorkStation() { DeviceCode = staJZ, WorkStationCode = BDRecCMD.RecDoorJZ, Context = bd };
            var workStaPZ = new WorkStation() { DeviceCode = staPZ, WorkStationCode = BDRecCMD.RecDoorPZ, Context = bd };
            bd.workStation = new List<WorkStation> { workStaJZ, workStaPZ };
            BDControl.workStations.AddRange(bd.workStation);
            dv_BDView.Rows.Add($"BD0{name}", $"高速{name}", ip, "无");
            bd.Run();
        }
        private void OnConnect(string showip, NotifyCommand type, string str1)
        {
            try
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    if (dv_BDView.SelectedRows.Count > 0)
                    {
                        var ip = (string)dv_BDView.SelectedRows[0].Cells[2].Value;
                        if (ip != showip) return;
                        if (type == NotifyCommand.Connect)
                        {
                            lb_connect.Text = "已连接";
                            lb_connect.BackColor = Color.Lime;
                        }
                        else
                        {
                            lb_connect.Text = "断线";
                            lb_connect.BackColor = Color.Red;
                        }
                    };
                }));
            }
            catch (Exception)
            {

            }
        }
        private void dv_BDView_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dv_BDView.SelectedRows.Count > 0)
                {
                    var ip = (string)dv_BDView.SelectedRows[0].Cells[2].Value;
                    var ctxs = BDControl.workStations.FirstOrDefault(c => c.Context?._ServerAddr == ip);
                    if (ctxs?.Context.ConnectStatus == true)
                    {
                        lb_connect.Text = "已连接";
                        lb_connect.BackColor = Color.Lime;
                        foreach (var item in ctxs?.Context.workStation)
                        {
                            if (BDDoorStatus.DoorStatusDesc.ContainsKey(item.DoorStatus))
                            {
                                switch (item.WorkStationCode)
                                {
                                    case BDRecCMD.RecDoorJZ:
                                        lbl_JZ.Text = BDDoorStatus.DoorStatusDesc[item.DoorStatus];
                                        break;
                                    case BDRecCMD.RecDoorPZ:
                                        lbl_PZ.Text = BDDoorStatus.DoorStatusDesc[item.DoorStatus];
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        lb_connect.Text = "断线";
                        lb_connect.BackColor = Color.Red;
                        lbl_JZ.Text = "待读取";
                        lbl_PZ.Text = "待读取";
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
