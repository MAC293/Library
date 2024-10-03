using Library.Models;
using Library.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BrunoZell.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrarianBookController : ControllerBase
    {
        #region Attributes
        private ClaimVerifierService _ClaimVerifier;
        private LibraryDbContext _Context;
        private HelperService _HelperService;
        private CacheManagerService _CacheManagerService;

        public LibrarianBookController(ClaimVerifierService cv, LibraryDbContext ctx, HelperService hs, CacheManagerService cms)
        {
            ClaimVerifier = cv;
            Context = ctx;
            HelperService = hs;
            CacheManagerService = cms;
        }
        public ClaimVerifierService ClaimVerifier
        {
            get { return _ClaimVerifier; }
            set { _ClaimVerifier = value; }
        }

        public LibraryDbContext Context
        {
            get { return _Context; }
            set { _Context = value; }
        }

        public HelperService HelperService
        {
            get { return _HelperService; }
            set { _HelperService = value; }
        }
        
        public CacheManagerService CacheManagerService
        {
            get { return _CacheManagerService; }
            set { _CacheManagerService = value; }
        }
        #endregion

        #region Add a New Book (POST)
        [HttpPost]
        [Route("AddBook")]
        [Authorize]
        //public async Task<IActionResult> CreateBook([ModelBinder(BinderType = typeof(JsonModelBinder))][FromForm] Book newBook, [FromForm] IFormFile newCover)
        public async Task<IActionResult> CreateBook([ModelBinder(BinderType = typeof(CustomBinderService))] BookCoverService incomingBook)
        {
            try
            {
                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (ClaimVerifier.ClaimID.StartsWith('L'))
                {
                    if (incomingBook == null || !ModelState.IsValid)
                    {
                        return BadRequest();
                    }

                    var bookDAL = await Context.Books.FirstOrDefaultAsync(book => book.Id == incomingBook.ID);

                    if (bookDAL != null)
                    {
                        return BadRequest();
                    }

                    if (HelperService.CheckBookStorage(incomingBook.Title.Trim()) >= 3)
                    {
                        return BadRequest("The library is limited to 3 copies per book.");
                    }

                    Context.Books.Add(MappingBookCover(incomingBook));
                    await Context.SaveChangesAsync();

                    return Created("", $"\"{incomingBook.Title}\" has been added to the Library.");

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
        private Book MappingBookCover(BookCoverService coverBook)
        {
            var newBook = new Book()
            {
                Id = coverBook.ID.Trim(),
                Title = coverBook.Title.Trim(),
                Author = coverBook.Author.Trim(),
                Genre = coverBook.Genre.Trim(),
                Year = (int)coverBook.Year,
                Editorial = coverBook.Editorial.Trim(),
                Available = coverBook.Available,
                Cover = coverBook.Cover
            };

            return newBook;
        }
        #endregion

        #region Update a Book (PUT)
        [HttpPut]
        [Route("UpdateBook")]
        [Authorize]
        public async Task<IActionResult> EditBook([ModelBinder(BinderType = typeof(CustomBinderService))] BookCoverService incomingBook)
        {
            try
            {
                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (ClaimVerifier.ClaimID.StartsWith('L'))
                {
                    if (incomingBook == null || !ModelState.IsValid)
                    {
                        return BadRequest();
                    }

                    var bookDAL = await Context.Books.FirstOrDefaultAsync(book => book.Id.Trim() == incomingBook.ID.Trim());

                    if (bookDAL == null)
                    {
                        return Conflict();
                    }

                    MappingPUT(bookDAL, incomingBook);

                    await Context.SaveChangesAsync();

                    CacheManagerService.HasBook(bookDAL);

                    return NoContent();

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

        private void MappingPUT(Book bookDAL, BookCoverService updateBook)
        {
            bookDAL.Title = updateBook.Title?.Trim();
            bookDAL.Author = updateBook.Author?.Trim();
            bookDAL.Genre = updateBook.Genre?.Trim();
            bookDAL.Year = updateBook.Year;
            bookDAL.Editorial = updateBook.Editorial?.Trim();
            bookDAL.Available = updateBook.Available;
            bookDAL.Cover = updateBook.Cover;
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
                        Context.Books.Remove(bookDAL);

                        CacheManagerService.HasBook(bookDAL);
                        CacheManagerService.DeleteBook(bookDAL);

                        await Context.SaveChangesAsync();

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
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: "+ex);
            }
        }
        #endregion

        #region Read a Book (GET)
        [HttpGet("ViewBook/{ID}")]
        [Authorize]
        public async Task<ActionResult<BookService>> DisplayBook([FromRoute] String ID)
        {
            try
            {
                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (ClaimVerifier.ClaimID.StartsWith('L'))
                {
                    var isCacheBook = CacheManagerService.CacheService.Get<BookService>($"book:{ID}");

                    if (isCacheBook != null)
                    {
                        return isCacheBook;
                    }

                    var bookDAL = await Context.Books.FirstOrDefaultAsync(book => book.Id == ID);

                    if (bookDAL != null)
                    {
                        CacheManagerService.CacheService.Set(ID, MappingBook(bookDAL));

                        return MappingBook(bookDAL);
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

        private BookService MappingBook(Book displayBook)
        {
            var newBook = new BookService()
            {
                Title = displayBook.Title.Trim(),
                Author = displayBook.Author.Trim(),
                Genre = displayBook.Genre.Trim(),
                Year = (int)displayBook.Year,
                Editorial = displayBook.Editorial.Trim(),
                Available = displayBook.Available,
                Cover = displayBook.Cover
            };

            return newBook;
        }
        #endregion
    }
}
