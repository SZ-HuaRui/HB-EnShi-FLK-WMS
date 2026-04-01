using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace CWCS.UIDesigner.Tool
{
    public partial class DrawArea : UserControl
    {
        #region Constructor, Dispose
        public DrawArea()
        {
            // create list of Layers, with one default active visible layer
            //_layers = new Layers();
            // _layers.CreateNewLayer("Default");
            _panning = false;
            _panX = 0;
            _panY = 0;
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.DrawArea_MouseWheel);
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            //DrawImage o = new DrawImage(0, 0);
            //DrawTools.ToolObject.AddNewObject(this, new DrawImage(0, 0));

            drawObject = new DrawRectangle();
        }
        #endregion Constructor, Dispose

        #region Members
        private float _zoom = 1.0f;
        private float _rotation = 0f;
        private int _panX = 0;
        private int _panY;
        private int _originalPanY;
        private bool _panning = false;
        private Point lastPoint;
        private Color _lineColor = Color.Black;
        private Color _fillColor = Color.White;
        private bool _drawFilled = false;
        private int _lineWidth = -1;
        private DrawingPens.PenType _penType;
        private Pen _currentPen;
        private Brush _currentBrush;
        
        // group selection rectangle
        private Rectangle netRectangle;
        private bool drawNetRectangle = false;

        private Form myparent;
        private ToolRectangle _tool;
        private DrawObject drawObject;
        internal ToolRectangle Tool
        {
            get { return _tool; }
            set { _tool = value; }
        }
        public Form MyParent
        {
            get { return myparent; }
            set { myparent = value; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Allow tools and objects to see the type of pen set
        /// </summary>
        public DrawingPens.PenType PenType
        {
            get { return _penType; }
            set { _penType = value; }
        }
        public Brush CurrentBrush
        {
            get { return _currentBrush; }
            set { _currentBrush = value; }
        }
        /// <summary>
        /// Current Drawing Pen
        /// </summary>
        public Pen CurrentPen
        {
            get { return _currentPen; }
            set { _currentPen = value; }
        }

        /// <summary>
        /// Current Line Width
        /// </summary>
        public int LineWidth
        {
            get { return _lineWidth; }
            set { _lineWidth = value; }
        }

        /// <summary>
        /// Flag determines if objects will be drawn filled or not
        /// </summary>
        public bool DrawFilled
        {
            get { return _drawFilled; }
            set { _drawFilled = value; }
        }

        /// <summary>
        /// Color to draw filled objects with
        /// </summary>
        public Color FillColor
        {
            get { return _fillColor; }
            set { _fillColor = value; }
        }

        /// <summary>
        /// Color for drawing lines
        /// </summary>
        public Color LineColor
        {
            get { return _lineColor; }
            set { _lineColor = value; }
        }

        /// <summary>
        /// Original Y position - used when panning
        /// </summary>
        public int OriginalPanY
        {
            get { return _originalPanY; }
            set { _originalPanY = value; }
        }

        /// <summary>
        /// Flag is true if panning active
        /// </summary>
        public bool Panning
        {
            get { return _panning; }
            set { _panning = value; }
        }

        /// <summary>
        /// Current pan offset along X-axis
        /// </summary>
        public int PanX
        {
            get { return _panX; }
            set { _panX = value; }
        }

        /// <summary>
        /// Current pan offset along Y-axis
        /// </summary>
        public int PanY
        {
            get { return _panY; }
            set { _panY = value; }
        }

        /// <summary>
        /// Degrees of rotation of the drawing
        /// </summary>
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        /// <summary>
        /// Current Zoom factor
        /// </summary>
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        /// <summary>
        /// Group selection rectangle. Used for drawing.
        /// </summary>
        public Rectangle NetRectangle
        {
            get { return netRectangle; }
            set { netRectangle = value; }
        }

        /// <summary>
        /// Flag is set to true if group selection rectangle should be drawn.
        /// </summary>
        public bool DrawNetRectangle
        {
            get { return drawNetRectangle; }
            set { drawNetRectangle = value; }
        }

      
        #endregion

        private void DrawArea_MouseWheel(object sender, MouseEventArgs e)
        {
        }

        #region Event Handlers
        /// <summary>
        /// Draw graphic objects and group selection rectangle (optionally)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawArea_Paint(object sender, PaintEventArgs e)
        {
            Matrix mx = new Matrix();
            mx.Translate(-ClientSize.Width / 2f, -ClientSize.Height / 2f, MatrixOrder.Append);
            mx.Rotate(_rotation, MatrixOrder.Append);
            mx.Translate(ClientSize.Width / 2f + _panX, ClientSize.Height / 2f + _panY, MatrixOrder.Append);
            mx.Scale(_zoom, _zoom, MatrixOrder.Append);
            e.Graphics.Transform = mx;
            // Determine center of ClientRectangle
            Point centerRectangle = new Point();
            centerRectangle.X = ClientRectangle.Left + ClientRectangle.Width / 2;
            centerRectangle.Y = ClientRectangle.Top + ClientRectangle.Height / 2;
            // Get true center point
            centerRectangle = BackTrackMouse(centerRectangle);
            // Determine offset from current mouse position

            SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 251));
            e.Graphics.FillRectangle(brush,
                         ClientRectangle);

            DrawNetSelection(e.Graphics);

            drawObject.Draw(e.Graphics);

            brush.Dispose();
        }
        #endregion


        /// <summary>
        /// Back Track the Mouse to return accurate coordinates regardless of zoom or pan effects.
        /// </summary>
        /// <param name="p">Point to backtrack</param>
        /// <returns>Backtracked point</returns>
        public Point BackTrackMouse(Point p)
        {
            // Backtrack the mouse...
            Point[] pts = new Point[] { p };
            Matrix mx = new Matrix();
            mx.Translate(-ClientSize.Width / 2f, -ClientSize.Height / 2f, MatrixOrder.Append);
            mx.Rotate(_rotation, MatrixOrder.Append);
            mx.Translate(ClientSize.Width / 2f + _panX, ClientSize.Height / 2f + _panY, MatrixOrder.Append);
            mx.Scale(_zoom, _zoom, MatrixOrder.Append);
            mx.Invert();
            mx.TransformPoints(pts);
            return pts[0];
        }

        /// <summary>
        ///  Draw group selection rectangle
        /// </summary>
        /// <param name="g"></param>
        public void DrawNetSelection(Graphics g)
        {
            if (!DrawNetRectangle)
                return;
            ControlPaint.DrawFocusRectangle(g, NetRectangle, Color.Black, Color.Transparent);
           
        }

        /// <summary>
        /// Mouse down.
        /// Left button down event is passed to active tool.
        /// Right button down event is handled in this class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawArea_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = BackTrackMouse(e.Location);
          
        }

        //        else if (e.Button == MouseButtons.Right)
        //{
        //if (_panning == true)
        //_panning = false; 

        //if (activeTool == DrawToolType.PolyLine)
        //tools[(int)activeTool].OnMouseDown(this, e);

        //ActiveTool = TETemplateDrawArea.DrawToolType.Pointer;
        //}

        /// <summary>
        /// Mouse move.
        /// Moving without button pressed or with left button pressed
        /// is passed to active tool.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawArea_MouseMove(object sender, MouseEventArgs e)
        {
            Point curLoc = BackTrackMouse(e.Location);
            if (e.Button == MouseButtons.Left ||
              e.Button == MouseButtons.None)
                if (e.Button == MouseButtons.Left && _panning)
                {
                    if (curLoc.X !=
                      lastPoint.X)
                        _panX += curLoc.X - lastPoint.X;
                    if (curLoc.Y !=
                      lastPoint.Y)
                        _panY += curLoc.Y - lastPoint.Y;
                    Invalidate();
                }
                else
                {
                    //drawObject = new DrawRectangle(this._panX, this._panY, this.Width, this.Height);
                }
            else
                Cursor = Cursors.Default;
            lastPoint = BackTrackMouse(e.Location);
        }

        /// <summary>
        /// Mouse up event.
        /// Left button up event is passed to active tool.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawArea_MouseUp(object sender, MouseEventArgs e)
        {
            //lastPoint = BackTrackMouse(e.Location);
            if (e.Button ==
              MouseButtons.Left)
            {
                //this.AddCommandToHistory(new CommandAdd(this.TheLayers[al].Graphics[0]));
               // tools[(int)activeTool].OnMouseUp(this, e);
            }
        }
    }
       
}
