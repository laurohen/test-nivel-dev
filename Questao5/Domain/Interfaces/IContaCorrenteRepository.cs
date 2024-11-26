using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces
{
    public interface IContaCorrenteRepository
    {
        Task<ContaCorrente> GetContaByIdAsync(string idContaCorrente);
        Task AddMovimentoAsync(Movimento movimento);
        Task<Idempotencia> GetIdempotenciaAsync(string chaveIdempotencia);
        Task AddIdempotenciaAsync(Idempotencia idempotencia);
    }
}
