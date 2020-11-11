using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BuffetAdminMVC.Models;
using BuffetDAL.AdditionalModels;
using BuffetDAL.Models;
using BuffetDAL.Repos.ADO.ReposForEntities;

namespace BuffetDAL.Repos.ADO
{
    public class AdminRepositoryADO
    {
        private readonly string _connectionString;

        public AdminRepositoryADO(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void CreateMenuForDay(Menu menu, IEnumerable<MenuFood> listOfMenuFoods)
        {
            MenuRepositoryADO menuRepo = new MenuRepositoryADO(this._connectionString);
            menuRepo.Create(menu);

            MenuFoodRepositoryADO menuFoodRepo = new MenuFoodRepositoryADO(this._connectionString);
            foreach(MenuFood mf in listOfMenuFoods)
            {
                menuFoodRepo.Create(mf);
            }
        }

        public DataTable ReadMenuForDay(int menuId)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"select 
                        f.name as FoodName,f.weight as Weight,f.description as Description,f.categoryid as CategoryId,
                        mf.price as Price,mf.baseamount as Base,mf.availableamount as Available,mf.insufficientamount as Insufficient 
                        from menus m 
                        join menufoods mf on mf.menuid = m.id 
                        join foods f on f.id = mf.foodid 
                        where m.id = @menuId
                        order by categoryid";
                    command.Parameters.AddWithValue("@menuId", menuId);
                    DataTable resultData = new DataTable();
                    resultData.Load(command.ExecuteReader());
                    return resultData;
                }
            }
        }

        public List<MenuUpdateModel> ReadMenuForDayAsList(int menuId)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"select 
                        f.name as FoodName,f.weight as Weight,f.description as Description,f.categoryid as CategoryId,
                        mf.id as MenuFoodId, mf.price as Price,mf.baseamount as Base,mf.availableamount as Available,mf.insufficientamount as Insufficient  
                        from menus m 
                        join menufoods mf on mf.menuid = m.id 
                        join foods f on f.id = mf.foodid 
                        where m.id = @menuId 
                        order by CategoryId asc"; //MenuFoodId
                    command.Parameters.AddWithValue("@menuId", menuId);

                    using (var dataReader = command.ExecuteReader())
                    {
                        List<MenuUpdateModel> menuUpdModels = new List<MenuUpdateModel>();
                        while (dataReader.Read())
                        {
                            MenuUpdateModel menuUpdateModel = new MenuUpdateModel();
                            menuUpdateModel.Id = dataReader.GetInt32("MenuFoodId");
                            menuUpdateModel.FoodName = dataReader.GetString("FoodName");
                            menuUpdateModel.Weight = dataReader.GetDecimal("Weight");
                            menuUpdateModel.Description = dataReader.GetString("Description");
                            menuUpdateModel.CategoryId = dataReader.GetInt32("CategoryId");
                            menuUpdateModel.Price = dataReader.GetDecimal("Price");
                            menuUpdateModel.Base = dataReader.GetInt32("Base"); 
                            menuUpdateModel.Available = dataReader.GetInt32("Available");
                            menuUpdateModel.Insufficient = dataReader.GetInt32("Insufficient");
                            menuUpdModels.Add(menuUpdateModel);
                        }
                        return menuUpdModels;
                    }
                }
            }
        }

        public void UpdateMenuInfo(int menuId, List<MenuUpdateModel> menuDetails)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                foreach(MenuUpdateModel model in menuDetails)
                {
                    int foodId = new FoodRepositoryADO(this._connectionString).Read(model.FoodName).Id; // ploho
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"update menufoods  
                                                set  
                                                    BaseAmount = @base,
                                                    AvailableAmount = @base 
                                                where menuid = @menuId and foodid = @foodId";
                        command.Parameters.AddWithValue("@menuId", menuId);
                        command.Parameters.AddWithValue("@foodId", foodId);
                        command.Parameters.AddWithValue("@base", model.Base);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public IEnumerable<User> GetUsersInRole(int roleId) 
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"select u.* from users u 
                                            join roles r on r.id = u.roleid 
                                            where u.roleid = @roleId";
                    command.Parameters.AddWithValue("@roleId", roleId);
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<User> users = new List<User>();
                        while (dataReader.Read())
                        {
                            User user = new User();
                            user.Id = dataReader.GetInt32("id");
                            user.Name = dataReader.GetString("name");
                            user.Surname = dataReader.GetString("surname");
                            //user.Password = dataReader.GetString("password");
                            user.Email = dataReader.GetString("email");
                            user.RoleId = dataReader.GetInt32("roleid");
                            users.Add(user);
                        }
                        return users;
                    }
                }
            }
        }

        public IEnumerable<Food> GetUserFavourites(string userEmail) //необяз // int userid
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"select f.id, f.name, f.weight, f.description, f.categoryid from foods f 
                                            join userfavouritefoods uff on f.Id = uff.Foodid 
                                            join users u on u.id = uff.userid 
                                            where u.email = @userEmail";
                    command.Parameters.AddWithValue("@userEmail", userEmail);
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<Food> foods = new List<Food>();
                        while (dataReader.Read())
                        {
                            Food food = new Food();
                            food.Id = dataReader.GetInt32("f.id");
                            food.Name = dataReader.GetString("f.name");
                            food.Weight = dataReader.GetDecimal("f.weight");
                            food.Description = dataReader.GetString("f.description");
                            food.CategoryId = dataReader.GetInt32("f.categoryid");
                            foods.Add(food);
                        }
                        return foods;
                    }
                }
            }
        }

        public IEnumerable<Reserve> GetUserReserves(int userId) // необяз
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"select r.id, r.userid, r.datetime, r.isaccepted 
                                           from reserves r 
                                           join users u on r.userid = u.id 
                                           where userid = @userId";
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<Reserve> reserves = new List<Reserve>();
                        while (dataReader.Read())
                        {
                            Reserve reserve = new Reserve();
                            reserve.Id = dataReader.GetInt32("r.id");
                            reserve.UserId = dataReader.GetInt32("r.userid");
                            reserve.DateTime = dataReader.GetDateTime("r.datetime");
                            reserve.IsAccepted = dataReader.GetBoolean("r.isaccepted");
                            reserves.Add(reserve);
                        }
                        return reserves;
                    }
                }
            }
        }

        public List<ReserveModel> GetReservesWithUserNames() 
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"select r.id as rid, r.datetime as rdatetime, u.email as uemail, u.name as uname, u.surname as usurname, r.isaccepted as risaccepted from reserves r 
                                            join users u on u.id = r.userid 
                                            order by datetime desc";
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<ReserveModel> reserves = new List<ReserveModel>();
                        while (dataReader.Read())
                        {
                            ReserveModel reserve = new ReserveModel();
                            reserve.Id = dataReader.GetInt32("rid");
                            reserve.DateTime = dataReader.GetDateTime("rdatetime");
                            reserve.Email = dataReader.GetString("uemail");
                            reserve.Name = dataReader.GetString("uname");
                            reserve.Surname = dataReader.GetString("usurname");
                            reserve.IsAccepted = dataReader.IsDBNull("risaccepted") ? null : (dataReader.GetBoolean("risaccepted") as bool?);
                            reserves.Add(reserve);
                        }
                        return reserves;
                    }
                }
            }
        }

        public ReserveModel GetReserveWithUserName(int reserveId)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"select r.id as rid, r.datetime as rdatetime, u.email as uemail, u.name as uname, u.surname as usurname, r.isaccepted as risaccepted from reserves r 
                                            join users u on u.id = r.userid 
                                            where r.id = @reserveId
                                            order by datetime desc";
                    command.Parameters.AddWithValue("@reserveId", reserveId);
                    using (var dataReader = command.ExecuteReader())
                    {
                        ReserveModel reserve = new ReserveModel();
                        while (dataReader.Read())
                        {
                            reserve.Id = dataReader.GetInt32("rid");
                            reserve.DateTime = dataReader.GetDateTime("rdatetime");
                            reserve.Email = dataReader.GetString("uemail");
                            reserve.Name = dataReader.GetString("uname");
                            reserve.Surname = dataReader.GetString("usurname");
                            reserve.IsAccepted = dataReader.IsDBNull("risaccepted") ? null : (dataReader.GetBoolean("risaccepted") as bool?);
                        }
                        return reserve;
                    }
                }
            }
        }

        public List<ReserveDetailsModel> GetReserveDetails(int reserveId)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"select 
                                                f.Name as FName, f.Description as FDescription,
	                                            mfr.amount as MFRAmount,
	                                            mf.price as MFPrice,
	                                            mf.price * mfr.Amount as FoodSum
                                            from reserves r
                                            join menufoodreserves mfr on r.id = mfr.reserveid
                                            join menufoods mf on mf.id = mfr.menufoodid
                                            join foods f on mf.foodid = f.id
                                            join users u on r.userid = u.id
                                            where r.id = @reserveId";
                    command.Parameters.AddWithValue("@reserveId", reserveId);
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<ReserveDetailsModel> reserveDetails = new List<ReserveDetailsModel>();
                        while (dataReader.Read())
                        {
                            ReserveDetailsModel reserve = new ReserveDetailsModel();
                            reserve.FoodName = dataReader.GetString("FName");
                            reserve.FoodDescription = dataReader.GetString("FDescription");
                            reserve.FoodAmount = dataReader.GetInt32("MFRAmount");
                            reserve.FoodPrice = dataReader.GetDecimal("MFPrice");
                            reserve.FoodSum = dataReader.GetDecimal("FoodSum");
                            reserveDetails.Add(reserve);
                        }
                        return reserveDetails;
                    }
                }
            }
        }

        public IEnumerable<Feedback> GetUserFeedbacks(int userId) // необяз
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"select f.id, f.message, f.userid from feedbacks f 
                                            join users u on u.id = f.userid 
                                            where u.id = @userId";
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var dataReader = command.ExecuteReader())
                    {
                        List<Feedback> feedbacks = new List<Feedback>();
                        while (dataReader.Read())
                        {
                            Feedback feedback = new Feedback();
                            feedback.Id = dataReader.GetInt32("f.id");
                            feedback.Message = dataReader.GetString("f.message");
                            feedback.UserId = dataReader.GetInt32("f.userid");
                            feedbacks.Add(feedback);
                        }
                        return feedbacks;
                    }
                }
            }
        }

        public Role GetUserRole(int userId) // необяз
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"select r.* from roles r 
                                            join users u on u.roleid = r.id 
                                            where r.id = @userId";
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var dataReader = command.ExecuteReader())
                    {
                        Role role = new Role();
                        while (dataReader.Read())
                        {
                            role.Id = dataReader.GetInt32("r.id");
                            role.Name = dataReader.GetString("r.name");
                        }
                        return role;
                    }
                }
            }
        }
    }
}
