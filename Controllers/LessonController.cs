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
    [Route("api/lesson")]
    public class LessonController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly JwtService _jwtService;
        public LessonController(UserContext context, JwtService jwtService){
            _context = context;
            _jwtService = jwtService;
        }
        [HttpGet("detail/{id}")]
        public IActionResult GetLessonContent(int id){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

            var lesson = _context.Lessons.FirstOrDefault(l => l.Id == id);
            if(lesson==null){
                return NotFound();
            }
            var LessonDto = new LessonDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Content = lesson.Content
            };
            return Ok(LessonDto);
        }
        [HttpPost("create-lesson")]
        public async Task<ActionResult<Lesson>> CreateLesson(NewLessonDto lessonDto){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

            var lessonNew = new Lesson{
                Title = lessonDto.Title,
                Content = lessonDto.Content,
                UnitId = lessonDto.UnitId
            };
            _context.Lessons.Add(lessonNew);
            await _context.SaveChangesAsync();
            return lessonNew;
        }

        [HttpGet("lessons-by-unit/{unitId}")]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetLessonsByUnit(int unitId){
             if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

            var lessons = await _context.Lessons
                .Where(l => l.UnitId == unitId)
                .Select(l => l)
                .ToListAsync();

            return Ok(lessons);
        }

        [HttpPut("update-lesson/{id}")]
        public async Task<IActionResult> UpdateLesson(int id, LessonDto lesson){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

            if(id != lesson.Id){
                return BadRequest();
            }

            var lessonEdit = await _context.Lessons.FindAsync(id);

            if(lessonEdit is null){
                return NotFound();
            }

            lessonEdit.Title = lesson.Title;
            lessonEdit.Content = lesson.Content;
            await _context.SaveChangesAsync();
            return Ok(lessonEdit);
        }

        [HttpDelete("delete-lesson/{Id}")]
        public async Task<IActionResult> DeleteLessonAsync(int Id){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

             var lesson = await _context.Lessons
                .FirstOrDefaultAsync(u => u.Id == Id);

            if (lesson == null)
            {
                return NotFound();
            }

            // Eliminar el usuario
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}