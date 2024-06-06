using System.ComponentModel.DataAnnotations;

namespace Library.Services
{
    public class UserService
    {
        private String _Username;
        private String _Password;

        public UserService()
        {
            
        }

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
    }
}
