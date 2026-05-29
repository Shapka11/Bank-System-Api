using Extensions;
using OpenTelemetry;
using System.Diagnostics;

namespace Processors;

public sealed class OpenTelemetryTraceSuppressor : BaseProcessor<Activity>
{
    public override void OnEnd(Activity data)
    {
        if (data.TryGetTag("rpc.service", out string? service)
            && service.Contains("opentelemetry", StringComparison.OrdinalIgnoreCase))
        {
            data.Suppress();
        }
    }
}
