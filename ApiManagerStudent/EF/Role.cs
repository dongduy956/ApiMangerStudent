using System;
using System.Collections.Generic;

#nullable disable

namespace ApiManagerStudent.EF
{
    public partial class Role
    {
        public Role()
        {
            Teachers = new HashSet<Teacher>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }

        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}
