using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CWCS.UIDesigner.Tool;
using System.Drawing.Drawing2D;
using System.Timers;

namespace CWCS.UIDesigner.ConveryControl
{
    public partial class UserConvery : Panel
    {
        public List<DrawRectangle> Units = new List<DrawRectangle>();//需要绘制的对象集合
        private int unitLW = 30;
        Point? m_MousedownPoint = null;
        Point? m_lastPoint;
        Rectangle? m_MousedownRect;
        Rectangle? m_lastRect;
        ConveryUnit m_lastUnit;
        ConveryUnit m_MousedownUnit;
        List<DrawRectangle> SelectedUnits = new List<DrawRectangle>();
        List<DrawRectangle> tempSelectedUnits = new List<DrawRectangle>();
        Point lastPoint;
        private System.Timers.Timer t_Timer = new System.Timers.Timer();
        public UserConvery()
        {
            if (DesignMode) return;
            //InitializeComponent();
            this.AllowDrop = true;
            this.DoubleBuffered = true;
            t_Timer.Interval = 400;
            t_Timer.Elapsed += new ElapsedEventHandler(t_Timer_Elapsed);

            Units.Add(new DrawRectangle(0, 1, 60, unitLW));
            Units.Add(new DrawRectangle(60, 1, 60, unitLW));
            Units.Add(new DrawRectangle(120, 1, 60, unitLW));
            Units.Add(new DrawRectangle(180, 1, 60, unitLW));
            Units.Add(new DrawRectangle(240, 1, 90, unitLW));
        
        }
        private void t_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Invalidate();
        }

        public void Add(DrawRectangle unit)
        {
            if (!Units.Exists(it => it.ObjNum == unit.ObjNum))
            {
                Units.Add(unit);
                //unit.Parent = this;
                //ResizeScrollMinSize();
            }
        }

        public void Remove(DrawRectangle unit)
        {
            Units.Remove(unit);
            //unit.Parent = null;
            //ResizeScrollMinSize();
        }

        public bool Contains(DrawRectangle unit)
        {
            return Units.Contains(unit);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            DrawUnits(pe);
        }

        void DrawUnits(PaintEventArgs e)
        {
            if (Units == null || Units.Count <= 0)
                return;
            foreach (var item in Units)
            {
                item.Draw(e.Graphics);
            }
        }


        public void DrawError(ConveryUnit u, Rectangle rect, Graphics g)
        {
            u.DrawUnit(rect, g);
        }

       

        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_MousedownPoint = GetPointCurr();
            m_lastPoint = m_MousedownPoint;
            m_MousedownRect = GetRectCurr();
            m_lastRect = m_MousedownRect;
            m_MousedownUnit = null;
            lastPoint = new DrawArea().BackTrackMouse(e.Location);
            DrawObject o = null;

            foreach (var item in Units)
            {
                if (item.HitTest(lastPoint) == 0)
                {
                   o=item;
                   break;
                }
            }
            if (o != null)
            {
                if (!o.Selected)
                {
                    for (int i = 0; i < Units.Count; i++)
                    {
                        Units[i].Selected = false;
                        Units[i].Draw(Graphics.FromHwnd(this.Handle));
                    }
                    o.Selected = true;
                    o.Draw(Graphics.FromHwnd(this.Handle));
                    SelectedUnits.Add(o as DrawRectangle);
                }
            }
            if (ModifierKeys == Keys.Shift && SelectedUnits.Count > 0)
            {
                this.DoDragDrop(SelectedUnits, DragDropEffects.Move);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            this.Focus();
            m_lastPoint = GetPointCurr();
            DrawObject o = null;

            foreach (var item in Units)
            {
                if (item.HitTest(lastPoint) == 0)
                {
                    o = item;
                    break;
                }
            }
            if(o!=null)
              SelectedUnits.Add(o as DrawRectangle);
           

            base.OnMouseClick(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
          
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (MouseButtons == MouseButtons.Left)
            {
                DrawObject o = null;

                //foreach (var item in Units)
                //{
                //    if (item.Selected)
                //    {
                //        o = item;
                //        break;
                //    }
                //}
                //if (o != null)
                //{
                //    if (o.Selected)
                //    {
                //        Point point = new DrawArea().BackTrackMouse(new Point(e.X, e.Y));

                //        o.MoveHandleTo(point, 5);

                //    }
                //}
            }
            base.OnMouseMove(e);
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
        List<Rectangle> lastCellsMove = new List<Rectangle>();

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

            foreach (DrawRectangle u in SelectedUnits)
            {
                Graphics.FromHwnd(this.Handle).FillRectangle(new SolidBrush(this.BackColor), u.Rectangle);
                if (u != null)
                {
                    u.SetRectangle(u.Rectangle.X + x, u.Rectangle.Y + y, u.Rectangle.Width, u.Rectangle.Height);
                }
            }
            //ResizeScrollMinSize();

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

            }

            lastCellsMove.Clear();

            Rectangle rectall = new Rectangle();
            GraphicsPath path = new GraphicsPath();
            List<Point> ps = new List<Point>();
            ps.Clear();




            rectall.Inflate(-1, -1);
            lastCellsMove.Clear();

            lastCellsMove.Clear();
            lastCellsMove.Add(rectall);

            Pen pen1 = new Pen(Color.Red, 2);
            pen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;//绘制虚线框
            Graphics.FromHwnd(this.Handle).DrawRectangle(pen1, rectall.X + this.AutoScrollPosition.X, rectall.Y + this.AutoScrollPosition.Y, rectall.Width, rectall.Height);
        }
    }
}
