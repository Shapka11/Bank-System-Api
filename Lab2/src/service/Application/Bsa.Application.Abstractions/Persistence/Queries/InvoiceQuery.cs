using Bsa.Domain.Invoices;
using Bsa.Domain.Invoices.States;
using Bsa.Domain.ValueObjects;
using SourceKit.Generators.Builder.Annotations;

namespace Bsa.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public sealed partial record InvoiceQuery(
    InvoiceId[] Ids,
    AccountNumber[] SenderAccountNumbers,
    AccountNumber[] ReceiverAccountNumbers,
    InvoiceStatus[] Statuses,
    InvoiceId? InvoiceIdCursor,
    [RequiredValue] int PageSize);