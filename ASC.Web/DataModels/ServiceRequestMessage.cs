using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.DataModels
{
    public class ServiceRequestMessage: BaseEntity
    {
      public Guid? ServiceRequestId { get; set; }
      public string FromDisplayName { get; set; }
      public string FromEmail { get; set; }
      public string Message { get; set; }
      public DateTime? MessageDate { get; set; }
    }
}
