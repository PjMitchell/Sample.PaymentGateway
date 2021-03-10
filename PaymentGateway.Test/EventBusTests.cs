using EasyNetQ;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PaymentGateway.Test
{
    public class EventBusTests
    {
        private readonly Mock<IPubSub> pubsub;
        private readonly IEventBus target;

        public EventBusTests()
        {
            pubsub = new Mock<IPubSub>();
            target = new EventBus(pubsub.Object);
        }

        [Fact]
        public async Task CanPublish()
        {
            var ev = new MerchantPaymentRequestSuccessfullyProcessed();
            await target.PublishEventAsync(ev);
            pubsub.Verify(v => v.PublishAsync(ev, It.IsAny<Action<IPublishConfiguration>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
