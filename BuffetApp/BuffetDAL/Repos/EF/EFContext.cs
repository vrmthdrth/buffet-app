using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using BuffetDAL.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BuffetDAL.Repos.EF
{
    public class EFContext : DbContext
    {
        public DbSet<User> Users { get; set; } 
        public DbSet<Role> Roles { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Reserve> Reserves { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<MenuFood> MenuFoods { get; set; }
        public DbSet<MenuFoodReserve> MenuFoodReserves { get; set; }
        public DbSet<UserFavouriteFood> UserFavouriteFoods { get; set; }
        public ClientRepositoryEF ClientRepository { get; set; }

        public EFContext(DbContextOptions<EFContext> options) : base(options)
        {
            this.ClientRepository = new ClientRepositoryEF(this); 
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            Role sa = new Role { Id = 1, Name = "SA" };
            Role admin = new Role { Id = 2, Name = "Admin" };
            Role user = new Role { Id = 3, Name = "User" };


            modelBuilder.Entity<Role>().HasData(
                    new Role[]
                    {
                        sa,
                        admin,
                        user
                    }
                );

            Category drinks = new Category { Id = 1, Name = "Drinks" };
            Category fCourse = new Category { Id = 2, Name = "First Course" };
            Category sCourse = new Category { Id = 3, Name = "Second Course" };
            Category desserts = new Category { Id = 4, Name = "Desserts" };

            modelBuilder.Entity<Category>().HasData(
                    new Category[]
                    {
                        drinks,
                        fCourse,
                        sCourse,
                        desserts
                    }
                );

            User[] users = new User[]
                    {
                        new User
                        {
                            Id = 1,
                            Email = "vit@yan.ru",
                           // Password = "Sk2kso1",
                            Name = "Виталий",
                            Surname = "Александров",
                            RoleId = 1
                        }
                    };
    
            modelBuilder.Entity<User>().HasData(users);
        }
    }
}
