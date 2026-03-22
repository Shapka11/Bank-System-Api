using Spectre.Console.Cli;
using System.ComponentModel;

namespace Bsa.Cli.Presentation.Cli.Commands.User;

public sealed class WithdrawCommand : CommandSettings
{
    [CommandArgument(1, "[amount]")]
    [Description("The money to withdraw")]
    public decimal? Money { get; init; }
}