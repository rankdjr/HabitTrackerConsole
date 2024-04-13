using HabitTrackerConsole.Services;
using HabitTrackerConsole.Util;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    .AddChoices(Enum.GetNames(typeof(MenuOption)).Select(e => e.Replace('_', ' '))));

            MenuOption selectedOption = Enum.Parse<MenuOption>(option.Replace(' ', '_'));

            switch (selectedOption)
            {
                case MenuOption.AddLogEntry:
                    AddLogEntry();
                    break;
                case MenuOption.UpdateLogEntry:
                    UpdateLogEntry();
                    break;
                case MenuOption.DeleteLogEntry:
                    DeleteLogEntry();
                    break;
                case MenuOption.DeleteAllLogEntries:
                    DeleteAllLogEntries();
                    break;
                case MenuOption.ViewLogEntries:
                    ViewLogEntries();
                    break;
                case MenuOption.AddNewHabit:
                    AddHabit();
                    break;
                case MenuOption.SeedDatabase:
                    SeedDatabase();
                    break;
                case MenuOption.Exit:
                    AnsiConsole.Markup("[grey]Goodbye![/]");
                    return;
            }
        }
    }

    private void AddLogEntry()
    {
        var habits = _habitService.GetAllHabitsOverviews();
        if (!habits.Any())
        {
            AnsiConsole.Markup("[red]No habits available to add a log entry. Please add a habit first.[/]");
            return;
        }

        var habitOptions = habits.Select(h => $"{h.HabitName} (ID: {h.HabitId})").ToArray();
        string selectedHabit = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a habit:")
                .PageSize(10)
                .AddChoices(habitOptions));

        int habitId = int.Parse(selectedHabit.Split("(ID: ").Last().TrimEnd(')'));

        var date = AnsiConsole.Ask<DateTime>("Enter the date for the log entry:");
        var quantity = AnsiConsole.Ask<int>("Enter the quantity:");

        if (_logEntryService.InsertLogEntryIntoHabitLog(date.ToString("yyyy-MM-dd"), habitId, quantity))
        {
            AnsiConsole.Markup("[green]Log entry added successfully![/]");
        }
        else
        {
            AnsiConsole.Markup("[red]Failed to add log entry.[/]");
        }
    }

    private void UpdateLogEntry()
    {
        // Similar logic for fetching log entry details and updating
    }

    private void DeleteLogEntry()
    {
        // Logic to select and delete a log entry
    }

    private void ViewLogEntries()
    {
        var entries = _logEntryService.GetAllLogEntriesFromHabitsLogView();
        if (!entries.Any())
        {
            AnsiConsole.Markup("[yellow]No log entries found![/]");
            return;
        }

        var table = new Table();
        table.Border(TableBorder.Rounded);
        table.Title("Log Entries");
        table.AddColumn("Record ID");
        table.AddColumn("Date");
        table.AddColumn("Habit Name");
        table.AddColumn("Quantity");

        foreach (var entry in entries)
        {
            table.AddRow(entry.RecordId.ToString(), entry.Date.ToString("yyyy-MM-dd"), entry.HabitName, entry.Quantity.ToString());
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey();
    }

    private void AddHabit()
    {
        string habitName = AnsiConsole.Ask<string>("Enter the name of the new habit:");
        if (_habitService.InsertHabitIntoHabitsTable(habitName))
        {
            AnsiConsole.Markup("[green]Habit added successfully![/]");
        }
        else
        {
            AnsiConsole.Markup("[red]Failed to add habit.[/]");
        }
    }

    private void SeedDatabase()
    {
        AnsiConsole.WriteLine("Starting database seeding...");

        // Use a spinner to show activity
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .Start("Processing...", ctx =>
            {
                DatabaseSeeder.SeedHabits(_habitService, 3);
                DatabaseSeeder.SeedLogEntries(_logEntryService, _habitService, 5);

                // Keep the spinner for a minimum amount of time to ensure it's seen
                System.Threading.Thread.Sleep(2000); // Sleep for 2 seconds for the visual effect
            });

        AnsiConsole.WriteLine("Database seeded successfully!");
        Console.ReadKey();  // Wait for user to read the message
        AnsiConsole.Clear();
    }

    private void DeleteAllLogEntries()
    {
        if (!AnsiConsole.Confirm("Are you sure you want to delete ALL log entries?"))
        {
            AnsiConsole.Markup("[yellow]Operation cancelled.[/]");
            return;
        }

        if (_logEntryService.DeleteAllLogEntries())
        {
            AnsiConsole.Markup("[green]All log entries have been successfully deleted![/]");
        }
        else
        {
            AnsiConsole.Markup("[red]Failed to delete log entries.[/]");
        }
    }


}

