using Invengo.NetAPI.Protocol.IRP1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CWCS.Equipment.Rfid
{
    public abstract class RfidBase
    {
        public Action<string> NotifyStringMessage { get; set; }
        public string ReaderName { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public string EpcData { get; set; }
        public string UserData { get; set; }
        public string ReserveData { get; set; }
        public string TID { get; set; }
        public bool IsConnected { get; set; } = false;
        public bool IsReConnectLoopStarted { get; set; } = false;

        public RfidBase(string ip,string port)
        {
            Ip = ip;
            Port = port;
        }
        public void ResetData()
        {
            TID = string.Empty;
            EpcData = string.Empty;
            UserData = string.Empty;
        }
        public virtual string ReadString()
        {
            return string.Empty;
        }
        public virtual byte[] ReadBytes()
        {
            return null;
        }
      
        public void ReConnectLoop()
        {
            if (!IsConnected && !IsReConnectLoopStarted)
            {
                IsReConnectLoopStarted = true;
                Task.Run(() => {
                    while (true)
                    {
                        if (Connect())
                        {
                            IsReConnectLoopStarted = false;
                            break;
                        }
                        else
                        {
                            Thread.Sleep(5000);
                        }
                    }
                });
            }
        }

        public virtual bool WriteString(string message)
        {
            return true;
        }
        public virtual bool WriteBytes(byte[] bytes)
        {
            return true;
        }

        public virtual bool Connect()
        {
            return true;
        }

        public virtual bool Close()
        {
            return true;
        }

        public virtual bool Initialize()
        {
            return true;
        }

        public virtual void ResolveTID(byte[] bytes)
        {

        }
        public virtual void ResolveEpcData(byte[] bytes)
        {

        }
        public virtual void ResolveUserData(byte[] bytes)
        {

        }
        public virtual void ResolveReserveData(byte[] bytes)
        {

        }

        public virtual void ReadEpcData()
        {

        }
        public virtual void ReadUserData()
        {

        }
        public virtual void ReadReserveData()
        {

        }
        public virtual void ReadTID()
        {

        }
        public virtual void ReadAll()
        {

        }
        public virtual bool WriteEpcDataString(string message)
        {
            return true;
        }
        public virtual bool WriteUserDataString(string message)
        {
            return true;
        }
        public virtual bool WriteReserveString(string message)
        {
            return true;
        }
        public virtual bool WriteEpcDataBytes(byte[] bytes)
        {
            return true;
        }
        public virtual bool WriteUserDataBytes(byte[] bytes)
        {
            return true;
        }
        public virtual bool WriteReserveBytes(byte[] bytes)
        {
            return true;
        }

    }
}
