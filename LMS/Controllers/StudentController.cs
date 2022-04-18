using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : CommonController
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            // Perform Variable Type Conversion
            UInt32 uNID = UInt32.Parse(uid.Substring(1));

            IEnumerable<Object> Classes =
                from Enrollment in this.db.Enrollments
                where Enrollment.UId == uNID
                join Class in this.db.Classes
                on Enrollment.ClassId equals Class.ClassId
                into Joined1

                from element1 in Joined1
                join Course in this.db.Courses
                on element1.CourseId equals Course.CourseId
                select new
                {
                    subject = Course.DprtAbv,
                    number = Course.CourseNum,
                    name = Course.CourseName,
                    season = Regex.Split(element1.Semester, " ")[0],
                    year = Regex.Split(element1.Semester, " ")[1],
                    grade = Enrollment.Grade

                };
            return Json(Classes.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            UInt32 uNID = UInt32.Parse(uid.Substring(1));
            String Semester = new StringBuilder(season).Append(" ").Append(year).ToString();

            IQueryable<UInt32> ClassIDs =
                from Course in this.db.Courses
                where Course.DprtAbv.Equals(subject) && Course.CourseNum == (UInt32)num
                join Class in this.db.Classes
                on new { Id = Course.CourseId, F = Semester } equals new { Id = Class.CourseId, F = Class.Semester }
                into Joined1

                from element1 in Joined1
                join Enrollment in this.db.Enrollments
                on new { ID = element1.ClassId, F = uNID } equals new { ID = Enrollment.ClassId, F = Enrollment.UId }
                into Joined2
                from element2 in Joined2
                select element2.ClassId;

            IEnumerable<Object> Assignments =
                from C in ClassIDs
                join Category in this.db.AssignmentCategories
                on C equals Category.ClassId
                into Joined1

                from element1 in Joined1
                join Assignment in this.db.Assignments
                on element1.CattId equals Assignment.CattId
                into Joined2

                from element2 in Joined2
                join Submission in this.db.Submitted
                on element2.AssignId equals Submission.AssignId
                into Joined3

                from element3 in Joined3
                select new
                {
                    aname = element2.AssignName,
                    cname = element1.CattName,
                    due = element2.DueDate.HasValue ? element2.DueDate.ToString() : "",
                    score = element3.Sub == null ? element3.Sub : null
                };

            return Json(Assignments.ToArray());
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// Does *not* automatically reject late submissions.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}.</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {

            return Json(new { success = false });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false},
        /// false if the student is already enrolled in the Class.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            // Perform Variable Type Conversion
            UInt32 uNID = UInt32.Parse(uid.Substring(1));
            String Semester = new StringBuilder(season).Append(" ").Append(year).ToString();

            IEnumerable<UInt32> Class =
                from Course in this.db.Courses
                join Offering in this.db.Classes
                on Course.CourseId equals Offering.CourseId
                into Joined

                from element in Joined
                where Course.DprtAbv == subject && Course.CourseNum == num && element.Semester.Equals(Semester)
                select element.ClassId;

            UInt32 cID = Class.ElementAt(0);

            try
            {
                this.db.Enrollments.Add(new Enrollments
                {
                    ClassId = cID,
                    UId = uNID,
                    Grade = "--"
                });
                this.db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }

            return Json(new { success = true });
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student does not have any grades, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            UInt32 uNID = UInt32.Parse(uid.Substring(1));

            IEnumerable<String> Grades =
                from Enrollment in this.db.Enrollments
                where Enrollment.UId == uNID && !Enrollment.Grade.Equals("--")
                select Enrollment.Grade;

            Double GPA = ConvertGPA(Grades);

            return Json(new { gpa = GPA });
        }

        /// <summary>
        /// Converts list of a student's Grades to a Grade Point Average
        /// </summary>
        /// <param name="GradeList"></param>
        /// <returns></returns>
        public Double ConvertGPA(IEnumerable<String> GradeList)
        {
            if(GradeList.Count() == 0)
            {
                return 0.0;
            }

            Double Sum = 0.0;
            foreach(String Grade in GradeList)
            {
                if (Grade.Equals("A"))
                {
                    Sum += 4.0;
                }
                else if (Grade.Equals("A-"))
                {
                    Sum += 3.7;
                }
                else if (Grade.Equals("B+"))
                {
                    Sum += 3.3;
                }
                else if (Grade.Equals("B"))
                {
                    Sum += 3.0;
                }
                else if (Grade.Equals("B-"))
                {
                    Sum += 2.7;
                }
                else if (Grade.Equals("C+"))
                {
                    Sum += 2.3;
                }
                else if (Grade.Equals("C"))
                {
                    Sum += 2.0;
                }
                else if (Grade.Equals("C-"))
                {
                    Sum += 1.7;
                }
                else if (Grade.Equals("D+"))
                {
                    Sum += 1.3;
                }
                else if (Grade.Equals("D"))
                {
                    Sum += 1.0;
                }
                else if (Grade.Equals("D-"))
                {
                    Sum += .7;
                }
                else if (Grade.Equals("E"))
                {
                    Sum += 0.0;
                }
            }

            Double GPA = Sum / GradeList.Count();

            return GPA;
        }

        /*******End code to modify********/

    }
}