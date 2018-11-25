using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Models.BaseTypes
{
  public static class Constants
  {
    public const string Equal = "==";
    public const string NotEqual = "!=";
    public const string GreaterThan = ">";
    public const string GreaterThanOrEqual = ">=";
    public const string LessThan = "<";
    public const string LessThanOrEqual = "<=";
  }

  public enum Roles
  {
    Admin, Engineer, User
  }
}
