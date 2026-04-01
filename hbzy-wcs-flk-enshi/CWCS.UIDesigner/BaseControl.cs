using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CWCS.UIDesigner
{
    public  abstract class BaseControl 
    {
        protected float winX = 1;
        protected float winY = 1;

        protected void SetTag(System.Windows.Forms.Control cons)
        {
            foreach (System.Windows.Forms.Control con in cons.Controls)
            {
                if (con != null)
                {
                  con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                  if (con.Controls.Count > 0)
                       SetTag(con);
                }
            }
        }

        protected void setControls(float x, float y, System.Windows.Forms.Control parent)
        {
            foreach (System.Windows.Forms.Control con in parent.Controls)
            {
                if (con != null)
                {
                    if (con.Tag.ToString().Contains(":"))
                    {
                        string[] myTag = con.Tag.ToString().Split(':');
                        float tempValue = Convert.ToSingle(myTag[0]) * x;
                        con.Width = (int)tempValue;
                        tempValue = Convert.ToSingle(myTag[1]) * y;
                        con.Height = (int)tempValue;

                        tempValue = Convert.ToSingle(myTag[2]) * x;
                        con.Left = (int)tempValue;

                        tempValue = Convert.ToSingle(myTag[3]) * y;
                        con.Top = (int)tempValue;
                        if (con.Controls.Count > 0)
                            setControls(x, y, con);
                    }
                }
            }
        }


    }
}
