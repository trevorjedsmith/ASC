using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.Configuration;
using ASC.Web.Interfaces;
using ASC.Web.Models;
using ASC.Web.Models.BaseTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ASC.Web.Data.Seed
{
  public class IdentitySeed : IIdentitySeed
  {
    public async Task Seed(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<ApplicationSettings> options, ApplicationDbContext context)
    {
      var roles = options.Value.Roles.Split(new char[] { ',' });

      foreach (var role in roles)
      {
        if (!await roleManager.RoleExistsAsync(role))
        {
          IdentityRole storageRole = new IdentityRole
          {
            Name = role
          };
          IdentityResult roleResult = await roleManager.CreateAsync(storageRole);
        }
      }

      var admin = await userManager.FindByEmailAsync(options.Value.AdminEmail);
      if (admin == null)
      {
        ApplicationUser user = new ApplicationUser
        {
          UserName = options.Value.AdminEmail,
          Email = options.Value.AdminEmail,
          EmailConfirmed = true
        };

        IdentityResult result = await userManager.CreateAsync(user, options.Value.AdminPassword);

        var userClaimsAdd = await userManager.FindByEmailAsync(options.Value.AdminEmail);
        CustomUserClaims c1 = new CustomUserClaims
        {
          UserId = userClaimsAdd.Id,
          ClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
          ClaimValue = options.Value.AdminEmail
        };
        CustomUserClaims c2 = new CustomUserClaims
        {
          UserId = userClaimsAdd.Id,
          ClaimType = "IsActive",
          ClaimValue = "True"
        };

        context.CustomUserClaims.Add(c1);
        context.CustomUserClaims.Add(c2);
        context.SaveChanges();

        if (result.Succeeded)
        {
          await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
        }

      }

      // Create a service engineer if he doesn’t exist
      var engineer = await userManager.FindByEmailAsync(options.Value.EngineerEmail);
      if (engineer == null)
      {
        ApplicationUser user = new ApplicationUser
        {
          UserName = options.Value.EngineerEmail,
          Email = options.Value.EngineerEmail,
          EmailConfirmed = true,
          LockoutEnabled = false
        };
        IdentityResult result = await userManager.CreateAsync(user, options.Value.
          EngineerPassword);
        var userClaimsAdd = await userManager.FindByEmailAsync(options.Value.EngineerEmail);
        CustomUserClaims c1 = new CustomUserClaims
        {
          UserId = userClaimsAdd.Id,
          ClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
          ClaimValue = options.Value.AdminEmail
        };
        CustomUserClaims c2 = new CustomUserClaims
        {
          UserId = userClaimsAdd.Id,
          ClaimType = "IsActive",
          ClaimValue = "True"
        };

        context.CustomUserClaims.Add(c1);
        context.CustomUserClaims.Add(c2);
        context.SaveChanges();

        // Add Service Engineer to Engineer role
        if (result.Succeeded)
        {
          await userManager.AddToRoleAsync(user, Roles.Engineer.ToString());
        }
      }


      // Create a user if he doesn’t exist
      var systemuser = await userManager.FindByEmailAsync(options.Value.UserEmail);
      if (systemuser == null)
      {
        ApplicationUser user = new ApplicationUser
        {
          UserName = options.Value.UserEmail,
          Email = options.Value.UserEmail,
          EmailConfirmed = true,
          LockoutEnabled = false
        };
        IdentityResult result = await userManager.CreateAsync(user, options.Value.
          UserPassword);
        var userClaimsAdd = await userManager.FindByEmailAsync(options.Value.UserEmail);
        CustomUserClaims c1 = new CustomUserClaims
        {
          UserId = userClaimsAdd.Id,
          ClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
          ClaimValue = options.Value.UserEmail
        };
        CustomUserClaims c2 = new CustomUserClaims
        {
          UserId = userClaimsAdd.Id,
          ClaimType = "IsActive",
          ClaimValue = "True"
        };

        context.CustomUserClaims.Add(c1);
        context.CustomUserClaims.Add(c2);
        context.SaveChanges();

        // Add USer to User Role
        if (result.Succeeded)
        {
          await userManager.AddToRoleAsync(user, Roles.User.ToString());
        }
      }
    }
  }
}
