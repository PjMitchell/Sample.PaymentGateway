using System.Threading.Tasks;

namespace PaymentGateway
{
    /// <summary>
    /// Used as a facade for whatever bank payments protocols we need to run up against.
    /// </summary>
    public interface IBank
    {
        Task<BankTransactionResult> RequestPayment(MerchantPaymentRequest paymentRequest);
    }
}
