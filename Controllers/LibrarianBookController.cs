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
        public async Task<IActionResult> CreateBook([ModelBinder(BinderType = typeof(CustomBinderService))] Book newBook)
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

                    if (HelperService.CheckBookStorage(newBook.Title.Trim()) >= 3)
                    {
                        return BadRequest("The library is limited to 3 copies per book.");
                    }

                    //newBook.Cover = ImageToByte(newCover);
                    //newBook.Cover = ImageToByte(newBook.Cover);

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

        private Byte[] ImageToByte(IFormFile uploadedFile)
        {
            if (uploadedFile.Length == 0 || uploadedFile == null)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                uploadedFile.CopyTo(memoryStream);

                return memoryStream.ToArray();
            }
        }

        //private Boolean ValidateCoverExtension(IFormFile cover)
        //{
        //    if (cover == null || cover.ContentType == null)
        //    {
        //        return false;
        //    }

        //    var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

        //    var extension = Path.GetExtension(cover.FileName).ToLowerInvariant();

        //    if (allowedExtensions.Contains(extension))
        //    {
        //        return true;
        //    }

        //    return false;
        //}
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

                    var bookDAL = await Context.Books.FirstOrDefaultAsync(book => book.Id.Trim() == updateBook.Id.Trim());

                    if (bookDAL == null)
                    {
                        return Conflict();
                    }

                    MappingPUT(bookDAL, updateBook, updateCover);

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

        private void MappingPUT(Book bookDAL, Book updateBook, IFormFile updateCover)
        {
            bookDAL.Title = updateBook.Title?.Trim();
            bookDAL.Author = updateBook.Author?.Trim();
            bookDAL.Genre = updateBook.Genre?.Trim();
            bookDAL.Year = updateBook.Year;
            bookDAL.Editorial = updateBook.Editorial?.Trim();
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
                        //CacheManagerService.CheckDelete(bookDAL);
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
