namespace PaymentGateway
{
    public record ValidationResult(params string[] Errors)
    {
        public bool HasErrors => Errors.Length != 0;
    }
}
