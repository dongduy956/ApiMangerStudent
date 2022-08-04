using ApiManagerStudent.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiManagerStudent.Models
{
    public class TeacherDTO
    {
        public TeacherDTO() { }
        public TeacherDTO(Teacher t) {
            this.Id = t.Id;
            this.Name = t.Name;
            this.DateOfBirth = t.DateOfBirth;
            this.Image = t.Image;
            this.Password = t.Password;
            this.IdRole = t.IdRole;
            this.Username = t.Username;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Image { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int? IdRole { get; set; }
    }
}
