using CartService.Application.Contracts.Messaging;
using System.Text.Json;

namespace CartService.Application.Services.Dispatcher
{
    public class MessageDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(string eventType, JsonElement data)
        {
            using var scope = _serviceProvider.CreateScope();
            var provider = scope.ServiceProvider;

            var handlers = _serviceProvider.GetServices<IMessageHandler>();

            foreach (var handler in handlers)
            {
                var handlerInterface = handler.GetType()
                    .GetInterfaces()
                    .FirstOrDefault(i =>
                        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>)
                    );

                if (handlerInterface == null)
                    continue;

                var eventTypeProperty = handler.GetType().GetProperty("EventType");
                var handlerEvent = eventTypeProperty?.GetValue(handler)?.ToString();

                if (handlerEvent != eventType)
                    continue;

                var payloadType = handlerInterface.GetGenericArguments()[0];
                var payload = data.Deserialize(payloadType)!;

                var method = handler.GetType().GetMethod("HandleAsync")!;
                await (Task)method.Invoke(handler, new[] { payload })!;
            }
        }
    }
}
