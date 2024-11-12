using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;
using System.Threading.Tasks;
using KrvNijeVoda.Models;
using System.Reflection.Metadata;
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly Neo4jService _neo4jService;

    public UserController(Neo4jService neo4jService)
    {
        _neo4jService = neo4jService;
    }

[HttpGet("{userId}")]
public async Task<IActionResult> GetUser(Guid userId)
{
    try
    {
        using (var session = _neo4jService.GetSession())
        {
            var query = "MATCH (u:User {UserId: $userId}) RETURN u.UserId as UserId, u.UserName as UserName, u.Email as Email, u.Password as Password, u.Ime as Ime, u.Prezime as Prezime, u.MestoRodjenja as MestoRodjenja, u.DatumRodjenja as DatumRodjenja, u.Slika as Slika, u.Pol as Pol";
            var parameters = new { userId = userId.ToString() };
            var result = await session.RunAsync(query, parameters);

            var records = await result.ToListAsync();

            if (records.Count == 0)
            {
                return NotFound("User not found");
            }

            var record = records.SingleOrDefault();

            // Accessing the properties directly
            var userIdValue = record["UserId"]?.As<string>();

            if (userIdValue == null)
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
                User = new User
                {
                    UserId = Guid.Parse(userIdValue),
                    UserName = record["UserName"]?.As<string>(),
                    Email = record["Email"]?.As<string>() ,
                    Ime = record["Ime"]?.As<string>() ,
                    Prezime = record["Prezime"]?.As<string>() ,
                    MestoRodjenja = record["MestoRodjenja"]?.As<string>() ,
                    //DatumRodjenja = (DateTime)(record["DatumRodjenja"]?.As<DateTime>()),
                    Slika = record["Slika"]?.As<string>(),
                    Pol = record["Pol"]?.As<string>(),
                    Password= record["Password"]?.As<string>()

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
public async Task<IActionResult> CreateUser([FromBody] User user)
{
    try
    {
        using (var session = _neo4jService.GetSession())
        {
            // Example query: "CREATE (u:User {UserId: $userId, UserName: $userName, Email: $email})"
            var query = "CREATE (u:User {UserId: $userId, Password: $password, UserName: $userName, Email: $email, Ime: $ime, Prezime: $prezime, DatumRodjenja: $datumrodjenja, Pol: $pol, MestoRodjenja: $mestorodjenja, Slika: $slika})";
            
            var parameters = new
            {
                userId= Guid.NewGuid().ToString(),
                userName = user.UserName,
                email = user.Email,
                ime= user.Ime,
                prezime= user.Prezime,
                datumrodjenja= user.DatumRodjenja,
                mestorodjenja= user.MestoRodjenja,
                pol= user.Pol,
                slika= user.Slika,
                password= user.Password
            };

            await session.RunAsync(query, parameters);
        }

        return CreatedAtAction(nameof(GetUser), new { userId = user.UserId }, user);
    }
    catch (Exception ex)
    {
        // Handle exceptions (e.g., log the error)
        return StatusCode(500, "Internal Server Error");
    }
}


   [HttpPut("{userId}")]
public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] User user)
{
    using (var session = _neo4jService.GetSession())
    {
        var query = $@"
            MATCH (u:User {{UserId: '{userId}'}})
            SET u.UserName = $userName, 
                u.Email = $email,
                u.Ime = $ime,
                u.Prezime = $prezime,
                u.DatumRodjenja = $datumRodjenja,
                u.Pol = $pol,
                u.MestoRodjenja = $mestoRodjenja,
                u.Slika = $slika
            RETURN u";

        var parameters = new
        {
            userName = user.UserName,
            email = user.Email,
            ime = user.Ime,
            prezime = user.Prezime,
            datumRodjenja = user.DatumRodjenja,
            pol = user.Pol,
            mestoRodjenja = user.MestoRodjenja,
            slika = user.Slika
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


    [HttpDelete("{userId}")]
public async Task<IActionResult> DeleteUser(Guid userId)
{
    try
    {
        using (var session = _neo4jService.GetSession())
        {
            // Example query: "MATCH (u:User {UserId: $userId}) DETACH DELETE u"
            var query = "MATCH (u:User {UserId: $userId}) DETACH DELETE u";
            
            var parameters = new
            {
                userId = userId.ToString()
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
