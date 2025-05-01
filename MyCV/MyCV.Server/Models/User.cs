using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyCV.Server.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string? Username { get; set; }

        [DataType(DataType.EmailAddress)]
        public string? Mail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [NotMapped]
        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        public string? ConfirmedPassword { get; set; }

    }
}
