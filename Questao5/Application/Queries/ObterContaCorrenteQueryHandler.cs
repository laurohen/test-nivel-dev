using MediatR;
using Questao5.Application.Queries.Requests;
using Questao5.Domain.Entities;
using Questao5.Domain.Exceptions;
using Questao5.Domain.Interfaces;

namespace Questao5.Application.Queries
{
    public class ObterContaCorrenteQueryHandler : IRequestHandler<ObterContaCorrenteQuery, ContaCorrente>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;

        public ObterContaCorrenteQueryHandler(IContaCorrenteRepository contaCorrenteRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
        }

        public async Task<ContaCorrente> Handle(ObterContaCorrenteQuery request, CancellationToken cancellationToken)
        {
            return await _contaCorrenteRepository.GetContaByIdAsync(request.IdContaCorrente)
                   ?? throw new BusinessException("Conta não encontrada.", "INVALID_ACCOUNT");
        }
    }
}
