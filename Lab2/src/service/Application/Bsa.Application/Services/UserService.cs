using Bsa.Application.Abstractions.Persistence;
using Bsa.Application.Contracts.Users;
using Bsa.Application.Contracts.Users.User;
using Bsa.Application.Mapping;
using Bsa.Domain.Accounts;
using Bsa.Domain.Sessions;
using Bsa.Domain.ValueObjects;

namespace Bsa.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IPersistenceContext _context;

    public UserService(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<LoginUser.Response> LoginAsync(LoginUser.Request request, CancellationToken cancellationToken)
    {
        var accountNumber = new AccountNumber(request.AccountNumber);
        var password = new Password(request.Password);

        Account? account = await _context.AccountsRepository
            .FindAccountByNumberAsync(accountNumber, cancellationToken);

        if (account is null)
            return new LoginUser.Response.AccountNotFound(request.AccountNumber);

        if (account.VerifyPassword(password) is false)
            return new LoginUser.Response.InvalidPassword();

        var session = new UserSession(Guid.NewGuid(), account.Id, DateTimeOffset.UtcNow);
        await _context.SessionRepository.AddAsync([session], cancellationToken);

        return new LoginUser.Response.Success(session.MapToDto());
    }

    public async Task<LogoutUser.Response> LogoutAsync(
        LogoutUser.Request request,
        CancellationToken cancellationToken)
    {
        SessionBase? session = await _context.SessionRepository
            .FindSessionByIdAsync(request.Id, cancellationToken);

        if (session is not UserSession)
            return new LogoutUser.Response.Unauthorized(request.Id, "Session not user");

        await _context.SessionRepository.RemoveAsync([session], cancellationToken);

        return new LogoutUser.Response.Success();
    }
}