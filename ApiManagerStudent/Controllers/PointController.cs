using ApiManagerStudent.EF;
using ApiManagerStudent.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiManagerStudent.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [EnableCors("MyPolicy")]
    [Authorize]

    public class PointController : ControllerBase
    {
        private readonly ManagerStudentContext db;
        public PointController(ManagerStudentContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pagesize = 5)
        {
            var list = new List<PointDTO>();
            await db.Points.Skip((page - 1) * pagesize).Take(pagesize)
                .ForEachAsync(x => list.Add(new PointDTO(x)));
            return new ObjectResult(new
            {
                data = list,
                page = page,
                pagesize = pagesize,
                totalPage = Math.Ceiling(db.Points.Count() / (float)pagesize),
                totalItems = db.Points.Count()
            });
        }


        [HttpGet("{idStudent}&{idSubject}&{numberOfTimes}")]
        [ProducesResponseType(typeof(PointDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByID(int idStudent, int idSubject, int numberOfTimes)
        {
            var point = await db.Points.FindAsync(idStudent, idSubject, numberOfTimes);
            return point == null ? NotFound(new
            {
                error = "Point object not found by id to get."
            }) : Ok(new PointDTO(point));
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search(string q, int page = 1, int pagesize = 5)
        {
            try
            {
                q = q.ToLower().Trim();

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "Invalid query."
                });
            }
            var list = new List<PointDTO>();
            var points = db.Points.Where(x =>
             x.IdStudentNavigation.Name.ToLower().Trim().Contains(q) ||
             x.IdSubjectNavigation.Name.ToLower().Trim().Contains(q) ||
             x.Points.ToString().Trim().Equals(q)
              );
           await points.Skip((page - 1) * pagesize).Take(pagesize)
                .ForEachAsync(x => list.Add(new PointDTO(x)));
            return new ObjectResult(new
            {
                data = list,
                page = page,
                pagesize = pagesize,
                totalPage = Math.Ceiling(db.Points.Count() / (float)pagesize),
                totalItems = points.Count()
            });
        }

        [HttpPut("{idStudent}&{idSubject}&{numberOfTimes}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int idStudent, int idSubject, int numberOfTimes, PointDTO pointDTO)
        {
            var point = await db.Points.FindAsync(idStudent, idSubject, numberOfTimes);
            if (point == null)
                return NotFound(new
                {
                    error = "Point object not found by id to update."
                });
            if (pointDTO.Points != null)
                point.Points = pointDTO.Points;
            try
            {
                db.Entry(point).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "An error occurred."
                });
            }

        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(PointDTO pointDTO)
        {
            try
            {
                pointDTO.Alias = $"{pointDTO.IdStudent}-{pointDTO.IdSubject}-{pointDTO.NumberOfTimes}";
                db.Points.Add(new Point()
                {
                    IdStudent = pointDTO.IdStudent,
                    IdSubject = pointDTO.IdSubject,
                    NumberOfTimes = pointDTO.NumberOfTimes,
                    Points = pointDTO.Points,
                    Alias = pointDTO.Alias
                });
                await db.SaveChangesAsync();

                return CreatedAtAction(nameof(GetByID), new { idStudent = pointDTO.IdStudent, idSubject = pointDTO.IdSubject, numberOfTimes = pointDTO.NumberOfTimes }, pointDTO);
            }
            catch (Exception ex)
            {

                return BadRequest(new
                {
                    error = "An error occurred."
                });
            }

        }
        [HttpDelete("{idStudent}&{idSubject}&{numberOfTimes}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int idStudent, int idSubject, int numberOfTimes)
        {
            var point = await db.Points.FindAsync(idStudent, idSubject, numberOfTimes);
            if (point == null)
                return NotFound(new
                {
                    error = "Point object not found by id to delete."
                });
            try
            {
                db.Points.Remove(point);
                await db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "An error occurred."
                });
            }
        }
    }
}
