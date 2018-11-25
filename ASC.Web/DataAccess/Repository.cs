using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ASC.Web.Data;
using ASC.Web.DataAccess.Interfaces;
using ASC.Web.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ASC.Web.DataAccess
{
  public class Repository<T> : IRepository<T> where T : BaseEntity
  {
    private readonly IUnitOfWork _unitOfWork;

    public Repository(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }
    public async void AddAsync(T entity)
    {
      var entityToInsert = entity as BaseEntity;
      entityToInsert.CreatedDate = DateTime.UtcNow;
      entityToInsert.UpdatedDate = DateTime.UtcNow;
      await _unitOfWork.Context.Set<T>().AddAsync(entity);
    }

    public void DeleteAsync(T entity)
    {
      var entityToDelete = entity as BaseEntity;
      entityToDelete.UpdatedDate = DateTime.UtcNow;
      entityToDelete.IsDeleted = true;
      _unitOfWork.Context.Set<T>().Update((T) entityToDelete);
    }

    public async Task<T> FindSingleByQuery(Expression<Func<T, bool>> predicate)
    {
      return await _unitOfWork.Context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public async Task<List<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
    {
      return await _unitOfWork.Context.Set<T>().Where(predicate).ToListAsync();
    }

    public Task<List<T>> FindAllAsync()
    {
      return _unitOfWork.Context.Set<T>().ToListAsync();
    }

    public async Task<List<T>> FindAllByQuery(Expression<Func<T, bool>> query)
    {
      return await _unitOfWork.Context.Set<T>().Where(query).ToListAsync();
    }

    public async Task<List<T>> FindAllInAuditByQuery(Expression<Func<T, bool>> query)
    {
      return await _unitOfWork.Context.Set<T>().Where(query).ToListAsync();
    }

    public IQueryable<T> FindAllByQueryCustom()
    {
      return _unitOfWork.Context.Set<T>().AsQueryable();
    }

    public async Task<T> FindAsync(Guid id)
    {
      return await _unitOfWork.Context.Set<T>().FindAsync(id);
    }

    public void UpdateAsync(T entity)
    {
      var entityToUpdate = entity as BaseEntity;
      entityToUpdate.UpdatedDate = DateTime.UtcNow;
      _unitOfWork.Context.Set<T>().Update((T)entityToUpdate);
    }
  }
}
