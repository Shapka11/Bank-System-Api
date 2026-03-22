using Bsa.Cli.Application.Contracts.User;
using Bsa.Cli.Application.Contracts.User.Operations;
using Bsa.Cli.Presentation.Cli.Commands.User;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace Bsa.Cli.Presentation.Cli.CommandHandlers.User;

public sealed class LoginUserCommandHandler : AsyncCommand<LoginUserCommand>
{
    private readonly IUserService _userService;

    public LoginUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        LoginUserCommand settings,
        CancellationToken cancellationToken)
    {
        string? accountNumber = settings.AccountNumber;
        string? password = settings.Password;

        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            accountNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Input [yellow]account number[/]:")
                    .PromptStyle("green"));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            password = AnsiConsole.Prompt(
                new TextPrompt<string>("Input [yellow]password[/]:")
                    .PromptStyle("green")
                    .Secret());
        }

        LoginUser.Result result = await _userService.LoginAsync(
            new LoginUser.Request(accountNumber, password),
            cancellationToken);

        if (result is LoginUser.Result.Success)
        {
            AnsiConsole.MarkupLine("[green]You have successfully logged in![/]");
            return 0;
        }

        if (result is LoginUser.Result.Failure failure)
        {
            AnsiConsole.MarkupLine($"[red]Error occurred: '{Markup.Escape(failure.ErrorMessage)}'[/]");
            return 1;
        }

        throw new UnreachableException();
    }
}