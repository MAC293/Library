using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Library.Services
{
    public  class ClaimVerifierService
    {
        private String _ClaimID;
        private readonly IHttpContextAccessor _HttpCTXA;

        public ClaimVerifierService(IHttpContextAccessor httpCTX)
        {
            _HttpCTXA = httpCTX;
        }

        public String ClaimID
        {
            get { return _ClaimID; }
            set { _ClaimID = value; }
        }
        
        public IHttpContextAccessor HttpCTXA
        {
            get { return _HttpCTXA; }
        }

        public Boolean ClaimValidation()
        {
            AssignClaim();

            if (HasClaim())
            {
                return true;
            }

            return false;
        }

        public void AssignClaim()
        {
            var userIdClaim = _HttpCTXA.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            ClaimID = userIdClaim.Value;
        }

        public Boolean HasClaim()
        {
            if (ClaimID == String.Empty)
            {
                return false;
            }
            return true;
        }

        public Boolean ValidClaim()
        {
            if (ClaimID.StartsWith('L') || Char.IsDigit(ClaimID[0]))
            {
                return true;
            }

            return false;
        }
    }
}
