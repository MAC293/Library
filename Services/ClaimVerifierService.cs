using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Library.Services
{
    public  class ClaimVerifierService
    {
        private String _ClaimID;
        //private HttpContext _HttpCTX;
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
            //set { _HttpCTX = value; }
        }

        private Boolean ClaimValidation()
        {
            AssignClaim();

            if (HasClaim())
            {
                return true;
            }

            return false;
        }
        private void AssignClaim()
        {
            var userIdClaim = _HttpCTXA.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            ClaimID = userIdClaim.Value;
        }

        private Boolean HasClaim()
        {
            if (ClaimID == String.Empty)
            {
                return false;
            }
            return true;
        }

        //Check if Claim is valid (L/0)
        private Boolean ValidClaim()
        {
            if (ClaimID.StartsWith('L') || Char.IsDigit(ClaimID[0]))
            {
                return true;
            }

            return false;
        }
    }
}
