using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Backend.Dtos
{
    public class RegisterDto
    {
        public string username {get; set;}
        public string email {get; set;}
        public string password {get; set;}
        public string rol {get; set;}
        public string? profile_pic {get; set;}
        public int CompanyId {get; set;}
    }
}