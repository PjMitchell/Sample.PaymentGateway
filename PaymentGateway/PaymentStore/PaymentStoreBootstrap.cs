using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;

namespace PaymentGateway
{
    public static class PaymentStoreBootstrap
    {
        public static void AddPaymentCollectionStore(this IServiceCollection serviceCollection, IConfiguration config)
        {
            
            BsonClassMap.RegisterClassMap<CompletedMerchantPaymentRequestEvent>(o => {
                o.AutoMap();
            });

            BsonClassMap.RegisterClassMap<MerchantPaymentRequestFailedToProcess>(o =>
            {
                o.AutoMap();
                o.SetDiscriminator("Failed");
                o.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<MerchantPaymentRequestSuccessfullyProcessed>(o =>
            {
                o.AutoMap();
                o.SetDiscriminator("Successful");
                o.SetIgnoreExtraElements(true);
            });
            var connectionString = config.GetValue<string>("MongoConnectionString");
            var client = new MongoClient(connectionString);
            serviceCollection.AddSingleton<IMongoClient>(client);
            serviceCollection.AddTransient(GetCollection);
            serviceCollection.AddTransient<IPaymentStore, PaymentStore>();
            serviceCollection.AddTransient<ICompletedMerchantPaymentRequestStorageService, CompletedMerchantPaymentRequestStorageService>();
            serviceCollection.AddTransient<IPaymentStoreSanitizer, PaymentStoreSanitizer>();

        }

        private static IMongoCollection<CompletedMerchantPaymentRequestEvent> GetCollection(IServiceProvider serviceProvider)
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            var db = client.GetDatabase("PaymentGateway");
            return db.GetCollection<CompletedMerchantPaymentRequestEvent>("Payments");
        }
    }

}
