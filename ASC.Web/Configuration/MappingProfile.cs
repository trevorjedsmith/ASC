using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.DataModels;
using ASC.Web.Models.ViewModels;
using AutoMapper;

namespace ASC.Web.Configuration
{
    public class MappingProfile : Profile
    {
      public MappingProfile()
      {
        CreateMap<MasterDataKey, MasterDataKeyViewModel>();
        CreateMap<MasterDataKeyViewModel, MasterDataKey>();

        CreateMap<MasterDataValue, MasterDataValueViewModel>();
        CreateMap<MasterDataValueViewModel, MasterDataValue>();
    }
    }
}
