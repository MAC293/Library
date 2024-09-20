using Library.Models;
using Library.Services;
using Serilog;


namespace Library.Services
{
    public class CacheManagerService
    {
        #region Attributes
        private CacheService _CacheService;
        private LibraryDbContext _Context;

        public CacheManagerService(CacheService cs, LibraryDbContext ctx)
        {
            CacheService = cs;
            Context = ctx;
        }

        public CacheService CacheService
        {
            get { return _CacheService; }
            set { _CacheService = value; }
        }

        public LibraryDbContext Context
        {
            get { return _Context; }
            set { _Context = value; }
        }
        #endregion

        #region Update Book
        public void HasBook(Book cacheBook)
        {
            IsAll(cacheBook);
            IsTitle(cacheBook);
            IsAuthor(cacheBook);
            IsGenre(cacheBook);
            IsEditorial(cacheBook);
            IsBook(cacheBook);
        }

        private void IsAll(Book isAllBook)
        {
            List<Book>? allList = CacheService.Get<List<Book>>($"book:all".Trim());

            if (allList != null)
            {
                if (allList.Any(book => book.Id?.Trim() == isAllBook.Id?.Trim()))
                {
                    CacheService.Remove("book:all".Trim());

                    var booksDAL = Context.Books.ToList();

                    if (booksDAL.Any())
                    {
                        var booksAllList = MappingCheckedList(booksDAL);

                        CacheService.Set("all".Trim(), booksAllList);
                    }
                }
            }
        }

        private void IsTitle(Book isTitleBook)
        {
            List<BookService>? titleList = CacheService.Get<List<BookService>>($"book:{isTitleBook.Title}".Trim());

            if (titleList != null)
            {
                if (titleList.Any(book => book.Title.Trim() == isTitleBook.Title?.Trim()))
                {
                    CacheService.Remove($"book:{isTitleBook.Title}".Trim());

                    var booksTitleDAL = Context.Books.Where(book => book.Title.Trim() == isTitleBook.Title.Trim()).ToList();

                    if (booksTitleDAL.Any())
                    {
                        var booksTitleList = MappingCheckedList(booksTitleDAL);

                        CacheService.Set(isTitleBook.Title.Trim(), booksTitleList);
                    }
                }
            }
        }

        private void IsAuthor(Book isAuthorBook)
        {
            List<BookService>? authorList = CacheService.Get<List<BookService>>($"book:{isAuthorBook.Author}".Trim());

            if (authorList != null)
            {
                if (authorList.Any(book => book.Author.Trim() == isAuthorBook.Author?.Trim()))
                {
                    CacheService.Remove($"book:{isAuthorBook.Author}".Trim());

                    var booksAuthorDAL = Context.Books.Where(book => book.Author.Trim() == isAuthorBook.Author.Trim()).ToList();

                    if (booksAuthorDAL.Any())
                    {
                        var booksAuthorList = MappingCheckedList(booksAuthorDAL);

                        CacheService.Set(isAuthorBook.Author.Trim(), booksAuthorList);
                    }
                }
            }
        }

        private void IsGenre(Book isGenreBook)
        {
            List<BookService>? genreList = CacheService.Get<List<BookService>>($"book:{isGenreBook.Genre}".Trim());

            if (genreList != null)
            {
                if (genreList.Any(book => book.Genre.Trim() == isGenreBook.Genre?.Trim()))
                {
                    CacheService.Remove($"book:{isGenreBook.Genre}".Trim());

                    var booksGenreDAL = Context.Books.Where(book => book.Genre.Trim() == isGenreBook.Genre.Trim()).ToList();

                    if (booksGenreDAL.Any())
                    {
                        var booksGenreList = MappingCheckedList(booksGenreDAL);

                        CacheService.Set(isGenreBook.Genre.Trim(), booksGenreList);
                    }
                }
            }
        }

        private void IsEditorial(Book isEditorialBook)
        {
            List<BookService>? editorialList = CacheService.Get<List<BookService>>($"book:{isEditorialBook.Title}".Trim());

            if (editorialList != null)
            {
                if (editorialList.Any(book => book.Editorial.Trim() == isEditorialBook.Editorial?.Trim()))
                {
                    CacheService.Remove($"book:{isEditorialBook.Editorial}".Trim());

                    var booksEditorialDAL = Context.Books.Where(book => book.Editorial.Trim() == isEditorialBook.Editorial.Trim()).ToList();

                    if (booksEditorialDAL.Any())
                    {
                        var booksEditorialList = MappingCheckedList(booksEditorialDAL);

                        CacheService.Set(isEditorialBook.Editorial.Trim(), booksEditorialList);
                    }
                }
            }
        }

        private List<BookService> MappingCheckedList(List<Book> aVehicle)
        {
            var carServiceList = aVehicle.Select(book => new BookService
            {
                Title = book.Title.Trim(),
                Author = book.Author.Trim(),
                Genre = book.Genre.Trim(),
                Year = (int)book.Year,
                Editorial = book.Editorial.Trim(),
                Available = book.Available,
                Cover = book.Cover

            }).ToList();

            return carServiceList;
        }

        private void IsBook(Book isBook)
        {
            var bookCache = CacheService.Get<Book>($"book:{isBook.Id}".Trim());

            if (bookCache != null)
            {
                CacheService.Remove($"book:{isBook.Id}".Trim());

                var bookDAL = Context.Books.FirstOrDefault(book => book.Id.Trim() == isBook.Id.Trim());

                if (bookDAL != null)
                { 
                    CacheService.Set(isBook.Id.Trim(), bookDAL);
                }
            }
        }
        #endregion

        #region Update Loans
        public void IsLoan(BorrowInformationService isBorrow)
        {
            List<BorrowInformationService>? borrowList = CacheService.Get<List<BorrowInformationService>>("book:loans");

            //Log.Information("Borrow List coming from Cache Get: {@BorrowList}", borrowList);
            //Log.Information("Borrow coming from endpoint: {@Borrow}", isBorrow);

            if (borrowList != null)
            {
                if (isBorrow != null)
                {
                    var bList = borrowList.FirstOrDefault(borrow => borrow.ID.Trim() == isBorrow.ID.Trim());

                    if (bList != null)
                    {
                        CacheService.Remove("book:loans".Trim());

                        var booksLoansDAL = Context.Borrows.ToList();

                        if (booksLoansDAL.Any())
                        {
                            CacheService.Set("loans".Trim(), MappingCheckedLoans(booksLoansDAL));
                        }
                    }
                }
            }
        }

        private List<BorrowInformationService> MappingCheckedLoans(List<Borrow> unsortedLoans)
        {
            var loanServiceList = unsortedLoans.Select(borrow => new BorrowInformationService
            {
                ID = borrow.Id.Trim(),
                BorrowDate = borrow.BorrowDate,
                DueDate = borrow.DueDate,
                ReturnDate = (DateTime)borrow.ReturnDate,
                Reader = borrow.Reader.Trim(),
                Book = borrow.Book.Trim()

            }).ToList();

            return loanServiceList;
        }
        #endregion

        #region Remove Book
        public void DeleteBook(Book bookDelete)
        {
            var bookCache = CacheService.Get<Book>($"book:{bookDelete.Id}".Trim());

            if (bookCache != null)
            {
                CacheService.Remove($"book:{bookDelete.Id}".Trim());
            }
        }
        #endregion
    }
}
