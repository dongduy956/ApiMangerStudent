using ApiManagerStudent.EF;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiManagerStudent.Models
{
    public class ClassDTO
    {
        public ClassDTO()
        {

        }
        public ClassDTO(Class _class)
        {
            this.Id = _class.Id;
            this.Name = _class.Name;
            this.Alias = _class.Alias;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}
