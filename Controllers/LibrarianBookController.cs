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

        //#region Borrow a Book (GET)
        //[HttpGet("BorrowBook/{bookToBorrow}")]
        //[Authorize]
        //public async Task<ActionResult<BorrowedBookService>> AcquireBook(String bookToBorrow)
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
        //                if (CheckBookStorage(bookToBorrow.Trim()) == 0)
        //                {
        //                    return NotFound("This book is not available in the library, yet.");
        //                }

        //                var bookDAL = await context.Books.FirstOrDefaultAsync(book => book.Title.Trim() == bookToBorrow.Trim() && book.Available);

        //                //bool isAnyBookAvailable = await context.Books.AnyAsync(book => book.Title == bookToBorrow && book.Available);
        //                //var anyAvailable = await context.Books.AnyAsync(book => book.Title == bookToBorrow && book.Available);

        //                if (bookDAL.Available)
        //                {
        //                    //context.Borrows.Add(newBook);
        //                    //await context.SaveChangesAsync();

        //                    LoadBorrowInformation(bookDAL, context);

        //                    bookDAL.Available = false;
        //                    context.SaveChanges();

        //                    //Console.WriteLine(bookDAL.Available);

        //                    return MapDALToServiceBook(bookDAL);
        //                }

        //                //return NotFound(bookToBorrow + "is not available. You have to wait until a reader returns a copy.");
        //                //'{bookToBorrow}'
        //                return NotFound($"\"{bookToBorrow}\" is not available. You have to wait until a reader returns a copy.");

        //            }
        //            if (ClaimID.StartsWith('L'))
        //            {
        //                return Unauthorized("Only readers can borrow books.");
        //            }

        //            return BadRequest();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("An exception has occurred: " + ex);

        //    }
        //}

        //private BorrowedBookService MapDALToServiceBook(Book availableBook)
        //{
        //    BorrowedBookService borrowedBook = new BorrowedBookService()
        //    {
        //        Title = availableBook.Title,
        //        Author = availableBook.Author,
        //        Genre = availableBook.Genre,
        //        Year = (int)availableBook.Year,
        //        Editorial = availableBook.Editorial,
        //        Cover = availableBook.Cover,
        //        BorrowDate = BorrowDate(),
        //        DueDate = DueDate()
        //        //Information = borrowInformation

        //    };

        //    return borrowedBook;
        //}

        ////private BorrowInformationService MapDALToBorrowInformationService()
        ////{
        ////    BorrowInformationService borrowInformation = new BorrowInformationService()
        ////    {
        ////        BorrowDate = BorrowDate(),
        ////        DueDate = DueDate()
        ////    };

        ////    return borrowInformation;
        ////}

        ////private DateTime BorrowDate()
        ////{
        ////    DateTime borrowDate = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"), CultureInfo.InvariantCulture);

        ////    return  borrowDate;
        ////}

        //private String BorrowDate()
        //{
        //    //String borrowDate = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"), CultureInfo.InvariantCulture);

        //    String borrowDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

        //    return borrowDate;
        //}

        //private String DueDate()
        //{
        //    DateTime dtDate = StrToDate(BorrowDate());
        //    var dueDate = dtDate.AddDays(7);
        //    String strDate = DateToStr(dueDate);

        //    return strDate;
        //}

        //private DateTime StrToDate(String date)
        //{
        //    return DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //}

        //public String DateToStr(DateTime date)
        //{
        //    return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        //}

        //private void LoadBorrowInformation(Book book, LibraryDbContext context)
        //{
        //    //using (LibraryDbContext context = new LibraryDbContext())
        //    //{

        //    //var bookDAL = await context.Borrows.FirstOrDefaultAsync(book => true);

        //    Borrow borrowDAL = context.Borrows.FirstOrDefault(borrow => borrow.Book == book.Id);

        //    //var borrowDAL = context.Borrows.FirstOrDefault(borrow => borrow.Book == book.Id);

        //    if (borrowDAL == null)
        //    {
        //        Borrow borrow = new Borrow();


        //        borrow.Id = BorrowID(book);
        //        borrow.BorrowDate = BorrowDateBorrow();
        //        borrow.DueDate = DueDateBorrow();
        //        borrow.ReturnDate = null;
        //        borrow.Reader = ReaderID(ClaimID);
        //        borrow.Book = book.Id;

        //        context.Borrows.Add(borrow);
        //        context.SaveChanges();
        //    }
        //    //}
        //}

        //private DateTime BorrowDateBorrow()
        //{
        //    String strDate = DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        //    return StrToDate(strDate);

        //}

        //private DateTime DueDateBorrow()
        //{
        //    DateTime dtDate = StrToDate(BorrowDate());
        //    var dueDate = dtDate.AddDays(7);

        //    return dueDate;

        //}

        ////Extract from Think&GrowRich-NapoleonHill-1965-Wealth-N°1, to Think&GrowRich-N°1                  
        //private String BorrowID(Book book)
        //{
        //    //String bookName = book.Id.Substring(0, book.Id.IndexOf('-'));
        //    //String bookCopy = book.Id.Substring(0, book.Id.LastIndexOf('-') + 1);

        //    //return bookName + "-" + bookCopy;

        //    int firstHyphenIndex = book.Id.IndexOf("-");
        //    String bookTitle = book.Id.Substring(0, firstHyphenIndex).Trim();

        //    int lastHyphenIndex = book.Id.LastIndexOf("-");
        //    String copyNumber = book.Id.Substring(lastHyphenIndex + 1).Trim();

        //    return $"{bookTitle}-{copyNumber}";
        //}

        //private String ReaderID(String claim)
        //{
        //    String endUserIDCleaned = claim.Replace("EndUser", "Reader");

        //    return endUserIDCleaned;
        //}

        ////private String BorrowReader()
        ////{

        ////}

        ////private String BorrowBook(Book book)
        ////{

        ////}
        //#endregion

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
