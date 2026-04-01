using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface IItem
    {
        DataRow Get(string itemCodeOrID);
        DataRow[] GetBy(string itemName);
        DataRow GetItemCatalog(string catalogID);
        DataRow GetSupplier(string itemID, string supplierID);
    }
}
