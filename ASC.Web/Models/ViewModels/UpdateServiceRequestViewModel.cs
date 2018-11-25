using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Models.ViewModels
{
    public class UpdateServiceRequestViewModel: NewServiceRequestViewModel
    {
      public Guid Id { get; set; }
      [Required]
      [Display(Name = "Service Engineer")]
      public string ServiceEngineer { get; set; }
      [Required]
      [Display(Name = "Status")]
      public string Status { get; set; }
  }
}
