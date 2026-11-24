using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Timers;
using COMRW;
using CWCS.Common.Log;
using CWCS.Entity;
using CWCS.Repository;

namespace COMResolution
{
    public delegate void UpdateAGVStatus(string Command, object state);

    public class CRLAgvControl
    {
        private RWLSocketClient nt8000 = new RWLSocketClient();
        private Timer HeartBeatTimer = new Timer();
        private string ServerAddr = "";
        private int ServerPort = 0;
        private string AgvExceptionDirectory => $"AGV�쳣��־";
        public OracleRepository _repository = new OracleRepository();
        public string AgvNo = string.Empty;

        public event UpdateAGVStatus Notify;


        public CRLAgvControl(string Ip,string Port)
        {
            ServerAddr = Ip;
            ServerPort = Convert.ToInt32(Port);

            nt8000.Notify += new NotifyCommandHander(nt8000_Notify);

            HeartBeatTimer.Interval = Convert.ToDouble(StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Equals("AgvConnectionStatusInterval")).ToList()[0].PARAM_VALUE) * 1000;
            HeartBeatTimer.Elapsed += new ElapsedEventHandler(HeartBeatTimer_Elapsed);
        }

        public void Run()
        {
            if (!nt8000.Connect(ServerAddr, ServerPort))
                HeartBeatTimer.Start();
            else
                Notify("Connect", null);
        }

        public bool ReStart()
        {
            if (!nt8000.Connected)
                Run();
            if (nt8000.Connected)
                return true;
            return false;
        }

        private void HeartBeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (nt8000.Connect(ServerAddr, ServerPort))
                HeartBeatTimer.Stop();
        }

        private void nt8000_Notify(string command, object data)
        {
            if (command == "RecvData")
            {
                List<byte[]> list = SplitAgvMessage((byte[])data);
                if (list != null && list.Count > 0)
                {
                    foreach (byte[] item in list)
                    {
                        HandleRecvMsgRecord(item);
                    }
                }
            }
            else
            {
                if (Notify != null)
                {
                    if (command == "Break")
                        HeartBeatTimer.Start();
                    Notify(command, data);
                }
            }
        }

        private void HandleRecvMsgRecord(byte[] data)
        {
            //�������������Ϣ��¼
            try
            {
                byte[] Buffer;
                switch ((int)data[7])
                {
                    //Normal Message
                    case 1:
                        {
                            Buffer = new byte[data.Length - 8];
                            Array.Copy(data, 8, Buffer, 0, data.Length - 8);
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
                            nt8000.Send(PackMsg(5, new Byte[0]));
                            break;
                        }
                    //Heart-Beat Ack
                    case 5:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Write(AgvExceptionDirectory, AgvExceptionDirectory, $"����HandleRecvMsgRecord�쳣��{ex.Message}");
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

        public bool SendRMessageStaNo(int id, int status)
        {
            byte[] retmsg = new byte[10];
            retmsg[0] = 0x00;
            retmsg[1] = (byte)'K';
            retmsg[2] = 0x00;
            retmsg[3] = 0x06;
            retmsg[4] = 0x00;
            retmsg[5] = 0x02;
            retmsg[6] = 0x00;
            retmsg[7] = (byte)id;
            retmsg[8] = (byte)status;
            retmsg[9] = 0x00;
            if (nt8000.Connected)
                return nt8000.Send(PackMsg(1, retmsg));
            return false;


        }

        /// <summary>
        /// AGV�������Ľ���
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<byte[]> SplitAgvMessage(byte[] data)
        {
            List<byte[]> list = new List<byte[]>();
            byte[] item;
            int i = 0;
            int arrayLength;
            int indexTemp;
            try
            {
                while (i < data.Length)
                {
                    if (data[i] != 0x87)
                    {
                        ++i;
                        continue;
                    }
                    if (data[++i] != 0xcd)
                    {
                        ++i;
                        continue;
                    }
                    indexTemp = i + 4;
                    if (data[indexTemp] == 0x36)
                    {
                        continue;
                    }
                    if (data[++i] * 256 + data[++i] != 8)
                    {
                        ++i;
                        continue;
                    }
                    arrayLength = 8 + data[i + 1] * 256 + data[i + 2];
                    item = new byte[arrayLength];
                    Array.Copy(data, i - 3, item, 0, arrayLength);
                    list.Add(item);
                    i = i - 3 + arrayLength;
                }
            }
            catch (Exception ex)
            {
                StringBuilder writeStr = new StringBuilder();
                if (data != null && data.Length > 0)
                {
                    foreach (byte b in data)
                    {
                        writeStr.Append(b.ToString("X2"));
                    }
                }
                LogUtil.Write(AgvExceptionDirectory, AgvExceptionDirectory, $"����SplitAgvMessage�쳣��{ex.Message + ex.StackTrace}��AGV������Ϣ:{writeStr}");
            }
            return list;
        }

        /// <summary>
        /// AGV�������Ľ���
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[][] SplitMsgFromRecvData(byte[] data)
        {
            //�ӽ��ջ����з����ָ����Ϣ
            byte[][] Buffer = new byte[9][];
            int indexTemp = 0;
            try
            {
                int i = 0;
                int j = 0;
                while (i < data.Length)
                {
                    if ((int)data[i] == 0x87)
                    {
                        if ((int)data[++i] == 0xcd)
                        {
                            indexTemp = i + 4;
                            if (data[indexTemp] != 0x36 && data[++i] * 256 + data[++i] == 8)
                            {
                                int k = 0;
                                k = 8 + data[i + 1] * 256 + data[i + 2];
                                Buffer[j] = new byte[k];
                                Array.Copy(data, i - 3, Buffer[j], 0, k);
                                i += k - 3;
                                j++;
                                if (j >= 9) break;
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
            catch (Exception ex)
            {
                StringBuilder writeStr = new StringBuilder();
                if (data != null && data.Length > 0)
                {
                    foreach (byte b in data)
                    {
                        writeStr.Append(b.ToString("X2"));
                    }
                }
                LogUtil.Write(AgvExceptionDirectory, AgvExceptionDirectory, $"����SplitMsgFromRecvData�쳣��{ex.Message + ex.StackTrace}��AGV������Ϣ:{writeStr}");
            }
            return Buffer;
        }

        public bool SendTask(int taskno, int fromstation, int tostation, string tasktype)
        {
            byte[] msg = new byte[14];
            msg[0] = 0x00;
            msg[1] = (byte)'q';
            msg[2] = 0x00;
            msg[3] = 0x0a;
            msg[4] = 0x01;
            //msg[5] = 0x81;
            msg[5] = 0x84;
            byte ylPriorityFloor02 = 0x84;
            byte rkPriorityFloor02 = 0x81;
            DataTable dt = new DataTable();
            if (StaticCommon.WcsParamConfigList.Exists(x => x.PARAM_CODE.Equals("YLPriorityFloor02")))
            {
                ylPriorityFloor02 = WcsToAgvPriority(StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Equals("YLPriorityFloor02")).ToList()[0].PARAM_VALUE);
            }
            if (StaticCommon.WcsParamConfigList.Exists(x => x.PARAM_CODE.Equals("RKPriorityFloor02")))
            {
                rkPriorityFloor02 = WcsToAgvPriority(StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Equals("RKPriorityFloor02")).ToList()[0].PARAM_VALUE);
            }
            //���ȼ���ʶ.ֵԽ��,���ȼ�Խ��.
            //�����ʼ��ַ��ת��ܡ����ȼ�����Ϊ9.
            if (fromstation.Equals(1) || fromstation.Equals(2) || fromstation.Equals(3))
            {
                msg[5] = 0x89;
            }
            if (fromstation.Equals(1))
            {
                msg[5] = 0x88;
            }
            //Ҫ�����ȼ�����4
            if (fromstation.Equals(5650) || fromstation.Equals(5653))
            {
                msg[5] = 0x84;
            }
            //��¥����վ̨���ȼ�
            if (tostation.ToString().Equals("7777"))
            {
                msg[5] = 0x83;
            }
            //�����Ҫ��,���ȼ�����Ϊ3
            if (fromstation.ToString().StartsWith("60") || tostation.ToString().StartsWith("60"))
            {
                //msg[5] = 0x83;
                msg[5] = 0x84;
            }
            //Ŀ���ַ��ת���,���ȼ�����Ϊ2
            if (tostation.Equals(1) || tostation.Equals(2) || tostation.Equals(3))
            {
                msg[5] = ylPriorityFloor02;
            }
            //���ƷҪ��վ̨
            if (tostation.Equals(5560) || tostation.Equals(5561))
            {
                msg[5] = ylPriorityFloor02;
            }
            if (fromstation.Equals(5722))
            {
                msg[5] = rkPriorityFloor02;
            }
            //���̿����������
            if (fromstation.Equals(5665))
            {
                msg[5] = 0x82;
            }
            //��¥�����̳���
            if (tostation.Equals(5722) || tostation.Equals(7777))
            {
                dt = _repository.QueryTableData(new WCS_TASKINFO() 
                { 
                    TASKNO = taskno
                });
                if (dt != null  && dt.Rows.Count == 1 && dt.Rows[0]["BARCODE"].ToString().Contains("999999"))
                {
                    msg[5] = 0x83;
                }
            }
            msg[6] = 0x00;
            msg[7] = 0x01;
            Array.Copy(GetHexBytes(taskno), 0, msg, 8, 2);
            Array.Copy(GetHexBytes(fromstation), 0, msg, 10, 2);
            Array.Copy(GetHexBytes(tostation), 0, msg, 12, 2);
            if (nt8000.Connected)
            {
                return nt8000.Send(PackMsg(1, msg));
            }
            else
            {
                try
                {
                    nt8000.RaiseNotifyEvent("Break", " �����жϣ�");
                }
                catch
                {
                    return false;
                }
                return false;
            }
            
        }

        public byte WcsToAgvPriority(string priority)
        {
            byte agvPriority = 0x80;
            switch (priority.Trim())
            {
                case "0": agvPriority = 0x80; break;
                case "1":agvPriority = 0x81; break;
                case "2":agvPriority = 0x82; break;
                case "3":agvPriority = 0x83; break;
                case "4":agvPriority = 0x84; break;
                case "5":agvPriority = 0x85; break;
                case "6":agvPriority = 0x86; break;
                case "7": agvPriority = 0x87; break;
                case "8": agvPriority = 0x88; break;
                case "9": agvPriority = 0x89; break;
                default: break;
            }
            return agvPriority;
        }

        public bool SendmMessage(int FromStation, int ToStation, int index, byte[] msg)
        {
            byte[] retmsg = new byte[11];
            retmsg[0] = 0x00;
            retmsg[1] = (byte)'m';
            retmsg[2] = 0x00;
            retmsg[3] = 0x06;
            Array.Copy(msg, 4, retmsg, 4, 2);
            retmsg[6] = 0x00;
            retmsg[7] = 0x00;
            Array.Copy(GetHexBytes(FromStation), 0, retmsg, 8, 2);
            Array.Copy(GetHexBytes(ToStation), 0, retmsg, 10, 2);
            //Array.Copy(GetHexBytes(ToStation), 0, retmsg, 8, 2);
            if (nt8000.Connected)
                return nt8000.Send(PackMsg(1, retmsg));
            return false;
        }

        //�޸Ĳ���
        public bool SendmMessage_UpdateLayer(byte[] msg,int fromStation,int toStation, int height)
        {
            byte[] retmsg = new byte[28];     
            retmsg[0] = 0x00;
            retmsg[1] = (byte)'m';
            retmsg[2] = 0x00;
            retmsg[3] = 0x18;
            Array.Copy(msg, 4, retmsg, 4, 2);
            retmsg[6] = 0x00;
            retmsg[7] = 0x00;
            Array.Copy(GetHexBytes(fromStation), 0, retmsg, 8, 2);
            Array.Copy(GetHexBytes(toStation), 0, retmsg, 10, 2);
            Array.Copy(GetHexBytes(height), 0, retmsg, 12, 2);
            retmsg[14] = 0x00;
            retmsg[15] = 0x00;
            retmsg[16] = 0x00;
            retmsg[17] = 0x00;
            retmsg[18] = 0x00;
            retmsg[19] = 0x00;
            retmsg[20] = 0x00;
            retmsg[21] = 0x00;
            retmsg[22] = 0x00;
            retmsg[23] = 0x00;
            retmsg[24] = 0x00;
            retmsg[25] = 0x00;
            retmsg[26] = 0x00;
            retmsg[27] = 0x01;
            if (nt8000.Connected)
            {
                bool flag = nt8000.Send(PackMsg(1, retmsg));
                return flag;
            }
            return false;
        }

        public bool SendmMessage_UpdateLayer_Manual(int index, int fromStation, int toStation, int height)
        {
            try
            {
                byte[] retmsg = new byte[28];
                retmsg[0] = 0x00;
                retmsg[1] = (byte)'m';
                retmsg[2] = 0x00;
                retmsg[3] = 0x18;
                retmsg[4] = 0x00;
                retmsg[5] = Convert.ToByte(Convert.ToString(index,2).PadLeft(8,'0'),2);
                retmsg[6] = 0x00;
                retmsg[7] = 0x00;
                Array.Copy(GetHexBytes(fromStation), 0, retmsg, 8, 2);
                Array.Copy(GetHexBytes(toStation), 0, retmsg, 10, 2);
                Array.Copy(GetHexBytes(height), 0, retmsg, 12, 2);
                retmsg[14] = 0x00;
                retmsg[15] = 0x00;
                retmsg[16] = 0x00;
                retmsg[17] = 0x00;
                retmsg[18] = 0x00;
                retmsg[19] = 0x00;
                retmsg[20] = 0x00;
                retmsg[21] = 0x00;
                retmsg[22] = 0x00;
                retmsg[23] = 0x00;
                retmsg[24] = 0x00;
                retmsg[25] = 0x00;
                retmsg[26] = 0x00;
                retmsg[27] = 0x01;
                if (nt8000.Connected)
                {
                    bool flag = nt8000.Send(PackMsg(1, retmsg));
                    return flag;
                }
                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
          
        }
        public bool SendmMessage(byte[] msg)
        {
                byte[] retmsg = new byte[10];
                retmsg[0] = 0x00;
                retmsg[1] = (byte)'m';
                retmsg[2] = 0x00;
                retmsg[3] = 0x06;
                Array.Copy(msg, 4, retmsg, 4, 2);
                retmsg[6] = 0x00;
                retmsg[7] = 0x09;
                retmsg[8] = 0x00;
                retmsg[9] = 0x01;
                if (nt8000.Connected)
                {
                    bool flag = nt8000.Send(PackMsg(1, retmsg));
                    return flag;
                }
                return false;
        }

        //�������ź�
        public bool SendRMessage(int singel)
        {
            byte[] retmsg = new byte[9];
            retmsg[0] = 0x00;
            retmsg[1] = (byte)'R';
            retmsg[2] = 0x00;
            retmsg[3] = 0x05;
            retmsg[4] = 0x00;
            retmsg[5] = 0x02;
            retmsg[6] = 0x01;
            retmsg[7] = 0x03;
            if (singel == 1)
                retmsg[8] = 0x01;
            else if (singel == 0)
                retmsg[8] = 0x00;
            //Array.Copy(GetHexBytes(ToStation), 0, retmsg, 8, 2);
            if (nt8000.Connected)
                return nt8000.Send(PackMsg(1, retmsg));
            return false;
        }

        public bool SendmMessage(byte[] msg, int tostation)
        {
            byte[] retmsg = new byte[10];
            retmsg[0] = 0x00;
            retmsg[1] = (byte)'m';
            retmsg[2] = 0x00;
            retmsg[3] = 0x06;
            Array.Copy(msg, 4, retmsg, 4, 2);
            retmsg[6] = 0x00;
            if (tostation == 1)
                retmsg[7] = 0x05;
            else
                retmsg[7] = 0x01;
            Array.Copy(GetHexBytes(tostation), 0, retmsg, 8, 2);
            if (nt8000.Connected)
                return nt8000.Send(PackMsg(1, retmsg));
            return false;
        }

        public bool SendjMessage(int index)
        {
            byte[] retmsg = new byte[3];
            retmsg[0] = (byte)'j';
            Array.Copy(GetHexBytes(index), 0, retmsg, 1, 2);
            if (nt8000.Connected)
                return nt8000.Send(PackMsg(1, retmsg));
            return false;
        }

        public bool SendjMessageByCarNo(int carno)
        {
            byte[] retmsg = new byte[4];
            retmsg[0] = (byte)'j';
            retmsg[1] = 0x00;
            retmsg[2] = 0x00;
            retmsg[3] = (byte)carno;
            if (nt8000.Connected)
                return nt8000.Send(PackMsg(1, retmsg));
            return false;
        }

        private byte[] PackMsg(int functionCode, byte[] Msg)
        {
            //��װ��Ϣ
            byte[] Buffer = new byte[8 + Msg.Length];
            Buffer[0] = 0x87;
            Buffer[1] = 0xcd;
            Buffer[2] = 0x00;
            Buffer[3] = 0x08;
            Buffer[4] = GetHexBytes(Msg.Length)[0];
            Buffer[5] = GetHexBytes(Msg.Length)[1];
            Buffer[6] = GetHexBytes(functionCode)[0];
            Buffer[7] = GetHexBytes(functionCode)[1];
            Array.Copy(Msg, 0, Buffer, 8, Msg.Length);
            return Buffer;
        }

        private byte[] GetHexBytes(int i)
        {
            //��һ������ת��Ϊ����Ϊ2��byte������
            byte[] Buffer = new byte[2];
            Buffer[0] = (byte)(i / 256);
            Buffer[1] = (byte)(i % 256);
            return Buffer;
        }
    }
}
