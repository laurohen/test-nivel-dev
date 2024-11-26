namespace Questao5.Domain.Entities
{
    /// <summary>
    /// Representa a entidade de idempotência utilizada para garantir a resiliência de operações repetidas.
    /// </summary>
    public class Idempotencia
    {
        /// <summary>
        /// Chave única que identifica a requisição para fins de idempotência.
        /// </summary>
        public string? ChaveIdempotencia { get; set; }

        /// <summary>
        /// Dados da requisição original.
        /// </summary>
        public string? Requisicao { get; set; }

        /// <summary>
        /// Resultado gerado pela requisição original.
        /// </summary>
        public string? Resultado { get; set; }
    }
}
