using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ASC.Web.DataAccess.Interfaces;
using ASC.Web.DataModels;
using ASC.Web.Interfaces;
using Microsoft.EntityFrameworkCore;
using Remotion.Linq.Clauses.ResultOperators;


namespace ASC.Web.Services
{
    public class ServiceRequestOperations : IServiceRequestOperations
    {
      private readonly IUnitOfWork _unitOfWork;
      private readonly IRepository<ServiceRequest> _serviceRepository;

      public ServiceRequestOperations(IUnitOfWork unitOfWork, IRepository<ServiceRequest> serviceRepository)
      {
        _unitOfWork = unitOfWork;
        _serviceRepository = serviceRepository;
      }
      public Task CreateServiceRequestAsync(ServiceRequest request)
      {
        using (_unitOfWork)
        {
          _serviceRepository.AddAsync(request);
          _unitOfWork.Commit();
          return Task.FromResult(0);
        }
      }

      public Task<ServiceRequest> UpdateServiceRequestAsync(ServiceRequest request)
      {
      using (_unitOfWork)
      {
        _serviceRepository.UpdateAsync(request);
        _unitOfWork.Commit();
        return Task.FromResult(request);
      }
    }

      public async Task<ServiceRequest> UpdateServiceRequestStatusAsync(Guid key, string status)
      {
        using (_unitOfWork)
        {
          var serviceRequest = await _serviceRepository.FindAsync(key);
          if (serviceRequest == null)
            throw new NullReferenceException();

          serviceRequest.Status = status;
          _serviceRepository.UpdateAsync(serviceRequest);
          _unitOfWork.Commit();
          return await Task.FromResult(serviceRequest);
        }
      }

      public async Task<ServiceRequest> GetServiceRequestByRowKey(Guid id)
      {
        var serviceRequest = await _serviceRepository.FindAsync(id);
        return serviceRequest;
      }

      public async Task<List<ServiceRequest>> GetServiceRequestsByRequestedDateAndStatus(DateTime? requestedDate, List<string> status = null, string email = "",
        string serviceEngineerEmail = "")
      {

        var query = _serviceRepository.FindAllByQueryCustom();

        if (requestedDate.HasValue)
        {
          //Filter on req date
          query = query.Where(x => x.RequestedDate >= requestedDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
          query = query.Where(x => x.Customer == email);
        }

        if (!string.IsNullOrEmpty(serviceEngineerEmail))
        {
          query = query.Where(x => x.ServiceEngineer == serviceEngineerEmail);
        }

        //if (status != null)
        //{
        //  foreach (var state in status)
        //  {
        //    query = query.Where(x => x.Status == null || x.Status == state);
        //  }          
        //}

        return await query.ToListAsync();
      }
    }
}
