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

        public LibrarianLoanController(ClaimVerifierService claimVerifier, LibraryDbContext ctx)
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
                    var allLoans = await Context.Borrows.ToListAsync();

                    if (allLoans.Any())
                    {
                        var allBooksLoan = MappingAllLoans(allLoans);
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

    }
}
