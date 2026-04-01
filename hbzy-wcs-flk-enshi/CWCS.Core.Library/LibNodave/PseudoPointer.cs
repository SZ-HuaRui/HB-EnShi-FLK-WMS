using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CWCS.Core.Library
{

    public class PseudoPointer : IDisposable
    {
        public IntPtr Pointer;

        [DllImport("libnodave.dll"/*, PreserveSig=false*/)]
        private static extern int daveFree(IntPtr p);
        protected static int Free(IntPtr p)
        {
            try
            {
                if (p == IntPtr.Zero) return 0;
                return (int)daveFree(p);
            }
            catch { return 0; }
        }

        public virtual void Dispose()
        {
            Free(Pointer);
        }
    }
}
