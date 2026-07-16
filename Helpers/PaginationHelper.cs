using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace EventManagement.Helpers;

public static class PaginationHelper
{
    public static object WithPage(object? query, string pageKey, int page)
    {
        var routeValues = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        if (query is IDictionary<string, object?> dict)
        {
            foreach (var kv in dict)
            {
                if (string.Equals(kv.Key, pageKey, StringComparison.OrdinalIgnoreCase)) continue;
                routeValues[kv.Key] = kv.Value;
            }
        }
        routeValues[pageKey] = page;
        return routeValues;
    }
}