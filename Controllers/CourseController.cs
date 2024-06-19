using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Backend.Data;
using LMS.Backend.Dtos;
using LMS.Backend.Helpers;
using LMS.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Backend.Controllers
{
    [ApiController]
    [Route("api/course")]
    public class CourseController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly JwtService _jwtService;
        public CourseController(UserContext context, JwtService jwtService){
            _context = context;
            _jwtService = jwtService;
        }
        
        //Crear curso
        [HttpPost]
        public async Task<ActionResult<Course>> CreateCourse([FromForm] CourseDto dto){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

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
            var courseNew = new Course
            {
                Name = dto.Name,
                Description = dto.Description,
                Image = fileName,
                CompanyId = dto.CompanyId
            };
            _context.Courses.Add(courseNew);
            await _context.SaveChangesAsync();
            return courseNew;
        }

        //Actualizar curso
        [HttpPut("update-course/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, CourseUpdateDto course){

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }


            if(id != course.Id){
                return BadRequest();
            }

            var courseEdit = await _context.Courses.FindAsync(id);

            if(courseEdit is null){
                return NotFound();
            }

            courseEdit.Name = course.Name;
            courseEdit.Description = course.Description;
            await _context.SaveChangesAsync();
            return Ok(courseEdit);
        }        

        //Obtener todos los cursos
        [HttpGet("courses/{companyId}")]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses(int companyId){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

            return await _context.Courses.Where(c => c.CompanyId == companyId).ToListAsync();
        }
        //Obtener cursos por usuario
        [HttpGet("cursos-usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Course>>> GetCursosUsuario(int usuarioId)
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

            var cursosUsuario = await _context.UserCourses
                .Where(uc => uc.UserId == usuarioId)
                .Select(uc => uc.Course)
                .ToListAsync();

            return Ok(cursosUsuario);
        }
        //Obtener curso con sus unidades, lecciones y examenes
        [HttpGet("detail/{id}")]
        public IActionResult GetCourseWithUnits(int id)
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
            
            // Utiliza Entity Framework Core o tu tecnología de acceso a datos para obtener el curso y sus unidades.
            var course = _context.Courses
                .Include(c => c.Units)
                    .ThenInclude(u => u.Lessons)
                .Include(c => c.Units)
                    .ThenInclude(u => u.Evaluations)
                .FirstOrDefault(c => c.Id == id);


            if (course == null)
            {
                return NotFound(); // Devuelve un código 404 si el curso no se encuentra.
            }

            // Mapea los datos necesarios a un objeto DTO (Data Transfer Object) si es necesario.
            var courseDto = new CourseDataDto
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                Units = course.Units?.Select(u => new UnitDto
                {
                    Id = u.id,
                    Title = u.Title,
                    Lessons = u.Lessons?.Select(l => new LessonDto
                    {
                        Id = l.Id,
                        Title = l.Title,
                        Content = l.Content
                    }).ToList() ?? new List<LessonDto>(),
                    Evaluations = u.Evaluations?.Select(e => new EvaluationDto{
                        Id = e.Id,
                        Title = e.Title,
                        UnitId = e.UnitId
                    }).ToList() ?? new List<EvaluationDto>()
                }).ToList() ?? new List<UnitDto>() // Si course.Units es nulo, crea una lista vacía
            };
            // Devuelve el objeto DTO en formato JSON como respuesta.
            return Ok(courseDto);
        }

        //Eliminar curso
        [HttpDelete("delete-course/{courseId}")]
        public async Task<IActionResult> DeleteCourseAsync(int courseId)
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
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return NotFound();
            }

            // Eliminar el usuario
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok();
        }

        //Listar usuarios en un curso
        [HttpGet("usuarios-curso/{cursoId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsuariosCurso(int cursoId)
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

            var usuariosCurso = await _context.UserCourses
                .Where(uc => uc.CourseId == cursoId)
                .Select(uc => uc.User)
                .ToListAsync();

            return Ok(usuariosCurso);
        }


        //Asignar y desasignar cursos
        [HttpPost("assign-course")]
        public async Task<ActionResult<UserCourses>> AssingCourse(AssingCourseDto dto){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.id == dto.UserId);

            if (user == null)
            {
                // El usuario con el UserId especificado no se encuentra en la base de datos.
                // Puedes manejar este caso de acuerdo a tus necesidades.
                return NotFound(new { message = "User not found" });
            }

            var user_course = new UserCourses
            {
                    UserId = dto.UserId,
                    CourseId = dto.CourseId
            };

            try{
                _context.UserCourses.Add(user_course);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex){
                return BadRequest(new {error = ex.Message});
            }

            return Ok(new {success = "true"});
        }

        [HttpDelete("unassign-course")]
        public async Task<ActionResult<UserCourses>> UnassingCourse(int UserId, int CourseId){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.id == UserId);

            if (user == null)
            {
                // El usuario con el UserId especificado no se encuentra en la base de datos.
                // Puedes manejar este caso de acuerdo a tus necesidades.
                return NotFound(new { message = "User not found" });
            }

            var user_course = new UserCourses
            {
                    UserId = UserId,
                    CourseId = CourseId
            };

            try{
                _context.UserCourses.Remove(user_course);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex){
                return BadRequest(new {error = ex.Message});
            }

            return Ok(new {success = "true"});
        }
    }
}

public class CourseUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class AssingCourseDto
{
    public int UserId {get; set;}
    public int CourseId {get; set;}
}
