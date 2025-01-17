using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Models;

namespace ReservationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Endpoint do przypisania roli użytkownikowi
        [HttpPost("AssignRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound($"Nie znaleziono użytkownika o ID: {request.UserId}");
            }

            if (!await _roleManager.RoleExistsAsync(request.Role))
            {
                return BadRequest($"Rola {request.Role} nie istnieje.");
            }

            var result = await _userManager.AddToRoleAsync(user, request.Role);
            if (result.Succeeded)
            {
                return Ok($"Rola {request.Role} została przypisana użytkownikowi {user.UserName}.");
            }

            return BadRequest("Nie udało się przypisać roli: " +
                               string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    public class AssignRoleRequest
    {
        public string UserId { get; set; }
        public string Role { get; set; }
    }
}
