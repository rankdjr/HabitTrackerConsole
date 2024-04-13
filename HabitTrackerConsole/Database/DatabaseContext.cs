using System.Data.SQLite;

namespace HabitTrackerConsole.Database;

public class DatabaseContext
{
    private readonly string dbConnectionString;

    public DatabaseContext(string dbPath)
    {
        dbConnectionString = $"Data Source={dbPath};Version=3;";
    }

    public SQLiteConnection GetNewDatabaseConnection()
    {
        var connection = new SQLiteConnection(dbConnectionString);
        try
        {
            connection.Open();
            return connection;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error opening database connection: {ex.Message}");
            connection.Dispose();
            throw;
        }
    }
}
