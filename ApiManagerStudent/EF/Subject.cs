using System;
using System.Collections.Generic;

#nullable disable

namespace ApiManagerStudent.EF
{
    public partial class Subject
    {
        public Subject()
        {
            Points = new HashSet<Point>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }

        public virtual ICollection<Point> Points { get; set; }
    }
}
