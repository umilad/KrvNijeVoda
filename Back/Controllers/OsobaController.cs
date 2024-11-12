using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;
using System.Threading.Tasks;
using KrvNijeVoda.Models;
using System.Reflection.Metadata;
[Route("api/[controller]")]
[ApiController]
public class OsobaController : ControllerBase
{
    private readonly Neo4jService _neo4jService;

    public OsobaController(Neo4jService neo4jService)
    {
        _neo4jService = neo4jService;
    }

[HttpGet("{osobaId}")]
public async Task<IActionResult> GetOsoba(Guid osobaId)
{
    try
    {
        using (var session = _neo4jService.GetSession())
        {
            var query = "MATCH (o:Osoba {OsobaID: $osobaId}) RETURN o.OsobaID as OsobaID, o.Ime as Ime, o.Prezime as Prezime, o.MestoRodjenja as MestoRodjenja, o.DatumRodjenja as DatumRodjenja, o.Slika as Slika, o.Pol as Pol, o.DatumSmrti as DatumSmrti, o.Biografija as Biografija, o.ZivMrtav as ZivMrtav ";
            var parameters = new { osobaId = osobaId.ToString() };
            var result = await session.RunAsync(query, parameters);

            var records = await result.ToListAsync();

            if (records.Count == 0)
            {
                return NotFound("Osoba not found");
            }

            var record = records.SingleOrDefault();

            // Accessing the properties directly
            var osobIdValue = record["OsobaID"]?.As<string>();

            if (osobIdValue == null)
            {
                return StatusCode(500, new 
                { 
                    Error = "Internal Server Error", 
                    Message = "UserId is null or not present in the Neo4j record",
                    DebugInfo = record?.Keys?.ToList() ?? new List<string>()
                });
            }

            return Ok(new 
            {
                Osoba = new Osoba
                {
                    OsobaID = Guid.Parse(osobIdValue),
                    Ime = record["Ime"]?.As<string>() ,
                    Prezime = record["Prezime"]?.As<string>() ,
                    MestoRodjenja = record["MestoRodjenja"]?.As<string>() ,
                    //DatumRodjenja = (DateTime)(record["DatumRodjenja"]?.As<DateTime>()),
                    Slika = record["Slika"]?.As<string>(),
                    Pol = record["Pol"]?.As<string>(),
                    Biografija= record["Biografija"]?.As<string>(),
                    ZivMrtav= record["ZivMrtav"]?.As<string>(),
                    

                },
            });
        }
    }
    catch (Exception ex)
    {
        // Log the error
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine($"StackTrace: {ex.StackTrace}");

        return StatusCode(500, new 
        { 
            Error = "Internal Server Error", 
            Message = ex.Message
        });
    }
}

[HttpPost]
public async Task<IActionResult> CreateOsoba([FromBody] Osoba osoba)
{
    try
    {
        using (var session = _neo4jService.GetSession())
        {
            // Example query: "CREATE (u:User {UserId: $userId, UserName: $userName, Email: $email})"
            var query = "CREATE (o:Osoba {OsobaID: $osobaId ,Ime: $ime, Prezime: $prezime, DatumRodjenja: $datumrodjenja, Pol: $pol, MestoRodjenja: $mestorodjenja, Slika: $slika, Biografija: $biografija, ZivMrtav: $zivmrtav, DatumSmrti: $datumsmrti})";
            
            var parameters = new
            {
                osobaId= Guid.NewGuid().ToString(),
                ime= osoba.Ime,
                prezime= osoba.Prezime,
                datumrodjenja= osoba.DatumRodjenja,
                mestorodjenja= osoba.MestoRodjenja,
                pol= osoba.Pol,
                slika= osoba.Slika,
                biografija= osoba.Biografija,
                datumsmrti= osoba.DatumSmrti,
                zivmrtav= osoba.ZivMrtav

            };

            await session.RunAsync(query, parameters);
        }

        return CreatedAtAction(nameof(CreateOsoba), new { osobaId = osoba.OsobaID }, osoba);
    }
    catch (Exception ex)
    {
        // Handle exceptions (e.g., log the error)
        return StatusCode(500, "Internal Server Error");
    }
}


   [HttpPut("{osobaId}")]
public async Task<IActionResult> UpdateOsobu(Guid osobaId, [FromBody] Osoba osoba)
{
    using (var session = _neo4jService.GetSession())
    {
        var query = $@"
            MATCH (o:Osoba {{OsobaID: '{osobaId}'}})
            SET 
                o.Ime = $ime,
                o.Prezime = $prezime,
                o.DatumRodjenja = $datumRodjenja,
                o.Pol = $pol,
                o.MestoRodjenja = $mestoRodjenja,
                o.Slika = $slika,
                o.DatumSmrti= $datumsmrti,
                o.Biografija= $biografija,
                o.ZivMrtav= $zivmrtav
            RETURN o";

        var parameters = new
        {

            ime = osoba.Ime,
            prezime = osoba.Prezime,
            datumRodjenja = osoba.DatumRodjenja,
            pol = osoba.Pol,
            mestoRodjenja = osoba.MestoRodjenja,
            slika = osoba.Slika,
            biografija= osoba.Biografija,
            zivmrtav= osoba.ZivMrtav,
            datumsmrti= osoba.DatumSmrti
            // Add other properties as needed
        };

        try
        {
            var result = await session.RunAsync(query, parameters);

            // You can handle the result if needed

            return NoContent();
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
}


[HttpDelete("{osobaId}")]
public async Task<IActionResult> DeleteUser(Guid osobaId)
{
    try
    {
        using (var session = _neo4jService.GetSession())
        {
            // Example query: "MATCH (u:User {UserId: $userId}) DETACH DELETE u"
            var query = "MATCH (o:Osoba {OsobaID: $osobaId}) DETACH DELETE o";
            
            var parameters = new
            {
                osobaId = osobaId.ToString()
            };

            await session.RunAsync(query, parameters);
        }

        return NoContent();
    }
    catch (Exception ex)
    {
        // Handle exceptions (e.g., log the error)
        return StatusCode(500, "Internal Server Error");
    }
}

}
