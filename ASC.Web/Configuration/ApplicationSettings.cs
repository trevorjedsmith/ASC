using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Configuration
{
  public class ApplicationSettings
  {
    //General
    public string ApplicationTitle { get; set; }
    public string AdminEmail { get; set; }
    public string AdminName { get; set; }
    public string AdminPassword { get; set; }
    public string Roles { get; set; }
    //Engineer
    public string EngineerEmail { get; set; }
    public string EngineerName { get; set; }
    public string EngineerPassword { get; set; }

    //User  
    public string UserEmail { get; set; }
    public string UserName { get; set; }
    public string UserPassword { get; set; }
    //Email
    public string SMTPServer { get; set; }
    public int SMTPPort { get; set; }
    public string SMTPAccount { get; set; }
    public string SMTPPassword { get; set; }
  }
}
