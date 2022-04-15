using Xunit;
using LMS.Controllers;
using LMS.Models.LMSModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace LMSUnitTestLibrary
{
    public class CommonControllerTests
    {
        [Fact]
        public void GetDepartmentsTest0()
        {
            Team112LMSContext Context = SingleDepartment();
            CommonController C = new CommonController(Context);
            JsonResult result = C.GetDepartments() as JsonResult;
            Assert.True(result.Value.GetType().IsArray);
        }

        [Fact]
        public void GetDepartmentsTest1()
        {
            Team112LMSContext Context = SingleDepartment();
            CommonController C = new CommonController(Context);
            JsonResult result = C.GetDepartments() as JsonResult;
            Object[] departments = (Object[])result.Value;
            Assert.Single(departments);
        }

        [Fact]
        public void GetDepartmentTest2()
        {
            Team112LMSContext Context = SingleDepartment();
            CommonController C = new CommonController(Context);
            JsonResult result = C.GetDepartments() as JsonResult;
            Object[] departments = (Object[])result.Value;
            dynamic dept = departments[0];

            Assert.True(dept.name is String);
            Assert.True(dept.subject is String);
        }

        [Fact]
        public void GetDepartmentTest3()
        {
            Team112LMSContext Context = SingleDepartment();
            CommonController C = new CommonController(Context);
            JsonResult result = C.GetDepartments() as JsonResult;
            Object[] departments = (Object[])result.Value;
            dynamic dept = departments[0];
            Assert.Equal("Computer Science", dept.name);
            Assert.Equal("CS", dept.subject);
        }

        [Fact]
        public void GetDepartmentCoursesTest1()
        {
            Team112LMSContext Context = SingleCourse();
            CommonController C = new CommonController(Context);
            JsonResult result = C.GetCatalog() as JsonResult;

            Object[] departments = (Object[])result.Value;
            dynamic dept = departments[0];

            IEnumerable<Object> c = (IEnumerable<Object>)dept.courses;

#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
            Assert.Equal(1, c.Count());
#pragma warning restore xUnit2013 // Do not use equality check to check for collection size.
        }

        private static ServiceProvider NewServiceProvider()
        {
            ServiceProvider serviceProvider = new ServiceCollection()
              .AddEntityFrameworkInMemoryDatabase()
              .BuildServiceProvider();
            return serviceProvider;
        }

        /// <summary>
        /// Creates a new empty Database Context, it has NOTHING in it. 
        /// </summary>
        /// <returns>An Empty Database Context.</returns>
        public static Team112LMSContext DefaultDatabase()
        {
            var optionsBuilder = new DbContextOptionsBuilder<Team112LMSContext>();
            optionsBuilder.UseInMemoryDatabase("SmallLms").UseApplicationServiceProvider(NewServiceProvider());
            Team112LMSContext Context = new Team112LMSContext(optionsBuilder.Options);
            Context.SaveChanges();
            return Context;
        }

        /// <summary>
        /// Creates a Database Context populated with a Single Department
        /// </summary>
        /// <returns>A Database Context populated with a Single Department, the CS Department.</returns>
        public Team112LMSContext SingleDepartment()
        {
            Team112LMSContext Context = DefaultDatabase();
            Context.Departments.Add(new Departments
            {
                DprtName = "Computer Science",
                DprtAbv = "CS"
            }
            );
            Context.SaveChanges();
            return Context;
        }

        /// <summary>
        /// Creates 
        /// </summary>
        /// <returns></returns>
        public Team112LMSContext FewDepartments()
        {
            Team112LMSContext Context = DefaultDatabase();
            Departments[] depts =
            {
            new Departments
            {
                DprtName = "Computer Science",
                DprtAbv = "CS"
            },
            new Departments
            {
                DprtName = "History",
                DprtAbv = "HIST"
            },
            new Departments
            {
                DprtName = "Mathematics",
                DprtAbv = "MATH"
            }
            };

            foreach (Departments department in depts)
            {
                Context.Departments.Add(department);
            }

            Context.SaveChanges();
            return Context;
        }

        public Team112LMSContext ManyDepartments()
        {
            Team112LMSContext Context = DefaultDatabase();
            Departments[] depts =
            {

            new Departments
            {
                DprtName = "Computer Science",
                DprtAbv = "CS"
            },
            new Departments
            {
                DprtName = "History",
                DprtAbv = "HIST"
            },
            new Departments
            {
                DprtName = "Mathematics",
                DprtAbv = "MATH"
            },
            new Departments
            {
                DprtName = "Writing",
                DprtAbv = "WRTG"
            },
            new Departments
            {
                DprtName = "Biology",
                DprtAbv = "BIOL"
            },
            new Departments
            {
                DprtName = "Economics",
                DprtAbv = "ECON"
            },
            new Departments
            {
                DprtName = "Physics",
                DprtAbv = "PHYS"
            }
            };

            foreach (Departments department in depts)
            {
                Context.Departments.Add(department);
            }
            Context.SaveChanges();
            return Context;
        }

        /// <summary>
        /// Builds a Context with a single course mapped to a single department
        /// </summary>
        /// <returns></returns>
        public Team112LMSContext SingleCourse()
        {
            Team112LMSContext Context = SingleDepartment();
            Context.Courses.Add(new Courses
            {
                DprtAbv = "CS",
                CourseNum = 1410,
                CourseName = "Intro to Obj Oriented Programming"
            }
            );
            Context.SaveChanges();

            return Context;
        }

        /// <summary>
        /// Builds a Context with a few Courses mapped to a single department
        /// </summary>
        /// <returns></returns>
        public Team112LMSContext FewCourses()
        {
            Team112LMSContext Context = SingleDepartment();
            Courses[] Courses =
            {
                new Courses
                {
                    DprtAbv = "CS",
                    CourseNum = 1410,
                    CourseName = "Intro to Obj Oriented Programming"
                },
                new Courses
                {
                    DprtAbv = "CS",
                    CourseNum = 2420,
                    CourseName = "Intro to Algorithms and Data Structures"
                },
                new Courses
                {
                    DprtAbv = "CS",
                    CourseNum = 3500,
                    CourseName = "Software Practice I"
                }
            };
            foreach(Courses Course in Courses)
            {
                Context.Courses.Add(Course);
            }

            return Context;
        }

        /// <summary>
        /// Builds a Context with many Courses mapped to a single department
        /// </summary>
        /// <returns></returns>
        public Team112LMSContext ManyCourses()
        {
            Team112LMSContext Context = SingleDepartment();
            Courses[] Courses =
            {
                new Courses
                {
                    DprtAbv = "CS",
                    CourseNum = 1410,
                    CourseName = "Intro to Obj Oriented Programming"
                },
                new Courses
                {
                    DprtAbv = "CS",
                    CourseNum = 2100,
                    CourseName = "Discrete Structures"
                },
                new Courses
                {
                    DprtAbv = "CS",
                    CourseNum = 2420,
                    CourseName = "Intro to Algorithms and Data Structures"
                },
                new Courses
                {
                    DprtAbv = "CS",
                    CourseNum = 3500,
                    CourseName = "Software Practice I"
                },
                new Courses
                {
                    DprtAbv = "CS",
                    CourseNum = 3505,
                    CourseName = "Software Practice II"
                },
                new Courses
                {
                    DprtAbv = "CS",
                    CourseNum = 3810,
                    CourseName = "Computer Architecture"
                },
                new Courses
                {
                    DprtAbv = "CS",
                    CourseNum = 4150,
                    CourseName = "Algorithms"
                }
            };
            foreach (Courses Course in Courses)
            {
                Context.Courses.Add(Course);
            }

            return Context;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Team112LMSContext SingleUser()
        {
            Team112LMSContext Context = SingleDepartment();
            Context.Professors.Add(new Professors
            {
                FName = "Daniel",
                Lname = "Kopta",
                DprtAbv = "CS",
                UId = 297211,
                Dob = new DateTime()
            }
            );
            Context.SaveChanges();
            return Context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Team112LMSContext FewUsers()
        {
            Team112LMSContext Context = SingleDepartment();
            Context.Professors.Add(new Professors
            {
                FName = "Daniel",
                Lname = "Kopta",
                DprtAbv = "CS",
                UId = 297211,
                Dob = new DateTime()
            }
            );

            Students[] Students =
            {
                new Students
                {
                    FName = "David",
                    Lname = "Harrington",
                    DprtAbv = "CS",
                    UId = 1184803,
                    Dob = new DateTime()
                },
                new Students
                {
                    FName = "Ethan",
                    Lname = "Quinlan",
                    DprtAbv = "CS",
                    UId = 1255186,
                    Dob = new DateTime()
                }
            };

            foreach(Students student in Students)
            {
                Context.Students.Add(student);
            }
            Context.SaveChanges();
            return Context;
        }

        /// <summary>
        /// Multiple users to multiple departments
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        public Team112LMSContext FewUsers(Team112LMSContext Context)
        {
            throw new NotImplementedException("Operation Not Implemented");
        }

        /// <summary>
        /// Many users to a single department
        /// </summary>
        /// <returns></returns>
        public Team112LMSContext ManyUsers()
        {
            throw new NotImplementedException("Operation Not Implemented");
        }

        public Team112LMSContext ManyUsers(Team112LMSContext Context)
        {
            throw new NotImplementedException("Operation Not Implemented");
        }

    }
}
