using System.Net;
using System.Net.Http.Json;

namespace HyperCache.Api.Tests;

public sealed class CustomPropertiesEndpointTests(HyperCacheFactory factory) : IClassFixture<HyperCacheFactory>
{
    private const string PagedUrl = "/api/customproperties/paged?page=1&pageSize=5";

    private sealed record PagedResponse(List<Item> Items, int CurrentPage, int TotalPages, bool HasPreviousPage, bool HasNextPage);
    private sealed record Item(Guid Id, string Name, string Value, string ParentTable, string CreatedBy, string ModifiedBy);

    [Fact]
    public async Task Paged_returns_page_with_etag()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync(PagedUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Headers.ETag);

        var body = await response.Content.ReadFromJsonAsync<PagedResponse>();
        Assert.NotNull(body);
        Assert.Equal(5, body.Items.Count);
        Assert.Equal(1, body.CurrentPage);
        Assert.Equal(HyperCacheFactory.SeedCount / 5, body.TotalPages);
        Assert.False(body.HasPreviousPage);
        Assert.True(body.HasNextPage);
    }

    [Fact]
    public async Task Paged_with_matching_etag_returns_304()
    {
        using var client = factory.CreateClient();

        var first = await client.GetAsync(PagedUrl);
        var etag = first.Headers.ETag;
        Assert.NotNull(etag);

        using var request = new HttpRequestMessage(HttpMethod.Get, PagedUrl);
        request.Headers.IfNoneMatch.Add(etag);
        var second = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotModified, second.StatusCode);
    }

    [Fact]
    public async Task Search_returns_matches_and_rejects_empty_keyword()
    {
        using var client = factory.CreateClient();

        var ok = await client.GetAsync("/api/customproperties/search?keyword=_Property_1");
        Assert.Equal(HttpStatusCode.OK, ok.StatusCode);
        var items = await ok.Content.ReadFromJsonAsync<List<Item>>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
        Assert.All(items, i => Assert.Contains("_Property_1", i.Name));

        var bad = await client.GetAsync("/api/customproperties/search?keyword=");
        Assert.Equal(HttpStatusCode.BadRequest, bad.StatusCode);
    }

    [Fact]
    public async Task Details_validates_id_and_returns_404_for_unknown()
    {
        using var client = factory.CreateClient();

        var bad = await client.GetAsync("/api/customproperties/not-a-guid");
        Assert.Equal(HttpStatusCode.BadRequest, bad.StatusCode);

        var missing = await client.GetAsync($"/api/customproperties/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);

        var page = await client.GetFromJsonAsync<PagedResponse>(PagedUrl);
        Assert.NotNull(page);
        var found = await client.GetAsync($"/api/customproperties/{page.Items[0].Id}");
        Assert.Equal(HttpStatusCode.OK, found.StatusCode);
    }

    [Fact]
    public async Task OpenApi_document_is_served_in_development()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        // Route templates keep the controller's casing in the document: /api/CustomProperties/paged
        Assert.Contains("/api/customproperties/paged", json, StringComparison.OrdinalIgnoreCase);
    }
}
