using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BuffetDAL.Repos.EF
{
    public class IdentityEFContext : IdentityDbContext<IdentityUser>
    {
        public IdentityEFContext(DbContextOptions<IdentityEFContext> options) 
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
