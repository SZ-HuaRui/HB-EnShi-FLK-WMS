using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CWCS.UIDesigner.WizardBase
{
    internal class WizardStepCollectionConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            WizardStepCollection steps = (WizardStepCollection)value;
            if (steps.Count != 0)
            {
                if (steps.Count != 1)
                {
                    return (steps.Count + " steps");
                }
                return "1 step";
            }
            else
            {
                return "Empty";
            }
        }
    }
}
