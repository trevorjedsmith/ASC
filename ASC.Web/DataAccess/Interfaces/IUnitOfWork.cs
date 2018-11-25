using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace ASC.Web.DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
      ApplicationDbContext Context { get; }
      void Commit();
  }
}
