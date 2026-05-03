using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Accessing an Environment Variable
string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
  ?? "Server=localhost;User ID=root;Password=password;Database=testdb";

app.MapGet("/", () => "The C# API is running!!");

app.MapGet("/db-check", async () =>
{
  try
  {
    using var connection = new MySqlConnection(connectionString);
    await connection.OpenAsync();
    return Results.Ok(new { Message = "Successfully connected to MySQL on Fedora!", Time = DateTime.Now });
  }
  catch (Exception ex)
  {
    return Results.Problem($"Database connection failed: {ex.Message}");
  }
});

app.Run("http://0.0.0.0:5000");

