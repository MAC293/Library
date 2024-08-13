using System.Globalization;
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
    public class ReaderBookController : ControllerBase
    {
        #region Attributes
        private readonly ClaimVerifierService _ClaimVerifier;
        private LibraryDbContext _Context;

        public ReaderBookController(ClaimVerifierService claimVerifier, LibraryDbContext ctx)
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
                    //if (CheckBookStorage(bookToBorrow.Trim()) == 0)
                    if (LibrarianBookController. == 0)
                    {
                        return NotFound("This book is not available in the library, yet.");
                    }

                    var bookDAL = await Context.Books.FirstOrDefaultAsync(book => book.Title.Trim() == bookToBorrow.Trim() && book.Available);


                    if (bookDAL.Available)
                    {
                        //context.Borrows.Add(newBook);
                        //await context.SaveChangesAsync();

                        LoadBorrowInformation(bookDAL, Context);

                        bookDAL.Available = false;
                        Context.SaveChanges();

                        //Console.WriteLine(bookDAL.Available);

                        return MapDALToServiceBook(bookDAL);
                    }

                    //return NotFound(bookToBorrow + "is not available. You have to wait until a reader returns a copy.");
                    //'{bookToBorrow}'
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

        private String BorrowDate()
        {
            //String borrowDate = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"), CultureInfo.InvariantCulture);

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

            //var borrowDAL = context.Borrows.FirstOrDefault(borrow => borrow.Book == book.Id);

            if (borrowDAL == null)
            {
                Borrow borrow = new Borrow();


                borrow.Id = BorrowID(book);
                borrow.BorrowDate = BorrowDateBorrow();
                borrow.DueDate = DueDateBorrow();
                borrow.ReturnDate = null;
                borrow.Reader = ReaderID(ClaimVerifier.ClaimID);
                borrow.Book = book.Id;

                context.Borrows.Add(borrow);
                context.SaveChanges();
            }
            //}
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

        //Extract from Think&GrowRich-NapoleonHill-1965-Wealth-N°1, to Think&GrowRich-N°1                  
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
    }
}
