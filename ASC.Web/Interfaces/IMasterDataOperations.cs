using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.DataModels;

namespace ASC.Web.Interfaces
{
    public interface IMasterDataOperations
    {
      Task<List<MasterDataKey>> GetAllMasterKeysAsync();
      Task<List<MasterDataKey>> GetMaserKeyByNameAsync(string name);
      Task<bool> InsertMasterKeyAsync(MasterDataKey key);
      Task<bool> UpdateMasterKeyAsync(MasterDataKey key);
      Task<List<MasterDataValue>> GetAllMasterValuesByKeyAsync(Guid key);
      Task<List<MasterDataValue>> GetAllMasterValuesAsync();
      Task<MasterDataValue> GetMasterValueByNameAsync(string name);
      Task<bool> InsertMasterValueAsync(MasterDataValue value);
      Task<bool> UpdateMasterValueAsync(MasterDataValue value);
  }
}
