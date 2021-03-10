namespace PaymentGateway
{
    public record BankTransactionResult(BankTransactionResultStatus Status, string BankTransactionId, params string[] Errors)
    {
        public static BankTransactionResult Empty => new BankTransactionResult(BankTransactionResultStatus.Rejected, string.Empty);
    }
}
