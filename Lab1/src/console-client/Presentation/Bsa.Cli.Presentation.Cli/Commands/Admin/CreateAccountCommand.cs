using Spectre.Console.Cli;
using System.ComponentModel;

namespace Bsa.Cli.Presentation.Cli.Commands.Admin;

public sealed class CreateAccountCommand : CommandSettings
{
    [CommandArgument(1, "[number]")]
    [Description("The account number to new account")]
    public string? AccountNumber { get; init; }

    [CommandArgument(2, "[password]")]
    [Description("The password to new account")]
    public string? Password { get; init; }
}