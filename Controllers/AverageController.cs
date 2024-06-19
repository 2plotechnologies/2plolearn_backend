using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Backend.Data;
using LMS.Backend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Backend.Controllers
{
    [ApiController]
    [Route("api/average")]
    public class AverageController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly JwtService _jwtService;
        public AverageController(UserContext context, JwtService jwtService){
            _context = context;
            _jwtService = jwtService;
        }   

        [HttpGet("average-score")]
        public async Task<IActionResult> GetAverageScore(int userId, int courseId)
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
            
            try
            {
                var averageScore = await _context.Scores
                    .Where(score => score.UserId == userId && score.CourseId == courseId)
                    .AverageAsync(score => score.ScoreFinal);

                return Ok(new { AverageScore = averageScore });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}