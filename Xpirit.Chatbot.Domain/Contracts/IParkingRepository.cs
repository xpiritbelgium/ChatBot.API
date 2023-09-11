using Xpirit.Chatbot.Domain.Entities;
namespace Xpirit.Chatbot.Domain.Contracts
{
	public interface IParkingRepository
	{
		Task<List<ParkingReservation>> GetAllReservationsAsync();
		Task<ParkingReservation> GetByIdAsync(Guid id);
		Task<List<ParkingReservation>> GetReservationsByDateAsync(DateTime date);
		Task InsertReservationAsync(ParkingReservation reservation);

	}
}
