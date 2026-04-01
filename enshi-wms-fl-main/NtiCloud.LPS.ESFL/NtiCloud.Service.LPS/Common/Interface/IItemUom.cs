using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Common.Interface
{
    public interface IItemUom
    {
        UomModel ConvertFromDefaultStatUom(decimal qty, string destUomID, decimal spec = 1m);
        UomModel ConvertFromDefaultPackUom(decimal qty, string destUomID, decimal spec = 1m);
        UomModel ConvertToDefaultStatUom(UomModel srcQty, decimal? spec = null);
        UomModel ConvertToDefaultStatUom(string uomID, decimal qty, decimal spec = 1m);
        UomModel ConvertToDefaultPackUom(UomModel srcQty, decimal spec = 1m);
    }
}
