using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.Data;
using ASC.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASC.Web.Services
{
  public class DatabaseHelper : IDatabaseHelper
  {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DatabaseHelper(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
      _context = context;
      _userManager = userManager;
    }
    public async Task<CustomUserClaims> GetClaim(string userId, string type)
    {
      return await _context.CustomUserClaims.FirstOrDefaultAsync(x => x.UserId == userId && x.ClaimType == type);

    }

    public async Task<List<CustomUserClaims>> GetUserClaims(string userId)
    {
      return await _context.CustomUserClaims.Where(x => x.UserId == userId).ToListAsync();
    }

    public async Task<bool> AddClaims(List<CustomUserClaims> claimsList)
    {
      _context.CustomUserClaims.AddRange(claimsList);
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateClaimValues(string userId, string type, string value)
    {
      var userClaims = _context.CustomUserClaims.Where(x => x.UserId == userId);
      foreach (var item in userClaims)
      {
        if (item.ClaimType == type)
        {
          item.ClaimValue = value;
          return await _context.SaveChangesAsync() > 0;
        }
      }

      return false;
    }

    public async Task<string> GetUserId(string email)
    {
      if (!string.IsNullOrEmpty(email))
      {
        var user = await _userManager.FindByEmailAsync(email);
        return user.Id ?? string.Empty;
      }

      return string.Empty;
    }
  }
}
