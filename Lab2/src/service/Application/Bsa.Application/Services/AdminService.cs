using Bsa.Application.Abstractions.Persistence;
using Bsa.Application.Contracts.Users;
using Bsa.Application.Contracts.Users.Admin;
using Bsa.Application.Mapping;
using Bsa.Application.Options;
using Bsa.Application.Specifications;
using Bsa.Domain.Sessions;
using Bsa.Domain.ValueObjects;
using Microsoft.Extensions.Options;

namespace Bsa.Application.Services;

public sealed class AdminService : IAdminService
{
    private readonly IPersistenceContext _context;
    private readonly SessionSpecifications _sessionSpecifications;
    private readonly IOptionsMonitor<SecurityOptions> _options;

    public AdminService(
        IPersistenceContext context,
        IOptionsMonitor<SecurityOptions> options,
        SessionSpecifications sessionSpecifications)
    {
        _context = context;
        _options = options;
        _sessionSpecifications = sessionSpecifications;
    }

    public async Task<LoginAdmin.Response> LoginAsync(
        LoginAdmin.Request request,
        CancellationToken cancellationToken)
    {
        var password = new Password(request.Password);
        var systemPassword = new Password(_options.CurrentValue.SystemPassword);

        if (password != systemPassword)
            return new LoginAdmin.Response.InvalidPassword();

        var session = new AdminSession(Guid.NewGuid(), DateTimeOffset.UtcNow);
        await _context.SessionRepository.AddAsync([session], cancellationToken);

        return new LoginAdmin.Response.Success(session.MapToDto());
    }

    public async Task<LogoutAdmin.Response> LogoutAsync(
        LogoutAdmin.Request request,
        CancellationToken cancellationToken)
    {
        SessionBase? adminSession = await _sessionSpecifications.FindSessionByIdAsync(request.Id, cancellationToken);

        if (adminSession is not AdminSession)
            return new LogoutAdmin.Response.Unauthorized(request.Id, "Session not admin");

        await _context.SessionRepository.RemoveAsync([adminSession], cancellationToken);

        return new LogoutAdmin.Response.Success();
    }
}