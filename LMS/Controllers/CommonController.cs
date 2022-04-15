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
    public class CommonController : Controller
    {

        /*******Begin code to modify********/

        // TODO: Uncomment and change 'X' after you have scaffoled


        protected Team112LMSContext db;

        public CommonController()
        {
            this.db = new Team112LMSContext();
        }

        /*
         * WARNING: This is the quick and easy way to make the controller
         *          use a different LibraryContext - good enough for our purposes.
         *          The "right" way is through Dependency Injection via the constructor 
         *          (look this up if interested).
        */

        // TODO: Uncomment and change 'X' after you have scaffoled

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
            // Query grabs all department names and abreviations.
            var query =
                from d in this.db.Departments
                select new { name = d.DprtName, subject = d.DprtAbv };

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
            var query =
                from d in this.db.Departments
                select new
                {
                    subject = d.DprtAbv,
                    dname = d.DprtName,
                    courses = from c in d.Courses
                              select new { number = c.CourseNum, cname = c.CourseName }
                };

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
            var query =
                from co in this.db.Courses
                join cl in this.db.Classes
                on co.CourseId equals cl.CourseId
                where co.DprtAbv == subject && co.CourseNum == number
                select new
                {
                    season = Regex.Split(cl.Semester, " ")[0],
                    year = UInt32.Parse(Regex.Split(cl.Semester, " ")[1]),
                    location = cl.Location,
                    start = cl.StartTime.HasValue ? cl.StartTime.Value.ToString("hh:mm:ss") : "No Meeting Time",
                    end = cl.EndTime.HasValue ? cl.EndTime.Value.ToString("hh:mm:ss") : "No Meeting Time",
                    fname = cl.TeacherNavigation.FName,
                    lname = cl.TeacherNavigation.Lname
                };

            return Json(query.ToArray());
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
            String Semester = new StringBuilder(season).Append(" ").Append(year).ToString();
            IQueryable<Courses> Courses =
                from Course in this.db.Courses
                where Course.DprtAbv == subject && Course.CourseNum == (UInt32)num
                select Course;

            IEnumerable<Assignments> Assignments =
                from Course in Courses
                join Class in this.db.Classes
                on new { A = Course.CourseId, B = Semester } equals new { A = Class.CourseId, B = Class.Semester }
                into Joined1
                from j in Joined1.DefaultIfEmpty()
                join AssCatt in this.db.AssignmentCategories
                on new { C = j.ClassId, D = category } equals new { C = AssCatt.ClassId, D = AssCatt.CattName }
                into Joined2
                from k in Joined2.DefaultIfEmpty()
                join Assignment in this.db.Assignments
                on new { E = k.CattId, F = asgname } equals new { E = Assignment.CattId, F = Assignment.AssignName }
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

            return Content("");
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
        public IActionResult GetUser(String uid)
        {
            //we want the numerical (integer) form of the uNID
            UInt32 numForm = UInt32.Parse(uid.Substring(1));

            IEnumerable<Students> Student =
                from s in this.db.Students
                where s.UId == numForm
                select s;

            if (Student.Count() > 0)
            {
                Students X = Student.ElementAt(0);
                var user = new
                {
                    fname = X.FName,
                    lname = X.Lname,
                    uid = X.UId,
                    department = X.DprtAbv
                };
                return Json(user);
            }

            IEnumerable<Professors> Professor =
                from p in this.db.Professors
                where p.UId == numForm
                select p;

            if (Professor.Count() > 0)
            {
                Professors X = Professor.ElementAt(0);
                var user = new
                {
                    fname = X.FName,
                    lname = X.Lname,
                    uid = X.UId,
                    department = X.DprtAbv
                };
                return Json(user);
            }

            IEnumerable<Administrators> Admin =
                from a in this.db.Administrators
                where a.UId == numForm
                select a;

            if(Admin.Count() > 0)
            {
                Administrators X = Admin.ElementAt(0);
                var user = new
                {
                    fname = X.FName,
                    lname = X.Lname,
                    uid = X.UId
                };
                return Json(user);
            }

            return Json(new { success = false });
        }


        /*******End code to modify********/

    }
}