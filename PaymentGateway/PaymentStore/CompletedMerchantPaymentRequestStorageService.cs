using System.Threading.Tasks;

namespace PaymentGateway
{
    public interface ICompletedMerchantPaymentRequestStorageService
    {
        Task StorePaymentRequest(CompletedMerchantPaymentRequestEvent request);
    }

    public class CompletedMerchantPaymentRequestStorageService : ICompletedMerchantPaymentRequestStorageService
    {
        private readonly IPaymentStoreSanitizer sanitizer;
        private readonly IPaymentStore store;

        public CompletedMerchantPaymentRequestStorageService(IPaymentStore store, IPaymentStoreSanitizer sanitizer)
        {
            this.store = store;
            this.sanitizer = sanitizer;
        }
        public async Task StorePaymentRequest(CompletedMerchantPaymentRequestEvent request)
        {
            /* It might be best to clean up the info before it goes onto the messaging system, depending on if you will need the raw informantion. It might even need to be stored in the raw format and tidied up when it is exposed to the api. However, where ever it is being stored in the raw format it should be secure, or encrypted*/
            var sanitizedRequest = sanitizer.Sanitize(request);
            await store.AddAsync(sanitizedRequest);
        }
    }
}
