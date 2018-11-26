using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ASC.Web.Configuration;
using ASC.Web.Utilities;

namespace ASC.Web.Controllers
{
  public class HomeController : AnonymousController
  {
    private IOptions<ApplicationSettings> _settings;
    public HomeController(IOptions<ApplicationSettings> settings)
    {
      _settings = settings;
    }

    public IActionResult Index()
    {
      // Usage of IOptions
      ViewBag.Title = _settings.Value.ApplicationTitle;
      return View();
    }

    public IActionResult Dashboard()
    {
      return View();
    }


    public IActionResult Error(string id)
    {
      if (id == "404")
        return View("NotFound");

      if (id == "401" && User.Identity.IsAuthenticated)
      {
        return View("AccessDenied");
      }
      else
      {
        return RedirectToAction("Login", "Account");
      }
    }

    public IActionResult DeveloperPage()
    {
      return View();
    }
  }
}
