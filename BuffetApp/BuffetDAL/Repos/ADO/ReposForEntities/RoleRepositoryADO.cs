using Microsoft.Data.SqlClient;
using BuffetDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BuffetDAL.Repos.ADO.ReposForEntities
{
    public class RoleRepositoryADO 
    {
        private readonly string _connectionString;
        public RoleRepositoryADO(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void Create(Role role)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "insert roles(name) values(@name)";
                    command.Parameters.AddWithValue("@name", role.Name);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Role Read(int id)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    Role role = new Role();
                    command.CommandText = "select id, name from roles where id=@id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            role.Id = dataReader.GetInt32("id");
                            role.Name = dataReader.GetString("name");
                        }
                        return role;
                    }
                }
            }
        }

        public Role Read(string roleName)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    Role role = new Role();
                    command.CommandText = "select id, name from roles where name=@roleName";
                    command.Parameters.AddWithValue("@roleName", roleName);
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            role.Id = dataReader.GetInt32("id");
                            role.Name = dataReader.GetString("name");
                        }
                        return role;
                    }
                }
            }
        }

        public IEnumerable<Role> ReadAll()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                List<Role> roles = new List<Role>();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, name from roles";
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            Role role = new Role();
                            role.Id = dataReader.GetInt32("id");
                            role.Name = dataReader.GetString("name");
                            roles.Add(role);
                        }
                        return roles;
                    }
                }
            } 
        }
    }
}
