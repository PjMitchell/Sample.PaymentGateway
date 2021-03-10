using Microsoft.AspNetCore.Authentication;

namespace PaymentGateway.Auth
{
    public class DevAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string MerchantId { get; set; } = string.Empty;
        public bool CanSubmitRequest { get; set; }
        public bool CanReadRequest { get; set; }

    }
}
