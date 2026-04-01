using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface IDock
    {
        DataRow GetByZone(string zoneID);

        DataRow[] GetDockByType(string type);

        DataRow GetDock(string id);
    }
}
