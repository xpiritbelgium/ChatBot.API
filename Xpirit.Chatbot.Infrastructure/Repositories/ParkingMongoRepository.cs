using MongoDB.Driver;
using Xpirit.Chatbot.Domain.Contracts;
using Xpirit.Chatbot.Domain.Entities;

namespace Xpirit.Chatbot.Infrastructure.Repositories
{
	public class ParkingMongoRepository : IParkingRepository
	{
		private readonly IMongoCollection<ParkingReservation> _parkingReservationCollection;
        private readonly IMongoCollection<ParkingSpot> _parkingSpotCollection;

        public ParkingMongoRepository(IMongoDatabase database)
		{
			_parkingReservationCollection = database.GetCollection<ParkingReservation>("parkingReservations");
            _parkingSpotCollection = database.GetCollection<ParkingSpot>("parkingSpots");
        }

        public async Task<IEnumerable<ParkingSpot>> GetAllParkingSpotsAsync()
        {
            return await _parkingSpotCollection.Find(Builders<ParkingSpot>.Filter.Empty).ToListAsync();
        }

        public async Task<IEnumerable<ParkingReservation>> GetAllReservationsAsync()
		{
			return await _parkingReservationCollection.Find(Builders<ParkingReservation>.Filter.Empty).ToListAsync();
		}

        public async Task<IEnumerable<ParkingReservation>> GetAllFutureReservationsForPerson(string personName)
        {
			var filterDate = DateTime.UtcNow.Date;
            return await _parkingReservationCollection.Find(Builders<ParkingReservation>.Filter.Where(x => x.PersonName == personName && x.ReservedDate >= filterDate)).ToListAsync();
        }

        public async Task<IEnumerable<ParkingReservation>> GetReservationsByDateAsync(DateTime date)
		{
			var results = await _parkingReservationCollection.Find(x => x.ReservedDate == date).ToListAsync();
			return results;
		}

        public async Task RemoveReservationAsync(DateTime date, string personName)
        {
            await _parkingReservationCollection.DeleteManyAsync(x => x.ReservedDate == date && x.PersonName == personName);
        }

        public async Task<ParkingReservation> GetByIdAsync(Guid id)
		{
			var reservation = await _parkingReservationCollection.Find(r => r.Id == id).FirstOrDefaultAsync();
			return reservation;
		}

		public async Task InsertReservationAsync(ParkingReservation reservation)
		{
			await _parkingReservationCollection.InsertOneAsync(reservation);
		}
	}
}
