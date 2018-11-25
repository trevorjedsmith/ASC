using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.DataModels;
using ASC.Web.Models.ViewModels;
using AutoMapper;


namespace ASC.Web.Configuration
{
    public class ServiceRequestMappingProfile : Profile
    {
      public ServiceRequestMappingProfile()
      {
        CreateMap<ServiceRequest, NewServiceRequestViewModel>();
        CreateMap<NewServiceRequestViewModel, ServiceRequest>();

        CreateMap<ServiceRequest, UpdateServiceRequestViewModel>();
        CreateMap<UpdateServiceRequestViewModel, ServiceRequest>();
    }
    }
}
