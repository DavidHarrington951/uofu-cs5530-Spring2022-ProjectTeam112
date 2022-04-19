using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submitted
    {
        public uint SubId { get; set; }
        public uint UId { get; set; }
        public uint AssignId { get; set; }
        public string Sub { get; set; }
        public uint? Score { get; set; }
        public DateTime? SubTime { get; set; }

        public virtual Assignments Assign { get; set; }
        public virtual Students U { get; set; }
    }
}
