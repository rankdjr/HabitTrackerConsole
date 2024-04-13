using System.Collections.Generic;
using System.Data.SQLite;

namespace HabitTrackerConsole.Util;

public class SqliteHelper
{
    public static SQLiteCommand PrepareCommand(string commandText, SQLiteConnection connection, Dictionary<string, object>? parameters = null, SQLiteTransaction? transaction = null)
    {
        var command = new SQLiteCommand(commandText, connection, transaction);
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
        }
        return command;
    }

    public static int ExecuteCommand(string commandText, SQLiteConnection connection, Dictionary<string, object>? parameters = null, SQLiteTransaction? transaction = null)
    {
        var command = PrepareCommand(commandText, connection, parameters, transaction);
        return command.ExecuteNonQuery();
    }
}