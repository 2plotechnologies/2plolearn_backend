using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Models
{
    public class Question
    {
        public int Id {get; set;}
        public string QuestionString {get; set;}
        public int EvaluationId {get; set;}
        public Evaluation Evaluation {get; set;}
        public IEnumerable<Answer> Answers {get; set;}
    }
}