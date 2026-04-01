using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Common.Automapper
{
    public class AutoMapperConfiguration
    {
        public static IMapper Initializer()
        {
            MapperConfiguration mapperConfig = new MapperConfiguration(x =>
                                               x.AddProfile(new WMSInBoundResponseToTaskProfile())
                                               );

            return mapperConfig.CreateMapper();
        }
    }
}
