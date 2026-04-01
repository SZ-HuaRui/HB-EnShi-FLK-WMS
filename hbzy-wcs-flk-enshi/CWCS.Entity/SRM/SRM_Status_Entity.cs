using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Entity.SRM
{
  public  class SRM_Status_Entity
    {
        private string crcnum;//堆垛机编号
        private string cranemode;//堆垛机模式
        private SCTaskMode sctaskmode;//堆垛机任务模式
        private bool connectstatus;//连接状态
        private string alarminfo;//报警信息
        private string alarmmessage;//报警信息（翻译）
        private int zalarm;//总报警
        private bool change;//信息是否有变化
        private string oldlabelinfo;//上一次信息
        private string labelinfo;//显示信息
        private string crcstatus;//堆垛机状态(Equipment_OK可用，Equipment_Error不可用)
        private string crctaskflag = "0";//获取任务标记(0不可获取,1可获取非转轨任务，2可获取转轨任务)
        private string taskno = "0";//任务号
        private string fromstation;//起始地址
        private string tostation;//目标地址
        private string statostation;//输送机目标地址
        private string position;//当前位置
        private string functionreport;//功能类型 
        private string functionmode;//功能模式
        private string actionpoint;//动作端
        private string aislenumber;//巷道号 01--06
        private string rackside;//巷道左右边 01左 02右
        private string rackposition;//列值 001--053
        private string rackplace;//货位内左右/1,2
        private string racklevel;//层值 01--08
        private string rackdepth;//深度 01--02
        private string palletnu;//托盘号
        private string isInitial = "0";//是否完成初始化话
        private bool isMan = false;
        private bool checkout = true;//是否判断出库站台有货
        private SCTaskType tasktype;//任务类型



        public string IsInitial
        {
            get { return isInitial; }
            set { isInitial = value; }
        }

        public string Palletnu
        {
            get { return palletnu; }
            set { palletnu = value; }
        }

        public bool CheckOut
        {
            get { return checkout; }
            set { checkout = value; }
        }

        public bool Connectstatus
        {
            get { return connectstatus; }
            set { connectstatus = value; }
        }

        public string CRCNum
        {
            set { crcnum = value; }
            get { return crcnum; }
        }
        public string CraneMode
        {
            get { return cranemode; }
            set { cranemode = value; }
        }

        public SCTaskMode SCTaskMode
        {
            get { return sctaskmode; }
            set { sctaskmode = value; }
        }
        public string AlarmInfo
        {
            get { return alarminfo; }
            set { alarminfo = value; }
        }
        public string AlarmMessage
        {
            get { return alarmmessage; }
            set { alarmmessage = value; }
        }
        public int ZAlarm
        {
            get { return zalarm; }
            set { zalarm = value; }
        }
        public bool Change
        {
            get { return change; }
            set { change = value; }
        }
        public string OldLabelInfo
        {
            get { return oldlabelinfo; }
            set { oldlabelinfo = value; }
        }
        public string LabelInfo
        {
            get { return labelinfo; }
            set { labelinfo = value; }
        }
        public string CrcStatus
        {
            get { return crcstatus; }
            set { crcstatus = value; }
        }
        public string CrcTaskFlag
        {
            get { return crctaskflag; }
            set { crctaskflag = value; }
        }
        public string TaskNo
        {
            get { return taskno; }
            set { taskno = value; }
        }
        public string Fromstation
        {
            get { return fromstation; }
            set { fromstation = value; }
        }
        public string Tostation
        {
            get { return tostation; }
            set { tostation = value; }
        }
        public string STAToStation
        {
            get { return statostation; }
            set { statostation = value; }
        }
        public string Position
        {
            get { return position; }
            set { position = value; }
        }
        public string FunctionReport
        {
            get { return functionreport; }
            set { functionreport = value; }
        }
        public string FunctionReportLow { get; set; }
        public string FunctionReportHigh { get; set; }
        public string FunctionMode
        {
            get { return functionmode; }
            set { functionmode = value; }
        }
        public string ActionPoint
        {
            get { return actionpoint; }
            set { actionpoint = value; }
        }
        public string AisleNumber
        {
            get { return aislenumber; }
            set { aislenumber = value; }
        }
        public string RackSide
        {
            get { return rackside; }
            set { rackside = value; }
        }
        public string RackPosition
        {
            get { return rackposition; }
            set { rackposition = value; }
        }
        public string RackPlace
        {
            get { return rackplace; }
            set { rackplace = value; }
        }
        public string RackLevel
        {
            get { return racklevel; }
            set { racklevel = value; }
        }
        public string RackDepth
        {
            get { return rackdepth; }
            set { rackdepth = value; }
        }

        public bool IsMan
        {
            get { return isMan; }
            set { isMan = value; }
        }
        public SCTaskType TaskType
        {
            get { return tasktype; }
            set { tasktype = value; }
        }


    }
}
