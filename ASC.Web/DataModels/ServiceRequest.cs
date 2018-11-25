using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.DataModels
{
    public class ServiceRequest: BaseEntity
    {
      public string VehicleName { get; set; }
      public string VehicleType { get; set; }
      public string Status { get; set; }
      public string RequestedServices { get; set; }
      public DateTime? RequestedDate { get; set; }
      public DateTime? CompletedDate { get; set; }
      public string ServiceEngineer { get; set; }
      public string Customer { get; set; }
  }
}
