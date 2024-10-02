using System.ComponentModel.DataAnnotations;
using Library.CustomDataAnnotations;

namespace Library.Services
{
    public class BookCoverService
    {
        private String _ID;
        private String _Title;
        private String _Author;
        private String _Genre;
        private int _Year;
        private String _Editorial;
        private Boolean _Available;
        private Byte[] _Cover;
        private String _CoverFileName;

        public BookCoverService()
        {

        }

        [Required(ErrorMessage = "ID field is required.")]
        [MaxLength(60, ErrorMessage = "ID cannot exceed 60 characters.")]
        public String ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        [Required(ErrorMessage = "Title field is required.")]
        [MaxLength(35, ErrorMessage = "Title length cannot exceed 35 characters.")]
        public String Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        [Required(ErrorMessage = "Author field is required.")]
        [MaxLength(30, ErrorMessage = "Author cannot exceed 30 characters.")]
        public String Author
        {
            get { return _Author; }
            set { _Author = value; }
        }

        [Required(ErrorMessage = "Genre field is required.")]
        [MaxLength(20, ErrorMessage = "Genre cannot exceed 20 characters.")]
        public String Genre
        {
            get { return _Genre; }
            set { _Genre = value; }
        }

        [Required(ErrorMessage = "Year field is required.")]
        [Range(1950, 2024, ErrorMessage = "Year must be between 1950 and 2024.")]
        public int Year
        {
            get { return _Year; }
            set { _Year = value; }
        }

        [Required(ErrorMessage = "Editorial field is required.")]
        [MaxLength(20, ErrorMessage = "Editorial cannot exceed 20 characters.")]
        public String Editorial
        {
            get { return _Editorial; }
            set { _Editorial = value; }
        }

        public Boolean Available
        {
            get { return _Available; }
            set { _Available = value; }
        }

        [NotNull]
        [Size]
        [AllowedExtensions]
        public Byte[] Cover
        {
            get { return _Cover; }
            set { _Cover = value; }
        }
        
        [AllowedExtensions]
        public String CoverFileName
        {
            get { return _CoverFileName; }
            set { _CoverFileName = value; }
        }

    }

}
