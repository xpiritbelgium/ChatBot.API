using MongoDB.Driver;
using Xpirit.Chatbot.Domain.Contracts;
using Xpirit.Chatbot.Domain.Entities;

namespace Xpirit.Chatbot.Infrastructure.Repositories
{
	public class ParkingMongoRepository : IParkingRepository
	{
		private readonly IMongoCollection<ParkingReservation> _collection;

		public ParkingMongoRepository(IMongoDatabase database)
		{
			_collection = database.GetCollection<ParkingReservation>("parkingReservations");
		}
		public async Task<List<ParkingReservation>> GetAllReservationsAsync()
		{
			return await _collection.Find(Builders<ParkingReservation>.Filter.Empty).ToListAsync();
		}

		public async Task<List<ParkingReservation>> GetReservationsByDateAsync(DateTime date)
		{
			var results = await _collection.Find(x => x.ReservedDate == date).ToListAsync();
			return results;
		}

		public async Task<ParkingReservation> GetByIdAsync(Guid id)
		{
			var reservation = await _collection.Find(r => r.Id == id).FirstOrDefaultAsync();
			return reservation;
		}

		public async Task InsertReservationAsync(ParkingReservation reservation)
		{
			await _collection.InsertOneAsync(reservation);
		}
	}
}
