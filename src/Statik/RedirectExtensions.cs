using System;
using Microsoft.AspNetCore.Http;
using Statik.Web;

namespace Statik
{
    public static class RedirectExtensions
    {
        public static void Redirect(this IWebBuilder webBuilder, string from, string to, object state = null, bool extractExactPath = false)
        {
            webBuilder.Register(from, async (context) =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync($@"<!DOCTYPE html>
<html lang=""en-US"">
<meta charset=""utf-8"">
<title>Redirecting&hellip;</title>
<link rel=""canonical"" href=""{context.Request.PathBase}{to}"">
<script>location=""{context.Request.PathBase}{to}""</script>
<meta http-equiv=""refresh"" content=""0; url={context.Request.PathBase}{to}"">
<meta name=""robots"" content=""noindex"">
<h1>Redirecting&hellip;</h1>
<a href=""{context.Request.PathBase}{to}"">Click here if you are not redirected.</a>
</html>");
                    
                },
                state,
                extractExactPath);
        }
    }
}