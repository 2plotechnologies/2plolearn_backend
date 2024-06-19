using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Models
{
    public class Answer
    {
        public int Id {get; set;}
        public string AnswerString {get; set;}
        public bool Correct {get; set;}
        public int QuestionId {get; set;}
        public Question Question {get; set;}
    }
}