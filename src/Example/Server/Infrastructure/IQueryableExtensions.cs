using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Server.Infrastructure
{
    public static class IQueryableExtensions
    {
        public static async Task<PaginatedList<T>> PaginatedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}