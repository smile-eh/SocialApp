using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")] // POST: api/account/register
        public async Task<ActionResult<AppUser>> Register(RegisterDto rDto)
        {
            if (await UserExists(rDto.Username))
            {
                return BadRequest("Username already exists");
            }
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = rDto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rDto.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(currentUser => currentUser.UserName.ToLower() == username.ToLower());
        }
    }
}
