using Microsoft.Data.SqlClient;
using BuffetDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BuffetDAL.Repos.ADO.ReposForEntities
{
    public class FoodRepositoryADO
    {
        private readonly string _connectionString;

        public FoodRepositoryADO(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void Create(Food food)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"insert foods(name, weight, description, categoryid) 
                                            values(@name, @weight, @description, @categoryid)";
                    command.Parameters.AddWithValue("@name", food.Name);
                    command.Parameters.AddWithValue("@weight", food.Weight);
                    command.Parameters.AddWithValue("@description", food.Description);
                    command.Parameters.AddWithValue("@categoryid", food.CategoryId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "delete foods where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Food Read(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, name, weight, description, categoryid from foods where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    using(var dataReader = command.ExecuteReader())
                    {
                        Food food = new Food();
                        while (dataReader.Read())
                        {
                            food.Id = dataReader.GetInt32("id");
                            food.Name = dataReader.GetString("name");
                            food.Weight = dataReader.GetDecimal("weight");
                            food.Description = dataReader.GetString("description");
                            food.CategoryId = dataReader.GetInt32("categoryid");
                        }
                        return food;
                    }
                }
            }
        }

        public Food Read(string name)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, name, weight, description, categoryid from foods where name = @name";
                    command.Parameters.AddWithValue("@name", name);
                    using (var dataReader = command.ExecuteReader())
                    {
                        Food food = new Food();
                        while (dataReader.Read())
                        {
                            food.Id = dataReader.GetInt32("id");
                            food.Name = dataReader.GetString("name");
                            food.Weight = dataReader.GetDecimal("weight");
                            food.Description = dataReader.GetString("description");
                            food.CategoryId = dataReader.GetInt32("categoryid");
                        }
                        return food;
                    }
                }
            }
        }

        public IEnumerable<Food> ReadAll()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, name, weight, description, categoryid from foods order by categoryid asc";
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<Food> foods = new List<Food>();
                        while (dataReader.Read())
                        {
                            Food food = new Food();
                            food.Id = dataReader.GetInt32("id");
                            food.Name = dataReader.GetString("name");
                            food.Weight = dataReader.GetDecimal("weight");
                            food.Description = dataReader.GetString("description");
                            food.CategoryId = dataReader.GetInt32("categoryid");
                            foods.Add(food);
                        }
                        return foods;
                    }
                }
            }
        }

        public void Update(Food food)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"update foods set 
                                            name = @name,
                                            weight = @weight,
                                            description = @description,
                                            categoryid = @categoryid 
                                            where id = @id";
                    command.Parameters.AddWithValue("@id", food.Id);
                    command.Parameters.AddWithValue("@name", food.Name);
                    command.Parameters.AddWithValue("@weight", food.Weight);
                    command.Parameters.AddWithValue("@description", food.Description);
                    command.Parameters.AddWithValue("@categoryid", food.CategoryId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
