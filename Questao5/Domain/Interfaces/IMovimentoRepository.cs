namespace Questao5.Domain.Interfaces
{
    public interface IMovimentoRepository
    {
        Task<decimal> ObterSomaCreditosAsync(string idContaCorrente);
        Task<decimal> ObterSomaDebitosAsync(string idContaCorrente);
    }
}
