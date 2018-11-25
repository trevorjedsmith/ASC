using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.DataModels
{
    public class MasterDataValue : BaseEntity
    {
      public bool IsActive { get; set; }
      public string Name { get; set; }
      public Guid MasterDataKeyId { get; set; }
      public MasterDataKey MasterDataKey { get; set; }
    }
}
