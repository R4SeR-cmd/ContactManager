using System.ComponentModel.DataAnnotations;

namespace ContactManager.Models
{
    public class Person
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public bool Married { get; set; }

        [Required]
        [StringLength(20)]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
