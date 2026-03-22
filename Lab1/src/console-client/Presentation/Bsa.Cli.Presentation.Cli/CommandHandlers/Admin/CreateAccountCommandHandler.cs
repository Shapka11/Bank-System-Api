using Bsa.Cli.Application.Contracts.Admin;
using Bsa.Cli.Application.Contracts.Admin.Operations;
using Bsa.Cli.Presentation.Cli.Commands.Admin;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace Bsa.Cli.Presentation.Cli.CommandHandlers.Admin;

public sealed class CreateAccountCommandHandler : AsyncCommand<CreateAccountCommand>
{
    private readonly IAdminService _adminService;

    public CreateAccountCommandHandler(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        CreateAccountCommand settings,
        CancellationToken cancellationToken)
    {
        string? accountNumber = settings.AccountNumber;
        string? password = settings.Password;

        if (string.IsNullOrEmpty(accountNumber))
        {
            accountNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Input [yellow]account number[/]:")
                    .PromptStyle("green"));
        }

        if (string.IsNullOrEmpty(password))
        {
            password = AnsiConsole.Prompt(
                new TextPrompt<string>("Input [yellow]password[/]:")
                    .PromptStyle("green"));
        }

        CreateAccount.Result result = await _adminService.CreateAccountAsync(
            new CreateAccount.Request(accountNumber, password),
            cancellationToken);

        if (result is CreateAccount.Result.Success)
        {
            AnsiConsole.MarkupLine("[green]Account created![/]");
            return 0;
        }

        if (result is CreateAccount.Result.Failure failure)
        {
            AnsiConsole.MarkupLine($"[red]Error occurred: '{Markup.Escape(failure.ErrorMessage)}'[/]");
            return 1;
        }

        throw new UnreachableException();
    }
}