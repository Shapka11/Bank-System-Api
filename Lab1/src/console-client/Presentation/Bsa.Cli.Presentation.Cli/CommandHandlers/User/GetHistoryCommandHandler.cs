using Bsa.Cli.Application.Contracts.User;
using Bsa.Cli.Application.Contracts.User.Models;
using Bsa.Cli.Application.Contracts.User.Operations;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace Bsa.Cli.Presentation.Cli.CommandHandlers.User;

public sealed class GetHistoryCommandHandler : AsyncCommand
{
    private readonly IUserService _userService;

    public GetHistoryCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        CancellationToken cancellationToken)
    {
        GetHistory.Result result = await _userService.GetHistoryAsync(cancellationToken);

        if (result is GetHistory.Result.Success success)
        {
            AnsiConsole.MarkupLine("[green]Success![/]");

            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn("[bold]ID[/]");
            table.AddColumn("[bold]Account[/]");
            table.AddColumn("[bold]Balance[/]");
            table.AddColumn("[bold]Type[/]");
            table.AddColumn("[bold]Time[/]");

            foreach (AccountOperationDto op in success.History)
            {
                string typeMarkup = op.Type switch
                {
                    "Create" => "[green]Create[/]",
                    "Deposit" => "[blue]Deposit[/]",
                    "Withdraw" => "[red]Withdraw[/]",
                    _ => op.Type,
                };

                table.AddRow(
                    op.Id.ToString(),
                    op.AccountNumber,
                    $"{op.Balance}",
                    typeMarkup,
                    op.Time.ToString("yyyy-MM-dd HH:mm"));
            }

            AnsiConsole.Write(table);
            return 0;
        }

        if (result is GetHistory.Result.Failure failure)
        {
            AnsiConsole.MarkupLine($"[red]Error occurred: '{Markup.Escape(failure.ErrorMessage)}'[/]");
            return 1;
        }

        throw new UnreachableException();
    }
}