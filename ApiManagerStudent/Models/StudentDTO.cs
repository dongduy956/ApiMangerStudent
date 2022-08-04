using ApiManagerStudent.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiManagerStudent.Models
{
    public class StudentDTO
    {
        public StudentDTO()
        {

        }
        public StudentDTO(Student s)
        {
            Id = s.Id;
            Name = s.Name;
            DateOfBirth = s.DateOfBirth;
            Image = s.Image;
            Alias = s.Alias;
            IdClass = s.IdClass;
        }
        
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Image { get; set; }
        public string Alias { get; set; }
        public int? IdClass { get; set; }
    }
}
