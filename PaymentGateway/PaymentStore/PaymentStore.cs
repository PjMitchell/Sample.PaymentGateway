using MongoDB.Driver;
using System.Threading.Tasks;

namespace PaymentGateway
{
    public interface IPaymentStore
    {
        Task AddAsync(CompletedMerchantPaymentRequestEvent paymentEvent);
        void Add(CompletedMerchantPaymentRequestEvent paymentEvent);
        Task<CompletedMerchantPaymentRequestEvent?> GetAsync(string merchantId, string trackingId);
    }

    /// <summary>
    /// Potentially could Unit test this on the IMongoCollection, but probably best to setup some integrations tests given the time
    /// </summary>
    public class PaymentStore : IPaymentStore
    {
        private readonly IMongoCollection<CompletedMerchantPaymentRequestEvent> paymentCollection;

        public PaymentStore(IMongoCollection<CompletedMerchantPaymentRequestEvent> paymentCollection)
        {
            this.paymentCollection = paymentCollection;
        }

        public Task AddAsync(CompletedMerchantPaymentRequestEvent paymentEvent)
        {
            return paymentCollection.InsertOneAsync(paymentEvent);
        }

        public void Add(CompletedMerchantPaymentRequestEvent paymentEvent)
        {
            paymentCollection.InsertOne(paymentEvent);
        }

        public async Task<CompletedMerchantPaymentRequestEvent?> GetAsync(string merchantId, string trackingId)
        {
            var filterBuilder = Builders<CompletedMerchantPaymentRequestEvent>.Filter;
            var filter = filterBuilder.Eq(v=> v.MerchantId,merchantId) & filterBuilder.Eq(v => v.TrackingId, trackingId);
            CompletedMerchantPaymentRequestEvent? result = await paymentCollection.Find(filter).FirstOrDefaultAsync();
            return result;
        }
    }
}
