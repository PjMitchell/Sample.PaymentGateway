using System;
using System.Threading.Tasks;

namespace PaymentGateway
{
    /// <summary>
    /// Test Bank will accept all amounts less than or equal to 100000 and fail anything else
    /// </summary>
    public class TestBank : IBank
    {
        public Task<BankTransactionResult> RequestPayment(MerchantPaymentRequest paymentRequest)
        {
            var bankRequestId = Guid.NewGuid().ToString();
            var result = paymentRequest.PaymentRequest.Amount <= 100000
                ? new BankTransactionResult(BankTransactionResultStatus.Accepted, bankRequestId)
                : new BankTransactionResult(BankTransactionResultStatus.Rejected, bankRequestId, "Too much");
            return Task.FromResult(result);
        }
    }
}
