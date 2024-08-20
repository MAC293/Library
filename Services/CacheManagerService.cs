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

        private void IsAll(Book updateBook)
        {
            List<Book>? allList = CacheService.Get<List<Book>>($"book:all");

            if (allList != null)
            {
                if (allList.Any(book => book.Id == updateBook.Id))
                {
                    CacheService.Remove("book:all");

                    var booksDAL = Context.Books.ToList();

                    if (booksDAL.Any())
                    {
                        var booksCacheList = MappingCheckedList(booksDAL);
                        CacheService.Set("book:all", booksDAL);
                    }

                }
            }

        }
        
        private void IsTitle(Book isTitleBook)
        {

        }

        private void IsAuthor(Book isAuthorBook)
        {

        }

        private void IsGenre(Book isGenreBook)
        {

        }

        private void IsEditorial(Book isEditorialBook)
        {

        }

        private void IsBook(Book isEditorialBook)
        {

        }

        private void IsLoan(Book isEditorialBook)
        {

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

        #endregion

    }
}
