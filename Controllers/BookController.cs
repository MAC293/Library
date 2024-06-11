using Library.Models;
using Library.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        #region Attributes
        private CacheService _CacheService;
        private String _ClaimID;

        public BookController(CacheService cacheService)
        {
            CacheService = cacheService;
        }

        public String ClaimID
        {
            get { return _ClaimID; }
            set { _ClaimID = value; }
        }
        public CacheService CacheService
        {
            get { return _CacheService; }
            set { _CacheService = value; }
        }
        #endregion

        #region Claim
        private Boolean hasClaim()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return false;
            }

            ClaimID = userIdClaim.Value;

            //validClaim(userIdClaim.Value);

            return true;
        }

        private String validClaim(String claimID)
        {
            if (claimID.StartsWith('L') || Char.IsDigit(claimID[0]))
            {
                ClaimID = claimID;

                return claimID;
            }

            return String.Empty;
        }
        #endregion

        #region Add a New Book (POST)
        [HttpPost]
        [Route("AddBook")]
        //This ensures that the JWT token is validated before the method is executed.
        [Authorize]
        public async Task<IActionResult> POST(Book newBook)
        {
            try
            {
                using (LibraryDbContext context = new LibraryDbContext())
                {
                    if (!hasClaim())
                    {
                        return Unauthorized("This user doesn't exist.");
                    }
                    else
                    {
                        if (ClaimID.StartsWith('L'))
                        {
                            
                        }
                        else if (Char.IsDigit(ClaimID[0]))
                        {
                            return Unauthorized("This user has no authorization to perform this action.");
                        }
                    }

                    var vehicleDAL = await context.Vehicles.FirstOrDefaultAsync(vehicle => vehicle.Patent == newVehicleService.Patent && vehicle.Driver == ClaimID);

                    if (vehicleDAL != null)
                    {
                        return BadRequest();
                    }

                    context.Vehicles.Add(MappingPOST(newVehicleService, ClaimID));
                    await context.SaveChangesAsync();

                    //return CreatedAtAction(nameof(GET), new { patent = newCar.Patent }, newCar);
                    return Created();
                }
            }
            catch (Exception ex)
            {
                return BadRequest("An exception has occurred: " + ex);

            }
        }
        #endregion
    }
}
