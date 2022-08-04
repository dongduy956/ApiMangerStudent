using System;
using System.Collections.Generic;

#nullable disable

namespace ApiManagerStudent.EF
{
    public partial class Student
    {
        public Student()
        {
            Points = new HashSet<Point>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Image { get; set; }
        public string Alias { get; set; }
        public int? IdClass { get; set; }

        public virtual Class IdClassNavigation { get; set; }
        public virtual ICollection<Point> Points { get; set; }
    }
}
