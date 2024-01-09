using System.ComponentModel.DataAnnotations;

namespace RedisNeo2.Models.Entities
{
    public class Korisnik
    {
        [Key]
        public int Id { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string Lozinka { get; set; } = string.Empty;
        public string Role { get; set; } = "Korisnik";
        public string KontaktTelefon { get; set; } = string.Empty;
    }
}
