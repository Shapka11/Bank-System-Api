using Bsa.Cli.Application.Contracts.Admin;
using Bsa.Cli.Application.Contracts.Admin.Operations;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace Bsa.Cli.Presentation.Cli.CommandHandlers.Admin;

public sealed class LogoutAdminCommandHandler : AsyncCommand
{
    private readonly IAdminService _adminService;

    public LogoutAdminCommandHandler(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        CancellationToken cancellationToken)
    {
        LogoutAdmin.Result result = await _adminService.LogoutAdminAsync(cancellationToken);

        if (result is LogoutAdmin.Result.Success)
        {
            AnsiConsole.MarkupLine("[green]You have successfully logged out![/]");
            return 0;
        }

        if (result is LogoutAdmin.Result.Failure failure)
        {
            AnsiConsole.MarkupLine($"[red]Error occurred: '{Markup.Escape(failure.ErrorMessage)}'[/]");
            return 1;
        }

        throw new UnreachableException();
    }
}