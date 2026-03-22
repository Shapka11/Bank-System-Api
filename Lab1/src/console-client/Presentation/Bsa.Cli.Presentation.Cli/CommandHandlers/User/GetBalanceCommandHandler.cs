using Bsa.Cli.Application.Contracts.User;
using Bsa.Cli.Application.Contracts.User.Operations;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace Bsa.Cli.Presentation.Cli.CommandHandlers.User;

public sealed class GetBalanceCommandHandler : AsyncCommand
{
    private readonly IUserService _userService;

    public GetBalanceCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        CancellationToken cancellationToken)
    {
        GetBalance.Result result = await _userService.GetBalanceAsync(cancellationToken);

        if (result is GetBalance.Result.Success success)
        {
            AnsiConsole.MarkupLine("[green]Success![/]");
            AnsiConsole.MarkupLine($"Your balance: [yellow]{success.Balance}[/]");
            return 0;
        }

        if (result is GetBalance.Result.Failure failure)
        {
            AnsiConsole.MarkupLine($"[red]Error occurred: '{Markup.Escape(failure.ErrorMessage)}'[/]");
            return 1;
        }

        throw new UnreachableException();
    }
}