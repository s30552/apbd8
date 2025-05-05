using System.ComponentModel.DataAnnotations;

namespace Tutorial8.Models.DTOs
{
    public class ClientCreateDTO
    {
        [Required, StringLength(120)]
        public string FirstName { get; set; }

        [Required, StringLength(120)]
        public string LastName { get; set; }

        [Required, EmailAddress, StringLength(120)]
        public string Email { get; set; }

        [Required, Phone, StringLength(120)]
        public string Telephone { get; set; }

        [Required, RegularExpression(@"^\d{11}$", ErrorMessage = "Pesel must be 11 digits")]
        public string Pesel { get; set; }
    }
}