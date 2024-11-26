namespace Questao5.Application.Queries.DTOs
{
    /// <summary>
    /// Resposta do saldo da conta corrente.
    /// </summary>
    public class SaldoResponse
    {
        /// <summary>
        /// Número da conta corrente.
        /// </summary>
        public int NumeroConta { get; set; }

        /// <summary>
        /// Nome do titular da conta corrente.
        /// </summary>
        public string? NomeTitular { get; set; }

        /// <summary>
        /// Data e hora da resposta da consulta.
        /// </summary>
        public DateTime DataHoraConsulta { get; set; }

        /// <summary>
        /// Saldo atual da conta corrente.
        /// </summary>
        public decimal SaldoAtual { get; set; }
    }
}
