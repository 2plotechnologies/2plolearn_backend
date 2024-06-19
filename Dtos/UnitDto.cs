using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Backend.Models;

namespace LMS.Backend.Dtos
{
    public class UnitDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<LessonDto> Lessons { get; set; }
        public List<EvaluationDto> Evaluations { get; set; }
    }
}