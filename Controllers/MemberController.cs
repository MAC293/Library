﻿using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Library.Services;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        #region Attributes
        private readonly String _SecretKey;
        private LibraryDbContext _Context;

        public MemberController(IConfiguration config, LibraryDbContext ctx)
        {
            _SecretKey = config.GetSection("Settings").GetSection("SecretKey").ToString();
            Context = ctx;
        }

        public LibraryDbContext Context
        {
            get { return _Context; }
            set { _Context = value; }
        }
        #endregion

        #region Sign Up Member
        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] ReaderService newMember)
        {
            try
            {
                if (newMember == null || !ModelState.IsValid)
                {
                    return BadRequest();
                }

                var memberDAL = Context.Members.FirstOrDefault(member => member.Id == newMember.IDMember);

                if (memberDAL != null)
                {
                    return BadRequest("This user already exists!");
                }

                Context.Members.Add(MappingMember(newMember));

                EndUserReader(newMember);

                await Context.SaveChangesAsync();

                //return StatusCode(StatusCodes.Status201Created, "Congratulations! Your account has been successfully created.");
                return Created("", "Your account has been successfully created.");
            }

            catch (DbUpdateException)
            {
                return StatusCode(500, "A database error occurred. Please try again.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");
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

        private void EndUserReader(ReaderService newMember)
        {
            EndUser newUser = new EndUser();
            newUser.Id = EndUserID(newMember.IDMember);
            newUser.Username = newMember.Username;
            newUser.Password = Hash(newMember.Password);
            Context.EndUsers.Add(newUser);

            Reader newReader = new Reader();
            newReader.Id = ReaderID(newMember.IDMember);
            newReader.Member = newMember.IDMember;
            newReader.EndUser = EndUserID(newMember.IDMember);
            Context.Readers.Add(newReader);
        }
        #endregion

        #region Sign Up Librarian
        [Route("Hire")]
        public async Task<IActionResult> SignUp([FromBody] LibrarianService librarianService)
        {

            if (librarianService == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var librarianDAL = Context.Librarians.FirstOrDefault(librarian => librarian.Id == librarianService.IDLibrarian);

            if (librarianDAL != null)
            {
                return BadRequest();
            }

            EndUserLibrarian(librarianService);

            await Context.SaveChangesAsync();

            return Created("", "Your new librarian account has been created.");

        }

        private void EndUserLibrarian(LibrarianService librarianService)
        {
            EndUser newEndUser = new EndUser();
            newEndUser.Id = IDLibrarian(librarianService.IDLibrarian);
            newEndUser.Username = librarianService.Username;
            newEndUser.Password = Hash(librarianService.Password);
            Context.EndUsers.Add(newEndUser);

            Librarian newLibrarian = new Librarian();
            newLibrarian.Id = librarianService.IDLibrarian;
            newLibrarian.EndUser = IDLibrarian(librarianService.IDLibrarian);
            Context.Librarians.Add(newLibrarian);
        }

        private void AddEndUser(LibrarianService librarianService)
        {
            EndUser newEndUser = new EndUser();
            newEndUser.Id = IDLibrarian(librarianService.IDLibrarian);
            newEndUser.Username = librarianService.Username;
            newEndUser.Password = Hash(librarianService.Password);
            Context.EndUsers.Add(newEndUser);
        }

        private void AddLibrarian(LibrarianService librarianService)
        {
            EndUser newEndUser = new EndUser();
            newEndUser.Id = IDLibrarian(librarianService.IDLibrarian);
            newEndUser.Username = librarianService.Username;
            newEndUser.Password = Hash(librarianService.Password);
            Context.EndUsers.Add(newEndUser);
        }
        #endregion

        #region Log In
        [HttpPost]
        [Route("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] UserService newUser)
        {
            try
            {
                if (newUser == null || !ModelState.IsValid)
                {
                    return BadRequest();
                }

                var users = Context.EndUsers
                    .Where(user => user.Username == newUser.Username)
                    .ToList();

                //HashVerifier cannot be performed inside the database, hast to be out if it
                var userDAL = users.FirstOrDefault(user => HashVerifier(newUser.Password, user.Password)
                                                           && UsernameComparison(newUser.Username, user.Username));

                if (userDAL != null)
                {
                    return Ok(CreateToken(userDAL.Id));
                }

                return NotFound("This user doesn't exist.");
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
        private String ReaderID(String memberID)
        {
            String newReaderID = IDCleaner(memberID) + "-Reader";

            return newReaderID;
        }

        private String EndUserID(String memberID)
        {
            String newEndUserID = IDCleaner(memberID) + "-EndUser";

            return newEndUserID;
        }

        private String IDLibrarian(String librarianID)
        {
            String newEndUserID = IDCleaner(librarianID) + "-EndUser";

            return newEndUserID;
        }

        private String IDCleaner(String memberID)
        {
            String cleanedID = memberID.Replace(".", "").Replace("-", "");

            return cleanedID;
        }
        #endregion

        #region Hashing
        private String Hash(String plainPassword)
        {
            String hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            return hashedPassword;
        }

        private Boolean HashVerifier(String plainPassword, String hashedPassword)
        {
            Boolean validPassword = BCrypt.Net.BCrypt.Verify(plainPassword.Trim(), hashedPassword.Trim());

            return validPassword;
        }
        #endregion

        #region Others
        private Boolean UsernameComparison(String input, String source)
        {
            if (String.Equals(input.Trim(), source.Trim(), StringComparison.CurrentCulture))
            {
                return true;
            }

            return false;
        }

        private String CreateToken(String ID)
        {
            var keyBytes = Encoding.ASCII.GetBytes(_SecretKey);
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
        #endregion
    }
}
