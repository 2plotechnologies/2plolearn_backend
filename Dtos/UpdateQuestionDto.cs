using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Dtos
{
    public class UpdateQuestionDto
    {
        public int Id {get; set;}
        public string QuestionString {get; set;}
    }
}