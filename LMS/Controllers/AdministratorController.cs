using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.Models.LMSModels;
using System.Text;

namespace LMS.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of all the courses in the given department.
        /// Each object in the array should have the following fields:
        /// "number" - The course number (as in 5530)
        /// "name" - The course name (as in "Database Systems")
        /// </summary>
        /// <param name="subject">The department subject abbreviation (as in "CS")</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetCourses(string subject)
        {
            IEnumerable<Object> CourseList =
                from c in this.db.Courses
                where c.DprtAbv.Equals(subject)
                select new { number = c.CourseNum, name = c.CourseName };

            return Json(CourseList.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetProfessors(string subject)
        {
            IEnumerable<Object> ProfessorsList =
                from p in this.db.Professors
                where p.DprtAbv.Equals(subject)
                select new { lname = p.Lname, fname = p.FName, uid = UnidStringFormat(p.UId) };

            return Json(ProfessorsList.ToArray());
        }



        /// <summary>
        /// Creates a course.
        /// A course is uniquely identified by its number + the subject to which it belongs
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false},
        /// false if the Course already exists.</returns>
        public IActionResult CreateCourse(string subject, int number, string name)
        {
            //create the course
            Courses C = new Courses
            {
                DprtAbv = subject,
                CourseNum = (UInt32) number,
                CourseName = name
            };

            //add it to the database
            this.db.Courses.Add(C);

            try
            {
                //try and save changes
                this.db.SaveChanges();
            }
            catch (Exception)
            {
                //return false if the course already exists
                return Json(new { success = false });
            }

            return Json(new { success = true });
        }



        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        /// false if another class occupies the same location during any time 
        /// within the start-end range in the same semester, or if there is already
        /// a Class offering of the same Course in the same Semester.</returns>
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {
            //TODO add Time Checking

            //Get the List of Courses that match the description
            IEnumerable<Courses> Courses =
                from c in this.db.Courses
                where c.DprtAbv.Equals(subject) && c.CourseNum == (UInt32)number
                select c;

            //Get the remaining data we need to create our new course
            UInt32 courseID = Courses.ElementAt(0).CourseId;
            String Semester = new StringBuilder(season).Append(" ").Append(year).ToString();
            UInt32 profID = UInt32.Parse(instructor.Substring(1));

            // Check if times overlap
            IEnumerable<Classes> ClassesAtLocationSemester =
                from cl in this.db.Classes
                where cl.Location == location && cl.Semester == Semester
                select cl;

            foreach (Classes c in ClassesAtLocationSemester)
            {
                //if end is between start and end
                if (c.StartTime.Value.TimeOfDay <= end.TimeOfDay && start.TimeOfDay <= c.EndTime.Value.TimeOfDay)
                {
                    return Json(new { success = false });
                }
            }

            Classes Class = new Classes
            {
                CourseId = courseID,
                Semester = Semester,
                Teacher = profID,
                Location = location,
                StartTime = start,
                EndTime = end
            };

            //Try and insert our class, if the class exists we return false
            try
            {
                this.db.Classes.Add(Class);
                this.db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }

            return Json(new { success = true });
        }


        /*******End code to modify********/

    }
}