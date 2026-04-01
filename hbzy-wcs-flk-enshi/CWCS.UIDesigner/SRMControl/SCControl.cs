using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Text.RegularExpressions;

namespace CWCS.UIDesigner.SRMControl
{
    public partial class SCControl : Panel
    {
        public int unitHeight = 240;
        public int unitWidth = 140;

        public List<RGVObject> Units = new List<RGVObject>();//需要绘制的对象集合

        Point? m_MousedownPoint = null;
        Point? m_lastPoint;
        Rectangle? m_MousedownRect;
        Rectangle? m_lastRect;
        public event MouseEventHandler SCMouseClick;
        public event MouseEventHandler SCMouseDown;
        public event MouseEventHandler SCMouseUp;

        public event EventHandler SCMouseEnter;
        public event EventHandler SCMouseLeave;

        public event EventHandler SCChangWCSWorMode;
        private System.Timers.Timer t_Timer = new System.Timers.Timer();

        public SCControl()
        {
            InitializeComponent();
            t_Timer.Interval = 1000;
            t_Timer.Elapsed += new ElapsedEventHandler(t_Timer_Elapsed);
            t_Timer.Start();
        }
        private void t_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Invalidate();
        }

        public bool Contains(RGVObject unit)
        {
            return Units.Contains(unit);
        }
        public void Add(RGVObject unit)
        {
            if (!Units.Exists(it => it.M_Num == unit.M_Num))
            {
                Units.Add(unit);
                unit.Parent = this;
                ResizeScrollMinSize();
            }
        }

        public void Remove(RGVObject unit)
        {
            Units.Remove(unit);
            unit.Parent = null;
            ResizeScrollMinSize();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            DrawUnits(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_MousedownPoint = GetPointCurr();
            m_lastPoint = m_MousedownPoint;
            m_MousedownRect = GetRectCurr();
            m_lastRect = m_MousedownRect;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            this.Focus();
            Point p = GetPointCurr();
            try
            {
                Rectangle rectCurr = GetRectCurr();
                if (m_lastRect.Value == rectCurr)
                {
                    RGVObject[] us = Units.Where(v => v.GetRect().Contains(p)).ToArray();
                    if (us.Length == 0)
                        return;
                    Rectangle rect = us[0].GetRect();
                    if (rect != null)
                    {
                        if (p.X >= rect.X && p.X <= rect.X + 36 && p.Y >= rect.Y && p.Y <= rect.Y + 20)
                        {
                            if (us[0].M_WCSControlState == RGVObject.WCSControlState.Auto)
                                us[0].M_WCSControlState = RGVObject.WCSControlState.Man;
                            else if (us[0].M_WCSControlState == RGVObject.WCSControlState.Man)
                                us[0].M_WCSControlState = RGVObject.WCSControlState.Auto;

                            if (SCChangWCSWorMode != null)
                            {
                                SCChangWCSWorMode(us[0], e);
                            }
                        }

                        SolidBrush brush = new SolidBrush(this.BackColor);
                        rect.Inflate(25, 10);
                        Graphics.FromHwnd(this.Handle).FillRectangle(brush, rect);

                        us[0].DrawUnit(us[0].GetRect(), Graphics.FromHwnd(this.Handle));

                        if (e.Button == MouseButtons.Right)
                        {
                            if (SCMouseClick != null && us.Length > 0)
                            {
                                SCMouseClick(us[0], e);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误提示：" + ex.Message, "提示");
            }
            base.OnMouseClick(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (MouseButtons == MouseButtons.Left)
            {

                if (m_MousedownPoint == null)
                    return;
                Rectangle rect = GetRectCurr();
                m_lastRect = rect;
            }
            base.OnMouseMove(e);
        }

        void DrawUnits(PaintEventArgs e)
        {
            BufferedGraphicsContext GraphicsContext = BufferedGraphicsManager.Current;
            BufferedGraphics myBuffer = GraphicsContext.Allocate(e.Graphics, e.ClipRectangle);//获取缓冲区
            Graphics g = myBuffer.Graphics;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;//高质量低速度呈现
            g.SmoothingMode = SmoothingMode.HighQuality;// 指定高质量、低速度呈现
            g.Clear(BackColor);
            int i = 0;
            foreach (RGVObject u in Units)
            {
                u.DrawUnit(new Rectangle(30 + 260 * u.XIndex, 30, unitWidth, unitHeight), g);
                u.DrawUnitInfo(new Rectangle(u.GetRect().X, u.GetRect().Y + u.GetRect().Height + 20, u.GetRect().Width+220, unitHeight+120), g);
                i++;
            }

            myBuffer.Render(e.Graphics);  //将图形缓冲区的内容写入指定的Graphics对象。
            g.Dispose();
            myBuffer.Dispose();
        }

        Point GetPointCurr()
        {
            Point p = this.PointToClient(MousePosition);
            p.X -= this.AutoScrollPosition.X + Padding.Left;
            p.Y -= this.AutoScrollPosition.Y + Padding.Top;
            return p;
        }

        Rectangle GetRectCurr()
        {
            Point p = this.PointToClient(MousePosition);
            p.X -= this.AutoScrollPosition.X + Padding.Left;
            p.Y -= this.AutoScrollPosition.Y + Padding.Top;
            return new Rectangle(p.X - p.X % unitWidth, p.Y - p.Y % unitHeight, unitWidth, unitHeight);
        }

        void ResizeScrollMinSize()
        {

            int maxX = 0;
            int maxY = 0;
            foreach (RGVObject u in Units)
            {
                if (u.GetRect().X + u.GetRect().Width > maxX)
                    maxX = u.GetRect().X + u.GetRect().Width;
                if (u.GetRect().Y + u.GetRect().Height > maxY)
                    maxY = u.GetRect().Y + u.GetRect().Height;
            }
            AutoScrollMinSize = new Size(maxX + 100, maxY + 100);
            Refresh();
        }

    }

    public abstract class RGVObject
    {
        #region Enum
        /// <summary>
        /// 工作模式
        /// </summary>
        public enum WorkMode
        {
            Auto = 1,
            Man = 2
        }
        /// <summary>
        /// 连接状态
        /// </summary>
        public enum ConncetState
        {
            Connected = 1,
            Disconnect = 2
        }
        /// <summary>
        /// 设备状态
        /// </summary>
        public enum EquipmentState
        {
            Equipment_OK = 1,
            Equipment_Error = 2
        }

        public enum LoadState
        {
            Unload = 0,//无货
            Putting = 1, //放货中
            Putted = 2,//放货完成
            Picking = 3,//取货中
            Picked = 4,//取货完成
            Loading = 5//载货
        }

        public enum WCSControlState
        {
            Auto = 1,
            Man = 2
        }

        public enum WCSControlTaskMode
        {
            All = 0,
            OnlyInFeed = 1,
            OnlyOutFeed = 2,
            Pause=3
        }
        public enum WCSTaskType
        {
            None = 0,
            Task_InBound = 1,
            Task_OutBound = 2,
            Task_Transfer = 3,
            Task_OnFire = 4
        }
        #endregion

        #region Properties
        //连接状态
        private ConncetState _m_ConnectState = ConncetState.Connected;

        public ConncetState M_ConnectState
        {
            get { return _m_ConnectState; }
            set { _m_ConnectState = value; }
        }

        //工作模式
        private WorkMode _m_WorkMode;

        public WorkMode M_WorkMode
        {
            get { return _m_WorkMode; }
            set { _m_WorkMode = value; }
        }

        //执行中任务号
        private string _m_WorkTaskno;

        public string M_WorkTaskno
        {
            get { return _m_WorkTaskno; }
            set { _m_WorkTaskno = value; }
        }

        //设备状态
        private EquipmentState _m_EquipmentState;

        public EquipmentState M_EquipmentState
        {
            get { return _m_EquipmentState; }
            set { _m_EquipmentState = value; }
        }
        private LoadState _m_LoadState = LoadState.Unload;

        public LoadState M_LoadState
        {
            get { return _m_LoadState; }
            set { _m_LoadState = value; }
        }
        private WCSControlState _m_WCSControlState = WCSControlState.Auto;

        public WCSControlState M_WCSControlState
        {
            get { return _m_WCSControlState; }
            set { _m_WCSControlState = value; }
        }
        private WCSControlTaskMode _m_WCSTaskMode = WCSControlTaskMode.All;

        public WCSControlTaskMode M_WCSTaskMode
        {
            get { return _m_WCSTaskMode; }
            set { _m_WCSTaskMode = value; }
        }

        private WCSTaskType _m_WCSTaskType = WCSTaskType.None;

        public WCSTaskType M_WCSTaskType
        {
            get { return _m_WCSTaskType; }
            set { _m_WCSTaskType = value; }
        }

        private Color backColor = Color.FromArgb(255, 165, 0);
        public Color BackColor { get { return backColor; } set { backColor = value; } }
        private Color foreColor = Color.Black;
        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }

        private Rectangle rectangle;
        private Rectangle rectofParent;

        private int m_Coulmn = 1;

        public int Coulmn
        {
            get { return m_Coulmn; }
            set { m_Coulmn = value; }
        }
        private string _m_Num = "01";

        public string M_Num
        {
            get { return _m_Num; }
            set { _m_Num = value; }
        }

        private int _m_SCIndex = 0;

        public int M_SCIndex
        {
            get { return _m_SCIndex; }
            set { _m_SCIndex = value; }
        }
        private string _m_SCNum;

        public string M_SCNum
        {
            get { return _m_SCNum; }
            set { _m_SCNum = value; }
        }
        /// <summary>
        /// 设备信息
        /// </summary>
        private string _m_EquipmentInfo;

        public string M_EquipmentInfo
        {
            get { return _m_EquipmentInfo; }
            set { _m_EquipmentInfo = value; }
        }

        /// <summary>
        /// 位置信息
        /// </summary>
        private string _m_PositionInfo;
        public string M_PositionInfo
        {
            get { return _m_PositionInfo; }
            set { _m_PositionInfo = value; }
        }

        /// <summary>
        /// 动作信息
        /// </summary>
        private string _m_ActionStatus;
        public string M_ActionStatus
        {
            get { return _m_ActionStatus; }
            set { _m_ActionStatus = value; }
        }

        private const string entryRectangle = "Rect";

        protected Rectangle Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }
        private SCControl parent = null;
        public SCControl Parent
        {
            get { return parent; }
            set
            {
                if (parent != null)
                    parent.Remove(this);
                parent = value;
                parent.Add(this);
            }
        }

        public Rectangle RectofParent
        {
            get { return rectofParent; }
            set { rectofParent = value; }
        }

        private Point? lastAutoScrollPosition;
        private int x = -1;
        private int y = -1;

        public int XIndex
        {
            get { return x; }
            set
            {
                x = value;
                lastAutoScrollPosition = new Point(1);
            }
        }
        public int YIndex
        {
            get { return y; }
            set
            {
                y = value;
                lastAutoScrollPosition = new Point(1);
            }
        }
        public int WidthColumns;
        public int HeightRows;
        #endregion

        public abstract void DrawUnit(Rectangle Rect, Graphics g);

        public abstract void DrawUnitInfo(Rectangle Rect, Graphics g);

        public Rectangle GetRect()
        {
            // if (lastAutoScrollPosition != parent.AutoScrollPosition)
            {
                rectangle = new Rectangle(parent.Padding.Left + XIndex * (parent.unitWidth + 120) + 30,
                    parent.Padding.Top + YIndex * parent.unitHeight + 30, parent.unitWidth, parent.unitHeight);
                lastAutoScrollPosition = parent.AutoScrollPosition;

                rectofParent = new Rectangle(parent.Padding.Left + XIndex * (parent.unitWidth + 50) + 30 + parent.AutoScrollPosition.X,
                      parent.Padding.Top + YIndex * parent.unitHeight + 30 + parent.AutoScrollPosition.Y, parent.unitWidth, parent.unitHeight);
                lastAutoScrollPosition = parent.AutoScrollPosition;
            }
            return rectangle;
        }

        public Rectangle GetRectOfParent()
        {
            //  if (lastAutoScrollPosition != parent.AutoScrollPosition)
            {
                rectangle = new Rectangle(parent.Padding.Left + XIndex * (parent.unitWidth + 50) + 30,
                     parent.Padding.Top + YIndex * parent.unitHeight + 30, WidthColumns * parent.unitWidth, HeightRows * parent.unitHeight);
                lastAutoScrollPosition = parent.AutoScrollPosition;

                rectofParent = new Rectangle(parent.Padding.Left + XIndex * (parent.unitWidth + 50) + 30 + parent.AutoScrollPosition.X,
                      parent.Padding.Top + YIndex * parent.unitHeight + 30 + parent.AutoScrollPosition.Y, WidthColumns * parent.unitWidth, HeightRows * parent.unitHeight);
                lastAutoScrollPosition = parent.AutoScrollPosition;
                //lastlw = parent.unitHeight;
            }
            return rectofParent;
        }
    }

    public class RGVUnit : RGVObject
    {

        public override void DrawUnit(Rectangle Rect, Graphics g)
        {

            SolidBrush brush = new SolidBrush(M_ConnectState == ConncetState.Connected ? BackColor : Color.Red);
            Rectangle resBorder = new System.Drawing.Rectangle(Rect.X, Rect.Y, Rect.Width,Rect.Height);
            resBorder.Inflate(1, 5);
            g.DrawRectangle(new Pen(new SolidBrush((M_WCSTaskType == WCSTaskType.Task_OnFire) ? Color.Red : Color.Transparent), 2), resBorder);

            Rectangle resOperation = new System.Drawing.Rectangle(Rect.X-10, Rect.Y, 65, 20);
            ControlPaint.DrawButton(g, resOperation, ButtonState.Normal);
            // g.DrawRectangle(new Pen(brush = new SolidBrush(Color.DarkGray), 1), Rect);
            if (M_WCSControlState == WCSControlState.Auto)
            {
                g.DrawString("自动模式", new Font(new FontFamily("宋体"), 10, FontStyle.Bold), new SolidBrush(BackColor), resOperation.X + 1, resOperation.Y + resOperation.Height / 2 - 7);
            }
            else
            {
                g.DrawString("手动控制", new Font(new FontFamily("宋体"), 10, FontStyle.Bold), new SolidBrush(Color.Red), resOperation.X + 1, resOperation.Y + resOperation.Height / 2 - 7);
            }
            //三色灯
            Rectangle resRed = new System.Drawing.Rectangle(Rect.X, Rect.Y+30, 15, 17);
            g.FillRectangle(new SolidBrush(M_WorkMode==WorkMode.Auto?Color.LawnGreen:Color.Yellow), resRed);
            ControlPaint.DrawBorder3D(g, resRed, Border3DStyle.RaisedOuter);
            Rectangle resGreen = new System.Drawing.Rectangle(Rect.X, Rect.Y + 45, 15, 17);
            g.FillRectangle(new SolidBrush(M_EquipmentState == EquipmentState.Equipment_Error ? Color.Red : Color.LawnGreen), resGreen);
            ControlPaint.DrawBorder3D(g, resGreen, Border3DStyle.RaisedOuter);

            //堆垛机载体
            Rectangle rest = new System.Drawing.Rectangle(Rect.X + 5, Rect.Y + Rect.Height - 10, Rect.Width - 10, 10);

            g.FillRectangle(brush, rest);
            ControlPaint.DrawBorder3D(g, rest, Border3DStyle.RaisedOuter);

            //堆垛机滚轮
            Rectangle restWheelL = new System.Drawing.Rectangle(rest.X + rest.Width / 4 - 10, Rect.Y + Rect.Height - 5, 9, 9);
            Rectangle restWheelR = new System.Drawing.Rectangle(rest.X + (rest.Width / 4) * 3, Rect.Y + Rect.Height - 5, 9, 9);

            g.FillEllipse(brush = new SolidBrush(Color.Black), restWheelL);

            restWheelL.Inflate(1, 1);
            g.DrawEllipse(new Pen(brush = new SolidBrush(Color.DarkGray), 1), restWheelL);

            g.FillEllipse(brush = new SolidBrush(Color.Black), restWheelR);
            restWheelR.Inflate(1, 1);
            g.DrawEllipse(new Pen(brush = new SolidBrush(Color.DarkGray), 1), restWheelR);

            //堆垛机悬臂
            Rectangle restBracket = new System.Drawing.Rectangle(Rect.X + Rect.Width / 2, Rect.Y - 2, 25, Rect.Height - 10);
            g.FillRectangle(brush = new SolidBrush(M_ConnectState == ConncetState.Connected ? BackColor : Color.Red), restBracket);
            restBracket.Inflate(2, 2);
            ControlPaint.DrawBorder3D(g, restBracket, Border3DStyle.RaisedInner);

            //电控柜
            Rectangle reBox = new System.Drawing.Rectangle(restBracket.X + restBracket.Width + 2, restBracket.Y + restBracket.Height - 62, 30, 60);
            g.FillRectangle(brush = new SolidBrush(Color.DarkGray), reBox);
            reBox.Inflate(2, 2);
            ControlPaint.DrawBorder3D(g, reBox, Border3DStyle.RaisedInner);

            //堆垛机编号
            g.DrawString(M_Num, new Font(new FontFamily("宋体"), 15, FontStyle.Bold), new SolidBrush(Color.Blue), reBox.X + reBox.Width / 2 - 15+1, reBox.Y + reBox.Height / 2 - 4);

            //载货台
            Rectangle restLoad = new System.Drawing.Rectangle(Rect.X + Rect.Width / 2 - 10, Rect.Y + Rect.Height - 40 - Coulmn, 6, 25);
            g.FillRectangle(brush = new SolidBrush(Color.Black), restLoad);
            g.FillRectangle(brush = new SolidBrush(Color.Black), new Rectangle(restLoad.X - 40, restLoad.Y + restLoad.Height - 1 - Coulmn, 50, 6));
            g.FillRectangle(brush = new SolidBrush(Color.Black), new Rectangle(restLoad.X - 33 + restLoad.Width / 3, restLoad.Y + restLoad.Height - 4 - Coulmn, 24, 5));


            if (M_LoadState != LoadState.Unload)
            {
                //货物
                Rectangle reGoods = new Rectangle(restLoad.X - 37, restLoad.Y + restLoad.Height - 23 - Coulmn, 34, 20);
                g.DrawRectangle(new Pen(brush = new SolidBrush(Color.Gray), 2), reGoods);
                g.DrawLine(new Pen(brush = new SolidBrush(Color.Gray), 2), reGoods.X, reGoods.Y, reGoods.X + reGoods.Width, reGoods.Y + reGoods.Height);
                g.DrawLine(new Pen(brush = new SolidBrush(Color.Gray), 2), reGoods.X, reGoods.Y + reGoods.Height, reGoods.X + reGoods.Width, reGoods.Y);

                if (M_LoadState == LoadState.Picking)
                {
                    DrawArrow(reGoods, g, "L");
                }
                else if (M_LoadState == LoadState.Putting)
                {
                    DrawArrow(reGoods, g, "R");
                }
            }
            else
            {
                Rectangle reGoods = new Rectangle(restLoad.X - 37, restLoad.Y + restLoad.Height - 23 - Coulmn, 34, 20);
                g.DrawRectangle(new Pen(brush = new SolidBrush(Color.Transparent), 2), reGoods);
                g.DrawLine(new Pen(brush = new SolidBrush(Color.Transparent), 2), reGoods.X, reGoods.Y, reGoods.X + reGoods.Width, reGoods.Y + reGoods.Height);
                g.DrawLine(new Pen(brush = new SolidBrush(Color.Transparent), 2), reGoods.X, reGoods.Y + reGoods.Height, reGoods.X + reGoods.Width, reGoods.Y);
            }
            //g.DrawString(M_PositionInfo, new Font(new FontFamily("宋体"), 10, FontStyle.Regular), new SolidBrush(Color.BlueViolet), restWheelL.X - 27, restWheelL.Y + restWheelL.Height / 2 + 12);
            //g.DrawString(M_ActionStatus, new Font(new FontFamily("宋体"), 10, FontStyle.Regular), new SolidBrush(Color.BlueViolet), restWheelL.X - 27, restWheelL.Y + restWheelL.Height / 2 + 32);

            DrawConnect(Rect, g);

        }

        //画箭头
        private void DrawArrow(Rectangle res, Graphics g, string arrowDirect)
        {
            Pen pen = new Pen(Color.LawnGreen, 5);
            if (arrowDirect == "L")//向右
            {
                pen.StartCap = LineCap.ArrowAnchor;
                pen.EndCap = LineCap.Round;
            }
            else//向右
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.ArrowAnchor;
            }
            g.DrawLine(pen, res.X - 10, res.Y + res.Height / 2, res.X - 25, res.Y + res.Height / 2);
        }

        private void DrawConnect(Rectangle res, Graphics g)
        {
            Rectangle reWCS = new Rectangle(res.X + res.Width - 35, res.Y, 25, 20);
            SolidBrush brush = new SolidBrush(Color.Gray);
            g.DrawRectangle(new Pen(brush = new SolidBrush(Color.Gray), 2), reWCS);
            reWCS.Inflate(-1, -1);
            g.FillRectangle(brush = new SolidBrush(Color.LightGray), reWCS);
            g.DrawString("WCS", new Font(new FontFamily("宋体"), 8, FontStyle.Bold), brush = new SolidBrush(Color.DarkGreen), reWCS.X, reWCS.Y + 3);
            Rectangle reWCS1 = new Rectangle(reWCS.X + reWCS.Width / 4, reWCS.Y + reWCS.Height + 2, (reWCS.Width * 3) / 5, 3);
            g.FillRectangle(brush = new SolidBrush(Color.DarkGray), reWCS1);
            Rectangle reWCS2 = new Rectangle(reWCS.X, reWCS.Y + reWCS.Height + 2 + reWCS1.Height, reWCS.Width, 5);
            g.FillRectangle(brush = new SolidBrush(Color.DarkGray), reWCS2);
            Pen pen = new Pen(M_ConnectState == ConncetState.Connected ? Color.ForestGreen : Color.Gray, 5);
            pen.DashStyle = DashStyle.DashDot;
            pen.StartCap = LineCap.ArrowAnchor;
            pen.EndCap = LineCap.ArrowAnchor;
            g.DrawLine(pen, reWCS.X + reWCS.Width / 2, reWCS.Y + reWCS.Height + 10, reWCS.X + reWCS.Width / 2, res.Height - (reWCS.Y + reWCS.Height) + 5);
        }

        public override void DrawUnitInfo(Rectangle Rect, Graphics g)
        {
            Rectangle reInfo = Rect;
            SolidBrush brush = new SolidBrush(Color.Transparent);
            g.FillRectangle(brush, reInfo);
            if (M_EquipmentState == EquipmentState.Equipment_Error)
            {
                g.DrawString(M_EquipmentInfo, new Font(new FontFamily("宋体"), 10, FontStyle.Bold), new SolidBrush(Color.Red), reInfo);
            }
            else
            {
                if (!string.IsNullOrEmpty(M_EquipmentInfo))
                {
                    if (!M_EquipmentInfo.Contains("可出可入"))
                    {
                        M_EquipmentInfo += "\r警告:当前模式非【可出可入】";
                    }
                    g.DrawString(M_EquipmentInfo, new Font(new FontFamily("宋体"), 10, FontStyle.Bold), new SolidBrush(Color.Blue), reInfo);
                }
              

            }
        }
    }
}
