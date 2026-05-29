using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Invoices;
using BankSystemApi.Domain.Invoices.States;
using SourceKit.Generators.Builder.Annotations;

namespace BankSystemApi.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public sealed partial record InvoiceQuery(
    InvoiceId[] Ids,
    AccountId[] SenderAccountIds,
    AccountId[] ReceiverAccountIds,
    InvoiceStatus[] Statuses,
    InvoiceId? InvoiceIdCursor,
    [RequiredValue] int PageSize);