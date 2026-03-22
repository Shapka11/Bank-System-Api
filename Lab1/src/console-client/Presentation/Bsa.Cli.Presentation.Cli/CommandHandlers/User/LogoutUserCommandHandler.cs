using Bsa.Cli.Application.Contracts.User;
using Bsa.Cli.Application.Contracts.User.Operations;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace Bsa.Cli.Presentation.Cli.CommandHandlers.User;

public sealed class LogoutUserCommandHandler : AsyncCommand
{
    private readonly IUserService _userService;

    public LogoutUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        CancellationToken cancellationToken)
    {
        LogoutUser.Result result = await _userService.LogoutAsync(cancellationToken);

        if (result is LogoutUser.Result.Success)
        {
            AnsiConsole.MarkupLine("[green]You have successfully logged out![/]");
            return 0;
        }

        if (result is LogoutUser.Result.Failure failure)
        {
            AnsiConsole.MarkupLine($"[red]Error occurred: '{Markup.Escape(failure.ErrorMessage)}'[/]");
            return 1;
        }

        throw new UnreachableException();
    }
}