
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services
{
    public class TripsService : ITripsService
    {
        private readonly string _connectionString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True";

        public async Task<List<TripDTO>> GetAllAsync()
        {
            var trips = new Dictionary<int, TripDTO>();
            const string sql = @"
                SELECT 
                    t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople,
                    c.IdCountry, c.Name AS CountryName
                FROM Trip t
                LEFT JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
                LEFT JOIN Country c       ON ct.IdCountry = c.IdCountry
                ORDER BY t.IdTrip";
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(sql, conn);
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var id = reader.GetInt32(reader.GetOrdinal("IdTrip"));
                if (!trips.TryGetValue(id, out var trip))
                {
                    trip = new TripDTO
                    {
                        Id = id,
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                        DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                        MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople"))
                    };
                    trips[id] = trip;
                }
                if (!reader.IsDBNull(reader.GetOrdinal("IdCountry")))
                {
                    trip.Countries.Add(new CountryDTO
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("IdCountry")),
                        Name = reader.GetString(reader.GetOrdinal("CountryName"))
                    });
                }
            }
            return new List<TripDTO>(trips.Values);
        }

        public async Task<bool> ClientExistsAsync(int clientId)
        {
            const string sql = "SELECT 1 FROM Client WHERE IdClient = @clientId";
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@clientId", clientId);
            await conn.OpenAsync();
            return await cmd.ExecuteScalarAsync() != null;
        }

        public async Task<bool> TripExistsAsync(int tripId)
        {
            const string sql = "SELECT 1 FROM Trip WHERE IdTrip = @tripId";
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@tripId", tripId);
            await conn.OpenAsync();
            return await cmd.ExecuteScalarAsync() != null;
        }

        public async Task<bool> RegistrationExistsAsync(int clientId, int tripId)
        {
            const string sql = "SELECT 1 FROM Client_Trip WHERE IdClient = @clientId AND IdTrip = @tripId";
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@clientId", clientId);
            cmd.Parameters.AddWithValue("@tripId", tripId);
            await conn.OpenAsync();
            return await cmd.ExecuteScalarAsync() != null;
        }

        public async Task<(int CurrentCount, int MaxPeople)> GetTripCapacityAsync(int tripId)
        {
            const string sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @tripId) AS CurrentCount,
                    t.MaxPeople
                FROM Trip t
                WHERE t.IdTrip = @tripId";
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@tripId", tripId);
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return (-1, -1);
            return (
                reader.GetInt32(reader.GetOrdinal("CurrentCount")),
                reader.GetInt32(reader.GetOrdinal("MaxPeople"))
            );
        }

        public async Task<List<ClientTripDTO>> GetClientTripsAsync(int clientId)
        {
            var list = new List<ClientTripDTO>();
            const string sql = @"
                SELECT
                    t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople,
                    ct.RegisteredAt, ct.PaymentDate
                FROM Client_Trip ct
                JOIN Trip t ON ct.IdTrip = t.IdTrip
                WHERE ct.IdClient = @clientId
                ORDER BY ct.RegisteredAt";
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@clientId", clientId);
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new ClientTripDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                    DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                    MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                    RegisteredAt = reader.GetDateTime(reader.GetOrdinal("RegisteredAt")),
                    PaymentDate = reader.IsDBNull(reader.GetOrdinal("PaymentDate"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("PaymentDate"))
                });
            }
            return list;
        }

        public async Task RegisterClientTripAsync(int clientId, int tripId)
        {
            const string sql = @"
                INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt)
                VALUES (@clientId, @tripId, GETDATE())";
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@clientId", clientId);
            cmd.Parameters.AddWithValue("@tripId", tripId);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task RemoveClientTripAsync(int clientId, int tripId)
        {
            const string sql = "DELETE FROM Client_Trip WHERE IdClient = @clientId AND IdTrip = @tripId";
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@clientId", clientId);
            cmd.Parameters.AddWithValue("@tripId", tripId);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
