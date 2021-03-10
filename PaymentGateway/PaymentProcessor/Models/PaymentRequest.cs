using System;

namespace PaymentGateway
{
    public record PaymentRequest
    {
        public string Currency { get; init; } = string.Empty;
        public long Amount { get; init; }
        // Assumption is that it is a card payment, might want to allow for multiple types of payment sources
        public CardPaymentSource Source { get; init; } = CardPaymentSource.Empty;
        public static readonly PaymentRequest Empty = new PaymentRequest();
    }

    public record MerchantPaymentRequest(string MerchantId, PaymentRequest PaymentRequest)
    {
        public string TrackingId { get; init; } = Guid.NewGuid().ToString();
    }

    public abstract record PaymentRequestResult(string TrackingId)
    {

    }

    public record SucessfulPaymentRequestResult(string TrackingId) : PaymentRequestResult(TrackingId)
    {

    }

    public record FailedPaymentRequestResult(string TrackingId, string ErrorType, params string[] Errors) : PaymentRequestResult(TrackingId)
    {

    }
}
