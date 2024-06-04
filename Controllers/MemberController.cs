using Library.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//Supportive classes
using Library.Services;
//String encoding
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection.PortableExecutable;


namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        #region Sign up
        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] ReaderService newMember)
        {
            using (LibraryDbContext context = new LibraryDbContext())
            {
                //Validate the coming JSON body
                if (newMember == null || !ModelState.IsValid)
                {
                    return BadRequest();
                }

                //Validation occurs on ReaderService.

                var memberDAL = context.Members.FirstOrDefault(member => member.Id == newMember.IDMember);

                if (memberDAL != null)
                {
                    return BadRequest("This user already exists!");
                }

                //New Member-Reader-EndUser creation on database

                //Member
                //IDMember from client
                //Name from client
                //Phone from client
                //Email from client
                //Age from client
                //Username from client
                //Password from client
                newMember.Password = Hash(newMember.Password);
                
                BackgroundAttributes(newMember);

                context.Members.Add(MappingMember(newMember));

                await context.SaveChangesAsync();

                return Created();
                //return default;
            }    
        }
        #endregion

        private Member MappingMember(ReaderService readerService)
        {
            var newMember = new Member()
            {

                Id = readerService.IDMember,
                Name = readerService.Name,
                Phone = readerService.Phone,
                Email = readerService.Email,
                Age = readerService.Age
            };

            return newMember;
        }

        //Add background attributes to entity
        private void BackgroundAttributes(ReaderService newMember)
        {
            using (LibraryDbContext context = new LibraryDbContext())
            {
                //EndUSer
                EndUser newUser = new EndUser();
                newUser.Id = EndUserID(newMember.IDMember);
                context.EndUsers.Add(newUser);

                //Reader
                Reader newReader = new Reader();
                newReader.Id = ReaderID(newMember.IDMember);
                newReader.Member = newMember.IDMember;
                newReader.EndUser = EndUserID(newMember.IDMember);
                context.Readers.Add(newReader);
            }
        }

        #region ID Cleaned
        //ReaderID cleaned for Sign Up
        private String ReaderID(String memberID)
        {
            String newReaderID = IDCleaner(memberID) + "-Reader";

            return newReaderID;
        }

        //EndUserID cleaned for Sign Up
        private String EndUserID(String memberID)
        {
            String newEndUserID = IDCleaner(memberID) + "-EndUser";

            return newEndUserID;
        }

        //Clean the coming IDMember from ReaderService
        private String IDCleaner(String memberID)
        {
            String cleanedID = memberID.Replace(".", "").Replace("-", "");
            //String cleanedID = Regex.Replace(memberID, @"[.-]", "");

            return cleanedID;

        }
        #endregion

        //Password hashing
        private String Hash(String plainPassword)
        {
            String hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            return hashedPassword;
        }

    }
}
