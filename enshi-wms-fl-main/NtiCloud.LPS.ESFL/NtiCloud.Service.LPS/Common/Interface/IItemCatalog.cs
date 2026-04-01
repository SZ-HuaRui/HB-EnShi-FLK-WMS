using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Common.Interface
{
    public interface IItemCatalog
    {
        DataRow Get(string id);
        DataRow GetParent(string id);
    }
}
