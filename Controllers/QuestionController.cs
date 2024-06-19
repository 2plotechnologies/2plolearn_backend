using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LMS.Backend.Data;
using LMS.Backend.Dtos;
using LMS.Backend.Helpers;
using LMS.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Backend.Controllers
{
    [ApiController]
    [Route("api/question")]
    public class QuestionController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly JwtService _jwtService;
        public QuestionController(UserContext context, JwtService jwtService){
            _context = context;
            _jwtService = jwtService;
        }
        [HttpPost("new")]
        public async Task<ActionResult<Question>> CreateQuestion(CreateQuestionDto dto){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

            var question = new Question{
                QuestionString = dto.QuestionString,
                EvaluationId = dto.EvaluationId
            };
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return Ok(question);
        }

        [HttpPut("update-question/{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, UpdateQuestionDto dto){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

             if(id != dto.Id){
                return BadRequest();
            }

            var questionEdit = await _context.Questions.FindAsync(id);

            if(questionEdit == null){
                return NotFound();
            }

            questionEdit.QuestionString = dto.QuestionString;
            await _context.SaveChangesAsync();
            return Ok(questionEdit);
        }

        [HttpDelete("delete-question/{Id}")]
        public async Task<IActionResult> DeleteQuestionAsync(int Id){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

             var question = await _context.Questions
                .FirstOrDefaultAsync(u => u.Id == Id);

            if (question == null)
            {
                return NotFound();
            }

            // Eliminar el usuario
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("questions-by-exam/{id}")]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestionsByExam(int id){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if (!_jwtService.ValidateToken(token))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var questions = await _context.Questions.Where(e => e.EvaluationId == id).ToListAsync();

            return Ok(questions);
        }
    }
}