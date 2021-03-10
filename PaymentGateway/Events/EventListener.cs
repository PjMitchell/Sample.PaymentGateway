using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway
{
    /// <summary>
    /// Potentially could Unit test, but probably best to setup some integrations tests given the time
    /// This could also be in a separate application, but I wanted to keep the payment store read and write in the same service
    /// </summary>
    public class EventListener : IHostedService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IPubSub pubSub;
        private readonly ILogger logger;
        private const string storeMerchantRequestSubscription = "storeMerchantRequest";
        private ISubscriptionResult? failedPaymentSubscription;
        private ISubscriptionResult? successfulPaymentSubscription;
        private int actionCount;

        public EventListener(IServiceProvider serviceProvider, IPubSub pubSub, ILogger<EventListener> logger)
        {
            this.serviceProvider = serviceProvider;
            this.pubSub = pubSub;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("EventListener starting up");
            failedPaymentSubscription = await pubSub.SubscribeAsync<MerchantPaymentRequestFailedToProcess>(storeMerchantRequestSubscription, OnCompletedMerchantPaymentRequestEvent, cancellationToken);
            successfulPaymentSubscription = await pubSub.SubscribeAsync<MerchantPaymentRequestSuccessfullyProcessed>(storeMerchantRequestSubscription, OnCompletedMerchantPaymentRequestEvent, cancellationToken);
            logger.LogInformation("EventListener starting complete");

        }

        private async Task OnCompletedMerchantPaymentRequestEvent(CompletedMerchantPaymentRequestEvent ev)
        {
            Interlocked.Increment(ref actionCount);
            try
            {
                using(var scope = serviceProvider.CreateScope())
                {
                    logger.LogInformation("Processing event {EventType}:{TrackingId}$", ev.GetType(), ev.TrackingId);
                    var storageService = scope.ServiceProvider.GetRequiredService<ICompletedMerchantPaymentRequestStorageService>();
                    await storageService.StorePaymentRequest(ev);
                }
            }
            catch(Exception e)
            {
                //Further error handling required
                logger.LogError(e, "Error Processing event {EventType}:{TrackingId}$", ev.GetType(), ev.TrackingId);
            }
            finally
            {
                Interlocked.Decrement(ref actionCount);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)  
        {
            logger.LogInformation("EventListener shutting down");

            failedPaymentSubscription?.ConsumerCancellation.Dispose();
            successfulPaymentSubscription?.ConsumerCancellation.Dispose();

            while(actionCount > 0)
            {
                await Task.Delay(300, cancellationToken);
            }

            logger.LogInformation("EventListener shutdown complete");


        }
    }


}
