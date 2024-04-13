using HabitTrackerConsole.Database;
using HabitTrackerConsole.Models;
using System.Data.SQLite;

namespace HabitTrackerConsole.Services;

public class HabitService
{
    private readonly DatabaseContext dbContext;

    public HabitService(DatabaseContext context)
    {
        dbContext = context;
    }

    public bool InsertHabitIntoHabitsTable(string name)
    {
        try
        {
            using (SQLiteConnection localDbConnection = dbContext.GetNewDatabaseConnection())
            {
                string sqlCommandString = @"
                    INSERT INTO tb_Habit 
                        (Name, DateCreated) 
                    VALUES 
                        (@Name, @DateCreated)";

                using (var command = new SQLiteCommand(sqlCommandString, localDbConnection))
                {
                    command.Parameters.Add("@Name", System.Data.DbType.String).Value = name;
                    command.Parameters.Add("@DateCreated", System.Data.DbType.String).Value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
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
            Console.WriteLine($"Error inserting new habit: {name}");
            Console.WriteLine($"{ex.Message}");
            return false;
        }
    }

    public bool UpdateHabit(int habitId, string newName)
    {
        try
        {
            using (var connection = dbContext.GetNewDatabaseConnection())
            {
                string sqlCommand = "UPDATE tb_Habit SET Name = @NewName WHERE Id = @HabitId";

                using (var command = new SQLiteCommand(sqlCommand, connection))
                {
                    command.Parameters.AddWithValue("@NewName", newName);
                    command.Parameters.AddWithValue("@HabitId", habitId);
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating habit: {ex.Message}");
            return false;
        }
    }

    public bool DeleteHabit(int habitId)
    {
        try
        {
            using (var connection = dbContext.GetNewDatabaseConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    // Delete all associated log entries
                    string deleteLogEntriesSql = "DELETE FROM tb_HabitLog WHERE HabitId = @HabitId";
                    using (var logCommand = new SQLiteCommand(deleteLogEntriesSql, connection))
                    {
                        logCommand.Parameters.AddWithValue("@HabitId", habitId);
                        logCommand.ExecuteNonQuery();
                    }

                    // Now, delete the habit
                    string deleteHabitSql = "DELETE FROM tb_Habit WHERE Id = @Id";
                    using (var habitCommand = new SQLiteCommand(deleteHabitSql, connection))
                    {
                        habitCommand.Parameters.AddWithValue("@Id", habitId);
                        habitCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting habit and its log entries: {ex.Message}");
            return false;
        }
    }


    public List<HabitViewModel> GetAllHabitsOverviews()
    {
        List<HabitViewModel> habitOverviews = new List<HabitViewModel>();

        try
        {
            using (SQLiteConnection localDbConnection = dbContext.GetNewDatabaseConnection())
            {
                string sqlCommandString = @"
                SELECT HabitId, HabitName, DateCreated, LastLogEntryDate, TotalLogs
                FROM vw_HabitSummary";

                using (var command = new SQLiteCommand(sqlCommandString, localDbConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows) return habitOverviews;  // return empty list if no rows are returned
                        while (reader.Read())
                        {
                            HabitViewModel overview = new HabitViewModel
                            {
                                HabitId = Convert.ToInt32(reader["HabitId"]),
                                HabitName = reader["HabitName"].ToString(),
                                DateCreated = reader["DateCreated"].ToString(),
                                LastLogEntryDate = reader.IsDBNull(reader.GetOrdinal("LastLogEntryDate")) ? null : reader["LastLogEntryDate"].ToString(),
                                TotalLogs = Convert.ToInt32(reader["TotalLogs"])
                            };
                            habitOverviews.Add(overview);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving habit overviews: {ex.Message}");
        }

        return habitOverviews;
    }
}
