
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsController : ControllerBase
    {
        private readonly ITripsService _tripsService;

        public ClientsController(ITripsService tripsService)
        {
            _tripsService = tripsService;
        }

        [HttpPut("{clientId}/trips/{tripId}")]
        public async Task<IActionResult> Register(int clientId, int tripId)
        {
            if (!await _tripsService.ClientExistsAsync(clientId) ||
                !await _tripsService.TripExistsAsync(tripId))
                return NotFound();

            if (await _tripsService.RegistrationExistsAsync(clientId, tripId))
                return Conflict();

            var (current, max) = await _tripsService.GetTripCapacityAsync(tripId);
            if (current >= max)
                return BadRequest("Max participants reached");

            await _tripsService.RegisterClientTripAsync(clientId, tripId);
            return Created($"/api/clients/{clientId}/trips/{tripId}", null);
        }

        [HttpDelete("{clientId}/trips/{tripId}")]
        public async Task<IActionResult> Unregister(int clientId, int tripId)
        {
            if (!await _tripsService.RegistrationExistsAsync(clientId, tripId))
                return NotFound();

            await _tripsService.RemoveClientTripAsync(clientId, tripId);
            return NoContent();
        }
    }
}