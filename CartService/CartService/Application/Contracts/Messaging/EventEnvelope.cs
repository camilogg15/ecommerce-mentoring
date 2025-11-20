using System.Text.Json;

namespace CartService.Application.Contracts.Messaging
{
    public class EventEnvelope
    {
        public string Type { get; set; } = default!;
        public JsonElement Data { get; set; }
    }
}
