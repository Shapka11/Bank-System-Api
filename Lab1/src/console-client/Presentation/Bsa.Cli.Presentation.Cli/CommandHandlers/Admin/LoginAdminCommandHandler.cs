using Bsa.Cli.Application.Contracts.Admin;
using Bsa.Cli.Application.Contracts.Admin.Operations;
using Bsa.Cli.Presentation.Cli.Commands.Admin;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace Bsa.Cli.Presentation.Cli.CommandHandlers.Admin;

public sealed class LoginAdminCommandHandler : AsyncCommand<LoginAdminCommand>
{
    private readonly IAdminService _adminService;

    public LoginAdminCommandHandler(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        LoginAdminCommand settings,
        CancellationToken cancellationToken)
    {
        string? password = settings.Password;

        if (string.IsNullOrWhiteSpace(password))
        {
            password = AnsiConsole.Prompt(
                new TextPrompt<string>("Введите [yellow]пароль[/]:")
                    .PromptStyle("green")
                    .Secret());
        }

        LoginAdmin.Result result = await _adminService.LoginAdminAsync(
            new LoginAdmin.Request(password),
            cancellationToken);

        if (result is LoginAdmin.Result.Success success)
        {
            AnsiConsole.MarkupLine("[green]You have successfully logged in![/]");
            return 0;
        }

        if (result is LoginAdmin.Result.Failure failure)
        {
            AnsiConsole.MarkupLine($"[red]Error occurred: '{Markup.Escape(failure.ErrorMessage)}'[/]");
            return 1;
        }

        throw new UnreachableException();
    }
}