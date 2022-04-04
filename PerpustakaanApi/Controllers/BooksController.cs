#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerpustakaanApi.Models;
using PerpustakaanApi.Models.Parameter;
using static PerpustakaanApi.Models.Enum.BookEnum;
using static PerpustakaanApi.Models.Enum.OrderEnum;
using static PerpustakaanApi.Models.Enum.UserEnum;

namespace PerpustakaanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApiContext _context;

        public BooksController(ApiContext context)
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

        string bookId()
        {
            var st = _context.Books.OrderByDescending(s => s.Id).Select(s => s.Id);
            if (st.Any())
            {
                var id = st.FirstOrDefault();
                var count = Convert.ToInt64(id.Substring(id.Length - 20, 20)) + 1;
                var idCount = "00000000000000000000" + count;
                return "BOOK" + idCount.Substring(idCount.Length - 20, 20);
            }
            else
            {
                return "BOOK00000000000000000001";
            }
        }

        long bookGenreId()
        {
            var st = _context.BookGenres.OrderByDescending(s => s.Id).Select(s => s.Id);
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

        // GET: api/Books
        [HttpPost("GetBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks([FromForm] SearchBookParameter searchBookParameter)
        {
            string searchParameter = searchBookParameter.Search;
            if (searchParameter == null) { searchParameter = String.Empty; }

            var st = new List<GetBookParameter>();
            var total = new List<Book>();
            var searchList = new List<string>();

            searchList.Add(searchParameter);
            searchList.AddRange(searchParameter.Replace("&&", "").Replace(" ", "").Split("||").ToList());
            searchList.AddRange(searchParameter.Replace("&&", "").Replace("||", "").Split(" ").ToList());
            searchList.AddRange(searchParameter.Replace(" ", "").Replace("||", "").Split("&&").ToList());

            foreach (var search in searchList)
            {
                if(search != "")
                {
                    var st1 = from s in _context.Books
                              join c in _context.Categories on s.Category equals c.Id
                              where c.Name.Contains(search)
                              select s;
                    var st2 = from s in _context.Books
                              join t in _context.BookGenres on s.Id equals t.BookId
                              join g in _context.Genres on t.GenreId equals g.Id
                              where g.Name.Contains(search)
                              select s;
                    var st3 = from s in _context.Books
                              join u in _context.Users on s.UserId equals u.Id
                              where s.Id.Contains(search) || s.Title.Contains(search) || s.Author.Contains(search) || s.Publisher.Contains(search) || s.Description.Contains(search) || s.Page.ToString().Contains(search) || s.Image.Contains(search) || s.Download.Contains(search) || s.DateCreated.ToString().Contains(search) || s.DateUpdated.ToString().Contains(search) || u.Name.Contains(search)
                              select s;

                    total.AddRange(st1); total.AddRange(st2); total.AddRange(st3);
                }
            }

            if (!total.Any())
            {
                var search = searchParameter;
                var st1 = from s in _context.Books
                          join c in _context.Categories on s.Category equals c.Id
                          where c.Name.Contains(search)
                          select s;
                var st2 = from s in _context.Books
                          join t in _context.BookGenres on s.Id equals t.BookId
                          join g in _context.Genres on t.GenreId equals g.Id
                          where g.Name.Contains(search)
                          select s;
                var st3 = from s in _context.Books
                          join u in _context.Users on s.UserId equals u.Id
                          where s.Id.Contains(search) || s.Title.Contains(search) || s.Author.Contains(search) || s.Publisher.Contains(search) || s.Description.Contains(search) || s.Page.ToString().Contains(search) || s.Image.Contains(search) || s.Download.Contains(search) || s.DateCreated.ToString().Contains(search) || s.DateUpdated.ToString().Contains(search) || u.Name.Contains(search)
                          select s;

                total.AddRange(st1); total.AddRange(st2); total.AddRange(st3);
            }

            var totalCom = new List<Book>(); totalCom.AddRange(total);
            total.Clear();
            foreach(var i in totalCom)
            {
                if(!total.Any(s => s.Id == i.Id))
                {
                    total.Add(i);
                }
            }

            var searchList1 = new List<string>(); var searchList2 = new List<string>();
            searchList1.AddRange(searchParameter.Replace("||", "").Replace("&&", "").Split(" ").ToList());
            searchList2.AddRange(searchParameter.Replace("||", "").Replace(" ", "").Split("&&").ToList());

            if(searchList1.Count > 1 || searchList2.Count > 1)
            {
                var totalSort = new List<Book>(); totalSort.AddRange(total);
                var bookSort = new List<Book>();
                total.Clear();
                
                if(searchList1.Count > 1)
                {
                    var st1 = new List<Book>(); var st2 = new List<Book>(); var st3 = new List<Book>();
                    st1.AddRange(totalSort); st2.AddRange(totalSort); st3.AddRange(totalSort);
                   
                    foreach (var search in searchList1)
                    {
                        if (search != "")
                        {
                            var st1c = _context.Categories.Where(s => s.Name.Contains(search)).OrderBy(s => s.Name).FirstOrDefault();
                            long st1cId;
                            if (st1c == null) { st1cId = 0; } else { st1cId = st1c.Id; }
                            st1 = st1.Where(s => s.Category == st1cId).ToList();

                            var st2g = _context.Genres.Where(s => s.Name.Contains(search)).OrderBy(s => s.Name).FirstOrDefault();
                            long st2gId;
                            if (st2g == null) { st2gId = 0; } else { st2gId = st2g.Id; }
                            var st2bg = _context.BookGenres.Where(s => s.GenreId == st2gId).ToList();
                            st2 = st2.Where(s => st2bg.Any(x => x.BookId == s.Id)).ToList();

                            st3 = st3.Where(s => s.Id.Contains(search) || s.Title.Contains(search) || s.Author.Contains(search) || s.Publisher.Contains(search) || s.Description.Contains(search) || s.Page.ToString().Contains(search) || s.Image.Contains(search) || s.Download.Contains(search) || s.DateCreated.ToString().Contains(search) || s.DateUpdated.ToString().Contains(search) || _context.Users.Where(x => x.Id == s.UserId).FirstOrDefault().Name.Contains(search)).ToList();
                        }
                    }
                    bookSort.AddRange(st1); bookSort.AddRange(st2); bookSort.AddRange(st3);
                }

                if(searchList2.Count > 1)
                {
                    var st1 = new List<Book>(); var st2 = new List<Book>(); var st3 = new List<Book>();
                    st1.AddRange(totalSort); st2.AddRange(totalSort); st3.AddRange(totalSort);
                    foreach (var search in searchList2)
                    {
                        if (search != "")
                        {
                            var st1c = _context.Categories.Where(s => s.Name.Contains(search)).OrderBy(s => s.Name).FirstOrDefault();
                            long st1cId;
                            if (st1c == null) { st1cId = 0; } else { st1cId = st1c.Id; }
                            st1 = st1.Where(s => s.Category == st1cId).ToList();

                            var st2g = _context.Genres.Where(s => s.Name.Contains(search)).OrderBy(s => s.Name).FirstOrDefault();
                            long st2gId;
                            if (st2g == null) { st2gId = 0; } else { st2gId = st2g.Id; }
                            var st2bg = _context.BookGenres.Where(s => s.GenreId == st2gId).ToList();
                            st2 = st2.Where(s => st2bg.Any(x => x.BookId == s.Id)).ToList();

                            st3 = st3.Where(s => s.Id.Contains(search) || s.Title.Contains(search) || s.Author.Contains(search) || s.Publisher.Contains(search) || s.Description.Contains(search) || s.Page.ToString().Contains(search) || s.Image.Contains(search) || s.Download.Contains(search) || s.DateCreated.ToString().Contains(search) || s.DateUpdated.ToString().Contains(search) || _context.Users.Where(x => x.Id == s.UserId).FirstOrDefault().Name.Contains(search)).ToList();
                        }
                    }
                    bookSort.AddRange(st1); bookSort.AddRange(st2); bookSort.AddRange(st3);
                }

                foreach (var i in bookSort)
                {
                    if (!total.Any(s => s.Id == i.Id))
                    {
                        total.Add(i);
                    }
                }
            }
            
            foreach(var i in total)
            {
                if(!st.Any(s => s.Id == i.Id))
                {
                    var genres1 = from s in _context.Books
                                  join t in _context.BookGenres on s.Id equals t.BookId
                                  join g in _context.Genres on t.GenreId equals g.Id
                                  where s.Id == i.Id
                                  orderby g.Name
                                  select g;
                    var genreParameter = new List<GetGenreParameter>();
                    foreach(var j in genres1)
                    {
                        genreParameter.Add(new GetGenreParameter
                        {
                            Id = j.Id,
                            Name = j.Name,
                        });
                    }
                    var category1 = _context.Categories.Where(s => s.Id == i.Category);
                    var categoryParameter = new GetCategoryParameter();
                    if(category1.Any())
                    {
                        var id = category1.FirstOrDefault();
                        categoryParameter = new GetCategoryParameter
                        {
                            Id = id.Id,
                            Name = id.Name,
                        };
                    };
                    var user1 = _context.Users.Where(s => s.Id == i.UserId).FirstOrDefault();
                    st.Add(new GetBookParameter
                    {
                        Id = i.Id,
                        User = new GetUserParameter
                        {
                            Id = user1.Id,
                            Name = user1.Name,
                            Email = user1.Email,
                            Role = (UserRole)user1.Role,
                            Gender = (UserGender)user1.Gender,
                            DateOfBirth = user1.DateOfBirth,
                            PhoneNumber = user1.PhoneNumber,
                            Address = user1.Address,
                            Image = user1.Image,
                            DateCreated = user1.DateCreated,
                            DateUpdated = user1.DateUpdated,
                        },
                        Title = i.Title,
                        Category = categoryParameter,
                        Genres = genreParameter,
                        Author = i.Author,
                        Publisher = i.Publisher,
                        Description = i.Description,
                        Page = i.Page,
                        Download = i.Download,
                        Image = i.Image,
                        ViewCount = i.ViewCount,
                        DateCreated = i.DateCreated,
                        DateUpdated = i.DateUpdated,
                    });
                }
            };

            st = st.OrderBy(s => s.Id).ToList();

            if (_context.Users.Any(s => s.Id == searchBookParameter.User))
            {
                if (searchBookParameter.Favorite == true)
                {
                    var favorite = new List<GetBookParameter>();
                    favorite.AddRange(st);
                    st.Clear();
                    foreach (var i in _context.Favorites.Where(s => s.UserId == searchBookParameter.User).ToList())
                    {
                        st.Add(favorite.Where(s => s.Id == i.BookId).FirstOrDefault());
                    }
                }
                else
                {
                    st = st.Where(s => s.User.Id == searchBookParameter.User).ToList();
                }
            }
            if (_context.Categories.Any(s => s.Id == searchBookParameter.Category))
            {
                st = st.Where(s => s.Category.Id == searchBookParameter.Category).ToList();
            }

            if (searchBookParameter.Genres != null)
            {
                foreach (var i in searchBookParameter.Genres)
                {
                    if (_context.Genres.Any(s => s.Id == i))
                    {
                        st = st.Where(s => s.Genres.Where(s => s.Id == i).Any()).ToList();
                    }
                }
            }

            if (searchBookParameter.Start != null)
            {
                st = st.Where(s => s.DateCreated >= searchBookParameter.Start).ToList();
            }

            if (searchBookParameter.End != null)
            {
                st = st.Where(s => s.DateUpdated <= searchBookParameter.End).ToList();
            }

            if (searchBookParameter.Sort == BookSort.Popularity)
            {
                st = st.OrderBy(s => s.ViewCount).AsEnumerable().Reverse().ToList();
            }
            else if (searchBookParameter.Sort == BookSort.Favorite)
            {
                st = st.OrderBy(s => _context.Favorites.Where(x => x.BookId == s.Id).Count()).AsEnumerable().Reverse().ToList();
            }
            else if (searchBookParameter.Sort == BookSort.Id)
            {
                st = st.OrderBy(s => s.Id).ToList();
            }
            else if (searchBookParameter.Sort == BookSort.Title)
            {
                st = st.OrderBy(s => s.Title).ToList();
            }
            else if (searchBookParameter.Sort == BookSort.Author)
            {
                st = st.OrderBy(s => s.Author).ToList();
            }
            else if (searchBookParameter.Sort == BookSort.Publisher)
            {
                st = st.OrderBy(s => s.Publisher).ToList();
            }
            else if (searchBookParameter.Sort == BookSort.TotalPage)
            {
                st = st.OrderBy(s => s.Page).AsEnumerable().Reverse().ToList();
            }
            else if (searchBookParameter.Sort == BookSort.DateCreated)
            {
                st = st.OrderBy(s => s.DateCreated).AsEnumerable().Reverse().ToList();
            }
            else if (searchBookParameter.Sort == BookSort.DateUpdated)
            {
                st = st.OrderBy(s => s.DateUpdated).AsEnumerable().Reverse().ToList();
            }
            else
            {
                st = st.OrderBy(s => s.Id).AsEnumerable().Reverse().ToList();
            }

            if (searchBookParameter.Order == Order.Descending)
            {
                st = st.AsEnumerable().Reverse().ToList();
            } 

            if (!st.Any())
            {
                return NotFound(new { errors = "Search Not Found!" });
            }

            int? page = searchBookParameter.Page;
            int? pick = searchBookParameter.Pick;

            int totalPage = 0;

            if (pick > 0)
            {
                totalPage = st.Count() / (int)pick;
                if (st.Count % (int)pick != 0)
                {
                    totalPage++;
                }
            }

            if (searchBookParameter.Content == BookContent.Lite)
            {
                var id = st.Select(s => new
                {
                    Id = s.Id,
                    Title = s.Title,
                    Author = s.Author,
                    Image = s.Image
                });

                if (page > 0 && pick > 0)
                {
                    if (searchBookParameter.Android)
                    {
                        return Ok(new { TotalBooks = st.Count(), TotalPages = totalPage, Books = id.Take((int)pick * (int)page).ToList() });
                    }
                    return Ok(new { TotalBooks = st.Count(), TotalPages = totalPage, Books = id.Skip(((int)pick * (int)page) - (int)pick).Take((int)pick).ToList() });
                }
                else if (pick > 0)
                {
                    return Ok(new { TotalBooks = st.Count(), TotalPages = totalPage, Books = id.Take((int)pick).ToList() });
                }
                else
                {
                    return Ok(new { TotalBooks = st.Count(), TotalPages = 1, Books = id.ToList() });
                }
            }

            if (searchBookParameter.Content == BookContent.Mid)
            {
                var id = st.Select(s => new
                {
                    Id = s.Id,
                    Title = s.Title,
                    Category = s.Category,
                    Genres = s.Genres,
                    Author = s.Author,
                    Publisher = s.Publisher,
                    Page = s.Page,
                    ViewCount = s.ViewCount,
                    Image = s.Image,
                    Description = s.Description,
                    DateCreated = s.DateCreated,
                    DateUpdated = s.DateUpdated,
                });

                if (page > 0 && pick > 0)
                {
                    if (searchBookParameter.Android)
                    {
                        return Ok(new { TotalBooks = st.Count(), TotalPages = totalPage, Books = id.Take((int)pick * (int)page).ToList() });
                    }
                    return Ok(new { TotalBooks = st.Count(), TotalPages = totalPage, Books = id.Skip(((int)pick * (int)page) - (int)pick).Take((int)pick).ToList() });
                }
                else if (pick > 0)
                {
                    return Ok(new { TotalBooks = st.Count(), TotalPages = totalPage, Books = id.Take((int)pick).ToList() });
                }
                else
                {
                    return Ok(new { TotalBooks = st.Count(), TotalPages = 1, Books = id.ToList() });
                }
            }

            if (page > 0 && pick > 0)
            {
                if (searchBookParameter.Android)
                {
                    return Ok(new { TotalBooks = st.Count(), TotalPages = totalPage, Books = st.Take((int)pick * (int)page).ToList() });
                }
                return Ok(new { TotalBooks = st.Count(), TotalPages = totalPage, Books = st.Skip(((int)pick * (int)page) - (int)pick).Take((int)pick).ToList() }); 
            }
            else if (pick > 0)
            {
                return Ok(new { TotalBooks = st.Count(), TotalPages = totalPage, Books = st.Take((int)pick).ToList() });
            }
            else
            {
                return Ok(new { TotalBooks = st.Count(), TotalPages = 1, Books = st.ToList() });
            }
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Book>> GetBook(string id)
        {
            if (!BookExists(id))
            {
                return NotFound(new { errors = "Book Not Found!" });
            }

            var genres = from s in _context.Books
                         join t in _context.BookGenres on s.Id equals t.BookId
                         join g in _context.Genres on t.GenreId equals g.Id
                         where s.Id == id
                         orderby g.Name
                         select g;
            var genreParameter = new List<GetGenreParameter>();
            foreach (var i in genres)
            {
                genreParameter.Add(new GetGenreParameter
                {
                    Id = i.Id,
                    Name = i.Name,
                });
            }
            var category = _context.Categories.Where(s => s.Id == _context.Books.Where(s => s.Id == id).Select(s => s.Category).FirstOrDefault());
            var categoryParameter = new GetCategoryParameter();
            if (category.Any())
            {
                categoryParameter = new GetCategoryParameter
                {
                    Id = category.FirstOrDefault().Id,
                    Name = category.FirstOrDefault().Name,
                };
            };
            var book = from s in _context.Books
                       join u in _context.Users on s.UserId equals u.Id
                       where s.Id == id
                       select new { s, u };

            var st = book.FirstOrDefault();

            var result = new
            {
                Book = new GetBookParameter
                {
                    Id = st.s.Id,
                    User = new GetUserParameter
                    {
                        Id = st.u.Id,
                        Name = st.u.Name,
                        Email = st.u.Email,
                        Role = (UserRole)st.u.Role,
                        Gender = (UserGender)st.u.Gender,
                        DateOfBirth = st.u.DateOfBirth,
                        PhoneNumber = st.u.PhoneNumber,
                        Address = st.u.Address,
                        Image = st.u.Image,
                        DateCreated = st.u.DateCreated,
                        DateUpdated = st.u.DateUpdated,
                    },
                    Title = st.s.Title,
                    Category = categoryParameter,
                    Genres = genreParameter,
                    Author = st.s.Author,
                    Publisher = st.s.Publisher,
                    Description = st.s.Description,
                    Page = st.s.Page,
                    Download = st.s.Download,
                    Image = st.s.Image,
                    ViewCount = st.s.ViewCount,
                    DateCreated = st.s.DateCreated,
                    DateUpdated = st.s.DateUpdated
                }
            };

            st.s.ViewCount += 1;
            _context.Entry(st.s).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(result);
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutBook(string id, [FromForm] BookParameter bookParameter)
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return Unauthorized(new { errors = "Access Unauthorized!" }); }
            if (!BookExists(id)) { return NotFound(new { errors = "Book Not Found!" }); }
            if (valid.Role != UserRole.Admin && _context.Books.Where(s => s.Id == id).Select(s => s.UserId).FirstOrDefault() != valid.Id) { return StatusCode(403, new { errors = "User Role must be Admin or Owner!" }); }
            if (!ModelState.IsValid) { return BadRequest(Method.error(ModelState)); }
            if (!_context.Categories.Any(s => s.Id == bookParameter.Category && bookParameter.Category != null)) { return BadRequest(new { errors = "Category Id Not Valid!" }); }

            var genres = new List<int>();

            if (bookParameter.Genres != null)
            {
                foreach (var i in bookParameter.Genres)
                {
                    if (!_context.Genres.Any(s => s.Id == i))
                    {
                        return BadRequest(new { errors = "Genre Id Not Valid!" });
                    }
                    genres.Add(i);
                }
            }
            else
            {
                try
                {
                    genres = bookParameter.GenreIds.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',').Select(s => Convert.ToInt32(s)).ToList();
                    foreach(var i in genres)
                    {
                        if(!_context.Genres.Any(s => s.Id == i))
                        {
                            return BadRequest(new { errors = "Genre Id Not Valid!" });
                        }
                    }
                }
                catch
                {
                    return BadRequest(new { errors = "Genre Id Not Valid!" });
                }
            }

            long user = (long)valid.Id;

            if (bookParameter.User != null && valid.Role == UserRole.Admin)
            {
                user = (long)bookParameter.User;
            }


            var st = _context.Books.Where(s => s.Id == id).FirstOrDefault();
            if (bookParameter.Download != null)
            {
                string ex = Path.GetExtension(bookParameter.Download.FileName);
                if (ex != ".pdf") { return BadRequest(new { errors = "Download format must be PDF!" }); }
            }

            string imgBook = st.Image;
            if (bookParameter.Image != null)
            {
                string path = Path.GetExtension(bookParameter.Image.FileName);
                if (!(path == ".png" || path == ".jpg" || path == ".jpeg")) { return BadRequest(new { errors = "Image format must be png, jpg, or jpeg!" }); }
                if (!Directory.Exists(Method.imgBookPath))
                {
                    Directory.CreateDirectory(Method.imgBookPath);
                }
                imgBook = "Image" + DateTime.Now.ToString("_yyyy_MM_dd_HHmmss_") + st.Id + path;
                using (var stream = new FileStream(Method.imgBookPath + imgBook, FileMode.Create))
                {
                    if (st.Image != "nopick.png" && st.Image != null)
                    {
                        var fi = new FileInfo(Method.imgBookPath + imgBook);
                        fi.Delete();
                    }
                    bookParameter.Image.CopyTo(stream);
                }
            }

            string downBook = st.Download;
            if (bookParameter.Download != null)
            {
                if (!Directory.Exists(Method.downBookPath))
                {
                    Directory.CreateDirectory(Method.downBookPath);
                }
                downBook = DateTime.Now.ToString("yyyy_MM_dd_HHmmss_") + st.Id + Path.GetExtension(bookParameter.Download.FileName);
                using (var stream = new FileStream(Method.downBookPath + downBook, FileMode.Create))
                {
                    if (st.Download != null)
                    {
                        var info = new FileInfo(Method.downBookPath + st.Download);
                        info.Delete();
                    }
                    bookParameter.Download.CopyTo(stream);
                }
            }

            st.UserId = user;
            st.Title = bookParameter.Title;
            st.Category = bookParameter.Category;
            st.Author = bookParameter.Author;
            st.Publisher = bookParameter.Publisher;
            st.Description = bookParameter.Description;
            st.Page = bookParameter.Page;
            st.Download = downBook;
            st.Image = imgBook;
            st.DateUpdated = DateTime.Now;
            _context.Entry(st).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            foreach (var i in _context.BookGenres.Where(s => s.BookId == id))
            {
                _context.BookGenres.Remove(i);
            }
            await _context.SaveChangesAsync();

            foreach (var i in genres)
            {
                if (!_context.BookGenres.Any(s => s.BookId == st.Id && s.GenreId == i))
                {
                    var bg = new BookGenre();
                    bg.Id = bookGenreId();
                    bg.BookId = st.Id;
                    bg.GenreId = i;
                    _context.BookGenres.Add(bg);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(new { messages = "Book successfully Updated!" }); ;
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Book>> PostBook([FromForm] BookParameter bookParameter)
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return Unauthorized(new { errors = "Access Unauthorized!" }); } 
            if (!ModelState.IsValid) { return BadRequest(Method.error(ModelState)); }
            if (!_context.Categories.Any(s => s.Id == bookParameter.Category && bookParameter.Category != null)) { return BadRequest(new { errors = "Category Id Not Valid!"}); }

            var genres = new List<int>();

            if (bookParameter.Genres != null)
            {
                foreach (var i in bookParameter.Genres)
                {
                    if (!_context.Genres.Any(s => s.Id == i))
                    {
                        return BadRequest(new { errors = "Genre Id Not Valid!" });
                    }
                    genres.Add(i);
                }
            }
            else
            {
                try
                {
                    genres = bookParameter.GenreIds.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',').Select(s => Convert.ToInt32(s)).ToList();
                    foreach (var i in genres)
                    {
                        if (!_context.Genres.Any(s => s.Id == i))
                        {
                            return BadRequest(new { errors = "Genre Id Not Valid!" });
                        }
                    }
                }
                catch
                {
                    return BadRequest(new { errors = "Genre Id Not Valid!" });
                }
            }

            long user = (long)valid.Id;

            if (bookParameter.User != null && valid.Role == UserRole.Admin)
            {
                user = (long)bookParameter.User;
            }

            if (bookParameter.Download == null) { return BadRequest(new { errors = "Download File cannot Empty!" }); }
            string ex = Path.GetExtension(bookParameter.Download.FileName);
            if (ex != ".pdf") { return BadRequest(new { errors = "Download format must be PDF!" }); }

            string imgBook = "nopict.png";
            if (bookParameter.Image != null)
            {
                string path = Path.GetExtension(bookParameter.Image.FileName);
                if (!(path == ".png" || path == ".jpg" || path == ".jpeg")) { return BadRequest(new { errors = "Image format must be png, jpg, or jpeg!" }); }
                if (!Directory.Exists(Method.imgBookPath))
                {
                    Directory.CreateDirectory(Method.imgBookPath);
                }
                imgBook = "Image" + DateTime.Now.ToString("_yyyy_MM_dd_HHmmss_") + bookId() + path;
                using (var stream = new FileStream(Method.imgBookPath + imgBook, FileMode.Create))
                {
                    bookParameter.Image.CopyTo(stream);
                }
            }

            string downBook = "Download" + DateTime.Now.ToString("_yyyy_MM_dd_HHmmss_") + bookId() + ex;
            if (!Directory.Exists(Method.downBookPath))
            {
                Directory.CreateDirectory(Method.downBookPath);
            }
            using (var stream = new FileStream(Method.downBookPath + downBook, FileMode.Create))
            {
                bookParameter.Download.CopyTo(stream);
            }

            var st = new Book();
            st.Id = bookId();
            st.UserId = user;
            st.Title = bookParameter.Title;
            st.Category = bookParameter.Category;
            st.Author = bookParameter.Author;
            st.Publisher = bookParameter.Publisher;
            st.Description = bookParameter.Description;
            st.Page = bookParameter.Page;
            st.Download = downBook;
            st.Image = imgBook;
            st.ViewCount = 0;
            st.DateCreated = DateTime.Now;
            st.DateUpdated = DateTime.Now;
            _context.Books.Add(st);
            await _context.SaveChangesAsync();

            foreach (var i in genres)
            {
                if (!_context.BookGenres.Any(s => s.BookId == st.Id && s.GenreId == i))
                {
                    var bg = new BookGenre();
                    bg.Id = bookGenreId();
                    bg.BookId = st.Id;
                    bg.GenreId = i;
                    _context.BookGenres.Add(bg);
                    await _context.SaveChangesAsync();
                }
            }
            

            return Created("Book", new { messages = "Book successfully Created!" });
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBook(string id)
        {
            var valid = Method.Decode(auth());
            if (!valid.IsValid) { return Unauthorized(new { errors = "Access Unauthorized!" }); }
            if (!BookExists(id)) { return NotFound(new { errors = "Book Not Found!" }); }
            if (valid.Role != UserRole.Admin && _context.Books.Where(s => s.Id == id).Select(s => s.UserId).FirstOrDefault() != valid.Id) { return StatusCode(403, new { errors = "User Role must be Admin or Owner!" }); }

            foreach(var i in _context.BookGenres.Where(s => s.BookId == id))
            {
                _context.BookGenres.Remove(i);
            }
            await _context.SaveChangesAsync();

            foreach (var i in _context.Favorites.Where(s => s.BookId == id))
            {
                _context.Favorites.Remove(i);
            }
            await _context.SaveChangesAsync();

            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            if(book.Image != "nopict.png" && book.Image != null)
            {
                var img = new FileInfo(Method.imgBookPath + book.Image);
                img.Delete();
            }
            var down = new FileInfo(Method.downBookPath + book.Download);
            down.Delete();

            return Ok(new { messages = "Book successfully Deleted!" }); 
        }

        [HttpGet("ImageBook/{image}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ImageBook(string image)
        {
            try
            {
                var file = System.IO.File.ReadAllBytes(Method.imgBookPath + image);
                return File(file, "image/jpeg");
            }
            catch
            {
                return NotFound(new { errors = "Book Image Not Found!" });
            }
        }

        [HttpGet("DownloadBook/{download}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadBook(string download)
        {
            try
            {
                var file = System.IO.File.ReadAllBytes(Method.downBookPath + download);
                return File(file, "application/pdf");
            }
            catch
            {
                return NotFound(new { errors = "Book Download Not Found!" });
            }
        }

        private bool BookExists(string id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
