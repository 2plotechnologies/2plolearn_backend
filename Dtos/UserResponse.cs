using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Dtos
{
    public class UserResponse
    {
        public int UserResponseId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public int EvaluationId { get; set; }
        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; }
    }
}