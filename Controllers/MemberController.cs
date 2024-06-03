using Library.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//Supportive classes
using Library.Services;
//String encoding
using System.Text;


namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        #region Sign up
        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] ReaderService newReader)
        {
            using (LibraryDbContext context = new LibraryDbContext())
            {
                //Validate the coming JSON body
                if (newReader == null || !ModelState.IsValid)
                {
                    return BadRequest();
                }

                //Validation

                var memberDAL = context.Members.FirstOrDefault(member => member.Id == newReader.IDMember);

                if (memberDAL != null)
                {
                    return BadRequest("This user already exists!");
                }

                //New Reader creation on database


                return default;
            }    
        }

        //Validate the incoming fields criteria format
        //private Boolean FieldsValidation(String ID, String name, int phone, int age, String username, String password)
        //{
        //    if (ID.Length >= 11 && ID.Length <= 12)
        //    {
                
        //    }

        //}
        #endregion

        //private Member MappingMember(ReaderService readerService)
        //{

        //}
    }
}
