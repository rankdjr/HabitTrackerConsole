﻿using HabitTrackerConsole.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTrackerConsole.Util;

public class DatabaseSeeder
{
    private static Random _random = new Random();

    // Generates random habit names
    private static string GetRandomHabitName()
    {
        string[] possibleNames = { "Exercise", "Reading", "Meditation", "Journaling", "Walking", "Programming" };
        return possibleNames[_random.Next(possibleNames.Length)];
    }

    // Generates random dates within the last year
    private static DateTime GetRandomDate()
    {
        int days = _random.Next(365);
        return DateTime.Today.AddDays(-days);
    }

    // Generates random quantity
    private static int GetRandomQuantity()
    {
        return _random.Next(1, 10);
    }

    // Seed habits
    public static void SeedHabits(HabitService habitService, int count)
    {
        for (int i = 0; i < count; i++)
        {
            string habitName = GetRandomHabitName();
            habitService.InsertHabitIntoHabitsTable(habitName);
        }
    }

    // Seed log entries
    public static void SeedLogEntries(LogEntryService logEntryService, HabitService habitService, int count)
    {
        var habits = habitService.GetAllHabitsOverviews();
        if (!habits.Any()) return;  // Make sure there are habits to log against

        foreach (var habit in habits)
        {
            for (int i = 0; i < count; i++)
            {
                string date = GetRandomDate().ToString("yyyy-MM-dd");
                int quantity = GetRandomQuantity();
                logEntryService.InsertLogEntryIntoHabitLog(date, habit.HabitId, quantity);
            }
        }
    }
}