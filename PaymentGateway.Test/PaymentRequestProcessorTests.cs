using Moq;
using System.Threading.Tasks;
using Xunit;

namespace PaymentGateway.Test
{
    public class PaymentRequestProcessorTests
    {
        private readonly Mock<IPaymentRequestValidator> validator;
        private readonly Mock<IBankTransactionService> bankTransactionService;
        private readonly Mock<IEventBus> eventBus;
        private readonly IPaymentRequestProcessor target;
        public const string defaultBankTransactionId = "12345";

        public PaymentRequestProcessorTests()
        {
            validator = new Mock<IPaymentRequestValidator>();
            validator.Setup(s => s.ValidatePaymentRequest(It.IsAny<MerchantPaymentRequest>()))
                .Returns(() => new ValidationResult());
            bankTransactionService = new Mock<IBankTransactionService>();
            eventBus = new Mock<IEventBus>();
            target = new PaymentRequestProcessor(validator.Object, bankTransactionService.Object, eventBus.Object);
        }

        [Fact]
        public async Task SubmitPaymentRequest_WhenValidationFails_ReturnsFailedResult()
        {
            var request = BuildRequest();
            var error = "Nope";
            validator.Setup(s => s.ValidatePaymentRequest(request))
                .Returns(() => new ValidationResult(error));
            var result = await target.SubmitPaymentRequest(request);
            var failedResult = Assert.IsType<FailedPaymentRequestResult>(result);
            Assert.Equal(request.TrackingId, failedResult.TrackingId);
            Assert.Equal(new[] { error }, failedResult.Errors);
            Assert.Equal(PaymentErrorTypes.InvalidRequest, failedResult.ErrorType);
        }

        [Fact]
        public async Task SubmitPaymentRequest_WhenValidationFails_PublishesFailedEvent()
        {
            var request = BuildRequest();
            var error = "Nope";
            validator.Setup(s => s.ValidatePaymentRequest(request))
                .Returns(() => new ValidationResult(error));
            eventBus.Setup(s => s.PublishEventAsync(It.IsAny<MerchantPaymentRequestFailedToProcess>()))
                .Callback((MerchantPaymentRequestFailedToProcess e) =>
                {
                    Assert.Equal(request.MerchantId, e.MerchantId);
                    Assert.Equal(request.PaymentRequest, e.PaymentRequest);
                    Assert.Equal(request.TrackingId, e.TrackingId);
                    Assert.Equal(PaymentErrorTypes.InvalidRequest, e.ErrorType);
                    Assert.Equal(new[] { error }, e.Errors);

                });
            var result = await target.SubmitPaymentRequest(request);
            eventBus.Verify(s => s.PublishEventAsync(It.IsAny<MerchantPaymentRequestFailedToProcess>()), Times.Once);
        }

        [Theory]
        [InlineData(BankTransactionResultStatus.NotFound, PaymentErrorTypes.BankNotFound)]
        [InlineData(BankTransactionResultStatus.Rejected, PaymentErrorTypes.RejectedByBank)]
        public async Task SubmitPaymentRequest_WhenBankTransactionFails_ReturnsFailedResult(BankTransactionResultStatus failedStatus, string expectedErrorType)
        {
            var request = BuildRequest();
            var error = "Nope";
            bankTransactionService.Setup(s => s.RequestPayment(request))
                .ReturnsAsync(() => new BankTransactionResult(failedStatus, defaultBankTransactionId, error));
            var result = await target.SubmitPaymentRequest(request);
            var failedResult = Assert.IsType<FailedPaymentRequestResult>(result);
            Assert.Equal(request.TrackingId, failedResult.TrackingId);
            Assert.Equal(new[] { error }, failedResult.Errors);
            Assert.Equal(expectedErrorType, failedResult.ErrorType);
        }

        [Theory]
        [InlineData(BankTransactionResultStatus.NotFound, PaymentErrorTypes.BankNotFound)]
        [InlineData(BankTransactionResultStatus.Rejected, PaymentErrorTypes.RejectedByBank)]
        public async Task SubmitPaymentRequest_WhenBankTransactionFails_PublishesFailedEvent(BankTransactionResultStatus failedStatus, string expectedErrorType)
        {
            var request = BuildRequest();
            var error = "Nope";
            bankTransactionService.Setup(s => s.RequestPayment(request))
                .ReturnsAsync(() => new BankTransactionResult(failedStatus, defaultBankTransactionId, error));
            eventBus.Setup(s => s.PublishEventAsync(It.IsAny<MerchantPaymentRequestFailedToProcess>()))
                .Callback((MerchantPaymentRequestFailedToProcess e) =>
                {
                    Assert.Equal(request.MerchantId, e.MerchantId);
                    Assert.Equal(request.PaymentRequest, e.PaymentRequest);
                    Assert.Equal(request.TrackingId, e.TrackingId);
                    Assert.Equal(expectedErrorType, e.ErrorType);
                    Assert.Equal(new[] { error }, e.Errors);

                });
            var result = await target.SubmitPaymentRequest(request);
            eventBus.Verify(s => s.PublishEventAsync(It.IsAny<MerchantPaymentRequestFailedToProcess>()), Times.Once);
        }

        [Fact]
        public async Task SubmitPaymentRequest_WhenBankTransactionAccepted_ReturnsSucessResult()
        {
            var request = BuildRequest();
            bankTransactionService.Setup(s => s.RequestPayment(request))
                .ReturnsAsync(() => new BankTransactionResult(BankTransactionResultStatus.Accepted, defaultBankTransactionId));
            var result = await target.SubmitPaymentRequest(request);
            var failedResult = Assert.IsType<SucessfulPaymentRequestResult>(result);
            Assert.Equal(request.TrackingId, failedResult.TrackingId);
        }

        [Fact]
        public async Task SubmitPaymentRequest_WhenBankTransactionAccepted_PublishesSuccessEvent()
        {
            var request = BuildRequest();
            var bankTransactionResult = new BankTransactionResult(BankTransactionResultStatus.Accepted, defaultBankTransactionId);
            bankTransactionService.Setup(s => s.RequestPayment(request))
                .ReturnsAsync(() => bankTransactionResult);
            eventBus.Setup(s => s.PublishEventAsync(It.IsAny<MerchantPaymentRequestSuccessfullyProcessed>()))
                .Callback((MerchantPaymentRequestSuccessfullyProcessed e) =>
                {
                    Assert.Equal(request.MerchantId, e.MerchantId);
                    Assert.Equal(request.PaymentRequest, e.PaymentRequest);
                    Assert.Equal(request.TrackingId, e.TrackingId);
                    Assert.Equal(bankTransactionResult, e.BankTransactionResult);
                });
            var result = await target.SubmitPaymentRequest(request);
            eventBus.Verify(s => s.PublishEventAsync(It.IsAny<MerchantPaymentRequestSuccessfullyProcessed>()), Times.Once);
        }


        private MerchantPaymentRequest BuildRequest() => new MerchantPaymentRequest("Acme101", new PaymentRequest {Amount = 100 });
    }
}
