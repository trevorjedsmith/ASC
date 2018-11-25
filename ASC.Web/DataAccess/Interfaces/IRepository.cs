using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ASC.Web.DataModels;

namespace ASC.Web.DataAccess.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
      void AddAsync(T entity);
      void UpdateAsync(T entity);
      void DeleteAsync(T entity);
      Task<T> FindAsync(Guid id);
      Task<T> FindSingleByQuery(Expression<Func<T, bool>> predicate);
      Task<List<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
      Task<List<T>> FindAllAsync();
      Task<List<T>> FindAllByQuery(Expression<Func<T, bool>> query);
      Task<List<T>> FindAllInAuditByQuery(Expression<Func<T, bool>> query);
      IQueryable<T> FindAllByQueryCustom();

    }
}
