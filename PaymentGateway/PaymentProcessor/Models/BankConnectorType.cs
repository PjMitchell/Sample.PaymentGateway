using System.Text.Json.Serialization;

namespace PaymentGateway
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BankConnectorType
    {
        Test
    }
}
