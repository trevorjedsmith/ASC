﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Models.ViewModels
{
    public class MasterValuesViewModel
    {
      public List<MasterDataValueViewModel> MasterValues { get; set; }
      public MasterDataValueViewModel MasterValueInContext { get; set; }
      public bool IsEdit { get; set; }
  }
}
