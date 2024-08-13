using Library.Models;
using Library.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using BrunoZell.ModelBinding;
using System.Text.Json.Nodes;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Library.CustomDataAnnotations;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Reflection.PortableExecutable;
using StackExchange.Redis;

namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrarianBookController : ControllerBase
    {
        #region Attributes
        private readonly ClaimVerifierService _ClaimVerifier;
        private LibraryDbContext _Context;

        public LibrarianBookController(ClaimVerifierService claimVerifier, LibraryDbContext ctx)
        {
            _ClaimVerifier = claimVerifier;
            Context = ctx;
        }
        public ClaimVerifierService ClaimVerifier
        {
            get { return _ClaimVerifier; }
        }

        public LibraryDbContext Context
        {
            get { return _Context; }
            set { _Context = value; }
        }
        #endregion

        #region Add a New Book (POST)
        [HttpPost]
        [Route("AddBook")]
        [Authorize]
        //public async Task<IActionResult> CreateBook([FromBody] Book newBook)
        //public async Task<IActionResult> CreateBook([FromForm] Book newBook, [FromForm] IFormFile cover)
        public async Task<IActionResult> CreateBook([ModelBinder(BinderType = typeof(JsonModelBinder))][FromForm] Book newBook, [FromForm] IFormFile newCover)
        //public async Task<IActionResult> CreateBook([ModelBinder(BinderType = typeof(JsonModelBinder))][FromForm] BookService newBook, [FromForm] IFormFile cover)
        {
            try
            {
                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (ClaimVerifier.ClaimID.StartsWith('L'))
                {
                    if (newBook == null || !ModelState.IsValid)
                    {
                        return BadRequest();
                    }

                    var bookDAL = await Context.Books.FirstOrDefaultAsync(book => book.Id == newBook.Id);

                    if (bookDAL != null)
                    {
                        return BadRequest();
                    }

                    if (CheckBookStorage(newBook.Title.Trim()) >= 3)
                    {
                        return BadRequest("The library is limited to 3 copies per book.");
                    }

                    newBook.Cover = ImageToByte(newCover);

                    Context.Books.Add(newBook);
                    await Context.SaveChangesAsync();

                    return Created("", $"\"{newBook.Title}\" has been added to the Library.");

                }
                if (Char.IsDigit(ClaimVerifier.ClaimID[0]))
                {
                    return Unauthorized("This user has no authorization to perform this action.");
                }

                return BadRequest("Invalid request.");


            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");

            }
        }

        private int CheckBookStorage(String title)
        {
            int matchQuantity = Context.Books.Count(book => book.Title == title.Trim());

            return matchQuantity;
        }

        //Convert the uploaded byte[] image to a varbinary
        private Byte[] ImageToByte(IFormFile uploadedFile)
        {
            if (uploadedFile.Length == 0)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                uploadedFile.CopyTo(memoryStream);

                return memoryStream.ToArray();
            }
        }

        private Boolean ValidateCoverExtension(IFormFile cover)
        {
            if (cover == null || cover.ContentType == null)
            {
                return false;
            }

            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

            var extension = Path.GetExtension(cover.FileName).ToLowerInvariant();

            if (allowedExtensions.Contains(extension))
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Update a Book (PUT)
        [HttpPut]
        [Route("UpdateBook")]
        [Authorize]
        public async Task<IActionResult> EditBook([ModelBinder(BinderType = typeof(JsonModelBinder))][FromForm] Book updateBook, [FromForm] IFormFile updateCover)
        {
            try
            {
                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (ClaimVerifier.ClaimID.StartsWith('L'))
                {
                    if (updateBook == null || !ModelState.IsValid)
                    {
                        return BadRequest();
                    }

                    var bookDAL = await Context.Books.FirstOrDefaultAsync(book => book.Id == updateBook.Id);


                    if (bookDAL == null)
                    {
                        return Conflict();
                    }

                    MappingPUT(bookDAL, updateBook, updateCover);

                    await Context.SaveChangesAsync();

                    //Check cache for updated book

                    return NoContent();
                    //return new ObjectResult("The book was updated successfully.") { StatusCode = 204 };

                }
                if (Char.IsDigit(ClaimVerifier.ClaimID[0]))
                {
                    return Unauthorized("This user has no authorization to perform this action.");
                }

                return BadRequest("Invalid request.");

            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");

            }
        }

        private void MappingPUT(Book bookDAL, Book updateBook, IFormFile updateCover)
        {
            //bookDAL.Id = updateBook.Id;
            bookDAL.Title = updateBook.Title;
            bookDAL.Author = updateBook.Author;
            bookDAL.Genre = updateBook.Genre;
            bookDAL.Year = updateBook.Year;
            bookDAL.Editorial = updateBook.Editorial;
            bookDAL.Available = updateBook.Available;
            bookDAL.Cover = ImageToByte(updateCover);
        }
        #endregion

        #region Remove a Book (DELETE)
        [HttpDelete("DeleteBook/{ID}")]
        [Authorize]
        public async Task<IActionResult> DeleteBook([FromRoute] String ID)
        {
            try
            {
                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (ClaimVerifier.ClaimID.StartsWith('L'))
                {
                    var bookDAL = await Context.Books.FirstOrDefaultAsync(book => book.Id == ID);

                    if (bookDAL != null)
                    {
                        //Check cache for deleted book

                        Context.Books.Remove(bookDAL);
                        await Context.SaveChangesAsync();

                        //return new ObjectResult("The book was removed successfully.") { StatusCode = 204 };
                        return NoContent();
                    }

                    return NotFound();
                }
                if (Char.IsDigit(ClaimVerifier.ClaimID[0]))
                {
                    return Unauthorized("This user has no authorization to perform this action.");
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }
        #endregion

        #region Read a Book (GET)
        [HttpGet("ViewBook/{ID}")]
        [Authorize]
        public async Task<ActionResult<Book>> DisplayBook([FromRoute] String ID)
        {
            try
            {

                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (ClaimVerifier.ClaimID.StartsWith('L'))
                {
                    var bookDAL = await Context.Books.FirstOrDefaultAsync(book => book.Id == ID);

                    if (bookDAL != null)
                    {
                        return bookDAL;
                    }

                    return NotFound();
                }
                if (Char.IsDigit(ClaimVerifier.ClaimID[0]))
                {
                    return Unauthorized("This user has no authorization to perform this action.");
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");

            }
        }
        #endregion

        //#region Read Books (GET)
        //[HttpGet]
        //[Route("ViewBooks")]
        //[Authorize]
        //public async Task<ActionResult<List<BooKService>>> DisplayBooks()
        //{
        //    try
        //    {
        //        using (LibraryDbContext context = new LibraryDbContext())
        //        {
        //            if (!ClaimValidation())
        //            {
        //                return Unauthorized("This user doesn't exist.");
        //            }

        //            if (Char.IsDigit(ClaimID[0]))
        //            {

        //                var allBooks = await context.Books.ToListAsync();

        //                if (allBooks.Any())
        //                {
        //                    var allBooksList = MappingAllBooks(allBooks);
        //                    return allBooksList;

        //                }

        //                return NotFound();
        //            }
        //            if (ClaimID.StartsWith('L'))
        //            {
        //                return Unauthorized("This user has no authorization to perform this action.");
        //            }

        //            return BadRequest();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("An exception has occurred: " + ex);

        //    }
        //}

        //private List<BooKService> MappingAllBooks(List<Book> aBooks)
        ////private ActionResult<List<BooKService>> MappingAllBooks(List<Book> aBooks)
        //{
        //    var bookServiceList = aBooks.Select(book => new BooKService()
        //    {
        //        Title = book.Title.Trim(),
        //        Author = book.Author.Trim(),
        //        Genre = book.Genre.Trim(),
        //        Year = (int)book.Year,
        //        Editorial = book.Editorial.Trim(),
        //        Available = book.Available,
        //        Cover = book.Cover

        //    }).ToList();

        //    return bookServiceList;
        //}
        //#endregion

        //#region Search a Book (GET)
        //[HttpGet("FindBook/{toSearch}")]
        //[Authorize]
        //public async Task<ActionResult<List<BooKService>>> SearchBook(String toSearch)
        //{
        //    try
        //    {
        //        using (LibraryDbContext context = new LibraryDbContext())
        //        {
        //            if (!ClaimValidation())
        //            {
        //                return Unauthorized("This user doesn't exist.");
        //            }

        //            if (Char.IsDigit(ClaimID[0]))
        //            {

        //                var allBooks = context.Books.AsQueryable();

        //                if (allBooks.Any())
        //                {
        //                    allBooks = allBooks.Where(book =>
        //                        book.Title.Contains(toSearch) ||
        //                        book.Author.Contains(toSearch) ||
        //                        book.Genre.Contains(toSearch) ||
        //                        book.Editorial.Contains(toSearch));

        //                    //var bookServiceList = allBooks.Select(book => new BooKService
        //                    //{
        //                    //    Title = book.Title.Trim(),
        //                    //    Author = book.Author.Trim(),
        //                    //    Genre = book.Genre.Trim(),
        //                    //    Year = (int)book.Year,
        //                    //    Editorial = book.Editorial.Trim(),
        //                    //    Available = book.Available,
        //                    //    Cover = book.Cover
        //                    //}).ToList();

        //                    //return bookServiceList;

        //                    var allBooksList = MappingAllBooksSearch(allBooks);
        //                    return allBooksList;

        //                }

        //                return NotFound();
        //            }
        //            if (ClaimID.StartsWith('L'))
        //            {
        //                return Unauthorized("This user has no authorization to perform this action.");
        //            }

        //            return BadRequest();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("An exception has occurred: " + ex);

        //    }
        //}

        ////private List<BooKService> MappingAllBooksSearch(List<Book> aBooks) 
        ////List<Book> aBooks
        //private ActionResult<List<BooKService>> MappingAllBooksSearch(IQueryable<Book> aBooks)
        //{
        //    var bookServiceList = aBooks.Select(book => new BooKService()
        //    {
        //        Title = book.Title.Trim(),
        //        Author = book.Author.Trim(),
        //        Genre = book.Genre.Trim(),
        //        Year = (int)book.Year,
        //        Editorial = book.Editorial.Trim(),
        //        Available = book.Available,
        //        Cover = book.Cover

        //    }).ToList();

        //    return bookServiceList;
        //}
        //#endregion
    }
}
