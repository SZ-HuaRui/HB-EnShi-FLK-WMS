using CWCS.BLL;
using CWCS.Entity.Convery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Timers;
using System.Windows.Forms;

namespace CWCS.UIDesigner.ConveryControl
{
    public partial class ConveryPanel : Panel
    {
        #region Field
        int unitLW = 30;//最小单元格长宽     
        public List<ConveryUnit> Units = new List<ConveryUnit>();//需要绘制的对象集合

        private float _zoom = 1.0f;
        private float _rotation = 0f;
        private int _panX = 0;
        private int _panY;
        private int _originalPanY;
        public static int count = 56;
        public static MoveDierection moveDirection = MoveDierection.Left;

        Point? m_MousedownPoint = null;
        Point? m_lastPoint;
        Rectangle? m_MousedownRect;
        Rectangle? m_lastRect;
        ConveryUnit m_lastUnit;
        ConveryUnit m_MousedownUnit;
        List<ConveryUnit> SelectedUnits = new List<ConveryUnit>();
        List<ConveryUnit> tempSelectedUnits = new List<ConveryUnit>();


        private Cursor cursor;
        private System.Timers.Timer t_Timer = new System.Timers.Timer();
        public int DefaultUnitWidthColumns = 3;
        public int DefaultUnitHeightRows = 3;
        List<Rectangle> lastCellsMove = new List<Rectangle>();
        List<ConveryUnit> lastRePaintUnit = new List<ConveryUnit>();
       

        public enum MoveDierection
        {
            Left = 0,
            Right= 1
        }

        #endregion

        #region Properties
        public int UnitLW
        {
            get { return unitLW; }
            set
            {
                unitLW = value;
                ResizeScrollMinSize();
            }
        }
        #endregion

        #region Event
        public event MouseEventHandler UnitMouseClick;
        public event MouseEventHandler UnitMouseDown;
        public event MouseEventHandler UnitMouseUp;

        public event EventHandler UnitMouseEnter;
        public event EventHandler UnitMouseLeave;

        #endregion

        public ConveryPanel()
        {
            try
            {
                //if (DesignMode)return;
                this.AutoScroll = true;
                this.AllowDrop = true;
                this.AutoScrollMinSize = new Size(0, 0);
                this.AutoScrollPosition = new Point(0, 0);
                this.DoubleBuffered = true;
                t_Timer.Interval = 400;
                this.BackColor = Color.White;
                t_Timer.Elapsed += new ElapsedEventHandler(t_Timer_Elapsed);
            }
            catch { }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            if (Units.Count > 0)
            {
                DrawUnits(pe);
            }
           
            
        }
        public void DrawTextDescription(Graphics g)
        {
            Pen linePen = new Pen(Color.Gray,  (float)3);
            linePen.DashStyle = DashStyle.Dot;
            g.DrawLine(linePen, this.AutoScrollPosition.X + 23 * unitLW,0, this.AutoScrollPosition.X + 23 * unitLW,  this.Parent.Height- this.AutoScrollPosition.Y);
            g.DrawLine(linePen, this.AutoScrollPosition.X + 33 * unitLW, 0, this.AutoScrollPosition.X + 33 * unitLW, this.Parent.Height - this.AutoScrollPosition.Y);
            g.DrawLine(linePen, 0, 14*unitLW + this.AutoScrollPosition.Y, this.Parent.Width-this.AutoScrollPosition.X, 14 * unitLW + this.AutoScrollPosition.Y);
            //g.DrawString("二楼", new Font("微软雅黑", 12), new SolidBrush(Color.Blue), this.AutoScrollPosition.X + 21 * unitLW-20, 0);
            //g.DrawString("三楼", new Font("微软雅黑", 12), new SolidBrush(Color.Blue), this.AutoScrollPosition.X + 21 * unitLW+22, 0);

            //一楼货架
            linePen.DashStyle = DashStyle.Solid;
            g.DrawLine(linePen, 
                this.AutoScrollPosition.X + 5 * unitLW,
                this.AutoScrollPosition.Y+ 14 * unitLW, 
                this.AutoScrollPosition.X + 5 * unitLW,
                this.AutoScrollPosition.Y+ 19 * unitLW);
            g.DrawLine(linePen,
               this.AutoScrollPosition.X + 15 * unitLW,
               this.AutoScrollPosition.Y + 14 * unitLW,
               this.AutoScrollPosition.X + 15 * unitLW,
               this.AutoScrollPosition.Y + 19 * unitLW);
            StringFormat stringFormat = new StringFormat(StringFormatFlags.DisplayFormatControl);
            g.DrawString("一楼仓库",
                new Font("华文楷体", 15),
                new SolidBrush(Color.Blue),
                this.AutoScrollPosition.X + 8 * unitLW,
                this.AutoScrollPosition.Y + 16 * unitLW,
                stringFormat);

            //二楼货架
            linePen.DashStyle = DashStyle.Solid;
            g.DrawLine(linePen,
                this.AutoScrollPosition.X + 5 * unitLW,
                this.AutoScrollPosition.Y + 1 * unitLW,
                this.AutoScrollPosition.X + 15 * unitLW,
                this.AutoScrollPosition.Y + 1 * unitLW);
            g.DrawLine(linePen,
               this.AutoScrollPosition.X + 15 * unitLW,
               this.AutoScrollPosition.Y + 1 * unitLW,
               this.AutoScrollPosition.X + 15 * unitLW,
               this.AutoScrollPosition.Y + 6 * unitLW);
            g.DrawLine(linePen,
               this.AutoScrollPosition.X + 15 * unitLW,
               this.AutoScrollPosition.Y + 6 * unitLW,
               this.AutoScrollPosition.X + 5 * unitLW,
               this.AutoScrollPosition.Y + 6 * unitLW);
            g.DrawLine(linePen,
               this.AutoScrollPosition.X + 5 * unitLW,
               this.AutoScrollPosition.Y + 6 * unitLW,
               this.AutoScrollPosition.X + 5 * unitLW,
               this.AutoScrollPosition.Y + 1 * unitLW);
            g.DrawString("二楼仓库",
                new Font("华文楷体", 15),
                new SolidBrush(Color.Blue),
                this.AutoScrollPosition.X + 8 * unitLW,
                this.AutoScrollPosition.Y + 3 * unitLW,
                stringFormat);
            //区域描述
            linePen.DashStyle = DashStyle.Solid;
            g.DrawString("卷包车间二楼",
                new Font("华文楷体", 15),
                new SolidBrush(Color.Blue),
                this.AutoScrollPosition.X + 0 * unitLW,
                this.AutoScrollPosition.Y + 0 * unitLW,
                stringFormat);
            int i = this.AutoScrollMargin.Width;
            g.DrawString("卷包车间一楼",
                new Font("华文楷体", 15),
                new SolidBrush(Color.Blue),
                this.AutoScrollPosition.X + 0 * unitLW,
                this.AutoScrollPosition.Y + 14 * unitLW,
                stringFormat);
            g.DrawString("综合库二楼",
             new Font("华文楷体", 15),
             new SolidBrush(Color.Blue),
             this.Width + this.AutoScrollPosition.X + (this.HorizontalScroll.Visible?320:- 150),
             this.AutoScrollPosition.Y + 0 * unitLW,
             stringFormat);
            g.DrawString("综合库一楼",
             new Font("华文楷体", 15),
             new SolidBrush(Color.Blue),
             this.Width + this.AutoScrollPosition.X + (this.HorizontalScroll.Visible ? 320 : - 150),
             this.AutoScrollPosition.Y + 14 * unitLW,
             stringFormat);
            //连廊
            g.DrawString("连 廊",
               new Font("华文楷体", 15),
               new SolidBrush(Color.Blue),
               this.AutoScrollPosition.X + 27 * unitLW+10,
               this.AutoScrollPosition.Y + 7 * unitLW,
               stringFormat);
            g.DrawString("连 廊",
              new Font("华文楷体", 15),
              new SolidBrush(Color.Blue),
              this.AutoScrollPosition.X + 27 * unitLW + 10,
              this.AutoScrollPosition.Y + 11 * unitLW,
              stringFormat);
        }




        public void DrawRGV(Graphics g)
        {
            for (int i = 0;i <=1;i++)
            {
                Rectangle horizontalBar1 = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 40 - 10-i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5 + 40 - 30, 40, 10);
                g.FillRectangle(new SolidBrush(Color.FromArgb(255, 165, 0)), horizontalBar1);

                Rectangle horizontalBar2 = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 40 - 10-i* 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5 + 40, 40, 10);
                g.FillRectangle(new SolidBrush(Color.FromArgb(255, 165, 0)), horizontalBar2);


                Rectangle leftChain = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 50 - 10 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5, 10, 60);
                g.DrawRectangle(new Pen(new SolidBrush(Color.Black), 2), leftChain);


                Rectangle rightChain = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 10 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5, 10, 60);
                g.DrawRectangle(new Pen(new SolidBrush(Color.Black), 2), rightChain);

                Rectangle nut1 = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 50 - 10 - 5 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5 + 40 - 30 + 2, 5, 5);
                g.FillRectangle(new SolidBrush(Color.Black), nut1);

                Rectangle nut2 = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 50 - 10 - 5 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5 + 40 - 30 + 2 + 30, 5, 5);
                g.FillRectangle(new SolidBrush(Color.Black), nut2);


                Rectangle cabinet = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 50 - 10 - 25 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5, 20, 60);
                g.DrawRectangle(new Pen(new SolidBrush(Color.Black), 2), cabinet);

                g.FillEllipse(new SolidBrush(Color.Black), this.AutoScrollPosition.X + 42 * UnitLW - 50 - 10 - 25 + 10 - 3 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5 + 10 - 1, 8, 8);

                //链条齿轮

                Rectangle leftGear1 = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 50 - 10 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5 + 10 - 2, 10, 5);
                g.FillRectangle(new SolidBrush(Color.Black), leftGear1);
                Rectangle leftGear2 = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 50 - 10 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5 + 10 + 10 - 2, 10, 5);
                g.FillRectangle(new SolidBrush(Color.Black), leftGear2);
                Rectangle leftGear3 = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 50 - 10 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5 + 10 + 10 + 10 - 2, 10, 5);
                g.FillRectangle(new SolidBrush(Color.Black), leftGear3);
                Rectangle leftGear4 = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 50 - 10 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5 + 10 + 10 + 10 + 10 - 2, 10, 5);
                g.FillRectangle(new SolidBrush(Color.Black), leftGear4);
                Rectangle leftGear5 = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 50 - 10 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5 + 10 + 10 + 10 + 10 + 10 - 2, 10, 5);
                g.FillRectangle(new SolidBrush(Color.Black), leftGear5);


                Rectangle rightGear1 = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 10 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5 + 10 - 2, 10, 5);
                g.FillRectangle(new SolidBrush(Color.Black), rightGear1);
                Rectangle rightGear2 = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 10 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5 + 10 + 10 - 2, 10, 5);
                g.FillRectangle(new SolidBrush(Color.Black), rightGear2);
                Rectangle rightGear3 = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 10 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5 + 10 + 10 + 10 - 2, 10, 5);
                g.FillRectangle(new SolidBrush(Color.Black), rightGear3);
                Rectangle rightGear4 = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 10 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5 + 10 + 10 + 10 + 10 - 2, 10, 5);
                g.FillRectangle(new SolidBrush(Color.Black), rightGear4);
                Rectangle rightGear5 = new Rectangle(this.AutoScrollPosition.X + 42 * UnitLW - 10 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5 + 10 + 10 + 10 + 10 + 10 - 2, 10, 5);
                g.FillRectangle(new SolidBrush(Color.Black), rightGear5);

                g.DrawString(Convert.ToString(2005+i), new Font("华文楷体", 9), new SolidBrush(Color.Blue), this.AutoScrollPosition.X + 42 * UnitLW - 40 - 10 + 5 - i * 250 - count, this.AutoScrollPosition.Y + 3 * unitLW + 5 + 40 - 30 + 10 + 2);

            }




        }

        public void Add(ConveryUnit unit)
        {
            if (!Units.Exists(it => it.Data == unit.Data))
            {
                Units.Add(unit);
                unit.Parent = this;
                ResizeScrollMinSize();
            }
        }

        public void Remove(ConveryUnit unit)
        {
            Units.Remove(unit);
            unit.Parent = null;
            ResizeScrollMinSize();
        }

        public bool Contains(ConveryUnit unit)
        {
            return Units.Contains(unit);
        }
        public void ClearMoveFlag()
        {
            ConveryUnit[] uMousedowns = Units.Where(v => v.MoveFlag).ToArray();
            foreach (var u in uMousedowns)
            {
                u.MoveFlag = false;
            }
        }

        void ResizeScrollMinSize()
        {

            int maxX = 0;
            int maxY = 0;
            foreach (ConveryUnit u in Units)
            {
                if (u.GetRect().X + u.GetRect().Width > maxX)
                    maxX = u.GetRect().X + u.GetRect().Width;
                if (u.GetRect().Y + u.GetRect().Height > maxY)
                    maxY = u.GetRect().Y + u.GetRect().Height;
            }
            AutoScrollMinSize = new Size(maxX + 100, maxY + 100);
            Refresh();
        }

       
        void DrawUnits(PaintEventArgs e)
        {
            List<ConveryUnit> us = new List<ConveryUnit>();
            List<ConveryUnit> usnew = new List<ConveryUnit>();
            List<ConveryUnit> userror = new List<ConveryUnit>();
            List<ConveryUnit> usPallet = new List<ConveryUnit>();
            BufferedGraphicsContext GraphicsContext = BufferedGraphicsManager.Current;
            BufferedGraphics myBuffer = GraphicsContext.Allocate(e.Graphics, e.ClipRectangle);//获取缓冲区
            Graphics g = myBuffer.Graphics;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;//高质量低速度呈现
            g.SmoothingMode = SmoothingMode.HighQuality;// 指定高质量、低速度呈现
            g.Clear(BackColor);
          //  DrawRGV(g);
            DrawTextDescription(g);
            Rectangle rectshow = Units.FirstOrDefault().GetRect();
            foreach (ConveryUnit u in Units.OrderBy(v => v.Visible))
            {
                if (u.XIndex == -1 && u.YIndex == -1)
                {
                    usnew.Add(u);
                    continue;
                }
                if (u.State == ConveryUnit.UnitState.Selected)
                {
                    us.Add(u);
                    continue;
                }
                if (u.EuipmentState == ConveryUnit.UnitEuipmentState.Error)
                {
                    userror.Add(u);
                    //t_Timer.Enabled = true;
                    continue;
                }
                if (u.GoodsType != ConveryUnit.UnitGoodsType.Empty)
                {
                    usPallet.Add(u);
                    continue;
                }
                u.DrawUnit(u.GetRectOfParent(), g);
            }

            int y = 0;
            if (Units.Count > 0)
                y = Units.Max(v => v.YIndex) + 1;
            int defaultCols = 50;
            int xind = 0;
            foreach (ConveryUnit u in usnew)
            {
                u.XIndex = xind;
                u.YIndex = y;
                u.WidthColumns = DefaultUnitWidthColumns;
                u.HeightRows = DefaultUnitHeightRows;

                if (u.State != ConveryUnit.UnitState.Selected)
                    u.DrawUnit(u.GetRectOfParent(), g);
                else
                    us.Add(u);
                if (u.EuipmentState == ConveryUnit.UnitEuipmentState.Error)
                    userror.Add(u);
                if (u.GoodsType != ConveryUnit.UnitGoodsType.Empty)
                    usPallet.Add(u);
                else
                    u.ArrowColor = Color.LimeGreen;
                xind += u.WidthColumns;
                if (xind >= defaultCols)
                {
                    xind = 0;
                    y += u.HeightRows;
                }

            }
            if (userror.Count > 0 || usPallet.Count > 0)
                t_Timer.Enabled = true;
            else
                t_Timer.Enabled = false;
            foreach (ConveryUnit u in us)
                u.DrawUnit(u.GetRectOfParent(), g);


            foreach (ConveryUnit u in usPallet)
            {
                u.ArrowColor = Color.LimeGreen;//(u.ArrowColor.Name == "LimeGreen" ? u.ArrowColor = BackColor : Color.LimeGreen);
                DrawError(u, u.GetRectOfParent(), g);
            }

            foreach (ConveryUnit u in userror)
            {
                u.TwinkleColor = (u.TwinkleColor.Name == "Red" ? u.TwinkleColor = BackColor : Color.Red);
                DrawError(u, u.GetRectOfParent(), g);
            }
            myBuffer.Render(e.Graphics);  //将图形缓冲区的内容写入指定的Graphics

            g.Dispose();

            myBuffer.Dispose();
        }

        public void DrawError(ConveryUnit u, Rectangle rect, Graphics g)
        {
            u.DrawUnit(rect, g);
        }

        private void t_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Invalidate();
        }

        #region MouseEvent

        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_MousedownPoint = GetPointCurr();
            m_lastPoint = m_MousedownPoint;
            m_MousedownRect = GetRectCurr();
            m_lastRect = m_MousedownRect;

            m_MousedownUnit = null;
            ConveryUnit[] uMousedowns = Units.Where(v => v.GetRect().Contains(m_MousedownPoint.Value)).ToArray();
            if (uMousedowns.Length > 0)
                m_MousedownUnit = uMousedowns[0];
            tempSelectedUnits.Clear();

            if (ModifierKeys != Keys.Control && ModifierKeys != Keys.Shift)
            {
                foreach (ConveryUnit u in SelectedUnits)
                {
                    u.State = ConveryUnit.UnitState.Normal;
                    u.DrawUnit(u.GetRectOfParent(), Graphics.FromHwnd(this.Handle));
                }
                SelectedUnits.Clear();
            }

            if (ModifierKeys == Keys.Shift && SelectedUnits.Count > 0)
            {
                this.DoDragDrop(SelectedUnits, DragDropEffects.Move);
            }
            if (UnitMouseDown != null && uMousedowns.Length > 0)
            {
                UnitMouseDown(uMousedowns[0], e);
            }

        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            this.Focus();
            m_lastPoint = GetPointCurr();

            if (m_lastRect.Value == GetRectCurr())
            {
                if (ModifierKeys != Keys.Control)
                {
                    ConveryUnit[] uselected = Units.Where(v => v.State == ConveryUnit.UnitState.Selected).ToArray();
                    foreach (ConveryUnit u in uselected)
                    {
                        u.State = ConveryUnit.UnitState.Normal;
                        u.DrawUnit(u.GetRectOfParent(), Graphics.FromHwnd(this.Handle));
                    }
                }
                ConveryUnit[] us = Units.Where(v => v.GetRect().Contains(m_lastPoint.Value)).ToArray();
                if (us.Length == 0)
                    return;
                us[0].State = ConveryUnit.UnitState.Selected;
                us[0].DrawUnit(us[0].GetRectOfParent(), Graphics.FromHwnd(this.Handle));
                if (!SelectedUnits.Exists(it => it.Data == us[0].Data))
                    SelectedUnits.Add(us[0]);

                if (UnitMouseClick != null && us.Length > 0)
                {
                    UnitMouseClick(us[0], e);
                }
            }

            base.OnMouseClick(e);
        }

        //protected override void OnMouseUp(MouseEventArgs e)
        //{
        //    base.OnMouseUp(e);
        //    foreach (ConveryUnit u in tempSelectedUnits)
        //    {
        //        if (!SelectedUnits.Exists(it => it.Data == u.Data))
        //            SelectedUnits.Add(u);
        //    }
        //    ConveryUnit[] us = Units.Where(v => v.GetRect().Contains(GetPointCurr())).ToArray();
        //    if (us.Length > 0 && UnitMouseUp != null)
        //    {
        //        UnitMouseUp(us[0], e);
        //    }
        //}

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            //if (e.Button == MouseButtons.Right)
            //{
            //    m_MousedownPoint = GetPointCurr();
            //    if (m_MousedownPoint == null || m_MousedownUnit == null)
            //        return;
            //    ConveryUnit[] us = Units.Where(v => v.GetRect().Contains((Point)m_MousedownPoint)).ToArray();
            //    if (us.Length > 0) return;
            //    m_MousedownUnit.XIndex += (m_MousedownPoint.Value.X - m_MousedownUnit.GetRect().X) / (m_MousedownUnit.WidthColumns * 30);
            //    m_MousedownUnit.YIndex += (m_MousedownPoint.Value.Y - m_MousedownUnit.GetRect().Y) / (m_MousedownUnit.HeightRows * 30);
            //    m_lastUnit = us[0];
            //    //m_MousedownUnit.MoveFlag = true;
            //    Refresh();
            //}
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (MouseButtons == MouseButtons.Left)
            {
                if (m_MousedownPoint == null || m_MousedownUnit == null)
                    return;
                Rectangle rect = GetRectCurr();

                ConveryUnit[] uselect = Units.Where(v => v.GetRect().Contains(GetPointCurr()) && v.Visible).ToArray();
                if (uselect.Length > 0)
                {
                    if (m_lastUnit == null || m_lastUnit != uselect[0])
                    {
                        m_lastUnit = uselect[0];
                        Point indexMousedown = GetIndexOfPoint(m_MousedownPoint.Value);
                        Point indexLast = GetIndexCurr();
                        m_lastRect = rect;

                        int sX = m_MousedownUnit.XIndex > m_lastUnit.XIndex ? m_lastUnit.XIndex : m_MousedownUnit.XIndex;
                        int bX = m_MousedownUnit.XIndex > m_lastUnit.XIndex ? m_MousedownUnit.XIndex : m_lastUnit.XIndex;
                        int sY = m_MousedownUnit.YIndex > m_lastUnit.YIndex ? m_lastUnit.YIndex : m_MousedownUnit.YIndex;
                        int bY = m_MousedownUnit.YIndex > m_lastUnit.YIndex ? m_MousedownUnit.YIndex : m_lastUnit.YIndex;

                        ConveryUnit[] us = Units.Where(v => v.XIndex <= bX && v.XIndex >= sX && v.YIndex <= bY && v.YIndex >= sY).ToArray();

                        for (int i = tempSelectedUnits.Count - 1; i >= 0; i--)
                        {
                            if (!us.Contains(tempSelectedUnits[i]))
                            {
                                if (SelectedUnits.Exists(it => it.Data == tempSelectedUnits[i].Data))
                                    continue;
                                tempSelectedUnits[i].State = ConveryUnit.UnitState.Normal;
                                tempSelectedUnits[i].DrawUnit(tempSelectedUnits[i].GetRectOfParent(), Graphics.FromHwnd(this.Handle));
                                tempSelectedUnits.RemoveAt(i);
                            }
                        }
                        Rectangle r = us[0].GetRectOfParent();
                        foreach (ConveryUnit u in us)
                        {
                            if (tempSelectedUnits.Exists(it => it.Data == u.Data))
                                continue;
                            u.State = ConveryUnit.UnitState.Selected;

                            u.DrawUnit(u.GetRectOfParent(), Graphics.FromHwnd(this.Handle));
                            if (!tempSelectedUnits.Exists(it => it.Data == u.Data))
                                tempSelectedUnits.Add(u);
                        }
                    }
                }

            }
            try
            {

                ConveryUnit[] uselect = Units.Where(v => v.GetRect().Contains(GetPointCurr())).ToArray();
                if (uselect.Length > 0)
                {
                    if (UnitMouseEnter != null)
                    {
                        if (m_lastUnit == null || m_lastUnit != uselect[0])
                        {
                            UnitMouseEnter(uselect[0], e);
                        }
                    }
                    if (UnitMouseLeave != null)
                    {
                        if (m_lastUnit != null && m_lastUnit != uselect[0])
                        {
                            UnitMouseLeave(m_lastUnit, e);
                        }
                    }
                    m_lastUnit = uselect[0];
                }
                else
                    m_lastUnit = null;


            }
            catch { }

            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            this.AutoScrollPosition = new Point(-AutoScrollPosition.X, -AutoScrollPosition.Y - e.Delta);
        }

        #endregion

        #region DragEvent

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);
            if (SelectedUnits.Count > 0)
                drgevent.Effect = DragDropEffects.Move;
            DrawCellMove();
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            Rectangle p = GetRectCurr();

            m_lastRect = p;
            int x, y;
            x = p.X / unitLW - m_MousedownRect.Value.X / unitLW;
            y = p.Y / unitLW - m_MousedownRect.Value.Y / unitLW;

            foreach (ConveryUnit u in SelectedUnits)
            {
                Graphics.FromHwnd(this.Handle).FillRectangle(new SolidBrush(this.BackColor), u.GetRectOfParent());
                u.XIndex += x;
                u.YIndex += y;
                u.MoveFlag = true;
            }
            ResizeScrollMinSize();

        }

        #endregion

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
            return new Rectangle(p.X - p.X % unitLW, p.Y - p.Y % unitLW, unitLW, unitLW);
        }



        Point GetIndexCurr()
        {
            Point p = this.PointToClient(MousePosition);
            p.X -= this.AutoScrollPosition.X + Padding.Left;
            p.Y -= this.AutoScrollPosition.Y + Padding.Top;
            return new Point(p.X / unitLW, p.Y / unitLW);
        }

        Point GetIndexOfPoint(Point p)
        {
            return new Point(p.X / unitLW, p.Y / unitLW);
        }

        void DrawCellMove()
        {
            Rectangle p = GetRectCurr();
            if (p == m_lastRect)
                return;
            m_lastRect = p;
            int x, y;
            x = p.X / unitLW - m_MousedownRect.Value.X / unitLW;
            y = p.Y / unitLW - m_MousedownRect.Value.Y / unitLW;

            foreach (Rectangle rect in lastCellsMove)
            {
                Pen pen2 = new Pen(this.BackColor, 2);
                pen2.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;//擦除虚线框
                Graphics.FromHwnd(this.Handle).DrawRectangle(pen2, rect.X + this.AutoScrollPosition.X, rect.Y + this.AutoScrollPosition.Y, rect.Width, rect.Height);


                List<ConveryUnit> us = Units.Where(v => rect.IntersectsWith(v.GetRect()) && !rect.Contains(v.GetRect())).ToList();

                lastRePaintUnit.Clear();
                for (int i = us.Count - 1; i >= 0; i--)
                {
                    //重绘被虚线框干扰过的unit
                    ConveryUnit u = us[i];
                    if (!SelectedUnits.Exists(it => it.Data == u.Data))
                    {
                        foreach (ConveryUnit un in SelectedUnits.Where(v => v.GetRectOfParent().IntersectsWith(u.GetRectOfParent())).ToArray())
                            lastRePaintUnit.Add(un);
                    }

                    u.DrawUnit(u.GetRectOfParent(), Graphics.FromHwnd(this.Handle));
                    us.RemoveAt(i);

                }
                foreach (ConveryUnit u in lastRePaintUnit)
                {
                    u.DrawUnit(u.GetRectOfParent(), Graphics.FromHwnd(this.Handle));
                }
            }

            lastCellsMove.Clear();

            Rectangle rectall = new Rectangle();
            GraphicsPath path = new GraphicsPath();
            List<Point> ps = new List<Point>();
            ps.Clear();
            foreach (ConveryUnit selectedU in SelectedUnits)
            {
                Rectangle r = selectedU.GetRect();
                Rectangle temp = new Rectangle();
                temp.Width = r.Width;
                temp.Height = r.Height;
                temp.X = r.X + x * unitLW;
                temp.Y = r.Y + y * unitLW;
                Point[] pos = new Point[4];
                pos[0] = temp.Location;
                pos[1] = new Point(temp.X + temp.Width, temp.Y);
                pos[2] = new Point(temp.X + temp.Width, temp.Y + temp.Height);
                pos[3] = new Point(temp.X, temp.Y + temp.Height);
                foreach (Point po in pos)
                {
                    if (ps.Contains(po))
                        ps.Remove(po);
                    else
                        ps.Add(po);
                }


                lastCellsMove.Add(temp);
                if (rectall.Width == 0)
                    rectall = temp;
                rectall = Rectangle.Union(rectall, temp);
                path.AddRectangle(temp);

                Pen pen = new Pen(Color.Red);
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                //Graphics.FromHwnd(this.Handle).DrawRectangle(pen, temp.X + this.AutoScrollPosition.X + 1, temp.Y + this.AutoScrollPosition.Y + 1, temp.Width - 2, temp.Height - 2);
            }


            rectall.Inflate(-1, -1);
            lastCellsMove.Clear();

            lastCellsMove.Add(rectall);

            Pen pen1 = new Pen(Color.Red, 2);
            pen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;//绘制虚线框
            Graphics.FromHwnd(this.Handle).DrawRectangle(pen1, rectall.X + this.AutoScrollPosition.X, rectall.Y + this.AutoScrollPosition.Y, rectall.Width, rectall.Height);
        }

    }


    public abstract class ConveryUnit
    {
       
        #region enum
        public enum UnitState
        {
            Normal = 0,    //正常(无货)
            Selected = 1   //选中
        }
        public enum UnitEuipmentState
        {
            OK = 0,
            Error = 1,        //故障
            Other = 2,    //其他：紧急模式、清扫模式
            Normal = 3  //put：正常模式、生产模式
        }
        public enum UnitGoodsType
        {
            Empty =0,
            NullPalletGroup = 10,//托盘组，--天蓝色
            RealPallet = 1,//母实盘，深黄色
            SingleNullPallet = 2,//单盘--浅蓝色
            unknown = 99,//未知货物类型--灰色
            ExceptionPallet = 3
        }
        public enum UnitType
        {
            InFeeding = 1,   //入库请求站台
            InFeeded = 2,  //入库到位站台
            OutFeeding = 3, //出口请求站台
            OutFeeded = 4, //出口到位站台
            Normal = 5
        }
        public enum UnitOrientation
        {
            Horizontal = 1,
            Vertical = 2
        }
        public enum UnitArrowDirection
        {
            Blank = 0,
            Right = 1,
            Left = 2,
            Down = 3,
            Up = 4,
            LeftRight = 5,
            UpDown = 6,
        }

        public enum UnitEquipmentType
        {
            Convery = 1,
            SetInfo = 2,
            Other = 3,
            Text = 4//文字描述
        }
        #endregion

        public UnitType StationType = UnitType.Normal;
        public UnitState State = UnitState.Normal;
        public string Text = string.Empty;
        public UnitEuipmentState EuipmentState = UnitEuipmentState.OK;
        public UnitGoodsType GoodsType = UnitGoodsType.Empty;
        public UnitOrientation Direct = UnitOrientation.Horizontal;
        public UnitArrowDirection Arrow = UnitArrowDirection.Blank;
        public UnitEquipmentType EquipmentType = UnitEquipmentType.Convery;

        public string TextDescription = string.Empty;

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
        public object Data;
        private Point? lastAutoScrollPosition;
        private int lastlw = 0;
        private Rectangle rect;
        private Rectangle rectofParent;
        private bool visible = true;
        private bool _MoveFlag = false;


        public bool Visible
        {
            get { return visible; }
            set
            {
                visible = value;
            }
        }

        private ConveryPanel parent = null;
        public ConveryPanel Parent
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
        public bool MoveFlag
        {
            get { return _MoveFlag; }
            set { _MoveFlag = value; }
        }
        /// <summary>
        /// 站台编号
        /// </summary>
        public string ConveryNumber { get; set; }
        public enum Mode { Color, Image }

        protected Mode displayMode = Mode.Color;
        /// <summary>
        /// 背景显示模式
        /// </summary>
        public Mode DisplayMode
        {
            get { return displayMode; }
            set { displayMode = value; }
        }

        protected Image backImage = null;
        /// <summary>
        /// 背景图片
        /// </summary>
        public Image BackImage
        {
            get { return backImage; }
            set { backImage = value; }
        }
        private Color twinkleColor = Color.Red;

        public Color TwinkleColor
        {
            get { return twinkleColor; }
            set { twinkleColor = value; }
        }
        private Color arrowColor = Color.LimeGreen;

        public Color ArrowColor
        {
            get { return arrowColor; }
            set { arrowColor = value; }
        }
        protected Color backColor = Color.LightGray;
        public Color BackColor { get { return backColor; } set { backColor = value; } }
        protected Color foreColor = Color.Black;
        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }

        public ConveryUnit() { }
        public ConveryUnit(int Xindex, int Yindex, int widthColumns, int heightRows, object Data)
        {
            this.XIndex = Xindex;
            this.YIndex = Yindex;
            this.WidthColumns = widthColumns;
            this.HeightRows = heightRows;
            this.Data = Data;
        }

        public Rectangle GetRect()
        {
            if (lastAutoScrollPosition != parent.AutoScrollPosition || lastlw != parent.UnitLW)
            {
                rect = new Rectangle(parent.Padding.Left + XIndex * parent.UnitLW,
                    parent.Padding.Top + YIndex * parent.UnitLW, WidthColumns * parent.UnitLW, HeightRows * parent.UnitLW);
                lastAutoScrollPosition = parent.AutoScrollPosition;

                rectofParent = new Rectangle(parent.Padding.Left + XIndex * parent.UnitLW + parent.AutoScrollPosition.X,
                      parent.Padding.Top + YIndex * parent.UnitLW + parent.AutoScrollPosition.Y, WidthColumns * parent.UnitLW, HeightRows * parent.UnitLW);
                lastAutoScrollPosition = parent.AutoScrollPosition;
                lastlw = parent.UnitLW;
            }
            return rect;
        }

        public Rectangle GetRectOfParent()
        {
            if (lastAutoScrollPosition != parent.AutoScrollPosition || lastlw != parent.UnitLW)
            {
                rect = new Rectangle(parent.Padding.Left + XIndex * parent.UnitLW,
                     parent.Padding.Top + YIndex * parent.UnitLW, WidthColumns * parent.UnitLW, HeightRows * parent.UnitLW);
                lastAutoScrollPosition = parent.AutoScrollPosition;

                rectofParent = new Rectangle(parent.Padding.Left + XIndex * parent.UnitLW + parent.AutoScrollPosition.X,
                      parent.Padding.Top + YIndex * parent.UnitLW + parent.AutoScrollPosition.Y, WidthColumns * parent.UnitLW, HeightRows * parent.UnitLW);
                lastAutoScrollPosition = parent.AutoScrollPosition;
                lastlw = parent.UnitLW;
            }
            return rectofParent;
        }

        public abstract void DrawUnit(Rectangle Rect, Graphics g);

        public abstract void DrawSelect(Rectangle Rect, Graphics g);


        /// <summary>
        /// Hit test to determine if object is hit.
        /// </summary>
        /// <param name="point">Point to test</param>
        /// <returns>			(-1)		no hit
        ///						(0)		hit anywhere
        ///						(1 to n)	handle number</returns>
        public virtual int HitTest(Point point)
        {
            return -1;
        }


        /// <summary>
        /// Test whether point is inside of the object
        /// </summary>
        /// <param name="point">Point to test</param>
        /// <returns>true if in object, false if not</returns>
        protected virtual bool PointInObject(Point point)
        {
            return false;
        }


        /// <summary>
        /// Get cursor for the handle
        /// </summary>
        /// <param name="handleNumber">handle number to return cursor for</param>
        /// <returns>Cursor object</returns>
        public virtual Cursor GetHandleCursor(int handleNumber)
        {
            return Cursors.Default;
        }

        /// <summary>
        /// Test whether object intersects with rectangle
        /// </summary>
        /// <param name="rectangle">Rectangle structure to test</param>
        /// <returns>true if intersect, false if not</returns>
        public virtual bool IntersectsWith(Rectangle rectangle)
        {
            return false;
        }

        /// <summary>
        /// Move object
        /// </summary>
        /// <param name="deltaX">Distance along X-axis: (+)=Right, (-)=Left</param>
        /// <param name="deltaY">Distance along Y axis: (+)=Down, (-)=Up</param>
        public virtual void Move(int deltaX, int deltaY)
        {
        }

        /// <summary>
        /// Move handle to the point
        /// </summary>
        /// <param name="point">Point to Move Handle to</param>
        /// <param name="handleNumber">Handle number to move</param>
        public virtual void MoveHandleTo(Point point, int handleNumber)
        {
        }
        /// <summary>
        /// Normalize object.
        /// Call this function in the end of object resizing.
        /// </summary>
        public virtual void Normalize()
        {
        }
        /// <summary>
        /// Get handle point by 1-based number
        /// </summary>
        /// <param name="handleNumber">1-based handle number to return</param>
        /// <returns>Point where handle is located, if found</returns>
        public virtual Point GetHandle(int handleNumber)
        {
            return new Point(0, 0);
        }
        /// <summary>
        /// Get handle rectangle by 1-based number
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns>Rectangle structure to draw the handle</returns>
        public virtual Rectangle GetHandleRectangle(int handleNumber)
        {
            Point point = GetHandle(handleNumber);
            // Take into account width of pen
            return new Rectangle(point.X - 2, point.Y - (2), 6, 6);
        }


    }

    public class ConveryUnitRect : ConveryUnit
    {
      


        private Color backColor = Color.LightGray;
        public Color BackColor { get { return backColor; } set { backColor = value; } }
        private Color foreColor = Color.Black;
        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }
        public ConveryUnitRect()
        {

        }
        public ConveryUnitRect(int Xindex, int Yindex, int widthColumns, int heightRows, object Data)
            : base(Xindex, Yindex, widthColumns, heightRows, Data) { }

        private Rectangle rectangle;

        private const string entryRectangle = "Rect";

        protected Rectangle Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }

        bool lastvisible = true;

        public override void DrawUnit(Rectangle Rect, Graphics g)
        {
            if (!this.Visible)
            {
                if (lastvisible)
                {
                    g.FillRectangle(new SolidBrush(Parent.BackColor), Rect);
                    lastvisible = false;
                }
                return;
            }
            else
                lastvisible = true;
        
            if (State == UnitState.Normal)
            {
                if (EuipmentState == UnitEuipmentState.OK)
                {
                    g.FillRectangle(new SolidBrush(backColor), Rect);
                    DrawConvery(Rect, g);
                }
            }
            else if (State == UnitState.Selected)
            {
                    DrawConvery(Rect, g);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.Yellow)), Rect);
                    ControlPaint.DrawBorder3D(g, Rect, Border3DStyle.Etched);
                   
            }
            if (EuipmentState == UnitEuipmentState.Error)
            {
                Rect.Inflate(1, 1);
                DrawTwinkle(Rect, g);
            }
            else if (EuipmentState == UnitEuipmentState.Other)
            {
                Rect.Inflate(1, 1);
                DrawTwinkle(Rect, g);
            }
            else if (EuipmentState == UnitEuipmentState.Normal)
            {
                Rect.Inflate(1, 1);
            }

            ConveryTaskInfo ci = Data as ConveryTaskInfo;
          

            DrawTray(Rect, g);
            float x, y;
            Font ft = new Font("黑体", (float)12);
            string temp = Text;
            SizeF size = g.MeasureString(temp, ft);
            while (size.Height > Rect.Height + 4 || size.Width > Rect.Width + 4)
            {
                if (ft.Size < 9)
                {
                    size = new SizeF(0, 0);
                    temp = "";
                    break;
                }
                ft = new Font("黑体", ft.Size - (float)0.5);
                size = g.MeasureString(Text, ft);

            }

            x = Rect.X + 3;
            if (HeightRows > 1 || WidthColumns > 1)
                y = Rect.Y - 4 + (Rect.Height - 6) / 2;
            else
                y = Rect.Y - 9 + (Rect.Height - 6);
            if (EquipmentType == UnitEquipmentType.Convery && Arrow != UnitArrowDirection.Blank)
            {
                g.DrawString(Text, new Font("宋体", 9), new SolidBrush(Color.DimGray), x, y);
            }
            if (EquipmentType == UnitEquipmentType.Other)
                g.DrawString(Text, new Font("宋体", 9), new SolidBrush(Color.FromArgb(250, 10, 100)), x, y - 13);
            if (!string.IsNullOrEmpty(TextDescription))
            {
                StringFormat stringFormat;
                if (Direct == UnitOrientation.Horizontal)
                {
                    stringFormat = new StringFormat(StringFormatFlags.DisplayFormatControl);
                }
                else
                {
                    stringFormat = new StringFormat(StringFormatFlags.DirectionVertical);
                }
                float drawY = y;
                if (EquipmentType == UnitEquipmentType.Convery)
                {
                   
                    if (Arrow == UnitArrowDirection.Blank)
                    {
                        drawY = drawY + 5;
                    }
                    else
                    {
                        drawY = drawY - 10;
                    }
                }
                if (EquipmentType == UnitEquipmentType.Text)
                {
                    if ( WidthColumns > 1)
                    {
                        drawY = drawY + 5;
                    }
                    else if(WidthColumns == 1 && HeightRows == 1)
                    {
                        drawY = drawY - 10;
                    }
                   
                }
                g.DrawString(TextDescription, new Font("华文楷体", 8), new SolidBrush(Color.Blue), x, drawY, stringFormat);


            }
        }

        public override void DrawSelect(Rectangle Rect, Graphics g)
        {
            Pen pen = new Pen(Color.SpringGreen);
            pen.DashStyle = DashStyle.Dot;
            pen.Width = 2;
            g.DrawRectangle(pen, new Rectangle(Rect.X + 3, Rect.Y + 3, Rect.Width - 6, Rect.Height - 6));
        }
        Color scolor = Color.Transparent;


        #region Helper Functions
        public Rectangle GetNormalizedRectangle(int x1, int y1, int x2, int y2)
        {
            if (x2 < x1)
            {
                int tmp = x2;
                x2 = x1;
                x1 = tmp;
            }

            if (y2 < y1)
            {
                int tmp = y2;
                y2 = y1;
                y1 = tmp;
            }
            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        public Rectangle GetNormalizedRectangle(Point p1, Point p2)
        {
            return GetNormalizedRectangle(p1.X, p1.Y, p2.X, p2.Y);
        }

        public Rectangle GetNormalizedRectangle(Rectangle r)
        {
            return GetNormalizedRectangle(r.X, r.Y, r.X + r.Width, r.Y + r.Height);
        }

        /// <summary>
        /// Get handle point by 1-based number
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public override Point GetHandle(int handleNumber)
        {
            int x, y, xCenter, yCenter;

            xCenter = rectangle.X + rectangle.Width / 2;
            yCenter = rectangle.Y + rectangle.Height / 2;
            x = rectangle.X;
            y = rectangle.Y;

            switch (handleNumber)
            {
                case 1:
                    x = rectangle.X;
                    y = rectangle.Y;
                    break;
                case 2:
                    x = xCenter;
                    y = rectangle.Y;
                    break;
                case 3:
                    x = rectangle.Right;
                    y = rectangle.Y;
                    break;
                case 4:
                    x = rectangle.Right;
                    y = yCenter;
                    break;
                case 5:
                    x = rectangle.Right;
                    y = rectangle.Bottom;
                    break;
                case 6:
                    x = xCenter;
                    y = rectangle.Bottom;
                    break;
                case 7:
                    x = rectangle.X;
                    y = rectangle.Bottom;
                    break;
                case 8:
                    x = rectangle.X;
                    y = yCenter;
                    break;
            }
            return new Point(x, y);
        }

        #endregion Helper Functions

        #region Draw Tray
        private void DrawTray(Rectangle Rect, Graphics g)
        {
            if (EquipmentType == UnitEquipmentType.Convery)
            {
                if (GoodsType == UnitGoodsType.Empty)
                    return;
                Color color = Color.Transparent;

                switch (GoodsType)
                {
                    case UnitGoodsType.SingleNullPallet: color = Color.FromArgb(58 ,95 ,205); break;//淡绿
                    //case UnitGoodsType.RealPallet: color = Color.FromArgb(0, 255, 0); break;//金色
                    case UnitGoodsType.NullPalletGroup: color = Color.Blue; break;//紫蓝
                    case UnitGoodsType.RealPallet: color = Color.Orange; break;//淡绿
                    case UnitGoodsType.unknown: color = Color.Gray; break;
                    case UnitGoodsType.ExceptionPallet: color = Color.Red; break;
                    default: break;
                }
                if (Direct == UnitOrientation.Horizontal)
                {
                    Rectangle rect = new Rectangle(Rect.X, Rect.Y, Rect.Width / WidthColumns, Rect.Height);
                    if (WidthColumns > 1)
                        rect.Inflate(-(Rect.Width / 12), -3);
                    else
                        rect.Inflate(-(Rect.Width / 6), -3);
                    g.FillRectangle(new SolidBrush(color), rect);
                    ControlPaint.DrawBorder3D(g, rect, Border3DStyle.Etched);
                }
                else
                {
                    Rectangle rect = new Rectangle(Rect.X, Rect.Y, Rect.Width, Rect.Height / HeightRows);
                    if (HeightRows > 1)
                        rect.Inflate(-3, -(Rect.Height / 12));
                    else
                        rect.Inflate(-3, -(Rect.Height / 6));
                    g.FillRectangle(new SolidBrush(color), rect);
                    ControlPaint.DrawBorder3D(g, rect, Border3DStyle.Etched);
                }
            }
        }
        #endregion

        #region Dray Convery

        private void DrawConvery(Rectangle Rect, Graphics g)
        {

            if (EquipmentType == UnitEquipmentType.Convery)
            {
                DrawArrowNew(Rect,g);
                int i = 0;
                if (Direct == UnitOrientation.Horizontal)//站台水平方向
                {
                    while (i <= Rect.Width - 3)
                    {
                        int icount = (Rect.Width / Parent.UnitLW);
                        ControlPaint.DrawBorder3D(g, new Rectangle(Rect.X + i, Rect.Y + 3, 3, Rect.Height - 5), Border3DStyle.RaisedOuter);
                        i += Rect.Width / (1);
                    }
                    ControlPaint.DrawBorder3D(g, new Rectangle(Rect.X, Rect.Y, Rect.Width, 2), Border3DStyle.RaisedOuter);
                    ControlPaint.DrawBorder3D(g, new Rectangle(Rect.X, Rect.Y + (Rect.Height - 2), Rect.Width, 2), Border3DStyle.RaisedOuter);
                }
                else//垂直方向
                {
                    while (i <= Rect.Height - 3)
                    {
                        int icount = (Rect.Height / Parent.UnitLW);
                        ControlPaint.DrawBorder3D(g, new Rectangle(Rect.X + 3, Rect.Y + i, Rect.Width - 5, 3), Border3DStyle.RaisedOuter);
                        i += Rect.Height / (1);
                    }
                    ControlPaint.DrawBorder3D(g, new Rectangle(Rect.X, Rect.Y, 2, Rect.Height), Border3DStyle.RaisedOuter);
                    ControlPaint.DrawBorder3D(g, new Rectangle(Rect.X + Rect.Width - 2, Rect.Y, 2, Rect.Height), Border3DStyle.RaisedOuter);

                }
            }
        }

        #endregion

        /// <summary>
        /// zpw
        /// </summary>
        /// <param name="Rect"></param>
        /// <param name="g"></param>
        private void DrawArrowNew(Rectangle Rect, Graphics g)
        {
            AdjustableArrowCap lineCap = new AdjustableArrowCap(4, 2, true);
            Pen pen = new Pen(ArrowColor, 4);
            pen.CustomEndCap = lineCap;
            if (this.Arrow == UnitArrowDirection.Right)
            {
                g.DrawLine(pen, Rect.X + 2*Rect.Width/8, Rect.Y + Rect.Height / 2, Rect.X + Rect.Width - Rect.Width / 8, Rect.Y + Rect.Height / 2);
            }
            if (this.Arrow == UnitArrowDirection.Left)
            {
                g.DrawLine(pen, Rect.X + Rect.Width - Rect.Width / 8, Rect.Y + Rect.Height / 2, Rect.X + 2*Rect.Width / 8, Rect.Y + Rect.Height / 2 );
            }
            if (this.Arrow == UnitArrowDirection.Up)
            {
                g.DrawLine(pen, Rect.X + Rect.Width/2, Rect.Y + Rect.Height - Rect.Height / 8, Rect.X + Rect.Width / 2, Rect.Y + 2*Rect.Height / 8);
            }
            if (this.Arrow == UnitArrowDirection.Down)
            {
                g.DrawLine(pen, Rect.X + Rect.Width / 2, Rect.Y + 2*Rect.Height / 8, Rect.X + Rect.Width / 2, Rect.Y + Rect.Height - Rect.Height / 8 );
            }
            if (this.Arrow == UnitArrowDirection.LeftRight)
            {
                pen.CustomEndCap = lineCap;
                pen.CustomStartCap = lineCap;
                g.DrawLine(pen, Rect.X +  2*Rect.Width / 8, Rect.Y + Rect.Height / 2, Rect.X + Rect.Width - Rect.Width / 8, Rect.Y + Rect.Height / 2);
            }
            if (this.Arrow == UnitArrowDirection.UpDown)
            {
                pen.CustomEndCap = lineCap;
                pen.CustomStartCap = lineCap;
                g.DrawLine(pen, Rect.X + Rect.Width / 2, Rect.Y + Rect.Height -  Rect.Height / 8, Rect.X + Rect.Width / 2, Rect.Y + 2*Rect.Height / 8);
            }
        }

        #region DrawArrow
        private void DrawArrow(Rectangle Rect, Graphics g)
        {
            PointF starPointF;
            PointF endPointF;
            SizeF triangleSize = new Size(15, 15);
            PointF p1, p2, p3, p4, p5, p6, p7;
            {
                Brush brush = new SolidBrush(ArrowColor);

                GraphicsPath path = new GraphicsPath();
                GraphicsPath path1 = new GraphicsPath();

                starPointF = new PointF(Rect.X + Rect.Width - 20, Rect.Y + Rect.Height / 2);

                endPointF = new PointF(Rect.X + Rect.Width - 20, Rect.Y + Rect.Height / 2);

                triangleSize = new Size(15, 15);

                if (Arrow == UnitArrowDirection.Right)//向右
                {
                    p1 = new PointF(endPointF.X, endPointF.Y - triangleSize.Height / 2);

                    p2 = new PointF(endPointF.X, endPointF.Y + triangleSize.Height / 2);

                    p3 = new PointF(endPointF.X + triangleSize.Width, endPointF.Y);

                    p4 = new PointF(endPointF.X - 8, endPointF.Y - triangleSize.Height / 4);

                    p5 = new PointF(endPointF.X - 8, endPointF.Y + (triangleSize.Height) / 4);

                    p6 = new PointF(endPointF.X, endPointF.Y - triangleSize.Height / 4);

                    p7 = new PointF(endPointF.X, endPointF.Y + (triangleSize.Height) / 4);
                    path.AddLine(p1, p2);
                    path.AddLine(p3, p1);
                    path1.AddLine(p4, p5);
                    path1.AddLine(p4, p6);
                    path1.AddLine(p7, p6);
                    path1.AddLine(p7, p5);

                }
                else if (Arrow == UnitArrowDirection.Left)
                {
                    p1 = new PointF(endPointF.X, endPointF.Y + triangleSize.Height / 2);

                    p2 = new PointF(endPointF.X, endPointF.Y - triangleSize.Height / 2);

                    p3 = new PointF(endPointF.X - triangleSize.Width, endPointF.Y);

                    p4 = new PointF(endPointF.X + 8, endPointF.Y + triangleSize.Height / 4);

                    p5 = new PointF(endPointF.X + 8, endPointF.Y - (triangleSize.Height) / 4);

                    p6 = new PointF(endPointF.X, endPointF.Y + triangleSize.Height / 4);

                    p7 = new PointF(endPointF.X, endPointF.Y - (triangleSize.Height) / 4);
                    path.AddLine(p1, p2);
                    path.AddLine(p3, p1);
                    path1.AddLine(p4, p5);
                    path1.AddLine(p4, p6);
                    path1.AddLine(p7, p6);
                    path1.AddLine(p7, p5);
                }
                else if (Arrow == UnitArrowDirection.LeftRight)
                {
                    starPointF = new PointF(Rect.X + Rect.Width - 15, Rect.Y + Rect.Height / 2);

                    endPointF = new PointF(Rect.X + Rect.Width - 15, Rect.Y + Rect.Height / 2);
                    p1 = new PointF(endPointF.X, endPointF.Y - triangleSize.Height / 2);

                    p2 = new PointF(endPointF.X, endPointF.Y + triangleSize.Height / 2);

                    p3 = new PointF(endPointF.X + triangleSize.Width, endPointF.Y);

                    p4 = new PointF(endPointF.X - 8, endPointF.Y - triangleSize.Height / 4);

                    p5 = new PointF(endPointF.X - 8, endPointF.Y + (triangleSize.Height) / 4);

                    p6 = new PointF(endPointF.X, endPointF.Y - triangleSize.Height / 4);

                    p7 = new PointF(endPointF.X, endPointF.Y + (triangleSize.Height) / 4);
                    path.AddLine(p1, p2);
                    path.AddLine(p3, p1);
                    path1.AddLine(p4, p5);
                    path1.AddLine(p4, p6);
                    path1.AddLine(p7, p6);
                    path1.AddLine(p7, p5);
                    starPointF = new PointF(Rect.X + Rect.Width - 19, Rect.Y + Rect.Height / 2);
                    endPointF = new PointF(Rect.X + Rect.Width - 19, Rect.Y + Rect.Height / 2);
                    p1 = new PointF(endPointF.X, endPointF.Y - triangleSize.Height / 2);
                    p2 = new PointF(endPointF.X, endPointF.Y + triangleSize.Height / 2);
                    p3 = new PointF(endPointF.X - triangleSize.Width, endPointF.Y);

                    //p4 = new PointF(endPointF.X - 8, endPointF.Y - triangleSize.Height / 4);
                    //p5 = new PointF(endPointF.X - 8, endPointF.Y + (triangleSize.Height) / 4);
                    //p6 = new PointF(endPointF.X, endPointF.Y - triangleSize.Height / 4);
                    //p7 = new PointF(endPointF.X, endPointF.Y + (triangleSize.Height) / 4);
                    path.AddLine(p1, p2);
                    path.AddLine(p3, p1);
                }
                else if (Arrow == UnitArrowDirection.Up)
                {
                    starPointF = new PointF(Rect.X + Rect.Width / 2, Rect.Y + Rect.Height - 20);

                    endPointF = new PointF(Rect.X + Rect.Width / 2, Rect.Y + Rect.Height - 20);

                    p1 = new PointF(endPointF.X - triangleSize.Height / 2, endPointF.Y);

                    p2 = new PointF(endPointF.X + triangleSize.Height / 2, endPointF.Y);

                    p3 = new PointF(endPointF.X, endPointF.Y + triangleSize.Width);

                    p4 = new PointF(endPointF.X - triangleSize.Width / 4, endPointF.Y - 8);

                    p5 = new PointF(endPointF.X + triangleSize.Width / 4, endPointF.Y - 8);

                    p6 = new PointF(endPointF.X - triangleSize.Width / 4, endPointF.Y);

                    p7 = new PointF(endPointF.X + triangleSize.Width / 4, endPointF.Y);


                    path.AddLine(p1, p2);
                    path.AddLine(p3, p1);
                    path1.AddLine(p4, p5);
                    path1.AddLine(p4, p6);
                    path1.AddLine(p7, p6);
                    path1.AddLine(p7, p5);
                }
                else if (Arrow == UnitArrowDirection.Down)
                {
                    starPointF = new PointF(Rect.X + Rect.Width / 2, Rect.Y + Rect.Height - 14);

                    endPointF = new PointF(Rect.X + Rect.Width / 2, Rect.Y + Rect.Height - 14);

                    p1 = new PointF(endPointF.X - triangleSize.Height / 2, endPointF.Y);

                    p2 = new PointF(endPointF.X + triangleSize.Height / 2, endPointF.Y);

                    p3 = new PointF(endPointF.X, endPointF.Y - triangleSize.Width);

                    p4 = new PointF(endPointF.X - triangleSize.Width / 4, endPointF.Y + 8);

                    p5 = new PointF(endPointF.X + triangleSize.Width / 4, endPointF.Y + 8);

                    p6 = new PointF(endPointF.X - triangleSize.Width / 4, endPointF.Y);

                    p7 = new PointF(endPointF.X + triangleSize.Width / 4, endPointF.Y);


                    path.AddLine(p1, p2);
                    path.AddLine(p3, p1);
                    path1.AddLine(p4, p5);
                    path1.AddLine(p4, p6);
                    path1.AddLine(p7, p6);
                    path1.AddLine(p7, p5);

                }
                else if (Arrow == UnitArrowDirection.UpDown)
                {
                    starPointF = new PointF(Rect.X + Rect.Width / 2, Rect.Y + Rect.Height - 15);

                    endPointF = new PointF(Rect.X + Rect.Width / 2, Rect.Y + Rect.Height - 15);

                    p1 = new PointF(endPointF.X - triangleSize.Height / 2, endPointF.Y);

                    p2 = new PointF(endPointF.X + triangleSize.Height / 2, endPointF.Y);

                    p3 = new PointF(endPointF.X, endPointF.Y + triangleSize.Width);

                    p4 = new PointF(endPointF.X - triangleSize.Width / 4, endPointF.Y - 8);

                    p5 = new PointF(endPointF.X + triangleSize.Width / 4, endPointF.Y - 8);

                    p6 = new PointF(endPointF.X - triangleSize.Width / 4, endPointF.Y);

                    p7 = new PointF(endPointF.X + triangleSize.Width / 4, endPointF.Y);


                    path.AddLine(p1, p2);
                    path.AddLine(p3, p1);
                    path1.AddLine(p4, p5);
                    path1.AddLine(p4, p6);
                    path1.AddLine(p7, p6);
                    path1.AddLine(p7, p5);

                    starPointF = new PointF(Rect.X + Rect.Width / 2, Rect.Y + Rect.Height - 20);

                    endPointF = new PointF(Rect.X + Rect.Width / 2, Rect.Y + Rect.Height - 20);

                    p1 = new PointF(endPointF.X - triangleSize.Height / 2, endPointF.Y);

                    p2 = new PointF(endPointF.X + triangleSize.Height / 2, endPointF.Y);

                    p3 = new PointF(endPointF.X, endPointF.Y - triangleSize.Width);

                    path.AddLine(p1, p2);
                    path.AddLine(p3, p1);

                }



                g.FillPath(brush, path);
                g.FillPath(brush, path1);
            }
        }

        #endregion

        #region Move Convery

        /// <summary>
        /// Move object
        /// </summary>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        public void Move(Rectangle Rect, int deltaX, int deltaY)
        {
            Rect.X += deltaX;
            Rect.Y += deltaY;
        }
        #endregion

        #region 闪烁效果

        private void DrawTwinkle(Rectangle rect, Graphics g)
        {
         //   Pen pen1 = new Pen(TwinkleColor, 2);
           // pen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;//绘制虚线框
            g.FillRectangle(new SolidBrush(Color.FromArgb(255, 99, 71)),rect);
            //g.DrawRectangle(pen1, rect.X, rect.Y, rect.Width, rect.Height);
        }
        #endregion


    }

}
