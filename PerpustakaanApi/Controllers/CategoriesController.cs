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
using static PerpustakaanApi.Models.Enum.OrderEnum;
using static PerpustakaanApi.Models.Enum.UserEnum;

namespace PerpustakaanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApiContext _context;

        public CategoriesController(ApiContext context)
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

        int categoryId()
        {
            var st = _context.Categories.OrderByDescending(s => s.Id).Select(s => s.Id);
            if (st.Any())
            {
                return st.FirstOrDefault() + 1;
            }
            else
            {
                return 1;
            }
        }

        // GET: api/Categories
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories(int? page = 1, int? pick = 20, string search = "", Order? order = null, bool android = false)
        {
            var st = new List<GetCategoryParameter>();
            var catagories = _context.Categories.Where(s => s.Id.ToString().Contains(search) || s.Name.Contains(search));
            foreach (var i in catagories)
            {
                st.Add(new GetCategoryParameter { Id = i.Id, Name = i.Name });
            }

            if (!st.Any())
            {
                return NotFound(new { errors = "Category Not Found!" });
            }

            if(order != Order.Ascending)
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
                    return Ok(new { TotalCategories = st.Count(), TotalPages = totalPage, Categories = st.Take((int)page * (int)pick).ToList() });
                }
                return Ok(new { TotalCategories = st.Count(), TotalPages = totalPage, Categories = st.Skip(((int)page * (int)pick) - (int)pick).Take((int)pick).ToList() });
            }
            else if (pick < 0)
            {
                return Ok(new { TotalCategories = st.Count(), TotalPages = totalPage, Categories = st.Take((int)pick).ToList() });
            }
            else
            {
                return Ok(new { TotalCategories = st.Count(), TotalPages = 1, Categories = await _context.Categories.ToListAsync() });
            }
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(new { errors = "Category Not Found!" });
            }

            return Ok(new
            {
                Category = new GetCategoryParameter
                {
                    Id = category.Id,
                    Name = category.Name
                }
            });
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutCategory(int id, [BindRequired, Required, StringLength(50)] string Name)
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return Unauthorized(new { errors = "Access Unauthorized!" }); }
            if (valid.Role != UserRole.Admin) { return StatusCode(403, new { errors = "User Role must be Admin!" }); }
            if (!ModelState.IsValid) { return BadRequest(Method.error(ModelState)); }
            if (!_context.Categories.Any(s => s.Id == id)) { return NotFound(new { errors = "Category Not Found!" }); }

            var st = _context.Categories.Where(s => s.Id == id).FirstOrDefault();
            st.Name = Name;

            _context.Entry(st).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new { messages = "Category successfully Updated!" }); ;
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Category>> PostCategory([BindRequired, Required, StringLength(50)] string Name)
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return Unauthorized(new { errors = "Access Unauthorized!" }); }
            if (valid.Role != UserRole.Admin) { return StatusCode(403, new { errors = "User Role must be Admin!" }); }
            if (!ModelState.IsValid) { return BadRequest(Method.error(ModelState)); }

            var st = new Category();
            st.Id = categoryId();
            st.Name = Name;

            _context.Categories.Add(st);
            await _context.SaveChangesAsync();

            return Created("Category", new { messages = "Category successfully Created!" });
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return Unauthorized(new { errors = "Access Unauthorized!" }); }
            if (valid.Role != UserRole.Admin) { return StatusCode(403, new { errors = "User Role must be Admin!" }); }

            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(new { errors = "Category Not Found!" }); 
            }

            var st = _context.Books.Where(s => s.Category == id);
            foreach(var i in st)
            {
                i.Category = null;
                await _context.SaveChangesAsync();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { messages = "Category successfully Deleted!" });
        }
    }
}
