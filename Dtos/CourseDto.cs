using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Dtos
{
    public class CourseDto
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public IFormFile Image { get; set; }
        public int CompanyId{get; set;}
    }
}