using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Pobieranie rezerwacji użytkownika
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetReservations()
        {
            var userId = User.FindFirst("sub")?.Value;
            var userRole = User.FindFirst("role")?.Value;

            if (userRole == "Admin")
            {
                var allReservations = await _context.Reservations.Include(r => r.Employee).ToListAsync();
                return Ok(allReservations);
            }

            var userReservations = await _context.Reservations
                .Where(r => r.UserId == userId)
                .Include(r => r.Employee)
                .ToListAsync();

            return Ok(userReservations);
        }

        // Tworzenie nowej rezerwacji
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReservation([FromBody] Reservation reservation)
        {
            var userId = User.FindFirst("sub")?.Value;

            reservation.UserId = userId;
            _context.Reservations.Add(reservation);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest($"Błąd podczas zapisywania rezerwacji: {ex.Message}");
            }

            return Ok("Rezerwacja utworzona pomyślnie.");
        }

        // Edytowanie rezerwacji
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] Reservation updatedReservation)
        {
            var userId = User.FindFirst("sub")?.Value;
            var userRole = User.FindFirst("role")?.Value;

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound("Rezerwacja nie została znaleziona.");

            if (reservation.UserId != userId && userRole != "Admin")
                return Forbid("Nie masz uprawnień do edycji tej rezerwacji.");

            reservation.Date = updatedReservation.Date;
            reservation.EmployeeId = updatedReservation.EmployeeId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest($"Błąd podczas aktualizacji rezerwacji: {ex.Message}");
            }

            return Ok("Rezerwacja zaktualizowana pomyślnie.");
        }

        // Usuwanie rezerwacji
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var userId = User.FindFirst("sub")?.Value;
            var userRole = User.FindFirst("role")?.Value;

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound("Rezerwacja nie została znaleziona.");

            if (reservation.UserId != userId && userRole != "Admin")
                return Forbid("Nie masz uprawnień do usunięcia tej rezerwacji.");

            _context.Reservations.Remove(reservation);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest($"Błąd podczas usuwania rezerwacji: {ex.Message}");
            }

            return Ok("Rezerwacja została usunięta.");
        }
    }
}
