﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Models.ViewModels
{
    public class MasterKeysViewModel
    {
      public List<MasterDataKeyViewModel> MasterKeys { get; set; }
      public MasterDataKeyViewModel MasterKeyInContext { get; set; }
      public bool IsEdit { get; set; }
  }
}
