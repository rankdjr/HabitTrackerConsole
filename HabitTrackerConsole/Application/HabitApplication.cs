using HabitTrackerConsole.Models;
using HabitTrackerConsole.Services;
using HabitTrackerConsole.Util;
using Spectre.Console;

namespace HabitTrackerConsole.Application;

public class HabitApplication
{
    private readonly HabitService _habitService;

    public HabitApplication(HabitService habitService)
    {
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
                .Title("Manage Habits")
                .PageSize(10)
                .AddChoices(Enum.GetNames(typeof(HabitMenuOption)).Select(ApplicationHelper.SplitCamelCase)));

            switch (Enum.Parse<HabitMenuOption>(option.Replace(" ", "")))
            {
                case HabitMenuOption.AddNewHabit:
                    AddHabit();
                    break;
                case HabitMenuOption.UpdateHabit:
                    UpdateHabit();
                    break;
                case HabitMenuOption.ViewHabitInformation:
                    ViewHabits();
                    break;
                case HabitMenuOption.DeleteHabit:
                    DeleteHabit();
                    break;
                case HabitMenuOption.ReturnToMainMenu:
                    return;
            }
        }
    }


    private void AddHabit()
    {
        string habitName = AnsiConsole.Ask<string>("Enter the name of the new habit:");
        if (_habitService.InsertHabitIntoHabitsTable(habitName))
        {
            AnsiConsole.Markup("\n[green]Habit added successfully![/]\n");
        }
        else
        {
            AnsiConsole.Markup("\n[red]Failed to add habit.[/]\n");
        }

        ApplicationHelper.ShowReturnToMainMenuPrompt();
    }

    private void UpdateHabit()
    {
        var habits = _habitService.GetAllHabitsOverviews();
        if (habits.Count == 0)
        {
            AnsiConsole.Markup("[red]No habits available to update.[/]");
            return;
        }

        var habitToUpdate = AnsiConsole.Prompt(
            new SelectionPrompt<HabitViewModel>()
                .Title("Which habit would you like to update?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to see more habits)[/]")
                .UseConverter(h => $"{h.HabitName} (ID: {h.HabitId})")
                .AddChoices(habits));

        string newName = AnsiConsole.Ask<string>("Enter the new name for the habit:");
        if (_habitService.UpdateHabit(habitToUpdate.HabitId, newName))
        {
            AnsiConsole.Markup("[green]Habit updated successfully![/]");
        }
        else
        {
            AnsiConsole.Markup("[red]Failed to update habit.[/]");
        }

        ApplicationHelper.ShowReturnToMainMenuPrompt();
    }

    private void DeleteHabit()
    {
        var habits = _habitService.GetAllHabitsOverviews();
        if (habits.Count == 0)
        {
            AnsiConsole.Markup("[red]No habits available to delete.[/]");
            return;
        }

        var habitToDelete = AnsiConsole.Prompt(
            new SelectionPrompt<HabitViewModel>()
                .Title("Which habit would you like to delete?")
                .PageSize(10)
                .UseConverter(h => $"{h.HabitName} (ID: {h.HabitId})")
                .AddChoices(habits));

        if (AnsiConsole.Confirm($"Are you sure you want to delete the habit '{habitToDelete.HabitName}' and all its log entries?"))
        {
            bool result = _habitService.DeleteHabit(habitToDelete.HabitId);
            if (result)
                AnsiConsole.Markup("[green]Habit and all related log entries successfully deleted![/]");
            else
                AnsiConsole.Markup("[red]Failed to delete the habit and log entries.[/]");
        }
        else
        {
            AnsiConsole.WriteLine("Operation cancelled.");
        }

        AnsiConsole.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey();
    }

    private void ViewHabits()
    {
        var habits = _habitService.GetAllHabitsOverviews();
        if (habits.Count == 0)
        {
            AnsiConsole.Markup("[yellow]No habits found![/]");
        }
        else
        {
            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn(new TableColumn("[u]ID[/]").Centered());
            table.AddColumn(new TableColumn("[u]Name[/]").Centered());
            table.AddColumn(new TableColumn("[u]Date Created[/]").Centered());
            table.AddColumn(new TableColumn("[u]Last Log Entry Date[/]").Centered());
            table.AddColumn(new TableColumn("[u]Total Logs[/]").Centered());

            foreach (var habit in habits)
            {
                table.AddRow(
                    habit.HabitId.ToString(),
                    habit.HabitName,
                    habit.DateCreated,
                    habit.LastLogEntryDate ?? "N/A",
                    habit.TotalLogs.ToString());
            }

            AnsiConsole.Write(table);
        }

        ApplicationHelper.ShowReturnToMainMenuPrompt();
    }


}
