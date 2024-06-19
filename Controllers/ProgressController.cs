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
    [Route("api/progress")]
    public class ProgressController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly JwtService _jwtService;
        public ProgressController(UserContext context, JwtService jwtService){
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("create-progress")]
        public async Task<ActionResult<Progress>> AddProgress(ProgressDto progressDto){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }
            
            var progress = new Progress{
                UserId = progressDto.UserId,
                CourseId = progressDto.CourseId,
                UnitId = progressDto.UnitId
            };

            _context.Progress.Add(progress);
            await _context.SaveChangesAsync();
            return Ok(progress);
        }

        [HttpGet("get-progress")]
        public async Task<IActionResult> GetCourseProgress(int userId, int courseId)
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
            
            // Consulta la base de datos para obtener la cantidad de unidades en el curso.
            int unitCount = await _context.Units
                .Where(u => u.CourseId == courseId)
                .CountAsync();

            // Consulta la tabla Progress para contar la cantidad de registros que coinciden con UserId y CourseId.
            int progressCount = await _context.Progress
                .Where(p => p.UserId == userId && p.CourseId == courseId)
                .CountAsync();

            // Crear un objeto anónimo con las cantidades.
            var result = new
            {
                UnitCount = unitCount,
                ProgressCount = progressCount
            };

            return Ok(result);
        }
    }
}