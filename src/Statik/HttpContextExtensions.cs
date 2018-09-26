using Microsoft.AspNetCore.Http;
using Statik.Web;

namespace Statik
{
    public static class HttpContextExtensions
    {
        public static Page GetCurrentPage(this HttpContext httpContext)
        {
            if (!httpContext.Items.TryGetValue("_statikPageAccessor", out var pageAccessor)) return null;
            if (pageAccessor is IPageAccessor accessor) return accessor.Page;

            return null;
        }
    }
}