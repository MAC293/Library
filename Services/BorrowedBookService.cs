using Library.CustomDataAnnotations;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Library.Services
{
    public class BorrowedBookService
    {
        private String _Title;
        private String _Author;
        private String _Genre;
        private int _Year;
        private String _Editorial;
        private String _BorrowDate;
        private String _DueDate;
        private Byte[] _Cover;
        //private BorrowInformationService _Information;

        public BorrowedBookService()
        {
            //Information = new BorrowInformationService();
        }

        public String Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        public String Author
        {
            get { return _Author; }
            set { _Author = value; }
        }

        public String Genre
        {
            get { return _Genre; }
            set { _Genre = value; }
        }

        public int Year
        {
            get { return _Year; }
            set { _Year = value; }
        }

        public String Editorial
        {
            get { return _Editorial; }
            set { _Editorial = value; }
        }

        //public BorrowInformationService Information
        //{
        //    get { return _Information; }
        //    set { _Information = value; }
        //}

        public String BorrowDate
        {
            get { return _BorrowDate; }
            set { _BorrowDate = value; }
        }

        public String DueDate
        {
            get { return _DueDate; }
            set { _DueDate = value; }
        }

        public Byte[] Cover
        {
            get { return _Cover; }
            set { _Cover = value; }
        }
    }
}
