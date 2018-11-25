using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.DataModels
{
    public class ExceptionLog: BaseEntity
    {
      public string Message { get; set; }
      public string StackTrace { get; set; }
    }
}
