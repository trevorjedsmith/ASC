using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.DataModels;

namespace ASC.Web.Interfaces
{
    public interface IServiceRequestMessageOperations
    {
      Task CreateServiceRequestMessageAsync(ServiceRequestMessage message);
      Task<List<ServiceRequestMessage>> GetServiceRequestMessageAsync(Guid
        serviceRequestId);
  }
}
