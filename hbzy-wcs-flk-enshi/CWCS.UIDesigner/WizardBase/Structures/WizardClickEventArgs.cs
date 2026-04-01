using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.UIDesigner.WizardBase
{
    public class WizardClickEventArgs : EventArgs
    {
        private bool cancel;

        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }
    }
}
