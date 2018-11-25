using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.DataModels;

namespace ASC.Web.Interfaces
{
    public interface IServiceRequestOperations
    {
    
      Task CreateServiceRequestAsync(ServiceRequest request);
      Task<ServiceRequest> UpdateServiceRequestAsync(ServiceRequest request);
      Task<ServiceRequest> UpdateServiceRequestStatusAsync(Guid key, string status);

      Task<ServiceRequest> GetServiceRequestByRowKey(Guid id);

    Task<List<ServiceRequest>> GetServiceRequestsByRequestedDateAndStatus
      (DateTime? requestedDate,
        List<string> status = null,
        string email = "",
        string serviceEngineerEmail = "");
  }
}
