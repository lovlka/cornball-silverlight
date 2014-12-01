using System.Collections.Generic;
using System.Web.Http;
using Cornball.Web.Services;

namespace Cornball.Web.Controllers
{
    public class StatisticsController : ApiController
    {
        public IEnumerable<DataItem> Get()
        {
            var statistics = new Statistics();
            return statistics.GetStatistics();
        }

        public void Post(DataItem dataItem)
        {
            var statistics = new Statistics();
            statistics.IncreaseValue(dataItem.Name);
        }
    }
}