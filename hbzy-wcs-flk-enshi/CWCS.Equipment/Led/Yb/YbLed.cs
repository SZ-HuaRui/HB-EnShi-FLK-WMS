using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CWCS.Equipment.Led.Yb.LedSdk;

namespace CWCS.Equipment.Led.Yb
{
    /**
     * 品牌：仰邦
     * 连接步骤：1.初始化Led。2.创建连接。
     *          
     */
    public class YbLed: YbLedBase, IYbLed
    {
        private uint _connectHandle = 0;

        private bx_5k_area_header bx_5k;

        private byte SoundMode;

        private byte SoundPerson;

        private byte SoundVolume;

        private byte SoundSpeed;

        private int SoundDataLen;
        private string _controllerType = "BX_5MK1";//控制器类型

        private string XPos = "0";//X坐标

        private string YPos = "0";//Y坐标

        private string Height = "64";//区域高度

        private string Width = "192";//区域宽度

        private int ShowLine = 1;//显示行数 0:单行显示  1:多行显示

        private int ChangeLine = 1;//换行方式 0:手动换行 1:自动换行

        private int LineSpacing = 0;//行间距(0-100点)

        private int RunningMode = 0;//运行模式 0:自动循环显示 1:完成后停留在最后一页 2:超时未完成删除该信息

        private int Limit = 2;//超时时间(秒)

        private int ShowStunt = 1;//显示特技 0:静止显示 1:快速打出 2:向左移动 3:向右移动 4:向上移动 5:向下移动

        private int RunningSpeed = 1;//运行速度

        private int StayTime = 10;//停留时间，单位(个0.5秒)

        private int Voice = 0;//不允许播放语音

        private int VoiceMan = 0;//播音人

        private int VoiceNum = 0;//音量

        private int VoiceVelocity = 0;//语速

        private string VoiceMsg = "";//语音内容

        public YbLed(string ip,string port = "5005"):base(ip,port)
        {
            bx_5k = new bx_5k_area_header();
            Initialize();
        }

        public override bool Connect()
        {
            if (IsConnected) return IsConnected;
            byte[] led_ip = Encoding.ASCII.GetBytes(Ip);
            uint led_port = Convert.ToUInt32(Port);
            byte[] card_type_list = new byte[12];
            card_type_list[0] = 0xFE;//任意
            card_type_list[1] = 0x51;//BX_5K1
            card_type_list[2] = 0x58;//BX_5K2
            card_type_list[3] = 0x54;//BX_5MK1
            card_type_list[4] = 0x53;//BX_5MK2
            card_type_list[5] = 0x5c;//BX_5K1Q_YY
            card_type_list[6] = 0x61;//BX_6K1
            card_type_list[7] = 0x62;//BX_6K2
            card_type_list[8] = 0x63;//BX_6K3
            card_type_list[9] = 0x64;//BX_6K1_YY
            card_type_list[10] = 0x65;//BX_6K2_YY
            card_type_list[11] = 0x66;//BX_6K3_YY
            int type = GetControllerType(_controllerType);
            _connectHandle = CreateClient(led_ip, led_port, (bx_5k_card_type)card_type_list[type], 1, 1, null);
            IsConnected = _connectHandle == 0 ? false : true;
            if (IsConnected)
            {
                NotifyStringMessage?.Invoke($"Led:{Ip}连接成功！");
            }
            else
            {
                NotifyStringMessage?.Invoke($"Led:{Ip}连接失败！");
            }
            ReConnectLoop();
            return IsConnected;
        }

        public override bool SendString(string message)
        {
            if (IsConnected)
            {
                bx_5k.DynamicAreaLoc = (byte)0;
                byte[] areaText = Encoding.Default.GetBytes(message.Trim());
                bx_5k.DataLen = areaText.Length;
                int x;
                x = SCREEN_SendDynamicArea(_connectHandle, bx_5k, (ushort)bx_5k.DataLen, areaText);
                IsConnected = x == 0 ? true : false;
            }
            if (!IsConnected)
            {
                NotifyStringMessage?.Invoke($"Led:{Ip}消息发送失败！");
            }
            ReConnectLoop();
            return IsConnected;
           
        }

        public override bool Initialize()
        {
            try
            {
                //初始化sdk
                InitSdk(2, 2);
                bx_5k.AreaType = 0x06;
                bx_5k.AreaX = Convert.ToInt16(XPos);
                bx_5k.AreaX /= 8;
                if (Ip.Equals("192.168.57.34"))
                {
                    YPos = "32";
                }
                bx_5k.AreaY = Convert.ToInt16(YPos);
                bx_5k.AreaWidth = Convert.ToInt16(Width);
                bx_5k.AreaWidth /= 8;
                bx_5k.AreaHeight = Convert.ToInt16(Height);
                bx_5k.Lines_sizes = Convert.ToByte(LineSpacing.ToString());

                byte[] RunMode_list = new byte[3];
                RunMode_list[0] = 0;
                RunMode_list[1] = 1;
                RunMode_list[2] = 2;
                int rl = RunningMode;
                bx_5k.RunMode = RunMode_list[rl];

                bx_5k.Timeout = Convert.ToInt16(Limit.ToString());

                byte[] SoundMode_list = new byte[3];
                SoundMode_list[0] = 0;
                SoundMode_list[1] = 1;
                SoundMode_list[2] = 2;
                int sml = Voice;
                SoundMode = SoundMode_list[sml];

                byte[] SoundPerson_list = new byte[6];
                SoundPerson_list[0] = 0;
                SoundPerson_list[1] = 1;
                SoundPerson_list[2] = 2;
                SoundPerson_list[3] = 3;
                SoundPerson_list[4] = 4;
                SoundPerson_list[5] = 5;
                int spl = VoiceMan;
                SoundPerson = SoundPerson_list[spl];

                SoundVolume = (byte)VoiceNum;

                SoundSpeed = (byte)VoiceVelocity;

                byte[] soundAreaText = Encoding.Default.GetBytes(VoiceMsg);
                SoundDataLen = soundAreaText.Length;

                bx_5k.Reserved1 = 0;
                bx_5k.Reserved2 = 0;
                bx_5k.Reserved3 = 0;

                byte[] SingleLine_list = new byte[2];
                SingleLine_list[0] = 0x01;
                SingleLine_list[1] = 0x02;
                int sll = ShowLine;
                bx_5k.SingleLine = SingleLine_list[sll];

                byte[] NewLine_list = new byte[2];
                NewLine_list[0] = 0x01;
                NewLine_list[1] = 0x02;
                int nl = ChangeLine;
                bx_5k.NewLine = NewLine_list[nl];


                byte[] DisplayMode_list = new byte[6];
                DisplayMode_list[0] = 0x01;
                DisplayMode_list[1] = 0x02;
                DisplayMode_list[2] = 0x03;
                DisplayMode_list[3] = 0x04;
                DisplayMode_list[4] = 0x05;
                DisplayMode_list[5] = 0x06;
                int dml = ShowStunt;
                bx_5k.DisplayMode = DisplayMode_list[dml];
                //bx_5k.DisplayMode = Convert.ToByte(comboBox4.SelectedIndex);

                bx_5k.ExitMode = 0x00;


                bx_5k.Speed = (byte)RunningSpeed;
                //bx_5k.Speed=Convert.ToByte(comboBox5.SelectedIndex);

                bx_5k.StayTime = Convert.ToByte(StayTime);
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }

        public override bool Close()
        {
            Destroy(_connectHandle);
            return true;
        }

        public int GetControllerType(string model)
        {
            int type;
            switch (model)
            {
                case "BX_5K1": type = 1; break;
                case "BX_5K2": type = 2; break;
                case "BX_5MK1": type = 3; break;
                case "BX_5MK2": type = 4; break;
                case "BX_5K1Q_YY":type = 5; break;
                case "BX_6K1": type = 6; break;
                case "BX_6K2": type = 7; break;
                case "BX_6K3": type = 8; break;
                case "BX_6K1_YY": type = 9; break;
                case "BX_6K2_YY": type = 10; break;
                case "BX_6K3_YY": type = 11; break;
                default: type = 0; break;
            }
            return type;
        }

       
    }
}
