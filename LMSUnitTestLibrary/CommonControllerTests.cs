using Xunit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LMS.Controllers;
using LMS.Models.LMSModels;
using static LMSUnitTestLibrary.DatabaseBuilder;

namespace LMSUnitTestLibrary
{
    /*
     * Authors: David Harrington and Ethan Quinlan
     */
    /// <summary>
    /// Testing Suite for CommonController Class. Contains tests for Type Checking and Value Checking
    /// </summary>
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
        /// Tests that GetClassOfferings returns an Array
        /// </summary>
        [Fact]
        public void GetClassOfferingsTest0()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetClassOfferings("CS", 1410) as JsonResult;

            Assert.True(result.Value.GetType().IsArray); 
        }

        /// <summary>
        /// Tests that GetClassOfferings still returns an Array 
        /// when Offered a valid department and an invalid course number
        /// </summary>
        [Fact]
        public void GetClassOfferingsTest1()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetClassOfferings("CS", 0) as JsonResult;

            Assert.True(result.Value.GetType().IsArray);
        }

        /// <summary>
        /// Tests that GetClassOfferings still returns an Array
        /// when offered completely invalid input. 
        /// </summary>
        [Fact]
        public void GetClassOfferingsTest2()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetClassOfferings("NONSENSE", 0) as JsonResult;

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
        /// Tests that the element type in the Array that GetClassOfferings returns contains
        /// 6 Strings and a UInt32 value.
        /// </summary>
        [Fact]
        public void GetClassOfferingsTest3()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetClassOfferings("CS", 1410) as JsonResult;
            Object[] offerings = (Object[])result.Value;
            dynamic offering = offerings[0];

            Assert.True(offering.season is String);
            Assert.True(offering.year is UInt32);
            Assert.True(offering.location is String);
            Assert.True(offering.start is String);
            Assert.True(offering.end is String);
            Assert.True(offering.fname is String);
            Assert.True(offering.lname is String);
        }

        /// <summary>
        /// Tests that GetDepartments returns a single value
        /// when the Database has a single Department
        /// </summary>
        [Fact]
        public void GetDepartmentsTest2()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetDepartments() as JsonResult;
            Object[] departments = (Object[])result.Value;

            Assert.Single(departments);
        }

        /// <summary>
        /// Tests that GetCatalog returns a single value 
        /// when the Database has a single Course
        /// </summary>
        [Fact]
        public void GetCatalogTest3()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetCatalog() as JsonResult;
            Object[] catalogs = (Object[])result.Value;

            Assert.Single(catalogs);
        }

        /// <summary>
        /// Tests that GetClassOfferings returns a single value
        /// when the Database has a single Class for a specific
        /// Course
        /// </summary>
        [Fact]
        public void GetClassOfferingsTest4()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetClassOfferings("CS", 1410) as JsonResult;
            Object[] offerings = (Object[])result.Value;

            Assert.Single(offerings);
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

        [Fact]
        public void GetClassOfferingsTest5()
        {
            CommonController C = DefaultCommonController();
            JsonResult result = C.GetClassOfferings("CS", 1410) as JsonResult;

            
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
            CommonController Controller = CommonController(SingleManyCourses());
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

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void GetAssignmentContentsTest0()
        {
            CommonController Controller = DefaultCommonController();
            ContentResult result =
                Controller.GetAssignmentContents("CS", 1410, "Spring", 2022, "Problem Set", "PS1") as ContentResult;

            Assert.Equal("Step 1: Profit", result.Content);
        }

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void GetSubmissionTextTest0()
        {
            CommonController Controller = DefaultCommonController();
            ContentResult result =
                Controller.GetSubmissionText("CS", 1410, "Spring", 2022, "Problem Set", "PS1", "u0000001") as ContentResult;

            Assert.Equal("Profit", result.Content);
        }
    }
}
