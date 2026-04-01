using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    internal static class WmsFactory
    {
        private static IConfigurationRoot config;
        static WmsFactory()
        {
            LogHelper.LogInformation("Init WmsFactory()");
            config = WmsConfig.GetJsonConfig();
        }

        /*
        public static IWave MakeWave()
        {
            var objName = config["Outbound:Wave"];
            return Assembly.Load("NtiCloud.Modules.WMS").CreateInstance(objName) as IWave;
        }

        public static IOutboundOrder MakeOutboundOrder()
        {
            var objName = config["Outbound:OutboundOrder"];
            return Assembly.Load("NtiCloud.Modules.WMS").CreateInstance(objName) as IOutboundOrder;
        }

        public static IOutboundRule MakeOutboundRule()
        {
            var objName = config["Outbound:OutboundRule"];
            return Assembly.Load("NtiCloud.Modules.WMS").CreateInstance(objName) as IOutboundRule;
        }

        public static ILocation MakeLocation()
        {
            var objName = config["Common:Location"];
            return Assembly.Load("NtiCloud.Modules.WMS").CreateInstance(objName) as ILocation;
        }

        public static IEquipment MakeEquipment()
        {
            var objName = config["Common:Equipment"];
            return Assembly.Load("NtiCloud.Modules.WMS").CreateInstance(objName) as IEquipment;
        }

        public static IDock MakeDock()
        {
            var objName = config["Common:Dock"];
            return Assembly.Load("NtiCloud.Modules.WMS").CreateInstance(objName) as IDock;
        }
        */


        [ThreadStatic]
        private static string objectRoot = "WMS:Objects";
        private static string configRoot = "WMS";
        public static string ConfigRoot
        {
            set
            {
                configRoot = value;
                objectRoot = value + ":Objects";
            }
        }

        /*
        public static T Make<T>()
        {
            string name = typeof(T).Name;
            name = name[0] == 'I' ? name.Substring(1) : name;

            var objName = config[$"{objectRoot}:{name}"];

            try
            {
                //return (T)Assembly.Load("NtiCloud.Modules.WMS").CreateInstance(objName);
                return (T)Assembly.Load(config[$"{configRoot}:Assembly"]).CreateInstance(objName);
            }
            catch 
            {
                LogHelper.LogError($"无法创建[{objName}]对象，请检查配置文件");
                throw;
            }
        }
        */

        public static T Make<T>() where T:new()
        {
            return new T();
            /*
            "Container": "NtiCloud.Modules.ISA.Inventory.Container",
      "Dock": "NtiCloud.Modules.ISA.Base.Dock",
      "Equipment": "NtiCloud.Modules.ISA.Base.Equipment",
      "InboundQC": "NtiCloud.Modules.ISA.QC.InboundQC",
      "Item": "NtiCloud.Modules.ISA.Base.Item",
      "Location": "NtiCloud.Modules.ISA.Base.Location",
      "LocationAllocator": "NtiCloud.Modules.ISA.Base.LocationAllocator",
      "OutboundOrder": "NtiCloud.Modules.ISA.Outbound.OutboundOrder",
      "OutboundQC": "NtiCloud.Modules.ISA.QC.OutboundQC",
      "Receipt": "NtiCloud.Modules.ISA.Receiving.Receipt",
      "PickingZone": "NtiCloud.Modules.WMS.CP.Outbound.CPPickingZone",
      "Wave": "NtiCloud.Modules.ISA.Outbound.Wave",
      "WorkTask": "NtiCloud.Modules.ISA.Tasking.WorkTask",
      "WorkTaskInterface": "NtiCloud.Modules.ISA.Interface.WCS.WorkTaskInterface",
      "WorkUserLog": "NtiCloud.Modules.WMS.Common.WorkUserLog"
      */
      /*
            string name = typeof(T).Name;
            name = name[0] == 'I' ? name.Substring(1) : name;

            var objName = config[$"{objectRoot}:{name}"];

            try
            {
                //return (T)Assembly.Load("NtiCloud.Modules.WMS").CreateInstance(objName);
                return (T)Assembly.Load(config[$"{configRoot}:Assembly"]).CreateInstance(objName);
            }
            catch 
            {
                LogHelper.LogError($"无法创建[{objName}]对象，请检查配置文件");
                throw;
            }
            */
        }

        /*
        public static IContainer MakeContainer()
        {
            var objName = config["Common:Container"];
            return Assembly.Load("NtiCloud.Modules.WMS").CreateInstance(objName) as IContainer;
        }

        public static IItem MakeItem()
        {
            var objName = config["Common:Item"];
            return Assembly.Load("NtiCloud.Modules.WMS").CreateInstance(objName) as IItem;
        }

        public static IWorkTask MakeWorkTask()
        {
            var objName = config["Common:WorkTask"];
            return Assembly.Load("NtiCloud.Modules.WMS").CreateInstance(objName) as IWorkTask;
        }

        public static IWorkTaskInterface MakeWorkTaskInterface()
        {
            var objName = config["Common:WorkTaskInterface"];
            return Assembly.Load("NtiCloud.Modules.WMS").CreateInstance(objName) as IWorkTaskInterface;
        }
        */
    }
}
