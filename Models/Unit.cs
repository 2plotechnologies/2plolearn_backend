using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Models
{
    public class Unit
    {
        public int id {get; set;}
        public int CourseId {get; set;}
        public string Title {get; set;}
        public IEnumerable<Lesson> Lessons {get; set;}
        public IEnumerable<Evaluation> Evaluations {get; set;}
        public Course Course { get; set; }
        public ICollection<Progress> Progresses { get; internal set; }
    }
}