using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway
{
    public interface IPaymentRequestValidator
    {
        ValidationResult ValidatePaymentRequest(MerchantPaymentRequest paymentRequest);
    }

    public class PaymentRequestValidator : IPaymentRequestValidator
    { 
        public ValidationResult ValidatePaymentRequest(MerchantPaymentRequest paymentRequest)
        {
            return new ValidationResult(RunValidationRules(paymentRequest).ToArray());
        }

        private IEnumerable<string> RunValidationRules(MerchantPaymentRequest paymentRequest) => GetRules().SelectMany(rule => rule(paymentRequest));

        private static IEnumerable<Func<MerchantPaymentRequest, IEnumerable<string>>> GetRules()
        {
            // More validation would be required here
            yield return ValidateMerchant;
            yield return ValidateCurrency;
            yield return ValidateCard;

        }

        private static IEnumerable<string> ValidateMerchant(MerchantPaymentRequest request)
        {
            if (string.IsNullOrEmpty(request.MerchantId))
                yield return "Invalid MerchantId";
        }

        private static IEnumerable<string> ValidateCurrency(MerchantPaymentRequest request)
        {
            if (string.IsNullOrEmpty(request.PaymentRequest.Currency))
                yield return "No Currency Provided";
        }

        private static IEnumerable<string> ValidateCard(MerchantPaymentRequest request)
        {
            if (request.PaymentRequest.Source.Cvv == 0)
                yield return "No CVV Provided";

            // Not sure what the actual validation is herw
            if (request.PaymentRequest.Source.CardNumber.Length <= 6)
                yield return "Invalid Card Number";
        }
    }
}
