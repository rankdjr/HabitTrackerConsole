using HabitTrackerConsole.Services;
using HabitTrackerConsole.Util;
using Spectre.Console;

namespace HabitTrackerConsole.Application;

public class ApplicationHandler
{
    private readonly HabitService _habitService;
    private readonly LogEntryService _logEntryService;

    public ApplicationHandler(HabitService habitService, LogEntryService logEntryService)
    {
        _habitService = habitService;
        _logEntryService = logEntryService;
    }

    public void Run()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Markup("[underline green]Welcome to the Habit Tracker![/]\n");
            var option = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What would you like to do?")
                .PageSize(10)
                .AddChoices(Enum.GetNames(typeof(MainMenuOption)).Select(name => ApplicationHelper.SplitCamelCase(name))));


            MainMenuOption selectedOption = ApplicationHelper.FromFriendlyString<MainMenuOption>(option);

            switch (selectedOption)
            {
                case MainMenuOption.ViewAndEditHabits:
                    var habitApp = new HabitApplication(_habitService);
                    habitApp.Run();
                    break;
                case MainMenuOption.ViewAndEditLogs:
                    var logApp = new LogApplication(_logEntryService, _habitService);
                    logApp.Run();
                    break;
                case MainMenuOption.SeedDatabase:
                    SeedDatabase();
                    break;
                case MainMenuOption.Exit:
                    Console.WriteLine();
                    ApplicationHelper.AnsiWriteLine(new Markup("[grey]Goodbye![/]"));
                    return;
            }
        }
    }

    private void SeedDatabase()
    {
        AnsiConsole.WriteLine("Starting database seeding...");

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .Start("Processing...", ctx =>
            {
                DatabaseSeeder.SeedHabits(_habitService, 3);
                DatabaseSeeder.SeedLogEntries(_logEntryService, _habitService, 2);
            });

        AnsiConsole.Write(new Markup("\n[green]Database seeded successfully![/]\n"));
        ApplicationHelper.ShowReturnToMainMenuPrompt();
    }    
}

