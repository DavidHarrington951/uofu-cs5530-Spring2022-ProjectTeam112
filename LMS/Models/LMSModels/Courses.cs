using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Courses
    {
        public Courses()
        {
            Classes = new HashSet<Classes>();
        }

        public uint CourseId { get; set; }
        public string DprtAbv { get; set; }
        public uint CourseNum { get; set; }
        public string CourseName { get; set; }

        public virtual Departments DprtAbvNavigation { get; set; }
        public virtual ICollection<Classes> Classes { get; set; }
    }
}
