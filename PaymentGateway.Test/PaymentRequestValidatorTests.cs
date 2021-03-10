using Xunit;

namespace PaymentGateway.Test
{
    public class PaymentRequestValidatorTests
    {
        private readonly IPaymentRequestValidator target;
        public PaymentRequestValidatorTests()
        {
            target = new PaymentRequestValidator();
        }

        [Fact]
        public void ValidatePaymentRequest_WhenValidRequest_ReturnsNoErrors()
        {
            var result = target.ValidatePaymentRequest(BuildValidRequest());
            Assert.Empty(result.Errors);
            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ValidatePaymentRequest_WhenNoFxCode_ReturnsError()
        {
            var request = BuildValidRequest();
            request = request with { PaymentRequest = request.PaymentRequest with { Currency = string.Empty } };
            var result = target.ValidatePaymentRequest(request);
            Assert.Equal(new[] { "No Currency Provided" }, result.Errors);
            Assert.True(result.HasErrors);
        }

        [Fact]
        public void ValidatePaymentRequest_WhenNoMerchantId_ReturnsError()
        {
            var request = BuildValidRequest();
            request = request with { MerchantId = string.Empty };
            var result = target.ValidatePaymentRequest(request);
            Assert.Equal(new[] { "Invalid MerchantId" }, result.Errors);
            Assert.True(result.HasErrors);
        }

        [Fact]
        public void ValidatePaymentRequest_WhenNoCVV_ReturnsError()
        {
            var request = BuildValidRequest();
            request = request with 
            { 
                PaymentRequest = request.PaymentRequest with
                {
                    Source = request.PaymentRequest.Source with { Cvv = 0 }
                }
            };
            var result = target.ValidatePaymentRequest(request);
            Assert.Equal(new[] { "No CVV Provided" }, result.Errors);
            Assert.True(result.HasErrors);
        }

        [Fact]
        public void ValidatePaymentRequest_WhenHasInvalidCardNumber_ReturnsError()
        {
            var request = BuildValidRequest();
            request = request with
            {
                PaymentRequest = request.PaymentRequest with
                {
                    Source = request.PaymentRequest.Source with { CardNumber = "1234" }
                }
            };
            var result = target.ValidatePaymentRequest(request);
            Assert.Equal(new[] { "Invalid Card Number" }, result.Errors);
            Assert.True(result.HasErrors);
        }



        private MerchantPaymentRequest BuildValidRequest() => new MerchantPaymentRequest("Acme101", new PaymentRequest { Amount = 100, Currency = "GBP", Source = new CardPaymentSource { Cvv = 123, CardNumber = "1234567890" } });
    }
}
