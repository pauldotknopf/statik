using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Statik.Web;

namespace Statik.Mvc
{
    public static class WebBuilderExtensions
    {
        public static void RegisterMvcServices(this IWebBuilder webBuilder)
        {
            webBuilder.RegisterServices(services =>
            {
                services.AddMvc();
            });
        }
        
        public static void RegisterMvc(this IWebBuilder webBuilder, string path, object routeData, object state = null)
        {
            webBuilder.Register(path, async context =>
                {
                    var actionSelector = context.RequestServices.GetRequiredService<IActionSelector>();
                    var actionInvokerFactory = context.RequestServices.GetRequiredService<IActionInvokerFactory>();
    
                    var routeContext = new RouteContext(context);
                    if (routeData != null)
                    {
                        foreach(var value in new RouteValueDictionary(routeData))
                        {
                            routeContext.RouteData.Values[value.Key] = value.Value;
                        }
                    }
                    
                    var candidates = actionSelector.SelectCandidates(routeContext);
                    if (candidates == null || candidates.Count == 0)
                    {
                        throw new Exception("No actions matched");
                    }
    
                    var actionDescriptor = actionSelector.SelectBestCandidate(routeContext, candidates);
                    if (actionDescriptor == null)
                    {
                        throw new Exception("No actions matched");
                    }
    
                    var actionContext = new ActionContext(context, routeContext.RouteData, actionDescriptor);
    
                    var invoker = actionInvokerFactory.CreateInvoker(actionContext);
                    if (invoker == null)
                    {
                        throw new InvalidOperationException("Couldn't create invoker");
                    }
    
                    await invoker.InvokeAsync();
                },
                state);
        }
    }
}