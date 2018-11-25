using System;
using ASC.Web.Controllers;
using Xunit;

using Moq;
using Microsoft.Extensions.Options;
using ASC.Web.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Tests
{

    public class HomeControllerTests
    {

      private readonly Mock<IOptions<ApplicationSettings>> _optionsMock;
      public HomeControllerTests()
      {
        //create mock of appsettings
        _optionsMock = new Mock<IOptions<ApplicationSettings>>();

        _optionsMock.Setup(ap => ap.Value).Returns(new ApplicationSettings
        {
          ApplicationTitle = "ASC Test Mock"
        });
      }

      [Fact]
      public void HomeController_Index_View_Test()
      {
        var controller = new HomeController(_optionsMock.Object);
      }

      [Fact]
      public void HomeController_Index_NoModel_Test()
      {
      var controller = new HomeController(_optionsMock.Object);
        // Assert Model for Null
        Assert.Null((controller.Index() as ViewResult).ViewData.Model);
    }

      [Fact]
      public void HomeController_Index_Validation_Test()
      {
        var controller = new HomeController(_optionsMock.Object);
        // Assert ModelState Error Count to 0
        Assert.Equal(0, (controller.Index() as ViewResult).ViewData.ModelState.ErrorCount);
    }
  }
}
