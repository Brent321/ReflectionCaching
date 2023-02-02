using System.ComponentModel.DataAnnotations;

namespace ReflectionCaching.Models
{
    internal class Person
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}