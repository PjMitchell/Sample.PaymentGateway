using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PaymentGateway
{
    public static class EventBusBootstrap
    {
        public static void AddEventBus(this IServiceCollection serviceCollection, IConfiguration config)
        {
            var connectionString = config.GetValue<string>("RabbitMqConnectionString");
            var bus = RabbitHutch.CreateBus(connectionString);
            serviceCollection.AddSingleton(bus);
            serviceCollection.AddSingleton(bus.PubSub);
            serviceCollection.AddTransient<IEventBus, EventBus>();
        }

        
    }


}
