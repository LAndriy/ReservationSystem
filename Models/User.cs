using Microsoft.AspNetCore.Identity;

namespace ReservationSystem.Models
{
    public class User : IdentityUser
    {
        public string GoogleAccountId { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
