using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.DataAccess.Interfaces;
using ASC.Web.DataModels;
using ASC.Web.Interfaces;

namespace ASC.Web.Services
{
    public class MasterDataOperations : IMasterDataOperations
    {
      private readonly IUnitOfWork _unitOfWork;
      private readonly IRepository<MasterDataValue> _masterDataValueRepo;
      private readonly IRepository<MasterDataKey> _masterDataKeyRepo;

      public MasterDataOperations(IUnitOfWork unitOfWork, IRepository<MasterDataValue> masterDataValueRepo, IRepository<MasterDataKey> masterDataKeyRepo)
      {
        _unitOfWork = unitOfWork;
        _masterDataValueRepo = masterDataValueRepo;
        _masterDataKeyRepo = masterDataKeyRepo;
      }
      public async Task<List<MasterDataKey>> GetAllMasterKeysAsync()
      {
        var masterKeys =  await _masterDataKeyRepo.FindAllAsync();
        return masterKeys.ToList();
      }

      public async Task<List<MasterDataKey>> GetMaserKeyByNameAsync(string name)
      {
        var masterKeys = await _masterDataKeyRepo.FindAllByQuery(x => x.Name == name);
        return masterKeys.ToList();
      }

      public async Task<bool> InsertMasterKeyAsync(MasterDataKey key)
      {
        using (_unitOfWork)
        {
          _masterDataKeyRepo.AddAsync(key);
          _unitOfWork.Commit();
          return await Task.FromResult(true);
        }
      }

      public async Task<bool> UpdateMasterKeyAsync(MasterDataKey key)
      {
        using (_unitOfWork)
        {
          var masterKey = await _masterDataKeyRepo.FindAsync(key.Id);
          masterKey.IsActive = key.IsActive;
          masterKey.IsDeleted = key.IsDeleted;
          masterKey.Name = key.Name;

          _masterDataKeyRepo.UpdateAsync(masterKey);
          _unitOfWork.Commit();
          return await Task.FromResult(true);
        }
      }

      public async Task<List<MasterDataValue>> GetAllMasterValuesByKeyAsync(Guid key)
      {
        var masterValues = await _masterDataValueRepo.FindAllAsync(x => x.MasterDataKeyId == key);
        return masterValues.ToList();
      }

      public async Task<List<MasterDataValue>> GetAllMasterValuesAsync()
      {
        var masterValues = await _masterDataValueRepo.FindAllAsync();
        return masterValues.ToList();
    }

      public async Task<MasterDataValue> GetMasterValueByNameAsync(string name)
      {
        var masterValues = await _masterDataValueRepo.FindAllByQuery(x => x.Name == name);
        return masterValues.FirstOrDefault();
    }

      public async Task<bool> InsertMasterValueAsync(MasterDataValue value)
      {
      using (_unitOfWork)
      {
        _masterDataValueRepo.AddAsync(value);
        _unitOfWork.Commit();
        return await Task.FromResult(true);
      }
    }

      public async Task<bool> UpdateMasterValueAsync(MasterDataValue value)
      {
      using (_unitOfWork)
      {
        var masterValue = await _masterDataValueRepo.FindAsync(value.Id);
        masterValue.IsActive = value.IsActive;
        masterValue.IsDeleted = value.IsDeleted;
        masterValue.Name = value.Name;

        _masterDataValueRepo.UpdateAsync(masterValue);
        _unitOfWork.Commit();
        return await Task.FromResult(true);
      }
    }
    }
}
