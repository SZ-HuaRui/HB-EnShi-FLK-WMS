using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace CWCS.Common.Converter
{
    public sealed class StringResource
    {
        private ResourceManager manager;

        private CultureInfo cultureInfo;

        internal StringResource()
        {
        }

        public string GetString(string name, params object[] args)
        {
            string @string = this.manager.GetString(name, this.cultureInfo);
            Guard.NullReference(@string, null);
            if (args != null && args.Length > 0)
            {
                return string.Format(@string, args);
            }
            return @string;
        }

        public static StringResource Create(string resourceName, Assembly assembly = null, CultureInfo cultureInfo = null)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetCallingAssembly();
            }
            return new StringResource
            {
                manager = new ResourceManager(assembly.GetName().Name + "." + resourceName, assembly),
                cultureInfo = (cultureInfo ?? CultureInfo.CurrentCulture)
            };
        }
    }
}
