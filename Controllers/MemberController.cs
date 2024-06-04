using Library.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//Supportive classes
using Library.Services;
//String encoding
using System.Text;
using System.Text.RegularExpressions;


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

                //Validation occurs on ReaderService.

                var memberDAL = context.Members.FirstOrDefault(member => member.Id == newReader.IDMember);

                if (memberDAL != null)
                {
                    return BadRequest("This user already exists!");
                }

                //New Reader creation on database
                


                return default;
            }    
        }
        #endregion

        private void BackgroundAttributes()
        {

        }

        //Clean the coming IDMember from ReaderService to load it into the Reader table ID along with Rea-01
        private String ReaderID(String memberID)
        {
            String cleanedID = memberID.Replace(".", "").Replace("-", "");
            //String cleanedID = Regex.Replace(memberID, @"[.-]", "");

            String newReaderID = cleanedID + "-Reader";

            return  newReaderID;
        }

        private String EndUserID()
        {

        }


        //private Member MappingMember(ReaderService readerService)
        //{

        //}
    }
}
