using System.ComponentModel;

namespace Library.Services
{
    public class BorrowInformationService
    {
        //private DateTime _BorrowDate;
        //private DateTime _DueDate;
        private String _BorrowDate;
        private String _DueDate;

        public BorrowInformationService()
        {

        }

        //public DateTime BorrowDate
        //{
        //    get { return _BorrowDate; }
        //    set { _BorrowDate = value; }
        //}

        //public DateTime DueDate
        //{
        //    get { return _DueDate; }
        //    set { _DueDate = value; }
        //}

        //[DisplayName("Borrow Date")]
        public String BorrowDate
        {
            get { return _BorrowDate; }
            set { _BorrowDate = value; }
        }

        //[DisplayName("Due Date")]
        public String DueDate
        {
            get { return _DueDate; }
            set { _DueDate = value; }
        }



    }
}
