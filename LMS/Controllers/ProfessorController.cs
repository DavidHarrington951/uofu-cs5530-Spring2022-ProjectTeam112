using System;
using System.Collections;
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
    //Author: Prof Daniel Kopta
    // Modified by: David Harrington and Ethan Quinlan
    [Authorize(Roles = "Professor")]
    public class ProfessorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var ClassID =
                from co in this.db.Courses
                join cl in this.db.Classes on co.CourseId equals cl.CourseId
                where co.DprtAbv == subject && co.CourseNum == num
                && Regex.Split(cl.Semester, " ")[0] == season && UInt32.Parse(Regex.Split(cl.Semester, " ")[1]) == year
                select cl;

            var Enrolled =
                from c in ClassID
                join e in this.db.Enrollments
                on c.ClassId equals e.ClassId
                select e;

            var StudentsInClass =
                from e in Enrolled
                join s in this.db.Students
                on e.UId equals s.UId
                select new { fname = s.FName, lname = s.Lname, uid = UnidStringFormat(s.UId), dob = s.Dob, grade = e.Grade };

            return Json(StudentsInClass.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            if (category == null)
            {
                String Semester = new StringBuilder(season).Append(" ").Append(year).ToString();
                IEnumerable<Object> x
                    = from course in this.db.Courses
                      where course.DprtAbv.Equals(subject) && course.CourseNum == num
                      join Class in this.db.Classes
                      on course.CourseId equals Class.CourseId
                      into Joined1

                      from element1 in Joined1
                      where element1.Semester.Equals(Semester)
                      join AssignmentCategory in this.db.AssignmentCategories
                      on element1.ClassId equals AssignmentCategory.ClassId
                      into Joined2

                      from element2 in Joined2
                      join Assignment in this.db.Assignments
                      on element2.CattId equals Assignment.CattId
                      into Joined3

                      from element3 in Joined3
                      select new
                      {
                          aname = element3.AssignName,
                          cname = element2.CattName,
                          due = element3.DueDate,
                          submissions = ((IEnumerable<Object>)(
                          from submission in this.db.Submitted
                          where submission.AssignId == element3.AssignId
                          select submission)).Count()
                      };

                return Json(x.ToArray());
            }
            else
            {
                String Semester = new StringBuilder(season).Append(" ").Append(year).ToString();
                IEnumerable<Object> x
                    = from course in this.db.Courses
                      where course.DprtAbv.Equals(subject) && course.CourseNum == num
                      join Class in this.db.Classes
                      on course.CourseId equals Class.CourseId
                      into Joined1

                      from element1 in Joined1
                      where element1.Semester.Equals(Semester)
                      join AssignmentCategory in this.db.AssignmentCategories
                      on element1.ClassId equals AssignmentCategory.ClassId
                      into Joined2

                      from element2 in Joined2
                      where element2.CattName.Equals(category)
                      join Assignment in this.db.Assignments
                      on element2.CattId equals Assignment.CattId
                      into Joined3

                      from element3 in Joined3
                      select new
                      {
                          aname = element3.AssignName,
                          cname = element2.CattName,
                          due = element3.DueDate,
                          submissions = ((IEnumerable<Object>)(
                          from submission in this.db.Submitted
                          where submission.AssignId == element3.AssignId
                          select submission)).Count()
                      };

                return Json(x.ToArray());
            }
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var query =
                from co in this.db.Courses
                join cl in this.db.Classes
                on co.CourseId equals cl.CourseId
                where co.DprtAbv == subject && co.CourseNum == num
                join ac in this.db.AssignmentCategories
                on cl.ClassId equals ac.ClassId
                where Regex.Split(cl.Semester, " ")[0] == season
                && UInt32.Parse(Regex.Split(cl.Semester, " ")[1]) == year
                select new { name = ac.CattName, weight = ac.GradeWeight };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false},
        ///	false if an assignment category with the same name already exists in the same class.</returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            IEnumerable<Courses> Courses =
                from c in this.db.Courses
                where c.DprtAbv.Equals(subject) && c.CourseNum == (UInt32)num
                select c;

            UInt32 courseID = Courses.ElementAt(0).CourseId;

            IEnumerable<Classes> Classes =
                from cl in this.db.Classes
                where cl.CourseId == courseID
                && Regex.Split(cl.Semester, " ")[0] == season
                && UInt32.Parse(Regex.Split(cl.Semester, " ")[1]) == year
                select cl;

            UInt32 classID = Classes.ElementAt(0).ClassId;

            AssignmentCategories ac = new AssignmentCategories
            {
                CattName = category,
                ClassId = classID,
                GradeWeight = (UInt32)catweight

            };

            //Try and insert our class, if the class exists we return false
            try
            {
                this.db.AssignmentCategories.Add(ac);
                this.db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }

            return Json(new { success = true });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false,
        /// false if an assignment with the same name already exists in the same assignment category.</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            IEnumerable<AssignmentCategories> query =
                from co in this.db.Courses
                join cl in this.db.Classes
                on co.CourseId equals cl.CourseId
                where co.DprtAbv == subject && co.CourseNum == num
                join ac in this.db.AssignmentCategories
                on cl.ClassId equals ac.ClassId
                where Regex.Split(cl.Semester, " ")[0] == season
                && UInt32.Parse(Regex.Split(cl.Semester, " ")[1]) == year
                && ac.CattName == category
                select ac;

            UInt32 cattID = query.ElementAt(0).CattId;

            Assignments Assign = new Assignments
            {
                AssignName = asgname,
                CattId = cattID,
                DueDate = asgdue,
                MaxPoints = (UInt32)asgpoints,
                Contents = asgcontents,

            };

            //Try and insert our class, if the class exists we return false
            try
            {
                this.db.Assignments.Add(Assign);
                this.db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }

            return Json(new { success = true });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            //Perform Parameter Variable Conversion
            String Semester = new StringBuilder(season).Append(" ").Append(year).ToString();

            IEnumerable<Object> Submissions =
                from Course in this.db.Courses
                where Course.DprtAbv.Equals(subject) && Course.CourseNum == (UInt32)num
                join Class in this.db.Classes
                on new { ID = Course.CourseId, F = Semester } equals new { ID = Class.CourseId, F = Class.Semester }
                into Joined1

                from element1 in Joined1
                join Category in this.db.AssignmentCategories
                on new { ID = element1.ClassId, F = category } equals new { ID = Category.ClassId, F = Category.CattName }
                into Joined2

                from element2 in Joined2
                join Assignment in this.db.Assignments
                on new { ID = element2.CattId, F = asgname } equals new { ID = Assignment.CattId, F = Assignment.AssignName }
                into Joined3

                from element3 in Joined3
                join Submission in this.db.Submitted
                on element3.AssignId equals Submission.AssignId
                into Joined4

                from element4 in Joined4
                join Student in this.db.Students
                on element4.UId equals Student.UId
                select new
                {
                    fname = Student.FName,
                    lname = Student.Lname,
                    uid = UnidStringFormat(Student.UId),
                    time = element4.SubTime.HasValue ? element4.SubTime.Value.ToString() : "Not Submitted",
                    score = element4.Score
                };

            return Json(Submissions.ToArray());
        }

        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            //Perform Parameter Variable and Type Conversion
            UInt32 uNID = UInt32.Parse(uid.Substring(1));
            String Semester = new StringBuilder(season).Append(" ").Append(year).ToString();

            var Submissions =
                from Course in this.db.Courses
                where Course.DprtAbv.Equals(subject) && Course.CourseNum == (UInt32)num
                join Class in this.db.Classes
                on new { ID = Course.CourseId, F = Semester } equals new { ID = Class.CourseId, F = Class.Semester }
                into Joined1

                from element1 in Joined1
                join Category in this.db.AssignmentCategories
                on new { ID = element1.ClassId, F = category } equals new { ID = Category.ClassId, F = Category.CattName }
                into Joined2

                from element2 in Joined2
                join Assignment in this.db.Assignments
                on new { ID = element2.CattId, F = asgname } equals new { ID = Assignment.CattId, F = Assignment.AssignName }
                into Joined3

                from element3 in Joined3
                join Sub in this.db.Submitted
                on new { ID = element3.AssignId, F = uNID } equals new { ID = Sub.AssignId, F = Sub.UId }
                select new
                {
                    sub = Sub,
                    cID = element1.ClassId
                };



            Submissions.ElementAt(0).sub.Score = (UInt32)score;

            UInt32 cID = Submissions.ElementAt(0).cID;

            //Try submitting changes to the database
            try
            {
                this.db.SaveChanges();

                //update the grade in the class
                UpdateGrade(uNID, cID);
            }

            //if the changes fail, return false
            catch (Exception)
            {
                return Json(new { success = false });
            }

            return Json(new { success = true });
        }

        public void UpdateGrade(UInt32 uID, UInt32 cID)
        {
            IEnumerable<AssignmentCategories> Categories =
                from Category in this.db.AssignmentCategories
                where Category.ClassId == cID
                select Category;

            Double total = 0;
            UInt32 WeightTotal = 0;

            foreach (AssignmentCategories Category in Categories)
            {
                UInt32 id = Category.CattId;

                IEnumerable<Assignments> Assignments =
                    from Assignment in this.db.Assignments
                    where Assignment.CattId == id
                    select Assignment;

                if (Assignments.Count() > 0)
                {
                    UInt32 weight = Category.GradeWeight;

                    WeightTotal += weight;

                    Double ScoreTotal = 0;
                    // divide by
                    Double AssignmentTotal = 0;

                    foreach (Assignments Assignment in Assignments)
                    {
                        Submitted Submission = this.db.Submitted.Find(uID, Assignment.AssignId);

                        AssignmentTotal += Assignment.MaxPoints;
                        ScoreTotal += Submission.Score;
                    }

                    Double percent_unscaled = ScoreTotal / AssignmentTotal;

                    Double Scale = percent_unscaled * weight;

                    total += Scale;
                }
            }

            Double factor = 100 / WeightTotal;

            Double Grade = factor * total;

            //convert Grade to Letter
            String Letter = this.toLetterGrade(Grade);

            //step n
            IEnumerable<Enrollments> Enrollment =
                from Enroll in this.db.Enrollments
                where Enroll.ClassId == cID && Enroll.UId == uID
                select Enroll;

            Enrollment.ElementAt(0).Grade = Letter;

            this.db.SaveChanges();
        }

        public String toLetterGrade(Double Percentage)
        {
            if (Percentage >= 93)
            {
                return "A";
            }
            else if (Percentage < 93 && Percentage > 90)
            {
                return "A-";
            }
            else if (Percentage < 90 && Percentage > 87)
            {
                return "B+";
            }
            else if (Percentage < 87 && Percentage > 83)
            {
                return "B";
            }
            else if (Percentage < 83 && Percentage > 80)
            {
                return "B-";
            }
            else if (Percentage < 80 && Percentage > 77)
            {
                return "C+";
            }
            else if (Percentage < 77 && Percentage > 73)
            {
                return "C";
            }
            else if (Percentage < 73 && Percentage > 70)
            {
                return "C-";
            }
            else if (Percentage < 70 && Percentage > 67)
            {
                return "D+";
            }
            else if (Percentage < 67 && Percentage > 63)
            {
                return "D";
            }
            else if (Percentage < 63 && Percentage > 60)
            {
                return "D-";
            }
            else
            {
                return "E";
            }

        }



        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            // Perform Parameter Type Conversion
            UInt32 uNID = UInt32.Parse(uid.Substring(1));

            // Get the list of the Professor's Classes
            IEnumerable<Object> Classes =
                //join Courses with Classes on the Filter CourseID and uNID == Teacher
                from Course in this.db.Courses
                join Class in this.db.Classes
                on new { ID = Course.CourseId, F = uNID } equals new { ID = Class.CourseId, F = Class.Teacher }

                //select our values from our table row
                select new
                {
                    subject = Course.DprtAbv,
                    number = Course.CourseNum,
                    name = Course.CourseName,
                    season = Regex.Split(Class.Semester, " ")[0],
                    year = Regex.Split(Class.Semester, " ")[1],
                };

            //Convert to an array and return as JsonResult
            return Json(Classes.ToArray());
        }
    }
}