using MediatR;
using Questao5.Application.Queries.DTOs;

namespace Questao5.Application.Queries.Requests
{
    public class ObterSaldoQuery : IRequest<SaldoResponse>
    {
        public string IdContaCorrente { get; set; }

        public ObterSaldoQuery(string idContaCorrente)
        {
            IdContaCorrente = idContaCorrente;
        }
    }
}
