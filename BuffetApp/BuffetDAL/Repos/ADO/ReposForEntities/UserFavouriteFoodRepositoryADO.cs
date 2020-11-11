using BuffetDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace BuffetDAL.Repos.ADO.ReposForEntities
{
    public class UserFavouriteFoodRepositoryADO
    {
        private readonly string _connectionString;
        public UserFavouriteFoodRepositoryADO(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void Create(UserFavouriteFood userFavouriteFood)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "insert userfavouritefoods(foodid, userid) values(@foodid, @userid)";
                    command.Parameters.AddWithValue("@foodid", userFavouriteFood.FoodId);
                    command.Parameters.AddWithValue("@userid", userFavouriteFood.UserId);
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
                    command.CommandText = "delete userfavouritefoods where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public UserFavouriteFood Read(int id)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, foodid, userid from userfavouritefoods where id=@id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var dataReader = command.ExecuteReader())
                    {
                        UserFavouriteFood userFavouriteFood = new UserFavouriteFood();
                        while (dataReader.Read())
                        {
                            userFavouriteFood.Id = dataReader.GetInt32("id");
                            userFavouriteFood.FoodId = dataReader.GetInt32("foodid");
                            userFavouriteFood.UserId = dataReader.GetInt32("userid");
                        }
                        return userFavouriteFood;
                    }
                }
            }
        }

        public IEnumerable<UserFavouriteFood> ReadAll()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, foodid, userid from userfavouritefoods";
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<UserFavouriteFood> userFavouriteFoods = new List<UserFavouriteFood>();
                        while (dataReader.Read())
                        {
                            UserFavouriteFood userFavouriteFood = new UserFavouriteFood();
                            userFavouriteFood.Id = dataReader.GetInt32("id");
                            userFavouriteFood.FoodId = dataReader.GetInt32("foodid");
                            userFavouriteFood.UserId = dataReader.GetInt32("userid");
                            userFavouriteFoods.Add(userFavouriteFood);
                        }
                        return userFavouriteFoods;
                    }
                }
            }
        }

        public void Update(UserFavouriteFood userFavouriteFood)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "update userfavouritefoods set foodid = @foodid, userid = @userid where id = @id";
                    command.Parameters.AddWithValue("@id", userFavouriteFood.Id);
                    command.Parameters.AddWithValue("@foodid", userFavouriteFood.FoodId);
                    command.Parameters.AddWithValue("@userid", userFavouriteFood.UserId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
