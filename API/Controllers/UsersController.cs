using System.Runtime.CompilerServices;
using System.Security.Claims;
using API.Data;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            this.photoService = photoService;
            this.mapper = mapper;
            this.userRepository = userRepository;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await userRepository.GetMembersAsync();

            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await userRepository.GetMemberByUsernameAsync(username);

            if (user == null)
            {
                return BadRequest("User not found");
            }
            else
            {
                return user;
            }
        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUsername();
            var user = await userRepository.GetUserByUsernameAsync(username);
            if (user == null) return NotFound();
            mapper.Map(memberUpdateDto, user);
            if (await userRepository.SaveAllAsync()) return NoContent();
            else return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var username = User.GetUsername();
            var user = await userRepository.GetUserByUsernameAsync(username);
            if (user == null) return NotFound();
            var result = await photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count < 1) photo.IsMain = true;
            user.Photos.Add(photo);
            if (await userRepository.SaveAllAsync())
            {
                return CreatedAtAction(
                    nameof(GetUser),
                    new { username = user.UserName },
                    mapper.Map<PhotoDto>(photo)
                );
            }
            return BadRequest("Cannot Add Photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var username = User.GetUsername();
            var user = await userRepository.GetUserByUsernameAsync(username);
            if (user == null) return NotFound();
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("This is already your main photo");
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            if (await userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update the main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> deletePhoto(int photoId)
        {
            var username = User.GetUsername();
            var user = await userRepository.GetUserByUsernameAsync(username);
            if (user == null) return NotFound();
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("You cannot delete your main photo");
            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);
            if (await userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Failed to delete the photo");
        }
    }
}