namespace Xpirit.Chatbot.Domain.Entities
{
    public class ParkingSpot
    {
        public Guid Id { get; protected set; }
        public string Name { get; set; } = null!;
    }
}
