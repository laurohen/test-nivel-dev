namespace Questao5.Application.Responses
{
    public class ErrorResponse
    {
        /// <summary>
        /// Tipo do erro, como "INVALID_ACCOUNT", "INACTIVE_ACCOUNT", etc.
        /// </summary>
        public string ErrorType { get; set; }

        /// <summary>
        /// Mensagem descritiva do erro para o usuário final.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Código de status HTTP associado ao erro, como 400 para bad request, 404 para não encontrado, etc.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Data e hora em que o erro ocorreu.
        /// </summary>
        public DateTime Timestamp { get; set; }

        public ErrorResponse(string errorType, string message, int statusCode)
        {
            ErrorType = errorType;
            Message = message;
            StatusCode = statusCode;
            Timestamp = DateTime.UtcNow;
        }
    }
}
