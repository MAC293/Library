using Library.Models;

namespace Library.Services
{
    public class HelperService
    {
        private LibraryDbContext _Context;

        public HelperService(ClaimVerifierService claimVerifier, LibraryDbContext ctx)
        {
            Context = ctx;
        }

        public LibraryDbContext Context
        {
            get { return _Context; }
            set { _Context = value; }
        }

        public int CheckBookStorage(String title)
        {
            int matchQuantity = Context.Books.Count(book => book.Title == title.Trim());

            return matchQuantity;
        }
    }
}
