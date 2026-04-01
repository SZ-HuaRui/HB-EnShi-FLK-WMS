using C1.Win.C1FlexGrid;
using C1.Win.C1Ribbon;
using CWCS.BLL;
using CWCS.Common;
using CWCS.Entity.SRM;
using CWCS.UIDesigner.SRMControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CWCS.UI
{
    public partial class FrmSRMManager : C1RibbonForm
    {
        private SRMBLL bll;
        private delegate void RichBoxCallBack(RichTextBox rb, string msg);
        private delegate void ShowSendLog_Delegate(string msg);
        private delegate void ShowStatus_Delegate(SRM_Robot_Entity sc);
        DataTable dtData = new DataTable();
        private Dictionary<string, string> AlarmInfoDic = new Dictionary<string, string>();
        private RGVObject SelectedRGV;
        public Dictionary<string, bool> dicEquipStatus = new Dictionary<string, bool>();
        public FrmSRMManager()
        {
            InitializeComponent();
            bll = new SRMBLL();
            bll.ShowSCStatus+=new SRMBLL.ShowEquipmentStatus(ShowStatus);
            bll.ShowSRMMsg += new SRMBLL.ShowEquipmentMessage(NotifyMsg); 
        }

        private void FrmSRMManager_Load(object sender, EventArgs e)
        {
            //Thread thread=new Thread(new ThreadStart(Start));
            //thread.Name = "FrmSRMManager";
            //thread.IsBackground = true;
            //thread.Start();
            Start();
            InitAlarmInfo();
        }

        private void Start()
        {
            bll.Start("Robo");
            if(SRMBLL.lstSRM.Count>0)
            {
                int i = 0;
                foreach (SRMObject so in SRMBLL.lstSRM.GroupBy(p=>p.SrmNo).Select(p=>p.First()).ToList())
                {
                    RGVUnit u = new RGVUnit();
                    u.XIndex = (i);
                    u.YIndex = (0);
                    u.M_SCNum = so.SrmNo;
                    u.M_Num = so.SrmNo.StartsWith("SC") ? so.SrmNo.Substring(so.SrmNo.IndexOf("SC") + 2, so.SrmNo.Length - 2 - so.SrmNo.IndexOf("SC")).PadLeft(2, '0') : so.SrmNo;
                    scControl1.Add(u);
                    i++;
                    if (!dicEquipStatus.ContainsKey(u.M_SCNum))
                        dicEquipStatus.Add(u.M_SCNum, true);
                }
            }
            scControl1.Invalidate();
        }

        private void ShowStatus(object obj)
        {
            try
            {
                SRM_Status_Entity sc = (SRM_Status_Entity)obj;
                {
                    RGVObject[] unit = scControl1.Units.Where(it => it.M_SCNum == sc.CRCNum).ToArray();

                    if (unit.Length > 0)
                    {
                        unit[0].M_WorkTaskno = sc.TaskNo;
                        if (sc.CrcStatus == "EquipmentStatus_Error")
                        {
                            unit[0].M_EquipmentState = RGVObject.EquipmentState.Equipment_Error;
                            dicEquipStatus[unit[0].M_SCNum] = false;
                        }
                        else
                        {
                            unit[0].M_EquipmentState = RGVObject.EquipmentState.Equipment_OK;
                            dicEquipStatus[unit[0].M_SCNum] = true;
                        }
                        if (sc.Connectstatus)
                            unit[0].M_ConnectState = RGVObject.ConncetState.Connected;
                        else
                            unit[0].M_ConnectState = RGVObject.ConncetState.Disconnect;
                        if (sc.CrcTaskFlag.Equals("1"))
                            unit[0].M_WorkMode = RGVObject.WorkMode.Auto;
                        else
                            unit[0].M_WorkMode = RGVObject.WorkMode.Man;
                        int actionstatus = TranSCStatus(sc.FunctionReport);
                        switch (actionstatus)
                        {
                            case 0: unit[0].M_LoadState = RGVObject.LoadState.Unload; 
                                break;//无货,等待任务
                            case 1:
                                unit[0].M_LoadState = RGVObject.LoadState.Picking;
                                break;//取货
                            case 2:
                                unit[0].M_LoadState = RGVObject.LoadState.Putting;
                                break;//放货
                            case 3://取货定位
                                unit[0].M_LoadState = RGVObject.LoadState.Loading;
                                break;
                        }
                        unit[0].M_WCSTaskType = RGVObject.WCSTaskType.None;
                        unit[0].M_PositionInfo = sc.Position;
                        if (sc.LabelInfo != null)
                        {
                            string[] info = sc.LabelInfo.Split(new char[] { '.' });
                            unit[0].M_ActionStatus = info[3];
                        }
                        switch (sc.TaskType)
                        {
                            case SCTaskType.None:
                                unit[0].M_WCSTaskType = RGVObject.WCSTaskType.None; break;//无
                            case SCTaskType.Task_InBound:
                                unit[0].M_WCSTaskType = RGVObject.WCSTaskType.Task_InBound; break;//入库
                            case SCTaskType.Task_OutBound:
                                unit[0].M_WCSTaskType = RGVObject.WCSTaskType.Task_OutBound; break;//出库任务
                            case SCTaskType.Task_Transfer:
                                unit[0].M_WCSTaskType = RGVObject.WCSTaskType.Task_Transfer; break;//移库任务
                        }
                        //if (sc.Change)
                        {
                            unit[0].M_EquipmentInfo = sc.LabelInfo.Replace(".","\r\n");
                            if (!string.IsNullOrEmpty(sc.AlarmMessage)) unit[0].M_EquipmentInfo += "故障:"+sc.AlarmMessage.Replace(";", "\r\n     "); ;
                        }

                    }
                }
               
                scControl1.Invalidate();
            }
            catch (Exception e)
            {

            }
        }



        private void NotifyMsg(string type, string msg)
        {
            if (type == "Connect"||type=="Break" || type == "SRMInit" || type == "SRMMsgInit")
            {
                ShowText(rtbCommunication, msg);
            }
            else
            {
                ShowText(rtbCommand, msg);
            }
        }

        private void ShowText(RichTextBox rb, string msg)
        {
            if (rb.InvokeRequired)
            {
                RichBoxCallBack rbcb = new RichBoxCallBack(ShowText);
                this.Invoke(rbcb, new object[] { rb, msg });
            }
            else
            {
                if (rb.TextLength > 5000)
                    rb.Clear();
                rb.AppendText(System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + ": " + msg + "\r\n");
            }
        }
    
        private void scControl1_SCMouseClick(object sender, MouseEventArgs e)
        {
            RGVObject u = (RGVObject)sender;
            if (u != null)
            {
                if (u.M_WCSControlState != RGVObject.WCSControlState.Auto)
                {
                    SelectedRGV = u;
                    this.c1ContextMenu1.ShowContextMenu(this.scControl1, new Point(e.X, e.Y));
                }
                else
                {
                    MessageBox.Show("请先将WCS堆垛机操作模式设置为手动模式,然后手动发送命令！", "提示");
                }
            }
        }

        private void c1CommandHolder1_CommandClick(object sender, C1.Win.C1Command.CommandClickEventArgs e)
        {
            try
            {
                if (SelectedRGV == null)
                {
                    MessageBox.Show("请选择堆垛机！");
                    return;
                }
                string type = "";
                switch (e.Command.Name.Trim())
                {
                    case "cmdSendTask": type = e.Command.Name.Trim(); break;
                    case "cmdChangeLoc": type = e.Command.Name.Trim(); break;
                    case "cmdTaskMode": type = e.Command.Name.Trim(); break;
                    default: type = ""; break;
                }
                if (string.IsNullOrEmpty(type))
                {
                    MessageBox.Show("命令错误，请选择正确命令！");
                    return;
                }
                FrmSRMSend frm = new FrmSRMSend(SelectedRGV, bll, type);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("命令异常，异常信息：" + ex.Message);
            }
            finally
            {
                SelectedRGV = null;
            }
        }

        private void InitAlarmInfo()
        {
            if (File.Exists(".\\SRMAlarm_ROBO.txt"))
            {
                Encoding fileEncoding = TxtFileEncoding.GetEncoding(".\\SRMAlarm_ROBO.txt", Encoding.GetEncoding("GB2312"));
                StreamReader sr = new StreamReader(".\\SRMAlarm_ROBO.txt", fileEncoding);
                string strline = sr.ReadToEnd();
                string[] msg = strline.Split(new char[] { ';' });
                string[] msg2;
                for (int i = 0; i < msg.Length; i++)
                {
                    msg2 = msg[i].Split(new char[] { ',' });
                    if (msg2.Length > 1 && !AlarmInfoDic.ContainsKey(msg2[0]))
                        AlarmInfoDic.Add(msg2[0].Trim(), msg2[1].Trim());
                }
            }
        }

        private void scControl1_SCChangWCSWorMode(object sender, EventArgs e)
        {
            RGVObject u = (RGVObject)sender;
            if (u != null)
            {
                int i_WcsWorkMode = 0;
                if (u.M_WCSControlState == RGVObject.WCSControlState.Auto)
                    i_WcsWorkMode = 1;
                else
                    i_WcsWorkMode = 0;
                bll.ChangeWCSWorkMode(u.M_SCNum, i_WcsWorkMode);
            }
        }

        private int TranSCStatus(string code)
        {
            int actionstatus = 0;
            switch (code)
            {
                case "0": actionstatus = 0; break;//return "CR_NO_FUNC_RUNNING";//空闲
                case "16": actionstatus = 1; break;// return "CR_POS_GET_WAIT_H";//等待取货
                case "17": actionstatus = 1; break;//return "CR_POS_GET_RUNNING";//取货运行中
                case "25": actionstatus = 1; break;// return "CR_POS_GET_EXECUTED";//取货完成
                case "26": actionstatus = 1; break;// return "CR_POS_GET_FAILED";//取货失败
                case "27": actionstatus = 1; break;// return "CR_POS_GET_ABORTED";//取货中断
                case "32": actionstatus = 1; break;//return "CR_GET_WAIT_H";
                case "33": actionstatus = 1; break;//return "CR_GET_RUNNING";
                case "41": actionstatus = 3; break;//return "CR_GET_EXECUTED";
                case "42": actionstatus = 1; break;//return "CR_GET_FAILED";
                case "43": actionstatus = 1; break;//return "CR_GET_ABORTED";
                case "44": actionstatus = 1; break;//return "CR_GET_MF_FAULT";
                case "45": actionstatus = 1; break;//return "CR_GET_LOC_EMPTY";
                case "46": actionstatus = 1; break;//return "CR_GET_PNOTREACH";
                case "48": actionstatus = 2; break;//return "CR_POS_PUT_WAIT_H";
                case "49": actionstatus = 2; break;//return "CR_POS_PUT_RUNNING";
                case "57": actionstatus = 2; break;//return "CR_POS_PUT_EXECUTED";
                case "58": actionstatus = 2; break;//return "CR_POS_PUT_FAILED";
                case "59": actionstatus = 2; break;// return "CR_POS_PUT_ABORTED";
                case "64": actionstatus = 3; break;//return "CR_PUT_WAIT_H";
                case "65": actionstatus = 2; break;//return "CR_PUT_RUNNING";
                case "73": actionstatus = 0; break;//return "CR_PUT_EXECUTED";
                case "74": actionstatus = 2; break;//return "CR_PUT_FAILED";
                case "75": actionstatus = 3; break;//return "CR_PUT_ABORTED";
                case "76": actionstatus = 3; break;//return "CR_PUT_MF_FAULT";
                case "77": actionstatus = 3; break;//return "CR_PUT_LOC_FULL";
                case "78": actionstatus = 2; break;//return "CR_PUT_PNOTREACH";
                case "80": actionstatus = 2; break;//return "CR_POS_PICK_WAIT_H";
                case "81": actionstatus = 2; break;//return "CR_POS_PICK_RUNNING";
                case "89": actionstatus = 1; break;//return "CR_POS_PICK_EXECUTED";
                case "90": actionstatus = 1; break;//return "CR_POS_PICK_FAILED";
                case "91": actionstatus = 1; break;//return "CR_POS_PICK_ABORTED";
                case "112": actionstatus = 1; break;// return "CR_GETC_WAIT_H";//等待取货
                case "113": actionstatus = 1; break;//return "CR_GETC_POS_RUNNING";//取货运行中
                case "114": actionstatus = 1; break;//return "CR_GETC_GET_RUNNING";//取货伸叉
                case "121": actionstatus = 1; break;// return "CR_GETC_EXECUTED";//取货完成
                case "122": actionstatus = 1; break;//return "CR_GETC_FAILED";//取货失败
                case "123": actionstatus = 1; break;//return "CR_GETC_ABORTED";//取货中止
                case "124": actionstatus = 1; break;//return "CR_GETC_MF_FAULT";//
                case "125": actionstatus = 1; break;//return "CR_GETC_LOC_EMPTY";
                case "126": actionstatus = 1; break;//return "CR_GETC_PNOTREACH";
                case "128": actionstatus = 3; break;//return "CR_PUTC_WAIT_H";
                case "129": actionstatus = 2; break;// return "CR_PUTC_POS_RUNNING";
                case "130": actionstatus = 2; break;//return "CR_PUTC_PUT_RUNNING";
                case "137": actionstatus = 0; break;//return "CR_PUTC_EXECUTED";
                case "138": actionstatus = 2; break;//return "CR_PUTC_FAILED";
                case "139": actionstatus = 2; break;//return "CR_PUTC_ABORTED";
                case "140": actionstatus = 2; break;//return "CR_PUTC_MF_FAULT";
                case "141": actionstatus = 2; break;//return "CR_PUTC_LOC_FULL";
                case "142": actionstatus = 2; break;//return "CR_PUTC_PNOTREACH";
                default: return 0;
            }
            return actionstatus;
        }
    }
}
