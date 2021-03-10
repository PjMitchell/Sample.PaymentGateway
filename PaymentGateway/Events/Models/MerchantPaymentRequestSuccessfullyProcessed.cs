namespace PaymentGateway
{
    public record MerchantPaymentRequestSuccessfullyProcessed : CompletedMerchantPaymentRequestEvent
    {
        public MerchantPaymentRequestSuccessfullyProcessed()
        {
            BankTransactionResult = BankTransactionResult.Empty;
        }

        public MerchantPaymentRequestSuccessfullyProcessed(MerchantPaymentRequest paymentRequest, BankTransactionResult bankTransactionResult) : base(paymentRequest)
        {
            BankTransactionResult = bankTransactionResult;
        }

        public BankTransactionResult BankTransactionResult { get; init; }
        public override string Status => "Accepted";

    }
}
