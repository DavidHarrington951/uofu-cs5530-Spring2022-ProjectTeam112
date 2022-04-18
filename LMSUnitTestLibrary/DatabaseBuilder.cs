using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using LMS.Controllers;
using LMS.Models.LMSModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LMSUnitTestLibrary
{
    class DatabaseBuilder
    {
        /// <summary>
        /// Creates a new Empty Default Service Provider
        /// </summary>
        /// <returns></returns>
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
        public static Team112LMSContext EmptyDatabase()
        {
            var optionsBuilder = new DbContextOptionsBuilder<Team112LMSContext>();
            optionsBuilder.UseInMemoryDatabase("SmallLms").UseApplicationServiceProvider(NewServiceProvider());
            Team112LMSContext Context = new Team112LMSContext(optionsBuilder.Options);
            Context.SaveChanges();
            return Context;
        }

        /// <summary>
        /// Creates a default database for testing, 
        /// consisting of a single department, 
        /// course, professor, student, class, assignment category set, assignment, and submission
        /// </summary>
        /// <returns></returns>
        public static Team112LMSContext DefaultDatabase()
        {
            Team112LMSContext Context = EmptyDatabase();

            //** Step 1: Add a Department **//
            Context.Departments.Add(new Departments
            {
                DprtName = "Computer Science",
                DprtAbv = "CS"
            });

            Context.SaveChanges();

            //** Step 2: Add a Course, a Professor and a Student **//

            Context.Courses.Add(new Courses
            {
                DprtAbv = "CS",
                CourseNum = 1410,
                CourseName = "Intro to Obj Oriented Programming"
            });

            Context.SaveChanges();

            Context.Professors.Add(new Professors
            {
                DprtAbv = "CS",
                FName = "Daniel",
                Lname = "Kopta",
                Dob = new DateTime()
            });

            Context.SaveChanges();

            Context.Students.Add(new Students
            {
                DprtAbv = "CS",
                FName = "David",
                Lname = "Harrington",
                Dob = new DateTime()
            });

            Context.SaveChanges();

            //** Step 3: Add a new Class **//
            Context.Classes.Add(new Classes
            {
                CourseId = 1,
                Semester = "Spring 2022",
                Teacher = 1,
                Location = "GC 1900",
                StartTime = new DateTime(1, 1, 1, 3, 0, 0),
                EndTime = new DateTime(1, 1, 1, 4, 20, 0)
            });

            Context.SaveChanges();

            //** Step 4: Add Cattegories **//
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

            foreach (AssignmentCategories Category in Categories)
            {
                Context.Add(Category);
            }

            Context.SaveChanges();

            Context.Assignments.Add(new Assignments
            {
                CattId = 4,
                AssignName = "PS1",
                MaxPoints = 100,
                Contents = "Step 1: Profit"
            });

            Context.SaveChanges();

            Context.Enrollments.Add(new Enrollments
            {
                UId = 1,
                ClassId = 1,
                Grade = "--"
            });

            Context.SaveChanges();

            Context.Submitted.Add(new Submitted
            {
                UId = 1,
                AssignId = 1,
                Sub = "Profit",
            });

            Context.SaveChanges();

            return Context;
        }

        /// <summary>
        /// Populates the Default Database with more Departments
        /// </summary>
        /// <returns></returns>
        public static Team112LMSContext ManyDepartments()
        {
            Team112LMSContext Context = DefaultDatabase();
            Departments[] Departments =
            {
                new Departments
                {
                    DprtAbv = "HIST",
                    DprtName = "History"
                },
                new Departments
                {
                    DprtAbv = "MATH",
                    DprtName = "Mathematics"
                },
                new Departments
                {
                    DprtAbv = "PHYS",
                    DprtName = "Physics"
                }
            };

            foreach (Departments Department in Departments)
            {
                Context.Add(Department);
            }

            Context.SaveChanges();
            return Context;
        }

        /// <summary>
        /// Populates the Test DB with more Courses for the CS Department
        /// </summary>
        /// <returns></returns>
        public static Team112LMSContext SingleManyCourses()
        {
            Team112LMSContext Context = DefaultDatabase();

            Courses[] Courses =
            {
                new Courses
                {
                    DprtAbv = "CS",
                    CourseNum = 2420,
                    CourseName = "Intro to Algorithms and Data Structures"
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
                    CourseNum = 3500,
                    CourseName = "Software Practice I"
                }
            };

            foreach(Courses Course in Courses)
            {
                Context.Courses.Add(Course);
            }

            Context.SaveChanges();
            return Context;
        }

        /// <summary>
        /// Populates the Test DB with more Classes for CS 1410 for the CS Department
        /// </summary>
        /// <returns></returns>
        public static Team112LMSContext SingleManyClasses()
        {
            Team112LMSContext Context = DefaultDatabase();

            Classes[] offerings =
            {
                new Classes
                {
                    CourseId = 1,
                    Semester = "Spring 2021",
                    Teacher = 1,
                    Location = "GC 1900",
                    StartTime = new DateTime(1, 1, 1, 3, 0, 0),
                    EndTime = new DateTime(1, 1, 1, 4, 20, 0)
                },
                new Classes
                {
                    CourseId = 1,
                    Semester = "Spring 2020",
                    Teacher = 1,
                    Location = "GC 1900",
                    StartTime = new DateTime(1, 1, 1, 3, 0, 0),
                    EndTime = new DateTime(1, 1, 1, 4, 20, 0)
                },
                new Classes
                {
                    CourseId = 1,
                    Semester = "Spring 2019",
                    Teacher = 1,
                    Location = "GC 1900",
                    StartTime = new DateTime(1, 1, 1, 3, 0, 0),
                    EndTime = new DateTime(1, 1, 1, 4, 20, 0)
                }
            };

            foreach (Classes offering in offerings)
            {
                Context.Classes.Add(offering);
            }

            Context.SaveChanges();

            return Context;
        }

        /// <summary>
        /// Creates a CommonController Based on the Default Database
        /// </summary>
        /// <returns></returns>
        public static CommonController DefaultCommonController()
        {
            return CommonController(DefaultDatabase());
        }

        public static CommonController CommonController(Team112LMSContext Context)
        {
            CommonController Controller = new CommonController();
            Controller.UseLMSContext(Context);
            return Controller;
        }

        public static AccountController DefaultAccountController()
        {
            AccountController Controller = new AccountController(null, null, null, null);

            throw new NotImplementedException();
        }


    }
}
