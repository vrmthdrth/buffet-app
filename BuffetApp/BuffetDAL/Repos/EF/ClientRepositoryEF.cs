using BuffetDAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using BuffetAuxiliaryLib.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BuffetDAL.Repos.EF
{
    public class ClientRepositoryEF
    {
        private EFContext _context;

        public ClientRepositoryEF(EFContext context)
        {
            this._context = context;
        }

        #region Favourite

        public List<Food> GetListOfFavouriteFoodsForUserName(string userName) // не нужно, изменить потом. вытаскивать не по userid а по User.Identity.Userid
        {
            var result = (from foods in _context.Foods
                          where foods.UserFavouriteFoods.Any(uff => uff.UserId == _context.Users.FirstOrDefault(u=>u.Email == userName).Id)
                          select foods);
            return result.ToList();
        }

        #endregion Favourite

        #region Menu

        public Menu GetMenuForDate(string date)//yyyy-MM-dd format
        {
            DateTime dateTime = DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            IQueryable<Menu> result = (from menus in _context.Menus
                                       where menus.Date == dateTime
                                       select menus);
            return result.FirstOrDefault();
        }

        public List<MenuFood> GetMenuListByMenuId(int menuId)
        {
            IQueryable<MenuFood> result = (from menufoods in _context.MenuFoods
                                           where menufoods.MenuId == menuId
                                           select menufoods);
            return result.ToList();
        }

        public List<Food> GetListOfFoods()
        {
            IQueryable<Food> foodList = (from foods in _context.Foods
                                         orderby foods.CategoryId ascending
                                         select foods);
            return foodList.ToList();
        }

        #endregion Menu

        #region Reserve

        private Reserve CreateAndAddReserveToDbForUser(User user)
        {
            DateTime dateTime = DateTime.Now;
            Reserve newReserve = new Reserve() { UserId = user.Id, DateTime = dateTime, IsAccepted = null };
            _context.Reserves.Add(newReserve);
            _context.SaveChanges();
            return _context.Reserves.FirstOrDefault(r => r.UserId == user.Id && r.DateTime == dateTime);
        }

        public void CreateReserveForUser(User user, List<CreateReserveModelDTO> reserve)
        {
            Reserve newReserve = CreateAndAddReserveToDbForUser(user);
            foreach(CreateReserveModelDTO row in reserve)
            {
                _context.MenuFoodReserves.Add(new MenuFoodReserve() { MenuFoodId = row.Id, ReserveId = newReserve.Id, Amount = row.Quantity });
                MenuFood menuFood = _context.MenuFoods.FirstOrDefault(mf => mf.Id == row.Id);
                menuFood.AvailableAmount = menuFood.AvailableAmount - row.Quantity;
                _context.Entry(menuFood).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        #endregion Reserve
    }
}
