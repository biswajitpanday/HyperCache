namespace HyperCache.Api.Models;

/// <summary>
/// Page of results plus navigation metadata. The JSON shape is mirrored by
/// HyperCache.Web/Dtos/PagedResponseDto.cs; keep the two in sync.
/// </summary>
public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int CurrentPage, int TotalPages)
{
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
