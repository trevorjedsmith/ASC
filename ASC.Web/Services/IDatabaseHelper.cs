using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.Models;

namespace ASC.Web.Services
{
    public interface IDatabaseHelper
    {
      Task<CustomUserClaims> GetClaim(string userId, string type);
      Task<List<CustomUserClaims>> GetUserClaims(string userId);

      Task<bool> AddClaims(List<CustomUserClaims> claimsList);

      Task<bool> UpdateClaimValues(string userId, string type, string value);
      Task<string> GetUserId(string email);
    }
}
