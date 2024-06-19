using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Models
{
    public class Progress
    {
        public int Id {get; set;}
        public int UserId {get; set;}
        public int CourseId {get; set;}
        public int UnitId{get; set;}
        public User User {get; set;}
        public Course Course {get; set;}
        public Unit Unit { get; set; }
    }
}