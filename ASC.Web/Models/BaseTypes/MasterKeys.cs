using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Models.BaseTypes
{
  public enum MasterKeys
  {
    VehicleName, VehicleType
  }

  public enum Status
  {
    New, Denied, Pending, Initiated, InProgress, PendingCustomerApproval,
    RequestForInformation, Completed
  }
}
