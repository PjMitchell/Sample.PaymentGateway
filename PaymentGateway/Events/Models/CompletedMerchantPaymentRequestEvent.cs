using System;

namespace PaymentGateway
{
    public abstract record CompletedMerchantPaymentRequestEvent : Event
    {
        public CompletedMerchantPaymentRequestEvent()
        {
            MerchantId = string.Empty;
            PaymentRequest = PaymentRequest.Empty;
        }

        public CompletedMerchantPaymentRequestEvent(MerchantPaymentRequest paymentRequest)
        {
            TrackingId = paymentRequest.TrackingId;
            TimeStamp = DateTimeOffset.UtcNow;
            MerchantId = paymentRequest.MerchantId;
            PaymentRequest = paymentRequest.PaymentRequest;
        }
        public string MerchantId { get; init; }
        public abstract string Status { get; }
        public PaymentRequest PaymentRequest { get; init; } 
    }
}
