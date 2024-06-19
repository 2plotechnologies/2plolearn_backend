using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LMS.Backend.Data;
using LMS.Backend.Dtos;
using LMS.Backend.Helpers;
using LMS.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS.Backend.Controllers
{
    [ApiController]
    [Route("api/unit")]
    public class UnitController:ControllerBase{
        private readonly UserContext _context;
        private readonly JwtService _jwtService;
        public UnitController(UserContext context, JwtService jwtService){
            _context = context;
            _jwtService = jwtService;
        }
        [HttpPost("create-unit")]
        public async Task<ActionResult<Unit>> CreateUnit(UnitCreationDto unitDto)
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

            var unitNew = new Unit{
                Title = unitDto.Name,
                CourseId = unitDto.CourseId
            };
            _context.Units.Add(unitNew);
            await _context.SaveChangesAsync();
            return unitNew;
        }

        [HttpGet("units-by-course/{courseId}")]
        public async Task<ActionResult<IEnumerable<Unit>>> GetUnitsByCourse(int courseId){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

            var units = await _context.Units
                .Where(u => u.CourseId == courseId)
                .Select(u => u)
                .ToListAsync();

            return Ok(units);
        }

        [HttpPut("update-unit/{id}")]
        public async Task<IActionResult> UpdateUnit(int id, UnitUpdateDto unit){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

            if(id != unit.Id){
                return BadRequest();
            }

            var unitEdit = await _context.Units.FindAsync(id);

            if(unitEdit is null){
                return NotFound();
            }

            unitEdit.Title = unit.Title;
            await _context.SaveChangesAsync();
            return Ok(unitEdit);
        }

        [HttpDelete("delete-unit/{Id}")]
        public async Task<IActionResult> DeleteUnitAsync(int Id){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

             var unit = await _context.Units
                .FirstOrDefaultAsync(u => u.id == Id);

            if (unit == null)
            {
                return NotFound();
            }

            // Eliminar el usuario
            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    public class UnitUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
