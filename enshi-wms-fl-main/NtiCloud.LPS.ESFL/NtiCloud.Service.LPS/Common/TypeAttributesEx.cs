using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NtiCloud.Service.LPS.Common
{
    public static class TypeAttributesEx
    {
        public static NtiCloud.Kernel.Core.DescriptionAttribute GetAttributeDescription(this Type type, string attr)
        {
            MemberInfo[] members = type.GetMembers();

            MemberInfo memberItem = members.First(p => p.Name == attr);

            Object[] obj = memberItem.GetCustomAttributes(typeof(NtiCloud.Kernel.Core.DescriptionAttribute), true);

            NtiCloud.Kernel.Core.DescriptionAttribute descriptionAttributes = obj[0] as NtiCloud.Kernel.Core.DescriptionAttribute;

            return descriptionAttributes;
        }
    }
}
