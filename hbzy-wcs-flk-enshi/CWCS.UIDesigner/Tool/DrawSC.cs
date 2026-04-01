using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CWCS.UIDesigner.Tool
{
   public  class DrawSC:DrawObject
    {
        private Rectangle rectangle;

        private const string entryRectangle = "Rect";

        public Rectangle Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }

        public override DrawObject Clone()
        {
            DrawSC drawRectangle = new DrawSC();
            drawRectangle.rectangle = rectangle;

            FillDrawObjectFields(drawRectangle);
            return drawRectangle;
        }

        public override void Draw(Graphics g)
        {

        }
    }
}
