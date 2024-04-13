namespace HabitTrackerConsole.Models;

internal class HabitViewModel : Habit
{
    public string? DateCreated { get; set; }
    public string? LastLogEntryDate { get; set; }
    public int TotalLogs { get; set; }
}
