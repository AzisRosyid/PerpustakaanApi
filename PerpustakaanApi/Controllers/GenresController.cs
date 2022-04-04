#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PerpustakaanApi.Models;
using PerpustakaanApi.Models.Parameter;
using static PerpustakaanApi.Models.Enum.GenreEnum;
using static PerpustakaanApi.Models.Enum.OrderEnum;
using static PerpustakaanApi.Models.Enum.UserEnum;

namespace PerpustakaanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApiContext _context;

        public GenresController(ApiContext context)
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

        int genreId()
        {
            var st = _context.Genres.OrderByDescending(s => s.Id).Select(s => s.Id);
            if (st.Any())
            {
                return st.FirstOrDefault() + 1;
            }
            else
            {
                return 1;
            }
        }

        // GET: api/Genres
        [HttpGet]
        [ProducesResponseType(typeof(GetGenreParameter), 200)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenres(int? page = 1, int? pick = 20, string search = "", GenreSort? sort = null, Order? order = null, bool android = false)
        {
            var st = new List<GetGenreParameter>();
            var genres = _context.Genres.Where(s => s.Id.ToString().Contains(search) || s.Name.Contains(search)).Select(s => new
            {
                Id = s.Id,
                Name = s.Name,
                Tags = _context.BookGenres.Where(x => x.GenreId == s.Id).Count()
            });
            
            foreach(var i in genres)
            {
                st.Add(new GetGenreParameter { Id = i.Id, Name = i.Name, Tags = i.Tags });
            }

            if (!st.Any())
            {
                return NotFound(new { errors = "Genre Not Found!" });
            }

            if(sort == GenreSort.Popularity)
            {
                st = st.OrderBy(s => s.Name).AsEnumerable().Reverse().ToList();
                st = st.OrderBy(s => s.Tags).AsEnumerable().Reverse().ToList();
            }
            else if (sort == GenreSort.Id)
            {
                st = st.OrderBy(s => s.Id).ToList();
            }
            else if (sort == GenreSort.Name)
            {
                st = st.OrderBy(s => s.Name).ToList();
            }
            else
            {
                st = st.OrderBy(s => s.Id).AsEnumerable().Reverse().ToList();
            }

            if (order == Order.Descending)
            {
                st = st.AsEnumerable().Reverse().ToList();
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
                    return Ok(new { TotalGenres = st.Count(), TotalPages = totalPage, Genres = st.Take((int)page * (int)pick).ToList() });
                }
                return Ok(new { TotalGenres = st.Count(), TotalPages = totalPage, Genres = st.Skip(((int)page * (int)pick) - (int)pick).Take((int)pick).ToList() });
            }
            else if (pick < 0)
            {
                return Ok(new { TotalGenres = st.Count(), TotalPages = totalPage, Genres = st.Take((int)pick).ToList() });
            }
            else
            {
                return Ok(new { TotalGenres = st.Count(), TotalPages = 1, Genres = await _context.Categories.ToListAsync() });
            }
        }

        // GET: api/Genres/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Genre>> GetGenre(int id)
        {
            var genre = await _context.Categories.FindAsync(id);
 
            if (genre == null)
            {
                return NotFound(new { errors = "Genre Not Found!" });
            }

            return Ok(new
            {
                Genre = new GetGenreParameter
                {
                    Id = genre.Id,
                    Name = genre.Name
                }
            });
        }

        // PUT: api/Genres/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutGenre(int id, GenreParameter genreParameter)
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return Unauthorized(new { errors = "Access Unauthorized!" }); }
            if (valid.Role != UserRole.Admin) { return StatusCode(403, new { errors = "User Role must be Admin!" }); }
            if (!ModelState.IsValid) { return BadRequest(Method.error(ModelState)); }
            if (!_context.Genres.Any(s => s.Id == id)) { return NotFound(new { errors = "Genre Not Found!" }); }

            var st = _context.Genres.Where(s => s.Id == id).FirstOrDefault();
            st.Name = genreParameter.Name;

            _context.Entry(st).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new { messages = "Genre successfully Updated!" }); ;
        }

        // POST: api/Genres
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Genre>> PostGenre(GenreParameter genreParameter)
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return Unauthorized(new { errors = "Access Unauthorized!" }); }
            if (valid.Role != UserRole.Admin) { return StatusCode(403, new { errors = "User Role must be Admin!" }); }
            if (!ModelState.IsValid) { return BadRequest(Method.error(ModelState)); }

            var st = new Genre();
            st.Id = genreId();
            st.Name = genreParameter.Name;

            _context.Genres.Add(st);
            await _context.SaveChangesAsync();

            return Created("Genre", new { messages = "Genre successfully Created!" });
        }

        // DELETE: api/Genres/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return Unauthorized(new { errors = "Access Unauthorized!" }); }
            if (valid.Role != UserRole.Admin) { return StatusCode(403, new { errors = "User Role must be Admin!" }); }

            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(new { errors = "Category Not Found!" });
            }

            var st = _context.BookGenres.Where(s => s.GenreId == id);
            foreach(var i in st)
            {
                _context.BookGenres.Remove(i);
                await _context.SaveChangesAsync();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { messages = "Genre successfully Deleted!" });
        }
    }
}
