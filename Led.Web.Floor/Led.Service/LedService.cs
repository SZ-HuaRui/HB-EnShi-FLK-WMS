using Led.Data.Repository;
using Led.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Service
{

    public interface ILedService
    {
        List<ItemProduction> GetTodayInBoundInfo(DateTime? startTime = null, DateTime? endTime = null);
        ClassProduction GetLastClassQty(DateTime? startTime = null, DateTime? endTime = null);

    }


    public class LedService: ILedService
    {
        private ILedRepository respository;
        public LedService(ILedRepository respository)
        {
            this.respository = respository;
        }

        public ClassProduction GetLastClassQty(DateTime? startTime = null, DateTime? endTime = null)
        {
            return respository.GetLastClassQty(startTime, endTime);
        }

        public List<ItemProduction> GetTodayInBoundInfo(DateTime? startTime = null, DateTime? endTime = null)
        {
           return respository.GetTodayInBoundInfo(startTime, endTime);
        }
    }
}
