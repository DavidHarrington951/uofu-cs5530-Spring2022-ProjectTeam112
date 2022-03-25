using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professors
    {
        public Professors()
        {
            Classes = new HashSet<Classes>();
        }

        public uint UId { get; set; }
        public string FName { get; set; }
        public string Lname { get; set; }
        public DateTime Dob { get; set; }
        public string DprtAbv { get; set; }

        public virtual Departments DprtAbvNavigation { get; set; }
        public virtual ICollection<Classes> Classes { get; set; }
    }
}
