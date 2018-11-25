using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.DataModels;

namespace ASC.Web.DataModels
{
    public class Log : BaseEntity
    {
      public string Message { get; set; }
    }
}
