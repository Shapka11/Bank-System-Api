using Extensions;
using OpenTelemetry;
using System.Diagnostics;

namespace BankSystemApi.HealthChecks;

public sealed class PostgresTraceSuppressor : BaseProcessor<Activity>
{
    public override void OnEnd(Activity data)
    {
        if (data.TryGetTag("db.query.text", out string? query) is false)
            return;

        if (query.Contains("information_schema", StringComparison.OrdinalIgnoreCase)
            || query.Contains("VersionInfo", StringComparison.OrdinalIgnoreCase)
            || query.Contains("SELECT 1", StringComparison.OrdinalIgnoreCase))
        {
            data.Suppress();
        }
    }
}
