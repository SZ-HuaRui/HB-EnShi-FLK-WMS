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
        List<ItemProduction> GetTodayInBoundInfo();
        ClassProduction GetLastClassQty();

    }


    public class LedService: ILedService
    {
        private ILedRepository respository;
        public LedService(ILedRepository respository)
        {
            this.respository = respository;
        }

        public ClassProduction GetLastClassQty()
        {
            return respository.GetLastClassQty();
        }

        public List<ItemProduction> GetTodayInBoundInfo()
        {
           return respository.GetTodayInBoundInfo();
        }
    }
}
