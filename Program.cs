using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Accessing an Environment Variable
string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
  ?? "Server=localhost;User ID=root;Password=password;Database=testdb";

MySqlConnection GetConnection() => new MySqlConnection(connectionString);

using (var conn = GetConnection())
{
  conn.Open();
  using var cmd = new MySqlCommand(
    "CREATE TABLE IF NOT EXISTS Messages (Id INT AUTO_INCREMENT PRIMARY KEY, Content TEXT, CreatedAt DATETIME);",
    conn
  );
  cmd.ExecuteNonQuery();
}

app.MapGet("/", () => "The C# Messaging API is Online!!");

app.MapGet("/messages", async () =>
{
  var messages = new List<object>();
  using var conn = GetConnection();
  await conn.OpenAsync();
  using var cmd = new MySqlCommand("SELECT Content, CreatedAt FROM Messages ORDER BY CreatedAt DESC", conn);
  using var reader = await cmd.ExecuteReaderAsync();
  while (await reader.ReadAsync())
  {
    messages.Add(new { Content = reader.GetString(0), Date = reader.GetDateTime(1) });
  }
  return Results.Ok(messages);
});

app.MapPost("/messages", async (string content) =>
{
  using var conn = GetConnection();
  await conn.OpenAsync();
  using var cmd = new MySqlCommand("INSERT INTO Messages (Content, CreatedAt) VALUES (@content, @now)", conn);
  cmd.Parameters.AddWithValue("@content", content);
  cmd.Parameters.AddWithValue("@now", DateTime.Now);
  await cmd.ExecuteNonQueryAsync();
  return Results.Created($"/messages", new { Status = "Saved!", Content = content });
});

app.Run("http://0.0.0.0:5000");

