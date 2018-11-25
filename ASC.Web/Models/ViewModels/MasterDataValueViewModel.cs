using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Models.ViewModels
{
    public class MasterDataValueViewModel
    {
      public Guid? Id { get; set; }

      public Guid? MasterDataKeyId { get; set; }
      public bool IsActive { get; set; }
      [Required]
      public string Name { get; set; }
  }
}
