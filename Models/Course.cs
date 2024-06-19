using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Models
{
    public class Course
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public string Description {get; set;}
        public string Image {get; set;}
        public int CompanyId {get; set;}
        public Company company {get; set;}
        public IEnumerable<User> Users {get; set;}
        public ICollection<UserCourses> UserCourses { get; set; }
        public IEnumerable<Unit> Units {get; set;}
        public IEnumerable<Progress> Progresses { get; set; }
        public ICollection<Score> Scores { get; set; }
        public ICollection<Average> Averages { get; set; }
    }
}