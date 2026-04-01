using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CWCS.CommonControl
{
    /// <summary>
    /// Author:zpw
    /// Description:Control for station
    /// </summary>
    public partial class StationControl : Control
    {
      
        private string _StationNO = "0";
        private string _TaskNO = "0";
        private string _FromStation = "0";
        private string _ToStation = "0";
        private string _GoodsType = "0";
        private string _ErrorInfo = "0";
        private string _Barcode = "0";
        private string _BarcodeHigh = "0";
        private string _Qty = "0";
        private string _IsLast = "0";
        private Color _PhotorTubeColor = Color.Lime;
        private ArrowDirection _ArrowDirection = ArrowDirection.Left;
        private int _BorderWidth = 0;
        private Color _BorderColor = Color.LightGray;
        private int _Station_X;
        private int _Station_Y;
        private int _Station_Z;
        private Color _ArrowColor = Color.Lime;
        private Font _StationNOFont = new Font("宋体",10f);
        private Color _StationNOColor = Color.Black;
        private bool _IsEmptyPallet = false;
        private Color _goodsColor_1 = Color.Gray;
        private Color _goodsColor_2 = Color.Blue;
        private Color _goodsColor_3 = Color.Blue;
        private Color _goodsColor_4 = Color.Blue;
        private Color _goodsColor_5 = Color.Blue;
        private Color _goodsColor_6 = Color.Blue;
        private Color _goodsColor_7 = Color.Blue;
        private Color _goodsColor_8 = Color.Blue;
        private Color _goodsColor_9 = Color.Blue;
        private Color _goodsColor_10 = Color.Blue;
        private string _DescriptionContent;
        private Color _DescriptionColor = Color.SlateBlue;
        private TextDirection _DescriptionDirection = TextDirection.Horizontal;
        private Font _DescriptionFont = new Font("宋体",10f);
        [Browsable(false)]
        public delegate void DelegateShowStationInfo();

        private int BaseLength { get; set; }
        public StationControl()
        {
            InitializeComponent();
        }

        public void Station_MoveEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        public void Station_OnClick(object sender, EventArgs e)
        {
            //this.BackColor = Color.Tan;
        }

        [Browsable(true)]
        [Description("任务号")]
        [Category("站台参数")]
        [DefaultValue("0")]
        public string TaskNO
        {
            get => _TaskNO;
            set => _TaskNO = value;
        }

        [Browsable(true)]
        [Description("数量")]
        [Category("站台参数")]
        [DefaultValue("0")]
        public string Qty
        {
            get => _Qty;
            set => _Qty = value;
        }

        [Browsable(true)]
        [Description("是否是空托盘")]
        [Category("站台参数")]
        [DefaultValue(false)]
        public bool IsEmptyPallet
        {
            get => _IsEmptyPallet;
            set => _IsEmptyPallet = value;
        }

        [Browsable(true)]
        [Description("站台号的字体")]
        [Category("站台参数")]
        public Font StationNOFont
        {
            get => _StationNOFont;
            set => _StationNOFont = value;
        }

        [Browsable(true)]
        [Description("站台号的颜色")]
        [Category("站台参数")]
        public Color StationNOColor
        {
            get => _StationNOColor;
            set => _StationNOColor = value;
        }

        [Browsable(true)]
        [Description("货物类型1的颜色")]
        [Category("站台参数")]
        public Color GoodsColor_1
        {
            get => _goodsColor_1;
            set => _goodsColor_1 = value;
        }

        [Browsable(true)]
        [Description("货物类型2的颜色")]
        [Category("站台参数")]
        public Color GoodsColor_2
        {
            get => _goodsColor_2;
            set => _goodsColor_2 = value;
        }
        [Browsable(true)]
        [Description("货物类型3的颜色")]
        [Category("站台参数")]
        public Color GoodsColor_3
        {
            get => _goodsColor_3;
            set => _goodsColor_3 = value;
        }
        [Browsable(true)]
        [Description("货物类型4的颜色")]
        [Category("站台参数")]
        public Color GoodsColor_4
        {
            get => _goodsColor_4;
            set => _goodsColor_4 = value;
        }

        [Browsable(true)]
        [Description("货物类型5的颜色")]
        [Category("站台参数")]
        public Color GoodsColor_5
        {
            get => _goodsColor_5;
            set => _goodsColor_5 = value;
        }

        [Browsable(true)]
        [Description("货物类型6的颜色")]
        [Category("站台参数")]
        public Color GoodsColor_6
        {
            get => _goodsColor_6;
            set => _goodsColor_6 = value;
        }

        [Browsable(true)]
        [Description("货物类型7的颜色")]
        [Category("站台参数")]
        public Color GoodsColor_7
        {
            get => _goodsColor_7;
            set => _goodsColor_7 = value;
        }

        [Browsable(true)]
        [Description("货物类型8的颜色")]
        [Category("站台参数")]
        public Color GoodsColor_8
        {
            get => _goodsColor_8;
            set => _goodsColor_8 = value;
        }

        [Browsable(true)]
        [Description("货物类型9的颜色")]
        [Category("站台参数")]
        public Color GoodsColor_9
        {
            get => _goodsColor_9;
            set => _goodsColor_9 = value;
        }

        [Browsable(true)]
        [Description("货物类型10的颜色")]
        [Category("站台参数")]
        public Color GoodsColor_10
        {
            get => _goodsColor_10;
            set => _goodsColor_10 = value;
        }
        [Browsable(false)]
        [Description("光电管颜色")]
        [Category("站台参数")]
        public Color PhotorTubeColor
        {
            get => _PhotorTubeColor;
            set => _PhotorTubeColor = value;
        }
        [Browsable(true)]
        [Description("站台号")]
        [Category("站台参数")]
        public string StationNO
        {
            get
            {
                return _StationNO;
            }
            set
            {
                _StationNO = value;
            }
        }
        [Browsable(true)]
        [Description("起始地址")]
        [Category("站台参数")]
        public string FromStation
        {
            get => _FromStation;
            set => _FromStation = value;
        }
        [Browsable(true)]
        [Description("目标地址")]
        [Category("站台参数")]
        public string ToStation
        {
            get => _ToStation;
            set => _ToStation = value;
        }
        [Browsable(true)]
        [Description("货物类型")]
        [Category("站台参数")]
        public string GoodsType
        {
            get => _GoodsType;
            set => _GoodsType = value;
        }
        [Browsable(true)]
        [Description("故障信息")]
        [Category("站台参数")]
        public string ErrorInfo
        {
            get => _ErrorInfo;
            set => _ErrorInfo = value;
        }
        [Browsable(true)]
        [Description("托盘条码")]
        [Category("站台参数")]
        public string Barcode
        {
            get => _Barcode;
            set => _Barcode = value;
        }

        [Browsable(true)]
        [Description("上箱号")]
        [Category("站台参数")]
        public string BarcodeHigh
        {
            get => _BarcodeHigh;
            set => _BarcodeHigh = value;
        }

        [Browsable(true)]
        [Description("是否尾箱")]
        [Category("站台参数")]
        public string IsLast
        {
            get => _IsLast;
            set => _IsLast = value;
        }
        [Browsable(true)]
        [Description("站台箭头方向")]
        [Category("站台参数")]
        public ArrowDirection ArrowDirection
        {
            get => _ArrowDirection;
            set => _ArrowDirection = value;
        }

        [Browsable(true)]
        [Description("站台箭头颜色")]
        [Category("站台参数")]
        public Color ArrowColor
        {
            get => _ArrowColor;
            set => _ArrowColor = value;
        }


        [Browsable(true)]
        [Description("站台边框宽度")]
        [Category("站台参数")]
        public int BorderWidth
        {
            get => _BorderWidth;
            set => _BorderWidth = value;
        }
        [Browsable(true)]
        [Description("站台边框颜色")]
        [Category("站台参数")]
        public Color BorderColor
        {
            get => _BorderColor;
            set => _BorderColor = value;
        }
        [Browsable(true)]
        [Description("站台描述的颜色")]
        [Category("站台参数")]
        public Color DescriptionColor
        {
            get => _DescriptionColor;
            set => _DescriptionColor = value;
        }

        [Browsable(true)]
        [Description("站台描述的文字方向")]
        [Category("站台参数")]
        public TextDirection DescriptionDirection
        {
            get => _DescriptionDirection;
            set => _DescriptionDirection = value;
        }

        [Browsable(true)]
        [Description("站台描述内容")]
        [Category("站台参数")]
        public string DescriptionContent
        {
            get => _DescriptionContent;
            set => _DescriptionContent = value;
        }

        [Browsable(true)]
        [Description("站台描述的字体")]
        [Category("站台参数")]
        public Font DescriptionFont
        {
            get => _DescriptionFont;
            set => _DescriptionFont = value;
        }


        private Rectangle _rectangleTop;
        private Rectangle _rectangleBelow;
        private Rectangle _rectangleLeft;
        private Rectangle _rectangleRight;

        //水平间隔
        private int _horizontalDistance = 0;
        //垂直间隔
        private int _verticalDistance = 0;

        #region 事件
        //public void OnClick(object sender,EventArgs e)
        //{
        //    if(((MouseEventArgs)e).Button == MouseButtons.Left)
        //    {
        //        ShowStationInfoEvent?.Invoke();
        //    }
           
        //}

        #endregion

        /// <summary>
        /// Draw station border
        /// </summary>
        /// <param name="g"></param>
        public void DrawBorder(Graphics g)
        {
            //上边
            _rectangleTop = new Rectangle(0,0, this.Width, this.BorderWidth);
            g.FillRectangle(new SolidBrush(BorderColor), _rectangleTop);
            //下边
            _rectangleBelow = new Rectangle(0, 0 + this.Height - this.BorderWidth, this.Width, this.BorderWidth);
            g.FillRectangle(new SolidBrush(BorderColor), _rectangleBelow);
            //左边
            _rectangleLeft = new Rectangle(0,0, this.BorderWidth, this.Height);
            g.FillRectangle(new SolidBrush(BorderColor), _rectangleLeft);
            //右边
            _rectangleRight = new Rectangle(0 - this.BorderWidth + this.Width,0, this.BorderWidth, this.Height);
            g.FillRectangle(new SolidBrush(BorderColor), _rectangleRight);
            if (_horizontalDistance == 0)
            {
                _horizontalDistance = _rectangleRight.X - _rectangleLeft.X - this.BorderWidth;
            }
            if (_verticalDistance == 0)
            {
                _verticalDistance = _rectangleBelow.Y - _rectangleTop.Y - this.BorderWidth;
            }

        }



        public void DrawArrow(Graphics g)
        {
            int arrrow_X =  _rectangleLeft.X + (_rectangleRight.X - _rectangleLeft.X) / 2;
            int arrow_Y = _rectangleTop.Y + (_rectangleBelow.Y - _rectangleTop.Y) / 2;
            System.Drawing.Drawing2D.AdjustableArrowCap lineCap = new AdjustableArrowCap(4,2,true);
            Pen pen = new Pen(ArrowColor,4);
            if (this.ArrowDirection == ArrowDirection.Right)
            {
                pen.CustomEndCap = lineCap;
                arrrow_X = _rectangleLeft.X + 2 * BorderWidth;
                g.DrawLine(pen, arrrow_X, arrow_Y, _rectangleRight.X - this.BorderWidth, arrow_Y);
            }
            if (this.ArrowDirection == ArrowDirection.Left)
            {
                pen.CustomStartCap = lineCap;
                arrrow_X = _rectangleLeft.X + 2 * BorderWidth;
                g.DrawLine(pen, arrrow_X, arrow_Y, _rectangleRight.X - this.BorderWidth, arrow_Y);
            }
            if (this.ArrowDirection == ArrowDirection.Up)
            {
                pen.CustomEndCap = lineCap;
                arrow_Y = this._rectangleBelow.Y - this.BorderWidth;
                g.DrawLine(pen, arrrow_X, arrow_Y, arrrow_X, _rectangleTop.Y+2*this.BorderWidth);
            }
            if (this.ArrowDirection == ArrowDirection.Down)
            {
                pen.CustomStartCap = lineCap;
                arrow_Y = this._rectangleBelow.Y - this.BorderWidth;
                g.DrawLine(pen, arrrow_X, arrow_Y, arrrow_X, _rectangleTop.Y + 2 * this.BorderWidth);
            }
            if (this.ArrowDirection == ArrowDirection.LeftRight)
            {
                pen.CustomEndCap = lineCap;
                pen.CustomStartCap = lineCap;
                arrrow_X = _rectangleLeft.X + 2 * BorderWidth;
                g.DrawLine(pen, arrrow_X, arrow_Y, _rectangleRight.X - this.BorderWidth, arrow_Y);
            }
            if (this.ArrowDirection == ArrowDirection.UpDown)
            {
                pen.CustomEndCap = lineCap;
                pen.CustomStartCap = lineCap;
                arrow_Y = this._rectangleBelow.Y - this.BorderWidth;
                g.DrawLine(pen, arrrow_X, arrow_Y, arrrow_X, _rectangleTop.Y + 2 * this.BorderWidth);
            }

        }

        public void DrawPhotorTubeColor(Graphics g)
        {
            int phtotorTuber_X = _rectangleRight.X;
            int phtotorTuber_Y = _rectangleRight.Y ;
            Rectangle photorTubeRectangle = new Rectangle(phtotorTuber_X,phtotorTuber_Y,this.BorderWidth,this.BorderWidth);
            g.FillRectangle(new SolidBrush(this.PhotorTubeColor),photorTubeRectangle);
        }

        public void DrawGoods(Graphics g)
        {
            
            int goods_X = _rectangleLeft.X + this.BorderWidth + _horizontalDistance / 6;
            int goods_Y = _rectangleTop.Y + this.BorderWidth + _verticalDistance / 6;
            Color drawColor = Color.Lime;
            switch (this.GoodsType)
            {
                case "1":drawColor = this.GoodsColor_1;break;
                case "2": drawColor = this.GoodsColor_2; break;
                case "3": drawColor = this.GoodsColor_3; break;
                case "4": drawColor = this.GoodsColor_4; break;
                case "5": drawColor = this.GoodsColor_5; break;
                case "6": drawColor = this.GoodsColor_6; break;
                case "7": drawColor = this.GoodsColor_7; break;
                case "8": drawColor = this.GoodsColor_8; break;
                case "9": drawColor = this.GoodsColor_9; break;
                case "10": drawColor = this.GoodsColor_10; break;
            }
            g.FillRectangle(new SolidBrush(Color.FromArgb(255, drawColor)), new Rectangle(goods_X, goods_Y, _horizontalDistance*4/6, _verticalDistance*4/ 6));
        }

        public void DrawEmptyPallet(Graphics g)
        {
            int EmptyPallet_X = _rectangleLeft.X + 2 * this.BorderWidth;
            int EmptyPallet_Y = _rectangleTop.Y + this.BorderWidth;
            g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255,222,173)),new Rectangle(EmptyPallet_X,EmptyPallet_Y,5,this.Height - this.BorderWidth));
            g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 222, 173)), new Rectangle(EmptyPallet_X+10, EmptyPallet_Y,5, this.Height - this.BorderWidth));
            g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 222, 173)), new Rectangle(EmptyPallet_X+20, EmptyPallet_Y,5, this.Height - this.BorderWidth));
        }

        public void DrawStationNO(Graphics g)
        {
            int stationNO_X = _rectangleLeft.X +this.BorderWidth;
            int stationNO_Y = _rectangleBelow.Y - this.StationNOFont.Height;
            g.DrawString(this.StationNO,this.StationNOFont,new SolidBrush(this.StationNOColor),stationNO_X,stationNO_Y);
        }

        public void DrawDescription(Graphics g)
        {
            if (!string.IsNullOrEmpty(this.DescriptionContent))
            {
                if (this.DescriptionDirection == TextDirection.Horizontal)
                {
                    g.DrawString(this.DescriptionContent,this.DescriptionFont,new SolidBrush(this.DescriptionColor),this._rectangleLeft.X+this.BorderWidth,this._rectangleTop.Y+this.BorderWidth+_verticalDistance/6);
                }
                if (this.DescriptionDirection == TextDirection.Vertical)
                {
                    g.DrawString(this.DescriptionContent, this.DescriptionFont, new SolidBrush(this.DescriptionColor), this._rectangleLeft.X + this.BorderWidth+_horizontalDistance/6, this._rectangleTop.Y + this.BorderWidth);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = this.CreateGraphics();
            DrawBorder(g);
           // DrawPhotorTubeColor(g);

            if (!this.GoodsType.Equals("0"))
            {
                //if (IsEmptyPallet)
                //{
                //    DrawEmptyPallet(g);
                //}
                //else
                //{
                //    DrawGoods(g);
                //}
                DrawGoods(g);
            }
            DrawArrow(g);
            DrawDescription(g);
            DrawStationNO(g);
            base.OnPaint(pe);
        }
    }

    public enum ArrowDirection:int
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
        UpDown = 4,
        LeftRight = 5

    }

    public enum TextDirection : int
    {
      Horizontal = 0,
      Vertical = 1
    }

}
