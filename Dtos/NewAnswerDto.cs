using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Dtos
{
    public class NewAnswerDto
    {
        public string AnswerString {get; set;}
        public bool Correct {get; set;}
        public int QuestionId {get; set;}
    }
}