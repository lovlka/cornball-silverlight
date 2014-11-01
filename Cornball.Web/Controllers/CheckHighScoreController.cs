using System;
using System.Collections.Generic;
using System.Web.Http;
using Cornball.Web.Services;

namespace Cornball.Web.Controllers
{
    public class CheckHighScoreController : ApiController
    {
        public bool Get(int score, int limit, int days)
        {
            var statistics = new Statistics();
            return statistics.IsHighscore(score, limit, days);
        }
    }
}