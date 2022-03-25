using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Students
    {
        public Students()
        {
            Enrollments = new HashSet<Enrollments>();
            Submitted = new HashSet<Submitted>();
        }

        public uint UId { get; set; }
        public string FName { get; set; }
        public string Lname { get; set; }
        public DateTime Dob { get; set; }
        public string DprtAbv { get; set; }

        public virtual Departments DprtAbvNavigation { get; set; }
        public virtual ICollection<Enrollments> Enrollments { get; set; }
        public virtual ICollection<Submitted> Submitted { get; set; }
    }
}
