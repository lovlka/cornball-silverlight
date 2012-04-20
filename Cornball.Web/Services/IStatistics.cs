using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Cornball.Web.Services
{
    [ServiceContract(Name = "Statistics", Namespace = "http://lantisen.stodell.se/")]
    public interface IStatistics
    {
        [OperationContract]
        void IncreaseValue(string name);

        [OperationContract]
        List<DataItem> GetStatistics();

        [OperationContract]
        void SaveHighscore(string name, int score);

        [OperationContract]
        List<DataItem> GetHighscores(int limit, DateTime? startDate, DateTime? endDate);

        [OperationContract]
        bool IsHighscore(int score, int limit, int days);
    }
}