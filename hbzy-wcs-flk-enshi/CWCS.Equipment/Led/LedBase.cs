using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CWCS.Equipment.Led
{
    public abstract class LedBase
    {
        public string Ip { get; set; }
        public string Port { get; set; }
        public bool IsConnected { get; set; } = false;
        public bool IsReConnectLoopStarted { get; set; } = false;
        public Action<string> NotifyStringMessage { get; set; }

        public LedBase(string ip,string port)
        {
            Ip = ip;
            Port = port;
        }

        public void ReConnectLoop()
        {
            if (!IsConnected && !IsReConnectLoopStarted)
            {
                IsReConnectLoopStarted = true;
                Task.Run(()=> {
                    while (true)
                    {
                        if (Connect())
                        {
                            IsReConnectLoopStarted = false;
                            break;
                        }
                        else
                        {
                            Thread.Sleep(3000);
                        }
                    }

                });
            }
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

        public virtual bool SendString(string message)
        {
            return true;
        }

        public virtual bool SendBytes(byte[] bytes)
        {
            return true;
        }
    }
}
