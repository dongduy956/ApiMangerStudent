using System;
using System.Collections.Generic;

#nullable disable

namespace ApiManagerStudent.EF
{
    public partial class Point
    {
        public int IdStudent { get; set; }
        public int IdSubject { get; set; }
        public int NumberOfTimes { get; set; }
        public double? Points { get; set; }
        public string Alias { get; set; }

        public virtual Student IdStudentNavigation { get; set; }
        public virtual Subject IdSubjectNavigation { get; set; }
    }
}
