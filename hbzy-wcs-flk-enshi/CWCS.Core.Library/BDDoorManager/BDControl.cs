using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;
using System.Data;
using CWCS.Entity;
using CWCS.Core.Library.BDManager;
using System.Net.Sockets;
using static Invengo.NetAPI.Protocol.IRP1.FrequencyTable;
using Invengo.NetAPI.Communication.USBLibrary;
using System.Linq;
using System.Runtime.InteropServices;
using Invengo.NetAPI.Core;

namespace CWCS.Core.Library.AGVManager
{
    public class BDControl
    {
        public static List<WorkStation> workStations = new List<WorkStation>();

        private int _ServerPort = 0;
        public string _ServerAddr = string.Empty;
        public string Name { get; private set; }
        public bool? ConnectStatus { get { return _BDClient?.Connected; } }
        public BDPackServic packService;
        public List<WorkStation> workStation;
        private SocketManager.SocketClient _BDClient = null;

        public event Action<string, NotifyCommand, string> ShowConn;

        /// <summary>
        /// 收到门状态
        /// </summary>
        public event Action<string, byte> OnConfirmCMD;
        public event Action<string, byte, byte> OnRecDoorStatus;
        private System.Timers.Timer HeartBeatTimer;

        #region  //类的构造函数         
        public BDControl(string addr, int port, string name = "default")
        {
            _ServerAddr = addr;
            _ServerPort = port;
            Name = name;
            packService = new BDPackServic();
            HeartBeatTimer = new System.Timers.Timer();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
        /// <summary>
        /// 门的状态  Action（门）
        /// </summary>
        Dictionary<byte, Action<byte>> handler;

        #region	//初始化函数
        public bool Run()
        {
            handler = new Dictionary<byte, Action<byte>>
            {
                { BDDoorStatus.InterfaceReset,
                    (door)=>
                    {
                        workStation.FirstOrDefault(c=>c.WorkStationCode==door).DoorStatus=0; 
                    } 
                },
                { BDDoorStatus.DoorOpenWhenPut,
                    (door)=>
                    {
                        workStation.FirstOrDefault(c => c.WorkStationCode == door).DoorStatus = BDDoorStatus.DoorOpenWhenPut;
                    } 
                },
                { 
                    BDDoorStatus.DoorOpenWhenGet,
                    (door)=>
                    {
                        workStation.FirstOrDefault(c => c.WorkStationCode == door).DoorStatus = BDDoorStatus.DoorOpenWhenGet; 
                    }
                },
            };
            _BDClient = new SocketManager.SocketClient();
            _BDClient.Notify += new SocketManager.NotifyCommandHander(BD_Notify);
            bool Connected = _BDClient.Connect(_ServerAddr, _ServerPort);
            if (Connected)
            {
                HeartBeatTimer.Interval = 10000;
                HeartBeatTimer.Elapsed += new ElapsedEventHandler(HeartBeatTimer_Elapsed);
                return true;
            }
            else
            {
                return false;
            }
        }

        //心跳监测
        private void HeartBeatTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            var result = packService.PackSendData(BDOPerationType.HeartBeat);
            if (result.IsSuccess)
            {
                //_logger.LogInformation($"WCS->BD:发送请求状态{BitConverter.ToString(result.Content)}");
                if (!_BDClient.Send(result.Content))
                {
                    Reconnect("与BD连接断开,系统开始尝试重连");
                    return;
                }
            }
            else
            {
                BD_Notify(NotifyCommand.Break, "");
            }
            //发送获取门状态指令
            var door1Res = packService.SendBDbufferInfo(BDDoorType.TP);
            _BDClient.Send(door1Res.Content);
            var door2Res = packService.SendBDbufferInfo(BDDoorType.CP);
            _BDClient.Send(door2Res.Content);
        }
        private void Reconnect(string msg)
        {
            QueueSpeech.AddQueue(msg);
            if (_BDClient.Connect(_ServerAddr, _ServerPort)) QueueSpeech.AddQueue(string.Format("与BD重连成功"));
        }

        /// <summary>
        /// 初始化BD连接
        /// </summary>
        /// <param name="obj"></param>
        private void CreateConnect()
        {
            //初始化连接；
            var result = packService.PackSendData(BDOPerationType.InitConnect, 1);
            if (result.IsSuccess)
            {
                //_logger.LogInformation($"WCS->BD:初始化连接{BitConverter.ToString(result.Content)}");
                _BDClient.Send(result.Content);
            }
            Thread.Sleep(200);
            ///WMStoBDOperational
            result = packService.PackSendData(BDOPerationType.WMSToBDOpera, 1);
            if (result.IsSuccess)
            {
                //_logger.LogInformation($"WCS->BD:WMStoBDOperational{BitConverter.ToString(result.Content)}");
                _BDClient.Send(result.Content);
            }
            Thread.Sleep(200);
            //心跳
            result = packService.PackSendData(BDOPerationType.HeartBeat);
            if (result.IsSuccess)
            {
                //_logger.LogInformation($"WCS->BD:心跳：{BitConverter.ToString(result.Content)}");
                _BDClient.Send(result.Content);
            }
            Thread.Sleep(200);
            //查询工位1 状态
            result = packService.PackSendData(BDOPerationType.ResetInterface1, 99);
            if (result.IsSuccess)
            {
                //_logger.LogInformation($"WCS->BD:查询工位1状态{BitConverter.ToString(result.Content)}");
                _BDClient.Send(result.Content);
            }
            Thread.Sleep(200);
            //查询工位2状态
            result = packService.PackSendData(BDOPerationType.ResetInterface2, 99);
            if (result.IsSuccess)
            {
                //_logger.LogInformation($"WCS->BD:查询工位2状态{BitConverter.ToString(result.Content)}");
                _BDClient.Send(result.Content);
            }
        }
        #endregion
        /// <summary>
        /// BD控制
        /// checkStatus:空则说明是关门，非空则是开门，需要检查是否已经开启。20 送货时，BD门已经打开，AGV可以放托盘  21取货时，BD门已经打开，AGV可以取走托盘
        /// wcsCmd："10", "AGV准备放货,WCS请求BD开门"   "11", "AGV准备取货,WCS请求BD开门"  "24", "AGV放货完成,WCS请求BD关门" "25", "AGV取货完成,WCS请求BD关门"
        /// 命令类型30 command1  31 command2  32 acknowledge1  33 acknowledge2
        ///</summary>

        public static OperateResult<bool> BdOpenControl(WorkStation workstation, byte wcsCmd, byte? expectStatus = null)
        {
            //"13", "AGV准备放货,WCS请求BD开门"  "25", "AGV取货完成,WCS请求BD关门"   "12", "AGV准备取货,WCS请求BD开门"  "24", "AGV放货完成,WCS请求BD关门"
            //门的状态 20 送货时，BD门已经打开，AGV可以放托盘  21取货时，BD门已经打开，AGV可以取走托盘

            if (expectStatus is not null)//进入门有期望状态
            {
                if (workstation.DoorStatus == expectStatus.Value)
                {
                    return OperateResult.SuccessResult(true);
                }
                else
                {
                    var reposereuslt = workstation.Context.packService.PackSendData(BDSendCMD.SendCMD[workstation.WorkStationCode], wcsCmd);
                    var res = workstation.Context._BDClient.Send(reposereuslt.Content);
                    return OperateResult.CreateFailResult<bool>($"{workstation.DeviceCode}进门时，门状态{workstation.DoorStatus}，期望状态{expectStatus.Value}，已发送开门指令");
                }
            }
            else
            {
                //关门 可以直接发
                var reposereuslt = workstation.Context.packService.PackSendData(BDSendCMD.SendCMD[workstation.WorkStationCode], wcsCmd);
                var res = workstation.Context._BDClient.Send(reposereuslt.Content);
                return OperateResult.SuccessResult(true);
            }
        }

        #region //封包解包及处理信号函数

        public void BD_Notify(NotifyCommand Command, object Data)
        {
            switch (Command)
            {
                case NotifyCommand.Connect:
                    CreateConnect();
                    HeartBeatTimer.Start();
                    ShowInfo(NotifyCommand.Connect, (string)Data);

                    //var workStation7002 = BDControl.workStations.FirstOrDefault(c => c.DeviceCode == "9132");
                    //BDControl.BdOpenControl(workStation7002, BDOPerationType.WMSOpenDoorBeforPut, BDDoorStatus.DoorOpenWhenGet);

                    break;
                case NotifyCommand.Break://断开要恢复门状态
                    workStation.ForEach(c => c.DoorStatus = 0);
                    ShowInfo(NotifyCommand.Break, (string)Data);
                    break;
                case NotifyCommand.SendData:
                    DoSentData((byte[])Data);
                    break;
                case NotifyCommand.RecvData:
                    DoRecvData((byte[])Data);
                    break;
            }
        }

        private void ShowInfo(NotifyCommand type, string msg)
        {
            ShowConn?.Invoke(_ServerAddr, type, msg);
        }

        private void DoSentData(byte[] data)
        {
            //将非网络检测数据记录到日志
            if (data[7] == 0x01)
            {
                byte[] msg = new byte[data.Length - 8];
                Array.Copy(data, 8, msg, 0, msg.Length);
                //WriteLog(true,msg);
            }
        }

        /// <summary>
        /// 处理接收到的数据
        //AGV取货：
        //命令：11 BD退料请求
        //      12 BD更改请求 
        //      25 BD取货完成关门
        //状态：21取货时，BD门已经打开，AGV可以取走托盘  
        //AGV放货：
        //命令：10 AGV进入请求 
        ///     24 BD放货完成关门
        //状态：20 BD允许AGV放货 
        /// </summary>
        /// <param name="data"></param>
        private void DoRecvData(byte[] data)
        {
            int len = int.Parse(data[0].ToString());

            switch (len)
            {
                case 3://心跳
                    //收到初始化连接确认及心跳不用管
                    ShowConn?.Invoke(_ServerAddr, NotifyCommand.Connect, "");
                    break;
                case 7://收到BD状态或确认反馈
                    var msgType = data[2];
                    var result = data[3];

                    var ackBytes = packService.PackSendData(BDRecCMD.ReplyCMD[msgType], result);
                    if (ackBytes.IsSuccess)
                    {
                        if (handler.TryGetValue(result, out Action<byte> act)) act.Invoke(msgType);
                        _BDClient.Send(ackBytes.Content);
                        try
                        {
                            OnRecDoorStatus?.Invoke(_ServerAddr, msgType, result);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                        try
                        {
                            OnConfirmCMD?.Invoke(_ServerAddr, result);
                        }
                        catch (Exception)
                        {

                        }
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

    }
}
