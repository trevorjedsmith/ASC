using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASC.Web.Models
{
    public class CurrentUser
    {
      public string Name { get; set; }
      public string Email { get; set; }
      public bool IsActive { get; set; }
      public string[] Roles { get; set; }
  }

  public static class ClaimsPrincipalExtensions
  {
    public static CurrentUser GetCurrentUserDetails(this ClaimsPrincipal principal)
    {
      if (!principal.Claims.Any())
        return null;

      return new CurrentUser
      {
        Name = principal.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault(),
        Email = principal.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault(),
        Roles = principal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray(),
        IsActive = Boolean.Parse(principal.Claims.Where(c => c.Type == "IsActive").Select(c => c.Value).SingleOrDefault()),
      };
    }

    public static CurrentUser GetCurrentUserCustomClaimDetails(List<CustomUserClaims> userClaims)
    {
      if (!userClaims.Any())
        return null;

      return new CurrentUser
      {
        Name = userClaims.Where(c => c.ClaimType == ClaimTypes.Name).Select(c => c.ClaimValue).SingleOrDefault(),
        Email = userClaims.Where(c => c.ClaimType == ClaimTypes.Email).Select(c => c.ClaimValue).SingleOrDefault(),
        Roles = userClaims.Where(c => c.ClaimType == ClaimTypes.Role).Select(c => c.ClaimValue).ToArray(),
        IsActive = Boolean.Parse(userClaims.Where(c => c.ClaimType == "IsActive").Select(c => c.ClaimValue).SingleOrDefault()),
      };
    }
  }
}
