using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Cornball.Web.Services
{
    public class Statistics : IStatistics
    {
        private const string Connection =
            "Server=tcp:dlndn6ivhf.database.windows.net,1433;Database=Cornball;User ID=victor@dlndn6ivhf;Password=Pazzv0rt04;Trusted_Connection=False;Encrypt=True;";

        #region IStatistics Members

        public void IncreaseValue(string name)
        {
            using (var connection = new SqlConnection(Connection))
            {
                using (var command = new SqlCommand("EXEC IncreaseValue @Name", connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<DataItem> GetStatistics()
        {
            var values = new List<DataItem>();
            using (var connection = new SqlConnection(Connection))
            {
                using (var command = new SqlCommand("EXEC GetStatistics", connection))
                {
                    command.Connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            values.Add(new DataItem(dataReader.GetString(0), dataReader.GetInt32(1)));
                        }
                    }
                }
            }
            return values;
        }

        public void SaveHighscore(string name, int score)
        {
            using (var connection = new SqlConnection(Connection))
            {
                using (var command = new SqlCommand("EXEC SaveHighscore @Name, @Score", connection))
                {
                    command.Parameters.AddWithValue("@Score", score);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<DataItem> GetHighscores(int limit, DateTime? startDate, DateTime? endDate)
        {
            var score = new List<DataItem>();
            using (var connection = new SqlConnection(Connection))
            {
                using (
                    var command = new SqlCommand("EXEC GetHighscores @Limit, @Days, @StartDate, @EndDate", connection))
                {
                    command.Parameters.AddWithValue("@Limit", limit);
                    command.Parameters.AddWithValue("@Days", 0);
                    command.Parameters.AddWithValue("@StartDate",
                                                    startDate.HasValue ? startDate.Value.Date : new DateTime(1970, 1, 1));
                    command.Parameters.AddWithValue("@EndDate",
                                                    endDate.HasValue ? endDate.Value.Date : new DateTime(1970, 1, 1));
                    command.Connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            score.Add(new DataItem(dataReader.GetString(0), dataReader.GetInt32(1)));
                        }
                    }
                }
            }
            return score;
        }

        public bool IsHighscore(int score, int limit, int days)
        {
            using (var connection = new SqlConnection(Connection))
            {
                using (var command = new SqlCommand("EXEC IsHighscore @Score, @Limit, @Days", connection))
                {
                    command.Parameters.AddWithValue("@Score", score);
                    command.Parameters.AddWithValue("@Limit", limit);
                    command.Parameters.AddWithValue("@Days", days);
                    command.Connection.Open();
                    return Convert.ToBoolean(command.ExecuteScalar());
                }
            }
        }

        #endregion

    }
}