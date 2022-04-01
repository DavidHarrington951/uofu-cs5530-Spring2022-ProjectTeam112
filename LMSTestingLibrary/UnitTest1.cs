using LMS.Controllers;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace LMSTestingLibrary
{
    public class UnitTest1
    {
        public Team112LMSContext generateSmallLms()
        {
            var optionsBuilder = new DbContextOptionsBuilder<Team112LMSContext>();
            optionsBuilder.UseInMemoryDatabase("SmallLms");

            Team112LMSContext context = new Team112LMSContext(optionsBuilder.Options);

            return context;
        }

        [Fact]
        public void TestDepartmentsType()
        {
            Team112LMSContext smallContext = generateSmallLms();
            CommonController controller = new CommonController(smallContext);
            JsonResult result = controller.GetDepartments() as JsonResult;

            var x = result.Value;
            Type valueType = x.GetType();
            Assert.True(valueType.IsArray);
        }

        [Fact]
        public void Test2()
        {

        }
    }
}
