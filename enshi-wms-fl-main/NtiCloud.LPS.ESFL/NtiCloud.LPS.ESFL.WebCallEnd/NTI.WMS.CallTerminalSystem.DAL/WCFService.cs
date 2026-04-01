using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;


namespace NTI.WMS.CallTerminalSystem.DAL
{
    /// <summary>
    /// 服务接口类
    /// Create by Qiuwenhuan 2009/05/13
    /// </summary>
    public class WCFService
    {
        static System.Threading.Thread KeepAliveThread;

        // static ChannelFactory<WMSInterface.ProductionSchemeWS> factory_ProductionScheme;
        static ChannelFactory<WMSInterface.InitializeWS> factory_Initialize;
        //static ChannelFactory<WMSInterface.StrategyWS> factory_Strategy;
        //static ChannelFactory<WMSInterface.StrategyValueWS> factory_StrategyValue;
        //static ChannelFactory<WMSInterface.ReceivingWS> factory_Receiving;
        //static ChannelFactory<WMSInterface.PermissionManagerWS> factory_PermissionManager;
        //static ChannelFactory<WMSInterface.LoginWS> factory_Login;
        //static ChannelFactory<WMSInterface.ShortInventoryWS> factory_ShortInventory;
        //static ChannelFactory<WMSInterface.ProducingPrintWS> factory_ProducingPrint;
        //static ChannelFactory<WMSInterface.LocationWS> factory_Location;
        //static ChannelFactory<WMSInterface.TaskWS> factory_Task;
        //static ChannelFactory<WMSInterface.PreInBoundWS> factory_PreInBound;
        //static ChannelFactory<WMSInterface.MessengerWS> factory_Messenger;
        //static ChannelFactory<WMSInterface.CountWS> factory_Count;
        ////static ChannelFactory<WMSInterface.OutBoundingWS> factory_OutBounding;
        //static ChannelFactory<WMSInterface.RunsWS> factory_Runs;
        //static ChannelFactory<WMSInterface.CheckWS> factory_Check;

        static ChannelFactory<WMSInterface.CallEndWS> factory_CallEndWS;
        //static ChannelFactory<WMSInterface.CombinationWS> factory_CombinationWS;
        //static ChannelFactory<WMSInterface.SingleItemsWS> factory_SingleItemsWS;

        //static WMSInterface.ProductionSchemeWS _ProductionSchemeWS;
        static WMSInterface.InitializeWS _InitializeWS;
        //static WMSInterface.StrategyWS _StrategyWS;
        //static WMSInterface.StrategyValueWS _StrategyValueWS;
        //static WMSInterface.ReceivingWS _ReceivingWS;
        //static WMSInterface.PermissionManagerWS _PermissionManagerWS;
        //static WMSInterface.LoginWS _LoginWS;
        //static WMSInterface.ShortInventoryWS _ShortInventoryWS;
        //static WMSInterface.ProducingPrintWS _ProducingPrintWS;
        //static WMSInterface.LocationWS _LocationWS;
        //static WMSInterface.TaskWS _TaskWS;
        //static WMSInterface.PreInBoundWS _PreInBoundWS;
        //static WMSInterface.MessengerWS _MessengerWS;
        //static WMSInterface.CountWS _CountWS;
        ////static WMSInterface.OutBoundingWS _OutBoundingWS;
        //static WMSInterface.RunsWS _RunsWS;
        //static WMSInterface.CheckWS _CheckWS;

        static WMSInterface.CallEndWS _CallEndWS;
        //static WMSInterface.CombinationWS _CombinationWS;
        //static WMSInterface.SingleItemsWS _SingleItemsWS;

        #region Property
        //public static WMSInterface.ProductionSchemeWS ProductionSchemeWS
        //{
        //    get
        //    {
        //        return _ProductionSchemeWS;
        //    }
        //}

        public static WMSInterface.InitializeWS InitializeWS
        {
            get
            {
                return _InitializeWS;
            }
        }

        //public static WMSInterface.StrategyWS StrategyWS
        //{
        //    get
        //    {
        //        return _StrategyWS;
        //    }
        //}

        //public static WMSInterface.StrategyValueWS StrategyValueWS
        //{
        //    get
        //    {
        //        return _StrategyValueWS;
        //    }
        //}

        //public static WMSInterface.ReceivingWS ReceivingWS
        //{
        //    get
        //    {
        //        return _ReceivingWS;
        //    }
        //}

        //public static WMSInterface.PermissionManagerWS PermissionManagerWS
        //{
        //    get
        //    {
        //        return _PermissionManagerWS;
        //    }
        //}

        //public static WMSInterface.LoginWS LoginWS
        //{
        //    get
        //    {
        //        return _LoginWS;
        //    }
        //}

        //public static WMSInterface.ShortInventoryWS ShortInventoryWS
        //{
        //    get
        //    {
        //        return _ShortInventoryWS;
        //    }
        //}

        //public static WMSInterface.ProducingPrintWS ProducingPrintWS
        //{
        //    get
        //    {
        //        return _ProducingPrintWS;
        //    }
        //}

        //public static WMSInterface.LocationWS LocationWS
        //{
        //    get
        //    {
        //        return _LocationWS;
        //    }
        //}

        //public static WMSInterface.TaskWS TaskWS
        //{
        //    get
        //    {
        //        return _TaskWS;
        //    }
        //}

        //public static WMSInterface.PreInBoundWS PreInBoundWS
        //{
        //    get
        //    {
        //        return _PreInBoundWS;
        //    }
        //}

        //public static WMSInterface.MessengerWS MessengerWS
        //{
        //    get
        //    {
        //        return _MessengerWS;
        //    }
        //}

        //public static WMSInterface.CountWS CountWS
        //{
        //    get
        //    {
        //        return _CountWS;
        //    }
        //}

        ////public static WMSInterface.OutBoundingWS OutBoundingWS
        ////{
        ////    get
        ////    {
        ////        return _OutBoundingWS;
        ////    }
        ////}

        //public static WMSInterface.RunsWS RunsWS
        //{
        //    get
        //    {
        //        return _RunsWS;
        //    }
        //}

        //public static WMSInterface.CheckWS CheckWS
        //{
        //    get
        //    {
        //        return _CheckWS;
        //    }
        //}

        public static WMSInterface.CallEndWS CallEndWS
        {
            get
            {
                return _CallEndWS;
            }
        }
        //public static WMSInterface.CombinationWS CombinationWS
        //{
        //    get { return _CombinationWS; }
        //}
        //public static WMSInterface.SingleItemsWS SingleItemsWS
        //{
        //    get { return _SingleItemsWS; }
        //}
        #endregion

        static void KeepAlive()
        {
            try
            {
                while (true)
                {
                    //ProductionSchemeWS.KeepAlive(); ;
                    InitializeWS.KeepAlive(); ;
                    //StrategyWS.KeepAlive(); ;
                    //StrategyValueWS.KeepAlive(); ;
                    //ReceivingWS.KeepAlive(); ;
                    //PermissionManagerWS.KeepAlive(); ;
                    //LoginWS.KeepAlive(); ;
                    //ShortInventoryWS.KeepAlive(); ;
                    //ProducingPrintWS.KeepAlive(); ;
                    //LocationWS.KeepAlive(); ;
                    //TaskWS.KeepAlive(); ;
                    //PreInBoundWS.KeepAlive(); ;
                    //MessengerWS.KeepAlive(); ;
                    //CountWS.KeepAlive(); ;
                    ////OutBoundingWS.KeepAlive(); ;
                    //RunsWS.KeepAlive(); ;
                    //CheckWS.KeepAlive(); ;

                    CallEndWS.KeepAlive();
                    //CombinationWS.KeepAlive();
                    //SingleItemsWS.KeepAlive();

                    for (int i = 0; i < int.Parse(Properties.Settings.Default.KeepAlive); i++)
                        System.Threading.Thread.Sleep(1000);
                }
            }
            catch (System.Threading.ThreadAbortException)
            { ; }
            catch
            {
                //MsgBox.ShowErr("NTIMessage.System_ServerError");
                //System.Windows.Forms.Application.Exit();
            }
            finally { }
        }

        static WCFService()
        {
            //factory_ProductionScheme = new ChannelFactory<WMSInterface.ProductionSchemeWS>("ProductionScheme");
            //_ProductionSchemeWS = factory_ProductionScheme.CreateChannel();

            factory_Initialize = new ChannelFactory<WMSInterface.InitializeWS>("Initialize");
            _InitializeWS = factory_Initialize.CreateChannel();

            //factory_Strategy = new ChannelFactory<WMSInterface.StrategyWS>("Strategy");
            //_StrategyWS = factory_Strategy.CreateChannel();

            //factory_StrategyValue = new ChannelFactory<WMSInterface.StrategyValueWS>("StrategyValue");
            //_StrategyValueWS = factory_StrategyValue.CreateChannel();

            //factory_Receiving = new ChannelFactory<WMSInterface.ReceivingWS>("Receiving");
            //_ReceivingWS = factory_Receiving.CreateChannel();

            //factory_PermissionManager = new ChannelFactory<WMSInterface.PermissionManagerWS>("PermissionManager");
            //_PermissionManagerWS = factory_PermissionManager.CreateChannel();

            //factory_Login = new ChannelFactory<WMSInterface.LoginWS>("Login");
            //_LoginWS = factory_Login.CreateChannel();

            //factory_ShortInventory = new ChannelFactory<WMSInterface.ShortInventoryWS>("ShortInventory");
            //_ShortInventoryWS = factory_ShortInventory.CreateChannel();

            //factory_ProducingPrint = new ChannelFactory<WMSInterface.ProducingPrintWS>("ProducingPrint");
            //_ProducingPrintWS = factory_ProducingPrint.CreateChannel();

            //factory_Location = new ChannelFactory<WMSInterface.LocationWS>("Location");
            //_LocationWS = factory_Location.CreateChannel();

            //factory_Task = new ChannelFactory<WMSInterface.TaskWS>("Task");
            //_TaskWS = factory_Task.CreateChannel();

            //factory_PreInBound = new ChannelFactory<WMSInterface.PreInBoundWS>("PreInBound");
            //_PreInBoundWS = factory_PreInBound.CreateChannel();

            //factory_Messenger = new ChannelFactory<WMSInterface.MessengerWS>("Messenger");
            //_MessengerWS = factory_Messenger.CreateChannel();

            //factory_Count = new ChannelFactory<WMSInterface.CountWS>("Count");
            //_CountWS = factory_Count.CreateChannel();

            //factory_OutBounding = new ChannelFactory<WMSInterface.OutBoundingWS>("OutBounding");
            //_OutBoundingWS = factory_OutBounding.CreateChannel();

            //factory_Runs = new ChannelFactory<WMSInterface.RunsWS>("Runs");
            //_RunsWS = factory_Runs.CreateChannel();

            //factory_Check = new ChannelFactory<WMSInterface.CheckWS>("Check");
            //_CheckWS = factory_Check.CreateChannel();

            factory_CallEndWS = new ChannelFactory<WMSInterface.CallEndWS>("CallEnd");
            _CallEndWS = factory_CallEndWS.CreateChannel();

            //factory_CombinationWS = new ChannelFactory<WMSInterface.CombinationWS>("CombinatePallet");
            //_CombinationWS = factory_CombinationWS.CreateChannel();

            //factory_SingleItemsWS = new ChannelFactory<WMSInterface.SingleItemsWS>("SingleItems");
            //_SingleItemsWS = factory_SingleItemsWS.CreateChannel();

            KeepAliveThread = new System.Threading.Thread(KeepAlive);
            KeepAliveThread.IsBackground = true;
            KeepAliveThread.Start();
        }


        public static void StopService()
        {
            try
            {
                KeepAliveThread.Abort();
                KeepAliveThread = null;
                //factory_ProductionScheme.Close();
                factory_Initialize.Close();
                //factory_Strategy.Close();
                //factory_StrategyValue.Close();
                //factory_Receiving.Close();
                //factory_PermissionManager.Close();
                //factory_Login.Close();
                //factory_ShortInventory.Close();
                //factory_ProducingPrint.Close();
                //factory_Location.Close();
                //factory_Task.Close();
                //factory_PreInBound.Close();
                //factory_Messenger.Close();
                //factory_Count.Close();
                //factory_OutBounding.Close();
                //factory_Runs.Close();
                //factory_Check.Close();

                factory_CallEndWS.Close();
                //factory_CombinationWS.Close();
                //factory_SingleItemsWS.Close();
            }
            catch { }
        }
    }
}
