using ApiManagerStudent.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiManagerStudent.Models
{
    public class SubjectDTO
    {
        public SubjectDTO() { }
        public SubjectDTO(Subject s) {
            this.Id = s.Id;
            this.Name = s.Name;
            this.Alias = s.Alias;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}
