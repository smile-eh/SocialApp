using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")] // POST: api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto rDto)
        {
            if (await UserExists(rDto.Username))
            {
                return BadRequest("Username already exists");
            }
            var user = _mapper.Map<AppUser>(rDto);
            using var hmac = new HMACSHA512();

            user.UserName = rDto.Username;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rDto.Password));
            user.PasswordSalt = hmac.Key;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            };
        }

        [HttpPost("login")] // POST: api/account/login
        public async Task<ActionResult<UserDto>> Login(LoginDto lDto)
        {
            var user = await _context.Users.Include(p => p.Photos).Where(u => (u.UserName == lDto.Username)).FirstOrDefaultAsync();
            if (user == null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var cHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(lDto.Password));

            for (int i = 0; i < cHash.Length; i++)
            {
                if (cHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(currentUser => currentUser.UserName.ToLower() == username.ToLower());
        }
    }
}
