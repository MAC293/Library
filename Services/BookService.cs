using Library.CustomDataAnnotations;
using System.Security.Cryptography;

namespace Library.Services
{
    public class BookService
    {
        //private IFormFile _Cover;
        private Byte[] _Cover;

        public BookService() { }

        public string? Id { get; set; } = null!;
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public int? Year { get; set; }
        public string? Editorial { get; set; }
        public bool? Available { get; set; }


        [AllowedExtensions(new String[] { ".jpg", ".jpeg", ".png" })]
        //public byte[] Cover { get; set; } = Array.Empty<Byte>();

        //public IFormFile Cover
        //{
        //    get { return _Cover; }
        //    set { _Cover = value; }
        //}
        public Byte[] Cover
        {
            get { return _Cover; }
            set { _Cover = value; }
        }
    }
}
