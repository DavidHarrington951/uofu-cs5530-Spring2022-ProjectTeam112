using Xunit;
using LMS.Controllers;
using LMS.Models.LMSModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections;
using static LMSUnitTestLibrary.DatabaseBuilder;

namespace LMSUnitTestLibrary
{
    public class CommonControllerTests
    {

        /// <summary>
        /// Test that GetDepartments returns an Array
        /// </summary>
        [Fact]
        public void GetDepartmentsTest0()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetDepartments() as JsonResult;

            Assert.True(result.Value.GetType().IsArray);
        }

        /// <summary>
        /// Test that GetDepartments returns an Array
        /// </summary>
        [Fact]
        public void GetCatalogTest0()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetCatalog() as JsonResult;

            Assert.True(result.Value.GetType().IsArray);
        }

        /// <summary>
        /// Tests that the element type in the Array that 
        /// GetDepartments returns contains two String values
        /// </summary>
        [Fact]
        public void GetDepartmentTest1()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetDepartments() as JsonResult;
            Object[] departments = (Object[])result.Value;
            dynamic dept = departments[0];

            Assert.True(dept.name is String);
            Assert.True(dept.subject is String);
        }

        /// <summary>
        /// Tests that the element type in the Array that GetCatalog returns contains
        /// 2 String values and an IEnumerable
        /// </summary>
        [Fact]
        public void GetCatalogTest1()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetCatalog() as JsonResult;
            Object[] catalogs = (Object[])result.Value;
            dynamic catalog = catalogs[0];

            Assert.True(catalog.subject is String);
            Assert.True(catalog.dname is String);
            Assert.True(catalog.courses is System.Collections.IEnumerable);
        }

        /// <summary>
        /// One of the value Types in the Array that GetCatalog returns is an IEnumerable,
        /// Tests that the IEnumerable element type contains an UInt and a String
        /// </summary>
        [Fact]
        public void GetCatalogTest2()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetCatalog() as JsonResult;
            Object[] catalogs = (Object[])result.Value;
            dynamic catalog = catalogs[0];
            IEnumerable courses = (IEnumerable)catalog.courses;
            IEnumerator enumerator = courses.GetEnumerator();
            enumerator.MoveNext();
            dynamic course = enumerator.Current;

            Assert.True(course.number is UInt32);
            Assert.True(course.cname is String);
        }

        /// <summary>
        /// Tests that GetDepartments returns a single value
        /// when the Database has a single database value
        /// </summary>
        [Fact]
        public void GetDepartmentsTest2()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetDepartments() as JsonResult;
            Object[] departments = (Object[])result.Value;

            Assert.Single(departments);
        }

        [Fact]
        public void GetCatalogTest3()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetCatalog() as JsonResult;
            Object[] catalogs = (Object[])result.Value;

            Assert.Single(catalogs);
        }

        /// <summary>
        /// Tests that GetDepartments returns a single value,
        /// and that that value contains correct values. 
        /// </summary>
        [Fact]
        public void GetDepartmentTest3()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetDepartments() as JsonResult;
            Object[] departments = (Object[])result.Value;
            dynamic dept = departments[0];

            Assert.Equal("Computer Science", dept.name);
            Assert.Equal("CS", dept.subject);
        }


        /// <summary>
        /// Tests that GetDepartments returns the correct data from the DB
        /// </summary>
        [Fact]
        public void GetCatalogTest4()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetCatalog() as JsonResult;
            Object[] catalogs = (Object[])result.Value;
            dynamic catalog = catalogs[0];

            Assert.Equal("CS", catalog.subject);
            Assert.Equal("Computer Science", catalog.dname);

            IEnumerable courses = (IEnumerable)catalog.courses;
            IEnumerator enumerator = courses.GetEnumerator();
            enumerator.MoveNext();
            dynamic course = enumerator.Current;

            Assert.True(course.number == 1410);
            Assert.Equal("Intro to Obj Oriented Programming", course.cname);
        }

        /// <summary>
        /// Tests that GetDepartments returns multiple values when
        /// the database contains multiple departments.
        /// </summary>
        [Fact]
        public void GetDepartmentTest4()
        {
            CommonController Controller = CommonController(ManyDepartments());
            JsonResult result = Controller.GetDepartments() as JsonResult;
            Object[] departments = (Object[])result.Value;
            Assert.Equal(4, departments.Length);
        }

        /// <summary>
        /// Tests that GetCatalog returns multiple values when the database contains multiple courses
        /// for the default department.
        /// </summary>
        [Fact]
        public void GetCatalogTest5()
        {
            CommonController Controller = CommonController(SingleManyClasses());
            JsonResult result = Controller.GetCatalog() as JsonResult;
            Object[] catalogs = (Object[])result.Value;

            dynamic catalog = catalogs[0];

            IEnumerable courses = (IEnumerable)catalog.courses;

            int count = 0;

            foreach(var Course in courses)
            {
                count++;
            }

            Assert.Equal(4, count);
        }
    }
}
