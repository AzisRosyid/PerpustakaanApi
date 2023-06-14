using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerpustakaanApi.Models;
using PerpustakaanApi.Models.Parameter;
using static PerpustakaanApi.Models.Enum.UserEnum;

namespace PerpustakaanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly ApiContext _context;

        public ProfilesController(ApiContext context)
        {
            _context = context;
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

        [HttpGet]
        [ProducesResponseType(typeof(GetUserParameter), 200)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MyProfile()
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return Unauthorized(new { errors = "Access Unauthorized!" }); }

            var user = _context.Users.Where(s => s.Id == valid.Id).FirstOrDefault();
            return Ok(new
            {
                User = new GetUserParameter
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = (UserRole)user.Role,
                    Gender = (UserGender)user.Gender,
                    DateOfBirth = user.DateOfBirth,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    Image = user.Image,
                    DateCreated = user.DateCreated,
                    DateUpdated = user.DateUpdated,
                }
            });
        }

        long favoriteId()
        {
            var st = _context.Favorites.OrderByDescending(s => s.Id).Select(s => s.Id);
            if (st.Any())
            {
                var id = st.FirstOrDefault();
                return id + 1;
            }
            else
            {
                return 1;
            }
        }

        [HttpGet("Favorite/{bookId}/{status}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Favorite(string bookId, bool status)
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return Unauthorized(new { errors = "Access Unauthorized!" }); }
            if (!_context.Books.Any(s => s.Id == bookId)) { return NotFound(new { errors = "Book Not Found!" }); }

            var favorite = _context.Favorites.Where(s => s.UserId == valid.Id && s.BookId == bookId);
            if(!status) { return Ok(new { Favorite = favorite.Any() }); }
            bool result;

            if (favorite.Any())
            {
                var id = favorite.FirstOrDefault();
                _context.Favorites.Remove(id);
                await _context.SaveChangesAsync();
                result = false;
            }
            else
            {
                var st = new Favorite();
                st.Id = favoriteId();
                st.UserId = (long)valid.Id;
                st.BookId = bookId;
                st.Date = DateTime.Now;
                _context.Favorites.Add(st);
                await _context.SaveChangesAsync();
                result = true;
            }

            return Ok(new { Favorite = result });
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PutProfile([FromForm] ProfileParameter profileParameter)
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return StatusCode(401, new { errors = "Access Unauthorized!" }); }
            if (!ModelState.IsValid) { return BadRequest(Method.error(ModelState)); }

            var st = _context.Users.Where(s => s.Id == valid.Id).FirstOrDefault();

            if (_context.Users.Any(s => s.Email == profileParameter.Email && s.Email != st.Email))
            {
                return Conflict(new { errors = "Email already exist!" });
            }

            string img = st.Image;
            if (profileParameter.Image != null)
            {
                var path = Path.GetExtension(profileParameter.Image.FileName);
                if (!(path == ".jpg" || path == ".png" || path == ".jpeg"))
                {
                    return StatusCode(400, new { errors = "Image format must be .png, .jpg, or .jpeg" });
                }

                if (!Directory.Exists(Method.profilePath))
                {
                    Directory.CreateDirectory(Method.profilePath);
                }

                img = DateTime.Now.ToString("yyyy_MM_dd_HHmmss_") + st.Email + path;

                using (var stream = new FileStream(Method.profilePath + img, FileMode.Create))
                {
                    if (st.Image != "nopict.png" && st.Image != null)
                    {
                        var file = new FileInfo(Method.profilePath + st.Image);
                        file.Delete();
                    }
                    profileParameter.Image.CopyTo(stream);
                }
            }

            st.Email = profileParameter.Email;
            st.Name = profileParameter.Name;
            if (profileParameter.Password != null)
            {
                st.Password = Method.Encrypt(profileParameter.Password);
            }
            st.Role = (int)profileParameter.Role;
            st.Gender = (int)profileParameter.Gender;
            st.DateOfBirth = profileParameter.DateOfBirth;
            st.PhoneNumber = profileParameter.PhoneNumber;
            st.Address = profileParameter.Address;
            st.Image = img;
            st.DateUpdated = DateTime.Now;
            _context.Entry(st).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return StatusCode(200, new { messages = "Profile successfully Updated!" });
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteProfile()
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return StatusCode(401, new { errors = "Access Unauthorized!" }); }

            foreach (var i in _context.Books.Where(s => s.UserId == valid.Id))
            {
                foreach (var j in _context.BookGenres.Where(s => s.BookId == i.Id))
                {
                    _context.BookGenres.Remove(j);
                }
                await _context.SaveChangesAsync();
                foreach (var j in _context.Favorites.Where(s => s.BookId == i.Id))
                {
                    _context.Favorites.Remove(j);
                }
                await _context.SaveChangesAsync();
                _context.Books.Remove(i);
                if (i.Image != "nopict.png" && i.Image != null)
                {
                    var img = new FileInfo(Method.imgBookPath + i.Image);
                    img.Delete();
                }
                var down = new FileInfo(Method.downBookPath + i.Download);
                down.Delete();
            }
            await _context.SaveChangesAsync();

            foreach(var i in _context.Favorites.Where(s => s.UserId == valid.Id))
            {
                _context.Favorites.Remove(i);
            }
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(valid.Id);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            if (user.Image != null && user.Image != "nopict.png")
            {
                var file = new FileInfo(Method.profilePath + user.Image);
                file.Delete();
            }

            return Ok(new { messages = "User successfully Deleted!" });
        }
    }
}
