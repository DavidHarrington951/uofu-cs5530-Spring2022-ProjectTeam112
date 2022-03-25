using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Classes
    {
        public Classes()
        {
            AssignmentCategories = new HashSet<AssignmentCategories>();
            Enrollments = new HashSet<Enrollments>();
        }

        public uint ClassId { get; set; }
        public uint CourseId { get; set; }
        public string Semester { get; set; }
        public uint Teacher { get; set; }
        public string Location { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public virtual Courses Course { get; set; }
        public virtual Professors TeacherNavigation { get; set; }
        public virtual ICollection<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual ICollection<Enrollments> Enrollments { get; set; }
    }
}
