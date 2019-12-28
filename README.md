# Statik

A **dead simple** static site generator, with **no features**, for .NET.

[![Statik](https://img.shields.io/nuget/v/Statik.svg?style=flat-square&label=Statik)](http://www.nuget.org/packages/Statik/)
[![Statik.Mvc](https://img.shields.io/nuget/v/Statik.Mvc.svg?style=flat-square&label=Statik.Mvc)](http://www.nuget.org/packages/Statik.Mvc/)
[![Statik.Files](https://img.shields.io/nuget/v/Statik.Mvc.svg?style=flat-square&label=Statik.Files)](http://www.nuget.org/packages/Statik.Files/)

## No features?

This is a simple tool/library. There are no opinions or abstractions, aside from the abstraction needed to host and export content. There is nothing preventing you, the developer, from doing what you want with your project. Parse and render markdown files in a directory for a blog? Build a user manual? What ever you want, you can do.

## Why?

I've spent many hours fighting static site generators (Hugo, Gatsby, Jekyll, etc). The time taken to learn the plumbing of a framework to do very simple tasks often doesn't compare to the time it would take me to do the same task myself if there were no opinionated abstractions bogging me down. In the end, I always wind up biting the bullet, telling myself "Hey, It will generate the static website for me, I must plow through".

## What does Statik look like?

```c#
var webBuilder = Statik.GetWebBuilder();
            
// This is where your meat-and-potatoes go.
webBuilder.Register("/hello", async context =>
{
    await context.Response.WriteAsync($"The time is {DateTime.Now.ToLongTimeString()}");
});

// All of your endpoints get registered here.
// You could scan a directory for markdown files,
// or register a directory of stylesheets and images.
webBuilder.RegisterDirectory("./assets");

using (var host = webBuilder.BuildWebHost())
{
    host.Listen();
    Console.WriteLine("Listening on port 8000...");
    Console.WriteLine("Try visiting http://localhost:8000/hello");
    Console.WriteLine("Press enter to exit...");
    Console.ReadLine();
    
    // NOTE: You can export this host to a directory.
    // Useful for GitHub pages, etc.
    // await Statik.ExportHost(host, "./somewhere");
}
```

You can also use MVC controllers to handle endpoints.

```c#
public class ExampleController : Controller
{
    public ActionResult Index(string markdownFile)
    {
        return Content($"The time is {DateTime.Now.ToLongTimeString()}");
    }
}

webBuilder.RegisterMvc("/blog/hello-world", new
{
    controller = "Example",
    action = "Index",
    markdownFile = "./somewhere/markdown.md"
});
```
