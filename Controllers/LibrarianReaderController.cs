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
    public class LibrarianReaderController : ControllerBase
    {
        #region Attributes
        private readonly ClaimVerifierService _ClaimVerifier;
        private LibraryDbContext _Context;

        public LibrarianReaderController(ClaimVerifierService claimVerifier, LibraryDbContext ctx)
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

        #region Remove Reader (DELETE)
        [HttpDelete("DeleteReader/{ID}")]
        [Authorize]
        public async Task<IActionResult> DeleteReader([FromRoute] String ID)
        {
            try
            {
                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (ClaimVerifier.ClaimID.StartsWith('L'))
                {

                    RemoveBorrows(ID + "-Reader");
                    RemoveReader(ID + "-Reader");
                    RemoveEndUser(ID + "-EndUser");
                    RemoveMember(MemberIDClear(ID));

                    return NoContent();

                    //return NotFound();
                }
                if (Char.IsDigit(ClaimVerifier.ClaimID[0]))
                {
                    return Unauthorized("This user has no authorization to perform this action.");
                }

                return BadRequest();

            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");

            }

            catch (InvalidOperationException ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");

            }

            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");

            }
        }

        private void RemoveBorrows(String borrowReader)
        {
            var borrowsDeletion = Context.Borrows.Where(borrow => borrow.Reader.Trim() == borrowReader.Trim()).ToList();

            //context.Remove(borrowsDeletion);
            Context.RemoveRange(borrowsDeletion);

            Context.SaveChanges();
        }

        private void RemoveEndUser(String endUserReader)
        {

            var endUserDeletion = Context.EndUsers.FirstOrDefault(user => user.Id == endUserReader);

            Context.Remove(endUserDeletion);

            Context.SaveChanges();

        }

        private void RemoveReader(String readerTo)
        {

            var readerDeletion = Context.Readers.FirstOrDefault(user => user.Id == readerTo);

            Context.Remove(readerDeletion);

            Context.SaveChanges();

        }

        private String MemberIDClear(String memberID)
        {
            String withoutK = memberID.TrimEnd('K');

            String formatted = withoutK;

            if (formatted.Length == 8)
            {

                formatted = $"{formatted.Substring(0, 2)}.{formatted.Substring(2, 3)}.{formatted.Substring(5, 3)}";
            }
            else if (formatted.Length == 7)
            {

                formatted = $"{formatted.Substring(0, 1)}.{formatted.Substring(1, 3)}.{formatted.Substring(4, 3)}";
            }

            return formatted + "-K";
        }

        private void RemoveMember(String memberTo)
        {

            var memberDeletion = Context.Members.FirstOrDefault(member => member.Id == memberTo);

            Context.Remove(memberDeletion);
            Context.SaveChanges();

        }
        #endregion
    }
}
