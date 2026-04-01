using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CWCS.UIDesigner.WizardBase
{
    internal class WizardCollectionEnumConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            WizardStepCollection steps = (WizardStepCollection)value;
            return steps[steps.Owner.CurrentStepIndex].ToString();
        }
    }
}
