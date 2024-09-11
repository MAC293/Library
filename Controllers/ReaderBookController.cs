using System.Globalization;
using Library.Models;
using Library.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReaderBookController : ControllerBase
    {
        #region Attributes
        private readonly ClaimVerifierService _ClaimVerifier;
        private LibraryDbContext _Context;
        private HelperService _HelperService;
        private CacheManagerService _CacheManagerService;

        public ReaderBookController(ClaimVerifierService claimVerifier, LibraryDbContext ctx, HelperService hs, CacheManagerService cms)
        {
            _ClaimVerifier = claimVerifier;
            Context = ctx;
            HelperService = hs;
            CacheManagerService = cms;
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

        #region Borrow a Book (GET)
        [HttpGet("BorrowBook/{bookToBorrow}")]
        [Authorize]
        public async Task<ActionResult<BorrowedBookService>> AcquireBook(String bookToBorrow)
        {
            try
            {
                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (Char.IsDigit(ClaimVerifier.ClaimID[0]))
                {
                    if (HelperService.CheckBookStorage(bookToBorrow.Trim()) == 0)
                    {
                        return NotFound("This book is not available in the library, yet.");
                    }

                    var bookDAL = await Context.Books.FirstOrDefaultAsync(book => book.Title.Trim() == bookToBorrow.Trim() && book.Available);


                    if (bookDAL.Available)
                    {
                        LoadBorrowInformation(bookDAL, Context);

                        bookDAL.Available = false;
                        Context.SaveChanges();

                        return MapDALToServiceBook(bookDAL);
                    }

                    return NotFound($"\"{bookToBorrow}\" is not available. You have to wait until a reader returns a copy.");

                }
                if (ClaimVerifier.ClaimID.StartsWith('L'))
                {
                    return Unauthorized("Only readers can borrow books.");
                }

                return BadRequest();

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");
                //return BadRequest("An exception has occurred: " + ex);
            }
        }

        private BorrowedBookService MapDALToServiceBook(Book availableBook)
        {
            BorrowedBookService borrowedBook = new BorrowedBookService()
            {
                Title = availableBook.Title.Trim(),
                Author = availableBook.Author.Trim(),
                Genre = availableBook.Genre.Trim(),
                Year = (int)availableBook.Year,
                Editorial = availableBook.Editorial.Trim(),
                Cover = availableBook.Cover,
                BorrowDate = BorrowDate(),
                DueDate = DueDate()
            };

            return borrowedBook;
        }

        private String BorrowDate()
        {
            String borrowDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            return borrowDate;
        }

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
            Borrow borrowDAL = context.Borrows.FirstOrDefault(borrow => borrow.Book == book.Id);

            if (borrowDAL == null)
            {
                Borrow borrow = new Borrow();

                borrow.Id = BorrowID(book);
                borrow.BorrowDate = BorrowDateBorrow();
                borrow.DueDate = DueDateBorrow();
                borrow.ReturnDate = new DateTime(1753, 1, 1); ;
                borrow.Reader = ReaderID(ClaimVerifier.ClaimID);
                borrow.Book = book.Id;

                context.Borrows.Add(borrow);
                context.SaveChanges();
            }
        }

        private DateTime BorrowDateBorrow()
        {
            String strDate = DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            return StrToDate(strDate);
        }

        private DateTime DueDateBorrow()
        {
            DateTime dtDate = StrToDate(BorrowDate());
            var dueDate = dtDate.AddDays(7);

            return dueDate;

        }

        private String BorrowID(Book book)
        {

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
        #endregion

        #region Read Books (GET)
        [HttpGet]
        [Route("ViewBooks")]
        [Authorize]
        public async Task<ActionResult<List<BooKService>>> DisplayBooks()
        {
            try
            {
                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (Char.IsDigit(ClaimVerifier.ClaimID[0]))
                {
                    var cacheBooks = CacheManagerService.CacheService.Get<List<BooKService>>($"book:all");

                    if (cacheBooks != null)
                    {
                        return cacheBooks;
                    }

                    var allBooks = await Context.Books.ToListAsync();

                    if (allBooks.Any())
                    {
                        var allBooksList = MappingAllBooks(allBooks);

                        CacheManagerService.CacheService.Set("all", allBooksList);

                        return allBooksList;
                    }

                    return NotFound();
                }
                if (ClaimVerifier.ClaimID.StartsWith('L'))
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

        private List<BooKService> MappingAllBooks(List<Book> aBooks)
        {
            var bookServiceList = aBooks.Select(book => new BooKService()
            {
                Title = book.Title.Trim(),
                Author = book.Author.Trim(),
                Genre = book.Genre.Trim(),
                Year = (int)book.Year,
                Editorial = book.Editorial.Trim(),
                Available = book.Available,
                Cover = book.Cover

            }).ToList();

            return bookServiceList;
        }
        #endregion

        #region Search a Book (GET)
        [HttpGet("FindBook/{toSearch}")]
        [Authorize]
        public async Task<ActionResult<List<BooKService>>> SearchBook(String toSearch)
        {
            try
            {
                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (Char.IsDigit(ClaimVerifier.ClaimID[0]))
                {
                    var searchBookCache = CacheManagerService.CacheService.Get<List<BooKService>>($"book:{toSearch}");

                    if (searchBookCache != null)
                    {
                        return searchBookCache;
                    }

                    var allBooks = await Context.Books.ToListAsync();

                    if (allBooks.Any())
                    {
                        var filteredBooks = allBooks.Where(book =>
                            book.Title.Contains(toSearch.Trim()) ||
                            book.Author.Contains(toSearch.Trim()) ||
                            book.Genre.Contains(toSearch.Trim()) ||
                            book.Editorial.Contains(toSearch.Trim())).ToList();

                        var allBooksList = MappingAllBooksSearch(filteredBooks);

                        CacheManagerService.CacheService.Set(toSearch.Trim(), allBooksList);

                        return allBooksList;
                    }

                    return NotFound();
                }
                if (ClaimVerifier.ClaimID.StartsWith('L'))
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

        private List<BooKService> MappingAllBooksSearch(List<Book> aBooks)
        {
            var bookServiceList = aBooks.Select(book => new BooKService()
            {
                Title = book.Title.Trim(),
                Author = book.Author.Trim(),
                Genre = book.Genre.Trim(),
                Year = (int)book.Year,
                Editorial = book.Editorial.Trim(),
                Available = book.Available,
                Cover = book.Cover

            }).ToList();

            return bookServiceList;
        }
        #endregion
    }
}
