using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace BuffetDAL.Repos.ADO
{
    public class SpecificReportsRepositoryADO
    {
        private string _connectionString;

        public SpecificReportsRepositoryADO(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public DataTable GetMostFavouriteFoods()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select f.Name, count(f.id) as Rating from foods as f " +
                        "join userfavouritefoods uff on f.id = uff.foodid " +
                        "join users u on uff.userid = u.id " +
                        "group by f.id, f.Name " +
                        "order by Rating desc";
                    DataTable resultData = new DataTable();
                    resultData.Load(command.ExecuteReader());
                    return resultData;
                }
            }
        }

        public DataTable GetDailyReservesRating()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select convert(date, r.datetime, 105) as Date, count(convert(date, r.datetime, 105)) as Orders " +
                        "from reserves r " +
                        "where r.IsAccepted = 1 " +
                        "group by convert(date, r.datetime, 105), r.isaccepted ";
                    DataTable resultData = new DataTable();
                    resultData.Load(command.ExecuteReader());
                    return resultData;
                }
            }
        }

        public DataTable GetMostPopularFoods()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select foods.Name, count(mf.foodid) as Bought " +
                        "from menufoods mf " +
                        "join menufoodreserves mfr on mf.id = mfr.menufoodid " +
                        "join reserves r on mfr.reserveid = r.id " +
                        "join foods on mf.foodid = foods.id " +
                        "where r.isaccepted = 1 " +
                        "group by mf.foodid, foods.Name " +
                        "order by Bought desc";
                    DataTable resultData = new DataTable();
                    resultData.Load(command.ExecuteReader());
                    return resultData;
                }
            }
        }

        public DataTable GetLackOfFoods()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select f.Name, count(mf.InsufficientAmount) as Lack " +
                        "from menufoods as mf " +
                        "join foods as f on mf.foodid = f.id " +
                        "where InsufficientAmount <> 0 " +
                        "group by f.Name, mf.foodid " +
                        "order by Lack desc";
                    DataTable resultData = new DataTable();
                    resultData.Load(command.ExecuteReader());
                    return resultData;
                }
            }
        }
    }
}
