using BuffetDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace BuffetDAL.Repos.ADO.ReposForEntities
{
    public class MenuFoodReserveRepositoryADO 
    {
        private readonly string _connectionString;
        public MenuFoodReserveRepositoryADO(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void Create(MenuFoodReserve menuFoodReserve)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"insert menufoodreserves(menufoodid, reserveid, amount) 
                                            values(@menufoodid, @reserveid, @amount)";
                    command.Parameters.AddWithValue("@menufoodid", menuFoodReserve.MenuFoodId);
                    command.Parameters.AddWithValue("@reserveid", menuFoodReserve.ReserveId);
                    command.Parameters.AddWithValue("@amount", menuFoodReserve.Amount);
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
                    command.CommandText = "delete menufoodreserves where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public MenuFoodReserve Read(int id)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, menufoodid, reserveid, amount from menufoodreserves where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var dataReader = command.ExecuteReader())
                    {
                        MenuFoodReserve menuFoodReserve = new MenuFoodReserve();
                        while (dataReader.Read())
                        {
                            menuFoodReserve.Id = dataReader.GetInt32("id");
                            menuFoodReserve.MenuFoodId = dataReader.GetInt32("menufoodid");
                            menuFoodReserve.ReserveId = dataReader.GetInt32("reserveid");
                            menuFoodReserve.Amount = dataReader.GetInt32("amount");
                        }
                        return menuFoodReserve;
                    }
                }
            }
        }

        public IEnumerable<MenuFoodReserve> ReadAllForReserveId(int reserveId)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, menufoodid, reserveid, amount from menufoodreserves where reserveid=@reserveid";
                    command.Parameters.AddWithValue("reserveid", reserveId);
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<MenuFoodReserve> menuFoodReserves = new List<MenuFoodReserve>();
                        while (dataReader.Read())
                        {
                            MenuFoodReserve menuFoodReserve = new MenuFoodReserve();
                            menuFoodReserve.Id = dataReader.GetInt32("id");
                            menuFoodReserve.MenuFoodId = dataReader.GetInt32("menufoodid");
                            menuFoodReserve.ReserveId = dataReader.GetInt32("reserveid");
                            menuFoodReserve.Amount = dataReader.GetInt32("amount");
                            menuFoodReserves.Add(menuFoodReserve);
                        }
                        return menuFoodReserves;
                    }
                }
            }
        }

        public IEnumerable<MenuFoodReserve> ReadAll()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, menufoodid, reserveid, amount from menufoodreserves";
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<MenuFoodReserve> menuFoodReserves = new List<MenuFoodReserve>();
                        while (dataReader.Read())
                        {
                            MenuFoodReserve menuFoodReserve = new MenuFoodReserve();
                            menuFoodReserve.Id = dataReader.GetInt32("id");
                            menuFoodReserve.MenuFoodId = dataReader.GetInt32("menufoodid");
                            menuFoodReserve.ReserveId = dataReader.GetInt32("reserveid");
                            menuFoodReserve.Amount = dataReader.GetInt32("amount");
                            menuFoodReserves.Add(menuFoodReserve);
                        }
                        return menuFoodReserves;
                    }
                }
            }
        }

        public void Update(MenuFoodReserve menuFoodReserve)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"update menufoodreserves set 
                                                menufoodid = @menufoodid,
                                                reserveid = @reserveid,
                                                amount = @amount
                                            where id = @id";
                    command.Parameters.AddWithValue("@id", menuFoodReserve.Id);
                    command.Parameters.AddWithValue("@menufoodid", menuFoodReserve.MenuFoodId);
                    command.Parameters.AddWithValue("@reserveid", menuFoodReserve.ReserveId);
                    command.Parameters.AddWithValue("@amount", menuFoodReserve.Amount);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
