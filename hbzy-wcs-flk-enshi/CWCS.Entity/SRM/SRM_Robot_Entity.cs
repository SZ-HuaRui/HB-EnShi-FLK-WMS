using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Entity.SRM
{
    public class SRM_Robot_Entity : EquipmentBase
    {
        #region 堆垛机DB块信息
        public string SCNO;      //堆垛机编号

        public string key_ip;    //IP地址

        public int DBAddress_R; //读取堆垛机数据信息DB块
        public int value_R;    //读取堆垛机信息起始地址值
        public int lenght_R;  //读取堆垛机信息长度值

        public int DBAddress_W; //下发数据信息DB块
        public int value_W;    //下发信息起始地址值
        public int lenght_W;  //下发信息长度值

        public bool cFlag;  //通讯标志
        public int cMode=1;//模式 1：单工位，2双工位
        #endregion

        #region WCS下发信息区域
        public int b_O_HandShake;//WCS与堆垛机握手信号，WCS每个周期将b_I_HandShake的值取反写入b_O_HandShake，3秒内握手信号未发生改变认为通讯中断
        public int b_O_SRM_Num;  //巷道编号 当有调度柜时需要指明该条指令是分配给某一指定巷道堆垛机。若WCS对堆垛机直接组态时，该变量无意义
        public int b_O_CallBack;  //堆垛机召回命令 [1:招回,0:默认]
        public int b_O_Cancel_Task1;// 堆垛机取消当前工位1任务命令 [1:取消任务,0:默认]
        public int b_O_Cancel_Task2; //堆垛机取消当前工位2任务命令 [1:取消任务,0:默认]
        public int b_O_EStop;       //急停信号[1:急停,0:默认]
        public int b_O_TravelPos1;//工位1行走起始列
        public int b_O_LiftPos1;  //工位1升降起始层
        public int b_O_Task1;     //工位1起始位置伸叉方向[1：左近，2：右近][3：左远，4：右远]
        public int b_O_TravelPos2; //工位1行走目标列
        public int b_O_LiftPos2;   //工位1升降目标层
        public int b_O_Task2;      //工位1目标位置伸叉方向[1：左近，2：右近][3：左远，4：右远]
        public int b_O_TravelPos3; // 工位2行走起始列
        public int b_O_LiftPos3;   // 工位2升降起始层
        public int b_O_Task3;      //工位2起始位置伸叉方向[1：左近，2：右近][3：左远，4：右远]
        public int b_O_TravelPos4; //工位2行走目标列
        public int b_O_LiftPos4;   // 工位2升降目标层
        public int b_O_Task4;      //工位2目标位置伸叉方向[1：左近，2：右近][3：左远，4：右远]
        public int b_O_TaskId1;    //工位1任务 ID
        public int b_O_TaskId2;    //工位2任务 ID
        public int b_O_New_Task1;  //TravelPos1/ LiftPos1/ Task1/ TravelPos2/ LiftPos2/ Task2以上6个变量值设置好后将该变量置=1 堆垛机开始执行工位1任务；=2 堆垛机移动到目标位置（工位1）
        public int b_O_New_Task2;  //TravelPos3/ LiftPos3/ Task3/ TravelPos4/ LiftPos4/ Task4以上6个变量值设置好后将该变量置=1 堆垛机开始执行工位2任务；=2 堆垛机移动到目标位置（工位2）
        public int b_O_Open_Tank;  //=0默认值，当WCS写入0时关闭干冰储罐电磁阀；=1开启干冰储罐电磁阀
        public int b_O_CompleteTask;//WCS收到任务完成置1，SRM把此位情0，同时任务完成b_I_Task_Finish1清0
        #endregion

        #region  WCS读取信息区域
        public int b_I_Auto;        //堆垛机当前运行模式指示1：自动模式 //T 0：非自动模式//F
        public int B_I_State;       //0：空闲，无任务1：等待，有任务，因安全原因暂停，条件许可即可恢复执行 2：定位3：工位1取货 4：工位1放货 5：工位2取货 6：工位2放货 98：维修
        public int b_I_HandShake;  //WCS与堆垛机握手信号，堆垛机每个周期将b_O_HandShake的值写入b_I_HandShake，3秒内握手信号未发生改变认为通讯中断
        public int b_I_Get_Finish1; //堆垛机完成取货信号（工位1）
        public int b_I_Put_Finish1;//堆垛机完成放货信号（工位1）
        public int b_I_Get_Finish2;//堆垛机完成取货信号（工位2）
        public int b_I_Put_Finish2;//堆垛机完成放货信号（工位2）
        public int b_I_Alarm;      //堆垛机报警状态指示
        public int b_I_Task_Finish1;//堆垛机工位1任务完成
        public int b_I_Task_Finish2;//堆垛机工位2任务完成
        public int b_I_Loaded1;     //货叉1有货物
        public int b_I_Loaded2;     //货叉2有货物
        public int b_I_Fork_Zero1; //货叉1处于原点位置
        public int b_I_Fork_Zero2; //货叉2处于原点位置
        public int b_I_SRM_Num;    //堆垛机号 考虑有一轨双车、或有调度柜时WCS需要明确知道具体某台堆垛机的信息，所以调度柜需要反馈该信息给WCS
        public int b_I_Alarm_Code; //堆垛机报警代码
        public int B_I_Row;        //堆垛机当前列（以货叉1为准）
        public int B_I_Layer;      //堆垛机当前层
        public int nTravelPos;     //堆垛机当前行走位置，单位mm
        public int nLiftPos;       //堆垛机当前升降位置，单位mm
        public int nForkPos_Single1; //堆垛机货叉1位置，单位mm货叉向左位置为正值，货叉向右位置为负值
        public int nForkPos_Single2; //堆垛机货叉2位置，单位mm货叉向左位置为正值，货叉向右位置为负值
        public int B_I_TaskId1;      //工位1任务号
        public int B_I_TaskId2;      //工位2任务号 
        public int B_I_Is_TankOpened; //0默认值 =1干冰储罐阀门已打开
        public int B_Mode;//1 手动 2 自动；
        #endregion

        #region WCS显示信息

        public int operationMethod;	//0：空闲，无任务1：等待，有任务，因安全原因暂停，条件许可即可恢复执行 2：定位3：工位1取货 4：工位1放货 5：工位2取货 6：工位2放货 98：维修

        public int equipmentStatus;	//	设备状态	0.空闲；1.正在执行；其他编号见故障代码表

        public int fork1_taskno;	//货叉1正在执行任务号	下发任务时为0
        public int fork2_taskno;//货叉2正在执行任务号	下发任务时为0
        public int forkStatus;	//	货叉状态	0都无货 1仅货叉1有货 2仅货叉2有货；3货叉都有货

        public int task1_status;	//	任务1执行阶段	任务执行的阶段1取货行走；2取货；3放货行走，4放货；5完成
        public int task2_status;	//	任务2执行阶段	任务执行的阶段1取货行走；2取货；3放货行走，4放货；5完成

        public int currentAddress1_line;//	当前排	货叉1的位置
        public int currentAddress1_column;//	当前列	货叉1的位置
        public int currentAddress1_layer;//	当前层	货叉1的位置
        public int targetAddress_line;	//	当前目标排	
        public int targetAddress_column;	//	当前目标列	
        public int targetAddress_layer;	//	当前目标层	
        public int currentTaskFork;	//	当前任务货叉	1:1号货叉；2:2号货叉
        public int sendEnable;	//	允许下发	1允许, 0 不允许
        public int pulse;	//	心跳	0.5秒的1 0.5秒的0


        public bool cChange = false;

        public string LabelInfo;//故障信息
        public string OldLabelInfo;//之前故障信息

        public int sourceAddress1_line;    //源排号1	
        public int sourceAddress1_column;  //源列号1	
        public int sourceAddress1_layer;  //源层号1	
        public int targetAddress1_line; //目标排号1	
        public int targetAddress1_column; //目标列号1	
        public int targetAddress1_layer; // 目标层号1	

        public int sourceAddress2_line; //源排号2	
        public int sourceAddress2_column; //源列号2	
        public int sourceAddress2_layer; //源层号2	
        public int targetAddress2_line; //目标排号2	
        public int targetAddress2_column; //目标列号2	
        public int targetAddress2_layer; //目标层号2	

        public int sendFlag;//指令下达完成标志	1启动下发的任务（PLc读取后改为0）


        public int WCSControlMode = 1;//1自动，2.手动
        public int wcstaskno;
        public string WCSBarCode;

        public SCTaskMode WCSTaskMode = SCTaskMode.All;//任务模式

        public string WCSFromstation = "";
        public string WCSToStation = "";

        public SCTaskType WCSTaskType = SCTaskType.None;

        public string WCSPauseCause = string.Empty;

        public List<SCStationInfo> lstStation = new List<SCStationInfo>();

        #endregion

    }
}
