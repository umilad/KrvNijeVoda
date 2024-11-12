
namespace KrvNijeVoda.Models {
public class Osoba
{
    public Guid OsobaID { get; set; }
    public string Ime {get; set;}
    public string Prezime {get; set;}
    public DateTime DatumRodjenja {get; set;}
    public DateTime? DatumSmrti {get; set;}
    public string Pol {get; set;}
    public string? MestoRodjenja {get; set;}
    public string Slika {get;set;}
    public string ZivMrtav {get; set;}
    public string? Biografija {get;set;}
    
}

}