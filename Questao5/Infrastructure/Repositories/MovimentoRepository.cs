using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Domain.Interfaces;

namespace Questao5.Infrastructure.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly string _connectionString;

        public MovimentoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<decimal> ObterSomaCreditosAsync(string idContaCorrente)
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.ExecuteScalarAsync<decimal>(
                "SELECT COALESCE(SUM(valor), 0) FROM movimento WHERE idcontacorrente = @Id AND tipomovimento = 'C'", new { Id = idContaCorrente });
        }

        public async Task<decimal> ObterSomaDebitosAsync(string idContaCorrente)
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.ExecuteScalarAsync<decimal>(
                "SELECT COALESCE(SUM(valor), 0) FROM movimento WHERE idcontacorrente = @Id AND tipomovimento = 'D'", new { Id = idContaCorrente });
        }
    }
}
