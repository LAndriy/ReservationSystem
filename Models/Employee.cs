using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReservationSystem.Models
{
    public class Employee
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
