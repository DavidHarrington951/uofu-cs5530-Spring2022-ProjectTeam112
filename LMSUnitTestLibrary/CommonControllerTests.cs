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
            Object[] departments = (Object[]) result.Value;
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
            Team112LMSContext Context = SingleDepartmentSingleCourse();
            CommonController C = new CommonController(Context);
            JsonResult result = C.GetCatalog() as JsonResult;
            Object[] departments = (Object[])result.Value;
            dynamic dept = departments[0];

            dynamic c = dept.courses;
            dynamic k = c[0];

            Assert.Equal(1, dept.courses.Length);
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

            foreach(Departments department in depts)
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
            }
            };

            foreach(Departments department in depts)
            {
                Context.Departments.Add(department);
            }
            Context.SaveChanges();
            return Context;
        }

        /// <summary>
        /// Creates a Database Context populated with a Single Department and Single Course
        /// </summary>
        /// <returns>A Database Context populated with a Single Department, the CS Department.</returns>
        public Team112LMSContext SingleDepartmentSingleCourse()
        {
            Team112LMSContext Context = DefaultDatabase();

            Departments department = new Departments
            {
                DprtAbv = "CS",
                DprtName = "Computer Science"
            };
            Context.Departments.Add(department);

            Courses course = new Courses
            {
                CourseName = "Software Practice",
                DprtAbv = "CS",
                CourseNum = 3500
            };
            Context.Courses.Add(course);

            Context.SaveChanges();

            return Context;
        }

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
