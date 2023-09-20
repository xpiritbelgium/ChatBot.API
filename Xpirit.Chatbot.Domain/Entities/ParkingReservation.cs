namespace Xpirit.Chatbot.Domain.Entities
{
	public class ParkingReservation
	{
		public Guid Id { get; protected set; }
		public string ParkingSpot { get; set; } = null!;
		public DateTime ReservedDate { get; set; }
		public string PersonName { get; set; } = null!;
    }
}
