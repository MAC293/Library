using Library.Models;
using Library.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrarianLoanController : ControllerBase
    {
        #region Attributes
        private readonly ClaimVerifierService _ClaimVerifier;
        private LibraryDbContext _Context;
        private CacheManagerService _CacheManagerService;

        public LibrarianLoanController(ClaimVerifierService claimVerifier, LibraryDbContext ctx, CacheManagerService cms)
        {
            _ClaimVerifier = claimVerifier;
            Context = ctx;
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

        public CacheManagerService CacheManagerService
        {
            get { return _CacheManagerService; }
            set { _CacheManagerService = value; }
        }
        #endregion

        #region Read Loans (GET)
        [HttpGet]
        [Route("ViewLoans")]
        [Authorize]
        public async Task<ActionResult<List<BorrowInformationService>>> DisplayLoans()
        {
            try
            {
                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (ClaimVerifier.ClaimID.StartsWith('L'))
                {
                    var checkLoan = CacheManagerService.CacheService.Get<List<BorrowInformationService>>("book:loans");

                    if (checkLoan != null)
                    {
                        return checkLoan;
                    }

                    var allLoans = await Context.Borrows.ToListAsync();

                    if (allLoans.Any())
                    {
                        var allBooksLoan = MappingAllLoans(allLoans);

                        CacheManagerService.CacheService.Set("loans", allBooksLoan);

                        return allBooksLoan;
                    }

                    return NotFound("Readers haven't requested a book.");
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

        private List<BorrowInformationService> MappingAllLoans(List<Borrow> aLoans)
        {
            var loansServiceList = aLoans.Select(loan => new BorrowInformationService
            {
                ID = loan.Id.Trim(),
                BorrowDate = loan.BorrowDate,
                DueDate = loan.DueDate,
                ReturnDate = DateTime.MinValue,
                Reader = loan.Reader.Trim(),
                Book = loan.Book.Trim()

            }).ToList();

            return loansServiceList;
        }

        private Boolean IsNull(DateTime returnDate)
        {
            if (returnDate == null)
            {
                return true;
            }

            return false;
        }

        private DateTime NotReturnedYet(DateTime returnDate)
        {
            return returnDate = DateTime.MinValue;
        }

        private DateTime NoBookYet(DateTime returnDate)
        {
            if (IsNull(returnDate))
            {
                return NotReturnedYet(returnDate);
            }

            return returnDate;
        }
        #endregion

        #region Update Loan (PUT)
        [HttpPut("UpdateLoan/{bookReturned}, {reader}")]
        [Authorize]
        public async Task<IActionResult> ReturnBook([FromRoute] String bookReturned, [FromRoute] String reader)
        {
            try
            {
                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (ClaimVerifier.ClaimID.StartsWith('L'))
                {
                    if (CheckLoans() == 0)
                    {
                        return NotFound("Readers haven't requested any book.");
                    }

                    //String cleanedBook = bookReturned.Replace(",", "");

                    var borrowDAL = await Context.Borrows.FirstOrDefaultAsync(borrow => borrow.Id.Trim() == BorrowID(bookReturned.Trim(), 
                            reader + "-Reader").Trim() && borrow.Reader.Trim() == reader + "-Reader".Trim());

                    if (borrowDAL != null)
                    {
                        borrowDAL.ReturnDate = DateTime.Now;
                     
                        AvailableAgain(ReadyBookAvailable(borrowDAL.Id.Trim()));

                        await Context.SaveChangesAsync();

                        CacheManagerService.IsLoan(borrowDAL);

                        return NoContent();
                    }

                    return Conflict();

                }
                if (Char.IsDigit(ClaimVerifier.ClaimID[0]))
                {
                    return Unauthorized("This user has no authorization to perform this action.");
                }

                return BadRequest("Invalid request.");


            }
            catch (Exception ex)
            {
                return StatusCode(400, "An unexpected error occurred. Please try again.");
            }
        }

        public int CheckLoans()
        {
            int areLoans = Context.Borrows.Count();

            return areLoans;
        }

        private String BorrowID(String readerBook, String readerID)
        {
            return BookReturned(readerBook.Trim(), readerID.Trim());
        }

        private String BookReturned(String bookReturned, String readerBorrower)
        {
            var bookToReturn = ReaderLoans(readerBorrower.Trim()).FirstOrDefault(borrow =>
                borrow.Id.Trim() == bookReturned + "-N°1".Trim() ||
                borrow.Id.Trim() == bookReturned + "-N°2".Trim() || borrow.Id.Trim() == bookReturned + "-N°3".Trim());

            if (bookToReturn != null)
            {
                return bookToReturn.Id.Trim();
            }

            return String.Empty;
        }

        private List<Borrow> ReaderLoans(String reader)
        {
            using (LibraryDbContext context = new LibraryDbContext())
            {
                var readerBooks = context.Borrows.Where(borrow => borrow.Reader.Trim() == reader.Trim()).ToList();

                return readerBooks;

            }
        }

        private List<Book> Books()
        {
            using (LibraryDbContext context = new LibraryDbContext())
            {
                var allBooks = context.Books.ToList();

                return allBooks;
            }
        }

        private void AvailableAgain(Book bookToChange)
        {
            using (LibraryDbContext context = new LibraryDbContext())
            {
                var availableAgain = context.Books.FirstOrDefault(book => book.Id.Trim() == bookToChange.Id.Trim());

                availableAgain.Available = true;

                context.SaveChanges();
            }
        }

        private Book ReadyBookAvailable(String borrowID)
        {
            foreach (var book in Books())
            {
                if (IsSubWithinMain(book.Id.Trim(), borrowID.Trim()))
                {
                    if (book != null)
                    {
                        return book;
                    }
                }
            }

            return new Book();
        }

        private Boolean IsSubWithinMain(String bookID, String borrowID)
        {
            bool firstHyphenEqual = bookID.Split('-')[0] == borrowID.Split('-')[0];
            bool secondHyphenEqual = bookID.Split('-').Last() == borrowID.Split('-').Last();

            if (firstHyphenEqual && secondHyphenEqual)
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}
