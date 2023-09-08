using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Xpirit.Parking.Controllers.Parking
{
    [ApiController]
    [Route("[controller]")]
    public class ParkingController : ControllerBase
    {
        private static List<Tuple<string, DateTime, string>> _reservations = new List<Tuple<string, DateTime, string>>();
        private static List<string> _parkingSpots = new List<string> { "Parking42", "Parking43", "Parking44" };

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetParkingSpots()
        {
            return Ok(_parkingSpots);
        }

        [HttpGet("Reservations")]
        public ActionResult<IEnumerable<Tuple<string, DateTime, string>>> GetAllReservations()
        {
            return Ok(_reservations);
        }

        [HttpGet("Reservations/Available")]
        public ActionResult<IEnumerable<string>> GetAvalableParkingSpotsForDate(DateTime? requestedDate)
        {
            if (requestedDate == null)
            {
                return BadRequest($"{nameof(requestedDate)} is not filled in");
            }

            var alreadyReservedparkingSpots = _reservations.Where(x => x.Item2 == requestedDate.Value.Date).Select(x => x.Item1).ToList();
            var freeParkingSpots = _parkingSpots.Where(x => !alreadyReservedparkingSpots.Contains(x)).ToList();

            return Ok(freeParkingSpots);
        }

        [HttpPost("Reservation")]
        public ActionResult<string> MakeReservation(string? requestedDate, string personName)
        {
            if (requestedDate == null)
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

            var alreadyReservedparkingSpots = _reservations.Where(x => x.Item2 == parsedDate.Date).Select(x => x.Item1).ToList();
            var freeParkingSpot = _parkingSpots.FirstOrDefault(x => !alreadyReservedparkingSpots.Contains(x));

            if (string.IsNullOrEmpty(freeParkingSpot))
            {
                return BadRequest($"No parking spots free for date {requestedDate}");
            }
            else
            {
                _reservations.Add(new Tuple<string, DateTime, string>(freeParkingSpot, parsedDate, personName));
                return Ok(freeParkingSpot);
            }
        }
    }
}