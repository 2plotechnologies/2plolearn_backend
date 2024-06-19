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
    [Route("api")]
    public class AuthController : Controller
    {
        private readonly IUserRepository _repository;
        private readonly JwtService _jwtService;
        private readonly UserContext _context;
        public AuthController(IUserRepository repository, JwtService jwtService, UserContext context)
        {
            _repository = repository;
            _jwtService = jwtService;
            _context = context;
        }
        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

            var user = new User
            {
                username = dto.username,
                email = dto.email,
                password = BCrypt.Net.BCrypt.HashPassword(dto.password),
                rol = dto.rol,
                profile_pic = "default.jpg",
                CompanyId = dto.CompanyId
            };
            return Created("success", _repository.Create(user));
        }

        [HttpPost("register-first")]
        public IActionResult RegisterFirst(RegisterDto dto)
        {
            var user = new User
            {
                username = dto.username,
                email = dto.email,
                password = BCrypt.Net.BCrypt.HashPassword(dto.password),
                rol = dto.rol,
                profile_pic = "default.jpg",
                CompanyId = dto.CompanyId
            };
            return Created("success", _repository.Create(user));
        }

        [HttpPost("login")]
        public IActionResult Login([Bind("email, password")]LoginDto dto){
            var user = _repository.GetByEmail(dto.email);
            if (user == null ) return BadRequest(new {message = "Invalid credentials"});
            if(!BCrypt.Net.BCrypt.Verify(dto.password, user.password)){
                return BadRequest(new {message = "Invalid credentials"});
            }
            var jwt = _jwtService.Generate(user.id);
            var response = new
            {
                Token = jwt,
                User = new
                {
                    id = user.id,
                    username = user.username,
                    email = user.email, 
                    rol = user.rol,
                    profile_pic = user.profile_pic,
                    CompanyId = user.CompanyId
                }
            };

            return Ok(response);
        }
        [HttpGet("users/{companyId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(int companyId){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }
            
            return await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();
        }

        [HttpGet("normal-users/{companyId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetNormalUsers(int companyId){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }
            
            return await _context.Users.Where(u=> u.rol == "user" && u.CompanyId == companyId).ToListAsync();
        }

        [HttpPut("update-profile/{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserProfileDto user){

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }


            if(id != user.id){
                return BadRequest();
            }

            var userEdit = await _context.Users.FindAsync(id);

            if(userEdit is null){
                return NotFound();
            }

            userEdit.email = user.email;
            userEdit.password = BCrypt.Net.BCrypt.HashPassword(user.password);
            userEdit.username = user.username;
            await _context.SaveChangesAsync();
            return Ok(userEdit);
        }        

        [HttpPut("update-image/{id}")]
        public async Task<IActionResult> UpdateImage(int id, [FromForm]UserImageDto dto){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

            if(id != dto.id){
                return BadRequest();
            }

            var userEdit = await _context.Users.FindAsync(id);

            if(userEdit is null){
                return NotFound();
            }

            var fileName = string.Empty;

            if(dto.profile_pic != null && dto.profile_pic.Length>0){
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

                // Asegúrate de que la carpeta de carga exista, de lo contrario, créala
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Genera un nombre de archivo único para evitar conflictos de nombres
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.profile_pic.FileName);

                // Combina la ruta de carga con el nombre del archivo
                var filePath = Path.Combine(uploadPath, fileName);

                // Guarda la imagen en el servidor
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.profile_pic.CopyToAsync(stream);
                }
            }
            userEdit.profile_pic = fileName;
            await _context.SaveChangesAsync();
            return Ok(userEdit);
        }

        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUserAsync(int userId)
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }
            // Buscar el usuario por su ID
            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.id == userId);

            if (usuario == null)
            {
                return NotFound();
            }

            // Eliminar el usuario
            _context.Users.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("validate")]
        public IActionResult ValidateLogin(){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

            return Ok("Valid Token");
        }

    }
}