using EasyNetQ;
using System.Threading.Tasks;

namespace PaymentGateway
{
    public interface IEventBus
    {
        Task PublishEventAsync<TEvent>(TEvent ev) where TEvent : Event;
    }

    public class EventBus : IEventBus
    {
        private readonly IPubSub bus;

        public EventBus(IPubSub bus)
        {
            this.bus = bus;
        }

        public async Task PublishEventAsync<TEvent>(TEvent ev) where TEvent : Event
        {
            await bus.PublishAsync(ev);
        }
    }
}
