using System.ComponentModel.DataAnnotations;

namespace ValidationExtensions.Test.Models
{
    public class Person: IValidateable
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}