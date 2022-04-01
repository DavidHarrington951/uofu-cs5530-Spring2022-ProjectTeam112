using Xunit;
using LMS.Controllers;
using LMS.Models.LMSModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LMSUnitTestLibrary
{
    public class UnitTest1
    {

        private static ServiceProvider NewServiceProvider()
        {
            ServiceProvider serviceProvider = new ServiceCollection()
              .AddEntityFrameworkInMemoryDatabase()
              .BuildServiceProvider();
            return serviceProvider;
        }

        [Fact]
        public void TestDepartmentQueryType()
        {
            var optionsBuilder = new DbContextOptionsBuilder<Team112LMSContext>();
            optionsBuilder.UseInMemoryDatabase("SmallLms").UseApplicationServiceProvider(NewServiceProvider());

            Team112LMSContext context = new Team112LMSContext(optionsBuilder.Options);

            CommonController controller = new CommonController();
            JsonResult result = controller.GetDepartments() as JsonResult;

            var x = result.Value;
            Type valueType = x.GetType();
            Assert.True(valueType.IsArray);
        }

    }
}
