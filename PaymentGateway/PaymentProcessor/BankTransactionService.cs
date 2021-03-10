using System.Threading.Tasks;

namespace PaymentGateway
{
    public interface IBankTransactionService
    {
        Task<BankTransactionResult> RequestPayment(MerchantPaymentRequest paymentRequest);
    }

    public class BankTransactionService : IBankTransactionService
    {
        private readonly IBankFactory bankFactory;

        public BankTransactionService(IBankFactory bankFactory)
        {
            this.bankFactory = bankFactory;
        }

        public Task<BankTransactionResult> RequestPayment(MerchantPaymentRequest paymentRequest)
        {
            var bank = bankFactory.GetBankOrDefault(paymentRequest.PaymentRequest.Source.Issuer);
            if (bank is not null)
                return bank.RequestPayment(paymentRequest);
            var bankNotFound = new BankTransactionResult(BankTransactionResultStatus.NotFound, string.Empty, $"{paymentRequest.PaymentRequest.Source.Issuer} not found");
            return Task.FromResult(bankNotFound);
        }
    }
}
