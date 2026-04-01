using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace CWCS.UIDesigner.RobotControl
{
    public partial class RobotBaseControl : UserControl
    {
        protected float winX = 1;
        protected float winY = 1;
        public RobotBaseControl()
        {
            InitializeComponent();
        }

        protected void SetTag(System.Windows.Forms.Control cons)
        {
            foreach (System.Windows.Forms.Control con in cons.Controls)
            {
                if (con != null)
                {
                    if (con is TrayControl)
                        con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size + ":" + ((TrayControl)con).TrayWidth + ":" + ((TrayControl)con).TrayHeight;
                    else
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
                    if (con is Label || con is TextBox)
                        continue;
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

                        Single currentSize = Convert.ToSingle(myTag[4]) * Math.Min(x, y);
                       // con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                        if (con is TrayControl)
                        {
                            tempValue = Convert.ToSingle(myTag[5]) * y;
                            ((TrayControl)con).TrayWidth = (int)(tempValue);
                            tempValue = Convert.ToSingle(myTag[6]) * y;
                            ((TrayControl)con).TrayHeight = (int)(tempValue);
                        }
                        if (con.Controls.Count > 0)
                            setControls(x, y, con);
                    }
                }
            }
        }
    }
}
