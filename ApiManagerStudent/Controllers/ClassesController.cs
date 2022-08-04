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
    [EnableCors("MyPolicy")]
    [Authorize]
    public class ClassesController : ControllerBase
    {
        private readonly ManagerStudentContext db;
        public ClassesController(ManagerStudentContext context)
        {
            db = context;
        }
        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            var list = new List<ClassDTO>();
            await db.Classes.ForEachAsync(x => list.Add(new ClassDTO(x)));
            return new ObjectResult(new
            {
                data = list
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page=1,int pagesize=5)
        {
            var list = new List<ClassDTO>();
            await db.Classes.Skip((page-1)*pagesize).Take(pagesize).ForEachAsync(x => list.Add(new ClassDTO(x)));
            return new ObjectResult(new { data = list,page=page, pagesize = pagesize,totalPage=Math.Ceiling(db.Classes.Count()/(float)pagesize),totalItems=db.Classes.Count() });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClassDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var _class = await db.Classes.FindAsync(id);
            return _class == null ? NotFound(new { error = "Object classes not found by id to get." }) : Ok(new ClassDTO(_class));
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
            var classes = db.Classes.Where(x => x.Name.ToLower().Contains(q));
            var list = new List<ClassDTO>();
            await classes.Skip((page - 1) * pagesize).Take(pagesize).ForEachAsync(x => list.Add(new ClassDTO(x)));
            return new ObjectResult(new { data = list, page = page, pagesize = pagesize, totalPage = Math.Ceiling(db.Classes.Count() / (float)pagesize), totalItems = classes.Count() });
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var classes = await db.Classes.FindAsync(id);
            if (classes == null)
                return NotFound(new
                {
                    error = "Classes object not found by id to delete."
                });
            try
            {
                db.Classes.Remove(classes);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "An error occurred." });
            }

            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(ClassDTO classDTO)
        {
            try
            {
                await db.Classes.AddAsync(new Class()
                {
                    Name = classDTO.Name,
                    Alias = Libary.Instances.convertToUnSign3(classDTO.Name.ToLower().Trim())
                });
                await db.SaveChangesAsync();
                var classes = db.Classes.OrderByDescending(x => x.Id).FirstOrDefault();
                return CreatedAtAction(nameof(GetById), new { id = classes.Id }, new ClassDTO(classes));
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "An error occurred."
                });
            }

        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, ClassDTO classDTO)
        {
            var classes = await db.Classes.FindAsync(id);
            if (classes == null)
                return BadRequest(new { error = "Object classes not found by id to update." });
            if (!string.IsNullOrEmpty(classDTO.Name))
            {
                classes.Name = classDTO.Name;
                classes.Alias = Libary.Instances.convertToUnSign3(classDTO.Name.ToLower().Trim());
            }
            try
            {

                db.Entry(classes).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred.");
            }
        }

    }
}
