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
    [Route("api/answer")]
    public class AnswerController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly JwtService _jwtService;
        public AnswerController(UserContext context, JwtService jwtService){
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("new")]
        public async Task<ActionResult<Answer>> CreateAnswer(NewAnswerDto dto){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

            var answer = new Answer{
                AnswerString = dto.AnswerString,
                Correct = dto.Correct,
                QuestionId = dto.QuestionId
            };
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return Ok(answer);
        }

        [HttpPut("update-answer/{id}")]
        public async Task<IActionResult> UpdateAnswer(int id, UpdateAnswerDto dto){
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

            var answerEdit = await _context.Answers.FindAsync(id);

            if(answerEdit == null){
                return NotFound();
            }

            answerEdit.AnswerString = dto.AnswerString;
            answerEdit.Correct = dto.Correct;
            await _context.SaveChangesAsync();
            return Ok(answerEdit);
        }

        [HttpDelete("delete-answer/{Id}")]
        public async Task<IActionResult> DeleteAnswerAsync(int Id){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

             var answer = await _context.Answers
                .FirstOrDefaultAsync(a => a.Id == Id);

            if (answer == null)
            {
                return NotFound();
            }

            // Eliminar el usuario
            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("answers-by-question/{questionId}")]
        public async Task<ActionResult<IEnumerable<Answer>>> GetAnswersByQuestion(int questionId){
             if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

            var answers = await _context.Answers
                .Where(a => a.QuestionId == questionId)
                .Select(a => a)
                .ToListAsync();

            return Ok(answers);
        }
    }
}