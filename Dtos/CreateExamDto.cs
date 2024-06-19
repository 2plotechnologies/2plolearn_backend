using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Dtos
{
    public class CreateExamDto
    {
        public string Title {get; set;}
        public int UnitId {get; set;}
    }
}