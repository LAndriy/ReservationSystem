using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationSystem.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public bool IsPaid { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
