using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.Configuration;
using ASC.Web.Data;
using ASC.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ASC.Web.Interfaces
{
    public interface IIdentitySeed
    {
      Task Seed(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole>
        roleManager, IOptions<ApplicationSettings> options, ApplicationDbContext context);
  }
}
