using System.ComponentModel.DataAnnotations;

namespace Library.Services
{
    public class LibrarianService
    {
        //private String _IDEndUser;
        private String _Username;
        private String _Password;
        private String _IDLibrarian;
        //private String _IDEndUserLibrarian;

        public LibrarianService()
        {
            
        }

        //L18625987K-EndUser
        //public String IDEndUser
        //{
        //    get { return _IDEndUser; }
        //    set { _IDEndUser = value; }
        //}

        [Required(ErrorMessage = "Username field is required.")]
        [MaxLength(12, ErrorMessage = "Username length cannot exceed 12 characters.")]
        public String Username
        {
            get { return _Username; }
            set { _Username = value; }
        }

        [Required(ErrorMessage = "Password field is required.")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Password length must be between 10 and 20 characters.")]
        public String Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        //L15.254.625-K
        [Required(ErrorMessage = "ID field is required.")]
        [StringLength(13, MinimumLength = 12, ErrorMessage = "ID must be valid.")]
        public String IDLibrarian
        {
            get { return _IDLibrarian; }
            set { _IDLibrarian = value; }
        }

        //L18625987K-EndUser
        //public String IDEndUserLibrarian
        //{
        //    get { return _IDEndUserLibrarian; }
        //    set { _IDEndUserLibrarian = value; }
        //}

    }
}
