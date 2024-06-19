using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Dtos
{
    public class LessonDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}