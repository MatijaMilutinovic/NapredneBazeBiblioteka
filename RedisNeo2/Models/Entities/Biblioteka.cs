using System.ComponentModel.DataAnnotations;

namespace RedisNeo2.Models.Entities
{
    public class Biblioteka
    {
        [Key]
        public int Pib { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string KontaktTelefon { get; set; } = string.Empty;
        public string Adresa { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string Lozinka { get; set; } = string.Empty;
        public string Role { get; set; } = "Biblioteka";
        public List<Dogadjaj> SpisakDogadjaja { get; set; } = new List<Dogadjaj>();

    }
}
