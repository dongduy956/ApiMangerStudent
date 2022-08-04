using ApiManagerStudent.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiManagerStudent.Models
{
    public class RoleDTO
    {
        public RoleDTO() { }
        public RoleDTO(Role r) {
            Id = r.Id;
            Name = r.Name;
            Alias = r.Alias;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}
