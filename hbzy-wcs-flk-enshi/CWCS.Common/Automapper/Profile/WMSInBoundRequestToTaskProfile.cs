using AutoMapper;
using CWCS.Entity;
using CWCS.Interface.Response.WMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Common.Automapper
{
    public class WMSInBoundResponseToTaskProfile: Profile
    {
        public WMSInBoundResponseToTaskProfile()
        {
            CreateMap<InBoundResponseData, WCS_RECEIVE_TASK>()
                .ForMember(dest => dest.WAREHOUSEID, opt => opt.MapFrom(src => src.WarehouseId))
                .ForMember(dest => dest.ZONEID, opt => opt.MapFrom(src => src.ZoneId))
                .ForMember(dest => dest.TASKNUM, opt => opt.MapFrom(src => src.TaskNum))
                .ForMember(dest => dest.TASKSTATUS, opt => opt.MapFrom(src => src.TaskStatus))
                .ForMember(dest => dest.TASKTYPE, opt => opt.MapFrom(src => src.TaskType))
                .ForMember(dest => dest.CONTAINERBARCODE, opt => opt.MapFrom(src => src.ContainerBarcode))
                .ForMember(dest => dest.WAVE_ID, opt => opt.MapFrom(src => src.Wave_ID))
                .ForMember(dest => dest.GROUPNUM, opt => opt.MapFrom(src => src.GroupNum))
                .ForMember(dest => dest.SEQNUM, opt => opt.MapFrom(src => src.SeqNum))
                .ForMember(dest => dest.SOURCEADDRESS, opt => opt.MapFrom(src => src.SourceAddress))
                .ForMember(dest => dest.TARGETADDRESS, opt => opt.MapFrom(src => src.TargetAddress))
                .ForMember(dest => dest.EQUIPMENTNUM, opt => opt.MapFrom(src => src.EquipmentNum))
                .ForMember(dest => dest.LEDTEXT, opt => opt.MapFrom(src => src.LedText));
        }
    }
}
