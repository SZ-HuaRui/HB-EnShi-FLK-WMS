using Invengo.NetAPI.Core;
using Invengo.NetAPI.Protocol.IRP1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CWCS.Equipment.Rfid.Invengo
{
    public class InvengoRfid: InvengoRfidBase, IInvengoRfid
    {
        /**
         * 1.读写器默认为IP：192.168.0.210，子网掩码： 255.255.255.0，网关：192.168.0.1。
         * 2.1000 00001使用天线1
         * 3.目前RFID一般采用6C协议,读取范围更大
         * 
         * 
         */
        public byte Antenna { get; set; } = 0x01;//天线
        public byte[] Password { get; set; } = new byte[] { 0, 0, 0, 0 };

        public InvengoRfid(string ip, string port = "7086"):base(ip,port)
        {
            Initialize();
        }

        public override bool Initialize()
        {
            ReaderName = $"{Ip}-{Port}-{Guid.NewGuid().ToString().Replace("-","")}";
            //Invengo规定格式【ip:port】
            ReaderCore = new Reader(ReaderName, "TCPIP_Client", $"{Ip}:{Port}");
            BaseReader.OnApiException += new ApiExceptionHandle(Reader_OnApiException);
            ReaderCore.OnMessageNotificationReceived += new MessageNotificationReceivedHandle(reader_OnMessageNotificationReceived);
            return true;
        }

        public void Reader_OnApiException(ErrInfo e)
        {
            NotifyStringMessage?.Invoke($"Rfid读写器：{ReaderName},调用OnApiException接口,读取RFID时出现异常,错误码:{e.Ei.ErrCode},错误信息:{e.Ei.ErrMsg}");
            IsConnected = false;
            ReConnectLoop();
        }

        public void reader_OnMessageNotificationReceived(BaseReader reader, IMessageNotification msg)
        {
            //msg不等于0，表示有错误
            if (msg.StatusCode != 0)
            {
                //显示错误信息
                ResetData();
                NotifyStringMessage?.Invoke($"Rfid读写器:{ReaderName},OnMessageNotificationReceived接口,Rfid读写器报告错误,错误码:{msg.StatusCode},错误信息:{msg.ErrInfo}");
                IsConnected = false;
                ReConnectLoop();
                return;
            }
            try
            {
                    string messageType = msg.GetMessageType();
                    messageType = messageType.Substring(messageType.LastIndexOf('.') + 1);
                    switch (messageType)
                    {
                        case "RXD_TagData":
                            {
                                RXD_TagData m = (RXD_TagData)msg;
                                string tagType = m.ReceivedMessage.TagType;
                                byte[] bytepc = m.ReceivedMessage.EPC;
                                ResetData();
                                ResolveTID(m.ReceivedMessage.TID);
                                ResolveEpcData(m.ReceivedMessage.EPC);
                                ResolveUserData(m.ReceivedMessage.UserData);
                                //关功放
                                ReaderCore.Send(new PowerOff());

                            }
                            break;
                        default: break;
                    }
            }
            catch (Exception ex)
            {
                NotifyStringMessage?.Invoke(ex.Message);
            }
        }
        public override void ReadUserData()
        {
            ReadAll();
            if (TID == null)
            {
                NotifyStringMessage?.Invoke($"Rfid读写器:{ReaderName},读失败！因为未识别到芯片ID");
                return;
            }
            ReadUserData2_6B readTag = new ReadUserData2_6B(Antenna, Util.ConvertHexStringToByteArray(TID), 0x00, 8);
            if (ReaderCore.Send(readTag))
            {
                ResolveUserData(readTag.ReceivedMessage.UserData);
                NotifyStringMessage?.Invoke($"Rfid读写器:{ReaderName},读取User区数据成功！");
                return;
            }
        }

        public override bool WriteUserDataString(string message)
        {
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            return WriteUserDataBytes(messageBytes);
        }

        public override bool WriteUserDataBytes(byte[] bytes)
        {
            ReadAll();
            if (TID == null)
            {
                NotifyStringMessage?.Invoke($"Rfid读写器:{ReaderName}写入失败！因为未识别到芯片ID");
                return false;
            }
            WriteUserData_6B writeTag = new WriteUserData_6B(Antenna, Util.ConvertHexStringToByteArray(TID), 0, bytes);
            if (ReaderCore.Send(writeTag))
            {
                NotifyStringMessage?.Invoke($"Rfid读写器:{ReaderName},写入成功！");
                return true;
            }
            NotifyStringMessage?.Invoke($"Rfid读写器:{ReaderName},写入失败！");
            return false;
        }



        

        public override bool Connect()
        {
            if (ReaderCore.IsConnected)
            {
                ReaderCore.Disconnect();
            }
            IsConnected = ReaderCore.Connect();
            if (!IsConnected)
            {
                NotifyStringMessage?.Invoke($"Rfid读写器:{ReaderName},连接失败！");
            }
            else
            {
                NotifyStringMessage?.Invoke($"Rfid读写器:{ReaderName},连接成功！");
            }
            ReConnectLoop();
            return IsConnected;
        }

        

        /// <summary>
        /// 读取所有区
        /// </summary>
        public override void ReadAll()
        {
            ResetData();
            //ReadTag每次读取状态可能会变化。需要重新实例化
            //ReadMemoryBank.ID_6B 读取6B标签的T_ID
            ReadTag readTag = new ReadTag(ReadTag.ReadMemoryBank.ID_6B,true);
            readTag.Antenna = Antenna;
            readTag.IsLoop = true;//循环读取
            readTag.Q = 0;//取值范围0-15
            readTag.AccessPwd = Password;
            readTag.IsReturn = true;//需要返回值
            readTag.Timeout = 3000;//3秒中响应
            ReaderCore.Send(readTag);
            //等待回调
            Thread.Sleep(2000);
            
        }

        public override void ReadTID()
        {
            ResetData();
            //ReadTag每次读取状态可能会变化。需要重新实例化
            ReadTag readTag = new ReadTag(ReadTag.ReadMemoryBank.ID_6B);
            readTag.Antenna = Antenna;
            readTag.IsLoop = true;//循环读取
            readTag.Q = 0;//取值范围0-15
            readTag.AccessPwd = Password;
            readTag.IsReturn = true;//需要返回值
            ReaderCore.Send(readTag);
            //等待回调
            Thread.Sleep(2000);
        }

        public override void ResolveEpcData(byte[] bytes)
        {
            EpcData = bytes != null ? Util.ConvertByteArrayToHexString(bytes) : string.Empty;
        }

        public override void ResolveTID(byte[] bytes)
        {
           
            TID = bytes != null ? Util.ConvertByteArrayToHexString(bytes) : string.Empty;
        }
        public override void ResolveUserData(byte[] bytes)
        {
            if (bytes == null)
            {
                UserData = string.Empty;
                return;
            }
            UserData = Encoding.ASCII.GetString(bytes);
        }

    }
}
