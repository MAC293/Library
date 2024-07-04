using Library.CustomDataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Library.Services
{
    public class BookService
    {
        private IFormFile _Cover;
        //private Byte[] _Cover;

        public BookService()
        {
            //Cover = Array.Empty<Byte>();
        }

        public string? Id { get; set; } = null!;
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public int? Year { get; set; }
        public string? Editorial { get; set; }
        public bool? Available { get; set; }

        //[Required(ErrorMessage = "Book cover is required.")]
        //[AllowedExtensions(new String[] { ".jpg", ".jpeg", ".png" })]
        //[AllowedExtensions]
        [AllowedExtensions(new String[] { ".jpg", ".png", ".jpeg" })]
        //public byte[] Cover { get; set; } = Array.Empty<Byte>();

        public IFormFile Cover
        {
            get { return _Cover; }
            set { _Cover = value; }
        }

        //public Byte[] Cover
        //{
        //    get { return _Cover; }
        //    set { _Cover = value; }
        //}
    }
}
