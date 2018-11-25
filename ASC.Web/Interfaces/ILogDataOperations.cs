using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Interfaces
{
    public interface ILogDataOperations
    {
      Task CreateLogAsync(string id, string message);
      Task CreateExceptionLogAsync(string category, string message, string stacktrace);
  }
}
