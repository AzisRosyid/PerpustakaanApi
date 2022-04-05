using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerpustakaanApi.Models;

namespace PerpustakaanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApiContext _context;

        public AuthController(ApiContext context)
        {
            _context = context;
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> Login(LoginParameter loginParameter)
        {
            if (!ModelState.IsValid) { return StatusCode(400, Method.error(ModelState)); }
            var st = _context.Users.Where(s => s.Email == loginParameter.Email);
            if (st.Count() < 1) { return StatusCode(404, new { errors = "Email does Not Found!" }); }
            if (st.Count() > 0 && st.Where(s => s.Password == Method.Encrypt(loginParameter.Password)).Count() > 0)
            {
                var id = st.FirstOrDefault();
                return Ok(new { Token = Method.Encode(id) });
            }
            return StatusCode(401, new { errors = "Email and Password does not correct!" });
        }

        [HttpGet("RefreshToken")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken()
        {
            if (!Method.Decode(auth()).IsValid) { return Unauthorized(new { errors = "Access Unautorized!" }); }
            var st = _context.Users.Where(s => s.Id == Method.Decode(auth()).Id);
            var email = st.FirstOrDefault();
            return Ok(new { Token = Method.Encode(email) });
        }

        string auth()
        {
            Request.Headers.TryGetValue("Authorization", out var auth);
            try
            {
                return auth.ToString().Replace("Bearer ", "");
            }
            catch
            {
                return auth;
            }
        }

        long userId()
        {
            var st = _context.Users.OrderByDescending(s => s.Id).Select(s => s.Id);
            if (st.Any())
            {
                return st.FirstOrDefault() + 1;
            } 
            else
            {
                return 1;
            }
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromForm] UserParameter userParameter)
        {
            if (!ModelState.IsValid) { return BadRequest(Method.error(ModelState)); }

            if (_context.Users.Any(s => s.Email == userParameter.Email))
            {
                return Conflict(new { errors = "Email already exist!" });
            }

            string img = "nopict.png";
            if (userParameter.Image != null)
            {
                var ext = Path.GetExtension(userParameter.Image.FileName);
                if (!(ext == ".jpg" || ext == ".png" || ext == ".jpeg"))
                {
                    return StatusCode(400, new { errors = "Image format must be .png, .jpg, or .jpeg" });
                }

                if (!Directory.Exists(Method.profilePath))
                {
                    Directory.CreateDirectory(Method.profilePath);
                }

                img = "Profile" + DateTime.Now.ToString("_yyyy_MM_dd_HHmmss_") + userParameter.Email + ext;

                using (var stream = new FileStream(Method.profilePath + img, FileMode.Create))
                {
                    userParameter.Image.CopyTo(stream);
                }
            }

            var st = new User();
            st.Id = userId();
            st.Name = userParameter.Name;
            st.Email = userParameter.Email;
            st.Password = Method.Encrypt(userParameter.Password);
            st.Role = 1;
            st.Gender = (int)userParameter.Gender;
            st.DateOfBirth = DateTime.Parse(userParameter.DateOfBirth.ToString("yyyy-MM-dd"));
            st.PhoneNumber = userParameter.PhoneNumber;
            st.Address = userParameter.Address;
            st.Image = img;
            st.DateCreated = DateTime.Now;
            st.DateUpdated = DateTime.Now;
            _context.Users.Add(st);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { messages = "User successfully Registered!" });
        }
    }
}
