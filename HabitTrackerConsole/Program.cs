using HabitTrackerConsole.Database;
using HabitTrackerConsole.Services;

namespace HabitTracker;

class Program
{
    static void Main(string[] args)
    {

        DatabaseContext dbContext = InitializeDatabase();
        var habitService = new HabitService(dbContext);
        var logEntryService = new LogEntryService(dbContext);

        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("Habit Tracker");
            Console.WriteLine("1. Add a new log entry");
            Console.WriteLine("2. Update an existing log entry");
            Console.WriteLine("3. Delete a log entry");
            Console.WriteLine("4. View all log entries");
            Console.WriteLine("5. Add a new Habit");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            int option = int.Parse(Console.ReadLine() ?? "0");

            switch (option)
            {
                case 1:
                    // add log entry
                    break;
                case 2:
                    // update existing log entry
                    break;
                case 3:
                    // delete log entry
                    break;
                case 4:
                    // view all log entries
                    break;
                case 5:
                    // add new habit
                    break;
                case 0:
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }

            if (running)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }

    static DatabaseContext InitializeDatabase()
    {
        string dbPath = "HabitTracker.db";
        var dbContext = new DatabaseContext(dbPath);
        var dbInitializer = new DatabaseInitializer(dbContext);
        try
        {
            dbInitializer.Initialize();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to initialize database, the application will now exit.");
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }
        return dbContext;
    }

}