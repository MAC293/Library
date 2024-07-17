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

namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        #region Attributes
        private CacheService _CacheService;
        private String _ClaimID;
        //private LibraryDbContext context;

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

                        if (CheckBookStorage(newBook.Title) < 3)
                        {
                            return BadRequest("You can't add more than 3 books.");
                        }

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
                return BadRequest("An exception has occurred: " + ex);

            }
        }

        private int CheckBookStorage(String title)
        {
            using (LibraryDbContext context = new LibraryDbContext())
            {
                int matchQuantity = context.Books.Count(book => book.Title == title.Trim());

                return matchQuantity;
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
                return BadRequest("An exception has occurred: " + ex);

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
                return BadRequest("An exception has occurred: " + ex);

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

        #region Borrow a Book (GET)
        [HttpGet("BorrowBook/{titleToBorrow}")]
        [Authorize]
        public async Task<ActionResult<BorrowedBookService>> AcquireBook(String titleToBorrow)
        {
            try
            {
                using (LibraryDbContext context = new LibraryDbContext())
                {
                    if (!ClaimValidation())
                    {
                        return Unauthorized("This user doesn't exist.");
                    }

                    if (Char.IsDigit(ClaimID[0]))
                    {
                        if (CheckBookStorage(titleToBorrow.Trim()) == 0)
                        {
                            return NotFound("This book is not available in the library, yet.");
                        }

                        var bookDAL = await context.Books.FirstOrDefaultAsync(book => book.Title == titleToBorrow);

                        if (bookDAL.Available)
                        {
                            //context.Borrows.Add(newBook);
                            //await context.SaveChangesAsync();

                            LoadBorrowInformation(bookDAL, context);

                            bookDAL.Available = false;
                            context.SaveChanges();

                            //Console.WriteLine(bookDAL.Available);

                            return MapDALToServiceBook(bookDAL);
                        }

                        return NotFound(titleToBorrow + "is not available. You have to wait until a reader returns a copy.");

                    }
                    if (ClaimID.StartsWith('L'))
                    {
                        return Unauthorized("Only readers can borrow books.");
                    }

                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest("An exception has occurred: " + ex);

            }
        }

        private BorrowedBookService MapDALToServiceBook(Book availableBook)
        {
            BorrowedBookService borrowedBook = new BorrowedBookService()
            {
                Title = availableBook.Title,
                Author = availableBook.Author,
                Genre = availableBook.Genre,
                Year = (int)availableBook.Year,
                Editorial = availableBook.Editorial,
                Cover = availableBook.Cover,
                BorrowDate = BorrowDate(),
                DueDate = DueDate()
                //Information = borrowInformation

            };

            return borrowedBook;
        }

        //private BorrowInformationService MapDALToBorrowInformationService()
        //{
        //    BorrowInformationService borrowInformation = new BorrowInformationService()
        //    {
        //        BorrowDate = BorrowDate(),
        //        DueDate = DueDate()
        //    };

        //    return borrowInformation;
        //}

        //private DateTime BorrowDate()
        //{
        //    DateTime borrowDate = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"), CultureInfo.InvariantCulture);

        //    return  borrowDate;
        //}

        private String BorrowDate()
        {
            //String borrowDate = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"), CultureInfo.InvariantCulture);

            String borrowDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            return borrowDate;
        }

        //private DateTime DueDate()
        //{
        //    DateTime borrowDate = BorrowDate();
        //    DateTime dueDate = borrowDate.AddDays(10);

        //    return  dueDate;
        //}

        private String DueDate()
        {
            DateTime dtDate = StrToDate(BorrowDate());
            var dueDate = dtDate.AddDays(7);
            String strDate = DateToStr(dueDate);

            return strDate;
        }

        private DateTime StrToDate(String date)
        {
            return DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        public String DateToStr(DateTime date)
        {
            return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        private void LoadBorrowInformation(Book book, LibraryDbContext context)
        {
            //using (LibraryDbContext context = new LibraryDbContext())
            //{

            //var bookDAL = await context.Borrows.FirstOrDefaultAsync(book => true);
            Borrow borrowDAL = context.Borrows.FirstOrDefault(borrow => borrow.Book == book.Id);

            //var borrowDAL = context.Borrows.FirstOrDefault(borrow => borrow.Book == book.Id);

            if (borrowDAL == null)
            {
                Borrow borrow = new Borrow();


                borrow.Id = BorrowID(book);
                borrow.BorrowDate = DateTime.Now;
                borrow.DueDate = DateTime.Now;
                borrow.ReturnDate = null;
                borrow.Reader = ReaderID(ClaimID);
                borrow.Book = book.Id;

                context.Borrows.Add(borrow);
                context.SaveChanges();
            }
            //}

        }

        //Extract from Think&GrowRich-NapoleonHill-1965-Wealth-N°1, to Think&GrowRich-N°1                  
        private String BorrowID(Book book)
        {
            //String bookName = book.Id.Substring(0, book.Id.IndexOf('-'));
            //String bookCopy = book.Id.Substring(0, book.Id.LastIndexOf('-') + 1);

            //return bookName + "-" + bookCopy;

            int firstHyphenIndex = book.Id.IndexOf("-");
            String bookTitle = book.Id.Substring(0, firstHyphenIndex).Trim();

            int lastHyphenIndex = book.Id.LastIndexOf("-");
            String copyNumber = book.Id.Substring(lastHyphenIndex + 1).Trim();

            return $"{bookTitle}-{copyNumber}";
        }

        private String ReaderID(String claim)
        {
            String endUserIDCleaned = claim.Replace("EndUser", "Reader");

            return endUserIDCleaned;
        }

        //private String BorrowReader()
        //{

        //}

        //private  String BorrowBook(Book book)
        //{

        //}
        #endregion
    }
}
