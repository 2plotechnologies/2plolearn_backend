using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Backend.Models;

namespace LMS.Backend.Data
{
    public interface IUserRepository
    {
        User Create(User user);
        User GetByEmail(string email);
    }
}