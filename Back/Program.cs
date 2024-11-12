var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<Neo4jService>(new Neo4jService("neo4j://localhost:7999", "neo4j", "password"));
builder.Services.AddControllers();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
