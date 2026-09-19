namespace SonkwoNoMo.Server.Config;

public record RegionEntry
{
    public required string Name { get; init; }
    public required string Location { get; init; }
}

public record RegionListOptions
{
    public required string AccInfo { get; init; }
    public required List<RegionEntry> Catalog { get; init; }
}