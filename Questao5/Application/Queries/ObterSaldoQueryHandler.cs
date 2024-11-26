using MediatR;
using Questao5.Application.Queries.DTOs;
using Questao5.Application.Queries.Requests;
using Questao5.Domain.Interfaces;

namespace Questao5.Application.Queries
{
    public class ObterSaldoQueryHandler : IRequestHandler<ObterSaldoQuery, SaldoResponse>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public ObterSaldoQueryHandler(
            IContaCorrenteRepository contaCorrenteRepository,
            IMovimentoRepository movimentoRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<SaldoResponse> Handle(ObterSaldoQuery request, CancellationToken cancellationToken)
        {
            var conta = await _contaCorrenteRepository.GetContaByIdAsync(request.IdContaCorrente);

            if (conta == null)
                throw new ArgumentException("Conta corrente não encontrada.", "INVALID_ACCOUNT");

            if (!conta.Ativo)
                throw new ArgumentException("Conta corrente está inativa.", "INACTIVE_ACCOUNT");

            var somaCreditos = await _movimentoRepository.ObterSomaCreditosAsync(request.IdContaCorrente);
            var somaDebitos = await _movimentoRepository.ObterSomaDebitosAsync(request.IdContaCorrente);

            return new SaldoResponse
            {
                NumeroConta = conta.Numero,
                NomeTitular = conta.Nome,
                DataHoraConsulta = DateTime.UtcNow,
                SaldoAtual = somaCreditos - somaDebitos
            };
        }
    }
}
