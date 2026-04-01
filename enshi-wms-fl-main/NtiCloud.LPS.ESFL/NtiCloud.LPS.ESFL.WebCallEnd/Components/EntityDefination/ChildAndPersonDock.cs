using System;
using System.Collections.Generic;
using System.Text;

namespace WMSInterface.EntityDefination
{

    [Serializable()]
    public class ChildAndPersonDock
    {
        public bool IsPersonDock = false;         //是父站台
        public bool HavePersonDock = false;     //存在从属站台
        public bool ChildDockUsing = false;       //子站台在用
        public bool PersonDockUsing = false;    //父站台在用

        public bool ChildAndPersonUsing = false;  //子、父站台同时被使用或未使用

        public string ChildDockNumber;
        public string PersonDockNumber;
    }
}
