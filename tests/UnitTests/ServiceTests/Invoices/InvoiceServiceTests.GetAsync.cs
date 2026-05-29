using AutoBogus;
using BankSystemApi.Application.Abstractions.Persistence.Queries;
using BankSystemApi.Application.Contracts.Invoices.Models;
using BankSystemApi.Application.Contracts.Invoices.Operations;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Invoices.States;
using BankSystemApi.Domain.Users;
using FluentAssertions;
using Moq;
using UnitTests.MockExtensions;
using UnitTests.Models;

namespace UnitTests.ServiceTests.Invoices;

public sealed partial class InvoiceServiceTests
{
    [Theory]
    [InlineData(1, 1, true)]
    [InlineData(2, 1, false)]
    [InlineData(1, 2, true)]
    public async Task GetAsync_ShouldReturnDtos(
        int pageSize,
        int invoiceCount,
        bool pageTokenReturned)
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account accounts = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();
        var invoices = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .Generate(invoiceCount)
            .Select(model => model.MapToDomain())
            .ToList();

        GetInvoices.Request request = new(user.AuthorizationId, [], [], InvoiceTypeDto.Incoming, pageSize, null);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountByUserId(user.Id, accounts);

        _persistenceContext.InvoicesRepository
            .Setup(repo => repo.QueryAsync(
                It.Is<InvoiceQuery>(q => q.PageSize == pageSize),
                It.IsAny<CancellationToken>()))
            .Returns(invoices.ToAsyncEnumerable());

        // Act
        GetInvoices.Response response = await _invoiceService.GetAsync(request, default);

        // Assert
        response
            .Should()
            .BeOfType<GetInvoices.Response.Success>()
            .Which.Invoices.Should()
            .HaveCount(invoiceCount);

        if (pageTokenReturned)
        {
            response
                .Should()
                .BeOfType<GetInvoices.Response.Success>()
                .Which.PageToken.Should()
                .NotBeNull();
        }
        else
        {
            response
                .Should()
                .BeOfType<GetInvoices.Response.Success>()
                .Which.PageToken.Should()
                .BeNull();
        }
    }

    [Fact]
    public async Task GetAsync_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();

        GetInvoices.Request request = new(
            user.AuthorizationId,
            [],
            [],
            InvoiceTypeDto.Incoming,
            _faker.Random.Int(0),
            null);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId);

        // Act
        GetInvoices.Response response = await _invoiceService.GetAsync(request, default);

        // Assert
        response.Should().BeOfType<GetInvoices.Response.Unauthorized>();
    }
}