using Spectre.Console.Cli;
using System.ComponentModel;

namespace Bsa.Cli.Presentation.Cli.Commands.User;

public sealed class LoginUserCommand : CommandSettings
{
    [CommandArgument(0, "[account number]")]
    [Description("The acount number of the user to login")]
    public string? AccountNumber { get; init; }

    [CommandArgument(1, "[password]")]
    [Description("The password of the user to login")]
    public string? Password { get; init; }
}