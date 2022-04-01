using LMS.Controllers;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using Xunit;

namespace LMSTestingLibrary
{
    public class UnitTest1
    {
        [Fact]
        public void TestDepartmentsType()
        {
            var optionsBuilder = new DbContextOptionsBuilder<Team112LMSContext>();
            optionsBuilder.UseInMemoryDatabase("SmallLms").UseApplicationServiceProvider();

            Team112LMSContext context = new Team112LMSContext(optionsBuilder.Options);

            CommonController controller = new CommonController();
            JsonResult result = controller.GetDepartments() as JsonResult;

            var x = result.Value;
            Type valueType = x.GetType();
            Assert.True(valueType.IsArray);
            //we won't test the element type since it is an anonymous type
        }

        [Fact]
        public void Test2()
        {

        }
    }
}
