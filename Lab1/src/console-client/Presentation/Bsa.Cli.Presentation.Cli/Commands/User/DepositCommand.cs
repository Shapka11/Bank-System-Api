using Spectre.Console.Cli;
using System.ComponentModel;

namespace Bsa.Cli.Presentation.Cli.Commands.User;

public sealed class DepositCommand : CommandSettings
{
    [CommandArgument(1, "[amount]")]
    [Description("The money to deposit")]
    public decimal? Money { get; init; }
}