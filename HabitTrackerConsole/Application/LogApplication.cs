using HabitTrackerConsole.Models;
using HabitTrackerConsole.Services;
using HabitTrackerConsole.Util;
using Spectre.Console;

namespace HabitTrackerConsole.Application;

public class LogApplication
{
    private readonly LogEntryService _logEntryService;
    private readonly HabitService _habitService;

    public LogApplication(LogEntryService logEntryService, HabitService habitService)
    {
        _logEntryService = logEntryService;
        _habitService = habitService;
    }

    public void Run()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Markup("[underline green]Select an option[/]\n");
            var option = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Manage Logs")
                .PageSize(10)
                .AddChoices(Enum.GetNames(typeof(LogMenuOption)).Select(ApplicationHelper.SplitCamelCase)));

            switch (Enum.Parse<LogMenuOption>(option.Replace(" ", "")))
            {
                case LogMenuOption.ViewLogEntries:
                    ViewLogEntries();
                    break;
                case LogMenuOption.AddLogEntry:
                    AddLogEntry();
                    break;
                case LogMenuOption.UpdateLogEntry:
                    UpdateLogEntry();
                    break;
                case LogMenuOption.DeleteLogEntry:
                    DeleteLogEntry();
                    break;
                case LogMenuOption.DeleteAllLogEntries:
                    DeleteAllLogEntries();
                    break;
                case LogMenuOption.ReturnToMainMenu:
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
            ApplicationHelper.AnsiWriteLine(new Markup("[green]Log entry added successfully![/]"));
        }
        else
        {
            ApplicationHelper.AnsiWriteLine(new Markup("[red]Failed to add log entry.[/]"));
        }
    }

    private void UpdateLogEntry()
    {
        // Retrieve all log entries to display for selection
        var entries = _logEntryService.GetAllLogEntriesFromHabitsLogView();
        if (!entries.Any())
        {
            AnsiConsole.Markup("[red]No log entries available to update.[/]");
            return;
        }

        // Prompt user to select a log entry to update
        var entryToUpdate = AnsiConsole.Prompt(
            new SelectionPrompt<LogEntryViewModel>()
                .Title("Select a log entry to update:")
                .PageSize(10)
                .UseConverter(entry => $"ID: {entry.RecordId}, Date: {entry.Date:yyyy-MM-dd}, Habit: {entry.HabitName}, Quantity: {entry.Quantity}")
                .AddChoices(entries));

        // Ask user for the new quantity
        int newQuantity = AnsiConsole.Ask<int>($"Enter the new quantity for log entry ID {entryToUpdate.RecordId}:");

        // Confirm the update operation
        if (AnsiConsole.Confirm($"Are you sure you want to update the quantity for this log entry (ID: {entryToUpdate.RecordId}) to {newQuantity}?"))
        {
            // Update log entry using LogEntryService
            bool result = _logEntryService.UpdateLogEntryInHabitsLog(entryToUpdate.RecordId, newQuantity);
            if (result)
                AnsiConsole.Markup("[green]Log entry updated successfully![/]");
            else
                AnsiConsole.Markup("[red]Failed to update log entry.[/]");
        }
        else
        {
            AnsiConsole.WriteLine("Update operation cancelled.");
        }

        ApplicationHelper.PauseForContinueInput();
    }


    private void DeleteLogEntry()
    {
        var entries = _logEntryService.GetAllLogEntriesFromHabitsLogView();
        if (!entries.Any())
        {
            AnsiConsole.Markup("[red]No log entries available to delete.[/]");
            return;
        }

        var entryToDelete = AnsiConsole.Prompt(
            new SelectionPrompt<LogEntryViewModel>()
                .Title("Which log entry would you like to delete?")
                .PageSize(10)
                .UseConverter(entry => $"ID: {entry.RecordId}, Date: {entry.Date:yyyy-MM-dd}, Habit: {entry.HabitName}, Quantity: {entry.Quantity}")
                .AddChoices(entries));

        if (AnsiConsole.Confirm($"Are you sure you want to delete this log entry (ID: {entryToDelete.RecordId})?"))
        {
            bool result = _logEntryService.DeleteLogEntryFromHabitsLog(entryToDelete.RecordId);
            if (result)
                AnsiConsole.Markup("[green]Log entry successfully deleted![/]");
            else
                AnsiConsole.Markup("[red]Failed to delete log entry.[/]");
        }
        else
        {
            AnsiConsole.WriteLine("Operation cancelled.");
        }

        ApplicationHelper.PauseForContinueInput();
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
            table.AddRow(entry.RecordId.ToString(), entry.Date.ToString("yyyy-MM-dd"), entry.HabitName!, entry.Quantity.ToString());
        }

        AnsiConsole.Write(table);
        ApplicationHelper.PauseForContinueInput();
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
            AnsiConsole.Write(new Markup("[green]All log entries have been successfully deleted![/]\n"));
        }
        else
        {
            AnsiConsole.Write(new Markup("[red]Failed to delete log entries.[/]\n"));
        }

        ApplicationHelper.PauseForContinueInput();
    }
}
