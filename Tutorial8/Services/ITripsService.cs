
using System.Collections.Generic;
using System.Threading.Tasks;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services
{
    public interface ITripsService
    {
        Task<List<TripDTO>> GetAllAsync();
        Task<bool> ClientExistsAsync(int clientId);
        Task<bool> TripExistsAsync(int tripId);
        Task<bool> RegistrationExistsAsync(int clientId, int tripId);
        Task<(int CurrentCount, int MaxPeople)> GetTripCapacityAsync(int tripId);
        Task RegisterClientTripAsync(int clientId, int tripId);
        Task RemoveClientTripAsync(int clientId, int tripId);
    }
}