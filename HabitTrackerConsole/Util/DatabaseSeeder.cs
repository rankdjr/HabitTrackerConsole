using HabitTrackerConsole.Models;
using HabitTrackerConsole.Services;
using System.Collections.Generic;

namespace HabitTrackerConsole.Util;

public class DatabaseSeeder
{
    private static Random _random = new Random();
    private static string[] habitNames = { "Exercise", "Reading", "Meditation", "Journaling", "Walking", "Programming" };

     private static DateTime GetRandomDate()
    {
        int days = _random.Next(365);
        return DateTime.Today.AddDays(-days);
    }

    private static int GetRandomQuantity()
    {
        return _random.Next(1, 10);
    }

    public static void SeedHabits(HabitService habitService)
    {
        foreach (string habitName in habitNames)
        {
            habitService.InsertHabitIntoHabitsTable(habitName);
        }
    }

    public static void SeedLogEntries(LogEntryService logEntryService, HabitService habitService, int numOfLogs)
    {
        List <HabitViewModel> habits = habitService.GetAllHabitsOverviews();
        if (!habits.Any()) return;  // Make sure there are habits to log against

        foreach (HabitViewModel habit in habits)
        {
            for (int i = 0; i < numOfLogs; i++)
            {
                string date = GetRandomDate().ToString("yyyy-MM-dd");
                int quantity = GetRandomQuantity();
                logEntryService.InsertLogEntryIntoHabitLog(date, habit.HabitId, quantity);
            }
        }
    }
}
