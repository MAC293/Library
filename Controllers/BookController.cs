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

namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        #region Attributes
        private CacheService _CacheService;
        private String _ClaimID;

        public BookController(CacheService cacheService)
        {
            CacheService = cacheService;
        }

        public String ClaimID
        {
            get { return _ClaimID; }
            set { _ClaimID = value; }
        }
        public CacheService CacheService
        {
            get { return _CacheService; }
            set { _CacheService = value; }
        }
        #endregion

        #region Claim
        private Boolean ClaimValidation()
        {
            AssignClaim();

            if (HasClaim())
            {
                return true;
            }

            return false;
        }
        private void AssignClaim()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            ClaimID = userIdClaim.Value;
        }

        private Boolean HasClaim()
        {
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (ClaimID == String.Empty)
            {
                return false;
            }
            //ClaimID = userIdClaim.Value;
            return true;
        }

        //Check if Claim is valid (L/0)
        private Boolean ValidClaim()
        {
            if (ClaimID.StartsWith('L') || Char.IsDigit(ClaimID[0]))
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Add a New Book (POST)
        [HttpPost]
        [Route("AddBook")]
        //This ensures that the JWT token is validated before the method is executed.
        [Authorize]
        //public async Task<IActionResult> CreateBook([FromBody] Book newBook)
        //public async Task<IActionResult> CreateBook([FromForm] Book newBook, [FromForm] IFormFile cover)
        public async Task<IActionResult> CreateBook([ModelBinder(BinderType = typeof(JsonModelBinder))][FromForm] Book newBook, [FromForm] IFormFile newCover)
        //public async Task<IActionResult> CreateBook([ModelBinder(BinderType = typeof(JsonModelBinder))][FromForm] BookService newBook, [FromForm] IFormFile cover)
        {
            try
            {
                using (LibraryDbContext context = new LibraryDbContext())
                {
                    if (!ClaimValidation())
                    {
                        return Unauthorized("This user doesn't exist.");
                    }

                    //newBook.Cover = cover;

                    //if (newBook == null || !ModelState.IsValid)
                    //{
                    //    return BadRequest();
                    //}

                    //newBook.Cover = ImageToByte(cover);

                    //Assign IFormFile to byte[] Cover property
                    //newBook.Cover = ImageToByte(cover);
                    //Trigger cover data validation

                    //newBook.Cover = ImageToByte(cover);

                    //Validate the cover file using the custom attribute

                    //newBook.Cover = cover;

                    //if (ClaimID.StartsWith('L'))
                    if (ClaimID.StartsWith('L'))
                    {
                        if (newBook == null || !ModelState.IsValid)
                        {
                            return BadRequest();
                        }

                        var bookDAL = await context.Books.FirstOrDefaultAsync(book => book.Id == newBook.Id);

                        if (bookDAL != null)
                        {
                            return BadRequest();
                        }

                        //if (!ValidateCoverExtension(cover))
                        //{
                        //    return BadRequest("Book cover is required.");
                        //}

                        //Image to Byte[]                       
                        //newBook.Cover = ImageToByte(cover);

                        newBook.Cover = ImageToByte(newCover);

                        context.Books.Add(newBook);
                        await context.SaveChangesAsync();

                        return Created("", newBook.Title + " has been added to the Library.");

                    }
                    //if (Char.IsDigit(ClaimID[0]))
                    if (Char.IsDigit(ClaimID[0]))
                    {
                        return Unauthorized("This user has no authorization to perform this action.");
                    }

                    return BadRequest("Invalid request.");

                }
            }
            catch (Exception ex)
            {
                return BadRequest("An exception has occurred: " +ex);

            }
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
                using (LibraryDbContext context = new LibraryDbContext())
                {
                    if (!ClaimValidation())
                    {
                        return Unauthorized("This user doesn't exist.");
                    }

                    if (ClaimID.StartsWith('L'))
                    {
                        if (updateBook == null || !ModelState.IsValid)
                        {
                            return BadRequest();
                        }

                        var bookDAL = await context.Books.FirstOrDefaultAsync(book => book.Id == updateBook.Id);


                        if (bookDAL == null)
                        {
                            return Conflict();
                        }

                        MappingPUT(bookDAL, updateBook, updateCover);

                        await context.SaveChangesAsync();

                        //Check cache for updated book

                        return NoContent();
                        //return new ObjectResult("The book was updated successfully.") { StatusCode = 204 };

                    }
                    if (Char.IsDigit(ClaimID[0]))
                    {
                        return Unauthorized("This user has no authorization to perform this action.");
                    }

                    return BadRequest("Invalid request.");


                }

            }
            catch (Exception ex)
            {
                return BadRequest("An exception has occurred: " +ex);

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
                using (LibraryDbContext context = new LibraryDbContext())
                {
                    if (!ClaimValidation())
                    {
                        return Unauthorized("This user doesn't exist.");
                    }

                    if (ClaimID.StartsWith('L'))
                    {
                        var bookDAL = await context.Books.FirstOrDefaultAsync(book => book.Id == ID);

                        if (bookDAL != null)
                        {
                            //Check cache for deleted book

                            context.Books.Remove(bookDAL);
                            await context.SaveChangesAsync();

                            //return new ObjectResult("The book was removed successfully.") { StatusCode = 204 };
                            return NoContent();
                        }

                        return NotFound();
                    }
                    if (Char.IsDigit(ClaimID[0]))
                    {
                        return Unauthorized("This user has no authorization to perform this action.");
                    }

                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest("An exception has occurred: " +ex);

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
                using (LibraryDbContext context = new LibraryDbContext())
                {
                    if (!ClaimValidation())
                    {
                        return Unauthorized("This user doesn't exist.");
                    }

                    if (ClaimID.StartsWith('L'))
                    {
                        //Check cache for obtained book

                        var bookDAL = await context.Books.FirstOrDefaultAsync(book => book.Id == ID);

                        if (bookDAL != null)
                        {
                            return bookDAL;
                        }

                        return NotFound();
                    }
                    if (Char.IsDigit(ClaimID[0]))
                    {
                        return Unauthorized("This user has no authorization to perform this action.");
                    }

                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest("An exception has occurred: " + ex);

            }
        }
        #endregion
    }
}
