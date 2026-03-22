using Bsa.Cli.Presentation.Cli.CommandHandlers.Admin;
using Bsa.Cli.Presentation.Cli.CommandHandlers.User;
using Spectre.Console.Cli;

namespace Bsa.Cli.Presentation.Cli;

public static class CommandAppExtension
{
    public static CommandApp AddCommands(this CommandApp app)
    {
        app.Configure(config =>
        {
            config.AddBranch(
                "admin",
                admin =>
                {
                    admin.AddCommand<LoginAdminCommandHandler>("login");
                    admin.AddCommand<LogoutAdminCommandHandler>("logout");
                    admin.AddCommand<CreateAccountCommandHandler>("create-account");
                });

            config.AddBranch(
                "user",
                user =>
                {
                    user.AddCommand<LoginUserCommandHandler>("login");
                    user.AddCommand<LogoutUserCommandHandler>("logout");
                    user.AddCommand<DepositCommandHandler>("deposit");
                    user.AddCommand<WithdrawCommandHandler>("withdraw");
                    user.AddCommand<GetBalanceCommandHandler>("get-balance");
                    user.AddCommand<GetHistoryCommandHandler>("get-history");
                });
        });

        return app;
    }
}