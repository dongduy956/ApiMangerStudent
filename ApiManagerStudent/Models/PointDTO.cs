using ApiManagerStudent.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiManagerStudent.Models
{
    public class PointDTO
    {
        public PointDTO()
        {

        }
        public PointDTO(Point p)
        {
            this.IdStudent = p.IdStudent;
            this.IdSubject = p.IdSubject;
            this.NumberOfTimes = p.NumberOfTimes;
            this.Points = p.Points;
            this.Alias = p.Alias;
        }
        public int IdStudent { get; set; }
        public int IdSubject { get; set; }
        public int NumberOfTimes { get; set; }
        public double? Points { get; set; }
        public string Alias { get; set; }

    }
}
