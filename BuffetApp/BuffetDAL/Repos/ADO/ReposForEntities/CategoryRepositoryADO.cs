using Microsoft.Data.SqlClient;
using BuffetDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BuffetDAL.Repos.ADO.ReposForEntities
{
    public class CategoryRepositoryADO
    {
        private readonly string _connectionString;
        public CategoryRepositoryADO(string connectionString)
        {
            this._connectionString = connectionString; 
        }

        public void Create(Category category)
        {
            using(SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "insert categories(name) values(@name)";
                    command.Parameters.AddWithValue("@name", category.Name);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Category Read(int id)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, name from categories where id=@id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var dataReader = command.ExecuteReader())
                    {
                        Category category = new Category();
                        while (dataReader.Read())
                        {
                            category.Id = dataReader.GetInt32("id");            
                            category.Name = dataReader.GetString("name");
                        }
                        return category;
                    }
                }
            }
        }

        public Category Read(string name)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, name from categories where name=@name";
                    command.Parameters.AddWithValue("@name", name);
                    using (var dataReader = command.ExecuteReader())
                    {
                        Category category = new Category();
                        while (dataReader.Read())
                        {
                            category.Id = dataReader.GetInt32("id");
                            category.Name = dataReader.GetString("name");
                        }
                        return category;
                    }
                }
            }
        }

        public IEnumerable<Category> ReadAll()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, name from categories";
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<Category> categories = new List<Category>();
                        while (dataReader.Read())
                        {
                            Category category = new Category();
                            category.Id = dataReader.GetInt32("id");
                            category.Name = dataReader.GetString("name");
                            categories.Add(category);
                        }
                        return categories;
                    }
                }
            }
        }

        public void Update(Category category)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.CommandText = "update categories set name = @name where id = @id";
                    command.Parameters.AddWithValue("@id", category.Id);
                    command.Parameters.AddWithValue("@name", category.Name);
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
                    command.CommandText = "delete categories where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
