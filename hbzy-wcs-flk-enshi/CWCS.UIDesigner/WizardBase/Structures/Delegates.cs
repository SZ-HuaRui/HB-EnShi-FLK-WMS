using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.UIDesigner.WizardBase
{
    public delegate void WizardClickEventHandler(ConveryWizardControl sender, WizardClickEventArgs args);

    public delegate void WizardNextButtonClickEventHandler(ConveryWizardControl sender, WizardNextButtonClickEventArgs args);
}
