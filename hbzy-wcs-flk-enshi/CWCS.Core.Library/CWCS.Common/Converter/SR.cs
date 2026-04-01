using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Common.Converter
{
    internal class SR
    {
        private static StringResource m_resource;

        static SR()
        {
            if (SR.m_resource == null)
            {
                SR.m_resource = StringResource.Create("Strings", typeof(SR).Assembly, null);
            }
        }

        internal static string GetString(string kind, params object[] args)
        {
            return SR.m_resource.GetString(kind, args);
        }
    }
}
