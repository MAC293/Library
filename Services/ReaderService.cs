//Data Annotation Validation
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Library.Services
{
    public class ReaderService
    {
        private String _IDMember;
        private String _Name;
        private String _Phone;
        private String _Email;
        private int _Age;
        private String _Username;
        private String _Password;

        public ReaderService()
        {
                
        }

        [Required(ErrorMessage = "ID field is required.")]
        [StringLength(12, MinimumLength = 11, ErrorMessage = "ID must be valid.")]
        public String IDMember
        {
            get { return _IDMember; }
            set { _IDMember = value; }
        }

        [Required(ErrorMessage = "Name field is required.")]
        [MaxLength(35, ErrorMessage = "Name cannot exceed 35 characters.")]
        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        [Required(ErrorMessage = "Phone field is required.")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Phone must be valid.")]
        public String Phone
        {
            get { return _Phone; }
            set { _Phone = value; }
        }

        [Required(ErrorMessage = "Email field is required.")]
        [MaxLength(25, ErrorMessage = "Email cannot exceed 25 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Invalid email format.")]
        public String Email
        {
            get { return _Email; }
            set { _Email = value; }
        }
        
        [Required(ErrorMessage = "Age field is required.")]
        [Range(18, 65, ErrorMessage = "Age must be between 18 and 65.")]
        public int Age
        {
            get { return _Age; }
            set { _Age = value; }
        }

        [Required(ErrorMessage = "Username field is required.")]
        [MaxLength(12, ErrorMessage = "Username cannot exceed 12 characters.")]
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
    }
}
