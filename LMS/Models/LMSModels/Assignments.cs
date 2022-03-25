using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignments
    {
        public Assignments()
        {
            Submitted = new HashSet<Submitted>();
        }

        public uint AssignId { get; set; }
        public string AssignName { get; set; }
        public uint CattId { get; set; }
        public DateTime? DueDate { get; set; }
        public uint MaxPoints { get; set; }
        public string Contents { get; set; }

        public virtual AssignmentCategories Catt { get; set; }
        public virtual ICollection<Submitted> Submitted { get; set; }
    }
}
