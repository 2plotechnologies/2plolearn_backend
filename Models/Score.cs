using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Models
{
    public class Score
    {
        public int Id {get; set;}
        public int ScoreFinal {get; set;}
        public int UserId {get; set;}
        public int CourseId {get; set;}
        public int EvaluationId {get; set;}
        public User User {get; set;}
        public Course Course {get; set;}
        public Evaluation Evaluation {get; set;}
    }
}