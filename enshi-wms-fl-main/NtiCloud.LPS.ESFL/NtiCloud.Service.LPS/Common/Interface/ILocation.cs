using NtiCloud.Service.WMS.Base;
using NtiCloud.Service.WMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface ILocation
    {
        DataRow Get(string locationIdOrNo);

        DataRow GetByContainer(string containerBarcodeOrID);

        DataRow GetEmptyLocation();

        DataRow[] GetLocationGroup();

        DataRow GetConveyorLocation();

        DataRow GetPickingLocation();

        bool IsCompactLocation(DataRow row);

        bool IsRackLocation(DataRow row);

        ILocation ModifyLocationStockState(string locationID, 
            string stockState, string previousState = null, IDbTransaction tran = null);

        ILocation RestoreLocationStockState(List<string> idList, out List<string> failureList,
            Dictionary<string, object> dict);

        ILocation SetContainer(string locationID, string containerID);

        ILocation SetItem(string locationID, string itemID);

        void UpdateStatus(
            string locationID, string stockState, string previousState = null);
        DataRow GetByAddress(Address relay);

        public  LocationModel GetLocationById(string id);
    }
}
