using System.ComponentModel.DataAnnotations;

namespace Bsa.Cli.Presentation.Cli.Options;

public sealed class CliOptions
{
    [Range(minimum: 1, maximum: int.MaxValue)]
    public int DefaultConfigurationCount { get; set; }
}