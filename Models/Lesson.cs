using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Models
{
    public class Lesson
    {
        public int Id {get; set;}
        public string Title {get; set;}
        public string Content {get; set;}
        public int UnitId {get; set;}
        public Unit Unit {get; set;}
    }
}