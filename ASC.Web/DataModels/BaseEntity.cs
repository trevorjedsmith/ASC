using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace ASC.Web.DataModels
{
    public abstract class BaseEntity
    {
      public Guid Id { get; set; }
      public bool IsDeleted { get; set; }
      public DateTime CreatedDate { get; set; }
      public DateTime UpdatedDate { get; set; }
      public string CreatedBy { get; set; }
      public string UpdatedBy { get; set; }
  }
}
