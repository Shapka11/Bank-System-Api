namespace Bsa.Cli.Presentation.Cli;

public static class AvailableCommands
{
    public static string Exit => "exit";

    private static readonly string[] AdminCommands =
    [
        "admin login",
        "admin logout",
        "admin create-account"
    ];

    private static readonly string[] UserCommands =
    [
        "user login",
        "user logout",
        "user deposit",
        "user withdraw",
        "user get-balance",
        "user get-history"
    ];

    public static IReadOnlyList<string> All =>
        AdminCommands
        .Concat(UserCommands)
        .Append(Exit)
        .ToArray();
}