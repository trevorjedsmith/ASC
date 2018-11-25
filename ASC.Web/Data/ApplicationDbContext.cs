using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.DataModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ASC.Web.Models;

namespace ASC.Web.Data
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CustomUserClaims> CustomUserClaims { get; set; }

    #region Master Key & Values
    public DbSet<MasterDataKey> MasterDataKeys { get; set; }
    public DbSet<MasterDataValue> MasterDataValues { get; set; } 
    #endregion

    public DbSet<ServiceRequest> ServiceRequests { get; set; }
    public DbSet<ServiceRequestMessage> ServiceRequestMessages { get; set; }

    #region Logging
    public DbSet<Log> Logs { get; set; }
    public DbSet<ExceptionLog> ExceptionLogs { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
      // Customize the ASP.NET Identity model and override the defaults if needed.
      // For example, you can rename the ASP.NET Identity table names and more.
      // Add your customizations after calling base.OnModelCreating(builder);
    }
  }
}
