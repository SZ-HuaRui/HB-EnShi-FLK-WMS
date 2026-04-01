using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Entity.Robot
{
   public class RobotInfo:EquipmentBase
   {
        #region  设备DB块信息
        public string robotnum;//机器人编号
        public int start;
        public int length;
        public string stationno;
        public int stationtype;//1 装盘 ；2 拆盘
        public string key_ip;
        public int DBAddress;
        #endregion
        
        
        public int wcstaskno;//WCS任务号
        public int rbtaskno;//机器人任务号 0，无任务；1，装盘中；2，装盘完成(RB清0)
        public int rbstate;//机器人运行状态 0，未启动；1，自动；2，手动；3，故障
        public int rbnumber;//当前装盘数量或者未拆完量
      
        public int l_number;//当前左装盘数量
        public int r_number;//当前右装盘数量 

        public int l_barcodereading;//机器人装盘条码读取信号
        public int r_barcodereading;//机器人装盘条码读取信号

        public int checkbarcodereading;//机器人装盘条码读取信号


        public int workstation;//工位位置  1左，2右 
        public int error1;//故障代码
        public int error2;//故障代码

        public int barcode1ng;
        public int barcode2ng;

        public int command;//装盘交互指令 0，无指令；1，读取条码(RB)；2,读取完成(WCS);3,读到2后置0(RB)  |分拣交互指令（0，没指令，1申请下发，2下发完成 ，3分拣完成）
        public int ckcommand;//校验指令

        public string L_traycode = "";//左托盘条码
        public string R_traycode = "";//右托盘条码

      
       

        public int barcode1Status;//电芯1状态
        public int barcode2Status;//电芯2状态

        public string barcode1;//校验条码1
        public string barcode2;//校验条码2
        public string barcode3;//校验条码3
        public string barcode4;//校验条码4


        private List<BatteryInfo> _lstBattery = new List<BatteryInfo>();//电芯条码

        public List<BatteryInfo> LstBattery
        {
            get { return _lstBattery; }
            set { _lstBattery = value; }
        }

        private List<BatteryInfo> _lstReBattery = new List<BatteryInfo>();//复测电芯条码

        public List<BatteryInfo> ReLstBattery
        {
            get { return _lstReBattery; }
            set { _lstReBattery = value; }
        }

        public bool m_ConnectStatus = false;

        public bool m_WriteFlag = false;


        public string reworktraycode = "";//rework托盘

        public int rbpickcount;//拆盘数量

        public int rbReworkCount;//Rework盘数量

        public int  R_PickPoint;// 抓取位置
        public int  R_PutPoint;//Rework放置位置

        public int L_PickPoint;// 抓取位置
        public int L_PutPoint;//Rework放置位置

        public string ZoneCode;//库区
        public string ZoneId;

        public string jk_mes_id_1;
        public string jk_mes_id_2;
        public string jk_mes_id_3;
        public string jk_mes_id_4;

        public int L_PickFinished;//左拆盘完成
        public int R_PickFinished;//右拆盘完成

        public bool sBand1;
        public bool sBand2;

    }
}
