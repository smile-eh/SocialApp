using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BugController : BaseApiController
    {
        private DataContext _context;
        public BugController(DataContext context)
        {
            _context = context;

        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var res = _context.Users.Find(-1);
            if (res == null) return NotFound();
            return res;
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var res = _context.Users.Find(-1);
            var resToReturn = res.ToString();
            return resToReturn;
        }
        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("Bad Request");
        }
    }
}