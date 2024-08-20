using Library.Models;
using Library.Services;


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
            IsLoan(cacheBook);
        }

        private void IsAll(Book isAllBook)
        {
            List<Book>? allList = CacheService.Get<List<Book>>($"book:all");

            if (allList != null)
            {
                if (allList.Any(book => book.Id == isAllBook.Id))
                {
                    CacheService.Remove("book:all");

                    var booksDAL = Context.Books.ToList();

                    if (booksDAL.Any())
                    {
                        var booksAllList = MappingCheckedList(booksDAL);

                        CacheService.Set("book:all", booksAllList);
                    }
                }
            }
        }

        private void IsTitle(Book isTitleBook)
        {
            List<BooKService>? titleList = CacheService.Get<List<BooKService>>($"book:{isTitleBook.Title}");

            if (titleList != null)
            {
                if (titleList.Any(book => book.Title == isTitleBook.Title))
                {
                    CacheService.Remove($"book:{isTitleBook.Title}");

                    var booksTitleDAL = Context.Books.Where(book => book.Title == isTitleBook.Title).ToList();

                    if (booksTitleDAL.Any())
                    {
                        var booksTitleList = MappingCheckedList(booksTitleDAL);

                        CacheService.Set($"book:{isTitleBook.Title}", booksTitleList);
                    }
                }
            }
        }

        private void IsAuthor(Book isAuthorBook)
        {
            List<BooKService>? authorList = CacheService.Get<List<BooKService>>($"book:{isAuthorBook.Author}");

            if (authorList != null)
            {
                if (authorList.Any(book => book.Author == isAuthorBook.Author))
                {
                    CacheService.Remove($"book:{isAuthorBook.Author}");

                    var booksAuthorDAL = Context.Books.Where(book => book.Author == isAuthorBook.Author).ToList();

                    if (booksAuthorDAL.Any())
                    {
                        var booksAuthorList = MappingCheckedList(booksAuthorDAL);

                        CacheService.Set($"book:{isAuthorBook.Author}", booksAuthorList);
                    }
                }
            }
        }

        private void IsGenre(Book isGenreBook)
        {
            List<BooKService>? genreList = CacheService.Get<List<BooKService>>($"book:{isGenreBook.Genre}");

            if (genreList != null)
            {
                if (genreList.Any(book => book.Genre == isGenreBook.Genre))
                {
                    CacheService.Remove($"book:{isGenreBook.Genre}");

                    var booksGenreDAL = Context.Books.Where(book => book.Genre == isGenreBook.Genre).ToList();

                    if (booksGenreDAL.Any())
                    {
                        var booksGenreList = MappingCheckedList(booksGenreDAL);

                        CacheService.Set($"book:{isGenreBook.Genre}", booksGenreList);
                    }
                }
            }
        }

        private void IsEditorial(Book isEditorialBook)
        {
            List<BooKService>? editorialList = CacheService.Get<List<BooKService>>($"book:{isEditorialBook.Title}");

            if (editorialList != null)
            {
                if (editorialList.Any(book => book.Editorial == isEditorialBook.Editorial))
                {
                    CacheService.Remove($"book:{isEditorialBook.Editorial}");

                    var booksEditorialDAL = Context.Books.Where(book => book.Editorial == isEditorialBook.Editorial).ToList();

                    if (booksEditorialDAL.Any())
                    {
                        var booksEditorialList = MappingCheckedList(booksEditorialDAL);

                        CacheService.Set($"book:{isEditorialBook.Editorial}", booksEditorialList);
                    }
                }
            }
        }

        private List<BooKService> MappingCheckedList(List<Book> aVehicle)
        {
            var carServiceList = aVehicle.Select(book => new BooKService
            {
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                Year = (int)book.Year,
                Editorial = book.Editorial,
                Available = book.Available,
                Cover = book.Cover

            }).ToList();

            return carServiceList;
        }

        private void IsBook(Book isEditorialBook)
        {

        }

        private void IsLoan(Book isEditorialBook)
        {

        }
        #endregion

    }
}
