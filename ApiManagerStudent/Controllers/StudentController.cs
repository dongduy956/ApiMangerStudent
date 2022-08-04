using ApiManagerStudent.EF;
using ApiManagerStudent.Models;
using ApiManagerStudent.Support;
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

    public class StudentController : ControllerBase
    {
        private readonly ManagerStudentContext db;
        public StudentController(ManagerStudentContext db)
        {
            this.db = db;
        }


        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            var list = new List<StudentDTO>();
            await db.Students.ForEachAsync(x => list.Add(new StudentDTO(x)));
            return new ObjectResult(new
            {
                data = list
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pagesize = 5)
        {            
            var list = new List<StudentDTO>();
            await db.Students.Skip((page - 1) * pagesize).Take(pagesize)
                .ForEachAsync(x => list.Add(new StudentDTO(x)));
            return new ObjectResult(new
            {
                data = list,
                page = page,
                pagesize = pagesize,
                totalPage = Math.Ceiling(db.Students.Count() / (float)pagesize),
                totalItems = db.Students.Count()
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StudentDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByID(int id)
        {
            var student = await db.Students.FindAsync(id);
            return student == null ? NotFound(new
            {
                error = "Student object not found by id to get."
            }) : Ok(new StudentDTO(student));
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(StudentDTO), StatusCodes.Status200OK)]
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
            var list = new List<StudentDTO>();
            var students = db.Students.Where(x =>
                x.Name.ToLower().Trim().Contains(q)
                || x.DateOfBirth.ToString().Contains(q)
                || x.IdClassNavigation.Name.ToLower().Trim().Contains(q)
               );
            await students.Skip((page - 1) * pagesize).Take(pagesize)
                 .ForEachAsync(x => list.Add(new StudentDTO(x)));
            return new ObjectResult(new
            {
                data = list,
                page = page,
                pagesize = pagesize,
                totalPage = Math.Ceiling(db.Students.Count() / (float)pagesize),
                totalItems = students.Count()
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, StudentDTO studentDTO)
        {
            var student = await db.Students.FindAsync(id);
            if (student == null)
                return NotFound(new
                {
                    error = "Student object not found by id to update."
                });
            if (!string.IsNullOrEmpty(studentDTO.Image))
                student.Image = studentDTO.Image;
            if (!string.IsNullOrEmpty(studentDTO.Name))
            {
                student.Name = studentDTO.Name.Trim();
                student.Alias = Libary.Instances.convertToUnSign3(student.Name.ToLower() + DateTime.Now.Ticks);
            }
            if (studentDTO.DateOfBirth != null)
                student.DateOfBirth = studentDTO.DateOfBirth;
            if (studentDTO.IdClass != null)
                student.IdClass = studentDTO.IdClass;
            try
            {
                db.Entry(student).State = EntityState.Modified;
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
        public async Task<IActionResult> Create(StudentDTO studentDTO)
        {
            try
            {
                db.Students.Add(new Student()
                {
                    Name = studentDTO.Name.Trim(),
                    DateOfBirth = studentDTO.DateOfBirth,
                    IdClass = studentDTO.IdClass,
                    Image = studentDTO.Image,
                    Alias = Libary.Instances.convertToUnSign3(studentDTO.Name.Trim().ToLower() + DateTime.Now.Ticks)
                });
                await db.SaveChangesAsync();

                var student = await db.Students.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                return CreatedAtAction(nameof(GetByID), new { id = student.Id }, new StudentDTO(student));
            }
            catch (Exception ex)
            {

                return BadRequest(new
                {
                    error = "An error occurred."
                });
            }

        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await db.Students.FindAsync(id);
            if (student == null)
                return NotFound(new
                {
                    error = "Student object not found by id to delete."
                });
            try
            {
                db.Students.Remove(student);
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
