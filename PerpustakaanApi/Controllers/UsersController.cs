#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerpustakaanApi.Models;
using PerpustakaanApi.Models.Parameter;
using static PerpustakaanApi.Models.Enum.OrderEnum;
using static PerpustakaanApi.Models.Enum.UserEnum;

namespace PerpustakaanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApiContext _context;

        public UsersController(ApiContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        [ProducesResponseType(typeof(GetUserParameter), 200)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<User>>> GetUser(int? page = 1, int? pick = 20, string search = "", UserRole? role = null, UserGender? gender = null, UserSort? sort = null, Order? order = null, bool android = false)
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return StatusCode(401, new { errors = "Access Unauthorized!" }); }
            if (valid.Role != UserRole.Admin) { return StatusCode(403, new { errors = "User Role must be Admin!" }); }

            int? roleSearch = null;
            int? genderSearch = null;
            try { roleSearch = (int)(UserRole)Enum.Parse(typeof(UserRole), search, true); } catch { }
            try { genderSearch = (int)(UserGender)Enum.Parse(typeof(UserGender), search, true); } catch { }

            var st = new List<GetUserParameter>();
            var users = _context.Users.Where(s => s.Id.ToString().Contains(search) || s.Email.Contains(search) || s.Name.Contains(search) || s.DateOfBirth.ToString().Contains(search) || s.PhoneNumber.Contains(search) || s.Address.Contains(search) || s.Image.Contains(search) || s.DateCreated.ToString().Contains(search) || s.DateUpdated.ToString().Contains(search) || s.Role == roleSearch || s.Gender == genderSearch );

            foreach(var i in users)
            {
                st.Add(new GetUserParameter
                {
                    Id = i.Id,
                    Name = i.Name,
                    Email = i.Email,
                    Role = (UserRole)i.Role,
                    Gender = (UserGender)i.Gender,
                    DateOfBirth = i.DateOfBirth,
                    PhoneNumber = i.PhoneNumber,
                    Address = i.Address,
                    Image = i.Image,
                    DateCreated = i.DateCreated,
                    DateUpdated = i.DateUpdated,
                });
            }

            if (role != null)
            {
                st = st.Where(s => s.Role == role).ToList();
            }
            if (gender != null)
            {
                st = st.Where(s => s.Gender == gender).ToList();
            }

            if (sort == UserSort.Id)
            {
                st = st.OrderBy(s => s.Id).ToList();
            }
            else if (sort == UserSort.Name)
            {
                st = st.OrderBy(s => s.Name).ToList();
            }
            else if (sort == UserSort.Email)
            {
                st = st.OrderBy(s => s.Email).ToList();
            }
            else if (sort == UserSort.Role)
            {
                st = st.OrderBy(s => s.Role).ToList();
            }
            else if (sort == UserSort.Gender)
            {
                st = st.OrderBy(s => s.Gender).AsEnumerable().Reverse().ToList();
            }
            else if (sort == UserSort.DateOfBirth)
            {
                st = st.OrderBy(s => s.DateOfBirth).AsEnumerable().Reverse().ToList();
            }
            else if (sort == UserSort.PhoneNumber)
            {
                st = st.OrderBy(s => s.PhoneNumber).ToList();
            }
            else if (sort == UserSort.DateCreated)
            {
                st = st.OrderBy(s => s.DateCreated).AsEnumerable().Reverse().ToList();
            }
            else if (sort == UserSort.DateUpdated)
            {
                st = st.OrderBy(s => s.DateUpdated).AsEnumerable().Reverse().ToList();
            }
            else
            {
                st = st.OrderBy(s => s.Id).AsEnumerable().Reverse().ToList();
            }

            if (order == Order.Descending)
            {
                st = st.AsEnumerable().Reverse().ToList();
            }


            if (!st.Any())
            {
                return NotFound(new { errors = "Search Not Found!" });
            }

            int totalPage = 0;

            if (pick > 0)
            {
                totalPage = st.Count() / (int)pick;
                if (st.Count % (int)pick != 0)
                {
                    totalPage++;
                }
            }

            if (page > 0 && pick > 0)
            {
                if (android)
                {
                    return Ok(new { TotalUsers = st.Count(), TotalPages = totalPage, Users = st.Take((int)pick * (int)page).ToList() });
                }
                return Ok(new { TotalUsers = st.Count(), TotalPages = totalPage, Users = st.Skip(((int)pick * (int)page) - (int)pick).Take((int)pick).ToList() });
            }
            else if (pick > 0)
            {
                return Ok(new { TotalUsers = st.Count(), TotalPages = totalPage, Users = st.Take((int)pick).ToList() });
            }
            else
            {
                return Ok(new { TotalUsers = st.Count(), TotalPages = 1, Users = st.ToList() });
            }
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetUserParameter), 200)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return StatusCode(401, new { errors = "Access Unauthorized!" }); }
            if (valid.Role != UserRole.Admin) { return StatusCode(403, new { errors = "User Role must be Admin!" }); }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return StatusCode(404, new { errors = "User not found!" });
            }

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

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PutUser(long id, [FromForm] UserParameter userParameter)
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return StatusCode(401, new { errors = "Access Unauthorized!" }); }
            if (valid.Role != UserRole.Admin) { return StatusCode(403, new { errors = "User Role must be Admin!" }); }
            if (!ModelState.IsValid) { return BadRequest(Method.error(ModelState)); }
            if (!UserExists(id))
            {
                return StatusCode(404, "User Not Found!");
            }

            var st = _context.Users.Where(s => s.Id == id).FirstOrDefault();

            if (_context.Users.Any(s => s.Email == userParameter.Email && s.Email != st.Email))
            {
                return Conflict(new { errors = "Email already exist!" });
            }

            string img = st.Image;
            if (userParameter.Image != null)
            {
                var path = Path.GetExtension(userParameter.Image.FileName);
                if (!(path == ".jpg" || path == ".png" || path == ".jpeg"))
                {
                    return StatusCode(400, new { errors = "Image format must be .png, .jpg, or .jpeg" });
                }

                if (!Directory.Exists(Method.profilePath))
                {
                    Directory.CreateDirectory(Method.profilePath);
                }

                img = "Profile" + DateTime.Now.ToString("_yyyy_MM_dd_HHmmss_") + st.Email + path;

                using (var stream = new FileStream(Method.profilePath + img, FileMode.Create))
                {
                    if (st.Image != "nopict.png" && st.Image != null)
                    {
                        var file = new FileInfo(Method.profilePath + st.Image);
                        file.Delete();
                    }
                    userParameter.Image.CopyTo(stream);
                }
            }

            st.Email = userParameter.Email;
            st.Name = userParameter.Name;
            st.Password = Method.Encrypt(userParameter.Password);
            st.Role = (int)userParameter.Role;
            st.Gender = (int)userParameter.Gender;
            st.DateOfBirth = userParameter.DateOfBirth;
            st.PhoneNumber = userParameter.PhoneNumber;
            st.Address = userParameter.Address;
            st.Image = img;
            st.DateUpdated = DateTime.Now;
            _context.Entry(st).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return StatusCode(200, new { messages = "User successfully Updated!" });
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<User>> PostUser([FromForm] UserParameter userParameter)
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return StatusCode(401, new { errors = "Access Unauthorized!" }); }
            if (valid.Role != UserRole.Admin) { return StatusCode(403, new { errors = "User Role must be Admin!" }); }
            if (!ModelState.IsValid) { return BadRequest(Method.error(ModelState)); }
            if (_context.Users.Any(s => s.Email == userParameter.Email))
            {
                return Conflict(new { errors = "Email already exist!" });
            }

            string img = "nopict.png";
            if (userParameter.Image != null)
            {
                var path = Path.GetExtension(userParameter.Image.FileName);
                if (!(path == ".jpg" || path == ".png" || path == ".jpeg"))
                {
                    return StatusCode(400, new { errors = "Image format must be .png, .jpg, or .jpeg" });
                }

                if (!Directory.Exists(Method.profilePath))
                {
                    Directory.CreateDirectory(Method.profilePath);
                }

                img = "Profile" + DateTime.Now.ToString("_yyyy_MM_dd_HHmmss_") + userParameter.Email + path;

                using (var stream = new FileStream(Method.profilePath + img, FileMode.Create))
                {
                    userParameter.Image.CopyTo(stream);
                }
            }

            var st = new User();
            st.Id = userId();
            st.Email = userParameter.Email;
            st.Name = userParameter.Name;
            st.Password = Method.Encrypt(userParameter.Password);
            st.Role = (int)userParameter.Role;
            st.Gender = (int)userParameter.Gender;
            st.DateOfBirth = userParameter.DateOfBirth;
            st.PhoneNumber = userParameter.PhoneNumber;
            st.Address = userParameter.Address;
            st.Image = img;
            st.DateCreated = DateTime.Now;
            st.DateUpdated = DateTime.Now;
            _context.Users.Add(st);

            await _context.SaveChangesAsync();

            return Created("User", new { messages = "User successfully Created!" });
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return StatusCode(401, new { errors = "Access Unauthorized!" }); }
            if (valid.Role != UserRole.Admin) { return StatusCode(403, new { errors = "User Role must be Admin!" }); }
            if (!UserExists(id))
            {
                return StatusCode(404, new { errors = "User Not Found!" });
            }

            foreach (var i in _context.Books.Where(s => s.UserId == id))
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

            foreach (var i in _context.Favorites.Where(s => s.UserId == valid.Id))
            {
                _context.Favorites.Remove(i);
            }
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(id);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            if (user.Image != null && user.Image != "nopict.png")
            {
                var file = new FileInfo(Method.profilePath + user.Image);
                file.Delete();
            }

            return Ok(new { messages = "User successfully Deleted!" });
        }

        [HttpGet("UserImage/{image}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UserImage(string image)
        {
            try { 
                var file = System.IO.File.ReadAllBytes(Method.profilePath + image);
                return File(file, "image/jpeg");
            }
            catch
            {
                return NotFound(new { errors = "User Image not found!" });
            } 
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
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
    }
}
