using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Models
{
    public class Evaluation
    {
        public int Id {get; set;}
        public string Title {get; set;}
        public int UnitId {get; set;}
        public IEnumerable<Question> Questions {get; set;}
        public Unit Unit { get; set; }
        public ICollection<Score> Scores { get; set; }
    }
}