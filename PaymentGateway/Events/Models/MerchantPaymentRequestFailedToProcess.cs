using System;

namespace PaymentGateway
{
    public record MerchantPaymentRequestFailedToProcess : CompletedMerchantPaymentRequestEvent
    {
        public MerchantPaymentRequestFailedToProcess()
        {
            ErrorType = string.Empty;
            Errors = Array.Empty<string>();
        }

        public MerchantPaymentRequestFailedToProcess(MerchantPaymentRequest paymentRequest, FailedPaymentRequestResult failedPaymentResult) : base(paymentRequest)
        {
            ErrorType = failedPaymentResult.ErrorType;
            Errors = failedPaymentResult.Errors;
        }

        public string ErrorType { get; init; }
        public string[] Errors { get; init; }
        public override string Status => "Failed";

    }
}
