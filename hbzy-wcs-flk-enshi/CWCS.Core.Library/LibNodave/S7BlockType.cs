using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Core.Library
{
    public enum S7BlockType : int
    {
        OB = '8',
        DB = 'A',
        SDB = 'B',
        FC = 'C',
        SFC = 'D',
        FB = 'E',
        SFB = 'F'
    }
}
