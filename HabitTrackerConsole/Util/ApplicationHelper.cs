using Spectre.Console;
using System.Text.RegularExpressions;

namespace HabitTrackerConsole.Util;

public class ApplicationHelper
{
    public static void AnsiWriteLine(Markup message)
    {
        AnsiConsole.Write(message);
        Console.WriteLine();
    }

    public static T FromFriendlyString<T>(string friendlyString) where T : struct, Enum
    {
        string enumString = friendlyString.Replace(" ", "");
        return Enum.Parse<T>(enumString, true);
    }

    public static string SplitCamelCase(string input)
    {
        return Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
    }

    public static void ShowReturnToMainMenuPrompt()
    {
        AnsiConsole.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
