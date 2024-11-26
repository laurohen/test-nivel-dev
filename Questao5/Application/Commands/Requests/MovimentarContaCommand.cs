using MediatR;

namespace Questao5.Application.Commands.Requests
{
    public class MovimentarContaCommand : IRequest<string>
    {
        public string? IdContaCorrente { get; set; }
        public decimal Valor { get; set; }
        public string? TipoMovimento { get; set; } // "C" = Crédito, "D" = Débito
        public string? ChaveIdempotencia { get; set; }
    }
}
