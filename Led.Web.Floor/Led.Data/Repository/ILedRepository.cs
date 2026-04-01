using Led.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Data.Repository
{
    public interface ILedRepository:IRepository<ItemProduction>
    {
        List<ItemProduction> GetTodayInBoundInfo(DateTime? startTime = null, DateTime? endTime = null);
        ClassProduction GetLastClassQty(DateTime? startTime = null, DateTime? endTime = null);
    }
}
