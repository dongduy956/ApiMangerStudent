using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiManagerStudent.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [EnableCors("MyPolicy")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        public UploadController(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var index = file.FileName.LastIndexOf('.');
                var fileName = file.FileName.Substring(0, index) + DateTime.Now.Ticks + file.FileName.Substring(index);
                string directoryPath = Path.Combine(webHostEnvironment.ContentRootPath, "Images");
                string filePath = Path.Combine(directoryPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) {
                    file.CopyTo(stream);
                }
                return new ObjectResult(new
                {
                    status = true,
                    filename = "Images/" + fileName
                });
            }
            return BadRequest(new { 
                status=false,
                error="An error occurred."
            });
        }
    }
}
