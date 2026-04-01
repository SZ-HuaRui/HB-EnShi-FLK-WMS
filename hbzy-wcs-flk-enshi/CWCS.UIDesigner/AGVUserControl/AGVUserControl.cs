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

namespace CWCS.UIDesigner.AGVUserControl
{
    public partial class AGVUserControl : Panel
    {
        public int unitHeight = 200;
        public int unitWidth = 250;

        public List<AGVObject> Units = new List<AGVObject>();//需要绘制的对象集合
 
        private System.Timers.Timer t_Timer = new System.Timers.Timer();

        public AGVUserControl()
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

        public bool Contains(AGVObject unit)
        {
            return Units.Contains(unit);
        }
        public void Add(AGVObject unit)
        {
            if (!Units.Exists(it => it.M_CarNo == unit.M_CarNo))
            {
                Units.Add(unit);
                unit.Parent = this;
                //ResizeScrollMinSize();
            }
        }

        public void Remove(AGVObject unit)
        {
            Units.Remove(unit);
            unit.Parent = null;
            //ResizeScrollMinSize();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            DrawUnits(e);
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
            foreach (AGVObject u in Units)
            {
                u.DrawUnit(new Rectangle(150+280 * u.XIndex, 30, unitWidth, unitHeight), g);
                u.DrawUnitInfo(new Rectangle(u.GetRect().X, u.GetRect().Y + u.GetRect().Height + 20, u.GetRect().Width, unitHeight+100), g);
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
            foreach (AGVObject u in Units)
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

    public abstract class AGVObject
    {
        #region Enum
        /// <summary>
        /// 工作模式
        /// </summary>
        public enum WorkMode
        {
            Empty = 1,
            Work = 2,
            Wash = 3
        }
        /// <summary>
        /// 设备状态
        /// </summary>
        public enum EquipStatus
        {
            Ok = 0,
            Error = 1
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

        #endregion

        #region Properties


        //工作模式
        private WorkMode _m_WorkMode;

        public WorkMode M_WorkMode
        {
            get { return _m_WorkMode; }
            set { _m_WorkMode = value; }
        }

        //设备状态
        private EquipStatus _m_EquipStatus;

        public EquipStatus M_EquipStatus
        {
            get { return _m_EquipStatus; }
            set { _m_EquipStatus = value; }
        }
        private LoadState _m_LoadState = LoadState.Unload;

        public LoadState M_LoadState
        {
            get { return _m_LoadState; }
            set { _m_LoadState = value; }
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
     

        private int _m_Carno;

        public int M_CarNo
        {
            get { return _m_Carno; }
            set { _m_Carno = value; }
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

        public string taskno;
        public string fromstation;
        public string tostation;
        public string action;

        private const string entryRectangle = "Rect";

        protected Rectangle Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }
        private AGVUserControl parent = null;
        public AGVUserControl Parent
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
                rectangle = new Rectangle(parent.Padding.Left + XIndex * (parent.unitWidth / 4) + 30,
                    parent.Padding.Top + YIndex * parent.unitHeight + 30, parent.unitWidth, parent.unitHeight);
                lastAutoScrollPosition = parent.AutoScrollPosition;

                rectofParent = new Rectangle(parent.Padding.Left + XIndex * (parent.unitWidth / 4) + parent.AutoScrollPosition.X,
                      parent.Padding.Top + YIndex * parent.unitHeight + 30 + parent.AutoScrollPosition.Y, parent.unitWidth, parent.unitHeight);
                lastAutoScrollPosition = parent.AutoScrollPosition;
            }
            return rectangle;
        }

        public Rectangle GetRectOfParent()
        {
            //  if (lastAutoScrollPosition != parent.AutoScrollPosition)
            {
                rectangle = new Rectangle(parent.Padding.Left + XIndex * (parent.unitWidth / 3) + 30,
                parent.Padding.Top + YIndex * parent.unitHeight + 30, WidthColumns * parent.unitWidth, HeightRows * parent.unitHeight);
                lastAutoScrollPosition = parent.AutoScrollPosition;

                rectofParent = new Rectangle(parent.Padding.Left + XIndex * (parent.unitWidth / 3) + 30 + parent.AutoScrollPosition.X,
                      parent.Padding.Top + YIndex * parent.unitHeight + 30 + parent.AutoScrollPosition.Y, WidthColumns * parent.unitWidth, HeightRows * parent.unitHeight);
                lastAutoScrollPosition = parent.AutoScrollPosition;
                //lastlw = parent.unitHeight;
            }
            return rectofParent;
        }
    }

    public class AGVUnit : AGVObject
    {

        public override void DrawUnit(Rectangle Rect, Graphics g)
        {

            SolidBrush brush = new SolidBrush(Color.Gray);
            Rectangle resBorder = new System.Drawing.Rectangle(Rect.X/2 - 40, Rect.Y, Rect.Width/3,Rect.Height/2);
            resBorder.Inflate(3, 5);

            Rectangle resOperation = new System.Drawing.Rectangle(Rect.X/2-40, Rect.Y, 40, 30);
            g.FillRectangle(brush, resOperation);
            ControlPaint.DrawButton(g, resOperation, ButtonState.Normal);
            g.DrawString(M_EquipStatus == EquipStatus.Ok ? "正常" : "故障", new Font(new FontFamily("宋体"), 10, FontStyle.Bold), new SolidBrush(M_EquipStatus == EquipStatus.Ok ? Color.Green : Color.Red), resOperation.X +1, resOperation.Y + resOperation.Height / 2 - 7);
            // g.DrawRectangle(new Pen(brush = new SolidBrush(Color.DarkGray), 1), Rect);

            //AGV载货
            Rectangle rest = new System.Drawing.Rectangle(resOperation.X - 27, resOperation.Y + Rect.Height/2-30, 85, 90);
            if (!string.IsNullOrEmpty(taskno))
            {
                g.FillRectangle(brush, rest);
                ControlPaint.DrawBorder3D(g, rest, Border3DStyle.RaisedOuter);
                g.DrawString("任务:" + taskno, new Font(new FontFamily("宋体"), 10, FontStyle.Bold), new SolidBrush(BackColor), rest.X + 1, rest.Y + rest.Height / 2 - 35);
                g.DrawString("取货:" + fromstation, new Font(new FontFamily("宋体"), 10, FontStyle.Bold), new SolidBrush(BackColor), rest.X + 1, rest.Y + rest.Height / 2 - 15);
                g.DrawString("放货:" + tostation, new Font(new FontFamily("宋体"), 10, FontStyle.Bold), new SolidBrush(BackColor), rest.X + 1, rest.Y + rest.Height / 2 + 5);
                g.DrawString( action, new Font(new FontFamily("宋体"), 10, FontStyle.Bold), new SolidBrush(BackColor), rest.X + 10, rest.Y + rest.Height / 2 + 25);
            }

            //电控柜
            Rectangle reBox = new System.Drawing.Rectangle(rest.X + rest.Width + 8, rest.Y + rest.Height/2-45 , 17, 105);
            g.FillRectangle(brush = new SolidBrush(Color.DarkGray), reBox);
            reBox.Inflate(2, 2);
            ControlPaint.DrawBorder3D(g, reBox, Border3DStyle.RaisedInner);
            string agv_Num = "AGV" + M_CarNo;
            for (int i = 0; i < agv_Num.Length; i++)
            {
                char[] n = agv_Num.ToArray();
                //AGV编号
                //g.DrawString(n[i].ToString(), new Font(new FontFamily("宋体"), 12, FontStyle.Bold), new SolidBrush(Color.FromArgb(5, 165, 100)), reBox.X + reBox.Width / 2 - 6, reBox.Y + reBox.Height / 4 + i * 14);
                g.DrawString(n[i].ToString(), new Font(new FontFamily("宋体"), 12, FontStyle.Bold), new SolidBrush(Color.Black), reBox.X + reBox.Width / 2 - 6, reBox.Y + reBox.Height / 4 + i * 14);
            }

            //拖斗
            Rectangle reTrail = new System.Drawing.Rectangle(resOperation.X - 45, reBox.Y+reBox.Height-15, 110, 15);
            g.FillRectangle(brush = new SolidBrush(Color.Black), reTrail);
            reBox.Inflate(2, 2);
            ControlPaint.DrawBorder3D(g, reTrail, Border3DStyle.RaisedInner);
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

       

        public override void DrawUnitInfo(Rectangle Rect, Graphics g)
        {
            Rectangle reInfo = Rect;
            SolidBrush brush = new SolidBrush(Color.Transparent);
            g.FillRectangle(brush, reInfo);
           
        }
    }
}
