using BuffetDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace BuffetDAL.Repos.ADO.ReposForEntities
{
    public class UserRepositoryADO 
    {
        private readonly string _connectionString;
        public UserRepositoryADO(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void Create(User user)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"insert users(name, surname, email, roleid) 
                                            values(@name, @surname, @email, @roleid)";
                    command.Parameters.AddWithValue("@name", user.Name); 
                    command.Parameters.AddWithValue("@surname", user.Surname);
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@roleid", user.RoleId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public User Read(int userId)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, name, surname, email, roleid from users where id = @userId";
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var dataReader = command.ExecuteReader())
                    {
                        User user = new User();
                        while (dataReader.Read())
                        {
                            user.Id = dataReader.GetInt32("id");
                            user.Name = dataReader.GetString("name");
                            user.Surname = dataReader.GetString("surname");
                            user.Email = dataReader.GetString("email");
                            user.RoleId = dataReader.GetInt32("roleid");
                        }
                        return user.Email.Equals(null) ? null : user;
                    }
                }
            }
        }

        public User Read(string userEmail)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, name, surname, email, roleid from users where email = @userEmail";
                    command.Parameters.AddWithValue("@userEmail", userEmail);
                    using (var dataReader = command.ExecuteReader())
                    {
                        User user = new User();
                        while (dataReader.Read())
                        {
                            user.Id = dataReader.GetInt32("id");
                            user.Name = dataReader.GetString("name");
                            user.Surname = dataReader.GetString("surname");
                            user.Email = dataReader.GetString("email");
                            user.RoleId = dataReader.GetInt32("roleid");
                        }
                        return user.Email == null ? null : user;
                    }
                }
            }
        }

        public IEnumerable<User> ReadAll()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, name, surname, email, roleid from users";
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<User> users = new List<User>();
                        while (dataReader.Read())
                        {
                            User user = new User();   
                            user.Id = dataReader.GetInt32("id"); 
                            user.Name = dataReader.GetString("name");
                            user.Surname = dataReader.GetString("surname");
                            user.Email = dataReader.GetString("email");
                            user.RoleId = dataReader.GetInt32("roleid");
                            users.Add(user);
                        }
                        return users;
                    }
                }
            }
        }

        public void Update(User user)
        {
            using(SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"update users set 
                                                name = @name,
                                                surname = @surname,
                                                email = @email,
                                                roleid = @roleid 
                                            where id = @id";
                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@name", user.Name); 
                    command.Parameters.AddWithValue("@surname", user.Surname);
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@roleid", user.RoleId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int userId)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "delete users where id = @userId";
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(string userEmail)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "delete users where email = @userEmail";
                    command.Parameters.AddWithValue("@userEmail", userEmail);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
