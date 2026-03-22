using Bsa.Cli.Application;
using Bsa.Cli.Application.Providers;
using Bsa.Cli.Infrastructure.ClientService.Admins;
using Bsa.Cli.Infrastructure.ClientService.Users;
using Bsa.Cli.Presentation.Cli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var sessionManager = new SessionManager();

IServiceCollection collection = new ServiceCollection()
    .AddSingleton<IConfiguration>(configuration)
    .AddSingleton(sessionManager)
    .AddApplication()
    .AddAdminClient()
    .AddUserClient()
    .AddPresentationConsole();

var registrar = new ServiceCollectionRegistrar(collection);

var app = new CommandApp(registrar);
app.AddCommands();

AnsiConsole.Clear();
AnsiConsole.Write(new FigletText("Bank system panel").Color(Color.Green));

bool isRunning = true;

while (isRunning)
{
    string input = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[bold]Что вы хотите сделать?[/]")
            .PageSize(10)
            .MoreChoicesText("[grey](Используйте стрелки для навигации)[/]")
            .AddChoices(AvailableCommands.All));

    if (input == AvailableCommands.Exit)
    {
        isRunning = false;
        continue;
    }

    string[] argsArray = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    await app.RunAsync(argsArray);

    AnsiConsole.WriteLine();
}

AnsiConsole.MarkupLine("[blue]Goodbye![/]");

return 0;