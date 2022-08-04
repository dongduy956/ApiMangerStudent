using System;
using System.Collections.Generic;

#nullable disable

namespace ApiManagerStudent.EF
{
    public partial class Class
    {
        public Class()
        {
            Students = new HashSet<Student>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }

        public virtual ICollection<Student> Students { get; set; }
    }
}
