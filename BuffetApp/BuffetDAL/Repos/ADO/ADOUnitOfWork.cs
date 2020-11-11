using BuffetDAL.Repos.ADO.ReposForEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuffetDAL.Repos.ADO
{
    public class ADOUnitOfWork : IDisposable
    {
        private string _connectionString;
        private UserRepositoryADO _users;
        private RoleRepositoryADO _roles;
        private FoodRepositoryADO _foods;
        private CategoryRepositoryADO _categories;
        private MenuRepositoryADO _menus;
        private ReserveRepositoryADO _reserves;
        private FeedbackRepositoryADO _feedbacks;
        private MenuFoodRepositoryADO _menuFoods;
        private MenuFoodReserveRepositoryADO _menuFoodReserves;
        private UserFavouriteFoodRepositoryADO _userFavouriteFoods;
        private SpecificReportsRepositoryADO _specificReports;
        private AdminRepositoryADO _admin;

        public ADOUnitOfWork(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public UserRepositoryADO Users
        {
            get
            {
                if(this._users == null)
                {
                    this._users = new UserRepositoryADO(this._connectionString);
                }
                return this._users;
            }
        }

        public RoleRepositoryADO Roles
        {
            get
            {
                if (this._roles == null)
                {
                    this._roles = new RoleRepositoryADO(this._connectionString);
                }
                return this._roles;
            }
        }

        public FoodRepositoryADO Foods
        {
            get
            {
                if (_foods == null)
                {
                    _foods = new FoodRepositoryADO(_connectionString);
                }
                return _foods;
            }
        }

        public CategoryRepositoryADO Categories
        {
            get
            {
                if (_categories == null)
                {
                    _categories = new CategoryRepositoryADO(_connectionString);
                }
                return _categories;
            }
        }

        public MenuRepositoryADO Menus
        {
            get
            {
                if (_menus == null)
                {
                    _menus = new MenuRepositoryADO(_connectionString);
                }
                return _menus;
            }
        }

        public ReserveRepositoryADO Reserves
        {
            get
            {
                if (_reserves == null)
                {
                    _reserves = new ReserveRepositoryADO(_connectionString);
                }
                return _reserves;
            }
        }

        public FeedbackRepositoryADO Feedbacks
        {
            get
            {
                if (_feedbacks == null)
                {
                    _feedbacks = new FeedbackRepositoryADO(_connectionString);
                }
                return _feedbacks;
            }
        }

        public MenuFoodRepositoryADO MenuFoods
        {
            get
            {
                if (_menuFoods == null)
                {
                    _menuFoods = new MenuFoodRepositoryADO(_connectionString);
                }
                return _menuFoods;
            }
        }

        public MenuFoodReserveRepositoryADO MenuFoodReserves
        {
            get
            {
                if (_menuFoodReserves == null)
                {
                    _menuFoodReserves = new MenuFoodReserveRepositoryADO(_connectionString);
                }
                return _menuFoodReserves;
            }
        }

        public UserFavouriteFoodRepositoryADO UserFavouriteFoods
        {
            get
            {
                if (_userFavouriteFoods == null)
                {
                    _userFavouriteFoods = new UserFavouriteFoodRepositoryADO(_connectionString);
                }
                return _userFavouriteFoods;
            }
        }

        public SpecificReportsRepositoryADO SpecificReports
        {
            get
            {
                if (_specificReports == null)
                {
                    _specificReports = new SpecificReportsRepositoryADO(_connectionString);
                }
                return _specificReports;
            }
        }

        public AdminRepositoryADO Admin
        {
            get
            {
                if (_admin == null)
                {
                    _admin = new AdminRepositoryADO(_connectionString);
                }
                return _admin;
            }
        }

        private bool _disposed = false;
        protected void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this._connectionString = null;
                }
                this._disposed = true;
            }
        }
        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}
