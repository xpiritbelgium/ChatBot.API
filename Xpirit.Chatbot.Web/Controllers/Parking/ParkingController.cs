using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Xpirit.Chatbot.Domain.Contracts;
using Xpirit.Chatbot.Domain.Entities;

[ApiController]
[Route("[controller]")]
public class ParkingController : ControllerBase
{
    private readonly IParkingRepository _parkingRepository;
    //private static List<string> _parkingSpots = new() { "Parking 42", "Parking 43", "Parking 44" };

    public ParkingController(IParkingRepository parkingRepository)
    {
        _parkingRepository = parkingRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<string>>> GetParkingSpots()
    {
        var parkingSpots = await _parkingRepository.GetAllParkingSpotsAsync();

        return Ok(parkingSpots?.Select(x => x.Name)?.ToList() ?? new List<string>());
    }

    [HttpGet("Reservations")]
    public async Task<ActionResult<IEnumerable<ParkingReservation>>> GetAllReservations(string? personName)
    {
        if (string.IsNullOrEmpty(personName))
        {
            var reservations = await _parkingRepository.GetAllReservationsAsync();
            return Ok(reservations);
        }
        else
        {
            var reservations = await _parkingRepository.GetAllFutureReservationsForPerson(personName);
            return Ok(reservations);
        }
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

        var parkingSpots = await _parkingRepository.GetAllParkingSpotsAsync();

        var freeParkingSpots = parkingSpots.Where(x => !alreadyReservedParkingSpots.Contains(x.Name)).ToList();

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

        var parkingSpots = await _parkingRepository.GetAllParkingSpotsAsync();
        var freeParkingSpot = parkingSpots.FirstOrDefault(x => !alreadyReservedParkingSpots.Contains(x.Name));

        if (string.IsNullOrEmpty(freeParkingSpot?.Name))
        {
            return BadRequest($"No parking spots free for date {requestedDate}");
        }
        else
        {
            var newReservation = new ParkingReservation
            {
                ParkingSpot = freeParkingSpot.Name,
                ReservedDate = parsedDate,
                PersonName = personName
            };

            await _parkingRepository.InsertReservationAsync(newReservation);
            return Ok(freeParkingSpot);
        }
    }

    [HttpDelete("Reservation")]
    public async Task<ActionResult<string>> RemoveReservation(string? requestedDate, string personName)
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

        await _parkingRepository.RemoveReservationAsync(parsedDate, personName);
        return Ok();
    }
}