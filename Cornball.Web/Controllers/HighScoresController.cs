using System;
using System.Collections.Generic;
using System.Web.Http;
using Cornball.Web.Services;

namespace Cornball.Web.Controllers
{
    public class HighScoresController : ApiController
    {
        public IEnumerable<DataItem> Get(int limit, DateTime? startDate = null, DateTime? endDate = null)
        {
            var statistics = new Statistics();
            return statistics.GetHighscores(limit, startDate, endDate);
        }
    }
}