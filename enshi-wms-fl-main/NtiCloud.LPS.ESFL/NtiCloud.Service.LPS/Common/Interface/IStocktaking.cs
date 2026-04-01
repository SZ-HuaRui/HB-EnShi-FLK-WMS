using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface IStocktaking
    {
        void CreateStocktakingBill(string mode, string target,
            string priority, string sourceBillNo, string memo, string locationIds);

        void TakeStock(string detailID, string palletCode, string reason, decimal qty);

        IStocktaking UpdateDetailMemo(string detailID, string memo);

        IStocktaking UpdateDetailPalletCode(string detailID, string palletCode);

        IStocktaking UpdateDetailQty(string detailID, decimal qty, decimal? oldQty = null);
    }
}
