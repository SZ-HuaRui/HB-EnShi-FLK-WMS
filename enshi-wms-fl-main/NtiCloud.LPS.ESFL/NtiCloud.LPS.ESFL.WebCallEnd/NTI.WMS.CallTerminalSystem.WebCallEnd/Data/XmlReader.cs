using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.IO;
using System.Reflection;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public class XmlReader
    {
        private static string XMLFile = Path.GetDirectoryName(Assembly.GetCallingAssembly().GetName().CodeBase) + "\\XmlApp.xml";
        private static string path = System.Web.HttpContext.Current.Server.MapPath("/") + @"XmlApp.xml";

        //private static string XMLFile = Path.GetDirectoryName(Properties.Settings.Default.CallEndXmlSet);
        //private static string path = Properties.Settings.Default.CallEndXmlSet;

        private static XmlDocument document;

        /// <summary>
        /// xml文件初始化
        /// </summary>
        /// <param name="isWrite">是否要进行写操作</param>
        private static void Init(bool isWrite)
        {
            try
            {
                FileInfo f = new FileInfo(path);
                if (!f.Exists)
                {
                    
                    //MessageBoxTimeOutStatic.show("错误提示", "无xml数据文件!", "error");
                }
                else
                {
                    if (isWrite && f.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    {
                        f.Attributes = FileAttributes.Normal;
                    }
                    document = new XmlDocument();
                    document.Load(path);
                }
            }
            catch(Exception e)
            {
                throw e;
               //MessageBoxTimeOutSt atic.show("错误提示", "xml数据文件操作失败", "error");
            }
            finally { }
        }

        /// <summary>
        /// xml文件保存
        /// </summary>
        private static void SaveXMLFile()
        {
            try
            {
                FileInfo f = new FileInfo(path);
                if (!f.Exists)
                {
                    //MessageBoxTimeOutStatic.show("错误提示", "无xml数据文件!", "error");
                }
                else
                {
                    if (f.Attributes.ToString().IndexOf("ReadOnly") <= 0)
                    {
                        document.Save(path);
                        f.Attributes = FileAttributes.ReadOnly;
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
                //MessageBoxTimeOutStatic.show("错误提示", "保存xml数据文件操作失败", "error");
            }
        }

        public static string GetLocalEquipmentID()
        {
            try
            {
                Init(false);
                XmlNode node = document["config"]["EquipmentNumber"];
                return node.InnerText.Trim();
            }
            catch
            {
                return null;
            }
        }
        public static bool SetLocalEquipmentID(string equipmentNumber)
        {
            try
            {
                Init(true);
                XmlNode node = document["config"]["EquipmentNumber"];
                node.InnerText = equipmentNumber;
                SaveXMLFile();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <returns></returns>
        public static Warehouse GetWarehouse()
        {
            try
            {
                Init(false);
                XmlNode node = document["config"]["Warehouse"];
                Warehouse warehouse = new Warehouse();
                warehouse.ID = node["ID"].InnerText;
                warehouse.Name = node["Name"].InnerText;
                return warehouse;
            }
            catch
            {
                return null;
            }
        }

        public static string GetMachineNumber()
        {
            try
            {
                Init(false);
                XmlNode node = document["config"]["MachineNumber"];
                return node.InnerText.ToString();
            }
            catch
            {
                return null;
            }
        }
        public static bool SetMachineNumber(string machineNumber)
        {
            try
            {
                Init(true);
                XmlNode node = document["config"]["MachineNumber"];
                node.InnerText = machineNumber;
                SaveXMLFile();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //保存班次
        public static bool SetClassTeam(string ClassTeam)
        {
            try
            {
                Init(true);
                XmlNode node = document["config"]["ClassTeam"];
                node.InnerText = ClassTeam;
                SaveXMLFile();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //读取班次
        public static string GetClassTeam()
        {
            try
            {
                Init(false);
                XmlNode node = document["config"]["ClassTeam"];
                return node.InnerText.ToString();
            }
            catch
            {
                return null;
            }
        }

        //保存班组
        public static bool SetClassGroup(string ClassGroup)
        {
            try
            {
                Init(true);
                XmlNode node = document["config"]["ClassGroup"];
                node.InnerText = ClassGroup;
                SaveXMLFile();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //读取班组
        public static string GetClassGroup()
        {
            try
            {
                Init(false);
                XmlNode node = document["config"]["ClassGroup"];
                return node.InnerText.ToString();
            }
            catch
            {
                return null;
            }
        }

        //保存用户名
        public static bool SetUserName(string UserName)
        {
            try
            {
                Init(true);
                XmlNode node = document["config"]["UserName"];
                node.InnerText = UserName;
                SaveXMLFile();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //读取用户名
        public static string GetUserName()
        {
            try
            {
                Init(false);
                XmlNode node = document["config"]["UserName"];
                return node.InnerText.ToString();
            }
            catch
            {
                return null;
            }
        }

        public static string GetMachineName()
        {
            try
            {
                Init(false);
                XmlNode node = document["config"]["MachineName"];
                return node.InnerText.ToString();
            }
            catch
            {
                return null;
            }
        }
        public static bool SetMachineName(string machineName)
        {
            try
            {
                Init(true);
                XmlNode node = document["config"]["MachineName"];
                node.InnerText = machineName;
                SaveXMLFile();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SetWorkShop(string workshop)
        {
            try
            {
                Init(true);
                XmlNode node = document["config"]["WorkShop"];
                node.InnerText = workshop;
                SaveXMLFile();
                return true;
            }
            catch { return false; }
        }

        public static string GetWorkShop()
        {
            try
            {
                Init(false);
                XmlNode node = document["config"]["WorkShop"];
                return (string)(node.InnerText);
            }
            catch
            {
                return null;
            }
        }

        public static int GetHasLGV()
        {
            try
            {
                Init(false);
                XmlNode node = document["config"]["HasLGV"];
                return int.Parse(node.InnerText);
            }
            catch
            {
                return 0;
            }
        }

        //获取默认的站台 PEQ 20110223
        public static string GetDockNumber()
        {
            try
            {
                Init(false);
                XmlNode node = document["config"]["MachineName"];
                return (string)(node.InnerText);
            }
            catch { return null; }
        }
        public static bool SetHasLGV(int HasLGV)
        {
            try
            {
                Init(true);
                XmlNode node = document["config"]["HasLGV"];
                node.InnerText = HasLGV.ToString();
                SaveXMLFile();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}