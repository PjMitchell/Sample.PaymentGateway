using System;

namespace PaymentGateway
{
    public record CardPaymentSource
    {
        public string Issuer { get; init; } = string.Empty;
        public int Cvv { get; init; }
        public string CardHolder { get; init; } = string.Empty;
        public string CardNumber { get; init; } = string.Empty;
        public DateTime Expiry { get; init; }

        public static readonly CardPaymentSource Empty = new CardPaymentSource();
    }
}
