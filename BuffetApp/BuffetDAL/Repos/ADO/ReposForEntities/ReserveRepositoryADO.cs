using BuffetDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;

namespace BuffetDAL.Repos.ADO.ReposForEntities
{
    public class ReserveRepositoryADO
    {
        private readonly string _connectionString;
        public ReserveRepositoryADO(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void Create(Reserve reserve)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "insert reserves(userid, datetime, isaccepted) values(@userid, @datetime, @isaccepted)";
                    command.Parameters.AddWithValue("@userid", reserve.UserId);
                    command.Parameters.AddWithValue("@datetime", reserve.DateTime);
                    command.Parameters.AddWithValue("@isaccepted", reserve.IsAccepted);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "delete reserves where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }



        public Reserve Read(int id)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, userid, datetime, isaccepted from reserves where id=@id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var dataReader = command.ExecuteReader())
                    {
                        Reserve reserve = new Reserve();
                        while (dataReader.Read())
                        {
                            reserve.Id = dataReader.GetInt32("id");
                            reserve.UserId = dataReader.GetInt32("userid");
                            reserve.DateTime = dataReader.GetDateTime("datetime");
                            reserve.IsAccepted = dataReader.IsDBNull("isaccepted") ? null : dataReader.GetBoolean("isaccepted") as bool?;
                        }
                        return reserve;
                    }
                }
            }
        }

        public IEnumerable<Reserve> ReadAll()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, userid, datetime, isaccepted from reserves";
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<Reserve> reserves = new List<Reserve>();
                        while (dataReader.Read())
                        {
                            Reserve reserve = new Reserve();
                            reserve.Id = dataReader.GetInt32("id");
                            reserve.UserId = dataReader.GetInt32("userid");
                            reserve.DateTime = dataReader.GetDateTime("datetime");
                            reserve.IsAccepted = dataReader.IsDBNull("isaccepted") ? null : dataReader.GetBoolean("isaccepted") as bool?;
                            reserves.Add(reserve);
                        }
                        return reserves;
                    }
                }
            }
        }

        public IEnumerable<Reserve> ReadAllNull()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"select 
                                                id,
                                                userid,
                                                datetime,
                                                isaccepted from reserves 
                                            where isaccepted is null";
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<Reserve> reserves = new List<Reserve>();
                        while (dataReader.Read())
                        {
                            Reserve reserve = new Reserve();
                            reserve.Id = dataReader.GetInt32("id");
                            reserve.UserId = dataReader.GetInt32("userid");
                            reserve.DateTime = dataReader.GetDateTime("datetime"); //DateTime.ParseExact(dataReader.GetString("datetime"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            reserve.IsAccepted = dataReader.IsDBNull("isaccepted") ? null : dataReader.GetBoolean("isaccepted") as bool?;
                            reserves.Add(reserve);
                        }
                        return reserves;
                    }
                }
            }
        }

        public void Update(Reserve reserve)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "update reserves set userid = @userid, datetime = @datetime, isaccepted = @isaccepted where id = @id";
                    command.Parameters.AddWithValue("@userid", reserve.UserId);
                    command.Parameters.AddWithValue("@datetime", reserve.DateTime);
                    command.Parameters.AddWithValue("@isaccepted", reserve.IsAccepted);
                    command.Parameters.AddWithValue("@id", reserve.Id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
