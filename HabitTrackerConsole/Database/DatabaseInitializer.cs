namespace HabitTrackerConsole.Database;

public class DatabaseInitializer
{
    private readonly DatabaseContext dbContext;

    private static readonly string CreateHabitTableCommand = @"
        CREATE TABLE IF NOT EXISTS tb_Habit (
            Id INTEGER PRIMARY KEY,
            Name TEXT NOT NULL,
            DateCreated TEXT NOT NULL
        )";

    private static readonly string CreateHabitRecordTableCommand = @"
        CREATE TABLE IF NOT EXISTS tb_HabitLog (
            Id INTEGER PRIMARY KEY,
            Date TEXT NOT NULL,
            HabitId INTEGER NOT NULL,
            Quantity INTEGER NOT NULL,
            FOREIGN KEY (HabitId) REFERENCES tb_Habit(Id)
        )";

    private static readonly string CreateHabitSummaryViewCommand = @"
        CREATE VIEW IF NOT EXISTS vw_HabitSummary AS
        SELECT h.Id AS HabitId, h.Name AS HabitName, h.DateCreated, MAX(l.Date) AS LastLogEntryDate, COUNT(l.HabitId) AS TotalLogs
        FROM tb_Habit h
        LEFT JOIN tb_HabitLog l ON h.Id = l.HabitId
        GROUP BY h.Id, h.Name, h.DateCreated";

    private static readonly string CreateHabitLogEntriesViewCommand = @"
        CREATE VIEW IF NOT EXISTS vw_HabitLogEntries AS
        SELECT hr.Id AS RecordId, hr.Date, h.Name AS HabitName, hr.Quantity
        FROM tb_HabitLog hr
        JOIN tb_Habit h ON hr.HabitId = h.Id";


    public DatabaseInitializer(DatabaseContext context)
    {
        dbContext = context;
    }

    public void Initialize()
    {
        try
        {
            using (var connection = dbContext.GetNewDatabaseConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        SqliteHelper.ExecuteCommand(CreateHabitTableCommand, connection, transaction: transaction);
                        SqliteHelper.ExecuteCommand(CreateHabitRecordTableCommand, connection, transaction: transaction);
                        SqliteHelper.ExecuteCommand(CreateHabitSummaryViewCommand, connection, transaction: transaction);
                        SqliteHelper.ExecuteCommand(CreateHabitLogEntriesViewCommand, connection, transaction: transaction);

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing the database: {ex.Message}");
            throw;
        }
    }
}
