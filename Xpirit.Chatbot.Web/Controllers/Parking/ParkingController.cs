using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Xpirit.Chatbot.Domain.Contracts;
using Xpirit.Chatbot.Domain.Entities;

[ApiController]
[Route("[controller]")]
public class ParkingController : ControllerBase
{
	private readonly IParkingRepository _parkingRepository;
	private static List<string> _parkingSpots = new() { "Parking 42", "Parking 43", "Parking 44" };

	public ParkingController(IParkingRepository parkingRepository)
	{
		_parkingRepository = parkingRepository;
	}

	[HttpGet]
	public ActionResult<IEnumerable<string>> GetParkingSpots()
	{
		return Ok(_parkingSpots);
	}

	[HttpGet("Reservations")]
	public async Task<ActionResult<IEnumerable<ParkingReservation>>> GetAllReservations()
	{
		var reservations = await _parkingRepository.GetAllReservationsAsync();
		return Ok(reservations);
	}

	[HttpGet("Reservations/Available")]
	public async Task<ActionResult<IEnumerable<string>>> GetAvailableParkingSpotsForDate(DateTime? requestedDate)
	{
		if (!requestedDate.HasValue)
		{
			return BadRequest($"{nameof(requestedDate)} is not filled in");
		}

		var reservationsForDate = await _parkingRepository.GetReservationsByDateAsync(requestedDate.Value);
		var alreadyReservedParkingSpots = reservationsForDate.Select(r => r.ParkingSpot).ToList();
		var freeParkingSpots = _parkingSpots.Where(x => !alreadyReservedParkingSpots.Contains(x)).ToList();

		return Ok(freeParkingSpots);
	}

	[HttpPost("Reservation")]
	public async Task<ActionResult<string>> MakeReservation(string? requestedDate, string personName)
	{
		if (string.IsNullOrEmpty(requestedDate))
		{
			return BadRequest($"{nameof(requestedDate)} is not filled in");
		}

		if (!DateTime.TryParseExact(requestedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
		{
			return BadRequest($"{nameof(requestedDate)} is not a valid date");
		}

		if (string.IsNullOrEmpty(personName))
		{
			return BadRequest($"{nameof(personName)} is not filled in");
		}

		var reservationsForDate = await _parkingRepository.GetReservationsByDateAsync(parsedDate);
		var alreadyReservedParkingSpots = reservationsForDate.Select(r => r.ParkingSpot).ToList();
		var freeParkingSpot = _parkingSpots.FirstOrDefault(x => !alreadyReservedParkingSpots.Contains(x));

		if (string.IsNullOrEmpty(freeParkingSpot))
		{
			return BadRequest($"No parking spots free for date {requestedDate}");
		}
		else
		{
			var newReservation = new ParkingReservation
			{
				ParkingSpot = freeParkingSpot,
				ReservedDate = parsedDate,
				PersonName = personName
			};

			await _parkingRepository.InsertReservationAsync(newReservation);
			return Ok(freeParkingSpot);
		}
	}
}