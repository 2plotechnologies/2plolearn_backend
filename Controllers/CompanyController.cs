using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LMS.Backend.Data;
using LMS.Backend.Dtos;
using LMS.Backend.Models;
using LMS.Backend.Helpers;
using Microsoft.EntityFrameworkCore;

namespace LMS.Backend.Controllers
{
    [ApiController]
    [Route("api/company")]
    public class CompanyController : Controller
    {
        private readonly IUserRepository _repository;
        private readonly JwtService _jwtService;
        private readonly UserContext _context;

         public CompanyController(IUserRepository repository, JwtService jwtService, UserContext context)
        {
            _repository = repository;
            _jwtService = jwtService;
            _context = context;
        }

        [HttpPost("create")]
        public async Task<ActionResult<Company>> RegisterCompany([FromForm] NewCompany dto)
        {
            if (dto.Image == null || dto.Image.Length == 0)
            {
                return BadRequest("No se ha proporcionado ninguna imagen.");
            }

            // Define una carpeta donde se guardarán las imágenes en el servidor
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            // Asegúrate de que la carpeta de carga exista, de lo contrario, créala
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Genera un nombre de archivo único para evitar conflictos de nombres
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);

            // Combina la ruta de carga con el nombre del archivo
            var filePath = Path.Combine(uploadPath, fileName);

            // Guarda la imagen en el servidor
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            var company = new Company{
                Name = dto.Name,
                Image = fileName
            };
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }
    }

    public class NewCompany
    {
        public string Name {get; set;}
        public IFormFile Image{get; set;}
    }
}