using BuffetDAL.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace BuffetDAL.Repos.ADO.ReposForEntities
{
    public class MenuRepositoryADO 
    {
        private readonly string _connectionString;
        public MenuRepositoryADO(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void Create(Menu menu)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "insert menus(date) values(@date)";
                    command.Parameters.AddWithValue("@date", menu.Date);
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
                    command.CommandText = "delete menus where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Menu Read(int id)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, date from menus where id=@id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var dataReader = command.ExecuteReader())
                    {
                        Menu menu = new Menu();
                        while (dataReader.Read())
                        {
                            menu.Id = dataReader.GetInt32("id");
                            menu.Date = dataReader.GetDateTime("date");
                        }
                        return menu;
                    }
                }
            }
        }

        public Menu Read(DateTime date) //проверить работу
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, date from menus where date=@date";
                    command.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
                    using (var dataReader = command.ExecuteReader())
                    {
                        Menu menu = new Menu();
                        while (dataReader.Read())
                        {
                            menu.Id = dataReader.GetInt32("id");
                            menu.Date = dataReader.GetDateTime("date");
                        }
                        return menu;
                    }
                }
            }
        }

        public IEnumerable<Menu> ReadAll()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, date from menus order by date desc";
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<Menu> menus = new List<Menu>();
                        while (dataReader.Read())
                        {
                            Menu menu = new Menu();
                            menu.Id = dataReader.GetInt32("id");
                            menu.Date = dataReader.GetDateTime("date");
                            menus.Add(menu);
                        }
                        return menus;
                    }
                }
            }
        }

        public void Update(Menu menu)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.CommandText = "update menus set date = @date where id = @id";
                    command.Parameters.AddWithValue("@id", menu.Id);
                    command.Parameters.AddWithValue("@date", menu.Date);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
