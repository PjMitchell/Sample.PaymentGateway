using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PaymentGateway.Auth;
using PaymentGateway.Controllers;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace PaymentGateway.Test
{
    public class PaymentControllerTests
    {

        private readonly Mock<IPaymentRequestProcessor> processor;
        private readonly PaymentController target;
        private const string merchantId = "Acme101";

        public PaymentControllerTests()
        {

            processor = new Mock<IPaymentRequestProcessor>();
            processor.Setup(s => s.SubmitPaymentRequest(It.IsAny<MerchantPaymentRequest>()))
                .ReturnsAsync(new SucessfulPaymentRequestResult(string.Empty));
            target = new PaymentController(NullLogger<PaymentController>.Instance);
            var httpContext = new Mock<HttpContext>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(MerchantClaims.MechantIdClaimType, merchantId) }));
            httpContext.Setup(s => s.User)
                .Returns(user);
            target.ControllerContext.HttpContext = httpContext.Object;
        }

        [Fact]
        public async Task RequestPayment_CallsProcessorWithMechantId()
        {
            await target.RequestPayment(processor.Object, BuildPaymentRequest());
            processor.Verify(v => v.SubmitPaymentRequest(It.Is<MerchantPaymentRequest>(a => a.MerchantId == merchantId)), Times.Once);
        }

        [Fact]
        public async Task RequestPayment_CallsProcessorWithRequest()
        {
            var request = BuildPaymentRequest();
            await target.RequestPayment(processor.Object, request);
            processor.Verify(v => v.SubmitPaymentRequest(It.Is<MerchantPaymentRequest>(a => a.PaymentRequest == request)), Times.Once);
        }

        [Fact]
        public async Task RequestPayment_ReturnsOkResultIfSucessfull()
        {
            var expected = new SucessfulPaymentRequestResult(string.Empty);
            processor.Setup(s => s.SubmitPaymentRequest(It.IsAny<MerchantPaymentRequest>()))
                .ReturnsAsync(expected);
            var result = await target.RequestPayment(processor.Object, BuildPaymentRequest());
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public async Task RequestPayment_ReturnsBadRequestResultIfFailed()
        {
            var expected = new FailedPaymentRequestResult(string.Empty, "Nope");
            processor.Setup(s => s.SubmitPaymentRequest(It.IsAny<MerchantPaymentRequest>()))
                .ReturnsAsync(expected);
            var result = await target.RequestPayment(processor.Object, BuildPaymentRequest());
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(expected, badResult.Value);
        }

        private PaymentRequest BuildPaymentRequest()
        {
            return new PaymentRequest
            {
                Amount = 1000000,
                Currency = "USD"
            };
        }
    }
}
