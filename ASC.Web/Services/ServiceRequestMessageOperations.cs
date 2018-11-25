using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.DataAccess.Interfaces;
using ASC.Web.DataModels;
using ASC.Web.Interfaces;

namespace ASC.Web.Services
{
    public class ServiceRequestMessageOperations: IServiceRequestMessageOperations
    {
      private readonly IUnitOfWork _unitOfWork;
      private readonly IRepository<ServiceRequestMessage> _serviceRequestOperations;

      public ServiceRequestMessageOperations(IUnitOfWork unitOfWork, IRepository<ServiceRequestMessage> serviceRequestOperations)
      {
        _unitOfWork = unitOfWork;
        _serviceRequestOperations = serviceRequestOperations;
      }


      public Task CreateServiceRequestMessageAsync(ServiceRequestMessage message)
      {
        using (_unitOfWork)
        {
          _serviceRequestOperations.AddAsync(message);
          _unitOfWork.Commit();
          return Task.FromResult(0);
        }
      }

      public async Task<List<ServiceRequestMessage>> GetServiceRequestMessageAsync(Guid serviceRequestId)
      {
        var serviceRequestMessages =
          await _serviceRequestOperations.FindAllByQuery(x => x.ServiceRequestId == serviceRequestId);

        return serviceRequestMessages.ToList();
      }
    }
}
