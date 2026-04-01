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
    public partial class PickRobotControl : RobotBaseControl
    {
        public PickRobotControl()
        {
            InitializeComponent();
            //this.Resize += new EventHandler(PickRobotControl_Resize);
            //SetTag(this);
            //winX = this.Width;
            //winY = this.Height;
        }
        //void PickRobotControl_Resize(object sender, EventArgs e)
        //{
        //    float newx = (this.Width) / winX;
        //    float newy = (this.Height) / winY;
        //    setControls(newx, newy, this);
        //}
    }
}
