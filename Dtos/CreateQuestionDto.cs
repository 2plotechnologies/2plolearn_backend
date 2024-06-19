using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Dtos
{
    public class CreateQuestionDto
    {
        public string QuestionString {get; set;}
        public int EvaluationId {get; set;}
    }
}