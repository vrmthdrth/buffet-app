using Microsoft.Data.SqlClient;
using BuffetDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace BuffetDAL.Repos.ADO.ReposForEntities
{
    public class MenuFoodRepositoryADO
    {
        private readonly string _connectionString;
        public MenuFoodRepositoryADO(string connectionString)
        {
            this._connectionString = connectionString;
        }
        
        public void Create(MenuFood menuFood)
        {
             
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"insert menufoods(foodid, menuid, price, baseamount, availableamount, insufficientamount) 
                                          values(@foodid, @menuid, @price, @baseamount, @availableamount, @insufficientamount)";
                    command.Parameters.AddWithValue("@foodid", menuFood.FoodId);
                    command.Parameters.AddWithValue("@menuid", menuFood.MenuId);
                    command.Parameters.AddWithValue("@price", menuFood.Price);
                    command.Parameters.AddWithValue("@baseamount", menuFood.BaseAmount);
                    command.Parameters.AddWithValue("@availableamount", menuFood.AvailableAmount);
                    command.Parameters.AddWithValue("@insufficientamount", menuFood.InsufficientAmount);
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
                    command.CommandText = "delete menufoods where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public MenuFood Read(int id)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, foodid, menuid, price, baseamount, availableamount, insufficientamount from menufoods where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var dataReader = command.ExecuteReader())
                    {
                        MenuFood menuFood = new MenuFood();
                        while (dataReader.Read())
                        {
                            menuFood.Id = dataReader.GetInt32("id");
                            menuFood.FoodId = dataReader.GetInt32("foodid");
                            menuFood.MenuId = dataReader.GetInt32("menuid");
                            menuFood.Price = dataReader.GetDecimal("price");
                            menuFood.BaseAmount = dataReader.GetInt32("baseamount");
                            menuFood.AvailableAmount = dataReader.GetInt32("availableamount");
                            menuFood.InsufficientAmount = dataReader.GetInt32("insufficientamount");
                        }
                        return menuFood;
                    }
                }
            }
        }

        public IEnumerable<MenuFood> ReadAll()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, foodid, menuid, price, baseamount, availableamount, insufficientamount from menufoods";
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<MenuFood> menuFoods = new List<MenuFood>();
                        while (dataReader.Read())
                        {
                            MenuFood menuFood = new MenuFood();
                            menuFood.Id = dataReader.GetInt32("id");
                            menuFood.FoodId = dataReader.GetInt32("foodid");
                            menuFood.MenuId = dataReader.GetInt32("menuid");
                            menuFood.Price = dataReader.GetDecimal("price");
                            menuFood.BaseAmount = dataReader.GetInt32("baseamount");
                            menuFood.AvailableAmount = dataReader.GetInt32("availableamount");
                            menuFood.InsufficientAmount = dataReader.GetInt32("insufficientamount");
                            menuFoods.Add(menuFood);
                        }
                        return menuFoods;
                    }
                }
            }
        }

        public void Update(MenuFood menuFood)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"update menufoods set 
                        foodid = @foodid,
                        menuid = @menuid,
                        price = @price,
                        baseamount = @baseamount,
                        availableamount = @availableamount,
                        insufficientamount = @insufficientamount 
                         where id = @id";
                    command.Parameters.AddWithValue("@id", menuFood.Id);
                    command.Parameters.AddWithValue("@foodid", menuFood.FoodId);
                    command.Parameters.AddWithValue("@menuid", menuFood.MenuId);
                    command.Parameters.AddWithValue("@price", menuFood.Price);
                    command.Parameters.AddWithValue("@baseamount", menuFood.BaseAmount);
                    command.Parameters.AddWithValue("@availableamount", menuFood.AvailableAmount);
                    command.Parameters.AddWithValue("@insufficientamount", menuFood.InsufficientAmount);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
