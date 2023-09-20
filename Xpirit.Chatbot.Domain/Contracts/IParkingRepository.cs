using Xpirit.Chatbot.Domain.Entities;
namespace Xpirit.Chatbot.Domain.Contracts
{
	public interface IParkingRepository
	{
		Task<IEnumerable<ParkingSpot>> GetAllParkingSpotsAsync();

        Task<IEnumerable<ParkingReservation>> GetAllReservationsAsync();
		Task<IEnumerable<ParkingReservation>> GetAllFutureReservationsForPerson(string personName);
        Task<ParkingReservation> GetByIdAsync(Guid id);
		Task<IEnumerable<ParkingReservation>> GetReservationsByDateAsync(DateTime date);
		Task InsertReservationAsync(ParkingReservation reservation);
		Task RemoveReservationAsync(DateTime date, string personName);
    }
}
