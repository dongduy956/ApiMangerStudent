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
    [Authorize]
    [EnableCors("MyPolicy")]
    public class TeacherController : ControllerBase
    {
        private const string PASSWORD_RESET = "12345";
        private readonly ManagerStudentContext db;
        public TeacherController(ManagerStudentContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pagesize = 5)
        {
            var list = new List<TeacherDTO>();
            await db.Teachers.Skip((page - 1) * pagesize).Take(pagesize)
                .ForEachAsync(x => list.Add(new TeacherDTO(x)));
            return new ObjectResult(new
            {
                data = list,
                page = page,
                pagesize = pagesize,
                totalPage = Math.Ceiling(db.Teachers.Count() / (float)pagesize),
                totalItems = db.Teachers.Count()
            });
        }
        [HttpPut("[action]/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var teacher = await db.Teachers.FindAsync(id);
            if (teacher != null)
            {
                teacher.Password = Libary.Instances.EncodeMD5(PASSWORD_RESET);
                db.Entry(teacher).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Ok(new
                {
                    message = "Reset password success.Password: 12345"
                });
            }
            else
            {
                return BadRequest(new
                {
                    error = "Object teacher not found by id to reset."
                });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TeacherDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByID(int id)
        {
            var teacher = await db.Teachers.FindAsync(id);
            return teacher == null ? NotFound(new
            {
                error = "Object teacher not found by id to get."
            }) : Ok(new TeacherDTO(teacher));
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(TeacherDTO), StatusCodes.Status200OK)]
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
            var list = new List<TeacherDTO>();
            var teachers = db.Teachers.Where(x =>
              x.Name.ToLower().Trim().Contains(q)
              || x.DateOfBirth.ToString().Contains(q)
              || x.Username.ToLower().Trim().Equals(q)
              || x.IdRoleNavigation.Name.ToLower().Trim().Contains(q)
              );
            await teachers.Skip((page - 1) * pagesize).Take(pagesize)
                 .ForEachAsync(x => list.Add(new TeacherDTO(x)));
            return new ObjectResult(new
            {
                data = list,
                page = page,
                pagesize = pagesize,
                totalPage = Math.Ceiling(db.Teachers.Count() / (float)pagesize),
                totalItems = teachers.Count()
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, TeacherDTO teacherDTO)
        {
            var teacher = await db.Teachers.FindAsync(id);
            if (teacher == null)
                return NotFound(new
                {
                    error = "Teacher object not found by id to update."
                });
            if (!string.IsNullOrEmpty(teacherDTO.Name))
                teacher.Name = teacherDTO.Name.Trim();
            if (teacherDTO.IdRole != null)
                teacher.IdRole = teacherDTO.IdRole;
            if (!string.IsNullOrEmpty(teacherDTO.Image))
                teacher.Image = teacherDTO.Image;
            if (teacherDTO.DateOfBirth != null)
                teacher.DateOfBirth = teacherDTO.DateOfBirth;
            try
            {
                db.Entry(teacher).State = EntityState.Modified;
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
        [HttpPut("[action]/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePass(int id, ChangePassword changePassword)
        {
            var teacher = await db.Teachers.SingleOrDefaultAsync(x => x.Id == id && x.Password.Equals(changePassword.password));
            if (teacher != null)
            {
                if (!changePassword.newPassword.Equals(changePassword.prePassword))
                    return BadRequest(new
                    {
                        error = "Confirm password is incorrect."
                    });
                teacher.Password = changePassword.newPassword;
                db.Entry(teacher).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return BadRequest(new
                {
                    error = "Password is incorrect."
                });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(TeacherDTO teacherDTO)
        {
            try
            {
                db.Teachers.Add(new Teacher()
                {
                    Name = teacherDTO.Name.Trim(),
                    DateOfBirth = teacherDTO.DateOfBirth,
                    IdRole = teacherDTO.IdRole,
                    Image = teacherDTO.Image,
                    Username = teacherDTO.Username,
                    Password = Libary.Instances.EncodeMD5("12345")
                });
                await db.SaveChangesAsync();

                var teacher = await db.Teachers.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                return CreatedAtAction(nameof(GetByID), new { id = teacher.Id }, new TeacherDTO(teacher));
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
            var teacher = await db.Teachers.FindAsync(id);
            if (teacher == null)
                return NotFound(new
                {
                    error = "Teacher object not found by id to delete."
                });
            try
            {
                db.Teachers.Remove(teacher);
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
