using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Pobieranie listy pracowników
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            return Ok(employees);
        }

        // Dodawanie nowego pracownika (tylko admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            if (string.IsNullOrEmpty(employee.Name))
                return BadRequest("Nazwa pracownika jest wymagana.");

            _context.Employees.Add(employee);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest($"Błąd podczas dodawania pracownika: {ex.Message}");
            }

            return Ok("Pracownik został dodany pomyślnie.");
        }

        // Edytowanie istniejącego pracownika (tylko admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee(string id, [FromBody] Employee updatedEmployee)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound("Pracownik nie został znaleziony.");

            employee.Name = updatedEmployee.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest($"Błąd podczas aktualizacji pracownika: {ex.Message}");
            }

            return Ok("Pracownik został zaktualizowany pomyślnie.");
        }

        // Usuwanie pracownika (tylko admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound("Pracownik nie został znaleziony.");

            _context.Employees.Remove(employee);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest($"Błąd podczas usuwania pracownika: {ex.Message}");
            }

            return Ok("Pracownik został usunięty pomyślnie.");
        }
    }
}
