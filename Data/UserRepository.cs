using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Backend.Data;
using LMS.Backend.Models;

namespace LMS.Backend.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;
        public UserRepository(UserContext context){
            _context = context;
        }
        public User Create(User user){
            _context.Users.Add(user);
            user.id = _context.SaveChanges();
            return user;
        }
        public User GetByEmail(string email){
            return _context.Users.FirstOrDefault(u => u.email == email);
        }
    }
}