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

    public class RoleController : ControllerBase
    {
        private readonly ManagerStudentContext db;
        public RoleController(ManagerStudentContext db)
        {
            this.db = db;
        }

        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            var list = new List<RoleDTO>();
            await db.Roles.ForEachAsync(x => list.Add(new RoleDTO(x)));
            return new ObjectResult(new
            {
                data = list
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pagesize = 5)
        {
            var list = new List<RoleDTO>();
            await db.Roles.Skip((page - 1) * pagesize).Take(pagesize)
                .ForEachAsync(x => list.Add(new RoleDTO(x)));
            return new ObjectResult(new
            {
                data = list,
                page = page,
                pagesize = pagesize,
                totalPage = Math.Ceiling(db.Roles.Count() / (float)pagesize),
                totalItems = db.Roles.Count()
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoleDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByID(int id)
        {
            var role = await db.Roles.FindAsync(id);
            return role == null ? NotFound(new
            {
                error = "Object roles not found by id to get."
            })
                : Ok(new RoleDTO(role));
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
                    error = "Invalid query."
                });
            }
            var list = new List<RoleDTO>();
            var roles = db.Roles.Where(x => x.Name.ToLower().Trim().Contains(q));
                      await roles.Skip((page - 1) * pagesize).Take(pagesize)
                .ForEachAsync(x => list.Add(new RoleDTO(x)));
            return new ObjectResult(new
            {
                data = list,
                page = page,
                pagesize = pagesize,
                totalPage = Math.Ceiling(db.Roles.Count() / (float)pagesize),
                totalItems = roles.Count()
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await db.Roles.FindAsync(id);
            if (role == null)
                return NotFound(new
                {
                    error = "Role object not found by id to delete."
                });
            try
            {
                db.Roles.Remove(role);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    error = "An error occurred."
                });
            }
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(RoleDTO roleDTO)
        {
            try
            {
                await db.Roles.AddAsync(new Role()
                {
                    Name = roleDTO.Name,
                    Alias = Libary.Instances.convertToUnSign3(roleDTO.Name.ToLower().Trim())
                });
                await db.SaveChangesAsync();
                var role = await db.Roles.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                return CreatedAtAction(nameof(GetByID), new { id = role.Id }, new RoleDTO(role));
            }
            catch (Exception)
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
        public async Task<IActionResult> Update(int id, RoleDTO roleDTO)
        {
            var role = await db.Roles.FindAsync(id);
            if (role == null)
                return BadRequest(new
                {
                    error = "Role object not found by id to update."
                });
            if (!string.IsNullOrEmpty(roleDTO.Name))
            {
                role.Name = roleDTO.Name.Trim();
                role.Alias = Libary.Instances.convertToUnSign3(role.Name.ToLower().Trim());
            }
            try
            {
                db.Entry(role).State = EntityState.Modified;
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
