using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Timers;

namespace CWCS.UIDesigner.RobotControl
{
    public enum TrayUnitStatus
    {
        Empty = 0,
        Fill = 1
    }

    public enum TrayConnect
    {
        DisConnected = 0,
        Connected = 1
    }


    public class TrayControl : ContainerControl
    {
        Rectangle rectTray;

        //TrayUnitStatus M_UnitStatus = TrayUnitStatus.Fill;
        private List<TrayUnitStatus> lstUnitStatus = new List<TrayUnitStatus>();


        private TrayConnect m_ConnectStatus = TrayConnect.Connected;

        internal TrayConnect ConnectStatus
        {
            get { return m_ConnectStatus; }
            set { m_ConnectStatus = value; }
        }
        Color TwinkleColor;

        internal List<TrayUnitStatus> LstUnitStatus
        {
            get { return lstUnitStatus; }
            set { lstUnitStatus = value; }
        }
        private System.Timers.Timer t_Timer = new System.Timers.Timer();

        private int _TrayWidth = 300;

        public int TrayWidth
        {
            get { return _TrayWidth; }
            set { _TrayWidth = value; }
        }
        private int _TrayHeight = 150;

        public int TrayHeight
        {
            get { return _TrayHeight; }
            set { _TrayHeight = value; }
        }

        private int _TrayFontSize = 10;

        public int TrayFontSize
        {
            get { return _TrayFontSize; }
            set { _TrayFontSize = value; }
        }
        private int _ContainerCount = 64;

        public int ContainerCount
        {
            get { return _ContainerCount; }
            set { _ContainerCount = value; }
        }
        #region Constructor

        public TrayControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            TwinkleColor = Color.Red;

            for (int i = 0; i < ContainerCount; i++)
            {
                lstUnitStatus.Add(TrayUnitStatus.Empty);
            }
            t_Timer.Interval = 400;
            t_Timer.Elapsed += new ElapsedEventHandler(t_Timer_Elapsed);
            t_Timer.Start();
        }

        #endregion

        #region  event
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            rectTray = new Rectangle(this.Width / 2 - TrayWidth / 2, this.Height / 2 - TrayHeight / 2, TrayWidth, TrayHeight);
            Graphics graphics = e.Graphics;
            graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black), 3), rectTray);
            DrawBatteryInfo(rectTray, graphics);
            rectTray.Inflate(3, 3);
            DrawTwinkle(rectTray, graphics);
        }


        private void DrawBatteryInfo(Rectangle Rect, Graphics g)
        {
            g.DrawString("1", new Font("Arial", TrayFontSize, FontStyle.Bold), new SolidBrush(Color.Black), Rect.X - 15 + Rect.Width, Rect.Y - 20);
            g.DrawString((ContainerCount/2).ToString(), new Font("Arial", TrayFontSize, FontStyle.Bold), new SolidBrush(Color.Black), Rect.X - 3, Rect.Y - 20);
            g.DrawString((1+ContainerCount / 2).ToString(), new Font("Arial", TrayFontSize, FontStyle.Bold), new SolidBrush(Color.Black), Rect.X - 22 + Rect.Width, Rect.Y + 2 + Rect.Height);
            g.DrawString(ContainerCount.ToString(), new Font("Arial", TrayFontSize, FontStyle.Bold), new SolidBrush(Color.Black), Rect.X - 3, Rect.Y + 2 + Rect.Height);

            int count = ContainerCount / 2;
            if (lstUnitStatus.Count > 0)
            {
                for (int i = 1; i <= count; i++)
                {
                    {
                        if (lstUnitStatus[i - 1] != TrayUnitStatus.Empty)
                        {
                            Rectangle recBattery1 = new System.Drawing.Rectangle(Rect.X + (count - i) * Rect.Width / count + 1, Rect.Y + 1, (Rect.Width) / count, (Rect.Height) / 2);
                            recBattery1.Inflate(-2, -3);
                            g.FillRectangle(new SolidBrush(lstUnitStatus[i - 1] == TrayUnitStatus.Empty ? Color.Transparent : Color.LimeGreen), recBattery1);
                            ControlPaint.DrawBorder3D(g, recBattery1, Border3DStyle.RaisedOuter);

                        }
                        if (lstUnitStatus[count + i - 1] != TrayUnitStatus.Empty)
                        {
                            Rectangle recBattery2 = new System.Drawing.Rectangle(Rect.X + (count - i) * Rect.Width / count + 1, Rect.Y + Rect.Height / 2, (Rect.Width) / count, (Rect.Height) / 2);
                            recBattery2.Inflate(-2, -3);
                            g.FillRectangle(new SolidBrush(lstUnitStatus[count + i - 1] == TrayUnitStatus.Empty ? Color.Transparent : Color.LimeGreen), recBattery2);
                            ControlPaint.DrawBorder3D(g, recBattery2, Border3DStyle.RaisedOuter);
                        }

                    }
                    if (i < count)
                    {
                        Rectangle recColums = new Rectangle(Rect.X + Rect.Width * i / count, Rect.Y, 2, Rect.Height - 3);
                        g.DrawRectangle(new Pen(new SolidBrush(Color.Black)), recColums);
                        ControlPaint.DrawBorder3D(g, recColums, Border3DStyle.RaisedOuter);
                    }
                }
                Rectangle recLevel = new System.Drawing.Rectangle(Rect.X + 2, Rect.Y + Rect.Height / 2, Rect.Width - 3, 2);
                g.DrawRectangle(new Pen(new SolidBrush(Color.Black)), recLevel);
                ControlPaint.DrawBorder3D(g, recLevel, Border3DStyle.RaisedOuter);
            }
        }


        #endregion

        #region 闪烁效果
        private void DrawTwinkle(Rectangle rect, Graphics g)
        {
            Pen pen1 = new Pen(TwinkleColor, 2);
            pen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;//绘制虚线框
            g.DrawRectangle(pen1, rect.X, rect.Y, rect.Width, rect.Height);
        }
        #endregion

        private void t_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (ConnectStatus != TrayConnect.Connected)
            {
                if (TwinkleColor == Color.Red)
                    TwinkleColor = Color.Transparent;
                else
                    TwinkleColor = Color.Red;
            }
            else
            {
                if (TwinkleColor == Color.LimeGreen)
                    TwinkleColor = Color.Transparent;
                else
                    TwinkleColor = Color.LimeGreen;
            }
            this.Invalidate();
        }
    }
}
