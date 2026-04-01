using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;
using System.Data;
using CWCS.Entity;

namespace CWCS.Core.Library.AGVManager
{
    public delegate DataSet Get_AgvOrders(bool AgvIsBusy);
    public delegate bool Judge_Agv_IsBusy(bool AgvIsBusy);
    public delegate Int16 Change_TaskPor(bool AgvIsBusy);
    public delegate string Get_TaskAGVminfo(string agvindex);
     public delegate void ShowConnect(string type, string msg);
    

    public delegate void UpdateAGVStatus(string Command, object state);
    public class AGVControl :AGVHelper
    {
        private volatile bool _canStop = false;
        private volatile bool _Manually = true;
        private string _ConnectMsg = string.Empty;
        private string _ServerAddr = string.Empty;
        private int _ServerPort = 0;
        public bool _AGVIsBusy = false;
        private SocketManager.SocketClient nt8000 = null;
        public event UpdateAGVStatus Notify;
        public event ShowConnect ShowConn;
       

        public event Get_AgvOrders Get_Agv_Data;
        public event Judge_Agv_IsBusy JudgeIsBusy;
        public event Change_TaskPor ChangeAgvTackPor;
        private System.Timers.Timer HeartBeatTimer = new System.Timers.Timer();



        
        #region	//类的构造函数
        public AGVControl(string addr,int port)
        {
            _ServerAddr = addr;
            _ServerPort = port;
            HeartBeatTimer.Interval = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CheckConnectTimer"]);
            HeartBeatTimer.Elapsed += new ElapsedEventHandler(HeartBeatTimer_Elapsed);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion


        #region	//初始化函数
        public bool Run()
        {
            nt8000 = new SocketManager.SocketClient();
            nt8000.Notify += new SocketManager.NotifyCommandHander(nt8000_Notify);
            bool Connected = nt8000.Connect(_ServerAddr, _ServerPort);
            if (Connected)
            {
                HeartBeatTimer.Start();
                WaitCallback wgetTask = new WaitCallback(GetTask);
                WaitCallback wchangePrior = new WaitCallback(ChangePriority);
                ThreadPool.QueueUserWorkItem(wgetTask, true);
                ThreadPool.QueueUserWorkItem(wchangePrior, true);
                //GetTask(true);
                return true;
            }
            else
            {
                return false;
            }
        }

        //心跳监测
        private void HeartBeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            byte[] beat = new byte[1] { 0xFF };
            if (!nt8000.Send(beat))
            {
                QueueSpeech.AddQueue(string.Format("与NT8000连接断开,系统开始尝试重连"));
                Stop();
                if (nt8000.Connect(_ServerAddr, _ServerPort)) QueueSpeech.AddQueue(string.Format("与NT8000重连成功"));
            }
            else if (_canStop)
            {
                _canStop = false;
            }
            if(!_canStop)
            {
                if (ShowConn != null)
                    ShowConn("Connect", "");
            }
        }

        public void Stop()
        {
            _canStop = true;
            if (nt8000 != null) nt8000.Close();
        }

        public void Manual()
        {
            if (_Manually) _Manually = false;
            else _Manually = true;
        }

        //轮询可执行任务，下发NT8000
        private void GetTask(object AGVIsBusy)
        {
            while (true)
            {
                if (!_canStop && !_Manually)
                {
                    try
                    {
                        DataSet orderList = this.Get_Agv_Data((bool)AGVIsBusy);
                        if (orderList != null && orderList.Tables.Count > 0 && orderList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < orderList.Tables[0].Rows.Count; i++)
                            {
                                LGVOrderPara p;
                                p.ikey = Int32.Parse(orderList.Tables[0].Rows[i]["AGVikey"].ToString());
                                p.fromStation = Int32.Parse(orderList.Tables[0].Rows[i]["fromStation"].ToString());
                                p.toStation = Int32.Parse(orderList.Tables[0].Rows[i]["midStation"].ToString());

                                byte[] msg = new byte[14];
                                msg[0] = 0x00;
                                msg[1] = (byte)'q';
                                msg[2] = 0x00;
                                msg[3] = 0x0a;
                                msg[4] = 0x01;
                                msg[5] = 0x80;
                                msg[6] = 0x00;
                                msg[7] = 0x01;
                                Array.Copy(GetHexBytes(p.ikey), 0, msg, 8, 2);
                                Array.Copy(GetHexBytes(p.fromStation), 0, msg, 10, 2);
                                Array.Copy(GetHexBytes(p.toStation), 0, msg, 12, 2);
                                SendMsg(PackMsg(1, msg));
                                string str = "任务下发,WCS->NT8000(q-Message),ikey:" + p.ikey.ToString() + "fromStation:" + p.fromStation.ToString() + "toStation:" + p.toStation.ToString();
                                ShowInfo("remindInfo", str);
                                Thread.Sleep(50);
                            }
                            orderList.Clear();
                            orderList = null;
                            GC.Collect();
                        }
                    }
                    catch (Exception ee)
                    {
                        //ScanDbTimer.Enabled = true;
                    }
                }
                Thread.Sleep(3000);
            }
        }

        private void ChangePriority(object state)
        {
            while (true)
            {
                if (!_canStop)
                {
                    this.ChangeAgvTackPor(true);
                }
                Thread.Sleep(3000);
            }
        }

        public void SendTask(Int32 AGVikey, Int32 fromstation, Int32 tostation)
        {
            LGVOrderPara p;
            p.ikey = AGVikey;
            p.fromStation = fromstation;
            p.toStation = tostation;
            byte[] msg = new byte[14];
            msg[0] = 0x00;
            msg[1] = (byte)'q';
            msg[2] = 0x00;
            msg[3] = 0x0a;
            msg[4] = 0x01;
            msg[5] = 0x80;
            msg[6] = 0x00;
            msg[7] = 0x01;
            Array.Copy(GetHexBytes(p.ikey), 0, msg, 8, 2);
            Array.Copy(GetHexBytes(p.fromStation), 0, msg, 10, 2);
            Array.Copy(GetHexBytes(p.toStation), 0, msg, 12, 2);
            SendMsg(PackMsg(1, msg));
            string str = "任务下发,WCS->NT8000(q-Message),ikey:" + p.ikey.ToString() + "fromStation:" + p.fromStation.ToString() + "toStation:" + p.toStation.ToString();
            ShowInfo("remindInfo", str);
        }

        private void JudgeIsBusy_Tick(object sender, System.EventArgs e)
        {
            //判断AGC任务是否忙
            bool b = this.JudgeIsBusy(_AGVIsBusy);
            if (!_AGVIsBusy)
            {
                if (b)
                {
                    byte[] msg = new byte[10];
                    msg[0] = 0x00;
                    msg[1] = (byte)'q';
                    msg[2] = 0x00;
                    msg[3] = 0x06;
                    msg[4] = 0x32;
                    msg[5] = 0x80;
                    msg[6] = 0x00;
                    msg[7] = 0x01;
                    msg[8] = (byte)'q';
                    msg[9] = (byte)'b';
                    SendMsg(PackMsg(1, msg));
                    Thread.Sleep(50);
                    _AGVIsBusy = true;
                }
            }
            else
            {
                if (!b)
                {
                    byte[] msg = new byte[10];
                    msg[0] = 0x00;
                    msg[1] = (byte)'q';
                    msg[2] = 0x00;
                    msg[3] = 0x06;
                    msg[4] = 0x33;
                    msg[5] = 0x80;
                    msg[6] = 0x00;
                    msg[7] = 0x01;
                    msg[8] = (byte)'q';
                    msg[9] = (byte)'b' + 1;
                    SendMsg(PackMsg(1, msg));
                    Thread.Sleep(50);
                    _AGVIsBusy = false;
                }
            }
        }
        #endregion


        #region //封包解包及处理信号函数

        public void nt8000_Notify(NotifyCommand Command, object Data)
		{
			switch(Command)
			{
				case NotifyCommand.Connect:
                    ShowInfo("Connect", (string)Data);
					break;
				case NotifyCommand.Break:
					Stop();
                    ShowInfo("Break", (string)Data);
					break;
				case NotifyCommand.SendData:
					DoSentData((byte[])Data);
					break;
				case NotifyCommand.RecvData:
					DoRecvData((byte[])Data);
					break;
			}
		}
		
		private byte[] PackMsg(int functionCode, byte[] Msg)
		{
			//封装信息
			byte[] Buffer = new byte[8+Msg.Length];
			Buffer[0] = 0x87;
			Buffer[1] = 0xcd;
			Buffer[2] = 0x00;
			Buffer[3] = 0x08;
			Buffer[4] = GetHexBytes(Msg.Length)[0];
			Buffer[5] = GetHexBytes(Msg.Length)[1];
			Buffer[6] = GetHexBytes(functionCode)[0];
			Buffer[7] = GetHexBytes(functionCode)[1];
			Array.Copy(Msg,0,Buffer,8,Msg.Length);
			return Buffer;
		}

		private byte[] GetHexBytes(int i)
		{
			//将一个整数转化为长度为2的byte型数组
			byte[] Buffer = new byte[2];
			Buffer[0] = (byte)(i/256);
			Buffer[1] = (byte)(i%256);
			return Buffer;
		}

        private void ShowInfo(string type, string msg)
        {
            if (ShowConn != null)
                ShowConn(type, msg);
        }

		private void DoSentData(byte[] data)
		{
			//将非网络检测数据记录到日志
			if (data[7]==0x01) 
			{
				byte[] msg = new byte[data.Length-8];
				Array.Copy(data,8,msg,0,msg.Length);
				//WriteLog(true,msg);
			}
		}

		private void DoRecvData(byte[] data)
		{
			//处理接收成功的数据
			byte[][] b;
			b = SplitMsgFromRecvData(data);
			for(int i=0; i<b.GetLength(0); i++)
			{
				if (b[i]!=null)
				{
					HandleRecvMsgRecord(b[i]);
				}
			}
		}
			
		private byte[][] SplitMsgFromRecvData(byte[] data)
		{
			//从接收缓存中分离出指令信息
			byte[][] Buffer = new byte[9][];
			try
			{
				int i=0;
				int j=0;
				while(i<data.Length)
				{
					if ((int)data[i]==0x87)
					{
						if ((int)data[++i]==0xcd)
						{
							if ((data[++i]*256+data[++i])==8)
							{
								int k=0;
								k = 8 + data[i+1]*256 + data[i+2];
								Buffer[j] = new byte[k];
								Array.Copy(data,i-3,Buffer[j],0,k);
								i += k-3;
								j++;
								if (j>=9) break;
							}
							else
							{
								++i;
								continue;
							}
						}
						else
						{
							++i;
							continue;
						}
					} 
					else
					{
						++i;
						continue;
					}
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine("LGV控制模块SplitMsgFromRecvData函数发生异常:"+ex.Message);
			}
			return Buffer;
		}
		
		private void HandleRecvMsgRecord(byte[] data)
		{
			//处理分离出的信息记录
			try
			{
				byte[] Buffer;
				switch ((int)data[7])
				{
						//Normal Message
					case 1:
					{
						Buffer = new byte[data.Length-8];
						Array.Copy(data, 8, Buffer, 0, data.Length-8);
						HandleRecvMsg(Buffer);
						break;
					}
					//Disconnect Link
					case 2:
					{
						break;
					}
					//Heart-Beat Poll
					case 4:
					{	
						SendMsg(PackMsg(5,new Byte[0]));
						break;
					}
						//Heart-Beat Ack
					case 5:
					{
						break;
					}
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine("LGV控制模块HandleRecvMsgRecord函数发生异常:"+ex.Message);
			}
		}

		private void HandleRecvMsg(byte[] msg)
		{
            if (Notify != null)
            {
                char ch = (char)msg[1];
                Notify(ch.ToString(), msg);
            }
		}

        private bool SendMsg(byte[] buffer)
        {
          return  nt8000.Send(buffer);
        }

        public bool SendmMessage(byte[] msg)
        {
            return nt8000.Send(PackMsg(1, msg));
        }

        private void DoMasterColdStart()
        {
            //重新设定NT8000 Virtual Input
            byte[] sMsg = new byte[10];
            for (int i = 0; i <= 3; i++)
            {
                sMsg[0] = 0x00;
                sMsg[1] = (byte)'K';
                sMsg[2] = 0x00;
                sMsg[3] = 0x06;
                sMsg[4] = 0x00;
                sMsg[5] = 0x06;
                sMsg[6] = 0x00;
                sMsg[7] = (byte)i;
                sMsg[8] = 0x00;
                //sMsg[8] = GetConveyerStatusByNo(i);
                sMsg[9] = 0x00;
                SendMsg(PackMsg(1, sMsg));
            }
            //查询NT8000 Virtual Output
            for (int i = 0; i <= 3; i++)
            {
                sMsg[0] = 0x00;
                sMsg[1] = (byte)'W';
                sMsg[2] = 0x00;
                sMsg[3] = 0x06;
                sMsg[4] = 0xFF;
                sMsg[5] = 0xFF;
                sMsg[6] = 0x01;
                sMsg[7] = 0x06;
                sMsg[8] = 0x00;
                sMsg[9] = (byte)i;
                SendMsg(PackMsg(1, sMsg));
            }
        }
        #endregion

    }
}
