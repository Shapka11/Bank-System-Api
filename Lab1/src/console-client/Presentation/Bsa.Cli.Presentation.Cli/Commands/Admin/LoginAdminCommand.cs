using Spectre.Console.Cli;
using System.ComponentModel;

namespace Bsa.Cli.Presentation.Cli.Commands.Admin;

public sealed class LoginAdminCommand : CommandSettings
{
    [CommandArgument(0, "[password]")]
    [Description("The password of the admin to login")]
    public string? Password { get; init; }
}