using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Models.AccountViewModels
{
    public class ServiceEngineerViewModel
    {
      public List<ApplicationUser> ServiceEngineers { get; set; }
      public ServiceEngineerRegistrationViewModel Registration { get; set; }

      public List<CustomUserClaims> ServiceEngineerClaims { get; set; }

      public ServiceEngineerViewModel()
      {
        ServiceEngineerClaims = new List<CustomUserClaims>();
      }
    
  }
}
