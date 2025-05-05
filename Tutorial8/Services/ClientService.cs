using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services
{
    public class ClientsService : IClientsService
    {
        private readonly string _connectionString =
            "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=APBD;Integrated Security=True";

        public async Task<int> CreateClientAsync(ClientCreateDTO dto)
        {
            const string sql = @"
                INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
                OUTPUT INSERTED.IdClient
                VALUES (@firstName, @lastName, @email, @telephone, @pesel)";

            await using var conn = new SqlConnection(_connectionString);
            await using var cmd  = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@firstName", dto.FirstName);
            cmd.Parameters.AddWithValue("@lastName",  dto.LastName);
            cmd.Parameters.AddWithValue("@email",     dto.Email);
            cmd.Parameters.AddWithValue("@telephone", dto.Telephone);
            cmd.Parameters.AddWithValue("@pesel",     dto.Pesel);

            await conn.OpenAsync();
            var newId = (int)await cmd.ExecuteScalarAsync();
            return newId;
        }
    }
}