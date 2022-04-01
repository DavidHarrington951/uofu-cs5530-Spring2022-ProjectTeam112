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

        public Team112LMSContext PopulateEmptyDatabase()
        {
            var optionsBuilder = new DbContextOptionsBuilder<Team112LMSContext>();
            optionsBuilder.UseInMemoryDatabase("SmallLms").UseApplicationServiceProvider(NewServiceProvider());
            Team112LMSContext context = new Team112LMSContext(optionsBuilder.Options);

            return context;
        }

        public Team112LMSContext PopulateSingleDepartment()
        {
            var optionsBuilder = new DbContextOptionsBuilder<Team112LMSContext>();
            optionsBuilder.UseInMemoryDatabase("SmallLms").UseApplicationServiceProvider(NewServiceProvider());
            Team112LMSContext context = new Team112LMSContext(optionsBuilder.Options);

            Departments department = new Departments
            {
                DprtAbv = "CS",
                DprtName = "Computer Science"
            };
            context.Departments.Add(department);
            context.SaveChanges();

            return context;
        }

        [Fact]
        public void TestDepartmentQueryType()
        {
            Team112LMSContext context = PopulateEmptyDatabase();
            CommonController controller = new CommonController(context);
            JsonResult result = controller.GetDepartments() as JsonResult;

            var x = result.Value;
            Type valueType = x.GetType();
            Assert.True(valueType.IsArray);
        }

        [Fact]
        public void TestEmptyTables()
        {
            Team112LMSContext context = PopulateEmptyDatabase();
            CommonController controller = new CommonController(context);

            JsonResult result = controller.GetDepartments() as JsonResult;
            dynamic x = result.Value;
            Assert.Equal(0, x.Length);
        }

        [Fact]
        public void TestSingleTable()
        {
            Team112LMSContext context = PopulateSingleDepartment();
            CommonController controller = new CommonController(context);

            JsonResult result = controller.GetDepartments() as JsonResult;
            dynamic x = result.Value;
            Assert.Equal(1, x.Length);

            Type type = x[0].GetType();
            String abv = (String)type.GetProperty("subject").GetValue(x[0], null);
        }



    }
}
