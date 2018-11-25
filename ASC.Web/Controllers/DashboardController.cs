using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.Configuration;
using ASC.Web.DataModels;
using ASC.Web.Interfaces;
using ASC.Web.Models.BaseTypes;
using ASC.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ASC.Web.Controllers
{
  public class DashboardController : BaseController
  {
    private readonly IOptions<ApplicationSettings> _settings;
    private readonly IServiceRequestOperations _operations;
    private readonly IMasterDataOperations _masterData;

    public DashboardController(IOptions<ApplicationSettings> settings, IServiceRequestOperations operations, IMasterDataOperations masterData)
    {
      _settings = settings;
      _operations = operations;
      _masterData = masterData;
    }

    public async Task<IActionResult> Dashboard()
    {

      var status = new List<string>
        {
          Status.New.ToString(),
          Status.InProgress.ToString(),
          Status.Initiated.ToString(),
          Status.RequestForInformation.ToString()

        };

      List<ServiceRequest> serviceRequests = new List<ServiceRequest>();

      if (HttpContext.User.IsInRole(Roles.Admin.ToString()))
      {
        serviceRequests =
          await _operations.GetServiceRequestsByRequestedDateAndStatus(DateTime.UtcNow.AddDays(-7), status);
      }

      else if (HttpContext.User.IsInRole(Roles.Engineer.ToString()))
      {
        serviceRequests =
          await _operations.GetServiceRequestsByRequestedDateAndStatus(DateTime.UtcNow.AddDays(-7), status,
            serviceEngineerEmail: HttpContext.User.Identity.Name);
      }

      else
      {
        serviceRequests =
          await _operations.GetServiceRequestsByRequestedDateAndStatus(DateTime.UtcNow.AddYears(-1),
            email: HttpContext.User.Identity.Name);
      }

      return View(new DashboardViewModel{ServiceRequests = serviceRequests.OrderByDescending(p=> p.RequestedDate).ToList()});
    }


  }
}
