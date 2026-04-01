using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Equipment.Led.Yb
{
    public interface IYbLed:ILed
    {
        //得到控制器类型
        int GetControllerType(string model);
    }
}
