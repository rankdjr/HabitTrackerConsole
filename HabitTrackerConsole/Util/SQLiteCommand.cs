namespace HabitTrackerConsole.Util
{
    internal class SQLiteCommand : SqliteCommand
    {
        private SQLiteConnection connection;
        private SQLiteTransaction transaction;

        public SQLiteCommand(string commandText, SQLiteConnection connection, SQLiteTransaction transaction)
        {
            CommandText = commandText;
            this.connection = connection;
            this.transaction = transaction;
        }
    }
}