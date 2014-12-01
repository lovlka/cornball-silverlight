using System;
using System.Web.Http;
using Cornball.Web.Services;

namespace Cornball.Web.Controllers
{
    public class HighScoreController : ApiController
    {
        public object Get(int limit, int score)
        {
            var statistics = new Statistics();
            return new { isHighscore = statistics.IsHighscore(score, limit, DateTime.Today.Day) };
        }

        public void Post(DataItem dataItem)
        {
            var statistics = new Statistics();
            statistics.SaveHighscore(dataItem.Name, dataItem.Value);
        }
    }
}