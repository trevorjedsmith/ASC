using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.DataModels;

namespace ASC.Web.Models.ViewModels
{
    public class DashboardViewModel
    {
      public List<ServiceRequest> ServiceRequests { get; set; }
    }
}
