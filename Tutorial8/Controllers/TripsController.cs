using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase
    {
        private readonly ITripsService _tripsService;

        public TripsController(ITripsService tripsService)
        {
            _tripsService = tripsService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TripDTO>>> Get()
        {
            try
            {
                var trips = await _tripsService.GetAllAsync();
                return Ok(trips);
            }
            catch
            {
                return StatusCode(500, "Wystąpił błąd podczas pobierania danych.");
            }
        }
    }
}