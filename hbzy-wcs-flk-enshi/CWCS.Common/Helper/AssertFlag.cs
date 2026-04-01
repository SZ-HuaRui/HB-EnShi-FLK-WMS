using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Common
{
    public sealed class AssertFlag
    {
        private bool flag;

        private readonly bool flag1;

        public AssertFlag()
        {
            this.flag = (this.flag1 = true);
        }

        public bool AssertTrue()
        {
            if (this.flag == this.flag1)
            {
                this.flag = !this.flag;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            this.flag = this.flag1;
        }
    }
}
