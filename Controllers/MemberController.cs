using Library.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Library.Models;


namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        #region Sign up

        [Route("SignUp")]
        [HttpPost]
        public async Task<IActionResult> SignUp()
        {
            using (LibraryDbContext context = new LibraryDbContext())
            {

            }    
        }

        #endregion
    }
}
