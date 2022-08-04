using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiManagerStudent.EF;
using ApiManagerStudent.Models;
using Microsoft.AspNetCore.Cors;
using ApiManagerStudent.Support;
using Microsoft.AspNetCore.Authorization;

namespace ApiManagerStudent.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    [EnableCors("MyPolicy")]
    public class SubjectController : ControllerBase
    {
        private readonly ManagerStudentContext db;

        public SubjectController(ManagerStudentContext context)
        {
            db = context;
        }


        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            var list = new List<SubjectDTO>();
            await db.Subjects.ForEachAsync(x => list.Add(new SubjectDTO(x)));
            return new ObjectResult(new
            {
                data = list
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pagesize = 5)
        {
            var list = new List<SubjectDTO>();
            await db.Subjects.Skip((page - 1) * pagesize).Take(pagesize)
                .ForEachAsync(x => list.Add(new SubjectDTO(x)));
            return new ObjectResult(new
            {
                data = list,
                page = page,
                pagesize = pagesize,
                totalPage = Math.Ceiling(db.Subjects.Count() / (float)pagesize),
                totalItems = db.Subjects.Count()
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SubjectDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByID(int id)
        {
            var subject = await db.Subjects.FindAsync(id);
            return subject == null ? NotFound(new
            {
                error = "Subject object not found by id to get."
            }) : Ok(new SubjectDTO(subject));
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                    error = "Invalid query"
                });
            }
            var list = new List<SubjectDTO>();
            var subjects = db.Subjects.Where(x => x.Name.ToLower().Trim().Contains(q));
               await subjects.Skip((page - 1) * pagesize).Take(pagesize)
                .ForEachAsync(x => list.Add(new SubjectDTO(x)));
            return new ObjectResult(new
            {
                data = list,
                page = page,
                pagesize = pagesize,
                totalPage = Math.Ceiling(db.Subjects.Count() / (float)pagesize),
                totalItems = subjects.Count()
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, SubjectDTO subjectDTO)
        {
            var subject = await db.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound(new
                {
                    error = "Subject object not found by id to update."
                });
            if (!string.IsNullOrEmpty(subjectDTO.Name))
            {
                subject.Name = subjectDTO.Name.Trim();
                subject.Alias = Libary.Instances.convertToUnSign3(subject.Name.ToLower());
            }
            try
            {
                db.Entry(subject).State = EntityState.Modified;
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
        public async Task<IActionResult> Create(SubjectDTO subjectDTO)
        {
            try
            {
                db.Subjects.Add(new Subject()
                {
                    Name = subjectDTO.Name.Trim(),
                    Alias = Libary.Instances.convertToUnSign3(subjectDTO.Name.Trim().ToLower())
                });
                await db.SaveChangesAsync();

                var subject = await db.Subjects.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                return CreatedAtAction(nameof(GetByID), new { id = subject.Id }, new SubjectDTO(subject));
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
            var subject = await db.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound(new
                {
                    error = "Subject object not found by id to delete."
                });
            try
            {
                db.Subjects.Remove(subject);
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
