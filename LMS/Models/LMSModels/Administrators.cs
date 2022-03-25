using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Administrators
    {
        public uint UId { get; set; }
        public string FName { get; set; }
        public string Lname { get; set; }
        public DateTime Dob { get; set; }
    }
}
