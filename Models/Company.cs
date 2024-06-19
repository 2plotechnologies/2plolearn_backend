using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Models
{
    public class Company
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public string Image{get; set;}
        public IEnumerable<User> Users {get; set;}
        public IEnumerable<Course> Courses {get; set;}
    }
}