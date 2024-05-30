using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Library.Services
{
    public class ReaderService
    {
        //Input
        private String _IDMember;
        private String _Name;
        private String _Phone;
        private String _Email;
        private int _Age;
        //Input
        //Background
        //private String _IDReader;
        //private String _IDMemberReader;
        //private String _IDEndUserReader;
        //private String _IDEndUser;
        //Input
        private String _Username;
        private String _Password;

        public ReaderService()
        {
                
        }

        [Required]
        [MaxLength(12)]
        public String IDMember
        {
            get { return _IDMember; }
            set { _IDMember = value; }
        }

        [Required]
        [MaxLength(35)]

        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        [Required]
        [MaxLength(9)]
        [MinLength(9)]
        public String Phone
        {
            get { return _Phone; }
            set { _Phone = value; }
        }

        [Required]
        [MaxLength(20)]
        public String Email
        {
            get { return _Email; }
            set { _Email = value; }
        }

        [Required]
        [Range(18,50)]
        public int Age
        {
            get { return _Age; }
            set { _Age = value; }
        }

        //public String IDReader
        //{
        //    get { return _IDReader; }
        //    set { _IDReader = value; }
        //}

        //public String IDMemberReader
        //{
        //    get { return _IDMemberReader; }
        //    set { _IDMemberReader = value; }
        //}

        //public String IDEndUserReader
        //{
        //    get { return _IDEndUserReader; }
        //    set { _IDEndUserReader = value; }
        //}

        //public String IDEndUser
        //{
        //    get { return _IDEndUser; }
        //    set { _IDEndUser = value; }
        //}

        [Required]
        [MaxLength(25)]
        public String Username
        {
            get { return _Username; }
            set { _Username = value; }
        }

        [Required]
        [MaxLength(25)]
        public String Password
        {
            get { return _Password; }
            set { _Password = value; }
        }


    }
}
