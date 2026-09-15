using HyperCache.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HyperCache.Api.Extensions;

public static class QueryableExtensions
{
    // C# 14 extension members: the receiver is declared once for the whole block.
    extension<T>(IQueryable<T> source)
    {
        /// <summary>
        /// Runs a COUNT plus a Skip/Take page query and wraps the results with paging metadata.
        /// </summary>
        public async Task<PagedResponse<T>> ToPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await source.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await source
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<T>(items, page, totalPages);
        }
    }
}
