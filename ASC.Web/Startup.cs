using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ASC.Web.Data;
using ASC.Web.Models;
using ASC.Web.Services;
using ASC.Web.Configuration;
using ASC.Web.Data.Seed;
using ASC.Web.DataAccess;
using ASC.Web.DataAccess.Interfaces;
using ASC.Web.DataModels;
using ASC.Web.Filters;
using ASC.Web.Interfaces;
using ASC.Web.Models.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;


namespace ASC.Web
{
  public class Startup
  {
    public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

      if (env.IsDevelopment())
      {
        // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
        builder.AddUserSecrets<Startup>();
      }

      builder.AddEnvironmentVariables();
      Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      // Add framework services.
      services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

      services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
          options.User.RequireUniqueEmail = true;
          options.Cookies.ApplicationCookie.AutomaticChallenge = false;
        })
          .AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();

      services.AddOptions();
      services.Configure<ApplicationSettings>(Configuration.GetSection("AppSettings"));

      services.AddSignalR(options => { options.Hubs.EnableDetailedErrors = true; });
      services.AddMvc(f => { f.Filters.Add(typeof(CustomExceptionFilter)); });

      // Add application services.
      #region DI Mappings
      services.AddTransient<IEmailSender, AuthMessageSender>();
      services.AddTransient<ISmsSender, AuthMessageSender>();
      services.AddTransient<IDatabaseHelper, DatabaseHelper>();
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
      services.AddScoped<IUnitOfWork, UnitOfWork>();
      services.AddScoped<CustomExceptionFilter>();
      services.AddSingleton<IIdentitySeed, IdentitySeed>();
      services.AddScoped<ILogDataOperations, LogDataOperations>();
      services.AddScoped<IServiceRequestOperations, ServiceRequestOperations>();
      services.AddScoped<IMasterDataOperations, MasterDataOperations>();
      services.AddScoped<IServiceRequestMessageOperations, ServiceRequestMessageOperations>(); 
      #endregion

      services.AddAutoMapper();

      services.AddDistributedMemoryCache();

      services.AddSession();

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IIdentitySeed storageSeed)
    {
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
        app.UseBrowserLink();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStatusCodePagesWithRedirects("/Home/Error/{0}");
      app.UseSession();
      app.UseStaticFiles();

      app.UseIdentity();

      // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715



      app.UseMvc(routes =>
      {
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });

      app.UseWebSockets();
      app.UseSignalR();

      await storageSeed.Seed(app.ApplicationServices.GetService<UserManager<ApplicationUser>>(),
                       app.ApplicationServices.GetService<RoleManager<IdentityRole>>(),
                       app.ApplicationServices.GetService<IOptions<ApplicationSettings>>(),
        app.ApplicationServices.GetService<ApplicationDbContext>());

    }
  }
}
