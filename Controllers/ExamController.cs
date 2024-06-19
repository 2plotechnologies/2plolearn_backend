using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
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
    [Route("api/exam")]
    public class ExamController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly JwtService _jwtService;
        public ExamController(UserContext context, JwtService jwtService){
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("new")]
        public async Task<ActionResult<Evaluation>> CreateExam(CreateExamDto dto){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

            var Exam = new Evaluation{
                Title = dto.Title,
                UnitId = dto.UnitId
            };
            _context.Evaluations.Add(Exam);
            await _context.SaveChangesAsync();
            return Ok(Exam);
        }

        [HttpPut("update-exam/{id}")]
        public async Task<IActionResult> UpdateExam(int id, ExamUpdateDto dto){
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

            var examEdit = await _context.Evaluations.FindAsync(id);

            if(examEdit == null){
                return NotFound();
            }

            examEdit.Title = dto.Title;
            await _context.SaveChangesAsync();
            return Ok(examEdit);
        }

        [HttpDelete("delete-exam/{Id}")]
        public async Task<IActionResult> DeleteExamAsync(int Id){
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // El token de autorización no se proporcionó en los encabezados
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            string token = Request.Headers["Authorization"]!;

            if(!_jwtService.ValidateToken(token)){
                return Unauthorized(new { message = "Invalid token" });
            }

             var exam = await _context.Evaluations
                .FirstOrDefaultAsync(e => e.Id == Id);

            if (exam == null)
            {
                return NotFound();
            }

            // Eliminar el usuario
            _context.Evaluations.Remove(exam);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetQuestionsAndAnswers(int id)
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

            var questions = await _context.Questions
                .Include(q => q.Answers)
                .Where(q => q.EvaluationId == id)
                .ToListAsync();
            
            if(questions == null){
                return NotFound();
            }
            // Mapea las preguntas y respuestas a un objeto anónimo para una respuesta JSON personalizada
            var result = questions.Select(q => new
            {
                QuestionId = q.Id,
                QuestionString = q.QuestionString,
                Answers = q.Answers.Select(a => new
                {
                    AnswerId = a.Id,
                    AnswerString = a.AnswerString,
                    Correct = a.Correct
                }).ToList()
                }).ToList();
                return Ok(new { data = result });
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitUserResponses(List<UserResponse> userResponses)
        {
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

            try
            {
                var results = new List<UserResponseResult>();

                UserResponse userResponse = null; // Definir la variable fuera del bucle

                foreach (var response in userResponses)
                {
                    userResponse = response; // Asignar el valor en cada iteración

                    // Verifica si la respuesta es correcta
                    var correctAnswer = await _context.Answers
                        .Where(a => a.QuestionId == userResponse.QuestionId && a.Correct == true)
                        .FirstOrDefaultAsync();

                    var isCorrect = correctAnswer != null && correctAnswer.Id == userResponse.SelectedAnswerId;

                    var userResponseResult = new UserResponseResult
                    {
                        QuestionId = userResponse.QuestionId,
                        IsCorrect = isCorrect
                    };

                    results.Add(userResponseResult);
                }

                // Calcula la calificación total
                var totalCorrect = results.Count(ur => ur.IsCorrect);
                var totalQuestions = results.Count;
                var userScore = (int)((decimal)totalCorrect / totalQuestions * 20);

                var score = new Score
                {
                    ScoreFinal = userScore,
                    UserId = userResponse.UserId, // Acceso a userResponse aquí
                    CourseId = userResponse.CourseId, // Acceso a userResponse aquí
                    EvaluationId = userResponse.EvaluationId, // Acceso a userResponse aquí
                };

                _context.Scores.Add(score);
                await _context.SaveChangesAsync();

                return Ok(new { Results = results, UserScore = userScore });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("save-score")]
        public async Task<IActionResult> SaveUserScore(ScoreDataDto dto)
        {
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

            // Buscar el registro existente en la base de datos
            var existingScore = await _context.Scores
                .FirstOrDefaultAsync(s =>
                    s.CourseId == dto.CourseId &&
                    s.UserId == dto.UserId &&
                    s.EvaluationId == dto.EvaluationId);

            if (existingScore != null)
            {
                // Si el registro ya existe, actualizar la calificación
                existingScore.ScoreFinal = dto.ScoreFinal;
            }
            else
            {
                // Si no hay un registro existente, crear uno nuevo
                var newScore = new Score
                {
                    ScoreFinal = dto.ScoreFinal,
                    UserId = dto.UserId,
                    CourseId = dto.CourseId,
                    EvaluationId = dto.EvaluationId
                };

                _context.Scores.Add(newScore);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Calificación registrada" });
        }
        [HttpGet("exams-by-unit/{id}")]
        public async Task<ActionResult<IEnumerable<Unit>>> GetExamsByUnit(int id){
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

            var exams = await _context.Evaluations.Where(e => e.UnitId == id).ToListAsync();

            return Ok(exams);
        }
    }

    public class ScoreDataDto
    {
        public int ScoreFinal {get; set;}
        public int UserId {get; set;}
        public int CourseId {get; set;}
        public int EvaluationId {get; set;}
    }

    internal class UserResponseResult
    {
        public int QuestionId { get; set; }
        public bool IsCorrect { get; set; }
    }
}