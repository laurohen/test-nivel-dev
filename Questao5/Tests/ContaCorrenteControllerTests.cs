using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Queries.DTOs;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Responses;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Exceptions;
using Questao5.Infrastructure.Services.Controllers;
using Xunit;

namespace Questao5.Tests
{
    public class ContaCorrenteControllerTests
    {
        private readonly ContaCorrenteController _controller;
        private readonly IMediator _mediatorMock;

        public ContaCorrenteControllerTests()
        {
            _mediatorMock = Substitute.For<IMediator>();
            _controller = new ContaCorrenteController(_mediatorMock);
        }

        [Fact]
        public async Task Movimentar_DeveRetornarSucesso()
        {
            // Arrange
            var command = new MovimentarContaCommand
            {
                IdContaCorrente = "123",
                Valor = 100.00m,
                TipoMovimento = "C",
                ChaveIdempotencia = "chave123"
            };

            var movimentoId = Guid.NewGuid().ToString();
            _mediatorMock.Send(command).Returns(Task.FromResult(movimentoId));

            // Act
            var result = await _controller.Movimentar(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Movimentar_DeveRetornarBadRequest_QuandoContaNaoExiste()
        {
            // Arrange
            var command = new MovimentarContaCommand
            {
                IdContaCorrente = "999", // Conta inexistente
                Valor = 100.00m,
                TipoMovimento = "C",
                ChaveIdempotencia = "chave123"
            };

            _mediatorMock.Send(Arg.Any<MovimentarContaCommand>())
                .Throws(new BusinessException("A conta informada não existe.", "INVALID_ACCOUNT"));

            // Act
            var result = await _controller.Movimentar(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("INVALID_ACCOUNT", errorResponse.ErrorType);
            Assert.Equal("A conta informada não existe.", errorResponse.Message);
        }

        [Fact]
        public async Task Movimentar_DeveRetornarBadRequest_QuandoContaInativa()
        {
            // Arrange
            var command = new MovimentarContaCommand
            {
                IdContaCorrente = "123", // Conta inativa
                Valor = 100.00m,
                TipoMovimento = "C",
                ChaveIdempotencia = "chave123"
            };

            _mediatorMock.Send(Arg.Any<MovimentarContaCommand>())
                .Throws(new BusinessException("A conta está inativa.", "INACTIVE_ACCOUNT"));

            // Act
            var result = await _controller.Movimentar(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("INACTIVE_ACCOUNT", errorResponse.ErrorType);
            Assert.Equal("A conta está inativa.", errorResponse.Message);
        }

        [Fact]
        public async Task Movimentar_DeveRetornarBadRequest_QuandoValorInvalido()
        {
            // Arrange
            var command = new MovimentarContaCommand
            {
                IdContaCorrente = "123",
                Valor = -50.00m, // Valor negativo
                TipoMovimento = "C",
                ChaveIdempotencia = "chave123"
            };

            _mediatorMock.Send(Arg.Any<MovimentarContaCommand>())
                .Throws(new BusinessException("O valor informado é inválido.", "INVALID_VALUE"));

            // Act
            var result = await _controller.Movimentar(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("INVALID_VALUE", errorResponse.ErrorType);
            Assert.Equal("O valor informado é inválido.", errorResponse.Message);
        }

        [Fact]
        public async Task Movimentar_DeveRetornarBadRequest_QuandoTipoMovimentoInvalido()
        {
            // Arrange
            var command = new MovimentarContaCommand
            {
                IdContaCorrente = "123",
                Valor = 100.00m,
                TipoMovimento = "X", // Tipo inválido
                ChaveIdempotencia = "chave123"
            };

            _mediatorMock.Send(Arg.Any<MovimentarContaCommand>())
                .Throws(new BusinessException("O tipo de movimento é inválido.", "INVALID_TYPE"));

            // Act
            var result = await _controller.Movimentar(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("INVALID_TYPE", errorResponse.ErrorType);
            Assert.Equal("O tipo de movimento é inválido.", errorResponse.Message);
        }

        [Fact]
        public async Task ObterSaldo_DeveRetornar200_QuandoDadosValidos()
        {
            // Arrange
            var idConta = "123";
            var saldoEsperado = new SaldoResponse
            {
                NumeroConta = 123,
                NomeTitular = "João Silva",
                DataHoraConsulta = DateTime.UtcNow,
                SaldoAtual = 500.00m
            };

            _mediatorMock.Send(Arg.Any<ObterSaldoQuery>()).Returns(saldoEsperado);

            // Act
            var result = await _controller.ObterSaldo(idConta);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<SaldoResponse>(okResult.Value);
            Assert.Equal(saldoEsperado.NumeroConta, response.NumeroConta);
            Assert.Equal(saldoEsperado.NomeTitular, response.NomeTitular);
            Assert.Equal(saldoEsperado.SaldoAtual, response.SaldoAtual);
        }

        [Fact]
        public async Task ObterSaldo_DeveRetornar400_QuandoContaNaoCadastrada()
        {
            // Arrange
            var idConta = "123";

            _mediatorMock
                .Send(Arg.Any<ObterSaldoQuery>())
                .ThrowsAsync(new ArgumentException("Conta não cadastrada.", "INVALID_ACCOUNT"));

            // Act
            var result = await _controller.ObterSaldo(idConta);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal(ErrorCodes.INVALID_ACCOUNT, errorResponse.ErrorType);
            Assert.Equal("Conta não cadastrada.", errorResponse.Message);
        }

        [Fact]
        public async Task ObterSaldo_DeveRetornar400_QuandoContaInativa()
        {
            // Arrange
            var idConta = "123";

            _mediatorMock
                .Send(Arg.Any<ObterSaldoQuery>())
                .ThrowsAsync(new ArgumentException("Conta inativa.", "INACTIVE_ACCOUNT"));

            // Act
            var result = await _controller.ObterSaldo(idConta);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal(ErrorCodes.INACTIVE_ACCOUNT, errorResponse.ErrorType);
            Assert.Equal("Conta inativa.", errorResponse.Message);
        }

        [Fact]
        public async Task ObterSaldo_DeveRetornar200_QuandoContaSemMovimentacoes()
        {
            // Arrange
            var idConta = "123";
            var saldoEsperado = new SaldoResponse
            {
                NumeroConta = 123,
                NomeTitular = "João Silva",
                DataHoraConsulta = DateTime.UtcNow,
                SaldoAtual = 0.00m
            };

            _mediatorMock.Send(Arg.Any<ObterSaldoQuery>()).Returns(saldoEsperado);

            // Act
            var result = await _controller.ObterSaldo(idConta);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<SaldoResponse>(okResult.Value);
            Assert.Equal(0.00m, response.SaldoAtual);
        }

    }
}

