using System.ComponentModel;

namespace Library.Services
{
    public class BorrowInformationService
    {
        private String _ID;
        private DateTime _BorrowDate;
        private DateTime _DueDate;
        private DateTime _ReturnDate;
        private String _Reader;
        private String _Book;


        public BorrowInformationService()
        {

        }

        public String ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public DateTime BorrowDate
        {
            get { return _BorrowDate; }
            set { _BorrowDate = value; }
        }

        public DateTime DueDate
        {
            get { return _DueDate; }
            set { _DueDate = value; }
        }

        public DateTime ReturnDate
        {
            get { return _ReturnDate; }
            set { _ReturnDate = value; }
        }

      
        public String Reader
        {
            get { return _Reader; }
            set { _Reader = value; }
        }

        public String Book
        {
            get { return _Book; }
            set { _Book = value; }
        }



    }
}
