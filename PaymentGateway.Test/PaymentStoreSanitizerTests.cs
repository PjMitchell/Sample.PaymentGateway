using System;
using Xunit;

namespace PaymentGateway.Test
{
    public class PaymentStoreSanitizerTests
    {

        private DateTimeOffset timeStamp;
        private readonly IPaymentStoreSanitizer target;
        private readonly string[] errors;
        public PaymentStoreSanitizerTests()
        {
            errors = new[] { "Nope" };
            timeStamp = DateTimeOffset.Now;
            target = new PaymentStoreSanitizer();
        }

        [Fact]
        public void Sanitize_FailedToProcess()
        {
            var input = BuildFailed(123, "123456790");
            var expected = BuildFailed(0, "*6790");

            var result = target.Sanitize(input);
            Assert.Equal(expected, result);

        }

        [Fact]
        public void Sanitize_SucessfullyProcessed()
        {
            var input = BuildSucess(123, "123456790");
            var expected = BuildSucess(0, "*6790");

            var result = target.Sanitize(input);
            Assert.Equal(expected, result);

        }

        private MerchantPaymentRequestFailedToProcess BuildFailed(int cvv, string cardNumber)
        {
            return new MerchantPaymentRequestFailedToProcess
            {
                Errors = errors,
                TimeStamp = timeStamp,
                ErrorType = "Just Failed",
                MerchantId = "Acme101",
                TrackingId = "123345",
                PaymentRequest = BuildStatus(cvv, cardNumber),
            };
        }

        private MerchantPaymentRequestSuccessfullyProcessed BuildSucess(int cvv, string cardNumber)
        {
            return new MerchantPaymentRequestSuccessfullyProcessed
            {
                TimeStamp = timeStamp,
                BankTransactionResult = new BankTransactionResult(BankTransactionResultStatus.Accepted, "B112233"),
                MerchantId = "Acme101",
                TrackingId = "123345",
                PaymentRequest = BuildStatus(cvv, cardNumber),
            };
        }



        private PaymentRequest BuildStatus(int cvv, string cardNumber)
        {
            return new PaymentRequest
            {
                Amount = 100,
                Currency = "GBP",
                Source = new CardPaymentSource
                {
                    CardHolder = "A Person",
                    CardNumber = cardNumber,
                    Cvv = cvv,
                    Expiry = new DateTime(2017, 1, 1),
                    Issuer = "Bank"
                }
            };
        }
    }
}
