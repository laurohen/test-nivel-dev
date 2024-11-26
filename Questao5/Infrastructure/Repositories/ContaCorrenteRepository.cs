using Microsoft.Data.Sqlite;
using Dapper;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;

namespace Questao5.Infrastructure.Repositories
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly string _connectionString;

        public ContaCorrenteRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ContaCorrente> GetContaByIdAsync(string idContaCorrente)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = "SELECT * FROM contacorrente WHERE idcontacorrente = @Id";
            return await connection.QueryFirstOrDefaultAsync<ContaCorrente>(sql, new { Id = idContaCorrente });
        }

        public async Task AddMovimentoAsync(Movimento movimento)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)";
            await connection.ExecuteAsync(sql, movimento);
        }

        public async Task<Idempotencia> GetIdempotenciaAsync(string chaveIdempotencia)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = "SELECT * FROM idempotencia WHERE chave_idempotencia = @Chave";
            return await connection.QueryFirstOrDefaultAsync<Idempotencia>(sql, new { Chave = chaveIdempotencia });
        }

        public async Task AddIdempotenciaAsync(Idempotencia idempotencia)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = "INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@ChaveIdempotencia, @Requisicao, @Resultado)";
            await connection.ExecuteAsync(sql, idempotencia);
        }
    }
}
