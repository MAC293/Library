﻿using Library.CustomDataAnnotations;

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

        public String ID
        {
            get { return _ID; }
            set { _ID = value; }
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
