using HabitTrackerConsole.Database;
using HabitTrackerConsole.Models;
using System.Data.SQLite;

namespace HabitTrackerConsole.Services;

public class LogEntryService
{
    private readonly DatabaseContext dbContext;

    public LogEntryService(DatabaseContext context)
    {
        dbContext = context;
    }

    public bool InsertLogEntryIntoHabitLog(string date, int habitId, int quantity)
    {
        try
        {
            using (SQLiteConnection localDbConnection = dbContext.GetNewDatabaseConnection())
            {
                string sqlCommandString = @"
                    INSERT INTO tb_HabitLog (
                        date, habitid, quantity
                    )
                    VALUES (
                        @Date, @HabitId, @Quantity
                    )";


                using (var command = new SQLiteCommand(sqlCommandString, localDbConnection))
                {
                    command.Parameters.Add("@Date", System.Data.DbType.String).Value = date;
                    command.Parameters.Add("@HabitId", System.Data.DbType.Int32).Value = habitId;
                    command.Parameters.Add("@Quantity", System.Data.DbType.Int32).Value = quantity;
                    int affectedRows = command.ExecuteNonQuery();

                    if (affectedRows == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding log entry: {date}\t{habitId}\t{quantity}");
            Console.WriteLine($"{ex.Message}");
            return false;
        }
    }

    public bool UpdateLogEntryInHabitsLog(int id, int newQuantity)
    {
        try
        {
            using (SQLiteConnection localDbConnection = dbContext.GetNewDatabaseConnection())
            {
                string sqlCommandString = @"
                    UPDATE tb_HabitLog 
                        SET Quantity = @Quantity 
                        WHERE Id = @Id";

                using (var command = new SQLiteCommand(sqlCommandString, localDbConnection))
                {
                    command.Parameters.Add("@Quantity", System.Data.DbType.Int32).Value = newQuantity;
                    command.Parameters.Add("@Id", System.Data.DbType.Int32).Value = id;
                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding log entry: ID: {id}\tQuantity: {newQuantity}");
            Console.WriteLine($"{ex.Message}");
            return false;
        }
    }

    public bool DeleteLogEntryFromHabitsLog(int id)
    {
        try
        {
            using (SQLiteConnection localDbConnection = dbContext.GetNewDatabaseConnection())
            {
                string sqlCommandString = @"
                    DELETE FROM tb_HabitLog 
                    WHERE Id = @Id";

                using (var command = new SQLiteCommand(sqlCommandString, localDbConnection))
                {
                    command.Parameters.Add("@Id", System.Data.DbType.Int32).Value = id;
                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting log entry: ID: {id}");
            Console.WriteLine($"{ex.Message}");
            return false;
        }
    }

    public List<LogEntryViewModel> GetAllLogEntriesFromHabitsLogView()
    {
        List<LogEntryViewModel> logEntries = new List<LogEntryViewModel>();

        try
        {
            using (SQLiteConnection localDbConnection = dbContext.GetNewDatabaseConnection())
            {
                string sqlCommandString = @"
                SELECT 
                    RecordId, Date, HabitName, Quantity
                FROM vw_HabitLogEntries";

                using (var command = new SQLiteCommand(sqlCommandString, localDbConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows) return logEntries;  // return empty list if no rows are returned

                        while (reader.Read())
                        {
                            var logEntry = new LogEntryViewModel
                            {
                                RecordId = Convert.ToInt32(reader["RecordId"]),
                                Date = DateTime.Parse(reader["Date"].ToString()!),
                                HabitName = reader["HabitName"].ToString(),
                                Quantity = Convert.ToInt32(reader["Quantity"])
                            };

                            logEntries.Add(logEntry);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error retrieving log entries.");
            Console.WriteLine(ex.Message);
        }

        return logEntries;
    }

    public bool DeleteAllLogEntries()
    {
        try
        {
            using (SQLiteConnection localDbConnection = dbContext.GetNewDatabaseConnection())
            {
                string sqlCommandString = @"
                DELETE FROM tb_HabitLog";

                using (var command = new SQLiteCommand(sqlCommandString, localDbConnection))
                {
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting all log entries: {ex.Message}");
            return false;
        }
    }

}
