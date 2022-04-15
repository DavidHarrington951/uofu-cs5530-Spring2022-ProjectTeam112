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
            Team112LMSContext Context = DefaultDepartment();
            CommonController C = new CommonController();
            C.UseLMSContext(Context);
            JsonResult result = C.GetDepartments() as JsonResult;
            Assert.True(result.Value.GetType().IsArray);
        }

        [Fact]
        public void GetDepartmentsTest1()
        {
            Team112LMSContext Context = DefaultDepartment();
            CommonController C = new CommonController();
            C.UseLMSContext(Context);
            JsonResult result = C.GetDepartments() as JsonResult;
            Object[] departments = (Object[])result.Value;
            Assert.Single(departments);
        }

        [Fact]
        public void GetDepartmentTest2()
        {
            Team112LMSContext Context = DefaultDepartment();
            CommonController C = new CommonController();
            C.UseLMSContext(Context);
            JsonResult result = C.GetDepartments() as JsonResult;
            Object[] departments = (Object[])result.Value;
            dynamic dept = departments[0];

            Assert.True(dept.name is String);
            Assert.True(dept.subject is String);
        }

        [Fact]
        public void GetDepartmentTest3()
        {
            Team112LMSContext Context = DefaultDepartment();
            CommonController C = new CommonController();
            C.UseLMSContext(Context);
            JsonResult result = C.GetDepartments() as JsonResult;
            Object[] departments = (Object[])result.Value;
            dynamic dept = departments[0];
            Assert.Equal("Computer Science", dept.name);
            Assert.Equal("CS", dept.subject);
        }

        [Fact]
        public void GetCatalog1()
        {
            Team112LMSContext Context = DefaultCourse(DefaultDepartment());
            CommonController C = new CommonController();
            C.UseLMSContext(Context);
            JsonResult result = C.GetCatalog() as JsonResult;
            Object[] departments = (Object[])result.Value;

            dynamic dept = departments[0];
            dynamic c = dept.courses;

            IEnumerable<Object> course = (IEnumerable<Object>)c;

            Assert.Single(course);
        }

        [Fact]
        public void GetClassOfferings1()
        {
            Team112LMSContext Context = DefaultClass(DefaultProfessor(DefaultCourse(DefaultDepartment())));
            CommonController C = new CommonController();
            C.UseLMSContext(Context);
            JsonResult result = C.GetClassOfferings("CS", 1410) as JsonResult;
            Object[] classes = (Object[])result.Value;
            Assert.Single(classes);
        }

        [Fact]
        public void GetAssignmentContents1()
        {
            Team112LMSContext Context = DefaultClass(DefaultProfessor(DefaultCourse(DefaultDepartment())));
            CommonController C = new CommonController();
            C.UseLMSContext(Context);
            JsonResult result = C.GetAssignmentContents("CS", 1410, "Spring", 2020, "Problem Set", "PS1") as JsonResult;
            Object[] assignments = (Object[])result.Value;
            Assert.Single(assignments);
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
        public Team112LMSContext DefaultDepartment()
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

        public Team112LMSContext DefaultCourse(Team112LMSContext Context)
        {
            Courses Course = new Courses
            {
                DprtAbv = "CS",
                CourseNum = 1410,
                CourseName = "Intro to Obj Oriented Programming"
            };
            Context.Courses.Add(Course);
            Context.SaveChanges();
            return Context;
        }

        public Team112LMSContext DefaultProfessor(Team112LMSContext Context)
        {
            Professors Professor = new Professors
            {
                DprtAbv = "CS",
                FName = "Daniel",
                Lname = "Kopta",
                Dob = new DateTime()
            };
            Context.Professors.Add(Professor);
            Context.SaveChanges();
            return Context;
        }

        public Team112LMSContext DefaultStudent(Team112LMSContext Context)
        {
            Students Student = new Students
            {
                DprtAbv = "CS",
                FName = "David",
                Lname = "Harrington",
                Dob = new DateTime()
            };
            Context.Students.Add(Student);
            Context.SaveChanges();
            return Context;
        }

        public Team112LMSContext DefaultClass(Team112LMSContext Context)
        {
            Classes Class = new Classes
            {
                CourseId = 1,
                Semester = "Spring 2020",
                Teacher = 1,
                Location = "GC 1900",
                StartTime = new DateTime(1, 1, 1, 3, 0, 0),
                EndTime = new DateTime(1, 1, 1, 4, 20, 0)
            };
            Context.Classes.Add(Class);
            Context.SaveChanges();
            return Context;
        }

        public Team112LMSContext DefaultCattegories(Team112LMSContext Context)
        {
            AssignmentCategories[] Categories =
            {
                new AssignmentCategories
                {
                    ClassId = 1,
                    CattName = "Exam",
                    GradeWeight = 40
                },
                new AssignmentCategories
                {
                    ClassId = 1,
                    CattName = "Quiz",
                    GradeWeight = 15
                },
                new AssignmentCategories
                {
                    ClassId = 1,
                    CattName = "Participation",
                    GradeWeight = 5
                },
                new AssignmentCategories
                {
                    ClassId = 1,
                    CattName = "Problem Set",
                    GradeWeight = 40
                }
            };

            foreach(AssignmentCategories Category in Categories)
            {
                Context.Add(Category);
            }
            Context.SaveChanges();
            return Context;
        }

        public Team112LMSContext DefaultAssignment(Team112LMSContext Context)
        {
            Assignments Assignment = new Assignments
            {
                CattId = 4,
                AssignName = "PS1",
                MaxPoints = 100,
                Contents = "Step 1: Profit"
            };
            Context.Assignments.Add(Assignment);
            Context.SaveChanges();
            return Context;
        }

        public Team112LMSContext DefaultEnrollment(Team112LMSContext Context)
        {
            Enrollments Enrollment = new Enrollments
            {
                UId = 1,
                ClassId = 1,
                Grade = "--"
            };
            Context.Enrollments.Add(Enrollment);
            Context.SaveChanges();
            return Context;
        }

        public Team112LMSContext DefaultSubmission(Team112LMSContext Context)
        {
            Submitted Submission = new Submitted
            {
                UId = 1,
                AssignId = 1,
                Sub = "Profit",
            };
            Context.Submitted.Add(Submission);
            Context.SaveChanges();
            return Context;
        }
    }
}
