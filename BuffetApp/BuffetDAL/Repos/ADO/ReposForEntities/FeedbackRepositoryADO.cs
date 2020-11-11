using Microsoft.Data.SqlClient;
using BuffetDAL.Models;
using BuffetDAL.Repos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BuffetDAL.Repos.ADO.ReposForEntities
{
    public class FeedbackRepositoryADO
    {
        private readonly string _connectionString;

        public FeedbackRepositoryADO(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void Create(Feedback feedback)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "insert feedbacks(message, userid) values(@message, @userid)";
                    command.Parameters.AddWithValue("@message", feedback.Message);
                    command.Parameters.AddWithValue("@userid", feedback.UserId);
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
                    command.CommandText = "delete feedbacks where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Feedback Read(int id)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, message, userid from feedbacks where id=@id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var dataReader = command.ExecuteReader())
                    {
                        Feedback feedback = new Feedback();
                        while (dataReader.Read())
                        {
                            feedback.Id = dataReader.GetInt32("id");
                            feedback.Message = dataReader.GetString("message");
                            feedback.UserId = dataReader.GetInt32("userid");
                        }
                        return feedback;
                    }
                }
            }
        }

        public IEnumerable<Feedback> ReadAll()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, message, userid from feedbacks order by id desc";
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<Feedback> feedbacks = new List<Feedback>();
                        while (dataReader.Read())
                        {
                            Feedback feedback = new Feedback();
                            feedback.Id = dataReader.GetInt32("id");
                            feedback.Message = dataReader.GetString("message");
                            feedback.UserId = dataReader["userid"] as int?; //(int) bad
                            feedbacks.Add(feedback);
                        }
                        return feedbacks;
                    }
                }
            }
        }
    }
}
