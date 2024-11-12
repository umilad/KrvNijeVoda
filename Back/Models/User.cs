
namespace KrvNijeVoda.Models {
public class User
{
    public Guid UserId { get; set; }
    public string Password {get; set;}
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Ime {get; set;}
    public string Prezime {get; set;}
    public DateTime DatumRodjenja {get; set;}
    public string Pol {get; set;}
    public string? MestoRodjenja {get; set;}
    public string Slika {get;set;}
    
}

}