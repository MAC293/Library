using Library.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//Supportive classes
using Library.Services;
//String encoding
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection.PortableExecutable;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        //Token
        private readonly String secretKey;

        //Token
        public MemberController(IConfiguration config)
        {
            secretKey = config.GetSection("Settings").GetSection("SecretKey").ToString();
        }

        #region Sign up Member
        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] ReaderService newMember)
        {
            using (LibraryDbContext context = new LibraryDbContext())
            {
                try
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

                    //Member
                    context.Members.Add(MappingMember(newMember));

                    EndUserReader(newMember, context);

                    //EndUSer
                    //EndUser newUser = new EndUser();
                    //newUser.Id = EndUserID(newMember.IDMember);
                    //newUser.Username = newMember.Username;
                    //newUser.Password = Hash(newMember.Password);
                    //context.EndUsers.Add(newUser);

                    //Reader
                    //Reader newReader = new Reader();
                    //newReader.Id = ReaderID(newMember.IDMember);
                    //newReader.Member = newMember.IDMember;
                    //newReader.EndUser = EndUserID(newMember.IDMember);
                    //context.Readers.Add(newReader);

                    await context.SaveChangesAsync();

                    //var uri = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}/{newMember.IDMember}");
                    //return Created(uri, "Congratulations! Your account has been successfully created.");
                    //return StatusCode(StatusCodes.Status201Created, "Congratulations! Your account has been successfully created.");
                    return Created("", "Congratulations! Your account has been successfully created.");

                    //return default;
                }

                catch (DbUpdateException ex)
                {
                    return BadRequest("A DbUpdateException has occurred: " + ex);
                }

                catch (Exception ex)
                {
                    return BadRequest("An exception has occurred: " + ex);

                }
            }
        }

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

        //Add background attributes to EndUser, and Reader entities
        private void EndUserReader(ReaderService newMember, LibraryDbContext context)
        {

            //EndUSer
            EndUser newUser = new EndUser();
            newUser.Id = EndUserID(newMember.IDMember);
            newUser.Username = newMember.Username;
            newUser.Password = Hash(newMember.Password);
            context.EndUsers.Add(newUser);

            //Reader
            Reader newReader = new Reader();
            newReader.Id = ReaderID(newMember.IDMember);
            newReader.Member = newMember.IDMember;
            newReader.EndUser = EndUserID(newMember.IDMember);
            context.Readers.Add(newReader);
        }
        #endregion

        #region Sign Up Librarian
        [Route("Hire")]
        public async Task<IActionResult> SignUp([FromBody] LibrarianService librarianService)
        {
            using (LibraryDbContext context = new LibraryDbContext())
            {
                //Validate the coming JSON body
                if (librarianService == null || !ModelState.IsValid)
                {
                    return BadRequest();
                }

                var librarianDAL = context.Librarians.FirstOrDefault(librarian => librarian.Id == librarianService.IDLibrarian);

                if (librarianDAL != null)
                {
                    return BadRequest();
                }

                EndUserLibrarian(librarianService, context);

                await context.SaveChangesAsync();

                return Created("", "Your new librarian account has been created.");
            }
        }

        private void EndUserLibrarian(LibrarianService librarianService, LibraryDbContext context)
        {
            //EndUSer
            EndUser newEndUser = new EndUser();
            newEndUser.Id = IDLibrarian(librarianService.IDLibrarian);
            newEndUser.Username = librarianService.Username;
            newEndUser.Password = Hash(librarianService.Password);
            context.EndUsers.Add(newEndUser);

            //Librarian
            Librarian newLibrarian = new Librarian();
            newLibrarian.Id = librarianService.IDLibrarian;
            newLibrarian.EndUser = IDLibrarian(librarianService.IDLibrarian);
            context.Librarians.Add(newLibrarian);
        }
        #endregion

        #region Log In
        [HttpPost]
        [Route("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] UserService newUser)
        {
            try
            {
                using (LibraryDbContext context = new LibraryDbContext())
                {
                    if (newUser == null || !ModelState.IsValid)
                    {
                        return BadRequest();
                    }

                    var users = context.EndUsers
                        .Where(user => user.Username == newUser.Username)
                        .ToList();

                    var userDAL = users.FirstOrDefault(user => HashVerifier(newUser.Password, user.Password) 
                                                               && UsernameComparison(newUser.Username, user.Username));

                    if (userDAL != null)
                    {
                        return Ok(CreateToken(userDAL.Id));
                    }

                    return NotFound("This user doesn't exist.");

                }
            }
            catch (DbUpdateException ex)
            {
                return BadRequest("A DbUpdateException has occurred: " + ex);
            }

            catch (InvalidOperationException ex)
            {
                return BadRequest("An exception has occurred: " + ex);
            }

            catch (Exception ex)
            {
                return BadRequest("An exception has occurred: " + ex);

            }
        }
        #endregion

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

        //IDEndUser and IDEndUserLibrarian cleaned for Hire
        private String IDLibrarian(String librarianID)
        {
            String newEndUserID = IDCleaner(librarianID) + "-EndUser";

            return newEndUserID;
        }

        //Clean the coming ID
        private String IDCleaner(String memberID)
        {
            String cleanedID = memberID.Replace(".", "").Replace("-", "");
            //String cleanedID = Regex.Replace(memberID, @"[.-]", "");

            return cleanedID;

        }
        #endregion

        #region Hashing
        //Hash the password
        private String Hash(String plainPassword)
        {
            String hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            return hashedPassword;
        }

        //Verify password
        private Boolean HashVerifier(String plainPassword, String hashedPassword)
        {
            Boolean validPassword = BCrypt.Net.BCrypt.Verify(plainPassword.Trim(), hashedPassword.Trim());

            return validPassword;
        }
        #endregion

        //Compare incoming and stored username
        private Boolean UsernameComparison(String input, String source)
        {
            if (String.Equals(input.Trim(), source.Trim(), StringComparison.CurrentCulture))
            {
                return true;
            }

            return false;
        }

        //Create token based on incoming ID
        private String CreateToken(String ID)
        {
            var keyBytes = Encoding.ASCII.GetBytes(secretKey);
            var claims = new ClaimsIdentity();

            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, ID));

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claims,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            String createdToken = tokenHandler.WriteToken(tokenConfig);

            return createdToken;
        }

    }
}
