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



    }
}
