using Library.Models;

namespace Library.Services
{
    public class CacheManagerService
    {
        #region Attributes
        private CacheService _CacheService;

        public CacheManagerService(CacheService cs)
        {
            _CacheService = cs;
        }

        public CacheService CacheService
        {
            get { return _CacheService; }
        }
        #endregion



    }
}
