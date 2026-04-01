using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Spring.Context;
using Spring.Context.Support;

namespace NTI.WMS.CallTerminalSystem.DAL
{
    static class Class_WCFService
    {
        static Dictionary<Type, ChannelFactory> factorys = new Dictionary<Type, ChannelFactory>();
        static IApplicationContext ctx = null;

        static Class_WCFService()
        {
            AddFactory<WMSInterface.InitializeWS>("Initialize");
            AddFactory<WMSInterface.CallEndWS>("CallEnd");
        }

        public static void ReConnect()
        {
            ContextRegistry.Clear();
            ctx = null;
        }

        static public void CloseService(Object srv)
        {
            if (srv is System.ServiceModel.IClientChannel)
            {
                ((System.ServiceModel.IClientChannel)srv).Close();
            }
        }

        static void AddFactory<Type>(String configName)
        {
            var value = new ChannelFactory<Type>(configName);
            factorys.Add(typeof(Type), value);
        }

        public static Type GetFactory<Type>()
        {
            return ((ChannelFactory<Type>)factorys[typeof(Type)]).CreateChannel();
        }

        public static void StopService()
        {
            try
            {
                foreach (var item in factorys.Values)
                {
                    try
                    {
                        item.Close();
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
