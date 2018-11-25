using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.Data;
using ASC.Web.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASC.Web.DataAccess
{
    public class UnitOfWork: IUnitOfWork
    {
      private readonly ApplicationDbContext _context;

      public UnitOfWork(ApplicationDbContext context)
      {
        _context = context;

      }


      public ApplicationDbContext Context => _context;

      public void Commit()
      {
        _context.SaveChanges();
      }

      public void Dispose()
      {
        _context?.Dispose();
      }
    }
}
