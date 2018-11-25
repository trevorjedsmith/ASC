using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.DataAccess.Interfaces;
using ASC.Web.DataModels;
using ASC.Web.Interfaces;

namespace ASC.Web.Services
{
  public class LogDataOperations : ILogDataOperations
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Log> _logs;
    private readonly IRepository<ExceptionLog> _exceptionLogs;

    public LogDataOperations(IUnitOfWork unitOfWork,IRepository<Log> logs, IRepository<ExceptionLog> exceptionLogs)
    {
      _unitOfWork = unitOfWork;
      _logs = logs;
      _exceptionLogs = exceptionLogs;
    }
    public Task CreateLogAsync(string location, string message)
    {
      using (_unitOfWork)
      {
        var log = new Log
        {
          Message = $"Location: {location}, Message: {message}"
        };
        _logs.AddAsync(log);
        _unitOfWork.Commit();
      }

      return Task.FromResult(0);
    }

    public Task CreateExceptionLogAsync(string location, string message, string stacktrace)
    {

      using (_unitOfWork)
      {
        var log = new ExceptionLog
        {
          Message = $"Location: {location}, Message: {message}",
          StackTrace = stacktrace
        };
        _exceptionLogs.AddAsync(log);
        _unitOfWork.Commit();
      }

      return Task.FromResult(0);
    }
  }
}
