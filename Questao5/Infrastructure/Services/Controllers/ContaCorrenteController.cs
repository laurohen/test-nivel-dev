using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Queries.DTOs;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Responses;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Exceptions;

namespace Questao5.Infrastructure.Services.Controllers
{
    [ApiController]
    [Route("api/conta_corrente")]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContaCorrenteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Efetua movimento credito e debito da conta corrente.
        /// </summary>
        /// <param name="command">
        /// Objeto contendo os dados necessários para realizar a movimentação da conta corrente. Os campos incluem:
        /// <list type="bullet">
        /// <item><description><c>IdContaCorrente</c>: Identificação única da conta corrente onde a movimentação será registrada.</description></item>
        /// <item><description><c>Valor</c>: Valor monetário da movimentação. Deve ser maior que zero.</description></item>
        /// <item><description><c>TipoMovimento</c>: Tipo da operação a ser realizada. Valores aceitos: 
        /// <c>"C"</c> (Crédito) ou <c>"D"</c> (Débito).</description></item>
        /// <item><description><c>ChaveIdempotencia</c>: Chave única para garantir a idempotência da operação e evitar registros duplicados.</description></item>
        /// </list>
        /// </param>
        /// <returns>Movimento conta corrente.</returns>
        /// <response code="200">Retorna id da movimentacao realizada.</response>
        /// <response code="400">Dados inconsistentes.</response>
        [HttpPost("movimentar")]
        public async Task<IActionResult> Movimentar([FromBody] MovimentarContaCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new ErrorResponse(ex.ErrorCode, ex.Message, StatusCodes.Status400BadRequest));
            }
        }

        /// <summary>
        /// Obtém o saldo da conta corrente.
        /// </summary>
        /// <param name="id">Identificação da conta corrente.</param>
        /// <returns>Saldo da conta corrente.</returns>
        /// <response code="200">Saldo retornado com sucesso.</response>
        /// <response code="400">Dados inconsistentes.</response>
        [HttpGet("{id}/saldo")]
        [ProducesResponseType(typeof(SaldoResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> ObterSaldo(string id)
        {
            try
            {
                var query = new ObterSaldoQuery(id);
                var saldo = await _mediator.Send(query);
                return Ok(saldo);
            }
            catch (ArgumentException ex) when (ex.ParamName == "INVALID_ACCOUNT")
            {

                var errorResponse = new ErrorResponse(
                    ErrorCodes.INVALID_ACCOUNT,
                    "Conta não cadastrada.", // Mensagem sem o parâmetro adicional
                    StatusCodes.Status400BadRequest
                );
                return BadRequest(errorResponse);
            }
            catch (ArgumentException ex) when (ex.ParamName == "INACTIVE_ACCOUNT")
            {
                var errorResponse = new ErrorResponse(
                    ErrorCodes.INACTIVE_ACCOUNT,
                    "Conta inativa.",
                    StatusCodes.Status400BadRequest
                );
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ErrorResponse(
                    "GENERIC_ERROR",
                    "Ocorreu um erro inesperado.",
                    StatusCodes.Status500InternalServerError
                );
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
    }
}
