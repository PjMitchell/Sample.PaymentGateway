using System.Text.Json.Serialization;

namespace PaymentGateway
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BankTransactionResultStatus
    {
        NotFound,
        Rejected,
        Accepted
    }
}
