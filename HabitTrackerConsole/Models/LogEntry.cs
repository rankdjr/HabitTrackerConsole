namespace HabitTrackerConsole.Models
{
    internal class LogEntry
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int HabitId { get; set; }
        public int Quantity { get; set; }
    }
}
