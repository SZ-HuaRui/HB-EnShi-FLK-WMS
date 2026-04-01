using System;
using System.Collections.Generic;
using System.Text;
using NtiCloud.Service.WMS.Common.Interface;
using NtiCloud.Service;

namespace NtiCloud.Service.WMS.Common
{
    public abstract class WmsObject
    {
        private IWorkTask workTask = null;
        protected IWorkTask WorkTask
        {
            get
            {
                if (workTask != null)
                {
                    return workTask;
                }
                //workTask = WmsFactory.Make<WorkTask>();
                workTask = WmsFactory.Make<WMS.Tasking.WorkTask>();
                return workTask;
            }
        }

        private IWorkTaskInterface workTaskInterface = null;
        protected IWorkTaskInterface WorkTaskInterface
        {
            get
            {
                if (workTaskInterface != null)
                {
                    return workTaskInterface;
                }
                //workTaskInterface = WmsFactory.Make<IWorkTaskInterface>();
                workTaskInterface = WmsFactory.Make<WMS.Interface.WCS.WorkTaskInterface>();
                return workTaskInterface;
            }
        }

        private IWorkTaskItem workTaskItem = null;
        protected IWorkTaskItem WorkTaskItem
        {
            get
            {
                if (workTaskItem != null)
                {
                    return workTaskItem;
                }
                //workTask = WmsFactory.Make<WorkTask>();
                workTaskItem = WmsFactory.Make<WMS.Tasking.WorkTaskItem>();
                return workTaskItem;
            }
        }

        private IWave wave = null;
        protected IWave Wave
        {
            get
            {
                if (wave != null)
                {
                    return wave;
                }
                //wave = WmsFactory.Make<IWave>();
                wave = WmsFactory.Make<WMS.Outbound.Wave>();
                return wave;
            }
        }

        private IPickingZone pickingZone = null;
        protected IPickingZone PickingZone
        {
            get
            {
                if (pickingZone != null)
                {
                    return pickingZone;
                }
                //pickingZone = WmsFactory.Make<IPickingZone>();
                pickingZone = null;
                return pickingZone;
            }
        }

        private ILocationAllocator locationAllocator = null;
        protected ILocationAllocator LocationAllocator
        {
            get
            {
                if (locationAllocator != null)
                {
                    return locationAllocator;
                }
                //locationAllocator = WmsFactory.Make<ILocationAllocator>();
                locationAllocator = WmsFactory.Make<WMS.Base.LocationAllocator>();
                return locationAllocator;
            }
        }

        private IOutboundOrder outboundOrder = null;
        protected IOutboundOrder OutboundOrder
        {
            get
            {
                if (outboundOrder != null)
                {
                    return outboundOrder;
                }
                //outboundOrder = WmsFactory.Make<IOutboundOrder>();
                outboundOrder = WmsFactory.Make<WMS.Outbound.OutboundOrder>();
                return outboundOrder;
            }
        }

        /*
        private IReceipt receipt = null;
        protected IReceipt Receipt
        {
            get
            {
                if (receipt != null)
                {
                    return receipt;
                }
                //receipt = WmsFactory.Make<IReceipt>();
                receipt = WmsFactory.Make<WMS.Receiving.Receipt>();
                return receipt;
            }
        }
        */

        private IContainer container = null;
        protected IContainer Container
        {
            get
            {
                if (container != null)
                {
                    return container;
                }
                //container = WmsFactory.Make<IContainer>();
                container = WmsFactory.Make<WMS.Inventory.Container>();
                return container;
            }
        }

        private ILocation location = null;
        protected ILocation Location
        {
            get
            {
                if (location != null)
                {
                    return location;
                }
                //location = WmsFactory.Make<ILocation>();
                location = WmsFactory.Make<WMS.Base.Location>();
                return location;
            }
        }

        private IItem item = null;
        protected IItem Item
        {
            get
            {
                if (item != null)
                {
                    return item;
                }
                //item = WmsFactory.Make<IItem>();
                item = WmsFactory.Make<WMS.Base.Item>();
                return item;
            }
        }

        private IItemCatalog itemCatalog = null;
        protected IItemCatalog ItemCatalog
        {
            get
            {
                if (itemCatalog != null)
                {
                    return itemCatalog;
                }
                //item = WmsFactory.Make<IItem>();
                itemCatalog = WmsFactory.Make<WMS.Base.ItemCatalog>();
                return itemCatalog;
            }
        }

        private IItemUom itemUom = null;
        protected IItemUom ItemUom
        {
            get
            {
                if (itemUom != null)
                {
                    return itemUom;
                }
                //item = WmsFactory.Make<IItem>();
                itemUom = WmsFactory.Make<WMS.Base.ItemUom>();
                return itemUom;
            }
        }

        private IEquipment equipment = null;
        protected IEquipment Equipment
        {
            get
            {
                if (equipment != null)
                {
                    return equipment;
                }
                //equipment = WmsFactory.Make<IEquipment>();
                equipment = WmsFactory.Make<WMS.Base.Equipment>();
                return equipment;
            }
        }

        private IDock dock = null;
        protected IDock Dock
        {
            get
            {
                if (dock != null)
                {
                    return dock;
                }
                //dock = WmsFactory.Make<IDock>();
                dock = WmsFactory.Make<WMS.Base.Dock>();
                return dock;
            }
        }

        private IBom bom = null;
        protected IBom Bom
        {
            get
            {
                if (bom != null)
                {
                    return bom;
                }
                //dock = WmsFactory.Make<IDock>();
                bom = WmsFactory.Make<WMS.Base.Bom>();
                return bom;
            }
        }

        /*
        private IInboundQC inboundQC = null;
        protected IInboundQC InboundQC
        {
            get
            {
                if (inboundQC != null)
                {
                    return inboundQC;
                }
                //inboundQC = WmsFactory.Make<IInboundQC>();
                inboundQC = WmsFactory.Make<WMS.QC.InboundQC>();
                return inboundQC;
            }
        }
        */

        private IOutboundQC outboundQC = null;
        protected IOutboundQC OutboundQC
        {
            get
            {
                if (outboundQC != null)
                {
                    return outboundQC;
                }
                //outboundQC = WmsFactory.Make<IOutboundQC>();
                outboundQC = WmsFactory.Make<WMS.QC.OutboundQC>();
                return outboundQC;
            }
        }
    }
}
