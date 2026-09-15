namespace HyperCache.Api.Data;

/// <summary>
/// Controls the one-time seeding that runs at startup. Bound from the "Seed" configuration section,
/// so the row count can be lowered for local runs and tests via Seed:Count / Seed__Count.
/// </summary>
public sealed class SeedOptions
{
    public const string SectionName = "Seed";

    /// <summary>
    /// Number of rows to insert when the CustomProperties table is empty. Defaults to 1,000,000.
    /// </summary>
    public int Count
    {
        get;
        // C# 14 `field` keyword: validate in the setter without declaring a backing field by hand.
        set => field = value > 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(Count), value, "Seed:Count must be greater than 0.");
    } = 1_000_000;
}
