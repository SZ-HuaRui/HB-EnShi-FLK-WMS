using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.LPS.Common
{
    public static class WmsLock
    {
        public static readonly object GroupLock = new object();

        public static readonly object ComposeLock = new object();

        public static readonly object InboundLock = new object();
    }
}
