using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("LMSUnitTestLibrary")]

namespace LMS.Controllers
{
    //Author: Prof Daniel Kopta
    // Modified by: David Harrington and Ethan Quinlan
    /// <summary>
    /// Inherits from Controller, a base Controller class for all other Controllers to Inherit from. 
    /// Contains basic Query Functionality. 
    /// </summary>
    public class CommonController : Controller
    {
        protected Team112LMSContext db;

        /// <summary>
        /// Default Constructor, initializes class with Database Context
        /// </summary>
        public CommonController()
        {
            this.db = new Team112LMSContext();
        }

        //TODO: learn how to use Dependency Injection
        /// <summary>
        /// Set the Database Context to the Context we wish to use. 
        /// </summary>
        /// <param name="ctx"></param>
        public void UseLMSContext(Team112LMSContext ctx)
        {
            db = ctx;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {
            //Get the List of all Departments in our Database
            IEnumerable<Object> query =
                //foreach Department in our Database
                from Department in this.db.Departments
                select new { name = Department.DprtName, subject = Department.DprtAbv };

            //Convert to an array and return
            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 5530)
        ///            "cname": The course name (e.g. "Database Systems")
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetCatalog()
        {
            //Get the List of Courses Associated with each Department in our Database
            IEnumerable<Object> query =
                //foreach Department in our Database
                from Department in this.db.Departments

                //Get requested data from it 
                select new
                {
                    subject = Department.DprtAbv,
                    dname = Department.DprtName,
                    courses = from c in Department.Courses
                              select new { number = c.CourseNum, cname = c.CourseName }
                };

            //Convert to an Array and return
            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject">The subject abbreviation, as in "CS"</param>
        /// <param name="number">The course number, as in 5530</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {
            //Get the List of Course Offerings that Match our Filter
            IEnumerable<Object> Offerings =
                // join Courses and Classes on CourseID
                from Course in this.db.Courses
                join Class in this.db.Classes
                on Course.CourseId equals Class.CourseId

                // select where DprtAbv = value and 
                where Course.DprtAbv == subject && Course.CourseNum == (UInt32)number
                select new
                {
                    season = Regex.Split(Class.Semester, " ")[0],
                    year = UInt32.Parse(Regex.Split(Class.Semester, " ")[1]),
                    location = Class.Location,
                    start = Class.StartTime.HasValue ? Class.StartTime.Value.ToString("hh:mm:ss") : "No Meeting Time",
                    end = Class.EndTime.HasValue ? Class.EndTime.Value.ToString("hh:mm:ss") : "No Meeting Time",
                    fname = Class.TeacherNavigation.FName,
                    lname = Class.TeacherNavigation.Lname
                };

            //Convert the IEnumerable to an Array
            return Json(Offerings.ToArray());
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <returns>The assignment contents</returns>
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {
            //Perform Variable Type Conversion
            String Semester = new StringBuilder(season).Append(" ").Append(year).ToString();

            IEnumerable<Assignments> Assignments =
                //select the course where it's dprt and coursenum meet requirements
                from Course in this.db.Courses
                where Course.DprtAbv == subject && Course.CourseNum == (UInt32)num

                //join classes on courseId and Semester
                join offering in this.db.Classes
                on new { A = Course.CourseId, B = Semester } equals new { A = offering.CourseId, B = offering.Semester }
                into Join1

                //join categories on classID and category name
                from element1 in Join1
                join Cattegory in this.db.AssignmentCategories
                on new { A = element1.ClassId, B = category } equals new { A = Cattegory.ClassId, B = Cattegory.CattName }
                into Join2

                //join assignments on catID and assignment name
                from element2 in Join2
                join Assignment in this.db.Assignments
                on new { A = element2.CattId, B = asgname } equals new { A = Assignment.CattId, B = Assignment.AssignName }
                select Assignment;

            return Content(Assignments.ElementAt(0).Contents);
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// Returns the empty string ("") if there is no submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {
            //Perform Variable Type Conversion
            UInt32 uNID = UInt32.Parse(uid.Substring(1));
            String Semester = new StringBuilder(season).Append(" ").Append(year).ToString();

            //query for the submission
            IEnumerable<Submitted> Submissions =
                //select the course where it's dprt and coursenum meet requirements
                from Course in this.db.Courses
                where Course.DprtAbv == subject && Course.CourseNum == (UInt32)num
                
                //join classes on courseId and Semester
                join offering in this.db.Classes
                on new { A = Course.CourseId, B = Semester } equals new { A = offering.CourseId, B = offering.Semester }
                into Join1

                //join categories on classID and category name
                from element1 in Join1
                join Category in this.db.AssignmentCategories
                on new { A = element1.ClassId, B = category } equals new { A = Category.ClassId, B = Category.CattName }
                into Join2

                //join assignments on catID and assignment name
                from element2 in Join2
                join Assignment in this.db.Assignments
                on new { A = element2.CattId, B = asgname } equals new { A = Assignment.CattId, B = Assignment.AssignName }
                into Join3

                //join submissions on assignmentID and uNID
                from element3 in Join3
                join Submission in this.db.Submitted
                on new { A = element3.AssignId, B = uNID } equals new { A = Submission.AssignId, B = Submission.UId }
                select Submission;

            //if the query returned a value
            if(Submissions.Count() > 0)
            {
                //return that value
                return Content(Submissions.ElementAt(0).Sub);
            }

            //else return an empty string
            else
            {
                return Content("");
            }
        }

        /// <summary>
        /// Gets information about a user as a single JSON object.
        /// The object should have the following fields:
        /// "fname": the user's first name
        /// "lname": the user's last name
        /// "uid": the user's uid
        /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
        ///               If the user is a Professor, this is the department they work in.
        ///               If the user is a Student, this is the department they major in.    
        ///               If the user is an Administrator, this field is not present in the returned JSON
        /// </summary>
        /// <param name="uid">The ID of the user</param>
        /// <returns>
        /// The user JSON object 
        /// or an object containing {success: false} if the user doesn't exist
        /// </returns>
        public IActionResult GetUser(string uid)
        {
            //Perform Parameter Type Conversion
            UInt32 uNID = UInt32.Parse(uid.Substring(1));

            //query each user table, one after the other until we get what we want, then
            // we return

            //Foreach Student, select if they equal our uNID
            IEnumerable<Students> Students =
                from Student in this.db.Students
                where Student.UId == uNID
                select Student;

            if (Students.Count() > 0)
            {
                Students X = Students.ElementAt(0);
                var user = new
                {
                    fname = X.FName,
                    lname = X.Lname,
                    uid = UnidStringFormat(X.UId),
                    department = X.DprtAbv
                };
                return Json(user);
            }

            //Foreach Professor, select if they equal our uNID
            IEnumerable<Professors> Professors =
                from Professor in this.db.Professors
                where Professor.UId == uNID
                select Professor;

            if (Professors.Count() > 0)
            {
                Professors X = Professors.ElementAt(0);
                var user = new
                {
                    fname = X.FName,
                    lname = X.Lname,
                    uid = UnidStringFormat(X.UId),
                    department = X.DprtAbv
                };
                return Json(user);
            }

            //Foreach Admin, select if they equal our uNID
            IEnumerable<Administrators> Admins =
                from Admin in this.db.Administrators
                where Admin.UId == uNID
                select Admin;

            if (Admins.Count() > 0)
            {
                Administrators X = Admins.ElementAt(0);
                var user = new
                {
                    fname = X.FName,
                    lname = X.Lname,
                    uid = UnidStringFormat(X.UId)
                };
                return Json(user);
            }

            //After we have gone through each user table
            // if the user doesn't exist, return false.
            return Json(new { success = false });
        }

        /// <summary>
        /// Converts a provided uNID from UInt32 into a string formatted as u0000000
        /// </summary>
        /// <param name="uNID"></param>
        /// <returns></returns>
        internal String UnidStringFormat(UInt32 uNID)
        {
            StringBuilder format = new StringBuilder("u");

            String numString = uNID.ToString();
            int numZero = 7 - numString.Length;
            for (int i = 0; i < numZero; i++)
            {
                format.Append("0");
            }
            format.Append(numString);
            return format.ToString();
        }
    }
}