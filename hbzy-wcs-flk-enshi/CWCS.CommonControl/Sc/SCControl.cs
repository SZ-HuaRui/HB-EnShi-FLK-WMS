
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CWCS.CommonControl
{
    /// <summary>
    /// Author:zpw
    /// Description:Control for Sc
    /// </summary>
    public partial class SCControl : Control
    {
        /** 
         * SC：天轨、立柱、载货台、地轨、货叉、电控系统、轮子、滑梯
         * 
         * 
         */
        public SCControl()
        {
            InitializeComponent();
        }
        private const int _ColumnHeight = 200;
        private const int _GroundRaiHeight = 8;
        //载货台高度
        private const int _LoadPlatFormHight = 40;
        private int _SkyRailLength = 140;
        private Color _SkyRailColor = Color.FromArgb(255, 193, 37);
        private Color _ColumnColor = Color.FromArgb(255, 193, 37);
        private int _ColumnWidth = 40;
        private int _SlideWidth = 10;
        private Color _SlideColor = Color.Black;
        private Color _LoadPlatFormColor = Color.Black;
        private Color _CabinetColor = Color.Silver;// Color.FromArgb(205 ,205, 180);//205 205 180
        private Color _WheelColor = Color.Black;
        private string _SCNO = "01";
        private string _TaskNO = "12345";
        private string _Position = "S0010030021";
        private string _WorkMode = "自动；就绪；远程";
        private string _OutBoundMode = "可出可入";
        private string _ExecuteState = "取货中";
        private bool _IsAuto = true;
        private bool _IsConnected = false;
        private string _FromAddress = "0";
        private string _ToAddress = "0";
        private Color _GroundRailColor = Color.FromArgb(255, 193, 37);
        private int _GroundRailLength = 140;
        private Color _SCNOColor = Color.Blue;
        private ComponentPosition _LoadPlatFormPosition = ComponentPosition.Low;
        private Color _TaskNOColor = Color.Blue;
        private bool _isAlarm = false;
        private Color _WorkModeColor = Color.Blue;
        private Color _PositionColor = Color.Blue;
        private Color _ExecuteStateColor = Color.Blue;
        private Color _FromAndToColor = Color.Blue;
        private string _SCTaskInfoFont = "微软雅黑";
        private string _AlarmDescription = "货叉不在中位";
        private Color _AlarmColor = Color.Blue;
        private bool _isLoading = true;
        private Color _GoodsColor = Color.DarkGoldenrod;
        //public SCTaskInfo TaskInfo { get; set; } = new SCTaskInfo()
        //{
        //    TaskNO = "0",
        //    SCNO = "0",
        //    FromStation = "0",
        //    ToStation = "0",
        //    IsAlarm = false,
        //    IsConnected = false,
        //    Position = "0",
        //    WorkMode = SCWorkMode.Manual,
        //    OutBoundMode = SCOutBOundMode.All,
        //    AlarmDescription = string.Empty
        //};

        //堆垛机外观设计
        //天轨
        [Browsable(true)]
        [Category("外观")]
        [DefaultValue(50)]
        [Description("设置天轨的长度")]
        public int SkyRailLength
        {
            get => _SkyRailLength;
            set => _SkyRailLength = value;
        }

        [Browsable(true)]
        [Category("外观")]
        [DefaultValue(typeof(Color), "0x#409eff")]
        [Description("设置天轨的颜色")]
        public Color SkyRailColor
        {
            get => _SkyRailColor;
            set => _SkyRailColor = value;
        }

        /// <summary>
        /// 载货
        /// </summary>
        [Browsable(false)]
        public bool IsLoading
        {
            get => _isLoading;
            set => _isLoading = value;
        }

        public Color GoodsColor
        {
            get => _GoodsColor;
            set => _GoodsColor = value;
        }


        //天轨
        [Browsable(true)]
        [Category("外观")]
        [DefaultValue(50)]
        [Description("设置地轨的长度")]
        public int GroundRailLength
        {
            get => _GroundRailLength;
            set => _GroundRailLength = value;
        }

        [Browsable(true)]
        [Category("外观")]
        [DefaultValue(typeof(Color), "0x#409eff")]
        [Description("设置地轨的颜色")]
        public Color GroundRailColor
        {
            get => _GroundRailColor;
            set => _GroundRailColor = value;
        }

        [Browsable(true)]
        [Category("外观")]
        [DefaultValue(typeof(Color), "0x#409eff")]
        [Description("设置立柱的颜色")]
        public Color ColumnColor
        {
            get => _ColumnColor;
            set => _ColumnColor = value;
        }

        [Browsable(true)]
        [Category("外观")]
        [DefaultValue(20)]
        [Description("设置立柱的宽度")]
        public int ColumnWidth
        {
            get => _ColumnWidth;
            set => _ColumnWidth = value;
        }


        [Browsable(true)]
        [Category("外观")]
        [DefaultValue(20)]
        [Description("设置滑梯的宽度")]
        public int SlideWidth
        {
            get => _SlideWidth;
            set => _SlideWidth = value;
        }

        [Browsable(true)]
        [Category("外观")]
        [DefaultValue(typeof(Color), "0x#409eff")]
        [Description("设置滑梯的颜色")]
        public Color SlideColor
        {
            get => _SlideColor;
            set => _SlideColor = value;
        }


        [Browsable(true)]
        [Category("外观")]
        [DefaultValue(typeof(Color), "0x#409eff")]
        [Description("设置载货台的颜色")]
        public Color LoadPlatFormColor
        {
            get => _LoadPlatFormColor;
            set => _LoadPlatFormColor = value;
        }

        [Browsable(false)]
        [Description("设置载货台的位置")]
        public ComponentPosition LoadPlatFormPosition
        {
            get => _LoadPlatFormPosition;
            set => _LoadPlatFormPosition = value;
        }

        [Browsable(true)]
        [Category("外观")]
        [DefaultValue(typeof(Color), "0x#409eff")]
        [Description("设置电控柜的颜色")]
        public Color CabinetColor
        {
            get => _CabinetColor;
            set => _CabinetColor = value;
        }



        [Browsable(true)]
        [Category("外观")]
        [DefaultValue(typeof(Color), "0x#409eff")]
        [Description("设置轮子的颜色")]
        public Color WheelColor
        {
            get => _WheelColor;
            set => _WheelColor = value;
        }

        [Browsable(true)]
        [Category("参数")]
        [DefaultValue(typeof(Color), "0x#409eff")]
        [Description("设置堆垛机的编号")]
        public string SCNO
        {
            get => _SCNO;
            set => _SCNO = value;
        }


        [Browsable(true)]
        [Category("参数")]
        [DefaultValue(typeof(string), "01")]
        [Description("设置堆垛机编号的颜色")]
        public Color SCNOColor
        {
            get => _SCNOColor;
            set => _SCNOColor = value;
        }

        /// <summary>
        /// WCS任务号
        /// </summary>
        [Browsable(false)]
        public string TaskNO
        {
            get => _TaskNO;
            set => _TaskNO = value;
        }


        /// <summary>
        /// 任务颜色
        /// </summary>
        public Color TaskNOColor
        {
            get => _TaskNOColor;
            set => _TaskNOColor = value;
        }

        /// <summary>
        /// 堆垛机位置
        /// </summary>
        [Browsable(false)]
        public string Position
        {
            get => _Position;
            set => _Position = value;
        }

        /// <summary>
        /// 位置颜色
        /// </summary>
        [Browsable(false)]
        public Color PostionColor
        {
            get => _PositionColor;
            set => _PositionColor = value;
        }

        /// <summary>
        /// 堆垛机工作模式：自动；就绪；远程
        /// </summary>
        /// 
        [Browsable(false)]
        public string WorkMode
        {
            get => _WorkMode;
            set => _WorkMode = value;
        }

        /// <summary>
        /// 堆垛机连接状态：连接正常
        /// </summary>
        /// 
        [Browsable(false)]
        public bool IsConnected
        {
            get => _IsConnected;
            set => _IsConnected = value;
        }
        /// <summary>
        /// 工作模式颜色
        /// </summary>
        [Browsable(false)]
        public Color WorkModeColor
        {

            get => _WorkModeColor;
            set => _WorkModeColor = value;
        }

        
        /// <summary>
        /// 出库模式：可出可入
        /// </summary>
        [Browsable(false)]
        public string OutBoundMode
        {
            get => _OutBoundMode;
            set => _OutBoundMode = value;
        }

     

        /// <summary>
        /// 执行状态：空闲；取货中；放货中；
        /// </summary>
        [Browsable(false)]
        public string ExecuteState
        {
            get => _ExecuteState;
            set => _ExecuteState = value;
        }
        /// <summary>
        /// 执行状态颜色
        /// </summary>
        [Browsable(false)]
        public Color ExecuteStateColor
        {
            get => _ExecuteStateColor;
            set => _ExecuteStateColor = value;
        }

        /// <summary>
        /// 是否自动
        /// </summary>
        [Browsable(false)]
        public bool IsAuto
        {
            get => _IsAuto;
            set => _IsAuto = value;
        }


        /// <summary>
        /// 堆垛机起始地址
        /// </summary>
        [Browsable(false)]
        public string FromAddress
        {
            get => _FromAddress;
            set => _FromAddress = value;
        }


        /// <summary>
        /// 堆垛机目标地址
        /// </summary>
        [Browsable(false)]
        public string ToAddress
        {
            get => _ToAddress;
            set => _ToAddress = value;
        }

        /// <summary>
        /// 执行状态颜色
        /// </summary>
        [Browsable(false)]
        public Color FromAndToColor
        {
            get => _FromAndToColor;
            set => _FromAndToColor = value;
        }

        /// <summary>
        /// 任务信息字体
        /// </summary>
        [Browsable(false)]
        public string SCTaskInfoFont
        {
            get => _SCTaskInfoFont;
            set => _SCTaskInfoFont = value;
        }

        /// <summary>
        /// 报警描述
        /// </summary>
        [Browsable(false)]
        public string AlarmDescription
        {
            get => _AlarmDescription;
            set => _AlarmDescription = value;
        }

        /// <summary>
        /// 报警颜色
        /// </summary>
        [Browsable(true)]
        public Color AlarmColor
        {
            get => _AlarmColor;
            set => _AlarmColor = value;
        }



        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = this.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            base.OnPaint(pe);
            DrawComlun(g);
            DrawSlide(g);
            DrawGroundRail(g);
            DrawWheel(g);
            DrawCabinet(g);
            DrawLoadPlatForm(g);
            DrawTaskInfo(g);
        }

        //画立柱
        private void DrawComlun(Graphics g)
        {
            Rectangle rectangle = new Rectangle(this.GroundRailLength/2,0,this.ColumnWidth,_ColumnHeight);
            g.FillRectangle(new SolidBrush(this.ColumnColor),rectangle);
            //立柱外侧线
            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddLines(new PointF[] {
                new Point(this.GroundRailLength / 2, 0),
                new Point(this.GroundRailLength / 2 + this.ColumnWidth, 0),
                new Point(this.GroundRailLength / 2 + this.ColumnWidth, _ColumnHeight),
                new Point(this.GroundRailLength / 2, _ColumnHeight),
                new Point(this.GroundRailLength / 2, 0),

            });
            g.DrawPath(new Pen(Color.DarkGoldenrod, 2),graphicsPath);
        }

        

        //画滑梯
        private void DrawSlide(Graphics g)
        {
            Point pLeft = new Point(this.ColumnWidth-this.SlideWidth+this.GroundRailLength/2, 0);
            Point pRight = new Point(this.GroundRailLength/2+this.ColumnWidth, 0);
            for (int i = 0; pLeft.Y < _ColumnHeight; i++)
            {
                g.DrawLine(new Pen(Color.Black), pLeft, pRight);
                pLeft = new Point(pLeft.X, pLeft.Y + 5);
                pRight = new Point(pRight.X, pRight.Y + 5);
            }
            g.DrawLine(new Pen(Color.Black), this.ColumnWidth - this.SlideWidth + this.GroundRailLength / 2, 0, this.ColumnWidth - this.SlideWidth + this.GroundRailLength / 2, _ColumnHeight);
            g.DrawLine(new Pen(Color.Black), this.GroundRailLength / 2 + this.ColumnWidth, 0, this.GroundRailLength / 2 + this.ColumnWidth, _ColumnHeight);
        }

        //画载货台
        private void DrawLoadPlatForm(Graphics g)
        {

            if (this.LoadPlatFormPosition == ComponentPosition.High)
            {
               

            }
            if (this.LoadPlatFormPosition == ComponentPosition.Middle)
            {

            }
            if (this.LoadPlatFormPosition == ComponentPosition.Low)
            {
                g.DrawLine(new Pen(new SolidBrush(this.LoadPlatFormColor),4), this.GroundRailLength / 6, _ColumnHeight-2, this.GroundRailLength / 2 - 10, _ColumnHeight-2);
                g.DrawLine(new Pen(new SolidBrush(this.LoadPlatFormColor),4), this.GroundRailLength / 6, _ColumnHeight-2, this.GroundRailLength / 6, _ColumnHeight - _LoadPlatFormHight);
                g.DrawLine(new Pen(new SolidBrush(this.LoadPlatFormColor),4), this.GroundRailLength / 2 - 10, _ColumnHeight-2, this.GroundRailLength / 2 - 10, _ColumnHeight - _LoadPlatFormHight);
                g.DrawLine(new Pen(new SolidBrush(this.LoadPlatFormColor), 4), this.GroundRailLength / 2 - 10, _ColumnHeight - 2-_LoadPlatFormHight/3, this.GroundRailLength / 2, _ColumnHeight - 2 - _LoadPlatFormHight / 3);
                g.DrawLine(new Pen(new SolidBrush(this.LoadPlatFormColor), 4), this.GroundRailLength / 2 - 10, _ColumnHeight - 2 - _LoadPlatFormHight * 2 / 3, this.GroundRailLength / 2, _ColumnHeight - 2 - _LoadPlatFormHight * 2 / 3);

                if (this.IsLoading)
                {
                    g.FillRectangle(new SolidBrush(this.GoodsColor), this.GroundRailLength / 6 + 2, _ColumnHeight - _LoadPlatFormHight - 2 + 4, _LoadPlatFormHight-6, _LoadPlatFormHight - 6);
                }
            }

        }

        //画电控柜
        private void DrawCabinet(Graphics g)
        {
            Rectangle rectangle = new Rectangle(this.GroundRailLength/2+this.ColumnWidth+2,_ColumnHeight*3/4,this.GroundRailLength/4-10, _ColumnHeight/4);
            g.FillRectangle(new SolidBrush(this.CabinetColor),rectangle);

            //画堆垛机编号
            StringFormat s = new StringFormat();
            s.Alignment = StringAlignment.Center;
            s.LineAlignment = StringAlignment.Center;
            g.DrawString(this.SCNO, new Font("Verdana",10,FontStyle.Bold), new SolidBrush(this.SCNOColor), rectangle, s);

        }

        //画轮子
        private void DrawWheel(Graphics g)
        {
            Rectangle rectangle = new Rectangle(20,_ColumnHeight+_GroundRaiHeight/2, _GroundRaiHeight, _GroundRaiHeight);
            g.FillEllipse(new SolidBrush(this.WheelColor), rectangle);
            rectangle = new Rectangle(_GroundRailLength - 20, _ColumnHeight + _GroundRaiHeight / 2, _GroundRaiHeight, _GroundRaiHeight);
            g.FillEllipse(new SolidBrush(this.WheelColor), rectangle);
        }

        /// <summary>
        /// 画地轨
        /// </summary>
        /// <param name="g"></param>
        private void DrawGroundRail(Graphics g)
        {
            Rectangle rectangle = new Rectangle(0,_ColumnHeight,this.GroundRailLength,_GroundRaiHeight);
            g.FillRectangle(new SolidBrush(this.GroundRailColor),rectangle);
            //地轨外侧线
            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddLines(new PointF[] {
                new Point(0,_ColumnHeight),
                new Point(this.GroundRailLength, _ColumnHeight),
                new Point(this.GroundRailLength, _GroundRaiHeight+_ColumnHeight),
                new Point(0, _GroundRaiHeight+_ColumnHeight),
                new Point(0,_ColumnHeight),

            });
            g.DrawPath(new Pen(Color.DarkGoldenrod, 2), graphicsPath);
        }

        /// <summary>
        /// 画任务信息
        /// </summary>
        /// <param name="g"></param>
        private void DrawTaskInfo(Graphics g)
        {
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Center;
            Rectangle rectangle = new Rectangle(0,_ColumnHeight+_GroundRaiHeight+20,this.Width,this.Height - _ColumnHeight - _GroundRaiHeight);
            g.DrawString(SCPrefix._TaskNO+this.TaskNO, new Font(this.SCTaskInfoFont,10,FontStyle.Regular),new SolidBrush(this.TaskNOColor),0, _ColumnHeight + _GroundRaiHeight + 20,stringFormat);
            g.DrawString(SCPrefix._WorkMode + this.WorkMode, new Font(this.SCTaskInfoFont, 10, FontStyle.Regular), new SolidBrush(this.WorkModeColor), 0, _ColumnHeight + _GroundRaiHeight + 20 * 2, stringFormat);
            g.DrawString(SCPrefix._ConnectionState + (this._IsConnected ? "正常":"断开"), new Font(this.SCTaskInfoFont, 10, FontStyle.Regular), (this._IsConnected ? new SolidBrush(Color.Lime) : new SolidBrush(Color.Red)), 0, _ColumnHeight + _GroundRaiHeight + 20 * 3, stringFormat);
            g.DrawString(SCPrefix._Position + this.Position, new Font(this.SCTaskInfoFont, 10, FontStyle.Regular), new SolidBrush(this.PostionColor), 0, _ColumnHeight + _GroundRaiHeight + 20 * 4, stringFormat);
            g.DrawString(SCPrefix._ExecuteState + this.ExecuteState, new Font(this.SCTaskInfoFont, 10, FontStyle.Regular), new SolidBrush(this.ExecuteStateColor), 0, _ColumnHeight + _GroundRaiHeight + 20 * 5, stringFormat);
            g.DrawString(SCPrefix._FromAndTo + $"{this.FromAddress}-->{this.ToAddress}", new Font(this.SCTaskInfoFont, 10, FontStyle.Regular), new SolidBrush(this.FromAndToColor), 0, _ColumnHeight + _GroundRaiHeight + 20 * 6, stringFormat);
            g.DrawString(SCPrefix._AlarmDescription + this.AlarmDescription, new Font(this.SCTaskInfoFont, 10, FontStyle.Regular), (this._isAlarm ? new SolidBrush(Color.Red) : new SolidBrush(Color.Blue)), 0, _ColumnHeight + _GroundRaiHeight + 20 * 7, stringFormat);
        }


    }


    //组件位置
    public enum ComponentPosition : int
    {
        Low = 0,
        Middle = 1,
        High = 2
    }

    /// <summary>
    /// 堆垛机描述前缀
    /// </summary>
    public class SCPrefix
    {
        public static string _TaskNO = @"任务号：";
        public static string _WorkMode = @"工作模式：";
        public static string _Position = @"位置：";
        public static string _WorkState = @"工作状态：";
        public static string _ConnectionState = @"连接状态：";
        public static string _ExecuteState = @"执行状态：";
        public static string _FromAndTo = @"起止：";
        public static string _AlarmDescription = @"报警：";
    }


}
