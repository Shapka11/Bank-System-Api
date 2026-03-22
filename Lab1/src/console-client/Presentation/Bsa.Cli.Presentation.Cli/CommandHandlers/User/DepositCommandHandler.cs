using Bsa.Cli.Application.Contracts.User;
using Bsa.Cli.Application.Contracts.User.Operations;
using Bsa.Cli.Presentation.Cli.Commands.User;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace Bsa.Cli.Presentation.Cli.CommandHandlers.User;

public sealed class DepositCommandHandler : AsyncCommand<DepositCommand>
{
    private readonly IUserService _userService;

    public DepositCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        DepositCommand settings,
        CancellationToken cancellationToken)
    {
        decimal? amount = settings.Money;
        if (amount is null)
        {
            amount = AnsiConsole.Prompt(
                new TextPrompt<decimal>("Input [yellow]amount[/]:")
                    .PromptStyle("green"));
        }

        Deposit.Result result = await _userService.DepositAsync(
            new Deposit.Request(amount.Value),
            cancellationToken);

        if (result is Deposit.Result.Success)
        {
            AnsiConsole.MarkupLine("[green]You have successfully deposited money![/]");
            return 0;
        }

        if (result is Deposit.Result.Failure failure)
        {
            AnsiConsole.MarkupLine($"[red]Error occurred: '{Markup.Escape(failure.ErrorMessage)}'[/]");
            return 1;
        }

        throw new UnreachableException();
    }
}