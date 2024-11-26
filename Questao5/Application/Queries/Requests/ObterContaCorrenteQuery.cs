using MediatR;
using Questao5.Domain.Entities;

namespace Questao5.Application.Queries.Requests
{
    public class ObterContaCorrenteQuery : IRequest<ContaCorrente>
    {
        public string? IdContaCorrente { get; set; }
    }
}
