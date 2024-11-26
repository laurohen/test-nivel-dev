using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Entities;
using Questao5.Domain.Exceptions;
using Questao5.Domain.Interfaces;

namespace Questao5.Application.Commands.Handlers
{
    public class MovimentarContaCommandHandler : IRequestHandler<MovimentarContaCommand, string>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;

        public MovimentarContaCommandHandler(IContaCorrenteRepository contaCorrenteRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
        }

        public async Task<string> Handle(MovimentarContaCommand request, CancellationToken cancellationToken)
        {
            // Verifica idempotência
            var idempotencia = await _contaCorrenteRepository.GetIdempotenciaAsync(request.ChaveIdempotencia);
            if (idempotencia != null)
            {
                return idempotencia.Resultado;
            }

            // Validações de negócio
            var conta = await _contaCorrenteRepository.GetContaByIdAsync(request.IdContaCorrente);
            if (conta == null) throw new BusinessException("Conta corrente inválida.", "INVALID_ACCOUNT");
            if (!conta.Ativo) throw new BusinessException("Conta corrente inativa.", "INACTIVE_ACCOUNT");
            if (request.Valor <= 0) throw new BusinessException("O valor deve ser positivo.", "INVALID_VALUE");
            if (request.TipoMovimento != "C" && request.TipoMovimento != "D")
                throw new BusinessException("Tipo de movimento inválido.", "INVALID_TYPE");

            // Cria o movimento
            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid().ToString(),
                IdContaCorrente = request.IdContaCorrente,
                DataMovimento = DateTime.UtcNow,
                TipoMovimento = request.TipoMovimento,
                Valor = request.Valor
            };

            await _contaCorrenteRepository.AddMovimentoAsync(movimento);

            // Salva idempotência
            var novaIdempotencia = new Idempotencia
            {
                ChaveIdempotencia = request.ChaveIdempotencia,
                Requisicao = Newtonsoft.Json.JsonConvert.SerializeObject(request),
                Resultado = movimento.IdMovimento
            };
            await _contaCorrenteRepository.AddIdempotenciaAsync(novaIdempotencia);

            return movimento.IdMovimento;
        }
    }
}
