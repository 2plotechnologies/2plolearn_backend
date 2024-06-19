using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Models
{
    public class User
    {
        public int id {get; set;}
        public string username {get; set;}
        public string email {get; set;}
        public string password {get; set;}
        public string rol {get; set;}
        public string? profile_pic {get; set;}
        public int CompanyId {get; set;}
        public Company company {get; set;}
        public ICollection<UserCourses> UserCourses { get; set; }
        public IEnumerable<Course> Courses {get; set;}
        public ICollection<Score> Scores { get; set; }
        public ICollection<Progress> Progresses { get; internal set; }
        public ICollection<Average> Averages { get; set; }
    }
}