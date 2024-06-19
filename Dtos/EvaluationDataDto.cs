using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Backend.Models;

namespace LMS.Backend.Dtos
{
    public class EvaluationDataDto
    {
        public int Id {get; set;}
        public string QuestionString {get; set;}
        public int EvaluationId {get; set;}
        public IEnumerable<Answer> Answers {get; set;}
    }
}