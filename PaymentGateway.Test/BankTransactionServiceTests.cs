using Moq;
using System.Threading.Tasks;
using Xunit;

namespace PaymentGateway.Test
{
    public class BankTransactionServiceTests
    {
        private Mock<IBankFactory> bankFactory;
        private IBankTransactionService target;
        private MerchantPaymentRequest defaultRequest;

        public BankTransactionServiceTests()
        {
            bankFactory = new Mock<IBankFactory>();
            defaultRequest = new MerchantPaymentRequest("Acme101", new PaymentRequest { Amount = 101, Source = new CardPaymentSource { Issuer = "OmniBank" } });
            target = new BankTransactionService(bankFactory.Object);
        }

        [Fact]
        public async Task RequestPayment_IfNotFound_ReturnsRejected()
        {
            bankFactory.Setup(s => s.GetBankOrDefault(defaultRequest.PaymentRequest.Source.Issuer))
                .Returns(() => null);
            var result = await target.RequestPayment(defaultRequest);
            Assert.Equal(BankTransactionResultStatus.NotFound, result.Status);
            Assert.Equal(string.Empty, result.BankTransactionId);
            Assert.Equal(new[] { $"{defaultRequest.PaymentRequest.Source.Issuer} not found" }, result.Errors);
        }

        [Fact]
        public async Task RequestPayment_CallsFoundBank()
        {
            var bank = new Mock<IBank>();
            var expectedResult = new BankTransactionResult(BankTransactionResultStatus.Accepted, "1234");
            bank.Setup(s => s.RequestPayment(defaultRequest))
                .ReturnsAsync(() => expectedResult);
            bankFactory.Setup(s => s.GetBankOrDefault(defaultRequest.PaymentRequest.Source.Issuer))
                .Returns(() => bank.Object);
            var result = await target.RequestPayment(defaultRequest);
            Assert.Equal(expectedResult, result);
        }
    }
}
