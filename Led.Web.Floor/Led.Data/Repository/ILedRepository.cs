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
        List<ItemProduction> GetTodayInBoundInfo();
        ClassProduction GetLastClassQty();
    }
}
