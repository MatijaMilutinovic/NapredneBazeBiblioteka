using System.ComponentModel.DataAnnotations;

namespace RedisNeo2.Models.Entities
{
    public class Dogadjaj
    {
        [Key]
        public string Naziv { get; set; } = string.Empty;
        public int BrojUcesnika { get; set; }
        public int TrenutniBrojUcesnika { get; set; } = 0;
        public string Opis { get; set; } = string.Empty;
        public string Kategorije { get; set; } = string.Empty;
        public string Lokacija { get; set; } = string.Empty;
        public DateTime DatumOdvijanja { get; set; }
        public List<Korisnik> PrijavljeniKorisnici { get; set; } = new List<Korisnik>();
        public List<Korisnik> PrimljeniKorisnici { get; set; } = new List<Korisnik>();
        public string Organizator { get; set; } = string.Empty;
    }
}
