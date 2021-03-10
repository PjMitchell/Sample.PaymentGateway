namespace PaymentGateway
{
    public interface IPaymentStoreSanitizer
    {
        CompletedMerchantPaymentRequestEvent Sanitize(CompletedMerchantPaymentRequestEvent request);
    }

    public class PaymentStoreSanitizer : IPaymentStoreSanitizer
    {
        public CompletedMerchantPaymentRequestEvent Sanitize(CompletedMerchantPaymentRequestEvent request)
        {
            return request with
            {
                PaymentRequest = request.PaymentRequest with
                {
                    Source = request.PaymentRequest.Source with
                    {
                        Cvv = 0,
                        CardNumber = SanitizeCardNumber(request.PaymentRequest.Source.CardNumber)
                    }
                }
            };
        }

        private string SanitizeCardNumber(string cardNumber)
        {
            if (cardNumber.Length <= 5)
                return "*****";
            var last4Digits = cardNumber.Substring(cardNumber.Length -4, 4);
            return $"*{last4Digits}";
        }
    }
}
