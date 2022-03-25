﻿using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Enrollments
    {
        public uint EId { get; set; }
        public uint UId { get; set; }
        public uint ClassId { get; set; }
        public string Grade { get; set; }

        public virtual Classes Class { get; set; }
        public virtual Students U { get; set; }
    }
}
